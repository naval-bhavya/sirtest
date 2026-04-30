using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.Common;
using System.Data;
using System.Xml;

namespace WpfMvvmStability.Views
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        string configPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\Settings\StabilityConfig.xml";
        
        public SettingsWindow()
        {
            InitializeComponent();
            LoadConfig();
        }
        
        private void LoadConfig()
        {
            try
            {
                if (System.IO.File.Exists(configPath))
                {
                    System.Xml.XmlDocument xml = new System.Xml.XmlDocument();
                    xml.Load(configPath);
                    
                    XmlNode serverNode = xml.SelectSingleNode("/Settings/ServerName");
                    if (serverNode != null) txtServer.Text = serverNode.InnerText;
                    
                    XmlNode userNode = xml.SelectSingleNode("/Settings/UserId");
                    if (userNode != null) txtUser.Text = userNode.InnerText;
                    
                    XmlNode passNode = xml.SelectSingleNode("/Settings/Password");
                    if (passNode != null) txtPassword.Password = passNode.InnerText;
                    
                    XmlNode dbNode = xml.SelectSingleNode("/Settings/Database");
                    if (dbNode != null) txtDatabase.Text = dbNode.InnerText;
                }
            }
            catch { }
        }
        
        private void SaveConfig()
        {
            try
            {
                System.Xml.XmlDocument xml = new System.Xml.XmlDocument();
                
                if (System.IO.File.Exists(configPath))
                {
                    xml.Load(configPath);
                }
                else
                {
                    System.Xml.XmlDeclaration declaration = xml.CreateXmlDeclaration("1.0", "UTF-8", "yes");
                    xml.AppendChild(declaration);
                    System.Xml.XmlComment comment = xml.CreateComment("This is an XML Generated File");
                    xml.AppendChild(comment);
                    System.Xml.XmlElement settingsRoot = xml.CreateElement("Settings");
                    xml.AppendChild(settingsRoot);
                }
                
                System.Xml.XmlNode rootNode = xml.SelectSingleNode("/Settings");
                if (rootNode == null)
                {
                    rootNode = xml.CreateElement("Settings");
                    xml.AppendChild(rootNode);
                }
                
                SetNodeValue(xml, "ServerName", txtServer.Text);
                SetNodeValue(xml, "UserId", txtUser.Text);
                SetNodeValue(xml, "Password", txtPassword.Password);
                SetNodeValue(xml, "Database", txtDatabase.Text);
                
                xml.Save(configPath);
                MessageBox.Show("Configuration saved successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving config: " + ex.Message);
            }
        }
        
        private void SetNodeValue(System.Xml.XmlDocument xml, string nodeName, string value)
        {
            System.Xml.XmlNode node = xml.SelectSingleNode("/Settings/" + nodeName);
            if (node == null)
            {
                node = xml.CreateElement(nodeName);
                xml.DocumentElement.AppendChild(node);
            }
            node.InnerText = value;
        }

        private void btnResStabilitycal_Click(object sender, RoutedEventArgs e)
        {
            DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
            string Err = "";
            string query = "update [tblStability_CalculationStatus] set [Stability_Calculation_Status]= 0" ;
            command.CommandText = query;
            command.CommandType = CommandType.Text;
            int res= Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);
            if (res > 0)
            {
                MessageBox.Show("Stability Calculation Updated Successfully");
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
            string Err = "";
            string query = " update [tblMaster_Config] set [Active]='" + cmbHighLow.Text + "' , [Rise_Fall] ='"+ txtFldRate.Text+"'   ";
            command.CommandText = query;
            command.CommandType = CommandType.Text;
            int res = Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);
            if (res > 0)
            {
                MessageBox.Show("Records Updated Successfully");
            }
        }

        private void lblStabilitySettings_Loaded(object sender, RoutedEventArgs e)
        {
           
            DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
             string Err = "";
            string query = "select  [Active], [Rise_Fall] from [tblMaster_Config]";
            command.CommandText = query;
            command.CommandType = CommandType.Text;
            DataTable dt = Models.DAL.clsDBUtilityMethods.GetTable(command, Err);
            if (dt.Rows[0]["Active"].ToString()=="False")
            {
                cmbHighLow.Text = Convert.ToString (0);
            }
            else
            {
                cmbHighLow.Text = Convert.ToString(1);
            }
            
            ;
            txtFldRate.Text = dt.Rows[0]["Rise_Fall"].ToString();   
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        
        private void btnSaveConfig_Click(object sender, RoutedEventArgs e)
        {
            SaveConfig();
        }
        
        private void btnTestConn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string connStr = $"Data Source={txtServer.Text};Initial Catalog={txtDatabase.Text};User ID={txtUser.Text};Password={txtPassword.Password};Encrypt=False;TrustServerCertificate=True;Connection Timeout=5";
                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connStr))
                {
                    conn.Open();
                    MessageBox.Show("Connection successful!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection failed: " + ex.Message);
            }
        }

       
    }
}
