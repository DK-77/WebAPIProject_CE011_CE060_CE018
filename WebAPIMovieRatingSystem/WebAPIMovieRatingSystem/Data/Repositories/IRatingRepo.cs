using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPIMovieRatingSystem.Models;

namespace WebAPIMovieRatingSystem.Data.Repositories
{
    public interface IRatingRepository
    {
        Rating GetRating(int Id);

        IEnumerable<Rating> GetAllRatings();

        Rating Add(Rating rating);

        Rating Update(Rating rating_changes);

        Rating Delete(int Id);

        IList<Rating> GetRatingsOfUser(int id);

        IList<Rating> GetRatingsOfMovie(int id);

        Rating GetRatingOfUserOfMovie(int userid, int movieid);
    }

    public class SQLRatingRepository : IRatingRepository
    {
        private readonly MovieRatingDbContext context;

        public SQLRatingRepository(MovieRatingDbContext context)
        {
            this.context = context;
        }

        Rating IRatingRepository.GetRating(int Id)
        {
            return context.Ratings.Find(Id);
        }

        IEnumerable<Rating> IRatingRepository.GetAllRatings()
        {
            return context.Ratings;
        }

        Rating IRatingRepository.Add(Rating rating)
        {
            context.Ratings.Add(rating);
            context.SaveChanges();
            return rating;
        }

        Rating IRatingRepository.Delete(int Id)
        {
            Rating rating = context.Ratings.Find(Id);
            if (rating != null)
            {
                context.Ratings.Remove(rating);
                context.SaveChanges();
            }
            return rating;
        }

        Rating IRatingRepository.Update(Rating rating_changes)
        {
            var rating = context.Ratings.Attach(rating_changes);
            rating.State = EntityState.Modified;
            context.SaveChanges();
            return rating_changes;
        }

        IList<Rating> IRatingRepository.GetRatingsOfUser(int id)
        {
            return context.Ratings.Where(r => r.UserId == id).ToList();
        }

        Rating IRatingRepository.GetRatingOfUserOfMovie(int userid, int movieid)
        {
            return context.Ratings.Where(r => r.UserId == userid && r.MovieId == movieid).FirstOrDefault();
        }

        IList<Rating> IRatingRepository.GetRatingsOfMovie(int id) 
        {
            return context.Ratings.Where(r => r.MovieId == id).ToList();
        }
    }
}
