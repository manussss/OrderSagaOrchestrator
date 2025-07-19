namespace SagaOrchestrator.API.Models;

public readonly record struct Result(bool IsSuccess, string? Error);