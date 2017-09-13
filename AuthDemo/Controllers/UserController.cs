using AuthDemo.Cache;
using AuthDemo.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AuthDemo.Controllers
{
    public class UserController : Controller
    {
        private ApplicationUserManager _userManager;

        public UserController()
        {
        }

        public UserController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _userManager = value; }
        }


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

            ViewBag.IsAdmin = false;
            ViewBag.Name = "Not LoggedIn";

            return View();
        }

        public async Task<ActionResult> Revoke(string id)
        {
            var identityResult = await UserManager.UpdateSecurityStampAsync(id);
            if (identityResult.Succeeded)
            {
                SessionCache.Remove(id);
            }

            return RedirectToAction("Index");
        }
    }
}