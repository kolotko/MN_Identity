using System;
using System.Security.Cryptography;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebFormsIdentity
{
    public partial class Login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
 
        }
        
        protected void Login1_Authenticate(object sender, AuthenticateEventArgs e)
        {
            var loginControl = (System.Web.UI.WebControls.Login)sender;
            string username = loginControl.UserName;
            string password = loginControl.Password;

            try
            {
                var result = Hooks.UserRepositoryHook.Repository.GetUserPassHashAandSalt("admin");
                var isPasswordOk = Hooks.PasswordHasherHook.Hasher.VerifyPassword(password, result.hashedPassword, result.salt);
                if (isPasswordOk)
                {
                    e.Authenticated = true;
                    FormsAuthentication.SetAuthCookie(username, false);
                    Session["Uprawnienie"] = "przykladowe";
                    return;
                }
                
                Session.Clear();
                e.Authenticated = false;
            }
            catch (Exception exception)
            {
                Session.Clear();
                e.Authenticated = false;
            }
        }
    }
}