using VH_2ND_TASK.Application.DTOs;
using VH_2ND_TASK.Application.Entities;

namespace VH_2ND_TASK.Application.Abstractions.Persistence;

public interface IBookRepository
{
    Task<(List<Book> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken ct);
    Task<Book?> GetByIdAsync(int id, CancellationToken ct);
    Task AddAsync(Book book, CancellationToken ct);
}