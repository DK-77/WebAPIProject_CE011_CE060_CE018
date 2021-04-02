using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieRatingSystemClient.ViewModels
{
    public class UserRating
    {
        public string name { get; set; }

        public double rating { get; set; }

        public string genre { get; set; }

        public int ratingId { get; set; }
    }
}
