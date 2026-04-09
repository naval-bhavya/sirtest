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
using System.Windows.Media.Media3D;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Data;
using System.Reflection;
using WpfMvvmStability.Models.BO;
//using System.Windows.Input;
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
using System.Text.RegularExpressions;
using System.Data.Common;
using System.Collections;
using System.ComponentModel;
using WpfMvvmStability.Models.DAL;
using System.Runtime.Serialization.Formatters.Binary;
using System.Globalization;
using System.Windows.Controls.Primitives;
using System.Data.SqlClient;
using HelixToolkit.SharpDX;
#endregion 

namespace WpfMvvmStability.Views
{
    /// <summary>
    /// Interaction logic for SimulationModeMain.xaml
    /// </summary>
    public partial class SimulationModeMain : UserControl
    {
        double x1 = 0, y1 = 0, x2 = 0, y2 = 0, x3 = 0, y3 = 0, x4 = 0, y4 = 0, x5 = 0, x6 = 0, y5 = 0, y6 = 0, x7 = 0, x8 = 0, y7 = 0, y8 = 0, x9=0, y9=0, x10=0, y10=0;
        private string folderPath = "3D\\";
        private readonly Dictionary<string, HelixToolkit.SharpDX.MeshGeometry3D> _geometryCache = new Dictionary<string, HelixToolkit.SharpDX.MeshGeometry3D>();
        private int runningWorkers;
        int index3D;
        private HelixToolkit.Wpf.Plane3D ContourPlane;
        int index;
        int PK = 0;
        int canvasselect = 0;
        int indexvar;
        string header;
        public int popMessageBox =0;
        public int selectionchangecount = 0;
        int TankID;
        TextBox tb = new TextBox();
        private decimal PercentageFill;
        private string TankNameForPercentage;
        public static Dictionary<int, decimal> maxVolume;

        private Bounds3D bounds;
        private Bounds3D boundsProfile;
        private Bounds3D boundsPlanA;
        private Bounds3D boundsPlanB;
        private WpfWireframeGraphics3DUsingDrawingVisual wpfGraphics;
        private WireframeGraphics2Cache graphicsCache;
        private GraphicsConfig graphicsConfig;

        private Bounds3D boundsNew;
        private WpfWireframeGraphics3DUsingDrawingVisual wpfGraphicsNew;
        private WireframeGraphics2Cache graphicsCacheNew;
        private GraphicsConfig graphicsConfigNew;

        private delegate void UpdateProgressBarDelegate(
         System.Windows.DependencyProperty dp, Object value);

        private WW.Math.Vector3D translation;
        private double scaling = 1d;

        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();

        BackgroundWorker bgWorker;
        private bool isCalculationRunning = false;

        public SimulationModeMain()
        {
            try
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                InitializeComponent();
                this.MouseRightButtonDown += new System.Windows.Input.MouseButtonEventHandler(canvas2DProfile_MouseRightButtonDown_1);
                GetDataforDataGridInitialized();

                {
                    Models.BO.clsGlobVar.DamageCase = "";
                    DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
                    string Err = "";
                    string cmd = "Update [tblSimulationMode_Tank_Status] set [IsDamaged]=0 where [User]='dbo' and [Tank_ID] between 1 and 97 ";
                    cmd += "Update  [tblSimulationMode_Loading_Condition] set [IsDamaged]=0 ,[Status]=0 where [User]='dbo' and [Tank_ID] between 1 and 97 ";
                  
                    command.CommandText = cmd;
                    command.CommandType = CommandType.Text;
                    Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err); 
                }

              
                Models.TableModel.SimulationModeData();
             
