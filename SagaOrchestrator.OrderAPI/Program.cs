using SagaOrchestrator.Common.Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//TODO save to database
var orders = new List<Guid>();

app.MapPost("/orders", (CreateOrder cmd) =>
{
    orders.Add(cmd.OrderId);
    return Results.Ok(new Success(cmd.OrderId));
});

app.MapPost("/orders/compensate", (Guid orderId) =>
{
    orders.Remove(orderId);
    return Results.Ok();
});

await app.RunAsync("http://localhost:7001");
