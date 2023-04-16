using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using roommate_app.Models;
using roommate_app.Other.Services;
using System.Diagnostics.CodeAnalysis;

namespace roommate_app.Controllers.Favorites;

[Route("[controller]")]
[ApiController]
[ExcludeFromCodeCoverage]
public class FavoritesController : Controller
{
    private readonly IFavoritesService _favoritesService;
    private readonly IGenericService _genericService;
    private readonly IRatingsService _ratingsService;

    private User user;

    public FavoritesController(IFavoritesService favoritesService, IGenericService genericService, IRatingsService ratingsService)
    {
        _favoritesService = favoritesService;
        _genericService = genericService;
        _ratingsService = ratingsService;
    }

    void addDataToListing(Listing listing)
    {
        listing.IsFavorite = _favoritesService.IsFavorite(user.Id, listing.Id);
        listing.RatingCount = _ratingsService.GetRatingsCount(listing.Id);
        if (listing.RatingCount > 0)
            listing.Rating = _ratingsService.GetRating(listing.Id);
        listing.UserHasRated = _ratingsService.UserHasRated(user.Id, listing.Id);
        if (listing.UserHasRated)
            listing.UserRating = _ratingsService.GetUserRating(user.Id, listing.Id);
    }

    [HttpGet]
    public async Task<IActionResult> GetFavoriteListings(int userId)
    {
        user = (User)HttpContext.Items["User"];
        var favorites = await _genericService.GetAllAsync<Listing>();
        favorites.ForEach(l => addDataToListing(l));
        favorites = favorites.Where(l => l.IsFavorite).ToList();
        return Ok(favorites);
    }

    [HttpPost]
    public IActionResult AddFavorite([FromBody] FavoriteRequest favoriteRequest)
    {
        try
        {
            _favoritesService.AddFavorite(favoriteRequest.UserId, favoriteRequest.ListingId);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        }
        return Ok("Success");
    }

    [HttpDelete]
    public IActionResult DeleteFavorite([FromBody] FavoriteRequest favoriteRequest)
    {
        try
        {
            _favoritesService.RemoveFavorite(favoriteRequest.UserId, favoriteRequest.ListingId);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        }
        return Ok("Success");
    }

}
