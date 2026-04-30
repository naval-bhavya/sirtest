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
        private static void EnsureTankGridColumns(DataTable table)
        {
            if (table == null)
            {
                return;
            }

            EnsureColumn(table, "FSMType", typeof(int), 0, "FSM Type", "FSM_Type", "FSMTYPE");
            EnsureColumn(table, "Status", typeof(int), 0, "Tank_Status", "STATUS");
            EnsureLookupTextColumn(table, "FSMTypeText", "FSMType",
                new Dictionary<string, string> { { "0", "Actual" }, { "1", "MAX" }, { "2", "UserInput" } },
                "FSM Type", "FSM_Type", "FSMTYPE");
            EnsureLookupTextColumn(table, "StatusText", "Status",
                new Dictionary<string, string> { { "0", "Intact" }, { "1", "Damage" }, { "2", "Flood" } },
                "Tank_Status", "STATUS");
            EnsureColumn(table, "FloodRate", typeof(decimal), 0m, "Flood_Rate", "Flood Rate");
            EnsureColumn(table, "FloodTime", typeof(decimal), 0m, "Flood_Time", "Flood Time");
            EnsureColumn(table, "Sounding_Level", typeof(decimal), 0m, "Sounding", "Innage");
            EnsureColumn(table, "Weight", typeof(decimal), 0m, "Mass", "Mass(T)");
            EnsureColumn(table, "SG", typeof(decimal), 1m, "Specific_Gravity", "Specific Gravity");
            EnsureColumn(table, "Percent_Full", typeof(decimal), 0m, "Percent Fill", "PercentFull");
            EnsureColumn(table, "FSM", typeof(decimal), 0m, "FSMInput", "FSM (T-m)", "FSM(T-M)");
        }

        private static void EnsureColumn(DataTable table, string columnName, Type type, object defaultValue, params string[] aliases)
        {
            if (table.Columns.Contains(columnName))
            {
                return;
            }

            string sourceColumn = aliases.FirstOrDefault(alias => table.Columns.Contains(alias));
            table.Columns.Add(columnName, type);

            foreach (DataRow row in table.Rows)
            {
                object value = sourceColumn == null ? defaultValue : row[sourceColumn];
                if (value == DBNull.Value || value == null || value.ToString() == string.Empty)
                {
                    row[columnName] = defaultValue;
                    continue;
                }

                try
                {
                    row[columnName] = Convert.ChangeType(value, type);
                }
                catch
                {
                    row[columnName] = defaultValue;
                }
            }
        }

        private static void EnsureLookupTextColumn(DataTable table, string columnName, string valueColumn, Dictionary<string, string> lookup, params string[] aliases)
        {
            if (table.Columns.Contains(columnName))
            {
                return;
            }

            string sourceColumn = aliases.FirstOrDefault(alias => table.Columns.Contains(alias));
            table.Columns.Add(columnName, typeof(string));

            foreach (DataRow row in table.Rows)
            {
                object rawValue = sourceColumn == null && table.Columns.Contains(valueColumn) ? row[valueColumn] : row[sourceColumn];
                string text = rawValue == DBNull.Value || rawValue == null ? string.Empty : rawValue.ToString().Trim();
                row[columnName] = lookup.ContainsKey(text) ? lookup[text] : text;
            }
        }

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
                EnsureTankGridColumns(Models.BO.clsGlobVar.dtRealBallastTanks);

                DV = Models.BO.clsGlobVar.dtRealModeAllTanks.AsDataView();
                DV.RowFilter = "Group = 'CARGO'";
                Models.BO.clsGlobVar.dtRealCargoTanks = DV.ToTable();
                EnsureTankGridColumns(Models.BO.clsGlobVar.dtRealCargoTanks);

                DV = Models.BO.clsGlobVar.dtRealModeAllTanks.AsDataView();
                DV.RowFilter = "Group = 'FUELOIL_TANK'";
                Models.BO.clsGlobVar.dtRealFuelOilTanks = DV.ToTable();
                EnsureTankGridColumns(Models.BO.clsGlobVar.dtRealFuelOilTanks);

                DV = Models.BO.clsGlobVar.dtRealModeAllTanks.AsDataView();
                DV.RowFilter = "Group = 'FRESHWATER_TANK'";
                Models.BO.clsGlobVar.dtRealFreshWaterTanks = DV.ToTable();
                EnsureTankGridColumns(Models.BO.clsGlobVar.dtRealFreshWaterTanks);

                DV = Models.BO.clsGlobVar.dtRealModeAllTanks.AsDataView();
                DV.RowFilter = "Group = 'MISC_TANK'";
                Models.BO.clsGlobVar.dtRealMiscTanks = DV.ToTable();
                EnsureTankGridColumns(Models.BO.clsGlobVar.dtRealMiscTanks);

                DV = Models.BO.clsGlobVar.dtRealModeAllTanks.AsDataView();
                DV.RowFilter = "Group = 'Compartment'";
                Models.BO.clsGlobVar.dtRealCompartments = DV.ToTable();
                EnsureTankGridColumns(Models.BO.clsGlobVar.dtRealCompartments);

                DV=Models.BO.clsGlobVar.dtRealModeAllTanks.AsDataView();
                DV.RowFilter = "Group = 'WT_REGION'";
                Models.BO.clsGlobVar.dtRealWaterTightRegion = DV.ToTable();
                EnsureTankGridColumns(Models.BO.clsGlobVar.dtRealWaterTightRegion);


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
                
                if (Models.BO.clsGlobVar.dtSimulationModeAllTanks == null || Models.BO.clsGlobVar.dtSimulationModeAllTanks.Rows.Count == 0)
                {
                    System.Windows.MessageBox.Show("No data loaded from database. Check connection!");
                    return;
                }
                
                DataView DV = Models.BO.clsGlobVar.dtSimulationModeAllTanks.AsDataView();
                DV.RowFilter = "Group = 'BALLAST_TANK'";
                Models.BO.clsGlobVar.dtSimulationBallastTanks = DV.ToTable();
                EnsureTankGridColumns(Models.BO.clsGlobVar.dtSimulationBallastTanks);


                DV = Models.BO.clsGlobVar.dtSimulationModeAllTanks.AsDataView();
                DV.RowFilter = "Group = 'FUELOIL_TANK'";
                Models.BO.clsGlobVar.dtSimulationFuelOilTanks = DV.ToTable();
                EnsureTankGridColumns(Models.BO.clsGlobVar.dtSimulationFuelOilTanks);

                DV = Models.BO.clsGlobVar.dtSimulationModeAllTanks.AsDataView();
                DV.RowFilter = "Group = 'FRESHWATER_TANK'";
                Models.BO.clsGlobVar.dtSimulationFreshWaterTanks = DV.ToTable();
                EnsureTankGridColumns(Models.BO.clsGlobVar.dtSimulationFreshWaterTanks);

                DV = Models.BO.clsGlobVar.dtSimulationModeAllTanks.AsDataView();
                DV.RowFilter = "Group = 'DIESELOIL_TANK'";
                Models.BO.clsGlobVar.dtSimulationDisealOilTanks = DV.ToTable();
                EnsureTankGridColumns(Models.BO.clsGlobVar.dtSimulationDisealOilTanks);


                DV = Models.BO.clsGlobVar.dtSimulationModeAllTanks.AsDataView();
                DV.RowFilter = "Group = 'CARGO'";
                Models.BO.clsGlobVar.dtSimulationCargoTanks = DV.ToTable();
                EnsureTankGridColumns(Models.BO.clsGlobVar.dtSimulationCargoTanks);
                
                if (Models.BO.clsGlobVar.dtSimulationCargoTanks == null)
                {
                    Models.BO.clsGlobVar.dtSimulationCargoTanks = new DataTable();
                    EnsureTankGridColumns(Models.BO.clsGlobVar.dtSimulationCargoTanks);
                }


                DV = Models.BO.clsGlobVar.dtSimulationModeAllTanks.AsDataView();
                DV.RowFilter = "Group = 'MISC_TANK'";

                Models.BO.clsGlobVar.dtSimulationMiscTanks = DV.ToTable();
                EnsureTankGridColumns(Models.BO.clsGlobVar.dtSimulationMiscTanks);

                DV = Models.BO.clsGlobVar.dtSimulationModeAllTanks.AsDataView();
                DV.RowFilter = "Group = 'Compartment'";


                Models.BO.clsGlobVar.dtSimulationCompartments = DV.ToTable();
                EnsureTankGridColumns(Models.BO.clsGlobVar.dtSimulationCompartments);

                DV = Models.BO.clsGlobVar.dtSimulationModeAllTanks.AsDataView();
                DV.RowFilter = "Group = 'WT_REGION'";


                Models.BO.clsGlobVar.dtSimulationWTRegion = DV.ToTable();
                EnsureTankGridColumns(Models.BO.clsGlobVar.dtSimulationWTRegion);
                Models.BO.clsGlobVar.SimulationFilling = DAL.clsDAL.ExecuteSPFillingSimulation("spGet_SimulationMode_TankCompartmentFillDetails");
                Models.BO.clsGlobVar.dtSimulationVariableItems = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationModeVariableDetails");
                if (Models.BO.clsGlobVar.dtSimulationVariableItems == null)
                {
                    Models.BO.clsGlobVar.dtSimulationVariableItems = new DataTable();
                }
                Models.BO.clsGlobVar.dtSimulationEquillibriumValues = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationModeEquillibriumValues");
                Models.BO.clsGlobVar.dtSimulationLoadingSummary = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationModeLoadingSummaryCurrent");
                Models.BO.clsGlobVar.dtSimulationDrafts = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationModeDraftsCurrent");
                Models.BO.clsGlobVar.dtSimulationGZ = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationModeGzDataCurrent");
                Models.BO.clsGlobVar.dtSimulationGZDamaged = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationModeGzDataCurrentDamaged");
                Models.BO.clsGlobVar.dtSimulationfloodsummary = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationModeStabilityfloodSummary");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("SimulationModeData Error: " + ex.Message + "\n\nStackTrace: " + ex.StackTrace);
            }
        }

        ///  <summary> 
        ///Initializes all the Simulation Mode Hydrostatics
        ///</summary> 
        public static void LoadSimulationHydrostatics()
        {
            try
            {
                Models.BO.clsGlobVar.dtSimulationHydrostatics = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationModeHydrostaticDataCurrent");
                Models.BO.clsGlobVar.dtSimulationStabilityCriteriaIntact = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationModeIntactStabilityCriteriaCurrent");
                Models.BO.clsGlobVar.dtSimulationStabilityCriteriaDamage = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationModeDamageStabilityCriteriaCurrent");
                Models.BO.clsGlobVar.dtSimulationStabilitySummary = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationModeStabilitySummary");
                Models.BO.clsGlobVar.dtSimulationLongitudinal = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationModeLongitudinalDataCurrent");
                Models.BO.clsGlobVar.dtSimulationSFandBMMax = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationSFandBMMax");
                Models.BO.clsGlobVar.dtRealHydrostatics2 = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetRealModeHydrostatics2");
                Models.BO.clsGlobVar.dtSimulationHydrostatics2 = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationModeHydrostatics2");
                Models.BO.clsGlobVar.dtsimulationDraftsReport = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationModeDraftsReport");
                Models.BO.clsGlobVar.dtSimulationMouldedDraft = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSimulationModeMouldedDraft");
                Models.BO.clsGlobVar.dtSoundingPer = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetSoundingPercentage");
                if (Models.BO.clsGlobVar.dtSoundingPer == null)
                {
                    Models.BO.clsGlobVar.dtSoundingPer = new DataTable();
                }
                Models.BO.clsGlobVar.dtDamagedDisplacement = Models.BLL.clsBLL.GetEnttyDBRecs("vsGetDamagedDisplacement");
                if (Models.BO.clsGlobVar.dtDamagedDisplacement == null)
                {
                    Models.BO.clsGlobVar.dtDamagedDisplacement = new DataTable();
                }
                Models.BO.clsGlobVar.dtgetsimulationtype = Models.BLL.clsBLL.GetEnttyDBRecs("vsGettypeValues");
                if (Models.BO.clsGlobVar.dtgetsimulationtype == null)
                {
                    Models.BO.clsGlobVar.dtgetsimulationtype = new DataTable();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("LoadSimulationHydrostatics Error: " + ex.Message);
                if (Models.BO.clsGlobVar.dtSoundingPer == null)
                {
                    Models.BO.clsGlobVar.dtSoundingPer = new DataTable();
                }
                if (Models.BO.clsGlobVar.dtDamagedDisplacement == null)
                {
                    Models.BO.clsGlobVar.dtDamagedDisplacement = new DataTable();
                }
                if (Models.BO.clsGlobVar.dtgetsimulationtype == null)
                {
                    Models.BO.clsGlobVar.dtgetsimulationtype = new DataTable();
                }
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
            for (int i = 0; i < 97; i++ ) // 504
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
            //Models.BO.clsGlobVar.Tank99_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[98]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank100_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[99]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank101_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[100]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank102_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[101]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank103_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[102]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank104_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[103]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank105_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[104]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank106_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[105]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank107_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[106]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank108_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[107]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank109_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[108]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank110_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[109]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank111_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[110]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank112_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[111]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank113_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[112]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank114_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[113]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank115_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[114]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank116_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[115]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank117_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[116]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank118_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[117]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank119_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[118]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank120_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[119]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank121_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[120]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank122_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[121]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank123_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[122]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank124_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[123]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank125_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[124]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank126_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[125]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank127_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[126]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank128_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[127]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank129_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[128]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank130_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[129]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank131_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[130]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank132_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[131]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank133_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[132]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank134_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[133]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank135_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[134]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank136_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[135]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank137_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[136]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank138_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[137]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank139_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[138]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank140_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[139]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank141_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[140]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank142_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[141]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank143_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[142]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank144_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[143]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank145_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[144]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank146_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[145]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank147_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[146]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank148_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[147]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank149_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[148]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank150_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[149]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank151_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[150]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank152_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[151]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank153_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[152]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank154_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[153]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank155_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[154]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank156_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[155]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank157_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[156]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank158_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[157]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank159_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[158]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank160_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[159]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank161_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[160]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank162_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[161]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank163_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[162]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank164_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[163]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank165_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[164]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank166_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[165]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank167_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[166]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank168_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[167]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank169_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[168]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank170_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[169]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank171_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[160]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank172_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[171]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank173_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[172]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank174_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[173]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank175_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[174]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank176_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[175]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank177_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[176]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank178_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[177]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank179_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[178]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank180_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[179]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank181_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[180]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank182_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[181]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank183_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[182]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank184_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[183]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank185_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[184]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank186_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[185]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank187_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[186]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank188_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[187]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank189_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[188]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank190_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[189]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank191_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[190]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank192_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[191]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank193_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[192]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank194_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[193]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank195_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[194]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank196_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[195]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank197_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[196]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank198_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[197]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank199_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[198]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank200_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[199]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank201_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[200]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank202_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[201]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank203_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[202]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank204_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[203]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank205_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[204]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank206_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[205]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank207_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[206]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank208_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[207]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank209_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[208]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank210_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[209]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank211_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[210]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank212_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[211]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank213_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[212]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank214_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[213]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank215_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[214]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank216_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[215]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank217_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[216]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank218_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[217]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank219_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[218]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank220_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[219]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank221_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[220]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank222_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[221]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank223_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[222]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank224_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[223]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank225_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[224]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank226_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[225]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank227_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[226]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank228_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[227]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank229_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[228]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank230_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[229]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank231_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[230]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank232_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[231]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank233_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[232]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank234_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[233]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank235_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[234]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank236_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[235]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank237_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[236]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank238_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[237]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank239_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[238]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank240_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[239]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank241_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[240]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank242_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[241]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank243_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[242]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank244_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[243]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank245_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[244]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank246_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[245]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank247_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[246]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank248_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[247]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank249_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[248]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank250_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[249]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank251_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[250]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank252_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[251]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank253_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[252]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank254_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[253]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank255_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[254]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank256_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[255]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank257_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[256]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank258_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[257]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank259_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[258]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank260_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[259]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank261_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[260]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank262_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[261]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank263_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[262]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank264_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[263]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank265_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[264]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank266_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[265]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank267_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[266]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank268_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[267]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank269_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[268]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank270_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[269]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank271_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[270]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank272_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[271]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank273_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[272]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank274_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[273]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank275_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[274]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank276_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[275]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank277_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[276]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank278_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[277]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank279_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[278]["Percent_Full"]);


            //Models.BO.clsGlobVar.Tank280_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[279]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank281_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[280]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank282_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[281]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank283_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[282]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank284_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[283]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank285_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[284]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank286_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[285]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank287_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[286]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank288_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[287]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank289_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[288]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank290_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[289]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank291_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[290]["Percent_Full"]);

            //Models.BO.clsGlobVar.Tank292_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[291]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank293_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[292]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank294_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[293]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank295_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[294]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank296_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[295]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank297_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[296]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank298_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[297]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank299_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[298]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank300_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[299]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank301_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[300]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank302_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[301]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank303_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[302]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank304_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[303]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank305_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[304]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank306_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[305]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank307_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[306]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank308_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[307]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank309_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[308]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank310_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[309]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank311_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[310]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank312_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[311]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank313_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[312]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank314_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[313]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank315_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[314]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank316_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[315]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank317_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[316]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank318_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[317]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank319_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[318]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank320_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[319]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank321_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[320]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank322_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[321]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank323_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[322]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank324_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[323]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank325_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[324]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank326_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[325]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank327_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[326]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank328_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[327]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank329_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[328]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank330_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[329]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank331_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[330]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank332_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[331]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank333_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[332]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank334_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[333]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank335_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[334]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank336_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[335]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank337_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[336]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank338_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[337]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank339_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[338]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank340_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[339]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank341_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[340]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank342_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[341]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank343_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[342]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank344_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[343]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank345_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[344]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank346_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[345]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank347_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[346]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank348_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[347]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank349_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[348]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank350_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[349]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank351_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[350]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank352_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[351]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank353_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[352]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank354_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[353]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank355_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[354]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank356_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[355]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank357_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[356]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank358_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[357]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank359_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[358]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank360_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[359]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank361_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[360]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank362_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[361]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank363_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[362]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank364_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[363]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank365_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[364]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank366_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[365]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank367_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[366]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank368_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[367]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank369_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[368]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank360_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[369]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank371_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[370]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank372_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[371]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank373_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[372]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank374_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[373]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank375_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[374]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank376_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[375]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank377_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[376]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank378_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[377]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank379_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[378]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank380_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[379]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank381_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[380]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank382_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[381]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank383_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[382]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank384_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[383]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank385_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[384]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank386_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[385]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank387_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[386]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank388_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[387]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank389_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[388]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank390_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[389]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank391_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[390]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank392_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[391]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank393_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[392]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank394_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[393]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank395_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[394]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank396_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[395]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank397_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[396]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank398_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[397]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank399_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[398]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank400_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[399]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank401_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[400]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank402_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[401]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank403_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[402]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank404_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[403]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank405_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[404]["Percent_Full"]);

            //Models.BO.clsGlobVar.Tank406_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[405]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank407_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[406]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank408_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[407]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank409_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[408]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank410_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[409]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank411_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[410]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank412_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[411]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank413_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[412]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank414_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[413]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank415_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[414]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank416_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[415]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank417_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[416]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank418_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[417]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank419_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[418]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank420_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[419]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank421_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[420]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank422_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[421]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank423_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[422]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank424_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[423]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank425_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[424]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank426_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[425]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank427_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[426]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank428_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[427]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank429_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[428]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank430_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[429]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank431_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[430]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank432_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[431]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank433_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[432]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank434_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[433]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank435_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[434]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank436_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[435]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank437_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[436]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank438_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[437]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank439_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[438]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank440_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[439]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank441_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[440]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank442_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[441]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank443_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[442]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank444_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[443]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank445_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[444]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank446_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[445]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank447_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[446]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank448_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[447]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank449_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[448]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank450_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[449]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank451_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[450]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank452_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[451]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank453_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[452]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank454_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[453]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank455_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[454]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank456_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[455]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank457_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[456]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank458_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[457]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank459_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[458]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank460_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[459]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank461_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[460]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank462_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[461]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank463_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[462]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank464_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[463]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank465_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[464]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank466_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[465]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank467_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[466]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank468_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[467]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank469_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[468]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank470_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[469]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank471_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[470]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank472_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[471]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank473_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[472]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank474_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[473]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank475_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[474]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank476_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[475]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank477_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[476]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank478_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[477]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank479_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[478]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank480_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[479]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank481_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[480]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank482_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[481]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank483_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[482]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank484_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[483]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank485_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[484]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank486_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[485]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank487_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[486]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank488_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[487]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank489_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[488]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank490_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[489]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank491_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[490]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank492_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[491]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank493_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[492]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank494_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[493]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank495_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[494]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank496_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[495]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank497_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[496]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank498_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[497]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank499_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[498]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank500_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[499]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank501_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[500]["Percent_Full"]);

            //Models.BO.clsGlobVar.Tank502_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[501]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank503_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[502]["Percent_Full"]);
            //Models.BO.clsGlobVar.Tank504_PercentFill = Convert.ToDecimal(Models.BO.clsGlobVar.dtRealModeAllTanks.Rows[503]["Percent_Full"]);


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
                // Build the generic Tanks dictionary (keyed by row index).
                Models.BO.clsGlobVar.CoordinatePlanA.Tanks.Clear();
                int tankRowIdx = 0;
                foreach (DataRow tankRow in dtCoordinatesPlanA.Rows)
                {
                    var tank = new Models.BO.clsGlobVar.CoordinatePlanA.TankData();
                    tank.Tank_ID = Convert.ToInt32(tankRow["Tank_ID"]);
                    for (int k = 1; k <= 10; k++)
                    {
                        tank.X[k] = Convert.ToDouble(tankRow["X" + k]);
                        tank.Y[k] = Convert.ToDouble(tankRow["Y" + k]);
                    }
                    Models.BO.clsGlobVar.CoordinatePlanA.Tanks[tankRowIdx++] = tank;
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

                // Build the generic Tanks dictionary (keyed by row index).
                Models.BO.clsGlobVar.CoordinatePlanB.Tanks.Clear();
                int planBRowIdx = 0;
                foreach (DataRow planBRow in dtCoordinatesPlanB.Rows)
                {
                    var planBTank = new Models.BO.clsGlobVar.CoordinatePlanB.TankData();
                    planBTank.Tank_ID = Convert.ToInt32(planBRow["Tank_ID"]);
                    for (int k = 1; k <= 13; k++)
                    {
                        planBTank.X[k] = Convert.ToDouble(planBRow["X" + k]);
                        planBTank.Y[k] = Convert.ToDouble(planBRow["Y" + k]);
                    }
                    Models.BO.clsGlobVar.CoordinatePlanB.Tanks[planBRowIdx++] = planBTank;
                }
                #endregion PlanB

                //#region PlanC
                //sCmd = "Select * from tbl_GA_Plan_C";

                //command = Models.DAL.clsDBUtilityMethods.GetCommand();
                //command.CommandText = sCmd;
                //command.CommandType = CommandType.Text;
                //Err = "";
                //DataTable dtCoordinatesPlanC = new DataTable();
                //dtCoordinatesPlanC = Models.DAL.clsDBUtilityMethods.GetTable(command, Err);

                //for (int i = 1; i <= 13; i++)
                //{

                //    string sc = Convert.ToString("X" + i);
                //    string sr = Convert.ToString("Y" + i);

                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank142x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[0][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank143x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[1][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank144x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[2][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank148x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[3][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank149ax[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[4][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank149bx[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[5][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank178x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[6][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank188x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[7][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank302x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[8][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank303x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[9][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank304x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[10][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank305x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[11][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank306x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[12][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank307x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[13][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank308x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[14][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank309x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[15][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank310x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[16][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank311x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[17][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank312x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[18][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank313x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[19][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank314x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[20][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank315x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[21][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank316x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[22][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank318x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[23][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank319x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[24][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank320x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[25][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank321x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[26][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank322x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[27][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank323x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[28][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank324x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[29][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank325x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[30][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank326x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[31][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank327x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[32][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank328x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[33][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank329x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[34][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank330x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[35][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank331x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[36][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank332x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[37][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank333x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[38][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank334x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[39][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank335x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[40][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank336x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[41][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank337x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[42][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank338x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[43][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank339x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[44][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank340x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[45][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank341x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[46][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank342x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[47][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank343x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[48][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank344x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[49][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank345x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[50][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank346x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[51][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank347x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[52][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank348x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[53][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank349x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[54][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank350x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[55][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank351x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[56][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank352x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[57][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank353x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[58][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank354x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[59][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank355x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[60][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank356x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[61][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank357x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[62][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank358x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[63][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank359x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[64][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank360x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[65][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank362x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[66][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank363x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[67][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank364x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[68][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank365x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[69][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank366x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[70][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank367x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[71][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank368x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[72][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank369x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[73][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank370x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[74][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank371x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[75][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank372x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[76][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank373x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[77][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank374x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[78][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank375x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[79][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank376x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[80][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank377x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[81][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank378x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[82][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank379x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[83][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank380x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[84][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank381x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[85][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank382x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[86][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank383x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[87][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank384x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[88][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank385x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[89][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank386x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[90][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank387x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[91][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank388x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[92][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank389x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[93][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank390x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[94][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank391x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[95][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank392x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[96][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank393x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[97][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank394x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[98][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank395x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[99][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank396x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[100][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank397x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[101][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank399x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[102][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank400x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[103][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank401x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[104][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank402x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[105][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank403x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[106][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank404x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[107][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank405x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[108][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank406x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[109][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank407x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[110][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank408x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[111][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank409x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[112][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank410x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[113][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank411x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[114][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank412x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[115][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank413x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[116][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank414x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[117][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank415x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[118][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank416ax[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[119][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank416bx[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[120][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank417x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[121][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank418x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[122][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank419x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[123][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank420x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[124][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank421x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[125][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank423x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[126][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank424x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[127][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank425x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[128][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank426x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[129][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank427x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[130][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank428x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[131][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank429x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[132][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank430x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[133][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank431x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[134][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank432x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[135][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank433x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[136][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank434x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[137][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank435x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[138][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank439x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[139][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank443x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[140][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank448x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[141][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank449x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[142][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank452x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[143][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank453x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[144][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank457x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[145][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank461x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[146][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank466x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[147][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank467x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[148][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank470x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[149][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank475ax[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[150][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank475bx[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[151][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank476x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[152][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank478x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[153][sc]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank494x[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[154][sc]);


                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank142y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[0][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank143y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[1][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank144y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[2][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank148y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[3][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank149ay[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[4][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank149by[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[5][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank178y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[6][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank188y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[7][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank302y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[8][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank303y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[9][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank304y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[10][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank305y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[11][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank306y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[12][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank307y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[13][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank308y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[14][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank309y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[15][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank310y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[16][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank311y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[17][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank312y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[18][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank313y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[19][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank314y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[20][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank315y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[21][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank316y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[22][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank318y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[23][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank319y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[24][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank320y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[25][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank321y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[26][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank322y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[27][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank323y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[28][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank324y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[29][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank325y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[30][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank326y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[31][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank327y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[32][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank328y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[33][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank329y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[34][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank330y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[35][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank331y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[36][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank332y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[37][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank333y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[38][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank334y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[39][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank335y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[40][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank336y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[41][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank337y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[42][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank338y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[43][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank339y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[44][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank340y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[45][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank341y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[46][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank342y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[47][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank343y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[48][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank344y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[49][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank345y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[50][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank346y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[51][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank347y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[52][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank348y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[53][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank349y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[54][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank350y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[55][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank351y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[56][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank352y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[57][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank353y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[58][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank354y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[59][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank355y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[60][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank356y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[61][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank357y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[62][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank358y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[63][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank359y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[64][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank360y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[65][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank362y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[66][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank363y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[67][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank364y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[68][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank365y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[69][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank366y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[70][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank367y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[71][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank368y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[72][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank369y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[73][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank370y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[74][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank371y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[75][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank372y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[76][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank373y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[77][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank374y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[78][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank375y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[79][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank376y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[80][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank377y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[81][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank378y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[82][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank379y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[83][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank380y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[84][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank381y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[85][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank382y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[86][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank383y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[87][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank384y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[88][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank385y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[89][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank386y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[90][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank387y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[91][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank388y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[92][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank389y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[93][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank390y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[94][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank391y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[95][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank392y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[96][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank393y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[97][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank394y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[98][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank395y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[99][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank396y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[100][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank397y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[101][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank399y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[102][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank400y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[103][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank401y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[104][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank402y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[105][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank403y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[106][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank404y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[107][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank405y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[108][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank406y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[109][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank407y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[110][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank408y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[111][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank409y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[112][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank410y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[113][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank411y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[114][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank412y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[115][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank413y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[116][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank414y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[117][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank415y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[118][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank416ay[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[119][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank416by[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[120][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank417y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[121][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank418y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[122][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank419y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[123][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank420y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[124][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank421y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[125][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank423y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[126][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank424y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[127][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank425y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[128][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank426y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[129][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank427y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[130][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank428y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[131][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank429y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[132][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank430y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[133][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank431y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[134][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank432y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[135][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank433y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[136][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank434y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[137][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank435y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[138][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank439y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[139][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank443y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[140][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank448y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[141][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank449y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[142][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank452y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[143][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank453y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[144][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank457y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[145][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank461y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[146][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank466y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[147][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank467y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[148][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank470y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[149][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank475ay[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[150][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank475by[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[151][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank476y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[152][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank478y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[153][sr]);
                //    Models.BO.clsGlobVar.CoordinatePlanC.Tank494y[i] = Convert.ToDouble(dtCoordinatesPlanC.Rows[154][sr]);

                //}

                //#endregion PlanC

                #region Profile

                sCmd = "Select * from tbl_GA_Plan_Profile";
                command = Models.DAL.clsDBUtilityMethods.GetCommand();
                command.CommandText = sCmd;
                command.CommandType = CommandType.Text;
                Err = "";
                DataTable dtCoordinatesProfile = new DataTable();
                dtCoordinatesProfile = Models.DAL.clsDBUtilityMethods.GetTable(command, Err);

                // Build the generic Profiles dictionary (keyed by Tank_ID).
                Models.BO.clsGlobVar.ProfileCoordinate.Profiles.Clear();
                foreach (DataRow profileRow in dtCoordinatesProfile.Rows)
                {
                    int tankId = Convert.ToInt32(profileRow["Tank_ID"]);
                    var profile = new Models.BO.clsGlobVar.ProfileCoordinate.TankProfile();
                    for (int k = 1; k <= 4; k++)
                    {
                        profile.X[k] = Convert.ToDouble(profileRow["X" + k]);
                        profile.Y[k] = Convert.ToDouble(profileRow["Y" + k]);
                    }
                    Models.BO.clsGlobVar.ProfileCoordinate.Profiles[tankId] = profile;
               
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
                        for (int l = 0; l <= 97; l++)
                        {

                            clsGlobVar.CoordinatePlanA.Tank_IDPlanA[l] = Convert.ToInt32(dtCoordinatesPlanA.Rows[l]["Tank_ID"]);
                            clsGlobVar.CoordinatePlanA.TankX_maxPlanA[l] = Convert.ToInt32(dtCoordinatesPlanA.Rows[l]["Xmax"]);
                            clsGlobVar.CoordinatePlanA.TankX_minPlanA[l] = Convert.ToInt32(dtCoordinatesPlanA.Rows[l]["Xmin"]);
                            clsGlobVar.CoordinatePlanA.TankY_maxPlanA[l] = Convert.ToInt32(dtCoordinatesPlanA.Rows[l]["Ymax"]);
                            clsGlobVar.CoordinatePlanA.TankY_minPlanA[l] = Convert.ToInt32(dtCoordinatesPlanA.Rows[l]["Ymin"]);
                            //clsGlobVar.CoordinatePlanA.Tank_NamePlanA[l] = Convert.ToString(dtCoordinatesPlanA.Rows[l]["Tank_Name"]);
                            //if (Convert.ToString(dtCoordinatesPlanA.Rows[l]["Group"]) == "A")
                            //{
                            //    clsGlobVar.CoordinatePlanA.GroupName_PlanA[l] = 1;
                            //}
                            //else if (Convert.ToString(dtCoordinatesPlanA.Rows[l]["Group"]) == "B")
                            //{
                            //    clsGlobVar.CoordinatePlanA.GroupName_PlanA[l] = 2;
                            //}
                            //else
                            //{ }


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
                        for (int l = 0; l <= 97; l++)
                        {

                            clsGlobVar.CoordinatePlanB.Tank_IDPlanB[l] = Convert.ToInt32(dtCoordinatesPlanB.Rows[l]["Tank_ID"]);
                            clsGlobVar.CoordinatePlanB.TankX_maxPlanB[l] = Convert.ToInt32(dtCoordinatesPlanB.Rows[l]["Xmax"]);
                            clsGlobVar.CoordinatePlanB.TankX_minPlanB[l] = Convert.ToInt32(dtCoordinatesPlanB.Rows[l]["Xmin"]);
                            clsGlobVar.CoordinatePlanB.TankY_maxPlanB[l] = Convert.ToInt32(dtCoordinatesPlanB.Rows[l]["Ymax"]);
                            clsGlobVar.CoordinatePlanB.TankY_minPlanB[l] = Convert.ToInt32(dtCoordinatesPlanB.Rows[l]["Ymin"]);
                            //clsGlobVar.CoordinatePlanB.Tank_NamePlanB[l] = Convert.ToString(dtCoordinatesPlanB.Rows[l]["Tank_Name"]);
                            //if (Convert.ToString(dtCoordinatesPlanB.Rows[l]["Group"]) == "A")
                            //{
                            //    clsGlobVar.CoordinatePlanB.GroupName_PlanB[l] = 1;
                            //}
                            //else if (Convert.ToString(dtCoordinatesPlanB.Rows[l]["Group"]) == "B")
                            //{
                            //    clsGlobVar.CoordinatePlanB.GroupName_PlanB[l] = 2;
                            //}
                            //else
                            //{ }


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

                //#region Selection PlanC
                //{


                //    try
                //    {
                //        for (int l = 0; l <= 154; l++)
                //        {

                //            clsGlobVar.CoordinatePlanC.Tank_IDPlanC[l] = Convert.ToInt32(dtCoordinatesPlanC.Rows[l]["Tank_ID"]);
                //            clsGlobVar.CoordinatePlanC.TankX_maxPlanC[l] = Convert.ToInt32(dtCoordinatesPlanC.Rows[l]["Xmax"]);
                //            clsGlobVar.CoordinatePlanC.TankX_minPlanC[l] = Convert.ToInt32(dtCoordinatesPlanC.Rows[l]["Xmin"]);
                //            clsGlobVar.CoordinatePlanC.TankY_maxPlanC[l] = Convert.ToInt32(dtCoordinatesPlanC.Rows[l]["Ymax"]);
                //            clsGlobVar.CoordinatePlanC.TankY_minPlanC[l] = Convert.ToInt32(dtCoordinatesPlanC.Rows[l]["Ymin"]);
                //            //clsGlobVar.CoordinatePlanB.Tank_NamePlanB[l] = Convert.ToString(dtCoordinatesPlanB.Rows[l]["Tank_Name"]);
                //            if (Convert.ToString(dtCoordinatesPlanC.Rows[l]["Group"]) == "A")
                //            {
                //                clsGlobVar.CoordinatePlanC.GroupName_PlanC[l] = 1;
                //            }
                //            else if (Convert.ToString(dtCoordinatesPlanC.Rows[l]["Group"]) == "B")
                //            {
                //                clsGlobVar.CoordinatePlanC.GroupName_PlanC[l] = 2;
                //            }
                //            else
                //            { }


                //        }

                //        for (int i = 0; i <= 154; i++)
                //        {
                //            for (int j = 0; j < 1; j++)
                //            {
                //                clsGlobVar.CoordinatePlanC.mulPlanC[i, j] = clsGlobVar.CoordinatePlanC.Tank_IDPlanC[i];
                //                clsGlobVar.CoordinatePlanC.mulPlanC[i, j + 1] = clsGlobVar.CoordinatePlanC.TankX_maxPlanC[i];
                //                clsGlobVar.CoordinatePlanC.mulPlanC[i, j + 2] = clsGlobVar.CoordinatePlanC.TankX_minPlanC[i];
                //                clsGlobVar.CoordinatePlanC.mulPlanC[i, j + 3] = clsGlobVar.CoordinatePlanC.TankY_maxPlanC[i];
                //                clsGlobVar.CoordinatePlanC.mulPlanC[i, j + 4] = clsGlobVar.CoordinatePlanC.TankY_minPlanC[i];
                //                clsGlobVar.CoordinatePlanC.mulPlanC[i, j + 5] = clsGlobVar.CoordinatePlanC.GroupName_PlanC[i];

                //            }


                //        }


                //    }
                //    catch
                //    {

                //    }

                //}
                //#endregion Selection

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
