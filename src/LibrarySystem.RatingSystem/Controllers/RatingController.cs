using LibrarySystem.RatingSystem.Context;
using LibrarySystem.RatingSystem.DTO;
using LibrarySystem.RatingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.RatingSystem.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class RatingController : ControllerBase
{
    private readonly ILogger<RatingController> _logger;
    private readonly RatingsContext _ratingsContext;

    public RatingController(ILogger<RatingController> logger, RatingsContext ratingsContext)
    {
        _logger = logger;
        _ratingsContext = ratingsContext;
    }

    [HttpGet]
    public async Task<ActionResult<UserRatingResponse>> Get([FromHeader(Name = "X-User-Name")] string xUserName)
    {
        _logger.LogInformation("Requested rating for user {UserName}", xUserName);
        var rating = await _ratingsContext.Ratings.FirstOrDefaultAsync(r => r.Username.Equals(xUserName));
        var response = new UserRatingResponse()
        {
            Stars = 1,
        };
        if (rating != null)
        {
            response.Stars = rating.Stars;
        }

        return response;
    }

    [HttpPatch]
    public async Task<ActionResult<UserRatingResponse>> ChangeUserRating(
        [FromHeader(Name = "X-User-Name")] string xUserName,
        [FromBody] ChangeUserRatingRequest request)
    {
        var rating = await _ratingsContext.Ratings.FirstOrDefaultAsync(r => r.Username.Equals(xUserName));
        if (rating == null)
        {
            rating = new Rating()
            {
                Username = xUserName,
                Stars = 1,
            };
            await _ratingsContext.Ratings.AddAsync(rating);
        }

        rating.Stars += request.Value;
        if (rating.Stars < 1)
        {
            rating.Stars = 1;
        }

        await _ratingsContext.SaveChangesAsync();
        var response = new UserRatingResponse()
        {
            Stars = rating.Stars
        };
        return response;
    }
}