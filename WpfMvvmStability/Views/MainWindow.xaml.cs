using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Threading;
using System.Threading;
using System;
using WpfMvvmStability.Models.BO;
using System.Windows.Media;
using System.Data;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Controls;
using System.Data.Common;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using WpfMvvmStability.Models.DAL;
using System.Runtime.Serialization.Formatters.Binary;




namespace WpfMvvmStability.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow :  Window
    {
        public static int switchLoginstatus = 0;
        BackgroundWorker bw;
        BackgroundWorker bgWorker;
        private bool isCalculationRunning = false;
        private const string folderPath = "P15B_3DModel\\";
        DataTable dtStabilityType;
        bool flagStart = false;
        public static int _statusTabRealSimulation = 0;
       public DataTable dtIntactCondition;

       public SimulationModeMain pageSimulation;

       public Action ReflectLoadedAction;

       public Action Progressaction;

       public void CallingLoading()
       {
           pageSimulation.LoadingFixedSave();
       }

       public void CallingProcess()
       {
           pageSimulation.Process();
       }
        
        string path = "";

        public MainWindow()
        {
            
            InitializeComponent();

            ReflectLoadedAction = CallingLoading;
            Progressaction = CallingProcess;
        }

        private void Main_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {

                {
                    flagStart = true;

                    ViewModels.CadViewModel.Cad2dModels();

                    Models.TableModel.CoordinateData();
                  

                    bw = new BackgroundWorker();
                    bw.DoWork += new DoWorkEventHandler(bw_DoWork);
                    bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
                    //bw.RunWorkerAsync();
                    System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
                    dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
                    dispatcherTimer.Start();
                    dispatcherTimer.Interval = new TimeSpan(0, 0, 10);
                    //....................................................

                }
            }
            catch
            {
            }
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (!bw.IsBusy && Models.BO.clsGlobVar.ConnectionError == false)
                bw.RunWorkerAsync();
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            Models.TableModel.RealModeData();
            Models.TableModel.RealModePercentFill(); //Commneted for fast debug@12122016@12:30PM
        }
        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {

                if (Models.BO.clsGlobVar.ConnectionError == false)
                {
                    if (flagStart == true)
                    {
                        RealModeMain page2 = new RealModeMain();
                        frameRealMode.NavigationService.Navigate(page2);
                   
                         pageSimulation = new SimulationModeMain();
                        frameSimulationMode.NavigationService.Navigate(pageSimulation);
                        dtIntactCondition = Models.BO.clsGlobVar.dtSimulationModeAllTanks;

                    }
                    this.tbProgress.Visibility = Visibility.Collapsed;
                    (frameRealMode.Template.FindName("lblRealStabilityType", frameRealMode) as Label).Content = Convert.ToString(clsGlobVar.dtRealStabilitySummary.Rows[0]["Stability_Type"]).ToUpper();
                    (frameRealMode.Template.FindName("lblRealStabilityStatus", frameRealMode) as Label).Content = Convert.ToString(clsGlobVar.dtRealStabilitySummary.Rows[0]["Stability_Status"]).ToUpper();


                   // (frameRealMode.Template.FindName("lblRealModeLongitudinalResult", frameRealMode) as Label).Content = Convert.ToString(clsGlobVar.dtRealStabilitySummary.Rows[1]["Stability_Status"]).ToUpper();

                    if ((frameRealMode.Template.FindName("lblRealStabilityStatus", frameRealMode) as Label).Content.ToString() == "OK")
                    {
                        (frameRealMode.Template.FindName("borderRealStabilityStatus", frameRealMode) as Border).Background = new SolidColorBrush(Colors.LawnGreen);
                    }
                    else if ((frameRealMode.Template.FindName("lblRealStabilityStatus", frameRealMode) as Label).Content.ToString() == "NOT OK")
                    {
                        (frameRealMode.Template.FindName("borderRealStabilityStatus", frameRealMode) as Border).Background = new SolidColorBrush(Colors.Red);
                    }

                    if (Convert.ToString((frameRealMode.Template.FindName("lblRealStabilityType", frameRealMode) as Label).Content) == "Intact".ToUpper())
                    {
                       // (frameRealMode.Template.FindName("btnRealModeLongitudinal", frameRealMode) as Button).IsEnabled = false;
                        (frameRealMode.Template.FindName("btnRealModeCorrectiveAction", frameRealMode) as Button).IsEnabled = false;
                        (frameRealMode.Template.FindName("borderRealStabilityType", frameRealMode) as Border).Background = new SolidColorBrush(System.Windows.Media.Colors.LawnGreen);
                    }
                    else
                    {
                       // (frameRealMode.Template.FindName("btnRealModeLongitudinal", frameRealMode) as Button).IsEnabled = false;
                        (frameRealMode.Template.FindName("btnRealModeCorrectiveAction", frameRealMode) as Button).IsEnabled = true;
                        (frameRealMode.Template.FindName("borderRealStabilityType", frameRealMode) as Border).Background = new SolidColorBrush(System.Windows.Media.Colors.Red);
                    }
                    flagStart = false;
                    Models.TableModel.Write_Log("After Background Worker");
                }
                else
                {
                    MessageBox.Show("Error While Connecting to Database");
                }
            }
            catch//(Exception ex)
            {
               // System.Windows.MessageBox.Show(ex.ToString());
            }

        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow objAbout = new AboutWindow();
            objAbout.ShowInTaskbar = false;
            objAbout.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            objAbout.Show();
        }

        private void Configuration_Click(object sender, RoutedEventArgs e)
        {
            ConfigurationWindow objConfiguration = new ConfigurationWindow();
            objConfiguration.ShowDialog();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnExitSimulation_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }


        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Printreport_click(object sender, RoutedEventArgs e)
        {
            try
            {
                string executionPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Reports\\";
                System.Diagnostics.Process.Start("explorer.exe", executionPath);
                string[] programFile = executionPath.Split('\\');
                string bitSelector = programFile[1];

                if (bitSelector.ToString() == "Program Files")
                {
                    System.Diagnostics.Process.Start("explorer.exe", @"C:\Program Files\Aegis\Stability\Reports");
                }
                else if (bitSelector.ToString() == "Program Files (x86)")
                {
                    System.Diagnostics.Process.Start("explorer.exe", @"C:\Program Files (x86)\Aegis\Stability\Reports");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Could Not Find The Path Please Contact to Administrator");
            }
        }

        private void OpenUserManual_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string st = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                path = st + "\\UserManual.Pdf";
                System.Diagnostics.Process.Start(path);
            }
            catch
            {

            }
        }

        private void OnForward(object sender, RoutedEventArgs e)
        {
            if (frameRealMode.NavigationService.CanGoForward)
            {
                frameRealMode.NavigationService.GoForward();
            }
            if (frameSimulationMode.NavigationService.CanGoForward)
            {
                frameSimulationMode.NavigationService.GoForward();
            }
        }
        private void OnBackward(object sender, RoutedEventArgs e)
        {
            if (frameRealMode.NavigationService.CanGoBack)
            {
                frameRealMode.NavigationService.GoBack();
            }
            if (frameSimulationMode.NavigationService.CanGoBack)
            {
                frameSimulationMode.NavigationService.GoBack();
            }

        }
        private void OnRefresh(object sender, RoutedEventArgs e)
        {
            frameRealMode.NavigationService.Refresh();

            frameSimulationMode.NavigationService.Refresh();
        }
        private void OnGoPageRealStabilitySummary(object sender, RoutedEventArgs e)
        {

            Models.BO.clsGlobVar.Mode = "Real";

            Models.BO.clsGlobVar.StabilityType = (frameRealMode.Template.FindName("lblRealStabilityType", frameRealMode) as Label).Content.ToString();
            StabilityDetail page = new StabilityDetail();
            frameRealMode.NavigationService.Navigate(page);
        }
        private void OnGoPageRealReport(object sender, RoutedEventArgs e)
        {
            Models.BO.clsGlobVar.Mode = "Real";
            RealModeReport page = new RealModeReport();
            frameRealMode.NavigationService.Navigate(page);
        }
        private void OnGoPageRealLongitudinal(object sender, RoutedEventArgs e)
        {
            Models.BO.clsGlobVar.Mode = "Real";
            Longitudinal page = new Longitudinal();
            frameRealMode.NavigationService.Navigate(page);
        }
        private void OnGoPageRealCorrectiveAction(object sender, RoutedEventArgs e)
        {
            Models.BO.clsGlobVar.Mode = "Real";
            CorrectiveAction page = new CorrectiveAction();
            frameRealMode.NavigationService.Navigate(page);
        }

        private void OnGoPageSimulationStabilitySummary(object sender, RoutedEventArgs e)
        {
            Models.BO.clsGlobVar.Mode = "Simulation";
            StabilityDetail page = new StabilityDetail();
            frameSimulationMode.NavigationService.Navigate(page);
        }
        private void OnGoPageSimulationReport(object sender, RoutedEventArgs e)
        {
            Models.BO.clsGlobVar.Mode = "Simulation";
            RealModeReport page = new RealModeReport();
            frameSimulationMode.NavigationService.Navigate(page);
        }
        private void OnGoPageSimulationLongitudinal(object sender, RoutedEventArgs e)
        {
            Models.BO.clsGlobVar.Mode = "Simulation";
            Longitudinal page = new Longitudinal();
            frameSimulationMode.NavigationService.Navigate(page);
        }
        private void OnGoPageSimulationCorrectiveAction(object sender, RoutedEventArgs e)
        {
           
            try
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

                isCalculationRunning = true;
                bgWorker = new BackgroundWorker();
                bgWorker.DoWork += new DoWorkEventHandler(bgWorker_Do_Work);
                bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler
                        (bgWorker_RunWorkerCompleted);
                bgWorker.WorkerReportsProgress = true;

                bgWorker.RunWorkerAsync();
                while (isCalculationRunning)
                {
                    for (int i = 0; i < 100; i++)
                    {
                        Thread.Sleep(1);
                        if (isCalculationRunning == true)
                        {
                            Progressaction();
                        }
                        else
                        { i = 100; }
                    }
                }
               pageSimulation.pRGSCalculation.Value = 0; ;
                Mouse.OverrideCursor = null;
            }
            catch
            {
            }

        }

        public void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            isCalculationRunning = false;
            Models.BO.clsGlobVar.Mode = "Simulation";
            CorrectiveAction page2 = new CorrectiveAction();
            frameSimulationMode.NavigationService.Navigate(page2);

        }

        private void bgWorker_Do_Work(object sender, DoWorkEventArgs e)

        {
            try
            {
                Models.TableModel.Write_Log("Start : Corrective action");
                string user = "dbo";
                int res;
                DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
                string Err = "";
                command.CommandText = "spCal_Simulation_CorrectiveMeasures";
                command.CommandType = CommandType.StoredProcedure;

                DbParameter param1 = Models.DAL.clsDBUtilityMethods.GetParameter();
                param1.DbType = DbType.String;
                param1.ParameterName = "@_iUser";
                command.Parameters.Add(param1);

                DbParameter param2 = Models.DAL.clsDBUtilityMethods.GetParameter();
                param2.DbType = DbType.DateTime;
                param2.ParameterName = "@_iCalculationDate";
                command.Parameters.Add(param2);

                param1.Value = user;
                param2.Value = DateTime.Now;
                Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);

                isCalculationRunning = false;
                //if (Models.BO.clsGlobVar.CalculationResult == 1)
                {
                    Models.TableModel.Write_Log("Get Corrective DATA");
                   Models.TableModel.SimulationModeCorrectiveData();
                    //Models.TableModel.SimulationModePercentFill();

                }
            }
            catch
            {

            }
        }
        private void btnSimulationGenerateReport_Click(object sender, RoutedEventArgs e)
        {
            Models.BO.clsGlobVar.Mode = "Simulation";
            RealModeReport page = new RealModeReport();
            frameSimulationMode.NavigationService.Navigate(page);
        }

        private void SavedLoadingCondition_Click(object sender, RoutedEventArgs e)
        {
            LoadingCondition load = new LoadingCondition("\\SMData", ReflectLoadedAction  );
            load.ShowDialog();
        }
        private void StandardLoadingCondition_Click(object sender, RoutedEventArgs e)
        {
            LoadingCondition load = new LoadingCondition("\\StandardData\\",ReflectLoadedAction);
            load.ShowDialog();
        }


        //private void listBoxPDF_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    try
        //    {
        //        lblBrowserError.Visibility = Visibility.Hidden;
        //        webBrowserPDF.Navigate(path + "\\" + listBoxPDF.SelectedItem.ToString());
        //    }
        //    catch
        //    {
        //    }
        //}

        //private void ViewPDF_Loaded(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {

        //        //MainWindow1 N = new MainWindow1();
        //        //N.Show();

        //        listBoxPDF.Items.Clear();
        //        lblListBoxError.Visibility = Visibility.Hidden;
        //        string st = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        //        path = st + "\\Reports";

        //        var dir = Directory.GetFiles(path);

        //        string names;
        //        foreach (string s in dir)
        //        {
        //            //names = s.Remove(0, index);
        //            listBoxPDF.Items.Add(Path.GetFileName(s));
        //        }
        //        listBoxPDF.Items.SortDescriptions.Add(
        //                new System.ComponentModel.SortDescription("",
        //                System.ComponentModel.ListSortDirection.Ascending));
        //        if (listBoxPDF.Items.Count == 0)
        //        {
        //            lblListBoxError.Visibility = Visibility.Visible;
        //        }

        //        lblBrowserError.Visibility = Visibility.Hidden;

        //        webBrowserPDF.Navigate(path + "\\" + "Default_Report.pdf");
        //    }
        //    catch
        //    {

        //    }
        //}

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void tabControl1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void LightShip_Click(object sender, RoutedEventArgs e)
        {
            switchLoginstatus = 1;

            //LightShipdetails objlightship = new LightShipdetails();
            //objlightship.ShowDialog();

            LoginWindow objLogin = new LoginWindow();
            objLogin.ShowDialog();
        }



        public MouseButtonEventHandler canvas2DPlanALL_MouseDown { get; set; }

        private void Doc1_click(object sender, RoutedEventArgs e)
        {
            try
            {
                string st = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                path = st + "\\Main Particulars and Reference System.Pdf";
                System.Diagnostics.Process.Start(path);
            }
            catch
            {

            }
        }

        private void Doc2_click(object sender, RoutedEventArgs e)
        {

            try
            {
                string st = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                path = st + "\\GA Fi.Pdf";
                System.Diagnostics.Process.Start(path);
            }
            catch
            {

            }
        }

        private void Doc3_click(object sender, RoutedEventArgs e)
        {
            try
            {
                string st = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                path = st + "\\P15B_Tank_Plan.Pdf";
                System.Diagnostics.Process.Start(path);
            }
            catch
            {

            }
        }

        private void settings_Click(object sender, RoutedEventArgs e)
        {
            switchLoginstatus = 2;
            LoginWindow objLogin = new LoginWindow();
            objLogin.ShowDialog();
        }

        private void windspeed_Click(object sender, RoutedEventArgs e)
        {
            WindSpeedWindow objWSW = new WindSpeedWindow();
            objWSW.ShowDialog();
        }

        private void tabItemReal_GotFocus(object sender, RoutedEventArgs e)
        {
            MainWindow._statusTabRealSimulation = 0;
        }

        private void tabItemSimulation_GotFocus(object sender, RoutedEventArgs e)
        {
            MainWindow._statusTabRealSimulation = 1;
           // MainMenu.Background=Color.FromRgb()

        }

        private void cMBDamage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //int type = Convert.ToInt32(clsGlobVar.dtgetsimulationtype.Rows[0]["type"]);
            //if (type != 0)
            //{
            //    System.Windows.MessageBox.Show("FIRST RUN INTACT CONDITION");

            //}
            //else {
            if (cMBDamage.SelectedIndex > 0)
            {
                int type = Convert.ToInt32(clsGlobVar.dtgetsimulationtype.Rows[0]["type"]);
                if (type != 0)
                {
                    System.Windows.MessageBox.Show("FIRST RUN INTACT CONDITION");
                    cMBDamage.SelectedIndex = 0;
                }
                 else
                 { 
                if (Models.BO.clsGlobVar.SimulationStabilityType == "Intact")
                {
                    dtIntactCondition = Models.BO.clsGlobVar.dtSimulationModeAllTanks;
                }
                //Models.BO.clsGlobVar.loadingconname = "";
                string cmbSelected = cMBDamage.SelectedItem.ToString();
                
                string CmprItem = Convert.ToString(cmbSelected.Split(':')[1]);
                clsGlobVar.cmbload = Convert.ToString(cmbSelected.Split(':')[1]); //added by sachin 5/12/22
                DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
                string Err = "", cmd = "";
                cmd = "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=0 where [User]='dbo' and [Tank_ID] between 1 and 515";
                cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=0,[Status]=0 where [User]='dbo' and [Tank_ID] between 1 and 515";
                for (int i = 0; i <= 96; i++)
                {
                    cmd += "Update [tblSimulationMode_Tank_Status] set [Volume]=" + dtIntactCondition.Rows[i]["Volume"].ToString() + " where [User]='dbo' and [Tank_ID]=" + dtIntactCondition.Rows[i]["Tank_ID"].ToString() + " ";
                }

                if (CmprItem.Trim() == "Damage case 1")
                {
                    Models.BO.clsGlobVar.DamageCase = "Damage case 1";
                    // txtLoadingConditionName.Text = clsGlobVar.DamageCase;
                    cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1 where [User]='dbo' and [Tank_ID] in (10,12,30,48)";
                    cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1 , [Status]=1 where [User]='dbo' and [Tank_ID] in (10,12,30,48)";
                }
                if (CmprItem.Trim() == "Damage case 2")
                {
                    Models.BO.clsGlobVar.DamageCase = "Damage case 2";
                    // txtLoadingConditionName.Text = clsGlobVar.DamageCase;
                    cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1 where [User]='dbo' and [Tank_ID] in ( 13,32,50)";
                    cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1 , [Status]=1 where [User]='dbo' and [Tank_ID] in (13,32,50)";
                }
                if (CmprItem.Trim() == "Damage case 3")
                {
                    Models.BO.clsGlobVar.DamageCase = "Damage case 3";
                    // txtLoadingConditionName.Text = clsGlobVar.DamageCase;
                    cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1 where [User]='dbo' and [Tank_ID] in (26,44,63,65)";
                    cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1 , [Status]=1 where [User]='dbo' and [Tank_ID] in (26,44,63,65)";
                }
                if (CmprItem.Trim() == "Damage case 4")
                {
                    Models.BO.clsGlobVar.DamageCase = "Damage case 4";
                    // txtLoadingConditionName.Text = clsGlobVar.DamageCase;
                    cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1 where [User]='dbo' and [Tank_ID] in (28,66,76,69)";
                    cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1 , [Status]=1 where [User]='dbo' and [Tank_ID] in (28,66,76,69)";
                }
                if (CmprItem.Trim() == "Damage case 5")
                {
                    Models.BO.clsGlobVar.DamageCase = "Damage case 5";
                    // txtLoadingConditionName.Text = clsGlobVar.DamageCase;
                    cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1 where [User]='dbo' and [Tank_ID] in (61,70,67,11)";
                    cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1 , [Status]=1 where [User]='dbo' and [Tank_ID] in (61,70,67,11)";
                }

                    Models.BO.clsGlobVar.SimulationStabilityType = "Damage";
                command.CommandText = cmd;
                command.CommandType = CommandType.Text;
                Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);
                Models.TableModel.SimulationModeData();


                foreach (Window window in Application.Current.Windows)
                {
                    if (window.GetType() == typeof(MainWindow))
                    {
                        (window as MainWindow).pageSimulation.dgCargoTanks.ItemsSource = Models.BO.clsGlobVar.dtSimulationCargoTanks.DefaultView;
                        (window as MainWindow).pageSimulation.dgBallastTanks.ItemsSource = Models.BO.clsGlobVar.dtSimulationBallastTanks.DefaultView;
                        (window as MainWindow).pageSimulation.dgFreshWaterTanks.ItemsSource = Models.BO.clsGlobVar.dtSimulationFreshWaterTanks.DefaultView;
                        (window as MainWindow).pageSimulation.dgFuelOilTanks.ItemsSource = Models.BO.clsGlobVar.dtSimulationFuelOilTanks.DefaultView;
                        (window as MainWindow).pageSimulation.dgMiscTanks.ItemsSource = Models.BO.clsGlobVar.dtSimulationMiscTanks.DefaultView;
                        (window as MainWindow).pageSimulation.dgCompartments.ItemsSource = Models.BO.clsGlobVar.dtSimulationCompartments.DefaultView;
                        (window as MainWindow).pageSimulation.dgVariableItems.ItemsSource = Models.BO.clsGlobVar.dtSimulationVariableItems.DefaultView;
                        (window as MainWindow).pageSimulation.dgWTRegion.ItemsSource = Models.BO.clsGlobVar.dtSimulationWTRegion.DefaultView;

                    }
                }
                //}
                string ee = "";

            }
           
        }
            Mouse.OverrideCursor = null;
        }

        private void menuIntactDamage_Click(object sender, RoutedEventArgs e)
        {
            Models.BO.clsGlobVar.DamageCase = "";
            DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
            string Err = "";
            string cmd = "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=0 where [User]='dbo' and [Tank_ID] between 1 and 101";
            cmd += "Update  [tblSimulationMode_Loading_Condition] set [IsDamaged]=0 ,[Status]=0 where [User]='dbo' and [Tank_ID] between 1 and 101 ";
            for (int i = 0; i <= 101; i++)
            {

                cmd += "Update [tblSimulationMode_Tank_Status] set [Volume]=" + dtIntactCondition.Rows[i]["Volume"].ToString() + " where [User]='dbo' and [Tank_ID]=" + dtIntactCondition.Rows[i]["Tank_ID"].ToString() + " ";

            }
            Models.BO.clsGlobVar.SimulationStabilityType = "Intact";
            command.CommandText = cmd;
            command.CommandType = CommandType.Text;
            Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);
            Models.TableModel.SimulationModeData();



            string ee = "";
            Mouse.OverrideCursor = null;
        }

        private void SM_Click(object sender, RoutedEventArgs e)
        {
            tabItemSimulation_GotFocus(sender, e);
            MainWindow._statusTabRealSimulation = 1;
       

         
        }

        private void RM_Click(object sender, RoutedEventArgs e)
        {
            tabItemReal_GotFocus(sender, e);
            MainWindow._statusTabRealSimulation = 0;
        }

        private void setipmspath_Click(object sender, RoutedEventArgs e)
        {
            string str = @"C:\Program Files (x86)\Aegis\Stability\Stability.exe";
            Process process = new Process();
            process.StartInfo.FileName = str;
            process.Start();
        }

        private void frameSimulationMode_Navigated(object sender, NavigationEventArgs e)
        {

        }
    }

    public static class ExtensionMethods
    {
        private static Action EmptyDelegate = delegate() { };

        public static void Refresh(this UIElement uiElement)
        {
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }
    }


}
