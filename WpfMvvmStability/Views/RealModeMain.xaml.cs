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
using System.Windows.Navigation;
using System.Windows.Shapes;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.SharpDX;
using System.Windows.Media.Media3D;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Data;
using System.Reflection;
using WpfMvvmStability.Models.BO;


#region CADLIB
using WW.Cad.Base;
using WW.Cad.Drawing;
using WW.Cad.Drawing.GDI;
using WW.Cad.IO;
using WW.Cad.Model;
using WW.Math;
using WW.Cad.Drawing.Wpf;
using WW.Cad.Model.Objects;
using WW.Math.Geometry;
//using _3DTools;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Data.Common;
using System.Collections;
using WpfMvvmStability.Models.DAL;
using System.Data.SqlClient;
#endregion
namespace WpfMvvmStability.Views
{
    /// <summary>
    /// Interaction logic for RealModeMain.xaml
    /// </summary>
    public partial class RealModeMain : Page
    {
        double x1 = 0, y1 = 0, x2 = 0, y2 = 0, x3 = 0, y3 = 0, x4 = 0, y4 = 0, x5 = 0, x6 = 0, y5 = 0, y6 = 0, x7 = 0, x8 = 0, y7 = 0, y8 = 0;
        private const string folderPath = @"3D\";
        private Dictionary<string, HelixToolkit.SharpDX.MeshGeometry3D> _geometryCache = new Dictionary<string, HelixToolkit.SharpDX.MeshGeometry3D>();
        private bool _pendingFit = false;

        Assembly assembly = Assembly.GetExecutingAssembly();
        BackgroundWorker bw;

        TextBox tb = new TextBox();
        private bool _forceRealModeCalculation = true;
        private bool _pendingRealModeRefresh = false;
        private string _lastRealModeVolumeSignature = string.Empty;
        private string TankNameForPercentage;
        private Bounds3D bounds;
        private Bounds3D boundsProfile;
        private Bounds3D boundsPlanA;
        private Bounds3D boundsPlanB;
        private Bounds3D boundsPlanC;
        private Bounds3D boundsPlanALL;
        private WpfWireframeGraphics3DUsingDrawingVisual wpfGraphics;
        private WireframeGraphics2Cache graphicsCache;
        private GraphicsConfig graphicsConfig;
        private WW.Math.Vector3D translation;
        private double scaling = 1d;
        private bool is2DPanning;
        private System.Windows.Point last2DPanPoint;
        private Canvas active2DCanvas;

        int indexvar;

        private decimal PercentageFill;
        private Bounds3D boundsNew;
        private WpfWireframeGraphics3DUsingDrawingVisual wpfGraphicsNew;
        private WireframeGraphics2Cache graphicsCacheNew;
        private GraphicsConfig graphicsConfigNew;

        string header = string.Empty;
        public static Dictionary<int, decimal> maxVolume;

        private string NormalizedGridHeader
        {
            get { return NormalizeGridHeader(header); }
        }

        private static string NormalizeGridHeader(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            string cleaned = value.ToUpperInvariant()
                .Replace(" ", string.Empty)
                .Replace(".", string.Empty)
                .Replace("-", string.Empty);

            if (cleaned == "VOLUME" || cleaned == "VOLUME(CUM)" || cleaned == "VOLUME(CU.M)") return "VOLUME";
            if (cleaned == "PERCENTFILL" || cleaned == "PERCENT_FULL") return "PERCENT";
            if (cleaned == "SPECIFICGRAVITY" || cleaned == "SG" || cleaned == "SGRAVITY") return "SG";
            return cleaned;
        }
        System.Windows.Threading.DispatcherTimer TimerGraphicsRefresh = new System.Windows.Threading.DispatcherTimer();

        private void Show3DLoading(string message = "Loading 3D model...")
        {
            if (overlay3DLoading != null)
            {
                txt3DLoadingTitle.Text = message;
                txt3DLoadingHint.Text = "Please wait a few seconds. The model will appear shortly.";
                overlay3DLoading.Visibility = Visibility.Visible;
            }
        }

        private void Hide3DLoading()
        {
            if (overlay3DLoading != null)
            {
                overlay3DLoading.Visibility = Visibility.Collapsed;
            }
        }

        private bool _is3DRefreshInProgress = false;
        private bool _pending3DRefresh = false;
        private bool _hasRendered3DOnce = false;
        private bool _logged3DBasePath = false;
        private int _mouseMoveLogCounter = 0;
        private bool _host2DHooksAttached = false;

        private void Log2D(string msg)
        {
            System.Diagnostics.Debug.WriteLine("[2D-REAL] " + msg);
        }

        private void EnsureHost2DInteractionHooks()
        {
            if (_host2DHooksAttached) return;
            _host2DHooksAttached = true;

            Action<Canvas> hook = c =>
            {
                if (c == null) return;
                if (c.Background == null) c.Background = Brushes.Transparent;
                c.AddHandler(UIElement.PreviewMouseWheelEvent, new MouseWheelEventHandler(Canvas2D_MouseWheel), true);
                c.AddHandler(UIElement.PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(Canvas2D_MouseLeftButtonDown), true);
                c.AddHandler(UIElement.PreviewMouseLeftButtonUpEvent, new MouseButtonEventHandler(Canvas2D_MouseLeftButtonUp), true);
                c.AddHandler(UIElement.PreviewMouseMoveEvent, new MouseEventHandler(Canvas2D_MouseMove), true);
            };

            hook(canvas2DProfile);
            hook(canvas2DPlanA);
            hook(canvas2DPlanB);
            hook(canvas2DPlanC);
            hook(canvas2DPlanALL);
            Log2D("Host canvas hooks attached");
        }

        private void Schedule3DRefresh(bool forceShowLoading = false)
        {
            if (viewPort3d1 == null || viewPort3d1.EffectsManager == null)
            {
                return;
            }

            // If 3D tab is not active, queue refresh and do it when user opens Graphics 3D.
            if (tabControl2 == null || tabControl2.SelectedItem != tabItem6)
            {
                _pending3DRefresh = true;
                return;
            }

            if (_is3DRefreshInProgress) return;
            _is3DRefreshInProgress = true;

            bool showLoading = forceShowLoading || !_hasRendered3DOnce;
            if (showLoading)
            {
                Show3DLoading("Updating 3D tank filling...");
            }
            var refreshTimer = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromMilliseconds(250) };
            refreshTimer.Tick += (s, e) =>
            {
                refreshTimer.Stop();
                Refresh3dNew();
            };
            refreshTimer.Start();
        }

