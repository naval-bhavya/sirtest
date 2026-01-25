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
using System.Data;
using System.IO;
using System.Windows.Controls.DataVisualization.Charting;
using System.Reflection;

namespace WpfMvvmStability.Views
{
    /// <summary>
    /// Interaction logic for RealModeLongitudinal.xaml
    /// </summary>
    public partial class Longitudinal : Page
    {
        List<KeyValuePair<double, double>> SFList = new List<KeyValuePair<double, double>>();
        List<KeyValuePair<double, double>> BMList = new List<KeyValuePair<double, double>>();
        List<KeyValuePair<double, double>> BMPermissibleHigh = new List<KeyValuePair<double, double>>();
        List<KeyValuePair<double, double>> BMPermissibleLow = new List<KeyValuePair<double, double>>();
        List<KeyValuePair<double, double>> SFPermissibleHigh = new List<KeyValuePair<double, double>>();
        List<KeyValuePair<double, double>> SFPermissibleLow = new List<KeyValuePair<double, double>>();
        int rowCount = 0;
        public Longitudinal()
        {
            InitializeComponent();
            if (Models.BO.clsGlobVar.Mode == "Real")
            {
                dgLongitudinal.ItemsSource = Models.BO.clsGlobVar.dtRealLongitudinal.DefaultView;
                rowCount = Models.BO.clsGlobVar.dtRealLongitudinal.Rows.Count;
                LongitudinalGraph();
            }
            else if (Models.BO.clsGlobVar.Mode == "Simulation")
            {
                dgLongitudinal.ItemsSource = Models.BO.clsGlobVar.dtSimulationLongitudinal.DefaultView;
                rowCount = Models.BO.clsGlobVar.dtSimulationLongitudinal.Rows.Count;
                LongitudinalGraph();
            }
      
        }


