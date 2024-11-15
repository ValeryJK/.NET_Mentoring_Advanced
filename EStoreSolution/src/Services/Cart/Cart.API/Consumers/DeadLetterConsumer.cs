using Contracts.Messages;
using MassTransit;

namespace Cart.API.Consumers
{
	/// <summary>
	/// Consumer class for handling messages that are moved to the Dead Letter Queue (DLQ).
	/// This consumer processes failed messages of type <see cref="UpdateCartItemEvent"/>
	/// that could not be successfully delivered after retries.
	/// Additional logic for analyzing and handling these messages should be implemented as necessary.
	/// </summary>
	public class DeadLetterConsumer : IConsumer<UpdateCartItemEvent>
	{
		private readonly ILogger<DeadLetterConsumer> _logger;

		/// <summary>
		/// Initializes a new instance of the <see cref="DeadLetterConsumer"/> class.
		/// </summary>
		/// <param name="logger">The logger instance used for logging information and errors.</param>
		public DeadLetterConsumer(ILogger<DeadLetterConsumer> logger)
		{
			_logger = logger;
		}

		/// <summary>
		/// Consumes messages moved to the Dead Letter Queue (DLQ) for further processing or analysis.
		/// Logs the details of the failed message for further inspection.
		/// Implement additional logic here to analyze or handle such messages as needed.
		/// </summary>
		/// <param name="context">The context of the consumed message, containing details of the <see cref="UpdateCartItemEvent"/>.</param>
		public async Task Consume(ConsumeContext<UpdateCartItemEvent> context)
		{
			_logger.LogWarning("Message moved to DLQ: ItemId: {ItemId}, Name: {Name}", context.Message.Id, context.Message.Name);

			// Additional logic for processing the DLQ message can be added here
			await Task.CompletedTask;
		}
	}
}