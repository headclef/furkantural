using furkantural.Models;
using Microsoft.EntityFrameworkCore;

namespace furkantural.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Log> Logs { get; set; }
}
