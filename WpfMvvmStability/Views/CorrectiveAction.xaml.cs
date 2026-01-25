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
using WW.Cad.Model.Objects;
using WW.Cad.Drawing;
using WW.Cad.Drawing.Wpf;
using WW.Cad.Base;
using WW.Math;
using WW.Cad.Model;
using System.Data;
using WpfMvvmStability.Models.BO;
using System.IO;
using System.Reflection;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Data.Common;
using System.ComponentModel;
using System.Threading;

namespace WpfMvvmStability.Views
{
    /// <summary>
    /// Interaction logic for CorrectiveAction.xaml
    /// </summary>
    public partial class CorrectiveAction : Page
    {
        private Bounds3D bounds;
        private WpfWireframeGraphics3DUsingDrawingVisual wpfGraphics;
        private WireframeGraphics2Cache graphicsCache;
        private GraphicsConfig graphicsConfig;
        private WW.Math.Vector3D translation;
        private double scaling = 1d;
        List<KeyValuePair<double, double>> GZList = new List<KeyValuePair<double, double>>();
        string CorrectiveReportPath = "";
        private bool isCalculationRunning = false;
        BackgroundWorker bgWorker;
        public CorrectiveAction()
        {
            InitializeComponent();
           
            if (Models.BO.clsGlobVar.Mode == "Real")
            {
                //btnCorrectiveReport.Visibility = Visibility.Hidden;
                ShowCorrectiveRealData();
                Models.TableModel.RealmodeCorrectiveFill();
                
                //GZChart();
              
            }
            if (Models.BO.clsGlobVar.Mode == "Simulation")
            {
                //btnCorrectiveReport.Visibility = Visibility.Visible;
                ShowCorrectiveSimulationData();
                //GZChart();
                
            }
        }

