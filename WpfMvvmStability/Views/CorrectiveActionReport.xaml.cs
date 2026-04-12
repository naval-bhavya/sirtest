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
using System.IO;
using System.Data;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Reflection;
using System.Data.Common;
using System.Windows.Controls.DataVisualization.Charting;
using WpfMvvmStability.Models.BO;

namespace WpfMvvmStability.Views
{
    /// <summary>
    /// Interaction logic for RealModeReport.xaml
    /// </summary>
    public partial class CorrectiveActionReport : Window
    {
        DateTime myDate;
        
     
        List<KeyValuePair<double, double>> GZList = new List<KeyValuePair<double, double>>();
        List<KeyValuePair<double, double>> WHList = new List<KeyValuePair<double, double>>();
        List<KeyValuePair<double, double>> HLList = new List<KeyValuePair<double, double>>();
        List<KeyValuePair<double, double>> HSList = new List<KeyValuePair<double, double>>();
        List<KeyValuePair<double, double>> PCList = new List<KeyValuePair<double, double>>();
        List<KeyValuePair<double, double>> DFList = new List<KeyValuePair<double, double>>();


        List<KeyValuePair<double, double>> SFList = new List<KeyValuePair<double, double>>();
        List<KeyValuePair<double, double>> BMList = new List<KeyValuePair<double, double>>();
        List<KeyValuePair<double, double>> BMPermissibleHigh = new List<KeyValuePair<double, double>>();
        List<KeyValuePair<double, double>> BMPermissibleLow = new List<KeyValuePair<double, double>>();
        List<KeyValuePair<double, double>> SFPermissibleHigh = new List<KeyValuePair<double, double>>();
        List<KeyValuePair<double, double>> SFPermissibleLow = new List<KeyValuePair<double, double>>();
        int rowCount = 0;
        string stabilityType = "";
        string reportPath = "";

        public CorrectiveActionReport()
        {
            InitializeComponent();
            try
            {
                if (Models.BO.clsGlobVar.Mode == "Real")
                {
                    datePickerFrom.Visibility = Visibility.Visible;
                    datePickerTo.Visibility = Visibility.Visible;
                    btnGenerateDateList.Visibility = Visibility.Visible;
                    listBoxDateList.Visibility = Visibility.Visible;
                    lblFrom.Visibility = Visibility.Visible;
                    lblTo.Visibility = Visibility.Visible;
                    stabilityType = Models.BO.clsGlobVar.StabilityType;
                    

                }
                if (Models.BO.clsGlobVar.Mode == "Simulation")
                {
                    btnGenerateReport.Visibility = Visibility.Visible;
                    datePickerFrom.Visibility = Visibility.Hidden;
                    datePickerTo.Visibility = Visibility.Hidden;
                    btnGenerateDateList.Visibility = Visibility.Hidden;
                    listBoxDateList.Visibility = Visibility.Hidden;
                    lblFrom.Visibility = Visibility.Hidden;
                    lblTo.Visibility = Visibility.Hidden;
                    stabilityType = Models.BO.clsGlobVar.SimulationStabilityType;
                    ShowSimulationData();
                    rowCount = Models.BO.clsGlobVar.dtSimulationLongitudinal.Rows.Count;
                    rowCount = clsGlobVar.dtSimulationGZ.Rows.Count;
                    GZChart();

                }
            }
            catch
            {
            }
         
        }

        public void ShowSimulationData()
        {
            try
            {
                dgLoadingSummary.ItemsSource = Models.BO.clsGlobVar.dtSimulationLoadingSummaryCorrective.DefaultView;
                dgGZ.ItemsSource = Models.BO.clsGlobVar.dtSimulationGZCorrective.DefaultView;
                dgDraft.ItemsSource = Models.BO.clsGlobVar.dtSimulationImersionCorrective.DefaultView;
                dgHydrostatics.ItemsSource = Models.BO.clsGlobVar.dtSimulationHydrostaticsCorrective.DefaultView;
                dgHydrostatics2.ItemsSource = Models.BO.clsGlobVar.dtSimulationHydrostatics2Corrective.DefaultView;
                dgDraft1.ItemsSource = Models.BO.clsGlobVar.dtsimulationDraftsReportCorrective.DefaultView;
                dgMouldedDraft.ItemsSource = Models.BO.clsGlobVar.dtSimulationMouldedDraftCorrective.DefaultView;
                dgStability.ItemsSource = Models.BO.clsGlobVar.dtSimulationStabilityCriteriaDamageCorrective.DefaultView;
                test1corr.ItemsSource = Models.BO.clsGlobVar.dtSimulationfloodsummary.DefaultView;
                    
                   
                   
                
            }
            catch
            {
            }

        }

        public void GZChart()
        {
            try
            {
                for (int index = 0; index < dgGZ.Items.Count; index++)
                {
                    GZList.Add(new KeyValuePair<double, double>(Convert.ToDouble((dgGZ.Items[index] as DataRowView)[0]), Convert.ToDouble((dgGZ.Items[index] as DataRowView)[1])));
                    if (stabilityType == "Damage")
                    {
                        WHList.Add(new KeyValuePair<double, double>(Convert.ToDouble((dgGZ.Items[index] as DataRowView)[0]), Convert.ToDouble((dgGZ.Items[index] as DataRowView)[2])));
                        //HLList.Add(new KeyValuePair<double, double>(Convert.ToDouble((dgGZ.Items[index] as DataRowView)[0]), Convert.ToDouble((dgGZ.Items[index] as DataRowView)[3])));
                        //HSList.Add(new KeyValuePair<double, double>(Convert.ToDouble((dgGZ.Items[index] as DataRowView)[0]), Convert.ToDouble((dgGZ.Items[index] as DataRowView)[4])));
                        //PCList.Add(new KeyValuePair<double, double>(Convert.ToDouble((dgGZ.Items[index] as DataRowView)[0]), Convert.ToDouble((dgGZ.Items[index] as DataRowView)[5])));

                    }
                }
            
  
                    GZSeries.ItemsSource = GZList;

                    Style style = new Style
                    {
                        TargetType = typeof(LegendItem)
                    };
                    style.Setters.Add(new Setter(LegendItem.VisibilityProperty, Visibility.Hidden));

                    WHSeries.LegendItemStyle = style;
                    HLSeries.LegendItemStyle = style;
                    HSSeries.LegendItemStyle = style;
                    PCSeries.LegendItemStyle = style;
                    //DFSeries.LegendItemStyle = style;

                
            }
            catch
            {
            }

        }

