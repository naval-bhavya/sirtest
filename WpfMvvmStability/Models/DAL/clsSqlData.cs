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
        ///  <summary> 
        ///Initializes ConnectionString for SQL DataBase Connection 
        ///</summary> 
        public static string GetConnectionString()
        {
            //Reading Sql connection string(Server name) from XML file
            if (ServerName == "")
            {
                ReadValuefromXml();
            }
          ConString = "Data Source=" + "localhost\\SQLEXPRESS01" + ";Initial Catalog=StabilityP15B;Integrated Security=SSPI";
       
         
        
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
               string Con = "Data Source=" + "localhost\\SQLEXPRESS01" + ";Initial Catalog=StabilityP15B;Integrated Security=SSPI";
            
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
                XPathDocument document = new XPathDocument(path);
                XPathNavigator navigator = document.CreateNavigator();
                XPathNodeIterator node = navigator.Select("/Settings/ServerName");
                foreach (XPathNavigator item in node)
                {
                    ServerName = item.Value;
                }
            }
            catch
            {
            }
        }
    }
}