        public void ShowCorrectiveRealData()
        {
            Models.TableModel.RealModeCorrectiveData();

            //if (Convert.ToDouble(Models.BO.clsGlobVar.dtRealCorrectiveEquillibriumValues.Rows[0]["Heel"]) > 0)
            //{
            //    cheelr.Content = "Heel(deg.) STBD";
            //    txtheel.Content = Models.BO.clsGlobVar.dtRealCorrectiveEquillibriumValues.Rows[0]["Heel"].ToString();
            //}
            //else if (Convert.ToDouble(Models.BO.clsGlobVar.dtRealCorrectiveEquillibriumValues.Rows[0]["Heel"]) < 0)
            //{
            //    cheelr.Content = "Heel(deg.) PORT";
            //    Double abs = (Convert.ToDouble(Models.BO.clsGlobVar.dtRealCorrectiveEquillibriumValues.Rows[0]["Heel"]));
            //    txtheel.Content = Convert.ToString((-1) * abs);
            //}
            //else
            //{
            //    cheelr.Content = "Heel(deg.)  ";
            //    txtheel.Content = Models.BO.clsGlobVar.dtRealCorrectiveEquillibriumValues.Rows[0]["Heel"].ToString();
            //}


            //if (Convert.ToDouble(Models.BO.clsGlobVar.dtRealCorrectiveEquillibriumValues.Rows[0]["Trim"]) > 0)
            //{
            //    ctrimr.Content = "Trim(m) AFT";

            //    txttrim.Content = (Convert.ToString(Models.BO.clsGlobVar.dtRealCorrectiveEquillibriumValues.Rows[0]["TRIM"]));
            //}
            //else if (Convert.ToDouble(Models.BO.clsGlobVar.dtRealCorrectiveEquillibriumValues.Rows[0]["Trim"]) < 0)
            //{
            //    ctrimr.Content = "Trim(m) FWD";
            //    Double abs = (Convert.ToDouble(Models.BO.clsGlobVar.dtRealCorrectiveEquillibriumValues.Rows[0]["TRIM"]));
            //    txttrim.Content = Convert.ToString((-1) * abs);
            //}
            //else
            //{
            //    ctrimr.Content = "Trim(m)";
            //    txttrim.Content = (Convert.ToString(Models.BO.clsGlobVar.dtRealCorrectiveEquillibriumValues.Rows[0]["TRIM"]));
            //}


            //dgGZ.ItemsSource = Models.BO.clsGlobVar.dtRealCorrectiveGZ.DefaultView;
            //dgStability.ItemsSource = Models.BO.clsGlobVar.dtRealCorrectiveStabilityCriteriaDamage.DefaultView;

            
 
            //int count = 1;
            //for (int i = 0; i < Models.BO.clsGlobVar.dtRealCorrectiveMeasures.Rows.Count; i++)
            //{
            //    //listBoxCorrectiveMeasures.Items.Add(count+". "+ Models.BO.clsGlobVar.dtRealCorrectiveMeasures.Rows[i]["Measures_Suggested"].ToString());
            //    count++;
            //}

            //for (int i = 0; i < Models.BO.clsGlobVar.dtRealDeFloodingvalues.Rows.Count; i++)
            //{
            //    listBoxDeDlooting.Items.Add(count + ". " + Models.BO.clsGlobVar.dtRealDeFloodingvalues.Rows[i]["Deflooding_Option"].ToString());
            //    count++;
            //}

        }
        public void ShowCorrectiveSimulationData()
        {
            Models.TableModel.SimulationModeCorrectiveData();

            //if (Convert.ToDouble(Models.BO.clsGlobVar.dtSimulationCorrectiveEquillibriumValues.Rows[0]["Heel"]) > 0)
            //{
            //    cheelr.Content = "List(deg.) PORT";
            //    txtheel.Content = Models.BO.clsGlobVar.dtSimulationCorrectiveEquillibriumValues.Rows[0]["Heel"].ToString();
            //}
            //else if (Convert.ToDouble(Models.BO.clsGlobVar.dtSimulationCorrectiveEquillibriumValues.Rows[0]["Heel"]) < 0)
            //{
            //    cheelr.Content = "List(deg.) STBD";
            //    Double abs = (Convert.ToDouble(Models.BO.clsGlobVar.dtSimulationCorrectiveEquillibriumValues.Rows[0]["Heel"]));
            //    txtheel.Content = Convert.ToString((-1) * abs);
            //}
            //else
            //{
            //    cheelr.Content = "List(deg.)  ";
            //    txtheel.Content = Models.BO.clsGlobVar.dtSimulationCorrectiveEquillibriumValues.Rows[0]["Heel"].ToString();
            //}


            //if (Convert.ToDouble(Models.BO.clsGlobVar.dtSimulationCorrectiveEquillibriumValues.Rows[0]["Trim"]) > 0)
            //{
            //    ctrimr.Content = "Trim(m) AFT";

            //    txttrim.Content = (Convert.ToString(Models.BO.clsGlobVar.dtSimulationCorrectiveEquillibriumValues.Rows[0]["TRIM"]));
            //}
            //else if (Convert.ToDouble(Models.BO.clsGlobVar.dtSimulationCorrectiveEquillibriumValues.Rows[0]["Trim"]) < 0)
            //{
            //    ctrimr.Content = "Trim(m) FWD";
            //    Double abs = (Convert.ToDouble(Models.BO.clsGlobVar.dtSimulationCorrectiveEquillibriumValues.Rows[0]["TRIM"]));
            //    txttrim.Content = Convert.ToString((-1) * abs);
            //}
            //else
            //{
            //    ctrimr.Content = "Trim(m)";
            //    txttrim.Content = (Convert.ToString(Models.BO.clsGlobVar.dtSimulationCorrectiveEquillibriumValues.Rows[0]["TRIM"]));
            //}
            
            //dgGZ.ItemsSource = Models.BO.clsGlobVar.dtSimulationCorrectiveGZ.DefaultView;
            //dgStability.ItemsSource = Models.BO.clsGlobVar.dtSimulationCorrectiveStabilityCriteriaDamage.DefaultView;
            //dgCorrectivehydro.ItemsSource = Models.BO.clsGlobVar.dtCorrectiveSimulationEquilibriumvalues.DefaultView;

            //dgCorrectiveDraft.ItemsSource = Models.BO.clsGlobVar.dtCorrectiveSimulationDraftvalues.DefaultView;





            //if (Models.BO.clsGlobVar.dtSimulationDeFloodingvalues != null)
            //{
            //    listBox1.ItemsSource = Models.BO.clsGlobVar.dtSimulationDeFloodingvalues.DefaultView;
            //}
            //if (Models.BO.clsGlobVar.dtSimulationCorrectiveBallast != null)
            //{
            //    listBox2.ItemsSource = Models.BO.clsGlobVar.dtSimulationCorrectiveBallast.DefaultView;
            //}
            //if (Models.BO.clsGlobVar.dtSimulationCorrectiveDeBallast != null)
            //{

            //    listBox3.ItemsSource = Models.BO.clsGlobVar.dtSimulationCorrectiveDeBallast.DefaultView;
            //}
            //if (Models.BO.clsGlobVar.dtSimulationCorrectiveFluid != null)
            //{
            //    listBox4.ItemsSource = Models.BO.clsGlobVar.dtSimulationCorrectiveFluid.DefaultView;
            //}

            btnballastcalc.Visibility = Visibility.Hidden;
            btndeballastcalc.Visibility = Visibility.Hidden;
            btnFlTransfer.Visibility = Visibility.Hidden;
            btnFuelTransfer.Visibility = Visibility.Hidden;
            int count = 1;

            if (Models.BO.clsGlobVar.dtSimulationDeFloodingvalues != null)
            {
                for (int i = 0; i < Models.BO.clsGlobVar.dtSimulationDeFloodingvalues.Rows.Count; i++)
                {
                    listBoxDeDlooting.Items.Add(Models.BO.clsGlobVar.dtSimulationDeFloodingvalues.Rows[i]["Deflooding_Option"].ToString());

                }
            }
           
            if (Models.BO.clsGlobVar.dtSimulationCorrectiveBallast != null)
            {
                btnballastcalc.Visibility = Visibility.Visible;
                listBoxBallastCorrectiveMeasures.Items.Clear();
                for (int i = 0; i < Models.BO.clsGlobVar.dtSimulationCorrectiveBallast.Rows.Count; i++)
                {
                    listBoxBallastCorrectiveMeasures.Items.Add(Models.BO.clsGlobVar.dtSimulationCorrectiveBallast.Rows[i]["Measures_Suggested"].ToString());

                }
            }
            else
            {
                listBoxBallastCorrectiveMeasures.Items.Clear();
                listBoxBallastCorrectiveMeasures.Items.Add("No Options For Ballasting");
            }


            if (Models.BO.clsGlobVar.dtSimulationCorrectiveDeBallast != null)
            {
                btndeballastcalc.Visibility = Visibility.Visible;
                listBox3.Items.Clear();
                for (int i = 0; i < Models.BO.clsGlobVar.dtSimulationCorrectiveDeBallast.Rows.Count; i++)
                {
                    listBox3.Items.Add(Models.BO.clsGlobVar.dtSimulationCorrectiveDeBallast.Rows[i]["Measures_Suggested"].ToString());

                }
            }
            else
            {
                listBox3.Items.Clear();
                listBox3.Items.Add("No Options For Deballasting");
            }
            

            if (Models.BO.clsGlobVar.dtSimulationCorrectiveFluid != null)
            {
                btnFlTransfer.Visibility = Visibility.Visible;
                listBox4.Items.Clear();

                for (int i = 0; i < Models.BO.clsGlobVar.dtSimulationCorrectiveFluid.Rows.Count; i++)
                {
                    listBox4.Items.Add(Models.BO.clsGlobVar.dtSimulationCorrectiveFluid.Rows[i]["Measures_Suggested"].ToString());

                }
            }
            else
            {
                listBox4.Items.Clear();
                listBox4.Items.Add("No Options For Fluid Transfer");
            }
            if (Models.BO.clsGlobVar.dtSimulationCorrectiveFluidfuel != null)
            {
                btnFuelTransfer.Visibility = Visibility.Visible;
                listBox5.Items.Clear();

                for (int i = 0; i < Models.BO.clsGlobVar.dtSimulationCorrectiveFluidfuel.Rows.Count; i++)
                {
                    listBox5.Items.Add(Models.BO.clsGlobVar.dtSimulationCorrectiveFluidfuel.Rows[i]["Measures_Suggested"].ToString());

                }
            }
            else
            {
                listBox5.Items.Clear();
                listBox5.Items.Add("No Options For Fluid Transfer");
            }



        }
        //public void GZChart()
        //{
        //    for (int index = 0; index <= 30; index++)
        //    {
        //        try
        //        {
        //            GZList.Add(new KeyValuePair<double, double>(Convert.ToDouble((dgGZ.Items[index] as DataRowView)[0]), Convert.ToDouble((dgGZ.Items[index] as DataRowView)[1])));
        //        }
        //        catch
        //        { }
        //    }
        //    GZSeries.ItemsSource = GZList;
        //}

