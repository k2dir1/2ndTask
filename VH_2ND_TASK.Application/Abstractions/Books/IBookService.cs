using VH_2ND_TASK.Application.DTOs;

namespace VH_2ND_TASK.Application.Abstractions.Books;

public interface IBookService
{
    Task<PagedResult<BookResponse>> GetAllAsync(int page, int pageSize, CancellationToken ct);
    Task<BookResponse?> GetByIdAsync(int id, CancellationToken ct);
    Task<BookResponse> CreateAsync(CreateBookRequest dto, CancellationToken ct);
    Task<BookResponse> ReplaceAsync(int id, UpdateBookRequest dto, CancellationToken ct);
    Task<BookResponse> PatchAsync(int id, PatchBookRequest dto, CancellationToken ct);
}