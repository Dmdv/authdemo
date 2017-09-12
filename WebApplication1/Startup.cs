using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using System.Linq;
using AuthDemo.Models;
using AuthDemo;

[assembly: OwinStartup(typeof(Startup))]

namespace AuthDemo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateRolesandUsers();
        }

        private void CreateRolesandUsers()
        {
            using (var context = new ApplicationDbContext())
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

                var usersConfig = UsersConfig.Create();

                if (!roleManager.RoleExists(Constants.Admin))
                {
                    roleManager.Create(new IdentityRole {Name = Constants.Admin});

                    var userConfig = usersConfig.Users.First(x => x.IsAdmin);

                    CreateUser(userConfig, userManager, Constants.Admin);
                }

                if (!roleManager.RoleExists(Constants.User))
                {
                    roleManager.Create(new IdentityRole {Name = Constants.User});

                    foreach (var userConfig in usersConfig.Users.Where(x => !x.IsAdmin))
                    {
                        CreateUser(userConfig, userManager, Constants.User);
                    }
                }
            }
        }

        private static void CreateUser(User userConfig, UserManager<ApplicationUser, string> userManager, string role)
        {
            var user = new ApplicationUser
            {
                UserName = userConfig.Name
            };

            var chkUser = userManager.Create(user, userConfig.Password);

            if (chkUser.Succeeded)
            {
                userManager.AddToRole(user.Id, role);
            }
        }
    }
}