        private void btnCorrectiveReport_Click(object sender, RoutedEventArgs e)
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
        }

        static String ISO_Date()
        {
            return DateTime.Now.ToString("yyyyMM_dd HH_mm_ss");
        }
        public void printToPdf()
        {
            try
            {

                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                Document doc = new Document(iTextSharp.text.PageSize.A4, 30, 30, 30, 20);
                CorrectiveReportPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Reports\\" + ISO_Date() + "_CorrectiveReport.pdf";
                PdfWriter wri = PdfWriter.GetInstance(doc, new FileStream(CorrectiveReportPath, FileMode.Create));



                doc.Open();//Open Document to write
                wri.PageEvent = new pdfFormating();

                iTextSharp.text.Paragraph p2 = new iTextSharp.text.Paragraph("StabilityP15B Report P15B : Countermeasure", FontFactory.GetFont(FontFactory.TIMES, 12, iTextSharp.text.Font.BOLD));
                p2.Alignment = Element.ALIGN_CENTER;
                doc.Add(p2);
                // for adition of logo.......................................
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
                ///............................................................

                doc.Add(new iTextSharp.text.Paragraph("  "));

                //for List 1...................................................

                iTextSharp.text.Paragraph CorrectiveActions = new iTextSharp.text.Paragraph(" Suggested Corrective Actions   ", FontFactory.GetFont(FontFactory.TIMES, 12, iTextSharp.text.Font.BOLD)); // Loading Summary Table Name
                CorrectiveActions.Alignment = Element.ALIGN_CENTER;
                doc.Add(CorrectiveActions);
                doc.Add(new iTextSharp.text.Paragraph("  "));


                iTextSharp.text.Font fntHeader = FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Color.BLUE);   //Header
                iTextSharp.text.Font fntBody = FontFactory.GetFont("Times New Roman", 6.5f);   // Body
                PdfPTable tblList1 = new PdfPTable(1);
                tblList1.WidthPercentage = 90;
                float[] widthsList1 = new float[] { 8f };
                tblList1.SetWidths(widthsList1);

                PdfPCell dflood = new PdfPCell(new Phrase("De-Flooding Options", fntHeader));

                dflood.HorizontalAlignment = Element.ALIGN_LEFT;
                tblList1.AddCell(dflood);
              
                DataTable dtlist1 = new DataTable();
                dtlist1 = ((DataView)listBox1.ItemsSource).ToTable();
                int columnCountlst1 = dtlist1.Columns.Count;
                int rowCountlst1 = dtlist1.Rows.Count;
                for (int rowCounter = 0; rowCounter < rowCountlst1; rowCounter++)
                {
                    for (int columnCounter = 0; columnCounter < columnCountlst1; columnCounter++)
                    {

                        string temp;
                        object obj = dtlist1.Rows[rowCounter][columnCounter];
                        string strValue111 = (obj.ToString());
                        //decimal d = Convert.ToDecimal(strValue111);
                        //temp = Convert.ToString(Math.Round(d, 2));
                        PdfPCell cell1 = new PdfPCell(new Phrase(strValue111, FontFactory.GetFont("Times New Roman", 7)));
                        cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                        tblList1.AddCell(cell1);

                    }
                }
                doc.Add(tblList1);
                doc.Add(new iTextSharp.text.Paragraph("  "));
                //.............................................................

                // for List2..................................................

                PdfPTable tblList2 = new PdfPTable(1);
                tblList2.WidthPercentage = 90;
                float[] widthsList2 = new float[] { 8f };
                tblList2.SetWidths(widthsList2);

                PdfPCell ballasting = new PdfPCell(new Phrase("Ballasting , De-Ballasting and Fluid Transfer ", fntHeader));

                dflood.HorizontalAlignment = Element.ALIGN_LEFT;
                tblList2.AddCell(ballasting);

                DataTable dtlist2 = new DataTable();
                dtlist2 = ((DataView)listBox2.ItemsSource).ToTable();
                int columnCountlst2 = dtlist2.Columns.Count;
                int rowCountlst2 = dtlist2.Rows.Count;
                for (int rowCounter = 0; rowCounter < rowCountlst2-1; rowCounter++)
                {
                    for (int columnCounter = 1; columnCounter < columnCountlst2; columnCounter++)
                    {

                        string temp;
                        object obj = dtlist2.Rows[rowCounter][columnCounter];
                        string strValue121 = (obj.ToString());
                        PdfPCell cell1 = new PdfPCell(new Phrase(strValue121, FontFactory.GetFont("Times New Roman", 7)));
                        cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                        tblList2.AddCell(cell1);

                    }
                }
                doc.Add(tblList2);
                doc.Add(new iTextSharp.text.Paragraph("  "));


                //............................................................

                iTextSharp.text.Paragraph p33 = new iTextSharp.text.Paragraph(" Ship's Expected Condition  Post Application of Corretive Action ", FontFactory.GetFont(FontFactory.TIMES, 12, iTextSharp.text.Font.BOLD | iTextSharp.text.Font.UNDERLINE)); // Loading Summary Table Name
                p33.Alignment = Element.ALIGN_CENTER;
                doc.Add(p33);
                doc.Add(new iTextSharp.text.Paragraph("  "));

                // for Hydrostatics Data table......................................
                int columnCount;
                int rowCount;
                iTextSharp.text.Paragraph p3 = new iTextSharp.text.Paragraph(" Hydrostatics and Drafts ", FontFactory.GetFont(FontFactory.TIMES, 12, iTextSharp.text.Font.BOLD)); // Loading Summary Table Name
                p3.Alignment = Element.ALIGN_CENTER;
                doc.Add(p3);
                doc.Add(new iTextSharp.text.Paragraph("  "));
               // iTextSharp.text.Font fntHeader = FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Color.BLUE);   //Header
                //iTextSharp.text.Font fntBody = FontFactory.GetFont("Times New Roman", 6.5f);   // Body
                PdfPTable tblhydrostatics  = new PdfPTable(11);
                tblhydrostatics.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                tblhydrostatics.WidthPercentage = 95;
                float[] widthsHydrostatics = new float[] { 2.4f, 1.2f, 1.2f, 1.2f, 1.2f, 1.2f, 1.2f, 1.2f, 1.2f, 1.2f, 1.2f };
                tblhydrostatics.SetWidths(widthsHydrostatics);
                tblhydrostatics.HorizontalAlignment = Element.ALIGN_CENTER;
                PdfPCell dispacement = new PdfPCell(new Phrase("DISPLACEMENT(T)", fntHeader));
                dispacement.HorizontalAlignment = Element.ALIGN_CENTER;
                PdfPCell gmt = new PdfPCell(new Phrase("GMT(M)", fntHeader));
                gmt.HorizontalAlignment = Element.ALIGN_CENTER;
                PdfPCell kg = new PdfPCell(new Phrase("KG(M)", fntHeader));
                kg.HorizontalAlignment = Element.ALIGN_CENTER;
                PdfPCell kgf = new PdfPCell(new Phrase("KGF(M)", fntHeader));
                kgf.HorizontalAlignment = Element.ALIGN_CENTER;
                PdfPCell fsc = new PdfPCell(new Phrase("FSC(M)", fntHeader));
                fsc.HorizontalAlignment = Element.ALIGN_CENTER;
                PdfPCell lcg = new PdfPCell(new Phrase("LCG(M)", fntHeader));
                lcg.HorizontalAlignment = Element.ALIGN_CENTER;
                PdfPCell tcg = new PdfPCell(new Phrase("TCG(M)", fntHeader));
                tcg.HorizontalAlignment = Element.ALIGN_CENTER;
                PdfPCell lcb = new PdfPCell(new Phrase("LCB(M)", fntHeader));
                lcb.HorizontalAlignment = Element.ALIGN_CENTER;
                PdfPCell tcb = new PdfPCell(new Phrase("TCB(M)", fntHeader));
                tcb.HorizontalAlignment = Element.ALIGN_CENTER;
                PdfPCell lcf = new PdfPCell(new Phrase("LCF(M)", fntHeader));
                lcf.HorizontalAlignment = Element.ALIGN_CENTER;
                PdfPCell tcf = new PdfPCell(new Phrase("TCF(M)", fntHeader));
                tcf.HorizontalAlignment = Element.ALIGN_CENTER;
              
                //Add header to table
                tblhydrostatics.AddCell(dispacement);
                tblhydrostatics.AddCell(gmt);
                tblhydrostatics.AddCell(kg);
                tblhydrostatics.AddCell(kgf);
                tblhydrostatics.AddCell(fsc);
                tblhydrostatics.AddCell(lcg);
                tblhydrostatics.AddCell(tcg);
                tblhydrostatics.AddCell(lcb);
                tblhydrostatics.AddCell(tcb);
                tblhydrostatics.AddCell(lcf);
                tblhydrostatics.AddCell(tcf);
               
                DataTable dtHydrostaticsData = new DataTable();
                //dtHydrostaticsData = ((DataView)dgCorrectivehydro.ItemsSource).ToTable();
                int columnCountHydro = dtHydrostaticsData.Columns.Count;
                int rowCountHydro = dtHydrostaticsData.Rows.Count;
                int [] CorrHydro = new int[11]{2,11,14,16,15,18,27,29,30,19,32};
                
                for (int rowCounter = 0; rowCounter < rowCountHydro; rowCounter++)
                {
                    for (int columnCounter = 0; columnCounter < 11; columnCounter++)
                    {                        
                       int columnCounter1 = CorrHydro[columnCounter];  
                        string strValue = (dtHydrostaticsData.Rows[rowCounter][columnCounter1].ToString());
                        PdfPCell pdf = new PdfPCell(new iTextSharp.text.Paragraph(strValue, fntBody));
                        pdf.HorizontalAlignment = Element.ALIGN_CENTER;
                        tblhydrostatics.AddCell(pdf);
                    }
                }
                doc.Add(tblhydrostatics);
                doc.Add(new iTextSharp.text.Paragraph("  "));

                //.......................................................

                // Drafts..............................................................

                // doc.Add(new iTextSharp.text.Paragraph("  "));
            
                iTextSharp.text.Font fntHeader1 = FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Color.BLUE);
                PdfPTable tblDrafts = new PdfPTable(9);
                tblDrafts.WidthPercentage = 95;
                float[] widthsHydro2 = new float[] { 1f, 1.7f, 1.6f, 1f, 1.7f, 1.8f, 1f, 1.6f,2.1f };
                tblDrafts.SetWidths(widthsHydro2);
                DataTable dtHydroDrafts = new DataTable();
                //dtHydroDrafts = ((DataView)dgCorrectiveDraft.ItemsSource).ToTable();
                int columnCountHydro2 = dtHydroDrafts.Columns.Count;
                int rowCountHydro2 = dtHydroDrafts.Rows.Count;
                decimal trimValue =  Convert.ToDecimal(dtHydroDrafts.Rows[0][7]);
               // decimal tvalue = trimValue.ToString("N");
                decimal heelValue = Convert.ToDecimal(dtHydroDrafts.Rows[0][8]);
                      
             
                PdfPCell ap = new PdfPCell(new Phrase("AP(M)", fntHeader));
                PdfPCell propellr = new PdfPCell(new Phrase("PROPELLER(M)", fntHeader1));
                PdfPCell aftmark = new PdfPCell(new Phrase("AFT MARK(M)", fntHeader1));
                PdfPCell mid = new PdfPCell(new Phrase("MID(M)", fntHeader));
                PdfPCell fwdmark = new PdfPCell(new Phrase("FWD MARK(M)", fntHeader1));
                PdfPCell sonar = new PdfPCell(new Phrase("SONAR DOME(M)", fntHeader1));
                PdfPCell fp = new PdfPCell(new Phrase("FP(M)", fntHeader1));
                PdfPCell trim;
                PdfPCell list;
                if (trimValue > 0)
                {
                     trim = new PdfPCell(new Phrase("Trim(M) FWD", fntHeader1));
                }
                else if (trimValue < 0)
                {
                    trim = new PdfPCell(new Phrase("Trim(M) AFT", fntHeader1));
                }
                else
                {
                     trim = new PdfPCell(new Phrase("Trim(M)", fntHeader1));
                }

