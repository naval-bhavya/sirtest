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
using System.Data.SqlClient;
using System.Data.Sql;
using System.Xml;
using System.IO;
using Microsoft.Win32;
using System.Reflection;
using System.Xml.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Data.Common;

namespace WpfMvvmStability.Views
{
    /// <summary>
    /// Interaction logic for LightShipData.xaml
    /// </summary>
    public partial class LightShipData : Window
    {
        public LightShipData()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataTable DB = Models.BO.clsGlobVar.lightShipData;
            txtWeight.Text = DB.Rows[0]["Lightship_Weight"].ToString();
            txtVCG.Text = DB.Rows[0]["Lightship_VCG"].ToString();
            txtTCG.Text = DB.Rows[0]["Lightship_TCG"].ToString();
            txtLCG.Text = DB.Rows[0]["Lightship_LCG"].ToString();      
        }
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            string wt = txtWeight.Text;
            string lcg = txtLCG.Text;
            string vcg = txtVCG.Text;
            string tcg = txtTCG.Text;
            DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
            string Err = "";
            string query = "update [tblMaster_Config_Addi] set [Lightship_Weight]=" + wt + ",[Lightship_LCG]=" + lcg + ",[Lightship_VCG]=" + vcg + ",[Lightship_TCG]=" + tcg;

            command.CommandText = query;
            command.CommandType = CommandType.Text;
            Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);
            Models.TableModel.LightShipData();
            DataTable DB = Models.BO.clsGlobVar.lightShipData;
            txtWeight.Text = DB.Rows[0]["Lightship_Weight"].ToString();
            txtVCG.Text = DB.Rows[0]["Lightship_VCG"].ToString();
            txtTCG.Text = DB.Rows[0]["Lightship_TCG"].ToString();
            txtLCG.Text = DB.Rows[0]["Lightship_LCG"].ToString();
            MessageBox.Show("Records saved Successfully");
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnclear_Click(object sender, RoutedEventArgs e)
        {
             txtWeight.Text = " ";
             txtLCG.Text = " ";
             txtVCG.Text = " ";
             txtTCG.Text = " ";
        }
  
    }
}
