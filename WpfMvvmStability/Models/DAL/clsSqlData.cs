using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Xml.XPath;
using System.Data.SqlClient;
using System.Configuration;
using System.Reflection;

namespace WpfMvvmStability.Models.DAL
{
    ///  <summary> 
    ///Class for reading SQL Server Name from StabilityConfig.xml,SQL connection , Connection String and DBFactory
    ///</summary> 
    public class clsSqlData
    {
        static string ServerName = "", ConString;
        static string UserId = "navalarchgroup";
        static string Password = "navalarch26";
        static string Database = "StabilityP15B";
        
        ///  <summary> 
        ///Initializes ConnectionString for SQL DataBase Connection 
        ///</summary> 
        public static string GetConnectionString()
        {
            if (ServerName == "")
            {
                ReadValuefromXml();
            }
            
            if (string.IsNullOrEmpty(ServerName))
            {
                ServerName = FindLocalSqlServer();
            }
            
            ConString = $"Data Source={ServerName};Initial Catalog={Database};User ID={UserId};Password={Password};Encrypt=False;TrustServerCertificate=True;Connection Timeout=30";
       
         
        
            return ConString;
        }
        ///  <summary> 
        ///Initializes SQL Connection for SQL DataBase  
        ///</summary> 
        public static DbConnection GetConnection()
        {
            try
            {
                DbProviderFactory Dbfactory = clsSqlData.GetDbFactory();
                DbConnection conn = Dbfactory.CreateConnection();
              
                conn.ConnectionString = clsSqlData.GetConnectionString();
           
                return conn;

               
            }
            catch
            {
                throw new Exception("An exception has occured while creating the connection. Please check Connection String settings in the web.config file.");
            }

        }

        public static SqlConnection getConnection()
        {       
            SqlConnection connect = new SqlConnection();
            try
            {
                ReadValuefromXml();
                
                if (string.IsNullOrEmpty(ServerName))
                {
                    ServerName = FindLocalSqlServer();
                }
                
                string Con = $"Data Source={ServerName};Initial Catalog={Database};User ID={UserId};Password={Password};Encrypt=False;TrustServerCertificate=True;Connection Timeout=30";
             
                 connect.ConnectionString = Con;

            }
            catch
            {

            }
            return connect;
        }
        ///  <summary> 
        ///Initializes DBFactory for SQL/SQLite/mysql DataBase Connection 
        ///</summary> 
        public static DbProviderFactory GetDbFactory()
        {
            try
            {
                String ProviderName = ConfigurationManager.AppSettings.Get("ProviderName");
                DbProviderFactory Dbfactory = DbProviderFactories.GetFactory(ProviderName);
                return Dbfactory;
            }

            catch (DbException generatedExceptionName)
            {
                throw new Exception("An exception has occured while creating the database provider factory. Please check the ProviderName specified in the app.config file." + generatedExceptionName.Message);
            }

        }
        ///  <summary> 
        ///Reading SQL server Name from StabilityConfig.xml 
        ///</summary> 
        public static void ReadValuefromXml()
        {
            try
            {
                string path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Settings\\StabilityConfig.xml";
                if (System.IO.File.Exists(path))
                {
                    XPathDocument document = new XPathDocument(path);
                    XPathNavigator navigator = document.CreateNavigator();
                    XPathNodeIterator node = navigator.Select("/Settings/ServerName");
                    foreach (XPathNavigator item in node)
                    {
                        ServerName = item.Value;
                    }
                    
                    XPathNodeIterator userIdNode = navigator.Select("/Settings/UserId");
                    foreach (XPathNavigator item in userIdNode)
                    {
                        if (!string.IsNullOrEmpty(item.Value))
                            UserId = item.Value;
                    }
                    
                    XPathNodeIterator passwordNode = navigator.Select("/Settings/Password");
                    foreach (XPathNavigator item in passwordNode)
                    {
                        if (!string.IsNullOrEmpty(item.Value))
                            Password = item.Value;
                    }
                    
                    XPathNodeIterator databaseNode = navigator.Select("/Settings/Database");
                    foreach (XPathNavigator item in databaseNode)
                    {
                        if (!string.IsNullOrEmpty(item.Value))
                            Database = item.Value;
                    }
                }
            }
            catch
            {
            }
        }
        
        ///  <summary> 
        ///Automatically find ANY local SQL Server instance dynamically
        ///</summary> 
        public static string FindLocalSqlServer()
        {
            try
            {
                // First try standard local defaults for speed
                string[] quickDefaults = { ".", ".\\SQLEXPRESS", "localhost", Environment.MachineName };
                foreach (var server in quickDefaults)
                {
                    if (TestConnection(server)) return server;
                }

                // If defaults fail, scan the machine for ALL local SQL instances
                DataTable dt = System.Data.Sql.SqlDataSourceEnumerator.Instance.GetDataSources();
                foreach (DataRow row in dt.Rows)
                {
                    string serverName = row["ServerName"].ToString();
                    string instanceName = row["InstanceName"].ToString();
                    
                    string fullPath = string.IsNullOrEmpty(instanceName) ? serverName : $"{serverName}\\{instanceName}";
                    
                    // Test if this instance is local and has our database
                    if (TestConnection(fullPath)) return fullPath;
                }
            }
            catch { }
            
            return ".\\SQLEXPRESS"; // Final fallback
        }
        
        ///  <summary> 
        ///Test SQL Server connection
        ///</summary> 
        private static bool TestConnection(string server)
        {
            try
            {
                string testConn = $"Data Source={server};Initial Catalog=master;User ID={UserId};Password={Password};Encrypt=False;TrustServerCertificate=True;Connection Timeout=5";
                using (SqlConnection conn = new SqlConnection(testConn))
                {
                    conn.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