                if (heelValue > 0)
                {
                    list = new PdfPCell(new Phrase("LIST(DEG.) STBD", fntHeader1));
                }
                else if (heelValue < 0)
                {
                    list = new PdfPCell(new Phrase("LIST(DEG.) PORT", fntHeader1));
                }
                else { list = new PdfPCell(new Phrase("LIST(DEG.)", fntHeader1)); ;}

                ap.HorizontalAlignment = Element.ALIGN_CENTER;
                propellr.HorizontalAlignment = Element.ALIGN_CENTER;
                aftmark.HorizontalAlignment = Element.ALIGN_CENTER;
                mid.HorizontalAlignment = Element.ALIGN_CENTER;
                fwdmark.HorizontalAlignment = Element.ALIGN_CENTER;
                sonar.HorizontalAlignment = Element.ALIGN_CENTER;
                fp.HorizontalAlignment = Element.ALIGN_CENTER;
                trim.HorizontalAlignment = Element.ALIGN_CENTER;
                list.HorizontalAlignment = Element.ALIGN_CENTER;

                tblDrafts.AddCell(ap);
                tblDrafts.AddCell(propellr);
                tblDrafts.AddCell(aftmark);
                tblDrafts.AddCell(mid);
                tblDrafts.AddCell(fwdmark);
                tblDrafts.AddCell(sonar);
                tblDrafts.AddCell(fp);
                tblDrafts.AddCell(trim);
                tblDrafts.AddCell(list);

                
               // dtHydroDrafts = ((DataView)dgCorrectiveDraft.ItemsSource).ToTable();
                 columnCountHydro2 = dtHydroDrafts.Columns.Count;
                 rowCountHydro2 = dtHydroDrafts.Rows.Count;
                for (int rowCounter = 0; rowCounter < rowCountHydro2; rowCounter++)
                {
                    for (int columnCounter = 0; columnCounter < columnCountHydro2; columnCounter++)
                    {
                        PdfPCell pdf;
                         string strValue = dtHydroDrafts.Rows[rowCounter][columnCounter].ToString();
                         decimal value = Convert.ToDecimal (strValue);
                        string strvalueNew="";
                         if (((columnCounter == 7) || (columnCounter == 8 )) &&  (value < 0))
                         {
                             string dd =value.ToString("N");
                             decimal mm = Convert.ToDecimal(dd);
                             strvalueNew = Convert.ToString( Convert.ToDecimal (mm) * (-1));
                             
                            
                             pdf = new PdfPCell(new iTextSharp.text.Paragraph(strvalueNew, fntBody));
                         }
                         else
                         {
                            
                             pdf = new PdfPCell(new iTextSharp.text.Paragraph(strValue, fntBody));
                         }
                        pdf.HorizontalAlignment = Element.ALIGN_CENTER;
                        tblDrafts.AddCell(pdf);
                    }
                }
                doc.Add(tblDrafts);
                doc.Add(new iTextSharp.text.Paragraph("  "));

