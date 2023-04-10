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

    public FavoritesController(IFavoritesService favoritesService, IGenericService genericService)
    {
        _favoritesService = favoritesService;
        _genericService = genericService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetFavoriteListings(int userId)
    {
        var favorites = await _genericService.GetAllAsync<Listing>();
        favorites.ForEach(l => l.isFavorite = _favoritesService.IsFavorite(userId, l.Id));
        favorites = favorites.Where(l => l.isFavorite).ToList();
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
