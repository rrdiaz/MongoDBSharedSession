using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MongoDBWeb
{
    public partial class Provider : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["KeyFromASPNET"] = "1234net";
            //Session["KeyFromASPNET1"] = "JALKJALSJAKSLJALSKJALKSJKLAJLSJASLKS";
            //Session["KeyFromASPNET2"] = "JALKJALSJAKSLJALSKJALKSJKLAJLSJASLKS";
            //Session["KeyFromASPNET3"] = "JALKJALSJAKSLJALSKJALKSJKLAJLSJASLKS";
            //Session["KeyFromASPNET4"] = "JALKJALSJAKSLJALSKJALKSJKLAJLSJASLKS";
            //Session["KeyFromASPNET5"] = "JALKJALSJAKSLJALSKJALKSJKLAJLSJASLKS";
            //Session["KeyFromASPNET6"] = "JALKJALSJAKSLJALSKJALKSJKLAJLSJASLKS";
            //Session["KeyFromASPNET7"] = "JALKJALSJAKSLJALSKJALKSJKLAJLSJASLKS";

            this.IDSESSION.Text = Session.SessionID;
            
        }
    }
}