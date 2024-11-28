using FluentResults;

namespace Cart.Application.Validation
{
    public class NotFoundError : Error
    {
        public NotFoundError(string message)
            : base(message)
        {
        }
    }
}