using AuthDemo.Cache;
using AuthDemo.Controllers;
using System.Web.Mvc;

namespace AuthDemo.Filters
{
    public class RegisterUser : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

            var accountController = filterContext.Controller as AccountController;

            if (accountController?.LoggedUser != null)
            {
                var applicationUser = accountController.LoggedUser;

                var users = SessionCache.GetLoggedInUsers();

                if (users.ContainsKey(applicationUser.Id))
                {
                    users[applicationUser.Id] = applicationUser;
                }
                else
                {
                    users.Add(applicationUser.Id, applicationUser);
                }
            }
        }
    }
}