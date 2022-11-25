using LibrarySystem.Gateway.DTO;
using LibrarySystem.Gateway.Models;
using LibrarySystem.Gateway.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Gateway.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class LibrariesController : ControllerBase
{
    private readonly ILogger<LibrariesController> _logger;
    private readonly LibrariesService _librariesService;

    public LibrariesController(ILogger<LibrariesController> logger, LibrariesService librariesService)
    {
        _logger = logger;
        _librariesService = librariesService;
    }

    [HttpGet]
    public async Task<PaginationResponse<IEnumerable<Library>>?> Get([FromQuery] string city,
        [FromQuery] int? page,
        [FromQuery] int? size)
    {
        _logger.LogInformation("Request to libs by city {City}", city);
        var response = await _librariesService.GetLibrariesByCityAsync(city, page, size);
        return response;
    }

    [HttpGet("{libraryUid:guid}/books")]
    public async Task<PaginationResponse<IEnumerable<Book>>?> GetBooksInLibrary([FromRoute] Guid libraryUid,
        [FromQuery] int? page,
        [FromQuery] int? size,
        [FromQuery] bool? showAll)
    {
        _logger.LogInformation("Request to books by lib {LibUid}", libraryUid.ToString());
        var response = await _librariesService.GetBooksByLibraryAsync(libraryUid, page, size, showAll);
        return response;
    }
}