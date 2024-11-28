using DotNetEnv;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;

using static Nuke.Common.Tools.DotNet.DotNetTasks;

class Build : NukeBuild
{
    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

    string[] Services => new[] { "Cart", "Catalog" };

    public static int Main() => Execute<Build>(x => x.CompileAllServices);

    void LoadEnvironmentVariables()
    {
        string envFilePath = RootDirectory / "EStoreSolution" / ".env";

        if (!File.Exists(envFilePath))
        {
            throw new Exception($"Environment file '{envFilePath}' not found.");
        }

        Env.Load(envFilePath);
        Log.Information($"Loaded environment variables from {envFilePath}");
    }

    void ValidateEnvironmentVariables(params string[] variables)
    {
        foreach (var variable in variables)
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(variable)))
            {
                Log.Warning($"Environment variable '{variable}' is not set.");
            }
        }
    }

    AbsolutePath GetServiceDirectory(string service) => RootDirectory / "EStoreSolution" / "src" / "Services" / service;

    AbsolutePath GetServiceArtifactDirectory(string service) => ArtifactsDirectory / service;

    static Build()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .CreateLogger();
    }

    Target CleanAllServices => _ => _
        .Executes(() =>
        {
            Log.Information("Cleaning all artifacts...");
            ArtifactsDirectory.CreateOrCleanDirectory();

            foreach (var service in Services)
            {
                Log.Information($"Cleaning artifacts for service: {service}...");
                GetServiceArtifactDirectory(service).CreateOrCleanDirectory();
            }
            Log.Information("All services cleaned.");
        });

    Target RestoreAllServices => _ => _
        .DependsOn(CleanAllServices)
        .Executes(() =>
        {
            AbsolutePath serviceDirectories = RootDirectory / "EStoreSolution" / "src" / "Services";

            ProcessProjects(serviceDirectories, project =>
                DotNetRestore(s => s.SetProjectFile(project)));
        });

    Target CompileAllServices => _ => _
        .DependsOn(RestoreAllServices)
        .Executes(() =>
        {
            AbsolutePath servicesRoot = RootDirectory / "EStoreSolution" / "src" / "Services";

            ProcessProjects(servicesRoot, project =>
                DotNetBuild(s => s
                    .SetProjectFile(project)
                    .SetConfiguration(Configuration)
                    .EnableNoRestore()
                    .SetOutputDirectory(ArtifactsDirectory / project.Parent.Name)
                    .SetProcessArgumentConfigurator(args => args.Add("/warnaserror"))));
        });

    Target RunAllTests => _ => _
        .DependsOn(CompileAllServices)
        .Executes(() =>
        {
            AbsolutePath testsRoot = RootDirectory / "EStoreSolution" / "tests";

            if (!testsRoot.DirectoryExists())
            {
                throw new Exception($"Tests directory not found: {testsRoot}");
            }

            IReadOnlyCollection<AbsolutePath> testProjects = testsRoot.GlobFiles("**/*.UnitTests.csproj");
            if (testProjects.Count == 0)
            {
                Log.Warning("No test projects found in the tests directory.");
                return;
            }

            foreach (AbsolutePath testProject in testProjects)
            {
                Log.Information($"Running tests for project: {testProject}...");
                DotNetTest(s => s
                    .SetProjectFile(testProject)
                    .SetConfiguration(Configuration.Debug));
            }

            Log.Information("All tests completed successfully.");
        });

    Target CheckFormatting => _ => _
        .Executes(() =>
        {
            try
            {
                Log.Information("Checking code formatting...");
                ProcessTasks.StartProcess("dotnet", "format --verify-no-changes").AssertZeroExitCode();
                Log.Information("Code formatting check passed.");
            }
            catch (Exception)
            {
                Log.Error("Code formatting check failed. Run 'ApplyFormatting' to fix formatting issues.");
                throw;
            }
        });

    Target ApplyFormatting => _ => _
        .Executes(() =>
        {
            Log.Information("Applying code formatting...");
            ProcessTasks.StartProcess("dotnet", "format").AssertZeroExitCode();
            Log.Information("Code formatting applied.");
        });

    Target SonarQubeAnalysis => _ => _
        .DependsOn(CompileAllServices)
        .Executes(() =>
        {
            LoadEnvironmentVariables();
            ValidateEnvironmentVariables("SONAR_HOST_URL", "SONAR_TOKEN", "SONAR_PROJECT_KEY");

            var sonarHostUrl = Environment.GetEnvironmentVariable("SONAR_HOST_URL");
            var sonarToken = Environment.GetEnvironmentVariable("SONAR_TOKEN");
            var projectKey = Environment.GetEnvironmentVariable("SONAR_PROJECT_KEY");

            ValidateEnvironmentVariables("SONAR_HOST_URL", "SONAR_TOKEN", "SONAR_PROJECT_KEY");

            Log.Information("Starting SonarQube analysis...");

            ProcessTasks.StartProcess("dotnet-sonarscanner", $"begin /k:\"{projectKey}\" /d:sonar.host.url=\"{sonarHostUrl}\" " +
                $"/d:sonar.token=\"{sonarToken}\"").AssertZeroExitCode();
            ProcessTasks.StartProcess("dotnet", "build").AssertZeroExitCode();
            ProcessTasks.StartProcess("dotnet-sonarscanner", $"end /d:sonar.token=\"{sonarToken}\"").AssertZeroExitCode();

            Log.Information($"SonarQube analysis completed. Check results at: {sonarHostUrl}");
        });

    Target DockerBuild => _ => _
        .DependsOn(CompileAllServices)
        .Executes(() =>
        {
            foreach (var service in Services)
            {
                AbsolutePath servicePath = GetServiceDirectory(service);
                var dockerFile = servicePath / "Dockerfile";

                if (!dockerFile.FileExists())
                {
                    Log.Warning($"Dockerfile not found for service: {service}");
                    continue;
                }

                var imageName = $"{service.ToLowerInvariant()}:{Configuration.ToString().ToLower()}";
                Log.Information($"Building Docker image for service {service} with tag {imageName}...");

                ProcessTasks.StartProcess("docker", $"build -t {imageName} {servicePath}").AssertZeroExitCode();
            }

            Log.Information("All Docker images built successfully.");
        });

    Target DockerComposeUp => _ => _
        .DependsOn(DockerBuild)
        .Executes(() =>
        {
            AbsolutePath dockerComposeFile = RootDirectory / "EStoreSolution" / "docker-compose.yml";
            AbsolutePath envFile = RootDirectory / "EStoreSolution" / ".env";

            if (!dockerComposeFile.FileExists())
                throw new Exception("docker-compose.yml file not found in the project root.");

            if (!envFile.FileExists())
                throw new Exception(".env file not found in the project root.");

            Log.Information("Starting Docker Compose...");
            ProcessTasks.StartProcess("docker-compose", $"-f {dockerComposeFile} --env-file {envFile} up -d").AssertZeroExitCode();

            Log.Information("Docker Compose up completed.");
        });

    Target DockerComposeDown => _ => _
        .Executes(() =>
        {
            AbsolutePath dockerComposeFile = RootDirectory / "EStoreSolution" / "docker-compose.yml";

            if (!dockerComposeFile.FileExists())
                throw new Exception("docker-compose.yml file not found in the project root.");

            Log.Information("Stopping Docker Compose...");
            ProcessTasks.StartProcess("docker-compose", $"-f {dockerComposeFile} down").AssertZeroExitCode();

            Log.Information("Docker Compose down completed.");
        });

    Target BuildAllServices => _ => _
        .DependsOn(CleanAllServices, RestoreAllServices, CompileAllServices, RunAllTests)
        .Executes(() =>
        {
            Log.Information("Full build process completed successfully!");
        });

    void ProcessProjects(AbsolutePath directory, Action<AbsolutePath> action)
    {
        if (!directory.DirectoryExists())
        {
            Log.Warning($"Directory not found: {directory}");
            return;
        }

        foreach (var csprojFile in directory.GlobFiles("**/*.csproj"))
        {
            Log.Information($"Processing project: {csprojFile}...");
            action(csprojFile);
        }
    }
}