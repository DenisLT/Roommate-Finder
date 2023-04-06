using System.Diagnostics.CodeAnalysis;

namespace roommate_app.Models;

[ExcludeFromCodeCoverage]
public class Rating
{
    public int UserId { get; set; }
    public int ListingId { get; set; }
    public int Score { get; set; }
}