                //......................................................................

                //IntactStability Or Damage StabilityP15B Criteria.................
                //PdfPTable tblIntact = new PdfPTable(4);
                //tblIntact.WidthPercentage = 50;
                //float[] widthsIntact = new float[] { 6f, 1.4f, 1.4f, 1f };
                //tblIntact.SetWidths(widthsIntact);

                //PdfPCell criterion = new PdfPCell(new Phrase("Criterion", fntHeader));
                //PdfPCell criticalValue = new PdfPCell(new Phrase("Critical Value", fntHeader));
                //PdfPCell actualvalue = new PdfPCell(new Phrase("Actual Value", fntHeader));
                //PdfPCell status = new PdfPCell(new Phrase("Status", fntHeader));
                //criterion.HorizontalAlignment = Element.ALIGN_CENTER;
                //criticalValue.HorizontalAlignment = Element.ALIGN_CENTER;
                //actualvalue.HorizontalAlignment = Element.ALIGN_CENTER;
                //status.HorizontalAlignment = Element.ALIGN_CENTER;
                //tblIntact.AddCell(criterion);
                //tblIntact.AddCell(criticalValue);
                //tblIntact.AddCell(actualvalue);
                //tblIntact.AddCell(status);
                //DataTable dtfinal = new DataTable();

