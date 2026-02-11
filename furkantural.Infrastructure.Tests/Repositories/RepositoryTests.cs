using FluentAssertions;
using furkantural.Application.Services.Abstract;
using furkantural.Domain.Entities;
using furkantural.Infrastructure.Data;
using furkantural.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
namespace furkantural.Infrastructure.Tests.Repositories;

public class RepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly Repository<Log> _repository;
    private readonly Mock<IDateTimeProvider> _dateTime = new();
    private readonly DateTime _fixedNow = new(2026, 2, 10, 12, 0, 0);

    public RepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _dateTime.Setup(d => d.Now).Returns(_fixedNow);
        _repository = new Repository<Log>(_context, _dateTime.Object);
    }

    private static Log CreateLog(string message = "Test") => new()
    {
        Project = "Test",
        Level = "Info",
        Message = message,
        Date = DateTime.UtcNow
    };

    #region AddAsync

    [Fact]
    public async Task AddAsync_ShouldSetCreatedAt()
    {
        var log = CreateLog();
        await _repository.AddAsync(log);
        await _context.SaveChangesAsync();

        log.CreatedAt.Should().Be(_fixedNow);
    }

    [Fact]
    public async Task AddAsync_ShouldPersistEntity()
    {
        var log = CreateLog("persist-test");
        await _repository.AddAsync(log);
        await _context.SaveChangesAsync();

        var found = await _context.Logs.FirstOrDefaultAsync(l => l.Message == "persist-test");
        found.Should().NotBeNull();
    }

    #endregion

    #region GetByIdAsync

    [Fact]
    public async Task GetByIdAsync_WhenExists_ShouldReturnEntity()
    {
        var log = CreateLog();
        await _repository.AddAsync(log);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(log.Id);

        result.Should().NotBeNull();
        result!.Message.Should().Be("Test");
    }

    [Fact]
    public async Task GetByIdAsync_WhenDeleted_ShouldReturnNull()
    {
        var log = CreateLog();
        log.IsDeleted = true;
        _context.Logs.Add(log);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(log.Id);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotExists_ShouldReturnNull()
    {
        var result = await _repository.GetByIdAsync(999);

        result.Should().BeNull();
    }

    #endregion

    #region GetAllAsync

    [Fact]
    public async Task GetAllAsync_ShouldExcludeDeletedEntities()
    {
        var active = CreateLog("active");
        var deleted = CreateLog("deleted");
        deleted.IsDeleted = true;

        _context.Logs.AddRange(active, deleted);
        await _context.SaveChangesAsync();

        var results = await _repository.GetAllAsync();

        results.Should().ContainSingle();
        results.First().Message.Should().Be("active");
    }

    #endregion

    #region FindAsync

    [Fact]
    public async Task FindAsync_ShouldFilterByPredicate()
    {
        var log1 = CreateLog("alpha");
        var log2 = CreateLog("beta");
        await _repository.AddAsync(log1);
        await _repository.AddAsync(log2);
        await _context.SaveChangesAsync();

        var results = await _repository.FindAsync(l => l.Message == "alpha");

        results.Should().ContainSingle();
        results.First().Message.Should().Be("alpha");
    }

    [Fact]
    public async Task FindAsync_ShouldExcludeDeletedEntities()
    {
        var log = CreateLog("findme");
        log.IsDeleted = true;
        _context.Logs.Add(log);
        await _context.SaveChangesAsync();

        var results = await _repository.FindAsync(l => l.Message == "findme");

        results.Should().BeEmpty();
    }

    #endregion

    #region Update

    [Fact]
    public async Task Update_ShouldSetUpdatedAt()
    {
        var log = CreateLog();
        await _repository.AddAsync(log);
        await _context.SaveChangesAsync();

        log.Message = "Updated";
        _repository.Update(log);
        await _context.SaveChangesAsync();

        log.UpdatedAt.Should().Be(_fixedNow);
    }

    #endregion

    #region SoftDelete

    [Fact]
    public async Task SoftDelete_ShouldMarkAsDeletedAndInactive()
    {
        var log = CreateLog();
        await _repository.AddAsync(log);
        await _context.SaveChangesAsync();

        _repository.SoftDelete(log);
        await _context.SaveChangesAsync();

        log.IsDeleted.Should().BeTrue();
        log.IsActive.Should().BeFalse();
        log.DeletedAt.Should().Be(_fixedNow);
    }

    [Fact]
    public async Task SoftDelete_ShouldHideFromGetAllAsync()
    {
        var log = CreateLog();
        await _repository.AddAsync(log);
        await _context.SaveChangesAsync();

        _repository.SoftDelete(log);
        await _context.SaveChangesAsync();

        var results = await _repository.GetAllAsync();
        results.Should().BeEmpty();
    }

    #endregion

    #region Remove

    [Fact]
    public async Task Remove_ShouldPhysicallyDeleteEntity()
    {
        var log = CreateLog();
        await _repository.AddAsync(log);
        await _context.SaveChangesAsync();

        _repository.Remove(log);
        await _context.SaveChangesAsync();

        var count = await _context.Logs.CountAsync();
        count.Should().Be(0);
    }

    #endregion

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}