namespace SagaOrchestrator.Common.Contracts;

// Commands issued *by* the orchestrator
public record ReserveStock(Guid OrderId);
public record CapturePayment(Guid OrderId, decimal Amount);
public record CreateOrder(Guid OrderId, decimal Amount);

// Replies sent *to* the orchestrator
public record Success(Guid OrderId);
public record Failure(Guid OrderId, string Reason);
