using Microsoft.EntityFrameworkCore;
using roommate_app.Data;
using roommate_app.Models;

namespace roommate_app.Other.Services;

public interface IRatingsService
{
    void AddRating(int userId, int listingId, int rating);
    void RemoveRating(int userId, int listingId);
    IList<Rating> GetRatings(int userId);
    int GetRatingsCount(int listingId);
    double GetRating(int listingId);
    bool UserHasRated(int userId, int listingId);
    int GetUserRating(int userId, int listingId);
}

public class RatingsService : IRatingsService
{
    private readonly ApplicationDbContext _context;

    public RatingsService(ApplicationDbContext context)
    {
        _context = context;
    }

    public void AddRating(int userId, int listingId, int rating)
    {
        _context.Ratings.Where(r => r.UserId == userId && r.ListingId == listingId).ForEachAsync(r => _context.Ratings.Remove(r));
        _context.Ratings.Add(new Rating
        {
            UserId = userId,
            ListingId = listingId,
            Score = rating
        });
        _context.SaveChanges();
    }

    public IList<Rating> GetRatings(int userId)
    {
        return _context.Ratings.Where(r => r.UserId == userId).ToList();
    }

    public int GetRatingsCount(int listingId)
    {
        return _context.Ratings.Where(r => r.ListingId == listingId).Count();
    }

    public double GetRating(int listingId)
    {
        return _context.Ratings.Where(r => r.ListingId == listingId).Average(r => r.Score);
    }

    public bool UserHasRated(int userId, int listingId)
    {
        return _context.Ratings.Any(r => r.UserId == userId && r.ListingId == listingId);
    }

    public int GetUserRating(int userId, int listingId)
    {
        return _context.Ratings.FirstOrDefault(r => r.UserId == userId && r.ListingId == listingId).Score;
    }

    public void RemoveRating(int userId, int listingId)
    {
        var rating = _context.Ratings.FirstOrDefault(r => r.UserId == userId && r.ListingId == listingId);
        if (rating != null)
        {
            _context.Ratings.Remove(rating);
        }
        _context.SaveChanges();
    }
}
