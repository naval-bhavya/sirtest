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
            EnsureRequiredDataLoaded();
            
            if (Models.BO.clsGlobVar.Mode == "Real") 
            {
                RealModeIntactStabilityData();
            }
            if (Models.BO.clsGlobVar.Mode == "Simulation")
            {
                SimulationModeIntactStabilityData();
            }
        }

        private static void EnsureRequiredDataLoaded()
        {
            if (Models.BO.clsGlobVar.Mode == "Real")
            {
                if (!HasRows(clsGlobVar.dtRealLoadingSummary) || !HasRows(clsGlobVar.dtRealStabilitySummary))
                {
                    Models.TableModel.RealModeData();
                }
            }

            if (Models.BO.clsGlobVar.Mode == "Simulation")
            {
                if (!HasRows(clsGlobVar.dtSimulationLoadingSummary) || !HasRows(clsGlobVar.dtSimulationStabilitySummary))
                {
                    Models.TableModel.SimulationModeData();
                }
            }

            Models.TableModel.LoadSimulationHydrostatics();
        }

        private static bool HasRows(DataTable table)
        {
            return table != null && table.Rows.Count > 0;
        }

        private static DataView ViewOrEmpty(DataTable table)
        {
            return (table ?? new DataTable()).DefaultView;
        }

        public void RealModeIntactStabilityData()
        {
            try
            {
                DataTable dtFinalereal = new DataTable();
                dgLoadingSummary.ItemsSource = clsGlobVar.dtRealLoadingSummary.DefaultView;
                dgDraft.ItemsSource = clsGlobVar.dtRealImersion.DefaultView;
                dgGZ.ItemsSource = clsGlobVar.dtRealGZ.DefaultView;
                dgHydrostatics.ItemsSource = clsGlobVar.dtRealHydrostatics.DefaultView;
                dgMouldedDraft.ItemsSource = clsGlobVar.dtRealdgMouldedDraft.DefaultView;
                gbSoundingPer.Visibility = Visibility.Hidden;
                stabilityType = Models.BO.clsGlobVar.StabilityType.ToUpper();

                if (stabilityType == "INTACT")
                {
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
                }
                else if (stabilityType == "DAMAGE")
                {
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

                txtStabilityCriteriaTitle.Text = "NES 109 StabilityP15B Criteria-" + Models.BO.clsGlobVar.StabilityType;
                rowCount = clsGlobVar.dtRealGZ.Rows.Count;

            if (clsGlobVar.Mode == "Real")
            {
                dgStabilitySummary.ItemsSource = clsGlobVar.dtRealEquillibriumValues.DefaultView;
                dgTrimList.ItemsSource = clsGlobVar.dtRealEquillibriumValues.DefaultView;
            }
            else
            {
                dgStabilitySummary.ItemsSource = clsGlobVar.dtSimulationEquillibriumValues.DefaultView;
                dgTrimList.ItemsSource = clsGlobVar.dtSimulationEquillibriumValues.DefaultView;
            }
                GZChart();
                UpdateSummaryFields();
            }
            catch { }
        }

        public void SimulationModeIntactStabilityData()
        {
            try
            {
                dgLoadingSummary.ItemsSource = clsGlobVar.dtSimulationLoadingSummary.DefaultView;
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
                 
                    dgSoundingPer.ItemsSource = ViewOrEmpty(Models.BO.clsGlobVar.dtSoundingPer);
                    dgGZ.ItemsSource = clsGlobVar.dtSimulationGZ.DefaultView;
                    
                    dtFinale = clsGlobVar.dtSimulationStabilityCriteriaIntact.Clone();
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
                else if (stabilityType == "DAMAGE")
                {
                    dgGZ.Columns[3].Visibility = Visibility.Hidden;
                    dgGZ.Columns[4].Visibility = Visibility.Hidden;
                    dgGZ.Columns[5].Visibility = Visibility.Hidden;
                    gbSoundingPer.Visibility = Visibility.Visible;
                    dgGZ.ItemsSource = Models.BO.clsGlobVar.dtSimulationGZDamaged.DefaultView;
                    dgSoundingPer.ItemsSource = ViewOrEmpty(Models.BO.clsGlobVar.dtSoundingPer);

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
                    dgStability.ItemsSource = dtFinale.DefaultView;
                }

                txtStabilityCriteriaTitle.Text = "NES 109 StabilityP15B Criteria-" + Models.BO.clsGlobVar.SimulationStabilityType;
                rowCount = clsGlobVar.dtSimulationGZ.Rows.Count;

                dgStabilitySummary.ItemsSource = clsGlobVar.dtSimulationEquillibriumValues.DefaultView;
                dgTrimList.ItemsSource = clsGlobVar.dtSimulationEquillibriumValues.DefaultView;
                GZChart();
                UpdateSummaryFields();
            }
            catch { }
        }

        private void UpdateSummaryFields()
        {
            try
            {
                DataTable eqTable = (clsGlobVar.Mode == "Real") ? clsGlobVar.dtRealEquillibriumValues : clsGlobVar.dtSimulationEquillibriumValues;
                if (eqTable != null && eqTable.Rows.Count > 0)
                {
                    DataRow row = eqTable.Rows[0];
                    txtGMT.Text = Convert.ToDecimal(row["GMT"]).ToString("N3");
                    txtDisplacement.Text = Convert.ToDecimal(row["Displacement"]).ToString("N0");
                    txtTrim.Text = Convert.ToDecimal(row["TRIM"]).ToString("N3");
                    txtList.Text = Convert.ToDecimal(row["Heel"]).ToString("N2");

                    // Update Draft Marks
                    txtDraftAP.Text = Convert.ToDecimal(row["Draft_AP"]).ToString("N3");
                    txtDraftAft.Text = Convert.ToDecimal(row["Draft_Aft_Mark"]).ToString("N3");
                    txtDraftMid.Text = Convert.ToDecimal(row["Draft_Mean"]).ToString("N3");
                    txtDraftFwd.Text = Convert.ToDecimal(row["Draft_Fore_Mark"]).ToString("N3");
                    txtDraftFP.Text = Convert.ToDecimal(row["Draft_FP"]).ToString("N3");
                }
            }
            catch { }
        }

        public void GZChart()
        {
            try
            {
                GZList.Clear(); WHList.Clear(); HLList.Clear(); HSList.Clear(); PCList.Clear();
                for (int index = 0; index < dgGZ.Items.Count; index++)
                {
                    var rowView = dgGZ.Items[index] as DataRowView;
                    if (rowView == null) continue;

                    GZList.Add(new KeyValuePair<double, double>(Convert.ToDouble(rowView[0]), Convert.ToDouble(rowView[1])));
                    if (stabilityType == "INTACT")
                    {
                        if (rowView.Row.Table.Columns.Count > 2) WHList.Add(new KeyValuePair<double, double>(Convert.ToDouble(rowView[0]), Convert.ToDouble(rowView[2])));
                        if (rowView.Row.Table.Columns.Count > 3) HLList.Add(new KeyValuePair<double, double>(Convert.ToDouble(rowView[0]), Convert.ToDouble(rowView[3])));
                        if (rowView.Row.Table.Columns.Count > 4) HSList.Add(new KeyValuePair<double, double>(Convert.ToDouble(rowView[0]), Convert.ToDouble(rowView[4])));
                        if (rowView.Row.Table.Columns.Count > 5) PCList.Add(new KeyValuePair<double, double>(Convert.ToDouble(rowView[0]), Convert.ToDouble(rowView[5])));
                    }
                }

                GZSeries.ItemsSource = null; GZSeries.ItemsSource = GZList;
                if (stabilityType == "INTACT")
                {
                    WHSeries.ItemsSource = null; WHSeries.ItemsSource = WHList;
                    HLSeries.ItemsSource = null; HLSeries.ItemsSource = HLList;
                    HSSeries.ItemsSource = null; HSSeries.ItemsSource = HSList;
                    PCSeries.ItemsSource = null; PCSeries.ItemsSource = PCList;
                }
                else if (stabilityType == "DAMAGE")
                {
                    Style style = new Style { TargetType = typeof(LegendItem) };
                    style.Setters.Add(new Setter(LegendItem.VisibilityProperty, Visibility.Hidden));
                    WHSeries.LegendItemStyle = style;
                    HLSeries.LegendItemStyle = style;
                    HSSeries.LegendItemStyle = style;
                    PCSeries.LegendItemStyle = style;
                }
            }
            catch { }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dgLoadingSummary.ItemsSource = null;
                dgGZ.ItemsSource = null;
                dgStability.ItemsSource = null;
                dgHydrostatics.ItemsSource = null;
                dgStabilitySummary.ItemsSource = null;
                dgTrimList.ItemsSource = null;
                
                txtGMT.Text = "N/A";
                txtDisplacement.Text = "N/A";
                txtTrim.Text = "N/A";
                txtList.Text = "N/A";
                
                GZSeries.ItemsSource = null;
                WHSeries.ItemsSource = null;
                HLSeries.ItemsSource = null;
                HSSeries.ItemsSource = null;
                PCSeries.ItemsSource = null;
            }
            catch { }
        }

        private void dgLoadingSummary_SelectionChanged(object sender, SelectionChangedEventArgs e) { }
        private void scrollViewer2_ScrollChanged(object sender, ScrollChangedEventArgs e) { }
    }
}
