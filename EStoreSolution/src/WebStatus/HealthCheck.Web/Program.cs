var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecksUI().AddInMemoryStorage();

var app = builder.Build();

app.UseRouting();

app.MapHealthChecksUI(options =>
{
    options.UIPath = "/health-ui";
    options.ApiPath = "/health-ui-api";
});

await app.RunAsync();
