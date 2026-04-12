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
using System.Data.SqlClient;
using System.Data.Sql;
using System.Xml;
using System.IO;
using Microsoft.Win32;
using System.Reflection;
using System.Xml.Linq;


namespace WpfMvvmStability.Views
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : Window
    {
        private SqlConnection conn;
        private SqlCommand command;
        private SqlDataReader reader;
        string sql = "";
        string connectionstring = "";
        string connectionstring1 = "";
        private XmlDocument doc;
        string pwdCheck;
        string password;
        string user;
        private string PATH = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Settings\StabilityConfig.xml";
        public ConfigurationWindow()
        {
            InitializeComponent();
            radioButtonTCP.IsChecked = true;
            instances();
        }
        private void instances()
        {
            try
            {    //RegistryKey lm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default);
                //RegistryKey key = lm.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL");
                btnCreate.IsEnabled = false;
                //if (key != null)
                //{
                //    foreach (string s in key.GetValueNames())
                //        Console.WriteLine("localhost\\" + s);
                //    key.Close();
                //}

                //key = lm.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Microsoft SQL Server\Instance Names\SQL");

                //if (key != null)
                //{
                //    foreach (string s in key.GetValueNames())
                //        Console.WriteLine("localhost\\" + s);
                //    key.Close();
                //}
                //lm.Close();  
                //SOFTWARE\Microsoft\Microsoft SQL Server
                RegistryView registryView = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
                using (RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView))
                {
                    try
                    {
                        RegistryKey rk = hklm.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server");
                        if (rk != null)
                        {
                            String[] instances = (String[])rk.GetValue("InstalledInstances");
                            //String[] instances = (String[])rk.GetValueNames();
                            if (instances.Length > 0)
                            {
                                foreach (String element in instances)
                                {
                                    if (element == "MSSQLSERVER")
                                        listBoxSQLInstances.Items.Add(System.Environment.MachineName);
                                    else
                                        listBoxSQLInstances.Items.Add(System.Environment.MachineName + @"\" + element);
                                }
                            }
                            rk.Close();
                        }
                    }
                    catch
                    {

                    }

                    RegistryKey key = hklm.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Microsoft SQL Server\Instance Names\SQL");
                    //MessageBox.Show(key.ToString());
                    if (key != null)
                    {
                        // String[] instances = (String[])key.GetValue("InstalledInstances");
                        String[] instances = (String[])key.GetValueNames();
                        // MessageBox.Show(instances.Length.ToString());
                        if (instances.Length > 0)
                        {
                            foreach (String element in instances)
                            {
                                // MessageBox.Show(element);
                                if (element == "MSSQLSERVER")
                                    listBoxSQLInstances.Items.Add(System.Environment.MachineName);
                                else
                                    listBoxSQLInstances.Items.Add(System.Environment.MachineName + @"\" + element);
                            }
                        }
                        key.Close();
                    }
                    hklm.Close();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Backup Files(*.bak)|*.bak|All Files(*.*)|*.*";
            dlg.FilterIndex = 0;
            if (dlg.ShowDialog() == true)
            {
                textBoxRestorePath.Text = dlg.FileName;
            }
        }

        private void btnRestore_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                if (textBoxServerName.Text == "")
                {
                    MessageBox.Show("Please Enter Server Name");
                }
                else if (textBoxRestorePath.Text == "")
                {
                    MessageBox.Show("Please Select Database Path");
                }
                else
                {
                    conn = new SqlConnection(connectionstring);
                    conn.Open();
                    //sql= "IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'" + cmbDatabase.Text + "') DROP DATABASE " + cmbDatabase.Text + " RESTORE DATABASE " + cmbDatabase.Text + " FROM DISK = '" + txtRestoreFileLocation.Text + "'";
                    sql = "USE Master;";
                    sql += "Alter Database StabilityP15B Set OFFLINE WITH IMMEDIATE;";
                    sql += "Restore Database StabilityP15B FROM Disk = '" + textBoxRestorePath.Text + "' WITH REPLACE;";
                    command = new SqlCommand(sql, conn);
                    command.CommandTimeout = 500;
                    command.ExecuteNonQuery();
                    conn.Close();
                    conn.Dispose();
                    MessageBox.Show("Database Successfully Restored");
                    Mouse.OverrideCursor = null;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Mouse.OverrideCursor = null;
            }

        }

        private void btnConnectionString_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (textBoxServerName.Text == "")
                {
                    MessageBox.Show("Please Enter Server Name");
                }
                else if (textBoxUser.Text == "")
                {
                    MessageBox.Show("Please Enter User");
                }
                else if (textBoxPassword.Text == "")
                {
                    MessageBox.Show("Please Enter Password");
                }
                else
                {
                    XmlDocument doc = new XmlDocument();
                    //if (!System.IO.File.Exists(PATH))
                    //{
                    //Create neccessary nodes
                    XmlDeclaration declaration = doc.CreateXmlDeclaration("1.0", "UTF-8", "yes");
                    doc.AppendChild(declaration);
                    XmlComment comment = doc.CreateComment("This is an XML Generated File");
                    doc.AppendChild(comment);

                    XmlElement root = doc.CreateElement("Settings");
                    doc.AppendChild(root);

                    XmlElement ServerName = doc.CreateElement("ServerName");
                    ServerName.InnerText = textBoxServerName.Text;
                    root.AppendChild(ServerName);

                    //XmlElement User = doc.CreateElement("User");
                    //User.InnerText = textBoxUser.Text;
                    //root.AppendChild(User);

                    //XmlElement Password = doc.CreateElement("Password");
                    //Password.InnerText = textBoxPassword.Text;
                    //root.AppendChild(Password);

                    doc.Save(PATH);

                    MessageBox.Show("Configuration String Created");
                }
            }
            catch
            {
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                password = textBoxPassword.Text;
                user = textBoxUser.Text;
                connectionstring1 = "Data Source=" + listBoxSQLInstances.SelectedItem.ToString() + ";Initial Catalog=master;User ID=" + user + ";Password=" + password;
                // connectionstring = "Data Source=localhost;Integrated Security=True;";
                connectionstring = "Data Source=" + listBoxSQLInstances.SelectedItem.ToString() + ";Initial Catalog=StabilityP15B;User ID=StabilityP15B;Password=stabilityp15b";
                conn = new SqlConnection(connectionstring1);
                conn.Open();
                sql = "EXEC sp_databases";
                command = new SqlCommand(sql, conn);
                reader = command.ExecuteReader();

                listBoxSqlDatabases.Items.Clear();


                while (reader.Read())
                {

                    listBoxSqlDatabases.Items.Add(reader[0].ToString());
                }

                conn.Close();
                conn.Dispose();
                reader.Dispose();
                Mouse.OverrideCursor = null;
                btnCreate.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                Mouse.OverrideCursor = null;
            }

        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                conn = new SqlConnection(connectionstring1);
                conn.Open();
                sql = "";
                //sql= "IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'" + cmbDatabase.Text + "') DROP DATABASE " + cmbDatabase.Text + " RESTORE DATABASE " + cmbDatabase.Text + " FROM DISK = '" + txtRestoreFileLocation.Text + "'";
                sql = "USE Master;";
                //sql += "Alter Database StabilityP15B Set OFFLINE WITH ROLLBACK IMMEDIATE;";
                sql += "Create database StabilityP15B";
                command = new SqlCommand(sql, conn);
                command.CommandTimeout = 180;
                command.ExecuteNonQuery();
                conn.Close();
                conn.Dispose();
                Mouse.OverrideCursor = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Mouse.OverrideCursor = null;
            }
        }

        private void listBoxSQLInstances_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                textBoxServerName.Text = listBoxSQLInstances.SelectedItem.ToString();
            }
            catch
            {
                MessageBox.Show("Please select Instance");
            }

        }

        private void btnMasterLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                conn = new SqlConnection(connectionstring1);
                conn.Open();
                //sql= "IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'" + cmbDatabase.Text + "') DROP DATABASE " + cmbDatabase.Text + " RESTORE DATABASE " + cmbDatabase.Text + " FROM DISK = '" + txtRestoreFileLocation.Text + "'";
                sql = "USE Master;";
                sql += "IF  EXISTS (SELECT * FROM sys.server_principals WHERE name = N'StabilityP15B')";
                sql += "DROP LOGIN [StabilityP15B];";
                sql += "CREATE LOGIN [StabilityP15B] WITH PASSWORD=N'stabilityp15b', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF;";
                sql += "EXEC sys.sp_addsrvrolemember @loginame = N'StabilityP15B', @rolename = N'sysadmin';";
                sql += "USE Master;";
                sql += "EXEC master.dbo.sp_configure 'show advanced options', 1";
                sql += "RECONFIGURE WITH OVERRIDE;";
                sql += "EXEC master.dbo.sp_configure 'xp_cmdshell', 1";
                sql += "RECONFIGURE WITH OVERRIDE";
                command = new SqlCommand(sql, conn);
                command.CommandTimeout = 180;
                command.ExecuteNonQuery();
                conn.Close();
                conn.Dispose();
                MessageBox.Show("Master Login Created Successfully");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSaveToXml_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                //if (!System.IO.File.Exists(PATH))
                //{
                //Create neccessary nodes
                XmlDeclaration declaration = doc.CreateXmlDeclaration("1.0", "UTF-8", "yes");
                doc.AppendChild(declaration);
                XmlComment comment = doc.CreateComment("This is an XML Generated File");
                doc.AppendChild(comment);
                XmlElement root = doc.CreateElement("Settings");
                doc.AppendChild(root);

                XmlElement ModbusType = doc.CreateElement("MODBUSTYPE");
                if (radioButtonSerial.IsChecked == true)
                {
                    ModbusType.InnerText = Convert.ToString(0);
                }
                else if (radioButtonTCP.IsChecked == true)
                {
                    ModbusType.InnerText = Convert.ToString(1);
                }
                root.AppendChild(ModbusType);

                XmlElement ComPort = doc.CreateElement("PORT");
                if (radioButtonSerial.IsChecked == true)
                {
                    ComPort.InnerText = txtComPort.Text;
                }
                else if (radioButtonTCP.IsChecked == true)
                {
                    ComPort.InnerText = txtIP.Text;
                }
                root.AppendChild(ComPort);

                XmlElement ServerName = doc.CreateElement("ServerName");
                ServerName.InnerText = txtServerName.Text;
                root.AppendChild(ServerName);

                //XmlElement UserID = doc.CreateElement("UserID");
                //UserID.InnerText = textBox2.Text;
                //root.AppendChild(UserID);

                //XmlElement Password = doc.CreateElement("Password");
                //Password.InnerText = textBox3.Text;
                //root.AppendChild(Password);

                XmlElement Computers = doc.CreateElement("Computers");
                root.AppendChild(Computers);

                //XmlElement Computers = doc.CreateElement("Computers");
                XmlElement Computer = doc.CreateElement("Computer");
                Computers.AppendChild(Computer);

                XmlAttribute ComputerName = doc.CreateAttribute("ComputerName");
                ComputerName.InnerText = textBox4.Text;
                Computer.Attributes.Append(ComputerName);

                XmlAttribute DataBaseName = doc.CreateAttribute("DataBaseName");
                DataBaseName.InnerText = "Stability";
                Computer.Attributes.Append(DataBaseName);


                XmlAttribute ComputerIPAddress = doc.CreateAttribute("ComputerIPAddress");
                ComputerIPAddress.InnerText = textBox3.Text;
                Computer.Attributes.Append(ComputerIPAddress);

                XmlAttribute QueuePathName = doc.CreateAttribute("QueuePathName");
                QueuePathName.InnerText = @".\Private$\{0}";
                Computer.Attributes.Append(QueuePathName);


                XmlAttribute IsReplicationAllowed = doc.CreateAttribute("IsReplicationAllowed");
                IsReplicationAllowed.InnerText = textBox5.Text;
                Computer.Attributes.Append(IsReplicationAllowed);



                XmlAttribute TimeInterval = doc.CreateAttribute("TimeInterval");
                TimeInterval.InnerText = textBox6.Text;
                Computer.Attributes.Append(TimeInterval);


                XmlAttribute MulticastAddress = doc.CreateAttribute("MulticastAddress");
                MulticastAddress.InnerText = "234.1.1.1:8888";
                Computer.Attributes.Append(MulticastAddress);

                XmlElement Computer2 = doc.CreateElement("Computer");
                Computers.AppendChild(Computer2);

                XmlAttribute ComputerName2 = doc.CreateAttribute("ComputerName");
                ComputerName2.InnerText = textBox7.Text;
                Computer2.Attributes.Append(ComputerName2);

                XmlAttribute DataBaseName2 = doc.CreateAttribute("DataBaseName");
                DataBaseName2.InnerText = "Stability";
                Computer2.Attributes.Append(DataBaseName2);


                XmlAttribute ComputerIPAddress2 = doc.CreateAttribute("ComputerIPAddress");
                ComputerIPAddress2.InnerText = textBox8.Text;
                Computer2.Attributes.Append(ComputerIPAddress2);

                XmlAttribute QueuePathName2 = doc.CreateAttribute("QueuePathName");
                QueuePathName2.InnerText = @".\Private$\{0}";
                Computer2.Attributes.Append(QueuePathName2);


                XmlAttribute IsReplicationAllowed2 = doc.CreateAttribute("IsReplicationAllowed");
                IsReplicationAllowed2.InnerText = textBox9.Text;
                Computer2.Attributes.Append(IsReplicationAllowed2);



                XmlAttribute TimeInterval2 = doc.CreateAttribute("TimeInterval");
                TimeInterval2.InnerText = textBox10.Text;
                Computer2.Attributes.Append(TimeInterval2);


                XmlAttribute MulticastAddress2 = doc.CreateAttribute("MulticastAddress");
                MulticastAddress2.InnerText = "234.1.1.1:8888";
                Computer2.Attributes.Append(MulticastAddress2);

                XmlElement Computer3 = doc.CreateElement("Computer");
                Computers.AppendChild(Computer3);
                XmlAttribute ComputerName3 = doc.CreateAttribute("ComputerName");
                ComputerName3.InnerText = textBox11.Text;
                Computer3.Attributes.Append(ComputerName3);
                XmlAttribute DataBaseName3 = doc.CreateAttribute("DataBaseName");
                DataBaseName3.InnerText = "Stability";
                Computer3.Attributes.Append(DataBaseName3);
                XmlAttribute ComputerIPAddress3 = doc.CreateAttribute("ComputerIPAddress");
                ComputerIPAddress3.InnerText = textBox12.Text;
                Computer3.Attributes.Append(ComputerIPAddress3);
                XmlAttribute QueuePathName3 = doc.CreateAttribute("QueuePathName");
                QueuePathName3.InnerText = @".\Private$\{0}";
                Computer3.Attributes.Append(QueuePathName3);
                XmlAttribute IsReplicationAllowed3 = doc.CreateAttribute("IsReplicationAllowed");
                IsReplicationAllowed3.InnerText = textBox13.Text;
                Computer3.Attributes.Append(IsReplicationAllowed3);
                XmlAttribute TimeInterval3 = doc.CreateAttribute("TimeInterval");
                TimeInterval3.InnerText = textBox14.Text;
                Computer3.Attributes.Append(TimeInterval3);
                XmlAttribute MulticastAddress3 = doc.CreateAttribute("MulticastAddress");
                MulticastAddress3.InnerText = "234.1.1.1:8888";
                Computer3.Attributes.Append(MulticastAddress3);

                XmlElement Computer4 = doc.CreateElement("Computer");
                Computers.AppendChild(Computer4);
                XmlAttribute ComputerName4 = doc.CreateAttribute("ComputerName");
                ComputerName4.InnerText = textBox15.Text;
                Computer4.Attributes.Append(ComputerName4);
                XmlAttribute DataBaseName4 = doc.CreateAttribute("DataBaseName");
                DataBaseName4.InnerText = "Stability";
                Computer4.Attributes.Append(DataBaseName4);
                XmlAttribute ComputerIPAddress4 = doc.CreateAttribute("ComputerIPAddress");
                ComputerIPAddress4.InnerText = textBox16.Text;
                Computer4.Attributes.Append(ComputerIPAddress4);
                XmlAttribute QueuePathName4 = doc.CreateAttribute("QueuePathName");
                QueuePathName4.InnerText = @".\Private$\{0}";
                Computer4.Attributes.Append(QueuePathName4);
                XmlAttribute IsReplicationAllowed4 = doc.CreateAttribute("IsReplicationAllowed");
                IsReplicationAllowed4.InnerText = textBox17.Text;
                Computer4.Attributes.Append(IsReplicationAllowed4);
                XmlAttribute TimeInterval4 = doc.CreateAttribute("TimeInterval");
                TimeInterval4.InnerText = textBox18.Text;
                Computer4.Attributes.Append(TimeInterval4);
                XmlAttribute MulticastAddress4 = doc.CreateAttribute("MulticastAddress");
                MulticastAddress4.InnerText = "244.1.1.1:8888";
                Computer4.Attributes.Append(MulticastAddress4);

                XmlElement Computer5 = doc.CreateElement("Computer");
                Computers.AppendChild(Computer5);
                XmlAttribute ComputerName5 = doc.CreateAttribute("ComputerName");
                ComputerName5.InnerText = textBox19.Text;
                Computer5.Attributes.Append(ComputerName5);
                XmlAttribute DataBaseName5 = doc.CreateAttribute("DataBaseName");
                DataBaseName5.InnerText = "Stability";
                Computer5.Attributes.Append(DataBaseName5);
                XmlAttribute ComputerIPAddress5 = doc.CreateAttribute("ComputerIPAddress");
                ComputerIPAddress5.InnerText = textBox20.Text;
                Computer5.Attributes.Append(ComputerIPAddress5);
                XmlAttribute QueuePathName5 = doc.CreateAttribute("QueuePathName");
                QueuePathName5.InnerText = @".\Private$\{0}";
                Computer5.Attributes.Append(QueuePathName5);
                XmlAttribute IsReplicationAllowed5 = doc.CreateAttribute("IsReplicationAllowed");
                IsReplicationAllowed5.InnerText = textBox21.Text;
                Computer5.Attributes.Append(IsReplicationAllowed5);
                XmlAttribute TimeInterval5 = doc.CreateAttribute("TimeInterval");
                TimeInterval5.InnerText = textBox22.Text;
                Computer5.Attributes.Append(TimeInterval5);
                XmlAttribute MulticastAddress5 = doc.CreateAttribute("MulticastAddress");
                MulticastAddress5.InnerText = "255.1.1.1:8888";
                Computer5.Attributes.Append(MulticastAddress5);

                //XmlElement tables = doc.CreateElement("Tables");
                //root.AppendChild(tables);
                //XmlElement table = doc.CreateElement("Table");
                //tables.AppendChild(table);
                //XmlAttribute currTable = doc.CreateAttribute("CurrentTable");
                //currTable.InnerText = textBox11.Text;
                //table.Attributes.Append(currTable);
                //XmlAttribute histTable = doc.CreateAttribute("HistoryTable");
                //histTable.InnerText = textBox12.Text;
                //table.Attributes.Append(histTable);

                //XmlElement table2 = doc.CreateElement("Table");
                //tables.AppendChild(table2);
                //XmlAttribute currTable2 = doc.CreateAttribute("CurrentTable");
                //currTable2.InnerText = textBox18.Text;
                //table2.Attributes.Append(currTable2);
                //XmlAttribute histTable2 = doc.CreateAttribute("HistoryTable");
                //histTable2.InnerText = textBox19.Text;
                //table2.Attributes.Append(histTable2);

                //XmlElement table3 = doc.CreateElement("Table");
                //tables.AppendChild(table3);
                //XmlAttribute currTable3 = doc.CreateAttribute("CurrentTable");
                //currTable3.InnerText = textBox20.Text;
                //table3.Attributes.Append(currTable3);
                //XmlAttribute histTable3 = doc.CreateAttribute("HistoryTable");
                //histTable3.InnerText = textBox21.Text;
                //table3.Attributes.Append(histTable3);

                //XmlElement table4 = doc.CreateElement("Table");
                //tables.AppendChild(table4);
                //XmlAttribute currTable4 = doc.CreateAttribute("CurrentTable");
                //currTable4.InnerText = textBox22.Text;
                //table4.Attributes.Append(currTable4);
                //XmlAttribute histTable4 = doc.CreateAttribute("HistoryTable");
                //histTable4.InnerText = textBox23.Text;
                //table4.Attributes.Append(histTable4);

                //XmlElement table5 = doc.CreateElement("Table");
                //tables.AppendChild(table5);
                //XmlAttribute currTable5 = doc.CreateAttribute("CurrentTable");
                //currTable5.InnerText = textBox24.Text;
                //table5.Attributes.Append(currTable5);
                //XmlAttribute histTable5 = doc.CreateAttribute("HistoryTable");
                //histTable5.InnerText = textBox25.Text;
                //table5.Attributes.Append(histTable5);

                //XmlElement table6 = doc.CreateElement("Table");
                //tables.AppendChild(table6);
                //XmlAttribute currTable6 = doc.CreateAttribute("CurrentTable");
                //currTable6.InnerText = textBox26.Text;
                //table6.Attributes.Append(currTable6);
                //XmlAttribute histTable6 = doc.CreateAttribute("HistoryTable");
                //histTable6.InnerText = textBox27.Text;
                //table6.Attributes.Append(histTable6);

                doc.Save(PATH);

                MessageBox.Show("All Computers Added to Configuration String");
            }
            catch
            {
            }
        }

        private void radioButtonTCP_Checked(object sender, RoutedEventArgs e)
        {
            lblCOM.IsEnabled = false;
            txtComPort.IsEnabled = false;
            lblIP.IsEnabled = true;
            txtIP.IsEnabled = true;
        }

        private void radioButtonSerial_Checked(object sender, RoutedEventArgs e)
        {
            lblCOM.IsEnabled = true;
            txtComPort.IsEnabled = true;
            lblIP.IsEnabled = false;
            txtIP.IsEnabled = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var xml1 = XDocument.Load(PATH);

                XmlDocument xml = new XmlDocument();
                // You'll need to put the correct path to your xml file here
                xml.Load(PATH);
                XmlNode typeNode = xml.SelectSingleNode("Settings/MODBUSTYPE");
                // Get its value
                if (typeNode.InnerText == "0")
                {
                    radioButtonSerial.IsChecked = true;
                    XmlNode node = xml.SelectSingleNode("Settings/PORT");
                    // Get its value
                    txtComPort.Text = node.InnerText;
                }
                if (typeNode.InnerText == "1")
                {
                    radioButtonTCP.IsChecked = true;
                    XmlNode node = xml.SelectSingleNode("Settings/PORT");
                    // Get its value
                    txtIP.Text = node.InnerText;
                }

                XmlNode node1 = xml.SelectSingleNode("Settings/ServerName");
                txtServerName.Text = node1.InnerText;
                XmlNodeList detailNodes = xml.SelectNodes("Settings/Computers");
                foreach (XmlNode detail in detailNodes)
                {
                    string test = detail.InnerXml.ToString();
                    //textBox4.Text = detail.SelectSingleNode("Computer/@ComputerName").InnerText;
                    //textBox6.Text = detail.SelectSingleNode("Computer/@ComputerIPAddress").InnerText;
                    //textBox8.Text = detail.SelectSingleNode("Computer/@IsReplicationAllowed").InnerText;
                    //textBox9.Text = detail.SelectSingleNode("Computer/@TimeInterval").InnerText;

                }

                foreach (XElement element in xml1.Descendants("Settings"))
                {
                    int i = 1;
                    foreach (XElement childEllement in element.Descendants("Computers"))
                    {
                        foreach (var childEl in childEllement.Descendants("Computer"))
                        {
                            if (i == 1)
                            {
                                var ComputerName = childEl.Attributes("ComputerName");

                                foreach (var xAttribute in ComputerName)
                                {
                                    textBox4.Text = xAttribute.Value;
                                }

                                var ComputerIPAddress = childEl.Attributes("ComputerIPAddress");

                                foreach (var xAttribute in ComputerIPAddress)
                                {
                                    textBox3.Text = xAttribute.Value;
                                }
                                var IsReplicationAllowed = childEl.Attributes("IsReplicationAllowed");

                                foreach (var xAttribute in IsReplicationAllowed)
                                {
                                    textBox5.Text = xAttribute.Value;
                                }
                                var TimeInterval = childEl.Attributes("TimeInterval");

                                foreach (var xAttribute in TimeInterval)
                                {
                                    textBox6.Text = xAttribute.Value;
                                }
                                //string text = childEl.FirstNode().("ComputerName") ;
                                //Console.WriteLine(childEl);
                                //Console.WriteLine(i);
                            }
                            if (i == 2)
                            {
                                var ComputerName = childEl.Attributes("ComputerName");

                                foreach (var xAttribute in ComputerName)
                                {
                                    textBox7.Text = xAttribute.Value;
                                }

                                var ComputerIPAddress = childEl.Attributes("ComputerIPAddress");

                                foreach (var xAttribute in ComputerIPAddress)
                                {
                                    textBox8.Text = xAttribute.Value;
                                }
                                var IsReplicationAllowed = childEl.Attributes("IsReplicationAllowed");

                                foreach (var xAttribute in IsReplicationAllowed)
                                {
                                    textBox9.Text = xAttribute.Value;
                                }
                                var TimeInterval = childEl.Attributes("TimeInterval");

                                foreach (var xAttribute in TimeInterval)
                                {
                                    textBox10.Text = xAttribute.Value;
                                }
                                //string text = childEl.FirstNode().("ComputerName") ;
                                //Console.WriteLine(childEl);
                                //Console.WriteLine(i);
                            }
                            if (i == 3)
                            {
                                var ComputerName = childEl.Attributes("ComputerName");

                                foreach (var xAttribute in ComputerName)
                                {
                                    textBox11.Text = xAttribute.Value;
                                }

                                var ComputerIPAddress = childEl.Attributes("ComputerIPAddress");

                                foreach (var xAttribute in ComputerIPAddress)
                                {
                                    textBox12.Text = xAttribute.Value;
                                }
                                var IsReplicationAllowed = childEl.Attributes("IsReplicationAllowed");

                                foreach (var xAttribute in IsReplicationAllowed)
                                {
                                    textBox13.Text = xAttribute.Value;
                                }
                                var TimeInterval = childEl.Attributes("TimeInterval");

                                foreach (var xAttribute in TimeInterval)
                                {
                                    textBox14.Text = xAttribute.Value;
                                }
                                //string text = childEl.FirstNode().("ComputerName") ;
                                //Console.WriteLine(childEl);
                                //Console.WriteLine(i);
                            }
                            if (i == 4)
                            {
                                var ComputerName = childEl.Attributes("ComputerName");

                                foreach (var xAttribute in ComputerName)
                                {
                                    textBox15.Text = xAttribute.Value;
                                }

                                var ComputerIPAddress = childEl.Attributes("ComputerIPAddress");

                                foreach (var xAttribute in ComputerIPAddress)
                                {
                                    textBox16.Text = xAttribute.Value;
                                }
                                var IsReplicationAllowed = childEl.Attributes("IsReplicationAllowed");

                                foreach (var xAttribute in IsReplicationAllowed)
                                {
                                    textBox17.Text = xAttribute.Value;
                                }
                                var TimeInterval = childEl.Attributes("TimeInterval");

                                foreach (var xAttribute in TimeInterval)
                                {
                                    textBox18.Text = xAttribute.Value;
                                }
                                //string text = childEl.FirstNode().("ComputerName") ;
                                //Console.WriteLine(childEl);
                                //Console.WriteLine(i);
                            }
                            if (i == 5)
                            {
                                var ComputerName = childEl.Attributes("ComputerName");

                                foreach (var xAttribute in ComputerName)
                                {
                                    textBox19.Text = xAttribute.Value;
                                }

                                var ComputerIPAddress = childEl.Attributes("ComputerIPAddress");

                                foreach (var xAttribute in ComputerIPAddress)
                                {
                                    textBox20.Text = xAttribute.Value;
                                }
                                var IsReplicationAllowed = childEl.Attributes("IsReplicationAllowed");

                                foreach (var xAttribute in IsReplicationAllowed)
                                {
                                    textBox21.Text = xAttribute.Value;
                                }
                                var TimeInterval = childEl.Attributes("TimeInterval");

                                foreach (var xAttribute in TimeInterval)
                                {
                                    textBox22.Text = xAttribute.Value;
                                }
                                //string text = childEl.FirstNode().("ComputerName") ;
                                //Console.WriteLine(childEl);
                                //Console.WriteLine(i);
                            }
                            i++;
                        }
                    }
                }
            }

            catch
            {
            }
        }
    }
}