                //{
                //    dtfinal = ((DataView)dgStability.ItemsSource).ToTable().Clone();
                //    foreach (DataRow dr in ((DataView)dgStability.ItemsSource).ToTable().Rows)
                //    {
                //        dtfinal.Rows.Add(dr.ItemArray);
                //    }
                //    dtfinal.Columns.Remove("Status");
                //    dtfinal.Columns.Add("Status", typeof(string));

                //    for (int i = 0; i < ((DataView)dgStability.ItemsSource).ToTable().Rows.Count; i++)
                //    {
                //        if (((DataView)dgStability.ItemsSource).ToTable().Rows[i]["Status"].ToString() == "True")
                //        {
                //            dtfinal.Rows[i]["Status"] = "Pass";
                //        }
                //        else
                //        {
                //            dtfinal.Rows[i]["Status"] = "Fail";
                //        }
                //    }
                //    dtfinal.Columns["Status"].ReadOnly = true;
                //}
                //int columnCountIntact = dtfinal.Columns.Count;
                //int rowCountIntact = dtfinal.Rows.Count;
                //for (int rowCounter = 0; rowCounter < rowCountIntact; rowCounter++)
                //{
                //    for (int columnCounter = 0; columnCounter < columnCountIntact; columnCounter++)
                //    {
                //        string strValue = dtfinal.Rows[rowCounter][columnCounter].ToString();
                //        PdfPCell pdf = new PdfPCell(new iTextSharp.text.Paragraph(strValue, fntBody));
                //        if (columnCounter == 0)
                //        {
                //            pdf.HorizontalAlignment = Element.ALIGN_LEFT;
                //        }
                //        else
                //        {
                //            pdf.HorizontalAlignment = Element.ALIGN_CENTER;
                //        }
                //        tblIntact.AddCell(pdf);
                //    }
                //}
                //iTextSharp.text.Paragraph pHeaderIntact = new iTextSharp.text.Paragraph("NES 109 StabilityP15B Criteria- Damage", FontFactory.GetFont(FontFactory.TIMES, 12, iTextSharp.text.Font.BOLD));
                //pHeaderIntact.Alignment = Element.ALIGN_CENTER;
                //doc.Add(pHeaderIntact);
                //doc.Add(new iTextSharp.text.Paragraph("  "));
                //doc.Add(tblIntact);
                //doc.NewPage();
                //................................................................................................


