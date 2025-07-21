namespace SagaOrchestrator.Common.Models;

public class OrderSagaState
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public SagaStatus Status { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}