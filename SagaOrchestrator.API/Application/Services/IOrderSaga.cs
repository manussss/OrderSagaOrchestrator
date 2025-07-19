using SagaOrchestrator.API.Models;

namespace SagaOrchestrator.API.Application.Services;

public interface IOrderSaga
{
    Task<Result> ExecuteAsync(OrderRequest req);
}