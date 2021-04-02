using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPIMovieRatingSystem.Models;

namespace WebAPIMovieRatingSystem.Data.Repositories
{
        public interface IUserRepository
        {
            User GetUser(int Id);

            IEnumerable<User> GetAllUsers();

            User Add(User user);

            User Update(User user_changes);

            User Delete(int Id);



        }

        public class SQLUserRepository : IUserRepository
        {
            private readonly MovieRatingDbContext context;

            public SQLUserRepository(MovieRatingDbContext context)
            {
                this.context = context;
            }

            User IUserRepository.GetUser(int Id)
            {
                return context.Users.Find(Id);
            }

            IEnumerable<User> IUserRepository.GetAllUsers()
            {
                return context.Users;
            }

            User IUserRepository.Add(User user)
            {
                context.Users.Add(user);
                context.SaveChanges();
                return user;
            }

            User IUserRepository.Delete(int Id)
            {
                User user = context.Users.Find(Id);
                if (user != null)
                {
                    context.Users.Remove(user);
                    context.SaveChanges();
                }
                return user;
            }

            User IUserRepository.Update(User user_changes)
            {
                var user = context.Users.Attach(user_changes);
                user.State = EntityState.Modified;
                context.SaveChanges();
                return user_changes;
            }


        }
    
}
