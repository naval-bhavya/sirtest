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
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void btnResStabilitycal_Click(object sender, RoutedEventArgs e)
        {
            DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
            string Err = "";
            string query = "update [tblStability_CalculationStatus] set [Stability_Calculation_Status]= 0" ;
            command.CommandText = query;
            command.CommandType = CommandType.Text;
            int res= Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);
            if (res > 0)
            {
                MessageBox.Show("StabilityP15B Calculation Updated Successfully");
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
            string Err = "";
            string query = " update [tblMaster_Config] set [Active]='" + cmbHighLow.Text + "' , [Rise_Fall] ='"+ txtFldRate.Text+"'   ";
            command.CommandText = query;
            command.CommandType = CommandType.Text;
            int res = Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);
            if (res > 0)
            {
                MessageBox.Show("Records Updated Successfully");
            }
        }

        private void lblStabilitySettings_Loaded(object sender, RoutedEventArgs e)
        {
           
            DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
             string Err = "";
            string query = "select  [Active], [Rise_Fall] from [tblMaster_Config]";
            command.CommandText = query;
            command.CommandType = CommandType.Text;
            DataTable dt = Models.DAL.clsDBUtilityMethods.GetTable(command, Err);
            if (dt.Rows[0]["Active"].ToString()=="False")
            {
                cmbHighLow.Text = Convert.ToString (0);
            }
            else
            {
                cmbHighLow.Text = Convert.ToString(1);
            }
            
            ;
            txtFldRate.Text = dt.Rows[0]["Rise_Fall"].ToString();   
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            
        }

       
    }
}
