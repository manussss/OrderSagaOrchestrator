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

// TODO save to database
var captured = new List<Guid>();

app.MapPost("/payment", (CapturePayment cmd) =>
{
    captured.Add(cmd.OrderId);
    return Results.Ok(new Success(cmd.OrderId));
});

app.MapPost("/payment/refund", (Guid orderId) =>
{
    captured.Remove(orderId);
    return Results.Ok();
});

await app.RunAsync("http://localhost:7003");