                ViewModels.CadViewModel.Cad2dModels();
                GetDataforDataGridInitialized();
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.GetType() == typeof(MainWindow))
                    {
                        //((window as MainWindow).frameSimulationMode.Template.FindName("btnSimulationModeLongitudinal", (window as MainWindow).frameSimulationMode) as Button).IsEnabled = false;
                        ((window as MainWindow).frameSimulationMode.Template.FindName("btnSimulationModeStability", (window as MainWindow).frameSimulationMode) as Button).IsEnabled = false;
                        ((window as MainWindow).frameSimulationMode.Template.FindName("btnSimulationModeCorrectiveAction", (window as MainWindow).frameSimulationMode) as Button).IsEnabled = false;
                        ((window as MainWindow).frameSimulationMode.Template.FindName("btnSimulationGenerateReport", (window as MainWindow).frameSimulationMode) as Button).IsEnabled = false;
                    }
                }
                Mouse.OverrideCursor = null;
            }
            catch
            {

            }

        }
        public void GetDataforDataGridInitialized()
        {
            try
            {
                SortedList FSMList = new SortedList();
                FSMList.Add("Actual", "0");
                FSMList.Add("MAX", "1");
                FSMList.Add("UserInput", "2");
                FSMType.ItemsSource = FSMList;
                FSMTypeCargo.ItemsSource = FSMList;
                FSMTypeBallast.ItemsSource = FSMList;
                FSMTypeFresh.ItemsSource = FSMList;
                FSMTypeFuel.ItemsSource = FSMList;
                FSMTypeMisc.ItemsSource = FSMList;
                FSMTypeWTRegion.ItemsSource = FSMList;


                SortedList statusList = new SortedList();
                SortedList statusListCmp = new SortedList();
                statusList.Add("Intact", "0");
                statusList.Add("Damage", "1");
                statusList.Add("Flood", "2");
                statusListCmp.Add("Intact", "0");
                statusListCmp.Add("Damage", "1");
                statusListCmp.Add("Flood", "2");

                Status.ItemsSource = statusListCmp;
                StatusCargo.ItemsSource = statusList;
                StatusBallast.ItemsSource = statusList;
                StatusFresh.ItemsSource = statusList;
                StatusFuel.ItemsSource = statusList;
                StatusMisc.ItemsSource = statusList;
                StatusWTRegion.ItemsSource = statusList;

                //var x=Models.BO.clsGlobVar.dtSimulationBallastTanks.DefaultView;
                dgCargoTanks.ItemsSource = Models.BO.clsGlobVar.dtSimulationCargoTanks.DefaultView;
                dgBallastTanks.ItemsSource = Models.BO.clsGlobVar.dtSimulationBallastTanks.DefaultView;
                dgFuelOilTanks.ItemsSource = Models.BO.clsGlobVar.dtSimulationFuelOilTanks.DefaultView;
                dgFreshWaterTanks.ItemsSource = Models.BO.clsGlobVar.dtSimulationFreshWaterTanks.DefaultView;
                dgMiscTanks.ItemsSource = Models.BO.clsGlobVar.dtSimulationMiscTanks.DefaultView;
                dgCompartments.ItemsSource = Models.BO.clsGlobVar.dtSimulationCompartments.DefaultView;

                dgVariableItems.ItemsSource = Models.BO.clsGlobVar.dtSimulationVariableItems.DefaultView;
                dgWTRegion.ItemsSource = Models.BO.clsGlobVar.dtSimulationWTRegion.DefaultView;
              


                Model(Models.BO.clsGlobVar.Profile, canvas2DProfile);
                Model(Models.BO.clsGlobVar.PlanA, canvas2DPlanA);
                Model(Models.BO.clsGlobVar.PlanB, canvas2DPlanB);
                //Model(Models.BO.clsGlobVar.PlanC, canvas2DPlanC);
                //ModelNew(Models.BO.clsGlobVar.PlanALL, canvas2DAll);


                //lblDeadWeight.Content = Math.Round(Convert.ToDouble(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["Lightship_Weight"]), 1).ToString();
                //lblDisplacement.Content = Math.Round(Convert.ToDouble(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["Displacement"]), 2).ToString();

                lblDisplacement.Content = Math.Round(Convert.ToDouble(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["Displacement"])).ToString();
                //decimal VarDisplacenrt = Convert.ToDecimal(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["Displacement"]);
                //lblDisplacement.Content = VarDisplacenrt.ToString("N");

                //lblGMT.Content = Math.Round(Convert.ToDouble(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["GMT"]), 2).ToString();
                decimal VarGMT = Convert.ToDecimal(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["GMT"]);
                lblGMT.Content = VarGMT.ToString("N3");

                // lblDraftAP.Content = Convert.ToString(Math.Round((Convert.ToDouble(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["Draft_AP"])),2));

                decimal VarDraftAft = Convert.ToDecimal(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["Draft_AP"]);
                lblDraftAP.Content = VarDraftAft.ToString("N3");


                //lblDraftFP.Content = Convert.ToString(Math.Round((Convert.ToDouble(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["Draft_FP"])),2));

                decimal VarDraftfp = Convert.ToDecimal(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["Draft_FP"]);
                lblDraftFP.Content = VarDraftfp.ToString("N3");

                //  lblDraftAftMark.Content = Convert.ToString(Math.Round((Convert.ToDouble(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["Draft_Aft_Mark"])),2));

                decimal VarDraftAftmark = Convert.ToDecimal(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["Draft_Aft_Mark"]);
                lblDraftAftMark.Content = VarDraftAftmark.ToString("N3");

                decimal VarfwdMark = Convert.ToDecimal(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["Draft_Fore_Mark"]);
                lblDraftFwdMark.Content = VarfwdMark.ToString("N3");
                decimal varmid = Convert.ToDecimal(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["Draft_Mean"]);
                lblDraftMean.Content = varmid.ToString("N3");
                decimal varDome = Convert.ToDecimal(Models.BO.clsGlobVar.dtSimulationDrafts.Rows[0]["Draft_Sonar_Dome"]);
                lblSONARDOME.Content = varDome.ToString("N3");
                decimal VarPropeller = Convert.ToDecimal(Models.BO.clsGlobVar.dtSimulationDrafts.Rows[0]["Draft_Propeller"]);
                lblPROPELLER.Content = VarPropeller.ToString("N3");
                decimal KGS = Convert.ToDecimal(Models.BO.clsGlobVar.dtSimulationDrafts.Rows[0]["KG(Solid)"]);
                lblKG.Content = KGS.ToString("N3");
                decimal VarKGF = Convert.ToDecimal(Models.BO.clsGlobVar.dtSimulationDrafts.Rows[0]["KG(Fluid)"]);
                lblKGF.Content = VarKGF.ToString("N3");

                decimal varLCG = Convert.ToDecimal(Models.BO.clsGlobVar.dtSimulationDrafts.Rows[0]["LCG"]);
                lblLCG.Content = varLCG.ToString("N3");


                decimal val = Convert.ToDecimal(Models.BO.clsGlobVar.dtSimulationDrafts.Rows[0]["FSC"]);
                lblFSC.Content = val.ToString("N3");
                decimal varTpc = Convert.ToDecimal(Models.BO.clsGlobVar.dtSimulationDrafts.Rows[0]["TPC"]);
                lblTPC.Content = varTpc.ToString("N3");
                decimal varMCT = Convert.ToDecimal(Models.BO.clsGlobVar.dtSimulationDrafts.Rows[0]["MCT"]);
                lblMTC.Content = varMCT.ToString("0");


                if (Convert.ToDouble(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["Heel"]) < 0)
                {
                    lblHeel.Content = "PORT";
                    decimal HeelSTBD = (Convert.ToDecimal(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["Heel"]));
                    HeelSTBD = HeelSTBD * -1;
                    lblHeel.Content = HeelSTBD.ToString("N3");

                }
                else if (Convert.ToDouble(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["Heel"]) > 0)
                {
                    lblHeel.Content = "STBD";
                    Double abs = (Convert.ToDouble(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["Heel"]));
                    decimal HeelPort = Convert.ToDecimal((-1) * abs);
                    lblHeel.Content = HeelPort.ToString("N3");

                }
                else
                {
                    lblHeel.Content = " ";
                    lblHeel.Content = Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["Heel"].ToString();
                }


                if (Convert.ToDouble(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["Trim"]) > 0)
                {
                    lblTrim.Content = "FWD";

                    // lblTrim.Content = (Convert.ToString(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["TRIM"])); VarTrimAFWD

                    decimal VarTrimAFWD = Convert.ToDecimal(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["TRIM"]);
                    lblTrim.Content = VarTrimAFWD.ToString("N3");

                    //lblTrim.Content = Convert.ToString(Math.Round((Convert.ToDouble(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["TRIM"])), 2));
                }
                else if (Convert.ToDouble(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["Trim"]) < 0)
                {
                    lblTrim.Content = "AFT";
                    Double abs = (Convert.ToDouble(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["TRIM"]));
                    // lblTrim.Content = Convert.ToString((-1) * abs); VarTrimAFTR
                    decimal VarTrimAFTR = Convert.ToDecimal((-1) * abs);
                    lblTrim.Content = VarTrimAFTR.ToString("N3");

                }
                else
                {
                    lblTrim.Content = " ";
                    lblTrim.Content = (Convert.ToString(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["TRIM"]));
                }

                if (Convert.ToString(clsGlobVar.dtSimulationStabilitySummary.Rows[0]["Stability_Type"]).ToUpper() == "INTACT")
                {
                    //lblMaxBM.Content = Models.BO.clsGlobVar.dtSimulationSFandBMMax.Rows[0]["Max_BM"].ToString();
                    //lblMaxSF.Content = Models.BO.clsGlobVar.dtSimulationSFandBMMax.Rows[0]["Max_SF"].ToString();
                    //lblFrameBM.Content = Models.BO.clsGlobVar.dtSimulationSFandBMMax.Rows[0]["Distance_BM"].ToString();
                    //lblFrameSF.Content = Models.BO.clsGlobVar.dtSimulationSFandBMMax.Rows[0]["Distance_SF"].ToString();
                }
                else
                {
                    //lblMaxBM.Content = "NA";
                    //lblMaxSF.Content = "NA";
                    //lblFrameBM.Content = "NA";
                    //lblFrameSF.Content = "NA";
                }
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                //ViewModels.MainViewModel.Flag3D = false;
                //this.DataContext = new ViewModels.MainViewModel(folderPath, viewPort3d);
                //ViewModels.MainViewModel.Flag3D = false;
                Refresh3dNew();
                Mouse.OverrideCursor = null;

                maxVolume = new Dictionary<int, decimal>();

                DataTable dtmaxVolume = new DataTable();
                DataView DV = Models.BO.clsGlobVar.dtSimulationModeAllTanks.AsDataView();
                DV.RowFilter = "Group NOT LIKE 'Variable Data'";
                DV.RowFilter = "Group NOT LIKE 'Lightship'";
                dtmaxVolume = DV.ToTable();

                for (int i = 0; i < dtmaxVolume.Rows.Count - 2; i++)
                {
                    maxVolume.Add(Convert.ToInt32(dtmaxVolume.Rows[i]["Tank_ID"]), Convert.ToDecimal(dtmaxVolume.Rows[i]["Max_Volume"]));

                }



            }
            catch
            {

            }

        }

        private void dgTankCompartment_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //Models.TableModel.Write_Log(" START : dgTankCompartment_PreviewTextInput");
            if (header != "Item Name")
            {
                if (!char.IsDigit(e.Text, e.Text.Length - 1) && !e.Text.Contains('.'))
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
            Models.TableModel.Write_Log(" END : dgTankCompartment_PreviewTextInput");
        }

        private void dgTankCompartment_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            selectionchangecount = 0;
            tb = e.Column.GetCellContent(e.Row) as TextBox;
            Models.TableModel.Write_Log(" START :  dgTankCompartment_PreparingCellForEdit");
        }

        private void btn3DZoomExtents_Click(object sender, RoutedEventArgs e)
        {
            FitCameraToScene();
        }

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

                canvas2D.Children.Add(wpfGraphics.Canvas);

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

        List<SelectionProcess> _objSelection = new List<SelectionProcess>();
      
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
            else activeBounds = bounds;

            double canvasWidth = canvas2D.ActualWidth;
            double canvasHeight = canvas2D.ActualHeight;

            // The DXF has a fixed reference datum at Y≈-120000 far below the ship.
            // All views: ship content spans Y≈0 to Y=Corner2.Y, so center at Corner2.Y*0.5.
            // Profile gets tighter zoom (0.60) to show hull larger; plan views use 0.75.
            double yCenter = activeBounds.Corner2.Y * 0.45;
            double yHalf = (canvas2D == canvas2DProfile)
                ? activeBounds.Corner2.Y * 0.65
                : (canvas2D == canvas2DPlanB) ? activeBounds.Corner2.Y * 0.55 : activeBounds.Corner2.Y * 0.75;
            Point2D effectiveCorner1 = new Point2D(activeBounds.Corner1.X, yCenter - yHalf);
            Point2D effectiveCorner2 = new Point2D(activeBounds.Corner2.X, yCenter + yHalf);
            Point2D effectiveCenter  = new Point2D(activeBounds.Center.X,  yCenter);

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
            transformGroup.Children.Add(new TranslateTransform()
            {
                X = translation.X * canvasWidth / 2d,
                Y = -translation.Y * canvasHeight / 2d
            });

            //@MT Code added for Ship Profile Image Rotate :START
            if (canvas2D == canvas2DProfile)
            {
                transformGroup.Children.Add(new RotateTransform()
                {
                    Angle = angle
                });
            }
            //@MT Code added for Ship Profile Image Rotate :END

            canvas2D.RenderTransform = transformGroup;

        }
        /****************************************************************************************************/
       
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
            transformGroup.Children.Add(new TranslateTransform()
            {
                X = translation.X * canvasWidth / 2d,
                Y = -translation.Y * canvasHeight / 2d
            });

            canvas2D.RenderTransform = transformGroup;

        }
        /*********************************************************************************************/

        public decimal GetSoundingFromVolume(int TankID, decimal Volume)
        {
            Models.TableModel.Write_Log(" START : GetSoundingFromVolume");
            decimal innage = 0;
            try
            {
                string Err = "";
                string queryInnage = @"
                                    BEGIN
	                                    DECLARE @Volume DECIMAL(10,3), @Sounding DECIMAL(10,3),@Sounding_low DECIMAL(10,3),@Sounding_high DECIMAL(10,3),@Volume_low DECIMAL(10,3),@Volume_high DECIMAL(10,3)
                                    DECLARE @i_Tank_ID int,	@i_Volume DECIMAL(10,3),@o_Sounding DECIMAL(10,3)
	
	                                    Set @i_Tank_ID=" + TankID + @"
	                                    Set @i_Volume=" + Volume + @"
	                                    IF EXISTS(SELECT 1 FROM tblMaster_Calibration tmhh WHERE Heel = 0 AND Trim = 0 AND Capacity_MC = @i_Volume and Tank_ID=@i_Tank_ID)
	                                    BEGIN
		                                    SELECT @Sounding = Sounding FROM 	 [dbo].[tblMaster_Calibration]  
		                                    WHERE HEEL = 0  AND TRIM = 0 AND Capacity_MC = @i_Volume and Tank_ID=@i_Tank_ID
	                                    END
	                                    ELSE
	                                    BEGIN
		
			                                    SELECT TOP 1 @Sounding_low = Sounding,@Volume_low = Capacity_MC FROM 	 [dbo].[tblMaster_Calibration]  
			                                    WHERE HEEL = 0  AND TRIM = 0 AND Tank_ID=@i_Tank_ID  AND Capacity_MC < @i_Volume ORDER BY Capacity_MC DESC				
	
			                                    SELECT TOP 1 @Sounding_high = Sounding,@Volume_high = Capacity_MC FROM 	 [dbo].[tblMaster_Calibration]  
			                                    WHERE HEEL = 0  AND TRIM = 0 AND Tank_ID=@i_Tank_ID AND Capacity_MC > @i_Volume ORDER BY Capacity_MC

		
		                                    SET @Sounding = ROUND(((@i_Volume*@Sounding_high)-(@i_Volume*@Sounding_low)-(@Volume_low*@Sounding_high)+(@Volume_high*@Sounding_low))/(@Volume_high-@Volume_low),3)
	
	                                    END
	
	                                    SET @o_Sounding =@Sounding
	                                    select @o_Sounding
                                    END
                                    ";
                DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
                command.CommandText = queryInnage;
                command.CommandType = CommandType.Text;
                DataTable dtsounding = Models.DAL.clsDBUtilityMethods.GetTable(command, Err);
                innage = Convert.ToDecimal(dtsounding.Rows[0][0]);
                return innage;
                Models.TableModel.Write_Log(" START : GetSoundingFromVolume");
            }
            catch { return innage; }

        }

        public decimal GetVolumeFromSounding(int TankID, decimal innage)
        {
            Models.TableModel.Write_Log(" START : GetVolumeFromSounding");
            decimal volume = 0;
            try
            {
                string Err = "";
                string queryVolume = @"
                                            BEGIN
	                                        DECLARE @Volume DECIMAL(10,3), @Sounding DECIMAL(10,3),@Sounding_low DECIMAL(10,3),@Sounding_high DECIMAL(10,3),@Volume_low DECIMAL(10,3),@Volume_high DECIMAL(10,3)
                                            DECLARE @i_Tank_ID int,	@o_Volume  DECIMAL(10,3),@i_Sounding  DECIMAL(10,3)
	
	                                        Set @i_Tank_ID=" + TankID + @"
	                                        Set @i_Sounding=" + innage + @"
	
	                                        IF EXISTS(SELECT 1 FROM tblMaster_Calibration tmhh WHERE Heel = 0 AND Trim = 0 AND Sounding = @i_Sounding and Tank_ID=@i_Tank_ID)
	                                        BEGIN
		                                        SELECT @Volume = Capacity_MC FROM 	 [dbo].[tblMaster_Calibration]  
		                                        WHERE HEEL = 0  AND TRIM = 0 AND Sounding = @i_Sounding and Tank_ID=@i_Tank_ID
	                                        END
	                                        ELSE
	                                        BEGIN
		
			                                        SELECT TOP 1 @Sounding_low = Sounding,@Volume_low = Capacity_MC FROM 	 [dbo].[tblMaster_Calibration]  
			                                        WHERE HEEL = 0  AND TRIM = 0 AND Tank_ID=@i_Tank_ID  AND Sounding < @i_Sounding ORDER BY Capacity_MC DESC				
	
			                                        SELECT TOP 1 @Sounding_high = Sounding,@Volume_high = Capacity_MC FROM 	 [dbo].[tblMaster_Calibration]  
			                                        WHERE HEEL = 0  AND TRIM = 0 AND Tank_ID=@i_Tank_ID AND Sounding < @i_Sounding ORDER BY Capacity_MC

		
		                                        SET @Volume = ROUND(((@i_Sounding*@Volume_high)-(@i_Sounding*@Volume_low)-(@Sounding_low*@Volume_high)+(@Sounding_high*@Volume_low))/(@Sounding_high-@Sounding_low),3)
	
	                                        END
	
	                                        SET @o_Volume =@Volume
	                                        select @o_Volume
                                            END";

                DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
                command.CommandText = queryVolume;
                command.CommandType = CommandType.Text;
                DataTable dtvolume = Models.DAL.clsDBUtilityMethods.GetTable(command, Err);
                volume = Convert.ToDecimal(dtvolume.Rows[0][0]);

                return volume;

                Models.TableModel.Write_Log(" END : GetVolumeFromSounding");
            }
            catch { return volume; }

        }

        private void dgTankCompartment_CurrentCellChanged(object sender, EventArgs e)
        {
            try
            {             
                Models.TableModel.Write_Log(" START : dgTankCompartment_CurrentCellChanged");
                DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
                string Err = "";
                int TankId, fsmType;
                decimal percentfill;
                bool isVisible;
                //if (clsGlobVar.FlagDamageCases == false)
                {
                    TankId = Convert.ToInt16(((sender as DataGrid).Items[index] as DataRowView)["Tank_ID"]);
                    //isVisible=Convert.ToBoolean(((sender as DataGrid).Items[index] as DataRowView)["IsVisible"]);

                    if (header.Trim() == "Volume (Cu.m)" || header.Trim() == "Percent Fill" || header.Trim() == "S.Gravity" || header.Trim() == "Mass(T)" || header.Trim() == "Innage(m)" || header.Trim() == "Flood Time(min)" || header.Trim() == "Flood Rate(TPH)")
                    {
                        decimal volume = 0, sg, weight = 0, innage = 0,FloodRate=0,FloodTime=0;
                        decimal minsounding = 0;
                        sg = Convert.ToDecimal(((sender as DataGrid).Items[index] as DataRowView)["SG"]);
                        percentfill = Convert.ToDecimal(((sender as DataGrid).Items[index] as DataRowView)["Percent_Full"]);
                        FloodRate = Convert.ToDecimal(((sender as DataGrid).Items[index] as DataRowView)["FloodRate"]);
                        FloodTime = Convert.ToDecimal(((sender as DataGrid).Items[index] as DataRowView)["FloodTime"]);
                        decimal maxsounding = maxVolume[TankId];

                        if (header.Trim() == "Volume (Cu.m)")
                        {
                            volume = Convert.ToDecimal(((sender as DataGrid).Items[index] as DataRowView)["Volume"]);
                            percentfill = Convert.ToDecimal((volume * 100) / maxsounding);
                        }
                        if (header.Trim() == "Percent Fill")
                        {
                            volume = (percentfill * maxsounding) / 100;
                        }
                        if (header.Trim() == "S.Gravity")
                        {
                            volume = Convert.ToDecimal(((sender as DataGrid).Items[index] as DataRowView)["Volume"]);
                            weight = volume * sg;
                        }
                        if (header.Trim() == "Mass(T)")
                        {
                            volume = Convert.ToDecimal(((sender as DataGrid).Items[index] as DataRowView)["Weight"]) / sg;
                            percentfill = Convert.ToDecimal((volume * 100) / maxsounding);
                        }

                        if (header.Trim() == "Flood Time(min)")
                        {
                            FloodTime = Convert.ToDecimal(((sender as DataGrid).Items[index] as DataRowView)["FloodTime"]);
                            weight = FloodRate * FloodTime / 60;
                            volume = weight / sg;
                            percentfill = Convert.ToDecimal((volume * 100) / maxsounding);

                            if(percentfill>100)
                            {
                                percentfill=100;
                                volume=maxsounding;
                                weight=volume*sg;

                                string error1 = "Compartment Fully Flooded After " + Math.Round((weight * 60 / FloodRate),1) + " Minutes";
                                System.Windows.MessageBox.Show(error1);
                                
                            }
                        }
                        if (header.Trim() == "Flood Rate(TPH)")
                        {
                            FloodRate = Convert.ToDecimal(((sender as DataGrid).Items[index] as DataRowView)["FloodRate"]);
                            weight = FloodRate * FloodTime / 60;
                            volume = weight / sg;

                            percentfill = Convert.ToDecimal((volume * 100) / maxsounding);
                            if (percentfill > 100)
                            {
                                percentfill = 100;
                                volume = maxsounding;
                                weight = volume * sg;

                                string error2 = "Compartment Fully Flooded After " + Math.Round((weight * 60 / FloodRate),1) + " Minutes";
                                System.Windows.MessageBox.Show(error2);
                                
                            }
                        }


                        if (header.Trim() == "Innage(m)")
                        {
                            volume = GetVolumeFromSounding(TankId, Convert.ToDecimal(((sender as DataGrid).Items[index] as DataRowView)["Sounding_Level"]));
                            percentfill = Convert.ToDecimal((volume * 100) / maxsounding);
                        }
                        innage = GetSoundingFromVolume(TankId, volume);
                        weight = volume * sg;

                        if (FloodRate == 0)
                            FloodTime = 0;
                        else
                        FloodTime = weight*60/ FloodRate;

                        ((sender as DataGrid).Items[index] as DataRowView)["Volume"] = Math.Round(volume, 3);
                        ((sender as DataGrid).Items[index] as DataRowView)["Weight"] = Math.Round(weight, 3);
                        ((sender as DataGrid).Items[index] as DataRowView)["Percent_Full"] = Math.Round(percentfill, 3);
                        ((sender as DataGrid).Items[index] as DataRowView)["Sounding_Level"] = Math.Round(innage, 3);
                        ((sender as DataGrid).Items[index] as DataRowView)["FloodTime"] = Math.Round(FloodTime, 3);
                        //((sender as DataGrid).Items[index] as DataRowView)["Mass(T)"] = Math.Round(innage, 3);
                        decimal res1 = decimal.Compare(minsounding, volume);
                        decimal res2 = decimal.Compare(volume, maxsounding);
                        int result1 = (int)res1;
                        int result2 = (int)res2;
                        if (result1 > 0 || result2 > 0)
                        {
                            
                            string error = "Volume should be between " + minsounding + " and " + maxsounding;
                            System.Windows.MessageBox.Show(error);
                            return;
                        }
                        else
                        {
                            string query = "update tblSimulationMode_Tank_Status set [Volume]=" + volume + ",SG=" + sg + ",Weight=" + weight + " where Tank_ID=" + TankId;
                            command = Models.DAL.clsDBUtilityMethods.GetCommand();
                            command.CommandText = query;
                            command.CommandType = CommandType.Text;
                            Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);
                            query = "update [tblSimulationMode_Loading_Condition] set [Volume]=" + volume + ",SG=" + sg + ",Weight=" + weight + ",Percent_Full=" + percentfill + ",Sounding_Level=" + innage + ",FloodRate="+ FloodRate +",FloodTime="+ FloodTime +" where Tank_ID=" + TankId;
                            command = Models.DAL.clsDBUtilityMethods.GetCommand();
                            command.CommandText = query;
                            command.CommandType = CommandType.Text;
                            Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);
                        }
                        index = -1;
                        TankId = 0;
                       
                    }
                    //if (header == "3D")
                    //{
                    //    string query = "update [tblSimulationMode_Loading_Condition] set [IsVisible]="+isVisible+" where Tank_ID=" + TankId;
                    //    command = Models.DAL.clsDBUtilityMethods.GetCommand();
                    //    command.CommandText = query;
                    //    command.CommandType = CommandType.Text;
                    //    Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);
                    //    string TankName = Convert.ToString(((sender as DataGrid).Items[index] as DataRowView)["Tank_Name"]);
                    //    string str1 = TankName.Replace("/", "");
                    //    for (int j = 0; j < 30; j++)
                    //    {
                    //        try
                    //        {
                    //            Visual3D model = scene3D.Children[j + 2] as Visual3D;
                    //            string modelname = model.GetName();
                    //            if (modelname == str1)
                    //            {
                    //                //scene3D.Children.RemoveAt(j + 2);
                    //                scene3D.Children.Remove(model);

                    //            }
                    //        }
                    //        catch
                    //        {
                    //        }
                    //    }
                    //}

                    if (header.Trim() == "FSM(T-m)")
                    {
                        decimal fsm;
                        fsm = Convert.ToDecimal(((sender as DataGrid).Items[index] as DataRowView)["FSM"]);
                        fsmType = Convert.ToInt16(((sender as DataGrid).Items[index] as DataRowView)["FSMType"]);
                        if (fsmType == 2)
                        {
                            command = Models.DAL.clsDBUtilityMethods.GetCommand();
                            string query1 = "update tblSimulationMode_Loading_Condition set FSMInput=" + fsm + " where Tank_ID=" + TankId;
                            command.CommandText = query1;
                            command.CommandType = CommandType.Text;
                            Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);

                        }
                    }

                    if (header.Trim() == "Flood Rate(TPH)")
                    {
                        decimal floodRate;
                        floodRate = Convert.ToInt16(((sender as DataGrid).Items[index] as DataRowView)["FloodRate"]);
                        string query1 = "update tblSimulationMode_Loading_Condition set FloodRate=" + floodRate + " where Tank_ID=" + TankId;
                        command.CommandText = query1;
                        command.CommandType = CommandType.Text;
                        Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);
                    }

                }
                Models.TableModel.Write_Log(" END : dgTankCompartment_CurrentCellChanged");
            }
            catch
            {
            }

        }
        private void dgTankCompartment_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            selectionchangecount = 0;
            Models.TableModel.Write_Log(" START : dgTankCompartment_BeginningEdit");
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
                Models.TableModel.Write_Log(" END : dgTankCompartment_BeginningEdit");
            }
            catch
            {

            }
        }
        private void FSMType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Models.TableModel.Write_Log(" START : FSMType_SelectionChanged");
            try
            {
                DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
                string Err = "";
                int TankId;
                var cb = ((System.Windows.Controls.ComboBox)sender);
                //DataGridRow dataGridRow = FindParent<DataGridRow>(cb);
                //int index = dataGridRow.GetIndex();
                DataGrid dataGrid = FindParent<DataGrid>(cb);
                TankId = Convert.ToInt16((dataGrid.Items[index] as DataRowView)["Tank_ID"]);
                var comboBox = sender as System.Windows.Controls.ComboBox;
                var selectedItem = comboBox.SelectedValue;
                string FSMType = Convert.ToString(selectedItem);
                string query = "update tblSimulationMode_Loading_Condition set [FSMType]=" + FSMType.ToString() + " where Tank_ID=" + TankId;
                command.CommandText = query;
                command.CommandType = CommandType.Text;
                Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);
                Models.TableModel.Write_Log(" END : FSMType_SelectionChanged");
            }
            catch
            {
            }
        }
        //public void cmbchange()
        //{
        //    System.Windows.MessageBox.Show("FIRST RUN INTACT CONDITION THEN DAMAGE");
        //}
        private void StatusType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //sachin 21.9.22
            //int type = Convert.ToInt32(clsGlobVar.dtgetsimulationtype.Rows[0]["type"]);
            //var comboBox = sender as System.Windows.Controls.ComboBox;
            //var selectedItem = comboBox.SelectedValue;
            //string statusType = Convert.ToString(selectedItem);
            //sachin 21.9.22

            if (selectionchangecount!=0) 
            {
                return;
            }

            if (dgCargoTanks.IsLoaded == true || dgBallastTanks.IsLoaded==true || dgFreshWaterTanks.IsLoaded==true|| dgFuelOilTanks.IsLoaded==true ||dgMiscTanks.IsLoaded==true || dgCompartments.IsLoaded==true || dgWTRegion.IsLoaded==true )
            {
                try
                {
                    //if (type != 0 && "1" == statusType.ToString())
                    //{
                    //    comboBox.SelectedValue = 0;
                    //    System.Windows.MessageBox.Show("FIRST RUN INTACT CONDITION THEN DAMAGE");
                    //    //statusType = "0";
                    //    //comboBox.SelectedValue = 0;
                    //    //cmbchange();
                    //}
                    //else
                    {
                        int TankId, CellCnt;

                        string Err = string.Empty;
                        string status = string.Empty;
                        if (dgWTRegion.IsLoaded == true)
                        {
                            DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
                            var RowsCnt = dgWTRegion.SelectedItems;

                            if (RowsCnt.Count > 1)
                            {
                                DataRowView row = (DataRowView)dgWTRegion.SelectedItems[0];
                                string selectedStatus = row["status"].ToString();
                                TankId = Convert.ToInt16(row["Tank_Id"]);
                                string query = " ";
                                for (int i = 1; i < RowsCnt.Count; i++)
                                {
                                    DataRowView mulRow = (DataRowView)RowsCnt[i];
                                    mulRow["Status"] = selectedStatus;
                                    int TankIdMul = Convert.ToInt16(mulRow["tank_Id"]);
                                    query += "update  tblSimulationMode_Loading_Condition set Status=" + selectedStatus.ToString() + " where Tank_ID=" + TankIdMul;
                                    if (selectedStatus == "0")
                                    {
                                        query += "  update  tblSimulationMode_Loading_Condition set IsDamaged=0 where Tank_ID=" + TankIdMul;
                                        query += "  update  tblSimulationMode_Tank_Status set IsDamaged=0 where Tank_ID=" + TankIdMul;
                                    }
                                    //sachin
                                    else if (selectedStatus == "2")
                                    {
                                        query += "  update  tblSimulationMode_Loading_Condition set IsDamaged=0 where Tank_ID=" + TankIdMul;
                                        query += "  update  tblSimulationMode_Tank_Status set IsDamaged=0 where Tank_ID=" + TankIdMul;
                                    }
                                    //sachin above
                                    else
                                    {
                                        query += "  update  tblSimulationMode_Loading_Condition set IsDamaged=1 where Tank_ID=" + TankIdMul;
                                        query += "  update  tblSimulationMode_Tank_Status set IsDamaged=1 where Tank_ID=" + TankIdMul;
                                    }
                                    command.CommandText = query;
                                    command.CommandType = CommandType.Text;
                                    Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);
                                }
                                query = "update  tblSimulationMode_Loading_Condition set Status=" + selectedStatus.ToString() + " where Tank_ID=" + TankId;
                                if (selectedStatus == "0")
                                {
                                    query += "  update  tblSimulationMode_Loading_Condition set IsDamaged=0 where Tank_ID=" + TankId;
                                    query += "  update  tblSimulationMode_Tank_Status set IsDamaged=0 where Tank_ID=" + TankId;
                                }
                                //sachin
                                else if (selectedStatus == "2")
                                {
                                    query += "  update  tblSimulationMode_Loading_Condition set IsDamaged=0 where Tank_ID=" + TankId;
                                    query += "  update  tblSimulationMode_Tank_Status set IsDamaged=0 where Tank_ID=" + TankId;
                                }
                                //sachin above
                                else
                                {
                                    query += "  update  tblSimulationMode_Loading_Condition set IsDamaged=1 where Tank_ID=" + TankId;
                                    query += "  update  tblSimulationMode_Tank_Status set IsDamaged=1 where Tank_ID=" + TankId;

                                }
                                command.CommandText = query;
                                command.CommandType = CommandType.Text;
                                Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);

                            }

                        }
                    }
                }
                catch { }

                try
                {
                    //if (type != 0 && "1" == statusType.ToString())
                    //{
                    //    //System.Windows.MessageBox.Show("FIRST RUN INTACT CONDITION THEN DAMAGE");
                    //    //statusType= "0";
                    //    //comboBox.SelectedValue = 0;

                    //}
                    //else
                    {

                        DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
                        string Err = "";
                        int TankId;
                        var cb = ((System.Windows.Controls.ComboBox)sender);
                        DataGrid dataGrid = FindParent<DataGrid>(cb);
                        TankId = Convert.ToInt16((dataGrid.Items[index] as DataRowView)["Tank_ID"]);
                        var comboBox = sender as System.Windows.Controls.ComboBox;

                        var selectedItem = comboBox.SelectedValue;
                        string statusType = Convert.ToString(selectedItem);
                        int type = Convert.ToInt32(clsGlobVar.dtgetsimulationtype.Rows[0]["type"]);
                       // if (type != 0 && "1" == statusType.ToString() && popMessageBox==0)
                            if (type != 0 && "1" == statusType.ToString())
                            {
                           // popMessageBox = 1;
                          //  comboBox.SelectedValue = 0;
                            System.Windows.MessageBox.Show("FIRST RUN INTACT CONDITION THEN DAMAGE");
                            selectionchangecount = 1;
                            comboBox.SelectedValue = 0;
                            return;
                            //statu.SelectedIndex = 0;

                        }
                        if (type != 0 && "2" == statusType.ToString())
                        {
                            // popMessageBox = 1;
                            //  comboBox.SelectedValue = 0;
                            System.Windows.MessageBox.Show("FIRST RUN INTACT CONDITION THEN DAMAGE");
                            selectionchangecount = 1;
                            comboBox.SelectedValue = 0;
                            return;
                            //statu.SelectedIndex = 0;

                        }
                        else
                        {
                            
                            string query = "update  tblSimulationMode_Loading_Condition set Status=" + statusType.ToString() + " where Tank_ID=" + TankId;
                            if (statusType == "0")
                            {
                                query += "  update  tblSimulationMode_Loading_Condition set IsDamaged=0 where Tank_ID=" + TankId;
                                query += "  update  tblSimulationMode_Tank_Status set IsDamaged=0 where Tank_ID=" + TankId;
                            }
                            //sachin
                            else if (statusType == "2")
                            {
                                query += "  update  tblSimulationMode_Loading_Condition set IsDamaged=0 where Tank_ID=" + TankId;
                                query += "  update  tblSimulationMode_Tank_Status set IsDamaged=0 where Tank_ID=" + TankId;
                            }
                            //sachin above
                            else
                            {
                                query += "  update  tblSimulationMode_Loading_Condition set IsDamaged=1 where Tank_ID=" + TankId;
                                query += "  update  tblSimulationMode_Tank_Status set IsDamaged=1 where Tank_ID=" + TankId;

                            }
                            command.CommandText = query;
                            command.CommandType = CommandType.Text;
                            Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);
                            Models.TableModel.Write_Log(" END : StatusType_SelectionChanged");
                        }
                    }
                }
                catch
                {
                }
            
            }

        }

        public static Parent FindParent<Parent>(DependencyObject child) where Parent : DependencyObject
        {
            Models.TableModel.Write_Log(" START : FindParent");
            DependencyObject parentObject = child;

            //We are not dealing with Visual, so either we need to fnd parent or
            //get Visual to get parent from Parent Heirarchy.
            while (!((parentObject is System.Windows.Media.Visual)
                    || (parentObject is System.Windows.Media.Media3D.Visual3D)))
            {
                if (parentObject is Parent || parentObject == null)
                {
                    return parentObject as Parent;
                }
                else
                {
                    parentObject = (parentObject as FrameworkContentElement).Parent;
                }
            }

            //We have not found parent yet , and we have now visual to work with.
            parentObject = VisualTreeHelper.GetParent(parentObject);

            //check if the parent matches the type we're looking for
            if (parentObject is Parent || parentObject == null)
            {
                return parentObject as Parent;
            }
            else
            {
                //use recursion to proceed with next level
                return FindParent<Parent>(parentObject);
            }
            Models.TableModel.Write_Log(" END : FindParent");
        }


        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((MessageBox.Show("Are you sure you want to add new Variable Item?", "Please confirm.", MessageBoxButton.YesNo) == MessageBoxResult.Yes))
                {
                    DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
                    string Err = "";
                    string query = @"INSERT INTO tblFixedLoad_Simulation ([Load_Name],[Weight],[LCG],[TCG],[VCG],[Length],[Breadth],[Depth])
                                                              VALUES ('New Variable Load',0,0,0,0,0,0,0)";
                    command.CommandText = query;
                    command.CommandType = CommandType.Text;
                    Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);
                    Models.BO.clsGlobVar.dtSimulationVariableItems = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationModeVariableDetails");
                    dgVariableItems.ItemsSource = Models.BO.clsGlobVar.dtSimulationVariableItems.DefaultView;
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
                if (Models.BO.clsGlobVar.dtSimulationVariableItems.Rows.Count > 1)
                {
                    if ((MessageBox.Show("Are you sure, you want to delete selected Variable Item?", "Please confirm.", MessageBoxButton.YesNo) == MessageBoxResult.Yes))
                    {
                        DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
                        string Err = "";
                        int LoadId;
                        LoadId = Convert.ToInt16(((dgVariableItems).Items[indexvar] as DataRowView)["Load_Id"]);
                        string query = @"DELETE FROM tblFixedLoad_Simulation
                                    WHERE Load_Id=" + LoadId;
                        command.CommandText = query;
                        command.CommandType = CommandType.Text;
                        Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);
                        Models.BO.clsGlobVar.dtSimulationVariableItems = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationModeVariableDetails");
                        dgVariableItems.ItemsSource = Models.BO.clsGlobVar.dtSimulationVariableItems.DefaultView;
                    }
                }
                else
                {
                    MessageBox.Show("There should be atleast one row in the table");
                }
            }
            catch
            {
            }
        }

        private void dgVariableItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            indexvar = (sender as DataGrid).SelectedIndex;
            
        }

        private void btnCalculate_Click(object sender, RoutedEventArgs e)
        {
            //int type = Convert.ToInt32(clsGlobVar.dtgetsimulationtype.Rows[0]["type"]);
            //if (type != 0 && CmprItem.Trim() == "GHIJ(ii)")
            //{
            //    System.Windows.MessageBox.Show("FIRST RUN INTACT CONDITION");

            //}
            //else
            //{
                CalculateSimulationStability();
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.GetType() == typeof(MainWindow))
                    {
                        (window as MainWindow).MainWin.cMBDamage.SelectedIndex = 0;
                    }
                }
            //}
            
        }

        private void CalculateSimulationStability()
        {

            try
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

                isCalculationRunning = true;
                //(frameSimulationMode.Template.FindName("btnSimulationCalculate", frameSimulationMode) as Button).IsEnabled = false;
                bgWorker = new BackgroundWorker();
                bgWorker.DoWork += new DoWorkEventHandler(bgWorker_Do_Work);
                bgWorker.ProgressChanged += new ProgressChangedEventHandler
                        (bgWorker_ProgressChanged);
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
                            Process();
                        }
                        else
                        { i = 100; } 
                    }
                }

                if (!isCalculationRunning)
                {
                    //RefreshScreen();
                    if (Models.BO.clsGlobVar.CalculationResult == 0)
                    {
                        System.Windows.MessageBox.Show("Calculation terminated");
                       // System.Windows.MessageBox.Show("StabilityP15B calculation succeeded");
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("StabilityP15B calculation succeeded");
                        foreach (Window window in Application.Current.Windows)
                        {
                            if (window.GetType() == typeof(MainWindow))
                            {
                                ((window as MainWindow).frameSimulationMode.Template.FindName("btnSimulationGenerateReport", (window as MainWindow).frameSimulationMode) as Button).IsEnabled = true;
                            }
                        }
                    }
                }
                pRGSCalculation.Foreground = System.Windows.Media.Brushes.Red;
                pRGSCalculation.Value = 0; ;
                  Mouse.OverrideCursor = null;
            }
            catch
            {
            }

        }

        public void Process()
        {
            try
            {
                pRGSCalculation.Minimum = 0;
                pRGSCalculation.Maximum = 1400;
                pRGSCalculation.Value = 0;
                pRGSCalculation.Foreground = System.Windows.Media.Brushes.Green;
                double value = 0;
                UpdateProgressBarDelegate updatePbDelegate =
                    new UpdateProgressBarDelegate(pRGSCalculation.SetValue);
                do
                {
                    value += 1;
                    Dispatcher.Invoke(updatePbDelegate,
                        System.Windows.Threading.DispatcherPriority.Background,
                        new object[] { System.Windows.Controls.ProgressBar.ValueProperty, value });
                }
                while (pRGSCalculation.Value != pRGSCalculation.Maximum);
            }
            catch//(Exception ex)
            { 
                //ex.ToString(); 
            }
        }

        #region BackgroundWorker

        public void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            isCalculationRunning = false;
            Models.BO.clsGlobVar.SimulationStabilityType = Convert.ToString(clsGlobVar.dtSimulationStabilitySummary.Rows[0]["Stability_Type"]);

            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(MainWindow))
                {
                   int type=Convert.ToInt32(clsGlobVar.dtgetsimulationtype.Rows[0]["type"]);
                    if (type == 0)
                    {
                        ((window as MainWindow).frameSimulationMode.Template.FindName("lblSimulationStabilityType", (window as MainWindow).frameSimulationMode) as Label).Content = "Intact".ToUpper();
                    }
                    if (type != 0)
                    {
                        ((window as MainWindow).frameSimulationMode.Template.FindName("lblSimulationStabilityType", (window as MainWindow).frameSimulationMode) as Label).Content = "damage".ToUpper();
                    }
                    ((window as MainWindow).frameSimulationMode.Template.FindName("lblSimulationStabilityStatus", (window as MainWindow).frameSimulationMode) as Label).Content = Convert.ToString(clsGlobVar.dtSimulationStabilitySummary.Rows[0]["Stability_Status"]).ToUpper();

                    if (((window as MainWindow).frameSimulationMode.Template.FindName("lblSimulationStabilityStatus", (window as MainWindow).frameSimulationMode) as Label).Content.ToString() == "OK")
                    {
                        ((window as MainWindow).frameSimulationMode.Template.FindName("lblSimulationStabilityStatus", (window as MainWindow).frameSimulationMode) as Label).Background = new SolidColorBrush(System.Windows.Media.Colors.LawnGreen);
                    }
                    else if (((window as MainWindow).frameSimulationMode.Template.FindName("lblSimulationStabilityStatus", (window as MainWindow).frameSimulationMode) as Label).Content.ToString() == "NOT OK")
                    {
                      //  ((window as MainWindow).frameSimulationMode.Template.FindName("lblSimulationStabilityStatus", (window as MainWindow).frameSimulationMode) as Label).Background = new SolidColorBrush(System.Windows.Media.Colors.Red);
                    }


                    ((window as MainWindow).frameSimulationMode.Template.FindName("btnSimulationModeStability", (window as MainWindow).frameSimulationMode) as Button).IsEnabled = true;
                    if (Convert.ToString(((window as MainWindow).frameSimulationMode.Template.FindName("lblSimulationStabilityType", (window as MainWindow).frameSimulationMode) as Label).Content) == "Intact".ToUpper())
                    {
                        //((window as MainWindow).frameSimulationMode.Template.FindName("btnSimulationModeLongitudinal", (window as MainWindow).frameSimulationMode) as Button).IsEnabled = false;
                        ((window as MainWindow).frameSimulationMode.Template.FindName("btnSimulationModeCorrectiveAction", (window as MainWindow).frameSimulationMode) as Button).IsEnabled = false;
                        ((window as MainWindow).frameSimulationMode.Template.FindName("lblSimulationStabilityType", (window as MainWindow).frameSimulationMode) as Label).Background = new SolidColorBrush(System.Windows.Media.Colors.LawnGreen);
                        //((window as MainWindow).frameSimulationMode.Template.FindName("lblSimulationModeLongitudinalResult", (window as MainWindow).frameSimulationMode) as Label).Content = Convert.ToString(clsGlobVar.dtSimulationStabilitySummary.Rows[1]["Stability_Status"]).ToUpper();
                        //if (((window as MainWindow).frameSimulationMode.Template.FindName("lblSimulationModeLongitudinalResult", (window as MainWindow).frameSimulationMode) as Label).Content.ToString() == "OK")
                        //{
                        //    ((window as MainWindow).frameSimulationMode.Template.FindName("lblSimulationModeLongitudinalResult", (window as MainWindow).frameSimulationMode) as Label).Background = new SolidColorBrush(System.Windows.Media.Colors.LawnGreen);
                        //}
                        //else if (((window as MainWindow).frameSimulationMode.Template.FindName("lblSimulationModeLongitudinalResult", (window as MainWindow).frameSimulationMode) as Label).Content.ToString() == "NOT OK")
                        //{
                        //    ((window as MainWindow).frameSimulationMode.Template.FindName("lblSimulationModeLongitudinalResult", (window as MainWindow).frameSimulationMode) as Label).Background = new SolidColorBrush(System.Windows.Media.Colors.Red);
                        //}
                    }
                    else
                    {
                        //((window as MainWindow).frameSimulationMode.Template.FindName("btnSimulationModeLongitudinal", (window as MainWindow).frameSimulationMode) as Button).IsEnabled = false;
                        ((window as MainWindow).frameSimulationMode.Template.FindName("btnSimulationModeCorrectiveAction", (window as MainWindow).frameSimulationMode) as Button).IsEnabled = true;
                        ((window as MainWindow).frameSimulationMode.Template.FindName("lblSimulationStabilityType", (window as MainWindow).frameSimulationMode) as Label).Background = new SolidColorBrush(System.Windows.Media.Colors.Red);
                        // ((window as MainWindow).frameSimulationMode.Template.FindName("lblSimulationModeLongitudinalResult", (window as MainWindow).frameSimulationMode) as Label).IsEnabled = false;
                        //((window as MainWindow).frameSimulationMode.Template.FindName("lblSimulationModeLongitudinalResult", (window as MainWindow).frameSimulationMode) as Label).Content = "NA";
                    }
                }
            }
            dgCargoTanks.ItemsSource = Models.BO.clsGlobVar.dtSimulationCargoTanks.DefaultView;
            dgBallastTanks.ItemsSource = Models.BO.clsGlobVar.dtSimulationBallastTanks.DefaultView;
            dgFreshWaterTanks.ItemsSource = Models.BO.clsGlobVar.dtSimulationFreshWaterTanks.DefaultView;
            dgFuelOilTanks.ItemsSource = Models.BO.clsGlobVar.dtSimulationFuelOilTanks.DefaultView;
            dgMiscTanks.ItemsSource = Models.BO.clsGlobVar.dtSimulationMiscTanks.DefaultView;
            dgCompartments.ItemsSource = Models.BO.clsGlobVar.dtSimulationCompartments.DefaultView;


            //{
            //    DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
            //    string Err = "";
            //    string query = @"DELETE FROM tblFixedLoad_Simulation";
            //    DataTable veriableSimulation =  Models.BO.clsGlobVar.dtRealVariableItems;
            //    command.CommandText = query;
            //    command.CommandType = CommandType.Text;
            //    Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);

            //   SqlConnection    conn = clsSqlData.getConnection();
            //   SqlCommand sqlcmd = new SqlCommand();
            //   sqlcmd.Connection = conn;
            //   conn.Open();
            //   SqlTransaction   tran = conn.BeginTransaction();
            //   sqlcmd.Transaction = tran;
            //   sqlcmd.CommandText = "INSERT INTO tblFixedLoad_Simulation ([Load_Name],[Weight],[LCG],[TCG],[VCG],[Length],[Breadth],[Depth]) VALUES ( @Load_Name,@Weight,@LCG,@TCG,@VCG,@Length,@Breadth,@Depth) ";
            //   sqlcmd.Parameters.AddWithValue("@Load_Name", SqlDbType.VarChar);
            //   sqlcmd.Parameters.AddWithValue("@Weight", SqlDbType.Decimal);
            //   sqlcmd.Parameters.AddWithValue("@LCG", SqlDbType.Decimal);
            //   sqlcmd.Parameters.AddWithValue("@TCG", SqlDbType.Decimal);
            //   sqlcmd.Parameters.AddWithValue("@VCG", SqlDbType.Decimal);
            //   sqlcmd.Parameters.AddWithValue("@Length", SqlDbType.Decimal);
            //   sqlcmd.Parameters.AddWithValue("@Breadth", SqlDbType.Decimal);
            //   sqlcmd.Parameters.AddWithValue("@Depth", SqlDbType.Decimal);
              
            //    for (int i = 0; i < veriableSimulation.Rows.Count;i++ )
            //    {
                    
                    
            //        sqlcmd.Parameters["@Load_Name"].Value= veriableSimulation.Rows[i]["Load_Name"].ToString();
            //        sqlcmd.Parameters["@Weight"].Value=Convert.ToDecimal(veriableSimulation.Rows[i]["Weight"].ToString());
            //        sqlcmd.Parameters["@LCG"].Value=Convert.ToDecimal(veriableSimulation.Rows[i]["LCG"].ToString());
            //        sqlcmd.Parameters["@TCG"].Value=Convert.ToDecimal(veriableSimulation.Rows[i]["TCG"].ToString());
            //        sqlcmd.Parameters["@VCG"].Value= Convert.ToDecimal(veriableSimulation.Rows[i]["VCG"].ToString());
            //        sqlcmd.Parameters["@Length"].Value= Convert.ToDecimal(veriableSimulation.Rows[i]["Length"].ToString());
            //        sqlcmd.Parameters["@Breadth"].Value= Convert.ToDecimal(veriableSimulation.Rows[i]["Breadth"].ToString());
            //        sqlcmd.Parameters["@Depth"].Value= Convert.ToDecimal(veriableSimulation.Rows[i]["Depth"].ToString());

            //        int rows=sqlcmd.ExecuteNonQuery();
            //    }

            //    tran.Commit();
            //    if (conn.State == ConnectionState.Open)
            //    {
            //        conn.Close();
            //    }
            //}
            //Models.BO.clsGlobVar.dtSimulationVariableItems = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationModeVariableDetails");
            dgVariableItems.ItemsSource = Models.BO.clsGlobVar.dtSimulationVariableItems.DefaultView;
            dgWTRegion.ItemsSource = Models.BO.clsGlobVar.dtSimulationWTRegion.DefaultView;

            lblDisplacement.Content = Math.Round(Convert.ToDouble(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["Displacement"])).ToString();
            //decimal varoutdisp = Convert.ToDecimal(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["Displacement"]);
            //lblDisplacement.Content = varoutdisp.ToString("N");

            // lblGMT.Content = Math.Round(Convert.ToDouble(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["GMT"]), 1).ToString();
            decimal varoutGMT = Convert.ToDecimal(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["GMT"]);
            lblGMT.Content = varoutGMT.ToString("N3");

            //lblDraftAP.Content = Convert.ToString(Math.Round((Convert.ToDouble(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["Draft_AP"])), 2));
            decimal varoutAP = Convert.ToDecimal(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["Draft_AP"]);
            lblDraftAP.Content = varoutAP.ToString("N3");

            // lblDraftFP.Content = Convert.ToString(Math.Round((Convert.ToDouble(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["Draft_FP"])), 2));

            decimal varoutAFP = Convert.ToDecimal(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["Draft_FP"]);
            lblDraftFP.Content = varoutAFP.ToString("N3");
            // lblDraftAftMark.Content = Convert.ToString(Math.Round((Convert.ToDouble(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["Draft_Aft_Mark"])), 2));

            decimal varoutAFtmark = Convert.ToDecimal(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["Draft_Aft_Mark"]);
            lblDraftAftMark.Content = varoutAFtmark.ToString("N3");

            //lblDraftFwdMark.Content = Convert.ToString(Math.Round((Convert.ToDouble(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["Draft_Fore_Mark"])), 2));
            decimal varoutFwmark = Convert.ToDecimal(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["Draft_Fore_Mark"]);
            lblDraftFwdMark.Content = varoutFwmark.ToString("N3");
            // lblDraftMean.Content = Convert.ToString(Math.Round((Convert.ToDouble(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["Draft_Mean"])), 2));

            decimal varoutmean = Convert.ToDecimal(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["Draft_Mean"]);
            lblDraftMean.Content = varoutmean.ToString("N3");

            // lblSONARDOME.Content = Convert.ToString(Math.Round((Convert.ToDouble(Models.BO.clsGlobVar.dtSimulationDrafts.Rows[0]["Draft_Sonar_Dome"])), 2));
            decimal varsondome = Convert.ToDecimal(Models.BO.clsGlobVar.dtSimulationDrafts.Rows[0]["Draft_Sonar_Dome"]);
            lblSONARDOME.Content = varsondome.ToString("N3");
            //lblPROPELLER.Content = Convert.ToString(Math.Round((Convert.ToDouble(Models.BO.clsGlobVar.dtSimulationDrafts.Rows[0]["Draft_Propeller"])), 2));
            decimal varPropeller = Convert.ToDecimal(Models.BO.clsGlobVar.dtSimulationDrafts.Rows[0]["Draft_Propeller"]);
            lblPROPELLER.Content = varPropeller.ToString("N3");


            // Hydrostatics after calculation
            decimal KGS = Convert.ToDecimal(Models.BO.clsGlobVar.dtSimulationDrafts.Rows[0]["KG(Solid)"]);
            lblKG.Content = KGS.ToString("N3");
            decimal varLCG = Convert.ToDecimal(Models.BO.clsGlobVar.dtSimulationDrafts.Rows[0]["LCG"]);
            lblLCG.Content = varLCG.ToString("N3");
            decimal varTpc = Convert.ToDecimal(Models.BO.clsGlobVar.dtSimulationDrafts.Rows[0]["TPC"]);
            lblTPC.Content = varTpc.ToString("N3");
            decimal VarKGF = Convert.ToDecimal(Models.BO.clsGlobVar.dtSimulationDrafts.Rows[0]["KG(Fluid)"]);
            lblKGF.Content = VarKGF.ToString("N3");
            decimal val = Convert.ToDecimal(Models.BO.clsGlobVar.dtSimulationDrafts.Rows[0]["FSC"]);
            lblFSC.Content = val.ToString("N3");
            decimal varMCT = Convert.ToDecimal(Models.BO.clsGlobVar.dtSimulationDrafts.Rows[0]["MCT"]);
            lblMTC.Content = varMCT.ToString("0");





            // Heel and trim for only assignig values after calculation click

            if (Convert.ToDouble(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["Heel"]) < 0)
            {
                lblHeel.Content = "PORT";
                Double abs = (Convert.ToDouble(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["Heel"]));
                //  lblHeel.Content = Convert.ToString((-1) * abs);

                decimal varheel1 = Convert.ToDecimal(Convert.ToString((-1) * abs));
                lblHeel.Content = varheel1.ToString("N3");
            }
            else if (Convert.ToDouble(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["Heel"]) > 0)
            {
                lblHeel.Content = "STBD";
                //lblHeel.Content = (Convert.ToString(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["Heel"]));
                decimal varheel1 = Convert.ToDecimal(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["Heel"]);
                //decimal varheel1 = Convert.ToDecimal(Convert.ToString((-1) * heel2));
                lblHeel.Content = varheel1.ToString("N3");
            }
            else
            {
                // lblHeel.Content = (Convert.ToString(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["Heel"]));
                lblHeel.Content = " ";
                decimal heel3 = Convert.ToDecimal(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["Heel"]);
                lblHeel.Content = heel3.ToString("N3");
            }


            if (Convert.ToDouble(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["TRIM"]) > 0)
            {
                lblTrim.Content = "FWD";
                Double abs = (Convert.ToDouble(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["TRIM"]));
                decimal trim1 = Convert.ToDecimal((-1) * abs);
                lblTrim.Content = abs.ToString("N3");
            }
            else if (Convert.ToDouble(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["TRIM"]) < 0)
            {
                lblTrim.Content = "AFT";
                //lblTrim.Content = (Convert.ToString(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["TRIM"]));
                decimal trim2 = Convert.ToDecimal(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["TRIM"]);
                decimal trim1 = Convert.ToDecimal((-1) * trim2);
                lblTrim.Content = trim1.ToString("N3");
            }
            else
            {
                lblTrim.Content = " ";
                //alblTrim.Content = (Convert.ToString(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["TRIM"]));
                decimal trim3 = Convert.ToDecimal(Models.BO.clsGlobVar.dtSimulationEquillibriumValues.Rows[0]["TRIM"]);
                lblTrim.Content = trim3.ToString("N3");
            }

            if (Convert.ToString(clsGlobVar.dtSimulationStabilitySummary.Rows[0]["Stability_Type"]).ToUpper() == "INTACT")
            {
                //lblMaxBM.Content = Models.BO.clsGlobVar.dtSimulationSFandBMMax.Rows[0]["Max_BM"].ToString();
                //lblMaxSF.Content = Models.BO.clsGlobVar.dtSimulationSFandBMMax.Rows[0]["Max_SF"].ToString();
                //lblFrameBM.Content = Models.BO.clsGlobVar.dtSimulationSFandBMMax.Rows[0]["Distance_BM"].ToString();
                //lblFrameSF.Content = Models.BO.clsGlobVar.dtSimulationSFandBMMax.Rows[0]["Distance_SF"].ToString();
            }
            else
            {
                //lblMaxBM.Content = "NA";
                //lblMaxSF.Content = "NA";
                //lblFrameBM.Content = "NA";
                //lblFrameSF.Content = "NA";
            }


            Refresh3dNew();
            AddHatchProfile();
            AddHatchDeckPlanA();
            AddHatchDeckPlanB();
            //AddHatchDeckPlanC();
        }


        private void bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // pbCalculation.Value = e.ProgressPercentage;

        }
       
        private void bgWorker_Do_Work(object sender, DoWorkEventArgs e)
        {
            try
            {
                string user = "dbo";
                int res;
                DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
                string Err = "";
                command.CommandText = "spCal_SimulationMode_Stability";
                command.CommandType = CommandType.StoredProcedure;
                DbParameter param1 = Models.DAL.clsDBUtilityMethods.GetParameter();
                param1.DbType = DbType.String;
                param1.ParameterName = "@User";
                command.Parameters.Add(param1);

                DbParameter param2 = Models.DAL.clsDBUtilityMethods.GetParameter();
                param2.DbType = DbType.Int16;
                param2.Direction = ParameterDirection.Output;
                param2.ParameterName = "@Stability_Calculation_Result";
                command.Parameters.Add(param2);

                param1.Value = user;
                Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);
                Models.BO.clsGlobVar.CalculationResult = Convert.ToInt32(command.Parameters[1].Value);
                isCalculationRunning = false;
                //if (Models.BO.clsGlobVar.CalculationResult == 1)
                {
                    Models.TableModel.SimulationModeData();
                    Models.TableModel.SimulationModePercentFill();
                   //Models.TableModel.simulationmodeCorrectiveFill();

                }
            }
            catch
            {
            }
        }
        #endregion BackgroundWorker
        public string pdfsavename = string.Empty;
        private void btnSaveLoadingCondition_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string loadingName = string.Empty ;

                CustomeInpute objCustIp = new CustomeInpute();
                objCustIp.ShowDialog();

                loadingName = objCustIp.loading_Name;
                pdfsavename= objCustIp.loading_Name; 
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                string path = "";
                //  string path = System.IO.Directory.GetCurrentDirectory();


                path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\SMData\\" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm")+ " " + loadingName;

                    Directory.CreateDirectory(path);
                    List<Tanks> liTanks = new List<Tanks>();
                    Tanks objFresh = new Tanks();
                    DataTable dt = new DataTable();
                    dt = Models.BO.clsGlobVar.dtSimulationModeAllTanks;
                    //    dt = dsSMFreshWaterTank.Tables[0];
                    foreach (DataRow dr in dt.Rows)
                    {
                        try
                        {

                            int tank_ID = Convert.ToInt32(dr[0].ToString());
                            string group = Convert.ToString(dr[1]);
                            string tank_Name = Convert.ToString(dr[2]);
                            decimal maxvolume = Convert.ToDecimal(Convert.ToString(dr[3]));
                            decimal soundinglevel = Convert.ToDecimal(Convert.ToString(dr[4]));
                            decimal volume = Convert.ToDecimal(Convert.ToString(dr[5]));
                            decimal sG = Convert.ToDecimal(Convert.ToString(dr[6]));
                            decimal weight = Convert.ToDecimal(Convert.ToString(dr[7]));
                            decimal percent_Full = Convert.ToDecimal(Convert.ToString(dr[8]));
                            decimal lcg = Convert.ToDecimal(Convert.ToString(dr[9]));
                            decimal tcg = Convert.ToDecimal(Convert.ToString(dr[10]));
                            decimal vcg = Convert.ToDecimal(Convert.ToString(dr[11]));
                            decimal fSM = Convert.ToDecimal(Convert.ToString(dr[12]));
                            bool isDamaged = Convert.ToBoolean(dr[13]);
                            int status = Convert.ToInt16(Convert.ToString(dr[14]));
                            int fsmtype = Convert.ToInt16(Convert.ToString(dr[15]));
                            bool isvisible = Convert.ToBoolean(dr[16]);
                            liTanks.Add(new Tanks
                            {
                                Tank_ID = tank_ID,
                                Group = group,
                                Tank_Name = tank_Name,
                                Max_Volume = maxvolume,
                                Sounding_Level = soundinglevel,
                                Volume = volume,
                                SG = sG,
                                Weight = weight,
                                Percent_Full = percent_Full,
                                LCG = lcg,
                                TCG = tcg,
                                VCG = vcg,
                                FSM = fSM,
                                IsDamaged = isDamaged,
                                Status = status,
                                FSMType = fsmtype,
                                IsVisible = isvisible
                            });
                        }
                        catch
                        {
                        }

                    }

                    string fn = path + "\\Tanks.cnd";
                    FileStream fs = new FileStream(fn, FileMode.Create, FileAccess.Write, FileShare.None);
                    BinaryFormatter objFormat = new BinaryFormatter();
                    objFormat.Serialize(fs, liTanks);
                    fs.Close();

                    List<FixedItems> listFixedLoads = new List<FixedItems>();
                    FixedItems objDeck1 = new FixedItems();
                    dt = Models.BO.clsGlobVar.dtSimulationVariableItems;

                    foreach (DataRow dr in dt.Rows)
                    {
                        listFixedLoads.Add(new FixedItems
                        {
                            Load_Id = Convert.ToInt32(dr[0].ToString()),
                            Load_Name = dr[1].ToString(),
                            Weight = Convert.ToDecimal(dr[2].ToString()),
                            LCG = Convert.ToDecimal(dr[3].ToString()),
                            TCG = Convert.ToDecimal(dr[4].ToString()),
                            VCG = Convert.ToDecimal(dr[5].ToString()),
                            Length = Convert.ToDecimal(dr[6].ToString()),
                            Breadth = Convert.ToDecimal(dr[7].ToString()),
                            Depth = Convert.ToDecimal(dr[8].ToString()),
                            //FSMType = dr[9].ToString(),
                            //FSM = Convert.ToDecimal(dr[10].ToString()),
                         
                        });
                    }
                    fn = path + "\\FixedLoads.cnd";

                    fs = new FileStream(fn, FileMode.Create, FileAccess.Write, FileShare.None);
                    objFormat = new BinaryFormatter();
                    objFormat.Serialize(fs, listFixedLoads);
                    fs.Close();
                    Mouse.OverrideCursor = null;
                    if (loadingName != string.Empty)
                    {
                        System.Windows.MessageBox.Show("Loading Condition Saved Successfully");
                    }
                    else
                    { System.Windows.MessageBox.Show("Loading Condition Saved Without name"); }
              
            }
            catch
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void btnImportRealModeData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                string query = "", Err = "";
                for (int i = 0; i < Models.BO.clsGlobVar.dtRealModeAllTanks.Rows.Count; i++)
                {
                    if (Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[i]["IsDamaged"].ToString() == "False")
                    {

                        query += @" update tblSimulationMode_Tank_Status set [Volume]=" + Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[i]["Volume"] + ", [IsDamaged]=" + 0 + ", SG=" + Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[i]["SG"]  + ",Weight=" + Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[i]["Weight"] + " where Tank_ID=" + (i + 1) + @"
                                update tblSimulationMode_Loading_Condition set [Volume]=" + Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[i]["Volume"] + ", [IsDamaged]=" + 0 + ", SG=" + Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[i]["SG"] + ",Status=" + 0 + ",Weight=" + Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[i]["Weight"] + " where Tank_ID=" + (i + 1);
                    }
                    else if (Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[i]["IsDamaged"].ToString() == "True")
                    {
                        query += @" update tblSimulationMode_Tank_Status set [Volume]=" + Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[i]["Volume"] + ", [IsDamaged]=" + 1 + ", SG=" + Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[i]["SG"] + ",Weight=" + Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[i]["Weight"] + " where Tank_ID=" + (i + 1) + @"
                                update tblSimulationMode_Loading_Condition set [Volume]=" + Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[i]["Volume"] + ", [IsDamaged]=" + 1 + ", SG=" + Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[i]["SG"] + ",Status=" + 1 + ",  Weight=" + Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[i]["Weight"] + " where Tank_ID=" + (i + 1);
                
                    }
                }

                DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
                command.CommandText = query;
                command.CommandType = CommandType.Text;
                Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);

                query = @" Delete from tblFixedLoad_Simulation";
                command = Models.DAL.clsDBUtilityMethods.GetCommand();
                command.CommandText = query;
                command.CommandType = CommandType.Text;
                Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);
                query = "";
                for (int i = 0; i < Models.BO.clsGlobVar.dtRealVariableItems.Rows.Count; i++)
                {
                    query += @" insert into tblFixedLoad_Simulation  ([Load_Name]
      ,[Weight]
      ,[LCG]
      ,[TCG]
      ,[VCG]
      ,[Length]
      ,[Breadth]
      ,[Depth]) VALUES('" + Models.BO.clsGlobVar.dtRealVariableItems.Rows[i]["Load_Name"] + "'," + Models.BO.clsGlobVar.dtRealVariableItems.Rows[i]["Weight"] + "," + Models.BO.clsGlobVar.dtRealVariableItems.Rows[i]["LCG"] + @"
                                  ," + Models.BO.clsGlobVar.dtRealVariableItems.Rows[i]["TCG"] + @"
                                  ," + Models.BO.clsGlobVar.dtRealVariableItems.Rows[i]["VCG"] + @"
                                  ," + Models.BO.clsGlobVar.dtRealVariableItems.Rows[i]["Length"] + @"
                                  ," + Models.BO.clsGlobVar.dtRealVariableItems.Rows[i]["Breadth"] + @"
                                  ," + Models.BO.clsGlobVar.dtRealVariableItems.Rows[i]["Depth"] + ")";
                }
                command = Models.DAL.clsDBUtilityMethods.GetCommand();
                command.CommandText = query;
                command.CommandType = CommandType.Text;
                Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);

                Models.TableModel.SimulationModeData();
                dgCargoTanks.ItemsSource = Models.BO.clsGlobVar.dtSimulationCargoTanks.DefaultView;
                dgBallastTanks.ItemsSource = Models.BO.clsGlobVar.dtSimulationBallastTanks.DefaultView;
                dgFreshWaterTanks.ItemsSource = Models.BO.clsGlobVar.dtSimulationFreshWaterTanks.DefaultView;
                dgFuelOilTanks.ItemsSource = Models.BO.clsGlobVar.dtSimulationFuelOilTanks.DefaultView;
                dgMiscTanks.ItemsSource = Models.BO.clsGlobVar.dtSimulationMiscTanks.DefaultView;
                dgCompartments.ItemsSource = Models.BO.clsGlobVar.dtSimulationCompartments.DefaultView;

                dgVariableItems.ItemsSource = Models.BO.clsGlobVar.dtSimulationVariableItems.DefaultView;
                Mouse.OverrideCursor = null;
            }
            catch
            {
                Mouse.OverrideCursor = null;
            }

        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            dispatcherTimer.Stop();
        }

        // True when models are loaded but the 3D tab was not yet visible at load time.
        private bool _pendingFit = false;

        private void viewPort3d_Loaded(object sender, RoutedEventArgs e)
        {
            viewPort3d.EffectsManager = new DefaultEffectsManager();
        }

        private void ViewPort3d_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width > 0 && e.NewSize.Height > 0 && _pendingFit)
            {
                _pendingFit = false;
                viewPort3d.SizeChanged -= ViewPort3d_SizeChanged;
                // Small delay so the viewport finishes layout before fitting
                var t = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };
                t.Tick += (s, ev) => { t.Stop(); FitCameraToScene(); };
                t.Start();
            }
        }

        /// <summary>
        /// Fits the camera to show all loaded 3-D geometry, then locks the rotation/zoom
        /// pivot to the computed scene centre so the model never drifts off screen.
        /// </summary>
        private void FitCameraToScene()
        {
            if (viewPort3d?.EffectsManager == null) return;

            // If the 3D tab is not yet visible the viewport has no size — defer.
            if (viewPort3d.ActualWidth <= 0 || viewPort3d.ActualHeight <= 0)
            {
                _pendingFit = true;
                viewPort3d.SizeChanged -= ViewPort3d_SizeChanged; // remove old handler if any
                viewPort3d.SizeChanged += ViewPort3d_SizeChanged;
                return;
            }

            // -- Step 1: compute scene bounding box centre (CPU-side, no GPU needed) --
            var bounds = ViewportExtensions.FindBounds3D(viewPort3d);

            if (!bounds.IsEmpty)
            {
                // Lock rotation/zoom pivot to exact scene centre BEFORE calling ZoomExtents
                // so that even mid-animation rotation is around the correct point.
                var centre = new System.Windows.Media.Media3D.Point3D(
                    bounds.X + bounds.SizeX * 0.5,
                    bounds.Y + bounds.SizeY * 0.5,
                    bounds.Z + bounds.SizeZ * 0.5);
                viewPort3d.FixedRotationPoint        = centre;
                viewPort3d.FixedRotationPointEnabled = true;

                // -- Step 2: smooth camera fly-in to fit all geometry --
                viewPort3d.ZoomExtents(animationTime: 400);
            }
            else
            {
                // Fallback: instant fit so camera.Position/LookDirection are final values,
                // then derive centre from camera state (LookDirection = target − position).
                viewPort3d.ZoomExtents(animationTime: 0);
                var cam = viewPort3d.Camera as HelixToolkit.Wpf.SharpDX.PerspectiveCamera;
                if (cam != null && cam.LookDirection.Length > 0)
                {
                    var centre = new System.Windows.Media.Media3D.Point3D(
                        cam.Position.X + cam.LookDirection.X,
                        cam.Position.Y + cam.LookDirection.Y,
                        cam.Position.Z + cam.LookDirection.Z);
                    viewPort3d.FixedRotationPoint        = centre;
                    viewPort3d.FixedRotationPointEnabled = true;
                }
            }
        }

        private void dgVariableItems_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            //Models.TableModel.Write_Log(" START : dgVariableItems_BeginningEdit");
            try
            {
                header = e.Column.Header.ToString();
                index = e.Row.GetIndex();
                if (e.Column.GetType().ToString() == "System.Windows.Controls.DataGridTextColumn")
                {
                    //index = e.Row.GetIndex();
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
                Models.TableModel.Write_Log(" END : dgVariableItems_BeginningEdit");
            }
            catch
            {

            }
        }

        private void dgVariableItems_CurrentCellChanged(object sender, EventArgs e)
        {
            //Models.TableModel.Write_Log(" START : dgVariableItems_CurrentCellChanged");
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


                    string query = "update tblFixedLoad_Simulation set Load_Name='" + LoadName + "',Weight=" + weight + ",LCG=" + LCG + ",TCG=" + TCG + ",VCG=" + VCG + @"
                            ,Length=" + length + ",Breadth=" + breadth + ",Depth=" + depth + " where Load_Id=" + LoadId;
                    command = Models.DAL.clsDBUtilityMethods.GetCommand();
                    command.CommandText = query;
                    command.CommandType = CommandType.Text;
                    Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);
                    index = -1;
                    LoadId = 0;
                   // Models.TableModel.Write_Log(" END : dgVariableItems_CurrentCellChanged");

                }
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
                canvas2DPlanB.Children.RemoveRange(1, canvas2DPlanB.Children.Count - 1);

                foreach (var kvp in clsGlobVar.CoordinatePlanB.Tanks)
                {
                    int tankId = kvp.Value.Tank_ID;
                    if (tankId <= 0 || tankId > clsGlobVar.dtSimulationModeAllTanks.Rows.Count) continue;

                    DataRow tankRow = clsGlobVar.dtSimulationModeAllTanks.Rows[tankId - 1];
                    decimal percent = Convert.ToDecimal(tankRow["Percent_Full"]);
                    bool isDamaged = tankRow["IsDamaged"].ToString() == Boolean.TrueString ?  true :false;
                 
                    DrawHatchDeckPlan(canvas2DPlanB, tankId, percent, kvp.Value.X, kvp.Value.Y, System.Windows.Media.Color.FromArgb(180, 194, 214, 154));
                    UpdateRenderTransform(canvas2DPlanB);
                }

               
            }
            catch //(Exception ex)
            {
              //  System.Windows.MessageBox.Show(ex.Message);
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

                foreach (var kvp in clsGlobVar.CoordinatePlanA.Tanks)
                {
                    int tankId = kvp.Value.Tank_ID;
                    DataRow tankRow = clsGlobVar.SimulationFilling
                                           .AsEnumerable()
                                           .FirstOrDefault(r => Convert.ToInt32(r["Tank_ID"]) == tankId);
                    if (tankRow == null) continue;
                    decimal percent = Convert.ToDecimal(tankRow["Percent_Full"]);
                    DataRow tankDamageRow = clsGlobVar.dtSimulationLoadingSummary.Rows[tankId - 1];
                   
                    bool isDamaged = tankDamageRow["IsDamaged"].ToString() == Boolean.TrueString
                                    ? true : false;
                    System.Windows.Media.Color tankColor = isDamaged
                       ? System.Windows.Media.Color.FromArgb(255, 218, 97, 78) : System.Windows.Media.Color.FromArgb(180, 194, 214, 154);
                    DrawHatchDeckPlan(canvas2DPlanA, tankId, percent, kvp.Value.X, kvp.Value.Y, tankColor);
                    UpdateRenderTransform(canvas2DPlanA);
                }

               
            }
            catch //(Exception ex)
            {
               // System.Windows.MessageBox.Show(ex.Message);
            }
        }

        // ---- dead code kept for reference, replaced by the foreach above ----
      

        /// <summary>
        /// Add Hatch Filling of Tanks/Compartments to DeckPlans Canvas as per TankID 
        /// </summary>
       

        /// <summary>
        /// Add Hatch Filling of Tanks/Compartments to Profile Canvas as per TankID 
        /// </summary>
        public void AddHatchProfile()
        {
            try
            {
                canvas2DProfile.Children.RemoveRange(1, canvas2DProfile.Children.Count - 1);

             
                foreach (var kvp in clsGlobVar.ProfileCoordinate.Profiles)
                {
                    int tankId = kvp.Key;

                    DataRow tankRow = clsGlobVar.SimulationFilling
                                          .AsEnumerable()
                                          .FirstOrDefault(r => Convert.ToInt32(r["Tank_ID"]) == tankId);
                    if (tankRow == null) continue;

                    decimal percent = Convert.ToDecimal(tankRow["Percent_Full"]);
                    DataRow tankDamageRow = clsGlobVar.dtSimulationLoadingSummary.Rows[tankId - 1];
                    bool isDamaged  = tankDamageRow["IsDamaged"].ToString() == Boolean.TrueString
                                    ? true : false;
                    System.Windows.Media.Color tankColor = isDamaged
                        ? System.Windows.Media.Color.FromArgb(255, 218, 97, 78)
                        : System.Windows.Media.Color.FromArgb(180, 194, 214, 154);
                    DrawHatchProfile(canvas2DProfile, tankId, percent, kvp.Value.X, kvp.Value.Y, tankColor);
                    UpdateRenderTransform(canvas2DProfile);
                    DrawTrimLine();
                }
                
                
            }
            catch //(Exception ex)
            {
                //MessageBox.Show(ex.ToString());
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
            xAP = -15000;
            yAP = 6250 + 1157.40 + ((yAvg - angle) * 1000);
            xFP = 350000;
            yFP = 6250 + 1157.40 + ((yAvg + angle) * 1000);

            //@MT Code changed for Ship Profile Trim Line Should be Steady :END

            // Remove any previously drawn trim line before adding a new one
            var existing = canvas2DProfile.Children.OfType<System.Windows.Shapes.Line>()
                .FirstOrDefault(l => l.Tag as string == "TrimLine");
            if (existing != null)
                canvas2DProfile.Children.Remove(existing);

            System.Windows.Shapes.Line trimLine = new System.Windows.Shapes.Line();
            trimLine.Tag = "TrimLine";
            trimLine.X1 = xAP;
            trimLine.Y1 = yAP;
            trimLine.X2 = xFP;
            trimLine.Y2 = yFP;
            trimLine.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 0, 255));
            trimLine.StrokeThickness = 200;
            canvas2DProfile.Children.Add(trimLine);

        }

        // <summary>
        /// Logic for Adding hatches to Show Filling on Profile plan as per Percentage
        /// </summary>
        /// <param name="canvasTwoD">Canvas on which hatch is to be created</param> 
        /// <param name="Tank_ID">TankID of Tank/Compartment.</param> 
        /// <param name="percent">Percentage Filling</param> 
        /// <param name="xx">Collection of all the x coordinates for drawing the hatch.</param> 
        /// <param name="yy">Collection of all the y coordinates for drawing the hatch.</param> 
        /// <param name="color">Hatch Color </param> 
        public void DrawHatchProfile(Canvas canvasTwoD, int Tank_ID, decimal percent, double[] xx, double[] yy, System.Windows.Media.Color color)
        {
            try
            {
                if (percent > 0)
                {
                    double d = yy[3] - yy[2];
                    double Fill = Convert.ToInt32(percent) * (d / 100);

                    // Profile canvas
                    {
                        System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                        p.Stroke = System.Windows.Media.Brushes.Black;
                        SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                        mySolidColorBrush.Color = color;
                        p.Fill = mySolidColorBrush;
                        PointCollection pointCollection = new PointCollection();
                        pointCollection.Add(new System.Windows.Point(xx[1], yy[1]));
                        pointCollection.Add(new System.Windows.Point(xx[2], yy[2]));
                        pointCollection.Add(new System.Windows.Point(xx[2], yy[2] + Fill));
                        pointCollection.Add(new System.Windows.Point(xx[1], yy[2] + Fill));
                        p.Points = pointCollection;
                        canvasTwoD.Children.Add(p);
                    }
                }
            }
            catch
            {
            }
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
                if ((Convert.ToBoolean(Models.BO.clsGlobVar.dtSimulationModeAllTanks.Rows[Tank_ID - 1]["IsDamaged"]) == true) && (Convert.ToBoolean(Models.BO.clsGlobVar.dtSimulationModeAllTanks.Rows[Tank_ID - 1]["Status"]) == Convert.ToBoolean(1)))
                {

                    #region Plan
                    {
                        System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                        p.Stroke = System.Windows.Media.Brushes.Black;

                        SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                         mySolidColorBrush.Color = System.Windows.Media.Color.FromArgb(220, 255, 0, 0); ;
                        p.Fill = mySolidColorBrush;

                        System.Windows.Point[] point = new System.Windows.Point[15];
                        PointCollection pointCollection = new PointCollection();
                        if (canvasTwoD == canvas2DPlanA)
                        {
                            for (int index = 1; index <= 24; index++)
                            {

                                pointCollection.Add(new System.Windows.Point(xx[index], yy[index]));

                            }
                        }
                        else
                        {
                            for (int index = 1; index <= 13; index++)
                            {

                                pointCollection.Add(new System.Windows.Point(xx[index], yy[index]));

                            }
                        }
                        p.Points = pointCollection;
                        canvasTwoD.Children.Add(p);
                    }
                    #endregion

                    #region PlanALL
                    {

                        if (percent >= 0 && percent <= 100)
                        {
                            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                            p.Stroke = System.Windows.Media.Brushes.Black;

                            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                            mySolidColorBrush.Color = System.Windows.Media.Color.FromArgb(220, 255, 0, 0);
                            p.Fill = mySolidColorBrush;

                            System.Windows.Point[] point = new System.Windows.Point[15];
                            PointCollection pointCollection = new PointCollection();


                            if (canvasTwoD == canvas2DPlanA)
                            {
                                for (int index = 1; index <= 24; index++)
                                {

                                    pointCollection.Add(new System.Windows.Point(xx[index] - 9862, yy[index] - 139500));

                                }
                            }
                            else if (canvasTwoD == canvas2DPlanB)
                            {
                                for (int index = 1; index <= 13; index++)
                                {

                                    pointCollection.Add(new System.Windows.Point(xx[index] - 10537, yy[index] - 94085));

                                }
                            }

                          
                          
                        }
                    }
                    #endregion

                    percent = 0;
                }
                else

                    if (percent > 0 && percent <= 101)
                    {
                        //percent = 100;
                        #region Plan
                        {
                            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                            p.Stroke = System.Windows.Media.Brushes.Black;

                            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                            mySolidColorBrush.Color = color;
                            p.Fill = mySolidColorBrush;

                            System.Windows.Point[] point = new System.Windows.Point[15];
                            PointCollection pointCollection = new PointCollection();
                            if (canvasTwoD == canvas2DPlanA)
                            {
                                for (int index = 1; index <= 24; index++)
                                {

                                    pointCollection.Add(new System.Windows.Point(xx[index], yy[index]));

                                }
                            }
                            else
                            {
                                for (int index = 1; index <= 13; index++)
                                {

                                    pointCollection.Add(new System.Windows.Point(xx[index], yy[index]));

                                }
                            }
                            p.Points = pointCollection;
                            canvasTwoD.Children.Add(p);
                        }
                        #endregion

                        #region PlanALL
                        {

                            if (percent > 0 && percent <= 100)
                            {
                            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                            p.Stroke = System.Windows.Media.Brushes.Black;

                            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                            mySolidColorBrush.Color = color;
                            p.Fill = mySolidColorBrush;

                            System.Windows.Point[] point = new System.Windows.Point[15];
                            PointCollection pointCollection = new PointCollection();


                            if (canvasTwoD == canvas2DPlanA)
                            {
                                for (int index = 1; index <= 24; index++)
                                {

                                    pointCollection.Add(new System.Windows.Point(xx[index]-9862, yy[index]-139500));

                                }
                            }
                            else if (canvasTwoD == canvas2DPlanB)
                            {
                                for (int index = 1; index <= 13; index++)
                                {

                                    pointCollection.Add(new System.Windows.Point(xx[index]-10537, yy[index]-94085));

                                }
                            }
                            
                            }
                        }
                        #endregion

                        percent = 0;
                    }
                    else if (percent == 0)
                    {
                        //System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                        //p.Stroke = System.Windows.Media.Brushes.Black;

                        //SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                        //mySolidColorBrush.Color = System.Windows.Media.Color.FromArgb(0, 255, 255, 255);
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
            catch
            {
            }
        }

        private void Refresh3dNew()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Refresh3dNew START - scene3D null? {scene3D == null}, viewport null? {viewPort3d == null}, EffectsManager null? {viewPort3d?.EffectsManager == null}");
                if (scene3D == null) { System.Diagnostics.Debug.WriteLine("scene3D is NULL - aborting"); return; }
                scene3D.Children.Clear();

                // Build name→path dictionaries once per folder (avoids O(tanks×files) inner loops)
                var freshWaterFiles   = BuildTankFileDictionary(folderPath + "\\FreshWaterTank\\");
                var freshWater25Files = BuildTankFileDictionary(folderPath + "\\FreshWaterTank25\\");
                var freshWater50Files = BuildTankFileDictionary(folderPath + "\\FreshWaterTank50\\");
                var freshWater75Files = BuildTankFileDictionary(folderPath + "\\FreshWaterTank75\\");

                var ballastFiles   = BuildTankFileDictionary(folderPath + "\\BallastTank\\");
                var ballast25Files = BuildTankFileDictionary(folderPath + "\\BallastTank25\\");
                var ballast50Files = BuildTankFileDictionary(folderPath + "\\BallastTank50\\");
                var ballast75Files = BuildTankFileDictionary(folderPath + "\\BallastTank75\\");

                var fuelOilFiles   = BuildTankFileDictionary(folderPath + "\\FuelOilTank\\");
                var fuelOil25Files = BuildTankFileDictionary(folderPath + "\\FuelOilTank25\\");
                var fuelOil50Files = BuildTankFileDictionary(folderPath + "\\FuelOilTank50\\");
                var fuelOil75Files = BuildTankFileDictionary(folderPath + "\\FuelOilTank75\\");

                var cargoFiles   = BuildTankFileDictionary(folderPath + "\\CargoTank\\");
                var cargo25Files = BuildTankFileDictionary(folderPath + "\\CargoTank25\\");
                var cargo50Files = BuildTankFileDictionary(folderPath + "\\CargoTank50\\");
                var cargo75Files = BuildTankFileDictionary(folderPath + "\\CargoTank75\\");

                var dieselFiles   = BuildTankFileDictionary(folderPath + "\\DieselOilTank\\");
                var diesel25Files = BuildTankFileDictionary(folderPath + "\\DieselOilTank25\\");
                var diesel50Files = BuildTankFileDictionary(folderPath + "\\DieselOilTank50\\");
                var diesel75Files = BuildTankFileDictionary(folderPath + "\\DieselOilTank75\\");

                var miscFiles   = BuildTankFileDictionary(folderPath + "\\MiscTank\\");
                var misc25Files = BuildTankFileDictionary(folderPath + "\\MiscTank25\\");
                var misc50Files = BuildTankFileDictionary(folderPath + "\\MiscTank50\\");

                // Fresh water tanks
                for (int i = 0; i < 2; i++)
                {
                    try
                    {
                        string stats = Convert.ToString((dgFreshWaterTanks.Items[i] as DataRowView)["IsDamaged"]);
                        string Name = Convert.ToString((dgFreshWaterTanks.Items[i] as DataRowView)["Tank_Name"]).Split('.')[0].Replace("/", "");
                        if (!freshWaterFiles.TryGetValue(Name, out string baseFile)) continue;
                        TankNameForPercentage = Name;
                        Getpercentage();
                        Element3D device3D = ResolveTankModel(Name, baseFile, stats == "True", PercentageFill,
                            freshWater25Files, freshWater50Files, freshWater75Files, 180, 0, 0, 100);
                        if (device3D != null) { device3D.Tag = Name; scene3D.Children.Add(device3D); }
                    }
                    catch { }
                }

                // Ballast tanks
                for (int i = 0; i < 51; i++)
                {
                    string stats = Convert.ToString((dgBallastTanks.Items[i] as DataRowView)["IsDamaged"]);
                    string Name = Convert.ToString((dgBallastTanks.Items[i] as DataRowView)["Tank_Name"]).Replace("/", "");
                    if (!ballastFiles.TryGetValue(Name, out string baseFile)) continue;
                    TankNameForPercentage = Name;
                    Getpercentage();
                    Element3D device3D = ResolveTankModel(Name, baseFile, stats == "True", PercentageFill,
                        ballast25Files, ballast50Files, ballast75Files, 180, 0, 200, 0);
                    if (device3D != null) { device3D.Tag = Name; scene3D.Children.Add(device3D); }
                }

                // Fuel oil tanks
                for (int i = 0; i < 13; i++)
                {
                    string stats = Convert.ToString((dgFuelOilTanks.Items[i] as DataRowView)["IsDamaged"]);
                    string Name = Convert.ToString((dgFuelOilTanks.Items[i] as DataRowView)["Tank_Name"]).Split('.')[0].Replace("/", "");
                    if (!fuelOilFiles.TryGetValue(Name, out string baseFile)) continue;
                    TankNameForPercentage = Name;
                    Getpercentage();
                    Element3D device3D = ResolveTankModel(Name, baseFile, stats == "True", PercentageFill,
                        fuelOil25Files, fuelOil50Files, fuelOil75Files, 200, 185, 92, 0);
                    if (device3D != null) { device3D.Tag = Name; scene3D.Children.Add(device3D); }
                }

                // Cargo tanks
                for (int i = 0; i < 9; i++)
                {
                    string stats = Convert.ToString((dgCargoTanks.Items[i] as DataRowView)["IsDamaged"]);
                    string Name = Convert.ToString((dgCargoTanks.Items[i] as DataRowView)["Tank_Name"]).Split('.')[0].Replace("/", "");
                    if (!cargoFiles.TryGetValue(Name, out string baseFile)) continue;
                    TankNameForPercentage = Name;
                    Getpercentage();
                    Element3D device3D = ResolveTankModel(Name, baseFile, stats == "True", PercentageFill,
                        cargo25Files, cargo50Files, cargo75Files, 200, 185, 92, 0);
                    if (device3D != null) { device3D.Tag = Name; scene3D.Children.Add(device3D); }
                }

                // Diesel oil tanks
                for (int i = 0; i < 3; i++)
                {
                    string stats = Convert.ToString((dgFuelOilTanks.Items[i] as DataRowView)["IsDamaged"]);
                    string Name = Convert.ToString((dgFuelOilTanks.Items[i] as DataRowView)["Tank_Name"]).Split('.')[0].Replace("/", "");
                    if (!dieselFiles.TryGetValue(Name, out string baseFile)) continue;
                    TankNameForPercentage = Name;
                    Getpercentage();
                    Element3D device3D = ResolveTankModel(Name, baseFile, stats == "True", PercentageFill,
                        diesel25Files, diesel50Files, diesel75Files, 200, 185, 92, 0);
                    if (device3D != null) { device3D.Tag = Name; scene3D.Children.Add(device3D); }
                }

                // Misc tanks (51-75% uses MiscTank50 folder — preserved from original)
                for (int i = 0; i <17; i++)
                {
                    string stats = Convert.ToString((dgMiscTanks.Items[i] as DataRowView)["IsDamaged"]);
                    string Name = Convert.ToString((dgMiscTanks.Items[i] as DataRowView)["Tank_Name"]).Split('.')[0].Replace("/", "");
                    if (!miscFiles.TryGetValue(Name, out string baseFile)) continue;
                    TankNameForPercentage = Name;
                    Getpercentage();
                    Element3D device3D = ResolveTankModel(Name, baseFile, stats == "True", PercentageFill,
                        misc25Files, misc50Files, misc50Files, 180, 255, 128, 192);
                    if (device3D != null) { device3D.Tag = Name; scene3D.Children.Add(device3D); }
                }

                // Ship hull and structural models in root 3D folder
                foreach (string file in Directory.EnumerateFiles(folderPath, "*.stl"))
                {
                    string TankName = System.IO.Path.GetFileNameWithoutExtension(file).Replace("/", "");
                    Element3D device3D = Display3d(file, 70, 150, 150, 150);
                    if (device3D != null) { device3D.Tag = TankName; scene3D.Children.Add(device3D); }
                }
            }
            catch
            {
            }
            System.Diagnostics.Debug.WriteLine($"Refresh3dNew done - scene3D children count: {scene3D.Children.Count}");
            // Give the renderer one frame to process the newly added geometry,
            // then fit the camera and lock the rotation pivot to the scene centre.
            var zoomTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(300) };
            zoomTimer.Tick += (s, e) => { zoomTimer.Stop(); FitCameraToScene(); };
            zoomTimer.Start();
            Models.TableModel.Write_Log(" END : Refresh3dNew");
        }

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
                string query = "SELECT [Percent_Full] FROM [tblSimulationMode_Loading_Condition] WHERE Tank_ID= (SELECT Tank_ID FROM tblMaster_Tank WHERE Tank_Name like'%" +TankNameForPercentage +"%')";
                SqlCommand cmd = new SqlCommand(query, cn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dspercent);
                cn.Close();

                PercentageFill = Math.Round(Convert.ToDecimal(dspercent.Tables[0].Rows[0]["Percent_Full"].ToString()),3);


              
            }
                
            catch //(System.Exception)
            {


            }
        }
        //private void Refresh3dNewPercent()
        //{
        //    try
        //    {
        //        scene3D.Children.Clear();
        //        DefaultLights light = new DefaultLights();
        //        scene3D.Children.Add(light);
        //        viewPort3d.RotateGesture = new MouseGesture(MouseAction.LeftClick);

        //        for (int i = 0; i < 6; i++)
        //        {
        //            string stats = Convert.ToString((dgFreshWaterTanks.Items[i] as DataRowView)["IsDamaged"]);
        //            bool isvisible = Convert.ToBoolean((dgFreshWaterTanks.Items[i] as DataRowView)["IsVisible"]);
        //            string Nametxt = Convert.ToString((dgFreshWaterTanks.Items[i] as DataRowView)["Tank_Name"]);
        //            string NameSplit = Nametxt.Split('.')[0];
        //            string Name = NameSplit.Replace("/", "");

        //            foreach (string file in Directory.EnumerateFiles(folderPath + "\\FreshWaterTank\\", "*.stl"))
        //            {
        //                Element3D device3D = null;
        //                string str = System.IO.Path.GetFileName(file);
        //                string str1 = str.Split('.')[0];
        //                string TankName = str1.Replace("/", "");
        //                if (Name == TankName)
        //                {
        //                    if (isvisible)
        //                    {
        //                        if (stats == "True")
        //                        {
        //                            device3D = Display3d(file, 255, 255, 0, 0);
        //                        }
        //                        else
        //                        {
        //                            device3D = Display3d(file, 180, 0, 0, 100);
        //                        }
        //                        if (device3D != null) device3D.Tag = TankName; ;
        //                        scene3D.Children.Add(device3D);
        //                    }
        //                }
        //            }
        //        }
        //        for (int i = 0; i < 11; i++)
        //        {
        //            string stats = Convert.ToString((dgBallastTanks.Items[i] as DataRowView)["IsDamaged"]);
        //            bool isvisible = Convert.ToBoolean((dgBallastTanks.Items[i] as DataRowView)["IsVisible"]);
        //            string Nametxt = Convert.ToString((dgBallastTanks.Items[i] as DataRowView)["Tank_Name"]);
        //            string Name = Nametxt.Replace("/", "");

        //            foreach (string file in Directory.EnumerateFiles(folderPath + "\\BallastTank\\", "*.stl"))
        //            {
        //                Element3D device3D = null;
        //                string str = System.IO.Path.GetFileName(file);
        //                string str1 = str.Split('.')[0];
        //                string TankName = str1.Replace("/", "");
        //                if (Name == TankName)
        //                {
        //                    if (isvisible)
        //                    {
        //                        if (stats == "True")
        //                        {
        //                            device3D = Display3d(file, 255, 255, 0, 0);
        //                        }
        //                        else
        //                        {
        //                            device3D = Display3d(file, 180, 0, 200, 0);
        //                        }
        //                        if (device3D != null) device3D.Tag = TankName;
        //                        scene3D.Children.Add(device3D);
        //                    }
        //                }
        //            }

        //        }
        //        for (int i = 0; i < 30; i++)
        //        {
        //            string stats = Convert.ToString((dgFuelOilTanks.Items[i] as DataRowView)["IsDamaged"]);
        //            bool isvisible = Convert.ToBoolean((dgFuelOilTanks.Items[i] as DataRowView)["IsVisible"]);
        //            string Nametxt = Convert.ToString((dgFuelOilTanks.Items[i] as DataRowView)["Tank_Name"]);
        //            string splitName = Nametxt.Split('.')[0];
        //            string Name = splitName.Replace("/", "");
        //            foreach (string file in Directory.EnumerateFiles(folderPath + "\\FuelOilTank\\", "*.stl"))
        //            {
        //                Element3D device3D = null;
        //                string str = System.IO.Path.GetFileName(file);
        //                string str1 = str.Split('.')[0];
        //                string TankName = str1.Replace("/", "");
        //                string[] results = file.Split(new string[] { "*.stl" }, StringSplitOptions.None);
        //                if (Name == TankName)
        //                {
        //                    if (isvisible)
        //                    {
        //                        if (stats == "True")
        //                        {
        //                            device3D = Display3d(file, 255, 255, 0, 0);
        //                        }
        //                        else
        //                        {
        //                            device3D = Display3d(file, 200, 185, 92, 0);
        //                        }
        //                        if (device3D != null) device3D.Tag = TankName;

        //                        scene3D.Children.Add(device3D);
        //                    }
        //                }
        //            }
        //        }
        //        for (int i = 0; i < 8; i++)
        //        {
        //            string stats = Convert.ToString((dgMiscTanks.Items[i] as DataRowView)["IsDamaged"]);
        //            bool isvisible = Convert.ToBoolean((dgMiscTanks.Items[i] as DataRowView)["IsVisible"]);
        //            string Nametxt = Convert.ToString((dgMiscTanks.Items[i] as DataRowView)["Tank_Name"]);
        //            string spliteName = Nametxt.Split('.')[0];
        //            string Name = spliteName.Replace("/", "");
        //            foreach (string file in Directory.EnumerateFiles(folderPath + "\\MiscTank\\", "*.stl"))
        //            {

        //                Element3D device3D = null;
        //                string str = System.IO.Path.GetFileName(file);
        //                string str1 = str.Split('.')[0];
        //                string TankName = str1.Replace("/", "");
        //                if (Name == TankName)
        //                {
        //                    if (isvisible)
        //                    {
        //                        if (stats == "True")
        //                        {
        //                            device3D = Display3d(file, 255, 255, 0, 0);
        //                        }
        //                        else
        //                        {
        //                            device3D = Display3d(file, 180, 255, 128, 192);
        //                        }
        //                        if (device3D != null) device3D.Tag = TankName;
        //                        scene3D.Children.Add(device3D);
        //                    }
        //                }
        //            }
        //        }
        //        for (int i = 0; i < 383; i++)
        //        {
        //            try
        //            {
        //                string stats = Convert.ToString((dgCompartments.Items[i] as DataRowView)["IsDamaged"]);
        //                bool isvisible = Convert.ToBoolean((dgCompartments.Items[i] as DataRowView)["IsVisible"]);
        //                string Nametxt = Convert.ToString((dgCompartments.Items[i] as DataRowView)["Tank_Name"]);
        //                string spliteName = Nametxt.Split('.')[0];
        //                string Name = spliteName.Replace("/", "");
        //                foreach (string file in Directory.EnumerateFiles(folderPath + "\\Compartment\\", "*.stl"))
        //                {
        //                    Element3D device3D = null;
        //                    string str = System.IO.Path.GetFileName(file);
        //                    string str1 = str.Split('.')[0];
        //                    string TankName = str1.Replace("/", "");
        //                    string[] results = file.Split(new string[] { "*.stl" }, StringSplitOptions.None);
        //                    if (Name == TankName)
        //                    {
        //                        if (isvisible)
        //                        {
        //                            if (stats == "True")
        //                            {
        //                                device3D = Display3d(file, 255, 255, 0, 0);
        //                            }
        //                            else
        //                            {
        //                                device3D = Display3d(file, 120, 239, 228, 176);
        //                            }
        //                            if (device3D != null) device3D.Tag = TankName;

        //                            scene3D.Children.Add(device3D);
        //                        }
        //                    }
        //                }
        //            }
        //            catch
        //            {
        //            }
        //        }

        //        for (int i = 0; i < 68; i++)
        //        {
        //            string stats = Convert.ToString((dgWTRegion.Items[i] as DataRowView)["IsDamaged"]);
        //            bool isvisible = Convert.ToBoolean((dgWTRegion.Items[i] as DataRowView)["IsVisible"]);
        //            string Nametxt = Convert.ToString((dgWTRegion.Items[i] as DataRowView)["Tank_Name"]);
        //            string spliteName = Nametxt.Split('.')[0];
        //            string Name = spliteName.Replace("/", "");

        //            foreach (string file in Directory.EnumerateFiles(folderPath + "\\WT_REGION\\", "*.stl"))
        //            {

        //                Element3D device3D = null;
        //                string str = System.IO.Path.GetFileName(file);
        //                string str1 = str.Split('.')[0];
        //                string TankName = str1.Replace("/", "");
        //                if (Name == TankName)
        //                {
        //                    if (isvisible)
        //                    {
        //                        if (stats == "True")
        //                        {
        //                            device3D = Display3d(file, 255, 255, 0, 0);
        //                        }
        //                        else
        //                        {
        //                            device3D = Display3d(file, 100, 255, 255, 255);
        //                        }
        //                        if (device3D != null) device3D.Tag = TankName;
        //                        scene3D.Children.Add(device3D);
        //                    }
        //                }
        //            }
        //        }


        //        foreach (string file in Directory.EnumerateFiles(folderPath, "*.stl"))
        //        {
        //            string str = file.Split('\\')[1];
        //            string str1 = str.Split('.')[0];
        //            string TankName = str1.Replace("/", "");
        //            {
        //                Element3D device3D = null;
        //                device3D = Display3d(file, 70, 150, 150, 150);
        //                if (device3D != null) device3D.Tag = TankName;
        //                scene3D.Children.Add(device3D);
        //            }
        //        }
        //    }
        //    catch
        //    {
        //    }
        //    Models.TableModel.Write_Log(" END : Refresh3dNew");
        //}
        //private void Refresh3dNew()
        //{
        //    try
        //    {
        //        scene3D.Children.Clear();
        //        DefaultLights light = new DefaultLights();
        //        scene3D.Children.Add(light);
        //        viewPort3d.RotateGesture = new MouseGesture(MouseAction.LeftClick);

        //        for (int i = 0; i < 6; i++)
        //        {
        //            string stats = Convert.ToString((dgFreshWaterTanks.Items[i] as DataRowView)["IsDamaged"]);
        //            bool isvisible = Convert.ToBoolean((dgFreshWaterTanks.Items[i] as DataRowView)["IsVisible"]);
        //            string Nametxt = Convert.ToString((dgFreshWaterTanks.Items[i] as DataRowView)["Tank_Name"]);
        //            string NameSplit = Nametxt.Split('.')[0];
        //            string Name = NameSplit.Replace("/", "");

        //            foreach (string file in Directory.EnumerateFiles(folderPath + "\\FreshWaterTank\\", "*.stl"))
        //            {
        //                Element3D device3D = null;
        //                string str = System.IO.Path.GetFileName(file);
        //                string str1 = str.Split('.')[0];
        //                string TankName = str1.Replace("/", "");
        //                if (Name == TankName)
        //                {
        //                    if (isvisible)
        //                    {
        //                        if (stats == "True")
        //                        {
        //                            device3D = Display3d(file, 255, 255, 0, 0);
        //                        }
        //                        else
        //                        {
        //                            device3D = Display3d(file, 180, 0, 0, 100);
        //                        }
        //                        if (device3D != null) device3D.Tag = TankName; ;
        //                        scene3D.Children.Add(device3D);
        //                    }
        //                }
        //            }
        //        }
        //        for (int i = 0; i < 11; i++)
        //        {
        //            string stats = Convert.ToString((dgBallastTanks.Items[i] as DataRowView)["IsDamaged"]);
        //            bool isvisible = Convert.ToBoolean((dgBallastTanks.Items[i] as DataRowView)["IsVisible"]);
        //            string Nametxt = Convert.ToString((dgBallastTanks.Items[i] as DataRowView)["Tank_Name"]);
        //            string Name = Nametxt.Replace("/", "");

        //            foreach (string file in Directory.EnumerateFiles(folderPath + "\\BallastTank\\", "*.stl"))
        //            {
        //                Element3D device3D = null;
        //                string str = System.IO.Path.GetFileName(file);
        //                string str1 = str.Split('.')[0];
        //                string TankName = str1.Replace("/", "");
        //                if (Name == TankName)
        //                {
        //                    if (isvisible)
        //                    {
        //                        if (stats == "True")
        //                        {
        //                            device3D = Display3d(file, 255, 255, 0, 0);
        //                        }
        //                        else
        //                        {
        //                            device3D = Display3d(file, 180, 0, 200, 0);
        //                        }
        //                        if (device3D != null) device3D.Tag = TankName;
        //                        scene3D.Children.Add(device3D);
        //                    }
        //                }
        //            }

        //        }
        //        for (int i = 0; i < 30; i++)
        //        {
        //            string stats = Convert.ToString((dgFuelOilTanks.Items[i] as DataRowView)["IsDamaged"]);
        //            bool isvisible = Convert.ToBoolean((dgFuelOilTanks.Items[i] as DataRowView)["IsVisible"]);
        //            string Nametxt = Convert.ToString((dgFuelOilTanks.Items[i] as DataRowView)["Tank_Name"]);
        //            string splitName = Nametxt.Split('.')[0];
        //            string Name = splitName.Replace("/", "");
        //            foreach (string file in Directory.EnumerateFiles(folderPath + "\\FuelOilTank\\", "*.stl"))
        //            {
        //                Element3D device3D = null;
        //                string str = System.IO.Path.GetFileName(file);
        //                string str1 = str.Split('.')[0];
        //                string TankName = str1.Replace("/", "");
        //                string[] results = file.Split(new string[] { "*.stl" }, StringSplitOptions.None);
        //                if (Name == TankName)
        //                {
        //                    if (isvisible)
        //                    {
        //                        if (stats == "True")
        //                        {
        //                            device3D = Display3d(file, 255, 255, 0, 0);
        //                        }
        //                        else
        //                        {
        //                            device3D = Display3d(file, 200, 185, 92, 0);
        //                        }
        //                        if (device3D != null) device3D.Tag = TankName;

        //                        scene3D.Children.Add(device3D);
        //                    }
        //                }
        //            }
        //        }
        //        for (int i = 0; i < 8; i++)
        //        {
        //            string stats = Convert.ToString((dgMiscTanks.Items[i] as DataRowView)["IsDamaged"]);
        //            bool isvisible = Convert.ToBoolean((dgMiscTanks.Items[i] as DataRowView)["IsVisible"]);
        //            string Nametxt = Convert.ToString((dgMiscTanks.Items[i] as DataRowView)["Tank_Name"]);
        //            string spliteName = Nametxt.Split('.')[0];
        //            string Name = spliteName.Replace("/", "");
        //            foreach (string file in Directory.EnumerateFiles(folderPath + "\\MiscTank\\", "*.stl"))
        //            {

        //                Element3D device3D = null;
        //                string str = System.IO.Path.GetFileName(file);
        //                string str1 = str.Split('.')[0];
        //                string TankName = str1.Replace("/", "");
        //                if (Name == TankName)
        //                {
        //                    if (isvisible)
        //                    {
        //                        if (stats == "True")
        //                        {
        //                            device3D = Display3d(file, 255, 255, 0, 0);
        //                        }
        //                        else
        //                        {
        //                            device3D = Display3d(file, 180, 255, 128, 192);
        //                        }
        //                        if (device3D != null) device3D.Tag = TankName;
        //                        scene3D.Children.Add(device3D);
        //                    }
        //                }
        //            }
        //        }
        //        for (int i = 0; i < 15; i++)
        //        {
        //            try
        //            {
        //                string stats = Convert.ToString((dgCompartments.Items[i] as DataRowView)["IsDamaged"]);
        //                bool isvisible = Convert.ToBoolean((dgCompartments.Items[i] as DataRowView)["IsVisible"]);
        //                string Nametxt = Convert.ToString((dgCompartments.Items[i] as DataRowView)["Tank_Name"]);
        //                string spliteName = Nametxt.Split('.')[0];
        //                string Name = spliteName.Replace("/", "");
        //                foreach (string file in Directory.EnumerateFiles(folderPath + "\\Compartment\\", "*.stl"))
        //                {
        //                    Element3D device3D = null;
        //                    string str = System.IO.Path.GetFileName(file);
        //                    string str1 = str.Split('.')[0];
        //                    string TankName = str1.Replace("/", "");
        //                    string[] results = file.Split(new string[] { "*.stl" }, StringSplitOptions.None);
        //                    if (Name == TankName)
        //                    {
        //                        if (isvisible)
        //                        {
        //                            if (stats == "True")
        //                            {
        //                                device3D = Display3d(file, 255, 255, 0, 0);
        //                            }
        //                            else
        //                            {
        //                                device3D = Display3d(file, 120, 239, 228, 176);
        //                            }
        //                            if (device3D != null) device3D.Tag = TankName;

        //                            scene3D.Children.Add(device3D);
        //                        }
        //                    }
        //                }
        //            }
        //            catch
        //            {
        //            }
        //        }

        //        for (int i = 0; i < 79; i++)
        //        {
        //            string stats = Convert.ToString((dgWTRegion.Items[i] as DataRowView)["IsDamaged"]);
        //            bool isvisible = Convert.ToBoolean((dgWTRegion.Items[i] as DataRowView)["IsVisible"]);
        //            string Nametxt = Convert.ToString((dgWTRegion.Items[i] as DataRowView)["Tank_Name"]);
        //            string spliteName = Nametxt.Split('.')[0];
        //            string Name = spliteName.Replace("/", "");

        //            foreach (string file in Directory.EnumerateFiles(folderPath + "\\WT_REGION\\", "*.stl"))
        //            {

        //                Element3D device3D = null;
        //                string str = System.IO.Path.GetFileName(file);
        //                string str1 = str.Split('.')[0];
        //                string TankName = str1.Replace("/", "");
        //                if (Name == TankName)
        //                {
        //                    if (isvisible)
        //                    {
        //                        if (stats == "True")
        //                        {
        //                            device3D = Display3d(file, 255, 255, 0, 0);
        //                        }
        //                        else
        //                        {
        //                            device3D = Display3d(file, 100, 255, 255, 255);
        //                        }
        //                        if (device3D != null) device3D.Tag = TankName;
        //                        scene3D.Children.Add(device3D);
        //                    }
        //                }
        //            }
        //        }


        //        foreach (string file in Directory.EnumerateFiles(folderPath, "*.stl"))
        //        {
        //            string str = file.Split('\\')[1];
        //            string str1 = str.Split('.')[0];
        //            string TankName = str1.Replace("/", "");
        //            {
        //                Element3D device3D = null;
        //                device3D = Display3d(file, 70, 150, 150, 150);
        //                if (device3D != null) device3D.Tag = TankName;
        //                scene3D.Children.Add(device3D);
        //            }
        //        }
        //    }
        //    catch
        //    {
        //    }
        //    Models.TableModel.Write_Log(" END : Refresh3dNew");
        //}
        private Element3D Display3d(string modelPath, byte A, byte R, byte G, byte B)
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
                System.Diagnostics.Debug.WriteLine($"3D Model loaded OK: {modelPath}");
                return model;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"3D Model FAILED: {modelPath} - {e.Message}");
                return null;
            }
        }

        private Dictionary<string, string> BuildTankFileDictionary(string folder)
        {
            var dict = new Dictionary<string, string>();
            if (!System.IO.Directory.Exists(folder)) return dict;
            foreach (string file in System.IO.Directory.EnumerateFiles(folder, "*.stl"))
                dict[System.IO.Path.GetFileNameWithoutExtension(file).Replace("/", "")] = file;
            return dict;
        }

        private Element3D ResolveTankModel(string name, string baseFile, bool isDamaged, decimal fill,
            Dictionary<string, string> files25, Dictionary<string, string> files50, Dictionary<string, string> files75,
            byte A, byte R, byte G, byte B)
        {
            if (isDamaged) return Display3d(baseFile, 255, 255, 0, 0);
            if (fill == 0m) return Display3d(baseFile, 200, 185, 185, 196);
            if (fill <= 25m && files25.TryGetValue(name, out string f25)) return Display3d(f25, A, R, G, B);
            if (fill <= 50m && files50.TryGetValue(name, out string f50)) return Display3d(f50, A, R, G, B);
            if (fill <= 75m && files75.TryGetValue(name, out string f75)) return Display3d(f75, A, R, G, B);
            return Display3d(baseFile, A, R, G, B);
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

        private void chkVisible_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                object a = e.Source;
                CheckBox chk = (CheckBox)sender;

                DataGridRow row = FindAncestor<DataGridRow>(chk);
                if (row != null)
                {
                    DataRowView rv = (DataRowView)row.Item;
                    byte A = 0, R = 0, G = 0, B = 0;

                    string Err = "";
                    string query = "update [tblSimulationMode_Loading_Condition] set [IsVisible]='" + (bool)chk.IsChecked + "' where Tank_ID=" + rv["Tank_ID"];
                    DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
                    command.CommandText = query;
                    command.CommandType = CommandType.Text;
                    Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);
                    string subfolder = "";
                    if (Convert.ToInt32(rv["Tank_ID"]) <= 11)
                    {
                        subfolder = "BallastTank";
                        A = 180;
                        R = 0;
                        G = 200;
                        B = 0;

                    }
                    else if (Convert.ToInt32(rv["Tank_ID"]) > 11 && (Convert.ToInt32(rv["Tank_ID"]) <= 16))
                    {
                        subfolder = "FreshWaterTank";
                        A = 180;
                        R = 0;
                        G = 0;
                        B = 100;
                    }
                    else if (Convert.ToInt32(rv["Tank_ID"]) > 16 && (Convert.ToInt32(rv["Tank_ID"]) <= 46))
                    {
                        subfolder = "FuelOilTank";
                        A = 200;
                        R = 185;
                        G = 92;
                        B = 0;
                    }
                    else if (Convert.ToInt32(rv["Tank_ID"]) > 46 && (Convert.ToInt32(rv["Tank_ID"]) <= 54))
                    {
                        subfolder = "MiscTank";
                        A = 180;
                        R = 255;
                        G = 128;
                        B = 192;
                    }

                    else if (Convert.ToInt32(rv["Tank_ID"]) > 54 && (Convert.ToInt32(rv["Tank_ID"]) <= 434))
                    {
                        subfolder = "Compartment";
                        A = 120;
                        R = 239;
                        G = 228;
                        B = 176;
                    }



                    else if (Convert.ToInt32(rv["Tank_ID"]) > 434 && (Convert.ToInt32(rv["Tank_ID"]) <= 504))
                    {
                        subfolder = "WT_REGION";
                        A = 120;
                        R = 239;
                        G = 228;
                        B = 176;
                    }

                    string TankName = Convert.ToString(rv["Tank_Name"]);
                    string FUllTankName = TankName.Split('.')[0];
                    string str1 = FUllTankName.Replace("/", "");
                    if ((bool)chk.IsChecked == false)
                    {
                        for (int j = 0; j < 30; j++)    //edit 30 it is total number of tank and compartment+4
                        {
                            try
                            {
                                var model = scene3D.Children[j] as Element3D;
                                string modelname = model?.Tag as string;
                                if (modelname == str1)
                                {
                                    scene3D.Children.Remove(model);
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                    else
                    {
                        scene3D.Children.RemoveAt(scene3D.Children.Count - 1);
                        scene3D.Children.RemoveAt(scene3D.Children.Count - 1);
                        string file = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + folderPath + "\\" + subfolder + "\\" + TankName + ".stl";
                        Element3D device3D = Display3d(file, A, R, G, B);
                        if (device3D != null) { device3D.Tag = TankName; scene3D.Children.Add(device3D); }

                        foreach (string file1 in Directory.EnumerateFiles(folderPath, "*.stl"))
                        {
                            string str = file1.Split('\\')[1];
                            str1 = str.Split('.')[0];
                            TankName = str1.Replace("/", "");
                            //if (Name == str1)
                            {
                                device3D = Display3d(file1, 70, 150, 150, 150);
                                if (device3D != null) { device3D.Tag = TankName; scene3D.Children.Add(device3D); }
                            }
                        }
                    }


                }
            }
            catch
            {
            }
        }

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

        //private void dgBallastTanks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{

        //}

        private void Status_CopyingCellClipboardContent(object sender, DataGridCellClipboardEventArgs e)
        {

        }

        private void dgCompartments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectionchangecount =0;
            Models.TableModel.Write_Log(" START : dgCompartments_SelectionChanged");
            try
            {
                int TankId, CellCnt;

                string Err = string.Empty;
                string status = string.Empty;
                if ( dgCompartments.IsLoaded == true)
                {
                    DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
                    var RowsCnt = dgCompartments.SelectedItems;

                    if (RowsCnt.Count > 1)
                    {
                        DataRowView row = (DataRowView)dgCompartments.SelectedItems[0];
                        string selectedStatus = row["status"].ToString();
                        TankId = Convert.ToInt16(row["Tank_Id"]);
                        string query = " ";
                        for (int i = 1; i < RowsCnt.Count; i++)
                        {
                            DataRowView mulRow = (DataRowView)RowsCnt[i];
                            mulRow["Status"] = selectedStatus;
                            int TankIdMul = Convert.ToInt16(mulRow["tank_Id"]);
                            query += "update  tblSimulationMode_Loading_Condition set Status=" + selectedStatus.ToString() + " where Tank_ID=" + TankIdMul;
                            if (selectedStatus == "0")
                            {
                                query += "  update  tblSimulationMode_Loading_Condition set IsDamaged=0 where Tank_ID=" + TankIdMul;
                                query += "  update  tblSimulationMode_Tank_Status set IsDamaged=0 where Tank_ID=" + TankIdMul;
                            }
                            else
                            {
                                query += "  update  tblSimulationMode_Loading_Condition set IsDamaged=1 where Tank_ID=" + TankIdMul;
                                query += "  update  tblSimulationMode_Tank_Status set IsDamaged=1 where Tank_ID=" + TankIdMul;
                            }
                            command.CommandText = query;
                            command.CommandType = CommandType.Text;
                            Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);
                        }
                        query = "update  tblSimulationMode_Loading_Condition set Status=" + selectedStatus.ToString() + " where Tank_ID=" + TankId;
                        if (selectedStatus == "0")
                        {
                            query += "  update  tblSimulationMode_Loading_Condition set IsDamaged=0 where Tank_ID=" + TankId;
                            query += "  update  tblSimulationMode_Tank_Status set IsDamaged=0 where Tank_ID=" + TankId;
                        }
                        else
                        {
                            query += "  update  tblSimulationMode_Loading_Condition set IsDamaged=1 where Tank_ID=" + TankId;
                            query += "  update  tblSimulationMode_Tank_Status set IsDamaged=1 where Tank_ID=" + TankId;

                        }
                        command.CommandText = query;
                        command.CommandType = CommandType.Text;
                        Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);
                        Models.TableModel.Write_Log(" END : dgCompartments_SelectionChanged");
                    }

                }
            }
            catch { }
        }

        public DataGridCell GetCell(int row, int column)
        {
            DataGridRow rowContainer = GetRow(row);

            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);

                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                if (cell == null)
                {
                    dgBallastTanks.ScrollIntoView(rowContainer, dgBallastTanks.Columns[column]);
                    cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                }
                return cell;
            }
            return null;
        }

        public DataGridCell GetCellFuel(int row, int column)
        {
            DataGridRow rowContainer = GetRowFuel(row);

            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);

                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                if (cell == null)
                {
                    dgFuelOilTanks.ScrollIntoView(rowContainer, dgFuelOilTanks.Columns[column]);
                    cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                }
                return cell;
            }
            return null;
        }

        public DataGridCell GetCellMisc(int row, int column)
        {
            DataGridRow rowContainer = GetRowMisc(row);

            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);

                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                if (cell == null)
                {
                    dgMiscTanks.ScrollIntoView(rowContainer, dgMiscTanks.Columns[column]);
                    cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                }
                return cell;
            }
            return null;
        }

        public DataGridCell GetCellCpm(int row, int column)
        {
            DataGridRow rowContainer = GetRowCmp(row);

            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);

                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                if (cell == null)
                {
                    dgCompartments.ScrollIntoView(rowContainer, dgCompartments.Columns[column]);
                    cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                }
                return cell;
            }
            return null;
        }

        public DataGridRow GetRow(int index)
        {
            DataGridRow row = (DataGridRow)dgBallastTanks.ItemContainerGenerator.ContainerFromIndex(index);
            if (row == null)
            {
                dgBallastTanks.UpdateLayout();
                dgBallastTanks.ScrollIntoView(dgBallastTanks.Items[index]);
                row = (DataGridRow)dgBallastTanks.ItemContainerGenerator.ContainerFromIndex(index);
            }
            return row;
        }

        public DataGridRow GetRowFuel(int index)
        {
            DataGridRow row = (DataGridRow)dgFuelOilTanks.ItemContainerGenerator.ContainerFromIndex(index);
            if (row == null)
            {
                dgFuelOilTanks.UpdateLayout();
                dgFuelOilTanks.ScrollIntoView(dgFuelOilTanks.Items[index]);
                row = (DataGridRow)dgFuelOilTanks.ItemContainerGenerator.ContainerFromIndex(index);
            }
            return row;
        }

        public DataGridRow GetRowMisc(int index)
        {
            DataGridRow row = (DataGridRow)dgMiscTanks.ItemContainerGenerator.ContainerFromIndex(index);
            if (row == null)
            {
                dgMiscTanks.UpdateLayout();
                dgBallastTanks.ScrollIntoView(dgMiscTanks.Items[index]);
                row = (DataGridRow)dgMiscTanks.ItemContainerGenerator.ContainerFromIndex(index);
            }
            return row;
        }

        public DataGridRow GetRowCmp(int index)
        {
            DataGridRow row = (DataGridRow)dgCompartments.ItemContainerGenerator.ContainerFromIndex(index);
            if (row == null)
            {
                dgCompartments.UpdateLayout();
                dgCompartments.ScrollIntoView(dgCompartments.Items[index]);
                row = (DataGridRow)dgCompartments.ItemContainerGenerator.ContainerFromIndex(index);
            }
            return row;
        }

        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }


     #region Loading Rows Dyanmically
        private void dgBallastTanks_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            //Models.TableModel.Write_Log(" START : dgBallastTanks_LoadingRow");
            //DataRowView item = e.Row.Item as DataRowView;
            //if (item != null)
            //{
            //    DataRow row = item.Row;

            //    e.Row.Background = new SolidColorBrush(System.Windows.Media.Colors.LightBlue);

            //}
            //Models.TableModel.Write_Log(" END : dgBallastTanks_LoadingRow");
        }

        private void dgFuelOilTanks_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            //DataRowView item = e.Row.Item as DataRowView;
            //if (item != null)
            //{
            //    DataRow row = item.Row;

            //    e.Row.Background = new SolidColorBrush(System.Windows.Media.Colors.LightBlue);


            //}
        }

        private void dgFreshWaterTanks_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            //DataRowView item = e.Row.Item as DataRowView;
            //if (item != null)
            //{
            //    DataRow row = item.Row;

            //    e.Row.Background = new SolidColorBrush(System.Windows.Media.Colors.LightBlue);


            //}
        }

        private void dgMiscTanks_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            //DataRowView item = e.Row.Item as DataRowView;
            //if (item != null)
            //{
            //    DataRow row = item.Row;

            //    e.Row.Background = new SolidColorBrush(System.Windows.Media.Colors.LightBlue);

            //}
        }

        private void dgVariableItems_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            //DataRowView item = e.Row.Item as DataRowView;
            //if (item != null)
            //{
            //    DataRow row = item.Row;

            //    e.Row.Background = new SolidColorBrush(System.Windows.Media.Colors.LightBlue);
            //}
        }

        private void dgWTRegion_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            //DataRowView item = e.Row.Item as DataRowView;
            //if (item != null)
            //{
            //    DataRow row = item.Row;

            //    e.Row.Background = new SolidColorBrush(System.Windows.Media.Colors.LightBlue);
            //}
        }
        private void dgCompartments_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            //Models.TableModel.Write_Log(" START : dgCompartments_LoadingRow");
            //DataRowView item = e.Row.Item as DataRowView;
            //if (item != null)
            //{
            //    DataRow row = item.Row;

            //    e.Row.Background = new SolidColorBrush(System.Windows.Media.Colors.LightBlue);
            //}
            //Models.TableModel.Write_Log(" END : dgCompartments_LoadingRow");
        }


        #endregion

        private void btnclearBallast_Click(object sender, RoutedEventArgs e)
        {

        }

        private void dgCargoTanks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectionchangecount = 0;
            Models.TableModel.Write_Log(" START WHR SELECTION = 1 : dgCargoTanks_SelectionChanged");
            try
            {
                int TankId, CellCnt;

                string Err = string.Empty;
                string status = string.Empty;
                if (dgCargoTanks.IsLoaded == true)
                {
                    DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();

                    var RowsCnt = dgCargoTanks.SelectedItems;

                    if (RowsCnt.Count > 1)
                    {
                        Models.TableModel.Write_Log(" START : dgCargoTanks_SelectionChanged");
                        PK = 1;
                        DataRowView row = (DataRowView)dgCargoTanks.SelectedItems[0];
                        // DataGridCell ThirdColumnInFirstRow = dgBallastTanks.Columns[2].GetCellContent(row).Parent as DataGridCell;

                        //ThirdColumnInFirstRow.Background = Brushes.LightPink;
                        string selectedStatus = row["status"].ToString();

                        TankId = Convert.ToInt16(row["Tank_Id"]);
                        string query = " ";
                        for (int i = 1; i < RowsCnt.Count; i++)
                        {
                            DataRowView mulRow = (DataRowView)RowsCnt[i];
                            mulRow["Status"] = selectedStatus;
                            //DataGridCell SelectedColumnInSelectedRow = dgBallastTanks.Columns[2].GetCellContent(mulRow).Parent as DataGridCell;
                            //SelectedColumnInSelectedRow.Background = Brushes.LightPink;

                            int TankIdMul = Convert.ToInt16(mulRow["tank_Id"]);
                            query += "update  tblSimulationMode_Loading_Condition set Status=" + selectedStatus.ToString() + " where Tank_ID=" + TankIdMul;
                            if (selectedStatus == "0")
                            {
                                query += "  update  tblSimulationMode_Loading_Condition set IsDamaged=0 where Tank_ID=" + TankIdMul;
                                query += "  update  tblSimulationMode_Tank_Status set IsDamaged=0 where Tank_ID=" + TankIdMul;
                            }
                            else
                            {
                                query += "  update  tblSimulationMode_Loading_Condition set IsDamaged=1 where Tank_ID=" + TankIdMul;
                                query += "  update  tblSimulationMode_Tank_Status set IsDamaged=1 where Tank_ID=" + TankIdMul;
                            }
                            command.CommandText = query;
                            command.CommandType = CommandType.Text;
                            Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);
                        }
                        query = "update  tblSimulationMode_Loading_Condition set Status=" + selectedStatus.ToString() + " where Tank_ID=" + TankId;
                        if (selectedStatus == "0")
                        {
                            query += "  update  tblSimulationMode_Loading_Condition set IsDamaged=0 where Tank_ID=" + TankId;
                            query += "  update  tblSimulationMode_Tank_Status set IsDamaged=0 where Tank_ID=" + TankId;
                        }
                        else
                        {
                            query += "  update  tblSimulationMode_Loading_Condition set IsDamaged=1 where Tank_ID=" + TankId;
                            query += "  update  tblSimulationMode_Tank_Status set IsDamaged=1 where Tank_ID=" + TankId;

                        }
                        command.CommandText = query;
                        command.CommandType = CommandType.Text;
                        Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);
                    }
                    Models.TableModel.Write_Log(" END : dgCargoTanks_SelectionChanged");
                }
            }
            catch { }
        }

        private void dgCargoTanks_LoadingRow(object sender, DataGridRowEventArgs e)
        {

        }

        private void btnclearCargo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                #region Cargo
                {
                    DataTable cargotank = Models.BO.clsGlobVar.dtSimulationCargoTanks;
                    DataColumn dc = new DataColumn("Volume");
                    DataColumn dcStatus = new DataColumn("Status");
                    dc.DataType = typeof(int);
                    dcStatus.DataType = typeof(int);
                    dcStatus.DefaultValue = 0;
                    dc.DefaultValue = 0;
                    cargotank.Columns.Remove("Volume");
                    cargotank.Columns.Remove("Status");
                    cargotank.Columns.Add(dc);
                    cargotank.Columns.Add(dcStatus);
                    dgBallastTanks.ItemsSource = cargotank.DefaultView;
                }
                #endregion
                DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
                string Err = "";

                for (int i = 0; i < Models.BO.clsGlobVar.dtSimulationCargoTanks.Rows.Count; i++)
                {

                    ((dgBallastTanks.Items[i] as DataRowView)["Volume"]) = 0;
                    ((dgBallastTanks.Items[i] as DataRowView)["Percent_Full"]) = 0;
                    ((dgBallastTanks.Items[i] as DataRowView)["Sounding_Level"]) = 0;
                    ((dgBallastTanks.Items[i] as DataRowView)["Weight"]) = 0;

                }

                string query = "update tblSimulationMode_Tank_Status SET Volume =0 , IsDamaged =0 where Tank_ID in(select Tank_ID  from tblmaster_tank where [group]='CARGO');" +
                                " update [tblSimulationMode_Loading_Condition] SET Volume =0,IsDamaged = 0,Status=0 where Tank_ID in(select Tank_ID  from tblmaster_tank where [group]='CARGO')";
                command = Models.DAL.clsDBUtilityMethods.GetCommand();
                command.CommandText = query;
                command.CommandType = CommandType.Text;
                Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);
                MessageBox.Show("CARGO Tanks Cleared ");
            }
            catch //(Exception)
            {

                // throw;
            }
        }

        private void canvas2DProfile_MouseRightButtonDown_1(object sender, MouseButtonEventArgs e)
        {
           
            double IDSelect = 0;
            int Group_Name;
            string SelectedGroup;
             double[] ShadeX;
             double[] ShadeY;
             bool found = false;
             IEnumerable<SelectionProcess> _InumSelection;
             
            #region PROFILE
            if (canvas2DProfile.IsLoaded == true)
            {
              Point  tp = e.GetPosition(this.canvas2DProfile);
                double xx = tp.X;
                double yy = tp.Y;
                 AddHatchProfile();
                 ShadeX = new double[5];
                 ShadeY = new double[5];

                for (int i = 0; i <= 91; i++)//29
                {
                    for (int j = 1; j < 2; j++)

                        if ((xx <= clsGlobVar.ProfileCoordinate.mul[i, j]) && (xx >= clsGlobVar.ProfileCoordinate.mul[i, j + 1]))
                        {
                            IDSelect = clsGlobVar.ProfileCoordinate.mul[i, j - 1];

                            for (int p = 0; p <= 91; p++)//29
                            {
                                for (int n = 1; n < 2; n++)
                                {
                                    if ((yy <= clsGlobVar.ProfileCoordinate.mul[p, n + 2]) && (yy >= clsGlobVar.ProfileCoordinate.mul[p, n + 3]) && (clsGlobVar.ProfileCoordinate.mul[p, n - 1] == IDSelect))
                                    {
                                        IDSelect = clsGlobVar.ProfileCoordinate.mul[i, j - 1];
                                        string sCmd = "Select * from tbl_GA_Plan_Profile where Tank_ID='" + IDSelect + "'";
                                        DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
                                        command.CommandText = sCmd;
                                        command.CommandType = CommandType.Text;
                                        string Err = "";
                                        DataTable dtcoordinateProfile = new DataTable();
                                        dtcoordinateProfile = Models.DAL.clsDBUtilityMethods.GetTable(command, Err);
                                        for (int o = 1; o <= 4; o++)
                                        {
                                            string sc = Convert.ToString("X" + o);
                                            string sr = Convert.ToString("Y" + o);
                                            ShadeX[o] = Convert.ToDouble(dtcoordinateProfile.Rows[0][sc]);
                                            ShadeY[o] = Convert.ToDouble(dtcoordinateProfile.Rows[0][sr]);
                                        }
                                        TankID = Convert.ToInt16(IDSelect);                            
                                        DrawHatchProfile(canvas2DProfile, Convert.ToInt32(IDSelect), 101, ShadeX, ShadeY, System.Windows.Media.Color.FromArgb(180, 255, 255, 128));
                                        Propertyset();
                                        Selection();
                                        p = 91;
                                        i = 91;
                                        break;


                                    }
                                } 
                                
                            }
                        }
                }

                for (int ii = 0; ii < 13; ii++ )
                {

                    for(int jj=0; jj < 1; jj++)
                    {
                        if ((xx <= clsGlobVar.ProfileCoordinate.MxMnCurved[ii, jj + 1]) && (xx >= clsGlobVar.ProfileCoordinate.MxMnCurved[ii, jj + 2]))
                        {
                            int id = Convert.ToInt16(clsGlobVar.ProfileCoordinate.MxMnCurved[ii, 0]);

                            for (int pp = 0; pp < 13; pp++ )
                            {
                                for (int np = 0; np < 1; np++ )
                                {
                                    if ((yy <= clsGlobVar.ProfileCoordinate.MxMnCurved[pp, np + 3]) && (yy >= clsGlobVar.ProfileCoordinate.MxMnCurved[pp, np + 4]) && (clsGlobVar.ProfileCoordinate.MxMnCurved[pp, np] == id))
                                    {
                                        
                                        TankID = Convert.ToInt16(id);   
                                        DrawHatchProfile(canvas2DProfile, id, 101, ShadeX, ShadeY, System.Windows.Media.Color.FromArgb(180, 255, 255, 128));
                                        Propertyset();
                                        Selection();
                                        pp = 12;
                                        ii = 12;
                                        break;
                                    }
                                }
                            }
                           
                        }
                    }
                }
                 _objSelection.Clear();

            }
#endregion

            #region PLANA
            if (canvas2DPlanA.IsLoaded == true)
            {
                double[] sss = new double[20]; 
                Point tp = e.GetPosition(this.canvas2DPlanA);
                double xx = tp.X;
                double yy = tp.Y;
                AddHatchDeckPlanA();
                ShadeX = new double[25];
                ShadeY = new double[25]; 
                for (int i = 0; i <= 161; i++)
                {
                    for (int j = 1; j < 2; j++)
                        if ((xx <= clsGlobVar.CoordinatePlanA.mulPlanA[i, j]) && (xx >= clsGlobVar.CoordinatePlanA.mulPlanA[i, j + 1]))
                        {
                            IDSelect = clsGlobVar.CoordinatePlanA.mulPlanA[i, j - 1];
                            SelectionProcess obj = new SelectionProcess();
                            obj.ID = IDSelect;
                            obj.MaxValue = clsGlobVar.CoordinatePlanA.mulPlanA[i, j];
                            obj.MinValue = clsGlobVar.CoordinatePlanA.mulPlanA[i, j + 1];
                            obj.SmallValue = obj.MaxValue - obj.MinValue;
                            _objSelection.Add(obj);
                        }
                }

                {
                    _objSelection.Sort();
                    _InumSelection = _objSelection;
                }

                foreach (SelectionProcess f in _InumSelection)
                {
                   
                    for (int p = 0; p <= 161; p++)
                    {
                        for (int n = 1; n < 2; n++)
                        {
                            if ((yy <= clsGlobVar.CoordinatePlanA.mulPlanA[p, n + 2]) && (yy >= clsGlobVar.CoordinatePlanA.mulPlanA[p, n + 3]) && (clsGlobVar.CoordinatePlanA.mulPlanA[p, n - 1] == f.ID))
                            {
                                Group_Name = Convert.ToInt16(clsGlobVar.CoordinatePlanA.mulPlanA[p, n + 4]);
                                if (Group_Name == 1) { SelectedGroup = "A"; } else { SelectedGroup = "B"; }
                                IDSelect = clsGlobVar.CoordinatePlanA.mulPlanA[p, n - 1];


                                string sCmd = "Select * from tbl_GA_Plan_A where Tank_ID='" + IDSelect + "'";
                                DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
                                command.CommandText = sCmd;
                                command.CommandType = CommandType.Text;
                                string Err = "";
                                DataTable dtcoordinateA = new DataTable();
                                dtcoordinateA = Models.DAL.clsDBUtilityMethods.GetTable(command, Err);
                                for (int o = 1; o <= 24; o++)
                                {
                                    string sc = Convert.ToString("X" + o);
                                    string sr = Convert.ToString("Y" + o);
                                    ShadeX[o] = Convert.ToDouble(dtcoordinateA.Rows[0][sc]);
                                    ShadeY[o] = Convert.ToDouble(dtcoordinateA.Rows[0][sr]);
                                }
                                TankID = Convert.ToInt16(IDSelect);
                                DrawHatchDeckPlan(canvas2DPlanA, Convert.ToInt32(IDSelect), 101, ShadeX, ShadeY, System.Windows.Media.Color.FromArgb(180, 255, 255, 128));
                                Propertyset();
                                Selection();
                                p = 161;
                                found=true;
                                break;
                            }
                        }
                    }

                    if (found==true)
                    {           
                        break;
                    } 
                }
                _objSelection.Clear();
                found = false;
            }

            #endregion

            #region PLANB
            if (  canvas2DPlanB.IsLoaded == true)
           {
               Point tp = e.GetPosition(this.canvas2DPlanB);
               double xx = tp.X;
               double yy = tp.Y;
               AddHatchDeckPlanB();
               ShadeX = new double[14];
               ShadeY = new double[14];
               for (int i = 0; i <= 249; i++)
               {
                   for (int j = 1; j < 2; j++)

                       if ((xx <= clsGlobVar.CoordinatePlanB.mulPlanB[i, j]) && (xx >= clsGlobVar.CoordinatePlanB.mulPlanB[i, j + 1]))
                       {
                          

                           IDSelect = clsGlobVar.CoordinatePlanB.mulPlanB[i, j - 1];
                           SelectionProcess obj = new SelectionProcess();
                           obj.ID = IDSelect;
                           obj.MaxValue = clsGlobVar.CoordinatePlanB.mulPlanB[i, j];
                           obj.MinValue = clsGlobVar.CoordinatePlanB.mulPlanB[i, j + 1];
                           obj.SmallValue = obj.MaxValue - obj.MinValue;
                           _objSelection.Add(obj);

                       }
               }
                 {
                    _objSelection.Sort();
                    _InumSelection = _objSelection;
                }

                 foreach (SelectionProcess f in _InumSelection)
                 {
                     for (int p = 0; p <= 249; p++)
                     {
                         for (int n = 1; n < 2; n++)
                         {
                             if ((yy <= clsGlobVar.CoordinatePlanB.mulPlanB[p, n + 2]) && (yy >= clsGlobVar.CoordinatePlanB.mulPlanB[p, n + 3]) && (clsGlobVar.CoordinatePlanB.mulPlanB[p, n - 1] == f.ID))
                             {
                                 Group_Name = Convert.ToInt16(clsGlobVar.CoordinatePlanB.mulPlanB[p, n + 4]);
                                 if (Group_Name == 1) { SelectedGroup = "A"; } else { SelectedGroup = "B"; }
                                 IDSelect = clsGlobVar.CoordinatePlanB.mulPlanB[p, n - 1];


                                 string sCmd = "Select * from tbl_GA_Plan_B where Tank_ID='" + IDSelect + "' and [Group]='" + SelectedGroup + "'";
                                 DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
                                 command.CommandText = sCmd;
                                 command.CommandType = CommandType.Text;
                                 string Err = "";
                                 DataTable coordinateB = new DataTable();
                                 coordinateB = Models.DAL.clsDBUtilityMethods.GetTable(command, Err);
                                 for (int o = 1; o <= 13; o++)
                                 {
                                     string sc = Convert.ToString("X" + o);
                                     string sr = Convert.ToString("Y" + o);
                                     ShadeX[o] = Convert.ToDouble(coordinateB.Rows[0][sc]);
                                     ShadeY[o] = Convert.ToDouble(coordinateB.Rows[0][sr]);
                                 }
                                 TankID = Convert.ToInt16(IDSelect);
                                 DrawHatchDeckPlan(canvas2DPlanB, Convert.ToInt32(IDSelect), 101, ShadeX, ShadeY, System.Windows.Media.Color.FromArgb(180, 255, 255, 128));
                                 Propertyset();
                                 Selection();
                                 p = 249;
                                 found = true;
                                 break;
                             }
                         }
                     }
                     if (found == true)
                     {
                         break;
                     }
                 }
                 _objSelection.Clear();
                 found = false;

           }
            #endregion

           // #region PLANC
           // if ( canvas2DPlanC.IsLoaded == true)
           //{
           //    Point tp = e.GetPosition(this.canvas2DPlanC);
           //    double xx = tp.X;
           //    double yy = tp.Y;
           //    AddHatchDeckPlanC();
           //    ShadeX = new double[14];
           //    ShadeY = new double[14];
           //    for (int i = 0; i <= 154; i++)
           //    {
           //        for (int j = 1; j < 2; j++)

           //            if ((xx <= clsGlobVar.CoordinatePlanC.mulPlanC[i, j]) && (xx >= clsGlobVar.CoordinatePlanC.mulPlanC[i, j + 1]))
           //            {
           //                IDSelect = clsGlobVar.CoordinatePlanC.mulPlanC[i, j - 1];
           //                SelectionProcess obj = new SelectionProcess();
           //                obj.ID = IDSelect;
           //                obj.MaxValue = clsGlobVar.CoordinatePlanC.mulPlanC[i, j];
           //                obj.MinValue = clsGlobVar.CoordinatePlanC.mulPlanC[i, j + 1];
           //                obj.SmallValue = obj.MaxValue - obj.MinValue;
           //                _objSelection.Add(obj);
           //            }
           //    }
           //      {
           //         _objSelection.Sort();
           //         _InumSelection = _objSelection;
           //     }

           //     foreach (SelectionProcess f in _InumSelection)
           //     {

           //                for (int p = 0; p <= 154; p++)
           //                {
           //                    for (int n = 1; n < 2; n++)
           //                    {
           //                        if ((yy <= clsGlobVar.CoordinatePlanC.mulPlanC[p, n + 2]) && (yy >= clsGlobVar.CoordinatePlanC.mulPlanC[p, n + 3]) && (clsGlobVar.CoordinatePlanC.mulPlanC[p, n - 1] == f.ID))
           //                        {
           //                            Group_Name = Convert.ToInt16(clsGlobVar.CoordinatePlanC.mulPlanC[p, n + 4]);
           //                            if (Group_Name == 1) { SelectedGroup = "A"; } else { SelectedGroup = "B"; }
           //                            IDSelect = clsGlobVar.CoordinatePlanC.mulPlanC[p, n - 1];


           //                            string sCmd = "Select * from [tbl_GA_Plan_C] where Tank_ID='" + IDSelect + "' and [Group]='" + SelectedGroup + "'";
           //                            DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
           //                            command.CommandText = sCmd;
           //                            command.CommandType = CommandType.Text;
           //                            string Err = "";
           //                            DataTable dtcoordinateC = new DataTable();
           //                            dtcoordinateC = Models.DAL.clsDBUtilityMethods.GetTable(command, Err);
           //                            for (int o = 1; o <= 13; o++)
           //                            {
           //                                string sc = Convert.ToString("X" + o);
           //                                string sr = Convert.ToString("Y" + o);
           //                                ShadeX[o] = Convert.ToDouble(dtcoordinateC.Rows[0][sc]);
           //                                ShadeY[o] = Convert.ToDouble(dtcoordinateC.Rows[0][sr]);
           //                            }
           //                            TankID = Convert.ToInt16(IDSelect);
           //                            DrawHatchDeckPlan(canvas2DPlanC, Convert.ToInt32(IDSelect), 101, ShadeX, ShadeY, System.Windows.Media.Color.FromArgb(180, 255, 255, 128));
           //                            Propertyset();
           //                            Selection();
           //                            p = 154;
           //                            found = true;
           //                            break;
           //                        }
           //                    }
           //                }

           //        if (found==true)
           //         {           
           //             break;
           //         } 
           //     }
           //     _objSelection.Clear();
           //     found = false;
               
           //}
           // #endregion
    
        }
        public void Selection()
        {
            if (TankID >= 1 && TankID <= 11)
            {
                Dispatcher.BeginInvoke((System.Action)(() => expander1.Background = Brushes.Blue));
                Dispatcher.BeginInvoke((System.Action)(() => tabControl4.SelectedIndex = 0));
                dgBallastTanks.SelectedIndex = TankID - 1;
                dgBallastTanks.ScrollIntoView(dgBallastTanks.Items[TankID - 1]);
                expander1.IsExpanded = true;
                expander2.IsExpanded = false;
                expander3.IsExpanded = false;
                expander4.IsExpanded = false;
            }
            if (TankID >= 12 && TankID <= 16 )
            {
                Dispatcher.BeginInvoke((System.Action)(() => expander3.Background = Brushes.Blue));
                Dispatcher.BeginInvoke((System.Action)(() => tabControl4.SelectedIndex = 0));
                dgFreshWaterTanks.SelectedIndex = TankID - 12;
                dgFreshWaterTanks.ScrollIntoView(dgFreshWaterTanks.Items[TankID - 12]);
                expander1.IsExpanded = false;
                expander2.IsExpanded = false;
                expander3.IsExpanded = true;
                expander4.IsExpanded = false;
            }
            if (TankID >= 17 && TankID <= 46)
            {
                Dispatcher.BeginInvoke((System.Action)(() => expander2.Background = Brushes.Blue));
                Dispatcher.BeginInvoke((System.Action)(() => tabControl4.SelectedIndex = 0));
                dgFuelOilTanks.SelectedIndex = TankID - 17;
                dgFuelOilTanks.ScrollIntoView(dgFuelOilTanks.Items[TankID - 17]);
                expander1.IsExpanded = false;
                expander2.IsExpanded = true;
                expander3.IsExpanded = false;
                expander4.IsExpanded = false;
               
                
            }
            if (TankID >= 47 && TankID <= 54)
            {
                Dispatcher.BeginInvoke((System.Action)(() => expander4.Background = Brushes.Blue));
                Dispatcher.BeginInvoke((System.Action)(() => tabControl4.SelectedIndex = 0));
                dgMiscTanks.SelectedIndex = TankID - 47;
                dgMiscTanks.ScrollIntoView(dgMiscTanks.Items[TankID - 47]);
                expander1.IsExpanded = false;
                expander2.IsExpanded = false;
                expander3.IsExpanded = false;
                expander4.IsExpanded = true;
            }

            if (TankID >= 56 && TankID <= 57)
            {
                Dispatcher.BeginInvoke((System.Action)(() => tabControl4.SelectedIndex = 2));
                dgWTRegion.SelectedIndex = TankID - 56;
                dgWTRegion.ScrollIntoView(dgWTRegion.Items[TankID - 56]);
            }
            //if (TankID >= 58 && TankID <= 63)
            //{
            //    Dispatcher.BeginInvoke((System.Action)(() => tabControl4.SelectedIndex = 1));
            //    dgCompartments.SelectedIndex = TankID - 58;
            //    dgCompartments.ScrollIntoView(dgCompartments.Items[TankID - 58]);
            //}

            //if (TankID == 64 || TankID == 65)
            //{
            //    Dispatcher.BeginInvoke((System.Action)(() => tabControl4.SelectedIndex = 2));
            //    dgWTRegion.SelectedIndex = TankID - 62;
            //    dgWTRegion.ScrollIntoView(dgWTRegion.Items[TankID - 62]);
            //}

            //if (TankID >= 56 && TankID <= 70)
            //{
            //    Dispatcher.BeginInvoke((System.Action)(() => tabControl4.SelectedIndex = 1));
            //    dgCompartments.SelectedIndex = TankID - 55;
            //    dgCompartments.ScrollIntoView(dgCompartments.Items[TankID - 55]);
            //}

            if (TankID >= 154 && TankID <= 157)
           //if (TankID >= 150 && TankID <= 153)
            {
                Dispatcher.BeginInvoke((System.Action)(() => tabControl4.SelectedIndex = 2));
                dgWTRegion.SelectedIndex = TankID - 150;
                dgWTRegion.ScrollIntoView(dgWTRegion.Items[TankID - 150]);
            }

            if (TankID >= 158 && TankID <= 434)
            // if (TankID >= 154 && TankID <= 428)
            {
                Dispatcher.BeginInvoke((System.Action)(() => tabControl4.SelectedIndex = 1));
                dgCompartments.SelectedIndex = TankID - 64;
                dgCompartments.ScrollIntoView(dgCompartments.Items[TankID - 64]);
            }
            if (TankID >= 435 && TankID <= 504)
                //if (TankID >= 429 && TankID <= 498)
            {
                Dispatcher.BeginInvoke((System.Action)(() => tabControl4.SelectedIndex = 2));
                dgWTRegion.SelectedIndex = TankID - 427;
                dgWTRegion.ScrollIntoView(dgWTRegion.Items[TankID - 427]);
            }
            if(TankID==55)
            {
                Dispatcher.BeginInvoke((System.Action)(() => expander3.Background = Brushes.Blue));
                Dispatcher.BeginInvoke((System.Action)(() => tabControl4.SelectedIndex = 0));
                dgFreshWaterTanks.SelectedIndex = 5;
                dgFreshWaterTanks.ScrollIntoView(dgFreshWaterTanks.Items[5]);
                expander1.IsExpanded = false;
                expander2.IsExpanded = false;
                expander3.IsExpanded = true;
                expander4.IsExpanded = false;

            }

            if (TankID >= 58 && TankID <=63)
            {
                Dispatcher.BeginInvoke((System.Action)(() => tabControl4.SelectedIndex = 1));
                dgCompartments.SelectedIndex = TankID - 58;
                dgCompartments.ScrollIntoView(dgCompartments.Items[TankID - 58]);

            }

            if (TankID >= 66 && TankID <= 153)
            {
                Dispatcher.BeginInvoke((System.Action)(() => tabControl4.SelectedIndex = 1));
                dgCompartments.SelectedIndex = TankID - 60;
                dgCompartments.ScrollIntoView(dgCompartments.Items[TankID - 60]);

            }
            if (TankID >= 158 && TankID <= 434)
            {
                Dispatcher.BeginInvoke((System.Action)(() => tabControl4.SelectedIndex = 1));
                dgCompartments.SelectedIndex = TankID - 64;
                dgCompartments.ScrollIntoView(dgCompartments.Items[TankID - 64]);

            }
            if (TankID >= 507 && TankID <= 515)
            {
                Dispatcher.BeginInvoke((System.Action)(() => tabControl4.SelectedIndex = 1));
                dgCompartments.SelectedIndex = TankID - 136;
                dgCompartments.ScrollIntoView(dgCompartments.Items[TankID - 136]);

            }


        }

        public void Propertyset()
        {
            expander1.Background = Brushes.Gray;
            expander1.BorderBrush = Brushes.Gray;
            expander2.Background = Brushes.Gray;
            expander2.BorderBrush = Brushes.Gray;
            expander3.Background = Brushes.Gray;
            expander3.BorderBrush = Brushes.Gray;
            expander4.Background = Brushes.Gray;
            expander4.BorderBrush = Brushes.Gray;
        }

        private void btnclear_Click(object sender, RoutedEventArgs e)
        {
            #region Cargo
            {
                DataTable ballasttank = Models.BO.clsGlobVar.dtSimulationCargoTanks;
                DataColumn dc = new DataColumn("Volume");
                DataColumn dcStatus = new DataColumn("Status");
                dc.DataType = typeof(int);
                dcStatus.DataType = typeof(int);
                dcStatus.DefaultValue = 0;
                dc.DefaultValue = 0;
                ballasttank.Columns.Remove("Volume");
                ballasttank.Columns.Remove("Status");
                ballasttank.Columns.Add(dc);
                ballasttank.Columns.Add(dcStatus);
                dgCargoTanks.ItemsSource = ballasttank.DefaultView;
            }
            #endregion

            #region Ballast
            {
                DataTable ballasttank = Models.BO.clsGlobVar.dtSimulationBallastTanks;
                DataColumn dc = new DataColumn("Volume");
                DataColumn dcStatus = new DataColumn("Status");
                dc.DataType = typeof(int);
                dcStatus.DataType = typeof(int);
                dcStatus.DefaultValue = 0;
                dc.DefaultValue = 0;
                ballasttank.Columns.Remove("Volume");
                ballasttank.Columns.Remove("Status");
                ballasttank.Columns.Add(dc);
                ballasttank.Columns.Add(dcStatus);
                dgBallastTanks.ItemsSource = ballasttank.DefaultView;
            }
            #endregion

            #region FOTank
            {
                DataTable FOtank = Models.BO.clsGlobVar.dtSimulationFuelOilTanks;
                DataColumn dc = new DataColumn("Volume");
                DataColumn dcStatus = new DataColumn("Status");
                dc.DataType = typeof(int);
                dcStatus.DataType = typeof(int);
                dcStatus.DefaultValue = 0;
                dc.DefaultValue = 0;
                FOtank.Columns.Remove("Volume");
                FOtank.Columns.Remove("Status");
                FOtank.Columns.Add(dc);
                FOtank.Columns.Add(dcStatus);
                dgFuelOilTanks.ItemsSource = FOtank.DefaultView;
            }
            #endregion

            #region FWTank
            {
                DataTable FWtank = Models.BO.clsGlobVar.dtSimulationFreshWaterTanks;
                DataColumn dc = new DataColumn("Volume");
                DataColumn dcStatus = new DataColumn("Status");
                dc.DataType = typeof(int);
                dcStatus.DataType = typeof(int);
                dcStatus.DefaultValue = 0;
                dc.DefaultValue = 0;
                FWtank.Columns.Remove("Volume");
                FWtank.Columns.Remove("Status");
                FWtank.Columns.Add(dc);
                FWtank.Columns.Add(dcStatus);
                dgFreshWaterTanks.ItemsSource = FWtank.DefaultView;
            }
            #endregion

            #region MiscTank
            {
                DataTable Misctank = Models.BO.clsGlobVar.dtSimulationMiscTanks;
                DataColumn dc = new DataColumn("Volume");
                DataColumn dcStatus = new DataColumn("Status");
                dc.DataType = typeof(int);
                dcStatus.DataType = typeof(int);
                dcStatus.DefaultValue = 0;
                dc.DefaultValue = 0;
                Misctank.Columns.Remove("Volume");
                Misctank.Columns.Remove("Status");
                Misctank.Columns.Add(dc);
                Misctank.Columns.Add(dcStatus);
                dgMiscTanks.ItemsSource = Misctank.DefaultView;
            }
            #endregion

            #region Cmp
            {
                DataTable cmp = Models.BO.clsGlobVar.dtSimulationCompartments;
                DataColumn dc = new DataColumn("Volume");
                DataColumn dcStatus = new DataColumn("Status");
                dc.DataType = typeof(int);
                dcStatus.DataType = typeof(int);
                dcStatus.DefaultValue = 0;
                dc.DefaultValue = 0;
                cmp.Columns.Remove("Volume");
                cmp.Columns.Remove("Status");
                cmp.Columns.Add(dc);
                cmp.Columns.Add(dcStatus);
                dgCompartments.ItemsSource = cmp.DefaultView;
            }
            #endregion

            #region WTRegion
            {
                DataTable WTR = Models.BO.clsGlobVar.dtSimulationWTRegion;
                DataColumn dc = new DataColumn("Volume");
                DataColumn dcStatus = new DataColumn("Status");
                dc.DataType = typeof(int);
                dcStatus.DataType = typeof(int);
                dcStatus.DefaultValue = 0;
                dc.DefaultValue = 0;
                WTR.Columns.Remove("Volume");
                WTR.Columns.Remove("Status");
                WTR.Columns.Add(dc);
                WTR.Columns.Add(dcStatus);
                dgWTRegion.ItemsSource = WTR.DefaultView;
            }
            #endregion

            DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
            string Err = "";
            string query = " update [tblSimulationMode_Loading_Condition] SET Volume =0,IsDamaged = 0,Status=0; update [tblSimulationMode_Tank_Status] SET Volume =0 , IsDamaged =0";
            command.CommandText = query;
            command.CommandType = CommandType.Text;
            Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);
            MessageBox.Show(" All Data Cleared ");
        }

        private void dgBallastTanks_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            selectionchangecount = 0;
            Models.TableModel.Write_Log(" START WHR SELECTION = 1 : dgBallastTanks_SelectionChanged_1");
            try
            {
                int TankId,CellCnt;  

                string Err = string.Empty;
                string status = string.Empty;
                if (dgBallastTanks.IsLoaded == true)
                {
                    DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();

                    var RowsCnt= dgBallastTanks.SelectedItems;

                    if (RowsCnt.Count > 1 )
                    {
                        Models.TableModel.Write_Log(" START : dgBallastTanks_SelectionChanged_1");
                        PK = 1;
                          DataRowView row = (DataRowView)dgBallastTanks.SelectedItems[0];
                         // DataGridCell ThirdColumnInFirstRow = dgBallastTanks.Columns[2].GetCellContent(row).Parent as DataGridCell;

                          //ThirdColumnInFirstRow.Background = Brushes.LightPink;
                          string selectedStatus = row["status"].ToString();
                     
                          TankId = Convert.ToInt16(row["Tank_Id"]);
                          string query=" ";
                         for (int i = 1; i < RowsCnt.Count; i++)
                         {
                             DataRowView mulRow = (DataRowView)RowsCnt[i];
                             mulRow["Status"] = selectedStatus;
                             //DataGridCell SelectedColumnInSelectedRow = dgBallastTanks.Columns[2].GetCellContent(mulRow).Parent as DataGridCell;
                             //SelectedColumnInSelectedRow.Background = Brushes.LightPink;
                         
                             int TankIdMul= Convert.ToInt16(mulRow["tank_Id"]);
                             query += "update  tblSimulationMode_Loading_Condition set Status=" + selectedStatus.ToString() + " where Tank_ID=" + TankIdMul;
                             if (selectedStatus == "0")
                             {
                                 query += "  update  tblSimulationMode_Loading_Condition set IsDamaged=0 where Tank_ID=" + TankIdMul;
                                 query += "  update  tblSimulationMode_Tank_Status set IsDamaged=0 where Tank_ID=" + TankIdMul;
                             }
                             else
                             {
                                 query += "  update  tblSimulationMode_Loading_Condition set IsDamaged=1 where Tank_ID=" + TankIdMul;
                                 query += "  update  tblSimulationMode_Tank_Status set IsDamaged=1 where Tank_ID=" + TankIdMul;
                             }
                             command.CommandText = query;
                             command.CommandType = CommandType.Text;
                             Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);
                         }
                        query = "update  tblSimulationMode_Loading_Condition set Status=" + selectedStatus.ToString() + " where Tank_ID=" + TankId;
                         if (selectedStatus == "0")
                         {
                             query += "  update  tblSimulationMode_Loading_Condition set IsDamaged=0 where Tank_ID=" +TankId;
                             query += "  update  tblSimulationMode_Tank_Status set IsDamaged=0 where Tank_ID=" + TankId;
                         }
                         else
                         {
                             query += "  update  tblSimulationMode_Loading_Condition set IsDamaged=1 where Tank_ID=" + TankId;
                             query += "  update  tblSimulationMode_Tank_Status set IsDamaged=1 where Tank_ID=" + TankId;

                         }
                         command.CommandText = query;
                         command.CommandType = CommandType.Text;
                        Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);
                    }
                    Models.TableModel.Write_Log(" END : dgBallastTanks_SelectionChanged_1");
                }
            }
            catch { }
        }

        private void dgFuelOilTanks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Models.TableModel.Write_Log(" START WHN SELECTION = 1 : dgFuelOilTanks_SelectionChanged");
            try
            {
                int TankId, CellCnt;

                string Err = string.Empty;
                string status = string.Empty;
                if (dgFuelOilTanks.IsLoaded == true)
                {
                    DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
                    var RowsCnt = dgFuelOilTanks.SelectedItems;

                    if (RowsCnt.Count > 1)
                    {
                        Models.TableModel.Write_Log(" START  : dgFuelOilTanks_SelectionChanged");
                        DataRowView row = (DataRowView)dgFuelOilTanks.SelectedItems[0];
                        string selectedStatus = row["status"].ToString();
                        TankId = Convert.ToInt16(row["Tank_Id"]);
                        string query = " ";
                        for (int i = 1; i < RowsCnt.Count; i++)
                        {
                            DataRowView mulRow = (DataRowView)RowsCnt[i];
                            mulRow["Status"] = selectedStatus;
                            int TankIdMul = Convert.ToInt16(mulRow["tank_Id"]);
                            query += "update  tblSimulationMode_Loading_Condition set Status=" + selectedStatus.ToString() + " where Tank_ID=" + TankIdMul;
                            if (selectedStatus == "0")
                            {
                                query += "  update  tblSimulationMode_Loading_Condition set IsDamaged=0 where Tank_ID=" + TankIdMul;
                                query += "  update  tblSimulationMode_Tank_Status set IsDamaged=0 where Tank_ID=" + TankIdMul;
                            }
                            else
                            {
                                query += "  update  tblSimulationMode_Loading_Condition set IsDamaged=1 where Tank_ID=" + TankIdMul;
                                query += "  update  tblSimulationMode_Tank_Status set IsDamaged=1 where Tank_ID=" + TankIdMul;
                            }
                            command.CommandText = query;
                            command.CommandType = CommandType.Text;
                            Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);
                        }
                        query = "update  tblSimulationMode_Loading_Condition set Status=" + selectedStatus.ToString() + " where Tank_ID=" + TankId;
                        if (selectedStatus == "0")
                        {
                            query += "  update  tblSimulationMode_Loading_Condition set IsDamaged=0 where Tank_ID=" + TankId;
                            query += "  update  tblSimulationMode_Tank_Status set IsDamaged=0 where Tank_ID=" + TankId;
                        }
                        else
                        {
                            query += "  update  tblSimulationMode_Loading_Condition set IsDamaged=1 where Tank_ID=" + TankId;
                            query += "  update  tblSimulationMode_Tank_Status set IsDamaged=1 where Tank_ID=" + TankId;

                        }
                        command.CommandText = query;
                        command.CommandType = CommandType.Text;
                        Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);
                    }
                    Models.TableModel.Write_Log(" END : dgFuelOilTanks_SelectionChanged");
                }
            }
            catch { }
        }

        private void dgFreshWaterTanks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Models.TableModel.Write_Log(" START WHN SELECTION = 1 : dgFreshWaterTanks_SelectionChanged");
            try
            {
                int TankId, CellCnt;

                string Err = string.Empty;
                string status = string.Empty;
                if  (dgFreshWaterTanks.IsLoaded == true)
                {
                    DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
                    var RowsCnt = dgFreshWaterTanks.SelectedItems;

                    if (RowsCnt.Count > 1)
                    {
                        Models.TableModel.Write_Log(" START : dgFreshWaterTanks_SelectionChanged");
                        DataRowView row = (DataRowView)dgFreshWaterTanks.SelectedItems[0];
                        string selectedStatus = row["status"].ToString();
                        TankId = Convert.ToInt16(row["Tank_Id"]);
                        string query = " ";
                        for (int i = 1; i < RowsCnt.Count; i++)
                        {
                            DataRowView mulRow = (DataRowView)RowsCnt[i];
                            mulRow["Status"] = selectedStatus;
                            int TankIdMul = Convert.ToInt16(mulRow["tank_Id"]);
                            query += "update  tblSimulationMode_Loading_Condition set Status=" + selectedStatus.ToString() + " where Tank_ID=" + TankIdMul;
                            if (selectedStatus == "0")
                            {
                                query += "  update  tblSimulationMode_Loading_Condition set IsDamaged=0 where Tank_ID=" + TankIdMul;
                                query += "  update  tblSimulationMode_Tank_Status set IsDamaged=0 where Tank_ID=" + TankIdMul;
                            }
                            else
                            {
                                query += "  update  tblSimulationMode_Loading_Condition set IsDamaged=1 where Tank_ID=" + TankIdMul;
                                query += "  update  tblSimulationMode_Tank_Status set IsDamaged=1 where Tank_ID=" + TankIdMul;
                            }
                            command.CommandText = query;
                            command.CommandType = CommandType.Text;
                            Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);
                        }
                        query = "update  tblSimulationMode_Loading_Condition set Status=" + selectedStatus.ToString() + " where Tank_ID=" + TankId;
                        if (selectedStatus == "0")
                        {
                            query += "  update  tblSimulationMode_Loading_Condition set IsDamaged=0 where Tank_ID=" + TankId;
                            query += "  update  tblSimulationMode_Tank_Status set IsDamaged=0 where Tank_ID=" + TankId;
                        }
                        else
                        {
                            query += "  update  tblSimulationMode_Loading_Condition set IsDamaged=1 where Tank_ID=" + TankId;
                            query += "  update  tblSimulationMode_Tank_Status set IsDamaged=1 where Tank_ID=" + TankId;

                        }
                        command.CommandText = query;
                        command.CommandType = CommandType.Text;
                        Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);
                        Models.TableModel.Write_Log(" END : dgFreshWaterTanks_SelectionChanged");
                    }

                }
            }
            catch { }
        }

        private void dgMiscTanks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                int TankId, CellCnt;

                string Err = string.Empty;
                string status = string.Empty;
                if (dgMiscTanks.IsLoaded == true)
                {
                    DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
                    var RowsCnt = dgMiscTanks.SelectedItems;

                    if (RowsCnt.Count > 1)
                    {
                        DataRowView row = (DataRowView)dgMiscTanks.SelectedItems[0];
                        string selectedStatus = row["status"].ToString();
                        TankId = Convert.ToInt16(row["Tank_Id"]);
                        string query = " ";
                        for (int i = 1; i < RowsCnt.Count; i++)
                        {
                            DataRowView mulRow = (DataRowView)RowsCnt[i];
                            mulRow["Status"] = selectedStatus;
                            int TankIdMul = Convert.ToInt16(mulRow["tank_Id"]);
                            query += "update  tblSimulationMode_Loading_Condition set Status=" + selectedStatus.ToString() + " where Tank_ID=" + TankIdMul;
                            if (selectedStatus == "0")
                            {
                                query += "  update  tblSimulationMode_Loading_Condition set IsDamaged=0 where Tank_ID=" + TankIdMul;
                                query += "  update  tblSimulationMode_Tank_Status set IsDamaged=0 where Tank_ID=" + TankIdMul;
                            }
                            else
                            {
                                query += "  update  tblSimulationMode_Loading_Condition set IsDamaged=1 where Tank_ID=" + TankIdMul;
                                query += "  update  tblSimulationMode_Tank_Status set IsDamaged=1 where Tank_ID=" + TankIdMul;
                            }
                            command.CommandText = query;
                            command.CommandType = CommandType.Text;
                            Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);
                        }
                        query = "update  tblSimulationMode_Loading_Condition set Status=" + selectedStatus.ToString() + " where Tank_ID=" + TankId;
                        if (selectedStatus == "0")
                        {
                            query += "  update  tblSimulationMode_Loading_Condition set IsDamaged=0 where Tank_ID=" + TankId;
                            query += "  update  tblSimulationMode_Tank_Status set IsDamaged=0 where Tank_ID=" + TankId;
                        }
                        else
                        {
                            query += "  update  tblSimulationMode_Loading_Condition set IsDamaged=1 where Tank_ID=" + TankId;
                            query += "  update  tblSimulationMode_Tank_Status set IsDamaged=1 where Tank_ID=" + TankId;

                        }
                        command.CommandText = query;
                        command.CommandType = CommandType.Text;
                        Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);
                    }

                }
            }
            catch { }
        }


        public void LoadingFixedSave()
        {
            dgCargoTanks.ItemsSource = Models.BO.clsGlobVar.dtSimulationCargoTanks.DefaultView;
            dgBallastTanks.ItemsSource = Models.BO.clsGlobVar.dtSimulationBallastTanks.DefaultView;
            dgFreshWaterTanks.ItemsSource = Models.BO.clsGlobVar.dtSimulationFreshWaterTanks.DefaultView;
            dgFuelOilTanks.ItemsSource = Models.BO.clsGlobVar.dtSimulationFuelOilTanks.DefaultView;
            dgMiscTanks.ItemsSource = Models.BO.clsGlobVar.dtSimulationMiscTanks.DefaultView;
            dgCompartments.ItemsSource = Models.BO.clsGlobVar.dtSimulationCompartments.DefaultView;
            dgVariableItems.ItemsSource = Models.BO.clsGlobVar.dtSimulationVariableItems.DefaultView;
            dgWTRegion.ItemsSource = Models.BO.clsGlobVar.dtSimulationWTRegion.DefaultView;
        }

        private void dgVariableItems_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
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

        private void tabControl2_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //Models.BO.clsGlobVar._Damageswitch = true;
            //MainWindow gg = new MainWindow();
            //    gg.Visibility = Visibility.Hidden;
        }



        private void btnclearBallast_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                 #region Ballast
                {
                    DataTable ballasttank = Models.BO.clsGlobVar.dtSimulationBallastTanks;
                    DataColumn dc = new DataColumn("Volume");
                    DataColumn dcStatus = new DataColumn("Status");
                    dc.DataType = typeof(int);
                    dcStatus.DataType = typeof(int);
                    dcStatus.DefaultValue = 0;
                    dc.DefaultValue = 0;
                    ballasttank.Columns.Remove("Volume");
                    ballasttank.Columns.Remove("Status");
                    ballasttank.Columns.Add(dc);
                    ballasttank.Columns.Add(dcStatus);
                    dgBallastTanks.ItemsSource = ballasttank.DefaultView;
                }
                #endregion
                DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
                string Err = "";

                for (int i = 0; i < Models.BO.clsGlobVar.dtSimulationBallastTanks.Rows.Count; i++)
                {

                    ((dgBallastTanks.Items[i] as DataRowView)["Volume"]) = 0;
                    ((dgBallastTanks.Items[i] as DataRowView)["Percent_Full"]) = 0;
                    ((dgBallastTanks.Items[i] as DataRowView)["Sounding_Level"]) = 0;
                    ((dgBallastTanks.Items[i] as DataRowView)["Weight"]) = 0;

                }

                string query = "update tblSimulationMode_Tank_Status SET Volume =0 , IsDamaged =0 where Tank_ID in(select Tank_ID  from tblmaster_tank where [group]='BALLAST_TANK');" +
                                " update [tblSimulationMode_Loading_Condition] SET Volume =0,IsDamaged = 0,Status=0 where Tank_ID in(select Tank_ID  from tblmaster_tank where [group]='BALLAST_TANK')";
                command = Models.DAL.clsDBUtilityMethods.GetCommand();
                command.CommandText = query;
                command.CommandType = CommandType.Text;
                Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);
                MessageBox.Show("Ballast Tanks Cleared ");
            }
            catch //(Exception)
            {
                
               // throw;
            }
        }

        private void btnclearFuelOil_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                #region FOTank
                {
                    DataTable FOtank = Models.BO.clsGlobVar.dtSimulationFuelOilTanks;
                    DataColumn dc = new DataColumn("Volume");
                    DataColumn dcStatus = new DataColumn("Status");
                    dc.DataType = typeof(int);
                    dcStatus.DataType = typeof(int);
                    dcStatus.DefaultValue = 0;
                    dc.DefaultValue = 0;
                    FOtank.Columns.Remove("Volume");
                    FOtank.Columns.Remove("Status");
                    FOtank.Columns.Add(dc);
                    FOtank.Columns.Add(dcStatus);
                    dgFuelOilTanks.ItemsSource = FOtank.DefaultView;
                }
                #endregion

                DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
                string Err = "";

                for (int i = 0; i < Models.BO.clsGlobVar.dtSimulationFuelOilTanks.Rows.Count; i++)
                {
                    ((dgFuelOilTanks.Items[i] as DataRowView)["Volume"]) = 0;

                    ((dgFuelOilTanks.Items[i] as DataRowView)["Percent_Full"]) = 0;
                    ((dgFuelOilTanks.Items[i] as DataRowView)["Sounding_Level"]) = 0;
                    ((dgFuelOilTanks.Items[i] as DataRowView)["Weight"]) = 0;

                }

                string query = "update tblSimulationMode_Tank_Status SET Volume =0 , IsDamaged =0 where Tank_ID in(select Tank_ID  from tblmaster_tank where [group]='FUELOIL_TANK');" +
                                "update [tblSimulationMode_Loading_Condition] SET Volume =0,IsDamaged = 0,Status=0 where Tank_ID in(select Tank_ID  from tblmaster_tank where [group]='FUELOIL_TANK')";
                command = Models.DAL.clsDBUtilityMethods.GetCommand();
                command.CommandText = query;
                command.CommandType = CommandType.Text;
                Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);
                MessageBox.Show("Fuel Oil Tanks Cleared ");
            }
            catch //(Exception)
            {

                //throw;
            }
        }

        private void btnclearFreshWater_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
 #region FWTank
            {
                DataTable FWtank = Models.BO.clsGlobVar.dtSimulationFreshWaterTanks;
                DataColumn dc = new DataColumn("Volume");
                DataColumn dcStatus = new DataColumn("Status");
                dc.DataType = typeof(int);
                dcStatus.DataType = typeof(int);
                dcStatus.DefaultValue = 0;
                dc.DefaultValue = 0;
                FWtank.Columns.Remove("Volume");
                FWtank.Columns.Remove("Status");
                FWtank.Columns.Add(dc);
                FWtank.Columns.Add(dcStatus);
                dgFreshWaterTanks.ItemsSource = FWtank.DefaultView;
            }
            #endregion


            DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
            string Err = "";

            for (int i = 0; i < Models.BO.clsGlobVar.dtSimulationFreshWaterTanks.Rows.Count; i++)
            {
                ((dgFreshWaterTanks.Items[i] as DataRowView)["Volume"]) = 0;
                ((dgFreshWaterTanks.Items[i] as DataRowView)["Percent_Full"]) = 0;
                ((dgFreshWaterTanks.Items[i] as DataRowView)["Sounding_Level"]) = 0;
                ((dgFreshWaterTanks.Items[i] as DataRowView)["Weight"]) = 0;

            }

            string query = "update tblSimulationMode_Tank_Status SET Volume =0 , IsDamaged =0 where Tank_ID  in(select Tank_ID  from tblmaster_tank where [group]='FRESHWATER_TANK');" +
                            "update [tblSimulationMode_Loading_Condition] SET Volume =0,IsDamaged = 0,Status=0 where Tank_ID in(select Tank_ID  from tblmaster_tank where [group]='FRESHWATER_TANK')";
            command = Models.DAL.clsDBUtilityMethods.GetCommand();
            command.CommandText = query;
            command.CommandType = CommandType.Text;
            Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);

            MessageBox.Show("Fresh Water Tanks Cleared ");
            }
            catch //(Exception)
            {
                
               // throw;
            }
        }

        private void btnclearMisc_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                #region MiscTank
                {
                    DataTable Misctank = Models.BO.clsGlobVar.dtSimulationMiscTanks;
                    DataColumn dc = new DataColumn("Volume");
                    DataColumn dcStatus = new DataColumn("Status");
                    dc.DataType = typeof(int);
                    dcStatus.DataType = typeof(int);
                    dcStatus.DefaultValue = 0;
                    dc.DefaultValue = 0;
                    Misctank.Columns.Remove("Volume");
                    Misctank.Columns.Remove("Status");
                    Misctank.Columns.Add(dc);
                    Misctank.Columns.Add(dcStatus);
                    dgMiscTanks.ItemsSource = Misctank.DefaultView;
                }
                #endregion



                DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
                string Err = "";

                for (int i = 0; i < Models.BO.clsGlobVar.dtSimulationMiscTanks.Rows.Count; i++)
                {

                    ((dgMiscTanks.Items[i] as DataRowView)["Volume"]) = 0;
                    ((dgMiscTanks.Items[i] as DataRowView)["Percent_Full"]) = 0;
                    ((dgMiscTanks.Items[i] as DataRowView)["Sounding_Level"]) = 0;
                    ((dgMiscTanks.Items[i] as DataRowView)["Weight"]) = 0;

                }


                string query = "update tblSimulationMode_Tank_Status SET Volume =0 , IsDamaged =0 where Tank_ID in(select Tank_ID  from tblmaster_tank where [group]='MISC_TANK') ;" +
                                "update [tblSimulationMode_Loading_Condition] SET Volume =0,IsDamaged = 0,Status=0 where Tank_ID in(select Tank_ID  from tblmaster_tank where [group]='MISC_TANK')";
                command = Models.DAL.clsDBUtilityMethods.GetCommand();
                command.CommandText = query;
                command.CommandType = CommandType.Text;
                Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);
                MessageBox.Show("Miscellaneous Tanks Cleared ");
            }
            catch //(Exception)
            {

              //  throw;
            }
        }

        private void CollapseExpander(object sender, RoutedEventArgs e)
        {
            
        }   

        private void expander1_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void expander1_Collapsed(object sender, RoutedEventArgs e)
        {
            
        }


    }

    public class SelectionProcess: IComparable<SelectionProcess>
    {
        public double ID { get; set; }
       public double MaxValue { get; set; }
       public double MinValue { get; set; }
       public double SmallValue { get; set; }

       public int CompareTo(SelectionProcess other)
       {
           if (this.SmallValue > other.SmallValue)
           {
               return 1;
           }
           else if (this.SmallValue < other.SmallValue)
           {
               return -1;
           }
           else
               return 0;
       }
    }

   
}

    
   


