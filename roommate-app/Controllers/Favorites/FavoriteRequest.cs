using System.ComponentModel.DataAnnotations;

namespace roommate_app.Controllers.Favorites
{
    public class FavoriteRequest
    {
        [Required]
        public int ListingId { get; set; }
        [Required]
        public int UserId { get; set; }
    }
}
