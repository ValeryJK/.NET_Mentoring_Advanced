using FluentResults;

namespace Catalog.Application.Validation
{
	public class ValidationError : Error
	{
		public ValidationError(string message) : base(message)
		{
		}
	}
}
