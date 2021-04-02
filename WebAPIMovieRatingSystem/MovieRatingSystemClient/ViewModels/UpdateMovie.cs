using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieRatingSystemClient.ViewModels
{
    public class UpdateMovie
    {
        public int MovieId { get; set; }

        public string Name { get; set; }

        public string Genre { get; set; }

        public double CumulativeRating { get; set; }
    }
}
