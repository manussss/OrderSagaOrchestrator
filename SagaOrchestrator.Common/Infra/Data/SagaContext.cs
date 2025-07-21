using Microsoft.EntityFrameworkCore;
using SagaOrchestrator.Common.Models;

namespace SagaOrchestrator.Common.Infra.Data;

public class SagaDbContext : DbContext
{
    public SagaDbContext(DbContextOptions<SagaDbContext> options) : base(options) { }

    public DbSet<OrderSagaState> OrderSagaState => Set<OrderSagaState>();
}