        private void btnGenerateDateList_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string date = datePickerTo.SelectedDate.ToString();
                if (datePickerTo.SelectedDate >= DateTime.Now)
                {
                    MessageBox.Show("To date must be greater than or equal to Todays Date");
                    return;
                }            
                listBoxDateList.Items.Clear();
                StringBuilder sb = new StringBuilder();
                foreach (string txtName in Directory.GetFiles("C:\\StabilityHistory\\LoadingSummary", "*.txt"))
                {
                    if (txtName == "C:\\StabilityHistory\\LoadingSummary\\Config.txt")
                    {
                    }
                    else
                    {
                        try
                        {
                            var txtPathname = txtName.Split('\\');
                            DateTime dt1 = Convert.ToDateTime(datePickerFrom.Text);
                            dt1.AddHours(12);
                            DateTime dt2 = Convert.ToDateTime(datePickerTo.Text);
                            TimeSpan time = new TimeSpan(23,59,60);
                            DateTime dt2Spam=  dt2.Add(time);

                            string filename = txtPathname[3];
                            string[] file1 = filename.Split('.');
                            string file2 = file1[0];
                            string file = file2.Replace("_", "-");
                            file = file.Insert(10, " ");
                            file = file.Insert(13, ":");
                            file = file.Insert(16, ":");
                            file = file.Insert(19, ".");
                            DateTime dt3 = Convert.ToDateTime(file);
                            file = dt3.ToString("yyyy-MM-dd HH:mm:ss.fff");
                            if (dt3 >= dt1 && dt3 <= dt2Spam)
                            {
                                listBoxDateList.Items.Add(file);
                            }
                        }
                        catch
                        {
                        }
                    }
                }
                if (listBoxDateList.Items.Count == 0)
                {
                    MessageBox.Show("Data is not available for Current Selected Dates");
                    return;
                }
                

            }
            catch (Exception ex)
            {
                //MessageBox.Show("Please Change System Date Format as mm/dd/yyyy", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                MessageBox.Show("Change System Format : " + ex.Message);

            }
        }
         

         private void listBoxDateList_SelectionChanged(object sender, SelectionChangedEventArgs e)
         {
             try
             {
              
                 dgLoadingSummary.ItemsSource = null;
                 dgGZ.ItemsSource = null;
                 dgHydrostatics.ItemsSource = null;
                 dgDraft.ItemsSource = null;
                 dgStability.ItemsSource = null;
                 dgMouldedDraft.ItemsSource = null;

                 string DataPath = "C:\\StabilityHistory\\";
                 string s = listBoxDateList.SelectedItem.ToString();
                 string s1 = s.Replace(" ", "");
                 string s2 = s1.Replace("-", "_");
                 string s3 = s2.Replace(":", "");
                 string s4 = s3.Replace(".", "");
                 myDate = DateTime.ParseExact(s, "yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
                 DateTime dd = Convert.ToDateTime(s);
                 string dataFileName = DataPath + "LoadingSummary\\" + s4 + ".txt";
                 //Read the data from text file
                 string[] textData = System.IO.File.ReadAllLines(dataFileName);
                 DataTable dtLoadingSummary = new DataTable();
                 dtLoadingSummary.Columns.Add("Tank_Name", typeof(string), null);
                 dtLoadingSummary.Columns.Add("Frames", typeof(string), null);
                 dtLoadingSummary.Columns.Add("Cargo", typeof(string), null);

                 dtLoadingSummary.Columns.Add("Percent_Full", typeof(string));
                 dtLoadingSummary.Columns.Add("Volume", typeof(string));
                 dtLoadingSummary.Columns.Add("SG", typeof(string));
                 dtLoadingSummary.Columns.Add("Weight", typeof(decimal));
                 dtLoadingSummary.Columns.Add("LCG", typeof(decimal));
                 dtLoadingSummary.Columns.Add("TCG", typeof(decimal));
                 dtLoadingSummary.Columns.Add("VCG", typeof(decimal));
                 dtLoadingSummary.Columns.Add("FSM", typeof(decimal));
                 dtLoadingSummary.Columns.Add("IsDamaged", typeof(int));
                 for (int i = 0; i < textData.Length; i++)
                 {
                     dtLoadingSummary.Rows.Add(textData[i].Split(','));
                 }

                 dgLoadingSummary.ItemsSource = dtLoadingSummary.DefaultView;
                 dataFileName = DataPath + "DraftatEquilibriumAngle\\" + s4 + ".txt";
                 //Read the data from text file
                 string[] DataDraft = System.IO.File.ReadAllLines(dataFileName);
                 DataTable dtDraftatEquilibriumAngle = new DataTable();
                 dtDraftatEquilibriumAngle.Columns.Add("Draft_Mean", typeof(decimal));
                 dtDraftatEquilibriumAngle.Columns.Add("Draft_AP", typeof(decimal));
                 dtDraftatEquilibriumAngle.Columns.Add("Draft_FP", typeof(decimal));
                 dtDraftatEquilibriumAngle.Columns.Add("Draft_Aft_Mark", typeof(decimal));                                  
                 dtDraftatEquilibriumAngle.Columns.Add("Draft_Fore_Mark", typeof(decimal));
                 dtDraftatEquilibriumAngle.Columns.Add("Draft_Mid_Mark", typeof(decimal));

                 for (int i = 0; i < DataDraft.Length; i++)
                 {
                     dtDraftatEquilibriumAngle.Rows.Add(DataDraft[i].Split(','));
                 }

                 dgDraft.ItemsSource = dtDraftatEquilibriumAngle.DefaultView;
                 dataFileName = DataPath + "HydrostaticsatEquilibriumAngle\\" + s4 + ".txt";
                 //Read the data from text file
                 string[] Datahydro = System.IO.File.ReadAllLines(dataFileName);
                 DataTable dtHydroStatic = new DataTable();
                 dtHydroStatic.Columns.Add("Displacement", typeof(decimal));
                 dtHydroStatic.Columns.Add("TRIM", typeof(decimal));
                 dtHydroStatic.Columns.Add("Heel", typeof(decimal));
                 dtHydroStatic.Columns.Add("GMT", typeof(decimal));
                 dtHydroStatic.Columns.Add("FSC", typeof(decimal));
                 dtHydroStatic.Columns.Add("KG(Solid)", typeof(decimal));
                 dtHydroStatic.Columns.Add("KG(Fluid)", typeof(decimal));
                 dtHydroStatic.Columns.Add("LCG", typeof(decimal));
                 dtHydroStatic.Columns.Add("TCG", typeof(decimal));
                 dtHydroStatic.Columns.Add("LCF", typeof(decimal));
                 dtHydroStatic.Columns.Add("MCT", typeof(decimal));
                 dtHydroStatic.Columns.Add("TPC", typeof(decimal));
                 dtHydroStatic.Columns.Add("WPA(m²)", typeof(decimal));

                 for (int i = 0; i < Datahydro.Length; i++)
                 {
                     dtHydroStatic.Rows.Add(Datahydro[i].Split(','));
                 }
                 dgHydrostatics.ItemsSource = dtHydroStatic.DefaultView;

                 dataFileName = DataPath + "MouldedDraftValues\\" + s4 + ".txt";
                 //Read the data from text file  dgMouldedDraft
                 string[] DataMouldedDraft = System.IO.File.ReadAllLines(dataFileName);
                 DataTable dtMouldedDraft = new DataTable();
                 dtHydroStatic.Columns.Add("Draft_AP", typeof(decimal));
                // dtHydroStatic.Columns.Add("Draft_Propeller", typeof(decimal));
                 dtHydroStatic.Columns.Add("Draft_Aft_Mark", typeof(decimal));
                 dtHydroStatic.Columns.Add("Draft_Mean", typeof(decimal));
                 dtHydroStatic.Columns.Add("Draft_Fore_Mark", typeof(decimal));
               //  dtHydroStatic.Columns.Add("Draft_Sonar_Dome", typeof(decimal));
                 dtHydroStatic.Columns.Add("Draft_FP", typeof(decimal));

                 for (int i = 0; i < DataMouldedDraft.Length; i++)
                 {
                     dtMouldedDraft.Rows.Add(DataMouldedDraft[i].Split(','));
                 }
                 dgMouldedDraft.ItemsSource = dtMouldedDraft.DefaultView;


                 dataFileName = DataPath + "ManualLoadingCondition\\" + s4 + ".txt";
                 //Read the data from text file
                 string[] DataManualCondition = System.IO.File.ReadAllLines(dataFileName);
                 DataTable dtManualLoadingCondition = new DataTable();
                 if (DataManualCondition.Length > 0)
                 {
                     dtManualLoadingCondition.Columns.Add("Tank_Name", typeof(string));
                     dtManualLoadingCondition.Columns.Add("Weight", typeof(decimal));
                     dtManualLoadingCondition.Columns.Add("Sounding_level", typeof(decimal));
                     dataFileName = DataPath + "StabilityType\\" + s4 + ".txt";
                     //Read tyothe data from text file
                     string[] Stabilitytype = System.IO.File.ReadAllLines(dataFileName);
                     DataTable dtStabilitytype = new DataTable();
                     dtStabilitytype.Columns.Add("Stability_Type", typeof(string));

                     for (int i = 0; i < Stabilitytype.Length; i++)
                     {
                         dtStabilitytype.Rows.Add(Stabilitytype[i].Split(','));

                     }
                     stabilityType = dtStabilitytype.Rows[0]["Stability_Type"].ToString();
                     //if (stabilityType == "Intact")
                     //{
            

                     //    dataFileName = DataPath + "IntactStabilityCriteria\\" + s4 + ".txt";
                     //    //Read the data from text file
                     //    string[] DataIntactStability = System.IO.File.ReadAllLines(dataFileName);
                     //    DataTable dtIntactStability = new DataTable();

                     //    dtIntactStability.Columns.Add("Criterion", typeof(string));
                     //    dtIntactStability.Columns.Add("Critical_Value", typeof(decimal));
                     //    dtIntactStability.Columns.Add("Actual_Value", typeof(decimal),null);
                     //    dtIntactStability.Columns.Add("Status", typeof(string), null);
                     //    try
                     //    {
                     //        for (int i = 0; i < DataIntactStability.Length; i++)
                     //        {
                     //            dtIntactStability.Rows.Add(DataIntactStability[i].Split(','));
                     //        }
                     //    }
                     //    catch{
                     //    }

                     //    DataTable dtfinal = dtIntactStability.Clone();
                     //    foreach (DataRow dr in dtIntactStability.Rows)
                     //    {

                     //        dtfinal.Rows.Add(dr.ItemArray);
                     //    }
                     //    dtfinal.Columns.Remove("Status");
                     //    dtfinal.Columns.Add("Status", typeof(string));


                     //    for (int i = 0; i < dtIntactStability.Rows.Count; i++)
                     //    {
                     //        if (dtIntactStability.Rows[i]["Status"].ToString() == "1")
                     //        {
                     //            dtfinal.Rows[i]["Status"] = "Pass";
                     //        }
                     //        else
                     //        {
                     //            dtfinal.Rows[i]["Status"] = "Fail";
                     //        }

                     //    }
                     //    dtfinal.Columns["Status"].ReadOnly = true;

                     //    dgStability.ItemsSource = dtfinal.DefaultView;
                     //    groupBoxStability.Header = "NES 109 StabilityP15B Criteria - Intact";
                     //    string temp1 = groupBoxStability.Header.ToString();
                     //    dataFileName = DataPath + "GZ_Data_Intact\\" + s4 + ".txt";
                     //    //Read the data from text file
                     //    string[] DataGZ = System.IO.File.ReadAllLines(dataFileName);
                     //    DataTable dtGZData = new DataTable();

                     //    dtGZData.Columns.Add("heelAng", typeof(decimal));
                     //    dtGZData.Columns.Add("heelGZ", typeof(decimal));
                     //    dtGZData.Columns.Add("WH", typeof(decimal));
                     //    dtGZData.Columns.Add("HL", typeof(decimal));
                     //    dtGZData.Columns.Add("HS", typeof(decimal));
                     //    dtGZData.Columns.Add("PC", typeof(decimal));


                     //    for (int i = 0; i < DataGZ.Length; i++)
                     //    {
                     //        dtGZData.Rows.Add(DataGZ[i].Split(','));
                     //    }

                     //    dgGZ.ItemsSource = dtGZData.DefaultView;

                           

                     //    dataFileName = DataPath + "Combine_StnNo\\" + s4 + ".txt";
                     //    //Read the data from text file
                     //    string[] DataLongitudinal = System.IO.File.ReadAllLines(dataFileName);
                     //    DataTable dtLongitudinalData = new DataTable();

                     //    dtLongitudinalData.Columns.Add("Length", typeof(decimal));
                     //    dtLongitudinalData.Columns.Add("BuoyanceUDL", typeof(decimal));
                     //    dtLongitudinalData.Columns.Add("NetUDL", typeof(decimal));
                     //    dtLongitudinalData.Columns.Add("SF", typeof(decimal));
                     //    dtLongitudinalData.Columns.Add("BM", typeof(decimal));

                     //    for (int i = 0; i < DataLongitudinal.Length; i++)
                     //    {
                     //        dtLongitudinalData.Rows.Add(DataLongitudinal[i].Split(','));
                     //    }

                     //   // dgLongitudinal.ItemsSource = dtLongitudinalData.DefaultView;
                     //    rowCount =dtLongitudinalData.Rows.Count;
                     //    //LongitudinalGraph(); 
                     //    rowCount = dtGZData.Rows.Count;
                     //    GZChart();
                     //}
                     //else if (stabilityType == "Damage")
                     //{
                     //    dataFileName = DataPath + "DamageStabilityCriteria\\" + s4 + ".txt";
                     //    //Read the data from text file
                     //    string[] DataIntactStability = System.IO.File.ReadAllLines(dataFileName);
                     //    DataTable dtIntactStability = new DataTable();

                     //    dtIntactStability.Columns.Add("Criterion", typeof(string));
                     //    dtIntactStability.Columns.Add("Critical_Value", typeof(decimal));
                     //    dtIntactStability.Columns.Add("Actual_Value", typeof(decimal),null);
                     //    dtIntactStability.Columns.Add("Status", typeof(string), null);
                     //    for (int i = 0; i < DataIntactStability.Length; i++)
                     //    {
                     //        dtIntactStability.Rows.Add(DataIntactStability[i].Split(','));
                     //    }

                     //    DataTable dtfinal = dtIntactStability.Clone();
                     //    foreach (DataRow dr in dtIntactStability.Rows)
                     //    {

                     //        dtfinal.Rows.Add(dr.ItemArray);
                     //    }
                     //    dtfinal.Columns.Remove("Status");
                     //    dtfinal.Columns.Add("Status", typeof(string));


                     //    for (int i = 0; i < dtIntactStability.Rows.Count; i++)
                     //    {
                     //        if (dtIntactStability.Rows[i]["Status"].ToString() == "1")
                     //        {
                     //            dtfinal.Rows[i]["Status"] = "Pass";
                     //        }
                     //        else
                     //        {
                     //            dtfinal.Rows[i]["Status"] = "Fail";
                     //        }

                     //    }
                     //    dtfinal.Columns["Status"].ReadOnly = true;

                     //    dgStability.ItemsSource = dtfinal.DefaultView;
                     //    groupBoxStability.Header = "NES 109 StabilityP15B Criteria - Damage";
                     //    string temp1 = groupBoxStability.Header.ToString();
                     //    //clsSqlData.rpt = temp1;

                     //    dataFileName = DataPath + "GZ_Data_Damage\\" + s4 + ".txt";
                     //    //Read the data from text file
                     //    string[] DataGZ = System.IO.File.ReadAllLines(dataFileName);
                     //    DataTable dtGZData = new DataTable();

                     //    dtGZData.Columns.Add("heelAng", typeof(decimal));
                     //    dtGZData.Columns.Add("heelGZ", typeof(decimal));
                     //    dtGZData.Columns.Add("WH", typeof(decimal));
                     //    dtGZData.Columns.Add("HL", typeof(decimal));
                     //    dtGZData.Columns.Add("HS", typeof(decimal));
                     //    dtGZData.Columns.Add("PC", typeof(decimal));

                     //    for (int i = 0; i < DataGZ.Length; i++)
                     //    {
                     //        dtGZData.Rows.Add(DataGZ[i].Split(','));
                     //    }

                     //    dgGZ.ItemsSource = dtGZData.DefaultView;
                     //    rowCount = dtGZData.Rows.Count;
                      
                     //}
                    
                     GZChart();
                 }

             }
             catch
             {
             }
         }

         private void scrollViewer1_ScrollChanged(object sender, ScrollChangedEventArgs e)
         {

         }

         private void NetLoadSeries_Loaded(object sender, RoutedEventArgs e)
         {

         }

         private void btnGenerateReport_Click(object sender, RoutedEventArgs e)
         {
             try
             {
                
                int chartWidth = Convert.ToInt32(GzChart.ActualWidth);
                int chartHeight = Convert.ToInt32(GzChart.ActualHeight);
                double resolution = 96d;
                 GzChart.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
                 GzChart.Arrange(new Rect(new System.Windows.Size(chartWidth, chartHeight)));
                 GzChart.UpdateLayout();
                 GzChart.InvalidateVisual();

                 RenderTargetBitmap bmp = new RenderTargetBitmap(chartWidth, chartHeight, resolution, resolution, PixelFormats.Pbgra32);
                 bmp.Render(GzChart);
                 BmpBitmapEncoder encoder = new BmpBitmapEncoder();
                 encoder.Frames.Add(BitmapFrame.Create(bmp));
                 FileStream fileStream = new FileStream(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Images\\GZ.png", FileMode.Create);
                 encoder.Save(fileStream);
                 fileStream.Close();
                 bmp.Clear();
                 printToPdf();
         
                 try
                 {
                     //foreach (Window window in Application.Current.Windows)
                     //{
                     //    if (window.GetType() == typeof(MainWindow))
                     //    {
                     //        (window as MainWindow).listBoxPDF.Items.Clear();
                     //      // (window as MainWindow).webBrowserPDF.Refresh();

                     //        (window as MainWindow).lblListBoxError.Visibility = Visibility.Hidden;
                     //        string st = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                     //        string path = st + "\\Reports";

                     //        var dir = Directory.GetFiles(path);

                     //        string names;
                     //        foreach (string s in dir)
                     //        {
                     //            //names = s.Remove(0, index);
                     //            (window as MainWindow).listBoxPDF.Items.Add(System.IO.Path.GetFileName(s));
                     //        }
                     //        (window as MainWindow).listBoxPDF.Items.SortDescriptions.Add(
                     //                new System.ComponentModel.SortDescription("",
                     //                System.ComponentModel.ListSortDirection.Ascending));
                     //        if ((window as MainWindow).listBoxPDF.Items.Count == 0)
                     //        {
                     //            (window as MainWindow).lblListBoxError.Visibility = Visibility.Visible;
                     //        }
                     //    }
                     //}
                 }
                 catch
                 {

                 }
             }
             catch
             {
             }
         }
         #region PDFWriter
         static String ISO_Date()
        {
            return DateTime.Now.ToString("yyyy_MM_dd HH_mm_ss");
        }
        public void printToPdf()
        {
            try
            {

                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                Document doc = new Document(iTextSharp.text.PageSize.A4, 30, 30, 30, 20);
                reportPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Reports\\" + ISO_Date() + "_Report.pdf";
                PdfWriter wri = PdfWriter.GetInstance(doc, new FileStream(reportPath, FileMode.Create));
                doc.Open();//Open Document to write
                wri.PageEvent = new pdfFormating();
                iTextSharp.text.Paragraph p2 = new iTextSharp.text.Paragraph(" Stability Report : " + stabilityType, FontFactory.GetFont(FontFactory.TIMES, 12, iTextSharp.text.Font.BOLD)); // Loading Summary Table Name
                p2.Alignment = Element.ALIGN_CENTER;
                doc.Add(p2);

                //...........StartOFLogo.........................................
                iTextSharp.text.Image LogoWatermark = iTextSharp.text.Image.GetInstance(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Images\\Watermark.png");
                iTextSharp.text.Image pic = iTextSharp.text.Image.GetInstance(LogoWatermark);
                pic.Alignment = Element.ALIGN_LEFT;
                pic.ScaleToFit(70, 55);
                doc.Add(pic);
                iTextSharp.text.Image logoMdl = iTextSharp.text.Image.GetInstance(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Images\\LOGOMDL.jpg");
                iTextSharp.text.Image pic1 = iTextSharp.text.Image.GetInstance(logoMdl);
                pic1.Alignment = Element.ALIGN_RIGHT;
                pic1.ScaleToFit(40, 40);
                pic1.SetAbsolutePosition(514, 730);
                doc.Add(pic1);
                iTextSharp.text.Image logoLnT = iTextSharp.text.Image.GetInstance(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Images\\LOGOLnT.gif");
                iTextSharp.text.Image pic2 = iTextSharp.text.Image.GetInstance(logoLnT);
                pic2.Alignment = Element.ALIGN_RIGHT;
                pic2.ScaleToFit(40, 40);
                pic2.SetAbsolutePosition(470, 730);
                doc.Add(pic2);
                //...........EndOFLogo.........................................


                //...........StartOfloadingsummary.............................
                int columnCount;
                int rowCount;
                iTextSharp.text.Paragraph p3 = new iTextSharp.text.Paragraph("Loading Summary: Corrective", FontFactory.GetFont(FontFactory.TIMES, 12, iTextSharp.text.Font.BOLD)); // Loading Summary Table Name
                p3.Alignment = Element.ALIGN_CENTER;
                doc.Add(p3);
                doc.Add(new iTextSharp.text.Paragraph("  "));
                iTextSharp.text.Font fntHeader = FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Color.BLUE);   //Header
                iTextSharp.text.Font fntBody = FontFactory.GetFont("Times New Roman", 6.5f);   // Body
                PdfPTable tblLoadingSummary = new PdfPTable(10);
                tblLoadingSummary.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                tblLoadingSummary.WidthPercentage = 90;
                float[] widthsLoading = new float[] { 2.5f, 1.8f, 1.8f, 1.2f, 1.2f, 1.2f, 1.2f, 1.2f, 1.5f, 1f };
                tblLoadingSummary.SetWidths(widthsLoading);
                tblLoadingSummary.HorizontalAlignment = Element.ALIGN_CENTER;
                iTextSharp.text.Font fntRedColor = FontFactory.GetFont("Times New Roman", 1, iTextSharp.text.Color.ORANGE);// Body

                PdfPCell tankName = new PdfPCell(new Phrase("Tank Name", fntHeader));
                tankName.HorizontalAlignment = Element.ALIGN_CENTER;
                PdfPCell tankStatus = new PdfPCell(new Phrase("Status", fntHeader));
                tankStatus.HorizontalAlignment = Element.ALIGN_CENTER;
                PdfPCell percentFull = new PdfPCell(new Phrase("Percent Fill", fntHeader));
                percentFull.HorizontalAlignment = Element.ALIGN_CENTER;
                PdfPCell Volume = new PdfPCell(new Phrase("Volume(cu.m)", fntHeader));
                Volume.HorizontalAlignment = Element.ALIGN_CENTER;
                PdfPCell sg = new PdfPCell(new Phrase("SG", fntHeader));
                sg.HorizontalAlignment = Element.ALIGN_CENTER;
                PdfPCell weight = new PdfPCell(new Phrase("Weight(T)", fntHeader));
                weight.HorizontalAlignment = Element.ALIGN_CENTER;
                PdfPCell lcg = new PdfPCell(new Phrase("LCG(m)", fntHeader));
                lcg.HorizontalAlignment = Element.ALIGN_CENTER;
                PdfPCell tcg = new PdfPCell(new Phrase("TCG(m)", fntHeader));
                tcg.HorizontalAlignment = Element.ALIGN_CENTER;
                PdfPCell vcg = new PdfPCell(new Phrase("VCG(m)", fntHeader));
                vcg.HorizontalAlignment = Element.ALIGN_CENTER;
                PdfPCell fsm = new PdfPCell(new Phrase("FSM(T-m)", fntHeader));
                fsm.HorizontalAlignment = Element.ALIGN_CENTER;
                //Add header to table
                tblLoadingSummary.AddCell(tankName);
                tblLoadingSummary.AddCell(percentFull);
                tblLoadingSummary.AddCell(Volume);
                tblLoadingSummary.AddCell(sg);
                tblLoadingSummary.AddCell(weight);
                tblLoadingSummary.AddCell(lcg);
                tblLoadingSummary.AddCell(tcg);
                tblLoadingSummary.AddCell(vcg);
                tblLoadingSummary.AddCell(fsm);
                tblLoadingSummary.AddCell(tankStatus);
                DataTable dtLoadingSummary = ((DataView)dgLoadingSummary.ItemsSource).ToTable().Clone();

                foreach (DataRow dr in ((DataView)dgLoadingSummary.ItemsSource).ToTable().Rows)
                {

                    dtLoadingSummary.Rows.Add(dr.ItemArray);

                }

                dtLoadingSummary.Columns.Remove("IsDamaged");
                dtLoadingSummary.Columns.Add("IsDamaged", typeof(string));

                dtLoadingSummary.Columns.Remove("Tank_ID");

                for (int i = 0; i < ((DataView)dgLoadingSummary.ItemsSource).ToTable().Rows.Count; i++)
                {
                    if (((DataView)dgLoadingSummary.ItemsSource).ToTable().Rows[i]["IsDamaged"].ToString() == "True")
                    {
                        dtLoadingSummary.Rows[i]["IsDamaged"] = "Damaged";
                    }
                    else if (Models.BO.clsGlobVar.Mode != "Real")
                    {
                        if (((DataView)test1corr.ItemsSource).ToTable().Rows[i]["status"].ToString() == "2")
                        {
                            dtLoadingSummary.Rows[i]["IsDamaged"] = "Flood";
                        }


                        else
                        {

                            dtLoadingSummary.Rows[i]["IsDamaged"] = "Intact";

                        }
                    }
                    else
                    {

                        dtLoadingSummary.Rows[i]["IsDamaged"] = "Intact";

                    }
                }
                dtLoadingSummary.Columns["IsDamaged"].ReadOnly = true;
                columnCount = dtLoadingSummary.Columns.Count;
                rowCount = dtLoadingSummary.Rows.Count;
                //for (int rowCounter = 0; rowCounter < rowCount; rowCounter++)
                for (int rowCounter = 0; rowCounter < 47; rowCounter++)
                {
                    for (int columnCounter = 0; columnCounter < columnCount; columnCounter++)
                    {
                        string strValue = (dtLoadingSummary.Rows[rowCounter][columnCounter].ToString());
                        string temp;
                        string strDmage = (dtLoadingSummary.Rows[rowCounter][11].ToString());

                        if (columnCounter != 1 && columnCounter != 2)
                        {
                            if (rowCounter == rowCount - 1 || rowCounter == rowCount - 2 || rowCounter == rowCount - 3)
                            {
                                if (columnCounter == 0)
                                {
                                    try
                                    {
                                        PdfPCell pdf1;
                                        pdf1 = new PdfPCell(new iTextSharp.text.Paragraph(strValue, FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                                        pdf1.HorizontalAlignment = Element.ALIGN_CENTER;
                                        pdf1.Colspan = 4;
                                        tblLoadingSummary.AddCell(pdf1);
                                    }
                                    catch
                                    {
                                    }
                                }

                                else if (columnCounter == 6 || columnCounter == 7 || columnCounter == 8 || columnCounter == 9 || columnCounter == 10)
                                {
                                    decimal d = Convert.ToDecimal(strValue);
                                    temp = Convert.ToString(Math.Round(d, 3));
                                    PdfPCell pdf = new PdfPCell(new iTextSharp.text.Paragraph(temp, fntBody));
                                    pdf.HorizontalAlignment = Element.ALIGN_CENTER;
                                    tblLoadingSummary.AddCell(pdf);
                                }
                                else if (columnCounter == 11)
                                {
                                    PdfPCell pdf1 = new PdfPCell(new iTextSharp.text.Paragraph(" ", fntBody));
                                    pdf1.HorizontalAlignment = Element.ALIGN_CENTER;
                                    tblLoadingSummary.AddCell(pdf1);
                                }
                            }
                            else if (columnCounter == 3 || columnCounter == 4 || columnCounter == 5 || columnCounter == 6 || columnCounter == 7 || columnCounter == 8 || columnCounter == 9 || columnCounter == 10 && rowCounter != rowCount - 1 && rowCounter != rowCount - 2 && rowCounter != rowCount - 3)
                            {
                                PdfPCell pdf;
                                decimal d = Convert.ToDecimal(strValue);
                                temp = Convert.ToString(Math.Round(d, 3));
                                if (temp == Convert.ToString(0))
                                {
                                    pdf = new PdfPCell(new iTextSharp.text.Paragraph(" ", fntBody));

                                }
                                else
                                {
                                    // pdf = new PdfPCell(new iTextSharp.text.Paragraph(temp, fntBody));
                                    if (strDmage == "Damaged")
                                    {
                                        pdf = new PdfPCell(new iTextSharp.text.Paragraph(" ", fntRedColor));
                                        pdf.BackgroundColor = new iTextSharp.text.Color(214, 108, 105);
                                    }
                                    else
                                    {
                                        pdf = new PdfPCell(new iTextSharp.text.Paragraph(temp, fntBody));
                                    }
                                }
                                pdf.HorizontalAlignment = Element.ALIGN_CENTER;
                                tblLoadingSummary.AddCell(pdf);
                            }
                            else
                            {
                                PdfPCell pdf1;
                                temp = Convert.ToString(strValue);
                                if (temp == Convert.ToString(0))
                                {
                                    pdf1 = new PdfPCell(new iTextSharp.text.Paragraph(" ", fntBody));
                                }
                                else
                                {//pdf1 = new PdfPCell(new iTextSharp.text.Paragraph(temp, fntBody));
                                    {
                                        if (strValue == "Damaged")
                                        {
                                            iTextSharp.text.Font fntBody1 = FontFactory.GetFont("Times New Roman", 6.5f, iTextSharp.text.Color.RED);
                                            pdf1 = new PdfPCell(new iTextSharp.text.Paragraph(temp, fntBody1));
                                        }
                                        else
                                        {
                                            pdf1 = new PdfPCell(new iTextSharp.text.Paragraph(temp, fntBody));
                                        }
                                    }
                                }
                                if (columnCounter == 0)
                                {
                                    pdf1.HorizontalAlignment = Element.ALIGN_LEFT;
                                }
                                else
                                {
                                    pdf1.HorizontalAlignment = Element.ALIGN_CENTER;
                                }
                                tblLoadingSummary.AddCell(pdf1);
                            }

                        }

                    }
                }
                doc.Add(tblLoadingSummary);
                doc.Add(new iTextSharp.text.Paragraph("  "));
                //...........EndOfloadingsummary.....................................

                //...........StartOfloadingsummarylast 11 lines.......................
                try
                {

                    doc.Add(new iTextSharp.text.Paragraph("  "));
                    PdfPTable tblLoadingSummary1 = new PdfPTable(10);
                    tblLoadingSummary1.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    tblLoadingSummary1.WidthPercentage = 90;
                    float[] widthsLoading1 = new float[] { 2.5f, 1.8f, 1.8f, 1.2f, 1.2f, 1.2f, 1.2f, 1.2f, 1.5f, 1f };
                    iTextSharp.text.Font fntRedColor1 = FontFactory.GetFont("Times New Roman", 1, iTextSharp.text.Color.ORANGE);
                    tblLoadingSummary1.SetWidths(widthsLoading1);
                    tblLoadingSummary1.HorizontalAlignment = Element.ALIGN_CENTER;
                    PdfPCell tankName1 = new PdfPCell(new Phrase("Tank Name", fntHeader));
                    tankName1.HorizontalAlignment = Element.ALIGN_CENTER;
                    PdfPCell tankStatus1 = new PdfPCell(new Phrase("Status", fntHeader));
                    tankStatus1.HorizontalAlignment = Element.ALIGN_CENTER;
                    PdfPCell percentFull1 = new PdfPCell(new Phrase("Percent Fill", fntHeader));
                    percentFull1.HorizontalAlignment = Element.ALIGN_CENTER;
                    PdfPCell Volume1 = new PdfPCell(new Phrase("Volume(cu.m)", fntHeader));
                    Volume1.HorizontalAlignment = Element.ALIGN_CENTER;
                    PdfPCell sg1 = new PdfPCell(new Phrase("SG", fntHeader));
                    sg1.HorizontalAlignment = Element.ALIGN_CENTER;
                    PdfPCell weight1 = new PdfPCell(new Phrase("Weight(T)", fntHeader));
                    weight1.HorizontalAlignment = Element.ALIGN_CENTER;
                    PdfPCell lcg1 = new PdfPCell(new Phrase("LCG(m)", fntHeader));
                    lcg1.HorizontalAlignment = Element.ALIGN_CENTER;
                    PdfPCell tcg11 = new PdfPCell(new Phrase("TCG(m)", fntHeader));
                    tcg11.HorizontalAlignment = Element.ALIGN_CENTER;
                    PdfPCell vcg1 = new PdfPCell(new Phrase("VCG(m)", fntHeader));
                    vcg1.HorizontalAlignment = Element.ALIGN_CENTER;
                    PdfPCell fsm1 = new PdfPCell(new Phrase("FSM(T-m)", fntHeader));
                    fsm1.HorizontalAlignment = Element.ALIGN_CENTER;
                    tblLoadingSummary1.AddCell(tankName1);
                    tblLoadingSummary1.AddCell(percentFull1);
                    tblLoadingSummary1.AddCell(Volume1);
                    tblLoadingSummary1.AddCell(sg1);
                    tblLoadingSummary1.AddCell(weight1);
                    tblLoadingSummary1.AddCell(lcg1);
                    tblLoadingSummary1.AddCell(tcg11);
                    tblLoadingSummary1.AddCell(vcg1);
                    tblLoadingSummary1.AddCell(fsm1);
                    tblLoadingSummary1.AddCell(tankStatus1);
                    DataTable dtLoadingSummary1 = ((DataView)dgLoadingSummary.ItemsSource).ToTable().Clone();
                    foreach (DataRow dr in ((DataView)dgLoadingSummary.ItemsSource).ToTable().Rows)
                    {
                        dtLoadingSummary1.Rows.Add(dr.ItemArray);
                    }
                    dtLoadingSummary1.Columns.Remove("IsDamaged");
                    dtLoadingSummary1.Columns.Add("IsDamaged", typeof(string));

                    dtLoadingSummary1.Columns.Remove("Tank_ID");
                    for (int i = 0; i < ((DataView)dgLoadingSummary.ItemsSource).ToTable().Rows.Count; i++)
                    {
                        if (((DataView)dgLoadingSummary.ItemsSource).ToTable().Rows[i]["IsDamaged"].ToString() == "True")
                        {
                            dtLoadingSummary1.Rows[i]["IsDamaged"] = "Damaged";
                        }
                        else if (Models.BO.clsGlobVar.Mode != "Real")
                        {
                            if (((DataView)test1corr.ItemsSource).ToTable().Rows[i]["status"].ToString() == "2")
                            {
                                dtLoadingSummary1.Rows[i]["IsDamaged"] = "Flood";
                            }


                            else
                            {

                                dtLoadingSummary1.Rows[i]["IsDamaged"] = "Intact";

                            }
                        }
                        else
                        {

                            dtLoadingSummary1.Rows[i]["IsDamaged"] = "Intact";

                        }
                    }
                    dtLoadingSummary1.Columns["IsDamaged"].ReadOnly = true;
                    columnCount = dtLoadingSummary1.Columns.Count;
                    rowCount = dtLoadingSummary1.Rows.Count;
                    if (rowCount > 90)
                    {
                        for (int rowCounter = 47; rowCounter < 91; rowCounter++)
                        {
                            if (rowCounter == 47)
                            {
                                doc.Add(new iTextSharp.text.Paragraph("  "));
                            }
                            // doc.Add(new iTextSharp.text.Paragraph("  "));
                            for (int columnCounter = 0; columnCounter < columnCount; columnCounter++)
                            {
                                string strValue = (dtLoadingSummary1.Rows[rowCounter][columnCounter].ToString());
                                string temp;
                                string strDmage = (dtLoadingSummary1.Rows[rowCounter][11].ToString());
                                if (columnCounter != 1 && columnCounter != 2)
                                {
                                    if (rowCounter == rowCount - 1 || rowCounter == rowCount - 2 || rowCounter == rowCount - 3)
                                    {
                                        if (columnCounter == 0)
                                        {
                                            try
                                            {
                                                PdfPCell pdf1;
                                                pdf1 = new PdfPCell(new iTextSharp.text.Paragraph(strValue, FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                                                pdf1.HorizontalAlignment = Element.ALIGN_CENTER;
                                                pdf1.Colspan = 4;
                                                tblLoadingSummary1.AddCell(pdf1);
                                            }
                                            catch
                                            {
                                            }
                                        }

                                        else if (columnCounter == 6 || columnCounter == 7 || columnCounter == 8 || columnCounter == 9 || columnCounter == 10)
                                        {
                                            decimal d = Convert.ToDecimal(strValue);
                                            temp = Convert.ToString(Math.Round(d, 3));
                                            PdfPCell pdf = new PdfPCell(new iTextSharp.text.Paragraph(temp, fntBody));
                                            pdf.HorizontalAlignment = Element.ALIGN_CENTER;
                                            tblLoadingSummary1.AddCell(pdf);
                                        }
                                        else if (columnCounter == 11)
                                        {
                                            PdfPCell pdf1 = new PdfPCell(new iTextSharp.text.Paragraph(" ", fntBody));
                                            pdf1.HorizontalAlignment = Element.ALIGN_CENTER;
                                            tblLoadingSummary1.AddCell(pdf1);
                                        }
                                    }
                                    else if (columnCounter == 3 || columnCounter == 4 || columnCounter == 5 || columnCounter == 6 || columnCounter == 7 || columnCounter == 8 || columnCounter == 9 || columnCounter == 10 && rowCounter != rowCount - 1 && rowCounter != rowCount - 2 && rowCounter != rowCount - 3)
                                    {
                                        PdfPCell pdf;
                                        decimal d = Convert.ToDecimal(strValue);
                                        temp = Convert.ToString(Math.Round(d, 3));
                                        if (temp == Convert.ToString(0))
                                        {
                                            pdf = new PdfPCell(new iTextSharp.text.Paragraph(" ", fntBody));
                                        }
                                        else
                                        {
                                            //pdf = new PdfPCell(new iTextSharp.text.Paragraph(temp, fntBody));
                                            if (strDmage == "Damaged")
                                            {
                                                pdf = new PdfPCell(new iTextSharp.text.Paragraph(" ", fntRedColor1));
                                                pdf.BackgroundColor = new iTextSharp.text.Color(214, 108, 105);
                                            }
                                            else
                                            {
                                                pdf = new PdfPCell(new iTextSharp.text.Paragraph(temp, fntBody));
                                            }
                                        }
                                        pdf.HorizontalAlignment = Element.ALIGN_CENTER;
                                        tblLoadingSummary1.AddCell(pdf);
                                    }

                                    else
                                    {
                                        PdfPCell pdf1;
                                        temp = Convert.ToString(strValue);
                                        if (temp == Convert.ToString(0))
                                        {
                                            pdf1 = new PdfPCell(new iTextSharp.text.Paragraph(" ", fntBody));
                                        }
                                        else
                                        {
                                            //pdf1 = new PdfPCell(new iTextSharp.text.Paragraph(temp, fntBody));

                                            {
                                                if (strValue == "Damaged")
                                                {
                                                    iTextSharp.text.Font fntBody1 = FontFactory.GetFont("Times New Roman", 6.5f, iTextSharp.text.Color.RED);
                                                    pdf1 = new PdfPCell(new iTextSharp.text.Paragraph(temp, fntBody1));
                                                }
                                                else
                                                {
                                                    pdf1 = new PdfPCell(new iTextSharp.text.Paragraph(temp, fntBody));
                                                }
                                            }

                                        }
                                        if (columnCounter == 0)
                                        {
                                            pdf1.HorizontalAlignment = Element.ALIGN_LEFT;
                                        }
                                        else
                                        {
                                            pdf1.HorizontalAlignment = Element.ALIGN_CENTER;
                                        }
                                        tblLoadingSummary1.AddCell(pdf1);
                                    }
                                }
                            }
                        }
                        doc.Add(tblLoadingSummary1);
                        doc.Add(new iTextSharp.text.Paragraph("  "));
                        doc.NewPage();
                    }
                    else
                    {
                        for (int rowCounter = 47; rowCounter < rowCount; rowCounter++)
                        {
                            if (rowCounter == 47)
                            {
                                doc.Add(new iTextSharp.text.Paragraph("  "));
                            }
                            // doc.Add(new iTextSharp.text.Paragraph("  "));
                            for (int columnCounter = 0; columnCounter < columnCount; columnCounter++)
                            {
                                string strValue = (dtLoadingSummary1.Rows[rowCounter][columnCounter].ToString());
                                string temp;
                                string strDmage = (dtLoadingSummary1.Rows[rowCounter][11].ToString());
                                if (columnCounter != 1 && columnCounter != 2)
                                {
                                    if (rowCounter == rowCount - 1 || rowCounter == rowCount - 2 || rowCounter == rowCount - 3)
                                    {
                                        if (columnCounter == 0)
                                        {
                                            try
                                            {
                                                PdfPCell pdf1;
                                                pdf1 = new PdfPCell(new iTextSharp.text.Paragraph(strValue, FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                                                pdf1.HorizontalAlignment = Element.ALIGN_CENTER;
                                                pdf1.Colspan = 4;
                                                tblLoadingSummary1.AddCell(pdf1);
                                            }
                                            catch
                                            {
                                            }
                                        }

                                        else if (columnCounter == 6 || columnCounter == 7 || columnCounter == 8 || columnCounter == 9 || columnCounter == 10)
                                        {
                                            decimal d = Convert.ToDecimal(strValue);
                                            temp = Convert.ToString(Math.Round(d, 3));
                                            PdfPCell pdf = new PdfPCell(new iTextSharp.text.Paragraph(temp, fntBody));
                                            pdf.HorizontalAlignment = Element.ALIGN_CENTER;
                                            tblLoadingSummary1.AddCell(pdf);
                                        }
                                        else if (columnCounter == 11)
                                        {
                                            PdfPCell pdf1 = new PdfPCell(new iTextSharp.text.Paragraph(" ", fntBody));
                                            pdf1.HorizontalAlignment = Element.ALIGN_CENTER;
                                            tblLoadingSummary1.AddCell(pdf1);
                                        }
                                    }
                                    else if (columnCounter == 3 || columnCounter == 4 || columnCounter == 5 || columnCounter == 6 || columnCounter == 7 || columnCounter == 8 || columnCounter == 9 || columnCounter == 10 && rowCounter != rowCount - 1 && rowCounter != rowCount - 2 && rowCounter != rowCount - 3)
                                    {
                                        PdfPCell pdf;
                                        decimal d = Convert.ToDecimal(strValue);
                                        temp = Convert.ToString(Math.Round(d, 3));
                                        if (temp == Convert.ToString(0))
                                        {
                                            pdf = new PdfPCell(new iTextSharp.text.Paragraph(" ", fntBody));
                                        }
                                        else
                                        {
                                            //pdf = new PdfPCell(new iTextSharp.text.Paragraph(temp, fntBody));
                                            if (strDmage == "Damaged")
                                            {
                                                pdf = new PdfPCell(new iTextSharp.text.Paragraph(" ", fntRedColor1));
                                                pdf.BackgroundColor = new iTextSharp.text.Color(214, 108, 105);
                                            }
                                            else
                                            {
                                                pdf = new PdfPCell(new iTextSharp.text.Paragraph(temp, fntBody));
                                            }
                                        }
                                        pdf.HorizontalAlignment = Element.ALIGN_CENTER;
                                        tblLoadingSummary1.AddCell(pdf);
                                    }

                                    else
                                    {
                                        PdfPCell pdf1;
                                        temp = Convert.ToString(strValue);
                                        if (temp == Convert.ToString(0))
                                        {
                                            pdf1 = new PdfPCell(new iTextSharp.text.Paragraph(" ", fntBody));
                                        }
                                        else
                                        {
                                            //pdf1 = new PdfPCell(new iTextSharp.text.Paragraph(temp, fntBody));

                                            {
                                                if (strValue == "Damaged")
                                                {
                                                    iTextSharp.text.Font fntBody1 = FontFactory.GetFont("Times New Roman", 6.5f, iTextSharp.text.Color.RED);
                                                    pdf1 = new PdfPCell(new iTextSharp.text.Paragraph(temp, fntBody1));
                                                }
                                                else
                                                {
                                                    pdf1 = new PdfPCell(new iTextSharp.text.Paragraph(temp, fntBody));
                                                }
                                            }

                                        }
                                        if (columnCounter == 0)
                                        {
                                            pdf1.HorizontalAlignment = Element.ALIGN_LEFT;
                                        }
                                        else
                                        {
                                            pdf1.HorizontalAlignment = Element.ALIGN_CENTER;
                                        }
                                        tblLoadingSummary1.AddCell(pdf1);
                                    }
                                }
                            }
                        }
                        doc.Add(tblLoadingSummary1);
                        doc.Add(new iTextSharp.text.Paragraph("  "));
                        doc.NewPage();
                    }
                }
                    
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);

                }
                //...........EndOfloadingsummarylast 11 lines.......................
                try
                {

                    doc.Add(new iTextSharp.text.Paragraph("  "));
                    PdfPTable tblLoadingSummary2 = new PdfPTable(10);
                    tblLoadingSummary2.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    tblLoadingSummary2.WidthPercentage = 90;
                    float[] widthsLoading2 = new float[] { 2.5f, 1.8f, 1.8f, 1.2f, 1.2f, 1.2f, 1.2f, 1.2f, 1.5f, 1f };
                    iTextSharp.text.Font fntRedColor1 = FontFactory.GetFont("Times New Roman", 1, iTextSharp.text.Color.ORANGE);
                    tblLoadingSummary2.SetWidths(widthsLoading2);
                    tblLoadingSummary2.HorizontalAlignment = Element.ALIGN_CENTER;
                    PdfPCell tankName2 = new PdfPCell(new Phrase("Tank Name", fntHeader));
                    tankName2.HorizontalAlignment = Element.ALIGN_CENTER;
                    PdfPCell tankStatus2 = new PdfPCell(new Phrase("Status", fntHeader));
                    tankStatus2.HorizontalAlignment = Element.ALIGN_CENTER;
                    PdfPCell percentFull2 = new PdfPCell(new Phrase("Percent Fill", fntHeader));
                    percentFull2.HorizontalAlignment = Element.ALIGN_CENTER;
                    PdfPCell Volume2 = new PdfPCell(new Phrase("Volume(cu.m)", fntHeader));
                    Volume2.HorizontalAlignment = Element.ALIGN_CENTER;
                    PdfPCell sg2 = new PdfPCell(new Phrase("SG", fntHeader));
                    sg2.HorizontalAlignment = Element.ALIGN_CENTER;
                    PdfPCell weight2 = new PdfPCell(new Phrase("Weight(T)", fntHeader));
                    weight2.HorizontalAlignment = Element.ALIGN_CENTER;
                    PdfPCell lcg2 = new PdfPCell(new Phrase("LCG(m)", fntHeader));
                    lcg2.HorizontalAlignment = Element.ALIGN_CENTER;
                    PdfPCell tcg12 = new PdfPCell(new Phrase("TCG(m)", fntHeader));
                    tcg12.HorizontalAlignment = Element.ALIGN_CENTER;
                    PdfPCell vcg2 = new PdfPCell(new Phrase("VCG(m)", fntHeader));
                    vcg2.HorizontalAlignment = Element.ALIGN_CENTER;
                    PdfPCell fsm2 = new PdfPCell(new Phrase("FSM(T-m)", fntHeader));
                    fsm2.HorizontalAlignment = Element.ALIGN_CENTER;
                    tblLoadingSummary2.AddCell(tankName2);
                    tblLoadingSummary2.AddCell(percentFull2);
                    tblLoadingSummary2.AddCell(Volume2);
                    tblLoadingSummary2.AddCell(sg2);
                    tblLoadingSummary2.AddCell(weight2);
                    tblLoadingSummary2.AddCell(lcg2);
                    tblLoadingSummary2.AddCell(tcg12);
                    tblLoadingSummary2.AddCell(vcg2);
                    tblLoadingSummary2.AddCell(fsm2);
                    tblLoadingSummary2.AddCell(tankStatus2);
                    DataTable dtLoadingSummary2 = ((DataView)dgLoadingSummary.ItemsSource).ToTable().Clone();
                    foreach (DataRow dr in ((DataView)dgLoadingSummary.ItemsSource).ToTable().Rows)
                    {
                        dtLoadingSummary2.Rows.Add(dr.ItemArray);
                    }
                    dtLoadingSummary2.Columns.Remove("IsDamaged");
                    dtLoadingSummary2.Columns.Add("IsDamaged", typeof(string));

                    dtLoadingSummary2.Columns.Remove("Tank_ID");
                    for (int i = 0; i < ((DataView)dgLoadingSummary.ItemsSource).ToTable().Rows.Count; i++)
                    {
                        if (((DataView)dgLoadingSummary.ItemsSource).ToTable().Rows[i]["IsDamaged"].ToString() == "True")
                        {
                            dtLoadingSummary2.Rows[i]["IsDamaged"] = "Damaged";
                        }
                        else if (Models.BO.clsGlobVar.Mode != "Real")
                        {
                            if (((DataView)test1corr.ItemsSource).ToTable().Rows[i]["status"].ToString() == "2")
                            {
                                dtLoadingSummary2.Rows[i]["IsDamaged"] = "Flood";
                            }


                            else
                            {

                                dtLoadingSummary2.Rows[i]["IsDamaged"] = "Intact";

                            }
                        }
                        else
                        {

                            dtLoadingSummary2.Rows[i]["IsDamaged"] = "Intact";

                        }
                    }
                    dtLoadingSummary2.Columns["IsDamaged"].ReadOnly = true;
                    columnCount = dtLoadingSummary2.Columns.Count;
                    rowCount = dtLoadingSummary2.Rows.Count;
                    if (rowCount > 90)
                    {
                        for (int rowCounter = 91; rowCounter < rowCount; rowCounter++)
                        {
                            if (rowCounter == 91)
                            {
                                doc.Add(new iTextSharp.text.Paragraph("  "));
                            }
                            // doc.Add(new iTextSharp.text.Paragraph("  "));
                            for (int columnCounter = 0; columnCounter < columnCount; columnCounter++)
                            {
                                string strValue = (dtLoadingSummary2.Rows[rowCounter][columnCounter].ToString());
                                string temp;
                                string strDmage = (dtLoadingSummary2.Rows[rowCounter][11].ToString());
                                if (columnCounter != 1 && columnCounter != 2)
                                {
                                    if (rowCounter == rowCount - 1 || rowCounter == rowCount - 2 || rowCounter == rowCount - 3)
                                    {
                                        if (columnCounter == 0)
                                        {
                                            try
                                            {
                                                PdfPCell pdf1;
                                                pdf1 = new PdfPCell(new iTextSharp.text.Paragraph(strValue, FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                                                pdf1.HorizontalAlignment = Element.ALIGN_CENTER;
                                                pdf1.Colspan = 4;
                                                tblLoadingSummary2.AddCell(pdf1);
                                            }
                                            catch
                                            {
                                            }
                                        }

                                        else if (columnCounter == 6 || columnCounter == 7 || columnCounter == 8 || columnCounter == 9 || columnCounter == 10)
                                        {
                                            decimal d = Convert.ToDecimal(strValue);
                                            temp = Convert.ToString(Math.Round(d, 3));
                                            PdfPCell pdf = new PdfPCell(new iTextSharp.text.Paragraph(temp, fntBody));
                                            pdf.HorizontalAlignment = Element.ALIGN_CENTER;
                                            tblLoadingSummary2.AddCell(pdf);
                                        }
                                        else if (columnCounter == 11)
                                        {
                                            PdfPCell pdf1 = new PdfPCell(new iTextSharp.text.Paragraph(" ", fntBody));
                                            pdf1.HorizontalAlignment = Element.ALIGN_CENTER;
                                            tblLoadingSummary2.AddCell(pdf1);
                                        }
                                    }
                                    else if (columnCounter == 3 || columnCounter == 4 || columnCounter == 5 || columnCounter == 6 || columnCounter == 7 || columnCounter == 8 || columnCounter == 9 || columnCounter == 10 && rowCounter != rowCount - 1 && rowCounter != rowCount - 2 && rowCounter != rowCount - 3)
                                    {
                                        PdfPCell pdf;
                                        decimal d = Convert.ToDecimal(strValue);
                                        temp = Convert.ToString(Math.Round(d, 3));
                                        if (temp == Convert.ToString(0))
                                        {
                                            pdf = new PdfPCell(new iTextSharp.text.Paragraph(" ", fntBody));
                                        }
                                        else
                                        {
                                            //pdf = new PdfPCell(new iTextSharp.text.Paragraph(temp, fntBody));
                                            if (strDmage == "Damaged")
                                            {
                                                pdf = new PdfPCell(new iTextSharp.text.Paragraph(" ", fntRedColor1));
                                                pdf.BackgroundColor = new iTextSharp.text.Color(214, 108, 105);
                                            }
                                            else
                                            {
                                                pdf = new PdfPCell(new iTextSharp.text.Paragraph(temp, fntBody));
                                            }
                                        }
                                        pdf.HorizontalAlignment = Element.ALIGN_CENTER;
                                        tblLoadingSummary2.AddCell(pdf);
                                    }

                                    else
                                    {
                                        PdfPCell pdf1;
                                        temp = Convert.ToString(strValue);
                                        if (temp == Convert.ToString(0))
                                        {
                                            pdf1 = new PdfPCell(new iTextSharp.text.Paragraph(" ", fntBody));
                                        }
                                        else
                                        {
                                            //pdf1 = new PdfPCell(new iTextSharp.text.Paragraph(temp, fntBody));

                                            {
                                                if (strValue == "Damaged")
                                                {
                                                    iTextSharp.text.Font fntBody1 = FontFactory.GetFont("Times New Roman", 6.5f, iTextSharp.text.Color.RED);
                                                    pdf1 = new PdfPCell(new iTextSharp.text.Paragraph(temp, fntBody1));
                                                }
                                                else
                                                {
                                                    pdf1 = new PdfPCell(new iTextSharp.text.Paragraph(temp, fntBody));
                                                }
                                            }

                                        }
                                        if (columnCounter == 0)
                                        {
                                            pdf1.HorizontalAlignment = Element.ALIGN_LEFT;
                                        }
                                        else
                                        {
                                            pdf1.HorizontalAlignment = Element.ALIGN_CENTER;
                                        }
                                        tblLoadingSummary2.AddCell(pdf1);
                                    }
                                }
                            }
                        }
                        doc.Add(tblLoadingSummary2);
                        doc.Add(new iTextSharp.text.Paragraph("  "));
                        doc.NewPage();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);

                }
            


                //..................StartOfHydrostatics1.........................................
                doc.Add(new iTextSharp.text.Paragraph("  "));
                iTextSharp.text.Paragraph pHeaderHydro = new iTextSharp.text.Paragraph("Hydrostatics", FontFactory.GetFont(FontFactory.TIMES, 12, iTextSharp.text.Font.BOLD)); // validation if data not available
                pHeaderHydro.Alignment = Element.ALIGN_CENTER;
                doc.Add(pHeaderHydro);
                doc.Add(new iTextSharp.text.Paragraph("  "));
                PdfPTable tblHydrostatic = new PdfPTable(11);
                tblHydrostatic.WidthPercentage = 95;
                float[] widthsHydro = new float[] { 1.2f, 0.6f, 0.6f, 0.6f, 0.6f, 0.6f, 0.6f, 0.6f, 0.6f, 0.6f,0.6f };
                tblHydrostatic.SetWidths(widthsHydro);

                PdfPCell Displacement = new PdfPCell(new Phrase("Displacement (T)", fntHeader));
                PdfPCell lcb = new PdfPCell(new Phrase("LCB (M)", fntHeader));
                PdfPCell tcb = new PdfPCell(new Phrase("TCB (M)", fntHeader));
                PdfPCell vcb = new PdfPCell(new Phrase("VCB (M)", fntHeader));
                PdfPCell LCF = new PdfPCell(new Phrase("LCF (M)", fntHeader));
                PdfPCell tcf = new PdfPCell(new Phrase("TCF (M)", fntHeader));
                PdfPCell Hydrotcg1 = new PdfPCell(new Phrase("TCG (M)", fntHeader));
                PdfPCell kmt = new PdfPCell(new Phrase("KMT (M)", fntHeader));
                PdfPCell kml = new PdfPCell(new Phrase("KML (M)", fntHeader));
                PdfPCell bmt = new PdfPCell(new Phrase("BMT (M)", fntHeader));
                PdfPCell BML = new PdfPCell(new Phrase("BML (M)", fntHeader));

                Displacement.HorizontalAlignment = Element.ALIGN_CENTER;
                lcb.HorizontalAlignment = Element.ALIGN_CENTER;
                tcb.HorizontalAlignment = Element.ALIGN_CENTER;
                vcb.HorizontalAlignment = Element.ALIGN_CENTER;
                tcf.HorizontalAlignment = Element.ALIGN_CENTER;
                Hydrotcg1.HorizontalAlignment = Element.ALIGN_CENTER;
                LCF.HorizontalAlignment = Element.ALIGN_CENTER;
                kmt.HorizontalAlignment = Element.ALIGN_CENTER;
                kml.HorizontalAlignment = Element.ALIGN_CENTER;
                bmt.HorizontalAlignment = Element.ALIGN_CENTER;
                BML.HorizontalAlignment = Element.ALIGN_CENTER;

                tblHydrostatic.AddCell(Displacement);
                tblHydrostatic.AddCell(lcb);
                tblHydrostatic.AddCell(tcb);
                tblHydrostatic.AddCell(vcb);
                tblHydrostatic.AddCell(LCF);
                tblHydrostatic.AddCell(tcf);
                tblHydrostatic.AddCell(Hydrotcg1);
                tblHydrostatic.AddCell(kmt);
                tblHydrostatic.AddCell(kml);
                tblHydrostatic.AddCell(bmt);
                tblHydrostatic.AddCell(BML);
                DataTable dtHydrostaticsData = new DataTable();
                dtHydrostaticsData = ((DataView)dgHydrostatics.ItemsSource).ToTable();
                int columnCountHydro = dtHydrostaticsData.Columns.Count;
                int rowCountHydro = dtHydrostaticsData.Rows.Count;
                for (int rowCounter = 0; rowCounter < rowCountHydro; rowCounter++)
                {
                    for (int columnCounter = 0; columnCounter < columnCountHydro; columnCounter++)
                    {
                        if ((columnCounter == 0) || (columnCounter == 1) || (columnCounter == 2) || (columnCounter == 3) || (columnCounter == 4) || (columnCounter == 5) || (columnCounter == 7) || (columnCounter == 9) || (columnCounter == 10) || (columnCounter == 11) || (columnCounter == 12))
                        {
                            string strValue = (dtHydrostaticsData.Rows[rowCounter][columnCounter].ToString());
                            PdfPCell pdf = new PdfPCell(new iTextSharp.text.Paragraph(strValue, fntBody));
                            pdf.HorizontalAlignment = Element.ALIGN_CENTER;
                            tblHydrostatic.AddCell(pdf);
                        }
                      
                    }
                }
                doc.Add(tblHydrostatic);
                doc.Add(new iTextSharp.text.Paragraph("  "));
                //..................EndOfHydrostatics1.........................................

                //..................StartOfHydrostatics2.........................................
                PdfPTable tblHydrostatic1 = new PdfPTable(10);
                tblHydrostatic1.WidthPercentage = 95;
                //float[] widthsHydro1 = new float[] { 1.5f, 1.5f, 0.8f, 0.8f, 0.8f, 0.8f, 1f, 1f, 1.6f };
                float[] widthsHydro1 = new float[] { 1.2f, 1.3f, 0.7f, 0.7f, 0.7f, 0.7f, 1f, 1f, 1.4f,1f };
                tblHydrostatic1.SetWidths(widthsHydro1);
                DataTable dtHydrostaticsData1 = new DataTable();
                dtHydrostaticsData1 = ((DataView)dgHydrostatics2.ItemsSource).ToTable();
                int columnCountHydro1 = dtHydrostaticsData1.Columns.Count;
                int rowCountHydro1 = dtHydrostaticsData1.Rows.Count;
                string strValue111 =   dtHydrostaticsData1.Rows[0][0].ToString();
                float strValue11 = float.Parse(strValue111);
                string strValue222 =  dtHydrostaticsData1.Rows[0][1].ToString() ;
                float strValue22 = float.Parse(strValue222);
                PdfPCell TRIM;
                PdfPCell LST;
                if (Convert.ToInt16(strValue11) < 0)
                {
                     TRIM = new PdfPCell(new Phrase("TRIM (M)-AFT", fntHeader));
                }
                else
                {
                     TRIM = new PdfPCell(new Phrase("TRIM(M)-FWD", fntHeader));
                }
                if (Convert.ToInt16(strValue22) > 0)
                {
                    LST = new PdfPCell(new Phrase("LIST(deg.)-STBD", fntHeader));
                }
                else
                {
                     LST = new PdfPCell(new Phrase("LIST(deg.)-PORT", fntHeader));
                }

                
                PdfPCell GMT = new PdfPCell(new Phrase("GMT", fntHeader));
                PdfPCell KG = new PdfPCell(new Phrase("KG (M)", fntHeader));
                PdfPCell KGF1 = new PdfPCell(new Phrase("KGF (M)", fntHeader));
                PdfPCell LCG = new PdfPCell(new Phrase("LCG (M)", fntHeader));
                PdfPCell FSC = new PdfPCell(new Phrase("FSC (M)", fntHeader));
                PdfPCell TPC = new PdfPCell(new Phrase("TPC (T/CM)", fntHeader));
                PdfPCell MTC = new PdfPCell(new Phrase("MTC (T-M/CM)", fntHeader));
                PdfPCell WPA = new PdfPCell(new Phrase("WPA (M²)", fntHeader));
                TRIM.HorizontalAlignment = Element.ALIGN_CENTER;
                LST.HorizontalAlignment = Element.ALIGN_CENTER;
                GMT.HorizontalAlignment = Element.ALIGN_CENTER;
                KG.HorizontalAlignment = Element.ALIGN_CENTER;
                KGF1.HorizontalAlignment = Element.ALIGN_CENTER;
                LCG.HorizontalAlignment = Element.ALIGN_CENTER;
                FSC.HorizontalAlignment = Element.ALIGN_CENTER;
                TPC.HorizontalAlignment = Element.ALIGN_CENTER;
                MTC.HorizontalAlignment = Element.ALIGN_CENTER;
                WPA.HorizontalAlignment = Element.ALIGN_CENTER;
               
                tblHydrostatic1.AddCell(TRIM);
                tblHydrostatic1.AddCell(LST);
                tblHydrostatic1.AddCell(GMT);
                tblHydrostatic1.AddCell(KG);
                tblHydrostatic1.AddCell(KGF1);
                tblHydrostatic1.AddCell(LCG);
                tblHydrostatic1.AddCell(FSC);
                tblHydrostatic1.AddCell(TPC);
                tblHydrostatic1.AddCell(MTC);
                tblHydrostatic1.AddCell(WPA);
               
                for (int rowCounter = 0; rowCounter < rowCountHydro1; rowCounter++)
                {
                    for (int columnCounter = 0; columnCounter < columnCountHydro1; columnCounter++)
                    {
                        PdfPCell pdf;
                        string strValue = dtHydrostaticsData1.Rows[rowCounter][columnCounter].ToString();
                        decimal value = Convert.ToDecimal(strValue);
                        string strvalueNew = "";
                        if (((columnCounter == 0) || (columnCounter == 1)) && (value < 0))
                        {
                            string dd = value.ToString("N");
                            decimal mm = Convert.ToDecimal(dd);
                            strvalueNew = Convert.ToString(Convert.ToDecimal(mm) * (-1));
                            pdf = new PdfPCell(new iTextSharp.text.Paragraph(strvalueNew, fntBody));
                        }
                        else
                        {
                            pdf = new PdfPCell(new iTextSharp.text.Paragraph(strValue, fntBody));
                        }
                        pdf.HorizontalAlignment = Element.ALIGN_CENTER;
                        tblHydrostatic1.AddCell(pdf);
                    }
                }
                doc.Add(tblHydrostatic1);
                doc.Add(new iTextSharp.text.Paragraph("  "));

                //..................EndOfHydrostatics2.........................................

                // .................startOfDrafts.....................................................
                //............Start of Moulded Draft....................................

                iTextSharp.text.Paragraph pHeaderMouldedDraft = new iTextSharp.text.Paragraph("Moulded Draft", FontFactory.GetFont(FontFactory.TIMES, 12, iTextSharp.text.Font.BOLD)); // validation if data not available
                pHeaderMouldedDraft.Alignment = Element.ALIGN_CENTER;
                doc.Add(pHeaderMouldedDraft);
                doc.Add(new iTextSharp.text.Paragraph("  "));
                iTextSharp.text.Font fntHeader_md = FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Color.BLUE);
                PdfPTable tblMouldedDraft = new PdfPTable(5);
                tblMouldedDraft.WidthPercentage = 90;
                float[] widthMouldedDraft = new float[] { 1.2f, 1.2f, 1.2f, 1.5f,1.2f };
                tblMouldedDraft.SetWidths(widthMouldedDraft);

                PdfPCell moulded_ap = new PdfPCell(new Phrase("AP(M)", fntHeader));
               //PdfPCell moulded_propellr = new PdfPCell(new Phrase("PROPELLER(M)", fntHeader_md));
                PdfPCell moulded_aftmark = new PdfPCell(new Phrase("AFT MARK(M)", fntHeader_md));
                PdfPCell moulded_mid = new PdfPCell(new Phrase("MID(M)", fntHeader));
                PdfPCell moulded_fwdmark = new PdfPCell(new Phrase("FWD MARK(M)", fntHeader_md));
              //  PdfPCell moulded_sonar = new PdfPCell(new Phrase("SONAR DOME(M)", fntHeader_md));
                PdfPCell moulded_fp = new PdfPCell(new Phrase("FP(M)", fntHeader));

                moulded_ap.HorizontalAlignment = Element.ALIGN_CENTER;
               // moulded_propellr.HorizontalAlignment = Element.ALIGN_CENTER;
                moulded_aftmark.HorizontalAlignment = Element.ALIGN_CENTER;
                moulded_mid.HorizontalAlignment = Element.ALIGN_CENTER;
                moulded_fwdmark.HorizontalAlignment = Element.ALIGN_CENTER;
               // moulded_sonar.HorizontalAlignment = Element.ALIGN_CENTER;
                moulded_fp.HorizontalAlignment = Element.ALIGN_CENTER;

                tblMouldedDraft.AddCell(moulded_ap);
               // tblMouldedDraft.AddCell(moulded_propellr);
                tblMouldedDraft.AddCell(moulded_aftmark);
                tblMouldedDraft.AddCell(moulded_mid);
                tblMouldedDraft.AddCell(moulded_fwdmark);
               // tblMouldedDraft.AddCell(moulded_sonar);
                tblMouldedDraft.AddCell(moulded_fp);

                //PdfPCell moulded_mid = new PdfPCell(new Phrase("MID(M)", fntHeader));
                //PdfPCell moulded_ap = new PdfPCell(new Phrase("AP(M)", fntHeader));
                //PdfPCell moulded_fp = new PdfPCell(new Phrase("FP(M)", fntHeader));
                //PdfPCell aftmrk = new PdfPCell(new Phrase("AFT MARK(M)", fntHeader_md));
                //PdfPCell moulded_fwdmark = new PdfPCell(new Phrase("FWD MARK(M)", fntHeader_md));
                //PdfPCell propeller = new PdfPCell(new Phrase("PROPELLER(M)", fntHeader_md));
                //PdfPCell sonardome = new PdfPCell(new Phrase("SONAR DOME(M)", fntHeader_md));
               

                //moulded_ap.HorizontalAlignment = Element.ALIGN_CENTER;
                //propeller.HorizontalAlignment = Element.ALIGN_CENTER;
                //aftmrk.HorizontalAlignment = Element.ALIGN_CENTER;
                //moulded_mid.HorizontalAlignment = Element.ALIGN_CENTER;
                //moulded_fwdmark.HorizontalAlignment = Element.ALIGN_CENTER;
                //sonardome.HorizontalAlignment = Element.ALIGN_CENTER;
                //moulded_fp.HorizontalAlignment = Element.ALIGN_CENTER;

                //tblMouldedDraft.AddCell(moulded_mid);
                //tblMouldedDraft.AddCell(moulded_ap);
                //tblMouldedDraft.AddCell(moulded_fp);
                //tblMouldedDraft.AddCell(aftmrk);
                //tblMouldedDraft.AddCell(moulded_fwdmark);
                //tblMouldedDraft.AddCell(propeller);
                //tblMouldedDraft.AddCell(sonardome);
                

                DataTable dtMouldedDraft = new DataTable();
                dtMouldedDraft = ((DataView)dgMouldedDraft.ItemsSource).ToTable();
                int columnCountMouldedDraft = dtMouldedDraft.Columns.Count;
                int rowCountMouldedDraft = dtMouldedDraft.Rows.Count;
                for (int rowCounter = 0; rowCounter < rowCountMouldedDraft; rowCounter++)
                {
                    for (int columnCounter = 0; columnCounter < columnCountMouldedDraft; columnCounter++)
                    {
                        string strValue = (dtMouldedDraft.Rows[rowCounter][columnCounter].ToString());
                        PdfPCell pdf = new PdfPCell(new iTextSharp.text.Paragraph(strValue, fntBody));
                        pdf.HorizontalAlignment = Element.ALIGN_CENTER;
                        tblMouldedDraft.AddCell(pdf);
                    }
                }
                doc.Add(tblMouldedDraft);
                //doc.Add(new iTextSharp.text.Paragraph("  "));
                //............End of Moulded Draft....................................
                // .................start Of Extreme Drafts.....................................................
                iTextSharp.text.Paragraph pHeaderExDraft = new iTextSharp.text.Paragraph("Extreme Draft", FontFactory.GetFont(FontFactory.TIMES, 12, iTextSharp.text.Font.BOLD)); // validation if data not available
                pHeaderExDraft.Alignment = Element.ALIGN_CENTER;
                doc.Add(pHeaderExDraft);
                doc.Add(new iTextSharp.text.Paragraph("  "));
                iTextSharp.text.Font fntHeader1 = FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Color.BLUE);
                PdfPTable tblDrafts = new PdfPTable(5);
                tblDrafts.WidthPercentage = 90;
                float[] widthsHydro2 = new float[] {1.2f, 1.2f, 1.2f, 1.5f,1.2f };
                tblDrafts.SetWidths(widthsHydro2);

                PdfPCell ap = new PdfPCell(new Phrase("AP(M)", fntHeader));
              //  PdfPCell propellr = new PdfPCell(new Phrase("PROPELLER(M)", fntHeader1));
                PdfPCell aftmark = new PdfPCell(new Phrase("AFT MARK(M)", fntHeader1));
                PdfPCell mid = new PdfPCell(new Phrase("MID(M)", fntHeader));
                PdfPCell fwdmark = new PdfPCell(new Phrase("FWD MARK(M)", fntHeader1));
                //PdfPCell sonar = new PdfPCell(new Phrase("SONAR DOME(M)", fntHeader1));
                PdfPCell fp = new PdfPCell(new Phrase("FP(M)", fntHeader));

                ap.HorizontalAlignment = Element.ALIGN_CENTER;
               // propellr.HorizontalAlignment = Element.ALIGN_CENTER;
                aftmark.HorizontalAlignment = Element.ALIGN_CENTER;
                mid.HorizontalAlignment = Element.ALIGN_CENTER;
                fwdmark.HorizontalAlignment = Element.ALIGN_CENTER;
              //  sonar.HorizontalAlignment = Element.ALIGN_CENTER;
                fp.HorizontalAlignment = Element.ALIGN_CENTER;

                tblDrafts.AddCell(ap);
               // tblDrafts.AddCell(propellr);
                tblDrafts.AddCell(aftmark);
                tblDrafts.AddCell(mid);
                tblDrafts.AddCell(fwdmark);
             //   tblDrafts.AddCell(sonar);
                tblDrafts.AddCell(fp);

                DataTable dtHydroDrafts = new DataTable();
                dtHydroDrafts = ((DataView)dgDraft1.ItemsSource).ToTable();
                int columnCountHydro2 = dtHydroDrafts.Columns.Count;
                int rowCountHydro2 = dtHydroDrafts.Rows.Count;
                for (int rowCounter = 0; rowCounter < rowCountHydro2; rowCounter++)
                {
                    for (int columnCounter = 0; columnCounter < columnCountHydro2; columnCounter++)
                    {

                        string strValue = (dtHydroDrafts.Rows[rowCounter][columnCounter].ToString());
                        PdfPCell pdf = new PdfPCell(new iTextSharp.text.Paragraph(strValue, fntBody));
                        pdf.HorizontalAlignment = Element.ALIGN_CENTER;
                        tblDrafts.AddCell(pdf);
                    }
                }
                doc.Add(tblDrafts);
                doc.Add(new iTextSharp.text.Paragraph("  "));
                // .................End Of Extreme Drafts.....................................................
                // .................EndOfDrafts.....................................................


                //............StartofImersion Particulars....................................

               // iTextSharp.text.Paragraph pHeaderDraft = new iTextSharp.text.Paragraph("Immersion Particulars", FontFactory.GetFont(FontFactory.TIMES, 12, iTextSharp.text.Font.BOLD)); // validation if data not available
               // pHeaderDraft.Alignment = Element.ALIGN_CENTER;
               // doc.Add(pHeaderDraft);
               // doc.Add(new iTextSharp.text.Paragraph("  "));
               // PdfPTable tblDraft = new PdfPTable(6);
               // tblDraft.WidthPercentage = 90;
               // float[] widthsDraft = new float[] { 1f, 2.5f, 0.6f, 0.6f, 0.6f, 1.6f};
               // tblDraft.SetWidths(widthsDraft);

               // PdfPCell name = new PdfPCell(new Phrase("NAME", fntHeader));
               // PdfPCell type = new PdfPCell(new Phrase("TYPE", fntHeader));
               // PdfPCell xmtr = new PdfPCell(new Phrase("X(M)", fntHeader));
               // PdfPCell ymtr = new PdfPCell(new Phrase("Y(M)", fntHeader));
               // PdfPCell zmtr = new PdfPCell(new Phrase("Z(M)", fntHeader));
               // PdfPCell floodangle = new PdfPCell(new Phrase("FLOOD ANGLE(DEG.)", fntHeader));
               // name.HorizontalAlignment = Element.ALIGN_CENTER;
               // type.HorizontalAlignment = Element.ALIGN_CENTER;
               // xmtr.HorizontalAlignment = Element.ALIGN_CENTER;
               // ymtr.HorizontalAlignment = Element.ALIGN_CENTER;
               // zmtr.HorizontalAlignment = Element.ALIGN_CENTER;
               // floodangle.HorizontalAlignment = Element.ALIGN_CENTER;

               // tblDraft.AddCell(name);
               // tblDraft.AddCell(type);
               // tblDraft.AddCell(xmtr);
               // tblDraft.AddCell(ymtr);
               // tblDraft.AddCell(zmtr);
               // tblDraft.AddCell(floodangle);
               // DataTable dtDraftData = new DataTable();
               // dtDraftData = ((DataView)dgDraft.ItemsSource).ToTable();
               // int columnCountDraft = dtDraftData.Columns.Count;
               // int rowCountDraft = dtDraftData.Rows.Count;
               // for (int rowCounter = 0; rowCounter < rowCountDraft; rowCounter++)
               // {
               //     for (int columnCounter = 0; columnCounter < columnCountDraft; columnCounter++)
               //     {
               //         string strValue = (dtDraftData.Rows[rowCounter][columnCounter].ToString());
               //         PdfPCell pdf = new PdfPCell(new iTextSharp.text.Paragraph(strValue, fntBody));
               //         pdf.HorizontalAlignment = Element.ALIGN_CENTER;
               //         tblDraft.AddCell(pdf);
               //     }
               // }
               // doc.Add(tblDraft);
               //doc.Add(new iTextSharp.text.Paragraph("  "));
                //............EndofImersion Particulars....................................


              // doc.NewPage();//@01082017

                //............StartOfIntactStability Or Damage StabilityP15B Criteria.................
                PdfPTable tblIntact = new PdfPTable(4);
                tblIntact.WidthPercentage = 50;
                float[] widthsIntact = new float[] { 6f, 1.4f, 1.4f, 1f };
                tblIntact.SetWidths(widthsIntact);

                PdfPCell criterion = new PdfPCell(new Phrase("Criterion", fntHeader));
                PdfPCell criticalValue = new PdfPCell(new Phrase("Critical Value", fntHeader));
                PdfPCell actualvalue = new PdfPCell(new Phrase("Actual Value", fntHeader));
                PdfPCell status = new PdfPCell(new Phrase("Status", fntHeader));
                criterion.HorizontalAlignment = Element.ALIGN_CENTER;
                criticalValue.HorizontalAlignment = Element.ALIGN_CENTER;
                actualvalue.HorizontalAlignment = Element.ALIGN_CENTER;
                status.HorizontalAlignment = Element.ALIGN_CENTER;
                tblIntact.AddCell(criterion);
                tblIntact.AddCell(criticalValue);
                tblIntact.AddCell(actualvalue);
                tblIntact.AddCell(status);
                DataTable dtfinal = new DataTable();
               
                {
                    dtfinal = ((DataView)dgStability.ItemsSource).ToTable().Clone();
                    foreach (DataRow dr in ((DataView)dgStability.ItemsSource).ToTable().Rows)
                    {
                        dtfinal.Rows.Add(dr.ItemArray);
                    }
                    dtfinal.Columns.Remove("Status");
                    dtfinal.Columns.Add("Status", typeof(string));

                    for (int i = 0; i < ((DataView)dgStability.ItemsSource).ToTable().Rows.Count; i++)
                    {
                        if (((DataView)dgStability.ItemsSource).ToTable().Rows[i]["Status"].ToString() == "True")
                        {
                            dtfinal.Rows[i]["Status"] = "Pass";
                        }
                        else
                        {
                            dtfinal.Rows[i]["Status"] = "Fail";
                        }
                    }
                    dtfinal.Columns["Status"].ReadOnly = true;
                }
                int columnCountIntact = dtfinal.Columns.Count;
                int rowCountIntact = dtfinal.Rows.Count;
                for (int rowCounter = 0; rowCounter < rowCountIntact; rowCounter++)
                {
                    for (int columnCounter = 0; columnCounter < columnCountIntact; columnCounter++)
                    {
                        string strValue = dtfinal.Rows[rowCounter][columnCounter].ToString();
                        PdfPCell pdf;
                        if (strValue.ToString() == "Fail")
                        {
                            iTextSharp.text.Font fntBody1 = FontFactory.GetFont("Times New Roman", 6.5f, iTextSharp.text.Color.RED);
                            //pdf = new PdfPCell(new iTextSharp.text.Paragraph(temp, fntBody1));
                            pdf = new PdfPCell(new iTextSharp.text.Paragraph(strValue, fntBody1));

                        }
                        else
                        {
                             pdf = new PdfPCell(new iTextSharp.text.Paragraph(strValue, fntBody));
                        }
                        if (columnCounter == 0)
                        {
                            pdf.HorizontalAlignment = Element.ALIGN_LEFT;
                        }
                        else
                        {
                            pdf.HorizontalAlignment = Element.ALIGN_CENTER;
                        }
                        tblIntact.AddCell(pdf);
                    }
                }
                doc.Add(new iTextSharp.text.Paragraph("  "));
                iTextSharp.text.Paragraph pHeaderIntact = new iTextSharp.text.Paragraph("NES 109 Stability Criteria-" + stabilityType, FontFactory.GetFont(FontFactory.TIMES, 12, iTextSharp.text.Font.BOLD));
                pHeaderIntact.Alignment = Element.ALIGN_CENTER;
                doc.Add(pHeaderIntact);
                doc.Add(new iTextSharp.text.Paragraph("  "));
                doc.Add(tblIntact);


                doc.NewPage();
                //............EndOfIntactStability Or Damage StabilityP15B Criteria.................


                ////.........................StartofGzGraph......................................
                if (stabilityType == "Intact")
                {
                    doc.Add(new iTextSharp.text.Paragraph("  "));
                    iTextSharp.text.Paragraph pHeaderGZ = new iTextSharp.text.Paragraph("GZ Graph", FontFactory.GetFont(FontFactory.TIMES, 12, iTextSharp.text.Font.BOLD)); // validation if data not available
                    pHeaderGZ.Alignment = Element.ALIGN_CENTER;
                    doc.Add(pHeaderGZ);
                    doc.Add(new iTextSharp.text.Paragraph("  "));

                    iTextSharp.text.Image imgGZ = iTextSharp.text.Image.GetInstance(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Images\\GZ.png");
                    imgGZ.Alignment = Element.ALIGN_CENTER;
                    imgGZ.SpacingAfter = 20f;
                    imgGZ.ScaleToFit(400, 300);
                    doc.Add(imgGZ);
                    doc.Add(new iTextSharp.text.Paragraph("  "));
                }
                else if (stabilityType == "Damage")
                {

                }
                ////.........................EndofGzGraph......................................

                //..............................StartOfIntact..................................
                
                if (stabilityType == "Intact")
                {
                    doc.NewPage();
                    doc.Add(new iTextSharp.text.Paragraph("  "));
                    doc.Add(new iTextSharp.text.Paragraph("  "));
                    PdfPTable pp1 = new PdfPTable(6);
                    pp1.WidthPercentage = 60;
                    float[] widthsGZ5 = new float[] { 2f, 2f, 2f, 2f, 2f, 2f };
                    pp1.SetWidths(widthsGZ5);

                    PdfPCell heel = new PdfPCell(new Phrase("Heel(Deg)", fntHeader));
                    PdfPCell GZ = new PdfPCell(new Phrase("GZ(m)", fntHeader));
                    PdfPCell WH = new PdfPCell(new Phrase("Wind Heeling(m)", fntHeader));
                    PdfPCell HL = new PdfPCell(new Phrase("Heavy Weights(m)", fntHeader));
                    PdfPCell HS = new PdfPCell(new Phrase("Turning(m)", fntHeader));
                    PdfPCell PC = new PdfPCell(new Phrase("Crowding(m)", fntHeader));
                    heel.HorizontalAlignment = Element.ALIGN_CENTER;
                    GZ.HorizontalAlignment = Element.ALIGN_CENTER;
                    WH.HorizontalAlignment = Element.ALIGN_CENTER;
                    HL.HorizontalAlignment = Element.ALIGN_CENTER;
                    HS.HorizontalAlignment = Element.ALIGN_CENTER;
                    PC.HorizontalAlignment = Element.ALIGN_CENTER;
                    pp1.AddCell(heel);
                    pp1.AddCell(GZ);
                    pp1.AddCell(WH);
                    pp1.AddCell(HL);
                    pp1.AddCell(HS);
                    pp1.AddCell(PC);
                    DataTable dtGZgraph = new DataTable();
                    dtGZgraph = ((DataView)dgGZ.ItemsSource).ToTable();
                    int columnCountGZ1 = dtGZgraph.Columns.Count;
                    int rowCountGZ1 = dtGZgraph.Rows.Count;
                    for (int rowCounter = 0; rowCounter < rowCountGZ1; rowCounter++)
                    {
                        for (int columnCounter = 0; columnCounter < 6; columnCounter++)
                        {

                            string temp;
                            object obj = dtGZgraph.Rows[rowCounter][columnCounter];
                            string strValue1 = (obj.ToString());
                            decimal d = Convert.ToDecimal(strValue1);
                            temp = Convert.ToString(Math.Round(d, 2));
                            PdfPCell cell1 = new PdfPCell(new Phrase(temp, FontFactory.GetFont("Times New Roman", 7)));
                            cell1.HorizontalAlignment = Element.ALIGN_CENTER;
                            pp1.AddCell(cell1);

                        }
                    }
                    doc.Add(pp1);
                }
                //..............................EndOfDamage..................................

                //..............................StartOfDamage..................................
                else if(stabilityType=="Damage")
                {
                    //PdfPTable pp1 = new PdfPTable(2);
                    //pp1.WidthPercentage = 25;
                    //float[] widthsGZ5 = new float[] { 2f, 2f };
                    //pp1.SetWidths(widthsGZ5);

                    //PdfPCell heel = new PdfPCell(new Phrase("Heel(Deg)", fntHeader));
                    //PdfPCell GZ = new PdfPCell(new Phrase("GZ(m)", fntHeader));
                    //heel.HorizontalAlignment = Element.ALIGN_CENTER;
                    //GZ.HorizontalAlignment = Element.ALIGN_CENTER;
                    //pp1.AddCell(heel);
                    //pp1.AddCell(GZ);
                    //DataTable dtGZgraph = new DataTable();
                    //dtGZgraph = ((DataView)dgGZ.ItemsSource).ToTable();
                    //int columnCountGZ1 = dtGZgraph.Columns.Count;
                    //int rowCountGZ1 = dtGZgraph.Rows.Count;
                    //for (int rowCounter = 0; rowCounter < rowCountGZ1; rowCounter++)
                    //{
                    //    for (int columnCounter = 0; columnCounter <2; columnCounter++)
                    //    {
                    //        string temp;
                    //        object obj = dtGZgraph.Rows[rowCounter][columnCounter];
                    //        string strValue1 = (obj.ToString());
                    //        decimal d = Convert.ToDecimal(strValue1);
                    //        temp = Convert.ToString(Math.Round(d, 2));
                    //        PdfPCell cell1 = new PdfPCell(new Phrase(temp, FontFactory.GetFont("Times New Roman", 7)));
                    //        cell1.HorizontalAlignment = Element.ALIGN_CENTER;
                    //        pp1.AddCell(cell1);
                    //    }
                    //}
                    //doc.Add(pp1);
                }
      //..............................EndOfDamage..................................
                doc.Close();
                Mouse.OverrideCursor = null;
                System.Windows.MessageBox.Show("PDF Created!");
            }
            catch (Exception ex)
            {
              //  System.Windows.MessageBox.Show(ex.Message.ToString());
            }
        }

        #endregion PDFWriter

       

    }
}
