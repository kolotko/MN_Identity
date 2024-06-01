using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using WebFormsIdentity.Repositories;

namespace WebFormsIdentity
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Hooks.UserRepositoryHook.Repository = new UserRepository();
            Hooks.PasswordHasherHook.Hasher = new PasswordHasher();
            AddAdminIfNotExist();
        }

        private void AddAdminIfNotExist()
        {
            try
            {
                Hooks.UserRepositoryHook.Repository.GetUserPassHashAandSalt("admin");
                return;
            }
            catch (Exception e)
            {
                //brak użytkownika
            }

            var password = "password";
            var (hashedPassword, salt) = Hooks.PasswordHasherHook.Hasher.HashPassword(password);
            Hooks.UserRepositoryHook.Repository.CreateUser("admin", hashedPassword, salt);
            
        }
    }
}