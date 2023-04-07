using roommate_app.Data;
using roommate_app.Models;

namespace roommate_app.Other.Services;

public interface IRatingsService
{
    public void AddRating(int userId, int listingId, int rating);
    public void RemoveRating(int userId, int listingId);
    public IList<Rating> GetRatings(int userId);
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
        _context.Ratings.Add(new Rating
        {
            UserId = userId,
            ListingId = listingId,
            Score = rating
        });
    }

    public IList<Rating> GetRatings(int userId)
    {
        return _context.Ratings.Where(r => r.UserId == userId).ToList();
    }

    public void RemoveRating(int userId, int listingId)
    {
        var rating = _context.Ratings.FirstOrDefault(r => r.UserId == userId && r.ListingId == listingId);
        if (rating != null)
        {
            _context.Ratings.Remove(rating);
        }
    }
}
