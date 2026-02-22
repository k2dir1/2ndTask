using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VH_2ND_TASK.Application.Abstractions.Books;
using VH_2ND_TASK.Application.DTOs;


namespace VH_2ND_TASK.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BooksController : ControllerBase
{
    private readonly IBookService _books;

    public BooksController(IBookService books) => _books = books;

    [HttpGet]
    public async Task<ActionResult<PagedResult<BookResponse>>> GetAll(int page, int pageSize, CancellationToken ct)
        => Ok(await _books.GetAllAsync(page, pageSize, ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookResponse>> GetById(int id, CancellationToken ct)
        => (await _books.GetByIdAsync(id, ct)) is { } book ? Ok(book) : NotFound();

    [Authorize(Roles = "Admin,CEO")]
    [HttpPost]
    public async Task<ActionResult<BookResponse>> Create([FromBody] CreateBookRequest dto, CancellationToken ct)
        => Ok(await _books.CreateAsync(dto, ct));

    [Authorize(Roles = "Admin,CEO")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Replace(int id, [FromBody] UpdateBookRequest dto, CancellationToken ct)
        => Ok(await _books.ReplaceAsync(id, dto, ct));

    [Authorize(Roles = "Admin,CEO")]
    [HttpPatch("{id:int}")]
    public async Task<IActionResult> Patch(int id, [FromBody] PatchBookRequest dto, CancellationToken ct)
        => Ok(await _books.PatchAsync(id, dto, ct));
}