using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebAPIMovieRatingSystem.Data.Repositories;
using WebAPIMovieRatingSystem.Models;

namespace WebAPIMovieRatingSystem.Controllers
{
    [Route("api/Ratings")]
    [ApiController]
    public class RatingsController : ControllerBase
    {
        private readonly ILogger<RatingsController> _logger;
        private readonly IMovieRepository _movieRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRatingRepository _ratingRepository;
        public RatingsController(ILogger<RatingsController> logger, IMovieRepository movieRepository, IUserRepository userRepository, IRatingRepository ratingRepository)
        {
            _logger = logger;
            _movieRepository = movieRepository;
            _userRepository = userRepository;
            _ratingRepository = ratingRepository;
        }

        [Route("ratingsofuser/{id}")]
        [HttpGet]
        public ActionResult RatingsOfUser(int id)
        {
            IList<Rating> ratings = _ratingRepository.GetRatingsOfUser(id);
            return Ok(ratings);
        }

        [Route("ratingsofmovie/{id}")]
        [HttpGet]
        public ActionResult RatingsOfMovie(int id)
        {
            IList<Rating> ratings = _ratingRepository.GetRatingsOfMovie(id);
            return Ok(ratings);
        }

        [Route("ratingsofuserofmovie/{userid}/{movieid}")]
        [HttpGet]
        public ActionResult RatingsOfUserOfMovie(int userid, int movieid)
        {
            Rating rating = _ratingRepository.GetRatingOfUserOfMovie(userid,movieid);
            return Ok(rating);
        }

        [HttpGet]
        public ActionResult<IEnumerable<Rating>> GetAllRatings()
        {
            var ratings = _ratingRepository.GetAllRatings();
            if (ratings == null)
            {
                return NotFound();
            }
            return Ok(ratings);
        }

        [HttpGet("{id}")]
        public ActionResult<Rating> GetRatingById(int id)
        {
            var rating = _ratingRepository.GetRating(id);
            if (rating == null)
            {
                return NotFound();
            }
            return Ok(rating);
        }

        [HttpPost]
        public ActionResult<Rating> CreateRating(Rating rating)
        {
            Rating rts = new Rating();
            rts.UserProvidedRating = rating.UserProvidedRating;
            rts.MovieId = rating.MovieId;
            rts.UserId = rating.UserId;
            rts.User = rating.User;
            rts.Movie = rating.Movie;
            rts = _ratingRepository.Add(rts);
            return Ok(rts);
        }

        [HttpPut("{id}")]
        public ActionResult<Rating> UpdateRating(int id, Rating rating)
        {
            Rating rts = _ratingRepository.GetRating(id);
            if (rts == null)
            {
                return BadRequest();
            }
            rts.UserProvidedRating = rating.UserProvidedRating;
            rts.User = rating.User;
            rts.Movie = rating.Movie;
            rts = _ratingRepository.Update(rts);

            return Ok(rts);
        }

        [HttpDelete("{id}")]

        public ActionResult<Rating> DeleteRating(int id)
        {
            Rating rts = _ratingRepository.Delete(id);
            if (rts == null)
            {
                return NotFound();
            }
            return Ok(rts);
        }

    }
}
