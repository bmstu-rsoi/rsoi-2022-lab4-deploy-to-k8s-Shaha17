using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using LibrarySystem.LibrarySystem.Context;
using LibrarySystem.LibrarySystem.DTO;
using LibrarySystem.LibrarySystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.LibrarySystem.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class LibrariesController : ControllerBase
{
    private readonly ILogger<LibrariesController> _logger;
    private readonly LibrariesContext _librariesContext;

    public LibrariesController(ILogger<LibrariesController> logger, LibrariesContext librariesContext)
    {
        _logger = logger;
        _librariesContext = librariesContext;
    }

    [HttpGet]
    public async Task<ActionResult<PaginationResponse<IEnumerable<Library>>>> Get([FromQuery] string city,
        [FromQuery] int? page,
        [FromQuery] int? size)
    {
        _logger.LogInformation("Request to libs by city {City}", city.ToString());

        var query = _librariesContext.Libraries.AsNoTracking().AsQueryable();
        if (!string.IsNullOrEmpty(city))
        {
            query = query.Where(l => l.City.ToLower() == city.ToLower());
        }

        var total = await query.CountAsync();

        if (page.HasValue && size.HasValue)
        {
            query = query.OrderBy(l => l.Id).Skip((page.Value - 1) * size.Value).Take(size.Value);
        }

        var libs = await query.ToListAsync();

        var response = new PaginationResponse<IEnumerable<Library>>()
        {
            Page = page.Value,
            PageSize = size.Value,
            Items = libs,
            TotalElements = total
        };

        return response;
    }

    [HttpGet("{libUid:guid}")]
    public async Task<ActionResult<Library>> GetLibraryByUid([FromRoute] Guid libUid)
    {
        var lib = await _librariesContext.Libraries.FirstOrDefaultAsync(l => l.LibraryUid.Equals(libUid));
        return lib;
    }

    [HttpGet("{libraryUid:guid}/books")]
    public async Task<ActionResult<PaginationResponse<IEnumerable<Book>>>> GetBooksInLibrary(
        [FromRoute] Guid libraryUid,
        [FromQuery] int? page,
        [FromQuery] int? size,
        [FromQuery] bool? showAll)
    {
        _logger.LogInformation("Request to books by lib {LibUid}", libraryUid.ToString());

        var libId = await _librariesContext.Libraries.AsNoTracking().AsQueryable()
            .Where(l => l.LibraryUid.Equals(libraryUid))
            .Select(l => l.Id).FirstOrDefaultAsync();

        var libBooksQuery = _librariesContext.LibraryBooks.AsNoTracking().AsQueryable()
            .Where(lb => lb.LibraryId.Equals(libId));
        if (!(showAll.HasValue && showAll.Value))
        {
            libBooksQuery = libBooksQuery.Where(lb => lb.AvailableCount > 0);
        }

        var booksIdsQuery = libBooksQuery.Select(lb => lb.BookId);
        var booksQuery = _librariesContext.Books.AsNoTracking().AsQueryable()
            .Where(b => booksIdsQuery.Contains(b.Id));

        var total = await booksQuery.CountAsync();

        if (page.HasValue && size.HasValue)
        {
            booksQuery = booksQuery.OrderBy(b => b.Id).Skip((page.Value - 1) * size.Value).Take(size.Value);
        }

        var books = await booksQuery.ToListAsync();
        var response = new PaginationResponse<IEnumerable<Book>>()
        {
            Page = page.Value,
            PageSize = size.Value,
            Items = books,
            TotalElements = total
        };

        return response;
    }

    [HttpGet("{libraryUid:guid}/books/{bookUid:guid}")]
    public async Task<ActionResult<LibraryBook>> GetBookInLibraryByUid([FromRoute] Guid libraryUid,
        [FromRoute] Guid bookUid)
    {
        _logger.LogInformation("Request to book from libUid {LibUid} by bookUid {BookUid}", libraryUid.ToString(),
            bookUid.ToString());

        var lib = await _librariesContext.Libraries.AsNoTracking().AsQueryable()
            .FirstOrDefaultAsync(l => l.LibraryUid.Equals(libraryUid));
        var book = await _librariesContext.Books.AsNoTracking().AsQueryable()
            .FirstOrDefaultAsync(b => b.BookUid.Equals(bookUid));

        var libBook =
            await _librariesContext.LibraryBooks.AsNoTracking().FirstOrDefaultAsync(lb =>
                lb.LibraryId.Equals(lib.Id) && lb.BookId.Equals(book.Id));
        if (libBook == null)
        {
            return NotFound();
        }

        libBook.Library = lib;
        libBook.Book = book;

        return libBook;
    }
    
    [HttpPatch("{libraryUid:guid}/books/{bookUid:guid}/increment")]
    public async Task<ActionResult<LibraryBook>> IncrementBookAvailableCount([FromRoute] Guid libraryUid,
        [FromRoute] Guid bookUid)
    {
        _logger.LogInformation("Request to increment book available count: libUid {LibUid}, bookUid {BookUid}",
            libraryUid.ToString(),
            bookUid.ToString());
        var lib = await _librariesContext.Libraries.AsNoTracking().AsQueryable()
            .FirstOrDefaultAsync(l => l.LibraryUid.Equals(libraryUid));
        var book = await _librariesContext.Books.AsNoTracking().AsQueryable()
            .FirstOrDefaultAsync(b => b.BookUid.Equals(bookUid));

        var libBook = await _librariesContext.LibraryBooks.FirstOrDefaultAsync(lb =>
            lb.LibraryId.Equals(lib.Id) && lb.BookId.Equals(book.Id));

        libBook.AvailableCount += 1;
        await _librariesContext.SaveChangesAsync();
        libBook.Book = book;
        libBook.Library = lib;
        return libBook;
    }

    [HttpPatch("{libraryUid:guid}/books/{bookUid:guid}/decrement")]
    public async Task<ActionResult<LibraryBook>> DecrementBookAvailableCount([FromRoute] Guid libraryUid,
        [FromRoute] Guid bookUid)
    {
        _logger.LogInformation("Request to increment book available count: libUid {LibUid}, bookUid {BookUid}",
            libraryUid.ToString(),
            bookUid.ToString());
        var lib = await _librariesContext.Libraries.AsNoTracking().AsQueryable()
            .FirstOrDefaultAsync(l => l.LibraryUid.Equals(libraryUid));
        var book = await _librariesContext.Books.AsNoTracking().AsQueryable()
            .FirstOrDefaultAsync(b => b.BookUid.Equals(bookUid));

        var libBook = await _librariesContext.LibraryBooks.FirstOrDefaultAsync(lb =>
            lb.LibraryId.Equals(lib.Id) && lb.BookId.Equals(book.Id));

        libBook.AvailableCount -= 1;
        await _librariesContext.SaveChangesAsync();
        libBook.Book = book;
        libBook.Library = lib;
        return libBook;
    }
}