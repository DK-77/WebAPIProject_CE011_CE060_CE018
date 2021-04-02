using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPIMovieRatingSystem.Models;

namespace WebAPIMovieRatingSystem.Data.Repositories
{
    public interface IAdminRepository
    {
        Admin GetAdmin(int Id);

        IEnumerable<Admin> GetAllAdmins();

        Admin Add(Admin admin);

        Admin Update(Admin admin_changes);

        Admin Delete(int Id);
    }

    public class SQLAdminRepository : IAdminRepository
    {
        private readonly MovieRatingDbContext context;

        public SQLAdminRepository(MovieRatingDbContext context)
        {
            this.context = context;
        }

        Admin IAdminRepository.GetAdmin(int Id)
        {
            return context.Admins.Find(Id);
        }

        IEnumerable<Admin> IAdminRepository.GetAllAdmins()
        {
            return context.Admins;
        }

        Admin IAdminRepository.Add(Admin admin)
        {
            context.Admins.Add(admin);
            context.SaveChanges();
            return admin;
        }

        Admin IAdminRepository.Update(Admin admin_changes)
        {
            var admin = context.Admins.Attach(admin_changes);
            admin.State = EntityState.Modified;
            context.SaveChanges();
            return admin_changes;
        }

        Admin IAdminRepository.Delete(int Id)
        {
            Admin admin = context.Admins.Find(Id);
            if (admin != null)
            {
                context.Admins.Remove(admin);
                context.SaveChanges();

            }
            return admin;
        }
    }
}
