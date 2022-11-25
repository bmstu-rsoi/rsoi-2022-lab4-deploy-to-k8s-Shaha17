using LibrarySystem.ReservationSystem.Context;
using LibrarySystem.ReservationSystem.DTO;
using LibrarySystem.ReservationSystem.Models;
using LibrarySystem.ReservationSystem.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.ReservationSystem.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ReservationsController : ControllerBase
{
    private readonly ILogger<ReservationsController> _logger;
    private readonly ReservationsContext _reservationsContext;

    public ReservationsController(ILogger<ReservationsController> logger, ReservationsContext reservationsContext)
    {
        _logger = logger;
        _reservationsContext = reservationsContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Reservation>>> GetByUsername(
        [FromHeader(Name = "X-User-Name")] string xUserName)
    {
        if (string.IsNullOrWhiteSpace(xUserName))
        {
            return BadRequest();
        }

        var query = _reservationsContext.Reservations.AsNoTracking().AsQueryable();
        query = query.Where(r => r.Username.Equals(xUserName));
        var response = await query.ToListAsync();
        return response;
    }

    [HttpGet("{reservationUid:guid}")]
    public async Task<ActionResult<Reservation?>> GetByUid([FromRoute] Guid reservationUid)
    {
        var reservation = await _reservationsContext.Reservations.AsNoTracking()
            .FirstOrDefaultAsync(r => r.ReservationUid.Equals(reservationUid));

        return reservation;
    }

    [HttpPut("{reservationUid:guid}")]
    public async Task<ActionResult<Reservation?>> UpdateByUid([FromRoute] Guid reservationUid,
        [FromBody] Reservation reservation)
    {
        var res = await _reservationsContext.Reservations
            .FirstOrDefaultAsync(r => r.ReservationUid.Equals(reservationUid));
        res.Status = reservation.Status;
        res.Username = reservation.Username;
        res.BookUid = reservation.BookUid;
        res.LibraryUid = reservation.LibraryUid;
        res.StartDate = reservation.StartDate;
        res.TillDate = reservation.TillDate;
        await _reservationsContext.SaveChangesAsync();
        return reservation;
    }

    [HttpDelete("{reservationUid:guid}")]
    public async Task<IActionResult> RemoveReservation([FromRoute] Guid reservationUid)
    {
        var res = await _reservationsContext.Reservations.FirstOrDefaultAsync(r =>
            r.ReservationUid.Equals(reservationUid));
        if (res != null)
        {
            _reservationsContext.Remove(res);
            await _reservationsContext.SaveChangesAsync();
        }

        return Ok();
    }


    [HttpPost]
    public async Task<ActionResult<Reservation>> TakeBook([FromHeader(Name = "X-User-Name")] string xUserName,
        [FromBody] TakeBookRequest request)
    {
        if (string.IsNullOrWhiteSpace(xUserName))
        {
            return BadRequest();
        }

        var newReservation = new Reservation()
        {
            Status = ReservationStatuses.RENTED,
            Username = xUserName,
            BookUid = request.BookUid,
            LibraryUid = request.LibraryUid,
            ReservationUid = Guid.NewGuid(),
            StartDate = DateTime.Now,
            TillDate = request.TillDate,
        };
        await _reservationsContext.Reservations.AddAsync(newReservation);
        await _reservationsContext.SaveChangesAsync();

        return newReservation;
    }
}