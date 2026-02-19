using VH_2ND_TASK.Application.Abstractions.Books;
using VH_2ND_TASK.Application.Abstractions.Persistence;
using VH_2ND_TASK.Application.DTOs;
using VH_2ND_TASK.Application.Entities;

namespace VH_2ND_TASK.Application.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _books;
    private readonly IUnitOfWork _uow;

    public BookService(IBookRepository books, IUnitOfWork uow)
    {
        _books = books;
        _uow = uow;
    }

    public async Task<PagedResult<BookResponse>> GetAllAsync(int page, int pageSize, CancellationToken ct)
    {
        if (page < 1) throw new InvalidOperationException("page 1den fazla olmali");
        if (pageSize < 1 || pageSize > 100) throw new InvalidOperationException("pageSize 1 ile 100 arasi olmasi lazim");

        var (items, totalCount) = await _books.GetPagedAsync(page, pageSize, ct);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var mapped = items.Select(b => new BookResponse(b.Id, b.Title, b.Author)).ToList();

        return new PagedResult<BookResponse>(page, pageSize, totalCount, totalPages, mapped);
    }

    public async Task<BookResponse?> GetByIdAsync(int id, CancellationToken ct)
    {
        var book = await _books.GetByIdAsync(id, ct);
        return book is null ? null : new BookResponse(book.Id, book.Title, book.Author);
    }

    public async Task<BookResponse> CreateAsync(CreateBookRequest dto, CancellationToken ct)
    {
        var book = new Book { Title = dto.Title.Trim(), Author = dto.Author.Trim() };
        await _books.AddAsync(book, ct);
        await _uow.SaveChangesAsync(ct);
        return new BookResponse(book.Id, book.Title, book.Author);
    }

    public async Task<BookResponse> ReplaceAsync(int id, UpdateBookRequest dto, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Author))
            throw new InvalidOperationException("Tum fieldler doldurulmali");

        var book = await _books.GetByIdAsync(id, ct);
        if (book is null) throw new KeyNotFoundException($"Book with id {id} not found");

        book.Title = dto.Title.Trim();
        book.Author = dto.Author.Trim();

        await _uow.SaveChangesAsync(ct);
        return new BookResponse(book.Id, book.Title, book.Author);
    }

    public async Task<BookResponse> PatchAsync(int id, PatchBookRequest dto, CancellationToken ct)
    {
        var book = await _books.GetByIdAsync(id, ct);
        if (book is null) throw new KeyNotFoundException("Book not found");

        var changed = false;

        if (!string.IsNullOrWhiteSpace(dto.Title)) { book.Title = dto.Title.Trim(); changed = true; }
        if (!string.IsNullOrWhiteSpace(dto.Author)) { book.Author = dto.Author.Trim(); changed = true; }

        if (!changed) throw new InvalidOperationException("En az bir tane field doldurmali");

        await _uow.SaveChangesAsync(ct);
        return new BookResponse(book.Id, book.Title, book.Author);
    }
}