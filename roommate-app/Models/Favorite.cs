using System.Diagnostics.CodeAnalysis;

namespace roommate_app.Models;

[ExcludeFromCodeCoverage]
public class Favorited
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ListingId { get; set; }
}
