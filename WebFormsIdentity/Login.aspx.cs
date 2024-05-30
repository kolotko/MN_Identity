using System;
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
            
            if (username == "admin" && password == "password")
            {
                e.Authenticated = true;
                FormsAuthentication.SetAuthCookie(username, false);
            }
            else
            {
                e.Authenticated = false;
            }
        }
    }
}