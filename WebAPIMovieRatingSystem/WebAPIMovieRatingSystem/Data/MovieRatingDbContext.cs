using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPIMovieRatingSystem.Models;

namespace WebAPIMovieRatingSystem.Data
{
    public class MovieRatingDbContext : DbContext
    {
        public MovieRatingDbContext(DbContextOptions<MovieRatingDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Movie> Movies { get; set; }

        public DbSet<Rating> Ratings { get; set; }

        public DbSet<Admin> Admins { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder ob)
        {

        }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            //one movie has many ratings
            mb.Entity<Rating>()
                .HasOne<Movie>(i => i.Movie)
                .WithMany(p => p.Ratings)
                .HasForeignKey(i => i.MovieId).OnDelete(DeleteBehavior.Cascade);

            //one user has many ratings
            mb.Entity<Rating>()
                .HasOne<User>(i => i.User)
                .WithMany(p => p.Ratings)
                .HasForeignKey(i => i.UserId).OnDelete(DeleteBehavior.Cascade);

        }

    }
}
