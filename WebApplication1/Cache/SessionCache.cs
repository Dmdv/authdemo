using AuthDemo.Models;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Security.Principal;

namespace AuthDemo.Cache
{
    public static class SessionCache
    {
        private const string CacheKey = "loggedinusers";

        public static void Remove(IPrincipal user)
        {
            var users = GetLoggedInUsers();
            users.Remove(user.Identity.GetUserId());
        }

        public static Dictionary<string, ApplicationUser> GetLoggedInUsers()
        {
            var loggedInUsers = new Dictionary<string, ApplicationUser>();

            var cache = MemoryCache.Default;

            if (cache.Contains(CacheKey))
            {
                loggedInUsers = cache[CacheKey] as Dictionary<string, ApplicationUser>;
            }
            else
            {
                cache[CacheKey] = loggedInUsers;
            }

            return loggedInUsers;
        }
    }
}