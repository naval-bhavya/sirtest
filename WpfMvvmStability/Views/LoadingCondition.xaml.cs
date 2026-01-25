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
using System.Data;
using System.Data.Common;
using System.IO;
using System.Collections;
using WpfMvvmStability.Models.DAL;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.Data.SqlClient;
using WpfMvvmStability.Models.BO;

namespace WpfMvvmStability.Views
{
    /// <summary>
    /// Interaction logic for LoadingCondition.xaml
    /// </summary>
    public partial class LoadingCondition : Window
    {
        string CalculationMethod, DamageCase;
        public static string folder = "";
        int index;

        public Action LoadingPageAction; 
        public LoadingCondition(string folderName, Action  obj)
        {
            InitializeComponent();
            LoadingConditionList(folderName);
            LoadingPageAction = obj;
                
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void LoadingConditionList(string folderName)
        {
            try
            {
                folder = folderName;
                lblError.Visibility = Visibility.Hidden;
                string st = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string path = st + folderName;
               
                var dir = System.IO.Directory.GetDirectories(path).OrderByDescending(d => new System.IO.DirectoryInfo(d).CreationTime);
                if (folder == "\\SMData")
                {
                    listBoxSavedCondition.Items.SortDescriptions.Add(
                          new System.ComponentModel.SortDescription("",
                          System.ComponentModel.ListSortDirection.Descending));
                    index = st.Length + 8;
                    btnDelete.Visibility = Visibility.Visible;
                    lblConditionType.Content = "Saved Loading Condition";
                }
                else
                {
                    
                    index = st.Length + 14;
                    btnDelete.Visibility = Visibility.Hidden;
                    lblConditionType.Content = "Standard Loading Condition";
                }
                string names;
                foreach (string s in dir)
                {
                    names = s.Remove(0, index);
                    listBoxSavedCondition.Items.Add(names);
                }
                //listBoxSavedCondition.Items.SortDescriptions.Add(
                //        new System.ComponentModel.SortDescription("",
                //        System.ComponentModel.ListSortDirection.Ascending));
                if (listBoxSavedCondition.Items.Count == 0)
                {
                    lblError.Visibility = Visibility.Visible;
                }
            }
            catch
            {

            }

        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

                if (listBoxSavedCondition.SelectedItem != null)
                {
                    string filename = listBoxSavedCondition.SelectedItem.ToString();
           
                    refresh();

                    LoadingPageAction();

                    #region reflect grid with refrence by mainwindow
                    //foreach (Window window in Application.Current.Windows)
                    //{
                    //    if (window.GetType() == typeof(MainWindow))
                    //    {

                    //        (window as MainWindow).pageSimulation.dgBallastTanks.ItemsSource = Models.BO.clsGlobVar.dtSimulationBallastTanks.DefaultView;
                    //        (window as MainWindow).pageSimulation.dgFreshWaterTanks.ItemsSource = Models.BO.clsGlobVar.dtSimulationFreshWaterTanks.DefaultView;
                    //        (window as MainWindow).pageSimulation.dgFuelOilTanks.ItemsSource = Models.BO.clsGlobVar.dtSimulationFuelOilTanks.DefaultView;
                    //        (window as MainWindow).pageSimulation.dgMiscTanks.ItemsSource = Models.BO.clsGlobVar.dtSimulationMiscTanks.DefaultView;
                    //        (window as MainWindow).pageSimulation.dgCompartments.ItemsSource = Models.BO.clsGlobVar.dtSimulationCompartments.DefaultView;
                    //        (window as MainWindow).pageSimulation.dgVariableItems.ItemsSource = Models.BO.clsGlobVar.dtSimulationVariableItems.DefaultView;
                           
                    //    }
                    //}
                    #endregion

                    this.Close();
                    MessageBox.Show(listBoxSavedCondition.SelectedItem.ToString() + " Loading Condition Loaded");
                    Models.BO.clsGlobVar.loadingconname = listBoxSavedCondition.SelectedItem.ToString();
                    clsGlobVar.cmbload =" ";
                    //Models.clsGlobVar.flagLoadingCondition = true;
                }
                else
                {
                    MessageBox.Show("Please Select a Loading Condition");
                }
                Mouse.OverrideCursor = null;
            }
            catch
            {
                this.Close();
                Mouse.OverrideCursor = null;
            }

            
        }

