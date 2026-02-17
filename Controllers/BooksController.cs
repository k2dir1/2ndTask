using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VH_2ND_TASK.Data;
using VH_2ND_TASK.DTOs;
using VH_2ND_TASK.Models;
using VH_2ND_TASK.Middleware.Exceptions;

namespace VH_2ND_TASK.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // login is required 
public class BooksController : ControllerBase
{
    private readonly AppDbContext _db;
    public BooksController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<PagedResult<BookResponse>>> GetAll(
    int page,
    int pageSize,
    CancellationToken cancellationToken)
    {
        if (page < 1) return BadRequest("page 1den fazla olmali");
        if (pageSize < 1 || pageSize > 100) return BadRequest("pageSize 1 ile 100 arasi olmasi lazim");

        var query = _db.Books.AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query
            .OrderBy(b => b.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(b => new BookResponse(b.Id, b.Title, b.Author))
            .ToListAsync(cancellationToken);

        var result = new PagedResult<BookResponse>(
            Page: page,
            PageSize: pageSize,
            TotalCount: totalCount,
            TotalPages: totalPages,
            Items: items
        );

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var book = await _db.Books.AsNoTracking()
            .Where(x => x.Id == id)
            .Select(b => new BookResponse(b.Id, b.Title, b.Author))
            .FirstOrDefaultAsync(cancellationToken);

        return book is null ? NotFound() : Ok(book);
    }

    [Authorize(Roles = "Admin,CEO")]
    [HttpPost]
    public async Task<ActionResult<BookResponse>> Create([FromBody] CreateBookRequest dto, CancellationToken cancellationToken)
    {
        var book = new Book
        {
            Title = dto.Title.Trim(),
            Author = dto.Author.Trim()
        };

        _db.Books.Add(book);
        await _db.SaveChangesAsync(cancellationToken);

        return Ok(new BookResponse(book.Id, book.Title, book.Author));
    }

    [Authorize(Roles = "Admin,CEO")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Replace(int id, [FromBody] UpdateBookRequest dto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Author))
            return BadRequest("Tum fieldler doldurulmali");

        var book = await _db.Books.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (book is null)
            throw new NotFoundException($"Book with id {id} not found");

        book.Title = dto.Title.Trim();
        book.Author = dto.Author.Trim();

        await _db.SaveChangesAsync(cancellationToken);

        return Ok(new BookResponse(book.Id, book.Title, book.Author));
    }

    [Authorize(Roles = "Admin,CEO")]
    [HttpPatch("{id:int}")]
    public async Task<IActionResult> Patch(int id, [FromBody] PatchBookRequest dto, CancellationToken cancellationToken)
    {
        var book = await _db.Books.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (book is null)
            throw new NotFoundException("Book not found");

        var changed = false;

        if (!string.IsNullOrWhiteSpace(dto.Title))
        {
            book.Title = dto.Title.Trim();
            changed = true;
        }

        if (!string.IsNullOrWhiteSpace(dto.Author))
        {
            book.Author = dto.Author.Trim();
            changed = true;
        }

        if (!changed)
            return BadRequest("En az bir tane field doldurmali");

        await _db.SaveChangesAsync(cancellationToken);

        return Ok(new BookResponse(book.Id, book.Title, book.Author));
    }
}