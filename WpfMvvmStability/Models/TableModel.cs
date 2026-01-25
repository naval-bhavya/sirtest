using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WpfMvvmStability.Models.BO;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;
using System.Windows.Forms;

namespace WpfMvvmStability.Models
{
    class TableModel
    {
        ///  <summary> 
        ///Initializes all the Real Mode DataTables 
        ///</summary> 
       
        public static void RealModeData()
        {
            try
            {
                Models.BO.clsGlobVar.dtRealModeAllTanks = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetAllTankLoadingStatusDetails");
                DataView DV = Models.BO.clsGlobVar.dtRealModeAllTanks.AsDataView();
                DV.RowFilter = "Group = 'BALLAST_TANK'";
                Models.BO.clsGlobVar.dtRealBallastTanks = DV.ToTable();


                DV = Models.BO.clsGlobVar.dtRealModeAllTanks.AsDataView();
                DV.RowFilter = "Group = 'FUELOIL_TANK'";
                Models.BO.clsGlobVar.dtRealFuelOilTanks = DV.ToTable();

                DV = Models.BO.clsGlobVar.dtRealModeAllTanks.AsDataView();
                DV.RowFilter = "Group = 'FRESHWATER_TANK'";
                Models.BO.clsGlobVar.dtRealFreshWaterTanks = DV.ToTable();

                DV = Models.BO.clsGlobVar.dtRealModeAllTanks.AsDataView();
                DV.RowFilter = "Group = 'MISC_TANK'";
                Models.BO.clsGlobVar.dtRealMiscTanks = DV.ToTable();

                DV = Models.BO.clsGlobVar.dtRealModeAllTanks.AsDataView();
                DV.RowFilter = "Group = 'Compartment'";
                Models.BO.clsGlobVar.dtRealCompartments = DV.ToTable();

                DV=Models.BO.clsGlobVar.dtRealModeAllTanks.AsDataView();
                DV.RowFilter = "Group = 'WT_REGION'";
                Models.BO.clsGlobVar.dtRealWaterTightRegion = DV.ToTable();


                Models.BO.clsGlobVar.dtRealVariableItems = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetVariableDetails");
                Models.BO.clsGlobVar.dtRealEquillibriumValues = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetRealModeEquillibriumValues");
                Models.BO.clsGlobVar.dtRealLoadingSummary = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetRealModeLoadingSummaryCurrent");
                Models.BO.clsGlobVar.dtRealDrafts = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetRealModeDraftsCurrent");
                Models.BO.clsGlobVar.dtRealImersion = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetRealImersionParticulars");
                Models.BO.clsGlobVar.dtSimulationImersion = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationImersionParticulars");
                Models.BO.clsGlobVar.dtRealGZ = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetRealModeGzDataCurrent");
                Models.BO.clsGlobVar.dtRealGZDamaged = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetRealModeGzDataCurrentDamaged");
                Models.BO.clsGlobVar.dtRealHydrostatics = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetRealModeHydrostaticDataCurrent");
                Models.BO.clsGlobVar.dtRealStabilityCriteriaIntact = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetRealModeIntactStabilityCriteriaCurrent");
                Models.BO.clsGlobVar.dtRealStabilityCriteriaDamage = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetRealModeDamageStabilityCriteriaCurrent");
                Models.BO.clsGlobVar.dtRealStabilitySummary = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetRealModeStabilitySummary");
                Models.BO.clsGlobVar.dtRealLongitudinal = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetRealModeLongitudinalDataCurrent");
                Models.BO.clsGlobVar.dtSFandBMPermissible = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSFandBMPermissible");
                Models.BO.clsGlobVar.dtSFandBMPermissibleMax = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSFandBMPermissibleMax");
                Models.BO.clsGlobVar.dtRealSFandBMMax = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetRealSFandBMMax");
                Models.BO.clsGlobVar.dtRealdgMouldedDraft = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetRealModeMouldedDraft");//For New table Moulded Draft--Sangita
                Models.BO.clsGlobVar.dtRealdgDraft = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetRealModeDraftsReport");//For extreme dreaft
            }
            catch
            {
            }
        }

        ///  <summary> 
        ///Initializes all the Simulation Mode DataTables 
        ///</summary> 
        public static void SimulationModeData()
        {
            try
            {
                Models.BO.clsGlobVar.dtSimulationModeAllTanks = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationAllTankLoadingStatusDetails");
                DataView DV = Models.BO.clsGlobVar.dtSimulationModeAllTanks.AsDataView();
                DV.RowFilter = "Group = 'BALLAST_TANK'";
                Models.BO.clsGlobVar.dtSimulationBallastTanks = DV.ToTable();


                DV = Models.BO.clsGlobVar.dtSimulationModeAllTanks.AsDataView();
                DV.RowFilter = "Group = 'FUELOIL_TANK'";


                Models.BO.clsGlobVar.dtSimulationFuelOilTanks = DV.ToTable();

                DV = Models.BO.clsGlobVar.dtSimulationModeAllTanks.AsDataView();
                DV.RowFilter = "Group = 'FRESHWATER_TANK'";


                Models.BO.clsGlobVar.dtSimulationFreshWaterTanks = DV.ToTable();

                DV = Models.BO.clsGlobVar.dtSimulationModeAllTanks.AsDataView();
                DV.RowFilter = "Group = 'MISC_TANK'";

                Models.BO.clsGlobVar.dtSimulationMiscTanks = DV.ToTable();

                DV = Models.BO.clsGlobVar.dtSimulationModeAllTanks.AsDataView();
                DV.RowFilter = "Group = 'Compartment'";


                Models.BO.clsGlobVar.dtSimulationCompartments = DV.ToTable();

                DV = Models.BO.clsGlobVar.dtSimulationModeAllTanks.AsDataView();
                DV.RowFilter = "Group = 'WT_REGION'";


                Models.BO.clsGlobVar.dtSimulationWTRegion = DV.ToTable();
                Models.BO.clsGlobVar.SimulationFilling = DAL.clsDAL.ExecuteSPFillingSimulation("spGet_SimulationMode_TankCompartmentFillDetails");
                Models.BO.clsGlobVar.dtSimulationVariableItems = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationModeVariableDetails");
                Models.BO.clsGlobVar.dtSimulationEquillibriumValues = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationModeEquillibriumValues");
                Models.BO.clsGlobVar.dtSimulationLoadingSummary = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationModeLoadingSummaryCurrent");
                Models.BO.clsGlobVar.dtSimulationDrafts = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationModeDraftsCurrent");
                Models.BO.clsGlobVar.dtSimulationGZ = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationModeGzDataCurrent");
                Models.BO.clsGlobVar.dtSimulationGZDamaged = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationModeGzDataCurrentDamaged");
                Models.BO.clsGlobVar.dtSimulationfloodsummary = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationModeStabilityfloodSummary");

             

                Models.BO.clsGlobVar.dtSimulationHydrostatics = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationModeHydrostaticDataCurrent");
                Models.BO.clsGlobVar.dtSimulationStabilityCriteriaIntact = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationModeIntactStabilityCriteriaCurrent");
                Models.BO.clsGlobVar.dtSimulationStabilityCriteriaDamage = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationModeDamageStabilityCriteriaCurrent");
                Models.BO.clsGlobVar.dtSimulationStabilitySummary = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationModeStabilitySummary");
                Models.BO.clsGlobVar.dtSimulationLongitudinal = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationModeLongitudinalDataCurrent");
                Models.BO.clsGlobVar.dtSimulationSFandBMMax = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationSFandBMMax");
                Models.BO.clsGlobVar.dtRealHydrostatics2 = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetRealModeHydrostatics2");
                Models.BO.clsGlobVar.dtSimulationHydrostatics2 = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationModeHydrostatics2");
                Models.BO.clsGlobVar.dtsimulationDraftsReport = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationModeDraftsReport");
               // Models.BO.clsGlobVar.dtsimulationDraftsReport = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationModeDraftsReport");

                Models.BO.clsGlobVar.dtSimulationMouldedDraft = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationModeMouldedDraft");
                Models.BO.clsGlobVar.dtSoundingPer = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSoundingPercentage");
                Models.BO.clsGlobVar.dtDamagedDisplacement = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetDamagedDisplacement");
                
               Models.BO.clsGlobVar.dtgetsimulationtype = Models.BLL.clsBLL.GetEnttyDBRecs("vsGettypeValues");
            }
            catch
            {
            }
        }


        public static void SimulationModeCorrectiveData()
        {
            try
            {
                //Models.BO.clsGlobVar.dtCorrectiveSimulationEquilibriumvalues = Models.BLL.clsBLL.GetEnttyDBRecs("CorrectiveSimulationEquilibriumvalues");
                //Models.BO.clsGlobVar.dtSimulationCorrectiveEquillibriumValues = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationCorrectiveEquillibriumValues");
                //Models.BO.clsGlobVar.dtSimulationCorrectiveEquillibriumValues1 = Convert.ToString(Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationCorrectiveEquillibriumValues"));
                //Models.BO.clsGlobVar.dtCorrectiveSimulationDraftvalues = Models.BLL.clsBLL.GetEnttyDBRecs("CorrectiveSimulationDraftvalues");
                //Models.BO.clsGlobVar.dtSimulationCorrectiveGZ = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationCorrectiveGZ");
                //Models.BO.clsGlobVar.dtSimulationCorrectiveStabilityCriteriaDamage = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationCorrectiveStabilityCriteria");
                ////Models.BO.clsGlobVar.dtSimulationCorrectiveMeasures = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationCorrectiveMeasures");
                Models.BO.clsGlobVar.dtSimulationCorrectiveBallast = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationCorrectiveMeasuresBallast");
                Models.BO.clsGlobVar.dtSimulationCorrectiveDeBallast = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationCorrectiveMeasuresDeBallast");
                Models.BO.clsGlobVar.dtSimulationCorrectiveFluid = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationCorrectiveMeasuresFluidTransfer");
                Models.BO.clsGlobVar.dtSimulationCorrectiveFluidfuel = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationCorrectiveMeasuresFluidTransferfuel");
                Models.BO.clsGlobVar.dtSimulationcorrectivefloodsummary = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationModeStabilitycorrectivefloodSummary");
                //Models.BO.clsGlobVar.dtSimulationCorrectiveDamageZones = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationDamageZones");
                Models.BO.clsGlobVar.dtSimulationDeFloodingvalues = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationDFlootingCorrectiveMeasures");
                //Models.BO.clsGlobVar.dtRealDeFloodingvalues = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetRealDFlootingCorrectiveMeasures");

                //corrective data tables
                Models.BO.clsGlobVar.dtSimulationLoadingSummaryCorrective = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationModeLoadingSummaryCorrective");
                Models.BO.clsGlobVar.dtSimulationGZCorrective = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationModeGzDataCorrective");
                Models.BO.clsGlobVar.dtSimulationImersionCorrective = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationImersionParticularsCorrective");
                Models.BO.clsGlobVar.dtSimulationHydrostaticsCorrective = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationModeHydrostaticDataCorrective");
                Models.BO.clsGlobVar.dtSimulationHydrostatics2Corrective = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationModeHydrostatics2Corrective");
                Models.BO.clsGlobVar.dtsimulationDraftsReportCorrective = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationModeDraftsReportCorrective");
                Models.BO.clsGlobVar.dtSimulationMouldedDraftCorrective = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationModeMouldedDraftCorrective");
                Models.BO.clsGlobVar.dtSimulationStabilityCriteriaDamageCorrective = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationModeDamageStabilityCriteriaCorrective");

                //corrective data tables end
            }
            catch
            {
            }
        }

