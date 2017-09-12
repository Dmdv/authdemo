using System;
using System.Collections.Generic;
using System.Web.Configuration;
using AuthDemo.Models;

namespace AuthDemo
{
    public class UsersConfig : IDisposable
    {
        private UsersConfig()
        {
            Users.Add(
                new User
                {
                    Name = WebConfigurationManager.AppSettings["SuperUser:Login"],
                    Password = WebConfigurationManager.AppSettings["SuperUser:Password"],
                    IsAdmin = true
                });

            Users.Add(
                new User
                {
                    Name = WebConfigurationManager.AppSettings["User1:Login"],
                    Password = WebConfigurationManager.AppSettings["User1:Password"]
                });

            Users.Add(
                new User
                {
                    Name = WebConfigurationManager.AppSettings["User2:Login"],
                    Password = WebConfigurationManager.AppSettings["User2:Password"]
                });
        }

        public List<User> Users { get; set; } = new List<User>();

        public void Dispose()
        {
        }

        public static UsersConfig Create()
        {
            return new UsersConfig();
        }
    }
}