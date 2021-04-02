using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIMovieRatingSystem.Models
{
    public class Rating
    {
        public int RatingId { get; set; }

        [Required]
        [Range(0.0,10.0)]
        public double UserProvidedRating { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public int MovieId { get; set; }

        public Movie Movie { get; set; }

    }
}
