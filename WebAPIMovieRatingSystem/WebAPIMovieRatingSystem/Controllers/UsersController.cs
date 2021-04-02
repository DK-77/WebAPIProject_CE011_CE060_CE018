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
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IMovieRepository _movieRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRatingRepository _ratingRepository;
        public UsersController(ILogger<UsersController> logger, IMovieRepository movieRepository, IUserRepository userRepository, IRatingRepository ratingRepository)
        {
            _logger = logger;
            _movieRepository = movieRepository;
            _userRepository = userRepository;
            _ratingRepository = ratingRepository;
        }
        

        [HttpGet]
        public ActionResult<IEnumerable<User>> GetAllUsers()
        {
            var users = _userRepository.GetAllUsers();
            if (users == null)
            {
                return NotFound();
            }
            return Ok(users);
        }

        [HttpGet("{id}")]
        public ActionResult<User> GetUserById(int id)
        {
            var user = _userRepository.GetUser(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost]
        public ActionResult<User> CreateUser(User user)
        {
            User usr = new User();
            usr.Email = user.Email;
            usr.Password = user.Password;
            usr.Ratings = user.Ratings;
            usr = _userRepository.Add(usr);
            return Ok(usr);
        }

        [HttpPut("{id}")]
        public ActionResult<User> UpdateUser(int id, User user)
        {
            User usr = _userRepository.GetUser(id);
            if (usr == null)
            {
                return BadRequest();
            }
            usr.Email = user.Email;
            usr.Password = user.Password;
            usr.Ratings = user.Ratings;
            usr = _userRepository.Update(usr);

            return Ok(usr);
        }

        [HttpDelete("{id}")]

        public ActionResult<User> DeleteUser(int id)
        {
            User usr = _userRepository.Delete(id);
            if (usr == null)
            {
                return NotFound();
            }
            return Ok(usr);
        }

        
        

    }
}
