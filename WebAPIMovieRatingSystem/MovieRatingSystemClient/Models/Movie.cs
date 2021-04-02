using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MovieRatingSystemClient.Models
{
    public class Movie
    {
        public int MovieId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Genre { get; set; }

        [Required]
        [Range(0.0, 10.0)]
        public double CumulativeRating { get; set; }

        public ICollection<Rating> Ratings { get; set; }
    }
}