                //Gz Graph..................................................
              
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

                doc.NewPage();
                doc.Add(new iTextSharp.text.Paragraph("  "));
                doc.Add(new iTextSharp.text.Paragraph("  "));
                PdfPTable pp1 = new PdfPTable(2);
                pp1.WidthPercentage = 20;
                float[] widthsGZ5 = new float[] { 1.5f, 1f };
                pp1.SetWidths(widthsGZ5);

                PdfPCell heel = new PdfPCell(new Phrase("Heel(Deg)", fntHeader));
                PdfPCell GZ = new PdfPCell(new Phrase("GZ(m)", fntHeader));
                heel.HorizontalAlignment = Element.ALIGN_CENTER;
                GZ.HorizontalAlignment = Element.ALIGN_CENTER;
                pp1.AddCell(heel);
                pp1.AddCell(GZ);
                DataTable dtGZgraph = new DataTable();
                //dtGZgraph = ((DataView)dgGZ.ItemsSource).ToTable();
                int columnCountGZ1 = dtGZgraph.Columns.Count;
                int rowCountGZ1 = dtGZgraph.Rows.Count;
                for (int rowCounter = 0; rowCounter < rowCountGZ1; rowCounter++)
                {
                    for (int columnCounter = 0; columnCounter < 2; columnCounter++)
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
                //.........................................................
                //if (stabilityType == "Intact")
                //{
                //    doc.NewPage();
                //    doc.Add(new iTextSharp.text.Paragraph("  "));
                //}


                doc.Close();
                Mouse.OverrideCursor = null;
                System.Windows.MessageBox.Show("PDF Created!");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message.ToString());
            }
        }

        private void btnFlTransfer_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void btnballastcalc_Click(object sender, RoutedEventArgs e)
        {

            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

            


            try
            {
                ///
                string user = "dbo";
                int res;
                DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
                string Err = "";
                command.CommandText = "spCal_SimulationMode_Stability_Corrective_Ballast";
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
        
                {
                    //Models.TableModel.Write_Log("Get Corrective DATA");
                    Models.TableModel.SimulationModeCorrectiveData();
                    //Models.TableModel.SimulationModePercentFill();
                    Mouse.OverrideCursor = null;

                    CorrectiveActionReport corp = new CorrectiveActionReport();
                    corp.ShowDialog();

                    

                }

            }
            catch
            {
            }
            
            
           
        }




        private void btndeballastcalc_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;




            try
            {
                ///
                string user = "dbo";
                int res;
                DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
                string Err = "";
                command.CommandText = "spCal_SimulationMode_Stability_Corrective_DeBallast";
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

                {
                    //Models.TableModel.Write_Log("Get Corrective DATA");
                    Models.TableModel.SimulationModeCorrectiveData();
                    //Models.TableModel.SimulationModePercentFill();
                    Mouse.OverrideCursor = null;

                    CorrectiveActionReport corp = new CorrectiveActionReport();
                    corp.ShowDialog();



                }

            }
            catch
            {
            }
               
        }

        private void btnFlTransfer_Click_1(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

            try
            {
                ///
                string user = "dbo";
                int res;
                DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
                string Err = "";
                command.CommandText = "spCal_SimulationMode_Stability_Corrective_Transfer";
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

                {
                    //Models.TableModel.Write_Log("Get Corrective DATA");
                    Models.TableModel.SimulationModeCorrectiveData();
                    //Models.TableModel.SimulationModePercentFill();
                    Mouse.OverrideCursor = null;

                    CorrectiveActionReport corp = new CorrectiveActionReport();
                    corp.ShowDialog();



                }

            }
            catch
            {
            }
        }

        private void btnAllCorrective_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                string user = "dbo";
                int res;
                DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
                string Err = "";

                string query = "TRUNCATE TABLE GZData_CorrectiveAll";
                command = Models.DAL.clsDBUtilityMethods.GetCommand();
                command.CommandText = query;
                command.CommandType = CommandType.Text;
                Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);
           
            if (Models.BO.clsGlobVar.dtSimulationCorrectiveBallast != null)
            {

                DbCommand command1 = Models.DAL.clsDBUtilityMethods.GetCommand();
                Err = "";
                command1.CommandText = "spCal_SimulationMode_Stability_Corrective_Ballast";
                command1.CommandType = CommandType.StoredProcedure;
                DbParameter param1 = Models.DAL.clsDBUtilityMethods.GetParameter();
                param1.DbType = DbType.String;
                param1.ParameterName = "@User";
                command1.Parameters.Add(param1);

                DbParameter param2 = Models.DAL.clsDBUtilityMethods.GetParameter();
                param2.DbType = DbType.Int16;
                param2.Direction = ParameterDirection.Output;
                param2.ParameterName = "@Stability_Calculation_Result";
                command1.Parameters.Add(param2);

                param1.Value = user;
                Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command1, Err);
                Models.BO.clsGlobVar.CalculationResult = Convert.ToInt32(command1.Parameters[1].Value);
                isCalculationRunning = false; 
            }
         


            if (Models.BO.clsGlobVar.dtSimulationCorrectiveDeBallast != null)
            {
                DbCommand command3 = Models.DAL.clsDBUtilityMethods.GetCommand();
                Err = "";
                command3.CommandText = "spCal_SimulationMode_Stability_Corrective_DeBallast";
                command3.CommandType = CommandType.StoredProcedure;
                DbParameter param1 = Models.DAL.clsDBUtilityMethods.GetParameter();
                param1.DbType = DbType.String;
                param1.ParameterName = "@User";
                command3.Parameters.Add(param1);

                DbParameter param2 = Models.DAL.clsDBUtilityMethods.GetParameter();
                param2.DbType = DbType.Int16;
                param2.Direction = ParameterDirection.Output;
                param2.ParameterName = "@Stability_Calculation_Result";
                command3.Parameters.Add(param2);

                param1.Value = user;
                Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command3, Err);
                Models.BO.clsGlobVar.CalculationResult = Convert.ToInt32(command3.Parameters[1].Value);
                isCalculationRunning = false; 
            }
          
            

            if (Models.BO.clsGlobVar.dtSimulationCorrectiveFluid != null)
            {

                DbCommand command4 = Models.DAL.clsDBUtilityMethods.GetCommand();
                Err = "";
                command4.CommandText = "spCal_SimulationMode_Stability_Corrective_Transfer";
                command4.CommandType = CommandType.StoredProcedure;
                DbParameter param1 = Models.DAL.clsDBUtilityMethods.GetParameter();
                param1.DbType = DbType.String;
                param1.ParameterName = "@User";
                command4.Parameters.Add(param1);

                DbParameter param2 = Models.DAL.clsDBUtilityMethods.GetParameter();
                param2.DbType = DbType.Int16;
                param2.Direction = ParameterDirection.Output;
                param2.ParameterName = "@Stability_Calculation_Result";
                command4.Parameters.Add(param2);

                param1.Value = user;
                Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command4, Err);
                Models.BO.clsGlobVar.CalculationResult = Convert.ToInt32(command4.Parameters[1].Value);
                isCalculationRunning = false; 
            }
            DbCommand command5 = Models.DAL.clsDBUtilityMethods.GetCommand();
            Err = "";
            string sCmd = "select top 1 CorrectiveAction from GZData_CorrectiveAll order by GZArea desc";
            command5.CommandText = sCmd;
            command5.CommandType = CommandType.Text;
       
                string BestCorrective ="";
           
             DataTable DB = Models.DAL.clsDBUtilityMethods.GetTable(command5, Err);

             if (DB.Rows.Count > 0)
                {
                    BestCorrective = DB.Rows[0]["CorrectiveAction"].ToString();
                }

                if (BestCorrective=="Transfer")
                {
                    System.Windows.MessageBox.Show(" Calculation succeeded. Best Corrective Option is Fluid Transfer");
                }
                 if (BestCorrective=="Ballast")
                {
                    System.Windows.MessageBox.Show(" Calculation succeeded. Best Corrective Option is Ballast");
                }
                     if (BestCorrective=="DeBallast")
                {
                    System.Windows.MessageBox.Show(" Calculation succeeded. Best Corrective Option is Deballast");
                         
                }
                     Mouse.OverrideCursor = null;
            }
            catch (Exception)
            {
                
                //throw;
            }
        }

        private void btnfuelFlTransfer_Click_1(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

            try
            {
               
                string user = "dbo";
                int res;
                DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
                string Err = "";
                command.CommandText = "spCal_SimulationMode_Stability_Corrective_TransferFO";
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

                {
                    //Models.TableModel.Write_Log("Get Corrective DATA");
                    Models.TableModel.SimulationModeCorrectiveData();
                    //Models.TableModel.SimulationModePercentFill();
                    Mouse.OverrideCursor = null;
                    CorrectiveActionReport corp = new CorrectiveActionReport();
                    corp.ShowDialog();
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Initialize AutoCAD 2D model on canvas
        /// </summary>
        /// <param name="model">DxfModel or AUTOCAD 2D</param> 
        ///<param name="canvas2D">Canvas.</param> 
        ///<returns> Initialize AutoCAD 2D model on canvas.</returns> 

    }
}
