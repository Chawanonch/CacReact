using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Entities;
using Microsoft.AspNetCore.Identity;

namespace Api.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(StoreContext context, UserManager<User> userManager)
        {
            #region Identityสร้างข้อมูล User
            if (!userManager.Users.Any())
            {
                var user = new User
                {
                    UserName = "chawanon",
                    Email = "chawanon@test.com"
                };
                await userManager.CreateAsync(user, "Pa$$w0rd"); //ทาการ hash Password
                await userManager.AddToRoleAsync(user, "Member"); // มีRole เดียว
                var admin = new User
                {
                    UserName = "admin",
                    Email = "admin@test.com"
                };
                await userManager.CreateAsync(admin, "Pa$$w0rd"); //ทาการ hash Password
                await userManager.AddToRolesAsync(admin, new[] { "Member", "Admin" }); //มีหลายRoles
            }
            #endregion
            
            context.SaveChanges();
        }
        
    }
}