        /// <summary>
        /// Constructor for RealModeMain.xaml
        /// </summary>
        public RealModeMain()
        {
            try
            {
                InitializeComponent();
                EnsureHost2DInteractionHooks();
                MainWindow._statusTabRealSimulation = 0;
                Models.TableModel.Write_Log("Enter in Real MOde main");
                maxVolume = new Dictionary<int, decimal>();
                DataTable dtmaxVolume = new DataTable();
                DataView DV = Models.BO.clsGlobVar.dtRealModeAllTanks.AsDataView();
                DV.RowFilter = "[GROUP] not like 'Variable Data'";
                DV.RowFilter = "[GROUP] NOT LIKE 'Lightship'";
                dtmaxVolume = DV.ToTable();

                for (int i = 0; i < dtmaxVolume.Rows.Count-3; i++)
                {
                    maxVolume.Add(Convert.ToInt32(dtmaxVolume.Rows[i]["Tank_ID"]), Convert.ToDecimal(dtmaxVolume.Rows[i]["Max_Volume"]));

                }

                //BackGroundWorker for Updating labels and Datagrid on DispatcherTimer Tick
                bw = new BackgroundWorker();
                bw.DoWork += new DoWorkEventHandler(bw_DoWork);
                bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler
                        (bw_RunWorkerCompleted);

                //Dispatcher Timer for Executing Backgroundworker and refereshing RealModeMain page controls every 10 seconds

                //........@SP 12012016..............
                System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
                dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
                dispatcherTimer.Start();
                dispatcherTimer.Interval = new TimeSpan(0, 0, 10);
                //..................................

                //Logic for reading .stl file from given folder and adding 3d model to HelixViewPort3D i.e. viewPort3d 
            
                //Logic for showing 2D autocad images on Canvas 
                canvas2DProfile.Children.Clear();
                Model(Models.BO.clsGlobVar.Profile, canvas2DProfile);
                canvas2DPlanA.Children.Clear();
                Model(Models.BO.clsGlobVar.PlanA, canvas2DPlanA);
                canvas2DPlanB.Children.Clear();
                Model(Models.BO.clsGlobVar.PlanB, canvas2DPlanB);
                canvas2DPlanC.Children.Clear();
                Model(Models.BO.clsGlobVar.PlanC, canvas2DPlanC);
                canvas2DPlanALL.Children.Clear();
                ModelNew(Models.BO.clsGlobVar.PlanALL, canvas2DPlanALL);

                dgVariableItems.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new System.Action(delegate()
                {
                    dgVariableItems.ItemsSource = Models.BO.clsGlobVar.dtRealVariableItems.DefaultView;
                }));
                dgCargoTanks.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new System.Action(delegate()
                {
                    if (Models.BO.clsGlobVar.dtRealCargoTanks != null)
                    {
                        dgCargoTanks.ItemsSource = Models.BO.clsGlobVar.dtRealCargoTanks.DefaultView;
                    }
                }));
            }
            catch //(Exception ex)
            {
            }
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            //Checking whether background worker IsBusy
            if (!bw.IsBusy)
                bw.RunWorkerAsync();
            else
                _pendingRealModeRefresh = true;
        }

        private void StartAutoRefresh()
        {
            _forceRealModeCalculation = true;
            if (bw != null && !bw.IsBusy)
            {
                SetCalculationProgress("Calculating", 15);
                bw.RunWorkerAsync();
            }
            else
            {
                _pendingRealModeRefresh = true;
            }
        }

        private void SetCalculationProgress(string status, double value)
        {
            try
            {
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new System.Action(delegate()
                {
                    if (txtProgressStatus != null)
                    {
                        txtProgressStatus.Text = status;
                        txtProgressStatus.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(31, 72, 255));
                    }
                    if (progressStatusDot != null)
                    {
                        progressStatusDot.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(31, 72, 255));
                    }
                    if (pRGSCalculation != null)
                    {
                        pRGSCalculation.Value = value;
                    }
                    if (txtProgressPercent != null)
                    {
                        txtProgressPercent.Text = $"{Math.Max(0, Math.Min(100, (int)Math.Round(value)))}%";
                    }
                }));
            }
            catch
            {
            }
        }

        private void SetCalculationReady()
        {
            try
            {
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new System.Action(delegate()
                {
                    if (txtProgressStatus != null)
                    {
                        txtProgressStatus.Text = "Completed";
                        txtProgressStatus.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(16, 185, 129));
                    }
                    if (progressStatusDot != null)
                    {
                        progressStatusDot.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(16, 185, 129));
                    }
                    if (pRGSCalculation != null)
                    {
                        pRGSCalculation.Value = 100;
                    }
                    if (txtProgressPercent != null)
                    {
                        txtProgressPercent.Text = "100%";
                    }
                }));
            }
            catch
            {
            }
        }

        private static string BuildVolumeSignature(DataTable table)
        {
            if (table == null || table.Rows.Count == 0)
            {
                return string.Empty;
            }

            StringBuilder signature = new StringBuilder();
            foreach (DataRow row in table.Rows)
            {
                if (!table.Columns.Contains("Tank_ID") || !table.Columns.Contains("Volume"))
                {
                    continue;
                }

                signature.Append(row["Tank_ID"]);
                signature.Append(':');
                signature.Append(row["Volume"]);
                signature.Append(';');
            }

            return signature.ToString();
        }

        private void RunRealModeCalculation()
        {
            try
            {
                string Err = "";
                DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
                command.CommandText = "spCal_RealMode_Stability";
                command.CommandType = CommandType.StoredProcedure;

                DbParameter user = Models.DAL.clsDBUtilityMethods.GetParameter();
                user.DbType = DbType.String;
                user.ParameterName = "@User";
                user.Value = "dbo";
                command.Parameters.Add(user);

                DbParameter result = Models.DAL.clsDBUtilityMethods.GetParameter();
                result.DbType = DbType.Int16;
                result.Direction = ParameterDirection.Output;
                result.ParameterName = "@Stability_Calculation_Result";
                command.Parameters.Add(result);

                Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);
                if (result.Value != null && result.Value != DBNull.Value)
                {
                    Models.BO.clsGlobVar.CalculationResult = Convert.ToInt32(result.Value);
                }
            }
            catch
            {
            }
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
         {
            //Below Code Updates Controls(Labels and DataGrids) of RealModeMain Page from background Worker(Different Thread)
            try
             {
                Models.TableModel.Write_Log("Enter in Real MOde main Do_work");
                Models.TableModel.RealModeData();
                string currentVolumeSignature = BuildVolumeSignature(Models.BO.clsGlobVar.dtRealModeAllTanks);
                bool shouldCalculate = _forceRealModeCalculation || currentVolumeSignature != _lastRealModeVolumeSignature;
                if (shouldCalculate)
                {
                    SetCalculationProgress("Calculating", 45);
                    RunRealModeCalculation();
                    _lastRealModeVolumeSignature = currentVolumeSignature;
                    _forceRealModeCalculation = false;
                    SetCalculationProgress("Updating", 75);
                    Models.TableModel.RealModeData();
                }
                Models.TableModel.RealModePercentFill();
                lblDisplacement.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new System.Action(delegate()
                {
                    Schedule3DRefresh(); // Refresh 3D model with new data
                    lblDisplacement.Content = Convert.ToString(Math.Round(Convert.ToDouble(Models.BO.clsGlobVar.dtRealEquillibriumValues.Rows[0]["Displacement"])));
                    //decimal varDisp = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealEquillibriumValues.Rows[0]["Displacement"]);
                    //lblDisplacement.Content =varDisp.ToString("N");
                }));
                lblGMT.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new System.Action(delegate()
                {
                    //lblGMT.Content = Convert.ToString(Math.Round(Convert.ToDouble(Models.BO.clsGlobVar.dtRealEquillibriumValues.Rows[0]["GMT"]), 2));

                   decimal varGMT =Convert.ToDecimal(Models.BO.clsGlobVar.dtRealEquillibriumValues.Rows[0]["GMT"]);
                   lblGMT.Content = varGMT.ToString("N3");

                }));
                lblDraftAP.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new System.Action(delegate()
                {
                    //lblDraftAP.Content = Convert.ToString(Math.Round((Convert.ToDouble(Models.BO.clsGlobVar.dtRealEquillibriumValues.Rows[0]["Draft_AP"])), 2));
                    decimal varap = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealEquillibriumValues.Rows[0]["Draft_AP"]);
                    lblDraftAP.Content = varap.ToString("N3");

                }));
                lblDraftFP.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new System.Action(delegate()
                {
                   // lblDraftFP.Content = Convert.ToString(Math.Round((Convert.ToDouble(Models.BO.clsGlobVar.dtRealEquillibriumValues.Rows[0]["Draft_FP"])), 2));

                    decimal varfp = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealEquillibriumValues.Rows[0]["Draft_FP"]);
                    lblDraftFP.Content = varfp.ToString("N3");
                }));
                lblDraftAftMark.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new System.Action(delegate()
                {
                    //lblDraftAftMark.Content = Convert.ToString(Math.Round((Convert.ToDouble(Models.BO.clsGlobVar.dtRealEquillibriumValues.Rows[0]["Draft_Aft_Mark"])), 2));
                    decimal varaftmark = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealEquillibriumValues.Rows[0]["Draft_Aft_Mark"]);
                    lblDraftAftMark.Content = varaftmark.ToString("N3");
                }));
                lblDraftFwdMark.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new System.Action(delegate()
                {
                    //lblDraftFwdMark.Content = Convert.ToString(Math.Round((Convert.ToDouble(Models.BO.clsGlobVar.dtRealEquillibriumValues.Rows[0]["Draft_Fore_Mark"])), 2));

                    decimal varfwmark = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealEquillibriumValues.Rows[0]["Draft_Fore_Mark"]);
                    lblDraftFwdMark.Content = varfwmark.ToString("N3");
                }));
                lblDraftMean.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new System.Action(delegate()
                {
                   // lblDraftMean.Content = Convert.ToString(Math.Round((Convert.ToDouble(Models.BO.clsGlobVar.dtRealEquillibriumValues.Rows[0]["Draft_Mean"])), 2));

                    decimal var = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealEquillibriumValues.Rows[0]["Draft_Mean"]);
                    lblDraftMean.Content = var.ToString("N3");
                }));

                lblSONARDOME.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new System.Action(delegate()
                    {
                       // lblSONARDOME.Content = Convert.ToString(Math.Round((Convert.ToDouble(Models.BO.clsGlobVar.dtRealDrafts.Rows[0]["Draft_Sonar_Dome"])), 2));
                        decimal val = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealDrafts.Rows[0]["Draft_Sonar_Dome"]);
                        lblSONARDOME.Content = val.ToString("N3");
                    }));


                lblPROPELLER.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new System.Action(delegate()
                {
                    //lblPROPELLER.Content = Convert.ToString(Math.Round((Convert.ToDouble(Models.BO.clsGlobVar.dtRealDrafts.Rows[0]["Draft_Propeller"])), 2));
                    decimal varPropeller = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealDrafts.Rows[0]["Draft_Propeller"]);
                    lblPROPELLER.Content = varPropeller.ToString("N3");
                }));

                lblPROPELLER.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new System.Action(delegate()
                {
                    //lblKG.Content = Convert.ToString(Math.Round((Convert.ToDouble(Models.BO.clsGlobVar.dtRealEquillibriumValues.Rows[0]["KG(Solid)"])), 2));

                    decimal varKGS = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealEquillibriumValues.Rows[0]["KG(Solid)"]);
                    lblKG.Content = varKGS.ToString("N3");
                }));

                lblPROPELLER.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new System.Action(delegate()
                {
                    //lblKGF.Content = Convert.ToString(Math.Round((Convert.ToDouble(Models.BO.clsGlobVar.dtRealEquillibriumValues.Rows[0]["KG(Fluid)"])), 2));
                    decimal varKGF = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealEquillibriumValues.Rows[0]["KG(Fluid)"]);
                    lblKGF.Content = varKGF.ToString("N3");
                }));


                lblPROPELLER.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new System.Action(delegate()
                {
                    //lblLCG.Content = Convert.ToString(Math.Round((Convert.ToDouble(Models.BO.clsGlobVar.dtRealEquillibriumValues.Rows[0]["LCG"])), 2));
                    decimal varLCG = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealEquillibriumValues.Rows[0]["LCG"]);
                    lblLCG.Content = varLCG.ToString("N3");
                }));

                lblPROPELLER.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new System.Action(delegate()
                {
                    lblFSC.Content = Convert.ToString(Math.Round((Convert.ToDouble(Models.BO.clsGlobVar.dtRealEquillibriumValues.Rows[0]["FSC"])), 2));
                    decimal val = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealEquillibriumValues.Rows[0]["FSC"]);
                    lblFSC.Content = val.ToString("N3");
                         
                }));

                lblPROPELLER.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new System.Action(delegate()
                {
                    lblTPC.Content = Convert.ToString(Math.Round((Convert.ToDouble(Models.BO.clsGlobVar.dtRealEquillibriumValues.Rows[0]["TPC"])), 3));
                }));

                lblPROPELLER.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new System.Action(delegate()
                {
                    lblMTC.Content = Convert.ToString(Math.Round((Convert.ToDouble(Models.BO.clsGlobVar.dtRealEquillibriumValues.Rows[0]["MCT"])), 2));
                }));

                lblHeel.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new System.Action(delegate()
                {
                    label8.Content = " ";

                    if (Convert.ToDouble(Models.BO.clsGlobVar.dtRealEquillibriumValues.Rows[0]["Heel"]) > 0)
                    {
                        label8.Content = "PORT";

                       // lblHeel.Content = Convert.ToString(Math.Round((Convert.ToDouble( Models.BO.clsGlobVar.dtRealEquillibriumValues.Rows[0]["Heel"])),2));

                        decimal varHeelPort = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealEquillibriumValues.Rows[0]["Heel"]);
                        lblHeel.Content = varHeelPort.ToString("N3");
                    }
                    else if (Convert.ToDouble(Models.BO.clsGlobVar.dtRealEquillibriumValues.Rows[0]["Heel"]) < 0)
                    {
                        label8.Content = "STBD";
                        Double abs = (Convert.ToDouble(Models.BO.clsGlobVar.dtRealEquillibriumValues.Rows[0]["Heel"]));
                        //lblHeel.Content = Convert.ToString((-1) * abs);

                       // lblHeel.Content =   Convert.ToString(Math.Round( Convert.ToDouble(Convert.ToString((-1) * abs)),2));
                       decimal varheelSTBD =Convert.ToDecimal(Convert.ToString(Math.Round(Convert.ToDouble(Convert.ToString((-1) * abs)), 2)));
                       lblHeel.Content = varheelSTBD.ToString("N3");
                    }
                    else
                    {
                        label8.Content = " ";
                        //lblHeel.Content = Convert.ToString(Models.BO.clsGlobVar.dtRealEquillibriumValues.Rows[0]["Heel"]);
                        decimal varHeel = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealEquillibriumValues.Rows[0]["Heel"]);
                        lblHeel.Content = varHeel.ToString("N3");

                    }

                }));
                lblTrim.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new System.Action(delegate()
                {
                    if (Convert.ToDouble(Models.BO.clsGlobVar.dtRealEquillibriumValues.Rows[0]["Trim"]) > 0)
                    {
                        label5.Content = "FWD";

                        //  lblTrim.Content = Convert.ToString(Math.Round((Convert.ToDouble(Models.BO.clsGlobVar.dtRealEquillibriumValues.Rows[0]["TRIM"])), 2));
                        decimal VartrimAft = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealEquillibriumValues.Rows[0]["TRIM"]);
                        lblTrim.Content = VartrimAft.ToString("N3");
                    }
                    else if (Convert.ToDouble(Models.BO.clsGlobVar.dtRealEquillibriumValues.Rows[0]["Trim"]) < 0)
                    {
                        label5.Content = "AFT";
                        Double abs = Math.Round((Convert.ToDouble(Models.BO.clsGlobVar.dtRealEquillibriumValues.Rows[0]["TRIM"])),2);
                        //lblTrim.Content = Convert.ToString((-1) * abs); 

                        decimal vartrimfwd = Convert.ToDecimal(Convert.ToString((1) * abs));
                        lblTrim.Content = vartrimfwd.ToString("N3");
                    }
                    else
                    {
                        label5.Content = " ";
                        //lblTrim.Content = Convert.ToString((Models.BO.clsGlobVar.dtRealEquillibriumValues.Rows[0]["TRIM"]));

                        decimal VartrimAft = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealEquillibriumValues.Rows[0]["TRIM"]);
                        lblTrim.Content = VartrimAft.ToString("N3");
                    }


                }));

                dgBallastTanks.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new System.Action(delegate()
                {
                    dgBallastTanks.ItemsSource = Models.BO.clsGlobVar.dtRealBallastTanks.DefaultView;
                }));
                dgCargoTanks.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new System.Action(delegate()
                {
                    if (Models.BO.clsGlobVar.dtRealCargoTanks != null)
                    {
                        dgCargoTanks.ItemsSource = Models.BO.clsGlobVar.dtRealCargoTanks.DefaultView;
                    }
                }));
                dgFuelOilTanks.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new System.Action(delegate()
                {
                    dgFuelOilTanks.ItemsSource = Models.BO.clsGlobVar.dtRealFuelOilTanks.DefaultView;
                }));
                dgFreshWaterTanks.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new System.Action(delegate()
                {
                    dgFreshWaterTanks.ItemsSource = Models.BO.clsGlobVar.dtRealFreshWaterTanks.DefaultView;
                }));
                dgMiscTanks.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new System.Action(delegate()
                {
                    dgMiscTanks.ItemsSource = Models.BO.clsGlobVar.dtRealMiscTanks.DefaultView;
                }));

                dgCompartments.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new System.Action(delegate()
                {
                    dgCompartments.ItemsSource = Models.BO.clsGlobVar.dtRealCompartments.DefaultView;
                }));

                dgWaterTightRegion.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new System.Action(delegate()
                    {
                        dgWaterTightRegion.ItemsSource = Models.BO.clsGlobVar.dtRealWaterTightRegion.DefaultView;

                    }));
            }
            catch //(Exception ex)
            {
                //System.Windows.MessageBox.Show(ex.ToString());
            }

        }

        public void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
               
                Models.TableModel.Write_Log("Enter in Real MOde DO_RunWorkerCompleted");
                string Err = "";
                string query = "";
                for (int TankId = 1; TankId < Models.BO.clsGlobVar.dtRealModeAllTanks.Rows.Count; TankId++)
                {
                    if (Convert.ToBoolean(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[TankId - 1]["IsSensorFaulty"]) == false)
                    {
                        query += " update tblLoading_Condition set IsManualEntry='" + Convert.ToBoolean(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[TankId - 1]["IsSensorFaulty"]) + "' where Tank_ID=" + TankId;
                    }
                }

                DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
                command.CommandText = query;
                command.CommandType = CommandType.Text;
                Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);

                AddHatchProfile();
                AddHatchDeckPlanA();
                AddHatchDeckPlanB();
                //AddHatchDeckPlanC();
                ////SimulationModeMain sww = new SimulationModeMain();
                ////sww.Refresh3dNew();

                // 3D models are loaded in viewPort3d_Loaded after EffectsManager is ready
                //Refresh3dNew();
            }
            
            catch//(Exception ex)
                 {
               // System.Windows.MessageBox.Show(ex.ToString());
            }
           Models.TableModel.Write_Log("End in Real MOde DO_RunWorkerCompleted");
           SetCalculationReady();
           if (_pendingRealModeRefresh)
           {
               _pendingRealModeRefresh = false;
               StartAutoRefresh();
           }

        }


        /// <summary>
        /// Validation Code for Datagrid to enter only decimal numbers with only single decimal point
        /// </summary>
        private void dgVariableItems_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            tb = e.Column.GetCellContent(e.Row) as TextBox;
        }

        private void dgVariableItems_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (header != "Item Name")
            {
                if (!char.IsDigit(e.Text, e.Text.Length - 1)
                     && !e.Text.Contains('.'))
                {
                    e.Handled = true;
                }
                // only allow one decimal point
                if (e.Text.Contains('.')
                    && tb.Text.IndexOf('.') > -1)
                {
                    e.Handled = true;
                }
            }
        }



        /// <summary>
        /// Initialize AutoCAD 2D model on canvas
        /// </summary>
        /// <param name="model">DxfModel or AUTOCAD 2D</param> 
        ///<param name="canvas2D">Canvas.</param> 
        ///<returns> Initialize AutoCAD 2D model on canvas.</returns> 
        public void Model(DxfModel model, Canvas canvas2D)
        {
            if (model != null)
            {

                DxfLayout paperSpaceLayout = model.ActiveLayout;
                if (model.Header.ShowModelSpace)
                {
                    paperSpaceLayout = null;
                }

                #region calculate the model's bounds to determine a proper dots per inch

                // The dots per inch value is important because it determines the eventual pen thickness.
                graphicsConfig = (GraphicsConfig)GraphicsConfig.WhiteBackgroundCorrectForBackColor.Clone();
                //GraphicsConfig.
                BoundsCalculator boundsCalculator = new BoundsCalculator();
                if (model.ActiveLayout == null || model.Header.ShowModelSpace)
                {
                    boundsCalculator.GetBounds(model);
                }
                else
                {
                    boundsCalculator.GetBounds(model, model.ActiveLayout);
                }
                bounds = boundsCalculator.Bounds;
                if (canvas2D == canvas2DProfile) boundsProfile = bounds;
                else if (canvas2D == canvas2DPlanA) boundsPlanA = bounds;
                else if (canvas2D == canvas2DPlanB) boundsPlanB = bounds;
                else if (canvas2D == canvas2DPlanC) boundsPlanC = bounds;
                else if (canvas2D == canvas2DPlanALL) boundsPlanALL = bounds;
                WW.Math.Vector3D delta = bounds.Delta;
                System.Windows.Size estimatedCanvasSize = new System.Windows.Size(200d, 200d);
                double estimatedScale = Math.Min(estimatedCanvasSize.Width / delta.X, estimatedCanvasSize.Height / delta.Y);
                graphicsConfig.DotsPerInch = 20d / estimatedScale;
                BoundsCalculator boundsCalculator1 = new BoundsCalculator();
                boundsCalculator1.GetBounds(model, model.Entities[20]);
                #endregion

                graphicsCache = new WireframeGraphics2Cache(false, false);
                graphicsCache.Config = graphicsConfig;
                if (model.ActiveLayout == null || model.Header.ShowModelSpace)
                {
                    graphicsCache.CreateDrawables(model, Matrix4D.Identity);

                }
                else
                {
                    graphicsCache.CreateDrawables(model, model.ActiveLayout);
                }

                wpfGraphics = new WpfWireframeGraphics3DUsingDrawingVisual();
                wpfGraphics.Config = graphicsConfig;

                wpfGraphics.Canvas.IsHitTestVisible = false;
                canvas2D.Children.Add(wpfGraphics.Canvas);
                Hook2DInteraction(canvas2D);

                UpdateWpfGraphics();
                canvas2D.SizeChanged += canvas_SizeChanged;

            }
        }

        private void UpdateWpfGraphics()
        {
            wpfGraphics.DrawingVisuals.Clear();
            IWireframeGraphicsFactory2 graphicsFactory = wpfGraphics.CreateGraphicsFactory();
            foreach (IWireframeDrawable2 drawable in graphicsCache.Drawables)
            {
                drawable.Draw(graphicsFactory);
            }
        }

        /// <summary>
        /// Update the canvas RenderTransform.
        /// </summary>
        private void canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender == canvas2DProfile)
            {
                UpdateRenderTransform(canvas2DProfile);
            }

            else if (sender == canvas2DPlanA)
            {
                UpdateRenderTransform(canvas2DPlanA);
            }
            else if (sender == canvas2DPlanB)
            {
                UpdateRenderTransform(canvas2DPlanB);
            }
            else if (sender == canvas2DPlanC)
            {
                UpdateRenderTransform(canvas2DPlanC);
            }
            else if (sender == canvas2DPlanALL)
            {
                UpdateRenderTransform(canvas2DPlanALL);
            }

        }

        private void UpdateRenderTransform(Canvas canvas2D)
        {

            //@MT Code added for Ship Profile Image Rotate :START
            double AP, FP;
            AP = Convert.ToDouble(lblDraftAP.Content);
            FP = Convert.ToDouble(lblDraftFP.Content);
            double angle = ((FP - AP) / 151.5) * (180 / Math.PI);
            //@MT Code added for Ship Profile Image Rotate :END

            Bounds3D activeBounds;
            if (canvas2D == canvas2DProfile && boundsProfile != null) activeBounds = boundsProfile;
            else if (canvas2D == canvas2DPlanA && boundsPlanA != null) activeBounds = boundsPlanA;
            else if (canvas2D == canvas2DPlanB && boundsPlanB != null) activeBounds = boundsPlanB;
            else if (canvas2D == canvas2DPlanC && boundsPlanC != null) activeBounds = boundsPlanC;
            else if (canvas2D == canvas2DPlanALL && boundsPlanALL != null) activeBounds = boundsPlanALL;
            else activeBounds = bounds;

            double canvasWidth = canvas2D.ActualWidth;
            double canvasHeight = canvas2D.ActualHeight;
            double contentMinY = activeBounds.Corner1.Y;
            double contentMaxY = activeBounds.Corner2.Y;
            if (contentMinY < 0d && Math.Abs(contentMinY) > Math.Abs(contentMaxY) * 3d)
            {
                contentMinY = 0d;
            }

            double contentHeight = Math.Max(1d, contentMaxY - contentMinY);
            double verticalPadding = (canvas2D == canvas2DProfile) ? 0.015d : 0.02d;
            Point2D effectiveCorner1 = new Point2D(activeBounds.Corner1.X, contentMinY - (contentHeight * verticalPadding));
            Point2D effectiveCorner2 = new Point2D(activeBounds.Corner2.X, contentMaxY + (contentHeight * verticalPadding));
            Point2D effectiveCenter = new Point2D(activeBounds.Center.X, (effectiveCorner1.Y + effectiveCorner2.Y) / 2d);
            MatrixTransform baseTransform = DxfUtil.GetScaleWMMatrixTransform(
                effectiveCorner1,
                effectiveCorner2,
                effectiveCenter,
                new Point2D(1d, canvasHeight),
                new Point2D(canvasWidth, 1d),
                new Point2D(0.5d * (canvasWidth + 1d), 0.5d * (canvasHeight + 1d))
                );

            TransformGroup transformGroup = new TransformGroup();
            transformGroup.Children.Add(baseTransform);
            transformGroup.Children.Add(new TranslateTransform()
            {
                X = -canvasWidth / 2d,
                Y = -canvasHeight / 2d
            });
            double fitScale = (canvas2D == canvas2DProfile) ? 1.35d : 1.18d;
            transformGroup.Children.Add(new ScaleTransform()
            {
                ScaleX = scaling * fitScale,
                ScaleY = scaling * fitScale
            });
            transformGroup.Children.Add(new TranslateTransform()
            {
                X = canvasWidth / 2d,
                Y = canvasHeight / 2d
            });

            // Auto-center content vertically so first-load and reset land in the visual middle.
            double autoOffsetY = 0d;
            try
            {
                var t1 = transformGroup.Transform(new System.Windows.Point(activeBounds.Corner1.X, activeBounds.Corner1.Y));
                var t2 = transformGroup.Transform(new System.Windows.Point(activeBounds.Corner2.X, activeBounds.Corner2.Y));
                double modelCenterY = (Math.Min(t1.Y, t2.Y) + Math.Max(t1.Y, t2.Y)) * 0.5d;
                autoOffsetY = (canvasHeight * 0.5d) - modelCenterY;
            }
            catch { }

            transformGroup.Children.Add(new TranslateTransform()
            {
                X = translation.X * canvasWidth / 2d,
                Y = (-translation.Y * canvasHeight / 2d) + autoOffsetY + (canvasHeight * 0.24d)
            });

            //@MT Code removed for Ship Profile Image Rotate (kept level as requested)
            /*
            if (canvas2D == canvas2DProfile)
            {
                transformGroup.Children.Add(new RotateTransform()
                {
                    Angle = angle,
                    CenterX = canvasWidth / 2d,
                    CenterY = canvasHeight / 2d
                });
            }
            */

            canvas2D.RenderTransform = transformGroup;

        }

        private void Canvas2D_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            // 2D zoom disabled as requested.
            e.Handled = true;
        }

        private void btnReset2D_Click(object sender, RoutedEventArgs e)
        {
            scaling = 1.0d;
            translation = new WW.Math.Vector3D(0, 0, 0);
            UpdateAll2DTransforms();
            Log2D("Reset clicked -> scaling=1.0 translation=0,0");
        }

        private void Canvas2D_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 2D pan disabled as requested.
            e.Handled = true;
        }

        private void Canvas2D_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // 2D pan disabled as requested.
            e.Handled = true;
        }

        private void Canvas2D_MouseMove(object sender, MouseEventArgs e)
        {
            // 2D pan disabled as requested.
            e.Handled = true;
        }

        private void UpdateAll2DTransforms()
        {
            UpdateRenderTransform(canvas2DProfile);
            UpdateRenderTransform(canvas2DPlanA);
            UpdateRenderTransform(canvas2DPlanB);
            UpdateRenderTransform(canvas2DPlanC);
            UpdateRenderTransform(canvas2DPlanALL);
        }

        private Canvas GetInteractionCanvas(object sender)
        {
            DependencyObject current = sender as DependencyObject;
            while (current != null)
            {
                if (current == canvas2DProfile) return canvas2DProfile;
                if (current == canvas2DPlanA) return canvas2DPlanA;
                if (current == canvas2DPlanB) return canvas2DPlanB;
                if (current == canvas2DPlanC) return canvas2DPlanC;
                if (current == canvas2DPlanALL) return canvas2DPlanALL;
                current = VisualTreeHelper.GetParent(current);
            }
            return null;
        }

        private Canvas GetActive2DCanvas()
        {
            try
            {
                if (tabControl5?.SelectedIndex == 0) return canvas2DProfile;
                if (tabControl5?.SelectedIndex == 1) return canvas2DPlanA;
                if (tabControl5?.SelectedIndex == 2) return canvas2DPlanB;
            }
            catch { }
            return canvas2DProfile ?? canvas2DPlanA ?? canvas2DPlanB;
        }

        private void Hook2DInteraction(UIElement element)
        {
            if (element == null)
            {
                return;
            }

            if (element is Panel panel && panel.Background == null)
            {
                panel.Background = Brushes.Transparent;
            }

            element.PreviewMouseWheel -= Canvas2D_MouseWheel;
            element.PreviewMouseLeftButtonDown -= Canvas2D_MouseLeftButtonDown;
            element.PreviewMouseLeftButtonUp -= Canvas2D_MouseLeftButtonUp;
            element.PreviewMouseMove -= Canvas2D_MouseMove;
            element.PreviewMouseWheel += Canvas2D_MouseWheel;
            element.PreviewMouseLeftButtonDown += Canvas2D_MouseLeftButtonDown;
            element.PreviewMouseLeftButtonUp += Canvas2D_MouseLeftButtonUp;
            element.PreviewMouseMove += Canvas2D_MouseMove;
        }

        //--------------------------------------------------------------------------------
        public void ModelNew(DxfModel model, Canvas canvas2D)
        {
            if (model != null)
            {

                DxfLayout paperSpaceLayout = model.ActiveLayout;
                if (model.Header.ShowModelSpace)
                {
                    paperSpaceLayout = null;

                }

                #region calculate the model's bounds to determine a proper dots per inch

                // The dots per inch value is important because it determines the eventual pen thickness.
                graphicsConfigNew = (GraphicsConfig)GraphicsConfig.WhiteBackgroundCorrectForBackColor.Clone();
                BoundsCalculator boundsCalculator = new BoundsCalculator();
                if (model.ActiveLayout == null || model.Header.ShowModelSpace)
                {
                    boundsCalculator.GetBounds(model);
                }
                else
                {
                    boundsCalculator.GetBounds(model, model.ActiveLayout);
                }
                boundsNew = boundsCalculator.Bounds;
                WW.Math.Vector3D delta = boundsNew.Delta;
                System.Windows.Size estimatedCanvasSize = new System.Windows.Size(200d, 200d);
                double estimatedScale = Math.Min(estimatedCanvasSize.Width / delta.X, estimatedCanvasSize.Height / delta.Y);
                graphicsConfigNew.DotsPerInch = 20d / estimatedScale;
                BoundsCalculator boundsCalculator1 = new BoundsCalculator();
                boundsCalculator1.GetBounds(model, model.Entities[20]);
                #endregion

                graphicsCacheNew = new WireframeGraphics2Cache(false, false);
                graphicsCacheNew.Config = graphicsConfigNew;
                if (model.ActiveLayout == null || model.Header.ShowModelSpace)
                {
                    graphicsCacheNew.CreateDrawables(model, Matrix4D.Identity);

                }
                else
                {
                    graphicsCacheNew.CreateDrawables(model, model.ActiveLayout);
                }

                wpfGraphicsNew = new WpfWireframeGraphics3DUsingDrawingVisual();
                wpfGraphicsNew.Config = graphicsConfigNew;

                wpfGraphicsNew.Canvas.IsHitTestVisible = false;
                canvas2D.Children.Add(wpfGraphicsNew.Canvas);

                UpdateWpfGraphicsNew();

                canvas2D.SizeChanged += canvas_SizeChangedNew;

            }
        }
        private void UpdateWpfGraphicsNew()
        {

            wpfGraphicsNew.DrawingVisuals.Clear();
            IWireframeGraphicsFactory2 graphicsFactory = wpfGraphicsNew.CreateGraphicsFactory();
            foreach (IWireframeDrawable2 drawable in graphicsCacheNew.Drawables)
            {
                drawable.Draw(graphicsFactory);

            }

        }
        /// <summary>
        /// Update the canvas RenderTransform.
        /// </summary>
        private void canvas_SizeChangedNew(object sender, SizeChangedEventArgs e)
        {
            UpdateRenderTransformNew(canvas2DPlanALL);
        }

        private void UpdateRenderTransformNew(Canvas canvas2D)
        {

            double canvasWidth = canvas2D.ActualWidth;
            double canvasHeight = canvas2D.ActualHeight;
            MatrixTransform baseTransform = DxfUtil.GetScaleWMMatrixTransform(
                (Point2D)boundsNew.Corner1,
                (Point2D)boundsNew.Corner2,
                (Point2D)boundsNew.Center,
                new Point2D(1d, canvasHeight),
                new Point2D(canvasWidth, 1d),
                new Point2D(0.5d * (canvasWidth + 1d), 0.5d * (canvasHeight + 1d))
                );

            TransformGroup transformGroup = new TransformGroup();
            transformGroup.Children.Add(baseTransform);
            transformGroup.Children.Add(new TranslateTransform()
            {
                X = -canvasWidth / 2d,
                Y = -canvasHeight / 2d
            });
            transformGroup.Children.Add(new ScaleTransform()
            {
                ScaleX = scaling,
                ScaleY = scaling
            });
            transformGroup.Children.Add(new TranslateTransform()
            {
                X = canvasWidth / 2d,
                Y = canvasHeight / 2d
            });

            double autoOffsetY = 0d;
            try
            {
                var t1 = transformGroup.Transform(new System.Windows.Point(boundsNew.Corner1.X, boundsNew.Corner1.Y));
                var t2 = transformGroup.Transform(new System.Windows.Point(boundsNew.Corner2.X, boundsNew.Corner2.Y));
                double modelCenterY = (Math.Min(t1.Y, t2.Y) + Math.Max(t1.Y, t2.Y)) * 0.5d;
                autoOffsetY = (canvasHeight * 0.5d) - modelCenterY;
            }
            catch { }

            transformGroup.Children.Add(new TranslateTransform()
            {
                X = translation.X * canvasWidth / 2d,
                Y = (-translation.Y * canvasHeight / 2d) + autoOffsetY + (canvasHeight * 0.24d)
            });

            canvas2D.RenderTransform = transformGroup;

        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Add Hatch Filling of Tanks/Compartments to Profile Canvas as per TankID 
        /// </summary>
        public void AddHatchProfile()
        {
            try
            {
                canvas2DProfile.Children.RemoveRange(1, canvas2DProfile.Children.Count - 1);
                DrawHatchProfile(canvas2DProfile, 1, clsGlobVar.Tank1_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[1].X, clsGlobVar.ProfileCoordinate.Profiles[1].Y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                DrawHatchProfile(canvas2DProfile, 2, clsGlobVar.Tank2_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[2].X, clsGlobVar.ProfileCoordinate.Profiles[2].Y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                DrawHatchProfile(canvas2DProfile, 9, clsGlobVar.Tank9_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[9].X, clsGlobVar.ProfileCoordinate.Profiles[9].Y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                DrawHatchProfile(canvas2DProfile, 10, clsGlobVar.Tank10_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[10].X, clsGlobVar.ProfileCoordinate.Profiles[10].Y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                DrawHatchProfile(canvas2DProfile, 17, clsGlobVar.Tank17_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[17].X, clsGlobVar.ProfileCoordinate.Profiles[17].Y, System.Windows.Media.Color.FromArgb(180, 151, 72, 7));
                DrawHatchProfile(canvas2DProfile, 18, clsGlobVar.Tank18_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[18].X, clsGlobVar.ProfileCoordinate.Profiles[18].Y, System.Windows.Media.Color.FromArgb(180, 151, 72, 7));
                DrawHatchProfile(canvas2DProfile, 20, clsGlobVar.Tank20_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[20].X, clsGlobVar.ProfileCoordinate.Profiles[20].Y, System.Windows.Media.Color.FromArgb(180, 151, 72, 7));
                DrawHatchProfile(canvas2DProfile, 21, clsGlobVar.Tank21_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[21].X, clsGlobVar.ProfileCoordinate.Profiles[21].Y, System.Windows.Media.Color.FromArgb(180, 151, 72, 7));
                DrawHatchProfile(canvas2DProfile, 22, clsGlobVar.Tank22_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[22].X, clsGlobVar.ProfileCoordinate.Profiles[22].Y, System.Windows.Media.Color.FromArgb(180, 151, 72, 7));
                DrawHatchProfile(canvas2DProfile, 26, clsGlobVar.Tank26_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[26].X, clsGlobVar.ProfileCoordinate.Profiles[26].Y, System.Windows.Media.Color.FromArgb(180, 151, 72, 7));
                DrawHatchProfile(canvas2DProfile, 28, clsGlobVar.Tank28_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[28].X, clsGlobVar.ProfileCoordinate.Profiles[28].Y, System.Windows.Media.Color.FromArgb(180, 151, 72, 7));
                DrawHatchProfile(canvas2DProfile, 33, clsGlobVar.Tank33_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[33].X, clsGlobVar.ProfileCoordinate.Profiles[33].Y, System.Windows.Media.Color.FromArgb(180, 151, 72, 7));
                DrawHatchProfile(canvas2DProfile, 39, clsGlobVar.Tank39_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[39].X, clsGlobVar.ProfileCoordinate.Profiles[39].Y, System.Windows.Media.Color.FromArgb(180, 151, 72, 7));
                DrawHatchProfile(canvas2DProfile, 40, clsGlobVar.Tank40_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[40].X, clsGlobVar.ProfileCoordinate.Profiles[40].Y, System.Windows.Media.Color.FromArgb(180, 151, 72, 7));
                DrawHatchProfile(canvas2DProfile, 42, clsGlobVar.Tank42_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[42].X, clsGlobVar.ProfileCoordinate.Profiles[42].Y, System.Windows.Media.Color.FromArgb(180, 151, 72, 7));
                DrawHatchProfile(canvas2DProfile, 44, clsGlobVar.Tank44_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[44].X, clsGlobVar.ProfileCoordinate.Profiles[44].Y, System.Windows.Media.Color.FromArgb(180, 151, 72, 7));
                DrawHatchProfile(canvas2DProfile, 46, clsGlobVar.Tank46_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[46].X, clsGlobVar.ProfileCoordinate.Profiles[46].Y, System.Windows.Media.Color.FromArgb(180, 203, 150, 69));
                DrawHatchProfile(canvas2DProfile, 49, clsGlobVar.Tank49_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[49].X, clsGlobVar.ProfileCoordinate.Profiles[49].Y, System.Windows.Media.Color.FromArgb(180, 255, 192, 203));
                DrawHatchProfile(canvas2DProfile, 50, clsGlobVar.Tank50_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[50].X, clsGlobVar.ProfileCoordinate.Profiles[50].Y, System.Windows.Media.Color.FromArgb(180, 255, 192, 203));
                DrawHatchProfile(canvas2DProfile, 53, clsGlobVar.Tank53_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[53].X, clsGlobVar.ProfileCoordinate.Profiles[53].Y, System.Windows.Media.Color.FromArgb(180, 255, 192, 203));
                DrawHatchProfile(canvas2DProfile, 54, clsGlobVar.Tank54_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[54].X, clsGlobVar.ProfileCoordinate.Profiles[54].Y, System.Windows.Media.Color.FromArgb(180, 255, 192, 203));
                DrawHatchProfile(canvas2DProfile, 56, clsGlobVar.Tank56_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[56].X, clsGlobVar.ProfileCoordinate.Profiles[56].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchProfile(canvas2DProfile, 57, clsGlobVar.Tank57_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[57].X, clsGlobVar.ProfileCoordinate.Profiles[57].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchProfile(canvas2DProfile, 66, clsGlobVar.Tank66_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[66].X, clsGlobVar.ProfileCoordinate.Profiles[66].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchProfile(canvas2DProfile, 67, clsGlobVar.Tank67_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[67].X, clsGlobVar.ProfileCoordinate.Profiles[67].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchProfile(canvas2DProfile, 68, clsGlobVar.Tank68_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[68].X, clsGlobVar.ProfileCoordinate.Profiles[68].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchProfile(canvas2DProfile, 69, clsGlobVar.Tank69_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[69].X, clsGlobVar.ProfileCoordinate.Profiles[69].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchProfile(canvas2DProfile, 70, clsGlobVar.Tank70_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[70].X, clsGlobVar.ProfileCoordinate.Profiles[70].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchProfile(canvas2DProfile, 76, clsGlobVar.Tank76_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[76].X, clsGlobVar.ProfileCoordinate.Profiles[76].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchProfile(canvas2DProfile, 79, clsGlobVar.Tank79_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[79].X, clsGlobVar.ProfileCoordinate.Profiles[79].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchProfile(canvas2DProfile, 80, clsGlobVar.Tank80_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[80].X, clsGlobVar.ProfileCoordinate.Profiles[80].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchProfile(canvas2DProfile, 82, clsGlobVar.Tank82_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[82].X, clsGlobVar.ProfileCoordinate.Profiles[82].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchProfile(canvas2DProfile, 84, clsGlobVar.Tank84_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[84].X, clsGlobVar.ProfileCoordinate.Profiles[84].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 90, clsGlobVar.Tank90_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[90].X, clsGlobVar.ProfileCoordinate.Profiles[90].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 92, clsGlobVar.Tank92_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[92].X, clsGlobVar.ProfileCoordinate.Profiles[92].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 96, clsGlobVar.Tank96_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[96].X, clsGlobVar.ProfileCoordinate.Profiles[96].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 101, clsGlobVar.Tank101_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[101].X, clsGlobVar.ProfileCoordinate.Profiles[101].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 147, clsGlobVar.Tank147_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[147].X, clsGlobVar.ProfileCoordinate.Profiles[147].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 150, clsGlobVar.Tank150_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[150].X, clsGlobVar.ProfileCoordinate.Profiles[150].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 151, clsGlobVar.Tank151_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[151].X, clsGlobVar.ProfileCoordinate.Profiles[151].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 177, clsGlobVar.Tank177_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[177].X, clsGlobVar.ProfileCoordinate.Profiles[177].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 184, clsGlobVar.Tank184_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[184].X, clsGlobVar.ProfileCoordinate.Profiles[184].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 185, clsGlobVar.Tank185_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[185].X, clsGlobVar.ProfileCoordinate.Profiles[185].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 187, clsGlobVar.Tank187_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[187].X, clsGlobVar.ProfileCoordinate.Profiles[187].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 192, clsGlobVar.Tank192_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[192].X, clsGlobVar.ProfileCoordinate.Profiles[192].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 195, clsGlobVar.Tank195_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[195].X, clsGlobVar.ProfileCoordinate.Profiles[195].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 227, clsGlobVar.Tank227_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[227].X, clsGlobVar.ProfileCoordinate.Profiles[227].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 234, clsGlobVar.Tank234_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[234].X, clsGlobVar.ProfileCoordinate.Profiles[234].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 236, clsGlobVar.Tank236_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[236].X, clsGlobVar.ProfileCoordinate.Profiles[236].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 244, clsGlobVar.Tank244_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[244].X, clsGlobVar.ProfileCoordinate.Profiles[244].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 250, clsGlobVar.Tank250_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[250].X, clsGlobVar.ProfileCoordinate.Profiles[250].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 253, clsGlobVar.Tank253_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[253].X, clsGlobVar.ProfileCoordinate.Profiles[253].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 254, clsGlobVar.Tank254_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[254].X, clsGlobVar.ProfileCoordinate.Profiles[254].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 260, clsGlobVar.Tank260_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[260].X, clsGlobVar.ProfileCoordinate.Profiles[260].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 261, clsGlobVar.Tank261_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[261].X, clsGlobVar.ProfileCoordinate.Profiles[261].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 263, clsGlobVar.Tank263_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[263].X, clsGlobVar.ProfileCoordinate.Profiles[263].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 264, clsGlobVar.Tank264_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[264].X, clsGlobVar.ProfileCoordinate.Profiles[264].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 267, clsGlobVar.Tank267_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[267].X, clsGlobVar.ProfileCoordinate.Profiles[267].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 273, clsGlobVar.Tank273_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[273].X, clsGlobVar.ProfileCoordinate.Profiles[273].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 275, clsGlobVar.Tank275_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[275].X, clsGlobVar.ProfileCoordinate.Profiles[275].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 276, clsGlobVar.Tank276_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[276].X, clsGlobVar.ProfileCoordinate.Profiles[276].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 280, clsGlobVar.Tank280_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[280].X, clsGlobVar.ProfileCoordinate.Profiles[280].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 281, clsGlobVar.Tank281_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[281].X, clsGlobVar.ProfileCoordinate.Profiles[281].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 284, clsGlobVar.Tank284_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[284].X, clsGlobVar.ProfileCoordinate.Profiles[284].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 285, clsGlobVar.Tank285_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[285].X, clsGlobVar.ProfileCoordinate.Profiles[285].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 292, clsGlobVar.Tank292_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[292].X, clsGlobVar.ProfileCoordinate.Profiles[292].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 293, clsGlobVar.Tank293_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[293].X, clsGlobVar.ProfileCoordinate.Profiles[293].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 297, clsGlobVar.Tank297_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[297].X, clsGlobVar.ProfileCoordinate.Profiles[297].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 298, clsGlobVar.Tank298_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[298].X, clsGlobVar.ProfileCoordinate.Profiles[298].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 299, clsGlobVar.Tank299_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[299].X, clsGlobVar.ProfileCoordinate.Profiles[299].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 301, clsGlobVar.Tank301_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[301].X, clsGlobVar.ProfileCoordinate.Profiles[301].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 303, clsGlobVar.Tank303_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[303].X, clsGlobVar.ProfileCoordinate.Profiles[303].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 304, clsGlobVar.Tank304_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[304].X, clsGlobVar.ProfileCoordinate.Profiles[304].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 311, clsGlobVar.Tank311_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[311].X, clsGlobVar.ProfileCoordinate.Profiles[311].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 312, clsGlobVar.Tank312_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[312].X, clsGlobVar.ProfileCoordinate.Profiles[312].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 325, clsGlobVar.Tank325_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[325].X, clsGlobVar.ProfileCoordinate.Profiles[325].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 326, clsGlobVar.Tank326_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[326].X, clsGlobVar.ProfileCoordinate.Profiles[326].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 336, clsGlobVar.Tank336_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[336].X, clsGlobVar.ProfileCoordinate.Profiles[336].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 356, clsGlobVar.Tank356_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[356].X, clsGlobVar.ProfileCoordinate.Profiles[356].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 360, clsGlobVar.Tank360_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[360].X, clsGlobVar.ProfileCoordinate.Profiles[360].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 380, clsGlobVar.Tank380_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[380].X, clsGlobVar.ProfileCoordinate.Profiles[380].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 392, clsGlobVar.Tank392_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[392].X, clsGlobVar.ProfileCoordinate.Profiles[392].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 411, clsGlobVar.Tank411_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[411].X, clsGlobVar.ProfileCoordinate.Profiles[411].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 424, clsGlobVar.Tank424_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[424].X, clsGlobVar.ProfileCoordinate.Profiles[424].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 427, clsGlobVar.Tank427_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[427].X, clsGlobVar.ProfileCoordinate.Profiles[427].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 428, clsGlobVar.Tank428_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[428].X, clsGlobVar.ProfileCoordinate.Profiles[428].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 430, clsGlobVar.Tank430_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[430].X, clsGlobVar.ProfileCoordinate.Profiles[430].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 432, clsGlobVar.Tank432_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[432].X, clsGlobVar.ProfileCoordinate.Profiles[432].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 433, clsGlobVar.Tank433_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[433].X, clsGlobVar.ProfileCoordinate.Profiles[433].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 434, clsGlobVar.Tank434_PercentFill, clsGlobVar.ProfileCoordinate.Profiles[434].X, clsGlobVar.ProfileCoordinate.Profiles[434].Y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));



                UpdateRenderTransform(canvas2DProfile);

                DrawTrimLine();
            }
            catch
            {

            }
        }

        /// <summary>
        /// Add Hatch Filling of Tanks/Compartments to DeckPlans Canvas as per TankID 
        /// </summary>
        public void AddHatchDeckPlanB()
        {
            try
            {

                #region
                {
                    //    canvas2DPlanB.Children.RemoveRange(1, canvas2DPlanB.Children.Count - 1);
                    //    DrawHatchDeckPlan(canvas2DPlanB, 16, clsGlobVar.Tank16_PercentFill, clsGlobVar.CoordinatePlanB.Tank16x, clsGlobVar.CoordinatePlanB.Tank16y, System.Windows.Media.Color.FromArgb(180, 151, 72, 7));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 17, clsGlobVar.Tank17_PercentFill, clsGlobVar.CoordinatePlanB.Tank17x, clsGlobVar.CoordinatePlanB.Tank17y, System.Windows.Media.Color.FromArgb(180, 151, 72, 7));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 18, clsGlobVar.Tank18_PercentFill, clsGlobVar.CoordinatePlanB.Tank18x, clsGlobVar.CoordinatePlanB.Tank18y, System.Windows.Media.Color.FromArgb(180, 151, 72, 7));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 19, clsGlobVar.Tank19_PercentFill, clsGlobVar.CoordinatePlanB.Tank19x, clsGlobVar.CoordinatePlanB.Tank19y, System.Windows.Media.Color.FromArgb(180, 151, 72, 7));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 21, clsGlobVar.Tank21_PercentFill, clsGlobVar.CoordinatePlanB.Tank21x, clsGlobVar.CoordinatePlanB.Tank21y, System.Windows.Media.Color.FromArgb(180, 255, 192, 203));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 22, clsGlobVar.Tank22_PercentFill, clsGlobVar.CoordinatePlanB.Tank22x, clsGlobVar.CoordinatePlanB.Tank22y, System.Windows.Media.Color.FromArgb(180, 255, 192, 203));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 26, clsGlobVar.Tank26_PercentFill, clsGlobVar.CoordinatePlanB.Tank26x, clsGlobVar.CoordinatePlanB.Tank26y, System.Windows.Media.Color.FromArgb(180, 255, 192, 203));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 27, clsGlobVar.Tank27_PercentFill, clsGlobVar.CoordinatePlanB.Tank27x, clsGlobVar.CoordinatePlanB.Tank27y, System.Windows.Media.Color.FromArgb(180, 255, 192, 203));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 38, clsGlobVar.Tank38_PercentFill, clsGlobVar.CoordinatePlanB.Tank38x, clsGlobVar.CoordinatePlanB.Tank38y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 54, clsGlobVar.Tank54_PercentFill, clsGlobVar.CoordinatePlanB.Tank54x, clsGlobVar.CoordinatePlanB.Tank54y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 55, clsGlobVar.Tank55_PercentFill, clsGlobVar.CoordinatePlanB.Tank55x, clsGlobVar.CoordinatePlanB.Tank55y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 56, clsGlobVar.Tank56_PercentFill, clsGlobVar.CoordinatePlanB.Tank56x, clsGlobVar.CoordinatePlanB.Tank56y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 57, clsGlobVar.Tank57_PercentFill, clsGlobVar.CoordinatePlanB.Tank57x, clsGlobVar.CoordinatePlanB.Tank57y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 58, clsGlobVar.Tank58_PercentFill, clsGlobVar.CoordinatePlanB.Tank58x, clsGlobVar.CoordinatePlanB.Tank58y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 59, clsGlobVar.Tank59_PercentFill, clsGlobVar.CoordinatePlanB.Tank59x, clsGlobVar.CoordinatePlanB.Tank59y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 60, clsGlobVar.Tank60_PercentFill, clsGlobVar.CoordinatePlanB.Tank60x, clsGlobVar.CoordinatePlanB.Tank60y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 61, clsGlobVar.Tank61_PercentFill, clsGlobVar.CoordinatePlanB.Tank61x, clsGlobVar.CoordinatePlanB.Tank61y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 62, clsGlobVar.Tank62_PercentFill, clsGlobVar.CoordinatePlanB.Tank62x, clsGlobVar.CoordinatePlanB.Tank62y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 63, clsGlobVar.Tank63_PercentFill, clsGlobVar.CoordinatePlanB.Tank63x, clsGlobVar.CoordinatePlanB.Tank63y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 64, clsGlobVar.Tank64_PercentFill, clsGlobVar.CoordinatePlanB.Tank64x, clsGlobVar.CoordinatePlanB.Tank64y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 65, clsGlobVar.Tank65_PercentFill, clsGlobVar.CoordinatePlanB.Tank65x, clsGlobVar.CoordinatePlanB.Tank65y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 66, clsGlobVar.Tank66_PercentFill, clsGlobVar.CoordinatePlanB.Tank66x, clsGlobVar.CoordinatePlanB.Tank66y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 67, clsGlobVar.Tank67_PercentFill, clsGlobVar.CoordinatePlanB.Tank67x, clsGlobVar.CoordinatePlanB.Tank67y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 68, clsGlobVar.Tank68_PercentFill, clsGlobVar.CoordinatePlanB.Tank68x, clsGlobVar.CoordinatePlanB.Tank68y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 69, clsGlobVar.Tank69_PercentFill, clsGlobVar.CoordinatePlanB.Tank69x, clsGlobVar.CoordinatePlanB.Tank69y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 70, clsGlobVar.Tank70_PercentFill, clsGlobVar.CoordinatePlanB.Tank70x, clsGlobVar.CoordinatePlanB.Tank70y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 71, clsGlobVar.Tank71_PercentFill, clsGlobVar.CoordinatePlanB.Tank71x, clsGlobVar.CoordinatePlanB.Tank71y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 72, clsGlobVar.Tank72_PercentFill, clsGlobVar.CoordinatePlanB.Tank72x, clsGlobVar.CoordinatePlanB.Tank72y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 73, clsGlobVar.Tank73_PercentFill, clsGlobVar.CoordinatePlanB.Tank73x, clsGlobVar.CoordinatePlanB.Tank73y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 74, clsGlobVar.Tank74_PercentFill, clsGlobVar.CoordinatePlanB.Tank74x, clsGlobVar.CoordinatePlanB.Tank74y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 75, clsGlobVar.Tank75_PercentFill, clsGlobVar.CoordinatePlanB.Tank75x, clsGlobVar.CoordinatePlanB.Tank75y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 77, clsGlobVar.Tank77_PercentFill, clsGlobVar.CoordinatePlanB.Tank77x, clsGlobVar.CoordinatePlanB.Tank77y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 78, clsGlobVar.Tank78_PercentFill, clsGlobVar.CoordinatePlanB.Tank78x, clsGlobVar.CoordinatePlanB.Tank78y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 79, clsGlobVar.Tank79_PercentFill, clsGlobVar.CoordinatePlanB.Tank79x, clsGlobVar.CoordinatePlanB.Tank79y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 80, clsGlobVar.Tank80_PercentFill, clsGlobVar.CoordinatePlanB.Tank80x, clsGlobVar.CoordinatePlanB.Tank80y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 81, clsGlobVar.Tank81_PercentFill, clsGlobVar.CoordinatePlanB.Tank81x, clsGlobVar.CoordinatePlanB.Tank81y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 82, clsGlobVar.Tank82_PercentFill, clsGlobVar.CoordinatePlanB.Tank82x, clsGlobVar.CoordinatePlanB.Tank82y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 83, clsGlobVar.Tank83_PercentFill, clsGlobVar.CoordinatePlanB.Tank83x, clsGlobVar.CoordinatePlanB.Tank83y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 84, clsGlobVar.Tank84_PercentFill, clsGlobVar.CoordinatePlanB.Tank84x, clsGlobVar.CoordinatePlanB.Tank84y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 85, clsGlobVar.Tank85_PercentFill, clsGlobVar.CoordinatePlanB.Tank85x, clsGlobVar.CoordinatePlanB.Tank85y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 86, clsGlobVar.Tank86_PercentFill, clsGlobVar.CoordinatePlanB.Tank86x, clsGlobVar.CoordinatePlanB.Tank86y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 87, clsGlobVar.Tank87_PercentFill, clsGlobVar.CoordinatePlanB.Tank87x, clsGlobVar.CoordinatePlanB.Tank87y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 88, clsGlobVar.Tank88_PercentFill, clsGlobVar.CoordinatePlanB.Tank88x, clsGlobVar.CoordinatePlanB.Tank88y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 89, clsGlobVar.Tank89_PercentFill, clsGlobVar.CoordinatePlanB.Tank89x, clsGlobVar.CoordinatePlanB.Tank89y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 90, clsGlobVar.Tank90_PercentFill, clsGlobVar.CoordinatePlanB.Tank90x, clsGlobVar.CoordinatePlanB.Tank90y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 91, clsGlobVar.Tank91_PercentFill, clsGlobVar.CoordinatePlanB.Tank91x, clsGlobVar.CoordinatePlanB.Tank91y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 92, clsGlobVar.Tank92_PercentFill, clsGlobVar.CoordinatePlanB.Tank92x, clsGlobVar.CoordinatePlanB.Tank92y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 93, clsGlobVar.Tank93_PercentFill, clsGlobVar.CoordinatePlanB.Tank93x, clsGlobVar.CoordinatePlanB.Tank93y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 94, clsGlobVar.Tank94_PercentFill, clsGlobVar.CoordinatePlanB.Tank94x, clsGlobVar.CoordinatePlanB.Tank94y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 95, clsGlobVar.Tank95_PercentFill, clsGlobVar.CoordinatePlanB.Tank95x, clsGlobVar.CoordinatePlanB.Tank95y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 96, clsGlobVar.Tank96_PercentFill, clsGlobVar.CoordinatePlanB.Tank96x, clsGlobVar.CoordinatePlanB.Tank96y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 97, clsGlobVar.Tank97_PercentFill, clsGlobVar.CoordinatePlanB.Tank97x, clsGlobVar.CoordinatePlanB.Tank97y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 98, clsGlobVar.Tank98_PercentFill, clsGlobVar.CoordinatePlanB.Tank98x, clsGlobVar.CoordinatePlanB.Tank98y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 99, clsGlobVar.Tank99_PercentFill, clsGlobVar.CoordinatePlanB.Tank99x, clsGlobVar.CoordinatePlanB.Tank99y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 100, clsGlobVar.Tank100_PercentFill, clsGlobVar.CoordinatePlanB.Tank100x, clsGlobVar.CoordinatePlanB.Tank100y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 101, clsGlobVar.Tank101_PercentFill, clsGlobVar.CoordinatePlanB.Tank101x, clsGlobVar.CoordinatePlanB.Tank101y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 102, clsGlobVar.Tank102_PercentFill, clsGlobVar.CoordinatePlanB.Tank102x, clsGlobVar.CoordinatePlanB.Tank102y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 103, clsGlobVar.Tank103_PercentFill, clsGlobVar.CoordinatePlanB.Tank103x, clsGlobVar.CoordinatePlanB.Tank103y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 104, clsGlobVar.Tank104_PercentFill, clsGlobVar.CoordinatePlanB.Tank104x, clsGlobVar.CoordinatePlanB.Tank104y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 105, clsGlobVar.Tank105_PercentFill, clsGlobVar.CoordinatePlanB.Tank105x, clsGlobVar.CoordinatePlanB.Tank105y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 106, clsGlobVar.Tank106_PercentFill, clsGlobVar.CoordinatePlanB.Tank106x, clsGlobVar.CoordinatePlanB.Tank106y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 107, clsGlobVar.Tank107_PercentFill, clsGlobVar.CoordinatePlanB.Tank107x, clsGlobVar.CoordinatePlanB.Tank107y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 108, clsGlobVar.Tank108_PercentFill, clsGlobVar.CoordinatePlanB.Tank108x, clsGlobVar.CoordinatePlanB.Tank108y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 109, clsGlobVar.Tank109_PercentFill, clsGlobVar.CoordinatePlanB.Tank109x, clsGlobVar.CoordinatePlanB.Tank109y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                    //    DrawHatchDeckPlan(canvas2DPlanB, 110, clsGlobVar.Tank110_PercentFill, clsGlobVar.CoordinatePlanB.Tank110x, clsGlobVar.CoordinatePlanB.Tank110y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                }
                #endregion

                canvas2DPlanB.Children.RemoveRange(1, canvas2DPlanB.Children.Count - 1);
                DrawHatchDeckPlan(canvas2DPlanB, 80, clsGlobVar.Tank80_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank80ax, clsGlobVar.CoordinatePlanB.Tank80ay, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                DrawHatchDeckPlan(canvas2DPlanB, 80, clsGlobVar.Tank80_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank80bx, clsGlobVar.CoordinatePlanB.Tank80by, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                DrawHatchDeckPlan(canvas2DPlanB, 81, clsGlobVar.Tank81_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank81x, clsGlobVar.CoordinatePlanB.Tank81y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                DrawHatchDeckPlan(canvas2DPlanB, 82, clsGlobVar.Tank82_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank82x, clsGlobVar.CoordinatePlanB.Tank82y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                DrawHatchDeckPlan(canvas2DPlanB, 85, clsGlobVar.Tank85_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank85ax, clsGlobVar.CoordinatePlanB.Tank85ay, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                DrawHatchDeckPlan(canvas2DPlanB, 85, clsGlobVar.Tank85_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank85bx, clsGlobVar.CoordinatePlanB.Tank85by, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 91, clsGlobVar.Tank91_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank91x, clsGlobVar.CoordinatePlanB.Tank91y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 92, clsGlobVar.Tank92_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank92x, clsGlobVar.CoordinatePlanB.Tank92y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 93, clsGlobVar.Tank93_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank93x, clsGlobVar.CoordinatePlanB.Tank93y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 94, clsGlobVar.Tank94_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank94x, clsGlobVar.CoordinatePlanB.Tank94y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 95, clsGlobVar.Tank95_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank95x, clsGlobVar.CoordinatePlanB.Tank95y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 96, clsGlobVar.Tank96_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank96x, clsGlobVar.CoordinatePlanB.Tank96y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 97, clsGlobVar.Tank97_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank97x, clsGlobVar.CoordinatePlanB.Tank97y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 98, clsGlobVar.Tank98_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank98x, clsGlobVar.CoordinatePlanB.Tank98y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 99, clsGlobVar.Tank99_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank99x, clsGlobVar.CoordinatePlanB.Tank99y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 100, clsGlobVar.Tank100_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank100x, clsGlobVar.CoordinatePlanB.Tank100y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 101, clsGlobVar.Tank101_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank101ax, clsGlobVar.CoordinatePlanB.Tank101ay, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 101, clsGlobVar.Tank101_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank101bx, clsGlobVar.CoordinatePlanB.Tank101by, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 102, clsGlobVar.Tank102_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank102x, clsGlobVar.CoordinatePlanB.Tank102y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 103, clsGlobVar.Tank103_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank103x, clsGlobVar.CoordinatePlanB.Tank103y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 104, clsGlobVar.Tank104_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank104x, clsGlobVar.CoordinatePlanB.Tank104y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 105, clsGlobVar.Tank105_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank105x, clsGlobVar.CoordinatePlanB.Tank105y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 106, clsGlobVar.Tank106_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank106x, clsGlobVar.CoordinatePlanB.Tank106y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 107, clsGlobVar.Tank107_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank107x, clsGlobVar.CoordinatePlanB.Tank107y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 108, clsGlobVar.Tank108_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank108x, clsGlobVar.CoordinatePlanB.Tank108y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 109, clsGlobVar.Tank109_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank109x, clsGlobVar.CoordinatePlanB.Tank109y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 110, clsGlobVar.Tank110_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank110x, clsGlobVar.CoordinatePlanB.Tank110y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 112, clsGlobVar.Tank112_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank112x, clsGlobVar.CoordinatePlanB.Tank112y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 113, clsGlobVar.Tank113_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank113x, clsGlobVar.CoordinatePlanB.Tank113y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 114, clsGlobVar.Tank114_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank114x, clsGlobVar.CoordinatePlanB.Tank114y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 115, clsGlobVar.Tank115_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank115x, clsGlobVar.CoordinatePlanB.Tank115y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 116, clsGlobVar.Tank116_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank116x, clsGlobVar.CoordinatePlanB.Tank116y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 117, clsGlobVar.Tank117_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank117ax, clsGlobVar.CoordinatePlanB.Tank117ay, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 117, clsGlobVar.Tank117_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank117bx, clsGlobVar.CoordinatePlanB.Tank117by, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 118, clsGlobVar.Tank118_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank118x, clsGlobVar.CoordinatePlanB.Tank118y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 119, clsGlobVar.Tank119_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank119x, clsGlobVar.CoordinatePlanB.Tank119y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 120, clsGlobVar.Tank120_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank120x, clsGlobVar.CoordinatePlanB.Tank120y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 121, clsGlobVar.Tank121_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank121x, clsGlobVar.CoordinatePlanB.Tank121y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 122, clsGlobVar.Tank122_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank122x, clsGlobVar.CoordinatePlanB.Tank122y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 123, clsGlobVar.Tank123_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank123x, clsGlobVar.CoordinatePlanB.Tank123y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 124, clsGlobVar.Tank124_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank124x, clsGlobVar.CoordinatePlanB.Tank124y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 125, clsGlobVar.Tank125_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank125x, clsGlobVar.CoordinatePlanB.Tank125y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 126, clsGlobVar.Tank126_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank126x, clsGlobVar.CoordinatePlanB.Tank126y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 127, clsGlobVar.Tank127_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank127x, clsGlobVar.CoordinatePlanB.Tank127y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 128, clsGlobVar.Tank128_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank128x, clsGlobVar.CoordinatePlanB.Tank128y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 129, clsGlobVar.Tank129_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank129x, clsGlobVar.CoordinatePlanB.Tank129y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 130, clsGlobVar.Tank130_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank130x, clsGlobVar.CoordinatePlanB.Tank130y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 131, clsGlobVar.Tank131_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank131x, clsGlobVar.CoordinatePlanB.Tank131y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 132, clsGlobVar.Tank132_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank132x, clsGlobVar.CoordinatePlanB.Tank132y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 133, clsGlobVar.Tank133_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank133x, clsGlobVar.CoordinatePlanB.Tank133y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 134, clsGlobVar.Tank134_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank134x, clsGlobVar.CoordinatePlanB.Tank134y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 135, clsGlobVar.Tank135_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank135x, clsGlobVar.CoordinatePlanB.Tank135y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 136, clsGlobVar.Tank136_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank136x, clsGlobVar.CoordinatePlanB.Tank136y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 137, clsGlobVar.Tank137_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank137x, clsGlobVar.CoordinatePlanB.Tank137y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 138, clsGlobVar.Tank138_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank138x, clsGlobVar.CoordinatePlanB.Tank138y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 139, clsGlobVar.Tank139_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank139x, clsGlobVar.CoordinatePlanB.Tank139y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 140, clsGlobVar.Tank140_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank140x, clsGlobVar.CoordinatePlanB.Tank140y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 141, clsGlobVar.Tank141_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank141x, clsGlobVar.CoordinatePlanB.Tank141y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 150, clsGlobVar.Tank150_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank150x, clsGlobVar.CoordinatePlanB.Tank150y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 152, clsGlobVar.Tank152_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank152x, clsGlobVar.CoordinatePlanB.Tank152y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 153, clsGlobVar.Tank153_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank153x, clsGlobVar.CoordinatePlanB.Tank153y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 158, clsGlobVar.Tank158_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank158x, clsGlobVar.CoordinatePlanB.Tank158y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 159, clsGlobVar.Tank159_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank159x, clsGlobVar.CoordinatePlanB.Tank159y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 160, clsGlobVar.Tank160_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank160x, clsGlobVar.CoordinatePlanB.Tank160y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 161, clsGlobVar.Tank161_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank161x, clsGlobVar.CoordinatePlanB.Tank161y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 162, clsGlobVar.Tank162_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank162x, clsGlobVar.CoordinatePlanB.Tank162y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 163, clsGlobVar.Tank163_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank163x, clsGlobVar.CoordinatePlanB.Tank163y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 165, clsGlobVar.Tank165_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank165x, clsGlobVar.CoordinatePlanB.Tank165y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 166, clsGlobVar.Tank166_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank166ax, clsGlobVar.CoordinatePlanB.Tank166ay, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 166, clsGlobVar.Tank166_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank166bx, clsGlobVar.CoordinatePlanB.Tank166by, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 167, clsGlobVar.Tank167_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank167x, clsGlobVar.CoordinatePlanB.Tank167y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 168, clsGlobVar.Tank168_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank168x, clsGlobVar.CoordinatePlanB.Tank168y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 169, clsGlobVar.Tank169_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank169x, clsGlobVar.CoordinatePlanB.Tank169y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 170, clsGlobVar.Tank170_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank170x, clsGlobVar.CoordinatePlanB.Tank170y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 171, clsGlobVar.Tank171_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank171x, clsGlobVar.CoordinatePlanB.Tank171y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 172, clsGlobVar.Tank172_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank172x, clsGlobVar.CoordinatePlanB.Tank172y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 173, clsGlobVar.Tank173_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank173x, clsGlobVar.CoordinatePlanB.Tank173y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 174, clsGlobVar.Tank174_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank174x, clsGlobVar.CoordinatePlanB.Tank174y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 175, clsGlobVar.Tank175_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank175x, clsGlobVar.CoordinatePlanB.Tank175y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 176, clsGlobVar.Tank176_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank176x, clsGlobVar.CoordinatePlanB.Tank176y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 177, clsGlobVar.Tank177_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank177ax, clsGlobVar.CoordinatePlanB.Tank177ay, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 177, clsGlobVar.Tank177_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank177bx, clsGlobVar.CoordinatePlanB.Tank177by, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 178, clsGlobVar.Tank178_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank178x, clsGlobVar.CoordinatePlanB.Tank178y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 179, clsGlobVar.Tank179_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank179x, clsGlobVar.CoordinatePlanB.Tank179y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 181, clsGlobVar.Tank181_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank181ax, clsGlobVar.CoordinatePlanB.Tank181ay, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 181, clsGlobVar.Tank181_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank181bx, clsGlobVar.CoordinatePlanB.Tank181by, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 182, clsGlobVar.Tank182_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank182x, clsGlobVar.CoordinatePlanB.Tank182y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 183, clsGlobVar.Tank183_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank183x, clsGlobVar.CoordinatePlanB.Tank183y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 184, clsGlobVar.Tank184_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank184x, clsGlobVar.CoordinatePlanB.Tank184y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 185, clsGlobVar.Tank185_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank185x, clsGlobVar.CoordinatePlanB.Tank185y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 187, clsGlobVar.Tank187_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank187ax, clsGlobVar.CoordinatePlanB.Tank187ay, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 187, clsGlobVar.Tank187_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank187bx, clsGlobVar.CoordinatePlanB.Tank187by, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 188, clsGlobVar.Tank188_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank188x, clsGlobVar.CoordinatePlanB.Tank188y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 189, clsGlobVar.Tank189_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank189x, clsGlobVar.CoordinatePlanB.Tank189y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 190, clsGlobVar.Tank190_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank190x, clsGlobVar.CoordinatePlanB.Tank190y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 191, clsGlobVar.Tank191_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank191x, clsGlobVar.CoordinatePlanB.Tank191y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 192, clsGlobVar.Tank192_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank192x, clsGlobVar.CoordinatePlanB.Tank192y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 193, clsGlobVar.Tank193_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank193x, clsGlobVar.CoordinatePlanB.Tank193y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 194, clsGlobVar.Tank194_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank194x, clsGlobVar.CoordinatePlanB.Tank194y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 195, clsGlobVar.Tank195_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank195x, clsGlobVar.CoordinatePlanB.Tank195y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 196, clsGlobVar.Tank196_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank196x, clsGlobVar.CoordinatePlanB.Tank196y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 197, clsGlobVar.Tank197_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank197x, clsGlobVar.CoordinatePlanB.Tank197y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 198, clsGlobVar.Tank198_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank198ax, clsGlobVar.CoordinatePlanB.Tank198ay, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 198, clsGlobVar.Tank198_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank198bx, clsGlobVar.CoordinatePlanB.Tank198by, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 199, clsGlobVar.Tank199_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank199x, clsGlobVar.CoordinatePlanB.Tank199y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 200, clsGlobVar.Tank200_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank200x, clsGlobVar.CoordinatePlanB.Tank200y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 201, clsGlobVar.Tank201_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank201x, clsGlobVar.CoordinatePlanB.Tank201y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 202, clsGlobVar.Tank202_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank202x, clsGlobVar.CoordinatePlanB.Tank202y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 203, clsGlobVar.Tank203_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank203x, clsGlobVar.CoordinatePlanB.Tank203y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 204, clsGlobVar.Tank204_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank204x, clsGlobVar.CoordinatePlanB.Tank204y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 205, clsGlobVar.Tank205_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank205x, clsGlobVar.CoordinatePlanB.Tank205y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 206, clsGlobVar.Tank206_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank206x, clsGlobVar.CoordinatePlanB.Tank206y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 207, clsGlobVar.Tank207_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank207ax, clsGlobVar.CoordinatePlanB.Tank207ay, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 207, clsGlobVar.Tank207_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank207bx, clsGlobVar.CoordinatePlanB.Tank207by, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 208, clsGlobVar.Tank208_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank208x, clsGlobVar.CoordinatePlanB.Tank208y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 209, clsGlobVar.Tank209_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank209x, clsGlobVar.CoordinatePlanB.Tank209y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 210, clsGlobVar.Tank210_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank210x, clsGlobVar.CoordinatePlanB.Tank210y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 211, clsGlobVar.Tank211_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank211x, clsGlobVar.CoordinatePlanB.Tank211y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 212, clsGlobVar.Tank212_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank212x, clsGlobVar.CoordinatePlanB.Tank212y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 213, clsGlobVar.Tank213_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank213x, clsGlobVar.CoordinatePlanB.Tank213y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 214, clsGlobVar.Tank214_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank214x, clsGlobVar.CoordinatePlanB.Tank214y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 215, clsGlobVar.Tank215_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank215x, clsGlobVar.CoordinatePlanB.Tank215y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 216, clsGlobVar.Tank216_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank216x, clsGlobVar.CoordinatePlanB.Tank216y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 217, clsGlobVar.Tank217_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank217x, clsGlobVar.CoordinatePlanB.Tank217y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 218, clsGlobVar.Tank218_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank218x, clsGlobVar.CoordinatePlanB.Tank218y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 219, clsGlobVar.Tank219_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank219x, clsGlobVar.CoordinatePlanB.Tank219y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 220, clsGlobVar.Tank220_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank220x, clsGlobVar.CoordinatePlanB.Tank220y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 221, clsGlobVar.Tank221_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank221x, clsGlobVar.CoordinatePlanB.Tank221y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 222, clsGlobVar.Tank222_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank222x, clsGlobVar.CoordinatePlanB.Tank222y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 223, clsGlobVar.Tank223_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank223x, clsGlobVar.CoordinatePlanB.Tank223y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 224, clsGlobVar.Tank224_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank224x, clsGlobVar.CoordinatePlanB.Tank224y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 225, clsGlobVar.Tank225_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank225x, clsGlobVar.CoordinatePlanB.Tank225y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 226, clsGlobVar.Tank226_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank226x, clsGlobVar.CoordinatePlanB.Tank226y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 227, clsGlobVar.Tank227_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank227x, clsGlobVar.CoordinatePlanB.Tank227y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 228, clsGlobVar.Tank228_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank228x, clsGlobVar.CoordinatePlanB.Tank228y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 229, clsGlobVar.Tank229_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank229x, clsGlobVar.CoordinatePlanB.Tank229y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 230, clsGlobVar.Tank230_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank230x, clsGlobVar.CoordinatePlanB.Tank230y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 231, clsGlobVar.Tank231_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank231x, clsGlobVar.CoordinatePlanB.Tank231y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 232, clsGlobVar.Tank232_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank232x, clsGlobVar.CoordinatePlanB.Tank232y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 233, clsGlobVar.Tank233_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank233x, clsGlobVar.CoordinatePlanB.Tank233y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 234, clsGlobVar.Tank234_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank234x, clsGlobVar.CoordinatePlanB.Tank234y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 235, clsGlobVar.Tank235_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank235x, clsGlobVar.CoordinatePlanB.Tank235y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 236, clsGlobVar.Tank236_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank236x, clsGlobVar.CoordinatePlanB.Tank236y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 237, clsGlobVar.Tank237_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank237x, clsGlobVar.CoordinatePlanB.Tank237y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 238, clsGlobVar.Tank238_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank238x, clsGlobVar.CoordinatePlanB.Tank238y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 239, clsGlobVar.Tank239_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank239x, clsGlobVar.CoordinatePlanB.Tank239y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 240, clsGlobVar.Tank240_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank240x, clsGlobVar.CoordinatePlanB.Tank240y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 241, clsGlobVar.Tank241_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank241x, clsGlobVar.CoordinatePlanB.Tank241y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 242, clsGlobVar.Tank242_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank242x, clsGlobVar.CoordinatePlanB.Tank242y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 243, clsGlobVar.Tank243_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank243x, clsGlobVar.CoordinatePlanB.Tank243y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 244, clsGlobVar.Tank244_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank244x, clsGlobVar.CoordinatePlanB.Tank244y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 245, clsGlobVar.Tank245_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank245x, clsGlobVar.CoordinatePlanB.Tank245y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 246, clsGlobVar.Tank246_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank246x, clsGlobVar.CoordinatePlanB.Tank246y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 247, clsGlobVar.Tank247_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank247x, clsGlobVar.CoordinatePlanB.Tank247y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 248, clsGlobVar.Tank248_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank248x, clsGlobVar.CoordinatePlanB.Tank248y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 249, clsGlobVar.Tank249_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank249x, clsGlobVar.CoordinatePlanB.Tank249y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 250, clsGlobVar.Tank250_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank250x, clsGlobVar.CoordinatePlanB.Tank250y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 251, clsGlobVar.Tank251_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank251x, clsGlobVar.CoordinatePlanB.Tank251y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 252, clsGlobVar.Tank252_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank252x, clsGlobVar.CoordinatePlanB.Tank252y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 253, clsGlobVar.Tank253_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank253x, clsGlobVar.CoordinatePlanB.Tank253y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 254, clsGlobVar.Tank254_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank254x, clsGlobVar.CoordinatePlanB.Tank254y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 255, clsGlobVar.Tank255_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank255x, clsGlobVar.CoordinatePlanB.Tank255y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 256, clsGlobVar.Tank256_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank256x, clsGlobVar.CoordinatePlanB.Tank256y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 257, clsGlobVar.Tank257_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank257x, clsGlobVar.CoordinatePlanB.Tank257y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 258, clsGlobVar.Tank258_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank258x, clsGlobVar.CoordinatePlanB.Tank258y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 259, clsGlobVar.Tank259_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank259x, clsGlobVar.CoordinatePlanB.Tank259y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 260, clsGlobVar.Tank260_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank260x, clsGlobVar.CoordinatePlanB.Tank260y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 262, clsGlobVar.Tank262_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank262x, clsGlobVar.CoordinatePlanB.Tank262y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 263, clsGlobVar.Tank263_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank263x, clsGlobVar.CoordinatePlanB.Tank263y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 264, clsGlobVar.Tank264_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank264x, clsGlobVar.CoordinatePlanB.Tank264y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 265, clsGlobVar.Tank265_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank265x, clsGlobVar.CoordinatePlanB.Tank265y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 266, clsGlobVar.Tank266_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank266x, clsGlobVar.CoordinatePlanB.Tank266y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 267, clsGlobVar.Tank267_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank267x, clsGlobVar.CoordinatePlanB.Tank267y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 268, clsGlobVar.Tank268_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank268x, clsGlobVar.CoordinatePlanB.Tank268y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 269, clsGlobVar.Tank269_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank269x, clsGlobVar.CoordinatePlanB.Tank269y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 270, clsGlobVar.Tank270_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank270x, clsGlobVar.CoordinatePlanB.Tank270y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 271, clsGlobVar.Tank271_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank271x, clsGlobVar.CoordinatePlanB.Tank271y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 272, clsGlobVar.Tank272_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank272x, clsGlobVar.CoordinatePlanB.Tank272y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 273, clsGlobVar.Tank273_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank273x, clsGlobVar.CoordinatePlanB.Tank273y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 274, clsGlobVar.Tank274_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank274x, clsGlobVar.CoordinatePlanB.Tank274y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 275, clsGlobVar.Tank275_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank275x, clsGlobVar.CoordinatePlanB.Tank275y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 276, clsGlobVar.Tank276_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank276x, clsGlobVar.CoordinatePlanB.Tank276y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 277, clsGlobVar.Tank277_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank277x, clsGlobVar.CoordinatePlanB.Tank277y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 278, clsGlobVar.Tank278_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank278x, clsGlobVar.CoordinatePlanB.Tank278y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 279, clsGlobVar.Tank279_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank279x, clsGlobVar.CoordinatePlanB.Tank279y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 280, clsGlobVar.Tank280_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank280x, clsGlobVar.CoordinatePlanB.Tank280y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 281, clsGlobVar.Tank281_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank281x, clsGlobVar.CoordinatePlanB.Tank281y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 282, clsGlobVar.Tank282_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank282x, clsGlobVar.CoordinatePlanB.Tank282y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 283, clsGlobVar.Tank283_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank283x, clsGlobVar.CoordinatePlanB.Tank283y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 284, clsGlobVar.Tank284_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank284x, clsGlobVar.CoordinatePlanB.Tank284y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 285, clsGlobVar.Tank285_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank285x, clsGlobVar.CoordinatePlanB.Tank285y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 286, clsGlobVar.Tank286_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank286x, clsGlobVar.CoordinatePlanB.Tank286y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 287, clsGlobVar.Tank287_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank287x, clsGlobVar.CoordinatePlanB.Tank287y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 288, clsGlobVar.Tank288_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank288x, clsGlobVar.CoordinatePlanB.Tank288y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 289, clsGlobVar.Tank289_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank289x, clsGlobVar.CoordinatePlanB.Tank289y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 290, clsGlobVar.Tank290_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank290x, clsGlobVar.CoordinatePlanB.Tank290y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 291, clsGlobVar.Tank291_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank291x, clsGlobVar.CoordinatePlanB.Tank291y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 292, clsGlobVar.Tank292_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank292x, clsGlobVar.CoordinatePlanB.Tank292y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 293, clsGlobVar.Tank293_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank293x, clsGlobVar.CoordinatePlanB.Tank293y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 294, clsGlobVar.Tank294_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank294x, clsGlobVar.CoordinatePlanB.Tank294y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 295, clsGlobVar.Tank295_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank295x, clsGlobVar.CoordinatePlanB.Tank295y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 296, clsGlobVar.Tank296_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank296x, clsGlobVar.CoordinatePlanB.Tank296y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 297, clsGlobVar.Tank297_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank297x, clsGlobVar.CoordinatePlanB.Tank297y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 298, clsGlobVar.Tank298_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank298x, clsGlobVar.CoordinatePlanB.Tank298y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 299, clsGlobVar.Tank299_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank299x, clsGlobVar.CoordinatePlanB.Tank299y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 300, clsGlobVar.Tank300_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank300x, clsGlobVar.CoordinatePlanB.Tank300y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 301, clsGlobVar.Tank301_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank301x, clsGlobVar.CoordinatePlanB.Tank301y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 436, clsGlobVar.Tank436_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank436x, clsGlobVar.CoordinatePlanB.Tank436y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 437, clsGlobVar.Tank437_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank437x, clsGlobVar.CoordinatePlanB.Tank437y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 440, clsGlobVar.Tank440_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank440x, clsGlobVar.CoordinatePlanB.Tank440y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 441, clsGlobVar.Tank441_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank441x, clsGlobVar.CoordinatePlanB.Tank441y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 445, clsGlobVar.Tank445_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank445x, clsGlobVar.CoordinatePlanB.Tank445y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 446, clsGlobVar.Tank446_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank446x, clsGlobVar.CoordinatePlanB.Tank446y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 447, clsGlobVar.Tank447_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank447ax, clsGlobVar.CoordinatePlanB.Tank447ay, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 447, clsGlobVar.Tank447_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank447bx, clsGlobVar.CoordinatePlanB.Tank447by, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 448, clsGlobVar.Tank448_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank448ax, clsGlobVar.CoordinatePlanB.Tank448ay, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 448, clsGlobVar.Tank448_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank448bx, clsGlobVar.CoordinatePlanB.Tank448by, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 450, clsGlobVar.Tank450_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank450x, clsGlobVar.CoordinatePlanB.Tank450y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 454, clsGlobVar.Tank454_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank454x, clsGlobVar.CoordinatePlanB.Tank454y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 455, clsGlobVar.Tank455_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank455ax, clsGlobVar.CoordinatePlanB.Tank455ay, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 455, clsGlobVar.Tank455_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank455bx, clsGlobVar.CoordinatePlanB.Tank455by, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 458, clsGlobVar.Tank458_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank458x, clsGlobVar.CoordinatePlanB.Tank458y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 459, clsGlobVar.Tank459_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank459ax, clsGlobVar.CoordinatePlanB.Tank459ay, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 459, clsGlobVar.Tank459_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank459bx, clsGlobVar.CoordinatePlanB.Tank459by, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 462, clsGlobVar.Tank462_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank462x, clsGlobVar.CoordinatePlanB.Tank462y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 463, clsGlobVar.Tank463_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank463x, clsGlobVar.CoordinatePlanB.Tank463y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 464, clsGlobVar.Tank464_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank464x, clsGlobVar.CoordinatePlanB.Tank464y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 465, clsGlobVar.Tank465_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank465x, clsGlobVar.CoordinatePlanB.Tank465y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 466, clsGlobVar.Tank466_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank466ax, clsGlobVar.CoordinatePlanB.Tank466ay, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 466, clsGlobVar.Tank466_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank466bx, clsGlobVar.CoordinatePlanB.Tank466by, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 468, clsGlobVar.Tank468_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank468x, clsGlobVar.CoordinatePlanB.Tank468y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 469, clsGlobVar.Tank469_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank469x, clsGlobVar.CoordinatePlanB.Tank469y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 471, clsGlobVar.Tank471_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank471x, clsGlobVar.CoordinatePlanB.Tank471y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 472, clsGlobVar.Tank472_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank472x, clsGlobVar.CoordinatePlanB.Tank472y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 474, clsGlobVar.Tank474_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank474x, clsGlobVar.CoordinatePlanB.Tank474y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 475, clsGlobVar.Tank475_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank475ax, clsGlobVar.CoordinatePlanB.Tank475ay, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 475, clsGlobVar.Tank475_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank475bx, clsGlobVar.CoordinatePlanB.Tank475by, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 480, clsGlobVar.Tank480_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank480x, clsGlobVar.CoordinatePlanB.Tank480y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 481, clsGlobVar.Tank481_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank481x, clsGlobVar.CoordinatePlanB.Tank481y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 486, clsGlobVar.Tank486_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank486x, clsGlobVar.CoordinatePlanB.Tank486y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 490, clsGlobVar.Tank490_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank490x, clsGlobVar.CoordinatePlanB.Tank490y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 491, clsGlobVar.Tank491_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank491x, clsGlobVar.CoordinatePlanB.Tank491y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 492, clsGlobVar.Tank492_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank492ax, clsGlobVar.CoordinatePlanB.Tank492ay, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 492, clsGlobVar.Tank492_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank492bx, clsGlobVar.CoordinatePlanB.Tank492by, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 493, clsGlobVar.Tank493_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank493x, clsGlobVar.CoordinatePlanB.Tank493y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 495, clsGlobVar.Tank495_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank495ax, clsGlobVar.CoordinatePlanB.Tank495ay, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 495, clsGlobVar.Tank495_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank495bx, clsGlobVar.CoordinatePlanB.Tank495by, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 496, clsGlobVar.Tank496_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank496x, clsGlobVar.CoordinatePlanB.Tank496y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 497, clsGlobVar.Tank497_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank497x, clsGlobVar.CoordinatePlanB.Tank497y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanB, 498, clsGlobVar.Tank498_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank498x, clsGlobVar.CoordinatePlanB.Tank498y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));

            }
            catch //(Exception ex)
            {
               // System.Windows.MessageBox.Show(ex.Message);
            }
        }


        /// <summary>
        /// Add Hatch Filling of Tanks/Compartments to DeckPlans Canvas as per TankID 
        /// </summary>
        public void AddHatchDeckPlanA()
        {
            try
            {
                canvas2DPlanA.Children.RemoveRange(1, canvas2DPlanA.Children.Count - 1);


                DrawHatchDeckPlan(canvas2DPlanA, 2, clsGlobVar.Tank2_PercentFill, clsGlobVar.CoordinatePlanA.Tank2x, clsGlobVar.CoordinatePlanA.Tank2y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                DrawHatchDeckPlan(canvas2DPlanA, 3, clsGlobVar.Tank3_PercentFill, clsGlobVar.CoordinatePlanA.Tank3x, clsGlobVar.CoordinatePlanA.Tank3y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                DrawHatchDeckPlan(canvas2DPlanA, 4, clsGlobVar.Tank4_PercentFill, clsGlobVar.CoordinatePlanA.Tank4ax, clsGlobVar.CoordinatePlanA.Tank4ay, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                DrawHatchDeckPlan(canvas2DPlanA, 4, clsGlobVar.Tank4_PercentFill, clsGlobVar.CoordinatePlanA.Tank4bx, clsGlobVar.CoordinatePlanA.Tank4by, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                DrawHatchDeckPlan(canvas2DPlanA, 5, clsGlobVar.Tank5_PercentFill, clsGlobVar.CoordinatePlanA.Tank5ax, clsGlobVar.CoordinatePlanA.Tank5ay, System.Windows.Media.Color.FromArgb(180, 141, 180, 227));
                DrawHatchDeckPlan(canvas2DPlanA, 5, clsGlobVar.Tank5_PercentFill, clsGlobVar.CoordinatePlanA.Tank5bx, clsGlobVar.CoordinatePlanA.Tank5by, System.Windows.Media.Color.FromArgb(180, 141, 180, 227));
                DrawHatchDeckPlan(canvas2DPlanA, 6, clsGlobVar.Tank6_PercentFill, clsGlobVar.CoordinatePlanA.Tank6ax, clsGlobVar.CoordinatePlanA.Tank6ay, System.Windows.Media.Color.FromArgb(180, 141, 180, 227));
                DrawHatchDeckPlan(canvas2DPlanA, 6, clsGlobVar.Tank6_PercentFill, clsGlobVar.CoordinatePlanA.Tank6bx, clsGlobVar.CoordinatePlanA.Tank6by, System.Windows.Media.Color.FromArgb(180, 141, 180, 227));
                DrawHatchDeckPlan(canvas2DPlanA, 7, clsGlobVar.Tank7_PercentFill, clsGlobVar.CoordinatePlanA.Tank7x, clsGlobVar.CoordinatePlanA.Tank7y, System.Windows.Media.Color.FromArgb(180, 151, 72, 7));
                DrawHatchDeckPlan(canvas2DPlanA, 8, clsGlobVar.Tank8_PercentFill, clsGlobVar.CoordinatePlanA.Tank8ax, clsGlobVar.CoordinatePlanA.Tank8ay, System.Windows.Media.Color.FromArgb(180, 151, 72, 7));
                DrawHatchDeckPlan(canvas2DPlanA, 8, clsGlobVar.Tank8_PercentFill, clsGlobVar.CoordinatePlanA.Tank8bx, clsGlobVar.CoordinatePlanA.Tank8by, System.Windows.Media.Color.FromArgb(180, 151, 72, 7));
                DrawHatchDeckPlan(canvas2DPlanA, 9, clsGlobVar.Tank9_PercentFill, clsGlobVar.CoordinatePlanA.Tank9x, clsGlobVar.CoordinatePlanA.Tank9y, System.Windows.Media.Color.FromArgb(180, 151, 72, 7));
                DrawHatchDeckPlan(canvas2DPlanA, 10, clsGlobVar.Tank10_PercentFill, clsGlobVar.CoordinatePlanA.Tank10x, clsGlobVar.CoordinatePlanA.Tank10y, System.Windows.Media.Color.FromArgb(180, 151, 72, 7));
                DrawHatchDeckPlan(canvas2DPlanA, 11, clsGlobVar.Tank11_PercentFill, clsGlobVar.CoordinatePlanA.Tank11ax, clsGlobVar.CoordinatePlanA.Tank11ay, System.Windows.Media.Color.FromArgb(180, 151, 72, 7));
                DrawHatchDeckPlan(canvas2DPlanA, 11, clsGlobVar.Tank11_PercentFill, clsGlobVar.CoordinatePlanA.Tank11bx, clsGlobVar.CoordinatePlanA.Tank11by, System.Windows.Media.Color.FromArgb(180, 151, 72, 7));
                DrawHatchDeckPlan(canvas2DPlanA, 12, clsGlobVar.Tank12_PercentFill, clsGlobVar.CoordinatePlanA.Tank12ax, clsGlobVar.CoordinatePlanA.Tank12ay, System.Windows.Media.Color.FromArgb(180, 151, 72, 7));
                DrawHatchDeckPlan(canvas2DPlanA, 12, clsGlobVar.Tank12_PercentFill, clsGlobVar.CoordinatePlanA.Tank12bx, clsGlobVar.CoordinatePlanA.Tank12by, System.Windows.Media.Color.FromArgb(180, 151, 72, 7));
                DrawHatchDeckPlan(canvas2DPlanA, 13, clsGlobVar.Tank13_PercentFill, clsGlobVar.CoordinatePlanA.Tank13ax, clsGlobVar.CoordinatePlanA.Tank13ay, System.Windows.Media.Color.FromArgb(180, 151, 72, 7));
                DrawHatchDeckPlan(canvas2DPlanA, 13, clsGlobVar.Tank13_PercentFill, clsGlobVar.CoordinatePlanA.Tank13bx, clsGlobVar.CoordinatePlanA.Tank13by, System.Windows.Media.Color.FromArgb(180, 151, 72, 7));
                DrawHatchDeckPlan(canvas2DPlanA, 14, clsGlobVar.Tank14_PercentFill, clsGlobVar.CoordinatePlanA.Tank14ax, clsGlobVar.CoordinatePlanA.Tank14ay, System.Windows.Media.Color.FromArgb(180, 151, 72, 7));
                DrawHatchDeckPlan(canvas2DPlanA, 14, clsGlobVar.Tank14_PercentFill, clsGlobVar.CoordinatePlanA.Tank14bx, clsGlobVar.CoordinatePlanA.Tank14by, System.Windows.Media.Color.FromArgb(180, 151, 72, 7));
                DrawHatchDeckPlan(canvas2DPlanA, 15, clsGlobVar.Tank15_PercentFill, clsGlobVar.CoordinatePlanA.Tank15ax, clsGlobVar.CoordinatePlanA.Tank15ay, System.Windows.Media.Color.FromArgb(180, 255, 192, 203));
                DrawHatchDeckPlan(canvas2DPlanA, 15, clsGlobVar.Tank15_PercentFill, clsGlobVar.CoordinatePlanA.Tank15bx, clsGlobVar.CoordinatePlanA.Tank15by, System.Windows.Media.Color.FromArgb(180, 255, 192, 203));
                DrawHatchDeckPlan(canvas2DPlanA, 16, clsGlobVar.Tank16_PercentFill, clsGlobVar.CoordinatePlanA.Tank16ax, clsGlobVar.CoordinatePlanA.Tank16ay, System.Windows.Media.Color.FromArgb(180, 255, 192, 203));
                DrawHatchDeckPlan(canvas2DPlanA, 16, clsGlobVar.Tank16_PercentFill, clsGlobVar.CoordinatePlanA.Tank16bx, clsGlobVar.CoordinatePlanA.Tank16by, System.Windows.Media.Color.FromArgb(180, 255, 192, 203));
                DrawHatchDeckPlan(canvas2DPlanA, 17, clsGlobVar.Tank17_PercentFill, clsGlobVar.CoordinatePlanA.Tank17ax, clsGlobVar.CoordinatePlanA.Tank17ay, System.Windows.Media.Color.FromArgb(180, 255, 192, 203));
                DrawHatchDeckPlan(canvas2DPlanA, 17, clsGlobVar.Tank17_PercentFill, clsGlobVar.CoordinatePlanA.Tank17bx, clsGlobVar.CoordinatePlanA.Tank17by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 18, clsGlobVar.Tank18_PercentFill, clsGlobVar.CoordinatePlanA.Tank18x, clsGlobVar.CoordinatePlanA.Tank18y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 19, clsGlobVar.Tank19_PercentFill, clsGlobVar.CoordinatePlanA.Tank19x, clsGlobVar.CoordinatePlanA.Tank19y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 20, clsGlobVar.Tank20_PercentFill, clsGlobVar.CoordinatePlanA.Tank20x, clsGlobVar.CoordinatePlanA.Tank20y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 21, clsGlobVar.Tank21_PercentFill, clsGlobVar.CoordinatePlanA.Tank21x, clsGlobVar.CoordinatePlanA.Tank21y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 22, clsGlobVar.Tank22_PercentFill, clsGlobVar.CoordinatePlanA.Tank22x, clsGlobVar.CoordinatePlanA.Tank22y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 23, clsGlobVar.Tank23_PercentFill, clsGlobVar.CoordinatePlanA.Tank23x, clsGlobVar.CoordinatePlanA.Tank23y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 24, clsGlobVar.Tank24_PercentFill, clsGlobVar.CoordinatePlanA.Tank24x, clsGlobVar.CoordinatePlanA.Tank24y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 25, clsGlobVar.Tank25_PercentFill, clsGlobVar.CoordinatePlanA.Tank25x, clsGlobVar.CoordinatePlanA.Tank25y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 26, clsGlobVar.Tank26_PercentFill, clsGlobVar.CoordinatePlanA.Tank26x, clsGlobVar.CoordinatePlanA.Tank26y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 27, clsGlobVar.Tank27_PercentFill, clsGlobVar.CoordinatePlanA.Tank27x, clsGlobVar.CoordinatePlanA.Tank27y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 28, clsGlobVar.Tank28_PercentFill, clsGlobVar.CoordinatePlanA.Tank28x, clsGlobVar.CoordinatePlanA.Tank28y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 29, clsGlobVar.Tank29_PercentFill, clsGlobVar.CoordinatePlanA.Tank29ax, clsGlobVar.CoordinatePlanA.Tank29ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 29, clsGlobVar.Tank29_PercentFill, clsGlobVar.CoordinatePlanA.Tank29bx, clsGlobVar.CoordinatePlanA.Tank29by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 30, clsGlobVar.Tank30_PercentFill, clsGlobVar.CoordinatePlanA.Tank30x, clsGlobVar.CoordinatePlanA.Tank30y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 31, clsGlobVar.Tank31_PercentFill, clsGlobVar.CoordinatePlanA.Tank31ax, clsGlobVar.CoordinatePlanA.Tank31ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 31, clsGlobVar.Tank31_PercentFill, clsGlobVar.CoordinatePlanA.Tank31bx, clsGlobVar.CoordinatePlanA.Tank31by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 32, clsGlobVar.Tank32_PercentFill, clsGlobVar.CoordinatePlanA.Tank32ax, clsGlobVar.CoordinatePlanA.Tank32ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 32, clsGlobVar.Tank32_PercentFill, clsGlobVar.CoordinatePlanA.Tank32bx, clsGlobVar.CoordinatePlanA.Tank32by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 33, clsGlobVar.Tank33_PercentFill, clsGlobVar.CoordinatePlanA.Tank33x, clsGlobVar.CoordinatePlanA.Tank33y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 34, clsGlobVar.Tank34_PercentFill, clsGlobVar.CoordinatePlanA.Tank34x, clsGlobVar.CoordinatePlanA.Tank34y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 35, clsGlobVar.Tank35_PercentFill, clsGlobVar.CoordinatePlanA.Tank35ax, clsGlobVar.CoordinatePlanA.Tank35ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 35, clsGlobVar.Tank35_PercentFill, clsGlobVar.CoordinatePlanA.Tank35bx, clsGlobVar.CoordinatePlanA.Tank35by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 36, clsGlobVar.Tank36_PercentFill, clsGlobVar.CoordinatePlanA.Tank36ax, clsGlobVar.CoordinatePlanA.Tank36ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 36, clsGlobVar.Tank36_PercentFill, clsGlobVar.CoordinatePlanA.Tank36bx, clsGlobVar.CoordinatePlanA.Tank36by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 37, clsGlobVar.Tank37_PercentFill, clsGlobVar.CoordinatePlanA.Tank37ax, clsGlobVar.CoordinatePlanA.Tank37ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 37, clsGlobVar.Tank37_PercentFill, clsGlobVar.CoordinatePlanA.Tank37bx, clsGlobVar.CoordinatePlanA.Tank37by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 38, clsGlobVar.Tank38_PercentFill, clsGlobVar.CoordinatePlanA.Tank38ax, clsGlobVar.CoordinatePlanA.Tank38ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 38, clsGlobVar.Tank38_PercentFill, clsGlobVar.CoordinatePlanA.Tank38bx, clsGlobVar.CoordinatePlanA.Tank38by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 39, clsGlobVar.Tank39_PercentFill, clsGlobVar.CoordinatePlanA.Tank39ax, clsGlobVar.CoordinatePlanA.Tank39ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 39, clsGlobVar.Tank39_PercentFill, clsGlobVar.CoordinatePlanA.Tank39bx, clsGlobVar.CoordinatePlanA.Tank39by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 40, clsGlobVar.Tank40_PercentFill, clsGlobVar.CoordinatePlanA.Tank40x, clsGlobVar.CoordinatePlanA.Tank40y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 41, clsGlobVar.Tank41_PercentFill, clsGlobVar.CoordinatePlanA.Tank41x, clsGlobVar.CoordinatePlanA.Tank41y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 42, clsGlobVar.Tank42_PercentFill, clsGlobVar.CoordinatePlanA.Tank42x, clsGlobVar.CoordinatePlanA.Tank42y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 44, clsGlobVar.Tank44_PercentFill, clsGlobVar.CoordinatePlanA.Tank44x, clsGlobVar.CoordinatePlanA.Tank44y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 45, clsGlobVar.Tank45_PercentFill, clsGlobVar.CoordinatePlanA.Tank45ax, clsGlobVar.CoordinatePlanA.Tank45ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 45, clsGlobVar.Tank45_PercentFill, clsGlobVar.CoordinatePlanA.Tank45bx, clsGlobVar.CoordinatePlanA.Tank45by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 46, clsGlobVar.Tank46_PercentFill, clsGlobVar.CoordinatePlanA.Tank46ax, clsGlobVar.CoordinatePlanA.Tank46ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 46, clsGlobVar.Tank46_PercentFill, clsGlobVar.CoordinatePlanA.Tank46bx, clsGlobVar.CoordinatePlanA.Tank46by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 47, clsGlobVar.Tank47_PercentFill, clsGlobVar.CoordinatePlanA.Tank47x, clsGlobVar.CoordinatePlanA.Tank47y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 48, clsGlobVar.Tank48_PercentFill, clsGlobVar.CoordinatePlanA.Tank48x, clsGlobVar.CoordinatePlanA.Tank48y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 50, clsGlobVar.Tank50_PercentFill, clsGlobVar.CoordinatePlanA.Tank50x, clsGlobVar.CoordinatePlanA.Tank50y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 52, clsGlobVar.Tank52_PercentFill, clsGlobVar.CoordinatePlanA.Tank52x, clsGlobVar.CoordinatePlanA.Tank52y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 53, clsGlobVar.Tank53_PercentFill, clsGlobVar.CoordinatePlanA.Tank53x, clsGlobVar.CoordinatePlanA.Tank53y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 54, clsGlobVar.Tank54_PercentFill, clsGlobVar.CoordinatePlanA.Tank54x, clsGlobVar.CoordinatePlanA.Tank54y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 55, clsGlobVar.Tank55_PercentFill, clsGlobVar.CoordinatePlanA.Tank55x, clsGlobVar.CoordinatePlanA.Tank55y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 56, clsGlobVar.Tank56_PercentFill, clsGlobVar.CoordinatePlanA.Tank56x, clsGlobVar.CoordinatePlanA.Tank56y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 57, clsGlobVar.Tank57_PercentFill, clsGlobVar.CoordinatePlanA.Tank57ax, clsGlobVar.CoordinatePlanA.Tank57ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 57, clsGlobVar.Tank57_PercentFill, clsGlobVar.CoordinatePlanA.Tank57bx, clsGlobVar.CoordinatePlanA.Tank57by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 58, clsGlobVar.Tank58_PercentFill, clsGlobVar.CoordinatePlanA.Tank58ax, clsGlobVar.CoordinatePlanA.Tank58ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 58, clsGlobVar.Tank58_PercentFill, clsGlobVar.CoordinatePlanA.Tank58bx, clsGlobVar.CoordinatePlanA.Tank58by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 59, clsGlobVar.Tank59_PercentFill, clsGlobVar.CoordinatePlanA.Tank59ax, clsGlobVar.CoordinatePlanA.Tank59ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 59, clsGlobVar.Tank59_PercentFill, clsGlobVar.CoordinatePlanA.Tank59bx, clsGlobVar.CoordinatePlanA.Tank59by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 60, clsGlobVar.Tank60_PercentFill, clsGlobVar.CoordinatePlanA.Tank60ax, clsGlobVar.CoordinatePlanA.Tank60ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 60, clsGlobVar.Tank60_PercentFill, clsGlobVar.CoordinatePlanA.Tank60bx, clsGlobVar.CoordinatePlanA.Tank60by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 61, clsGlobVar.Tank61_PercentFill, clsGlobVar.CoordinatePlanA.Tank61ax, clsGlobVar.CoordinatePlanA.Tank61ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 61, clsGlobVar.Tank61_PercentFill, clsGlobVar.CoordinatePlanA.Tank61bx, clsGlobVar.CoordinatePlanA.Tank61by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 62, clsGlobVar.Tank62_PercentFill, clsGlobVar.CoordinatePlanA.Tank62ax, clsGlobVar.CoordinatePlanA.Tank62ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 62, clsGlobVar.Tank62_PercentFill, clsGlobVar.CoordinatePlanA.Tank62bx, clsGlobVar.CoordinatePlanA.Tank62by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 63, clsGlobVar.Tank63_PercentFill, clsGlobVar.CoordinatePlanA.Tank63ax, clsGlobVar.CoordinatePlanA.Tank63ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 63, clsGlobVar.Tank63_PercentFill, clsGlobVar.CoordinatePlanA.Tank63bx, clsGlobVar.CoordinatePlanA.Tank63by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 64, clsGlobVar.Tank64_PercentFill, clsGlobVar.CoordinatePlanA.Tank64ax, clsGlobVar.CoordinatePlanA.Tank64ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 64, clsGlobVar.Tank64_PercentFill, clsGlobVar.CoordinatePlanA.Tank64bx, clsGlobVar.CoordinatePlanA.Tank64by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 65, clsGlobVar.Tank65_PercentFill, clsGlobVar.CoordinatePlanA.Tank65ax, clsGlobVar.CoordinatePlanA.Tank65ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 65, clsGlobVar.Tank65_PercentFill, clsGlobVar.CoordinatePlanA.Tank65bx, clsGlobVar.CoordinatePlanA.Tank65by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 66, clsGlobVar.Tank66_PercentFill, clsGlobVar.CoordinatePlanA.Tank66x, clsGlobVar.CoordinatePlanA.Tank66y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 67, clsGlobVar.Tank67_PercentFill, clsGlobVar.CoordinatePlanA.Tank67x, clsGlobVar.CoordinatePlanA.Tank67y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 68, clsGlobVar.Tank68_PercentFill, clsGlobVar.CoordinatePlanA.Tank68x, clsGlobVar.CoordinatePlanA.Tank68y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 69, clsGlobVar.Tank69_PercentFill, clsGlobVar.CoordinatePlanA.Tank69x, clsGlobVar.CoordinatePlanA.Tank69y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 70, clsGlobVar.Tank70_PercentFill, clsGlobVar.CoordinatePlanA.Tank70x, clsGlobVar.CoordinatePlanA.Tank70y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 71, clsGlobVar.Tank71_PercentFill, clsGlobVar.CoordinatePlanA.Tank71x, clsGlobVar.CoordinatePlanA.Tank71y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 72, clsGlobVar.Tank72_PercentFill, clsGlobVar.CoordinatePlanA.Tank72x, clsGlobVar.CoordinatePlanA.Tank72y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 73, clsGlobVar.Tank73_PercentFill, clsGlobVar.CoordinatePlanA.Tank73x, clsGlobVar.CoordinatePlanA.Tank73y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 74, clsGlobVar.Tank74_PercentFill, clsGlobVar.CoordinatePlanA.Tank74x, clsGlobVar.CoordinatePlanA.Tank74y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 75, clsGlobVar.Tank75_PercentFill, clsGlobVar.CoordinatePlanA.Tank75x, clsGlobVar.CoordinatePlanA.Tank75y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 76, clsGlobVar.Tank76_PercentFill, clsGlobVar.CoordinatePlanA.Tank76x, clsGlobVar.CoordinatePlanA.Tank76y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 77, clsGlobVar.Tank77_PercentFill, clsGlobVar.CoordinatePlanA.Tank77x, clsGlobVar.CoordinatePlanA.Tank77y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 78, clsGlobVar.Tank78_PercentFill, clsGlobVar.CoordinatePlanA.Tank78x, clsGlobVar.CoordinatePlanA.Tank78y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 79, clsGlobVar.Tank79_PercentFill, clsGlobVar.CoordinatePlanA.Tank79x, clsGlobVar.CoordinatePlanA.Tank79y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 80, clsGlobVar.Tank80_PercentFill, clsGlobVar.CoordinatePlanA.Tank80x, clsGlobVar.CoordinatePlanA.Tank80y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 81, clsGlobVar.Tank81_PercentFill, clsGlobVar.CoordinatePlanA.Tank81x, clsGlobVar.CoordinatePlanA.Tank81y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 82, clsGlobVar.Tank82_PercentFill, clsGlobVar.CoordinatePlanA.Tank82x, clsGlobVar.CoordinatePlanA.Tank82y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 83, clsGlobVar.Tank83_PercentFill, clsGlobVar.CoordinatePlanA.Tank83x, clsGlobVar.CoordinatePlanA.Tank83y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 84, clsGlobVar.Tank84_PercentFill, clsGlobVar.CoordinatePlanA.Tank84x, clsGlobVar.CoordinatePlanA.Tank84y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 85, clsGlobVar.Tank85_PercentFill, clsGlobVar.CoordinatePlanA.Tank85x, clsGlobVar.CoordinatePlanA.Tank85y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 86, clsGlobVar.Tank86_PercentFill, clsGlobVar.CoordinatePlanA.Tank86x, clsGlobVar.CoordinatePlanA.Tank86y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 87, clsGlobVar.Tank87_PercentFill, clsGlobVar.CoordinatePlanA.Tank87x, clsGlobVar.CoordinatePlanA.Tank87y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchDeckPlan(canvas2DPlanA, 88, clsGlobVar.Tank88_PercentFill, clsGlobVar.CoordinatePlanA.Tank88x, clsGlobVar.CoordinatePlanA.Tank88y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchDeckPlan(canvas2DPlanA, 89, clsGlobVar.Tank89_PercentFill, clsGlobVar.CoordinatePlanA.Tank89x, clsGlobVar.CoordinatePlanA.Tank89y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchDeckPlan(canvas2DPlanA, 90, clsGlobVar.Tank90_PercentFill, clsGlobVar.CoordinatePlanA.Tank90x, clsGlobVar.CoordinatePlanA.Tank90y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchDeckPlan(canvas2DPlanA, 154, clsGlobVar.Tank154_PercentFill, clsGlobVar.CoordinatePlanA.Tank154ax, clsGlobVar.CoordinatePlanA.Tank154ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchDeckPlan(canvas2DPlanA, 154, clsGlobVar.Tank154_PercentFill, clsGlobVar.CoordinatePlanA.Tank154bx, clsGlobVar.CoordinatePlanA.Tank154by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchDeckPlan(canvas2DPlanA, 155, clsGlobVar.Tank155_PercentFill, clsGlobVar.CoordinatePlanA.Tank155ax, clsGlobVar.CoordinatePlanA.Tank155ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchDeckPlan(canvas2DPlanA, 155, clsGlobVar.Tank155_PercentFill, clsGlobVar.CoordinatePlanA.Tank155bx, clsGlobVar.CoordinatePlanA.Tank155by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchDeckPlan(canvas2DPlanA, 156, clsGlobVar.Tank156_PercentFill, clsGlobVar.CoordinatePlanA.Tank156ax, clsGlobVar.CoordinatePlanA.Tank156ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchDeckPlan(canvas2DPlanA, 156, clsGlobVar.Tank156_PercentFill, clsGlobVar.CoordinatePlanA.Tank156bx, clsGlobVar.CoordinatePlanA.Tank156by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchDeckPlan(canvas2DPlanA, 157, clsGlobVar.Tank157_PercentFill, clsGlobVar.CoordinatePlanA.Tank157ax, clsGlobVar.CoordinatePlanA.Tank157ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchDeckPlan(canvas2DPlanA, 157, clsGlobVar.Tank157_PercentFill, clsGlobVar.CoordinatePlanA.Tank157bx, clsGlobVar.CoordinatePlanA.Tank157by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchDeckPlan(canvas2DPlanA, 438, clsGlobVar.Tank438_PercentFill, clsGlobVar.CoordinatePlanA.Tank438x, clsGlobVar.CoordinatePlanA.Tank438y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchDeckPlan(canvas2DPlanA, 442, clsGlobVar.Tank442_PercentFill, clsGlobVar.CoordinatePlanA.Tank442x, clsGlobVar.CoordinatePlanA.Tank442y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchDeckPlan(canvas2DPlanA, 451, clsGlobVar.Tank451_PercentFill, clsGlobVar.CoordinatePlanA.Tank451x, clsGlobVar.CoordinatePlanA.Tank451y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchDeckPlan(canvas2DPlanA, 456, clsGlobVar.Tank456_PercentFill, clsGlobVar.CoordinatePlanA.Tank456x, clsGlobVar.CoordinatePlanA.Tank456y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchDeckPlan(canvas2DPlanA, 460, clsGlobVar.Tank460_PercentFill, clsGlobVar.CoordinatePlanA.Tank460x, clsGlobVar.CoordinatePlanA.Tank460y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchDeckPlan(canvas2DPlanA, 464, clsGlobVar.Tank464_PercentFill, clsGlobVar.CoordinatePlanA.Tank464x, clsGlobVar.CoordinatePlanA.Tank464y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchDeckPlan(canvas2DPlanA, 466, clsGlobVar.Tank466_PercentFill, clsGlobVar.CoordinatePlanA.Tank466x, clsGlobVar.CoordinatePlanA.Tank466y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchDeckPlan(canvas2DPlanA, 469, clsGlobVar.Tank469_PercentFill, clsGlobVar.CoordinatePlanA.Tank469x, clsGlobVar.CoordinatePlanA.Tank469y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchDeckPlan(canvas2DPlanA, 473, clsGlobVar.Tank473_PercentFill, clsGlobVar.CoordinatePlanA.Tank473x, clsGlobVar.CoordinatePlanA.Tank473y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchDeckPlan(canvas2DPlanA, 475, clsGlobVar.Tank475_PercentFill, clsGlobVar.CoordinatePlanA.Tank475x, clsGlobVar.CoordinatePlanA.Tank475y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchDeckPlan(canvas2DPlanA, 477, clsGlobVar.Tank477_PercentFill, clsGlobVar.CoordinatePlanA.Tank477ax, clsGlobVar.CoordinatePlanA.Tank477ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchDeckPlan(canvas2DPlanA, 477, clsGlobVar.Tank477_PercentFill, clsGlobVar.CoordinatePlanA.Tank477bx, clsGlobVar.CoordinatePlanA.Tank477by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchDeckPlan(canvas2DPlanA, 479, clsGlobVar.Tank479_PercentFill, clsGlobVar.CoordinatePlanA.Tank479ax, clsGlobVar.CoordinatePlanA.Tank479ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchDeckPlan(canvas2DPlanA, 479, clsGlobVar.Tank479_PercentFill, clsGlobVar.CoordinatePlanA.Tank479bx, clsGlobVar.CoordinatePlanA.Tank479by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchDeckPlan(canvas2DPlanA, 482, clsGlobVar.Tank482_PercentFill, clsGlobVar.CoordinatePlanA.Tank482x, clsGlobVar.CoordinatePlanA.Tank482y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchDeckPlan(canvas2DPlanA, 483, clsGlobVar.Tank483_PercentFill, clsGlobVar.CoordinatePlanA.Tank483ax, clsGlobVar.CoordinatePlanA.Tank483ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchDeckPlan(canvas2DPlanA, 483, clsGlobVar.Tank483_PercentFill, clsGlobVar.CoordinatePlanA.Tank483bx, clsGlobVar.CoordinatePlanA.Tank483by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchDeckPlan(canvas2DPlanA, 484, clsGlobVar.Tank484_PercentFill, clsGlobVar.CoordinatePlanA.Tank484ax, clsGlobVar.CoordinatePlanA.Tank484ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchDeckPlan(canvas2DPlanA, 484, clsGlobVar.Tank484_PercentFill, clsGlobVar.CoordinatePlanA.Tank484bx, clsGlobVar.CoordinatePlanA.Tank484by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchDeckPlan(canvas2DPlanA, 487, clsGlobVar.Tank487_PercentFill, clsGlobVar.CoordinatePlanA.Tank487x, clsGlobVar.CoordinatePlanA.Tank487y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchDeckPlan(canvas2DPlanA, 488, clsGlobVar.Tank488_PercentFill, clsGlobVar.CoordinatePlanA.Tank488ax, clsGlobVar.CoordinatePlanA.Tank488ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchDeckPlan(canvas2DPlanA, 488, clsGlobVar.Tank488_PercentFill, clsGlobVar.CoordinatePlanA.Tank488bx, clsGlobVar.CoordinatePlanA.Tank488by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchDeckPlan(canvas2DPlanA, 489, clsGlobVar.Tank489_PercentFill, clsGlobVar.CoordinatePlanA.Tank489ax, clsGlobVar.CoordinatePlanA.Tank489ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchDeckPlan(canvas2DPlanA, 489, clsGlobVar.Tank489_PercentFill, clsGlobVar.CoordinatePlanA.Tank489bx, clsGlobVar.CoordinatePlanA.Tank489by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchDeckPlan(canvas2DPlanA, 499, clsGlobVar.Tank499_PercentFill, clsGlobVar.CoordinatePlanA.Tank499ax, clsGlobVar.CoordinatePlanA.Tank499ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchDeckPlan(canvas2DPlanA, 499, clsGlobVar.Tank499_PercentFill, clsGlobVar.CoordinatePlanA.Tank499bx, clsGlobVar.CoordinatePlanA.Tank499by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchDeckPlan(canvas2DPlanA, 500, clsGlobVar.Tank500_PercentFill, clsGlobVar.CoordinatePlanA.Tank500ax, clsGlobVar.CoordinatePlanA.Tank500ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchDeckPlan(canvas2DPlanA, 500, clsGlobVar.Tank500_PercentFill, clsGlobVar.CoordinatePlanA.Tank500bx, clsGlobVar.CoordinatePlanA.Tank500by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchDeckPlan(canvas2DPlanA, 501, clsGlobVar.Tank501_PercentFill, clsGlobVar.CoordinatePlanA.Tank501ax, clsGlobVar.CoordinatePlanA.Tank501ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchDeckPlan(canvas2DPlanA, 501, clsGlobVar.Tank501_PercentFill, clsGlobVar.CoordinatePlanA.Tank501bx, clsGlobVar.CoordinatePlanA.Tank501by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchDeckPlan(canvas2DPlanA, 502, clsGlobVar.Tank502_PercentFill, clsGlobVar.CoordinatePlanA.Tank502ax, clsGlobVar.CoordinatePlanA.Tank502ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchDeckPlan(canvas2DPlanA, 502, clsGlobVar.Tank502_PercentFill, clsGlobVar.CoordinatePlanA.Tank502bx, clsGlobVar.CoordinatePlanA.Tank502by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));



            }
            catch //(Exception ex)
            {
               // System.Windows.MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Add Hatch Filling of Tanks/Compartments to DeckPlans Canvas as per TankID 
        /// </summary>
        public void AddHatchDeckPlanC()
        {
            try
            {

                canvas2DPlanC.Children.RemoveRange(1, canvas2DPlanC.Children.Count - 1);
                //DrawHatchDeckPlan(canvas2DPlanC, 142, clsGlobVar.Tank142_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank142x, clsGlobVar.CoordinatePlanC.Tank142y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 143, clsGlobVar.Tank143_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank143x, clsGlobVar.CoordinatePlanC.Tank143y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 144, clsGlobVar.Tank144_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank144x, clsGlobVar.CoordinatePlanC.Tank144y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 148, clsGlobVar.Tank148_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank148x, clsGlobVar.CoordinatePlanC.Tank148y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 149, clsGlobVar.Tank149_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank149ax, clsGlobVar.CoordinatePlanC.Tank149ay, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 149, clsGlobVar.Tank149_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank149bx, clsGlobVar.CoordinatePlanC.Tank149by, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 178, clsGlobVar.Tank178_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank178x, clsGlobVar.CoordinatePlanC.Tank178y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 188, clsGlobVar.Tank188_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank188x, clsGlobVar.CoordinatePlanC.Tank188y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 302, clsGlobVar.Tank302_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank302x, clsGlobVar.CoordinatePlanC.Tank302y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 303, clsGlobVar.Tank303_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank303x, clsGlobVar.CoordinatePlanC.Tank303y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 304, clsGlobVar.Tank304_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank304x, clsGlobVar.CoordinatePlanC.Tank304y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 305, clsGlobVar.Tank305_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank305x, clsGlobVar.CoordinatePlanC.Tank305y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 306, clsGlobVar.Tank306_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank306x, clsGlobVar.CoordinatePlanC.Tank306y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 307, clsGlobVar.Tank307_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank307x, clsGlobVar.CoordinatePlanC.Tank307y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 308, clsGlobVar.Tank308_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank308x, clsGlobVar.CoordinatePlanC.Tank308y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 309, clsGlobVar.Tank309_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank309x, clsGlobVar.CoordinatePlanC.Tank309y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 310, clsGlobVar.Tank310_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank310x, clsGlobVar.CoordinatePlanC.Tank310y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 311, clsGlobVar.Tank311_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank311x, clsGlobVar.CoordinatePlanC.Tank311y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 312, clsGlobVar.Tank312_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank312x, clsGlobVar.CoordinatePlanC.Tank312y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 313, clsGlobVar.Tank313_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank313x, clsGlobVar.CoordinatePlanC.Tank313y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 314, clsGlobVar.Tank314_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank314x, clsGlobVar.CoordinatePlanC.Tank314y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 315, clsGlobVar.Tank315_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank315x, clsGlobVar.CoordinatePlanC.Tank315y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 316, clsGlobVar.Tank316_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank316x, clsGlobVar.CoordinatePlanC.Tank316y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 318, clsGlobVar.Tank318_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank318x, clsGlobVar.CoordinatePlanC.Tank318y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 319, clsGlobVar.Tank319_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank319x, clsGlobVar.CoordinatePlanC.Tank319y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 320, clsGlobVar.Tank320_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank320x, clsGlobVar.CoordinatePlanC.Tank320y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 321, clsGlobVar.Tank321_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank321x, clsGlobVar.CoordinatePlanC.Tank321y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 322, clsGlobVar.Tank322_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank322x, clsGlobVar.CoordinatePlanC.Tank322y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 323, clsGlobVar.Tank323_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank323x, clsGlobVar.CoordinatePlanC.Tank323y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 324, clsGlobVar.Tank324_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank324x, clsGlobVar.CoordinatePlanC.Tank324y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 325, clsGlobVar.Tank325_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank325x, clsGlobVar.CoordinatePlanC.Tank325y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 326, clsGlobVar.Tank326_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank326x, clsGlobVar.CoordinatePlanC.Tank326y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 327, clsGlobVar.Tank327_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank327x, clsGlobVar.CoordinatePlanC.Tank327y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 328, clsGlobVar.Tank328_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank328x, clsGlobVar.CoordinatePlanC.Tank328y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 329, clsGlobVar.Tank329_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank329x, clsGlobVar.CoordinatePlanC.Tank329y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 330, clsGlobVar.Tank330_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank330x, clsGlobVar.CoordinatePlanC.Tank330y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 331, clsGlobVar.Tank331_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank331x, clsGlobVar.CoordinatePlanC.Tank331y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 332, clsGlobVar.Tank332_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank332x, clsGlobVar.CoordinatePlanC.Tank332y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 333, clsGlobVar.Tank333_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank333x, clsGlobVar.CoordinatePlanC.Tank333y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 334, clsGlobVar.Tank334_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank334x, clsGlobVar.CoordinatePlanC.Tank334y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 335, clsGlobVar.Tank335_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank335x, clsGlobVar.CoordinatePlanC.Tank335y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 336, clsGlobVar.Tank336_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank336x, clsGlobVar.CoordinatePlanC.Tank336y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 337, clsGlobVar.Tank337_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank337x, clsGlobVar.CoordinatePlanC.Tank337y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 338, clsGlobVar.Tank338_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank338x, clsGlobVar.CoordinatePlanC.Tank338y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 339, clsGlobVar.Tank339_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank339x, clsGlobVar.CoordinatePlanC.Tank339y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 340, clsGlobVar.Tank340_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank340x, clsGlobVar.CoordinatePlanC.Tank340y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 341, clsGlobVar.Tank341_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank341x, clsGlobVar.CoordinatePlanC.Tank341y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 342, clsGlobVar.Tank342_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank342x, clsGlobVar.CoordinatePlanC.Tank342y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 343, clsGlobVar.Tank343_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank343x, clsGlobVar.CoordinatePlanC.Tank343y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 344, clsGlobVar.Tank344_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank344x, clsGlobVar.CoordinatePlanC.Tank344y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 345, clsGlobVar.Tank345_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank345x, clsGlobVar.CoordinatePlanC.Tank345y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 346, clsGlobVar.Tank346_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank346x, clsGlobVar.CoordinatePlanC.Tank346y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 347, clsGlobVar.Tank347_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank347x, clsGlobVar.CoordinatePlanC.Tank347y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 348, clsGlobVar.Tank348_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank348x, clsGlobVar.CoordinatePlanC.Tank348y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 349, clsGlobVar.Tank349_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank349x, clsGlobVar.CoordinatePlanC.Tank349y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 350, clsGlobVar.Tank350_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank350x, clsGlobVar.CoordinatePlanC.Tank350y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 351, clsGlobVar.Tank351_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank351x, clsGlobVar.CoordinatePlanC.Tank351y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 352, clsGlobVar.Tank352_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank352x, clsGlobVar.CoordinatePlanC.Tank352y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 353, clsGlobVar.Tank353_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank353x, clsGlobVar.CoordinatePlanC.Tank353y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 354, clsGlobVar.Tank354_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank354x, clsGlobVar.CoordinatePlanC.Tank354y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 355, clsGlobVar.Tank355_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank355x, clsGlobVar.CoordinatePlanC.Tank355y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 356, clsGlobVar.Tank356_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank356x, clsGlobVar.CoordinatePlanC.Tank356y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 357, clsGlobVar.Tank357_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank357x, clsGlobVar.CoordinatePlanC.Tank357y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 358, clsGlobVar.Tank358_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank358x, clsGlobVar.CoordinatePlanC.Tank358y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 359, clsGlobVar.Tank359_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank359x, clsGlobVar.CoordinatePlanC.Tank359y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 360, clsGlobVar.Tank360_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank360x, clsGlobVar.CoordinatePlanC.Tank360y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 362, clsGlobVar.Tank362_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank362x, clsGlobVar.CoordinatePlanC.Tank362y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 363, clsGlobVar.Tank363_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank363x, clsGlobVar.CoordinatePlanC.Tank363y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 364, clsGlobVar.Tank364_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank364x, clsGlobVar.CoordinatePlanC.Tank364y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 365, clsGlobVar.Tank365_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank365x, clsGlobVar.CoordinatePlanC.Tank365y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 366, clsGlobVar.Tank366_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank366x, clsGlobVar.CoordinatePlanC.Tank366y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 367, clsGlobVar.Tank367_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank367x, clsGlobVar.CoordinatePlanC.Tank367y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 368, clsGlobVar.Tank368_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank368x, clsGlobVar.CoordinatePlanC.Tank368y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 369, clsGlobVar.Tank369_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank369x, clsGlobVar.CoordinatePlanC.Tank369y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 370, clsGlobVar.Tank370_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank370x, clsGlobVar.CoordinatePlanC.Tank370y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 371, clsGlobVar.Tank371_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank371x, clsGlobVar.CoordinatePlanC.Tank371y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 372, clsGlobVar.Tank372_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank372x, clsGlobVar.CoordinatePlanC.Tank372y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 373, clsGlobVar.Tank373_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank373x, clsGlobVar.CoordinatePlanC.Tank373y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 374, clsGlobVar.Tank374_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank374x, clsGlobVar.CoordinatePlanC.Tank374y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 375, clsGlobVar.Tank375_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank375x, clsGlobVar.CoordinatePlanC.Tank375y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 376, clsGlobVar.Tank376_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank376x, clsGlobVar.CoordinatePlanC.Tank376y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 377, clsGlobVar.Tank377_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank377x, clsGlobVar.CoordinatePlanC.Tank377y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 378, clsGlobVar.Tank378_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank378x, clsGlobVar.CoordinatePlanC.Tank378y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 379, clsGlobVar.Tank379_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank379x, clsGlobVar.CoordinatePlanC.Tank379y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 380, clsGlobVar.Tank380_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank380x, clsGlobVar.CoordinatePlanC.Tank380y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 381, clsGlobVar.Tank381_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank381x, clsGlobVar.CoordinatePlanC.Tank381y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 382, clsGlobVar.Tank382_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank382x, clsGlobVar.CoordinatePlanC.Tank382y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 383, clsGlobVar.Tank383_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank383x, clsGlobVar.CoordinatePlanC.Tank383y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 384, clsGlobVar.Tank384_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank384x, clsGlobVar.CoordinatePlanC.Tank384y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 385, clsGlobVar.Tank385_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank385x, clsGlobVar.CoordinatePlanC.Tank385y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 386, clsGlobVar.Tank386_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank386x, clsGlobVar.CoordinatePlanC.Tank386y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 387, clsGlobVar.Tank387_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank387x, clsGlobVar.CoordinatePlanC.Tank387y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 388, clsGlobVar.Tank388_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank388x, clsGlobVar.CoordinatePlanC.Tank388y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 389, clsGlobVar.Tank389_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank389x, clsGlobVar.CoordinatePlanC.Tank389y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 390, clsGlobVar.Tank390_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank390x, clsGlobVar.CoordinatePlanC.Tank390y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 391, clsGlobVar.Tank391_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank391x, clsGlobVar.CoordinatePlanC.Tank391y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 392, clsGlobVar.Tank392_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank392x, clsGlobVar.CoordinatePlanC.Tank392y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 393, clsGlobVar.Tank393_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank393x, clsGlobVar.CoordinatePlanC.Tank393y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 394, clsGlobVar.Tank394_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank394x, clsGlobVar.CoordinatePlanC.Tank394y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 395, clsGlobVar.Tank395_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank395x, clsGlobVar.CoordinatePlanC.Tank395y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 396, clsGlobVar.Tank396_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank396x, clsGlobVar.CoordinatePlanC.Tank396y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 397, clsGlobVar.Tank397_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank397x, clsGlobVar.CoordinatePlanC.Tank397y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 399, clsGlobVar.Tank399_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank399x, clsGlobVar.CoordinatePlanC.Tank399y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 400, clsGlobVar.Tank400_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank400x, clsGlobVar.CoordinatePlanC.Tank400y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 401, clsGlobVar.Tank401_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank401x, clsGlobVar.CoordinatePlanC.Tank401y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 402, clsGlobVar.Tank402_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank402x, clsGlobVar.CoordinatePlanC.Tank402y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 403, clsGlobVar.Tank403_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank403x, clsGlobVar.CoordinatePlanC.Tank403y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 404, clsGlobVar.Tank404_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank404x, clsGlobVar.CoordinatePlanC.Tank404y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 405, clsGlobVar.Tank405_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank405x, clsGlobVar.CoordinatePlanC.Tank405y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 406, clsGlobVar.Tank406_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank406x, clsGlobVar.CoordinatePlanC.Tank406y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 407, clsGlobVar.Tank407_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank407x, clsGlobVar.CoordinatePlanC.Tank407y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 408, clsGlobVar.Tank408_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank408x, clsGlobVar.CoordinatePlanC.Tank408y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 409, clsGlobVar.Tank409_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank409x, clsGlobVar.CoordinatePlanC.Tank409y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 410, clsGlobVar.Tank410_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank410x, clsGlobVar.CoordinatePlanC.Tank410y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 411, clsGlobVar.Tank411_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank411x, clsGlobVar.CoordinatePlanC.Tank411y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 412, clsGlobVar.Tank412_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank412x, clsGlobVar.CoordinatePlanC.Tank412y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 413, clsGlobVar.Tank413_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank413x, clsGlobVar.CoordinatePlanC.Tank413y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 414, clsGlobVar.Tank414_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank414x, clsGlobVar.CoordinatePlanC.Tank414y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 415, clsGlobVar.Tank415_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank415x, clsGlobVar.CoordinatePlanC.Tank415y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 416, clsGlobVar.Tank416_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank416ax, clsGlobVar.CoordinatePlanC.Tank416ay, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 416, clsGlobVar.Tank416_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank416bx, clsGlobVar.CoordinatePlanC.Tank416by, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 417, clsGlobVar.Tank417_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank417x, clsGlobVar.CoordinatePlanC.Tank417y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 418, clsGlobVar.Tank418_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank418x, clsGlobVar.CoordinatePlanC.Tank418y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 419, clsGlobVar.Tank419_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank419x, clsGlobVar.CoordinatePlanC.Tank419y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 420, clsGlobVar.Tank420_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank420x, clsGlobVar.CoordinatePlanC.Tank420y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 421, clsGlobVar.Tank421_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank421x, clsGlobVar.CoordinatePlanC.Tank421y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 423, clsGlobVar.Tank423_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank423x, clsGlobVar.CoordinatePlanC.Tank423y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 424, clsGlobVar.Tank424_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank424x, clsGlobVar.CoordinatePlanC.Tank424y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 425, clsGlobVar.Tank425_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank425x, clsGlobVar.CoordinatePlanC.Tank425y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 426, clsGlobVar.Tank426_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank426x, clsGlobVar.CoordinatePlanC.Tank426y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 427, clsGlobVar.Tank427_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank427x, clsGlobVar.CoordinatePlanC.Tank427y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 428, clsGlobVar.Tank428_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank428x, clsGlobVar.CoordinatePlanC.Tank428y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 429, clsGlobVar.Tank429_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank429x, clsGlobVar.CoordinatePlanC.Tank429y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 430, clsGlobVar.Tank430_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank430x, clsGlobVar.CoordinatePlanC.Tank430y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 431, clsGlobVar.Tank431_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank431x, clsGlobVar.CoordinatePlanC.Tank431y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 432, clsGlobVar.Tank432_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank432x, clsGlobVar.CoordinatePlanC.Tank432y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 433, clsGlobVar.Tank433_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank433x, clsGlobVar.CoordinatePlanC.Tank433y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 434, clsGlobVar.Tank434_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank434x, clsGlobVar.CoordinatePlanC.Tank434y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 435, clsGlobVar.Tank435_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank435x, clsGlobVar.CoordinatePlanC.Tank435y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 439, clsGlobVar.Tank439_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank439x, clsGlobVar.CoordinatePlanC.Tank439y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 443, clsGlobVar.Tank443_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank443x, clsGlobVar.CoordinatePlanC.Tank443y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 448, clsGlobVar.Tank448_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank448x, clsGlobVar.CoordinatePlanC.Tank448y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 449, clsGlobVar.Tank449_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank449x, clsGlobVar.CoordinatePlanC.Tank449y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 452, clsGlobVar.Tank452_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank452x, clsGlobVar.CoordinatePlanC.Tank452y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 453, clsGlobVar.Tank453_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank453x, clsGlobVar.CoordinatePlanC.Tank453y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 457, clsGlobVar.Tank457_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank457x, clsGlobVar.CoordinatePlanC.Tank457y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 461, clsGlobVar.Tank461_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank461x, clsGlobVar.CoordinatePlanC.Tank461y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 466, clsGlobVar.Tank466_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank466x, clsGlobVar.CoordinatePlanC.Tank466y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 467, clsGlobVar.Tank467_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank467x, clsGlobVar.CoordinatePlanC.Tank467y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 470, clsGlobVar.Tank470_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank470x, clsGlobVar.CoordinatePlanC.Tank470y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 475, clsGlobVar.Tank475_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank475ax, clsGlobVar.CoordinatePlanC.Tank475ay, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 475, clsGlobVar.Tank475_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank475bx, clsGlobVar.CoordinatePlanC.Tank475by, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 476, clsGlobVar.Tank476_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank476x, clsGlobVar.CoordinatePlanC.Tank476y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 478, clsGlobVar.Tank478_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank478x, clsGlobVar.CoordinatePlanC.Tank478y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                //DrawHatchDeckPlan(canvas2DPlanC, 494, clsGlobVar.Tank494_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank494x, clsGlobVar.CoordinatePlanC.Tank494y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));




            }
            catch //(Exception ex)
            {
               // System.Windows.MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// Add Trim Line to Profile View
        /// </summary>
        public void DrawTrimLine()
        {
            //@MT Code changed for Ship Profile Trim Line Should be Steady :START
            double xAP, yAP, xFP, yFP, yAvg, angle, DraftAP, DraftFP;
            DraftAP = Convert.ToDouble(lblDraftAP.Content);
            DraftFP = Convert.ToDouble(lblDraftFP.Content);
            angle = (DraftFP - DraftAP) / 2;
            yAvg = (DraftFP + DraftAP) / 2;
            xAP = 5000;
            yAP = 6250 + 1157.40 + ((yAvg - angle) * 1000);
            xFP = 172045;
            yFP = 6250 + 1157.40 + ((yAvg + angle) * 1000);

            //@MT Code changed for Ship Profile Trim Line Should be Steady :END


            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
            p.Stroke = System.Windows.Media.Brushes.Black;

            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
            mySolidColorBrush.Color = System.Windows.Media.Color.FromArgb(255, 0, 0, 255);
            p.Fill = mySolidColorBrush;

            System.Windows.Point[] point = new System.Windows.Point[5];
            PointCollection pointCollection = new PointCollection();
            pointCollection.Add(new System.Windows.Point(xAP, yAP - 200));
            pointCollection.Add(new System.Windows.Point(xFP, yFP - 200));
            pointCollection.Add(new System.Windows.Point(xFP, yFP));
            pointCollection.Add(new System.Windows.Point(xAP, yAP));

            p.Points = pointCollection;
            canvas2DProfile.Children.Add(p);

        }

        /// <summary>
        /// Logic for Adding hatches to Show Filling on deck plans
        /// </summary>
        /// <param name="canvasTwoD">Canvas on which hatch is to be created</param> 
        /// <param name="Tank_ID">TankID of Tank/Compartment.</param> 
        /// <param name="percent">Percentage Filling</param> 
        /// <param name="xx">Collection of all the x coordinates for drawing the hatch.</param> 
        /// <param name="yy">Collection of all the y coordinates for drawing the hatch.</param> 
        /// <param name="color">Hatch Color </param> 
        public void DrawHatchDeckPlan(Canvas canvasTwoD, int Tank_ID, decimal percent, double[] xx, double[] yy, System.Windows.Media.Color color)
        {
            try
            {
                if (Convert.ToBoolean(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[Tank_ID - 1]["IsDamaged"]) == true)
                {
                    System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                    p.Stroke = System.Windows.Media.Brushes.Black;

                    SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                    mySolidColorBrush.Color = System.Windows.Media.Color.FromArgb(220, 255, 0, 0);
                    p.Fill = mySolidColorBrush;

                    System.Windows.Point[] point = new System.Windows.Point[15];
                    PointCollection pointCollection = new PointCollection();
                    for (int index = 1; index <= 13; index++)
                    {

                        pointCollection.Add(new System.Windows.Point(xx[index], yy[index]));

                    }
                    p.Points = pointCollection;
                    canvasTwoD.Children.Add(p);


                }
                else
                    if (percent > 0 && percent <= 100)
                    {
                        System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                        p.Stroke = System.Windows.Media.Brushes.Black;

                        SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                        mySolidColorBrush.Color = color;
                        p.Fill = mySolidColorBrush;

                        System.Windows.Point[] point = new System.Windows.Point[15];
                        PointCollection pointCollection = new PointCollection();
                        for (int index = 1; index <= 13; index++)
                        {

                            pointCollection.Add(new System.Windows.Point(xx[index], yy[index]));

                        }
                        p.Points = pointCollection;
                        canvasTwoD.Children.Add(p);
                        percent = 0;
                    }
                    else if (percent == 0)
                    {
                        //System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                        //p.Stroke = System.Windows.Media.Brushes.Black;

                        //SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                        //mySolidColorBrush.Color = System.Windows.Media.Color.FromArgb(0, 255, 0, 0);
                        //p.Fill = mySolidColorBrush;

                        //System.Windows.Point[] point = new System.Windows.Point[15];
                        //PointCollection pointCollection = new PointCollection();
                        //for (int index = 1; index <= 14; index++)
                        //{
                        //    pointCollection.Add(new System.Windows.Point(xx[index], yy[index]));
                        //}
                        //p.Points = pointCollection;
                        //canvasTwoD.Children.Add(p);
                        percent = 0;
                    }
            }
            catch //(Exception ex)
            {
                //MessageBox.Show(ex.Message.ToString());
            }
        }

        /// <summary>
        /// Logic for Adding hatches to Show Filling on Profile plan as per Percentage
        /// </summary>
        /// <param name="canvasTwoD">Canvas on which hatch is to be created</param> 
        /// <param name="Tank_ID">TankID of Tank/Compartment.</param> 
        /// <param name="percent">Percentage Filling</param> 
        /// <param name="xx">Collection of all the x coordinates for drawing the hatch.</param> 
        /// <param name="yy">Collection of all the y coordinates for drawing the hatch.</param> 
        /// <param name="color">Hatch Color </param> 
        /// 

        // Commented the drawhatchprofile with adding new 

        //public void DrawHatchProfile(Canvas canvasTwoD, int Tank_ID, decimal percent, double[] xx, double[] yy, System.Windows.Media.Color color)
        //{
        //    try
        //    {
        //        if (Convert.ToBoolean(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[Tank_ID - 1]["IsDamaged"]) == true)
        //        {
        //            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
        //            p.Stroke = System.Windows.Media.Brushes.Black;
        //            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
        //            mySolidColorBrush.Color = System.Windows.Media.Color.FromArgb(220, 255, 0, 0);
        //            p.Fill = mySolidColorBrush;
        //            System.Windows.Point[] point = new System.Windows.Point[25];
        //            PointCollection pointCollection = new PointCollection();
        //            pointCollection.Add(new System.Windows.Point(xx[1], yy[1]));
        //            pointCollection.Add(new System.Windows.Point(xx[2], yy[2]));
        //            pointCollection.Add(new System.Windows.Point(xx[3], yy[3]));
        //            pointCollection.Add(new System.Windows.Point(xx[4], yy[4]));
        //            p.Points = pointCollection;
        //            canvasTwoD.Children.Add(p);

        //        }
        //        else if (Tank_ID == 31 || Tank_ID == 33 || Tank_ID == 35)
        //        {
        //            if (percent > 0 && percent <= 10)
        //            {
        //                System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
        //                p.Stroke = System.Windows.Media.Brushes.Black;
        //                SolidColorBrush mySolidColorBrush = new SolidColorBrush();
        //                mySolidColorBrush.Color = color;
        //                p.Fill = mySolidColorBrush;
        //                double d = yy[3] - yy[2];
        //                double Fill = Convert.ToInt32(percent) * (d / 10);
        //                System.Windows.Point[] point = new System.Windows.Point[25];
        //                PointCollection pointCollection = new PointCollection();
        //                pointCollection.Add(new System.Windows.Point(xx[1], yy[1]));
        //                pointCollection.Add(new System.Windows.Point(xx[2], yy[2]));
        //                pointCollection.Add(new System.Windows.Point(xx[2], yy[2] + Fill));
        //                pointCollection.Add(new System.Windows.Point(xx[1], yy[2] + Fill));
        //                p.Points = pointCollection;
        //                canvasTwoD.Children.Add(p);
        //                percent = 0;
        //            }
        //            else if (percent > 10 && percent <= 100)
        //            {
        //                System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
        //                p.Stroke = System.Windows.Media.Brushes.Black;
        //                SolidColorBrush mySolidColorBrush = new SolidColorBrush();
        //                mySolidColorBrush.Color = color;
        //                p.Fill = mySolidColorBrush;
        //                //double d = yy[2] - yy[1];
        //                //double Fill = Convert.ToInt32(percent) * (d / 10);
        //                System.Windows.Point[] point = new System.Windows.Point[25];
        //                PointCollection pointCollection = new PointCollection();
        //                pointCollection.Add(new System.Windows.Point(xx[1], yy[1]));
        //                pointCollection.Add(new System.Windows.Point(xx[2], yy[2]));
        //                pointCollection.Add(new System.Windows.Point(xx[3], yy[3]));
        //                pointCollection.Add(new System.Windows.Point(xx[4], yy[4]));
        //                p.Points = pointCollection;
        //                canvasTwoD.Children.Add(p);
        //                percent = 0;
        //            }

        //        }
        //        else if (percent > 0 && percent <= 100)
        //        {
        //            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
        //            p.Stroke = System.Windows.Media.Brushes.Black;
        //            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
        //            mySolidColorBrush.Color = color;
        //            p.Fill = mySolidColorBrush;
        //            double d = yy[3] - yy[2];
        //            double Fill = Convert.ToInt32(percent) * (d / 100);
        //            System.Windows.Point[] point = new System.Windows.Point[25];
        //            PointCollection pointCollection = new PointCollection();
        //            pointCollection.Add(new System.Windows.Point(xx[1], yy[1]));
        //            pointCollection.Add(new System.Windows.Point(xx[2], yy[2]));
        //            pointCollection.Add(new System.Windows.Point(xx[2], yy[2] + Fill));
        //            pointCollection.Add(new System.Windows.Point(xx[1], yy[2] + Fill));
        //            p.Points = pointCollection;
        //            canvasTwoD.Children.Add(p);
        //            percent = 0;
        //        }
        //        else if (percent == 0)
        //        {
        //            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
        //            p.Stroke = System.Windows.Media.Brushes.Black;
        //            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
        //            mySolidColorBrush.Color = System.Windows.Media.Color.FromArgb(0, 255, 255, 255);
        //            p.Fill = mySolidColorBrush;
        //            System.Windows.Point[] point = new System.Windows.Point[25];
        //            PointCollection pointCollection = new PointCollection();
        //            pointCollection.Add(new System.Windows.Point(xx[1], yy[1]));
        //            pointCollection.Add(new System.Windows.Point(xx[2], yy[2]));
        //            pointCollection.Add(new System.Windows.Point(xx[3], yy[3]));
        //            pointCollection.Add(new System.Windows.Point(xx[4], yy[4]));
        //            p.Points = pointCollection;
        //            canvasTwoD.Children.Add(p);
        //        }
        //    }
        //    catch
        //    {

        //    }
        //} //


        public void DrawHatchProfile(Canvas canvasTwoD, int Tank_ID, decimal percent, double[] xx, double[] yy, System.Windows.Media.Color color)
        {
            try
            {
                //if (Convert.ToBoolean(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[Tank_ID - 1]["IsDamaged"]) == true)
                if (Tank_ID == 1000)
                {
                    System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                    p.Stroke = System.Windows.Media.Brushes.Black;
                    SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                    mySolidColorBrush.Color = System.Windows.Media.Color.FromArgb(220, 255, 0, 0);
                    p.Fill = mySolidColorBrush;
                    System.Windows.Point[] point = new System.Windows.Point[25];
                    PointCollection pointCollection = new PointCollection();
                    pointCollection.Add(new System.Windows.Point(xx[1], yy[1]));
                    pointCollection.Add(new System.Windows.Point(xx[2], yy[2]));
                    pointCollection.Add(new System.Windows.Point(xx[3], yy[3]));
                    pointCollection.Add(new System.Windows.Point(xx[4], yy[4]));
                    p.Points = pointCollection;
                    canvasTwoD.Children.Add(p);

                }

                //#region curved
                //else if (Tank_ID == 38 && percent != 0)      //filling tank id 38 
                //{

                //    x4 = 4506.5651;
                //    y4 = 6319.306;
                //    x3 = -595.0785;
                //    y3 = 6231.4572;
                //    x2 = -187.8462;
                //    y2 = 4100;
                //    x1 = 4500.0096;
                //    y1 = 4100;

                //    System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                //    p.Stroke = System.Windows.Media.Brushes.Black;
                //    SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                //    mySolidColorBrush.Color = color;
                //    p.Fill = mySolidColorBrush;


                //    double d = y3 - y2;
                //    double Fill = Convert.ToInt32(percent) * (d / 100);
                //    double dx = x3 - x2;
                //    double Fillx = Convert.ToInt32(percent) * (dx / 100);
                //    System.Windows.Point[] point = new System.Windows.Point[25];
                //    PointCollection pointCollection = new PointCollection();
                //    pointCollection.Add(new System.Windows.Point(x1, y1));
                //    pointCollection.Add(new System.Windows.Point(x2, y2));
                //    pointCollection.Add(new System.Windows.Point(x2 + Fillx, y1 + Fill));
                //    pointCollection.Add(new System.Windows.Point(x1, y1 + Fill));
                //    p.Points = pointCollection;
                //    canvasTwoD.Children.Add(p);
                //    percent = 0;

                //}
                //else if (Tank_ID == 31 || Tank_ID == 33 || Tank_ID == 35)
                //{
                //    if (percent > 0 && percent <= 10)
                //    {
                //        System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                //        p.Stroke = System.Windows.Media.Brushes.Black;
                //        SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                //        mySolidColorBrush.Color = color;
                //        p.Fill = mySolidColorBrush;
                //        double d = yy[3] - yy[2];
                //        double Fill = Convert.ToInt32(percent) * (d / 10);
                //        System.Windows.Point[] point = new System.Windows.Point[25];
                //        PointCollection pointCollection = new PointCollection();
                //        pointCollection.Add(new System.Windows.Point(xx[1], yy[1]));
                //        pointCollection.Add(new System.Windows.Point(xx[2], yy[2]));
                //        pointCollection.Add(new System.Windows.Point(xx[2], yy[2] + Fill));
                //        pointCollection.Add(new System.Windows.Point(xx[1], yy[2] + Fill));
                //        p.Points = pointCollection;
                //        canvasTwoD.Children.Add(p);
                //        percent = 0;
                //    }
                //    else if (percent > 10 && percent <= 100)
                //    {
                //        System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                //        p.Stroke = System.Windows.Media.Brushes.Black;
                //        SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                //        mySolidColorBrush.Color = color;
                //        p.Fill = mySolidColorBrush;
                //        System.Windows.Point[] point = new System.Windows.Point[25];
                //        PointCollection pointCollection = new PointCollection();
                //        pointCollection.Add(new System.Windows.Point(xx[1], yy[1]));
                //        pointCollection.Add(new System.Windows.Point(xx[2], yy[2]));
                //        pointCollection.Add(new System.Windows.Point(xx[3], yy[3]));
                //        pointCollection.Add(new System.Windows.Point(xx[4], yy[4]));
                //        p.Points = pointCollection;
                //        canvasTwoD.Children.Add(p);
                //        percent = 0;
                //    }

                //}
                //else if ((Tank_ID == 44 || Tank_ID == 76) && percent != 0 && percent <= 10)   // else condition for  filling the curved tank <=10 compartment
                //{
                //    if (percent > 0 && percent <= 10)
                //    {

                //        if (Tank_ID == 44)
                //        {
                //            x1 = 87747.8509;
                //            y1 = 4000.037;
                //            x2 = 90732.8238;
                //            y2 = 4000.0047;
                //            x3 = 91474.9989;
                //            y3 = 4892.44;
                //            x4 = 92314.6868;
                //            y4 = 5864.5274;
                //            x5 = 92876.0048;
                //            y5 = 6500;
                //            x6 = 87747.8779;
                //            y6 = 6500;
                //        }
                //        else if (Tank_ID == 76)
                //        {
                //            x1 = 87747.8779;
                //            y1 = 6500;
                //            x2 = 92876.0048;
                //            y2 = 6500;
                //            x3 = 94123.9043;
                //            y3 = 7829.5821;
                //            x4 = 95249.6596;
                //            y4 = 8938.5823;
                //            x5 = 96326.8665;
                //            y5 = 9949.9084;
                //            x6 = 87747.8778;
                //            y6 = 9568.4435;


                //        }

                //        System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                //        p.Stroke = System.Windows.Media.Brushes.Black;
                //        SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                //        mySolidColorBrush.Color = color;
                //        p.Fill = mySolidColorBrush;


                //        double d = y3 - y2;
                //        double Fill = Convert.ToInt32(percent) * (d / 40);
                //        double dx = x3 - x2;
                //        double Fillx = Convert.ToInt32(percent) * (dx / 40);
                //        System.Windows.Point[] point = new System.Windows.Point[25];
                //        PointCollection pointCollection = new PointCollection();
                //        pointCollection.Add(new System.Windows.Point(x1, y1));
                //        pointCollection.Add(new System.Windows.Point(x2, y2));
                //        pointCollection.Add(new System.Windows.Point(x2 + Fillx, y1 + Fill));
                //        pointCollection.Add(new System.Windows.Point(x1, y1 + Fill));
                //        p.Points = pointCollection;
                //        canvasTwoD.Children.Add(p);
                //        percent = 0;
                //    }
                //}


                //else if ((Tank_ID == 43 || Tank_ID == 44 || Tank_ID == 76) && percent != 0)   // else condition for  filling the curved compartment
                //{
                //    if (percent > 0 && percent <= 10)
                //    {
                //        if (Tank_ID == 43)
                //        {
                //            x1 = 85499.9826;
                //            y1 = 35.895;
                //            x2 = 87288.4132;
                //            y2 = 537.4795;
                //            x3 = 88398.7617;
                //            y3 = 1299.9649;
                //            x4 = 89742.2384;
                //            y4 = 2808.8661;
                //            x5 = 90732.8238;
                //            y5 = 4000.0047;
                //            x6 = 85499.9826;
                //        }


                //        System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                //        p.Stroke = System.Windows.Media.Brushes.Black;
                //        SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                //        mySolidColorBrush.Color = color;
                //        p.Fill = mySolidColorBrush;


                //        double d = y2 - y1;
                //        double Fill = Convert.ToInt32(percent) * (d / 10);
                //        double dx = x2 - x1;
                //        double Fillx = Convert.ToInt32(percent) * (dx / 10);
                //        System.Windows.Point[] point = new System.Windows.Point[25];
                //        PointCollection pointCollection = new PointCollection();
                //        pointCollection.Add(new System.Windows.Point(x1, y1));
                //        pointCollection.Add(new System.Windows.Point(x1 + Fillx, y1 + Fill));
                //        pointCollection.Add(new System.Windows.Point(x1, y1 + Fill));
                //        p.Points = pointCollection;
                //        canvasTwoD.Children.Add(p);
                //        percent = 0;
                //    }
                //    else if (percent > 10 && percent <= 40)
                //    {
                //        if (Tank_ID == 43)
                //        {
                //            x1 = 85499.9826;
                //            y1 = 35.895;
                //            x2 = 87288.4132;
                //            y2 = 537.4795;
                //            x3 = 88398.7617;
                //            y3 = 1299.9649;
                //            x4 = 89742.2384;
                //            y4 = 2808.8661;
                //            x5 = 90732.8238;
                //            y5 = 4000.0047;
                //            x6 = 85499.9826;
                //        }
                //        else if (Tank_ID == 44)
                //        {
                //            x1 = 87747.8509;
                //            y1 = 4000.037;
                //            x2 = 90732.8238;
                //            y2 = 4000.0047;
                //            x3 = 91474.9989;
                //            y3 = 4892.44;
                //            x4 = 92314.6868;
                //            y4 = 5864.5274;
                //            x5 = 92876.0048;
                //            y5 = 6500;
                //            x6 = 87747.8779;
                //            y6 = 6500;
                //        }

                //        else if (Tank_ID == 76)
                //        {
                //            x1 = 51000.8661;
                //            x1 = 87747.8779;
                //            y1 = 6500;
                //            x2 = 92876.0048;
                //            y2 = 6500;
                //            x3 = 94123.9043;
                //            y3 = 7829.5821;
                //            x4 = 95249.6596;
                //            y4 = 8938.5823;
                //            x5 = 96326.8665;
                //            y5 = 9949.9084;
                //            x6 = 87747.8778;
                //            y6 = 9568.4435;

                //        }


                //        System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                //        p.Stroke = System.Windows.Media.Brushes.Black;
                //        SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                //        mySolidColorBrush.Color = color;
                //        p.Fill = mySolidColorBrush;

                //        double d = y3 - y2;
                //        double Fill = Convert.ToInt32(percent) * (d / 40);
                //        double dx = x3 - x2;
                //        double Fillx = Convert.ToInt32(percent) * (dx / 40);
                //        System.Windows.Point[] point = new System.Windows.Point[25];
                //        PointCollection pointCollection = new PointCollection();
                //        pointCollection.Add(new System.Windows.Point(x1, y1));
                //        pointCollection.Add(new System.Windows.Point(x2, y2));
                //        pointCollection.Add(new System.Windows.Point(x2 + Fillx, y2 + Fill));
                //        pointCollection.Add(new System.Windows.Point(x1, y2 + Fill));
                //        p.Points = pointCollection;
                //        canvasTwoD.Children.Add(p);
                //        percent = 0;
                //    }

                //    else if (percent > 40 && percent <= 70)
                //    {

                //        if (Tank_ID == 43)
                //        {
                //            x1 = 85499.9826;
                //            y1 = 35.895;
                //            x2 = 87288.4132;
                //            y2 = 537.4795;
                //            x3 = 88398.7617;
                //            y3 = 1299.9649;
                //            x4 = 89742.2384;
                //            y4 = 2808.8661;
                //            x5 = 90732.8238;
                //            y5 = 4000.0047;
                //            x6 = 85499.9826;
                //        }
                //        else if (Tank_ID == 44)
                //        {
                //            x1 = 87747.8509;
                //            y1 = 4000.037;
                //            x2 = 90732.8238;
                //            y2 = 4000.0047;
                //            x3 = 91474.9989;
                //            y3 = 4892.44;
                //            x4 = 92314.6868;
                //            y4 = 5864.5274;
                //            x5 = 92876.0048;
                //            y5 = 6500;
                //            x6 = 87747.8779;
                //            y6 = 6500;
                //        }

                //        else if (Tank_ID == 76)
                //        {
                //            x1 = 87747.8779;
                //            y1 = 6500;
                //            x2 = 92876.0048;
                //            y2 = 6500;
                //            x3 = 94123.9043;
                //            y3 = 7829.5821;
                //            x4 = 95249.6596;
                //            y4 = 8938.5823;
                //            x5 = 96326.8665;
                //            y5 = 9949.9084;
                //            x6 = 87747.8778;
                //            y6 = 9568.4435;

                //        }
                //        System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                //        p.Stroke = System.Windows.Media.Brushes.Black;
                //        SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                //        mySolidColorBrush.Color = color;
                //        p.Fill = mySolidColorBrush;

                //        double d = y4 - y3;
                //        double Fill = Convert.ToInt32(percent) * (d / 70);
                //        double dx = x4 - x3;
                //        double Fillx = Convert.ToInt32(percent) * (dx / 70);
                //        System.Windows.Point[] point = new System.Windows.Point[25];
                //        PointCollection pointCollection = new PointCollection();
                //        pointCollection.Add(new System.Windows.Point(x1, y1));
                //        pointCollection.Add(new System.Windows.Point(x2, y2));
                //        pointCollection.Add(new System.Windows.Point(x3, y3));
                //        pointCollection.Add(new System.Windows.Point(x3 + Fillx, y3 + Fill));
                //        pointCollection.Add(new System.Windows.Point(x1, y3 + Fill));
                //        p.Points = pointCollection;
                //        canvasTwoD.Children.Add(p);
                //        percent = 0;
                //    }


                //    else if (percent > 70 && percent <= 100)
                //    {
                //        if (Tank_ID == 43)
                //        {
                //            x1 = 85499.9826;
                //            y1 = 35.895;
                //            x2 = 87288.4132;
                //            y2 = 537.4795;
                //            x3 = 88398.7617;
                //            y3 = 1299.9649;
                //            x4 = 89742.2384;
                //            y4 = 2808.8661;
                //            x5 = 90732.8238;
                //            y5 = 4000.0047;
                //            x6 = 85499.9826;
                //        }
                //        else if (Tank_ID == 44)
                //        {
                //            x1 = 87747.8509;
                //            y1 = 4000.037;
                //            x2 = 90732.8238;
                //            y2 = 4000.0047;
                //            x3 = 91474.9989;
                //            y3 = 4892.44;
                //            x4 = 92314.6868;
                //            y4 = 5864.5274;
                //            x5 = 92876.0048;
                //            y5 = 6500;
                //            x6 = 87747.8779;
                //            y6 = 6500;
                //        }
                //        else if (Tank_ID == 76)
                //        {
                //            x1 = 87747.8779;
                //            y1 = 6500;
                //            x2 = 92876.0048;
                //            y2 = 6500;
                //            x3 = 94123.9043;
                //            y3 = 7829.5821;
                //            x4 = 95249.6596;
                //            y4 = 8938.5823;
                //            x5 = 96326.8665;
                //            y5 = 9949.9084;
                //            x6 = 87747.8778;
                //            y6 = 9568.4435;

                //        }

                //        System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                //        p.Stroke = System.Windows.Media.Brushes.Black;
                //        SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                //        mySolidColorBrush.Color = color;
                //        p.Fill = mySolidColorBrush;

                //        double d = y5 - y4;
                //        double Fill = Convert.ToInt32(percent) * (d / 100);
                //        double dx = x5 - x4;
                //        double Fillx = Convert.ToInt32(percent) * (dx / 100);
                //        System.Windows.Point[] point = new System.Windows.Point[25];
                //        PointCollection pointCollection = new PointCollection();
                //        pointCollection.Add(new System.Windows.Point(x1, y1));
                //        pointCollection.Add(new System.Windows.Point(x2, y2));
                //        pointCollection.Add(new System.Windows.Point(x3, y3));
                //        pointCollection.Add(new System.Windows.Point(x4, y4));
                //        pointCollection.Add(new System.Windows.Point(x4 + Fillx, y4 + Fill));
                //        pointCollection.Add(new System.Windows.Point(x1, y4 + Fill));
                //        p.Points = pointCollection;
                //        canvasTwoD.Children.Add(p);
                //        percent = 0;
                //    }
                //}
                //    #endregion


                else if (percent > 0 && percent <= 100)
                {
                    System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                    p.Stroke = System.Windows.Media.Brushes.Black;
                    SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                    mySolidColorBrush.Color = color;
                    p.Fill = mySolidColorBrush;
                    double d = yy[3] - yy[2];

                    double Fill = Convert.ToInt32(percent) * (d / 100);
                    System.Windows.Point[] point = new System.Windows.Point[25];
                    PointCollection pointCollection = new PointCollection();
                    pointCollection.Add(new System.Windows.Point(xx[1], yy[1]));
                    pointCollection.Add(new System.Windows.Point(xx[2], yy[2]));
                    pointCollection.Add(new System.Windows.Point(xx[2], yy[2] + Fill));
                    pointCollection.Add(new System.Windows.Point(xx[1], yy[2] + Fill));


                    p.Points = pointCollection;
                    canvasTwoD.Children.Add(p);

                    percent = 0;
                }
                else if (percent == 0)
                {
                    //System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                    //p.Stroke = System.Windows.Media.Brushes.Black;
                    //SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                    //mySolidColorBrush.Color = System.Windows.Media.Color.FromArgb(0, 255, 255, 255);
                    //p.Fill = mySolidColorBrush;
                    //System.Windows.Point[] point = new System.Windows.Point[25];
                    //PointCollection pointCollection = new PointCollection();
                    //pointCollection.Add(new System.Windows.Point(xx[0], yy[0]));
                    //pointCollection.Add(new System.Windows.Point(xx[1], yy[1]));
                    //pointCollection.Add(new System.Windows.Point(xx[2], yy[2]));
                    //pointCollection.Add(new System.Windows.Point(xx[3], yy[3]));
                    //p.Points = pointCollection;
                    //canvasTwoD.Children.Add(p);
                }
            }
            catch //(Exception ex)
            {
               // MessageBox.Show(ex.Message);
            }
        }

        private void ResetOutputPanelValues()
        {
            lblGMT.Content = "0.000";
            lblDisplacement.Content = "0";
            lblTrim.Content = "0.000";
            lblHeel.Content = "0.000";
            label5.Content = string.Empty;
            label8.Content = string.Empty;
            lblDraftAP.Content = "0.000";
            lblDraftFP.Content = "0.000";
            lblDraftMean.Content = "0.000";
            lblDraftAftMark.Content = "0.000";
            lblDraftFwdMark.Content = "0.000";
            lblPROPELLER.Content = "0.000";
            lblSONARDOME.Content = "0.000";
            lblKG.Content = "0.000";
            lblKGF.Content = "0.000";
            lblLCG.Content = "0.000";
            lblFSC.Content = "0.000";
            lblTPC.Content = "0.000";
            lblMTC.Content = "0.000";
        }

        private void btnclear_Click(object sender, RoutedEventArgs e)
        {
            var selectedIndex = tabControlTankTypesReal != null ? tabControlTankTypesReal.SelectedIndex : -1;
            string tableName = GetRealTankTableName(selectedIndex);
            if (string.IsNullOrWhiteSpace(tableName))
            {
                ModernMessageBox.Show("Please select a tank table first.", "Clear", MessageBoxType.Warning);
                return;
            }

            var confirm = ModernMessageBox.Show(
                "Do you want to clear the " + tableName + " table values?",
                "Confirm Clear",
                MessageBoxType.Warning,
                MessageBoxButton.YesNo);

            if (confirm != MessageBoxResult.Yes)
            {
                return;
            }

            bool isCleared = ClearSelectedRealTankTable(selectedIndex);
            if (!isCleared)
            {
                ModernMessageBox.Show("Unable to clear selected table.", "Clear", MessageBoxType.Error);
                return;
            }

            ModernMessageBox.Show(tableName + " data cleared successfully.", "Clear Tanks", MessageBoxType.Success);
        }

        private void btnclearOutputPanel_Click(object sender, RoutedEventArgs e)
        {
            var confirm = ModernMessageBox.Show(
                "Do you want to clear output panel values?",
                "Confirm Clear",
                MessageBoxType.Warning,
                MessageBoxButton.YesNo);

            if (confirm != MessageBoxResult.Yes)
            {
                return;
            }

            ResetOutputPanelValues();
            ModernMessageBox.Show("Output panel values cleared.", "Clear", MessageBoxType.Success);
        }

        private void btnImportRealModeData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                Models.TableModel.RealModeData();
                Models.TableModel.RealModePercentFill();

                if (Models.BO.clsGlobVar.dtRealCargoTanks != null) dgCargoTanks.ItemsSource = Models.BO.clsGlobVar.dtRealCargoTanks.DefaultView;
                if (Models.BO.clsGlobVar.dtRealBallastTanks != null) dgBallastTanks.ItemsSource = Models.BO.clsGlobVar.dtRealBallastTanks.DefaultView;
                if (Models.BO.clsGlobVar.dtRealFuelOilTanks != null) dgFuelOilTanks.ItemsSource = Models.BO.clsGlobVar.dtRealFuelOilTanks.DefaultView;
                if (Models.BO.clsGlobVar.dtRealFreshWaterTanks != null) dgFreshWaterTanks.ItemsSource = Models.BO.clsGlobVar.dtRealFreshWaterTanks.DefaultView;
                if (Models.BO.clsGlobVar.dtRealMiscTanks != null) dgMiscTanks.ItemsSource = Models.BO.clsGlobVar.dtRealMiscTanks.DefaultView;
                if (Models.BO.clsGlobVar.dtRealCompartments != null) dgCompartments.ItemsSource = Models.BO.clsGlobVar.dtRealCompartments.DefaultView;
                if (Models.BO.clsGlobVar.dtRealWaterTightRegion != null) dgWaterTightRegion.ItemsSource = Models.BO.clsGlobVar.dtRealWaterTightRegion.DefaultView;
                if (Models.BO.clsGlobVar.dtRealVariableItems != null) dgVariableItems.ItemsSource = Models.BO.clsGlobVar.dtRealVariableItems.DefaultView;

                ModernMessageBox.Show("Real mode data imported/refreshed successfully.", "Import", MessageBoxType.Success);
            }
            catch (Exception ex)
            {
                ModernMessageBox.Show("Import failed: " + ex.Message, "Import", MessageBoxType.Error);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void btnSaveLoadingCondition_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                // Real mode values are persisted during grid edits; this action confirms and refreshes.
                Models.TableModel.RealModeData();
                Models.TableModel.RealModePercentFill();
                ModernMessageBox.Show("Real mode values saved/refreshed successfully.", "Save", MessageBoxType.Success);
            }
            catch (Exception ex)
            {
                ModernMessageBox.Show("Save failed: " + ex.Message, "Save", MessageBoxType.Error);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private static string GetRealTankTableName(int selectedIndex)
        {
            switch (selectedIndex)
            {
                case 0: return "Cargo Tanks";
                case 1: return "Ballast Tanks";
                case 2: return "Fuel Oil Tanks";
                case 3: return "Fresh Water Tanks";
                case 4: return "Miscellaneous Tanks";
                case 5: return "Non Tight";
                case 6: return "Water Tight";
                case 7: return "DWT Constant";
                default: return string.Empty;
            }
        }

        private bool ClearSelectedRealTankTable(int selectedIndex)
        {
            try
            {
                switch (selectedIndex)
                {
                    case 0:
                        return ClearRealTankRows(Models.BO.clsGlobVar.dtRealCargoTanks, dgCargoTanks);
                    case 1:
                        return ClearRealTankRows(Models.BO.clsGlobVar.dtRealBallastTanks, dgBallastTanks);
                    case 2:
                        return ClearRealTankRows(Models.BO.clsGlobVar.dtRealFuelOilTanks, dgFuelOilTanks);
                    case 3:
                        return ClearRealTankRows(Models.BO.clsGlobVar.dtRealFreshWaterTanks, dgFreshWaterTanks);
                    case 4:
                        return ClearRealTankRows(Models.BO.clsGlobVar.dtRealMiscTanks, dgMiscTanks);
                    case 5:
                        return ClearRealTankRows(Models.BO.clsGlobVar.dtRealCompartments, dgCompartments);
                    case 6:
                        return ClearRealTankRows(Models.BO.clsGlobVar.dtRealWaterTightRegion, dgWaterTightRegion);
                    case 7:
                        return ClearRealDwtConstantRows();
                    default:
                        return false;
                }
            }
            catch
            {
                return false;
            }
        }

        private bool ClearRealTankRows(DataTable tankTable, DataGrid targetGrid)
        {
            if (tankTable == null)
            {
                return false;
            }

            foreach (DataRow row in tankTable.Rows)
            {
                if (tankTable.Columns.Contains("Volume")) row["Volume"] = 0m;
                if (tankTable.Columns.Contains("Percent_Full")) row["Percent_Full"] = 0m;
                if (tankTable.Columns.Contains("Sounding_Level")) row["Sounding_Level"] = 0m;
                if (tankTable.Columns.Contains("Weight")) row["Weight"] = 0m;
                if (tankTable.Columns.Contains("Status")) row["Status"] = 0;
                if (tankTable.Columns.Contains("IsDamaged")) row["IsDamaged"] = false;
            }

            if (targetGrid != null)
            {
                targetGrid.ItemsSource = null;
                targetGrid.ItemsSource = tankTable.DefaultView;
            }

            string err = string.Empty;
            DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
            foreach (DataRow row in tankTable.Rows)
            {
                if (!tankTable.Columns.Contains("Tank_ID"))
                {
                    continue;
                }

                int tankId;
                if (!int.TryParse(Convert.ToString(row["Tank_ID"]), out tankId))
                {
                    continue;
                }

                command.CommandText =
                    "update tblTank_Status set Volume=0, IsDamaged=0 where Tank_ID=" + tankId + ";" +
                    "update tblLoading_Condition set Volume=0, Percent_Full=0, Weight=0, Status=0, IsDamaged=0 where Tank_ID=" + tankId + ";";
                command.CommandType = CommandType.Text;
                Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, err);
            }

            return true;
        }

        private bool ClearRealDwtConstantRows()
        {
            if (Models.BO.clsGlobVar.dtRealVariableItems == null)
            {
                return false;
            }

            foreach (DataRow row in Models.BO.clsGlobVar.dtRealVariableItems.Rows)
            {
                if (Models.BO.clsGlobVar.dtRealVariableItems.Columns.Contains("Weight")) row["Weight"] = 0m;
                if (Models.BO.clsGlobVar.dtRealVariableItems.Columns.Contains("LCG")) row["LCG"] = 0m;
                if (Models.BO.clsGlobVar.dtRealVariableItems.Columns.Contains("VCG")) row["VCG"] = 0m;
                if (Models.BO.clsGlobVar.dtRealVariableItems.Columns.Contains("TCG")) row["TCG"] = 0m;
                if (Models.BO.clsGlobVar.dtRealVariableItems.Columns.Contains("Length")) row["Length"] = 0m;
                if (Models.BO.clsGlobVar.dtRealVariableItems.Columns.Contains("Breadth")) row["Breadth"] = 0m;
                if (Models.BO.clsGlobVar.dtRealVariableItems.Columns.Contains("Depth")) row["Depth"] = 0m;
            }

            dgVariableItems.ItemsSource = null;
            dgVariableItems.ItemsSource = Models.BO.clsGlobVar.dtRealVariableItems.DefaultView;

            string err = string.Empty;
            DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
            command.CommandText = "update tblFixedLoad set Weight=0, LCG=0, TCG=0, VCG=0, Length=0, Breadth=0, Depth=0;";
            command.CommandType = CommandType.Text;
            Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, err);
            return true;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((ModernMessageBox.Show("Are you sure you want to add a new variable item?", "Please Confirm", MessageBoxType.Warning, MessageBoxButton.YesNo) == MessageBoxResult.Yes))
                {
                    DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
                    string Err = "";
                    string query = @"INSERT INTO tblFixedLoad ([Load_Name],[Weight],[LCG],[TCG],[VCG],[Length],[Breadth],[Depth])
                                                 VALUES ('New Variable Load',0,0,0,0,0,0,0)
                                 INSERT INTO tblFixedLoad_Simulation ([Load_Name],[Weight],[LCG],[TCG],[VCG],[Length],[Breadth],[Depth])
                                                              VALUES ('New Variable Load',0,0,0,0,0,0,0)";
                    command.CommandText = query;
                    command.CommandType = CommandType.Text;
                    Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);

                    Models.BO.clsGlobVar.dtRealVariableItems = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetVariableDetails");
                    Models.BO.clsGlobVar.dtSimulationVariableItems = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationModeVariableDetails");
                    dgVariableItems.ItemsSource = Models.BO.clsGlobVar.dtRealVariableItems.DefaultView;
                }

            }
            catch
            {
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Models.BO.clsGlobVar.dtRealVariableItems.Rows.Count > 1)
                {
                    if ((ModernMessageBox.Show("Are you sure you want to delete the selected variable item?", "Please Confirm", MessageBoxType.Warning, MessageBoxButton.YesNo) == MessageBoxResult.Yes))
                    {
                        DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
                        string Err = "";
                        int LoadId;
                        LoadId = Convert.ToInt16(((dgVariableItems).Items[indexvar] as DataRowView)["Load_Id"]);

                        string query = @"DELETE FROM tblFixedLoad
                                    WHERE Load_Id=" + LoadId;
                        command.CommandText = query;
                        command.CommandType = CommandType.Text;
                        Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);
                        Models.BO.clsGlobVar.dtRealVariableItems = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetVariableDetails");
                        dgVariableItems.ItemsSource = Models.BO.clsGlobVar.dtRealVariableItems.DefaultView;
                    }
                }
                else
                {
                    ModernMessageBox.Show("There should be at least one row in the table.", "Variable Items", MessageBoxType.Warning);
                }
            }
            catch
            {
            }
        }
        int index;
        private void dgVariableItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            indexvar = (sender as DataGrid).SelectedIndex;
        }

        private void dgTankCompartment_CurrentCellChanged(object sender, EventArgs e)
        {
            try
            {
                DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
                string Err = "";
                int TankId;
                bool IsSensorfaulty = false;
                decimal percentfill;
                string activeHeader = NormalizedGridHeader;
                DataGrid activeGrid = sender as DataGrid;
                if (activeGrid == null || index < 0 || index >= activeGrid.Items.Count || string.IsNullOrEmpty(activeHeader))
                {
                    return;
                }
                //if (clsGlobVar.FlagDamageCases == false)
                {

                    TankId = Convert.ToInt16((activeGrid.Items[index] as DataRowView)["Tank_ID"]);
                    IsSensorfaulty = Convert.ToBoolean((activeGrid.Items[index] as DataRowView)["IsSensorFaulty"]);
                    if (activeHeader == "VOLUME" || activeHeader == "PERCENT" || activeHeader == "SG")
                    {

                        decimal volume = 0, sg, weight = 0;
                        decimal minsounding = 0;
                        sg = Convert.ToDecimal(((sender as DataGrid).Items[index] as DataRowView)["SG"]);
                        percentfill = Convert.ToDecimal(((sender as DataGrid).Items[index] as DataRowView)["Percent_Full"]);

                        if (maxVolume == null || !maxVolume.ContainsKey(TankId))
                        {
                            return;
                        }

                        decimal maxsounding = maxVolume[TankId];

                        if (activeHeader == "VOLUME")
                        {
                            volume = Convert.ToDecimal(((sender as DataGrid).Items[index] as DataRowView)["Volume"]);
                            percentfill = Convert.ToDecimal((volume * 100) / maxsounding);
                        }
                        if (activeHeader == "PERCENT")
                        {
                            volume = (percentfill * maxsounding) / 100;

                        }
                        if (activeHeader == "SG")
                        {
                            volume = Convert.ToDecimal(((sender as DataGrid).Items[index] as DataRowView)["Volume"]);
                            weight = volume * sg;
                        }
                        weight = volume * sg;
                        ((sender as DataGrid).Items[index] as DataRowView)["Volume"] = Math.Round(volume, 3);
                        ((sender as DataGrid).Items[index] as DataRowView)["Weight"] = Math.Round(weight, 3);
                        ((sender as DataGrid).Items[index] as DataRowView)["Percent_Full"] = Math.Round(percentfill, 3);
                        decimal res1 = decimal.Compare(minsounding, volume);
                        decimal res2 = decimal.Compare(volume, maxsounding);
                        int result1 = (int)res1;
                        int result2 = (int)res2;
                        if (result1 > 0 || result2 > 0)
                        {

                            string error = "Volume should be between " + minsounding + " and " + maxsounding;
                            ModernMessageBox.Show(error, "Validation Error", MessageBoxType.Error);
                            // e.Cancel = true;
                            return;
                        }
                        else
                        {

                            string query = "update tblTank_Status set [Volume]=" + volume + ",SG=" + sg + ",Weight=" + weight + " where Tank_ID=" + TankId;
                            command = Models.DAL.clsDBUtilityMethods.GetCommand();
                            command.CommandText = query;
                            command.CommandType = CommandType.Text;
                            Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);
                            query = "update tblLoading_Condition set [Volume]=" + volume + ",SG=" + sg + ",Weight=" + weight + ",Percent_Full=" + percentfill + ",IsManualEntry='" + IsSensorfaulty + "' where Tank_ID=" + TankId;
                            command = Models.DAL.clsDBUtilityMethods.GetCommand();
                            command.CommandText = query;
                            command.CommandType = CommandType.Text;
                            Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);

                            Models.TableModel.RealModeData();
                            Models.TableModel.RealModePercentFill();
                            StartAutoRefresh();
                            Schedule3DRefresh();
                        }
                        index = -1;
                        TankId = 0;

                    }

                }
            }
            catch
            {
            }

        }
        private void dgTankCompartment_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            try
            {
                header = e.Column.Header == null ? string.Empty : e.Column.Header.ToString();
                index = e.Row.GetIndex();
                if (NormalizedGridHeader != "VOLUME")
                {
                    e.Cancel = true;
                    return;
                }

                if (e.Column.GetType().ToString() == "System.Windows.Controls.DataGridTextColumn")
                {

                    TextBlock cbo;
                    cbo = (System.Windows.Controls.TextBlock)e.Column.GetCellContent(e.Row);
                }
                if (e.Column.GetType().ToString() == "System.Windows.Controls.DataGridCheckBoxColumn")
                {
                    //index = e.Row.GetIndex();
                    CheckBox cbo;
                    cbo = (System.Windows.Controls.CheckBox)e.Column.GetCellContent(e.Row);
                    //cbo.SelectionChanged += new SelectionChangedEventHandler(FSMType_SelectionChanged);
                }
            }
            catch
            {

            }
        }

        private void dgTankCompartment_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1)
                 && !e.Text.Contains('.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if (e.Text.Contains('.')
                && tb.Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        private void dgTankCompartment_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            tb = e.Column.GetCellContent(e.Row) as TextBox;
        }

        private bool _3dInitialized = false;

        private void viewPort3d_Loaded(object sender, RoutedEventArgs e)
        {
            if (viewPort3d1.EffectsManager == null)
                viewPort3d1.EffectsManager = new DefaultEffectsManager();

            System.Diagnostics.Debug.WriteLine($"Viewport size: {viewPort3d1.ActualWidth} x {viewPort3d1.ActualHeight}");

            if (viewPort3d1.ActualWidth > 0 && viewPort3d1.ActualHeight > 0)
            {
                // Tab is already visible — load after short delay
                var initTimer = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromMilliseconds(300) };
                initTimer.Tick += (s, ev) => { initTimer.Stop(); Init3D(); };
                initTimer.Start();
            }
            else
            {
                // Tab not yet visible (0 size) — wait until it gets a real size
                viewPort3d1.SizeChanged += ViewPort3d1_SizeChanged;
            }
        }

        private void ViewPort3d1_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width > 0 && e.NewSize.Height > 0 && !_3dInitialized)
            {
                viewPort3d1.SizeChanged -= ViewPort3d1_SizeChanged;
                System.Diagnostics.Debug.WriteLine($"Viewport got real size: {e.NewSize.Width} x {e.NewSize.Height}");
                var initTimer = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromMilliseconds(300) };
                initTimer.Tick += (s, ev) => { initTimer.Stop(); Init3D(); };
                initTimer.Start();
            }
        }

        private void Init3D()
        {
            if (_3dInitialized) return;
            _3dInitialized = true;
            Show3DLoading();
            Refresh3dNew();
        }

        private void tabControl2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (tabControl2.SelectedItem == tabItem6)
                {
                    if (_pending3DRefresh || scene3D == null || scene3D.Children.Count == 0)
                    {
                        _pending3DRefresh = false;
                        Schedule3DRefresh(forceShowLoading: true);
                    }
                    else
                    {
                        var fitTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(180) };
                        fitTimer.Tick += (s, ev) =>
                        {
                            fitTimer.Stop();
                            Reset3DViewHard();
                        };
                        fitTimer.Start();
                    }
                }
            }
            catch { }
        }

        private void tabControl5_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                UpdateRenderTransform(canvas2DProfile);
                UpdateRenderTransform(canvas2DPlanA);
                UpdateRenderTransform(canvas2DPlanB);
            }
            catch { }
        }

        /// <summary>
        /// Fits the camera to show all loaded 3-D geometry, then locks the rotation/zoom
        /// pivot to the computed scene centre so the model never drifts off screen.
        /// </summary>
        private void FitCameraToScene()
        {
            if (viewPort3d1?.EffectsManager == null) return;

            // If the 3D tab is not yet visible the viewport has no size — defer.
            if (viewPort3d1.ActualWidth <= 0 || viewPort3d1.ActualHeight <= 0)
            {
                _pendingFit = true;
                viewPort3d1.SizeChanged -= ViewPort3d1_SizeChanged_Fit;
                viewPort3d1.SizeChanged += ViewPort3d1_SizeChanged_Fit;
                return;
            }

            // -- Step 1: compute scene bounding box centre --
            var bounds = ViewportExtensions.FindBounds3D(viewPort3d1);

            if (!bounds.IsEmpty && bounds.SizeX > 500) // avoid fitting to tiny partial geometry
            {
                var centre = new System.Windows.Media.Media3D.Point3D(
                    bounds.X + bounds.SizeX * 0.5,
                    bounds.Y + bounds.SizeY * 0.5,
                    bounds.Z + bounds.SizeZ * 0.5);
                
                // Set the rotation pivot first
                viewPort3d1.FixedRotationPoint = centre;
                viewPort3d1.FixedRotationPointEnabled = true;

                // Zoom to fit
                viewPort3d1.ZoomExtents(animationTime: 400);

                var cam = viewPort3d1.Camera as HelixToolkit.Wpf.SharpDX.PerspectiveCamera;
                if (cam != null)
                {
                    double maxSize = Math.Max(bounds.SizeX, Math.Max(bounds.SizeY, bounds.SizeZ));
                    cam.NearPlaneDistance = Math.Max(0.1, maxSize / 1000.0);
                    cam.FarPlaneDistance = Math.Max(10000.0, maxSize * 40.0);
                }
            }
            else
            {
                // Fallback: Use camera look direction to find a reasonable pivot
                var cam = viewPort3d1.Camera as HelixToolkit.Wpf.SharpDX.PerspectiveCamera;
                if (cam != null)
                {
                    viewPort3d1.ZoomExtents(animationTime: 0);
                    if (cam.LookDirection.Length > 0)
                    {
                        var centre = new System.Windows.Media.Media3D.Point3D(
                            cam.Position.X + cam.LookDirection.X,
                            cam.Position.Y + cam.LookDirection.Y,
                            cam.Position.Z + cam.LookDirection.Z);
                        viewPort3d1.FixedRotationPoint = centre;
                        viewPort3d1.FixedRotationPointEnabled = true;
                    }
                }
            }
        }

        private void ViewPort3d1_SizeChanged_Fit(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width > 0 && e.NewSize.Height > 0 && _pendingFit)
            {
                _pendingFit = false;
                viewPort3d1.SizeChanged -= ViewPort3d1_SizeChanged_Fit;
                var t = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };
                t.Tick += (s, ev) => { t.Stop(); FitCameraToScene(); };
                t.Start();
            }
        }
        private void TimerGraphicsRefresh_Tick(object sender, EventArgs e)
        {
            TimerGraphicsRefresh.Stop();
            Refresh3dNew();
        }

        private void dgVariableItems_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            try
            {
                header = e.Column.Header.ToString();
                index = e.Row.GetIndex();
                if (e.Column.GetType().ToString() == "System.Windows.Controls.DataGridTextColumn")
                {

                    TextBlock cbo;
                    cbo = (System.Windows.Controls.TextBlock)e.Column.GetCellContent(e.Row);
                }
                if (e.Column.GetType().ToString() == "System.Windows.Controls.DataGridCheckBoxColumn")
                {
                    //index = e.Row.GetIndex();
                    CheckBox cbo;
                    cbo = (System.Windows.Controls.CheckBox)e.Column.GetCellContent(e.Row);
                    //cbo.SelectionChanged += new SelectionChangedEventHandler(FSMType_SelectionChanged);
                }
            }
            catch
            {

            }
        }

        private void dgVariableItems_CurrentCellChanged(object sender, EventArgs e)
        {
            try
            {
                DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
                string Err = "", LoadName = "";
                int LoadId, fsmType;
                decimal weight, LCG, TCG, VCG, length, breadth, depth;
                //if (clsGlobVar.FlagDamageCases == false)
                {
                    LoadId = Convert.ToInt16(((sender as DataGrid).Items[index] as DataRowView)["Load_Id"]);
                    LoadName = Convert.ToString(((sender as DataGrid).Items[index] as DataRowView)["Load_Name"]);
                    weight = Convert.ToDecimal(((sender as DataGrid).Items[index] as DataRowView)["Weight"]);
                    LCG = Convert.ToDecimal(((sender as DataGrid).Items[index] as DataRowView)["LCG"]);
                    TCG = Convert.ToDecimal(((sender as DataGrid).Items[index] as DataRowView)["TCG"]);
                    VCG = Convert.ToDecimal(((sender as DataGrid).Items[index] as DataRowView)["VCG"]);
                    length = Convert.ToDecimal(((sender as DataGrid).Items[index] as DataRowView)["Length"]);
                    breadth = Convert.ToDecimal(((sender as DataGrid).Items[index] as DataRowView)["Breadth"]);
                    depth = Convert.ToDecimal(((sender as DataGrid).Items[index] as DataRowView)["Depth"]);


                    string query = "update tblFixedLoad set Load_Name='" + LoadName + "',Weight=" + weight + ",LCG=" + LCG + ",TCG=" + TCG + ",VCG=" + VCG + @"
                            ,Length=" + length + ",Breadth=" + breadth + ",Depth=" + depth + " where Load_Id=" + LoadId;
                    command = Models.DAL.clsDBUtilityMethods.GetCommand();
                    command.CommandText = query;
                    command.CommandType = CommandType.Text;
                    Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);
                    index = -1;
                    LoadId = 0;

                }
            }
            catch
            {
            }

        }

        ////private void Refresh3dNew()
        ////{
        ////    try
        ////    {
        ////        viewPort3d.Children.Clear();
        ////        DefaultLights light = new DefaultLights();
        ////        viewPort3d.Children.Add(light);
        ////        viewPort3d.RotateGesture = new MouseGesture(MouseAction.LeftClick);
        ////        for (int i = 0; i < 6; i++)
        ////        {
        ////            string stats = Convert.ToString((dgFreshWaterTanks.Items[i] as DataRowView)["IsDamaged"]);
        ////            bool isvisible = Convert.ToBoolean((dgFreshWaterTanks.Items[i] as DataRowView)["IsVisible"]);
        ////            string Nametxt = Convert.ToString((dgFreshWaterTanks.Items[i] as DataRowView)["Tank_Name"]);
        ////            string NameSplit = Nametxt.Split('.')[0];
        ////            string Name = NameSplit.Replace("/", "");

        ////            foreach (string file in Directory.EnumerateFiles(folderPath + "\\FreshWaterTank\\", "*.stl"))
        ////            {
        ////                ModelVisual3D device3D = new ModelVisual3D();
        ////                string str = file.Split('\\')[3];
        ////                string str1 = str.Split('.')[0];
        ////                string TankName = str1.Replace("/", "");
        ////                if (Name == TankName)
        ////                {
        ////                    if (isvisible)
        ////                    {
        ////                        if (stats == "True")
        ////                        {
        ////                            device3D.Content = Display3d(file, 255, 255, 0, 0);
        ////                        }
        ////                        else
        ////                        {
        ////                            device3D.Content = Display3d(file, 180, 0, 0, 100);
        ////                        }
        ////                        device3D.SetName(TankName); ;
        ////                        viewPort3d.Children.Add(device3D);
        ////                    }
        ////                }



        ////            }

        ////        }
        ////        for (int i = 0; i < 11; i++)
        ////        {
        ////            string stats = Convert.ToString((dgBallastTanks.Items[i] as DataRowView)["IsDamaged"]);
        ////            bool isvisible = Convert.ToBoolean((dgBallastTanks.Items[i] as DataRowView)["IsVisible"]);
        ////            string Nametxt = Convert.ToString((dgBallastTanks.Items[i] as DataRowView)["Tank_Name"]);
        ////            string Name = Nametxt.Replace("/", "");

        ////            foreach (string file in Directory.EnumerateFiles(folderPath + "\\BallastTank\\", "*.stl"))
        ////            {

        ////                ModelVisual3D device3D = new ModelVisual3D();
        ////                string str = file.Split('\\')[3];
        ////                string str1 = str.Split('.')[0];
        ////                string TankName = str1.Replace("/", "");
        ////                if (Name == TankName)
        ////                {
        ////                    if (isvisible)
        ////                    {
        ////                        if (stats == "True")
        ////                        {
        ////                            device3D.Content = Display3d(file, 255, 255, 0, 0);
        ////                        }
        ////                        else
        ////                        {
        ////                            device3D.Content = Display3d(file, 180, 0, 200, 0);
        ////                        }
        ////                        device3D.SetName(TankName);
        ////                        viewPort3d.Children.Add(device3D);
        ////                    }
        ////                }
        ////            }

        ////        }
        ////        for (int i = 0; i < 30; i++)
        ////        {
        ////            string stats = Convert.ToString((dgFuelOilTanks.Items[i] as DataRowView)["IsDamaged"]);
        ////            bool isvisible = Convert.ToBoolean((dgFuelOilTanks.Items[i] as DataRowView)["IsVisible"]);
        ////            string Nametxt = Convert.ToString((dgFuelOilTanks.Items[i] as DataRowView)["Tank_Name"]);
        ////            string splitName = Nametxt.Split('.')[0];
        ////            string Name = splitName.Replace("/", "");
        ////            foreach (string file in Directory.EnumerateFiles(folderPath + "\\FuelOilTank\\", "*.stl"))
        ////            {
        ////                ModelVisual3D device3D = new ModelVisual3D();
        ////                string str = file.Split('\\')[3];
        ////                string str1 = str.Split('.')[0];
        ////                string TankName = str1.Replace("/", "");
        ////                string[] results = file.Split(new string[] { "*.stl" }, StringSplitOptions.None);
        ////                if (Name == TankName)
        ////                {
        ////                    if (isvisible)
        ////                    {
        ////                        if (stats == "True")
        ////                        {
        ////                            device3D.Content = Display3d(file, 255, 255, 0, 0);
        ////                        }
        ////                        else
        ////                        {
        ////                            device3D.Content = Display3d(file, 200, 185, 92, 0);
        ////                        }
        ////                        device3D.SetName(TankName);

        ////                        viewPort3d.Children.Add(device3D);
        ////                    }
        ////                }
        ////            }
        ////        }
        ////        for (int i = 0; i < 8; i++)
        ////        {
        ////            string stats = Convert.ToString((dgMiscTanks.Items[i] as DataRowView)["IsDamaged"]);
        ////            bool isvisible = Convert.ToBoolean((dgMiscTanks.Items[i] as DataRowView)["IsVisible"]);
        ////            string Nametxt = Convert.ToString((dgMiscTanks.Items[i] as DataRowView)["Tank_Name"]);
        ////            string spliteName = Nametxt.Split('.')[0];
        ////            string Name = spliteName.Replace("/", "");
        ////            foreach (string file in Directory.EnumerateFiles(folderPath + "\\MiscTank\\", "*.stl"))
        ////            {

        ////                ModelVisual3D device3D = new ModelVisual3D();
        ////                string str = file.Split('\\')[3];
        ////                string str1 = str.Split('.')[0];
        ////                string TankName = str1.Replace("/", "");
        ////                if (Name == TankName)
        ////                {
        ////                    if (isvisible)
        ////                    {
        ////                        if (stats == "True")
        ////                        {
        ////                            device3D.Content = Display3d(file, 255, 255, 0, 0);
        ////                        }
        ////                        else
        ////                        {
        ////                            device3D.Content = Display3d(file, 180, 255, 128, 192);
        ////                        }
        ////                        device3D.SetName(TankName);
        ////                        viewPort3d.Children.Add(device3D);
        ////                    }
        ////                }
        ////            }
        ////        }
        ////        for (int i = 0; i < 16; i++)
        ////        {
        ////            try
        ////            {
        ////                string stats = Convert.ToString((dgCompartments.Items[i] as DataRowView)["IsDamaged"]);
        ////                bool isvisible = Convert.ToBoolean((dgCompartments.Items[i] as DataRowView)["IsVisible"]);
        ////                string Nametxt = Convert.ToString((dgCompartments.Items[i] as DataRowView)["Tank_Name"]);
        ////                string spliteName = Nametxt.Split('.')[0];
        ////                string Name = spliteName.Replace("/", "");
        ////                foreach (string file in Directory.EnumerateFiles(folderPath + "\\Compartment\\", "*.stl"))
        ////                {
        ////                    ModelVisual3D device3D = new ModelVisual3D();
        ////                    string str = file.Split('\\')[3];
        ////                    string str1 = str.Split('.')[0];
        ////                    string TankName = str1.Replace("/", "");
        ////                    string[] results = file.Split(new string[] { "*.stl" }, StringSplitOptions.None);
        ////                    if (Name == TankName)
        ////                    {
        ////                        if (isvisible)
        ////                        {
        ////                            if (stats == "True")
        ////                            {
        ////                                device3D.Content = Display3d(file, 255, 255, 0, 0);
        ////                            }
        ////                            else
        ////                            {
        ////                                device3D.Content = Display3d(file, 120, 239, 228, 176);
        ////                            }
        ////                            device3D.SetName(TankName);

        ////                            viewPort3d.Children.Add(device3D);
        ////                        }
        ////                    }
        ////                }
        ////            }
        ////            catch
        ////            {
        ////            }
        ////        }

        ////        for (int i = 0; i < 70; i++)
        ////        {
        ////            try
        ////            {
        ////                string stats = Convert.ToString((dgWaterTightRegion.Items[i] as DataRowView)["IsDamaged"]);
        ////                bool isvisible = Convert.ToBoolean((dgWaterTightRegion.Items[i] as DataRowView)["IsVisible"]);
        ////                string Nametxt = Convert.ToString((dgWaterTightRegion.Items[i] as DataRowView)["Tank_Name"]);
        ////                string spliteName = Nametxt.Split('.')[0];
        ////                string Name = spliteName.Replace("/", "");
        ////                foreach (string file in Directory.EnumerateFiles(folderPath + "\\WT_REGION\\", "*.stl"))
        ////                {
        ////                    ModelVisual3D device3D = new ModelVisual3D();
        ////                    string str = file.Split('\\')[3];
        ////                    string str1 = str.Split('.')[0];
        ////                    string TankName = str1.Replace("/", "");
        ////                    string[] results = file.Split(new string[] { "*.stl" }, StringSplitOptions.None);
        ////                    if (Name == TankName)
        ////                    {
        ////                        if (isvisible)
        ////                        {
        ////                            if (stats == "True")
        ////                            {
        ////                                device3D.Content = Display3d(file, 255, 255, 0, 0);
        ////                            }
        ////                            else
        ////                            {
        ////                                device3D.Content = Display3d(file, 100, 255, 255, 255);
        ////                            }
        ////                            device3D.SetName(TankName);

        ////                            viewPort3d.Children.Add(device3D);
        ////                        }
        ////                    }
        ////                }
        ////            }
        ////            catch
        ////            {
        ////            }
        ////        }


        ////        foreach (string file in Directory.EnumerateFiles(folderPath, "*.stl"))
        ////        {
        ////            string str = file.Split('\\')[1];
        ////            string str1 = str.Split('.')[0];
        ////            string TankName = str1.Replace("/", "");
        ////            if (Name == str1)
        ////            {
        ////                ModelVisual3D device3D = new ModelVisual3D();
        ////                device3D.Content = Display3d(file, 70, 150, 150, 150);
        ////                device3D.SetName(TankName);
        ////                viewPort3d.Children.Add(device3D);
        ////            }
        ////        }
        ////    }
        ////    catch
        ////    {
        ////    }
        ////}
        private void Getpercentage()
        {


            try
            {


              
                string cs = clsSqlData.GetConnectionString();



                SqlConnection cn = new SqlConnection(cs);
                try
                {
                    if (cn.State != ConnectionState.Open)
                    {
                        cn.Close();
                        cn.Open();
                    }
                }


                catch //(System.Exception)
                {
                    //System.Windows.MessageBox.Show("Problem in opening connection");

                }
                DataSet dspercent = new DataSet();
                //                string query = "SELECT [Percent_Full] FROM [tblSimulationMode_Loading_Condition] WHERE Tank_ID= (SELECT Tank_ID FROM tblMaster_Tank WHERE Tank_Name='" + TankNameForPercentage + "')";
                string query = "SELECT [Percent_Full] FROM [tblLoading_Condition] WHERE Tank_ID= (SELECT Tank_ID FROM tblMaster_Tank WHERE Tank_Name like'%" + TankNameForPercentage + "%')";
                SqlCommand cmd = new SqlCommand(query, cn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dspercent);
                cn.Close();

                PercentageFill = Math.Round(Convert.ToDecimal(dspercent.Tables[0].Rows[0]["Percent_Full"].ToString()), 3);



            }

            catch //(System.Exception)
            {


            }
        }




        private void Refresh3dNew()
        {
            try
            {
                if (scene3D == null || viewPort3d1.EffectsManager == null) { Hide3DLoading(); return; }

                scene3D.Children.Clear();

                // Hard-force same resolved path behavior as Virtual: <running-exe-folder>\3D
                string asmDir = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
                string base3D = System.IO.Path.Combine(asmDir, "3D");
                if (!Directory.Exists(base3D))
                {
                    // Last fallback keeps old behavior if deployment layout differs.
                    base3D = folderPath;
                }
                if (!_logged3DBasePath)
                {
                    System.Diagnostics.Debug.WriteLine($"[Sensor3D] Forced base3D='{base3D}'");
                    _logged3DBasePath = true;
                }

                // Optimized dictionary-based lookup for all categories
                var freshWaterFiles   = BuildTankFileDictionary(base3D + @"\FreshWaterTank\");
                var freshWater25Files = BuildTankFileDictionary(base3D + @"\FreshWaterTank25\");
                var freshWater50Files = BuildTankFileDictionary(base3D + @"\FreshWaterTank50\");
                var freshWater75Files = BuildTankFileDictionary(base3D + @"\FreshWaterTank75\");

                var ballastFiles   = BuildTankFileDictionary(base3D + @"\BallastTank\");
                var ballast25Files = BuildTankFileDictionary(base3D + @"\BallastTank25\");
                var ballast50Files = BuildTankFileDictionary(base3D + @"\BallastTank50\");
                var ballast75Files = BuildTankFileDictionary(base3D + @"\BallastTank75\");

                var fuelOilFiles   = BuildTankFileDictionary(base3D + @"\FuelOilTank\");
                var fuelOil25Files = BuildTankFileDictionary(base3D + @"\FuelOilTank25\");
                var fuelOil50Files = BuildTankFileDictionary(base3D + @"\FuelOilTank50\");
                var fuelOil75Files = BuildTankFileDictionary(base3D + @"\FuelOilTank75\");

                var cargoFiles   = BuildTankFileDictionary(base3D + @"\CargoTank\");
                var cargo25Files = BuildTankFileDictionary(base3D + @"\CargoTank25\");
                var cargo50Files = BuildTankFileDictionary(base3D + @"\CargoTank50\");
                var cargo75Files = BuildTankFileDictionary(base3D + @"\CargoTank75\");

                var dieselFiles   = BuildTankFileDictionary(base3D + @"\DieselOilTank\");
                var diesel25Files = BuildTankFileDictionary(base3D + @"\DieselOilTank25\");
                var diesel50Files = BuildTankFileDictionary(base3D + @"\DieselOilTank50\");
                var diesel75Files = BuildTankFileDictionary(base3D + @"\DieselOilTank75\");

                var miscFiles   = BuildTankFileDictionary(base3D + @"\MiscTank\");
                var misc25Files = BuildTankFileDictionary(base3D + @"\MiscTank25\");
                var misc50Files = BuildTankFileDictionary(base3D + @"\MiscTank50\");
                var misc75Files = BuildTankFileDictionary(base3D + @"\MiscTank75\");


                Dictionary<string, (decimal fill, bool damaged, int status)> BuildStateMap(DataTable dt)
                {
                    var map = new Dictionary<string, (decimal fill, bool damaged, int status)>(StringComparer.OrdinalIgnoreCase);
                    if (dt == null) return map;
                    DataTable snap = null;
                    try
                    {
                        snap = dt.Copy();
                    }
                    catch
                    {
                        // Table may be mutating on another thread; skip this cycle safely.
                        return map;
                    }
                    foreach (DataRow row in snap.Rows)
                    {
                        try
                        {
                            string name = Convert.ToString(row["Tank_Name"]).Split('.')[0].Trim().Replace("/", "");
                            bool isDamaged = Convert.ToString(row["IsDamaged"]) == "True";
                            int tankId = row.Table.Columns.Contains("Tank_ID") ? Convert.ToInt32(row["Tank_ID"]) : -1;
                            decimal percentFill = 0m;
                            if (tankId >= 0 && tankId < Models.BO.clsGlobVar.Tank_PercentFill.Length)
                                percentFill = Models.BO.clsGlobVar.Tank_PercentFill[tankId];
                            else if (row.Table.Columns.Contains("Percent_Full"))
                                percentFill = Convert.ToDecimal(row["Percent_Full"]);
                            int statusCode = 0;
                            if (row.Table.Columns.Contains("Status"))
                            {
                                try { statusCode = Convert.ToInt32(row["Status"]); } catch { statusCode = 0; }
                            }
                            map[name] = (percentFill, isDamaged, statusCode);
                        }
                        catch { }
                    }
                    return map;
                }

                // Always render all tank geometries from folder; data only controls fill/state colors.
                System.Action<DataTable, Dictionary<string, string>, Dictionary<string, string>, Dictionary<string, string>, Dictionary<string, string>, byte, byte, byte, byte> loadTanks =
                    (dt, baseDict, p25Dict, p50Dict, p75Dict, a, r, g, b) =>
                    {
                        if (baseDict == null || baseDict.Count == 0) return;
                        var states = BuildStateMap(dt);
                        foreach (var kv in baseDict)
                        {
                            try
                            {
                                string name = kv.Key;
                                string baseFile = kv.Value;
                                decimal percentFill = 0m;
                                bool isDamaged = false;
                                int statusCode = 0;
                                if (states.TryGetValue(name, out var st))
                                {
                                    percentFill = st.fill;
                                    isDamaged = st.damaged;
                                    statusCode = st.status;
                                }

                                Element3D device3D = ResolveTankModel(name, baseFile, isDamaged, percentFill, statusCode, p25Dict, p50Dict, p75Dict, a, r, g, b);
                                if (device3D != null)
                                {
                                    device3D.Tag = name;
                                    scene3D.Dispatcher.Invoke(() => { if (!scene3D.Children.Contains(device3D)) scene3D.Children.Add(device3D); });
                                }
                            }
                            catch { }
                        }
                    };

                DataTable ResolveTable(DataTable preferred, DataGrid grid)
                {
                    if (preferred != null && preferred.Rows.Count > 0) return preferred;
                    var dv = grid?.ItemsSource as DataView;
                    if (dv != null && dv.Table != null && dv.Table.Rows.Count > 0) return dv.Table;
                    return preferred;
                }

                var realFresh = ResolveTable(Models.BO.clsGlobVar.dtRealFreshWaterTanks, dgFreshWaterTanks);
                var realBallast = ResolveTable(Models.BO.clsGlobVar.dtRealBallastTanks, dgBallastTanks);
                var realFuel = ResolveTable(Models.BO.clsGlobVar.dtRealFuelOilTanks, dgFuelOilTanks);
                var realCargo = ResolveTable(Models.BO.clsGlobVar.dtRealCargoTanks, dgCargoTanks);
                var realMisc = ResolveTable(Models.BO.clsGlobVar.dtRealMiscTanks, dgMiscTanks);

                // Load all categories using resolved tables (global table first, grid fallback second)
                loadTanks(realFresh, freshWaterFiles, freshWater25Files, freshWater50Files, freshWater75Files, 180, 0, 0, 100);
                loadTanks(realBallast, ballastFiles, ballast25Files, ballast50Files, ballast75Files, 180, 0, 200, 0);
                loadTanks(realFuel, fuelOilFiles, fuelOil25Files, fuelOil50Files, fuelOil75Files, 200, 185, 92, 0);
                loadTanks(realFuel, dieselFiles, diesel25Files, diesel50Files, diesel75Files, 200, 185, 92, 0);
                loadTanks(realCargo, cargoFiles, cargo25Files, cargo50Files, cargo75Files, 200, 185, 92, 0);
                loadTanks(realMisc, miscFiles, misc25Files, misc50Files, misc75Files, 180, 255, 128, 192);

                // Ship hull and structural models from root folder
                int rootHullCount = 0;
                if (Directory.Exists(base3D))
                {
                    foreach (string file in Directory.EnumerateFiles(base3D, "*.stl"))
                    {
                        string TankName = System.IO.Path.GetFileNameWithoutExtension(file).Replace("/", "");
                        Element3D device3D = Display3d1(file, 70, 150, 150, 150);
                        if (device3D != null) { 
                            device3D.Tag = TankName; 
                            scene3D.Dispatcher.Invoke(() => scene3D.Children.Add(device3D)); 
                            rootHullCount++;
                        }
                    }
                }

                // Virtual-style simple fallback: only when scene is effectively empty.
                if (rootHullCount == 0 && scene3D.Children.Count <= 1 && ballastFiles.TryGetValue("FPT", out string fptFile))
                {
                     Element3D device3D = Display3d1(fptFile, 70, 150, 150, 150);
                     if (device3D != null) { device3D.Tag = "Hull_FPT"; scene3D.Dispatcher.Invoke(() => scene3D.Children.Add(device3D)); }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Refresh3dNew Error: {ex.Message}");
                _is3DRefreshInProgress = false;
                Hide3DLoading();
                return;
            }

            var zoomTimer = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromMilliseconds(520) };
            zoomTimer.Tick += (s, e) =>
            {
                zoomTimer.Stop();
                if (!_hasRendered3DOnce)
                {
                    Reset3DViewHard();
                }
                _hasRendered3DOnce = true;
                _is3DRefreshInProgress = false;
                Hide3DLoading();
            };
            zoomTimer.Start();
        }

        //private Model3D Display3d(string model, byte A, byte R, byte G, byte B)
        //{
        //    Model3D device = null;
        //    Material material = MaterialHelper.CreateMaterial(new SolidColorBrush(System.Windows.Media.Color.FromArgb(A, R, G, B)));
        //    try
        //    {
        //        //Adding a gesture here
        //        //viewPort3d.RotateGesture = new MouseGesture(MouseAction.LeftClick);
        //        //Import 3D model file
        //        ModelImporter import = new ModelImporter();
        //        import.DefaultMaterial = material;

        //        //Load the 3D model file
        //        device = import.Load(model);


        //    }
        //    catch (Exception e)
        //    {
        //        // Handle exception in case can not file 3D model
        //        System.Windows.MessageBox.Show(e.ToString());
        //        MessageBox.Show("Exception Error : " + e.StackTrace);
        //    }
        //    return device;
        //}
        //private Model3D Display3d1(string model, byte A, byte R, byte G, byte B)
        //{
        //    Model3D device1 = null;
        //    Material material = MaterialHelper.CreateMaterial(new SolidColorBrush(System.Windows.Media.Color.FromArgb(A, R, G, B)));
        //    try
        //    {
        //        //Adding a gesture here
        //        //viewPort3d.RotateGesture = new MouseGesture(MouseAction.LeftClick);
        //        //Import 3D model file
        //        ModelImporter import = new ModelImporter();
        //        import.DefaultMaterial = material;

        //        //Load the 3D model file
        //        device1 = import.Load(model);


        //    }
        //    catch //(Exception e)
        //    {
        //        // Handle exception in case can not file 3D model
        //       // System.Windows.MessageBox.Show(e.ToString());
        //      //  MessageBox.Show("Exception Error : " + e.StackTrace);
        //    }
        //    return device1;
        //}
        private Element3D Display3d1(string modelPath, byte A, byte R, byte G, byte B)
        {
            try
            {
                if (!_geometryCache.TryGetValue(modelPath, out HelixToolkit.SharpDX.MeshGeometry3D geometry))
                {
                    var reader = new HelixToolkit.SharpDX.StLReader();
                    using (var stream = new System.IO.FileStream(modelPath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                        reader.Read(stream, default(HelixToolkit.SharpDX.ModelInfo));
                    if (reader.Meshes == null || reader.Meshes.Count == 0) return null;
                    geometry = reader.Meshes[0].ToMeshGeometry3D();
                    _geometryCache[modelPath] = geometry;
                }
                var model = new MeshGeometryModel3D
                {
                    Geometry = geometry,
                    Material = new PhongMaterial
                    {
                        DiffuseColor = new HelixToolkit.Maths.Color4(R / 255f, G / 255f, B / 255f, A / 255f)
                    }
                };
                return model;
            }
            catch { return null; }
        }

        private System.Collections.Generic.Dictionary<string, string> BuildTankFileDictionary(string folder)
        {
            var dict = new System.Collections.Generic.Dictionary<string, string>();
            if (!System.IO.Directory.Exists(folder)) return dict;
            foreach (string file in System.IO.Directory.EnumerateFiles(folder, "*.stl"))
                dict[System.IO.Path.GetFileNameWithoutExtension(file).Replace("/", "")] = file;
            return dict;
        }

        private Element3D ResolveTankModel(string name, string baseFile, bool isDamaged, decimal fill, int statusCode,
            Dictionary<string, string> dict25, Dictionary<string, string> dict50, Dictionary<string, string> dict75,
            byte a, byte r, byte g, byte b)
        {
            string modelPath = baseFile;
            if (fill > 0 && fill <= 25) { if (dict25.TryGetValue(name, out string f)) modelPath = f; }
            else if (fill > 25 && fill <= 50) { if (dict50.TryGetValue(name, out string f)) modelPath = f; }
            else if (fill > 50 && fill <= 75) { if (dict75.TryGetValue(name, out string f)) modelPath = f; }
            else if (fill > 75) { if (dict75.TryGetValue(name, out string f)) modelPath = f; }

            if (!_geometryCache.TryGetValue(modelPath, out HelixToolkit.SharpDX.MeshGeometry3D geometry))
            {
                var reader = new HelixToolkit.SharpDX.StLReader();
                var objList = reader.Read(modelPath);
                if (objList != null && objList.Count > 0)
                {
                    geometry = objList[0].Geometry as HelixToolkit.SharpDX.MeshGeometry3D;
                    _geometryCache[modelPath] = geometry;
                }
            }

            if (statusCode == 1) return Display3d1(baseFile, 255, 255, 0, 0);      // Damage
            if (statusCode == 2) return Display3d1(baseFile, 255, 0, 120, 255);    // Flood
            if (isDamaged) return Display3d1(baseFile, 255, 255, 0, 0);
            return Display3d1(modelPath, a, r, g, b);
        }

        private string TankName;

        private string Volume;

        private string PercentFill;

        private string TankStatus;

        public string Tankname
        {
            get
            {
                return this.TankName;
            }
            set
            {
                this.TankName = value;
                this.OnPropertyChanged("Tankname");
            }
        }

        public string volume
        {
            get
            {
                return this.Volume;
            }
            set
            {
                this.Volume = value;
                this.OnPropertyChanged("volume");
            }
        }

        public string percent
        {
            get
            {
                return this.PercentFill;
            }
            set
            {
                this.PercentFill = value;
                this.OnPropertyChanged("percent");
            }
        }

        public string status
        {
            get
            {
                return this.TankStatus;
            }
            set
            {
                this.TankStatus = value;
                this.OnPropertyChanged("status");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        ////private void chkVisible_Click(object sender, RoutedEventArgs e)
        ////{



        ////    object a = e.Source;
        ////    CheckBox chk = (CheckBox)sender;

        ////    DataGridRow row = FindAncestor<DataGridRow>(chk);
        ////    if (row != null)
        ////    {
        ////        DataRowView rv = (DataRowView)row.Item;
        ////        byte A = 0, R = 0, G = 0, B = 0;

        ////        string Err = "";
        ////        string query = "update [tblLoading_Condition] set [IsVisible]='" + (bool)chk.IsChecked + "' where Tank_ID=" + rv["Tank_ID"];
        ////        DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
        ////        command.CommandText = query;
        ////        command.CommandType = CommandType.Text;
        ////        Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);
        ////        string subfolder = "";
        ////        if (Convert.ToInt32(rv["Tank_ID"]) <= 11)
        ////        {
        ////            subfolder = "BallastTank";
        ////            A = 180;
        ////            R = 0;
        ////            G = 200;
        ////            B = 0;

        ////        }
        ////        else if (Convert.ToInt32(rv["Tank_ID"]) > 11 && (Convert.ToInt32(rv["Tank_ID"]) <= 16))
        ////        {
        ////            subfolder = "FreshWaterTank";
        ////            A = 180;
        ////            R = 0;
        ////            G = 0;
        ////            B = 100;
        ////        }
        ////        else if (Convert.ToInt32(rv["Tank_ID"]) > 16 && (Convert.ToInt32(rv["Tank_ID"]) <= 46))
        ////        {
        ////            subfolder = "FuelOilTank";
        ////            A = 200;
        ////            R = 185;
        ////            G = 92;
        ////            B = 0;
        ////        }
        ////        else if (Convert.ToInt32(rv["Tank_ID"]) > 46 && (Convert.ToInt32(rv["Tank_ID"]) <= 54))
        ////        {
        ////            subfolder = "MiscTank";
        ////            A = 180;
        ////            R = 255;
        ////            G = 128;
        ////            B = 192;
        ////        }
        ////        else if (Convert.ToInt32(rv["Tank_ID"]) > 54 && (Convert.ToInt32(rv["Tank_ID"]) <= 434))
        ////        {
        ////            subfolder = "Compartment";
        ////            A = 120;
        ////            R = 239;
        ////            G = 228;
        ////            B = 176;
        ////        }

        ////        else if (Convert.ToInt32(rv["Tank_ID"]) > 434 && (Convert.ToInt32(rv["Tank_ID"]) <= 504))
        ////        {
        ////            subfolder = "WT_REGION";
        ////            A = 120;
        ////            R = 239;
        ////            G = 228;
        ////            B = 176;
        ////        }
        ////        string TankName = Convert.ToString(rv["Tank_Name"]);
        ////        string FUllTankName = TankName.Split('.')[0];
        ////        string str1 = FUllTankName.Replace("/", "");
        ////        if ((bool)chk.IsChecked == false)
        ////        {
        ////            for (int j = 0; j <= 152; j++)
        ////            {
        ////                try
        ////                {
        ////                    Visual3D model = viewPort3d1.Children[j + 1] as Visual3D;
        ////                    int hh = viewPort3d1.Children.Count;
        ////                    string modelname = model.GetName();
        ////                    Visual3D wd = viewPort3d1.Children.LastOrDefault();
        ////                    string gg = wd.GetName();

        ////                    if (modelname == str1)
        ////                    {
        ////                        viewPort3d1.Children.Remove(model);
        ////                    }

        ////                    if (modelname == gg)
        ////                        break;

        ////                }
        ////                catch (Exception ex)
        ////                {
        ////                    MessageBox.Show((ex.Message.ToString()));
        ////                }
        ////            }
        ////        }
        ////        else
        ////        {

        ////            try
        ////            {
        ////                viewPort3d1.Children.RemoveAt(viewPort3d1.Children.Count - 1);
        ////                //viewPort3d.Children.RemoveAt(viewPort3d.Children.Count - 1);
        ////                ModelVisual3D device3D = new ModelVisual3D();
        ////                string file = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + folderPath + "\\" + subfolder + "\\" + TankName + ".stl";
        ////                device3D.Content = Display3d1(file, A, R, G, B);
        ////                device3D.SetName(TankName);
        ////                viewPort3d1.Children.Add(device3D);

        ////                foreach (string file1 in Directory.EnumerateFiles(folderPath, "*.stl"))
        ////                {
        ////                    string str = file1.Split('\\')[1];
        ////                    str1 = str.Split('.')[0];
        ////                    TankName = str1.Replace("/", "");

        ////                    {
        ////                        device3D = new ModelVisual3D();
        ////                        device3D.Content = Display3d1(file1, 70, 150, 150, 150);
        ////                        device3D.SetName(TankName);
        ////                        viewPort3d1.Children.Add(device3D);
        ////                    }
        ////                }
        ////            }
        ////            catch (Exception ex)
        ////            {
        ////                MessageBox.Show((ex.Message.ToString()));
        ////            }
        ////        }

        ////    }
        ////}
        /// <summary>
        /// Returns the first ancester of specified type
        /// </summary>
        public static T FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            current = VisualTreeHelper.GetParent(current);
            while (current != null)
            {
                if (current is T)
                {
                    return (T)current;
                }

                current = VisualTreeHelper.GetParent(current);
            };
            return null;
        }

        private void btn3DZoomExtents_Click(object sender, RoutedEventArgs e)
        {
            Reset3DViewHard();
        }

        private bool ApplyPreferred3DView()
        {
            try
            {
                var cam = viewPort3d1?.Camera as HelixToolkit.Wpf.SharpDX.PerspectiveCamera;
                if (cam == null) return false;

                var bounds = ViewportExtensions.FindBounds3D(viewPort3d1);
                if (bounds.IsEmpty) return false;

                var centre = new System.Windows.Media.Media3D.Point3D(
                    bounds.X + bounds.SizeX * 0.5,
                    bounds.Y + bounds.SizeY * 0.5,
                    bounds.Z + bounds.SizeZ * 0.5);

                var radius = Math.Max(1.0, Math.Sqrt(
                    bounds.SizeX * bounds.SizeX +
                    bounds.SizeY * bounds.SizeY +
                    bounds.SizeZ * bounds.SizeZ) * 0.5);

                cam.FieldOfView = 34;
                var halfFov = cam.FieldOfView * Math.PI / 360.0;
                var distance = (radius / Math.Tan(halfFov)) * 1.15;

                // Stable side profile with slight top angle (matches expected default view).
                var viewDir = new System.Windows.Media.Media3D.Vector3D(-1.0, -0.16, -0.12);
                viewDir.Normalize();

                cam.Position = new System.Windows.Media.Media3D.Point3D(
                    centre.X - (viewDir.X * distance),
                    centre.Y - (viewDir.Y * distance),
                    centre.Z - (viewDir.Z * distance));
                cam.LookDirection = new System.Windows.Media.Media3D.Vector3D(
                    centre.X - cam.Position.X,
                    centre.Y - cam.Position.Y,
                    centre.Z - cam.Position.Z);
                cam.UpDirection = new System.Windows.Media.Media3D.Vector3D(0, 0, 1);
                cam.NearPlaneDistance = Math.Max(0.2, radius * 0.01);
                cam.FarPlaneDistance = Math.Max(1000, radius * 40);

                viewPort3d1.FixedRotationPoint = centre;
                viewPort3d1.FixedRotationPointEnabled = true;
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void Reset3DViewHard()
        {
            try
            {
                if (!ApplyPreferred3DView())
                {
                    viewPort3d1.FixedRotationPointEnabled = false;
                    FitCameraToScene();
                }

                var t = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(220) };
                int retryCount = 0;
                t.Tick += (s, ev) =>
                {
                    retryCount++;
                    if (ApplyPreferred3DView())
                    {
                        t.Stop();
                    }
                    else if (retryCount >= 6)
                    {
                        t.Stop();
                        FitCameraToScene();
                    }
                };
                t.Start();
            }
            catch
            {
            }
        }
        //public DataGridCell GetCell(int row, int column)
        //{
        //    DataGridRow rowContainer = GetRow(row);

        //    if (rowContainer != null)
        //    {
        //        DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);

        //        DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
        //        if (cell == null)
        //        {
        //            dgBallastTanks.ScrollIntoView(rowContainer, dgBallastTanks.Columns[column]);
        //            cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
        //        }
        //        return cell;
        //    }
        //    return null;
        //}
        //public DataGridRow GetRow(int index)
        //{
        //    DataGridRow row = (DataGridRow)dgBallastTanks.ItemContainerGenerator.ContainerFromIndex(index);
        //    if (row == null)
        //    {
        //        dgBallastTanks.UpdateLayout();
        //        dgBallastTanks.ScrollIntoView(dgBallastTanks.Items[index]);
        //        row = (DataGridRow)dgBallastTanks.ItemContainerGenerator.ContainerFromIndex(index);
        //    }
        //    return row;
        //}

        //public static T GetVisualChild<T>(Visual parent) where T : Visual
        //{
        //    T child = default(T);
        //    int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
        //    for (int i = 0; i < numVisuals; i++)
        //    {
        //        Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
        //        child = v as T;
        //        if (child == null)
        //        {
        //            child = GetVisualChild<T>(v);
        //        }
        //        if (child != null)
        //        {
        //            break;
        //        }
        //    }
        //    return child;
        //}

        private void ApplyNeutralSensorRowStyle(DataGridRow row)
        {
            if (row == null)
            {
                return;
            }

            row.ClearValue(DataGridRow.BackgroundProperty);
            row.ClearValue(DataGridRow.ForegroundProperty);
        }

        private void dgFuelOilTanks_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            Models.TableModel.Write_Log("Loading Color");
            ApplyNeutralSensorRowStyle(e.Row);
        }

       

        private void dgBallastTanks_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            ApplyNeutralSensorRowStyle(e.Row);
        }

        private void dgFreshWaterTanks_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            ApplyNeutralSensorRowStyle(e.Row);
        }

        private void dgMiscTanks_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            ApplyNeutralSensorRowStyle(e.Row);
        }

        private void dgCompartments_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            ApplyNeutralSensorRowStyle(e.Row);
        }

        private void dgWaterTightRegion_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            ApplyNeutralSensorRowStyle(e.Row);
        }

        private void dgVariableItems_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            ApplyNeutralSensorRowStyle(e.Row);
        }

        private void dgVariableItems_KeyDown(object sender, KeyEventArgs e)
        {
            System.Windows.Input.Key k = e.Key;

            if (header != "Item Name")
            {

                bool controlKeyIsDown = Keyboard.IsKeyDown(Key.LeftShift);
                if (!controlKeyIsDown && Key.D0 <= k && k <= Key.D9 || Key.NumPad0 <= k && k <= Key.NumPad9 || k == Key.Decimal || k == Key.OemPeriod || k == Key.OemMinus)
                {
                }

                else
                {
                    e.Handled = true;
                }
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Models.BO.clsGlobVar.SimulationStabilityType = "Intact";
        }

        

       

     




    }
}
