using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using roommate_app.Other.Services;
using System.Diagnostics.CodeAnalysis;

namespace roommate_app.Controllers.Favorites;

[Route("[controller]")]
[ApiController]
[ExcludeFromCodeCoverage]
public class FavoritesController : Controller
{
    private readonly IFavoritesService _favoritesService;

    public FavoritesController(IFavoritesService favoritesService)
    {
        _favoritesService = favoritesService;
    }

    [HttpGet]
    public IActionResult GetFavorites(int userId)
    {
        var favorites = _favoritesService.GetFavorites(userId);
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
