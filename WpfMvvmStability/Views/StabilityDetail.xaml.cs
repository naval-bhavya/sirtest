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
using MahApps.Metro.Controls;
using System.Data;
using System.Windows.Controls.DataVisualization.Charting;

using WpfMvvmStability.Models.BO;




namespace WpfMvvmStability.Views
{
    /// <summary>
    /// Interaction logic for RealModeIntact.xaml
    /// </summary>
    public partial class StabilityDetail : Page
    {
        List<KeyValuePair<double, double>> GZList = new List<KeyValuePair<double, double>>();
        List<KeyValuePair<double, double>> WHList = new List<KeyValuePair<double, double>>();
        List<KeyValuePair<double, double>> HLList = new List<KeyValuePair<double, double>>();
        List<KeyValuePair<double, double>> HSList = new List<KeyValuePair<double, double>>();
        List<KeyValuePair<double, double>> PCList = new List<KeyValuePair<double, double>>();
        List<KeyValuePair<double, double>> DFList = new List<KeyValuePair<double, double>>();

        int rowCount = 0;
        string stabilityType = "";
        List<Point> points = new List<Point>();
        public IEnumerable<Point> Points
        {
            get { return points; }
        }
        public StabilityDetail()
        {
            InitializeComponent();
            if (Models.BO.clsGlobVar.Mode == "Real") 
            {
                RealModeIntactStabilityData();
            }
            if (Models.BO.clsGlobVar.Mode == "Simulation")
            {
                SimulationModeIntactStabilityData();
            }

        }

