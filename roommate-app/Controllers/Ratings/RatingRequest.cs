﻿using System.ComponentModel.DataAnnotations;

namespace roommate_app.Controllers.Favorites
{
    public class RatingRequest
    {
        [Required]
        public int ListingId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int Rating { get; set; }
    }
}