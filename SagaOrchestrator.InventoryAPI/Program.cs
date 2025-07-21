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
var reserved = new List<Guid>();

app.MapPost("/stock", (ReserveStock cmd) =>
{
    // Fake failure for demo purposes
    if (Random.Shared.Next(1, 6) == 3)
        return Results.BadRequest(new Failure(cmd.OrderId, "No stock"));

    reserved.Add(cmd.OrderId);
    return Results.Ok(new Success(cmd.OrderId));
});

app.MapPost("/stock/compensate", (Guid orderId) =>
{
    reserved.Remove(orderId);
    return Results.Ok();
});

await app.RunAsync();