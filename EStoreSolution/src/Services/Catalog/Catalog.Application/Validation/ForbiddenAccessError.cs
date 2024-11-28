using FluentResults;

namespace Catalog.Application.Validation
{
    public class ForbiddenAccessError : Error
    {
        public ForbiddenAccessError(string message)
            : base(message)
        {
        }
    }
}
