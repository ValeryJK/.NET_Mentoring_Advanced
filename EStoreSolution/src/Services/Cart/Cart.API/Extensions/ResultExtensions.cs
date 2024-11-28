using Cart.Application.Validation;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Cart.API.Extensions
{
    /// <summary>
    /// Provides extension methods for converting <see cref="Result{T}"/> objects to HTTP responses.
    /// </summary>
    public static class ResultExtensions
    {
        /// <summary>
        /// Converts a <see cref="Result{T}"/> to an appropriate <see cref="ActionResult"/> based on its success or failure.
        /// </summary>
        /// <typeparam name="T">The type of the value held by the result.</typeparam>
        /// <param name="result">The result to convert.</param>
        /// <returns>An <see cref="ActionResult"/> representing the result.</returns>
        public static ActionResult ToHttpResponse<T>(this Result<T> result)
        {
            if (result.IsSuccess)
            {
                if (result.ValueOrDefault is Unit)
                {
                    return new NoContentResult();
                }

                return new OkObjectResult(result.Value);
            }

            return HandleError(result);
        }

        /// <summary>
        /// Converts a successful <see cref="Result{T}"/> to a 201 Created HTTP response, or handles errors if the result is unsuccessful.
        /// </summary>
        /// <typeparam name="T">The type of the value held by the result.</typeparam>
        /// <param name="result">The result to convert.</param>
        /// <returns>An <see cref="ActionResult"/> representing the result.</returns>
        public static ActionResult ToCreatedHttpResponse<T>(this Result<T> result)
        {
            if (result.IsSuccess)
            {
                return new ObjectResult(result.Value)
                {
                    StatusCode = StatusCodes.Status201Created
                };
            }

            return HandleError(result);
        }

        /// <summary>
        /// Handles errors in a <see cref="Result{T}"/> and returns an appropriate HTTP response.
        /// </summary>
        /// <typeparam name="T">The type of the value held by the result.</typeparam>
        /// <param name="result">The result containing errors.</param>
        /// <returns>An <see cref="ObjectResult"/> representing the error.</returns>
        private static ObjectResult HandleError<T>(Result<T> result)
        {
            var firstError = result.Errors.FirstOrDefault();

            if (firstError is ValidationError)
            {
                return HandleValidationError(result);
            }
            else if (firstError is NotFoundError)
            {
                return HandleNotFoundError(result);
            }

            return new BadRequestObjectResult(new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Unexpected Error",
                Detail = result.Errors.FirstOrDefault()?.Message ?? string.Empty
            });
        }

        /// <summary>
        /// Creates a Bad Request response with validation error details.
        /// </summary>
        /// <typeparam name="T">The type of the value held by the result.</typeparam>
        /// <param name="result">The result containing validation errors.</param>
        /// <returns>An <see cref="ObjectResult"/> with validation problem details.</returns>
        private static ObjectResult HandleValidationError<T>(Result<T> result)
        {
            var details = new ValidationProblemDetails(new Dictionary<string, string[]>
            { { "ValidationError", result.Errors.Select(c => c.Message).ToArray() } })
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Validation Error"
            };

            return new BadRequestObjectResult(details);
        }

        /// <summary>
        /// Creates a Not Found response when the specified resource is not found.
        /// </summary>
        /// <typeparam name="T">The type of the value held by the result.</typeparam>
        /// <param name="result">The result indicating the resource was not found.</param>
        /// <returns>An <see cref="ObjectResult"/> with not found problem details.</returns>
        private static ObjectResult HandleNotFoundError<T>(Result<T> result)
        {
            var details = new ProblemDetails()
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Title = "The specified resource was not found.",
                Detail = result.Errors.FirstOrDefault()?.Message ?? string.Empty
            };

            return new NotFoundObjectResult(details);
        }
    }
}