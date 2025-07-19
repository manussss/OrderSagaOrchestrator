using System.Net;
using Microsoft.AspNetCore.Mvc;
using SagaOrchestrator.API.Application.Services;
using SagaOrchestrator.API.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IOrderSaga, OrderSaga>();
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

app.MapPost("/checkout", async (OrderRequest req, [FromServices] IOrderSaga saga) =>
{
    var result = await saga.ExecuteAsync(req);

    return result.IsSuccess ?
        Results.Ok("Order completed") :
        Results.Problem(result.Error);
});

await app.RunAsync();