<%@ Application Language="C#" %>
<%@ Import Namespace="PersistentManager" %>
<%@ Import Namespace="System.Web" %>

<script runat="server">

    void Application_Start(object sender, EventArgs e) 
    {
        // Code that runs on application startup
        //string connectionString = ConfigurationManager.ConnectionStrings["SQLServer"].ConnectionString;
        //WebConfigurationFactory.GetInstance(connectionString);
        //WebConfigurationFactory.GetInstance().ProviderDialect = ProviderDialect.SqlProvider;

        //string connectionString = ConfigurationManager.ConnectionStrings["MySql"].ConnectionString;
        //WebConfigurationFactory.GetInstance(connectionString);
        //WebConfigurationFactory.GetInstance().ProviderDialect = ProviderDialect.MySQLProvider;

        string connectionString = ConfigurationManager.ConnectionStrings["MSAccess"].ConnectionString;
        connectionString = string.Format(connectionString, Server.MapPath(@"~\App_Data\demodatabase2.mdb"));
        WebConfigurationFactory.GetInstance(connectionString);
        WebConfigurationFactory.GetInstance().ProviderDialect = ProviderDialect.OleDbProvider;        
    }
    
    void Application_End(object sender, EventArgs e) 
    {
        //  Code that runs on application shutdown        
    }
        
    void Application_Error(object sender, EventArgs e) 
    { 
        // Code that runs when an unhandled error occurs

    }    

    void Session_Start(object sender, EventArgs e) 
    {
        // Code that runs when a new session is started

    }

    void Session_End(object sender, EventArgs e) 
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.

    }
       
</script>
