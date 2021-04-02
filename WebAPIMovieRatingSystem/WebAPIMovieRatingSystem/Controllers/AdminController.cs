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
    [Route("api/admins")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IMovieRepository _movieRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRatingRepository _ratingRepository;
        private readonly IAdminRepository _adminRepository;
        public AdminController(ILogger<AdminController> logger, IMovieRepository movieRepository, IUserRepository userRepository, IRatingRepository ratingRepository, IAdminRepository adminRepository)
        {
            _logger = logger;
            _movieRepository = movieRepository;
            _userRepository = userRepository;
            _ratingRepository = ratingRepository;
            _adminRepository = adminRepository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Admin>> GetAllAdmins()
        {
            var admins = _adminRepository.GetAllAdmins();
            if (admins == null)
            {
                return NotFound();
            }
            return Ok(admins);
        }
    }
}
