using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieRatingSystemClient.ViewModels
{
    public class RatingByUser
    {
        public int UserId { get; set; }
        public string Email { get; set; }

        public string Rating { get; set; }
    }
}