        public static void RealModeCorrectiveData()
        {
            //Models.BO.clsGlobVar.dtRealCorrectiveEquillibriumValues = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetRealCorrectiveEquillibriumValues");
            //Models.BO.clsGlobVar.dtRealCorrectiveGZ = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetRealCorrectiveGZ");
            //Models.BO.clsGlobVar.dtRealCorrectiveStabilityCriteriaDamage = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetRealCorrectiveStabilityCriteria");
            //Models.BO.clsGlobVar.dtRealCorrectiveMeasures = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetRealCorrectiveMeasures");
            //Models.BO.clsGlobVar.dtRealCorrectiveDamageZones = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetRealDamageZones");
        }

        public static void RealModePercentFill()
        {
            for (int i = 0; i < 503; i++ ) // 504
            {
                Models.BO.clsGlobVar.Tank_PercentFill[i] = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[i]["Percent_Full"]);
            }
             
            Models.BO.clsGlobVar.Tank1_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[0]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank2_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[1]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank3_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[2]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank4_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[3]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank5_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[4]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank6_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[5]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank7_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[6]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank8_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[7]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank9_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[8]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank10_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[9]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank11_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[10]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank12_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[11]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank13_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[12]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank14_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[13]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank15_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[14]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank16_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[15]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank17_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[16]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank18_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[17]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank19_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[18]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank20_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[19]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank21_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[20]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank22_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[21]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank23_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[22]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank24_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[23]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank25_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[24]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank26_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[25]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank27_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[26]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank28_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[27]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank29_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[28]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank30_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[29]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank31_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[30]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank32_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[31]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank33_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[32]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank34_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[33]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank35_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[34]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank36_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[35]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank37_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[36]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank38_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[37]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank39_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[38]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank40_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[39]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank41_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[40]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank42_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[41]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank43_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[42]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank44_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[43]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank45_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[44]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank46_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[45]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank47_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[46]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank48_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[47]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank49_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[48]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank50_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[49]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank51_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[50]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank52_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[51]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank53_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[52]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank54_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[53]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank55_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[54]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank56_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[55]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank57_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[56]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank58_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[57]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank59_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[58]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank60_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[59]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank61_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[60]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank62_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[61]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank63_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[62]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank64_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[63]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank65_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[64]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank66_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[65]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank67_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[66]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank68_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[67]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank69_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[68]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank70_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[69]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank71_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[70]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank72_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[71]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank73_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[72]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank74_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[73]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank75_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[74]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank76_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[75]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank77_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[76]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank78_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[77]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank79_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[78]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank80_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[79]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank81_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[80]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank82_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[81]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank83_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[82]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank84_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[83]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank85_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[84]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank86_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[85]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank87_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[86]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank88_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[87]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank89_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[88]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank90_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[89]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank91_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[90]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank92_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[91]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank93_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[92]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank94_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[93]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank95_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[94]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank96_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[95]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank97_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[96]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank98_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[97]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank99_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[98]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank100_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[99]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank101_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[100]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank102_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[101]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank103_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[102]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank104_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[103]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank105_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[104]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank106_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[105]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank107_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[106]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank108_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[107]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank109_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[108]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank110_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[109]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank111_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[110]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank112_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[111]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank113_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[112]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank114_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[113]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank115_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[114]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank116_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[115]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank117_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[116]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank118_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[117]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank119_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[118]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank120_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[119]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank121_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[120]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank122_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[121]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank123_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[122]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank124_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[123]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank125_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[124]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank126_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[125]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank127_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[126]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank128_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[127]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank129_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[128]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank130_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[129]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank131_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[130]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank132_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[131]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank133_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[132]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank134_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[133]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank135_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[134]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank136_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[135]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank137_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[136]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank138_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[137]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank139_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[138]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank140_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[139]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank141_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[140]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank142_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[141]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank143_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[142]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank144_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[143]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank145_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[144]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank146_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[145]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank147_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[146]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank148_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[147]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank149_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[148]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank150_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[149]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank151_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[150]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank152_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[151]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank153_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[152]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank154_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[153]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank155_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[154]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank156_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[155]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank157_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[156]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank158_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[157]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank159_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[158]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank160_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[159]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank161_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[160]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank162_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[161]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank163_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[162]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank164_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[163]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank165_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[164]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank166_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[165]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank167_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[166]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank168_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[167]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank169_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[168]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank170_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[169]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank171_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[160]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank172_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[171]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank173_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[172]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank174_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[173]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank175_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[174]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank176_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[175]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank177_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[176]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank178_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[177]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank179_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[178]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank180_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[179]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank181_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[180]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank182_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[181]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank183_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[182]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank184_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[183]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank185_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[184]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank186_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[185]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank187_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[186]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank188_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[187]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank189_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[188]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank190_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[189]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank191_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[190]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank192_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[191]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank193_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[192]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank194_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[193]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank195_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[194]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank196_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[195]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank197_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[196]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank198_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[197]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank199_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[198]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank200_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[199]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank201_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[200]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank202_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[201]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank203_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[202]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank204_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[203]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank205_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[204]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank206_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[205]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank207_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[206]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank208_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[207]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank209_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[208]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank210_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[209]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank211_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[210]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank212_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[211]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank213_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[212]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank214_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[213]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank215_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[214]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank216_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[215]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank217_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[216]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank218_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[217]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank219_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[218]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank220_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[219]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank221_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[220]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank222_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[221]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank223_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[222]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank224_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[223]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank225_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[224]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank226_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[225]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank227_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[226]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank228_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[227]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank229_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[228]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank230_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[229]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank231_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[230]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank232_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[231]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank233_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[232]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank234_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[233]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank235_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[234]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank236_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[235]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank237_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[236]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank238_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[237]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank239_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[238]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank240_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[239]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank241_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[240]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank242_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[241]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank243_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[242]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank244_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[243]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank245_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[244]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank246_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[245]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank247_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[246]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank248_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[247]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank249_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[248]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank250_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[249]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank251_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[250]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank252_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[251]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank253_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[252]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank254_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[253]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank255_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[254]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank256_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[255]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank257_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[256]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank258_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[257]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank259_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[258]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank260_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[259]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank261_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[260]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank262_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[261]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank263_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[262]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank264_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[263]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank265_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[264]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank266_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[265]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank267_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[266]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank268_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[267]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank269_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[268]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank270_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[269]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank271_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[270]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank272_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[271]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank273_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[272]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank274_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[273]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank275_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[274]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank276_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[275]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank277_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[276]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank278_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[277]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank279_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[278]["Percent_Full"]);