        public void LongitudinalGraph()
        {
            try
            {
                for (int index = 0; index < rowCount; index++)
                {
                    SFList.Add(new KeyValuePair<double, double>(Convert.ToDouble((dgLongitudinal.Items[index] as DataRowView)[0]), Convert.ToDouble((dgLongitudinal.Items[index] as DataRowView)[3])));
                }
                for (int index = 0; index < rowCount; index++)
                {
                    BMList.Add(new KeyValuePair<double, double>(Convert.ToDouble((dgLongitudinal.Items[index] as DataRowView)[0]), Convert.ToDouble((dgLongitudinal.Items[index] as DataRowView)[4])));
                }
                for (int index = 0; index < Models.BO.clsGlobVar.dtSFandBMPermissible.Rows.Count; index++)
                {
                    BMPermissibleHigh.Add(new KeyValuePair<double, double>(Convert.ToDouble(Models.BO.clsGlobVar.dtSFandBMPermissible.Rows[index]["Distance"]), Convert.ToDouble(Models.BO.clsGlobVar.dtSFandBMPermissible.Rows[index]["BM_permissible_high"])));
                }
                for (int index = 0; index < Models.BO.clsGlobVar.dtSFandBMPermissible.Rows.Count; index++)
                {
                    BMPermissibleLow.Add(new KeyValuePair<double, double>(Convert.ToDouble(Models.BO.clsGlobVar.dtSFandBMPermissible.Rows[index]["Distance"]), Convert.ToDouble(Models.BO.clsGlobVar.dtSFandBMPermissible.Rows[index]["BM_permissible_low"])));
                }

                for (int index = 0; index < Models.BO.clsGlobVar.dtSFandBMPermissible.Rows.Count; index++)
                {
                    SFPermissibleHigh.Add(new KeyValuePair<double, double>(Convert.ToDouble(Models.BO.clsGlobVar.dtSFandBMPermissible.Rows[index]["Distance"]), Convert.ToDouble(Models.BO.clsGlobVar.dtSFandBMPermissible.Rows[index]["SF_permissible_high"])));
                }
                for (int index = 0; index < Models.BO.clsGlobVar.dtSFandBMPermissible.Rows.Count; index++)
                {
                    SFPermissibleLow.Add(new KeyValuePair<double, double>(Convert.ToDouble(Models.BO.clsGlobVar.dtSFandBMPermissible.Rows[index]["Distance"]), Convert.ToDouble(Models.BO.clsGlobVar.dtSFandBMPermissible.Rows[index]["SF_permissible_low"])));
                }
                BM.Maximum = Convert.ToDouble(Models.BO.clsGlobVar.dtSFandBMPermissibleMax.Rows[0][0]) + 1000;
                BM.Minimum = -Convert.ToDouble(Models.BO.clsGlobVar.dtSFandBMPermissibleMax.Rows[0][1]) - 1000;
                SF.Maximum = Convert.ToDouble(Models.BO.clsGlobVar.dtSFandBMPermissibleMax.Rows[0][2]) + 100;
                SF.Minimum = -Convert.ToDouble(Models.BO.clsGlobVar.dtSFandBMPermissibleMax.Rows[0][3]) - 100;

                BMPermHigh.Maximum = Convert.ToDouble(Models.BO.clsGlobVar.dtSFandBMPermissibleMax.Rows[0][0]) + 1000;
                BMPermHigh.Minimum = -Convert.ToDouble(Models.BO.clsGlobVar.dtSFandBMPermissibleMax.Rows[0][1]) - 1000;
                BMPermLow.Maximum = Convert.ToDouble(Models.BO.clsGlobVar.dtSFandBMPermissibleMax.Rows[0][0]) + 1000;
                BMPermLow.Minimum = -Convert.ToDouble(Models.BO.clsGlobVar.dtSFandBMPermissibleMax.Rows[0][1]) - 1000;

                SFPermHigh.Maximum = Convert.ToDouble(Models.BO.clsGlobVar.dtSFandBMPermissibleMax.Rows[0][2]) + 100;
                SFPermHigh.Minimum = -Convert.ToDouble(Models.BO.clsGlobVar.dtSFandBMPermissibleMax.Rows[0][3]) - 100;
                SFPermLow.Maximum = Convert.ToDouble(Models.BO.clsGlobVar.dtSFandBMPermissibleMax.Rows[0][2]) + 100;
                SFPermLow.Minimum = -Convert.ToDouble(Models.BO.clsGlobVar.dtSFandBMPermissibleMax.Rows[0][3]) - 100;


                SFSeries.ItemsSource = SFList;
                BMSeries.ItemsSource = BMList;
                BMPermissibleHighSeries.ItemsSource = BMPermissibleHigh;
                BMPermissibleLowSeries.ItemsSource = BMPermissibleLow;
                SFPermissibleHighSeries.ItemsSource = SFPermissibleHigh;
                SFPermissibleLowSeries.ItemsSource = SFPermissibleLow;
            }
            catch
            {
            }
           
        }
        private void NetLoadSeries_Loaded(object sender, RoutedEventArgs e)
        {
        //    int chartWidth = 1000;
        //    int chartHeight = 390;
        //    double resolution = 96d;
        //    //LongitudinalChart.UpdateLayout();
        //    //LongitudinalChart.Arrange(new Rect(new System.Windows.Size(chartWidth, chartHeight)));
        //    //LongitudinalChart.UpdateLayout();
        //    LongitudinalChart.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
        //    LongitudinalChart.Arrange(new Rect(new System.Windows.Size(chartWidth, chartHeight)));
        //    LongitudinalChart.UpdateLayout();
        //    LongitudinalChart.InvalidateVisual();

        //    RenderTargetBitmap bmp = new RenderTargetBitmap(chartWidth, chartHeight, resolution, resolution, PixelFormats.Pbgra32);
        //    bmp.Render(LongitudinalChart);
        //    BmpBitmapEncoder encoder = new BmpBitmapEncoder();
        //    encoder.Frames.Add(BitmapFrame.Create(bmp));
        //    FileStream fileStream = new FileStream(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Images\\Longitudinal.png", FileMode.Create);
        //    encoder.Save(fileStream);
        //    fileStream.Close();
        //    bmp.Clear();

        }
      
    }
}
