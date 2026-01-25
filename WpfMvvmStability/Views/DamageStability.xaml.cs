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

namespace WpfMvvmStability.Views
{
    /// <summary>
    /// Interaction logic for RealModeDamage.xaml
    /// </summary>
    public partial class DamageStability : UserControl
    {
        List<KeyValuePair<double, double>> GZList = new List<KeyValuePair<double, double>>();
        public DamageStability()
        {
            InitializeComponent();
            if (Models.DAL.clsSqlData.Mode == "Real")
            {
                RealModeDamageStabilityData();
            }
            if (Models.DAL.clsSqlData.Mode == "Simulation")
            {
                SimulationModeIntactStabilityData();
            }
            GZChart();
        }
        public void RealModeDamageStabilityData()
        {
            dgLoadingSummary.ItemsSource = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetRealModeLoadingSummaryCurrent").DefaultView;
            dgDraft.ItemsSource = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetRealModeDraftsCurrent").DefaultView;
            dgGZ.ItemsSource = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetRealModeGzDataCurrent").DefaultView;
            dgHydrostatics.ItemsSource = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetRealModeHydrostaticDataCurrent").DefaultView;
            //dgRollingPeriod.ItemsSource = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetBallastTankLoadingStatusDetails").DefaultView;
            dgDamageStability.ItemsSource = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetRealModeDamageStabilityCriteriaCurrent").DefaultView;
            
        }
        public void SimulationModeIntactStabilityData()
        {
            dgLoadingSummary.ItemsSource = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationModeLoadingSummaryCurrent").DefaultView;
            dgDraft.ItemsSource = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationModeDraftsCurrent").DefaultView;
            dgGZ.ItemsSource = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationModeGzDataCurrent").DefaultView;
            dgHydrostatics.ItemsSource = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationModeHydrostaticDataCurrent").DefaultView;
            //dgRollingPeriod.ItemsSource = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetBallastTankLoadingStatusDetails").DefaultView;
            dgDamageStability.ItemsSource = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationModeDamageStabilityCriteriaCurrent").DefaultView;
          
        }
        public void GZChart()
        {
            for (int index = 0; index <= 24; index++)
            {
                GZList.Add(new KeyValuePair<double, double>(Convert.ToDouble((dgGZ.Items[index] as DataRowView)[0]), Convert.ToDouble((dgGZ.Items[index] as DataRowView)[1])));
            }
            GzChart.DataContext = GZList;
            //GzChart.ChartAreas[0]
            //.AxisX.MajorGrid.LineDashStyle = DataVisualization.Charting.ChartDashStyle.Dash;
        }
    }
}
