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
using System.Windows.Threading;
using System.Data;
using WpfMvvmStability.Models.BO;

namespace WpfMvvmStability.Views
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : UserControl
    {
        public Main()
        {
          
            InitializeComponent();
            //Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() => { new Main(); }));
            //Models.DAL.clsSqlData.Mode = "Real";
         
        }

        private void btnIntact_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

        private void btnRealModeStability_Click(object sender, RoutedEventArgs e)
        {
          
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
       
           
        }

        private void btnRealModeDamage_Click(object sender, RoutedEventArgs e)
        {
            //Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            //btnBack.Visibility = Visibility.Visible;
            //btnExit.Visibility = Visibility.Hidden;
            //RealModeContentArea.Content = null;
            //Models.DAL.clsSqlData.Mode = "Real";
            //RealModeContentArea.Content = new DamageStability();
            //Mouse.OverrideCursor = null;
        }

        private void btnRealModeLongitudinal_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void tabControl1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnBackSimulation_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void btnSimulationModeLongitudinal_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void btnSimulationModeDamage_Click(object sender, RoutedEventArgs e)
        {
            //Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            //btnBackSimulation.Visibility = Visibility.Visible;
            //btnExitSimulation.Visibility = Visibility.Hidden;
            //SimulationModeContentArea.Content = null;
            //Models.DAL.clsSqlData.Mode = "Simulation";
            //SimulationModeContentArea.Content = new DamageStability();
            //Mouse.OverrideCursor = null;
        }

        private void btnSimulationModeIntact_Click(object sender, RoutedEventArgs e)
        {
        }

        private void tabItemReal_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

        private void tabItemReal_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
           
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            

        }

        private void btnExitSimulation_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void btnSimulationModeStability_Click(object sender, RoutedEventArgs e)
        {
          

        }

        private void tabItemSimulation_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void tabItemSimulation_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {

        }
    }
}
