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
    [Route("api/Movie")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly ILogger<MovieController> _logger;
        private readonly IMovieRepository _movieRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRatingRepository _ratingRepository;
        public MovieController(ILogger<MovieController> logger, IMovieRepository movieRepository, IUserRepository userRepository, IRatingRepository ratingRepository)
        {
            _logger = logger;
            _movieRepository = movieRepository;
            _userRepository = userRepository;
            _ratingRepository = ratingRepository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Movie>> GetAllMovies()
        {
            var movies = _movieRepository.GetAllMovies();
            if(movies == null)
            {
                return NotFound();
            }
            return Ok(movies);
        }

        [HttpGet("{id}")]
        public ActionResult<Movie> GetMovieById(int id)
        {
            var movie = _movieRepository.GetMovie(id);
            if(movie == null)
            {
                return NotFound();
            }
            return Ok(movie);
        }

        [HttpPost]
        public ActionResult<Movie> CreateMovie(Movie movie)
        {
            Movie mv = new Movie();
            mv.Name = movie.Name;
            mv.Genre = movie.Genre;
            mv.CumulativeRating = movie.CumulativeRating;
            mv.Ratings = movie.Ratings;
            mv = _movieRepository.Add(mv);
            return Ok(mv);
        }

        [HttpPut("{id}")]
        public ActionResult<Movie> UpdateMovie(int id, Movie movie)
        {
            Movie mv = _movieRepository.GetMovie(id);
            if(mv == null)
            {
                return BadRequest();
            }
            mv.Name = movie.Name;
            mv.Genre = movie.Genre;
            mv.CumulativeRating = movie.CumulativeRating;
            mv.Ratings = movie.Ratings;
            mv = _movieRepository.Update(mv);

            return Ok(mv);
        }

        [HttpDelete("{id}")]

        public ActionResult<Movie> DeleteMovie(int id)
        {
            Movie mv = _movieRepository.Delete(id);
            if(mv == null)
            {
                return NotFound();
            }
            return Ok(mv);
        }
    }


}
