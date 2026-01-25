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
using System.Windows.Shapes;
using System.Data.Common;
using System.Data;

namespace WpfMvvmStability.Views
{
    /// <summary>
    /// Interaction logic for WindSpeedWindow.xaml
    /// </summary>
    public partial class WindSpeedWindow : Window
    {
        public WindSpeedWindow()
        {
            InitializeComponent();
        }

        private void btnWindSppedUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow._statusTabRealSimulation == 0)
            {
                DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
                string Err = "";
                string query = " update [tblMaster_Config] set [Wind_Speed]='" + txtWindSpped.Text + "'";
                command.CommandText = query;
                command.CommandType = CommandType.Text;
                int res = Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);
                if (res > 0)
                {
                    MessageBox.Show("Records Updated Successfully");
                }
            }
            else
            {
                DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
                string Err = "";
                string query = " update [tblSimulationMode_Master_Config] set [Wind_Speed]='" + txtWindSpped.Text + "'";
                command.CommandText = query;
                command.CommandType = CommandType.Text;
                int res = Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);
                if (res > 0)
                {
                    MessageBox.Show("Records Updated Successfully");
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (MainWindow._statusTabRealSimulation == 1)
            {
                DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
                string Err = "";
                string query = "select  [Wind_Speed] from [tblSimulationMode_Master_Config]";
                command.CommandText = query;
                command.CommandType = CommandType.Text;
                DataTable dt = Models.DAL.clsDBUtilityMethods.GetTable(command, Err);
                txtWindSpped.Text = dt.Rows[0]["Wind_Speed"].ToString();   
            }
            else
            {
                DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
                string Err = "";
                string query = "select  [Wind_Speed] from [tblMaster_Config]";
                command.CommandText = query;
                command.CommandType = CommandType.Text;
                DataTable dt = Models.DAL.clsDBUtilityMethods.GetTable(command, Err);
                txtWindSpped.Text = dt.Rows[0]["Wind_Speed"].ToString();   
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

       
    }
}
