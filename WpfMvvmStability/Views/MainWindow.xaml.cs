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
                for (int i = 0; i <= 514; i++)
                {
                    cmd += "Update [tblSimulationMode_Tank_Status] set [Volume]=" + dtIntactCondition.Rows[i]["Volume"].ToString() + " where [User]='dbo' and [Tank_ID]=" + dtIntactCondition.Rows[i]["Tank_ID"].ToString() + " ";
                }

                if (CmprItem.Trim() == "GHIJ(ii)")
                {
                    Models.BO.clsGlobVar.DamageCase = "GHIJ(ii)";
                    // txtLoadingConditionName.Text = clsGlobVar.DamageCase;
                    cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1 where [User]='dbo' and [Tank_ID] in ( 21,22,23,25,27,41,51,52,57,156,461,462,463,464,465,466,467,468,469,470,471,472,473)";
                    cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1 , [Status]=1 where [User]='dbo' and [Tank_ID] in (21,22,23,25,27,41,51,52,57,156,461,462,463,464,465,466,467,468,469,470,471,472,473)";
                }
                if (CmprItem.Trim() == "KLMN(ii)")
                {
                    clsGlobVar.DamageCase = "KLMN(ii)";
                    // txtLoadingConditionName.Text = clsGlobVar.DamageCase;
                    cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1 where [User]='dbo' and [Tank_ID] in ( 29,31,35,37,43,45,47,48,154,503,474,475,477,478,480,481,482,483,485,486,487,488,490,491)";
                    cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1 , [Status]=1 where [User]='dbo' and [Tank_ID] in ( 29,31,35,37,43,45,47,48,154,503,474,475,477,478,480,481,482,483,485,486,487,488,490,491)";
                }
                if (CmprItem.Trim() == "LMNOP(ii)")
                {
                    Models.BO.clsGlobVar.DamageCase = "LMNOP(ii)";
                    // txtLoadingConditionName.Text = clsGlobVar.DamageCase;
                    cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1  where [User]='dbo' and [Tank_ID] in ( 5,7,14,16,31,35,37,45,503,504,476,477,478,480,481,482,483,485,486,487,488,490,491,492,493,494,495,496)";
                    cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1 ,[Status]=1 where [User]='dbo' and [Tank_ID] in ( 5,7,14,16,31,35,37,45,503,504,476,477,478,480,481,482,483,485,486,487,488,490,491,492,493,494,495,496)";
                }
                if (CmprItem.Trim() == "EFGH")
                {
                    Models.BO.clsGlobVar.DamageCase = "EFGH";
                    // txtLoadingConditionName.Text = clsGlobVar.DamageCase;
                    cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1 where [User]='dbo' and [Tank_ID] in ( 3,18,19,21,22,23,41,51,52,56,57,156,452,453,454,455,456,457,458,459,460,461,462,463,464,465,466)";
                    cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1, [Status]=1 where [User]='dbo' and [Tank_ID] in ( 3,18,19,21,22,23,41,51,52,56,57,156,452,453,454,455,456,457,458,459,460,461,462,463,464,465,466)";
                }
                if (CmprItem.Trim() == "ABCD")
                {
                    Models.BO.clsGlobVar.DamageCase = "ABCD";
                    // txtLoadingConditionName.Text = clsGlobVar.DamageCase;
                    cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1 where [User]='dbo' and [Tank_ID] in (1,9,12,17,55,66,67,68,91,92,93,94,95,96,97,98,99,101,102,285,287,288,290,291,292,293,294,295,296,297,298,299,300,301,412,413,416,417,418,419,422,423,424,425,426,427,428,429,430,431,432,433,434)";
                    cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1 , [Status]=1 where [User]='dbo' and [Tank_ID] in (1,9,12,17,55,66,67,68,91,92,93,94,95,96,97,98,99,101,102,285,287,288,290,291,292,293,294,295,296,297,298,299,300,301,412,413,416,417,418,419,422,423,424,425,426,427,428,429,430,431,432,433,434)";
                    //cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1 where [User]='dbo' and [Tank_ID] in ( 21,22,23,25,27,41,51,52,57,156,461,462,463,464,465,466,467,468,469,470,471,472,473)";
                    //cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1 , [Status]=1 where [User]='dbo' and [Tank_ID] in (21,22,23,25,27,41,51,52,57,156,461,462,463,464,465,466,467,468,469,470,471,472,473)";
                }
                if (CmprItem.Trim() == "BCDE")
                {
                    clsGlobVar.DamageCase = "BCDE";
                    // txtLoadingConditionName.Text = clsGlobVar.DamageCase;                                                                                                                                                                                                          bcde
                    cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1 where [User]='dbo' and [Tank_ID] in ( 395,396,397,398,399,400,401,402,403,404,405,406,407,408,409,410,411,148,412,413,414,415,416,417,418,419,420,421,422,423,424,425,426,427,428,429,430,431,144,432,275,276,277,278,279,280,281,282,283,284,285,286,287,288,289,290,291,292,293,294,295,296,297,298,299,300,105,104,196,108,107,106,197,101,103,99,100,198,102,98,97,94,95,93,96,92,71,70,72,69,68,67,9,66,56,3,18,17,1,12,55)";
                    cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1 , [Status]=1 where [User]='dbo' and [Tank_ID] in ( 395,396,397,398,399,400,401,402,403,404,405,406,407,408,409,410,411,148,412,413,414,415,416,417,418,418,419,420,421,422,423,424,425,426,427,428,429,430,431,144,432,275,276,277,278,279,280,281,282,283,284,285,286,287,288,289,290,291,292,293,294,295,296,297,298,299,300,105,104,196,108,107,106,197,101,103,99,100,198,102,98,97,94,95,93,96,92,71,70,72,69,68,67,9,66,56,3,18,17,1,12,55)";
                }
                if (CmprItem.Trim() == "CDE")
                {
                    Models.BO.clsGlobVar.DamageCase = "CDE";
                    // txtLoadingConditionName.Text = clsGlobVar.DamageCase;
                    cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1  where [User]='dbo' and [Tank_ID] in ( 395,396,397,398,399,400,401,402,403,404,405,406,407,408,409,410,411,148,412,413,414,415,416,417,418,419,420,421,422,423,424,425,426,427,428,431,144,275,276,277,278,279,280,281,282,283,284,285,286,287,288,289,290,291,292,293,294,295,296,297,299,300,105,104,196,108,107,106,197,101,103,99,100,198,102,98,97,94,95,93,96,71,70,72,69,68,67,9,56,3,18,17,1,12,55)";
                    cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1 ,[Status]=1 where [User]='dbo' and [Tank_ID] in  ( 395,396,397,398,399,400,401,402,403,404,405,406,407,408,409,410,411,148,412,413,414,415,416,417,418,419,420,421,422,423,424,425,426,427,428,431,144,275,276,277,278,279,280,281,282,283,284,285,286,287,288,289,290,291,292,293,294,295,296,297,299,300,105,104,196,108,107,106,197,101,103,99,100,198,102,98,97,94,95,93,96,71,70,72,69,68,67,9,56,3,18,17,1,12,55)";
                }
                if (CmprItem.Trim() == "IJKL(I)")
                {
                    Models.BO.clsGlobVar.DamageCase = "IJKL(I)";
                    // txtLoadingConditionName.Text = clsGlobVar.DamageCase;
                    cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1 where [User]='dbo' and [Tank_ID] in ( 149,313,314,142,315,316,317,318,143,319,320,321,322,323,324,325,326,327,328,329,330,331,332,333,334,335,336,337,338,339,340,341,342,343,344,345,346,347,348,349,350,351,352,353,354,355,356,357,358,359,235,236,237,238,239,240,241,242,243,244,245,507,246,247,248,249,250,251,252,253,254,255,256,257,508,258,259,260,509,166,167,168,169,124,125,127,170,121,122,123,171,172,173,126,174,119,120,175,176,177,178,118,117,179,180,181,88,90,86,85,84,82,83,81,33,31,59,29,154,43,48,47,27,40,25)";
                    cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1, [Status]=1 where [User]='dbo' and [Tank_ID] in( 149,313,314,142,315,316,317,318,143,319,320,321,322,323,324,325,326,327,328,329,330,331,332,333,334,335,336,337,338,339,340,341,342,343,344,345,346,347,348,349,350,351,352,353,354,355,356,357,358,359,235,236,237,238,239,240,241,242,243,244,245,507,246,247,248,249,250,251,252,253,254,255,256,257,508,258,259,260,509,166,167,168,169,124,125,127,170,121,122,123,171,172,173,126,174,119,120,175,176,177,178,118,117,179,180,181,88,90,86,85,84,82,83,81,33,31,59,29,154,43,48,47,27,40,25)";
                }
                if (CmprItem.Trim() == "JKL(II)")
                {
                    Models.BO.clsGlobVar.DamageCase = "JKL(II)";
                    // txtLoadingConditionName.Text = clsGlobVar.DamageCase;
                    cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1 where [User]='dbo' and [Tank_ID] in ( 149,313,314,142,315,316,317,318,143,319,320,321,322,323,324,325,326,327,328,329,330,331,332,333,334,335,336,337,338,339,340,341,342,343,344,235,236,237,238,239,240,241,507,242,243,244,245,246,247,248,249,250,251,166,167,168,169,121,122,123,124,125,126,127,170,171,172,173,174,175,176,177,178,179,120,119,118,117,88,90,86,85,84,31,59,29,154,43,48,47,27)";
                    cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1, [Status]=1 where [User]='dbo' and [Tank_ID] in  ( 149,313,314,142,315,316,317,318,143,319,320,321,322,323,324,325,326,327,328,329,330,331,332,333,334,335,336,337,338,339,340,341,342,343,344,235,236,237,238,239,240,241,507,242,243,244,245,246,247,248,249,250,251,166,167,168,169,121,122,123,124,125,126,127,170,171,172,173,174,175,176,177,178,179,118,117,88,90,86,85,84,31,59,29,154,43,48,47,27)";
                }
                if (CmprItem.Trim() == "AB")
                {
                    Models.BO.clsGlobVar.DamageCase = "AB";
                    // txtLoadingConditionName.Text = clsGlobVar.DamageCase;
                    cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1 where [User]='dbo' and [Tank_ID] in ( 55,66,91,92,301,298)";
                    cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1, [Status]=1 where [User]='dbo' and [Tank_ID] in  ( 55,66,91,92,301,298)";
                }
                if (CmprItem.Trim() == "CD")
                {
                    Models.BO.clsGlobVar.DamageCase = "CD";
                    // txtLoadingConditionName.Text = clsGlobVar.DamageCase;
                    cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1 where [User]='dbo' and [Tank_ID] in (285,287,288,290,291,292,293,294,295,296,297,299,300,101,99,102,98,97,94,95,93,96,68,67,9,17,1,12,55)";
                    cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1, [Status]=1 where [User]='dbo' and [Tank_ID] in  (285,287,288,290,291,292,293,294,295,296,297,299,300,101,99,102,98,97,94,95,93,96,68,67,9,17,1,12,55)";
                }
                if (CmprItem.Trim() == "FG")
                {
                    Models.BO.clsGlobVar.DamageCase = "FG";
                    // txtLoadingConditionName.Text = clsGlobVar.DamageCase;
                    cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1 where [User]='dbo' and [Tank_ID] in ( 152,264,265,153,266,267,268,269,271,150,272,273,274,182,183,116,114,184,185,113,112,188,189,190,191,115,109,192,193,111,194,195,79,78,77,76,74,75,73,22,21,57,19)";
                    cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1, [Status]=1 where [User]='dbo' and [Tank_ID] in  ( 152,264,265,153,266,267,268,269,271,150,272,273,274,182,183,116,114,184,185,113,112,188,189,190,191,115,109,192,193,111,194,195,79,78,77,76,74,75,73,22,21,57,19)";
                }
                if (CmprItem.Trim() == "LM")
                {
                    Models.BO.clsGlobVar.DamageCase = "LM";
                    // txtLoadingConditionName.Text = clsGlobVar.DamageCase;
                    cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1 where [User]='dbo' and [Tank_ID] in (227,228,229,230,231,232,233,234,235,236,237,238,239,240,241,130,163,129,164,128,165,166,167,168,169,124,125,127,170,123,122,121,171,172,173,126,87,89,88,90,86,35,62,36,31,59,32)";
                    cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1, [Status]=1 where [User]='dbo' and [Tank_ID] in  (227,228,229,230,231,232,233,234,235,236,237,238,239,240,241,130,163,129,164,128,165,166,167,168,169,124,125,127,170,123,122,121,171,172,173,126,87,89,88,90,86,35,62,36,31,59,32)";
                }
                if (CmprItem.Trim() == "MN")
                {
                    Models.BO.clsGlobVar.DamageCase = "MN";
                    // txtLoadingConditionName.Text = clsGlobVar.DamageCase;
                    cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1 where [User]='dbo' and [Tank_ID] in ( 221,222,223,224,225,226,227,228,229,230,231,232,233,234,135,134,133,132,131,130,163,129,164,128,165,87,89,45,37,63,35,62,503)";
                    cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1, [Status]=1 where [User]='dbo' and [Tank_ID] in  ( 221,222,223,224,225,226,227,228,229,230,231,232,233,234,135,134,133,132,131,130,163,129,164,128,165,87,89,45,37,63,35,62,503)";
                }
                if (CmprItem.Trim() == "A")
                {
                    Models.BO.clsGlobVar.DamageCase = "A";
                    // txtLoadingConditionName.Text = clsGlobVar.DamageCase;
                    cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1 where [User]='dbo' and [Tank_ID] in (301,91,66,55)";
                    cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1, [Status]=1 where [User]='dbo' and [Tank_ID] in  (301,91,66,55)";
                }
                if (CmprItem.Trim() == "B")
                {
                    Models.BO.clsGlobVar.DamageCase = "B";
                    // txtLoadingConditionName.Text = clsGlobVar.DamageCase;
                    cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1 where [User]='dbo' and [Tank_ID] in ( 298,92,66,55)";
                    cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1, [Status]=1 where [User]='dbo' and [Tank_ID] in  ( 298,92,66,55)";
                }
                if (CmprItem.Trim() == "C")
                {
                    Models.BO.clsGlobVar.DamageCase = "C";
                    // txtLoadingConditionName.Text = clsGlobVar.DamageCase;
                    cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1 where [User]='dbo' and [Tank_ID] in (292,294,296,297,299,300,94,95,93,96,9,12,55)";
                    cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1, [Status]=1 where [User]='dbo' and [Tank_ID] in  (292,294,296,297,299,300,94,95,93,96,9,12,55)";
                }
                if (CmprItem.Trim() == "D")
                {
                    Models.BO.clsGlobVar.DamageCase = "D";
                    // txtLoadingConditionName.Text = clsGlobVar.DamageCase;
                    cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1 where [User]='dbo' and [Tank_ID] in (285,287,288,290,291,293,295,101,99,102,98,97,68,67,17,1)";
                    cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1, [Status]=1 where [User]='dbo' and [Tank_ID] in  (285,287,288,290,291,293,295,101,99,102,98,97,68,67,17,1)";
                }
                if (CmprItem.Trim() == "E")
                {
                    Models.BO.clsGlobVar.DamageCase = "E";
                    // txtLoadingConditionName.Text = clsGlobVar.DamageCase;
                    cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1 where [User]='dbo' and [Tank_ID] in ( 275,276,277,278,279,280,281,282,283,284,286,289,105,104,196,108,107,106,197,103,186,198,71,70,72,69,56,3,18)";
                    cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1, [Status]=1 where [User]='dbo' and [Tank_ID] in  ( 275,276,277,278,279,280,281,282,283,284,286,289,105,104,196,108,107,106,197,103,186,198,71,70,72,69,56,3,18)";
                }
                if (CmprItem.Trim() == "F")
                {
                    Models.BO.clsGlobVar.DamageCase = "F";
                    // txtLoadingConditionName.Text = clsGlobVar.DamageCase;
                    cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1 where [User]='dbo' and [Tank_ID] in (150,272,273,274,190,191,115,109,192,193,111,194,195,78,77,76,74,75,73,19)";
                    cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1, [Status]=1 where [User]='dbo' and [Tank_ID] in  (150,272,273,274,190,191,115,109,192,193,111,194,195,78,77,76,74,75,73,19)";
                }
                if (CmprItem.Trim() == "G")
                {
                    Models.BO.clsGlobVar.DamageCase = "G";
                    // txtLoadingConditionName.Text = clsGlobVar.DamageCase;
                    cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1 where [User]='dbo' and [Tank_ID] in (152,264,265,153,266,267,268,269,271,182,183,116,114,184,185,113,112,188,189,79,22,21,57)";
                    cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1, [Status]=1 where [User]='dbo' and [Tank_ID] in  (152,264,265,153,266,267,268,269,271,182,183,116,114,184,185,113,112,188,189,79,22,21,57)";
                }
                if (CmprItem.Trim() == "H")
                {
                    Models.BO.clsGlobVar.DamageCase = "H";
                    // txtLoadingConditionName.Text = clsGlobVar.DamageCase;
                    cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1 where [User]='dbo' and [Tank_ID] in ( 508,509,261,262,263,187,80,23,156,51,52,41)";
                    cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1, [Status]=1 where [User]='dbo' and [Tank_ID] in  ( 508,509,261,262,263,187,80,23,156,51,52,41)";
                }
                if (CmprItem.Trim() == "I")
                {
                    Models.BO.clsGlobVar.DamageCase = "I";
                    // txtLoadingConditionName.Text = clsGlobVar.DamageCase;
                    cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1 where [User]='dbo' and [Tank_ID] in (252,253,254,255,256,257,258,259,260,180,181,82,83,81,40,25 )";
                    cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1, [Status]=1 where [User]='dbo' and [Tank_ID] in  (252,253,254,255,256,257,258,259,260,180,181,82,83,81,40,25 )";
                }
                if (CmprItem.Trim() == "J")
                {
                    Models.BO.clsGlobVar.DamageCase = "J";
                    // txtLoadingConditionName.Text = clsGlobVar.DamageCase;
                    cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1 where [User]='dbo' and [Tank_ID] in ( 244,245,246,247,248,249,250,251,174,120,119,175,176,178,118,117,179,84,27)";
                    cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1, [Status]=1 where [User]='dbo' and [Tank_ID] in  ( 244,245,246,247,248,249,250,251,174,120,119,175,176,178,118,117,179,84,27)";
                }
                if (CmprItem.Trim() == "K")
                {
                    Models.BO.clsGlobVar.DamageCase = "K";
                    // txtLoadingConditionName.Text = clsGlobVar.DamageCase;
                    cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1 where [User]='dbo' and [Tank_ID] in ( 507,242,243,515,177,85,29,154,43,48,47)";
                    cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1, [Status]=1 where [User]='dbo' and [Tank_ID] in  ( 507,242,243,515,177,85,29,154,43,48,47)";
                }
                if (CmprItem.Trim() == "L")
                {
                    Models.BO.clsGlobVar.DamageCase = "L";
                    // txtLoadingConditionName.Text = clsGlobVar.DamageCase;
                    cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1 where [User]='dbo' and [Tank_ID] in ( 235,236,237,238,239,240,241,166,167,168,169,124,125,127,170,123,122,121,171,172,173,126,88,90,86,31,59)";
                    cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1, [Status]=1 where [User]='dbo' and [Tank_ID] in  ( 235,236,237,238,239,240,241,166,167,168,169,124,125,127,170,123,122,121,171,172,173,126,88,90,86,31,59)";
                }
                if (CmprItem.Trim() == "M")
                {
                    Models.BO.clsGlobVar.DamageCase = "M";
                    // txtLoadingConditionName.Text = clsGlobVar.DamageCase;
                    cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1 where [User]='dbo' and [Tank_ID] in (227,228,229,230,231,232,233,234,130,163,129,164,128,165,87,89,35,62 )";
                    cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1, [Status]=1 where [User]='dbo' and [Tank_ID] in  (227,228,229,230,231,232,233,234,130,163,129,164,128,165,87,89,35,62 )";
                }
                if (CmprItem.Trim() == "N")
                {
                    Models.BO.clsGlobVar.DamageCase = "N";
                    // txtLoadingConditionName.Text = clsGlobVar.DamageCase;
                    cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1 where [User]='dbo' and [Tank_ID] in ( 221,222,223,224,225,226,135,134,133,132,131,45,37,63,503)";
                    cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1, [Status]=1 where [User]='dbo' and [Tank_ID] in  ( 221,222,223,224,225,226,135,134,133,132,131,45,37,63,503)";
                }
                if (CmprItem.Trim() == "O")
                {
                    Models.BO.clsGlobVar.DamageCase = "O";
                    // txtLoadingConditionName.Text = clsGlobVar.DamageCase;
                    cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1 where [User]='dbo' and [Tank_ID] in ( 215,216,217,218,219,220,138,160,136,161,162,14,5,504)";
                    cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1, [Status]=1 where [User]='dbo' and [Tank_ID] in  ( 215,216,217,218,219,220,138,160,136,161,162,14,5,504)";
                }
                if (CmprItem.Trim() == "P")
                {
                    Models.BO.clsGlobVar.DamageCase = "P";
                    // txtLoadingConditionName.Text = clsGlobVar.DamageCase;
                    cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1 where [User]='dbo' and [Tank_ID] in ( 199,205,206,207,211,212,213,214,139,158,140,137,159,6,7,16,64)";
                    cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1, [Status]=1 where [User]='dbo' and [Tank_ID] in  ( 199,205,206,207,211,212,213,214,139,158,140,137,159,6,7,16,64)";
                }
                if (CmprItem.Trim() == "Q")
                {
                    Models.BO.clsGlobVar.DamageCase = "Q";
                    // txtLoadingConditionName.Text = clsGlobVar.DamageCase;
                    cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1 where [User]='dbo' and [Tank_ID] in ( 200,201,202,203,204,208,209,210,141,210,65,11,511,512)";
                    cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1, [Status]=1 where [User]='dbo' and [Tank_ID] in  ( 200,201,202,203,204,208,209,210,65,11,511,512)";
                }
                //if (CmprItem.Trim() == "Q")
                //{
                //    Models.BO.clsGlobVar.DamageCase = "Q";
                //    // txtLoadingConditionName.Text = clsGlobVar.DamageCase;
                //    cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1 where [User]='dbo' and [Tank_ID] in ( 200,201,202,203,204,208,209,210,141,65,11,511,512)";
                //    cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1, [Status]=1 where [User]='dbo' and [Tank_ID] in  ( 200,201,202,203,204,208,209,210,141,65,11,511,512)";
                //}
                if (CmprItem.Trim() == "LM(III)")
                {
                    Models.BO.clsGlobVar.DamageCase = "LM(III)";
                    // txtLoadingConditionName.Text = clsGlobVar.DamageCase;
                    cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1 where [User]='dbo' and [Tank_ID] in ( 31,35,36,32,86,87,88,89,90,121,122,123,124,125,126,127,128,129,130,163,164,165,166,167,168,169,170,171,172,173,227,228,229,230,231,232,233,234,235,236,237,238,239,240,241)";
                    cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1, [Status]=1 where [User]='dbo' and [Tank_ID] in  ( 31,35,36,32,86,87,88,89,90,121,122,123,124,125,126,127,128,129,130,163,164,165,166,167,168,169,170,171,172,173,227,228,229,230,231,232,233,234,235,236,237,238,239,240,241)";
                }
                if (CmprItem.Trim() == "JK(I)")
                {
                    Models.BO.clsGlobVar.DamageCase = "JK(I)";
                    // txtLoadingConditionName.Text = clsGlobVar.DamageCase;
                    cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1 where [User]='dbo' and [Tank_ID] in (27,29,43,47,48,84,85,117,118,119,120,154,174,175,176,177,178,179,242,243,244,245,246,247,248,249,250,251,507,515)";
                    cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1, [Status]=1 where [User]='dbo' and [Tank_ID] in  (27,29,43,47,48,84,85,117,118,119,120,154,174,175,176,177,178,179,242,243,244,245,246,247,248,249,250,251,507,515)";
                }
                if (CmprItem.Trim() == "NO(I)")
                {
                    Models.BO.clsGlobVar.DamageCase = "NO(I)";
                    // txtLoadingConditionName.Text = clsGlobVar.DamageCase;
                    cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1 where [User]='dbo' and [Tank_ID] in ( 5,14,37,45,63,131,132,133,134,135,136,137,138,160,161,162,215,216,217,218,219,220,221,222,223,224,225,226,503,504,39)";
                    cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1, [Status]=1 where [User]='dbo' and [Tank_ID] in  ( 5,14,37,45,63,131,132,133,134,135,136,137,138,160,161,162,215,216,217,218,219,220,221,222,223,224,225,226,503,504,39)";
                }
                if (CmprItem.Trim() == "NO(III)")
                {
                    Models.BO.clsGlobVar.DamageCase = "NO(III)";
                    // txtLoadingConditionName.Text = clsGlobVar.DamageCase;
                    cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1 where [User]='dbo' and [Tank_ID] in (14,5,45,37,490,491,492,493,494)";
                    cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1, [Status]=1 where [User]='dbo' and [Tank_ID] in (14,5,45,37,490,491,492,493,494)";
                }
                    if (CmprItem.Trim() == "KL(II)")
                    {
                        Models.BO.clsGlobVar.DamageCase = "KL(II)";
                        // txtLoadingConditionName.Text = clsGlobVar.DamageCase;
                        cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1 where [User]='dbo' and [Tank_ID] in (29,31,32,43,47,48,59,85,86,88,90,121,122,123,124,125,126,127,154,166,167,168,169,170,171,172,173,177,235,236,237,238,239,240,241,242,243,507,515)";
                        cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1, [Status]=1 where [User]='dbo' and [Tank_ID] in (29,31,32,43,47,48,59,85,86,88,90,121,122,123,124,125,126,127,154,166,167,168,169,170,171,172,173,177,235,236,237,238,239,240,241,242,243,507,515)";
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
            // clsGlobVar.FlagDamageCases = false;
        }
            Mouse.OverrideCursor = null;
 
           // if (cMBDamage.SelectedIndex > 0)
           // {
           //     if (Models.BO.clsGlobVar.SimulationStabilityType == "Intact")
           //     {
           //         dtIntactCondition = Models.BO.clsGlobVar.dtSimulationModeAllTanks;
           //     }

           //     string cmbSelected = cMBDamage.SelectedItem.ToString();
           //     string CmprItem = Convert.ToString(cmbSelected.Split(':')[1]);
           //     DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
           //     string Err = "", cmd = "";
           //     cmd = "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=0 where [User]='dbo' and [Tank_ID] between 1 and 515";
           //     cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=0,[Status]=0 where [User]='dbo' and [Tank_ID] between 1 and 515";
           //     for (int i = 0; i <= 514; i++)
           //     {
           //         cmd += "Update [tblSimulationMode_Tank_Status] set [Volume]=" + dtIntactCondition.Rows[i]["Volume"].ToString() + " where [User]='dbo' and [Tank_ID]=" + dtIntactCondition.Rows[i]["Tank_ID"].ToString() + " ";
           //     }
           //     if (CmprItem.Trim() == "ABCD")
           //     {
           //         Models.BO.clsGlobVar.DamageCase = "ABCD";
           //         // txtLoadingConditionName.Text = clsGlobVar.DamageCase;
           //         cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1 where [User]='dbo' and [Tank_ID] in (1,9,12,17,55,66,67,68,91,92,93,94,95,96,97,98,99,101,102,285,287,288,290,291,292,293,294,295,296,297,298,299,300,301,412,413,416,417,418,419,422,423,424,425,426,427,428,429,430,431,432,433,434)";
           //         cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1 , [Status]=1 where [User]='dbo' and [Tank_ID] in (1,9,12,17,55,66,67,68,91,92,93,94,95,96,97,98,99,101,102,285,287,288,290,291,292,293,294,295,296,297,298,299,300,301,412,413,416,417,418,419,422,423,424,425,426,427,428,429,430,431,432,433,434)";
           //     }
              
           //     if (CmprItem.Trim() == "BCDE")
           //     {
           //         clsGlobVar.DamageCase = "BCDE";
           //         // txtLoadingConditionName.Text = clsGlobVar.DamageCase;
           //         cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1 where [User]='dbo' and [Tank_ID] in ( 1,3,9,12,17,18,55,56,66,67,68,69,70,71,72,92,93,94,95,96,97,98,99,100,101,102,103,104,105,106,107,108,144,148,196,197,198,275,276,277,278,279,280,281,282,283,284,285,286,287,288,289,290,291,292,293,294,295,296,297,298,299,300,395,396,397,398,399,400,401,402,403,404,405,406,407,408,409,410,411,412,413,416,417,418,419,422,423,424,425,426,427,428,429,430,431,432)";
           //         cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1 , [Status]=1 where [User]='dbo' and [Tank_ID] in ( 1,3,9,12,17,18,55,56,66,67,68,69,70,71,72,92,93,94,95,96,97,98,99,100,101,102,103,104,105,106,107,108,144,148,196,197,198,275,276,277,278,279,280,281,282,283,284,285,286,287,288,289,290,291,292,293,294,295,296,297,298,299,300,395,396,397,398,399,400,401,402,403,404,405,406,407,408,409,410,411,412,413,416,417,418,419,422,423,424,425,426,427,428,429,430,431,432)";
           //     }
              
           //     if (CmprItem.Trim() == "CDE")
           //     {
           //         Models.BO.clsGlobVar.DamageCase = "CDE";
           //         // txtLoadingConditionName.Text = clsGlobVar.DamageCase;
           //         cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1  where [User]='dbo' and [Tank_ID] in ( 1,3,9,12,17,18,55,56,67,68,69,70,71,72,93,94,95,96,97,98,99,100,101,102,103,104,105,106,107,108,144,148,196,197,198,275,276,277,278,279,280,281,282,283,284,285,286,287,288,289,290,291,292,293,294,295,296,297,299,300,395,396,397,398,399,400,401,402,403,404,405,406,407,408,409,410,411,412,413,416,417,418,419,422,423,424,425,426,427,428,429,431,432)";
           //         cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1 ,[Status]=1 where [User]='dbo' and [Tank_ID] in ( 1,3,9,12,17,18,55,56,67,68,69,70,71,72,93,94,95,96,97,98,99,100,101,102,103,104,105,106,107,108,144,148,196,197,198,275,276,277,278,279,280,281,282,283,284,285,286,287,288,289,290,291,292,293,294,295,296,297,299,300,395,396,397,398,399,400,401,402,403,404,405,406,407,408,409,410,411,412,413,416,417,418,419,422,423,424,425,426,427,428,429,431,432)";
           //     }
              
           //     if (CmprItem.Trim() == "IJKL(I)")
           //     {
           //         Models.BO.clsGlobVar.DamageCase = "IJKL(I)";
           //         // txtLoadingConditionName.Text = clsGlobVar.DamageCase;
           //         cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1 where [User]='dbo' and [Tank_ID] in ( 25,27,29,31,33,40,43,47,48,59,81,82,83,84,85,86,88,90,117,118,119,120,121,122,123,124,125,126,127,142,143,149,154,166,167,168,169,170,171,172,173,174,175,176,177,178,179,180,181,235,236,237,238,239,240,241,242,243,244,245,246,247,248,249,250,251,252,253,254,255,256,257,258,259,260,313,314,315,316,317,318,319,320,321,322,323,324,325,326,327,328,329,330,331,332,333,334,335,336,337,338,339,340,341,342,343,344,345,346,347,348,349,350,351,352,353354,355,356,357,358,359,477,507,514,515)";
           //         cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1, [Status]=1 where [User]='dbo' and [Tank_ID] in( 25,27,29,31,33,40,43,47,48,59,81,82,83,84,85,86,88,90,117,118,119,120,121,122,123,124,125,126,127,142,143,149,154,166,167,168,169,170,171,172,173,174,175,176,177,178,179,180,181,235,236,237,238,239,240,241,242,243,244,245,246,247,248,249,250,251,252,253,254,255,256,257,258,259,260,313,314,315,316,317,318,319,320,321,322,323,324,325,326,327,328,329,330,331,332,333,334,335,336,337,338,339,340,341,342,343,344,345,346,347,348,349,350,351,352,353354,355,356,357,358,359,477,507,514,515)";
           //     }
              
           //     if (CmprItem.Trim() == "JKL(II)")
           //     {
           //         Models.BO.clsGlobVar.DamageCase = "JKL(II)";
           //         // txtLoadingConditionName.Text = clsGlobVar.DamageCase;
           //         cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1 where [User]='dbo' and [Tank_ID] in (27,29,31,43,47,48,59,84,85,86,88,90,117,118,119,120,121,122,123,124,125,126,127,142,143,149,154,166,167,168,169,170,171,172,173,174,175,176,177,178,179,180,235.236.237.238,239,240,241,242,243,244,245,246,247,248,249,250,251,313,314,315,316,317,318,319,320,321,322,323,324,325,326,327,328,329,330,331,332,333,334,335,336,337,338,339,340,341,342,343,344,477,507,514,515)";
           //         cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1, [Status]=1 where [User]='dbo' and [Tank_ID] in (27,29,31,43,47,48,59,84,85,86,88,90,117,118,119,120,121,122,123,124,125,126,127,142,143,149,154,166,167,168,169,170,171,172,173,174,175,176,177,178,179,180,235.236.237.238,239,240,241,242,243,244,245,246,247,248,249,250,251,313,314,315,316,317,318,319,320,321,322,323,324,325,326,327,328,329,330,331,332,333,334,335,336,337,338,339,340,341,342,343,344,477,507,514,515)";
           //     }
           //     if (CmprItem.Trim() == "GHIJ(ii)")
           //     {
           //         Models.BO.clsGlobVar.DamageCase = "GHIJ(ii)";
           //         // txtLoadingConditionName.Text = clsGlobVar.DamageCase; ( 21,22,23,25,27,41,51,52,57,156,461,462,463,464,465,466,467,468,469,470,471,472,473)";
           //         cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1 where [User]='dbo' and [Tank_ID] in ( 21,22,23,25,27,41,51,52,57,156,461,462,463,464,465,466,467,468,469,470,471,472,473)";
           //         cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1 , [Status]=1 where [User]='dbo' and [Tank_ID] in ( 21,22,23,25,27,41,51,52,57,156,461,462,463,464,465,466,467,468,469,470,471,472,473)";
           //        // cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1 where [User]='dbo' and [Tank_ID] in ( 21,22,23,25,27,41,51,52,57,156,461,462,463,464,465,466,467,468,469,470,471,472,473)";
           //         //cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1 , [Status]=1 where [User]='dbo' and [Tank_ID] in (21,22,23,25,27,41,51,52,57,156,461,462,463,464,465,466,467,468,469,470,471,472,473)";
           //     }
           //     if (CmprItem.Trim() == "KLMN(ii)")
           //     {
           //         clsGlobVar.DamageCase = "KLMN(ii)";
           //         // txtLoadingConditionName.Text = clsGlobVar.DamageCase;
           //         cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1 where [User]='dbo' and [Tank_ID] in ( 29,31,35,37,43,45,47,48,154,503,474,475,477,478,480,481,482,483,485,486,487,488,490,491)";
           //         cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1 , [Status]=1 where [User]='dbo' and [Tank_ID] in ( 29,31,35,37,43,45,47,48,154,503,474,475,477,478,480,481,482,483,485,486,487,488,490,491)";
           //     }
           //     if (CmprItem.Trim() == "LMNOP(ii)")
           //     {
           //         Models.BO.clsGlobVar.DamageCase = "LMNOP(ii)";
           //         // txtLoadingConditionName.Text = clsGlobVar.DamageCase;
           //         cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1  where [User]='dbo' and [Tank_ID] in ( 5,7,14,16,31,35,37,45,503,504,476,477,478,480,481,482,483,485,486,487,488,490,491,492,493,494,495,496)";
           //         cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1 ,[Status]=1 where [User]='dbo' and [Tank_ID] in ( 5,7,14,16,31,35,37,45,503,504,476,477,478,480,481,482,483,485,486,487,488,490,491,492,493,494,495,496)";
           //     }
           //     if (CmprItem.Trim() == "EFGH")
           //     {
           //         Models.BO.clsGlobVar.DamageCase = "EFGH";
           //         // txtLoadingConditionName.Text = clsGlobVar.DamageCase;
           //         cmd += "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=1 where [User]='dbo' and [Tank_ID] in ( 3,18,19,21,22,23,41,51,52,56,57,156,452,453,454,455,456,457,458,459,460,461,462,463,464,465,466)";
           //         cmd += "Update [tblSimulationMode_Loading_Condition] set [IsDamaged]=1, [Status]=1 where [User]='dbo' and [Tank_ID] in ( 3,18,19,21,22,23,41,51,52,56,57,156,452,453,454,455,456,457,458,459,460,461,462,463,464,465,466)";
           //     }
                
           //     Models.BO.clsGlobVar.SimulationStabilityType = "Damage";
           //     command.CommandText = cmd;
           //     command.CommandType = CommandType.Text;
           //     Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);
           //     Models.TableModel.SimulationModeData();
                

           //     foreach (Window window in Application.Current.Windows)
           //     {
           //         if (window.GetType() == typeof(MainWindow))
           //         {

           //             (window as MainWindow).pageSimulation.dgBallastTanks.ItemsSource = Models.BO.clsGlobVar.dtSimulationBallastTanks.DefaultView;
           //             (window as MainWindow).pageSimulation.dgFreshWaterTanks.ItemsSource = Models.BO.clsGlobVar.dtSimulationFreshWaterTanks.DefaultView;
           //             (window as MainWindow).pageSimulation.dgFuelOilTanks.ItemsSource = Models.BO.clsGlobVar.dtSimulationFuelOilTanks.DefaultView;
           //             (window as MainWindow).pageSimulation.dgMiscTanks.ItemsSource = Models.BO.clsGlobVar.dtSimulationMiscTanks.DefaultView;
           //             (window as MainWindow).pageSimulation.dgCompartments.ItemsSource = Models.BO.clsGlobVar.dtSimulationCompartments.DefaultView;
           //             (window as MainWindow).pageSimulation.dgVariableItems.ItemsSource = Models.BO.clsGlobVar.dtSimulationVariableItems.DefaultView;
           //             (window as MainWindow).pageSimulation.dgWTRegion.ItemsSource = Models.BO.clsGlobVar.dtSimulationWTRegion.DefaultView;

           //         }
           //     }

           //     string ee = "";
             
           // }
           //// clsGlobVar.FlagDamageCases = false;
           // Mouse.OverrideCursor = null;
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
