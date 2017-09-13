using AuthDemo.Cache;
using AuthDemo.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Linq;
using System.Web.Mvc;

namespace AuthDemo.Controllers
{
    public class UserController : Controller
    {
        public bool IsAdminUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity;
                using (var context = new ApplicationDbContext())
                {
                    var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                    var s = userManager.GetRoles(user.GetUserId());
                    return s.Contains(Constants.Admin);
                }
            }

            return false;
        }


        // GET: User
        //[Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity;

                ViewBag.Name = user.Name;
                ViewBag.IsAdmin = false;

                if (IsAdminUser())
                {
                    ViewBag.IsAdmin = true;
                }

                return View(SessionCache.GetLoggedInUsers().Select(x => x.Value));
            }

            ViewBag.Name = "Not LoggedIn";

            return View();
        }

        public ActionResult Revoke()
        {
            return RedirectToAction("Index");
        }
    }
}