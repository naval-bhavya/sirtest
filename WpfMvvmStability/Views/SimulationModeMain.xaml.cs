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
                StatusBallast.ItemsSource = statusList;
                StatusFresh.ItemsSource = statusList;
                StatusFuel.ItemsSource = statusList;
                StatusMisc.ItemsSource = statusList;
                StatusWTRegion.ItemsSource = statusList;

                //var x=Models.BO.clsGlobVar.dtSimulationBallastTanks.DefaultView;
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
                Model(Models.BO.clsGlobVar.PlanC, canvas2DPlanC);
                ModelNew(Models.BO.clsGlobVar.PlanALL, canvas2DAll);


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
            viewPort3d.ZoomExtents();
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
            else if (sender == canvas2DPlanC)
            {
                UpdateRenderTransform(canvas2DPlanC);
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

            double canvasWidth = canvas2D.ActualWidth;
            double canvasHeight = canvas2D.ActualHeight;
            MatrixTransform baseTransform = DxfUtil.GetScaleWMMatrixTransform(
                (Point2D)bounds.Corner1,
                (Point2D)bounds.Corner2,
                (Point2D)bounds.Center,
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
            UpdateRenderTransformNew(canvas2DAll);
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

            if (dgBallastTanks.IsLoaded==true || dgFreshWaterTanks.IsLoaded==true|| dgFuelOilTanks.IsLoaded==true ||dgMiscTanks.IsLoaded==true || dgCompartments.IsLoaded==true || dgWTRegion.IsLoaded==true )
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
            canvas2DAll.Children.RemoveRange(1, canvas2DAll.Children.Count - 1);
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
                   // Models.TableModel.simulationmodeCorrectiveFill();

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
            if (viewPort3d.ActualWidth > 0 && viewPort3d.ActualHeight > 0)
            {
                viewPort3d.ZoomExtents();
                System.Diagnostics.Debug.WriteLine($"ZoomExtents called - viewport size: {viewPort3d.ActualWidth} x {viewPort3d.ActualHeight}");
            }
        }

        private bool _zoomOnFirstShow = false;

        private void viewPort3d_Loaded(object sender, RoutedEventArgs e)
        {
            viewPort3d.EffectsManager = new DefaultEffectsManager();
            System.Diagnostics.Debug.WriteLine($"Viewport size: {viewPort3d.ActualWidth} x {viewPort3d.ActualHeight}");
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 2);
            dispatcherTimer.Start();

            // If viewport has no size yet (3D tab not selected), wait for first real size to ZoomExtents
            if (viewPort3d.ActualWidth == 0)
            {
                _zoomOnFirstShow = true;
                viewPort3d.SizeChanged += ViewPort3d_SizeChanged;
            }
        }

        private void ViewPort3d_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width > 0 && e.NewSize.Height > 0 && _zoomOnFirstShow)
            {
                _zoomOnFirstShow = false;
                viewPort3d.SizeChanged -= ViewPort3d_SizeChanged;
                var t = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromMilliseconds(400) };
                t.Tick += (s, ev) => { t.Stop(); viewPort3d.ZoomExtents(animationTime: 300); };
                t.Start();
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
                System.Windows.Media.Color flag = (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[0]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 141, 180, 227));
              
                DrawHatchDeckPlan(canvas2DPlanB, 80, clsGlobVar.Tank80_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank80ax, clsGlobVar.CoordinatePlanB.Tank80ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 80, clsGlobVar.Tank80_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank80bx, clsGlobVar.CoordinatePlanB.Tank80by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 81, clsGlobVar.Tank81_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank81x, clsGlobVar.CoordinatePlanB.Tank81y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 82, clsGlobVar.Tank82_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank82x, clsGlobVar.CoordinatePlanB.Tank82y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 85, clsGlobVar.Tank85_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank85ax, clsGlobVar.CoordinatePlanB.Tank85ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 85, clsGlobVar.Tank85_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank85bx, clsGlobVar.CoordinatePlanB.Tank85by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 91, clsGlobVar.Tank91_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank91x, clsGlobVar.CoordinatePlanB.Tank91y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 92, clsGlobVar.Tank92_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank92x, clsGlobVar.CoordinatePlanB.Tank92y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 93, clsGlobVar.Tank93_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank93x, clsGlobVar.CoordinatePlanB.Tank93y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 94, clsGlobVar.Tank94_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank94x, clsGlobVar.CoordinatePlanB.Tank94y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 95, clsGlobVar.Tank95_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank95x, clsGlobVar.CoordinatePlanB.Tank95y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 96, clsGlobVar.Tank96_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank96x, clsGlobVar.CoordinatePlanB.Tank96y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 97, clsGlobVar.Tank97_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank97x, clsGlobVar.CoordinatePlanB.Tank97y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 98, clsGlobVar.Tank98_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank98x, clsGlobVar.CoordinatePlanB.Tank98y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 99, clsGlobVar.Tank99_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank99x, clsGlobVar.CoordinatePlanB.Tank99y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 100, clsGlobVar.Tank100_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank100x, clsGlobVar.CoordinatePlanB.Tank100y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 101, clsGlobVar.Tank101_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank101ax, clsGlobVar.CoordinatePlanB.Tank101ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 101, clsGlobVar.Tank101_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank101bx, clsGlobVar.CoordinatePlanB.Tank101by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 102, clsGlobVar.Tank102_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank102x, clsGlobVar.CoordinatePlanB.Tank102y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 103, clsGlobVar.Tank103_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank103x, clsGlobVar.CoordinatePlanB.Tank103y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 104, clsGlobVar.Tank104_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank104x, clsGlobVar.CoordinatePlanB.Tank104y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 105, clsGlobVar.Tank105_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank105x, clsGlobVar.CoordinatePlanB.Tank105y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 106, clsGlobVar.Tank106_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank106x, clsGlobVar.CoordinatePlanB.Tank106y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 107, clsGlobVar.Tank107_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank107x, clsGlobVar.CoordinatePlanB.Tank107y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 108, clsGlobVar.Tank108_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank108x, clsGlobVar.CoordinatePlanB.Tank108y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 109, clsGlobVar.Tank109_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank109x, clsGlobVar.CoordinatePlanB.Tank109y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 110, clsGlobVar.Tank110_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank110x, clsGlobVar.CoordinatePlanB.Tank110y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 112, clsGlobVar.Tank112_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank112x, clsGlobVar.CoordinatePlanB.Tank112y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 113, clsGlobVar.Tank113_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank113x, clsGlobVar.CoordinatePlanB.Tank113y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 114, clsGlobVar.Tank114_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank114x, clsGlobVar.CoordinatePlanB.Tank114y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 115, clsGlobVar.Tank115_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank115x, clsGlobVar.CoordinatePlanB.Tank115y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 116, clsGlobVar.Tank116_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank116x, clsGlobVar.CoordinatePlanB.Tank116y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 117, clsGlobVar.Tank117_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank117ax, clsGlobVar.CoordinatePlanB.Tank117ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 117, clsGlobVar.Tank117_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank117bx, clsGlobVar.CoordinatePlanB.Tank117by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 118, clsGlobVar.Tank118_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank118x, clsGlobVar.CoordinatePlanB.Tank118y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 119, clsGlobVar.Tank119_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank119x, clsGlobVar.CoordinatePlanB.Tank119y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 120, clsGlobVar.Tank120_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank120x, clsGlobVar.CoordinatePlanB.Tank120y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 121, clsGlobVar.Tank121_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank121x, clsGlobVar.CoordinatePlanB.Tank121y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 122, clsGlobVar.Tank122_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank122x, clsGlobVar.CoordinatePlanB.Tank122y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 123, clsGlobVar.Tank123_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank123x, clsGlobVar.CoordinatePlanB.Tank123y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 124, clsGlobVar.Tank124_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank124x, clsGlobVar.CoordinatePlanB.Tank124y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 125, clsGlobVar.Tank125_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank125x, clsGlobVar.CoordinatePlanB.Tank125y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 126, clsGlobVar.Tank126_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank126x, clsGlobVar.CoordinatePlanB.Tank126y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 127, clsGlobVar.Tank127_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank127x, clsGlobVar.CoordinatePlanB.Tank127y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 128, clsGlobVar.Tank128_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank128x, clsGlobVar.CoordinatePlanB.Tank128y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 129, clsGlobVar.Tank129_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank129x, clsGlobVar.CoordinatePlanB.Tank129y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 130, clsGlobVar.Tank130_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank130x, clsGlobVar.CoordinatePlanB.Tank130y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 131, clsGlobVar.Tank131_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank131x, clsGlobVar.CoordinatePlanB.Tank131y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 132, clsGlobVar.Tank132_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank132x, clsGlobVar.CoordinatePlanB.Tank132y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 133, clsGlobVar.Tank133_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank133x, clsGlobVar.CoordinatePlanB.Tank133y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 134, clsGlobVar.Tank134_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank134x, clsGlobVar.CoordinatePlanB.Tank134y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 135, clsGlobVar.Tank135_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank135x, clsGlobVar.CoordinatePlanB.Tank135y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 136, clsGlobVar.Tank136_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank136x, clsGlobVar.CoordinatePlanB.Tank136y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 137, clsGlobVar.Tank137_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank137x, clsGlobVar.CoordinatePlanB.Tank137y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 138, clsGlobVar.Tank138_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank138x, clsGlobVar.CoordinatePlanB.Tank138y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 139, clsGlobVar.Tank139_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank139x, clsGlobVar.CoordinatePlanB.Tank139y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 140, clsGlobVar.Tank140_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank140x, clsGlobVar.CoordinatePlanB.Tank140y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 141, clsGlobVar.Tank141_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank141x, clsGlobVar.CoordinatePlanB.Tank141y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 150, clsGlobVar.Tank150_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank150x, clsGlobVar.CoordinatePlanB.Tank150y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 152, clsGlobVar.Tank152_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank152x, clsGlobVar.CoordinatePlanB.Tank152y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 153, clsGlobVar.Tank153_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank153x, clsGlobVar.CoordinatePlanB.Tank153y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 158, clsGlobVar.Tank158_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank158x, clsGlobVar.CoordinatePlanB.Tank158y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 159, clsGlobVar.Tank159_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank159x, clsGlobVar.CoordinatePlanB.Tank159y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 160, clsGlobVar.Tank160_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank160x, clsGlobVar.CoordinatePlanB.Tank160y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 161, clsGlobVar.Tank161_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank161x, clsGlobVar.CoordinatePlanB.Tank161y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 162, clsGlobVar.Tank162_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank162x, clsGlobVar.CoordinatePlanB.Tank162y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 163, clsGlobVar.Tank163_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank163x, clsGlobVar.CoordinatePlanB.Tank163y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 165, clsGlobVar.Tank165_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank165x, clsGlobVar.CoordinatePlanB.Tank165y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 166, clsGlobVar.Tank166_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank166ax, clsGlobVar.CoordinatePlanB.Tank166ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 166, clsGlobVar.Tank166_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank166bx, clsGlobVar.CoordinatePlanB.Tank166by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 167, clsGlobVar.Tank167_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank167x, clsGlobVar.CoordinatePlanB.Tank167y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 168, clsGlobVar.Tank168_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank168x, clsGlobVar.CoordinatePlanB.Tank168y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 169, clsGlobVar.Tank169_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank169x, clsGlobVar.CoordinatePlanB.Tank169y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 170, clsGlobVar.Tank170_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank170x, clsGlobVar.CoordinatePlanB.Tank170y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 171, clsGlobVar.Tank171_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank171x, clsGlobVar.CoordinatePlanB.Tank171y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 172, clsGlobVar.Tank172_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank172x, clsGlobVar.CoordinatePlanB.Tank172y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 173, clsGlobVar.Tank173_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank173x, clsGlobVar.CoordinatePlanB.Tank173y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 174, clsGlobVar.Tank174_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank174x, clsGlobVar.CoordinatePlanB.Tank174y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 175, clsGlobVar.Tank175_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank175x, clsGlobVar.CoordinatePlanB.Tank175y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 176, clsGlobVar.Tank176_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank176x, clsGlobVar.CoordinatePlanB.Tank176y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 177, clsGlobVar.Tank177_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank177ax, clsGlobVar.CoordinatePlanB.Tank177ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 177, clsGlobVar.Tank177_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank177bx, clsGlobVar.CoordinatePlanB.Tank177by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 178, clsGlobVar.Tank178_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank178x, clsGlobVar.CoordinatePlanB.Tank178y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 179, clsGlobVar.Tank179_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank179x, clsGlobVar.CoordinatePlanB.Tank179y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 181, clsGlobVar.Tank181_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank181ax, clsGlobVar.CoordinatePlanB.Tank181ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 181, clsGlobVar.Tank181_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank181bx, clsGlobVar.CoordinatePlanB.Tank181by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 182, clsGlobVar.Tank182_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank182x, clsGlobVar.CoordinatePlanB.Tank182y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 183, clsGlobVar.Tank183_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank183x, clsGlobVar.CoordinatePlanB.Tank183y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 184, clsGlobVar.Tank184_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank184x, clsGlobVar.CoordinatePlanB.Tank184y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 185, clsGlobVar.Tank185_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank185x, clsGlobVar.CoordinatePlanB.Tank185y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 187, clsGlobVar.Tank187_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank187ax, clsGlobVar.CoordinatePlanB.Tank187ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 187, clsGlobVar.Tank187_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank187bx, clsGlobVar.CoordinatePlanB.Tank187by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 188, clsGlobVar.Tank188_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank188x, clsGlobVar.CoordinatePlanB.Tank188y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 189, clsGlobVar.Tank189_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank189x, clsGlobVar.CoordinatePlanB.Tank189y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 190, clsGlobVar.Tank190_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank190x, clsGlobVar.CoordinatePlanB.Tank190y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 191, clsGlobVar.Tank191_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank191x, clsGlobVar.CoordinatePlanB.Tank191y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 192, clsGlobVar.Tank192_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank192x, clsGlobVar.CoordinatePlanB.Tank192y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 193, clsGlobVar.Tank193_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank193x, clsGlobVar.CoordinatePlanB.Tank193y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 194, clsGlobVar.Tank194_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank194x, clsGlobVar.CoordinatePlanB.Tank194y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 195, clsGlobVar.Tank195_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank195x, clsGlobVar.CoordinatePlanB.Tank195y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 196, clsGlobVar.Tank196_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank196x, clsGlobVar.CoordinatePlanB.Tank196y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 197, clsGlobVar.Tank197_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank197x, clsGlobVar.CoordinatePlanB.Tank197y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 198, clsGlobVar.Tank198_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank198ax, clsGlobVar.CoordinatePlanB.Tank198ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 198, clsGlobVar.Tank198_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank198bx, clsGlobVar.CoordinatePlanB.Tank198by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 199, clsGlobVar.Tank199_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank199x, clsGlobVar.CoordinatePlanB.Tank199y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 200, clsGlobVar.Tank200_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank200x, clsGlobVar.CoordinatePlanB.Tank200y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 201, clsGlobVar.Tank201_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank201x, clsGlobVar.CoordinatePlanB.Tank201y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 202, clsGlobVar.Tank202_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank202x, clsGlobVar.CoordinatePlanB.Tank202y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 203, clsGlobVar.Tank203_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank203x, clsGlobVar.CoordinatePlanB.Tank203y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 204, clsGlobVar.Tank204_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank204x, clsGlobVar.CoordinatePlanB.Tank204y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 205, clsGlobVar.Tank205_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank205x, clsGlobVar.CoordinatePlanB.Tank205y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 206, clsGlobVar.Tank206_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank206x, clsGlobVar.CoordinatePlanB.Tank206y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 207, clsGlobVar.Tank207_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank207ax, clsGlobVar.CoordinatePlanB.Tank207ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 207, clsGlobVar.Tank207_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank207bx, clsGlobVar.CoordinatePlanB.Tank207by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 208, clsGlobVar.Tank208_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank208x, clsGlobVar.CoordinatePlanB.Tank208y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 209, clsGlobVar.Tank209_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank209x, clsGlobVar.CoordinatePlanB.Tank209y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 210, clsGlobVar.Tank210_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank210x, clsGlobVar.CoordinatePlanB.Tank210y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 211, clsGlobVar.Tank211_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank211x, clsGlobVar.CoordinatePlanB.Tank211y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 212, clsGlobVar.Tank212_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank212x, clsGlobVar.CoordinatePlanB.Tank212y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 213, clsGlobVar.Tank213_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank213x, clsGlobVar.CoordinatePlanB.Tank213y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 214, clsGlobVar.Tank214_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank214x, clsGlobVar.CoordinatePlanB.Tank214y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 215, clsGlobVar.Tank215_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank215x, clsGlobVar.CoordinatePlanB.Tank215y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 216, clsGlobVar.Tank216_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank216x, clsGlobVar.CoordinatePlanB.Tank216y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 217, clsGlobVar.Tank217_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank217x, clsGlobVar.CoordinatePlanB.Tank217y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 218, clsGlobVar.Tank218_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank218x, clsGlobVar.CoordinatePlanB.Tank218y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 219, clsGlobVar.Tank219_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank219x, clsGlobVar.CoordinatePlanB.Tank219y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 220, clsGlobVar.Tank220_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank220x, clsGlobVar.CoordinatePlanB.Tank220y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 221, clsGlobVar.Tank221_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank221x, clsGlobVar.CoordinatePlanB.Tank221y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 222, clsGlobVar.Tank222_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank222x, clsGlobVar.CoordinatePlanB.Tank222y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 223, clsGlobVar.Tank223_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank223x, clsGlobVar.CoordinatePlanB.Tank223y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 224, clsGlobVar.Tank224_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank224x, clsGlobVar.CoordinatePlanB.Tank224y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 225, clsGlobVar.Tank225_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank225x, clsGlobVar.CoordinatePlanB.Tank225y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 226, clsGlobVar.Tank226_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank226x, clsGlobVar.CoordinatePlanB.Tank226y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 227, clsGlobVar.Tank227_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank227x, clsGlobVar.CoordinatePlanB.Tank227y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 228, clsGlobVar.Tank228_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank228x, clsGlobVar.CoordinatePlanB.Tank228y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 229, clsGlobVar.Tank229_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank229x, clsGlobVar.CoordinatePlanB.Tank229y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 230, clsGlobVar.Tank230_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank230x, clsGlobVar.CoordinatePlanB.Tank230y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 231, clsGlobVar.Tank231_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank231x, clsGlobVar.CoordinatePlanB.Tank231y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 232, clsGlobVar.Tank232_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank232x, clsGlobVar.CoordinatePlanB.Tank232y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 233, clsGlobVar.Tank233_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank233x, clsGlobVar.CoordinatePlanB.Tank233y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 234, clsGlobVar.Tank234_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank234x, clsGlobVar.CoordinatePlanB.Tank234y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 235, clsGlobVar.Tank235_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank235x, clsGlobVar.CoordinatePlanB.Tank235y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 236, clsGlobVar.Tank236_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank236x, clsGlobVar.CoordinatePlanB.Tank236y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 237, clsGlobVar.Tank237_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank237x, clsGlobVar.CoordinatePlanB.Tank237y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 238, clsGlobVar.Tank238_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank238x, clsGlobVar.CoordinatePlanB.Tank238y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 239, clsGlobVar.Tank239_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank239x, clsGlobVar.CoordinatePlanB.Tank239y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 240, clsGlobVar.Tank240_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank240x, clsGlobVar.CoordinatePlanB.Tank240y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 241, clsGlobVar.Tank241_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank241x, clsGlobVar.CoordinatePlanB.Tank241y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 242, clsGlobVar.Tank242_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank242x, clsGlobVar.CoordinatePlanB.Tank242y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 243, clsGlobVar.Tank243_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank243x, clsGlobVar.CoordinatePlanB.Tank243y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 244, clsGlobVar.Tank244_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank244x, clsGlobVar.CoordinatePlanB.Tank244y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 245, clsGlobVar.Tank245_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank245x, clsGlobVar.CoordinatePlanB.Tank245y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 246, clsGlobVar.Tank246_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank246x, clsGlobVar.CoordinatePlanB.Tank246y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 247, clsGlobVar.Tank247_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank247x, clsGlobVar.CoordinatePlanB.Tank247y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 248, clsGlobVar.Tank248_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank248x, clsGlobVar.CoordinatePlanB.Tank248y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 249, clsGlobVar.Tank249_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank249x, clsGlobVar.CoordinatePlanB.Tank249y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 250, clsGlobVar.Tank250_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank250x, clsGlobVar.CoordinatePlanB.Tank250y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 251, clsGlobVar.Tank251_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank251x, clsGlobVar.CoordinatePlanB.Tank251y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 252, clsGlobVar.Tank252_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank252x, clsGlobVar.CoordinatePlanB.Tank252y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 253, clsGlobVar.Tank253_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank253x, clsGlobVar.CoordinatePlanB.Tank253y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 254, clsGlobVar.Tank254_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank254x, clsGlobVar.CoordinatePlanB.Tank254y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 255, clsGlobVar.Tank255_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank255x, clsGlobVar.CoordinatePlanB.Tank255y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 256, clsGlobVar.Tank256_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank256x, clsGlobVar.CoordinatePlanB.Tank256y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 257, clsGlobVar.Tank257_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank257x, clsGlobVar.CoordinatePlanB.Tank257y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 258, clsGlobVar.Tank258_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank258x, clsGlobVar.CoordinatePlanB.Tank258y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 259, clsGlobVar.Tank259_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank259x, clsGlobVar.CoordinatePlanB.Tank259y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 260, clsGlobVar.Tank260_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank260x, clsGlobVar.CoordinatePlanB.Tank260y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 262, clsGlobVar.Tank262_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank262x, clsGlobVar.CoordinatePlanB.Tank262y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 263, clsGlobVar.Tank263_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank263x, clsGlobVar.CoordinatePlanB.Tank263y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 264, clsGlobVar.Tank264_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank264x, clsGlobVar.CoordinatePlanB.Tank264y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 265, clsGlobVar.Tank265_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank265x, clsGlobVar.CoordinatePlanB.Tank265y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 266, clsGlobVar.Tank266_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank266x, clsGlobVar.CoordinatePlanB.Tank266y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 267, clsGlobVar.Tank267_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank267x, clsGlobVar.CoordinatePlanB.Tank267y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 268, clsGlobVar.Tank268_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank268x, clsGlobVar.CoordinatePlanB.Tank268y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 269, clsGlobVar.Tank269_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank269x, clsGlobVar.CoordinatePlanB.Tank269y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 270, clsGlobVar.Tank270_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank270x, clsGlobVar.CoordinatePlanB.Tank270y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 271, clsGlobVar.Tank271_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank271x, clsGlobVar.CoordinatePlanB.Tank271y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 272, clsGlobVar.Tank272_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank272x, clsGlobVar.CoordinatePlanB.Tank272y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 273, clsGlobVar.Tank273_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank273x, clsGlobVar.CoordinatePlanB.Tank273y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 274, clsGlobVar.Tank274_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank274x, clsGlobVar.CoordinatePlanB.Tank274y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 275, clsGlobVar.Tank275_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank275x, clsGlobVar.CoordinatePlanB.Tank275y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 276, clsGlobVar.Tank276_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank276x, clsGlobVar.CoordinatePlanB.Tank276y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 277, clsGlobVar.Tank277_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank277x, clsGlobVar.CoordinatePlanB.Tank277y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 278, clsGlobVar.Tank278_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank278x, clsGlobVar.CoordinatePlanB.Tank278y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 279, clsGlobVar.Tank279_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank279x, clsGlobVar.CoordinatePlanB.Tank279y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 280, clsGlobVar.Tank280_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank280x, clsGlobVar.CoordinatePlanB.Tank280y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 281, clsGlobVar.Tank281_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank281x, clsGlobVar.CoordinatePlanB.Tank281y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 282, clsGlobVar.Tank282_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank282x, clsGlobVar.CoordinatePlanB.Tank282y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 283, clsGlobVar.Tank283_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank283x, clsGlobVar.CoordinatePlanB.Tank283y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 284, clsGlobVar.Tank284_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank284x, clsGlobVar.CoordinatePlanB.Tank284y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 285, clsGlobVar.Tank285_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank285x, clsGlobVar.CoordinatePlanB.Tank285y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 286, clsGlobVar.Tank286_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank286x, clsGlobVar.CoordinatePlanB.Tank286y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 287, clsGlobVar.Tank287_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank287x, clsGlobVar.CoordinatePlanB.Tank287y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 288, clsGlobVar.Tank288_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank288x, clsGlobVar.CoordinatePlanB.Tank288y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 289, clsGlobVar.Tank289_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank289x, clsGlobVar.CoordinatePlanB.Tank289y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 290, clsGlobVar.Tank290_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank290x, clsGlobVar.CoordinatePlanB.Tank290y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 291, clsGlobVar.Tank291_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank291x, clsGlobVar.CoordinatePlanB.Tank291y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 292, clsGlobVar.Tank292_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank292x, clsGlobVar.CoordinatePlanB.Tank292y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 293, clsGlobVar.Tank293_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank293x, clsGlobVar.CoordinatePlanB.Tank293y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 294, clsGlobVar.Tank294_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank294x, clsGlobVar.CoordinatePlanB.Tank294y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 295, clsGlobVar.Tank295_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank295x, clsGlobVar.CoordinatePlanB.Tank295y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 296, clsGlobVar.Tank296_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank296x, clsGlobVar.CoordinatePlanB.Tank296y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 297, clsGlobVar.Tank297_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank297x, clsGlobVar.CoordinatePlanB.Tank297y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 298, clsGlobVar.Tank298_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank298x, clsGlobVar.CoordinatePlanB.Tank298y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 299, clsGlobVar.Tank299_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank299x, clsGlobVar.CoordinatePlanB.Tank299y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 300, clsGlobVar.Tank300_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank300x, clsGlobVar.CoordinatePlanB.Tank300y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 301, clsGlobVar.Tank301_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank301x, clsGlobVar.CoordinatePlanB.Tank301y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 436, clsGlobVar.Tank436_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank436x, clsGlobVar.CoordinatePlanB.Tank436y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 437, clsGlobVar.Tank437_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank437x, clsGlobVar.CoordinatePlanB.Tank437y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 440, clsGlobVar.Tank440_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank440x, clsGlobVar.CoordinatePlanB.Tank440y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 441, clsGlobVar.Tank441_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank441x, clsGlobVar.CoordinatePlanB.Tank441y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 445, clsGlobVar.Tank445_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank445x, clsGlobVar.CoordinatePlanB.Tank445y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 446, clsGlobVar.Tank446_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank446x, clsGlobVar.CoordinatePlanB.Tank446y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 447, clsGlobVar.Tank447_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank447ax, clsGlobVar.CoordinatePlanB.Tank447ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 447, clsGlobVar.Tank447_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank447bx, clsGlobVar.CoordinatePlanB.Tank447by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 448, clsGlobVar.Tank448_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank448ax, clsGlobVar.CoordinatePlanB.Tank448ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 448, clsGlobVar.Tank448_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank448bx, clsGlobVar.CoordinatePlanB.Tank448by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 450, clsGlobVar.Tank450_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank450x, clsGlobVar.CoordinatePlanB.Tank450y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 454, clsGlobVar.Tank454_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank454x, clsGlobVar.CoordinatePlanB.Tank454y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 455, clsGlobVar.Tank455_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank455ax, clsGlobVar.CoordinatePlanB.Tank455ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 455, clsGlobVar.Tank455_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank455bx, clsGlobVar.CoordinatePlanB.Tank455by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 458, clsGlobVar.Tank458_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank458x, clsGlobVar.CoordinatePlanB.Tank458y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 459, clsGlobVar.Tank459_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank459ax, clsGlobVar.CoordinatePlanB.Tank459ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 459, clsGlobVar.Tank459_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank459bx, clsGlobVar.CoordinatePlanB.Tank459by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 462, clsGlobVar.Tank462_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank462x, clsGlobVar.CoordinatePlanB.Tank462y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 463, clsGlobVar.Tank463_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank463x, clsGlobVar.CoordinatePlanB.Tank463y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 464, clsGlobVar.Tank464_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank464x, clsGlobVar.CoordinatePlanB.Tank464y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 465, clsGlobVar.Tank465_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank465x, clsGlobVar.CoordinatePlanB.Tank465y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 466, clsGlobVar.Tank466_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank466ax, clsGlobVar.CoordinatePlanB.Tank466ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 466, clsGlobVar.Tank466_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank466bx, clsGlobVar.CoordinatePlanB.Tank466by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 468, clsGlobVar.Tank468_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank468x, clsGlobVar.CoordinatePlanB.Tank468y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 469, clsGlobVar.Tank469_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank469x, clsGlobVar.CoordinatePlanB.Tank469y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 471, clsGlobVar.Tank471_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank471x, clsGlobVar.CoordinatePlanB.Tank471y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 472, clsGlobVar.Tank472_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank472x, clsGlobVar.CoordinatePlanB.Tank472y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 474, clsGlobVar.Tank474_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank474x, clsGlobVar.CoordinatePlanB.Tank474y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 475, clsGlobVar.Tank475_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank475ax, clsGlobVar.CoordinatePlanB.Tank475ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 475, clsGlobVar.Tank475_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank475bx, clsGlobVar.CoordinatePlanB.Tank475by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 480, clsGlobVar.Tank480_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank480x, clsGlobVar.CoordinatePlanB.Tank480y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 481, clsGlobVar.Tank481_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank481x, clsGlobVar.CoordinatePlanB.Tank481y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 486, clsGlobVar.Tank486_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank486x, clsGlobVar.CoordinatePlanB.Tank486y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 490, clsGlobVar.Tank490_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank490x, clsGlobVar.CoordinatePlanB.Tank490y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 491, clsGlobVar.Tank491_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank491x, clsGlobVar.CoordinatePlanB.Tank491y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 492, clsGlobVar.Tank492_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank492ax, clsGlobVar.CoordinatePlanB.Tank492ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 492, clsGlobVar.Tank492_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank492bx, clsGlobVar.CoordinatePlanB.Tank492by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 493, clsGlobVar.Tank493_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank493x, clsGlobVar.CoordinatePlanB.Tank493y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 495, clsGlobVar.Tank495_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank495ax, clsGlobVar.CoordinatePlanB.Tank495ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 495, clsGlobVar.Tank495_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank495bx, clsGlobVar.CoordinatePlanB.Tank495by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 496, clsGlobVar.Tank496_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank496x, clsGlobVar.CoordinatePlanB.Tank496y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 497, clsGlobVar.Tank497_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank497x, clsGlobVar.CoordinatePlanB.Tank497y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanB, 498, clsGlobVar.Tank498_SimulationPercentFill, clsGlobVar.CoordinatePlanB.Tank498x, clsGlobVar.CoordinatePlanB.Tank498y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));


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
                System.Windows.Media.Color flag = (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[0]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 141, 180, 227));
                DrawHatchDeckPlan(canvas2DPlanA, 1, clsGlobVar.Tank1_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank1ax, clsGlobVar.CoordinatePlanA.Tank1ay,  (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[0]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 194, 214, 154)));
                DrawHatchDeckPlan(canvas2DPlanA, 2, clsGlobVar.Tank2_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank2x, clsGlobVar.CoordinatePlanA.Tank2y, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[1]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 194, 214, 154)));
                DrawHatchDeckPlan(canvas2DPlanA, 3, clsGlobVar.Tank3_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank3x, clsGlobVar.CoordinatePlanA.Tank3y,  (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[2]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 194, 214, 154))); 
                DrawHatchDeckPlan(canvas2DPlanA, 4, clsGlobVar.Tank4_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank4ax, clsGlobVar.CoordinatePlanA.Tank4ay,(((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[3]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 194, 214, 154)));
                DrawHatchDeckPlan(canvas2DPlanA, 4, clsGlobVar.Tank4_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank4bx, clsGlobVar.CoordinatePlanA.Tank4by,(((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[3]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 194, 214, 154)));
                DrawHatchDeckPlan(canvas2DPlanA, 5, clsGlobVar.Tank5_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank5ax, clsGlobVar.CoordinatePlanA.Tank5ay,(((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[4]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 194, 214, 154)));
                DrawHatchDeckPlan(canvas2DPlanA, 5, clsGlobVar.Tank5_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank5bx, clsGlobVar.CoordinatePlanA.Tank5by,(((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[4]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 194, 214, 154)));
                DrawHatchDeckPlan(canvas2DPlanA, 6, clsGlobVar.Tank6_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank6ax, clsGlobVar.CoordinatePlanA.Tank6ay,(((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[5]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 194, 214, 154)));
                DrawHatchDeckPlan(canvas2DPlanA, 6, clsGlobVar.Tank6_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank6bx, clsGlobVar.CoordinatePlanA.Tank6by,(((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[5]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 194, 214, 154)));
                DrawHatchDeckPlan(canvas2DPlanA, 7, clsGlobVar.Tank7_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank7x, clsGlobVar.CoordinatePlanA.Tank7y,  (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[6]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 194, 214, 154)));
                DrawHatchDeckPlan(canvas2DPlanA, 7, clsGlobVar.Tank7_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank7bx, clsGlobVar.CoordinatePlanA.Tank7by, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[6]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255, 218, 97, 78) : System.Windows.Media.Color.FromArgb(180, 194, 214, 154)));
                DrawHatchDeckPlan(canvas2DPlanA, 8, clsGlobVar.Tank8_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank8ax, clsGlobVar.CoordinatePlanA.Tank8ay,(((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[7]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 194, 214, 154)));
                DrawHatchDeckPlan(canvas2DPlanA, 8, clsGlobVar.Tank8_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank8bx, clsGlobVar.CoordinatePlanA.Tank8by,(((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[7]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 194, 214, 154)));
                DrawHatchDeckPlan(canvas2DPlanA, 9, clsGlobVar.Tank9_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank9x, clsGlobVar.CoordinatePlanA.Tank9y,        (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[8]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 194, 214, 154)));
                DrawHatchDeckPlan(canvas2DPlanA, 10, clsGlobVar.Tank10_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank10x, clsGlobVar.CoordinatePlanA.Tank10y,    (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[9]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 194, 214, 154)));
                DrawHatchDeckPlan(canvas2DPlanA, 11, clsGlobVar.Tank11_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank11ax, clsGlobVar.CoordinatePlanA.Tank11ay,  (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[10]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 194, 214, 154)));
                DrawHatchDeckPlan(canvas2DPlanA, 11, clsGlobVar.Tank11_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank11bx, clsGlobVar.CoordinatePlanA.Tank11by,  (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[10]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 194, 214, 154)));
                DrawHatchDeckPlan(canvas2DPlanA, 12, clsGlobVar.Tank12_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank12ax, clsGlobVar.CoordinatePlanA.Tank12ay, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[11]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 141, 180, 227)));
                DrawHatchDeckPlan(canvas2DPlanA, 12, clsGlobVar.Tank12_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank12bx, clsGlobVar.CoordinatePlanA.Tank12by, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[11]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 141, 180, 227)));
                DrawHatchDeckPlan(canvas2DPlanA, 13, clsGlobVar.Tank13_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank13ax, clsGlobVar.CoordinatePlanA.Tank13ay, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[12]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 141, 180, 227)));
                DrawHatchDeckPlan(canvas2DPlanA, 13, clsGlobVar.Tank13_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank13bx, clsGlobVar.CoordinatePlanA.Tank13by, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[12]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 141, 180, 227)));
                DrawHatchDeckPlan(canvas2DPlanA, 14, clsGlobVar.Tank14_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank14ax, clsGlobVar.CoordinatePlanA.Tank14ay, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[13]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 141, 180, 227)));
                DrawHatchDeckPlan(canvas2DPlanA, 14, clsGlobVar.Tank14_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank14bx, clsGlobVar.CoordinatePlanA.Tank14by, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[13]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 141, 180, 227)));
                DrawHatchDeckPlan(canvas2DPlanA, 15, clsGlobVar.Tank15_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank15ax, clsGlobVar.CoordinatePlanA.Tank15ay, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[14]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 141, 180, 227)));
                DrawHatchDeckPlan(canvas2DPlanA, 15, clsGlobVar.Tank15_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank15bx, clsGlobVar.CoordinatePlanA.Tank15by, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[14]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 141, 180, 227)));
                DrawHatchDeckPlan(canvas2DPlanA, 16, clsGlobVar.Tank16_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank16ax, clsGlobVar.CoordinatePlanA.Tank16ay, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[15]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 141, 180, 227)));
                DrawHatchDeckPlan(canvas2DPlanA, 16, clsGlobVar.Tank16_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank16bx, clsGlobVar.CoordinatePlanA.Tank16by, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[15]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 141, 180, 227)));
                DrawHatchDeckPlan(canvas2DPlanA, 17, clsGlobVar.Tank17_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank17ax, clsGlobVar.CoordinatePlanA.Tank17ay, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[16]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 151, 72, 7)));
                DrawHatchDeckPlan(canvas2DPlanA, 17, clsGlobVar.Tank17_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank17bx, clsGlobVar.CoordinatePlanA.Tank17by, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[16]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 151, 72, 7)));
                DrawHatchDeckPlan(canvas2DPlanA, 18, clsGlobVar.Tank18_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank18x, clsGlobVar.CoordinatePlanA.Tank18y,   (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[17]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 151, 72, 7)));
                DrawHatchDeckPlan(canvas2DPlanA, 19, clsGlobVar.Tank19_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank19x, clsGlobVar.CoordinatePlanA.Tank19y,   (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[18]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 151, 72, 7)));
                DrawHatchDeckPlan(canvas2DPlanA, 20, clsGlobVar.Tank20_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank20x, clsGlobVar.CoordinatePlanA.Tank20y,   (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[19]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 151, 72, 7)));
                DrawHatchDeckPlan(canvas2DPlanA, 21, clsGlobVar.Tank21_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank21x, clsGlobVar.CoordinatePlanA.Tank21y,   (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[20]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 151, 72, 7)));
                DrawHatchDeckPlan(canvas2DPlanA, 22, clsGlobVar.Tank22_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank22x, clsGlobVar.CoordinatePlanA.Tank22y,   (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[21]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 151, 72, 7)));
                DrawHatchDeckPlan(canvas2DPlanA, 23, clsGlobVar.Tank23_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank23x, clsGlobVar.CoordinatePlanA.Tank23y,   (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[22]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 151, 72, 7)));
                DrawHatchDeckPlan(canvas2DPlanA, 24, clsGlobVar.Tank24_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank24x, clsGlobVar.CoordinatePlanA.Tank24y,   (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[23]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 151, 72, 7)));
                DrawHatchDeckPlan(canvas2DPlanA, 25, clsGlobVar.Tank25_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank25x, clsGlobVar.CoordinatePlanA.Tank25y,   (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[24]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 151, 72, 7)));
                DrawHatchDeckPlan(canvas2DPlanA, 26, clsGlobVar.Tank26_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank26x, clsGlobVar.CoordinatePlanA.Tank26y,   (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[25]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 151, 72, 7)));
                DrawHatchDeckPlan(canvas2DPlanA, 27, clsGlobVar.Tank27_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank27x, clsGlobVar.CoordinatePlanA.Tank27y,   (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[26]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 151, 72, 7)));
                DrawHatchDeckPlan(canvas2DPlanA, 28, clsGlobVar.Tank28_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank28x, clsGlobVar.CoordinatePlanA.Tank28y,   (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[27]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 151, 72, 7)));
                DrawHatchDeckPlan(canvas2DPlanA, 29, clsGlobVar.Tank29_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank29ax, clsGlobVar.CoordinatePlanA.Tank29ay, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[28]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 151, 72, 7)));
                DrawHatchDeckPlan(canvas2DPlanA, 29, clsGlobVar.Tank29_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank29bx, clsGlobVar.CoordinatePlanA.Tank29by, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[28]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 151, 72, 7)));
                DrawHatchDeckPlan(canvas2DPlanA, 30, clsGlobVar.Tank30_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank30x, clsGlobVar.CoordinatePlanA.Tank30y,   (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[29]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 151, 72, 7)));
                DrawHatchDeckPlan(canvas2DPlanA, 31, clsGlobVar.Tank31_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank31ax, clsGlobVar.CoordinatePlanA.Tank31ay, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[30]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 151, 72, 7)));
                DrawHatchDeckPlan(canvas2DPlanA, 31, clsGlobVar.Tank31_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank31bx, clsGlobVar.CoordinatePlanA.Tank31by, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[30]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 151, 72, 7)));
                DrawHatchDeckPlan(canvas2DPlanA, 32, clsGlobVar.Tank32_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank32ax, clsGlobVar.CoordinatePlanA.Tank32ay, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[31]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 151, 72, 7)));
                DrawHatchDeckPlan(canvas2DPlanA, 32, clsGlobVar.Tank32_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank32bx, clsGlobVar.CoordinatePlanA.Tank32by, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[31]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 151, 72, 7)));
                DrawHatchDeckPlan(canvas2DPlanA, 33, clsGlobVar.Tank33_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank33x, clsGlobVar.CoordinatePlanA.Tank33y,   (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[32]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 151, 72, 7)));
                DrawHatchDeckPlan(canvas2DPlanA, 34, clsGlobVar.Tank34_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank34x, clsGlobVar.CoordinatePlanA.Tank34y,   (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[33]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 151, 72, 7)));
                DrawHatchDeckPlan(canvas2DPlanA, 35, clsGlobVar.Tank35_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank35ax, clsGlobVar.CoordinatePlanA.Tank35ay, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[34]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 151, 72, 7)));
                DrawHatchDeckPlan(canvas2DPlanA, 35, clsGlobVar.Tank35_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank35bx, clsGlobVar.CoordinatePlanA.Tank35by, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[34]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 151, 72, 7)));
                DrawHatchDeckPlan(canvas2DPlanA, 36, clsGlobVar.Tank36_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank36ax, clsGlobVar.CoordinatePlanA.Tank36ay, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[35]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 151, 72, 7)));
                DrawHatchDeckPlan(canvas2DPlanA, 36, clsGlobVar.Tank36_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank36bx, clsGlobVar.CoordinatePlanA.Tank36by, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[35]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 151, 72, 7)));
                DrawHatchDeckPlan(canvas2DPlanA, 37, clsGlobVar.Tank37_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank37ax, clsGlobVar.CoordinatePlanA.Tank37ay, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[36]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 151, 72, 7)));
                DrawHatchDeckPlan(canvas2DPlanA, 37, clsGlobVar.Tank37_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank37bx, clsGlobVar.CoordinatePlanA.Tank37by, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[36]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 151, 72, 7)));
                DrawHatchDeckPlan(canvas2DPlanA, 38, clsGlobVar.Tank38_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank38ax, clsGlobVar.CoordinatePlanA.Tank38ay, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[37]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 151, 72, 7)));
                DrawHatchDeckPlan(canvas2DPlanA, 38, clsGlobVar.Tank38_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank38bx, clsGlobVar.CoordinatePlanA.Tank38by, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[37]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 151, 72, 7)));
                DrawHatchDeckPlan(canvas2DPlanA, 39, clsGlobVar.Tank39_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank39ax, clsGlobVar.CoordinatePlanA.Tank39ay, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[38]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 151, 72, 7)));
                DrawHatchDeckPlan(canvas2DPlanA, 39, clsGlobVar.Tank39_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank39bx, clsGlobVar.CoordinatePlanA.Tank39by, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[38]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 151, 72, 7)));
                DrawHatchDeckPlan(canvas2DPlanA, 40, clsGlobVar.Tank40_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank40x, clsGlobVar.CoordinatePlanA.Tank40y,   (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[39]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 151, 72, 7)));
                DrawHatchDeckPlan(canvas2DPlanA, 41, clsGlobVar.Tank41_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank41x, clsGlobVar.CoordinatePlanA.Tank41y,   (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[40]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 151, 72, 7)));
                DrawHatchDeckPlan(canvas2DPlanA, 42, clsGlobVar.Tank42_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank42x, clsGlobVar.CoordinatePlanA.Tank42y,   (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[41]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 151, 72, 7)));
                DrawHatchDeckPlan(canvas2DPlanA, 44, clsGlobVar.Tank44_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank44x, clsGlobVar.CoordinatePlanA.Tank44y,   (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[43]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 151, 72, 7)));
                DrawHatchDeckPlan(canvas2DPlanA, 45, clsGlobVar.Tank45_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank45ax, clsGlobVar.CoordinatePlanA.Tank45ay, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[44]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 203, 150, 69)));
                DrawHatchDeckPlan(canvas2DPlanA, 45, clsGlobVar.Tank45_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank45bx, clsGlobVar.CoordinatePlanA.Tank45by, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[44]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 203, 150, 69)));
                DrawHatchDeckPlan(canvas2DPlanA, 46, clsGlobVar.Tank46_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank46ax, clsGlobVar.CoordinatePlanA.Tank46ay, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[45]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 203, 150, 69)));
                DrawHatchDeckPlan(canvas2DPlanA, 46, clsGlobVar.Tank46_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank46bx, clsGlobVar.CoordinatePlanA.Tank46by, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[45]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 203, 150, 69)));
                DrawHatchDeckPlan(canvas2DPlanA, 47, clsGlobVar.Tank47_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank47x, clsGlobVar.CoordinatePlanA.Tank47y,   (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[46]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 255, 192,203)));
                DrawHatchDeckPlan(canvas2DPlanA, 48, clsGlobVar.Tank48_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank48x, clsGlobVar.CoordinatePlanA.Tank48y,   (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[47]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 255, 192,203)));
                DrawHatchDeckPlan(canvas2DPlanA, 50, clsGlobVar.Tank50_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank50x, clsGlobVar.CoordinatePlanA.Tank50y,   (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[49]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 255, 192,203)));
                DrawHatchDeckPlan(canvas2DPlanA, 52, clsGlobVar.Tank52_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank52x, clsGlobVar.CoordinatePlanA.Tank52y,   (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[51]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 255, 192,203)));
                DrawHatchDeckPlan(canvas2DPlanA, 53, clsGlobVar.Tank53_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank53x, clsGlobVar.CoordinatePlanA.Tank53y,   (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[52]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 255, 192,203)));
                DrawHatchDeckPlan(canvas2DPlanA, 54, clsGlobVar.Tank54_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank54x, clsGlobVar.CoordinatePlanA.Tank54y,   (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[53]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 255, 192,203)));
                DrawHatchDeckPlan(canvas2DPlanA, 55, clsGlobVar.Tank55_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank55x, clsGlobVar.CoordinatePlanA.Tank55y,   (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[54]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 141, 180, 227)));
                DrawHatchDeckPlan(canvas2DPlanA, 56, clsGlobVar.Tank56_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank56x, clsGlobVar.CoordinatePlanA.Tank56y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 57, clsGlobVar.Tank57_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank57ax, clsGlobVar.CoordinatePlanA.Tank57ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 57, clsGlobVar.Tank57_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank57bx, clsGlobVar.CoordinatePlanA.Tank57by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 58, clsGlobVar.Tank58_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank58ax, clsGlobVar.CoordinatePlanA.Tank58ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 58, clsGlobVar.Tank58_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank58bx, clsGlobVar.CoordinatePlanA.Tank58by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 59, clsGlobVar.Tank59_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank59ax, clsGlobVar.CoordinatePlanA.Tank59ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 59, clsGlobVar.Tank59_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank59bx, clsGlobVar.CoordinatePlanA.Tank59by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 60, clsGlobVar.Tank60_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank60ax, clsGlobVar.CoordinatePlanA.Tank60ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 60, clsGlobVar.Tank60_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank60bx, clsGlobVar.CoordinatePlanA.Tank60by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 61, clsGlobVar.Tank61_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank61ax, clsGlobVar.CoordinatePlanA.Tank61ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 61, clsGlobVar.Tank61_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank61bx, clsGlobVar.CoordinatePlanA.Tank61by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 62, clsGlobVar.Tank62_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank62ax, clsGlobVar.CoordinatePlanA.Tank62ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 62, clsGlobVar.Tank62_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank62bx, clsGlobVar.CoordinatePlanA.Tank62by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 63, clsGlobVar.Tank63_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank63ax, clsGlobVar.CoordinatePlanA.Tank63ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 63, clsGlobVar.Tank63_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank63bx, clsGlobVar.CoordinatePlanA.Tank63by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 64, clsGlobVar.Tank64_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank64ax, clsGlobVar.CoordinatePlanA.Tank64ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 64, clsGlobVar.Tank64_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank64bx, clsGlobVar.CoordinatePlanA.Tank64by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 65, clsGlobVar.Tank65_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank65ax, clsGlobVar.CoordinatePlanA.Tank65ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 65, clsGlobVar.Tank65_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank65bx, clsGlobVar.CoordinatePlanA.Tank65by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 66, clsGlobVar.Tank66_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank66x, clsGlobVar.CoordinatePlanA.Tank66y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 67, clsGlobVar.Tank67_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank67x, clsGlobVar.CoordinatePlanA.Tank67y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 68, clsGlobVar.Tank68_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank68x, clsGlobVar.CoordinatePlanA.Tank68y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 69, clsGlobVar.Tank69_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank69x, clsGlobVar.CoordinatePlanA.Tank69y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 70, clsGlobVar.Tank70_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank70x, clsGlobVar.CoordinatePlanA.Tank70y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 71, clsGlobVar.Tank71_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank71x, clsGlobVar.CoordinatePlanA.Tank71y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 72, clsGlobVar.Tank72_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank72x, clsGlobVar.CoordinatePlanA.Tank72y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 73, clsGlobVar.Tank73_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank73x, clsGlobVar.CoordinatePlanA.Tank73y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 74, clsGlobVar.Tank74_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank74x, clsGlobVar.CoordinatePlanA.Tank74y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 75, clsGlobVar.Tank75_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank75x, clsGlobVar.CoordinatePlanA.Tank75y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 76, clsGlobVar.Tank76_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank76x, clsGlobVar.CoordinatePlanA.Tank76y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 77, clsGlobVar.Tank77_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank77x, clsGlobVar.CoordinatePlanA.Tank77y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 78, clsGlobVar.Tank78_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank78x, clsGlobVar.CoordinatePlanA.Tank78y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 79, clsGlobVar.Tank79_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank79x, clsGlobVar.CoordinatePlanA.Tank79y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 80, clsGlobVar.Tank80_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank80x, clsGlobVar.CoordinatePlanA.Tank80y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 81, clsGlobVar.Tank81_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank81x, clsGlobVar.CoordinatePlanA.Tank81y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 82, clsGlobVar.Tank82_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank82x, clsGlobVar.CoordinatePlanA.Tank82y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 83, clsGlobVar.Tank83_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank83x, clsGlobVar.CoordinatePlanA.Tank83y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 84, clsGlobVar.Tank84_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank84x, clsGlobVar.CoordinatePlanA.Tank84y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 85, clsGlobVar.Tank85_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank85x, clsGlobVar.CoordinatePlanA.Tank85y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 86, clsGlobVar.Tank86_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank86x, clsGlobVar.CoordinatePlanA.Tank86y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 87, clsGlobVar.Tank87_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank87x, clsGlobVar.CoordinatePlanA.Tank87y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 88, clsGlobVar.Tank88_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank88x, clsGlobVar.CoordinatePlanA.Tank88y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 89, clsGlobVar.Tank89_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank89x, clsGlobVar.CoordinatePlanA.Tank89y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 90, clsGlobVar.Tank90_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank90x, clsGlobVar.CoordinatePlanA.Tank90y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 154, clsGlobVar.Tank154_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank154ax, clsGlobVar.CoordinatePlanA.Tank154ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 154, clsGlobVar.Tank154_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank154bx, clsGlobVar.CoordinatePlanA.Tank154by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 155, clsGlobVar.Tank155_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank155ax, clsGlobVar.CoordinatePlanA.Tank155ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 155, clsGlobVar.Tank155_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank155bx, clsGlobVar.CoordinatePlanA.Tank155by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 156, clsGlobVar.Tank156_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank156ax, clsGlobVar.CoordinatePlanA.Tank156ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 156, clsGlobVar.Tank156_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank156bx, clsGlobVar.CoordinatePlanA.Tank156by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 157, clsGlobVar.Tank157_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank157ax, clsGlobVar.CoordinatePlanA.Tank157ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 157, clsGlobVar.Tank157_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank157bx, clsGlobVar.CoordinatePlanA.Tank157by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 438, clsGlobVar.Tank438_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank438x, clsGlobVar.CoordinatePlanA.Tank438y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 442, clsGlobVar.Tank442_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank442x, clsGlobVar.CoordinatePlanA.Tank442y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 451, clsGlobVar.Tank451_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank451x, clsGlobVar.CoordinatePlanA.Tank451y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 456, clsGlobVar.Tank456_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank456x, clsGlobVar.CoordinatePlanA.Tank456y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 460, clsGlobVar.Tank460_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank460x, clsGlobVar.CoordinatePlanA.Tank460y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 464, clsGlobVar.Tank464_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank464x, clsGlobVar.CoordinatePlanA.Tank464y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 466, clsGlobVar.Tank466_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank466x, clsGlobVar.CoordinatePlanA.Tank466y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 469, clsGlobVar.Tank469_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank469x, clsGlobVar.CoordinatePlanA.Tank469y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 473, clsGlobVar.Tank473_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank473x, clsGlobVar.CoordinatePlanA.Tank473y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 475, clsGlobVar.Tank475_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank475x, clsGlobVar.CoordinatePlanA.Tank475y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 477, clsGlobVar.Tank477_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank477ax, clsGlobVar.CoordinatePlanA.Tank477ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 477, clsGlobVar.Tank477_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank477bx, clsGlobVar.CoordinatePlanA.Tank477by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 479, clsGlobVar.Tank479_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank479ax, clsGlobVar.CoordinatePlanA.Tank479ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 479, clsGlobVar.Tank479_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank479bx, clsGlobVar.CoordinatePlanA.Tank479by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 482, clsGlobVar.Tank482_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank482x, clsGlobVar.CoordinatePlanA.Tank482y, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[481]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255, 218, 97, 78) : System.Windows.Media.Color.FromArgb(180, 141, 180, 227)));
               DrawHatchDeckPlan(canvas2DPlanA, 483, clsGlobVar.Tank483_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank483ax, clsGlobVar.CoordinatePlanA.Tank483ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               DrawHatchDeckPlan(canvas2DPlanA, 483, clsGlobVar.Tank483_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank483bx, clsGlobVar.CoordinatePlanA.Tank483by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 484, clsGlobVar.Tank484_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank484ax, clsGlobVar.CoordinatePlanA.Tank484ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 484, clsGlobVar.Tank484_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank484bx, clsGlobVar.CoordinatePlanA.Tank484by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
              DrawHatchDeckPlan(canvas2DPlanA, 487, clsGlobVar.Tank487_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank487x, clsGlobVar.CoordinatePlanA.Tank487y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
              DrawHatchDeckPlan(canvas2DPlanA, 488, clsGlobVar.Tank488_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank488ax, clsGlobVar.CoordinatePlanA.Tank488ay, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[487]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255, 218, 97, 78) : System.Windows.Media.Color.FromArgb(180, 141, 180, 227)));
              DrawHatchDeckPlan(canvas2DPlanA, 488, clsGlobVar.Tank488_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank488bx, clsGlobVar.CoordinatePlanA.Tank488by, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[487]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255, 218, 97, 78) : System.Windows.Media.Color.FromArgb(180, 141, 180, 227)));
                DrawHatchDeckPlan(canvas2DPlanA, 489, clsGlobVar.Tank489_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank489ax, clsGlobVar.CoordinatePlanA.Tank489ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 489, clsGlobVar.Tank489_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank489bx, clsGlobVar.CoordinatePlanA.Tank489by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 499, clsGlobVar.Tank499_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank499ax, clsGlobVar.CoordinatePlanA.Tank499ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 499, clsGlobVar.Tank499_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank499bx, clsGlobVar.CoordinatePlanA.Tank499by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 500, clsGlobVar.Tank500_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank500ax, clsGlobVar.CoordinatePlanA.Tank500ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 500, clsGlobVar.Tank500_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank500bx, clsGlobVar.CoordinatePlanA.Tank500by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 501, clsGlobVar.Tank501_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank501ax, clsGlobVar.CoordinatePlanA.Tank501ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 501, clsGlobVar.Tank501_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank501bx, clsGlobVar.CoordinatePlanA.Tank501by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 502, clsGlobVar.Tank502_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank502ax, clsGlobVar.CoordinatePlanA.Tank502ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanA, 502, clsGlobVar.Tank502_SimulationPercentFill, clsGlobVar.CoordinatePlanA.Tank502bx, clsGlobVar.CoordinatePlanA.Tank502by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));


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
                System.Windows.Media.Color flag = (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[0]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255, 218, 97, 78) : System.Windows.Media.Color.FromArgb(180, 141, 180, 227));
                DrawHatchDeckPlan(canvas2DPlanC, 142, clsGlobVar.Tank142_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank142x, clsGlobVar.CoordinatePlanC.Tank142y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 143, clsGlobVar.Tank143_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank143x, clsGlobVar.CoordinatePlanC.Tank143y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 144, clsGlobVar.Tank144_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank144x, clsGlobVar.CoordinatePlanC.Tank144y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 148, clsGlobVar.Tank148_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank148x, clsGlobVar.CoordinatePlanC.Tank148y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               DrawHatchDeckPlan(canvas2DPlanC, 149, clsGlobVar.Tank149_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank149ax, clsGlobVar.CoordinatePlanC.Tank149ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 149, clsGlobVar.Tank149_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank149bx, clsGlobVar.CoordinatePlanC.Tank149by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 178, clsGlobVar.Tank178_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank178x, clsGlobVar.CoordinatePlanC.Tank178y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 188, clsGlobVar.Tank188_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank188x, clsGlobVar.CoordinatePlanC.Tank188y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 302, clsGlobVar.Tank302_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank302x, clsGlobVar.CoordinatePlanC.Tank302y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 303, clsGlobVar.Tank303_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank303x, clsGlobVar.CoordinatePlanC.Tank303y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 304, clsGlobVar.Tank304_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank304x, clsGlobVar.CoordinatePlanC.Tank304y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 305, clsGlobVar.Tank305_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank305x, clsGlobVar.CoordinatePlanC.Tank305y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 306, clsGlobVar.Tank306_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank306x, clsGlobVar.CoordinatePlanC.Tank306y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 307, clsGlobVar.Tank307_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank307x, clsGlobVar.CoordinatePlanC.Tank307y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 308, clsGlobVar.Tank308_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank308x, clsGlobVar.CoordinatePlanC.Tank308y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 309, clsGlobVar.Tank309_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank309x, clsGlobVar.CoordinatePlanC.Tank309y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 310, clsGlobVar.Tank310_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank310x, clsGlobVar.CoordinatePlanC.Tank310y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 311, clsGlobVar.Tank311_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank311x, clsGlobVar.CoordinatePlanC.Tank311y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 312, clsGlobVar.Tank312_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank312x, clsGlobVar.CoordinatePlanC.Tank312y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 313, clsGlobVar.Tank313_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank313x, clsGlobVar.CoordinatePlanC.Tank313y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 314, clsGlobVar.Tank314_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank314x, clsGlobVar.CoordinatePlanC.Tank314y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 315, clsGlobVar.Tank315_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank315x, clsGlobVar.CoordinatePlanC.Tank315y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 316, clsGlobVar.Tank316_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank316x, clsGlobVar.CoordinatePlanC.Tank316y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 318, clsGlobVar.Tank318_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank318x, clsGlobVar.CoordinatePlanC.Tank318y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 319, clsGlobVar.Tank319_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank319x, clsGlobVar.CoordinatePlanC.Tank319y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 320, clsGlobVar.Tank320_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank320x, clsGlobVar.CoordinatePlanC.Tank320y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 321, clsGlobVar.Tank321_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank321x, clsGlobVar.CoordinatePlanC.Tank321y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 322, clsGlobVar.Tank322_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank322x, clsGlobVar.CoordinatePlanC.Tank322y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 323, clsGlobVar.Tank323_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank323x, clsGlobVar.CoordinatePlanC.Tank323y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 324, clsGlobVar.Tank324_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank324x, clsGlobVar.CoordinatePlanC.Tank324y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 325, clsGlobVar.Tank325_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank325x, clsGlobVar.CoordinatePlanC.Tank325y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 326, clsGlobVar.Tank326_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank326x, clsGlobVar.CoordinatePlanC.Tank326y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 327, clsGlobVar.Tank327_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank327x, clsGlobVar.CoordinatePlanC.Tank327y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 328, clsGlobVar.Tank328_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank328x, clsGlobVar.CoordinatePlanC.Tank328y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 329, clsGlobVar.Tank329_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank329x, clsGlobVar.CoordinatePlanC.Tank329y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 330, clsGlobVar.Tank330_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank330x, clsGlobVar.CoordinatePlanC.Tank330y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 331, clsGlobVar.Tank331_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank331x, clsGlobVar.CoordinatePlanC.Tank331y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 332, clsGlobVar.Tank332_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank332x, clsGlobVar.CoordinatePlanC.Tank332y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 333, clsGlobVar.Tank333_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank333x, clsGlobVar.CoordinatePlanC.Tank333y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 334, clsGlobVar.Tank334_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank334x, clsGlobVar.CoordinatePlanC.Tank334y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 335, clsGlobVar.Tank335_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank335x, clsGlobVar.CoordinatePlanC.Tank335y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 336, clsGlobVar.Tank336_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank336x, clsGlobVar.CoordinatePlanC.Tank336y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 337, clsGlobVar.Tank337_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank337x, clsGlobVar.CoordinatePlanC.Tank337y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 338, clsGlobVar.Tank338_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank338x, clsGlobVar.CoordinatePlanC.Tank338y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 339, clsGlobVar.Tank339_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank339x, clsGlobVar.CoordinatePlanC.Tank339y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 340, clsGlobVar.Tank340_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank340x, clsGlobVar.CoordinatePlanC.Tank340y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 341, clsGlobVar.Tank341_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank341x, clsGlobVar.CoordinatePlanC.Tank341y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 342, clsGlobVar.Tank342_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank342x, clsGlobVar.CoordinatePlanC.Tank342y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 343, clsGlobVar.Tank343_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank343x, clsGlobVar.CoordinatePlanC.Tank343y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 344, clsGlobVar.Tank344_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank344x, clsGlobVar.CoordinatePlanC.Tank344y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 345, clsGlobVar.Tank345_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank345x, clsGlobVar.CoordinatePlanC.Tank345y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 346, clsGlobVar.Tank346_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank346x, clsGlobVar.CoordinatePlanC.Tank346y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 347, clsGlobVar.Tank347_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank347x, clsGlobVar.CoordinatePlanC.Tank347y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 348, clsGlobVar.Tank348_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank348x, clsGlobVar.CoordinatePlanC.Tank348y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 349, clsGlobVar.Tank349_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank349x, clsGlobVar.CoordinatePlanC.Tank349y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 350, clsGlobVar.Tank350_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank350x, clsGlobVar.CoordinatePlanC.Tank350y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 351, clsGlobVar.Tank351_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank351x, clsGlobVar.CoordinatePlanC.Tank351y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 352, clsGlobVar.Tank352_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank352x, clsGlobVar.CoordinatePlanC.Tank352y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 353, clsGlobVar.Tank353_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank353x, clsGlobVar.CoordinatePlanC.Tank353y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 354, clsGlobVar.Tank354_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank354x, clsGlobVar.CoordinatePlanC.Tank354y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 355, clsGlobVar.Tank355_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank355x, clsGlobVar.CoordinatePlanC.Tank355y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 356, clsGlobVar.Tank356_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank356x, clsGlobVar.CoordinatePlanC.Tank356y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 357, clsGlobVar.Tank357_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank357x, clsGlobVar.CoordinatePlanC.Tank357y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 358, clsGlobVar.Tank358_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank358x, clsGlobVar.CoordinatePlanC.Tank358y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 359, clsGlobVar.Tank359_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank359x, clsGlobVar.CoordinatePlanC.Tank359y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 360, clsGlobVar.Tank360_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank360x, clsGlobVar.CoordinatePlanC.Tank360y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 362, clsGlobVar.Tank362_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank362x, clsGlobVar.CoordinatePlanC.Tank362y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 363, clsGlobVar.Tank363_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank363x, clsGlobVar.CoordinatePlanC.Tank363y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 364, clsGlobVar.Tank364_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank364x, clsGlobVar.CoordinatePlanC.Tank364y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 365, clsGlobVar.Tank365_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank365x, clsGlobVar.CoordinatePlanC.Tank365y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 366, clsGlobVar.Tank366_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank366x, clsGlobVar.CoordinatePlanC.Tank366y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 367, clsGlobVar.Tank367_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank367x, clsGlobVar.CoordinatePlanC.Tank367y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 368, clsGlobVar.Tank368_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank368x, clsGlobVar.CoordinatePlanC.Tank368y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 369, clsGlobVar.Tank369_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank369x, clsGlobVar.CoordinatePlanC.Tank369y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 370, clsGlobVar.Tank370_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank370x, clsGlobVar.CoordinatePlanC.Tank370y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 371, clsGlobVar.Tank371_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank371x, clsGlobVar.CoordinatePlanC.Tank371y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 372, clsGlobVar.Tank372_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank372x, clsGlobVar.CoordinatePlanC.Tank372y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 373, clsGlobVar.Tank373_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank373x, clsGlobVar.CoordinatePlanC.Tank373y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 374, clsGlobVar.Tank374_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank374x, clsGlobVar.CoordinatePlanC.Tank374y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 375, clsGlobVar.Tank375_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank375x, clsGlobVar.CoordinatePlanC.Tank375y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 376, clsGlobVar.Tank376_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank376x, clsGlobVar.CoordinatePlanC.Tank376y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 377, clsGlobVar.Tank377_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank377x, clsGlobVar.CoordinatePlanC.Tank377y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 378, clsGlobVar.Tank378_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank378x, clsGlobVar.CoordinatePlanC.Tank378y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 379, clsGlobVar.Tank379_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank379x, clsGlobVar.CoordinatePlanC.Tank379y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 380, clsGlobVar.Tank380_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank380x, clsGlobVar.CoordinatePlanC.Tank380y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 381, clsGlobVar.Tank381_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank381x, clsGlobVar.CoordinatePlanC.Tank381y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 382, clsGlobVar.Tank382_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank382x, clsGlobVar.CoordinatePlanC.Tank382y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 383, clsGlobVar.Tank383_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank383x, clsGlobVar.CoordinatePlanC.Tank383y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 384, clsGlobVar.Tank384_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank384x, clsGlobVar.CoordinatePlanC.Tank384y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 385, clsGlobVar.Tank385_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank385x, clsGlobVar.CoordinatePlanC.Tank385y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 386, clsGlobVar.Tank386_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank386x, clsGlobVar.CoordinatePlanC.Tank386y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 387, clsGlobVar.Tank387_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank387x, clsGlobVar.CoordinatePlanC.Tank387y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 388, clsGlobVar.Tank388_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank388x, clsGlobVar.CoordinatePlanC.Tank388y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 389, clsGlobVar.Tank389_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank389x, clsGlobVar.CoordinatePlanC.Tank389y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 390, clsGlobVar.Tank390_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank390x, clsGlobVar.CoordinatePlanC.Tank390y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 391, clsGlobVar.Tank391_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank391x, clsGlobVar.CoordinatePlanC.Tank391y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 392, clsGlobVar.Tank392_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank392x, clsGlobVar.CoordinatePlanC.Tank392y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 393, clsGlobVar.Tank393_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank393x, clsGlobVar.CoordinatePlanC.Tank393y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 394, clsGlobVar.Tank394_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank394x, clsGlobVar.CoordinatePlanC.Tank394y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 395, clsGlobVar.Tank395_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank395x, clsGlobVar.CoordinatePlanC.Tank395y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 396, clsGlobVar.Tank396_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank396x, clsGlobVar.CoordinatePlanC.Tank396y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 397, clsGlobVar.Tank397_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank397x, clsGlobVar.CoordinatePlanC.Tank397y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 399, clsGlobVar.Tank399_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank399x, clsGlobVar.CoordinatePlanC.Tank399y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 400, clsGlobVar.Tank400_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank400x, clsGlobVar.CoordinatePlanC.Tank400y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 401, clsGlobVar.Tank401_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank401x, clsGlobVar.CoordinatePlanC.Tank401y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 402, clsGlobVar.Tank402_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank402x, clsGlobVar.CoordinatePlanC.Tank402y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 403, clsGlobVar.Tank403_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank403x, clsGlobVar.CoordinatePlanC.Tank403y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 404, clsGlobVar.Tank404_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank404x, clsGlobVar.CoordinatePlanC.Tank404y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 405, clsGlobVar.Tank405_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank405x, clsGlobVar.CoordinatePlanC.Tank405y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 406, clsGlobVar.Tank406_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank406x, clsGlobVar.CoordinatePlanC.Tank406y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 407, clsGlobVar.Tank407_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank407x, clsGlobVar.CoordinatePlanC.Tank407y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 408, clsGlobVar.Tank408_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank408x, clsGlobVar.CoordinatePlanC.Tank408y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 409, clsGlobVar.Tank409_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank409x, clsGlobVar.CoordinatePlanC.Tank409y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 410, clsGlobVar.Tank410_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank410x, clsGlobVar.CoordinatePlanC.Tank410y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 411, clsGlobVar.Tank411_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank411x, clsGlobVar.CoordinatePlanC.Tank411y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 412, clsGlobVar.Tank412_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank412x, clsGlobVar.CoordinatePlanC.Tank412y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 413, clsGlobVar.Tank413_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank413x, clsGlobVar.CoordinatePlanC.Tank413y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 414, clsGlobVar.Tank414_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank414x, clsGlobVar.CoordinatePlanC.Tank414y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 415, clsGlobVar.Tank415_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank415x, clsGlobVar.CoordinatePlanC.Tank415y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 416, clsGlobVar.Tank416_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank416ax, clsGlobVar.CoordinatePlanC.Tank416ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 416, clsGlobVar.Tank416_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank416bx, clsGlobVar.CoordinatePlanC.Tank416by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 417, clsGlobVar.Tank417_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank417x, clsGlobVar.CoordinatePlanC.Tank417y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 418, clsGlobVar.Tank418_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank418x, clsGlobVar.CoordinatePlanC.Tank418y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 419, clsGlobVar.Tank419_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank419x, clsGlobVar.CoordinatePlanC.Tank419y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 420, clsGlobVar.Tank420_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank420x, clsGlobVar.CoordinatePlanC.Tank420y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 421, clsGlobVar.Tank421_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank421x, clsGlobVar.CoordinatePlanC.Tank421y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 423, clsGlobVar.Tank423_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank423x, clsGlobVar.CoordinatePlanC.Tank423y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 424, clsGlobVar.Tank424_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank424x, clsGlobVar.CoordinatePlanC.Tank424y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 425, clsGlobVar.Tank425_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank425x, clsGlobVar.CoordinatePlanC.Tank425y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 426, clsGlobVar.Tank426_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank426x, clsGlobVar.CoordinatePlanC.Tank426y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 427, clsGlobVar.Tank427_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank427x, clsGlobVar.CoordinatePlanC.Tank427y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 428, clsGlobVar.Tank428_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank428x, clsGlobVar.CoordinatePlanC.Tank428y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 429, clsGlobVar.Tank429_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank429x, clsGlobVar.CoordinatePlanC.Tank429y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 430, clsGlobVar.Tank430_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank430x, clsGlobVar.CoordinatePlanC.Tank430y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 431, clsGlobVar.Tank431_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank431x, clsGlobVar.CoordinatePlanC.Tank431y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 432, clsGlobVar.Tank432_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank432x, clsGlobVar.CoordinatePlanC.Tank432y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 433, clsGlobVar.Tank433_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank433x, clsGlobVar.CoordinatePlanC.Tank433y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 434, clsGlobVar.Tank434_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank434x, clsGlobVar.CoordinatePlanC.Tank434y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 435, clsGlobVar.Tank435_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank435x, clsGlobVar.CoordinatePlanC.Tank435y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 439, clsGlobVar.Tank439_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank439x, clsGlobVar.CoordinatePlanC.Tank439y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 443, clsGlobVar.Tank443_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank443x, clsGlobVar.CoordinatePlanC.Tank443y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 448, clsGlobVar.Tank448_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank448x, clsGlobVar.CoordinatePlanC.Tank448y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 449, clsGlobVar.Tank449_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank449x, clsGlobVar.CoordinatePlanC.Tank449y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 452, clsGlobVar.Tank452_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank452x, clsGlobVar.CoordinatePlanC.Tank452y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 453, clsGlobVar.Tank453_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank453x, clsGlobVar.CoordinatePlanC.Tank453y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 457, clsGlobVar.Tank457_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank457x, clsGlobVar.CoordinatePlanC.Tank457y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 461, clsGlobVar.Tank461_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank461x, clsGlobVar.CoordinatePlanC.Tank461y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 466, clsGlobVar.Tank466_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank466x, clsGlobVar.CoordinatePlanC.Tank466y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 467, clsGlobVar.Tank467_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank467x, clsGlobVar.CoordinatePlanC.Tank467y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 470, clsGlobVar.Tank470_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank470x, clsGlobVar.CoordinatePlanC.Tank470y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 475, clsGlobVar.Tank475_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank475ax, clsGlobVar.CoordinatePlanC.Tank475ay, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 475, clsGlobVar.Tank475_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank475bx, clsGlobVar.CoordinatePlanC.Tank475by, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 476, clsGlobVar.Tank476_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank476x, clsGlobVar.CoordinatePlanC.Tank476y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                DrawHatchDeckPlan(canvas2DPlanC, 478, clsGlobVar.Tank478_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank478x, clsGlobVar.CoordinatePlanC.Tank478y, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[477]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255, 218, 97, 78) : System.Windows.Media.Color.FromArgb(180, 141, 180, 227)));
                DrawHatchDeckPlan(canvas2DPlanC, 494, clsGlobVar.Tank494_SimulationPercentFill, clsGlobVar.CoordinatePlanC.Tank494x, clsGlobVar.CoordinatePlanC.Tank494y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));


            }
            catch //(Exception ex)
            {
               // System.Windows.MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Add Hatch Filling of Tanks/Compartments to Profile Canvas as per TankID 
        /// </summary>
        public void AddHatchProfile()
        {
            try
            {
                canvas2DProfile.Children.RemoveRange(1, canvas2DProfile.Children.Count - 1);

                object num ;
                System.Windows.Media.Color flag = (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[0]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(180, 141, 180, 227) : System.Windows.Media.Color.FromArgb(180, 255, 192, 203));
                //int flag = (((num = Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[0]["IsDamaged"]).ToString() == Boolean.TrueString) ? 1 : 0);

                DrawHatchProfile(canvas2DProfile, 10, 100, clsGlobVar.ProfileCoordinate.Tank1x, clsGlobVar.ProfileCoordinate.Tank1y,  (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[10]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 194, 214, 154)) );
                //DrawHatchProfile(canvas2DProfile, 2, clsGlobVar.Tank2_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank2x, clsGlobVar.ProfileCoordinate.Tank2y, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[1]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 194, 214, 154)) );
                //DrawHatchProfile(canvas2DProfile, 3, clsGlobVar.Tank4_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank4x, clsGlobVar.ProfileCoordinate.Tank4y, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[3]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 194, 214, 154)));
                //DrawHatchProfile(canvas2DProfile, 4, clsGlobVar.Tank6_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank6x, clsGlobVar.ProfileCoordinate.Tank6y,  (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[5]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 194, 214, 154)) ) ;
                //DrawHatchProfile(canvas2DProfile, 5, clsGlobVar.Tank9_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank9x, clsGlobVar.ProfileCoordinate.Tank9y,  (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[8]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 194, 214, 154)) );
                //DrawHatchProfile(canvas2DProfile, 6, clsGlobVar.Tank10_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank10x, clsGlobVar.ProfileCoordinate.Tank10y, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[9]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 194, 214, 154)));
                //DrawHatchProfile(canvas2DProfile, 7, clsGlobVar.Tank12_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank12x, clsGlobVar.ProfileCoordinate.Tank12y, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[11]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 141, 180, 227)));
                //DrawHatchProfile(canvas2DProfile, 8, clsGlobVar.Tank13_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank13x, clsGlobVar.ProfileCoordinate.Tank13y,(((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[12]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 141, 180, 227)));
                //DrawHatchProfile(canvas2DProfile, 9, clsGlobVar.Tank17_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank17x, clsGlobVar.ProfileCoordinate.Tank17y,(((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[16]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 151, 72, 7)));
                //DrawHatchProfile(canvas2DProfile, 10, clsGlobVar.Tank18_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank18x, clsGlobVar.ProfileCoordinate.Tank18y, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[17]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 151, 72, 7)));
                //DrawHatchProfile(canvas2DProfile, 11, clsGlobVar.Tank20_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank20x, clsGlobVar.ProfileCoordinate.Tank20y, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[19]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 151, 72, 7)));
                //DrawHatchProfile(canvas2DProfile, 12, clsGlobVar.Tank21_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank21x, clsGlobVar.ProfileCoordinate.Tank21y, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[20]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 151, 72, 7)));
                //DrawHatchProfile(canvas2DProfile, 12, clsGlobVar.Tank22_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank22x, clsGlobVar.ProfileCoordinate.Tank22y, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[21]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 151, 72, 7)));
                //DrawHatchProfile(canvas2DProfile, 13, clsGlobVar.Tank26_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank26x, clsGlobVar.ProfileCoordinate.Tank26y, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[25]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 151, 72, 7)));
                //DrawHatchProfile(canvas2DProfile, 14, clsGlobVar.Tank28_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank28x, clsGlobVar.ProfileCoordinate.Tank28y, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[27]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 151, 72, 7)));
                //DrawHatchProfile(canvas2DProfile, 15, clsGlobVar.Tank33_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank33x, clsGlobVar.ProfileCoordinate.Tank33y, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[32]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 151, 72, 7)));
                //DrawHatchProfile(canvas2DProfile, 16, clsGlobVar.Tank34_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank34x, clsGlobVar.ProfileCoordinate.Tank34y, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[33]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 151, 72, 7)));
                //DrawHatchProfile(canvas2DProfile, 17, clsGlobVar.Tank39_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank39x, clsGlobVar.ProfileCoordinate.Tank39y, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[38]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 151, 72, 7)));
                //DrawHatchProfile(canvas2DProfile, 18, clsGlobVar.Tank40_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank40x, clsGlobVar.ProfileCoordinate.Tank40y, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[39]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 151, 72, 7)));
                //DrawHatchProfile(canvas2DProfile, 19, clsGlobVar.Tank42_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank42x, clsGlobVar.ProfileCoordinate.Tank42y, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[41]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 151, 72, 7)));
                //DrawHatchProfile(canvas2DProfile, 20, clsGlobVar.Tank44_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank44x, clsGlobVar.ProfileCoordinate.Tank44y, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[43]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 151, 72, 7)));
                //DrawHatchProfile(canvas2DProfile, 21, clsGlobVar.Tank46_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank46x, clsGlobVar.ProfileCoordinate.Tank46y, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[45]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 203, 150, 69)));
                //DrawHatchProfile(canvas2DProfile, 22, clsGlobVar.Tank49_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank49x, clsGlobVar.ProfileCoordinate.Tank49y, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[48]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 255, 192, 203)));
                //DrawHatchProfile(canvas2DProfile, 23, clsGlobVar.Tank50_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank50x, clsGlobVar.ProfileCoordinate.Tank50y, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[49]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 255, 192, 203)));
                //DrawHatchProfile(canvas2DProfile, 24, clsGlobVar.Tank53_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank53x, clsGlobVar.ProfileCoordinate.Tank53y, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[52]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 255, 192, 203)));
                //DrawHatchProfile(canvas2DProfile, 25, clsGlobVar.Tank54_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank54x, clsGlobVar.ProfileCoordinate.Tank54y, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[53]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 255, 192, 203)));
                //DrawHatchProfile(canvas2DProfile, 26, clsGlobVar.Tank55_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank55x, clsGlobVar.ProfileCoordinate.Tank55y, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[54]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255,218,97,78) : System.Windows.Media.Color.FromArgb(180, 141, 180, 227)));
                //DrawHatchProfile(canvas2DProfile, 27, clsGlobVar.Tank56_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank56x, clsGlobVar.ProfileCoordinate.Tank56y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 28, clsGlobVar.Tank57_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank57x, clsGlobVar.ProfileCoordinate.Tank57y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 29, clsGlobVar.Tank65_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank65x, clsGlobVar.ProfileCoordinate.Tank65y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
                //DrawHatchProfile(canvas2DProfile, 30, clsGlobVar.Tank66_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank66x, clsGlobVar.ProfileCoordinate.Tank66y, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[65]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255, 218, 97, 78) : System.Windows.Media.Color.FromArgb(255, 218, 97, 78)));
                //DrawHatchProfile(canvas2DProfile, 31, clsGlobVar.Tank67_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank67x, clsGlobVar.ProfileCoordinate.Tank67y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // DrawHatchProfile(canvas2DProfile, 68, clsGlobVar.Tank68_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank68x, clsGlobVar.ProfileCoordinate.Tank68y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // DrawHatchProfile(canvas2DProfile, 69, clsGlobVar.Tank69_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank69x, clsGlobVar.ProfileCoordinate.Tank69y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // DrawHatchProfile(canvas2DProfile, 70, clsGlobVar.Tank70_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank70x, clsGlobVar.ProfileCoordinate.Tank70y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // DrawHatchProfile(canvas2DProfile, 76, clsGlobVar.Tank76_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank76x, clsGlobVar.ProfileCoordinate.Tank76y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // DrawHatchProfile(canvas2DProfile, 79, clsGlobVar.Tank79_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank79x, clsGlobVar.ProfileCoordinate.Tank79y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // DrawHatchProfile(canvas2DProfile, 80, clsGlobVar.Tank80_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank80x, clsGlobVar.ProfileCoordinate.Tank80y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // DrawHatchProfile(canvas2DProfile, 82, clsGlobVar.Tank82_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank82x, clsGlobVar.ProfileCoordinate.Tank82y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // DrawHatchProfile(canvas2DProfile, 84, clsGlobVar.Tank84_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank84x, clsGlobVar.ProfileCoordinate.Tank84y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // DrawHatchProfile(canvas2DProfile, 90, clsGlobVar.Tank90_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank90x, clsGlobVar.ProfileCoordinate.Tank90y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // DrawHatchProfile(canvas2DProfile, 92, clsGlobVar.Tank92_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank92x, clsGlobVar.ProfileCoordinate.Tank92y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // DrawHatchProfile(canvas2DProfile, 96, clsGlobVar.Tank96_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank96x, clsGlobVar.ProfileCoordinate.Tank96y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               //// DrawHatchProfile(canvas2DProfile, 101, clsGlobVar.Tank101_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank101x, clsGlobVar.ProfileCoordinate.Tank101y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               ////DrawHatchProfile(canvas2DProfile, 147, clsGlobVar.Tank147_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank147x, clsGlobVar.ProfileCoordinate.Tank147y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // DrawHatchProfile(canvas2DProfile, 150, clsGlobVar.Tank150_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank150x, clsGlobVar.ProfileCoordinate.Tank150y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               ////DrawHatchProfile(canvas2DProfile, 151, clsGlobVar.Tank151_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank151x, clsGlobVar.ProfileCoordinate.Tank151y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               ////DrawHatchProfile(canvas2DProfile, 177, clsGlobVar.Tank177_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank177x, clsGlobVar.ProfileCoordinate.Tank177y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // DrawHatchProfile(canvas2DProfile, 184, clsGlobVar.Tank184_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank184x, clsGlobVar.ProfileCoordinate.Tank184y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // DrawHatchProfile(canvas2DProfile, 185, clsGlobVar.Tank185_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank185x, clsGlobVar.ProfileCoordinate.Tank185y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // DrawHatchProfile(canvas2DProfile, 187, clsGlobVar.Tank187_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank187x, clsGlobVar.ProfileCoordinate.Tank187y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // //DrawHatchProfile(canvas2DProfile, 192, clsGlobVar.Tank192_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank192x, clsGlobVar.ProfileCoordinate.Tank192y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // DrawHatchProfile(canvas2DProfile, 195, clsGlobVar.Tank195_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank195x, clsGlobVar.ProfileCoordinate.Tank195y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // //DrawHatchProfile(canvas2DProfile, 227, clsGlobVar.Tank227_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank227x, clsGlobVar.ProfileCoordinate.Tank227y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // DrawHatchProfile(canvas2DProfile, 234, clsGlobVar.Tank234_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank234x, clsGlobVar.ProfileCoordinate.Tank234y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // DrawHatchProfile(canvas2DProfile, 236, clsGlobVar.Tank236_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank236x, clsGlobVar.ProfileCoordinate.Tank236y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // DrawHatchProfile(canvas2DProfile, 244, clsGlobVar.Tank244_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank244x, clsGlobVar.ProfileCoordinate.Tank244y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               //// DrawHatchProfile(canvas2DProfile, 250, clsGlobVar.Tank250_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank250x, clsGlobVar.ProfileCoordinate.Tank250y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // DrawHatchProfile(canvas2DProfile, 253, clsGlobVar.Tank253_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank253x, clsGlobVar.ProfileCoordinate.Tank253y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // DrawHatchProfile(canvas2DProfile, 254, clsGlobVar.Tank254_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank254x, clsGlobVar.ProfileCoordinate.Tank254y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               //// DrawHatchProfile(canvas2DProfile, 260, clsGlobVar.Tank260_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank260x, clsGlobVar.ProfileCoordinate.Tank260y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // //DrawHatchProfile(canvas2DProfile, 261, clsGlobVar.Tank261_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank261x, clsGlobVar.ProfileCoordinate.Tank261y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // //DrawHatchProfile(canvas2DProfile, 263, clsGlobVar.Tank263_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank263x, clsGlobVar.ProfileCoordinate.Tank263y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // //DrawHatchProfile(canvas2DProfile, 264, clsGlobVar.Tank264_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank264x, clsGlobVar.ProfileCoordinate.Tank264y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // //DrawHatchProfile(canvas2DProfile, 267, clsGlobVar.Tank267_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank267x, clsGlobVar.ProfileCoordinate.Tank267y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // //DrawHatchProfile(canvas2DProfile, 273, clsGlobVar.Tank273_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank273x, clsGlobVar.ProfileCoordinate.Tank273y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // DrawHatchProfile(canvas2DProfile, 275, clsGlobVar.Tank275_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank275x, clsGlobVar.ProfileCoordinate.Tank275y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // DrawHatchProfile(canvas2DProfile, 276, clsGlobVar.Tank276_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank276x, clsGlobVar.ProfileCoordinate.Tank276y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // //DrawHatchProfile(canvas2DProfile, 280, clsGlobVar.Tank280_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank280x, clsGlobVar.ProfileCoordinate.Tank280y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // //DrawHatchProfile(canvas2DProfile, 281, clsGlobVar.Tank281_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank281x, clsGlobVar.ProfileCoordinate.Tank281y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // DrawHatchProfile(canvas2DProfile, 284, clsGlobVar.Tank284_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank284x, clsGlobVar.ProfileCoordinate.Tank284y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               //// DrawHatchProfile(canvas2DProfile, 285, clsGlobVar.Tank285_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank285x, clsGlobVar.ProfileCoordinate.Tank285y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // DrawHatchProfile(canvas2DProfile, 292, clsGlobVar.Tank292_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank292x, clsGlobVar.ProfileCoordinate.Tank292y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // //DrawHatchProfile(canvas2DProfile, 293, clsGlobVar.Tank293_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank293x, clsGlobVar.ProfileCoordinate.Tank293y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // DrawHatchProfile(canvas2DProfile, 297, clsGlobVar.Tank297_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank297x, clsGlobVar.ProfileCoordinate.Tank297y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // DrawHatchProfile(canvas2DProfile, 298, clsGlobVar.Tank298_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank298x, clsGlobVar.ProfileCoordinate.Tank298y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // //DrawHatchProfile(canvas2DProfile, 299, clsGlobVar.Tank299_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank299x, clsGlobVar.ProfileCoordinate.Tank299y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // DrawHatchProfile(canvas2DProfile, 301, clsGlobVar.Tank301_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank301x, clsGlobVar.ProfileCoordinate.Tank301y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // //DrawHatchProfile(canvas2DProfile, 303, clsGlobVar.Tank303_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank303x, clsGlobVar.ProfileCoordinate.Tank303y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               ////DrawHatchProfile(canvas2DProfile, 304, clsGlobVar.Tank304_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank304x, clsGlobVar.ProfileCoordinate.Tank304y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // //DrawHatchProfile(canvas2DProfile, 311, clsGlobVar.Tank311_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank311x, clsGlobVar.ProfileCoordinate.Tank311y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // //DrawHatchProfile(canvas2DProfile, 312, clsGlobVar.Tank312_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank312x, clsGlobVar.ProfileCoordinate.Tank312y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // //DrawHatchProfile(canvas2DProfile, 325, clsGlobVar.Tank325_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank325x, clsGlobVar.ProfileCoordinate.Tank325y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // DrawHatchProfile(canvas2DProfile, 326, clsGlobVar.Tank326_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank326x, clsGlobVar.ProfileCoordinate.Tank326y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // DrawHatchProfile(canvas2DProfile, 336, clsGlobVar.Tank336_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank336x, clsGlobVar.ProfileCoordinate.Tank336y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               //// DrawHatchProfile(canvas2DProfile, 356, clsGlobVar.Tank356_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank356x, clsGlobVar.ProfileCoordinate.Tank356y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // DrawHatchProfile(canvas2DProfile, 360, clsGlobVar.Tank360_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank360x, clsGlobVar.ProfileCoordinate.Tank360y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // DrawHatchProfile(canvas2DProfile, 380, clsGlobVar.Tank380_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank380x, clsGlobVar.ProfileCoordinate.Tank380y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // DrawHatchProfile(canvas2DProfile, 392, clsGlobVar.Tank392_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank392x, clsGlobVar.ProfileCoordinate.Tank392y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // DrawHatchProfile(canvas2DProfile, 411, clsGlobVar.Tank411_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank411x, clsGlobVar.ProfileCoordinate.Tank411y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               ////DrawHatchProfile(canvas2DProfile, 424, clsGlobVar.Tank424_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank424x, clsGlobVar.ProfileCoordinate.Tank424y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               //// DrawHatchProfile(canvas2DProfile, 427, clsGlobVar.Tank427_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank427x, clsGlobVar.ProfileCoordinate.Tank427y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // //DrawHatchProfile(canvas2DProfile, 428, clsGlobVar.Tank428_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank428x, clsGlobVar.ProfileCoordinate.Tank428y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // DrawHatchProfile(canvas2DProfile, 430, clsGlobVar.Tank430_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank430x, clsGlobVar.ProfileCoordinate.Tank430y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               //// DrawHatchProfile(canvas2DProfile, 432, clsGlobVar.Tank432_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank432x, clsGlobVar.ProfileCoordinate.Tank432y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));

               // DrawHatchProfile(canvas2DProfile, 433, clsGlobVar.Tank433_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank433x, clsGlobVar.ProfileCoordinate.Tank433y, System.Windows.Media.Color.FromArgb(180, 255, 0, 0));
               // DrawHatchProfile(canvas2DProfile, 434, clsGlobVar.Tank434_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank434x, clsGlobVar.ProfileCoordinate.Tank434y,(((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[102]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255, 218, 97, 78) : System.Windows.Media.Color.FromArgb(255, 218, 97, 78)));
               // DrawHatchProfile(canvas2DProfile, 494, clsGlobVar.Tank494_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank494x, clsGlobVar.ProfileCoordinate.Tank494y, System.Windows.Media.Color.FromArgb(220, 255, 0, 0));
               // DrawHatchProfile(canvas2DProfile, 497, clsGlobVar.Tank497_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank497x, clsGlobVar.ProfileCoordinate.Tank497y, System.Windows.Media.Color.FromArgb(220, 255, 0, 0));
               // DrawHatchProfile(canvas2DProfile, 498, clsGlobVar.Tank498_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank498x, clsGlobVar.ProfileCoordinate.Tank498y, System.Windows.Media.Color.FromArgb(220, 255, 0, 0));
               // //below line is for no id box 503//
               // DrawHatchProfile(canvas2DProfile, 503, clsGlobVar.Tank503_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank503x, clsGlobVar.ProfileCoordinate.Tank503y, (((Models.BO.clsGlobVar.dtSimulationLoadingSummary.Rows[103]["IsDamaged"]).ToString() == Boolean.TrueString) ? System.Windows.Media.Color.FromArgb(255, 141, 180, 227) : System.Windows.Media.Color.FromArgb(180, 141, 180, 227)));
               // DrawHatchProfile(canvas2DProfile, 504, clsGlobVar.Tank504_SimulationPercentFill, clsGlobVar.ProfileCoordinate.Tank504x, clsGlobVar.ProfileCoordinate.Tank504y, System.Windows.Media.Color.FromArgb(220, 255, 0, 0));

                UpdateRenderTransform(canvas2DProfile);
                DrawTrimLine();
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

                if ((Convert.ToBoolean(Models.BO.clsGlobVar.dtSimulationModeAllTanks.Rows[Tank_ID - 1]["IsDamaged"]) == true) && (Convert.ToInt64(Models.BO.clsGlobVar.dtSimulationModeAllTanks.Rows[Tank_ID - 1]["Status"]) == 1))
                {
                    System.Windows.Media.Color color1 = System.Windows.Media.Color.FromArgb(220, 255, 0, 0);
                    percent = 100;
                    #region DamageCases
                    {
                        if (Tank_ID == 4 )
                        {
                            x1 = 34506.7765;
                            y1 = 8163.7309;
                            x2 = 34506.7765;
                            y2 = 8651.8266;
                            x3 = 34506.7765;
                            y3 = 11774.7089;
                            x4 = 34506.7765;
                            y4 = 11774.7089;
                            x5 = 32256.1412;
                            y5 = 8451.4532;
                            x6 = 33056.3671;
                            y6 = 8163.7309;
                           
                            {
                                #region profile
                                {
                                    System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                                    p.Stroke = System.Windows.Media.Brushes.Black;
                                    SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                                    mySolidColorBrush.Color = color1;
                                    p.Fill = mySolidColorBrush;
                                    double d = y3 - y2;
                                    double Fill = Convert.ToInt32(percent) * (d / 100);
                                    System.Windows.Point[] point = new System.Windows.Point[25];
                                    PointCollection pointCollection = new PointCollection();
                                    pointCollection.Add(new System.Windows.Point(x1, y1));
                                    pointCollection.Add(new System.Windows.Point(x6, y6));
                                    pointCollection.Add(new System.Windows.Point(x5, y5));
                                    pointCollection.Add(new System.Windows.Point(x5, y5 + Fill));
                                    pointCollection.Add(new System.Windows.Point(x2, y2 + Fill));
                                    p.Points = pointCollection;
                                    canvasTwoD.Children.Add(p);
                                }
                                #endregion
                                #region All
                                {
                                    System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                                    p.Stroke = System.Windows.Media.Brushes.Black;
                                    SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                                    mySolidColorBrush.Color = color1;
                                    p.Fill = mySolidColorBrush;
                                    double d = y4 - y5;
                                    double Fill = Convert.ToInt32(percent) * (d / 100);
                                    System.Windows.Point[] point = new System.Windows.Point[25];
                                    PointCollection pointCollection = new PointCollection();
                                    pointCollection.Add(new System.Windows.Point(x1, y1));
                                    pointCollection.Add(new System.Windows.Point(x6, y6));
                                    pointCollection.Add(new System.Windows.Point(x5, y5));
                                    pointCollection.Add(new System.Windows.Point(x5, y5 + Fill));
                                    pointCollection.Add(new System.Windows.Point(x2, y2 + Fill));
                                    p.Points = pointCollection;
                                    canvas2DAll.Children.Add(p);
                                    percent = 0;
                                }
                                #endregion
                            }
                        }
                        else if (Tank_ID == 6)
                        {

                            x1 = 16504.6588 + 10477;
                            y1 = 3649.1089 + 5958;
                            x2 = 16504.6588 + 10477;
                            y2 = 5896.7434 + 5958;
                            x3 = 12003.3883 + 10477;
                            y3 = 5896.7434 + 5958;
                            x4 = 12003.3883 + 10477;
                            y4 = 4481.3438 + 5958;
                         
                          
                            {

                                #region profile
                                {

                                    System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                                    p.Stroke = System.Windows.Media.Brushes.Black;
                                    SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                                    mySolidColorBrush.Color = color1;
                                    p.Fill = mySolidColorBrush;
                                    double d = y2 - y4;
                                    double Fill = Convert.ToInt32(percent) * (d / 100);
                                    System.Windows.Point[] point = new System.Windows.Point[25];
                                    PointCollection pointCollection = new PointCollection();
                                    pointCollection.Add(new System.Windows.Point(x1, y1));
                                    pointCollection.Add(new System.Windows.Point(x1, y4 + Fill));
                                    pointCollection.Add(new System.Windows.Point(x3, y4 + Fill));
                                    pointCollection.Add(new System.Windows.Point(x4, y4));
                                    p.Points = pointCollection;
                                    canvasTwoD.Children.Add(p);
                                }
                                #endregion

                                #region  All
                                {
                                    
                                    {
                                        System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                                        p.Stroke = System.Windows.Media.Brushes.Black;
                                        SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                                        mySolidColorBrush.Color = color1;
                                        p.Fill = mySolidColorBrush;
                                        double d = y2 - y4;
                                        double Fill = Convert.ToInt32(percent) * (d / 100);
                                        // double dx = x3 - x2;
                                        // double Fillx = Convert.ToInt32(percent) * (dx / 100);
                                        System.Windows.Point[] point = new System.Windows.Point[25];
                                        PointCollection pointCollection = new PointCollection();
                                        pointCollection.Add(new System.Windows.Point(x1, y1));
                                        pointCollection.Add(new System.Windows.Point(x1, y4 + Fill));
                                        pointCollection.Add(new System.Windows.Point(x3, y4 + Fill));
                                        pointCollection.Add(new System.Windows.Point(x4, y4));
                                        p.Points = pointCollection;
                                        canvas2DAll.Children.Add(p);
                                        percent = 0;
                                    }
                                }
                                #endregion
                            }
                        }

                        else if (Tank_ID == 13)
                        {
                            x1 = 21756.1412 + 10477;
                            y1 = 2451.4532 + 5958;
                            x2 = 21756.1412 + 10477;
                            y2 = 5774.7089 + 5958;
                            x3 = 19505.5059 + 10477;
                            y3 = 5774.7089 + 5958;
                            x4 = 18005.0824 + 10477;
                            y4 = 5896.7434 + 5958;
                            x5 = 18005.0824 + 10477;
                            y5 = 3800.1515 + 5958;
                            x6 = 19756.8863 + 10477;
                            y6 = 3170.2881 + 5958;
                           
                            {
                                #region profile
                                {
                                    System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                                    p.Stroke = System.Windows.Media.Brushes.Black;
                                    SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                                    mySolidColorBrush.Color = color1;
                                    p.Fill = mySolidColorBrush;
                                    double d = y2 - y5;
                                    double Fill = Convert.ToInt32(percent) * (d / 100);
                                    System.Windows.Point[] point = new System.Windows.Point[25];
                                    PointCollection pointCollection = new PointCollection();
                                    pointCollection.Add(new System.Windows.Point(x1, y1));
                                    pointCollection.Add(new System.Windows.Point(x1, y6));
                                    pointCollection.Add(new System.Windows.Point(x1, y5));
                                    pointCollection.Add(new System.Windows.Point(x1, y5 + Fill));
                                    pointCollection.Add(new System.Windows.Point(x4, y5 + Fill));
                                    pointCollection.Add(new System.Windows.Point(x5, y5));
                                    p.Points = pointCollection;
                                    canvasTwoD.Children.Add(p);
                                    //percent = 0;
                                }
                                #endregion

                                #region All
                                {
                                    if (percent > 20 && percent <= 100)
                                    {
                                        System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                                        p.Stroke = System.Windows.Media.Brushes.Black;
                                        SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                                        mySolidColorBrush.Color = color1;
                                        p.Fill = mySolidColorBrush;
                                        double d = y2 - y5;
                                        double Fill = Convert.ToInt32(percent) * (d / 100);
                                        System.Windows.Point[] point = new System.Windows.Point[25];
                                        PointCollection pointCollection = new PointCollection();
                                        pointCollection.Add(new System.Windows.Point(x1, y1));
                                        pointCollection.Add(new System.Windows.Point(x1, y6));
                                        pointCollection.Add(new System.Windows.Point(x1, y5));
                                        pointCollection.Add(new System.Windows.Point(x1, y5 + Fill));
                                        pointCollection.Add(new System.Windows.Point(x4, y5 + Fill));
                                        pointCollection.Add(new System.Windows.Point(x5, y5));
                                        p.Points = pointCollection;
                                        canvas2DAll.Children.Add(p);
                                        percent = 0;
                                    }
                                }
                                #endregion

                            }
                        }
                        else if (Tank_ID == 12)
                        {
                            x1 = 148538.9649;
                            y1 = 7075.7311;
                            x2 = 148538.9649;
                            y2 = 7659.9501;
                            x3 = 148538.9649;
                            y3 = 11774.7089;
                            x4 = 144037.6943;
                            y4 = 11774.7089;
                            x5 = 144037.6943;
                            y5 = 7602.5992;
                            x6 = 146836.4823;
                            y6 = 7576.523;
                           
                            {
                                #region profile
                                {
                                    System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                                    p.Stroke = System.Windows.Media.Brushes.Black;
                                    SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                                    mySolidColorBrush.Color = color1;
                                    p.Fill = mySolidColorBrush;
                                    double d = y4 - y6;
                                    double Fill = Convert.ToInt32(percent) * (d / 100);
                                    System.Windows.Point[] point = new System.Windows.Point[25];
                                    PointCollection pointCollection = new PointCollection();
                                    pointCollection.Add(new System.Windows.Point(x1, y1));
                                    //  pointCollection.Add(new System.Windows.Point(x2, y2));
                                    pointCollection.Add(new System.Windows.Point(x6, y6));
                                    pointCollection.Add(new System.Windows.Point(x5, y5));
                                    pointCollection.Add(new System.Windows.Point(x5, y5 + Fill));
                                    pointCollection.Add(new System.Windows.Point(x2, y2 + Fill));

                                    p.Points = pointCollection;
                                    canvasTwoD.Children.Add(p);
                                }
                                #endregion
                                #region All
                                {

                                    System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                                    p.Stroke = System.Windows.Media.Brushes.Black;
                                    SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                                    mySolidColorBrush.Color = color1;
                                    p.Fill = mySolidColorBrush;
                                    double d = y4 - y6;
                                    double Fill = Convert.ToInt32(percent) * (d / 100);
                                    System.Windows.Point[] point = new System.Windows.Point[25];
                                    PointCollection pointCollection = new PointCollection();
                                    pointCollection.Add(new System.Windows.Point(x1, y1));
                                    //  pointCollection.Add(new System.Windows.Point(x2, y2));
                                    pointCollection.Add(new System.Windows.Point(x6, y6));
                                    pointCollection.Add(new System.Windows.Point(x5, y5));
                                    pointCollection.Add(new System.Windows.Point(x5, y5 + Fill));
                                    pointCollection.Add(new System.Windows.Point(x2, y2 + Fill));

                                    p.Points = pointCollection;
                                    canvas2DAll.Children.Add(p);
                                    percent = 0;
                                }
                                #endregion

                            }


                        }
                        else if (Tank_ID == 55)
                        {
                            x1 = 138038.9649 + 10477; y1 = 1075.7311 + 5958;
                            x2 = 139912.5040 + 10477; y2 = 538.2302 + 5958;
                            x3 = 142349.6788 + 10477; y3 = -49.9359 + 5958;
                            x4 = 145441.1579 + 10477; y4 = -87.9468 + 5958;
                            x5 = 145930.1939 + 10477; y5 = -34.9319 + 5958;
                            x6 = 146646.8940 + 10477; y6 = 427.6990 + 5958;
                            x7 = 147220.3972 + 10477; y7 = 1075.7311 + 5958;
                            x8 = 148191.8308 + 10477; y8 = 2221.7056 + 5958;
                            x9 = 149040.7049 + 10477; y9 = 3251.0264 + 5958;
                            x10 = 138038.9649 + 10477; y10 = 3251.0264 + 5958;

                          
                           
                            {
                                #region
                                {
                                    System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                                    p.Stroke = System.Windows.Media.Brushes.Black;
                                    SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                                    mySolidColorBrush.Color = color1;
                                    p.Fill = mySolidColorBrush;
                                    double d = y10 - y1;
                                    double dx = x9 - x7;
                                    double Fill = Convert.ToInt32(percent) * (d / 100);
                                    double Fillx = Convert.ToInt32(percent) * (dx / 100);
                                    System.Windows.Point[] point = new System.Windows.Point[25];
                                    PointCollection pointCollection = new PointCollection();
                                    pointCollection.Add(new System.Windows.Point(x1, y1));
                                    pointCollection.Add(new System.Windows.Point(x2, y2));
                                    pointCollection.Add(new System.Windows.Point(x3, y3));
                                    pointCollection.Add(new System.Windows.Point(x5, y5));
                                    pointCollection.Add(new System.Windows.Point(x6, y6));
                                    pointCollection.Add(new System.Windows.Point(x7, y7));
                                    pointCollection.Add(new System.Windows.Point(x1, y1));
                                    pointCollection.Add(new System.Windows.Point(x7, y7));
                                    pointCollection.Add(new System.Windows.Point(x7 + Fillx, y7 + Fill));
                                    pointCollection.Add(new System.Windows.Point(x1, y7 + Fill));
                                    p.Points = pointCollection;
                                    canvasTwoD.Children.Add(p);
                                    //percent = 0;
                                }
                                #endregion

                                #region
                                {
                                    if (percent > 25 && percent <= 100)
                                    {
                                        System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                                        p.Stroke = System.Windows.Media.Brushes.Black;
                                        SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                                        mySolidColorBrush.Color = color1;
                                        p.Fill = mySolidColorBrush;
                                        double d = y10 - y1;
                                        double dx = x9 - x7;
                                        double Fill = Convert.ToInt32(percent) * (d / 100);
                                        double Fillx = Convert.ToInt32(percent) * (dx / 100);
                                        System.Windows.Point[] point = new System.Windows.Point[25];
                                        PointCollection pointCollection = new PointCollection();
                                        pointCollection.Add(new System.Windows.Point(x1, y1));
                                        pointCollection.Add(new System.Windows.Point(x2, y2));
                                        pointCollection.Add(new System.Windows.Point(x3, y3));
                                        pointCollection.Add(new System.Windows.Point(x5, y5));
                                        pointCollection.Add(new System.Windows.Point(x6, y6));
                                        pointCollection.Add(new System.Windows.Point(x7, y7));
                                        pointCollection.Add(new System.Windows.Point(x1, y1));
                                        pointCollection.Add(new System.Windows.Point(x7, y7));
                                        pointCollection.Add(new System.Windows.Point(x7 + Fillx, y7 + Fill));
                                        pointCollection.Add(new System.Windows.Point(x1, y7 + Fill));
                                        p.Points = pointCollection;
                                        canvas2DAll.Children.Add(p);
                                        percent = 0;
                                    }
                                }
                                #endregion
                            }
                        }
                        //else if (Tank_ID == 65)
                        //{
                        //    x1 = 9002.5412 + 10477; y1 = 4811.4365 + 5958;
                        //    x2 = 9002.5412 + 10477; y2 = 5896.7434 + 5958;
                        //    x3 = -422.3398 + 10477; y3 = 5896.7434 + 5958;
                        //    x4 = -248.2649 + 10477; y4 = 5289.6719 + 5958;
                        //    x5 = 3375.9529 + 10477; y5 = 5189.0425 + 5958;
                        //    x6 = 6301.7788 + 10477; y6 = 5035.1904 + 5958;

                           
                          
                        //    {
                        //        #region
                        //        {
                        //            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                        //            p.Stroke = System.Windows.Media.Brushes.Black;
                        //            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                        //            mySolidColorBrush.Color = color1;
                        //            p.Fill = mySolidColorBrush;
                        //            double d = y2 - y4;
                        //            double dx = x3 - x2;
                        //            double Fill = Convert.ToInt32(percent) * (d / 100);
                        //            double Fillx = Convert.ToInt32(percent) * (dx / 100);

                        //            System.Windows.Point[] point = new System.Windows.Point[25];
                        //            PointCollection pointCollection = new PointCollection();
                        //            pointCollection.Add(new System.Windows.Point(x1, y1));
                        //            pointCollection.Add(new System.Windows.Point(x1, y6));
                        //            pointCollection.Add(new System.Windows.Point(x1, y5));
                        //            pointCollection.Add(new System.Windows.Point(x1, y4));
                        //            pointCollection.Add(new System.Windows.Point(x1, y4 + Fill));
                        //            pointCollection.Add(new System.Windows.Point(x3, y4 + Fill));
                        //            pointCollection.Add(new System.Windows.Point(x4, y4));
                        //            p.Points = pointCollection;
                        //            canvasTwoD.Children.Add(p);
                        //            //percent = 0;
                        //        }
                        //        #endregion

                        //        #region
                        //        {
                        //            if (percent <= 100)
                        //            {
                        //                System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                        //                p.Stroke = System.Windows.Media.Brushes.Black;
                        //                SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                        //                mySolidColorBrush.Color = color1;
                        //                p.Fill = mySolidColorBrush;
                        //                double d = y2 - y4;
                        //                double dx = x3 - x2;
                        //                double Fill = Convert.ToInt32(percent) * (d / 100);
                        //                double Fillx = Convert.ToInt32(percent) * (dx / 100);

                        //                System.Windows.Point[] point = new System.Windows.Point[25];
                        //                PointCollection pointCollection = new PointCollection();
                        //                pointCollection.Add(new System.Windows.Point(x1, y1));
                        //                pointCollection.Add(new System.Windows.Point(x1, y6));
                        //                pointCollection.Add(new System.Windows.Point(x1, y5));
                        //                pointCollection.Add(new System.Windows.Point(x1, y4));
                        //                pointCollection.Add(new System.Windows.Point(x1, y4 + Fill));
                        //                pointCollection.Add(new System.Windows.Point(x3, y4 + Fill));
                        //                pointCollection.Add(new System.Windows.Point(x4, y4));
                        //                p.Points = pointCollection;
                        //                canvas2DAll.Children.Add(p);
                        //                percent = 0;
                        //            }
                        //        }
                        //        #endregion
                        //    }

                        //}

                        //else if (Tank_ID == 66 )
                        //{
                        //    x1 = 149040.7049 + 10477; y1 = 3251.0264 + 5958;
                        //    x2 = 149960.8238 + 10477; y2 = 4374.3121 + 5958;
                        //    x3 = 151124.1056 + 10477; y3 = 5774.7089 + 5958;
                        //    x4 = 141039.8120 + 10477; y4 = 5774.7089 + 5958;
                        //    x5 = 141039.8120 + 10477; y5 = 3219.0630 + 5958;

                          
                        //    {
                        //        #region
                        //        {
                        //            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                        //            p.Stroke = System.Windows.Media.Brushes.Black;
                        //            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                        //            mySolidColorBrush.Color = color1;
                        //            p.Fill = mySolidColorBrush;
                        //            double d = y2 - y1;
                        //            double dx = x3 - x2;
                        //            double Fill = Convert.ToInt32(percent) * (d / 90);
                        //            double Fillx = Convert.ToInt32(percent) * (dx / 90);

                        //            System.Windows.Point[] point = new System.Windows.Point[25];
                        //            PointCollection pointCollection = new PointCollection();
                        //            pointCollection.Add(new System.Windows.Point(x5, y5));
                        //            pointCollection.Add(new System.Windows.Point(x1, y1));
                        //            pointCollection.Add(new System.Windows.Point(x2, y2));
                        //            pointCollection.Add(new System.Windows.Point(x5, y5));
                        //            pointCollection.Add(new System.Windows.Point(x2, y2));
                        //            pointCollection.Add(new System.Windows.Point(x2 + Fillx, y2 + Fill));
                        //            pointCollection.Add(new System.Windows.Point(x4, y2 + Fill));
                        //            p.Points = pointCollection;
                        //            canvasTwoD.Children.Add(p);
                        //            //percent = 0;
                        //        }
                        //        #endregion

                        //        #region
                        //        {
                        //            if (percent <= 100)
                        //            {
                        //                System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                        //                p.Stroke = System.Windows.Media.Brushes.Black;
                        //                SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                        //                mySolidColorBrush.Color = color1;
                        //                p.Fill = mySolidColorBrush;
                        //                double d = y2 - y1;
                        //                double dx = x3 - x2;
                        //                double Fill = Convert.ToInt32(percent) * (d / 90);
                        //                double Fillx = Convert.ToInt32(percent) * (dx / 90);

                        //                System.Windows.Point[] point = new System.Windows.Point[25];
                        //                PointCollection pointCollection = new PointCollection();
                        //                pointCollection.Add(new System.Windows.Point(x5, y5));
                        //                pointCollection.Add(new System.Windows.Point(x1, y1));
                        //                pointCollection.Add(new System.Windows.Point(x2, y2));
                        //                pointCollection.Add(new System.Windows.Point(x5, y5));
                        //                pointCollection.Add(new System.Windows.Point(x2, y2));
                        //                pointCollection.Add(new System.Windows.Point(x2 + Fillx, y2 + Fill));
                        //                pointCollection.Add(new System.Windows.Point(x4, y2 + Fill));
                        //                p.Points = pointCollection;
                        //                canvas2DAll.Children.Add(p);
                        //                percent = 0;
                        //            }
                        //        }
                        //        #endregion
                        //    }
                        //}

                        //else if (Tank_ID == 301)
                        //{
                        //    x1 = 151119.0538 + 10477; y1 = 5774.7089 + 5958;
                        //    x2 = 152626.5848 + 10477; y2 = 7500.6959 + 5958;
                        //    x3 = 154183.5245 + 10477; y3 = 9126.6546 + 5958;
                        //    x4 = 156101.5399 + 10477; y4 = 10926.163 + 5958;
                        //    x5 = 145541.0825 + 10477; y5 = 10926.163 + 5958;
                        //    x6 = 145541.0825 + 10477; y6 = 5774.7089 + 5958;

                        //    {
                        //        #region
                        //        {
                        //            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                        //            p.Stroke = System.Windows.Media.Brushes.Black;
                        //            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                        //            mySolidColorBrush.Color = color1;
                        //            p.Fill = mySolidColorBrush;
                        //            double d = y3 - y2;
                        //            double dx = x4 - x3;
                        //            double Fill = Convert.ToInt32(percent) * (d / 100);
                        //            double Fillx = Convert.ToInt32(percent) * (dx / 100);

                        //            System.Windows.Point[] point = new System.Windows.Point[25];
                        //            PointCollection pointCollection = new PointCollection();
                        //            pointCollection.Add(new System.Windows.Point(x1, y1));
                        //            pointCollection.Add(new System.Windows.Point(x2, y2));
                        //            pointCollection.Add(new System.Windows.Point(x3, y3));
                        //            pointCollection.Add(new System.Windows.Point(x3 + Fill, y3 + Fill));
                        //            pointCollection.Add(new System.Windows.Point(x5, y3 + Fill));
                        //            pointCollection.Add(new System.Windows.Point(x5, y1));
                        //            p.Points = pointCollection;
                        //            canvasTwoD.Children.Add(p);
                        //            //percent = 0;
                        //        }
                        //        #endregion

                        //        #region
                        //        {
                        //            if (percent <= 100)
                        //            {
                        //                System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                        //                p.Stroke = System.Windows.Media.Brushes.Black;
                        //                SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                        //                mySolidColorBrush.Color = color1;
                        //                p.Fill = mySolidColorBrush;
                        //                double d = y3 - y2;
                        //                double dx = x4 - x3;
                        //                double Fill = Convert.ToInt32(percent) * (d / 100);
                        //                double Fillx = Convert.ToInt32(percent) * (dx / 100);

                        //                System.Windows.Point[] point = new System.Windows.Point[25];
                        //                PointCollection pointCollection = new PointCollection();
                        //                pointCollection.Add(new System.Windows.Point(x1, y1));
                        //                pointCollection.Add(new System.Windows.Point(x2, y2));
                        //                pointCollection.Add(new System.Windows.Point(x3, y3));
                        //                pointCollection.Add(new System.Windows.Point(x3 + Fill, y3 + Fill));
                        //                pointCollection.Add(new System.Windows.Point(x5, y3 + Fill));
                        //                pointCollection.Add(new System.Windows.Point(x5, y1));
                        //                p.Points = pointCollection;
                        //                canvas2DAll.Children.Add(p);
                        //                percent = 0;
                        //            }
                        //        }
                        //        #endregion
                        //    }


                        //}
                        //else if (Tank_ID == 434)
                        //{
                        //    x1 = 156101.5399 + 10477; y1 = 10926.163 + 5958;
                        //    x2 = 157544.4708 + 10477; y2 = 12171.5143 + 5958;
                        //    x3 = 159330.4791 + 10477; y3 = 13645.9303 + 5958;
                        //    x4 = 160806.3667 + 10477; y4 = 14822.1868 + 5958;
                        //    x5 = 150042.3531 + 10477; y5 = 14405.025 + 5958;
                        //    x6 = 150042.3531 + 10477; y6 = 10926.163 + 5958;
                          
                      
                        //    {
                        //        #region
                        //        {
                        //            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                        //            p.Stroke = System.Windows.Media.Brushes.Black;
                        //            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                        //            mySolidColorBrush.Color = color1;
                        //            p.Fill = mySolidColorBrush;
                        //            double d = y4 - y3;
                        //            double dx = x4 - x3;
                        //            double Fill = Convert.ToInt32(percent) * (d / 100);
                        //            double Fillx = Convert.ToInt32(percent) * (dx / 100);

                        //            System.Windows.Point[] point = new System.Windows.Point[25];
                        //            PointCollection pointCollection = new PointCollection();
                        //            pointCollection.Add(new System.Windows.Point(x1, y1));
                        //            pointCollection.Add(new System.Windows.Point(x2, y2));
                        //            pointCollection.Add(new System.Windows.Point(x3, y3));
                        //            pointCollection.Add(new System.Windows.Point(x3 + Fill, y3 + Fill));
                        //            pointCollection.Add(new System.Windows.Point(x5, y3 + Fill));
                        //            pointCollection.Add(new System.Windows.Point(x5, y1));
                        //            p.Points = pointCollection;
                        //            canvasTwoD.Children.Add(p);
                        //            //percent = 0;
                        //        }
                        //        #endregion

                        //        #region
                        //        {
                        //            if (percent <= 100)
                        //            {
                        //                System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                        //                p.Stroke = System.Windows.Media.Brushes.Black;
                        //                SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                        //                mySolidColorBrush.Color = color1;
                        //                p.Fill = mySolidColorBrush;
                        //                double d = y4 - y3;
                        //                double dx = x4 - x3;
                        //                double Fill = Convert.ToInt32(percent) * (d / 100);
                        //                double Fillx = Convert.ToInt32(percent) * (dx / 100);

                        //                System.Windows.Point[] point = new System.Windows.Point[25];
                        //                PointCollection pointCollection = new PointCollection();
                        //                pointCollection.Add(new System.Windows.Point(x1, y1));
                        //                pointCollection.Add(new System.Windows.Point(x2, y2));
                        //                pointCollection.Add(new System.Windows.Point(x3, y3));
                        //                pointCollection.Add(new System.Windows.Point(x3 + Fill, y3 + Fill));
                        //                pointCollection.Add(new System.Windows.Point(x5, y3 + Fill));
                        //                pointCollection.Add(new System.Windows.Point(x5, y1));
                        //                p.Points = pointCollection;
                        //                canvas2DAll.Children.Add(p);
                        //                percent = 0;
                        //            }
                        //        }
                        //        #endregion
                        //    }


                        //}

                        else if (Tank_ID == 95 )  // else condition for  filling the curved tank <=10 compartment
                        {
                            x1 = 24006.7765 + 10477;
                            x2 = 24006.7765 + 10477;
                            x3 = 22556.3671 + 10477;
                            x4 = 18005.0824 + 10477;
                            x5 = 18005.0824 + 10477;
                            x6 = 19620.1554 + 10477;
                            x7 = 20337.5244 + 10477;
                            x8 = 21230.993 + 10477;

                            y1 = 1172.1993 + 5958;
                            y2 = 2163.7309 + 5958;
                            y3 = 2163.7309 + 5958;
                            y4 = 3800.1515 + 5958;
                            y5 = 3099.9534 + 5958;
                            y6 = 2274.8719 + 5958;
                            y7 = 1818.9776 + 5958;
                            y8 = 1157.4053 + 5958;

                            #region
                            {
                                System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                                p.Stroke = System.Windows.Media.Brushes.Black;
                                SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                                mySolidColorBrush.Color = color1;
                                p.Fill = mySolidColorBrush;
                                System.Windows.Point[] point = new System.Windows.Point[25];
                                PointCollection pointCollection = new PointCollection();
                                pointCollection.Add(new System.Windows.Point(x1, y1));
                                pointCollection.Add(new System.Windows.Point(x2, y2));
                                pointCollection.Add(new System.Windows.Point(x3, y3));
                                pointCollection.Add(new System.Windows.Point(x4, y4));
                                pointCollection.Add(new System.Windows.Point(x5, y5));
                                pointCollection.Add(new System.Windows.Point(x6, y6));
                                pointCollection.Add(new System.Windows.Point(x7, y7));
                                pointCollection.Add(new System.Windows.Point(x8, y8));
                                p.Points = pointCollection;
                                canvasTwoD.Children.Add(p);
                                //percent = 0;
                            }
                            #endregion

                            #region
                            {
                                if (percent <= 100)
                                {
                                    System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                                    p.Stroke = System.Windows.Media.Brushes.Black;
                                    SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                                    mySolidColorBrush.Color = color1;
                                    p.Fill = mySolidColorBrush;
                                    System.Windows.Point[] point = new System.Windows.Point[25];
                                    PointCollection pointCollection = new PointCollection();
                                    pointCollection.Add(new System.Windows.Point(x1, y1));
                                    pointCollection.Add(new System.Windows.Point(x2, y2));
                                    pointCollection.Add(new System.Windows.Point(x3, y3));
                                    pointCollection.Add(new System.Windows.Point(x4, y4));
                                    pointCollection.Add(new System.Windows.Point(x5, y5));
                                    pointCollection.Add(new System.Windows.Point(x6, y6));
                                    pointCollection.Add(new System.Windows.Point(x7, y7));
                                    pointCollection.Add(new System.Windows.Point(x8, y8));
                                    p.Points = pointCollection;
                                    canvas2DAll.Children.Add(p);
                                    percent = 0;
                                }
                            }
                            #endregion

                        }
                        //else if (Tank_ID == 494)
                        //{

                        //    x1 = 39761.2236 + 10477; x2 = 39759.4048 + 10477; x3 = 22075.4712 + 10477; x4 = 21756.1412 + 10477;
                        //    y1 = 10926.1630 + 5958; y2 = 13821.4827 + 5958; y3 = 13826.9819 + 5958; y4 = 11226.2477 + 5958;

                        //    #region
                        //    {
                        //        System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                        //        p.Stroke = System.Windows.Media.Brushes.Black;
                        //        SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                        //        mySolidColorBrush.Color = System.Windows.Media.Color.FromArgb(0, 255, 255, 255);
                        //        p.Fill = mySolidColorBrush;
                        //        System.Windows.Point[] point = new System.Windows.Point[25];
                        //        PointCollection pointCollection = new PointCollection();
                        //        p.Stroke = System.Windows.Media.Brushes.Black;
                        //        mySolidColorBrush.Color = color1;
                        //        p.Fill = mySolidColorBrush;
                        //        double d = y2 - y1;
                        //        double dx = x3 - x1;
                        //        double Fill = Convert.ToInt32(percent) * (d / 100);
                        //        double Fillx = Convert.ToInt32(percent) * (dx / 100);
                        //        pointCollection.Add(new System.Windows.Point(x1, y1));
                        //        pointCollection.Add(new System.Windows.Point(x1, y1 + Fill));
                        //        pointCollection.Add(new System.Windows.Point(x3, y1 + Fill));
                        //        pointCollection.Add(new System.Windows.Point(x4, y4));
                        //        p.Points = pointCollection;
                        //        canvasTwoD.Children.Add(p);
                        //        //percent = 0;
                        //    }
                        //    #endregion

                        //    #region
                        //    {
                        //        if (percent <= 100)
                        //        {
                        //            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                        //            p.Stroke = System.Windows.Media.Brushes.Black;
                        //            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                        //            mySolidColorBrush.Color = System.Windows.Media.Color.FromArgb(0, 255, 255, 255);
                        //            p.Fill = mySolidColorBrush;
                        //            System.Windows.Point[] point = new System.Windows.Point[25];
                        //            PointCollection pointCollection = new PointCollection();
                        //            p.Stroke = System.Windows.Media.Brushes.Black;
                        //            mySolidColorBrush.Color = color1;
                        //            p.Fill = mySolidColorBrush;
                        //            double d = y2 - y1;
                        //            double dx = x3 - x1;
                        //            double Fill = Convert.ToInt32(percent) * (d / 100);
                        //            double Fillx = Convert.ToInt32(percent) * (dx / 100);
                        //            pointCollection.Add(new System.Windows.Point(x1, y1));
                        //            pointCollection.Add(new System.Windows.Point(x1, y1 + Fill));
                        //            pointCollection.Add(new System.Windows.Point(x3, y1 + Fill));
                        //            pointCollection.Add(new System.Windows.Point(x4, y4));
                        //            p.Points = pointCollection;
                        //            canvas2DAll.Children.Add(p);
                        //            percent = 0;
                        //        }
                        //    }
                        //    #endregion
                        //}
                        //else if (Tank_ID == 497 && percent != 0)
                        //{

                        //    x1 = 12003.3883 + 10477; x2 = 12003.3883 + 10477; x3 = -1950.5506 + 10477; x4 = -1099.4077 + 10477;
                        //    y1 = 8275.4148 + 5958; y2 = 11226.2477 + 5958; y3 = 11226.2477 + 5958; y4 = 8257.9598 + 5958;

                        //    #region
                        //    {
                        //        System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                        //        p.Stroke = System.Windows.Media.Brushes.Black;
                        //        SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                        //        mySolidColorBrush.Color = System.Windows.Media.Color.FromArgb(0, 255, 255, 255);
                        //        p.Fill = mySolidColorBrush;
                        //        System.Windows.Point[] point = new System.Windows.Point[25];
                        //        PointCollection pointCollection = new PointCollection();
                        //        p.Stroke = System.Windows.Media.Brushes.Black;
                        //        mySolidColorBrush.Color = color1;
                        //        p.Fill = mySolidColorBrush;
                        //        double d = y2 - y1;
                        //        double dx = x3 - x1;
                        //        double Fill = Convert.ToInt32(percent) * (d / 100);
                        //        double Fillx = Convert.ToInt32(percent) * (dx / 100);
                        //        pointCollection.Add(new System.Windows.Point(x1, y1));
                        //        pointCollection.Add(new System.Windows.Point(x1, y1 + Fill));
                        //        pointCollection.Add(new System.Windows.Point(x3, y1 + Fill));
                        //        pointCollection.Add(new System.Windows.Point(x4, y4));
                        //        p.Points = pointCollection;
                        //        canvasTwoD.Children.Add(p);
                        //        //percent = 0;
                        //    }
                        //    #endregion

                        //    #region
                        //    {
                        //        if (percent <= 100)
                        //        {
                        //            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                        //            p.Stroke = System.Windows.Media.Brushes.Black;
                        //            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                        //            mySolidColorBrush.Color = System.Windows.Media.Color.FromArgb(0, 255, 255, 255);
                        //            p.Fill = mySolidColorBrush;
                        //            System.Windows.Point[] point = new System.Windows.Point[25];
                        //            PointCollection pointCollection = new PointCollection();
                        //            p.Stroke = System.Windows.Media.Brushes.Black;
                        //            mySolidColorBrush.Color = color1;
                        //            p.Fill = mySolidColorBrush;
                        //            double d = y2 - y1;
                        //            double dx = x3 - x1;
                        //            double Fill = Convert.ToInt32(percent) * (d / 100);
                        //            double Fillx = Convert.ToInt32(percent) * (dx / 100);
                        //            pointCollection.Add(new System.Windows.Point(x1, y1));
                        //            pointCollection.Add(new System.Windows.Point(x1, y1 + Fill));
                        //            pointCollection.Add(new System.Windows.Point(x3, y1 + Fill));
                        //            pointCollection.Add(new System.Windows.Point(x4, y4));
                        //            p.Points = pointCollection;
                        //            canvas2DAll.Children.Add(p);
                        //            percent = 0;
                        //        }
                        //    }
                        //    #endregion
                        //}

                        //else if (Tank_ID == 498 && percent != 0)
                        //{
                        //    x1 = 12003.3883 + 10477; x2 = 12003.3883 + 10477; x3 = -1099.4077 + 10477; x4 = -422.3398 + 10477;
                        //    y1 = 5896.7434 + 5958; y2 = 8275.4148 + 5958; y3 = 8257.9598 + 5958; y4 = 5896.7434 + 5958;

                        //    #region
                        //    {
                        //        System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                        //        p.Stroke = System.Windows.Media.Brushes.Black;
                        //        SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                        //        mySolidColorBrush.Color = System.Windows.Media.Color.FromArgb(0, 255, 255, 255);
                        //        p.Fill = mySolidColorBrush;
                        //        System.Windows.Point[] point = new System.Windows.Point[25];
                        //        PointCollection pointCollection = new PointCollection();
                        //        p.Stroke = System.Windows.Media.Brushes.Black;
                        //        mySolidColorBrush.Color = color1;
                        //        p.Fill = mySolidColorBrush;
                        //        double d = y2 - y1;
                        //        double dx = x3 - x1;
                        //        double Fill = Convert.ToInt32(percent) * (d / 100);
                        //        double Fillx = Convert.ToInt32(percent) * (dx / 100);
                        //        pointCollection.Add(new System.Windows.Point(x1, y1));
                        //        pointCollection.Add(new System.Windows.Point(x1, y1 + Fill));
                        //        pointCollection.Add(new System.Windows.Point(x3, y1 + Fill));
                        //        pointCollection.Add(new System.Windows.Point(x4, y4));
                        //        p.Points = pointCollection;
                        //        canvasTwoD.Children.Add(p);
                        //        //percent = 0;
                        //    }
                        //    #endregion

                        //    #region
                        //    {
                        //        if (percent <= 100)
                        //        {
                        //            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                        //            p.Stroke = System.Windows.Media.Brushes.Black;
                        //            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                        //            mySolidColorBrush.Color = System.Windows.Media.Color.FromArgb(0, 255, 255, 255);
                        //            p.Fill = mySolidColorBrush;
                        //            System.Windows.Point[] point = new System.Windows.Point[25];
                        //            PointCollection pointCollection = new PointCollection();
                        //            p.Stroke = System.Windows.Media.Brushes.Black;
                        //            mySolidColorBrush.Color = color1;
                        //            p.Fill = mySolidColorBrush;
                        //            double d = y2 - y1;
                        //            double dx = x3 - x1;
                        //            double Fill = Convert.ToInt32(percent) * (d / 100);
                        //            double Fillx = Convert.ToInt32(percent) * (dx / 100);
                        //            pointCollection.Add(new System.Windows.Point(x1, y1));
                        //            pointCollection.Add(new System.Windows.Point(x1, y1 + Fill));
                        //            pointCollection.Add(new System.Windows.Point(x3, y1 + Fill));
                        //            pointCollection.Add(new System.Windows.Point(x4, y4));
                        //            p.Points = pointCollection;
                        //            canvas2DAll.Children.Add(p);
                        //            percent = 0;
                        //        }
                        //    }
                        //    #endregion
                        //}

                        else if (percent > 0 && percent <= 101)
                        {
                            # region Profile
                            {
                                System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                                p.Stroke = System.Windows.Media.Brushes.Black;
                                SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                                mySolidColorBrush.Color = color1;
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
                            }
                            #endregion

                            #region All
                            {
                                if (percent > 0 && percent <= 100)
                                {
                                    System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                                    p.Stroke = System.Windows.Media.Brushes.Black;
                                    SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                                    mySolidColorBrush.Color = color1;
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
                                    canvas2DAll.Children.Add(p);
                                }

                            }
                            #endregion
                            // canvas2DAll.Children.Add(p);

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
                            //pointCollection.Add(new System.Windows.Point(xx[1], yy[1]));
                            //pointCollection.Add(new System.Windows.Point(xx[2], yy[2]));
                            //pointCollection.Add(new System.Windows.Point(xx[3], yy[3]));
                            //pointCollection.Add(new System.Windows.Point(xx[4], yy[4]));
                            //p.Points = pointCollection;
                            //canvasTwoD.Children.Add(p);
                        }


                        //mySolidColorBrush.Color = System.Windows.Media.Color.FromArgb(220, 255, 0, 0);

                        percent = 0;
                    }
                    #endregion

                }
                else if (Tank_ID == 10 && percent != 0)
                {
                    x1 = 283401.3595;
                    y1 = 85.2742;
                    x2 = 285226.5459;
                    y2 = 498.5256;
                    x3 = 287154.7309;
                    y3 = 1342.0888;
                    x4 = 288360.3763;
                    y4 = 2151.5312;
                    x5 = 289462.3684;
                    y5 = 3529.0256;
                    x6 = 289944.5096;
                    y6 = 4872.097;
                    if (percent > 0 && percent <= 10)
                    {
                        #region profile
                        {
                            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                            p.Stroke = System.Windows.Media.Brushes.Black;
                            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                            mySolidColorBrush.Color = color;
                            p.Fill = mySolidColorBrush;
                            double d = y5 - y1;
                            double Fill = Convert.ToInt32(percent) * (d / 10);
                            System.Windows.Point[] point = new System.Windows.Point[25];
                            PointCollection pointCollection = new PointCollection();
                            pointCollection.Add(new System.Windows.Point(x6, y6));
                            pointCollection.Add(new System.Windows.Point(x1, y1));
                            pointCollection.Add(new System.Windows.Point(x1, y1 + Fill));
                            pointCollection.Add(new System.Windows.Point(x2, y2));
                            pointCollection.Add(new System.Windows.Point(x5, y5));
                            p.Points = pointCollection;
                            canvasTwoD.Children.Add(p);
                        }
                        # endregion

                        #region All
                        {
                            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                            p.Stroke = System.Windows.Media.Brushes.Black;
                            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                            mySolidColorBrush.Color = color;
                            p.Fill = mySolidColorBrush;
                            double d = y5 - y6;
                            double Fill = Convert.ToInt32(percent) * (d / 10);
                            System.Windows.Point[] point = new System.Windows.Point[25];
                            PointCollection pointCollection = new PointCollection();
                            pointCollection.Add(new System.Windows.Point(x6, y6));
                            pointCollection.Add(new System.Windows.Point(x1, y1));
                            pointCollection.Add(new System.Windows.Point(x1, y1 + Fill));
                            pointCollection.Add(new System.Windows.Point(x2, y2));
                            pointCollection.Add(new System.Windows.Point(x5, y5));
                            p.Points = pointCollection;
                            canvas2DAll.Children.Add(p);
                        }
                        #endregion
                    }
                    else if (percent > 10 && percent <= 101)
                    {
                        #region profile
                        {
                            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                            p.Stroke = System.Windows.Media.Brushes.Black;
                            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                            mySolidColorBrush.Color = color;
                            p.Fill = mySolidColorBrush;
                            //double d = y3 - y2;
                            double Fill = Convert.ToInt32(percent) * (10000 / 100);
                            System.Windows.Point[] point = new System.Windows.Point[25];
                            PointCollection pointCollection = new PointCollection();
                            pointCollection.Add(new System.Windows.Point(x1, y1));
                            pointCollection.Add(new System.Windows.Point(x6, y6));
                            pointCollection.Add(new System.Windows.Point(x5, y5));
                            pointCollection.Add(new System.Windows.Point(x5, y5 + Fill));
                            pointCollection.Add(new System.Windows.Point(x2, y2 + Fill));
                            p.Points = pointCollection;
                            canvasTwoD.Children.Add(p);
                        }
                        #endregion
                        #region All
                        {
                            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                            p.Stroke = System.Windows.Media.Brushes.Black;
                            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                            mySolidColorBrush.Color = color;
                            p.Fill = mySolidColorBrush;
                            double d = y4 - y5;
                            double Fill = Convert.ToInt32(percent) * (d / 100);
                            System.Windows.Point[] point = new System.Windows.Point[25];
                            PointCollection pointCollection = new PointCollection();
                            pointCollection.Add(new System.Windows.Point(x1, y1));
                            pointCollection.Add(new System.Windows.Point(x6, y6));
                            pointCollection.Add(new System.Windows.Point(x5, y5));
                            pointCollection.Add(new System.Windows.Point(x5, y5 + Fill));
                            pointCollection.Add(new System.Windows.Point(x2, y2 + Fill));
                            p.Points = pointCollection;
                            canvas2DAll.Children.Add(p);
                            percent = 0;
                        }
                        #endregion
                    }
                }
                else if (Tank_ID == 6 && percent != 0)
                {

                    x1 = 16504.6588 + 10477;
                    y1 = 3649.1089 + 5958;
                    x2 = 16504.6588 + 10477;
                    y2 = 5896.7434 + 5958;
                    x3 = 12003.3883 + 10477;
                    y3 = 5896.7434 + 5958;
                    x4 = 12003.3883 + 10477;
                    y4 = 4481.3438 + 5958;
                    if (percent <= 30)
                    {
                        #region profile
                        {
                            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                            p.Stroke = System.Windows.Media.Brushes.Black;
                            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                            mySolidColorBrush.Color = color;
                            p.Fill = mySolidColorBrush;
                            double d = y4 - y1;
                            double dx = x4 - x1;
                            double Fill = Convert.ToInt32(percent) * (d / 30);
                            double Fillx = Convert.ToInt32(percent) * (dx / 30);
                            System.Windows.Point[] point = new System.Windows.Point[25];
                            PointCollection pointCollection = new PointCollection();
                            pointCollection.Add(new System.Windows.Point(x1, y1));
                            pointCollection.Add(new System.Windows.Point(x1, y1 + Fill));
                            pointCollection.Add(new System.Windows.Point(x1 + Fillx, y1 + Fill));
                            p.Points = pointCollection;
                            canvasTwoD.Children.Add(p);
                        }
                        # endregion

                        #region All
                        {
                            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                            p.Stroke = System.Windows.Media.Brushes.Black;
                            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                            mySolidColorBrush.Color = color;
                            p.Fill = mySolidColorBrush;
                            double d = y4 - y1;
                            double dx = x4 - x1;
                            double Fill = Convert.ToInt32(percent) * (d / 30);
                            double Fillx = Convert.ToInt32(percent) * (dx / 30);
                            System.Windows.Point[] point = new System.Windows.Point[25];
                            PointCollection pointCollection = new PointCollection();
                            pointCollection.Add(new System.Windows.Point(x1, y1));
                            pointCollection.Add(new System.Windows.Point(x1, y1 + Fill));
                            pointCollection.Add(new System.Windows.Point(x1 + Fillx, y1 + Fill));
                            p.Points = pointCollection;
                            canvas2DAll.Children.Add(p);
                        }
                        #endregion
                    }
                    else if (percent > 30 && percent <= 101)
                    {

                        #region profile
                        {

                            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                            p.Stroke = System.Windows.Media.Brushes.Black;
                            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                            mySolidColorBrush.Color = color;
                            p.Fill = mySolidColorBrush;
                            double d = y2 - y4;
                            double Fill = Convert.ToInt32(percent) * (d / 100);
                            System.Windows.Point[] point = new System.Windows.Point[25];
                            PointCollection pointCollection = new PointCollection();
                            pointCollection.Add(new System.Windows.Point(x1, y1));
                            pointCollection.Add(new System.Windows.Point(x1, y4 + Fill));
                            pointCollection.Add(new System.Windows.Point(x3, y4 + Fill));
                            pointCollection.Add(new System.Windows.Point(x4, y4));
                            p.Points = pointCollection;
                            canvasTwoD.Children.Add(p);
                        }
                        #endregion

                        #region  All
                        {
                            if (percent > 30 && percent <= 100)
                            {
                                System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                                p.Stroke = System.Windows.Media.Brushes.Black;
                                SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                                mySolidColorBrush.Color = color;
                                p.Fill = mySolidColorBrush;
                                double d = y2 - y4;
                                double Fill = Convert.ToInt32(percent) * (d / 100);
                                // double dx = x3 - x2;
                                // double Fillx = Convert.ToInt32(percent) * (dx / 100);
                                System.Windows.Point[] point = new System.Windows.Point[25];
                                PointCollection pointCollection = new PointCollection();
                                pointCollection.Add(new System.Windows.Point(x1, y1));
                                pointCollection.Add(new System.Windows.Point(x1, y4 + Fill));
                                pointCollection.Add(new System.Windows.Point(x3, y4 + Fill));
                                pointCollection.Add(new System.Windows.Point(x4, y4));
                                p.Points = pointCollection;
                                canvas2DAll.Children.Add(p);
                                percent = 0;
                            }
                        }
                        #endregion
                    }
                }

                else if (Tank_ID == 13 && percent != 0)
                {
                    x1 = 21756.1412 + 10477;
                    y1 = 2451.4532 + 5958;
                    x2 = 21756.1412 + 10477;
                    y2 = 5774.7089 + 5958;
                    x3 = 19505.5059 + 10477;
                    y3 = 5774.7089 + 5958;
                    x4 = 18005.0824 + 10477;
                    y4 = 5896.7434 + 5958;
                    x5 = 18005.0824 + 10477;
                    y5 = 3800.1515 + 5958;
                    x6 = 19756.8863 + 10477;
                    y6 = 3170.2881 + 5958;
                    if (percent <= 10)
                    {
                        #region profile
                        {
                            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                            p.Stroke = System.Windows.Media.Brushes.Black;
                            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                            mySolidColorBrush.Color = color;
                            p.Fill = mySolidColorBrush;
                            double d = y6 - y1;
                            double Fill = Convert.ToInt32(percent) * (d / 10);
                            double dx = x6 - x1;
                            double Fillx = Convert.ToInt32(percent) * (dx / 10);
                            System.Windows.Point[] point = new System.Windows.Point[25];
                            PointCollection pointCollection = new PointCollection();
                            pointCollection.Add(new System.Windows.Point(x1, y1));
                            pointCollection.Add(new System.Windows.Point(x1, y1 + Fill));
                            pointCollection.Add(new System.Windows.Point(x1 + Fillx, y1 + Fill));
                            p.Points = pointCollection;
                            canvasTwoD.Children.Add(p);
                            //percent = 0;
                        }
                        #endregion

                        #region All
                        {
                            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                            p.Stroke = System.Windows.Media.Brushes.Black;
                            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                            mySolidColorBrush.Color = color;
                            p.Fill = mySolidColorBrush;
                            double d = y6 - y1;
                            double Fill = Convert.ToInt32(percent) * (d / 10);
                            double dx = x6 - x1;
                            double Fillx = Convert.ToInt32(percent) * (dx / 10);
                            System.Windows.Point[] point = new System.Windows.Point[25];
                            PointCollection pointCollection = new PointCollection();
                            pointCollection.Add(new System.Windows.Point(x1, y1));
                            pointCollection.Add(new System.Windows.Point(x1, y1 + Fill));
                            pointCollection.Add(new System.Windows.Point(x1 + Fillx, y1 + Fill));
                            p.Points = pointCollection;
                            canvas2DAll.Children.Add(p);
                            percent = 0;
                        }
                        #endregion
                    }
                    else if (percent > 10 && percent <= 20)
                    {
                        #region profile
                        {
                            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                            p.Stroke = System.Windows.Media.Brushes.Black;
                            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                            mySolidColorBrush.Color = color;
                            p.Fill = mySolidColorBrush;
                            double d = y5 - y6;
                            double Fill = Convert.ToInt32(percent) * (d / 20);
                            double dx = y5 - y6;
                            double Fillx = Convert.ToInt32(percent) * (dx / 20);
                            System.Windows.Point[] point = new System.Windows.Point[25];
                            PointCollection pointCollection = new PointCollection();
                            pointCollection.Add(new System.Windows.Point(x1, y1));
                            pointCollection.Add(new System.Windows.Point(x1, y6));
                            pointCollection.Add(new System.Windows.Point(x1, y6 + Fill));
                            pointCollection.Add(new System.Windows.Point(x5 + Fillx, y6 + Fill));
                            pointCollection.Add(new System.Windows.Point(x6, y6));
                            p.Points = pointCollection;
                            canvasTwoD.Children.Add(p);
                            // percent = 0;
                        }
                        #endregion

                        #region All
                        {
                            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                            p.Stroke = System.Windows.Media.Brushes.Black;
                            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                            mySolidColorBrush.Color = color;
                            p.Fill = mySolidColorBrush;
                            double d = y5 - y6;
                            double Fill = Convert.ToInt32(percent) * (d / 20);
                            double dx = y5 - y6;
                            double Fillx = Convert.ToInt32(percent) * (dx / 20);
                            System.Windows.Point[] point = new System.Windows.Point[25];
                            PointCollection pointCollection = new PointCollection();
                            pointCollection.Add(new System.Windows.Point(x1, y1));
                            pointCollection.Add(new System.Windows.Point(x1, y6));
                            pointCollection.Add(new System.Windows.Point(x1, y6 + Fill));
                            pointCollection.Add(new System.Windows.Point(x5 + Fillx, y6 + Fill));
                            pointCollection.Add(new System.Windows.Point(x6, y6));
                            p.Points = pointCollection;
                            canvas2DAll.Children.Add(p);
                            // percent = 0;
                        }
                        #endregion

                    }
                    else if (percent > 20)
                    {
                        #region profile
                        {
                            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                            p.Stroke = System.Windows.Media.Brushes.Black;
                            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                            mySolidColorBrush.Color = color;
                            p.Fill = mySolidColorBrush;
                            double d = y2 - y5;
                            double Fill = Convert.ToInt32(percent) * (d / 100);
                            System.Windows.Point[] point = new System.Windows.Point[25];
                            PointCollection pointCollection = new PointCollection();
                            pointCollection.Add(new System.Windows.Point(x1, y1));
                            pointCollection.Add(new System.Windows.Point(x1, y6));
                            pointCollection.Add(new System.Windows.Point(x1, y5));
                            pointCollection.Add(new System.Windows.Point(x1, y5 + Fill));
                            pointCollection.Add(new System.Windows.Point(x4, y5 + Fill));
                            pointCollection.Add(new System.Windows.Point(x5, y5));
                            p.Points = pointCollection;
                            canvasTwoD.Children.Add(p);
                            //percent = 0;
                        }
                        #endregion

                        #region All
                        {
                            if (percent > 20 && percent <= 100)
                            {
                                System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                                p.Stroke = System.Windows.Media.Brushes.Black;
                                SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                                mySolidColorBrush.Color = color;
                                p.Fill = mySolidColorBrush;
                                double d = y2 - y5;
                                double Fill = Convert.ToInt32(percent) * (d / 100);
                                System.Windows.Point[] point = new System.Windows.Point[25];
                                PointCollection pointCollection = new PointCollection();
                                pointCollection.Add(new System.Windows.Point(x1, y1));
                                pointCollection.Add(new System.Windows.Point(x1, y6));
                                pointCollection.Add(new System.Windows.Point(x1, y5));
                                pointCollection.Add(new System.Windows.Point(x1, y5 + Fill));
                                pointCollection.Add(new System.Windows.Point(x4, y5 + Fill));
                                pointCollection.Add(new System.Windows.Point(x5, y5));
                                p.Points = pointCollection;
                                canvas2DAll.Children.Add(p);
                                percent = 0;
                            }
                        }
                        #endregion

                    }
                }
                else if (Tank_ID == 12 && percent != 0)
                {
                    x1 = 148538.9649;
                    y1 = 7075.7311;
                    x2 = 148538.9649;
                    y2 = 7659.9501;
                    x3 = 148538.9649;
                    y3 = 11774.7089;
                    x4 = 144037.6943;
                    y4 = 11774.7089;
                    x5 = 144037.6943;
                    y5 = 7602.5992;
                    x6 = 146836.4823;
                    y6 = 7576.523;
                    if (percent > 0 && percent <= 05)
                    {
                        #region profile
                        {
                            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                            p.Stroke = System.Windows.Media.Brushes.Black;
                            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                            mySolidColorBrush.Color = color;
                            p.Fill = mySolidColorBrush;
                            double d = y6 - y1;
                            double Fill = Convert.ToInt32(percent) * (d / 05);
                            System.Windows.Point[] point = new System.Windows.Point[25];
                            PointCollection pointCollection = new PointCollection();
                            pointCollection.Add(new System.Windows.Point(x1, y1));
                            pointCollection.Add(new System.Windows.Point(x1, y1 + Fill));
                            pointCollection.Add(new System.Windows.Point(x2, y2));
                            pointCollection.Add(new System.Windows.Point(x6, y6));
                            p.Points = pointCollection;
                            canvasTwoD.Children.Add(p);
                        }
                        # endregion

                        #region All
                        {
                            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                            p.Stroke = System.Windows.Media.Brushes.Black;
                            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                            mySolidColorBrush.Color = color;
                            p.Fill = mySolidColorBrush;
                            double d = y6 - y1;
                            double Fill = Convert.ToInt32(percent) * (d / 05);
                            System.Windows.Point[] point = new System.Windows.Point[25];
                            PointCollection pointCollection = new PointCollection();
                            pointCollection.Add(new System.Windows.Point(x1, y1));
                            pointCollection.Add(new System.Windows.Point(x1, y1 + Fill));
                            pointCollection.Add(new System.Windows.Point(x2, y2));
                            pointCollection.Add(new System.Windows.Point(x6, y6));
                            p.Points = pointCollection;
                            canvas2DAll.Children.Add(p);
                        }
                        #endregion
                    }
                    else if (percent > 05 && percent <= 101)
                    {
                        #region profile
                        {
                            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                            p.Stroke = System.Windows.Media.Brushes.Black;
                            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                            mySolidColorBrush.Color = color;
                            p.Fill = mySolidColorBrush;
                            double d = y4 - y6;
                            double Fill = Convert.ToInt32(percent) * (d / 100);
                            System.Windows.Point[] point = new System.Windows.Point[25];
                            PointCollection pointCollection = new PointCollection();
                            pointCollection.Add(new System.Windows.Point(x1, y1));
                            //  pointCollection.Add(new System.Windows.Point(x2, y2));
                            pointCollection.Add(new System.Windows.Point(x6, y6));
                            pointCollection.Add(new System.Windows.Point(x5, y5));
                            pointCollection.Add(new System.Windows.Point(x5, y5 + Fill));
                            pointCollection.Add(new System.Windows.Point(x2, y2 + Fill));

                            p.Points = pointCollection;
                            canvasTwoD.Children.Add(p);
                        }
                        #endregion
                        #region All
                        {

                            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                            p.Stroke = System.Windows.Media.Brushes.Black;
                            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                            mySolidColorBrush.Color = color;
                            p.Fill = mySolidColorBrush;
                            double d = y2 - y5;
                            double Fill = Convert.ToInt32(percent) * (d / 100);
                            System.Windows.Point[] point = new System.Windows.Point[25];
                            PointCollection pointCollection = new PointCollection();
                            pointCollection.Add(new System.Windows.Point(x1, y1));
                            //  pointCollection.Add(new System.Windows.Point(x2, y2));
                            pointCollection.Add(new System.Windows.Point(x6, y6));
                            pointCollection.Add(new System.Windows.Point(x5, y5));
                            pointCollection.Add(new System.Windows.Point(x5, y5 + Fill));
                            pointCollection.Add(new System.Windows.Point(x2, y2 + Fill));

                            p.Points = pointCollection;
                            canvas2DAll.Children.Add(p);
                            percent = 0;
                        }
                        #endregion

                    }


                }
                else if (Tank_ID == 55 && percent != 0)
                {
                    x1 = 138038.9649 + 10477; y1 = 1075.7311 + 5958;
                    x2 = 139912.5040 + 10477; y2 = 538.2302 + 5958;
                    x3 = 142349.6788 + 10477; y3 = -49.9359 + 5958;
                    x4 = 145441.1579 + 10477; y4 = -87.9468 + 5958;
                    x5 = 145930.1939 + 10477; y5 = -34.9319 + 5958;
                    x6 = 146646.8940 + 10477; y6 = 427.6990 + 5958;
                    x7 = 147220.3972 + 10477; y7 = 1075.7311 + 5958;
                    x8 = 148191.8308 + 10477; y8 = 2221.7056 + 5958;
                    x9 = 149040.7049 + 10477; y9 = 3251.0264 + 5958;
                    x10 = 138038.9649 + 10477; y10 = 3251.0264 + 5958;

                    if (percent <= 12)
                    {
                        #region
                        {
                            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                            p.Stroke = System.Windows.Media.Brushes.Black;
                            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                            mySolidColorBrush.Color = color;
                            p.Fill = mySolidColorBrush;
                            double d = y3 - y2;
                            double dx = x6 - x2;
                            double Fill = Convert.ToInt32(percent) * (d / 100);
                            double Fillx = Convert.ToInt32(percent) * (dx / 100);

                            System.Windows.Point[] point = new System.Windows.Point[25];
                            PointCollection pointCollection = new PointCollection();
                            pointCollection.Add(new System.Windows.Point(x3, y3));
                            pointCollection.Add(new System.Windows.Point(x5, y5));
                            pointCollection.Add(new System.Windows.Point(x6, y6));
                            pointCollection.Add(new System.Windows.Point(x2, y2));
                            p.Points = pointCollection;
                            canvasTwoD.Children.Add(p);
                            //percent = 0;
                        }
                        #endregion

                        #region
                        {
                            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                            p.Stroke = System.Windows.Media.Brushes.Black;
                            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                            mySolidColorBrush.Color = color;
                            p.Fill = mySolidColorBrush;
                            double d = y3 - y2;
                            double dx = x6 - x2;
                            double Fill = Convert.ToInt32(percent) * (d / 100);
                            double Fillx = Convert.ToInt32(percent) * (dx / 100);

                            System.Windows.Point[] point = new System.Windows.Point[25];
                            PointCollection pointCollection = new PointCollection();
                            pointCollection.Add(new System.Windows.Point(x3, y3));
                            pointCollection.Add(new System.Windows.Point(x5, y5));
                            pointCollection.Add(new System.Windows.Point(x6, y6));
                            pointCollection.Add(new System.Windows.Point(x2, y2));
                            p.Points = pointCollection;
                            canvas2DAll.Children.Add(p);
                            percent = 0;
                        }
                        #endregion
                    }
                    else if (percent <= 22)
                    {
                        #region
                        {
                            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                            p.Stroke = System.Windows.Media.Brushes.Black;
                            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                            mySolidColorBrush.Color = color;
                            p.Fill = mySolidColorBrush;
                            double d = y3 - y2;
                            double dx = x6 - x2;
                            double Fill = Convert.ToInt32(percent) * (d / 100);
                            double Fillx = Convert.ToInt32(percent) * (dx / 100);

                            System.Windows.Point[] point = new System.Windows.Point[25];
                            PointCollection pointCollection = new PointCollection();
                            pointCollection.Add(new System.Windows.Point(x2, y2));
                            pointCollection.Add(new System.Windows.Point(x3, y3));
                            pointCollection.Add(new System.Windows.Point(x5, y5));
                            pointCollection.Add(new System.Windows.Point(x6, y6));
                            pointCollection.Add(new System.Windows.Point(x7, y7));
                            pointCollection.Add(new System.Windows.Point(x1, y1));
                            //pointCollection.Add(new System.Windows.Point(x5+Fillx, y5 + Fill));
                            //pointCollection.Add(new System.Windows.Point(x3+Fillx, y3 + Fill));
                            p.Points = pointCollection;
                            canvasTwoD.Children.Add(p);
                            //percent = 0;
                        }
                        # endregion


                        #region
                        {
                            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                            p.Stroke = System.Windows.Media.Brushes.Black;
                            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                            mySolidColorBrush.Color = color;
                            p.Fill = mySolidColorBrush;
                            double d = y3 - y2;
                            double dx = x6 - x2;
                            double Fill = Convert.ToInt32(percent) * (d / 100);
                            double Fillx = Convert.ToInt32(percent) * (dx / 100);

                            System.Windows.Point[] point = new System.Windows.Point[25];
                            PointCollection pointCollection = new PointCollection();
                            pointCollection.Add(new System.Windows.Point(x2, y2));
                            pointCollection.Add(new System.Windows.Point(x3, y3));
                            pointCollection.Add(new System.Windows.Point(x5, y5));
                            pointCollection.Add(new System.Windows.Point(x6, y6));
                            pointCollection.Add(new System.Windows.Point(x7, y7));
                            pointCollection.Add(new System.Windows.Point(x1, y1));
                            //pointCollection.Add(new System.Windows.Point(x5+Fillx, y5 + Fill));
                            //pointCollection.Add(new System.Windows.Point(x3+Fillx, y3 + Fill));
                            p.Points = pointCollection;
                            canvas2DAll.Children.Add(p);
                            percent = 0;
                        }
                        # endregion
                    }
                    else if (percent > 25)
                    {
                        #region
                        {
                            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                            p.Stroke = System.Windows.Media.Brushes.Black;
                            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                            mySolidColorBrush.Color = color;
                            p.Fill = mySolidColorBrush;
                            double d = y10 - y1;
                            double dx = x9 - x7;
                            double Fill = Convert.ToInt32(percent) * (d / 100);
                            double Fillx = Convert.ToInt32(percent) * (dx / 100);
                            System.Windows.Point[] point = new System.Windows.Point[25];
                            PointCollection pointCollection = new PointCollection();
                            pointCollection.Add(new System.Windows.Point(x1, y1));
                            pointCollection.Add(new System.Windows.Point(x2, y2));
                            pointCollection.Add(new System.Windows.Point(x3, y3));
                            pointCollection.Add(new System.Windows.Point(x5, y5));
                            pointCollection.Add(new System.Windows.Point(x6, y6));
                            pointCollection.Add(new System.Windows.Point(x7, y7));
                            pointCollection.Add(new System.Windows.Point(x1, y1));
                            pointCollection.Add(new System.Windows.Point(x7, y7));
                            pointCollection.Add(new System.Windows.Point(x7 + Fillx, y7 + Fill));
                            pointCollection.Add(new System.Windows.Point(x1, y7 + Fill));
                            p.Points = pointCollection;
                            canvasTwoD.Children.Add(p);
                            //percent = 0;
                        }
                        #endregion

                        #region
                        {
                            if (percent > 25 && percent <= 100)
                            {
                                System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                                p.Stroke = System.Windows.Media.Brushes.Black;
                                SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                                mySolidColorBrush.Color = color;
                                p.Fill = mySolidColorBrush;
                                double d = y10 - y1;
                                double dx = x9 - x7;
                                double Fill = Convert.ToInt32(percent) * (d / 100);
                                double Fillx = Convert.ToInt32(percent) * (dx / 100);
                                System.Windows.Point[] point = new System.Windows.Point[25];
                                PointCollection pointCollection = new PointCollection();
                                pointCollection.Add(new System.Windows.Point(x1, y1));
                                pointCollection.Add(new System.Windows.Point(x2, y2));
                                pointCollection.Add(new System.Windows.Point(x3, y3));
                                pointCollection.Add(new System.Windows.Point(x5, y5));
                                pointCollection.Add(new System.Windows.Point(x6, y6));
                                pointCollection.Add(new System.Windows.Point(x7, y7));
                                pointCollection.Add(new System.Windows.Point(x1, y1));
                                pointCollection.Add(new System.Windows.Point(x7, y7));
                                pointCollection.Add(new System.Windows.Point(x7 + Fillx, y7 + Fill));
                                pointCollection.Add(new System.Windows.Point(x1, y7 + Fill));
                                p.Points = pointCollection;
                                canvas2DAll.Children.Add(p);
                                percent = 0;
                            }
                        }
                        #endregion
                    }
                }
                //else if (Tank_ID == 65 && percent != 0)
                //{
                //    x1 = 9002.5412 + 10477; y1 = 4811.4365 + 5958;
                //    x2 = 9002.5412 + 10477; y2 = 5896.7434 + 5958;
                //    x3 = -422.3398 + 10477; y3 = 5896.7434 + 5958;
                //    x4 = -248.2649 + 10477; y4 = 5289.6719 + 5958;
                //    x5 = 3375.9529 + 10477; y5 = 5189.0425 + 5958;
                //    x6 = 6301.7788 + 10477; y6 = 5035.1904 + 5958;

                //    if (percent <= 5)
                //    {
                //        System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                //        p.Stroke = System.Windows.Media.Brushes.Black;
                //        SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                //        mySolidColorBrush.Color = color;
                //        p.Fill = mySolidColorBrush;
                //        double d = y6 - y1;
                //        double dx = x6 - x1;
                //        double Fill = Convert.ToInt32(percent) * (d / 05);
                //        double Fillx = Convert.ToInt32(percent) * (dx / 05);

                //        System.Windows.Point[] point = new System.Windows.Point[25];
                //        PointCollection pointCollection = new PointCollection();
                //        pointCollection.Add(new System.Windows.Point(x1, y1));
                //        pointCollection.Add(new System.Windows.Point(x1, y1 + Fill));
                //        pointCollection.Add(new System.Windows.Point(x1 + Fillx, y1 + Fill));
                //        p.Points = pointCollection;
                //        canvasTwoD.Children.Add(p);
                //        percent = 0;
                //    }
                //    else if (percent > 5 && percent <= 15)
                //    {
                //        #region
                //        {
                //            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                //            p.Stroke = System.Windows.Media.Brushes.Black;
                //            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                //            mySolidColorBrush.Color = color;
                //            p.Fill = mySolidColorBrush;
                //            double d = y5 - y6;
                //            double dx = x5 - x6;
                //            double Fill = Convert.ToInt32(percent) * (d / 15);
                //            double Fillx = Convert.ToInt32(percent) * (dx / 15);

                //            System.Windows.Point[] point = new System.Windows.Point[25];
                //            PointCollection pointCollection = new PointCollection();
                //            pointCollection.Add(new System.Windows.Point(x1, y1));
                //            pointCollection.Add(new System.Windows.Point(x1, y6));
                //            pointCollection.Add(new System.Windows.Point(x1, y6 + Fill));
                //            pointCollection.Add(new System.Windows.Point(x6 + Fillx, y6 + Fill));
                //            p.Points = pointCollection;
                //            canvasTwoD.Children.Add(p);
                //            //percent = 0;
                //        }
                //        #endregion

                //        #region
                //        {
                //            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                //            p.Stroke = System.Windows.Media.Brushes.Black;
                //            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                //            mySolidColorBrush.Color = color;
                //            p.Fill = mySolidColorBrush;
                //            double d = y5 - y6;
                //            double dx = x5 - x6;
                //            double Fill = Convert.ToInt32(percent) * (d / 15);
                //            double Fillx = Convert.ToInt32(percent) * (dx / 15);

                //            System.Windows.Point[] point = new System.Windows.Point[25];
                //            PointCollection pointCollection = new PointCollection();
                //            pointCollection.Add(new System.Windows.Point(x1, y1));
                //            pointCollection.Add(new System.Windows.Point(x1, y6));
                //            pointCollection.Add(new System.Windows.Point(x1, y6 + Fill));
                //            pointCollection.Add(new System.Windows.Point(x6 + Fillx, y6 + Fill));
                //            p.Points = pointCollection;
                //            canvas2DAll.Children.Add(p);
                //            percent = 0;
                //        }
                //        #endregion
                //    }
                //    else if (percent > 15 && percent <= 25)
                //    {
                //        #region
                //        {
                //            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                //            p.Stroke = System.Windows.Media.Brushes.Black;
                //            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                //            mySolidColorBrush.Color = color;
                //            p.Fill = mySolidColorBrush;
                //            double d = y4 - y5;
                //            double dx = x4 - x5;
                //            double Fill = Convert.ToInt32(percent) * (d / 25);
                //            double Fillx = Convert.ToInt32(percent) * (dx / 25);

                //            System.Windows.Point[] point = new System.Windows.Point[25];
                //            PointCollection pointCollection = new PointCollection();
                //            pointCollection.Add(new System.Windows.Point(x1, y1));
                //            pointCollection.Add(new System.Windows.Point(x1, y6));
                //            pointCollection.Add(new System.Windows.Point(x1, y5));
                //            pointCollection.Add(new System.Windows.Point(x1, y5 + Fill));
                //            pointCollection.Add(new System.Windows.Point(x5 + Fillx, y5 + Fill));
                //            p.Points = pointCollection;
                //            canvasTwoD.Children.Add(p);
                //            //percent = 0;
                //        }
                //        #endregion

                //        #region
                //        {
                //            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                //            p.Stroke = System.Windows.Media.Brushes.Black;
                //            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                //            mySolidColorBrush.Color = color;
                //            p.Fill = mySolidColorBrush;
                //            double d = y4 - y5;
                //            double dx = x4 - x5;
                //            double Fill = Convert.ToInt32(percent) * (d / 25);
                //            double Fillx = Convert.ToInt32(percent) * (dx / 25);

                //            System.Windows.Point[] point = new System.Windows.Point[25];
                //            PointCollection pointCollection = new PointCollection();
                //            pointCollection.Add(new System.Windows.Point(x1, y1));
                //            pointCollection.Add(new System.Windows.Point(x1, y6));
                //            pointCollection.Add(new System.Windows.Point(x1, y5));
                //            pointCollection.Add(new System.Windows.Point(x1, y5 + Fill));
                //            pointCollection.Add(new System.Windows.Point(x5 + Fillx, y5 + Fill));
                //            p.Points = pointCollection;
                //            canvas2DAll.Children.Add(p);
                //            percent = 0;
                //        }
                //        #endregion
                //    }
                //    else if (percent > 25)
                //    {
                //        #region
                //        {
                //            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                //            p.Stroke = System.Windows.Media.Brushes.Black;
                //            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                //            mySolidColorBrush.Color = color;
                //            p.Fill = mySolidColorBrush;
                //            double d = y2 - y4;
                //            double dx = x3 - x2;
                //            double Fill = Convert.ToInt32(percent) * (d / 100);
                //            double Fillx = Convert.ToInt32(percent) * (dx / 100);

                //            System.Windows.Point[] point = new System.Windows.Point[25];
                //            PointCollection pointCollection = new PointCollection();
                //            pointCollection.Add(new System.Windows.Point(x1, y1));
                //            pointCollection.Add(new System.Windows.Point(x1, y6));
                //            pointCollection.Add(new System.Windows.Point(x1, y5));
                //            pointCollection.Add(new System.Windows.Point(x1, y4));
                //            pointCollection.Add(new System.Windows.Point(x1, y4 + Fill));
                //            pointCollection.Add(new System.Windows.Point(x3, y4 + Fill));
                //            pointCollection.Add(new System.Windows.Point(x4, y4));
                //            p.Points = pointCollection;
                //            canvasTwoD.Children.Add(p);
                //            //percent = 0;
                //        }
                //        #endregion

                //        #region
                //        {
                //            if (percent <= 100)
                //            {
                //                System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                //                p.Stroke = System.Windows.Media.Brushes.Black;
                //                SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                //                mySolidColorBrush.Color = color;
                //                p.Fill = mySolidColorBrush;
                //                double d = y2 - y4;
                //                double dx = x3 - x2;
                //                double Fill = Convert.ToInt32(percent) * (d / 100);
                //                double Fillx = Convert.ToInt32(percent) * (dx / 100);

                //                System.Windows.Point[] point = new System.Windows.Point[25];
                //                PointCollection pointCollection = new PointCollection();
                //                pointCollection.Add(new System.Windows.Point(x1, y1));
                //                pointCollection.Add(new System.Windows.Point(x1, y6));
                //                pointCollection.Add(new System.Windows.Point(x1, y5));
                //                pointCollection.Add(new System.Windows.Point(x1, y4));
                //                pointCollection.Add(new System.Windows.Point(x1, y4 + Fill));
                //                pointCollection.Add(new System.Windows.Point(x3, y4 + Fill));
                //                pointCollection.Add(new System.Windows.Point(x4, y4));
                //                p.Points = pointCollection;
                //                canvas2DAll.Children.Add(p);
                //                percent = 0;
                //            }
                //        }
                //        #endregion
                //    }

                //}
                //else if (Tank_ID == 66 && percent != 0)
                //{
                //    x1 = 149040.7049 + 10477; y1 = 3251.0264 + 5958;
                //    x2 = 149960.8238 + 10477; y2 = 4374.3121 + 5958;
                //    x3 = 151124.1056 + 10477; y3 = 5774.7089 + 5958;
                //    x4 = 141039.8120 + 10477; y4 = 5774.7089 + 5958;
                //    x5 = 141039.8120 + 10477; y5 = 3219.0630 + 5958;

                //    if (percent <= 50)
                //    {
                //        #region
                //        {
                //            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                //            p.Stroke = System.Windows.Media.Brushes.Black;
                //            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                //            mySolidColorBrush.Color = color;
                //            p.Fill = mySolidColorBrush;
                //            double d = y2 - y1;
                //            double dx = x2 - x1;
                //            double Fill = Convert.ToInt32(percent) * (d / 50);
                //            double Fillx = Convert.ToInt32(percent) * (dx / 50);

                //            System.Windows.Point[] point = new System.Windows.Point[25];
                //            PointCollection pointCollection = new PointCollection();
                //            pointCollection.Add(new System.Windows.Point(x5, y5));
                //            pointCollection.Add(new System.Windows.Point(x1, y1));
                //            pointCollection.Add(new System.Windows.Point(x1 + Fillx, y1 + Fill));
                //            pointCollection.Add(new System.Windows.Point(x5, y1 + Fill));
                //            p.Points = pointCollection;
                //            canvasTwoD.Children.Add(p);
                //            //percent = 0;
                //        }
                //        #endregion

                //        #region
                //        {
                //            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                //            p.Stroke = System.Windows.Media.Brushes.Black;
                //            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                //            mySolidColorBrush.Color = color;
                //            p.Fill = mySolidColorBrush;
                //            double d = y2 - y1;
                //            double dx = x2 - x1;
                //            double Fill = Convert.ToInt32(percent) * (d / 50);
                //            double Fillx = Convert.ToInt32(percent) * (dx / 50);

                //            System.Windows.Point[] point = new System.Windows.Point[25];
                //            PointCollection pointCollection = new PointCollection();
                //            pointCollection.Add(new System.Windows.Point(x5, y5));
                //            pointCollection.Add(new System.Windows.Point(x1, y1));
                //            pointCollection.Add(new System.Windows.Point(x1 + Fillx, y1 + Fill));
                //            pointCollection.Add(new System.Windows.Point(x5, y1 + Fill));
                //            p.Points = pointCollection;
                //            canvas2DAll.Children.Add(p);
                //            percent = 0;
                //        }
                //        #endregion
                //    }
                //    else if (percent > 50)
                //    {
                //        #region
                //        {
                //            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                //            p.Stroke = System.Windows.Media.Brushes.Black;
                //            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                //            mySolidColorBrush.Color = color;
                //            p.Fill = mySolidColorBrush;
                //            double d = y2 - y1;
                //            double dx = x3 - x2;
                //            double Fill = Convert.ToInt32(percent) * (d / 90);
                //            double Fillx = Convert.ToInt32(percent) * (dx / 90);

                //            System.Windows.Point[] point = new System.Windows.Point[25];
                //            PointCollection pointCollection = new PointCollection();
                //            pointCollection.Add(new System.Windows.Point(x5, y5));
                //            pointCollection.Add(new System.Windows.Point(x1, y1));
                //            pointCollection.Add(new System.Windows.Point(x2, y2));
                //            pointCollection.Add(new System.Windows.Point(x5, y5));
                //            pointCollection.Add(new System.Windows.Point(x2, y2));
                //            pointCollection.Add(new System.Windows.Point(x2 + Fillx, y2 + Fill));
                //            pointCollection.Add(new System.Windows.Point(x4, y2 + Fill));
                //            p.Points = pointCollection;
                //            canvasTwoD.Children.Add(p);
                //            //percent = 0;
                //        }
                //        #endregion

                //        #region
                //        {
                //            if (percent <= 100)
                //            {
                //                System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                //                p.Stroke = System.Windows.Media.Brushes.Black;
                //                SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                //                mySolidColorBrush.Color = color;
                //                p.Fill = mySolidColorBrush;
                //                double d = y2 - y1;
                //                double dx = x3 - x2;
                //                double Fill = Convert.ToInt32(percent) * (d / 90);
                //                double Fillx = Convert.ToInt32(percent) * (dx / 90);

                //                System.Windows.Point[] point = new System.Windows.Point[25];
                //                PointCollection pointCollection = new PointCollection();
                //                pointCollection.Add(new System.Windows.Point(x5, y5));
                //                pointCollection.Add(new System.Windows.Point(x1, y1));
                //                pointCollection.Add(new System.Windows.Point(x2, y2));
                //                pointCollection.Add(new System.Windows.Point(x5, y5));
                //                pointCollection.Add(new System.Windows.Point(x2, y2));
                //                pointCollection.Add(new System.Windows.Point(x2 + Fillx, y2 + Fill));
                //                pointCollection.Add(new System.Windows.Point(x4, y2 + Fill));
                //                p.Points = pointCollection;
                //                canvas2DAll.Children.Add(p);
                //                percent = 0;
                //            }
                //        }
                //        #endregion
                //    }
                //}

                //else if (Tank_ID == 301 && percent != 0)
                //{
                //    x1 = 151119.0538 + 10477; y1 = 5774.7089 + 5958;
                //    x2 = 152626.5848 + 10477; y2 = 7500.6959 + 5958;
                //    x3 = 154183.5245 + 10477; y3 = 9126.6546 + 5958;
                //    x4 = 156101.5399 + 10477; y4 = 10926.163 + 5958;
                //    x5 = 145541.0825 + 10477; y5 = 10926.163 + 5958;
                //    x6 = 145541.0825 + 10477; y6 = 5774.7089 + 5958;

                //    if (percent <= 35)
                //    {
                //        #region
                //        {
                //            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                //            p.Stroke = System.Windows.Media.Brushes.Black;
                //            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                //            mySolidColorBrush.Color = color;
                //            p.Fill = mySolidColorBrush;
                //            double d = y2 - y1;
                //            double dx = x2 - x1;
                //            double Fill = Convert.ToInt32(percent) * (d / 35);
                //            double Fillx = Convert.ToInt32(percent) * (dx / 35);

                //            System.Windows.Point[] point = new System.Windows.Point[25];
                //            PointCollection pointCollection = new PointCollection();
                //            pointCollection.Add(new System.Windows.Point(x1, y1));
                //            pointCollection.Add(new System.Windows.Point(x1 + Fillx, y1 + Fill));
                //            pointCollection.Add(new System.Windows.Point(x6, y1 + Fill));
                //            pointCollection.Add(new System.Windows.Point(x6, y1));
                //            p.Points = pointCollection;
                //            canvasTwoD.Children.Add(p);
                //            //percent = 0;
                //        }
                //        #endregion

                //        #region
                //        {
                //            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                //            p.Stroke = System.Windows.Media.Brushes.Black;
                //            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                //            mySolidColorBrush.Color = color;
                //            p.Fill = mySolidColorBrush;
                //            double d = y2 - y1;
                //            double dx = x2 - x1;
                //            double Fill = Convert.ToInt32(percent) * (d / 35);
                //            double Fillx = Convert.ToInt32(percent) * (dx / 35);

                //            System.Windows.Point[] point = new System.Windows.Point[25];
                //            PointCollection pointCollection = new PointCollection();
                //            pointCollection.Add(new System.Windows.Point(x1, y1));
                //            pointCollection.Add(new System.Windows.Point(x1 + Fillx, y1 + Fill));
                //            pointCollection.Add(new System.Windows.Point(x6, y1 + Fill));
                //            pointCollection.Add(new System.Windows.Point(x6, y1));
                //            p.Points = pointCollection;
                //            canvas2DAll.Children.Add(p);
                //            percent = 0;
                //        }
                //        #endregion
                //    }
                //    else if (percent > 35 && percent <= 70)
                //    {
                //        #region
                //        {
                //            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                //            p.Stroke = System.Windows.Media.Brushes.Black;
                //            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                //            mySolidColorBrush.Color = color;
                //            p.Fill = mySolidColorBrush;
                //            double d = y3 - y2;
                //            double dx = x3 - x2;
                //            double Fill = Convert.ToInt32(percent) * (d / 70);
                //            double Fillx = Convert.ToInt32(percent) * (dx / 70);

                //            System.Windows.Point[] point = new System.Windows.Point[25];
                //            PointCollection pointCollection = new PointCollection();
                //            pointCollection.Add(new System.Windows.Point(x1, y1));
                //            pointCollection.Add(new System.Windows.Point(x2, y2));
                //            pointCollection.Add(new System.Windows.Point(x2 + Fillx, y2 + Fill));
                //            pointCollection.Add(new System.Windows.Point(x5, y2 + Fill));
                //            pointCollection.Add(new System.Windows.Point(x5, y2));
                //            pointCollection.Add(new System.Windows.Point(x5, y1));
                //            p.Points = pointCollection;
                //            canvasTwoD.Children.Add(p);
                //            //percent = 0;
                //        }
                //        #endregion

                //        #region
                //        {
                //            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                //            p.Stroke = System.Windows.Media.Brushes.Black;
                //            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                //            mySolidColorBrush.Color = color;
                //            p.Fill = mySolidColorBrush;
                //            double d = y3 - y2;
                //            double dx = x3 - x2;
                //            double Fill = Convert.ToInt32(percent) * (d / 70);
                //            double Fillx = Convert.ToInt32(percent) * (dx / 70);

                //            System.Windows.Point[] point = new System.Windows.Point[25];
                //            PointCollection pointCollection = new PointCollection();
                //            pointCollection.Add(new System.Windows.Point(x1, y1));
                //            pointCollection.Add(new System.Windows.Point(x2, y2));
                //            pointCollection.Add(new System.Windows.Point(x2 + Fillx, y2 + Fill));
                //            pointCollection.Add(new System.Windows.Point(x5, y2 + Fill));
                //            pointCollection.Add(new System.Windows.Point(x5, y2));
                //            pointCollection.Add(new System.Windows.Point(x5, y1));
                //            p.Points = pointCollection;
                //            canvas2DAll.Children.Add(p);
                //            percent = 0;
                //        }
                //        #endregion
                //    }
                //    else if (percent > 60)
                //    {
                //        #region
                //        {
                //            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                //            p.Stroke = System.Windows.Media.Brushes.Black;
                //            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                //            mySolidColorBrush.Color = color;
                //            p.Fill = mySolidColorBrush;
                //            double d = y3 - y2;
                //            double dx = x4 - x3;
                //            double Fill = Convert.ToInt32(percent) * (d / 100);
                //            double Fillx = Convert.ToInt32(percent) * (dx / 100);

                //            System.Windows.Point[] point = new System.Windows.Point[25];
                //            PointCollection pointCollection = new PointCollection();
                //            pointCollection.Add(new System.Windows.Point(x1, y1));
                //            pointCollection.Add(new System.Windows.Point(x2, y2));
                //            pointCollection.Add(new System.Windows.Point(x3, y3));
                //            pointCollection.Add(new System.Windows.Point(x3 + Fill, y3 + Fill));
                //            pointCollection.Add(new System.Windows.Point(x5, y3 + Fill));
                //            pointCollection.Add(new System.Windows.Point(x5, y1));
                //            p.Points = pointCollection;
                //            canvasTwoD.Children.Add(p);
                //            //percent = 0;
                //        }
                //        #endregion

                //        #region
                //        {
                //            if (percent <= 100)
                //            {
                //                System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                //                p.Stroke = System.Windows.Media.Brushes.Black;
                //                SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                //                mySolidColorBrush.Color = color;
                //                p.Fill = mySolidColorBrush;
                //                double d = y3 - y2;
                //                double dx = x4 - x3;
                //                double Fill = Convert.ToInt32(percent) * (d / 100);
                //                double Fillx = Convert.ToInt32(percent) * (dx / 100);

                //                System.Windows.Point[] point = new System.Windows.Point[25];
                //                PointCollection pointCollection = new PointCollection();
                //                pointCollection.Add(new System.Windows.Point(x1, y1));
                //                pointCollection.Add(new System.Windows.Point(x2, y2));
                //                pointCollection.Add(new System.Windows.Point(x3, y3));
                //                pointCollection.Add(new System.Windows.Point(x3 + Fill, y3 + Fill));
                //                pointCollection.Add(new System.Windows.Point(x5, y3 + Fill));
                //                pointCollection.Add(new System.Windows.Point(x5, y1));
                //                p.Points = pointCollection;
                //                canvas2DAll.Children.Add(p);
                //                percent = 0;
                //            }
                //        }
                //        #endregion
                //    }


                //}
                //else if (Tank_ID == 434 && percent != 0)
                //{
                //    x1 = 156101.5399 + 10477; y1 = 10926.163 + 5958;
                //    x2 = 157544.4708 + 10477; y2 = 12171.5143 + 5958;
                //    x3 = 159330.4791 + 10477; y3 = 13645.9303 + 5958;
                //    x4 = 160806.3667 + 10477; y4 = 14822.1868 + 5958;
                //    x5 = 150042.3531 + 10477; y5 = 14405.025 + 5958;
                //    x6 = 150042.3531 + 10477; y6 = 10926.163 + 5958;
                //    if (percent <= 40)
                //    {
                //        #region
                //        {
                //            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                //            p.Stroke = System.Windows.Media.Brushes.Black;
                //            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                //            mySolidColorBrush.Color = color;
                //            p.Fill = mySolidColorBrush;
                //            double d = y2 - y1;
                //            double dx = x2 - x1;
                //            double Fill = Convert.ToInt32(percent) * (d / 40);
                //            double Fillx = Convert.ToInt32(percent) * (dx / 40);

                //            System.Windows.Point[] point = new System.Windows.Point[25];
                //            PointCollection pointCollection = new PointCollection();
                //            pointCollection.Add(new System.Windows.Point(x1, y1));
                //            pointCollection.Add(new System.Windows.Point(x1 + Fillx, y1 + Fill));
                //            pointCollection.Add(new System.Windows.Point(x6, y1 + Fill));
                //            pointCollection.Add(new System.Windows.Point(x6, y1));
                //            p.Points = pointCollection;
                //            canvasTwoD.Children.Add(p);
                //            //percent = 0;
                //        }
                //        #endregion

                //        #region
                //        {
                //            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                //            p.Stroke = System.Windows.Media.Brushes.Black;
                //            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                //            mySolidColorBrush.Color = color;
                //            p.Fill = mySolidColorBrush;
                //            double d = y2 - y1;
                //            double dx = x2 - x1;
                //            double Fill = Convert.ToInt32(percent) * (d / 40);
                //            double Fillx = Convert.ToInt32(percent) * (dx / 40);

                //            System.Windows.Point[] point = new System.Windows.Point[25];
                //            PointCollection pointCollection = new PointCollection();
                //            pointCollection.Add(new System.Windows.Point(x1, y1));
                //            pointCollection.Add(new System.Windows.Point(x1 + Fillx, y1 + Fill));
                //            pointCollection.Add(new System.Windows.Point(x6, y1 + Fill));
                //            pointCollection.Add(new System.Windows.Point(x6, y1));
                //            p.Points = pointCollection;
                //            canvas2DAll.Children.Add(p);
                //            percent = 0;
                //        }
                //        #endregion
                //    }
                //    else if (percent > 40 && percent <= 80)
                //    {
                //        #region
                //        {
                //            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                //            p.Stroke = System.Windows.Media.Brushes.Black;
                //            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                //            mySolidColorBrush.Color = color;
                //            p.Fill = mySolidColorBrush;
                //            double d = y3 - y2;
                //            double dx = x3 - x2;
                //            double Fill = Convert.ToInt32(percent) * (d / 80);
                //            double Fillx = Convert.ToInt32(percent) * (dx / 80);

                //            System.Windows.Point[] point = new System.Windows.Point[25];
                //            PointCollection pointCollection = new PointCollection();
                //            pointCollection.Add(new System.Windows.Point(x1, y1));
                //            pointCollection.Add(new System.Windows.Point(x2, y2));
                //            pointCollection.Add(new System.Windows.Point(x2 + Fillx, y2 + Fill));
                //            pointCollection.Add(new System.Windows.Point(x5, y2 + Fill));
                //            pointCollection.Add(new System.Windows.Point(x5, y2));
                //            pointCollection.Add(new System.Windows.Point(x5, y1));
                //            p.Points = pointCollection;
                //            canvasTwoD.Children.Add(p);
                //            //percent = 0;
                //        }
                //        #endregion

                //        #region
                //        {
                //            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                //            p.Stroke = System.Windows.Media.Brushes.Black;
                //            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                //            mySolidColorBrush.Color = color;
                //            p.Fill = mySolidColorBrush;
                //            double d = y3 - y2;
                //            double dx = x3 - x2;
                //            double Fill = Convert.ToInt32(percent) * (d / 80);
                //            double Fillx = Convert.ToInt32(percent) * (dx / 80);

                //            System.Windows.Point[] point = new System.Windows.Point[25];
                //            PointCollection pointCollection = new PointCollection();
                //            pointCollection.Add(new System.Windows.Point(x1, y1));
                //            pointCollection.Add(new System.Windows.Point(x2, y2));
                //            pointCollection.Add(new System.Windows.Point(x2 + Fillx, y2 + Fill));
                //            pointCollection.Add(new System.Windows.Point(x5, y2 + Fill));
                //            pointCollection.Add(new System.Windows.Point(x5, y2));
                //            pointCollection.Add(new System.Windows.Point(x5, y1));
                //            p.Points = pointCollection;
                //            canvas2DAll.Children.Add(p);
                //            percent = 0;
                //        }
                //        #endregion
                //    }
                //    else if (percent > 80)
                //    {
                //        #region
                //        {
                //            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                //            p.Stroke = System.Windows.Media.Brushes.Black;
                //            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                //            mySolidColorBrush.Color = color;
                //            p.Fill = mySolidColorBrush;
                //            double d = y4 - y3;
                //            double dx = x4 - x3;
                //            double Fill = Convert.ToInt32(percent) * (d / 100);
                //            double Fillx = Convert.ToInt32(percent) * (dx / 100);

                //            System.Windows.Point[] point = new System.Windows.Point[25];
                //            PointCollection pointCollection = new PointCollection();
                //            pointCollection.Add(new System.Windows.Point(x1, y1));
                //            pointCollection.Add(new System.Windows.Point(x2, y2));
                //            pointCollection.Add(new System.Windows.Point(x3, y3));
                //            pointCollection.Add(new System.Windows.Point(x3 + Fill, y3 + Fill));
                //            pointCollection.Add(new System.Windows.Point(x5, y3 + Fill));
                //            pointCollection.Add(new System.Windows.Point(x5, y1));
                //            p.Points = pointCollection;
                //            canvasTwoD.Children.Add(p);
                //            //percent = 0;
                //        }
                //        #endregion

                //        #region
                //        {
                //            if (percent <= 100)
                //            {
                //                System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                //                p.Stroke = System.Windows.Media.Brushes.Black;
                //                SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                //                mySolidColorBrush.Color = color;
                //                p.Fill = mySolidColorBrush;
                //                double d = y4 - y3;
                //                double dx = x4 - x3;
                //                double Fill = Convert.ToInt32(percent) * (d / 100);
                //                double Fillx = Convert.ToInt32(percent) * (dx / 100);

                //                System.Windows.Point[] point = new System.Windows.Point[25];
                //                PointCollection pointCollection = new PointCollection();
                //                pointCollection.Add(new System.Windows.Point(x1, y1));
                //                pointCollection.Add(new System.Windows.Point(x2, y2));
                //                pointCollection.Add(new System.Windows.Point(x3, y3));
                //                pointCollection.Add(new System.Windows.Point(x3 + Fill, y3 + Fill));
                //                pointCollection.Add(new System.Windows.Point(x5, y3 + Fill));
                //                pointCollection.Add(new System.Windows.Point(x5, y1));
                //                p.Points = pointCollection;
                //                canvas2DAll.Children.Add(p);
                //                percent = 0;
                //            }
                //        }
                //        #endregion
                //    }


                //}

                else if (Tank_ID == 95 && percent != 0)  // else condition for  filling the curved tank <=10 compartment
                {
                    x1 = 24006.7765 + 10477;
                    x2 = 24006.7765 + 10477;
                    x3 = 22556.3671 + 10477;
                    x4 = 18005.0824 + 10477;
                    x5 = 18005.0824 + 10477;
                    x6 = 19620.1554 + 10477;
                    x7 = 20337.5244 + 10477;
                    x8 = 21230.993 + 10477;

                    y1 = 1172.1993 + 5958;
                    y2 = 2163.7309 + 5958;
                    y3 = 2163.7309 + 5958;
                    y4 = 3800.1515 + 5958;
                    y5 = 3099.9534 + 5958;
                    y6 = 2274.8719 + 5958;
                    y7 = 1818.9776 + 5958;
                    y8 = 1157.4053 + 5958;

                    #region
                    {
                        System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                        p.Stroke = System.Windows.Media.Brushes.Black;
                        SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                        mySolidColorBrush.Color = color;
                        p.Fill = mySolidColorBrush;
                        System.Windows.Point[] point = new System.Windows.Point[25];
                        PointCollection pointCollection = new PointCollection();
                        pointCollection.Add(new System.Windows.Point(x1, y1));
                        pointCollection.Add(new System.Windows.Point(x2, y2));
                        pointCollection.Add(new System.Windows.Point(x3, y3));
                        pointCollection.Add(new System.Windows.Point(x4, y4));
                        pointCollection.Add(new System.Windows.Point(x5, y5));
                        pointCollection.Add(new System.Windows.Point(x6, y6));
                        pointCollection.Add(new System.Windows.Point(x7, y7));
                        pointCollection.Add(new System.Windows.Point(x8, y8));
                        p.Points = pointCollection;
                        canvasTwoD.Children.Add(p);
                        //percent = 0;
                    }
                    #endregion

                    #region
                    {
                        if (percent <= 100)
                        {
                            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                            p.Stroke = System.Windows.Media.Brushes.Black;
                            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                            mySolidColorBrush.Color = color;
                            p.Fill = mySolidColorBrush;
                            System.Windows.Point[] point = new System.Windows.Point[25];
                            PointCollection pointCollection = new PointCollection();
                            pointCollection.Add(new System.Windows.Point(x1, y1));
                            pointCollection.Add(new System.Windows.Point(x2, y2));
                            pointCollection.Add(new System.Windows.Point(x3, y3));
                            pointCollection.Add(new System.Windows.Point(x4, y4));
                            pointCollection.Add(new System.Windows.Point(x5, y5));
                            pointCollection.Add(new System.Windows.Point(x6, y6));
                            pointCollection.Add(new System.Windows.Point(x7, y7));
                            pointCollection.Add(new System.Windows.Point(x8, y8));
                            p.Points = pointCollection;
                            canvas2DAll.Children.Add(p);
                            percent = 0;
                        }
                    }
                    #endregion

                }
                //else if (Tank_ID == 494 && percent != 0)
                //{

                //    x1 = 39761.2236 + 10477; x2 = 39759.4048 + 10477; x3 = 22075.4712 + 10477; x4 = 21756.1412 + 10477;
                //    y1 = 10926.1630 + 5958; y2 = 13821.4827 + 5958; y3 = 13826.9819 + 5958; y4 = 11226.2477 + 5958;

                //    #region
                //    {
                //        System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                //        p.Stroke = System.Windows.Media.Brushes.Black;
                //        SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                //        mySolidColorBrush.Color = System.Windows.Media.Color.FromArgb(0, 255, 255, 255);
                //        p.Fill = mySolidColorBrush;
                //        System.Windows.Point[] point = new System.Windows.Point[25];
                //        PointCollection pointCollection = new PointCollection();
                //        p.Stroke = System.Windows.Media.Brushes.Black;
                //        mySolidColorBrush.Color = color;
                //        p.Fill = mySolidColorBrush;
                //        double d = y2 - y1;
                //        double dx = x3 - x1;
                //        double Fill = Convert.ToInt32(percent) * (d / 100);
                //        double Fillx = Convert.ToInt32(percent) * (dx / 100);
                //        pointCollection.Add(new System.Windows.Point(x1, y1));
                //        pointCollection.Add(new System.Windows.Point(x1, y1 + Fill));
                //        pointCollection.Add(new System.Windows.Point(x3, y1 + Fill));
                //        pointCollection.Add(new System.Windows.Point(x4, y4));
                //        p.Points = pointCollection;
                //        canvasTwoD.Children.Add(p);
                //        //percent = 0;
                //    }
                //    #endregion

                //    #region
                //    {
                //        if (percent <= 100)
                //        {
                //            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                //            p.Stroke = System.Windows.Media.Brushes.Black;
                //            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                //            mySolidColorBrush.Color = System.Windows.Media.Color.FromArgb(0, 255, 255, 255);
                //            p.Fill = mySolidColorBrush;
                //            System.Windows.Point[] point = new System.Windows.Point[25];
                //            PointCollection pointCollection = new PointCollection();
                //            p.Stroke = System.Windows.Media.Brushes.Black;
                //            mySolidColorBrush.Color = color;
                //            p.Fill = mySolidColorBrush;
                //            double d = y2 - y1;
                //            double dx = x3 - x1;
                //            double Fill = Convert.ToInt32(percent) * (d / 100);
                //            double Fillx = Convert.ToInt32(percent) * (dx / 100);
                //            pointCollection.Add(new System.Windows.Point(x1, y1));
                //            pointCollection.Add(new System.Windows.Point(x1, y1 + Fill));
                //            pointCollection.Add(new System.Windows.Point(x3, y1 + Fill));
                //            pointCollection.Add(new System.Windows.Point(x4, y4));
                //            p.Points = pointCollection;
                //            canvas2DAll.Children.Add(p);
                //            percent = 0;
                //        }
                //    }
                //    #endregion
                //}
                //else if (Tank_ID == 497 && percent != 0)
                //{

                //    x1 = 12003.3883 + 10477; x2 = 12003.3883 + 10477; x3 = -1950.5506 + 10477; x4 = -1099.4077 + 10477;
                //    y1 = 8275.4148 + 5958; y2 = 11226.2477 + 5958; y3 = 11226.2477 + 5958; y4 = 8257.9598 + 5958;

                //    #region
                //    {
                //        System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                //        p.Stroke = System.Windows.Media.Brushes.Black;
                //        SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                //        mySolidColorBrush.Color = System.Windows.Media.Color.FromArgb(0, 255, 255, 255);
                //        p.Fill = mySolidColorBrush;
                //        System.Windows.Point[] point = new System.Windows.Point[25];
                //        PointCollection pointCollection = new PointCollection();
                //        p.Stroke = System.Windows.Media.Brushes.Black;
                //        mySolidColorBrush.Color = color;
                //        p.Fill = mySolidColorBrush;
                //        double d = y2 - y1;
                //        double dx = x3 - x1;
                //        double Fill = Convert.ToInt32(percent) * (d / 100);
                //        double Fillx = Convert.ToInt32(percent) * (dx / 100);
                //        pointCollection.Add(new System.Windows.Point(x1, y1));
                //        pointCollection.Add(new System.Windows.Point(x1, y1 + Fill));
                //        pointCollection.Add(new System.Windows.Point(x3, y1 + Fill));
                //        pointCollection.Add(new System.Windows.Point(x4, y4));
                //        p.Points = pointCollection;
                //        canvasTwoD.Children.Add(p);
                //        //percent = 0;
                //    }
                //    #endregion

                //    #region
                //    {
                //        if (percent <= 100)
                //        {
                //            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                //            p.Stroke = System.Windows.Media.Brushes.Black;
                //            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                //            mySolidColorBrush.Color = System.Windows.Media.Color.FromArgb(0, 255, 255, 255);
                //            p.Fill = mySolidColorBrush;
                //            System.Windows.Point[] point = new System.Windows.Point[25];
                //            PointCollection pointCollection = new PointCollection();
                //            p.Stroke = System.Windows.Media.Brushes.Black;
                //            mySolidColorBrush.Color = color;
                //            p.Fill = mySolidColorBrush;
                //            double d = y2 - y1;
                //            double dx = x3 - x1;
                //            double Fill = Convert.ToInt32(percent) * (d / 100);
                //            double Fillx = Convert.ToInt32(percent) * (dx / 100);
                //            pointCollection.Add(new System.Windows.Point(x1, y1));
                //            pointCollection.Add(new System.Windows.Point(x1, y1 + Fill));
                //            pointCollection.Add(new System.Windows.Point(x3, y1 + Fill));
                //            pointCollection.Add(new System.Windows.Point(x4, y4));
                //            p.Points = pointCollection;
                //            canvas2DAll.Children.Add(p);
                //            percent = 0;
                //        }
                //    }
                //    #endregion
                //}

                //else if (Tank_ID == 498 && percent != 0)
                //{
                //    x1 = 12003.3883 + 10477; x2 = 12003.3883 + 10477; x3 = -1099.4077 + 10477; x4 = -422.3398 + 10477;
                //    y1 = 5896.7434 + 5958; y2 = 8275.4148 + 5958; y3 = 8257.9598 + 5958; y4 = 5896.7434 + 5958;

                //    #region
                //    {
                //        System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                //        p.Stroke = System.Windows.Media.Brushes.Black;
                //        SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                //        mySolidColorBrush.Color = System.Windows.Media.Color.FromArgb(0, 255, 255, 255);
                //        p.Fill = mySolidColorBrush;
                //        System.Windows.Point[] point = new System.Windows.Point[25];
                //        PointCollection pointCollection = new PointCollection();
                //        p.Stroke = System.Windows.Media.Brushes.Black;
                //        mySolidColorBrush.Color = color;
                //        p.Fill = mySolidColorBrush;
                //        double d = y2 - y1;
                //        double dx = x3 - x1;
                //        double Fill = Convert.ToInt32(percent) * (d / 100);
                //        double Fillx = Convert.ToInt32(percent) * (dx / 100);
                //        pointCollection.Add(new System.Windows.Point(x1, y1));
                //        pointCollection.Add(new System.Windows.Point(x1, y1 + Fill));
                //        pointCollection.Add(new System.Windows.Point(x3, y1 + Fill));
                //        pointCollection.Add(new System.Windows.Point(x4, y4));
                //        p.Points = pointCollection;
                //        canvasTwoD.Children.Add(p);
                //        //percent = 0;
                //    }
                //    #endregion

                //    #region
                //    {
                //        if (percent <= 100)
                //        {
                //            System.Windows.Shapes.Polygon p = new System.Windows.Shapes.Polygon();
                //            p.Stroke = System.Windows.Media.Brushes.Black;
                //            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                //            mySolidColorBrush.Color = System.Windows.Media.Color.FromArgb(0, 255, 255, 255);
                //            p.Fill = mySolidColorBrush;
                //            System.Windows.Point[] point = new System.Windows.Point[25];
                //            PointCollection pointCollection = new PointCollection();
                //            p.Stroke = System.Windows.Media.Brushes.Black;
                //            mySolidColorBrush.Color = color;
                //            p.Fill = mySolidColorBrush;
                //            double d = y2 - y1;
                //            double dx = x3 - x1;
                //            double Fill = Convert.ToInt32(percent) * (d / 100);
                //            double Fillx = Convert.ToInt32(percent) * (dx / 100);
                //            pointCollection.Add(new System.Windows.Point(x1, y1));
                //            pointCollection.Add(new System.Windows.Point(x1, y1 + Fill));
                //            pointCollection.Add(new System.Windows.Point(x3, y1 + Fill));
                //            pointCollection.Add(new System.Windows.Point(x4, y4));
                //            p.Points = pointCollection;
                //            canvas2DAll.Children.Add(p);
                //            percent = 0;
                //        }
                //    }
                //    #endregion
                //}

                else if (percent > 0 && percent <= 101)
                {
                    # region Profile
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
                    }
                    #endregion

                    #region All
                    {
                        if (percent > 0 && percent <= 100)
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
                            canvas2DAll.Children.Add(p);
                        }

                    }
                    #endregion
                    // canvas2DAll.Children.Add(p);

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
                    //pointCollection.Add(new System.Windows.Point(xx[1], yy[1]));
                    //pointCollection.Add(new System.Windows.Point(xx[2], yy[2]));
                    //pointCollection.Add(new System.Windows.Point(xx[3], yy[3]));
                    //pointCollection.Add(new System.Windows.Point(xx[4], yy[4]));
                    //p.Points = pointCollection;
                    //canvasTwoD.Children.Add(p);
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

                            else if (canvasTwoD == canvas2DPlanC)
                            {
                                for (int index = 1; index <= 13; index++)
                                {

                                    pointCollection.Add(new System.Windows.Point(xx[index] - 950, yy[index] - 49850));

                                }
                            }
                            p.Points = pointCollection;
                            canvas2DAll.Children.Add(p);
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

                            else if(canvasTwoD == canvas2DPlanC)
                            {
                                for (int index = 1; index <= 13; index++)
                                {

                                    pointCollection.Add(new System.Windows.Point(xx[index]-950, yy[index]-49850));

                                }
                            }
                             p.Points = pointCollection;
                             canvas2DAll.Children.Add(p);
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
                    string stats = Convert.ToString((dgFuelOilTanks.Items[i] as DataRowView)["IsDamaged"]);
                    string Name = Convert.ToString((dgFuelOilTanks.Items[i] as DataRowView)["Tank_Name"]).Split('.')[0].Replace("/", "");
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
                for (int i = 0; i <13; i++)
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
            if (viewPort3d.EffectsManager != null)
            {
                var zoomTimer = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
                zoomTimer.Tick += (s, e) => { zoomTimer.Stop(); viewPort3d.ZoomExtents(animationTime: 300); };
                zoomTimer.Start();
            }
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


                                string sCmd = "Select * from tbl_GA_Plan_A where Tank_ID='" + IDSelect + "' and [Group]='" + SelectedGroup + "'";
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

            #region PLANC
            if ( canvas2DPlanC.IsLoaded == true)
           {
               Point tp = e.GetPosition(this.canvas2DPlanC);
               double xx = tp.X;
               double yy = tp.Y;
               AddHatchDeckPlanC();
               ShadeX = new double[14];
               ShadeY = new double[14];
               for (int i = 0; i <= 154; i++)
               {
                   for (int j = 1; j < 2; j++)

                       if ((xx <= clsGlobVar.CoordinatePlanC.mulPlanC[i, j]) && (xx >= clsGlobVar.CoordinatePlanC.mulPlanC[i, j + 1]))
                       {
                           IDSelect = clsGlobVar.CoordinatePlanC.mulPlanC[i, j - 1];
                           SelectionProcess obj = new SelectionProcess();
                           obj.ID = IDSelect;
                           obj.MaxValue = clsGlobVar.CoordinatePlanC.mulPlanC[i, j];
                           obj.MinValue = clsGlobVar.CoordinatePlanC.mulPlanC[i, j + 1];
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

                           for (int p = 0; p <= 154; p++)
                           {
                               for (int n = 1; n < 2; n++)
                               {
                                   if ((yy <= clsGlobVar.CoordinatePlanC.mulPlanC[p, n + 2]) && (yy >= clsGlobVar.CoordinatePlanC.mulPlanC[p, n + 3]) && (clsGlobVar.CoordinatePlanC.mulPlanC[p, n - 1] == f.ID))
                                   {
                                       Group_Name = Convert.ToInt16(clsGlobVar.CoordinatePlanC.mulPlanC[p, n + 4]);
                                       if (Group_Name == 1) { SelectedGroup = "A"; } else { SelectedGroup = "B"; }
                                       IDSelect = clsGlobVar.CoordinatePlanC.mulPlanC[p, n - 1];


                                       string sCmd = "Select * from [tbl_GA_Plan_C] where Tank_ID='" + IDSelect + "' and [Group]='" + SelectedGroup + "'";
                                       DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
                                       command.CommandText = sCmd;
                                       command.CommandType = CommandType.Text;
                                       string Err = "";
                                       DataTable dtcoordinateC = new DataTable();
                                       dtcoordinateC = Models.DAL.clsDBUtilityMethods.GetTable(command, Err);
                                       for (int o = 1; o <= 13; o++)
                                       {
                                           string sc = Convert.ToString("X" + o);
                                           string sr = Convert.ToString("Y" + o);
                                           ShadeX[o] = Convert.ToDouble(dtcoordinateC.Rows[0][sc]);
                                           ShadeY[o] = Convert.ToDouble(dtcoordinateC.Rows[0][sr]);
                                       }
                                       TankID = Convert.ToInt16(IDSelect);
                                       DrawHatchDeckPlan(canvas2DPlanC, Convert.ToInt32(IDSelect), 101, ShadeX, ShadeY, System.Windows.Media.Color.FromArgb(180, 255, 255, 128));
                                       Propertyset();
                                       Selection();
                                       p = 154;
                                       found = true;
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

    
   


