using furkantural.Application.Repositories;
using furkantural.Application.Services.Abstract;
using furkantural.Domain.Entities.Common;
using furkantural.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
namespace furkantural.Infrastructure.Repositories;

public class Repository<T>(AppDbContext context, IDateTimeProvider dateTime) : IRepository<T> where T : BaseEntity
{
    protected readonly DbSet<T> _dbSet = context.Set<T>();

    public async Task<T?> GetByIdAsync(int id)
        => await _dbSet.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);

    public async Task<IEnumerable<T>> GetAllAsync()
        => await _dbSet.Where(e => !e.IsDeleted).ToListAsync();

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        => await _dbSet.Where(e => !e.IsDeleted).Where(predicate).ToListAsync();

    public async Task AddAsync(T entity)
    {
        entity.CreatedAt = dateTime.Now;
        await _dbSet.AddAsync(entity);
    }

    public void Update(T entity)
    {
        entity.UpdatedAt = dateTime.Now;
        _dbSet.Update(entity);
    }

    public void Remove(T entity)
        => _dbSet.Remove(entity);

    public void SoftDelete(T entity)
    {
        entity.IsDeleted = true;
        entity.IsActive = false;
        entity.DeletedAt = dateTime.Now;
        _dbSet.Update(entity);
    }
}