        private void refresh()
        {
            try
            {
                string path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + folder + "\\" + listBoxSavedCondition.SelectedItem.ToString();

              
                FileStream fs = new FileStream(path + "\\Tanks.cnd", FileMode.Open, FileAccess.Read, FileShare.None);
                BinaryFormatter ob = new BinaryFormatter();


                List<Tanks> listTank = new List<Tanks>();
                listTank = (List<Tanks>)ob.Deserialize(fs);
                fs.Close();
                //dtSMBallast= liBallast.toDa
                DataTable dtTanks = CollectionHelper.ConvertTo<Tanks>(listTank);
                //dtTanks = dtSMTanks.Clone();
                DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
                string Err = "";
                string cmd = "";

                foreach (DataRow row in dtTanks.Rows)
                {
                    cmd += @" UPDATE tblSimulationMode_Loading_Condition  
                                SET Volume=" + row["Volume"].ToString() + @", SG=" + row["SG"].ToString() + @",Weight=" + row["Weight"].ToString() + @",
                                    Percent_Full=" + row["Percent_Full"].ToString() + @",LCG=" + row["LCG"].ToString() + @", TCG=" + row["TCG"].ToString() + @",VCG=" + row["VCG"].ToString() + @",
                                    FSM=" + row["FSM"].ToString() + @",IsDamaged=" + Convert.ToInt16(row["IsDamaged"]) + @", Status=" + Convert.ToInt16(row["Status"]) + @",
                                    FSMType=" + row["FSMType"] + @",FSMInput=" + row["FSM"].ToString() + @", IsVisible=" + Convert.ToInt16(row["IsVisible"]) + @" WHERE Tank_ID=" + row["Tank_ID"].ToString() + @"
                              UPDATE tblSimulationMode_Tank_Status  SET Volume=" + row["Volume"].ToString() + @", SG=" + row["SG"].ToString() + @",
                                    IsDamaged=" + Convert.ToInt16(row["IsDamaged"]) + @" WHERE Tank_ID=" + row["Tank_ID"].ToString(); 
                }
                //con.Close();
                command.CommandText = cmd;
                command.CommandType = CommandType.Text;
                Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);
           

                fs = new FileStream(path + "\\FixedLoads.cnd", FileMode.Open, FileAccess.Read, FileShare.None);
                //  ob = new BinaryFormatter();
                cmd = "";
                List<FixedItems> liDeck1 = new List<FixedItems>();
                liDeck1 = (List<FixedItems>)ob.Deserialize(fs);
                fs.Close();
                DataTable dtFixedLoads = CollectionHelper.ConvertTo<FixedItems>(liDeck1);



                {
                    //string Err = "";
                    string query = @"DELETE FROM tblFixedLoad_Simulation";
                  //  DataTable veriableSimulation = Models.BO.clsGlobVar.dtRealVariableItems;
                    command.CommandText = query;
                    command.CommandType = CommandType.Text;
                    Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);

                    SqlConnection conn = clsSqlData.getConnection();
                    SqlCommand sqlcmd = new SqlCommand();
                    sqlcmd.Connection = conn;
                    conn.Open();
                    SqlTransaction tran = conn.BeginTransaction();
                    sqlcmd.Transaction = tran;
                    sqlcmd.CommandText = "INSERT INTO tblFixedLoad_Simulation ([Load_Name],[Weight],[LCG],[TCG],[VCG],[Length],[Breadth],[Depth]) VALUES ( @Load_Name,@Weight,@LCG,@TCG,@VCG,@Length,@Breadth,@Depth) ";
                    sqlcmd.Parameters.AddWithValue("@Load_Name", SqlDbType.VarChar);
                    sqlcmd.Parameters.AddWithValue("@Weight", SqlDbType.Decimal);
                    sqlcmd.Parameters.AddWithValue("@LCG", SqlDbType.Decimal);
                    sqlcmd.Parameters.AddWithValue("@TCG", SqlDbType.Decimal);
                    sqlcmd.Parameters.AddWithValue("@VCG", SqlDbType.Decimal);
                    sqlcmd.Parameters.AddWithValue("@Length", SqlDbType.Decimal);
                    sqlcmd.Parameters.AddWithValue("@Breadth", SqlDbType.Decimal);
                    sqlcmd.Parameters.AddWithValue("@Depth", SqlDbType.Decimal);

                    for (int i = 0; i < dtFixedLoads.Rows.Count; i++)
                    {


                        sqlcmd.Parameters["@Load_Name"].Value = dtFixedLoads.Rows[i]["Load_Name"].ToString();
                        sqlcmd.Parameters["@Weight"].Value = Convert.ToDecimal(dtFixedLoads.Rows[i]["Weight"].ToString());
                        sqlcmd.Parameters["@LCG"].Value = Convert.ToDecimal(dtFixedLoads.Rows[i]["LCG"].ToString());
                        sqlcmd.Parameters["@TCG"].Value = Convert.ToDecimal(dtFixedLoads.Rows[i]["TCG"].ToString());
                        sqlcmd.Parameters["@VCG"].Value = Convert.ToDecimal(dtFixedLoads.Rows[i]["VCG"].ToString());
                        sqlcmd.Parameters["@Length"].Value = Convert.ToDecimal(dtFixedLoads.Rows[i]["Length"].ToString());
                        sqlcmd.Parameters["@Breadth"].Value = Convert.ToDecimal(dtFixedLoads.Rows[i]["Breadth"].ToString());
                        sqlcmd.Parameters["@Depth"].Value = Convert.ToDecimal(dtFixedLoads.Rows[i]["Depth"].ToString());

                        int rows = sqlcmd.ExecuteNonQuery();
                    }

                    tran.Commit();
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
                Models.TableModel.SimulationModeData();


//                command = Models.DAL.clsDBUtilityMethods.GetCommand();
//                foreach (DataRow row in dtFixedLoads.Rows)
//                {
//                    cmd += " UPDATE [tblFixedLoad_Simulation] SET Weight=" + row["Weight"].ToString() + "LCG=" + row["LCG"].ToString() + ",VCG=" + row["VCG"].ToString() + @",
//                    TCG=" + row["TCG"].ToString() + @",
//                    Length=" + row["Length"].ToString() + @",
//                    Breadth=" + row["Breadth"].ToString() + @",
//                    Depth=" + row["Depth"].ToString() + @"
//                    WHERE Tank_ID=" + row["Tank_ID"].ToString();
//                    cmd += " UPDATE tblSimulationMode_Tank_Status SET Weight=" + row["Weight"].ToString() + "  WHERE Tank_ID=" + row["Tank_ID"].ToString(); ;
//                }

//                command.CommandText = cmd;
//                command.CommandType = CommandType.Text;
//                Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);
//                Models.TableModel.SimulationModeData();
                //Models.BO.clsGlobVar.dtSimulationModeAllTanks = dtTanks;
                //Models.BO.clsGlobVar.dtSimulationVariableItems = dtFixedLoads;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (listBoxSavedCondition.SelectedItem != null)
                {
                    MessageBoxResult result = MessageBox.Show("Do you want to Delete Selected Loading Condition ?", "Delete Loading Condition", MessageBoxButton.YesNoCancel);
                    switch (result)
                    {
                        case MessageBoxResult.Yes:
                            Directory.Delete(System.IO.Directory.GetCurrentDirectory() + folder + "\\" + listBoxSavedCondition.SelectedItem.ToString(), true);
                            listBoxSavedCondition.Items.Clear();
                            LoadingConditionList(folder);
                            break;
                        case MessageBoxResult.No:

                            break;
                        case MessageBoxResult.Cancel:

                            break;
                    }
                }
                else
                {
                    MessageBox.Show("Please Select a Loading Condition");
                }

            }

            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

       
    }
}
