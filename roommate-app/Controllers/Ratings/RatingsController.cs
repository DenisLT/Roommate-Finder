using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using roommate_app.Other.Services;
using System.Diagnostics.CodeAnalysis;

namespace roommate_app.Controllers.Favorites;

[Route("[controller]")]
[ApiController]
[ExcludeFromCodeCoverage]
public class RatingsController : Controller
{
    private readonly IRatingsService _ratingsService;

    public RatingsController(IRatingsService ratingsService)
    {
        _ratingsService = ratingsService;
    }

    [HttpGet]
    public IActionResult GetRatings(int userId)
    {
        var ratings = _ratingsService.GetRatings(userId);
        return Ok(ratings);
    }

    [HttpPost]
    public IActionResult AddRating([FromBody] RatingRequest ratingRequest)
    {
        if (ratingRequest.Rating < 1 || ratingRequest.Rating > 5)
            return StatusCode(StatusCodes.Status400BadRequest, "Rating must be between 1 and 5");
        try
        {
            _ratingsService.AddRating(ratingRequest.UserId, ratingRequest.ListingId, ratingRequest.Rating);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        }
        return Ok("Success");
    }

    [HttpDelete]
    public IActionResult DeleteRating([FromBody] RatingRequest ratingRequest)
    {
        try
        {
            _ratingsService.RemoveRating(ratingRequest.UserId, ratingRequest.ListingId);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        }
        return Ok("Success");
    }

}
