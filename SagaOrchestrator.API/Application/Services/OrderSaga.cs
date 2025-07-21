using System.Net;
using SagaOrchestrator.API.Models;
using SagaOrchestrator.Common.Contracts;
using SagaOrchestrator.Common.Infra.Data;
using SagaOrchestrator.Common.Models;

namespace SagaOrchestrator.API.Application.Services;

public class OrderSaga(
    IHttpClientFactory factory,
    SagaDbContext context) : IOrderSaga
{
    public async Task<Result> ExecuteAsync(OrderRequest req)
    {
        var orderId = Guid.NewGuid();
        
        var state = new OrderSagaState { Id = orderId, Amount = req.Amount, Status = SagaStatus.Pending };
        context.OrderSagaState.Add(state);
        await context.SaveChangesAsync();

        if (!await Send("order", "/orders", new CreateOrder(orderId, req.Amount)))
            return Failure("Order service failed");

        if (!await Send("stock", "/stock", new ReserveStock(orderId)))
        {
            await Compensate("order", "/orders/compensate", orderId);
            return Failure("Inventory reservation failed");
        }

        if (!await Send("pay", "/payment", new CapturePayment(orderId, req.Amount)))
        {
            await Compensate("stock", "/stock/compensate",  orderId);
            await Compensate("order", "/orders/compensate", orderId);
            return Failure("Payment failed");
        }

        state.Status = SagaStatus.Completed;
        await context.SaveChangesAsync();

        return Success();
    }

    private static Result Success() => new(true, null);
    private static Result Failure(string reason) => new(false, reason);

    private async Task<bool> Send<T>(string client, string uri, T body)
    {
        var http  = factory.CreateClient(client);
        var resp  = await http.PostAsJsonAsync(uri, body);
        return resp.StatusCode == HttpStatusCode.OK;
    }

    private async Task Compensate(string client, string uri, Guid orderId)
    {
        var http = factory.CreateClient(client);
        _ = await http.PostAsJsonAsync(uri, orderId);
    }
}