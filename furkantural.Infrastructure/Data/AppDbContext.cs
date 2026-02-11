using furkantural.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace furkantural.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Log> Logs { get; set; }
}