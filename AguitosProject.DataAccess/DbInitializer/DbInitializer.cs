using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SummaBook.DataAccess.Data;
using SummaBook.Models;
using SummaBook.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SummaBook.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public DbInitializer(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext db)
        {
            _db = db;
            _userManager = userManager; 
            _roleManager = roleManager;
        }

        public void Initialize()
        {
            //migration if they are not applied
            try
            {
                if(_db.Database.GetPendingMigrations().Count() > 0) { 
                    _db.Database.Migrate();
                }
            }
            catch(Exception ex) {}

            //create roles if they are not created
            if (!_roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Company)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();

                //If roles are not created, then we will create admin user as well
                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "admin@test.com",
                    Email = "admin@test.com",
                    Name = "ADMIN TEST",
                    PhoneNumber = "5555555",
                    StreetAddress = "test 123 Ave.",
                    State = "IL",
                    PostalCode = "12345",
                    City = "Chicago",


                }, "Admin123*").GetAwaiter().GetResult();

                ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "admin@test.com");
                _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
            }

            return;
        }
    }
}
