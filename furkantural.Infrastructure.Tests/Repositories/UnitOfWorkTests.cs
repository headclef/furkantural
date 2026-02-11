using FluentAssertions;
using furkantural.Infrastructure.Data;
using furkantural.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
namespace furkantural.Infrastructure.Tests.Repositories;

public class UnitOfWorkTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly UnitOfWork _unitOfWork;

    public UnitOfWorkTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _unitOfWork = new UnitOfWork(_context);
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldPersistChanges()
    {
        _context.Logs.Add(new Domain.Entities.Log
        {
            Project = "Test",
            Level = "Info",
            Message = "UoW Test",
            Date = DateTime.UtcNow
        });

        var result = await _unitOfWork.SaveChangesAsync();

        result.Should().Be(1);
    }

    [Fact]
    public async Task SaveChangesAsync_WithNoChanges_ShouldReturnZero()
    {
        var result = await _unitOfWork.SaveChangesAsync();

        result.Should().Be(0);
    }

    [Fact]
    public void Dispose_ShouldNotThrow()
    {
        var action = () => _unitOfWork.Dispose();

        action.Should().NotThrow();
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}