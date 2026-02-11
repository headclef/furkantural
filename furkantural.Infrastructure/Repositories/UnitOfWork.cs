using furkantural.Application.Repositories;
using furkantural.Infrastructure.Data;
namespace furkantural.Infrastructure.Repositories;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await context.SaveChangesAsync(cancellationToken);

    public void Dispose()
    {
        context.Dispose();
        GC.SuppressFinalize(this);
    }
}