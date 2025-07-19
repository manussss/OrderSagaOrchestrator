using System.Net;
using SagaOrchestrator.API.Models;
using SagaOrchestrator.Common.Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<OrderSaga>();
builder.Services.AddHttpClient("order",  c => c.BaseAddress = new("http://localhost:7001"));
builder.Services.AddHttpClient("stock",  c => c.BaseAddress = new("http://localhost:7002"));
builder.Services.AddHttpClient("pay",    c => c.BaseAddress = new("http://localhost:7003"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/checkout", async (OrderRequest req, OrderSaga saga) =>
{
    var result = await saga.ExecuteAsync(req);

    return result.IsSuccess ?
        Results.Ok("🎉 Order completed") :
        Results.Problem(result.Error);
});

await app.RunAsync();

public class OrderSaga
{
    private readonly IHttpClientFactory _factory;
    public OrderSaga(IHttpClientFactory factory) => _factory = factory;

    public async Task<Result> ExecuteAsync(OrderRequest req)
    {
        var orderId = Guid.NewGuid();

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

        return Success();
    }

    private static Result Success() => new(true, null);
    private static Result Failure(string reason) => new(false, reason);

    private async Task<bool> Send<T>(string client, string uri, T body)
    {
        var http  = _factory.CreateClient(client);
        var resp  = await http.PostAsJsonAsync(uri, body);
        return resp.StatusCode == HttpStatusCode.OK;
    }

    private async Task Compensate(string client, string uri, Guid orderId)
    {
        var http = _factory.CreateClient(client);
        _ = await http.PostAsJsonAsync(uri, orderId);
    }
}

public readonly record struct Result(bool IsSuccess, string? Error);