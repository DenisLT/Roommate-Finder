using roommate_app.Data;
using roommate_app.Models;

namespace roommate_app.Other.Services;

public interface IFavoritesService
{
    public void AddFavorite(int userId, int listingId);
    public void RemoveFavorite(int userId, int listingId);
    public bool IsFavorite(int userId, int listingId);
    public IList<Favorited> GetFavorites(int userId);
}

public class FavoritesService : IFavoritesService
{
    private readonly ApplicationDbContext _context;

    public FavoritesService(ApplicationDbContext context)
    {
        _context = context;
    }

    public void AddFavorite(int userId, int listingId)
    {
        _context.Favorites.Add(new Favorited { UserId = userId, ListingId = listingId });
        _context.SaveChanges();
    }

    public void RemoveFavorite(int userId, int listingId)
    {
        var favorited = _context.Favorites.FirstOrDefault(f => f.UserId == userId && f.ListingId == listingId);
        _context.Favorites.Remove(favorited);
        _context.SaveChanges();
    }

    public bool IsFavorite(int userId, int listingId)
    {
        return _context.Favorites.Any(f => f.UserId == userId && f.ListingId == listingId);
    }

    public IList<Favorited> GetFavorites(int userId)
    {
        return _context.Favorites.Where(f => f.UserId == userId).ToList();
    }
}
