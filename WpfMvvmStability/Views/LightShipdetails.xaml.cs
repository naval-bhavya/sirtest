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
using System.Data;
using System.Data.Common;

namespace WpfMvvmStability.Views
{
    /// <summary>
    /// Interaction logic for LightShipdetails.xaml
    /// </summary>
    public partial class LightShipdetails : Window
    {
        public LightShipdetails()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
            string Err = "";
            string query = "select Lightship_Weight, [Lightship_LCG],[Lightship_VCG], [Lightship_TCG] from [tblMaster_Config_Addi]";
            command.CommandText = query;
            command.CommandType = CommandType.Text;
            DataTable DB = Models.DAL.clsDBUtilityMethods.GetTable(command, Err);

            //DataTable DB = Models.BO.clsGlobVar.lightShipData;
            txtWeight.Text = DB.Rows[0]["Lightship_Weight"].ToString();
            txtVCG.Text = DB.Rows[0]["Lightship_VCG"].ToString();
            txtTCG.Text = DB.Rows[0]["Lightship_TCG"].ToString();
            txtLCG.Text = DB.Rows[0]["Lightship_LCG"].ToString(); 
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            //string wt = txtWeight.Text;
            //string lcg = txtLCG.Text;
            //string vcg = txtVCG.Text;
            //string tcg = txtTCG.Text;
            //DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
            //string Err = "";
            //string query = "update [tblMaster_Config_Addi] set [Lightship_Weight]=" + wt + ",[Lightship_LCG]=" + lcg + ",[Lightship_VCG]=" + vcg + ",[Lightship_TCG]=" + tcg;

            //command.CommandText = query;
            //command.CommandType = CommandType.Text;
            //Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);
            //Models.TableModel.LightShipData();
            //DataTable DB = Models.BO.clsGlobVar.lightShipData;
            //txtWeight.Text = DB.Rows[0]["Lightship_Weight"].ToString();
            //txtVCG.Text = DB.Rows[0]["Lightship_VCG"].ToString();
            //txtTCG.Text = DB.Rows[0]["Lightship_TCG"].ToString();
            //txtLCG.Text = DB.Rows[0]["Lightship_LCG"].ToString();
            //MessageBox.Show("Records saved Successfully");


            DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
            string Err = "";
            string sCmd = "UPDATE [tblMaster_Config_Addi] SET [Lightship_wt]=" + txtWeight.Text + ",[Lightship_LCG]=" + txtLCG.Text + ",[Lightship_VCG]=" + txtVCG.Text + ",[Lightship_TCG]=" + txtTCG.Text + " ";
            command.CommandText = sCmd;
            command.CommandType = CommandType.Text;
            Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);


            sCmd = "";
            Err = "";
            sCmd = "update [tblMaster_Config_Addi] set [Lightship_Weight]=" + txtWeight.Text + ",[Lightship_LCG]=" + txtLCG.Text + ",[Lightship_VCG]=" +  txtVCG.Text + ",[Lightship_TCG]=" +  txtTCG.Text+"";                     
            command.CommandText = sCmd;
            command.CommandType = CommandType.Text;
            Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);


            sCmd = "";
            Err = "";
            sCmd = "UPDATE [tblSimulationMode_Loading_Condition] SET [Weight]=" + txtWeight.Text + ",[LCG]=" + txtLCG.Text + ",[VCG]=" + txtVCG.Text + ",[TCG]=" + txtTCG.Text + "," +
                   "[Lmom]=" + txtLCG.Text + "*" + txtWeight.Text + ",[Tmom]=" + txtTCG.Text + "*" + txtWeight.Text + ",[Vmom]=" + txtVCG.Text + "*" + txtWeight.Text + " " +
                   " WHERE [Tank_ID]= (SELECT [Tank_ID] FROM [tblMaster_Tank] WHERE [Tank_Name]='LIGHTSHIP WEIGHT' AND [Group]='Lightship')";
            command.CommandText = sCmd;
            command.CommandType = CommandType.Text;
            Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);

            sCmd = "";
            Err = "";
            sCmd = "UPDATE [tblSimulationMode_Tank_Status] SET [Weight]=" + txtWeight.Text + " WHERE [Tank_ID]= (SELECT [Tank_ID] FROM [tblMaster_Tank] WHERE [Tank_Name]='LIGHTSHIP WEIGHT' AND [Group]='Lightship')";
            command.CommandText = sCmd;
            command.CommandType = CommandType.Text;
            Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);

            sCmd = "";
            Err = "";
            sCmd = "UPDATE [tblSimulationMode_Equilibrium_Values] SET [Lightship_Weight]=" + txtWeight.Text + "";
            command.CommandText = sCmd;
            command.CommandType = CommandType.Text;
            Models.DAL.clsDBUtilityMethods.ExecuteNonQuery(command, Err);
            MessageBox.Show("Records saved Successfully");
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnclear_Click(object sender, RoutedEventArgs e)
        {
            txtWeight.Clear();
            txtLCG.Clear();
            txtVCG.Clear();
            txtTCG.Clear();
        }

    
    }
}