            Models.BO.clsGlobVar.Tank280_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[279]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank281_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[280]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank282_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[281]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank283_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[282]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank284_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[283]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank285_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[284]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank286_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[285]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank287_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[286]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank288_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[287]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank289_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[288]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank290_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[289]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank291_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[290]["Percent_Full"]);

            Models.BO.clsGlobVar.Tank292_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[291]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank293_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[292]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank294_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[293]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank295_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[294]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank296_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[295]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank297_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[296]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank298_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[297]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank299_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[298]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank300_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[299]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank301_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[300]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank302_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[301]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank303_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[302]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank304_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[303]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank305_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[304]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank306_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[305]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank307_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[306]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank308_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[307]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank309_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[308]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank310_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[309]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank311_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[310]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank312_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[311]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank313_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[312]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank314_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[313]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank315_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[314]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank316_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[315]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank317_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[316]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank318_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[317]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank319_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[318]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank320_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[319]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank321_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[320]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank322_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[321]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank323_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[322]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank324_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[323]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank325_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[324]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank326_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[325]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank327_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[326]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank328_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[327]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank329_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[328]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank330_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[329]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank331_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[330]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank332_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[331]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank333_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[332]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank334_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[333]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank335_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[334]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank336_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[335]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank337_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[336]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank338_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[337]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank339_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[338]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank340_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[339]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank341_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[340]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank342_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[341]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank343_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[342]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank344_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[343]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank345_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[344]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank346_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[345]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank347_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[346]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank348_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[347]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank349_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[348]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank350_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[349]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank351_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[350]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank352_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[351]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank353_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[352]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank354_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[353]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank355_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[354]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank356_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[355]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank357_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[356]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank358_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[357]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank359_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[358]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank360_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[359]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank361_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[360]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank362_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[361]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank363_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[362]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank364_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[363]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank365_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[364]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank366_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[365]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank367_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[366]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank368_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[367]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank369_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[368]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank360_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[369]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank371_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[370]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank372_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[371]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank373_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[372]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank374_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[373]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank375_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[374]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank376_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[375]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank377_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[376]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank378_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[377]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank379_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[378]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank380_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[379]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank381_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[380]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank382_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[381]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank383_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[382]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank384_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[383]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank385_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[384]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank386_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[385]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank387_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[386]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank388_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[387]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank389_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[388]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank390_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[389]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank391_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[390]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank392_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[391]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank393_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[392]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank394_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[393]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank395_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[394]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank396_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[395]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank397_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[396]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank398_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[397]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank399_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[398]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank400_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[399]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank401_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[400]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank402_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[401]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank403_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[402]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank404_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[403]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank405_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[404]["Percent_Full"]);

            Models.BO.clsGlobVar.Tank406_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[405]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank407_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[406]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank408_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[407]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank409_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[408]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank410_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[409]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank411_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[410]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank412_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[411]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank413_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[412]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank414_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[413]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank415_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[414]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank416_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[415]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank417_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[416]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank418_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[417]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank419_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[418]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank420_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[419]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank421_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[420]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank422_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[421]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank423_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[422]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank424_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[423]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank425_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[424]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank426_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[425]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank427_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[426]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank428_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[427]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank429_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[428]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank430_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[429]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank431_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[430]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank432_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[431]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank433_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[432]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank434_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[433]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank435_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[434]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank436_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[435]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank437_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[436]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank438_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[437]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank439_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[438]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank440_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[439]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank441_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[440]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank442_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[441]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank443_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[442]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank444_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[443]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank445_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[444]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank446_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[445]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank447_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[446]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank448_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[447]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank449_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[448]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank450_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[449]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank451_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[450]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank452_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[451]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank453_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[452]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank454_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[453]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank455_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[454]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank456_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[455]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank457_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[456]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank458_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[457]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank459_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[458]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank460_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[459]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank461_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[460]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank462_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[461]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank463_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[462]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank464_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[463]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank465_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[464]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank466_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[465]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank467_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[466]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank468_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[467]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank469_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[468]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank470_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[469]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank471_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[470]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank472_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[471]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank473_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[472]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank474_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[473]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank475_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[474]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank476_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[475]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank477_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[476]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank478_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[477]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank479_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[478]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank480_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[479]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank481_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[480]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank482_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[481]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank483_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[482]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank484_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[483]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank485_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[484]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank486_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[485]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank487_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[486]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank488_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[487]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank489_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[488]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank490_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[489]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank491_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[490]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank492_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[491]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank493_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[492]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank494_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[493]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank495_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[494]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank496_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[495]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank497_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[496]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank498_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[497]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank499_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[498]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank500_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[499]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank501_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[500]["Percent_Full"]);

            Models.BO.clsGlobVar.Tank502_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[501]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank503_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[502]["Percent_Full"]);
            Models.BO.clsGlobVar.Tank504_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[503]["Percent_Full"]);


        }

        public static void SimulationModePercentFill()
        {
            try
            {
                Models.BO.clsGlobVar.Tank1_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[0]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank2_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[1]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank3_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[2]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank4_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[3]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank5_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[4]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank6_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[5]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank7_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[6]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank8_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[7]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank9_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[8]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank10_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[9]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank11_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[10]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank12_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[11]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank13_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[12]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank14_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[13]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank15_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[14]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank16_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[15]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank17_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[16]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank18_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[17]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank19_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[18]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank20_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[19]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank21_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[20]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank22_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[21]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank23_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[22]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank24_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[23]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank25_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[24]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank26_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[25]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank27_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[26]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank28_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[27]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank29_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[28]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank30_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[29]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank31_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[30]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank32_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[31]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank33_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[32]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank34_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[33]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank35_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[34]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank36_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[35]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank37_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[36]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank38_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[37]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank39_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[38]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank40_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[39]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank41_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[40]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank42_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[41]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank43_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[42]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank44_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[43]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank45_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[44]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank46_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[45]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank47_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[46]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank48_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[47]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank49_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[48]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank50_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[49]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank51_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[50]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank52_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[51]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank53_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[52]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank54_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[53]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank55_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[54]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank56_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[55]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank57_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[56]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank58_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[57]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank59_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[58]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank60_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[59]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank61_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[60]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank62_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[61]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank63_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[62]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank64_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[63]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank65_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[64]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank66_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[65]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank67_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[66]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank68_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[67]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank69_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[68]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank70_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[69]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank71_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[70]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank72_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[71]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank73_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[72]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank74_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[73]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank75_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[74]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank76_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[75]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank77_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[76]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank78_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[77]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank79_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[78]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank80_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[79]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank81_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[80]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank82_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[81]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank83_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[82]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank84_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[83]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank85_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[84]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank86_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[85]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank87_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[86]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank88_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[87]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank89_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[88]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank90_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[89]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank91_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[90]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank92_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[91]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank93_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[92]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank94_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[93]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank95_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[94]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank96_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[95]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank97_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[96]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank98_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[97]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank99_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[98]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank100_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[99]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank101_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[100]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank102_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[101]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank103_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[102]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank104_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[103]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank105_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[104]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank106_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[105]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank107_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[106]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank108_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[107]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank109_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[108]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank110_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[109]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank111_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[110]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank112_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[111]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank113_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[112]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank114_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[113]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank115_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[114]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank116_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[115]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank117_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[116]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank118_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[117]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank119_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[118]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank120_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[119]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank121_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[120]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank122_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[121]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank123_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[122]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank124_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[123]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank125_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[124]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank126_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[125]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank127_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[126]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank128_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[127]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank129_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[128]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank130_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[129]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank131_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[130]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank132_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[131]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank133_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[132]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank134_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[133]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank135_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[134]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank136_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[135]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank137_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[136]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank138_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[137]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank139_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[138]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank140_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[139]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank141_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[140]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank142_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[141]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank143_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[142]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank144_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[143]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank145_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[144]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank146_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[145]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank147_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[146]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank148_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[147]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank149_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[148]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank150_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[149]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank151_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[150]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank152_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[151]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank153_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[152]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank154_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[153]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank155_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[154]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank156_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[155]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank157_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[156]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank158_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[157]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank159_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[158]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank160_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[159]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank161_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[160]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank162_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[161]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank163_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[162]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank164_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[163]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank165_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[164]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank166_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[165]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank167_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[166]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank168_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[167]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank169_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[168]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank170_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[169]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank171_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[160]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank172_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[171]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank173_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[172]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank174_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[173]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank175_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[174]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank176_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[175]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank177_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[176]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank178_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[177]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank179_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[178]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank180_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[179]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank181_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[180]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank182_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[181]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank183_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[182]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank184_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[183]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank185_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[184]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank186_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[185]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank187_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[186]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank188_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[187]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank189_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[188]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank190_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[189]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank191_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[190]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank192_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[191]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank193_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[192]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank194_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[193]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank195_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[194]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank196_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[195]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank197_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[196]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank198_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[197]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank199_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[198]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank200_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[199]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank201_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[200]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank202_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[201]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank203_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[202]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank204_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[203]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank205_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[204]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank206_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[205]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank207_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[206]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank208_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[207]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank209_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[208]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank210_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[209]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank211_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[210]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank212_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[211]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank213_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[212]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank214_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[213]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank215_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[214]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank216_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[215]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank217_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[216]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank218_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[217]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank219_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[218]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank220_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[219]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank221_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[220]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank222_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[221]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank223_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[222]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank224_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[223]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank225_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[224]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank226_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[225]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank227_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[226]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank228_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[227]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank229_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[228]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank230_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[229]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank231_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[230]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank232_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[231]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank233_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[232]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank234_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[233]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank235_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[234]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank236_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[235]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank237_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[236]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank238_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[237]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank239_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[238]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank240_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[239]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank241_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[240]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank242_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[241]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank243_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[242]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank244_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[243]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank245_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[244]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank246_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[245]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank247_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[246]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank248_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[247]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank249_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[248]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank250_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[249]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank251_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[250]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank252_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[251]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank253_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[252]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank254_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[253]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank255_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[254]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank256_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[255]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank257_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[256]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank258_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[257]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank259_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[258]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank260_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[259]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank261_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[260]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank262_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[261]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank263_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[262]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank264_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[263]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank265_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[264]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank266_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[265]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank267_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[266]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank268_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[267]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank269_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[268]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank270_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[269]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank271_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[270]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank272_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[271]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank273_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[272]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank274_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[273]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank275_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[274]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank276_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[275]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank277_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[276]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank278_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[277]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank279_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[278]["Percent_Full"]);


                Models.BO.clsGlobVar.Tank280_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[279]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank281_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[280]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank282_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[281]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank283_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[282]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank284_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[283]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank285_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[284]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank286_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[285]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank287_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[286]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank288_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[287]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank289_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[288]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank290_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[289]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank291_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[290]["Percent_Full"]);

                Models.BO.clsGlobVar.Tank292_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[291]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank293_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[292]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank294_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[293]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank295_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[294]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank296_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[295]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank297_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[296]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank298_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[297]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank299_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[298]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank300_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[299]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank301_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[300]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank302_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[301]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank303_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[302]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank304_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[303]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank305_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[304]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank306_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[305]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank307_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[306]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank308_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[307]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank309_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[308]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank310_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[309]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank311_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[310]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank312_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[311]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank313_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[312]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank314_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[313]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank315_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[314]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank316_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[315]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank317_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[316]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank318_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[317]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank319_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[318]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank320_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[319]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank321_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[320]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank322_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[321]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank323_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[322]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank324_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[323]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank325_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[324]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank326_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[325]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank327_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[326]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank328_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[327]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank329_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[328]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank330_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[329]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank331_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[330]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank332_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[331]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank333_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[332]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank334_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[333]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank335_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[334]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank336_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[335]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank337_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[336]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank338_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[337]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank339_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[338]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank340_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[339]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank341_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[340]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank342_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[341]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank343_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[342]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank344_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[343]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank345_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[344]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank346_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[345]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank347_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[346]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank348_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[347]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank349_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[348]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank350_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[349]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank351_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[350]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank352_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[351]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank353_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[352]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank354_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[353]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank355_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[354]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank356_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[355]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank357_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[356]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank358_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[357]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank359_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[358]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank360_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[359]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank361_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[360]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank362_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[361]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank363_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[362]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank364_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[363]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank365_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[364]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank366_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[365]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank367_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[366]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank368_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[367]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank369_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[368]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank360_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[369]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank371_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[370]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank372_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[371]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank373_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[372]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank374_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[373]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank375_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[374]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank376_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[375]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank377_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[376]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank378_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[377]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank379_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[378]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank380_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[379]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank381_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[380]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank382_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[381]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank383_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[382]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank384_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[383]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank385_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[384]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank386_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[385]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank387_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[386]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank388_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[387]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank389_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[388]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank390_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[389]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank391_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[390]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank392_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[391]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank393_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[392]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank394_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[393]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank395_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[394]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank396_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[395]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank397_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[396]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank398_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[397]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank399_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[398]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank400_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[399]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank401_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[400]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank402_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[401]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank403_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[402]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank404_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[403]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank405_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[404]["Percent_Full"]);

                Models.BO.clsGlobVar.Tank406_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[405]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank407_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[406]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank408_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[407]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank409_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[408]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank410_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[409]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank411_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[410]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank412_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[411]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank413_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[412]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank414_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[413]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank415_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[414]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank416_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[415]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank417_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[416]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank418_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[417]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank419_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[418]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank420_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[419]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank421_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[420]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank422_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[421]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank423_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[422]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank424_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[423]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank425_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[424]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank426_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[425]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank427_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[426]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank428_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[427]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank429_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[428]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank430_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[429]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank431_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[430]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank432_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[431]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank433_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[432]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank434_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[433]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank435_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[434]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank436_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[435]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank437_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[436]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank438_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[437]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank439_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[438]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank440_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[439]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank441_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[440]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank442_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[441]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank443_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[442]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank444_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[443]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank445_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[444]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank446_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[445]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank447_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[446]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank448_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[447]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank449_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[448]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank450_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[449]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank451_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[450]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank452_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[451]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank453_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[452]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank454_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[453]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank455_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[454]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank456_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[455]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank457_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[456]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank458_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[457]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank459_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[458]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank460_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[459]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank461_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[460]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank462_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[461]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank463_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[462]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank464_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[463]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank465_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[464]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank466_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[465]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank467_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[466]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank468_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[467]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank469_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[468]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank470_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[469]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank471_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[470]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank472_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[471]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank473_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[472]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank474_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[473]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank475_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[474]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank476_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[475]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank477_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[476]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank478_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[477]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank479_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[478]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank480_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[479]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank481_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[480]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank482_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[481]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank483_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[482]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank484_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[483]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank485_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[484]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank486_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[485]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank487_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[486]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank488_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[487]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank489_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[488]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank490_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[489]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank491_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[490]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank492_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[491]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank493_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[492]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank494_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[493]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank495_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[494]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank496_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[495]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank497_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[496]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank498_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[497]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank499_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[498]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank500_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[499]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank501_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[500]["Percent_Full"]);

                Models.BO.clsGlobVar.Tank502_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[501]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank503_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[502]["Percent_Full"]);
                Models.BO.clsGlobVar.Tank504_SimulationPercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.SimulationFilling.Rows[503]["Percent_Full"]);
            }
            catch (Exception)
            {
                
            }
           
        }

        public static void RealmodeCorrectiveFill()
        {
            

        }

        public static void CoordinateData()
        {
            try
            {
                #region PlanA
                string sCmd = "Select * from tbl_GA_Plan_A";

                DbCommand command = Models.DAL.clsDBUtilityMethods.GetCommand();
                command.CommandText = sCmd;
                command.CommandType = CommandType.Text;
                string Err = "";
                DataTable dtCoordinatesPlanA = new DataTable();
                dtCoordinatesPlanA = Models.DAL.clsDBUtilityMethods.GetTable(command, Err);
                for (int i = 1; i <= 24; i++)
                {
                    string sc = Convert.ToString("X" + i);
                    string sr = Convert.ToString("Y" + i);
                                                     // X region started //
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank2x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[0][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank3x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[1][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank4ax[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[2][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank4bx[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[3][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank5ax[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[4][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank5bx[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[5][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank6ax[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[6][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank6bx[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[7][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank7x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[8][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank8ax[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[9][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank8bx[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[10][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank9x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[11][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank10x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[12][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank11ax[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[13][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank11bx[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[14][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank12ax[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[15][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank12bx[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[16][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank13ax[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[17][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank13bx[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[18][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank14ax[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[19][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank14bx[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[20][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank15ax[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[21][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank15bx[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[22][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank16ax[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[23][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank16bx[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[24][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank17ax[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[25][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank17bx[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[26][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank18x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[27][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank19x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[28][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank20x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[29][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank21x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[30][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank22x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[31][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank23x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[32][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank24x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[33][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank25x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[34][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank26x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[35][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank27x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[36][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank28x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[37][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank29ax[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[38][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank29bx[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[39][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank30x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[40][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank31ax[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[41][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank31bx[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[42][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank32ax[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[43][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank32bx[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[44][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank33x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[45][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank34x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[46][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank35ax[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[47][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank35bx[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[48][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank36ax[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[49][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank36bx[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[50][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank37ax[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[51][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank37bx[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[52][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank38ax[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[53][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank38bx[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[54][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank39ax[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[55][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank39bx[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[56][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank40x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[57][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank41x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[58][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank42x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[59][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank44x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[60][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank45ax[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[61][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank45bx[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[62][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank46ax[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[63][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank46bx[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[64][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank47x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[65][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank48x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[66][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank50x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[67][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank52x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[68][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank53x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[69][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank54x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[70][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank55x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[71][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank56x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[72][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank57ax[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[73][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank57bx[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[74][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank58ax[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[75][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank58bx[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[76][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank59ax[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[77][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank59bx[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[78][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank60ax[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[79][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank60bx[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[80][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank61ax[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[81][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank61bx[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[82][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank62ax[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[83][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank62bx[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[84][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank63ax[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[85][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank63bx[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[86][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank64ax[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[87][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank64bx[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[88][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank65ax[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[89][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank65bx[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[90][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank66x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[91][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank67x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[92][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank68x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[93][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank69x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[94][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank70x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[95][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank71x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[96][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank72x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[97][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank73x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[98][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank74x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[99][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank75x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[100][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank76x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[101][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank77x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[102][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank78x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[103][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank79x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[104][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank80x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[105][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank81x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[106][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank82x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[107][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank83x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[108][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank84x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[109][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank85x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[110][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank86x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[111][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank87x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[112][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank88x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[113][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank89x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[114][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank90x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[115][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank154ax[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[116][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank154bx[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[117][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank155ax[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[118][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank155bx[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[119][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank156ax[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[120][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank156bx[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[121][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank157ax[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[122][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank157bx[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[123][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank438x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[124][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank442x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[125][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank451x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[126][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank456x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[127][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank460x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[128][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank464x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[129][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank466x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[130][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank469x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[131][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank473x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[132][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank475x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[133][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank477ax[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[134][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank477bx[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[135][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank479ax[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[136][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank479bx[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[137][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank482x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[138][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank483ax[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[139][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank483bx[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[140][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank484ax[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[141][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank484bx[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[142][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank487x[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[143][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank488ax[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[144][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank488bx[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[145][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank489ax[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[146][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank489bx[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[147][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank499ax[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[148][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank499bx[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[149][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank500ax[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[150][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank500bx[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[151][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank501ax[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[152][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank501bx[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[153][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank502ax[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[154][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank502bx[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[155][sc]);

                    Models.BO.clsGlobVar.CoordinatePlanA.Tank10bx[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[156][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank7bx[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[157][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank1ax[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[158][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank43ax[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[159][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank51ax[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[160][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank53ax[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[161][sc]);



                                                   // Y started //


                    Models.BO.clsGlobVar.CoordinatePlanA.Tank2y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[0][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank3y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[1][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank4ay[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[2][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank4by[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[3][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank5ay[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[4][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank5by[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[5][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank6ay[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[6][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank6by[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[7][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank7y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[8][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank8ay[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[9][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank8by[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[10][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank9y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[11][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank10y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[12][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank11ay[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[13][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank11by[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[14][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank12ay[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[15][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank12by[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[16][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank13ay[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[17][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank13by[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[18][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank14ay[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[19][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank14by[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[20][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank15ay[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[21][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank15by[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[22][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank16ay[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[23][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank16by[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[24][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank17ay[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[25][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank17by[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[26][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank18y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[27][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank19y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[28][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank20y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[29][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank21y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[30][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank22y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[31][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank23y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[32][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank24y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[33][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank25y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[34][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank26y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[35][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank27y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[36][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank28y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[37][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank29ay[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[38][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank29by[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[39][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank30y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[40][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank31ay[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[41][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank31by[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[42][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank32ay[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[43][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank32by[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[44][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank33y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[45][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank34y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[46][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank35ay[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[47][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank35by[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[48][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank36ay[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[49][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank36by[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[50][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank37ay[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[51][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank37by[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[52][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank38ay[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[53][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank38by[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[54][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank39ay[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[55][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank39by[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[56][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank40y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[57][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank41y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[58][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank42y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[59][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank44y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[60][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank45ay[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[61][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank45by[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[62][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank46ay[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[63][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank46by[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[64][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank47y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[65][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank48y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[66][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank50y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[67][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank52y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[68][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank53y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[69][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank54y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[70][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank55y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[71][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank56y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[72][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank57ay[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[73][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank57by[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[74][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank58ay[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[75][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank58by[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[76][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank59ay[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[77][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank59by[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[78][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank60ay[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[79][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank60by[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[80][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank61ay[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[81][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank61by[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[82][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank62ay[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[83][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank62by[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[84][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank63ay[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[85][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank63by[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[86][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank64ay[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[87][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank64by[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[88][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank65ay[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[89][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank65by[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[90][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank66y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[91][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank67y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[92][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank68y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[93][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank69y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[94][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank70y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[95][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank71y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[96][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank72y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[97][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank73y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[98][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank74y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[99][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank75y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[100][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank76y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[101][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank77y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[102][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank78y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[103][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank79y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[104][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank80y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[105][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank81y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[106][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank82y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[107][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank83y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[108][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank84y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[109][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank85y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[110][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank86y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[111][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank87y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[112][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank88y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[113][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank89y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[114][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank90y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[115][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank154ay[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[116][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank154by[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[117][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank155ay[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[118][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank155by[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[119][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank156ay[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[120][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank156by[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[121][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank157ay[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[122][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank157by[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[123][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank438y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[124][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank442y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[125][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank451y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[126][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank456y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[127][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank460y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[128][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank464y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[129][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank466y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[130][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank469y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[131][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank473y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[132][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank475y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[133][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank477ay[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[134][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank477by[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[135][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank479ay[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[136][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank479by[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[137][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank482y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[138][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank483ay[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[139][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank483by[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[140][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank484ay[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[141][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank484by[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[142][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank487y[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[143][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank488ay[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[144][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank488by[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[145][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank489ay[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[146][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank489by[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[147][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank499ay[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[148][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank499by[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[149][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank500ay[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[150][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank500by[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[151][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank501ay[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[152][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank501by[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[153][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank502ay[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[154][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank502by[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[155][sr]);

                    Models.BO.clsGlobVar.CoordinatePlanA.Tank10by[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[156][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank7by[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[157][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank1ay[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[158][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank43ay[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[159][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank51ay[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[160][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanA.Tank53ay[i] = Convert.ToDouble(dtCoordinatesPlanA.Rows[161][sr]);

                }
                #endregion PlanA

                #region PlanB

                sCmd = "Select * from tbl_GA_Plan_B";

                command = Models.DAL.clsDBUtilityMethods.GetCommand();
                command.CommandText = sCmd;
                command.CommandType = CommandType.Text;
                Err = "";
                DataTable dtCoordinatesPlanB = new DataTable();
                dtCoordinatesPlanB = Models.DAL.clsDBUtilityMethods.GetTable(command, Err);
                for (int i = 1; i <= 13; i++)
                {

                    string sc = Convert.ToString("X" + i);
                    string sr = Convert.ToString("Y" + i);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank80ax[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[0][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank80bx[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[1][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank81x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[2][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank82x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[3][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank85ax[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[4][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank85bx[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[5][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank91x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[6][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank92x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[7][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank93x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[8][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank94x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[9][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank95x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[10][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank96x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[11][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank97x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[12][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank98x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[13][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank99x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[14][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank100x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[15][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank101ax[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[16][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank101bx[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[17][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank102x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[18][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank103x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[19][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank104x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[20][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank105x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[21][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank106x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[22][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank107x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[23][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank108x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[24][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank109x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[25][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank110x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[26][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank112x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[27][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank113x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[28][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank114x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[29][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank115x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[30][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank116x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[31][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank117ax[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[32][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank117bx[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[33][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank118x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[34][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank119x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[35][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank120x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[36][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank121x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[37][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank122x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[38][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank123x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[39][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank124x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[40][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank125x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[41][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank126x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[42][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank127x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[43][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank128x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[44][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank129x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[45][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank130x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[46][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank131x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[47][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank132x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[48][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank133x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[49][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank134x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[50][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank135x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[51][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank136x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[52][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank137x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[53][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank138x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[54][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank139x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[55][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank140x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[56][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank141x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[57][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank150x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[58][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank152x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[59][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank153x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[60][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank158x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[61][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank159x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[62][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank160x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[63][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank161x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[64][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank162x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[65][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank163x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[66][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank165x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[67][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank166ax[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[68][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank166bx[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[69][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank167x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[70][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank168x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[71][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank169x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[72][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank170x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[73][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank171x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[74][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank172x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[75][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank173x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[76][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank174x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[77][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank175x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[78][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank176x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[79][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank177ax[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[80][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank177bx[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[81][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank178x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[82][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank179x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[83][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank181ax[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[84][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank181bx[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[85][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank182x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[86][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank183x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[87][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank184x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[88][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank185x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[89][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank187ax[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[90][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank187bx[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[91][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank188x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[92][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank189x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[93][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank190x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[94][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank191x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[95][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank192x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[96][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank193x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[97][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank194x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[98][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank195x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[99][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank196x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[100][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank197x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[101][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank198ax[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[102][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank198bx[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[103][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank199x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[104][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank200x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[105][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank201x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[106][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank202x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[107][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank203x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[108][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank204x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[109][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank205x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[110][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank206x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[111][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank207ax[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[112][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank207bx[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[113][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank208x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[114][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank209x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[115][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank210x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[116][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank211x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[117][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank212x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[118][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank213x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[119][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank214x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[120][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank215x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[121][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank216x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[122][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank217x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[123][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank218x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[124][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank219x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[125][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank220x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[126][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank221x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[127][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank222x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[128][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank223x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[129][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank224x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[130][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank225x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[131][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank226x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[132][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank227x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[133][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank228x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[134][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank229x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[135][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank230x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[136][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank231x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[137][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank232x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[138][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank233x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[139][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank234x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[140][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank235x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[141][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank236x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[142][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank237x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[143][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank238x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[144][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank239x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[145][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank240x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[146][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank241x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[147][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank242x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[148][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank243x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[149][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank244x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[150][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank245x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[151][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank246x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[152][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank247x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[153][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank248x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[154][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank249x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[155][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank250x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[156][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank251x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[157][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank252x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[158][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank253x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[159][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank254x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[160][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank255x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[161][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank256x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[162][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank257x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[163][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank258x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[164][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank259x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[165][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank260x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[166][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank262x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[167][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank263x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[168][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank264x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[169][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank265x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[170][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank266x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[171][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank267x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[172][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank268x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[173][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank269x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[174][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank270x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[175][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank271x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[176][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank272x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[177][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank273x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[178][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank274x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[179][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank275x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[180][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank276x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[181][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank277x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[182][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank278x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[183][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank279x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[184][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank280x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[185][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank281x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[186][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank282x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[187][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank283x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[188][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank284x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[189][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank285x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[190][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank286x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[191][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank287x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[192][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank288x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[193][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank289x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[194][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank290x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[195][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank291x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[196][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank292x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[197][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank293x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[198][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank294x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[199][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank295x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[200][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank296x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[201][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank297x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[202][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank298x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[203][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank299x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[204][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank300x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[205][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank301x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[206][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank436x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[207][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank437x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[208][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank440x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[209][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank441x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[210][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank445x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[211][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank446x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[212][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank447ax[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[213][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank447bx[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[214][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank448ax[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[215][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank448bx[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[216][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank450x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[217][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank454x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[218][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank455ax[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[219][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank455bx[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[220][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank458x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[221][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank459ax[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[222][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank459bx[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[223][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank462x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[224][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank463x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[225][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank464x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[226][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank465x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[227][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank466ax[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[228][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank466bx[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[229][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank468x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[230][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank469x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[231][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank471x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[232][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank472x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[233][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank474x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[234][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank475ax[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[235][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank475bx[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[236][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank480x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[237][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank481x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[238][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank486x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[239][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank490x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[240][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank491x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[241][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank492ax[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[242][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank492bx[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[243][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank493x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[244][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank495ax[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[245][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank495bx[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[246][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank496x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[247][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank497x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[248][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank498x[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[249][sc]);


                    Models.BO.clsGlobVar.CoordinatePlanB.Tank80ay[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[0][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank80by[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[1][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank81y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[2][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank82y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[3][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank85ay[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[4][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank85by[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[5][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank91y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[6][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank92y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[7][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank93y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[8][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank94y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[9][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank95y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[10][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank96y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[11][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank97y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[12][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank98y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[13][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank99y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[14][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank100y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[15][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank101ay[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[16][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank101by[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[17][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank102y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[18][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank103y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[19][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank104y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[20][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank105y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[21][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank106y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[22][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank107y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[23][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank108y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[24][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank109y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[25][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank110y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[26][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank112y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[27][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank113y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[28][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank114y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[29][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank115y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[30][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank116y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[31][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank117ay[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[32][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank117by[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[33][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank118y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[34][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank119y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[35][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank120y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[36][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank121y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[37][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank122y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[38][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank123y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[39][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank124y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[40][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank125y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[41][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank126y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[42][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank127y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[43][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank128y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[44][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank129y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[45][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank130y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[46][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank131y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[47][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank132y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[48][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank133y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[49][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank134y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[50][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank135y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[51][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank136y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[52][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank137y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[53][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank138y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[54][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank139y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[55][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank140y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[56][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank141y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[57][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank150y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[58][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank152y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[59][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank153y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[60][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank158y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[61][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank159y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[62][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank160y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[63][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank161y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[64][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank162y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[65][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank163y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[66][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank165y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[67][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank166ay[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[68][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank166by[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[69][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank167y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[70][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank168y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[71][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank169y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[72][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank170y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[73][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank171y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[74][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank172y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[75][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank173y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[76][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank174y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[77][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank175y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[78][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank176y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[79][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank177ay[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[80][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank177by[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[81][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank178y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[82][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank179y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[83][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank181ay[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[84][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank181by[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[85][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank182y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[86][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank183y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[87][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank184y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[88][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank185y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[89][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank187ay[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[90][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank187by[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[91][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank188y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[92][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank189y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[93][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank190y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[94][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank191y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[95][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank192y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[96][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank193y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[97][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank194y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[98][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank195y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[99][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank196y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[100][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank197y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[101][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank198ay[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[102][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank198by[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[103][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank199y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[104][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank200y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[105][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank201y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[106][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank202y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[107][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank203y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[108][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank204y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[109][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank205y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[110][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank206y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[111][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank207ay[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[112][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank207by[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[113][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank208y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[114][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank209y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[115][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank210y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[116][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank211y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[117][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank212y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[118][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank213y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[119][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank214y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[120][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank215y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[121][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank216y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[122][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank217y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[123][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank218y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[124][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank219y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[125][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank220y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[126][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank221y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[127][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank222y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[128][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank223y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[129][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank224y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[130][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank225y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[131][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank226y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[132][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank227y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[133][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank228y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[134][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank229y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[135][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank230y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[136][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank231y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[137][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank232y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[138][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank233y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[139][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank234y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[140][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank235y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[141][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank236y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[142][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank237y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[143][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank238y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[144][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank239y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[145][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank240y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[146][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank241y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[147][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank242y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[148][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank243y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[149][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank244y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[150][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank245y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[151][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank246y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[152][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank247y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[153][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank248y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[154][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank249y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[155][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank250y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[156][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank251y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[157][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank252y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[158][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank253y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[159][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank254y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[160][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank255y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[161][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank256y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[162][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank257y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[163][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank258y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[164][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank259y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[165][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank260y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[166][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank262y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[167][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank263y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[168][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank264y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[169][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank265y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[170][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank266y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[171][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank267y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[172][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank268y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[173][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank269y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[174][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank270y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[175][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank271y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[176][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank272y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[177][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank273y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[178][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank274y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[179][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank275y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[180][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank276y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[181][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank277y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[182][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank278y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[183][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank279y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[184][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank280y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[185][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank281y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[186][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank282y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[187][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank283y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[188][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank284y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[189][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank285y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[190][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank286y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[191][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank287y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[192][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank288y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[193][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank289y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[194][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank290y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[195][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank291y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[196][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank292y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[197][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank293y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[198][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank294y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[199][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank295y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[200][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank296y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[201][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank297y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[202][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank298y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[203][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank299y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[204][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank300y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[205][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank301y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[206][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank436y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[207][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank437y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[208][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank440y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[209][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank441y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[210][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank445y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[211][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank446y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[212][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank447ay[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[213][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank447by[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[214][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank448ay[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[215][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank448by[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[216][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank450y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[217][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank454y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[218][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank455ay[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[219][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank455by[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[220][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank458y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[221][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank459ay[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[222][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank459by[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[223][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank462y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[224][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank463y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[225][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank464y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[226][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank465y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[227][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank466ay[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[228][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank466by[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[229][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank468y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[230][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank469y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[231][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank471y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[232][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank472y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[233][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank474y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[234][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank475ay[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[235][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank475by[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[236][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank480y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[237][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank481y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[238][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank486y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[239][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank490y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[240][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank491y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[241][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank492ay[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[242][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank492by[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[243][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank493y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[244][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank495ay[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[245][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank495by[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[246][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank496y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[247][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank497y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[248][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanB.Tank498y[i] = Convert.ToDouble(dtCoordinatesPlanB.Rows[249][sr]);




                }
                #endregion PlanB

                #region PlanC
                sCmd = "Select * from tbl_GA_Plan_C";

                command = Models.DAL.clsDBUtilityMethods.GetCommand();
                command.CommandText = sCmd;
                command.CommandType = CommandType.Text;
                Err = "";
                DataTable dtCoordinatesPlanC = new DataTable();
                dtCoordinatesPlanC = Models.DAL.clsDBUtilityMethods.GetTable(command, Err);

                for (int i = 1; i <= 13; i++)
                {

                    string sc = Convert.ToString("X" + i);
                    string sr = Convert.ToString("Y" + i);

                    Models.BO.clsGlobVar.CoordinatePlanC.Tank142x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[0][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank143x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[1][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank144x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[2][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank148x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[3][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank149ax[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[4][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank149bx[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[5][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank178x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[6][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank188x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[7][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank302x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[8][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank303x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[9][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank304x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[10][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank305x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[11][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank306x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[12][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank307x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[13][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank308x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[14][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank309x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[15][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank310x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[16][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank311x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[17][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank312x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[18][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank313x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[19][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank314x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[20][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank315x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[21][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank316x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[22][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank318x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[23][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank319x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[24][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank320x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[25][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank321x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[26][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank322x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[27][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank323x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[28][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank324x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[29][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank325x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[30][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank326x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[31][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank327x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[32][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank328x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[33][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank329x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[34][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank330x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[35][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank331x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[36][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank332x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[37][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank333x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[38][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank334x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[39][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank335x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[40][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank336x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[41][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank337x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[42][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank338x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[43][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank339x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[44][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank340x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[45][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank341x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[46][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank342x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[47][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank343x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[48][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank344x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[49][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank345x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[50][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank346x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[51][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank347x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[52][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank348x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[53][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank349x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[54][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank350x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[55][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank351x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[56][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank352x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[57][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank353x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[58][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank354x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[59][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank355x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[60][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank356x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[61][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank357x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[62][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank358x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[63][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank359x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[64][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank360x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[65][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank362x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[66][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank363x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[67][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank364x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[68][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank365x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[69][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank366x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[70][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank367x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[71][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank368x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[72][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank369x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[73][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank370x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[74][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank371x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[75][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank372x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[76][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank373x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[77][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank374x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[78][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank375x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[79][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank376x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[80][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank377x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[81][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank378x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[82][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank379x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[83][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank380x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[84][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank381x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[85][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank382x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[86][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank383x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[87][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank384x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[88][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank385x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[89][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank386x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[90][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank387x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[91][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank388x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[92][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank389x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[93][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank390x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[94][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank391x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[95][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank392x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[96][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank393x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[97][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank394x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[98][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank395x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[99][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank396x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[100][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank397x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[101][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank399x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[102][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank400x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[103][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank401x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[104][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank402x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[105][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank403x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[106][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank404x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[107][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank405x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[108][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank406x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[109][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank407x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[110][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank408x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[111][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank409x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[112][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank410x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[113][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank411x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[114][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank412x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[115][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank413x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[116][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank414x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[117][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank415x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[118][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank416ax[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[119][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank416bx[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[120][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank417x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[121][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank418x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[122][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank419x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[123][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank420x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[124][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank421x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[125][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank423x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[126][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank424x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[127][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank425x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[128][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank426x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[129][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank427x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[130][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank428x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[131][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank429x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[132][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank430x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[133][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank431x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[134][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank432x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[135][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank433x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[136][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank434x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[137][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank435x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[138][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank439x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[139][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank443x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[140][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank448x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[141][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank449x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[142][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank452x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[143][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank453x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[144][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank457x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[145][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank461x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[146][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank466x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[147][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank467x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[148][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank470x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[149][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank475ax[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[150][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank475bx[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[151][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank476x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[152][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank478x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[153][sc]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank494x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[154][sc]);


                    Models.BO.clsGlobVar.CoordinatePlanC.Tank142y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[0][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank143y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[1][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank144y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[2][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank148y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[3][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank149ay[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[4][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank149by[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[5][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank178y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[6][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank188y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[7][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank302y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[8][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank303y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[9][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank304y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[10][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank305y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[11][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank306y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[12][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank307y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[13][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank308y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[14][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank309y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[15][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank310y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[16][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank311y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[17][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank312y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[18][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank313y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[19][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank314y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[20][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank315y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[21][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank316y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[22][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank318y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[23][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank319y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[24][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank320y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[25][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank321y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[26][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank322y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[27][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank323y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[28][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank324y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[29][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank325y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[30][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank326y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[31][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank327y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[32][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank328y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[33][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank329y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[34][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank330y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[35][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank331y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[36][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank332y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[37][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank333y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[38][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank334y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[39][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank335y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[40][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank336y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[41][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank337y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[42][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank338y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[43][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank339y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[44][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank340y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[45][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank341y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[46][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank342y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[47][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank343y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[48][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank344y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[49][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank345y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[50][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank346y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[51][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank347y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[52][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank348y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[53][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank349y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[54][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank350y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[55][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank351y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[56][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank352y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[57][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank353y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[58][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank354y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[59][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank355y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[60][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank356y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[61][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank357y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[62][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank358y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[63][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank359y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[64][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank360y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[65][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank362y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[66][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank363y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[67][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank364y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[68][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank365y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[69][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank366y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[70][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank367y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[71][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank368y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[72][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank369y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[73][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank370y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[74][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank371y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[75][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank372y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[76][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank373y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[77][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank374y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[78][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank375y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[79][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank376y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[80][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank377y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[81][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank378y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[82][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank379y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[83][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank380y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[84][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank381y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[85][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank382y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[86][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank383y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[87][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank384y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[88][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank385y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[89][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank386y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[90][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank387y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[91][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank388y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[92][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank389y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[93][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank390y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[94][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank391y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[95][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank392y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[96][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank393y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[97][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank394y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[98][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank395y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[99][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank396y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[100][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank397y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[101][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank399y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[102][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank400y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[103][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank401y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[104][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank402y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[105][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank403y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[106][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank404y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[107][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank405y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[108][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank406y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[109][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank407y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[110][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank408y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[111][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank409y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[112][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank410y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[113][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank411y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[114][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank412y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[115][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank413y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[116][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank414y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[117][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank415y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[118][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank416ay[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[119][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank416by[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[120][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank417y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[121][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank418y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[122][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank419y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[123][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank420y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[124][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank421y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[125][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank423y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[126][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank424y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[127][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank425y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[128][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank426y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[129][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank427y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[130][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank428y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[131][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank429y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[132][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank430y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[133][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank431y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[134][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank432y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[135][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank433y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[136][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank434y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[137][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank435y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[138][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank439y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[139][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank443y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[140][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank448y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[141][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank449y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[142][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank452y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[143][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank453y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[144][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank457y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[145][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank461y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[146][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank466y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[147][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank467y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[148][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank470y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[149][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank475ay[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[150][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank475by[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[151][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank476y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[152][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank478y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[153][sr]);
                    Models.BO.clsGlobVar.CoordinatePlanC.Tank494y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[154][sr]);

                }

                #endregion PlanC

                #region Profile

                sCmd = "Select * from tbl_GA_Profile";
                command = Models.DAL.clsDBUtilityMethods.GetCommand();
                command.CommandText = sCmd;
                command.CommandType = CommandType.Text;
                Err = "";
                DataTable dtCoordinatesProfile = new DataTable();
                dtCoordinatesProfile = Models.DAL.clsDBUtilityMethods.GetTable(command, Err);
                for (int i = 1; i <= 4; i++)
                {

                    string sc = Convert.ToString("X" + i);
                    string sr = Convert.ToString("Y" + i);
                    int j = i;
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank1x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[0][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank2x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[1][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank9x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[2][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank10x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[3][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank17x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[4][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank18x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[5][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank20x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[6][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank21x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[7][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank22x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[8][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank26x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[9][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank28x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[10][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank33x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[11][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank39x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[12][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank40x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[13][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank42x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[14][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank44x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[15][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank46x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[16][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank49x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[17][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank50x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[18][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank53x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[19][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank54x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[20][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank56x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[21][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank57x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[22][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank66x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[23][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank67x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[24][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank68x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[25][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank69x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[26][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank70x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[27][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank76x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[28][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank79x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[29][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank80x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[30][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank82x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[31][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank84x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[32][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank90x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[33][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank92x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[34][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank96x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[35][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank101x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[36][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank147x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[37][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank150x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[38][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank151x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[39][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank177x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[40][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank184x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[41][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank185x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[42][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank187x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[43][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank192x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[44][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank195x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[45][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank227x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[46][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank234x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[47][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank236x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[48][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank244x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[49][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank250x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[50][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank253x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[51][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank254x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[52][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank260x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[53][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank261x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[54][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank263x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[55][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank264x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[56][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank267x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[57][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank273x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[58][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank275x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[59][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank276x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[60][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank280x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[61][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank281x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[62][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank284x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[63][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank285x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[64][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank292x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[65][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank293x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[66][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank297x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[67][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank298x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[68][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank299x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[69][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank301x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[70][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank303x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[71][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank304x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[72][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank311x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[73][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank312x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[74][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank325x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[75][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank326x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[76][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank336x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[77][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank356x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[78][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank360x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[79][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank380x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[80][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank392x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[81][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank411x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[82][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank424x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[83][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank427x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[84][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank428x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[85][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank430x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[86][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank432x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[87][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank433x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[88][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank434x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[89][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank503x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[90][sc]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank34x[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[91][sc]);



                    Models.BO.clsGlobVar.ProfileCoordinate.Tank1y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[0][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank2y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[1][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank9y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[2][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank10y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[3][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank17y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[4][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank18y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[5][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank20y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[6][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank21y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[7][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank22y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[8][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank26y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[9][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank28y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[10][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank33y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[11][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank39y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[12][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank40y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[13][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank42y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[14][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank44y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[15][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank46y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[16][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank49y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[17][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank50y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[18][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank53y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[19][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank54y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[20][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank56y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[21][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank57y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[22][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank66y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[23][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank67y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[24][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank68y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[25][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank69y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[26][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank70y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[27][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank76y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[28][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank79y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[29][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank80y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[30][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank82y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[31][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank84y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[32][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank90y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[33][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank92y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[34][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank96y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[35][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank101y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[36][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank147y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[37][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank150y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[38][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank151y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[39][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank177y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[40][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank184y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[41][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank185y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[42][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank187y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[43][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank192y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[44][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank195y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[45][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank227y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[46][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank234y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[47][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank236y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[48][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank244y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[49][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank250y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[50][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank253y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[51][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank254y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[52][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank260y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[53][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank261y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[54][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank263y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[55][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank264y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[56][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank267y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[57][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank273y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[58][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank275y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[59][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank276y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[60][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank280y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[61][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank281y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[62][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank284y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[63][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank285y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[64][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank292y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[65][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank293y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[66][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank297y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[67][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank298y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[68][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank299y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[69][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank301y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[70][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank303y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[71][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank304y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[72][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank311y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[73][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank312y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[74][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank325y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[75][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank326y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[76][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank336y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[77][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank356y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[78][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank360y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[79][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank380y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[80][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank392y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[81][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank411y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[82][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank424y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[83][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank427y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[84][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank428y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[85][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank430y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[86][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank432y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[87][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank433y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[88][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank503y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[90][sr]);
                    Models.BO.clsGlobVar.ProfileCoordinate.Tank34y[j] = Convert.ToDouble(dtCoordinatesProfile.Rows[91][sr]);



                }
                #endregion Profile

                #region Selection Profile
                {


                    try
                    {
                        for (int l = 0; l <= 91; l++)
                        {

                            clsGlobVar.ProfileCoordinate.Tank_ID[l] = Convert.ToInt32(dtCoordinatesProfile.Rows[l]["Tank_ID"]);
                            clsGlobVar.ProfileCoordinate.TankX_max[l] = Convert.ToInt32(dtCoordinatesProfile.Rows[l]["Xmax"]);
                            clsGlobVar.ProfileCoordinate.TankX_min[l] = Convert.ToInt32(dtCoordinatesProfile.Rows[l]["Xmin"]);
                            clsGlobVar.ProfileCoordinate.TankY_max[l] = Convert.ToInt32(dtCoordinatesProfile.Rows[l]["Ymax"]);
                            clsGlobVar.ProfileCoordinate.TankY_min[l] = Convert.ToInt32(dtCoordinatesProfile.Rows[l]["Ymin"]);
                            //clsGlobVar.ProfileCoordinate.Tank_Name[l] =  Convert.ToString(dtCoordinatesProfile.Rows[l]["Tank_Name"]);

                        }

                        for (int i = 0; i <= 91; i++)
                        {
                            for (int j = 0; j < 1; j++)
                            {
                                clsGlobVar.ProfileCoordinate.mul[i, j] = clsGlobVar.ProfileCoordinate.Tank_ID[i];
                                clsGlobVar.ProfileCoordinate.mul[i, j + 1] = clsGlobVar.ProfileCoordinate.TankX_max[i];
                                clsGlobVar.ProfileCoordinate.mul[i, j + 2] = clsGlobVar.ProfileCoordinate.TankX_min[i];
                                clsGlobVar.ProfileCoordinate.mul[i, j + 3] = clsGlobVar.ProfileCoordinate.TankY_max[i];
                                clsGlobVar.ProfileCoordinate.mul[i, j + 4] = clsGlobVar.ProfileCoordinate.TankY_min[i];

                            }


                        }

                        //for (int y = 0; y <= 91;y++)
                        //{
                        //    for (int d = 0; d < 1; d++ )
                        //    {
                        //        clsGlobVar.ProfileCoordinate.Coordinate[y,d] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank_ID[y]);
                        //        clsGlobVar.ProfileCoordinate.Coordinate[y, d + 1] = clsGlobVar.ProfileCoordinate.Tank_Name[y];

                        //    }
                        //}
                        #region
                        {

                            // clsGlobVar.ProfileCoordinate.Coordinate[0, 2] =  Convert.ToString(clsGlobVar.ProfileCoordinate.Tank1x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[1, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank2x);
                            //// string  [] gg = new string[5];


                            // //Convert.ToString( gg) = clsGlobVar.ProfileCoordinate.Coordinate[0, 2];
                            // clsGlobVar.ProfileCoordinate.Coordinate[2, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank9x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[3, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank10x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[4, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank17x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[5, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank18x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[6, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank20x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[7, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank21x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[8, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank22x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[9, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank26x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[10, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank28x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[11, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank33x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[12, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank39x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[13, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank40x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[14, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank42x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[15, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank44x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[16, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank46x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[17, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank49x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[18, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank50x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[19, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank53x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[20, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank54x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[21, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank56x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[22, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank57x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[23, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank66x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[24, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank67x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[25, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank68x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[26, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank69x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[27, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank70x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[28, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank76x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[29, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank79x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[30, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank80x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[31, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank82x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[32, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank84x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[33, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank90x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[34, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank92x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[35, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank96x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[36, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank101x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[37, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank147x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[38, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank150x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[39, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank151x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[40, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank177x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[41, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank184x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[42, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank185x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[43, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank187x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[44, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank192x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[45, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank195x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[46, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank227x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[47, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank234x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[48, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank236x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[49, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank244x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[50, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank250x);

                            // clsGlobVar.ProfileCoordinate.Coordinate[51, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank253x);

                            // clsGlobVar.ProfileCoordinate.Coordinate[52, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank254x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[53, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank260x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[54, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank261x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[55, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank263x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[56, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank264x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[57, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank267x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[58, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank273x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[59, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank275x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[60, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank276x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[61, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank280x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[62, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank281x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[63, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank284x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[64, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank285x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[65, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank292x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[66, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank293x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[67, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank297x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[68, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank298x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[69, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank299x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[70, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank301x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[71, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank303x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[72, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank304x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[73, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank311x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[74, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank312x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[75, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank325x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[76, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank326x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[77, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank336x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[78, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank356x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[79, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank360x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[80, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank380x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[81, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank392x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[82, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank411x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[83, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank424x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[84, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank427x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[85, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank428x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[86, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank430x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[87, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank432x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[88, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank433x);
                            // clsGlobVar.ProfileCoordinate.Coordinate[89, 2] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank434x);

                            // clsGlobVar.ProfileCoordinate.Coordinate[0, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank1y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[1, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank2y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[2, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank9y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[3, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank10y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[4, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank17y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[5, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank18y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[6, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank20y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[7, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank21y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[8, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank22y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[9, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank26y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[10, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank28y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[11, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank33y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[12, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank39y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[13, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank40y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[14, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank42y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[15, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank44y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[16, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank46y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[17, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank49y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[18, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank50y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[19, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank53y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[20, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank54y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[21, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank56y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[22, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank57y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[23, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank66y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[24, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank67y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[25, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank68y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[26, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank69y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[27, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank70y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[28, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank76y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[29, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank79y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[30, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank80y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[31, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank82y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[32, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank84y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[33, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank90y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[34, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank92y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[35, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank96y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[36, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank101y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[37, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank147y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[38, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank150y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[39, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank151y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[40, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank177y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[41, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank184y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[42, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank185y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[43, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank187y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[44, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank192y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[45, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank195y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[46, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank227y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[47, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank234y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[48, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank236y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[49, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank244y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[50, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank250y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[51, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank253y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[52, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank254y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[53, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank260y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[54, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank261y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[55, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank263y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[56, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank264y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[57, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank267y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[58, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank273y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[59, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank275y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[60, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank276y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[61, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank280y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[62, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank281y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[63, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank284y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[64, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank285y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[65, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank292y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[66, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank293y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[67, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank297y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[68, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank298y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[69, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank299y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[70, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank301y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[71, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank303y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[72, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank304y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[73, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank311y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[74, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank312y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[75, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank325y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[76, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank326y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[77, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank336y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[78, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank356y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[79, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank360y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[80, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank380y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[81, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank392y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[82, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank411y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[83, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank424y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[84, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank427y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[85, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank428y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[86, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank430y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[87, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank432y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[88, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank433y);
                            // clsGlobVar.ProfileCoordinate.Coordinate[89, 3] = Convert.ToString(clsGlobVar.ProfileCoordinate.Tank434y);
                        }
                        #endregion

                    }
                    catch
                    {

                    }

                }
                #endregion Selection

                #region Selection PlanA
                {


                    try
                    {
                        for (int l = 0; l <= 161; l++)
                        {

                            clsGlobVar.CoordinatePlanA.Tank_IDPlanA[l] = Convert.ToInt32(dtCoordinatesPlanA.Rows[l]["Tank_ID"]);
                            clsGlobVar.CoordinatePlanA.TankX_maxPlanA[l] = Convert.ToInt32(dtCoordinatesPlanA.Rows[l]["Xmax"]);
                            clsGlobVar.CoordinatePlanA.TankX_minPlanA[l] = Convert.ToInt32(dtCoordinatesPlanA.Rows[l]["Xmin"]);
                            clsGlobVar.CoordinatePlanA.TankY_maxPlanA[l] = Convert.ToInt32(dtCoordinatesPlanA.Rows[l]["Ymax"]);
                            clsGlobVar.CoordinatePlanA.TankY_minPlanA[l] = Convert.ToInt32(dtCoordinatesPlanA.Rows[l]["Ymin"]);
                            //clsGlobVar.CoordinatePlanA.Tank_NamePlanA[l] = Convert.ToString(dtCoordinatesPlanA.Rows[l]["Tank_Name"]);
                            if (Convert.ToString(dtCoordinatesPlanA.Rows[l]["Group"]) == "A")
                            {
                                clsGlobVar.CoordinatePlanA.GroupName_PlanA[l] = 1;
                            }
                            else if (Convert.ToString(dtCoordinatesPlanA.Rows[l]["Group"]) == "B")
                            {
                                clsGlobVar.CoordinatePlanA.GroupName_PlanA[l] = 2;
                            }
                            else
                            { }


                        }

                        for (int i = 0; i <= 161; i++)
                        {
                            for (int j = 0; j < 1; j++)
                            {
                                clsGlobVar.CoordinatePlanA.mulPlanA[i, j] = clsGlobVar.CoordinatePlanA.Tank_IDPlanA[i];
                                clsGlobVar.CoordinatePlanA.mulPlanA[i, j + 1] = clsGlobVar.CoordinatePlanA.TankX_maxPlanA[i];
                                clsGlobVar.CoordinatePlanA.mulPlanA[i, j + 2] = clsGlobVar.CoordinatePlanA.TankX_minPlanA[i];
                                clsGlobVar.CoordinatePlanA.mulPlanA[i, j + 3] = clsGlobVar.CoordinatePlanA.TankY_maxPlanA[i];
                                clsGlobVar.CoordinatePlanA.mulPlanA[i, j + 4] = clsGlobVar.CoordinatePlanA.TankY_minPlanA[i];
                                clsGlobVar.CoordinatePlanA.mulPlanA[i, j + 5] = clsGlobVar.CoordinatePlanA.GroupName_PlanA[i];

                            }


                        }

                        //for (int y = 0; y <= 161; y++)
                        //{
                        //    for (int d = 0; d < 1; d++)
                        //    {
                        //        clsGlobVar.CoordinatePlanA.CoordinatePlnA[y, d] = Convert.ToString(clsGlobVar.CoordinatePlanA.Tank_IDPlanA[y]);
                        //        clsGlobVar.CoordinatePlanA.CoordinatePlnA[y, d + 1] = clsGlobVar.CoordinatePlanA.Tank_NamePlanA[y];

                        //    }
                        //}
                    }
                    catch
                    {

                    }

                }
                #endregion Selection

                #region Selection PlanB
                {


                    try
                    {
                        for (int l = 0; l <= 249; l++)
                        {

                            clsGlobVar.CoordinatePlanB.Tank_IDPlanB[l] = Convert.ToInt32(dtCoordinatesPlanB.Rows[l]["Tank_ID"]);
                            clsGlobVar.CoordinatePlanB.TankX_maxPlanB[l] = Convert.ToInt32(dtCoordinatesPlanB.Rows[l]["Xmax"]);
                            clsGlobVar.CoordinatePlanB.TankX_minPlanB[l] = Convert.ToInt32(dtCoordinatesPlanB.Rows[l]["Xmin"]);
                            clsGlobVar.CoordinatePlanB.TankY_maxPlanB[l] = Convert.ToInt32(dtCoordinatesPlanB.Rows[l]["Ymax"]);
                            clsGlobVar.CoordinatePlanB.TankY_minPlanB[l] = Convert.ToInt32(dtCoordinatesPlanB.Rows[l]["Ymin"]);
                            //clsGlobVar.CoordinatePlanB.Tank_NamePlanB[l] = Convert.ToString(dtCoordinatesPlanB.Rows[l]["Tank_Name"]);
                            if (Convert.ToString(dtCoordinatesPlanB.Rows[l]["Group"]) == "A")
                            {
                                clsGlobVar.CoordinatePlanB.GroupName_PlanB[l] = 1;
                            }
                            else if (Convert.ToString(dtCoordinatesPlanB.Rows[l]["Group"]) == "B")
                            {
                                clsGlobVar.CoordinatePlanB.GroupName_PlanB[l] = 2;
                            }
                            else
                            { }


                        }

                        for (int i = 0; i <= 249; i++)
                        {
                            for (int j = 0; j < 1; j++)
                            {
                                clsGlobVar.CoordinatePlanB.mulPlanB[i, j] = clsGlobVar.CoordinatePlanB.Tank_IDPlanB[i];
                                clsGlobVar.CoordinatePlanB.mulPlanB[i, j + 1] = clsGlobVar.CoordinatePlanB.TankX_maxPlanB[i];
                                clsGlobVar.CoordinatePlanB.mulPlanB[i, j + 2] = clsGlobVar.CoordinatePlanB.TankX_minPlanB[i];
                                clsGlobVar.CoordinatePlanB.mulPlanB[i, j + 3] = clsGlobVar.CoordinatePlanB.TankY_maxPlanB[i];
                                clsGlobVar.CoordinatePlanB.mulPlanB[i, j + 4] = clsGlobVar.CoordinatePlanB.TankY_minPlanB[i];
                                clsGlobVar.CoordinatePlanB.mulPlanB[i, j + 5] = clsGlobVar.CoordinatePlanB.GroupName_PlanB[i];

                            }


                        }


                    }
                    catch
                    {
                        throw;
                    }

                }
                #endregion Selection

                #region Selection PlanC
                {


                    try
                    {
                        for (int l = 0; l <= 154; l++)
                        {

                            clsGlobVar.CoordinatePlanC.Tank_IDPlanC[l] = Convert.ToInt32(dtCoordinatesPlanC.Rows[l]["Tank_ID"]);
                            clsGlobVar.CoordinatePlanC.TankX_maxPlanC[l] = Convert.ToInt32(dtCoordinatesPlanC.Rows[l]["Xmax"]);
                            clsGlobVar.CoordinatePlanC.TankX_minPlanC[l] = Convert.ToInt32(dtCoordinatesPlanC.Rows[l]["Xmin"]);
                            clsGlobVar.CoordinatePlanC.TankY_maxPlanC[l] = Convert.ToInt32(dtCoordinatesPlanC.Rows[l]["Ymax"]);
                            clsGlobVar.CoordinatePlanC.TankY_minPlanC[l] = Convert.ToInt32(dtCoordinatesPlanC.Rows[l]["Ymin"]);
                            //clsGlobVar.CoordinatePlanB.Tank_NamePlanB[l] = Convert.ToString(dtCoordinatesPlanB.Rows[l]["Tank_Name"]);
                            if (Convert.ToString(dtCoordinatesPlanC.Rows[l]["Group"]) == "A")
                            {
                                clsGlobVar.CoordinatePlanC.GroupName_PlanC[l] = 1;
                            }
                            else if (Convert.ToString(dtCoordinatesPlanC.Rows[l]["Group"]) == "B")
                            {
                                clsGlobVar.CoordinatePlanC.GroupName_PlanC[l] = 2;
                            }
                            else
                            { }


                        }

                        for (int i = 0; i <= 154; i++)
                        {
                            for (int j = 0; j < 1; j++)
                            {
                                clsGlobVar.CoordinatePlanC.mulPlanC[i, j] = clsGlobVar.CoordinatePlanC.Tank_IDPlanC[i];
                                clsGlobVar.CoordinatePlanC.mulPlanC[i, j + 1] = clsGlobVar.CoordinatePlanC.TankX_maxPlanC[i];
                                clsGlobVar.CoordinatePlanC.mulPlanC[i, j + 2] = clsGlobVar.CoordinatePlanC.TankX_minPlanC[i];
                                clsGlobVar.CoordinatePlanC.mulPlanC[i, j + 3] = clsGlobVar.CoordinatePlanC.TankY_maxPlanC[i];
                                clsGlobVar.CoordinatePlanC.mulPlanC[i, j + 4] = clsGlobVar.CoordinatePlanC.TankY_minPlanC[i];
                                clsGlobVar.CoordinatePlanC.mulPlanC[i, j + 5] = clsGlobVar.CoordinatePlanC.GroupName_PlanC[i];

                            }


                        }


                    }
                    catch
                    {

                    }

                }
                #endregion Selection

                #region MXMincurved Selection
                {
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[0, 0] = 6;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[1, 0] = 12;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[2, 0] = 13;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[3, 0] = 55;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[4, 0] = 65;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[5, 0] = 66;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[6, 0] = 301;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[7, 0] = 434;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[8, 0] = 494;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[9, 0] = 497;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[10, 0] = 498;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[11, 0] = 504;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[12, 0] = 4;

                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[0, 1] = 16504.6588 + 10477;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[1, 1] = 138038.4642 + 10477;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[2, 1] = 21756.1412 + 10477;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[3, 1] = 149040.7049 + 10477;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[4, 1] = 9002.5412 + 10477;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[5, 1] = 151124.1056 + 10477;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[6, 1] = 156101.5399 + 10477;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[7, 1] = 160806.3667 + 10477;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[8, 1] = 39761.2236 + 10477;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[9, 1] = 12003.3883 + 10477;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[10, 1] = 12003.3883 + 10477;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[11, 1] = 24006.7765 + 10477;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[12, 1] = 34506.7765;

                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[0, 2] = 12003.3883 + 10477;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[1, 2] = 133554.2546 + 10477;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[2, 2] = 18005.0824 + 10477;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[3, 2] = 138038.9649 + 10477;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[4, 2] = -422.3398 + 10477;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[5, 2] = 141039.8120 + 10477;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[6, 2] = 145541.0825 + 10477;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[7, 2] = 150042.3531 + 10477;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[8, 2] = 21756.1412 + 10477;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[9, 2] = -1950.5506 + 10477;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[10, 2] = -1099.4077 + 10477;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[11, 2] = 18005.0824 + 10477;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[12, 2] = 32256.1412;

                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[0, 3] = 5896.7434 + 5958;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[1, 3] = 11774.7089;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[2, 3] = 5896.7434 + 5958;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[3, 3] = 3251.0264 + 5958;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[4, 3] = 5896.7434 + 5958;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[5, 3] = 5774.7089 + 5958;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[6, 3] = 10926.163 + 5958;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[7, 3] = 14822.1868 + 5958;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[8, 3] = 13821.4827 + 5958;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[9, 3] = 11226.2477 + 5958;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[10, 3] = 8275.4148 + 5958;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[11, 3] = 3800.1515 + 5958;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[12, 3] = 11774.7089;



                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[0, 4] = 3649.1089 + 5958;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[1, 4] = 1075.8819 + 5958;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[2, 4] = 2451.4532 + 5958;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[3, 4] = -87.9468 + 5958;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[4, 4] = 4811.4365 + 5958;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[5, 4] = 3219.0630 + 5958;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[6, 4] = 5774.7089 + 5958;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[7, 4] = 10926.163 + 5958;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[8, 4] = 10926.1630 + 5958;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[9, 4] = 8257.9598 + 5958;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[10, 4] = 5896.7434 + 5958;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[11, 4] = 1157.4053 + 5958;
                    Models.BO.clsGlobVar.ProfileCoordinate.MxMnCurved[12, 4] = 8163.7309;
                }
                #endregion


                #region CorreActionAction
                //sCmd = "Select * from tbl_GA_Profile_Corrective";
                //command = Models.DAL.clsDBUtilityMethods.GetCommand();
                //command.CommandText = sCmd;
                //command.CommandType = CommandType.Text;
                //Err = "";
                //DataTable dtCoordinatesCorrective = new DataTable();
                //dtCoordinatesCorrective = Models.DAL.clsDBUtilityMethods.GetTable(command, Err);
                //for (int i = 1; i <= 4; i++)
                {

                    //    string sc = Convert.ToString("X" + i);
                    //    string sr = Convert.ToString("Y" + i);

                    //    Models.BO.clsGlobVar.CoordinateCorrective.wtx01[i] = Convert.ToDouble(dtCoordinatesCorrective.Rows[0][sc]);
                    //    Models.BO.clsGlobVar.CoordinateCorrective.wtx02[i] = Convert.ToDouble(dtCoordinatesCorrective.Rows[1][sc]);
                    //    Models.BO.clsGlobVar.CoordinateCorrective.wtx03[i] = Convert.ToDouble(dtCoordinatesCorrective.Rows[2][sc]);
                    //    Models.BO.clsGlobVar.CoordinateCorrective.wtx04[i] = Convert.ToDouble(dtCoordinatesCorrective.Rows[3][sc]);
                    //    Models.BO.clsGlobVar.CoordinateCorrective.wtx05[i] = Convert.ToDouble(dtCoordinatesCorrective.Rows[4][sc]);
                    //    Models.BO.clsGlobVar.CoordinateCorrective.wtx06[i] = Convert.ToDouble(dtCoordinatesCorrective.Rows[5][sc]);
                    //    Models.BO.clsGlobVar.CoordinateCorrective.wtx07[i] = Convert.ToDouble(dtCoordinatesCorrective.Rows[6][sc]);
                    //    Models.BO.clsGlobVar.CoordinateCorrective.wtx08[i] = Convert.ToDouble(dtCoordinatesCorrective.Rows[7][sc]);
                    //    Models.BO.clsGlobVar.CoordinateCorrective.wtx09[i] = Convert.ToDouble(dtCoordinatesCorrective.Rows[8][sc]);
                    //    Models.BO.clsGlobVar.CoordinateCorrective.wtx10[i] = Convert.ToDouble(dtCoordinatesCorrective.Rows[9][sc]);

                    //    Models.BO.clsGlobVar.CoordinateCorrective.wty01[i] = Convert.ToDouble(dtCoordinatesCorrective.Rows[0][sr]);
                    //    Models.BO.clsGlobVar.CoordinateCorrective.wty02[i] = Convert.ToDouble(dtCoordinatesCorrective.Rows[1][sr]);
                    //    Models.BO.clsGlobVar.CoordinateCorrective.wty03[i] = Convert.ToDouble(dtCoordinatesCorrective.Rows[2][sr]);
                    //    Models.BO.clsGlobVar.CoordinateCorrective.wty04[i] = Convert.ToDouble(dtCoordinatesCorrective.Rows[3][sr]);
                    //    Models.BO.clsGlobVar.CoordinateCorrective.wty05[i] = Convert.ToDouble(dtCoordinatesCorrective.Rows[4][sr]);
                    //    Models.BO.clsGlobVar.CoordinateCorrective.wty06[i] = Convert.ToDouble(dtCoordinatesCorrective.Rows[5][sr]);
                    //    Models.BO.clsGlobVar.CoordinateCorrective.wty07[i] = Convert.ToDouble(dtCoordinatesCorrective.Rows[6][sr]);
                    //    Models.BO.clsGlobVar.CoordinateCorrective.wty08[i] = Convert.ToDouble(dtCoordinatesCorrective.Rows[7][sr]);
                    //    Models.BO.clsGlobVar.CoordinateCorrective.wty09[i] = Convert.ToDouble(dtCoordinatesCorrective.Rows[8][sr]);
                    //    Models.BO.clsGlobVar.CoordinateCorrective.wty10[i] = Convert.ToDouble(dtCoordinatesCorrective.Rows[9][sr]);


                }

                #endregion CorreActionAction
            }
            catch (Exception)
            {
                
                throw;
            }
            
        }

        public static void LightShipData()
        {
            Models.BO.clsGlobVar.lightShipData = Models.BLL.clsBLL.GetEnttyDBRecs("vsLightShhipDataDetails");
        }


        public static void Write_Log(string message)
        {
            try
            {
                //string dateStr = "";
                //int day, month, year;
                //year = System.DateTime.Now.Year;
                //month = System.DateTime.Now.Month;
                //day = System.DateTime.Now.Day;

                //dateStr += year.ToString() + "";
                //if (month < 10) dateStr += "0";
                //dateStr += month.ToString() + "";
                //if (day < 10) dateStr += "0";
                //dateStr += day.ToString() + "";

                //writting the message

                // string logFile = Environment.CurrentDirectory + @"/LOG_" + dateStr + @".txt";
                //string logFile = @"d:\log.txt";
                //string logFile = System.Windows.Forms.Application.StartupPath + "/LOG_" + dateStr + @".txt";
                //// Application.StartupPath + "\\Images\\Ship Details.png"
                //System.IO.StreamWriter sw = new System.IO.StreamWriter(logFile, true);
                //sw.WriteLine(System.DateTime.Now.ToString() + "\t" + message);
                //sw.Close();
            }
            catch(Exception ex)
            {
                Write_Log(ex.Message);
            }
            
        }

      

    }
}
