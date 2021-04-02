using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPIMovieRatingSystem.Models;

namespace WebAPIMovieRatingSystem.Data.Repositories
{
    public interface IMovieRepository
    {
        Movie GetMovie(int Id);

        IEnumerable<Movie> GetAllMovies();

        Movie Add(Movie movie);

        Movie Update(Movie movie_changes);

        Movie Delete(int Id);
    }

    public class SQLMovieRepository : IMovieRepository
    {
        private readonly MovieRatingDbContext context;

        public SQLMovieRepository(MovieRatingDbContext context)
        {
            this.context = context;
        }

        Movie IMovieRepository.GetMovie(int Id)
        {
            return context.Movies.Find(Id);
        }

        IEnumerable<Movie> IMovieRepository.GetAllMovies()
        {
            return context.Movies;
        }

        Movie IMovieRepository.Add(Movie movie)
        {
            context.Movies.Add(movie);
            context.SaveChanges();
            return movie;
        }

        Movie IMovieRepository.Delete(int Id)
        {
            Movie movie = context.Movies.Find(Id);
            if (movie != null)
            {
                context.Movies.Remove(movie);
                context.SaveChanges();
            }
            return movie;
        }

        Movie IMovieRepository.Update(Movie movie_changes)
        {
            var movie = context.Movies.Attach(movie_changes);
            movie.State = EntityState.Modified;
            context.SaveChanges();
            return movie_changes;
        }


    }
}
