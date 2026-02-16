    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
using VH_2ND_TASK.Data;
    using VH_2ND_TASK.Models;
using VH_2ND_TASK.Middleware.Exceptions;

namespace VH_2ND_TASK.Controllers;

    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _db;
        public record CreateBookRequest(string Title, string Author);
        public record UpdateBookRequest(string Title, string Author);    
        public record PatchBookRequest(string? Title, string? Author);   
        public BooksController(AppDbContext db) => _db = db;

        [AllowAnonymous] 
        [HttpGet]
        public async Task<List<Book>> GetAll(CancellationToken cancellationToken) =>
            await _db.Books.AsNoTracking().ToListAsync(cancellationToken);
        
        [AllowAnonymous]      
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Book>> GetById(int id)
        {
            var book = await _db.Books.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        var item = 0;
        var avg = 2/item;
        return book is null ? NotFound() : Ok(book);
              
        }

        [Authorize] 
        [HttpPost]
        public async Task<ActionResult<Book>> Create([FromBody] CreateBookRequest dto, CancellationToken cancellationToken)
        {
            var book = new Book
            {
                Title = dto.Title.Trim(),
                Author = dto.Author.Trim()
            };

            _db.Books.Add(book);
            await _db.SaveChangesAsync(cancellationToken);

        return Ok(book);

        }

        [Authorize]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Replace(int id, [FromBody] UpdateBookRequest dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Author))
                return BadRequest("Tum fieldler doldurulmali");
;

            var book = await _db.Books.FirstOrDefaultAsync(x => x.Id == id);
            if (book is null)
                throw new NotFoundException($"Book with id {id} not found");

        book.Title = dto.Title.Trim();
            book.Author = dto.Author.Trim();

            await _db.SaveChangesAsync();

            return Ok(book); 
        }

        [Authorize]
        [HttpPatch("{id:int}")]
        public async Task<IActionResult> Patch(int id, [FromBody] PatchBookRequest dto)
        {
            var book = await _db.Books.FirstOrDefaultAsync(x => x.Id == id);
        if (book == null)
            throw new KeyNotFoundException("Book not found");

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

            await _db.SaveChangesAsync();

            return Ok(book); 
        }

    
        [AllowAnonymous]
        [HttpGet("test-error")]
        public IActionResult TestError()
        {
            throw new Exception("database exploded");
        }
}