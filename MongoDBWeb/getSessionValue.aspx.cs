using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MongoDBWeb
{
    public partial class getSessionValue : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            Response.Write(string.Format("SessionID = {0}", Session.SessionID));
            Response.Write(string.Format("Variable = {0}", Session["KeyFromASP"]));
            Response.Write(string.Format("Variable = {0}", Session["KeyFromASPNET"]));
        }
    }
}