        public void RealModeIntactStabilityData()
        {
            try
            {
                DataTable dtFinalereal = new DataTable();
                dgLoadingSummary.ItemsSource = clsGlobVar.dtRealLoadingSummary.DefaultView;
               // dgDraft.ItemsSource = clsGlobVar.dtRealDrafts.DefaultView;
                dgDraft.ItemsSource = clsGlobVar.dtRealImersion.DefaultView;
                dgGZ.ItemsSource = clsGlobVar.dtRealGZ.DefaultView;
                dgHydrostatics.ItemsSource = clsGlobVar.dtRealHydrostatics.DefaultView;
                dgMouldedDraft.ItemsSource = clsGlobVar.dtRealdgMouldedDraft.DefaultView;
                gbSoundingPer.Visibility = Visibility.Hidden;
                stabilityType = Models.BO.clsGlobVar.StabilityType.ToUpper();
                if (stabilityType == "INTACT")
                {
                    try
                    {
                        //foreach (DataRow dr in clsGlobVar.dtRealStabilityCriteriaIntact.Rows)
                        //{
                        //    clsGlobVar.dtRealStabilityCriteriaIntact.Rows.Add(dr.ItemArray);
                        //}
                        //clsGlobVar.dtRealStabilityCriteriaIntact.Columns.Remove("Status");
                        //clsGlobVar.dtRealStabilityCriteriaIntact.Columns.Add("Status", typeof(string));
                        //for (int i = 0; i < clsGlobVar.dtRealStabilityCriteriaIntact.Rows.Count; i++)
                        //{
                        //    if (clsGlobVar.dtRealStabilityCriteriaIntact.Rows[i]["Status"] == "True")
                        //    {
                        //        clsGlobVar.dtRealStabilityCriteriaIntact.Rows[i]["Status"] = "Pass";
                        //    }
                        //    else
                        //    {
                        //        clsGlobVar.dtRealStabilityCriteriaIntact.Rows[i]["Status"] = "Fail";
                        //    }

                        //}
                        //clsGlobVar.dtRealStabilityCriteriaIntact.Columns["Status"].ReadOnly = true;
                        dtFinalereal = clsGlobVar.dtRealStabilityCriteriaIntact.Clone();
                        foreach (DataRow dr in clsGlobVar.dtRealStabilityCriteriaIntact.Rows)
                        {
                            dtFinalereal.Rows.Add(dr.ItemArray);

                        }
                        dtFinalereal.Columns.Remove("Status");
                        dtFinalereal.Columns.Add("Status", typeof(string));

                        for (int i = 0; i < clsGlobVar.dtRealStabilityCriteriaIntact.Rows.Count; i++)
                        {
                            if (clsGlobVar.dtRealStabilityCriteriaIntact.Rows[i]["Status"].ToString() == "True") { dtFinalereal.Rows[i]["Status"] = "Pass"; }
                            else { dtFinalereal.Rows[i]["Status"] = "Fail"; }
                        }
                        dgStability.ItemsSource = dtFinalereal.DefaultView;

                        //dgStability.ItemsSource = clsGlobVar.dtRealStabilityCriteriaIntact.DefaultView;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                if (stabilityType == "DAMAGE")
                {
                  //  dgStability.ItemsSource = clsGlobVar.dtRealStabilityCriteriaDamage.DefaultView;
                    gbSoundingPer.Visibility = Visibility.Hidden;
                    dtFinalereal = clsGlobVar.dtRealStabilityCriteriaDamage.Clone();
                    foreach (DataRow dr in clsGlobVar.dtRealStabilityCriteriaDamage.Rows)
                    {
                        dtFinalereal.Rows.Add(dr.ItemArray);

                    }
                    dtFinalereal.Columns.Remove("Status");
                    dtFinalereal.Columns.Add("Status", typeof(string));

                    for (int i = 0; i < clsGlobVar.dtRealStabilityCriteriaDamage.Rows.Count; i++)
                    {
                        if (clsGlobVar.dtRealStabilityCriteriaDamage.Rows[i]["Status"].ToString() == "True") { dtFinalereal.Rows[i]["Status"] = "Pass"; }
                        else { dtFinalereal.Rows[i]["Status"] = "Fail"; }
                    }
                    dgStability.ItemsSource = dtFinalereal.DefaultView;
                    groupBox2.Visibility = System.Windows.Visibility.Hidden;
                }
                groupBoxStability.Header = "NES 109 StabilityP15B Criteria-" + Models.BO.clsGlobVar.StabilityType;
                rowCount = clsGlobVar.dtRealGZ.Rows.Count;

                GZChart();
            }
            catch
            {
            }

        }
        public void SimulationModeIntactStabilityData()
        {
            try
            {

                dgLoadingSummary.ItemsSource = clsGlobVar.dtSimulationLoadingSummary.DefaultView;
                //dgDraft.ItemsSource = clsGlobVar.dtSimulationDrafts.DefaultView;
                dgDraft.ItemsSource = clsGlobVar.dtSimulationImersion.DefaultView;
                dgHydrostatics.ItemsSource = clsGlobVar.dtSimulationHydrostatics.DefaultView;
                dgMouldedDraft.ItemsSource = clsGlobVar.dtSimulationMouldedDraft.DefaultView;

                stabilityType = Models.BO.clsGlobVar.SimulationStabilityType.ToUpper();
                DataTable dtFinale = new DataTable();

                if (stabilityType == "INTACT")
                {
                    gbSoundingPer.Visibility = Visibility.Hidden;
                    dgGZ.Columns[3].Visibility = Visibility.Visible;
                    dgGZ.Columns[4].Visibility = Visibility.Visible;
                    dgGZ.Columns[5].Visibility = Visibility.Visible;
                 
                    dgSoundingPer.ItemsSource = Models.BO.clsGlobVar.dtSoundingPer.DefaultView;
                    dgGZ.ItemsSource = clsGlobVar.dtSimulationGZ.DefaultView;
                    //dgStability.ItemsSource = clsGlobVar.dtSimulationStabilityCriteriaIntact.DefaultView;
                    dtFinale=clsGlobVar.dtSimulationStabilityCriteriaIntact.Clone();
                    foreach (DataRow dr in clsGlobVar.dtSimulationStabilityCriteriaIntact.Rows)
                    {
                        dtFinale.Rows.Add(dr.ItemArray);

                    }
                    dtFinale.Columns.Remove("Status");
                    dtFinale.Columns.Add("Status", typeof(string));

                    

                    for (int i = 0; i < clsGlobVar.dtSimulationStabilityCriteriaIntact.Rows.Count; i++ )
                    {
                        if (clsGlobVar.dtSimulationStabilityCriteriaIntact.Rows[i]["Status"].ToString() == "True") { dtFinale.Rows[i]["Status"] = "Pass"; }
                        else { dtFinale.Rows[i]["Status"] = "Fail"; }
                    }
                    dgStability.ItemsSource = dtFinale.DefaultView;
                }
                if (stabilityType == "DAMAGE")
                {

                    dgGZ.Columns[3].Visibility = Visibility.Hidden;
                    dgGZ.Columns[4].Visibility = Visibility.Hidden;
                    dgGZ.Columns[5].Visibility = Visibility.Hidden;
                    //DataTable newTable = clsGlobVar.dtSimulationGZ.DefaultView.ToTable(false, "heelAng", "heelGZ");
                    //dgGZ.ItemsSource = newTable.DefaultView;
                    gbSoundingPer.Visibility = Visibility.Visible;
                    dgGZ.ItemsSource = Models.BO.clsGlobVar.dtSimulationGZDamaged.DefaultView;
                    dgSoundingPer.ItemsSource = Models.BO.clsGlobVar.dtSoundingPer.DefaultView;
                    //dgStability.ItemsSource = clsGlobVar.dtSimulationStabilityCriteriaDamage.DefaultView;

                    dtFinale = clsGlobVar.dtSimulationStabilityCriteriaDamage.Clone();
                    foreach (DataRow dr in clsGlobVar.dtSimulationStabilityCriteriaDamage.Rows)
                    {
                        dtFinale.Rows.Add(dr.ItemArray);

                    }
                    dtFinale.Columns.Remove("Status");
                    dtFinale.Columns.Add("Status", typeof(string));

                    for (int i = 0; i < clsGlobVar.dtSimulationStabilityCriteriaDamage.Rows.Count; i++)
                    {
                        if (clsGlobVar.dtSimulationStabilityCriteriaDamage.Rows[i]["Status"].ToString() == "True") { dtFinale.Rows[i]["Status"] = "Pass"; }
                        else { dtFinale.Rows[i]["Status"] = "Fail"; }
                    }
                    dgStability.ItemsSource =  dtFinale.DefaultView;
                }
                groupBoxStability.Header = "NES 109 StabilityP15B Criteria-" + Models.BO.clsGlobVar.SimulationStabilityType;
                rowCount = clsGlobVar.dtSimulationGZ.Rows.Count;

                GZChart();

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
                    if (stabilityType == "INTACT")
                    {
                        WHList.Add(new KeyValuePair<double, double>(Convert.ToDouble((dgGZ.Items[index] as DataRowView)[0]), Convert.ToDouble((dgGZ.Items[index] as DataRowView)[2])));
                        HLList.Add(new KeyValuePair<double, double>(Convert.ToDouble((dgGZ.Items[index] as DataRowView)[0]), Convert.ToDouble((dgGZ.Items[index] as DataRowView)[3])));
                        HSList.Add(new KeyValuePair<double, double>(Convert.ToDouble((dgGZ.Items[index] as DataRowView)[0]), Convert.ToDouble((dgGZ.Items[index] as DataRowView)[4])));
                        PCList.Add(new KeyValuePair<double, double>(Convert.ToDouble((dgGZ.Items[index] as DataRowView)[0]), Convert.ToDouble((dgGZ.Items[index] as DataRowView)[5])));
                
                    }
                }
                if (stabilityType == "INTACT")
                {
                    GZSeries.ItemsSource = GZList;
                    WHSeries.ItemsSource = WHList;
                    HLSeries.ItemsSource = HLList;
                    HSSeries.ItemsSource = HSList;
                    PCSeries.ItemsSource = PCList;

                }
                else if (stabilityType == "DAMAGE")
                {
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
            }
            catch
            {
            }

        }

        private void dgLoadingSummary_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void scrollViewer2_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {

        }
    }

}

