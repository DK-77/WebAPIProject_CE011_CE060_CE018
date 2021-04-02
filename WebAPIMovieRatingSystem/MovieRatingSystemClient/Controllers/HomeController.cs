using System;

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MovieRatingSystemClient.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MovieRatingSystemClient.ViewModels;




namespace MovieRatingSystemClient.Controllers
{
    [Route("[controller]")]
    [Route("")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private static readonly HttpClient client = new HttpClient();
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> CreateRating()
        {

            return View();
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> Create(Rating rating)
        {
            if (ModelState.IsValid)
            {
                int id = Convert.ToInt32(TempData.Peek("userId"));
                Console.WriteLine(id);

                HttpRequestMessage fetch_rating = new HttpRequestMessage(HttpMethod.Get, "http://localhost:57881/api/Ratings/ratingsofuserofmovie/" + id + "/" + rating.MovieId);
                var response = await client.SendAsync(fetch_rating);

                if (response.StatusCode != System.Net.HttpStatusCode.NoContent)
                {
                    TempData["err"] = "Rating for Movie Already Provided, Please Edit Rating from List";
                    return Redirect("/Home/UserRatings");
                }

                HttpRequestMessage ratings_of_movie = new HttpRequestMessage(HttpMethod.Get, "http://localhost:57881/api/Ratings/ratingsofmovie/" + rating.MovieId);
                var res = await client.SendAsync(ratings_of_movie);

                if (res.IsSuccessStatusCode)
                {
                    var responseString = await res.Content.ReadAsStringAsync();
                    var ratings = JsonConvert.DeserializeObject<IEnumerable<Rating>>(responseString);
                    int num_of_ratings;

                    HttpRequestMessage fetch_movie = new HttpRequestMessage(HttpMethod.Get, "http://localhost:57881/api/Movie/" + rating.MovieId);
                    var result = await client.SendAsync(fetch_movie);

                    if (result.IsSuccessStatusCode)
                    {
                        responseString = await result.Content.ReadAsStringAsync();
                        var movie = JsonConvert.DeserializeObject<Movie>(responseString);
                        Movie updated_movie = new Movie();
                        Movie n_movie = new Movie()
                        {
                            MovieId = 1,
                            Name = "1",
                            Genre = "1",
                            CumulativeRating = 1.0
                        };

                        if (ratings == null)
                        {
                            num_of_ratings = 0;
                            
                            n_movie.MovieId = movie.MovieId;
                            n_movie.Name = movie.Name;
                            n_movie.Genre = movie.Genre;
                            n_movie.CumulativeRating = rating.UserProvidedRating;
                            updated_movie = n_movie;
                        }
                        else
                        {
                            num_of_ratings = ratings.Count();

                            double sum = (movie.CumulativeRating * num_of_ratings);
                            double new_sum = sum + rating.UserProvidedRating;
                            double new_cumulative_rating = new_sum / (num_of_ratings + 1);

                            n_movie.MovieId = movie.MovieId;
                            n_movie.Name = movie.Name;
                            n_movie.Genre = movie.Genre;
                            n_movie.CumulativeRating = new_cumulative_rating;

                            updated_movie = n_movie;
                        }

                        var updated_movie_Json = new StringContent(
                        System.Text.Json.JsonSerializer.Serialize(updated_movie),
                        Encoding.UTF8,
                        "application/json");

                        using var httpResponse = await client.PutAsync("http://localhost:57881/api/Movie/" + updated_movie.MovieId, updated_movie_Json);

                        httpResponse.EnsureSuccessStatusCode();

                    }
                }

                Rating new_rating = new Rating()
                {
                    UserProvidedRating = rating.UserProvidedRating,
                    MovieId = rating.MovieId,
                    UserId = id
                };

                var myContent = JsonConvert.SerializeObject(new_rating);
                var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var resp = client.PostAsync("http://localhost:57881/api/Ratings", byteContent).Result;

                return Redirect("/Home/UserRatings");

            }
            return View();
        }


        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> Movies()
        {
            HttpRequestMessage fetch_movies = new HttpRequestMessage(HttpMethod.Get, "http://localhost:57881/api/Movie");
            var response = await client.SendAsync(fetch_movies);
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var movies = JsonConvert.DeserializeObject<IEnumerable<Movie>>(responseString);
                List<Movie> movieList = movies.ToList();
                ViewBag.userId = TempData.Peek("userId") as string;
                return View(movieList);
            }
            else
            {
                List<Movie> movieList = new List<Movie>();
                return View(movieList);
            }
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> UserRatings()
        {
            int id = Convert.ToInt32(TempData.Peek("userId"));
            HttpRequestMessage fetch_ratings = new HttpRequestMessage(HttpMethod.Get, "http://localhost:57881/api/Ratings/ratingsofuser/" + id);
            var response = await client.SendAsync(fetch_ratings);
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var ratings = JsonConvert.DeserializeObject<IEnumerable<Rating>>(responseString);
                List<Rating> ratingList = ratings.ToList();
                var list = new List<Tuple<Rating, Movie>>();

                foreach (Rating rat in ratingList)
                {
                    int Id = Convert.ToInt32(rat.MovieId);
                    HttpRequestMessage fetch_movie = new HttpRequestMessage(HttpMethod.Get, "http://localhost:57881/api/Movie/" + Id);
                    var resp = await client.SendAsync(fetch_movie);

                    if (resp.IsSuccessStatusCode)
                    {
                        var respstring = await resp.Content.ReadAsStringAsync();
                        var movie = JsonConvert.DeserializeObject<Movie>(respstring);
                        list.Add(Tuple.Create(rat, movie));
                    }
                }

                List<UserRating> urlist = new List<UserRating>();
                foreach (var item in list)
                {
                    UserRating ur = new UserRating()
                    {
                        name = item.Item2.Name,
                        genre = item.Item2.Genre,
                        rating = item.Item1.UserProvidedRating,
                        ratingId = item.Item1.RatingId
                    };
                    urlist.Add(ur);
                }
                ViewBag.err = TempData.Peek("err");
                TempData["err"] = null;
                return View(urlist);
            }
            else
            {
                List<Rating> ratingList = new List<Rating>();
                return View(ratingList);
            }
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> EditRating(int id, string name, string genre)
        {
            HttpRequestMessage fetch_rating = new HttpRequestMessage(HttpMethod.Get, "http://localhost:57881/api/Ratings/" + id);
            var response = await client.SendAsync(fetch_rating);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var rating = JsonConvert.DeserializeObject<Rating>(responseString);

                UserRating ur = new UserRating()
                {
                    name = name,
                    genre = genre,
                    rating = rating.UserProvidedRating,
                    ratingId = rating.RatingId,
                };

                return View(ur);
            }
            else
            {
                UserRating ur = new UserRating();
                return View();
            }
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> Edit(UserRating userR)
        {
            if (ModelState.IsValid)
            {
                HttpRequestMessage fetch_rating = new HttpRequestMessage(HttpMethod.Get, "http://localhost:57881/api/Ratings/" + userR.ratingId);
                var response = await client.SendAsync(fetch_rating);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var rating = JsonConvert.DeserializeObject<Rating>(responseString);

                    Rating modified_rating = new Rating()
                    {
                        RatingId = rating.RatingId,
                        UserProvidedRating = userR.rating,
                        UserId = rating.UserId,
                        User = rating.User,
                        MovieId = rating.MovieId,
                        Movie = rating.Movie
                    };

                    var updated_rating_Json = new StringContent(
                        System.Text.Json.JsonSerializer.Serialize(modified_rating),
                        Encoding.UTF8,
                        "application/json");

                    using var httpResponse = await client.PutAsync("http://localhost:57881/api/Ratings/" + modified_rating.RatingId, updated_rating_Json);

                    httpResponse.EnsureSuccessStatusCode();


                    HttpRequestMessage fetch_movie = new HttpRequestMessage(HttpMethod.Get, "http://localhost:57881/api/Movie/" + rating.MovieId);
                    var result = await client.SendAsync(fetch_movie);

                    if (result.IsSuccessStatusCode)
                    {
                        responseString = await result.Content.ReadAsStringAsync();
                        var movie = JsonConvert.DeserializeObject<Movie>(responseString);

                        HttpRequestMessage ratings_of_movie = new HttpRequestMessage(HttpMethod.Get, "http://localhost:57881/api/Ratings/ratingsofmovie/" + rating.MovieId);
                        var res = await client.SendAsync(ratings_of_movie);

                        if (res.IsSuccessStatusCode)
                        {
                            var respString = await res.Content.ReadAsStringAsync();
                            var ratings = JsonConvert.DeserializeObject<IEnumerable<Rating>>(respString);
                            int num_of_ratings = ratings.Count();

                            double sum = movie.CumulativeRating * num_of_ratings;
                            double updated_sum = sum - rating.UserProvidedRating + userR.rating;
                            double new_cumulative_rating = updated_sum / num_of_ratings;

                            Movie updated_movie = new Movie();

                            updated_movie.CumulativeRating = new_cumulative_rating;
                            updated_movie.MovieId = movie.MovieId;
                            updated_movie.Name = movie.Name;
                            updated_movie.Genre = movie.Genre;

                            var updated_movie_Json = new StringContent(
                            System.Text.Json.JsonSerializer.Serialize(updated_movie),
                            Encoding.UTF8,
                            "application/json");

                            using var httpResp = await client.PutAsync("http://localhost:57881/api/Movie/" + updated_movie.MovieId, updated_movie_Json);

                            httpResp.EnsureSuccessStatusCode();

                        }
                    }


                    return Redirect("/Home/UserRatings");

                }
                else
                {
                    return View();
                }
            }
            return View();
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> DeleteRating(int id, string name, string genre)
        {
            HttpRequestMessage fetch_rating = new HttpRequestMessage(HttpMethod.Get, "http://localhost:57881/api/Ratings/" + id);
            var response = await client.SendAsync(fetch_rating);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var rating = JsonConvert.DeserializeObject<Rating>(responseString);

                HttpRequestMessage fetch_movie = new HttpRequestMessage(HttpMethod.Get, "http://localhost:57881/api/Movie/" + rating.MovieId);
                var result = await client.SendAsync(fetch_movie);

                if (result.IsSuccessStatusCode)
                {
                    responseString = await result.Content.ReadAsStringAsync();
                    var movie = JsonConvert.DeserializeObject<Movie>(responseString);

                    HttpRequestMessage ratings_of_movie = new HttpRequestMessage(HttpMethod.Get, "http://localhost:57881/api/Ratings/ratingsofmovie/" + rating.MovieId);
                    var res = await client.SendAsync(ratings_of_movie);

                    if (res.IsSuccessStatusCode)
                    {
                        var respString = await res.Content.ReadAsStringAsync();
                        var ratings = JsonConvert.DeserializeObject<IEnumerable<Rating>>(respString);
                        int num_of_ratings = ratings.Count();

                        Movie updated_movie = new Movie();

                        if (num_of_ratings == 1)
                        {
                            updated_movie.CumulativeRating = 0.0;
                            updated_movie.MovieId = movie.MovieId;
                            updated_movie.Name = movie.Name;
                            updated_movie.Genre = movie.Genre;
                        }
                        else
                        {
                            double sum = movie.CumulativeRating * num_of_ratings;
                            double updated_sum = sum - rating.UserProvidedRating;
                            double new_cumulative_rating = updated_sum / (num_of_ratings-1);

                            updated_movie.CumulativeRating = new_cumulative_rating;
                            updated_movie.MovieId = movie.MovieId;
                            updated_movie.Name = movie.Name;
                            updated_movie.Genre = movie.Genre;
                        }

                        var updated_movie_Json = new StringContent(
                            System.Text.Json.JsonSerializer.Serialize(updated_movie),
                            Encoding.UTF8,
                            "application/json");

                        using var httpResp = await client.PutAsync("http://localhost:57881/api/Movie/" + updated_movie.MovieId, updated_movie_Json);

                        httpResp.EnsureSuccessStatusCode();

                    }
                }
            }
            
            using var resp = await client.DeleteAsync("http://localhost:57881/api/Ratings/" + id);
            resp.EnsureSuccessStatusCode();

            return Redirect("/Home/UserRatings");
        }

        [Route("[action]")]
        [HttpGet]
        public IActionResult SignUp()
        {
            //TempData["err"] = "some error";
            ViewBag.title = "SignUp";
            ViewBag.err = TempData.Peek("err") as string;
            TempData["err"] = null;
            return View();
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> SignUp(User user)
        {
            if (ModelState.IsValid)
            {
                HttpRequestMessage msg1 = new HttpRequestMessage(HttpMethod.Get, "http://localhost:57881/api/users");
                var response1 = await client.SendAsync(msg1);
                if (response1.IsSuccessStatusCode)
                {
                    var responseStream = await response1.Content.ReadAsStringAsync();
                    var temp = JsonConvert.DeserializeObject<IEnumerable<User>>(responseStream);
                    List<User> userList = temp.ToList();
                    string errmsg;

                    if (userList != null)
                    {
                        //Console.WriteLine(user.Email);
                        foreach (User u in userList)
                        {
                            //Console.WriteLine(u.Email);
                            if (u.Email == user.Email)
                            {
                                errmsg = "Email already Registered";
                                TempData["err"] = errmsg;
                                //Console.WriteLine(TempData["err"]);
                                return Redirect("/Home/SignUp");
                            }
                        }
                    }

                    var myContent = JsonConvert.SerializeObject(user);
                    var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                    var byteContent = new ByteArrayContent(buffer);
                    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var new_user = client.PostAsync("http://localhost:57881/api/users", byteContent).Result;


                    var data = await new_user.Content.ReadAsStringAsync();
                    User usr = JsonConvert.DeserializeObject<User>(data);
                    TempData["userId"] = usr.UserId;
                    return Redirect("/Home/Movies");

                }

            }

            return View();
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Administration()
        {
            HttpRequestMessage msg1 = new HttpRequestMessage(HttpMethod.Get, "http://localhost:57881/api/Movie");
            var res = await client.SendAsync(msg1);

            if (res.IsSuccessStatusCode)
            {
                var respstring = await res.Content.ReadAsStringAsync();
                var movies = JsonConvert.DeserializeObject<IEnumerable<Movie>>(respstring);
                List<Movie> movieList = movies.ToList();
                ViewBag.err = TempData.Peek("err");
                TempData["err"] = null;
                return View(movieList);
            }
            else
            {
                List<Movie> movieList = new List<Movie>();
                ViewBag.err = TempData.Peek("err");
                TempData["err"] = null;
                return View(movieList);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> AddMovie()
        {
            return View();
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> AddMovie(Movie movie)
        {
            if (ModelState.IsValid)
            {
                HttpRequestMessage msg1 = new HttpRequestMessage(HttpMethod.Get, "http://localhost:57881/api/Movie");
                var res = await client.SendAsync(msg1);

                if (res.IsSuccessStatusCode)
                {
                    var respstring = await res.Content.ReadAsStringAsync();
                    var movies = JsonConvert.DeserializeObject<IEnumerable<Movie>>(respstring);
                    List<Movie> movieList = movies.ToList();

                    foreach (Movie m in movieList)
                    {
                        if (m.Name.ToLower() == movie.Name.ToLower())
                        {
                            TempData["err"] = "Movie alredy exists in the list.";
                            return Redirect("/Home/Administration");
                        }
                    }

                    Movie new_movie = new Movie()
                    {
                        Name = movie.Name,
                        Genre = movie.Genre,
                        CumulativeRating = movie.CumulativeRating
                    };

                    var myContent = JsonConvert.SerializeObject(new_movie);
                    var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                    var byteContent = new ByteArrayContent(buffer);
                    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var new_mov = client.PostAsync("http://localhost:57881/api/Movie", byteContent).Result;

                    return Redirect("/Home/Administration");

                }
            }
            return View();
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> EditMovie(int id, string name, string genre)
        {
            HttpRequestMessage msg1 = new HttpRequestMessage(HttpMethod.Get, "http://localhost:57881/api/Movie/" + id);
            var res = await client.SendAsync(msg1);

            if (res.IsSuccessStatusCode)
            {
                Console.WriteLine("Successful");
                var respstring = await res.Content.ReadAsStringAsync();
                var movie = JsonConvert.DeserializeObject<Movie>(respstring);
                UpdateMovie m = new UpdateMovie()
                {
                    MovieId = movie.MovieId,
                    Name = movie.Name,
                    Genre = movie.Genre,
                    CumulativeRating = movie.CumulativeRating
                };
                return View(m);

            }
            else
            {
                Console.WriteLine("Hello");
                return View();
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> EditMovie(UpdateMovie movie)
        {
            if (ModelState.IsValid)
            {


                Movie updated_movie = new Movie()
                {
                    MovieId = movie.MovieId,
                    Name = movie.Name,
                    Genre = movie.Genre,
                    CumulativeRating = movie.CumulativeRating
                };

                var updated_movie_Json = new StringContent(
                    System.Text.Json.JsonSerializer.Serialize(updated_movie),
                    Encoding.UTF8,
                    "application/json");

                using var httpResponse = await client.PutAsync("http://localhost:57881/api/Movie/" + updated_movie.MovieId, updated_movie_Json);

                httpResponse.EnsureSuccessStatusCode();

                return Redirect("/Home/Administration");

            }
            return View();
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> DeleteMovie(int id, string name, string genre)
        {
            using var response = await client.DeleteAsync("http://localhost:57881/api/Movie/" + id);
            response.EnsureSuccessStatusCode();

            return Redirect("/Home/Administration");
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> UserList()
        {
            HttpRequestMessage msg1 = new HttpRequestMessage(HttpMethod.Get, "http://localhost:57881/api/users");
            var res = await client.SendAsync(msg1);

            if (res.IsSuccessStatusCode)
            {
                var respstring = await res.Content.ReadAsStringAsync();
                var users = JsonConvert.DeserializeObject<IEnumerable<User>>(respstring);
                List<User> userList = users.ToList();

                return View(userList);
            }

            return View();

        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            using var response = await client.DeleteAsync("http://localhost:57881/api/users/" + id);
            response.EnsureSuccessStatusCode();

            return Redirect("/Home/UserList");
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ShowRatings(int id)
        {
            HttpRequestMessage fetch_ratings = new HttpRequestMessage(HttpMethod.Get, "http://localhost:57881/api/Ratings/ratingsofuser/" + id);
            var response = await client.SendAsync(fetch_ratings);
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var ratings = JsonConvert.DeserializeObject<IEnumerable<Rating>>(responseString);
                List<Rating> ratingList = ratings.ToList();
                var list = new List<Tuple<Rating, Movie>>();

                foreach (Rating rat in ratingList)
                {
                    int Id = Convert.ToInt32(rat.MovieId);
                    HttpRequestMessage fetch_movie = new HttpRequestMessage(HttpMethod.Get, "http://localhost:57881/api/Movie/" + Id);
                    var resp = await client.SendAsync(fetch_movie);

                    if (resp.IsSuccessStatusCode)
                    {
                        var respstring = await resp.Content.ReadAsStringAsync();
                        var movie = JsonConvert.DeserializeObject<Movie>(respstring);
                        list.Add(Tuple.Create(rat, movie));
                    }
                }

                List<UserRating> urlist = new List<UserRating>();
                foreach (var item in list)
                {
                    UserRating ur = new UserRating()
                    {
                        name = item.Item2.Name,
                        genre = item.Item2.Genre,
                        rating = item.Item1.UserProvidedRating,
                        ratingId = item.Item1.RatingId
                    };
                    urlist.Add(ur);
                }
                ViewBag.err = TempData.Peek("err");
                TempData["err"] = null;
                return View(urlist);
            }
            else
            {
                List<UserRating> urlist = new List<UserRating>();
                return View(urlist);
            }
        }

        [HttpGet]
        [Route("[action]")]
        [Route("")]
        public IActionResult Login()
        {
            ViewBag.err = TempData.Peek("err") as string;
            TempData["err"] = null;
            return View();
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Login(string email, string password)
        {
            HttpRequestMessage msg1 = new HttpRequestMessage(HttpMethod.Get, "http://localhost:57881/api/users");
            HttpRequestMessage msg2 = new HttpRequestMessage(HttpMethod.Get, "http://localhost:57881/api/admins");
            var response1 = await client.SendAsync(msg1);
            var response2 = await client.SendAsync(msg2);

            if (response1.IsSuccessStatusCode)
            {
                var responseStream = await response1.Content.ReadAsStringAsync();
                var temp = JsonConvert.DeserializeObject<IEnumerable<User>>(responseStream);
                List<User> userList = temp.ToList();                 
                
                foreach (User user in userList)
                {
                    if (user.Email == email && user.Password == password)
                    {
                        TempData["userId"] = user.UserId;
                        return Redirect("/Home/Movies");
                    }
                }
            }
            if (response2.IsSuccessStatusCode)
            {
                var responseStream = await response2.Content.ReadAsStringAsync();
                var temp = JsonConvert.DeserializeObject<IEnumerable<Admin>>(responseStream);
                List<Admin> adminList = temp.ToList();
                foreach (Admin admin in adminList)
                {
                    if (admin.Email == email && admin.Password == password)
                    {
                        TempData["userId"] = admin.AdminId;
                        return Redirect("/Home/Administration");
                    }

                }
            }

            string errmsg = "Invalid Credentials";
            TempData["err"] = errmsg;
            return Redirect("/Home/Login");
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult LogOut()
        {
            TempData["userId"] = null;
            return Redirect("/Home/Login");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
