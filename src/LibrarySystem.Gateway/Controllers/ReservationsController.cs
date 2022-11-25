using LibrarySystem.Gateway.DTO;
using LibrarySystem.Gateway.Services;
using LibrarySystem.Gateway.Utils;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Gateway.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ReservationsController : ControllerBase
{
    private readonly ILogger<ReservationsController> _logger;
    private readonly ReservationsService _reservationsService;
    private readonly LibrariesService _librariesService;
    private readonly RatingService _ratingService;

    public ReservationsController(ILogger<ReservationsController> logger, ReservationsService reservationsService,
        LibrariesService librariesService, RatingService ratingService)
    {
        _logger = logger;
        _reservationsService = reservationsService;
        _librariesService = librariesService;
        _ratingService = ratingService;
    }

    [HttpGet]
    public async Task<IEnumerable<BookReservationResponse>> GetByUsername(
        [FromHeader(Name = "X-User-Name")] string xUserName)
    {
        var reservations = await _reservationsService.GetReservationsByUsernameAsync(xUserName);
        if (reservations == null || !reservations.Any())
        {
            return Array.Empty<BookReservationResponse>();
        }

        var response = new List<BookReservationResponse>();
        var tasks = reservations.Select(reservation => Task.Run(async () =>
        {
            var libBook = await _librariesService.GetLibraryBookByLibUidAndBookUidAsync(reservation.LibraryUid,
                reservation.BookUid);
            response.Add(new BookReservationResponse()
            {
                Book = libBook.Book,
                Library = libBook.Library,
                Status = reservation.Status,
                ReservationUid = reservation.ReservationUid,
                StartDate = reservation.StartDate,
                TillDate = reservation.TillDate,
            });
        }));

        await Task.WhenAll(tasks);

        return response;
    }

    [HttpPost]
    public async Task<ActionResult<TakeBookResponse>> TakeBook([FromHeader(Name = "X-User-Name")] string xUserName,
        [FromBody] TakeBookRequest request)
    {
        if (string.IsNullOrWhiteSpace(xUserName))
        {
            return BadRequest();
        }

        var checkLibraryServiceTask = _librariesService.HealthCheckAsync();
        var checkRatingServiceTask = _ratingService.HealthCheckAsync();

        if (!(await checkLibraryServiceTask))
        {
            return Problem(statusCode: StatusCodes.Status503ServiceUnavailable);
        }

        if (!(await checkRatingServiceTask))
        {
            var resp = new ErrorResponse()
            {
                Message = "Bonus Service unavailable",
            };
            Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            return new ObjectResult(resp);
        }

        var checkReservationServiceTask = _reservationsService.HealthCheckAsync();

        if (!(await checkReservationServiceTask))
        {
            return Problem(statusCode: StatusCodes.Status503ServiceUnavailable);
        }

        var reservationsTask = _reservationsService.GetReservationsByUsernameAsync(xUserName);
        var userRatingTask = _ratingService.GetUserRatingAsync(xUserName);
        var reservations = await reservationsTask;
        var reservationsCount = reservations.Count(r => r.Status.Equals(ReservationStatuses.RENTED));
        var rating = (await userRatingTask).Stars;

        if (rating <= reservationsCount)
        {
            return BadRequest();
        }

        var reservationTask = _reservationsService.TakeBook(xUserName, request);
        var decrementTask =
            _librariesService.DecrementAvailableCountByLibUidAndBookUid(request.LibraryUid, request.BookUid);
        var reservation = await reservationTask;

        var libBook = await decrementTask;
        if (libBook == null)
        {
            await _reservationsService.RollbackReservationAsync(reservation.ReservationUid);
            return Problem(statusCode: StatusCodes.Status503ServiceUnavailable);
        }

        var response = new TakeBookResponse()
        {
            Book = new BookInfo()
            {
                Author = libBook.Book.Author,
                Genre = libBook.Book.Genre,
                Name = libBook.Book.Name,
                BookUid = libBook.Book.BookUid,
            },
            Library = new LibraryResponse()
            {
                Address = libBook.Library.Address,
                City = libBook.Library.City,
                Name = libBook.Library.Name,
                LibraryUid = libBook.Library.LibraryUid
            },
            Rating = new UserRatingResponse()
            {
                Stars = rating,
            },
            Status = reservation.Status,
            ReservationUid = reservation.ReservationUid,
            StartDate = DateOnly.FromDateTime(reservation.StartDate),
            TillDate = DateOnly.FromDateTime(reservation.TillDate)
        };

        return response;
    }

    [HttpPost("{reservationsUid:guid}/return")]
    public async Task<IActionResult> ReturnBook([FromHeader(Name = "X-User-Name")] string xUserName,
        [FromRoute] Guid reservationsUid, ReturnBookRequest request)
    {
        var reservationToUpd = await _reservationsService.GetReservationsByUidAsync(reservationsUid);
        if (reservationToUpd == null)
        {
            return BadRequest();
        }

        var penalty = 0;

        if (request.Date > reservationToUpd.TillDate)
        {
            reservationToUpd.Status = ReservationStatuses.EXPIRED;
            penalty += 1;
        }
        else
        {
            reservationToUpd.Status = ReservationStatuses.RETURNED;
        }

        var updateReservationTask = _reservationsService.UpdateReservationByUidAsync(reservationToUpd);

        var libBook =
            await _librariesService.GetLibraryBookByLibUidAndBookUidAsync(reservationToUpd.LibraryUid,
                reservationToUpd.BookUid);
        if (_librariesService.IsBookConditionWorse(libBook.Book.Condition, request.Condition))
        {
            penalty += 1;
        }

        var incrementTask = _librariesService.IncrementAvailableCountByLibUidAndBookUid(reservationToUpd.LibraryUid,
            reservationToUpd.BookUid);

        UserRatingResponse? userRatingResponse;
        if (penalty == 0)
        {
            userRatingResponse = await _ratingService.ChangeUserRating(xUserName, 1);
        }
        else
        {
            userRatingResponse = await _ratingService.ChangeUserRating(xUserName, -penalty * 10);
        }


        await updateReservationTask;

        await incrementTask;


        return NoContent();
    }
}