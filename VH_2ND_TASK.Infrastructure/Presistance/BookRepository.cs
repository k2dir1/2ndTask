using Microsoft.EntityFrameworkCore;
using VH_2ND_TASK.Application.Abstractions.Persistence;
using VH_2ND_TASK.Application.Entities;
using VH_2ND_TASK.Infrastructure.Data;

namespace VH_2ND_TASK.Infrastructure.Persistence;

public class BookRepository : IBookRepository
{
    private readonly AppDbContext _db;
    public BookRepository(AppDbContext db) => _db = db;

    public async Task<(List<Book> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken ct)
    {
        var query = _db.Books.AsNoTracking();

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderBy(b => b.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public Task<Book?> GetByIdAsync(int id, CancellationToken ct)
        => _db.Books.FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task AddAsync(Book book, CancellationToken ct)
    {
        _db.Books.Add(book);
        return Task.CompletedTask;
    }
}