<%@ Application Language="C#" %>
<%@ Import Namespace="IRDB" %>
<%@ Import Namespace="System.Web.Optimization" %>
<%@ Import Namespace="System.Web.Routing" %>

using System.Web.Security;
using System.Security.Principal;

<script runat="server">

    void Application_Start(object sender, EventArgs e)
    {
        RouteConfig.RegisterRoutes(RouteTable.Routes);
        BundleConfig.RegisterBundles(BundleTable.Bundles);
    }

    // Added from https://support.microsoft.com/en-us/help/316748/how-to-authenticate-against-the-active-directory-by-using-forms-authen
    void Application_AuthenticateRequest(Object sender, EventArgs e)
    {
        String cookieName = FormsAuthentication.FormsCookieName;
        HttpCookie authCookie = Context.Request.Cookies[cookieName];

        if(null == authCookie)
        {//There is no authentication cookie.
            return;
        }

        FormsAuthenticationTicket authTicket = null;

        try
        {
            authTicket = FormsAuthentication.Decrypt(authCookie.Value);
        }
        catch(Exception ex)
        {
            //Write the exception to the Event Log.
            return;
        }

        if(null == authTicket)
        {//Cookie failed to decrypt.
            return;
        }

        //When the ticket was created, the UserData property was assigned a
        //pipe-delimited string of group names.
        String[] groups = authTicket.UserData.Split(new char[]{'|'});

        //Create an Identity.
        GenericIdentity id = new GenericIdentity(authTicket.Name, "LdapAuthentication");

        //This principal flows throughout the request.
        GenericPrincipal principal = new GenericPrincipal(id, groups);

        Context.User = principal;

    }

</script>

