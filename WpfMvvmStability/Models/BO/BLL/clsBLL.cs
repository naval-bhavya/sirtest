using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;

namespace WpfMvvmStability.Models.BLL
{
    class clsBLL
    {
        ///  <summary> 
        ///Returns a DataTable filled with data as per EntityName which refers to sql query after calling DAL.clsDAL.GetAllRecsDT(string) 
        ///</summary> 
        ///<param name="sEntityName">EntityName which refers to Particular query</param> 
        ///<returns>Returns a DataTable filled with data as per EntityName which refers to sql query after calling DAL.clsDAL.GetAllRecsDT(string).</returns> 
        public static DataTable GetEnttyDBRecs(string sEntityName)
        {
            string sCmd1,sCmd="";
            DataTable dtable, dtHeelingArms = new DataTable();
           
            sCmd1 = "SELECT [lw1],[lw2] from [tblWindHeelSimulation_New] ";
     

            switch (sEntityName)
            {   ////////////////Real Mode///////////////
            
                case "vsGetAllTankLoadingStatusDetails":
                    sCmd = "Select M.Tank_ID, M.Tank_Name,M.Max_Volume,M.[Group], L.Sounding_Level, L.Volume, L.SG, L.Weight,L.Percent_Full, L.LCG, L.TCG, L.VCG, L.FSM ,S.IsDamaged,S.IsSensorFaulty,L.IsVisible,L.FloodTime,L.FloodRate ";  
	                sCmd+="From dbo.tblLoading_Condition L ";
	                sCmd+="JOIN dbo.tblMaster_Tank M ";
	                sCmd+="On L.Tank_ID = M.Tank_ID ";
	                sCmd+="Join tblTank_Status S ";
	                sCmd+="On L.Tank_ID = S.Tank_ID ";
	                sCmd+="ORDER BY Tank_ID";
                    break;
                case "vsGetBallastTankFillDetails":
                    sCmd = "Select   T.Tank_ID,T.Tank_Name, L.Volume, L.Percent_Full,L.SG ";
                     sCmd+=" From tblLoading_Condition L ";
                     sCmd+="Join tblMaster_Tank T ";
                     sCmd+="ON L.Tank_ID = T.Tank_ID ";
	                 sCmd+="Join tblTank_Status S ";
                     sCmd+= "On L.Tank_ID = S.Tank_ID ";
	                 sCmd+="Where T.[Group] = 'BALLAST_TANK' ORDER BY Tank_ID";
                    break;
                case "vsGetSFandBMPermissible":
                    sCmd = @"SELECT [SFAndBMId]
                                          ,[Distance]
                                          ,[BM_permissible_high]
                                          ,[BM_permissible_low]
                                          ,[SF_permissible_high]
                                          ,[SF_permissible_low]
                                      FROM [StabilityP15B].[dbo].[tbl_SFAndBM_Permissible]";
                    break;
                case "vsGetSFandBMPermissibleMax":
                    sCmd = @"  SELECT MAX(abs(BM_permissible_high)),MAX(abs(BM_permissible_low)),MAX(abs(SF_permissible_high)),MAX(abs(SF_permissible_low)) FROM [tbl_SFAndBM_Permissible]";
                    break;

                case "vsGetRealSFandBMMax":
                    sCmd = @"  SELECT [SFAndBMId]
                                      ,[Distance_SF]
                                      ,[Max_SF]
                                      ,[Distance_BM]
                                      ,[Max_BM] FROM tbl_SF_BM_Max";
                    break;
                case "vsGetFuelOilTankFillDetails":
                    sCmd = "Select T.Tank_ID, T.Tank_Name, L.Volume, L.Percent_Full,S.IsSensorFaulty,S.IsDamaged,L.SG ";
                     sCmd+=" From tblLoading_Condition L ";
                     sCmd+="Join tblMaster_Tank T ";
                     sCmd+="ON L.Tank_ID = T.Tank_ID ";
	                 sCmd+="Join tblTank_Status S ";
                     sCmd+= "On L.Tank_ID = S.Tank_ID ";
                     sCmd += "Where T.[Group] = 'FUELOIL_TANK' ORDER BY Tank_ID";
                    break;
                case "vsGetFreshWaterTankFillDetails":
                    sCmd = @"Select T.Tank_ID, T.Tank_Name, L.Volume, L.Percent_Full,S.IsSensorFaulty,S.IsDamaged,L.SG 
                                From tblLoading_Condition L 
                                Join tblMaster_Tank T 
                                ON L.Tank_ID = T.Tank_ID 
	                            Join tblTank_Status S 
                                On L.Tank_ID = S.Tank_ID 
                                Where T.[Group] = 'FRESHWATER_TANK' ORDER BY Tank_ID";
                    break;
                case "vsGetMiscTankFillDetails":
                    sCmd = "Select T.Tank_ID, T.Tank_Name, L.Volume, L.Percent_Full,S.IsSensorFaulty,S.IsDamaged,L.SG ";
                     sCmd+=" From tblLoading_Condition L ";
                     sCmd+="Join tblMaster_Tank T ";
                     sCmd+="ON L.Tank_ID = T.Tank_ID ";
	                 sCmd+="Join tblTank_Status S ";
                     sCmd+= "On L.Tank_ID = S.Tank_ID ";
                     sCmd += "Where T.[Group] = 'MISC_TANK' ORDER BY Tank_ID";
                    break;
                case "vsGetCompartmentFillDetails":
                    sCmd = "Select T.Tank_ID, T.Tank_Name, L.Volume, L.Percent_Full,S.IsSensorFaulty,S.IsDamaged,L.SG,L.FloodTime ";
                     sCmd+=" From tblLoading_Condition L ";
                     sCmd+="Join tblMaster_Tank T ";
                     sCmd+="ON L.Tank_ID = T.Tank_ID ";
	                 sCmd+="Join tblTank_Status S ";
                     sCmd+= "On L.Tank_ID = S.Tank_ID ";
                     sCmd += "Where T.[Group] = 'Compartment' ORDER BY Tank_ID";
                    break;
                case "vsGetVariableDetails":
                    sCmd = @"SELECT [Load_Id],[Load_Name],[Weight],[LCG],[TCG],[VCG],[Length],[Breadth],[Depth]
                            FROM [tblFixedLoad]";
                    break;
                case "vsGetBallastTankLoadingStatusDetails":
                    sCmd = "Select M.Tank_ID, M.Tank_Name, L.Volume, L.SG, L.Weight,L.Percent_Full, L.LCG, L.TCG, L.VCG, L.FSM ,S.IsDamaged,S.IsSensorFaulty ";  
	                sCmd+="From dbo.tblLoading_Condition L ";
	                sCmd+="JOIN dbo.tblMaster_Tank M ";
	                sCmd+="On L.Tank_ID = M.Tank_ID ";
	                sCmd+="Join tblTank_Status S ";
	                sCmd+="On L.Tank_ID = S.Tank_ID ";
	                sCmd+="Where M.[Group] = 'BALLAST_TANK' ORDER BY Tank_ID";
                    break;
                case "vsGetFuelOilTankLoadingStatusDetails":
                    sCmd = "Select M.Tank_ID, M.Tank_Name, L.Volume, L.SG, L.Weight,L.Percent_Full, L.LCG, L.TCG, L.VCG, L.FSM ,S.IsDamaged,S.IsSensorFaulty ";  
	                sCmd+="From dbo.tblLoading_Condition L ";
	                sCmd+="JOIN dbo.tblMaster_Tank M ";
	                sCmd+="On L.Tank_ID = M.Tank_ID ";
	                sCmd+="Join tblTank_Status S ";
	                sCmd+="On L.Tank_ID = S.Tank_ID ";
                    sCmd += "Where M.[Group] = 'FUELOIL_TANK' ORDER BY Tank_ID";
                    break;
                case "vsGetFreshWaterTankLoadingStatusDetails":
                    sCmd = "Select M.Tank_ID, M.Tank_Name, L.Volume, L.SG, L.Weight,L.Percent_Full, L.LCG, L.TCG, L.VCG, L.FSM ,S.IsDamaged,S.IsSensorFaulty ";  
	                sCmd+="From dbo.tblLoading_Condition L ";
	                sCmd+="JOIN dbo.tblMaster_Tank M ";
	                sCmd+="On L.Tank_ID = M.Tank_ID ";
	                sCmd+="Join tblTank_Status S ";
	                sCmd+="On L.Tank_ID = S.Tank_ID ";
                    sCmd += "Where M.[Group] = 'FRESHWATER_TANK' ORDER BY Tank_ID";
                    break;
                case "vsGetMiscTankLoadingStatusDetails":
                    sCmd = "Select M.Tank_ID, M.Tank_Name, L.Volume, L.SG, L.Weight,L.Percent_Full, L.LCG, L.TCG, L.VCG, L.FSM ,S.IsDamaged,S.IsSensorFaulty ";  
	                sCmd+="From dbo.tblLoading_Condition L ";
	                sCmd+="JOIN dbo.tblMaster_Tank M ";
	                sCmd+="On L.Tank_ID = M.Tank_ID ";
	                sCmd+="Join tblTank_Status S ";
	                sCmd+="On L.Tank_ID = S.Tank_ID ";
                    sCmd += "Where M.[Group] = 'MISC_TANK' ORDER BY Tank_ID";
                    break;
                case "vsGetCompartmentLoadingStatusDetails":
                    sCmd = "Select M.Tank_ID, M.Tank_Name, L.Volume, L.SG, L.Weight,L.Percent_Full, L.LCG, L.TCG, L.VCG, L.FSM ,S.IsDamaged,S.IsSensorFaulty,L.FloodTime,L.FloodRate ";  
	                sCmd+="From dbo.tblLoading_Condition L ";
	                sCmd+="JOIN dbo.tblMaster_Tank M ";
	                sCmd+="On L.Tank_ID = M.Tank_ID ";
	                sCmd+="Join tblTank_Status S ";
	                sCmd+="On L.Tank_ID = S.Tank_ID ";
                    sCmd += "Where M.[Group] = 'Compartment' ORDER BY Tank_ID";
                    break;
                case "vsGetRealModeEquillibriumValues":
                    sCmd = " SELECT [Stability_Values_ID],[Lightship_Weight],[Displacement],";
                    sCmd += " [Draft_Mean],[Draft_AP],[Draft_FP],[Draft_Aft_Mark],[Draft_Fore_Mark]";
                    sCmd += ",[TRIM],[Heel],[GMT],[SF],[BM],[KG(Solid)],[KG(Fluid)],[LCG],[FSC],[TPC],[MCT],Roll_Period ";
                    sCmd += "FROM tblRealMode_Equilibrium_Values";
                    break;
                case "vsGetRealModeStabilitySummary":
                    sCmd = " Select (Case M_Status When 1 Then 'OK' "; 
					sCmd+="When 0 Then 'NOT OK' ";
					sCmd+="Else 'NA' END) Stability_Status, ";
					sCmd+="Stability_Type ";
                    sCmd+="From ";					
                    sCmd+="(Select Min(Cast(C.Status As Int)) M_Status, S.Summary_Type AS Stability_Type ";
                    sCmd += "From tblRealMode_Stability_Actual_Criteria_Calc C ";
                    sCmd+="Join tblMaster_Stability_Criteria_Summary S ";
                    sCmd+="ON C.Stability_Summary_ID = S.Stability_Summary_ID ";
                    sCmd += "where  S.Summary_Type IN ('Damage', 'Intact') ";
                    sCmd+="Group By S.Summary_Type) A ";
                    sCmd += "Where A.M_Status Is Not Null ";
                    break;
                case "vsGetRealModeStabilityType":
                    sCmd = " Select Stability_Type ";
	                sCmd+="From ";					
	                sCmd+="(Select Min(Cast(C.Status As Int)) M_Status, S.Summary_Type AS Stability_Type ";
                    sCmd += "From tblRealMode_Stability_Actual_Criteria_Calc C ";
	                sCmd+="Join tblMaster_Stability_Criteria_Summary S ";
	                sCmd+="ON C.Stability_Summary_ID = S.Stability_Summary_ID ";
	                sCmd+="Where S.Summary_Type IN ('Damage', 'Intact') ";
                    sCmd += "Group By S.Summary_Type) A ";    
	                sCmd+="Where A.M_Status Is Not Null";
                    break;
                case "vsGetRealModeDraftsReport":
                    // sCmd = "Select Draft_AP,Draft_Propeller, Draft_Aft_Mark, Draft_Mean, Draft_Fore_Mark, Draft_Sonar_Dome, Draft_FP From tblSimulationMode_Equilibrium_Values Where [USER] = 'dbo'";
                    sCmd = "Select Draft_AP, Draft_Aft_Mark, Draft_Mean, Draft_Fore_Mark,Draft_FP From tblRealMode_Equilibrium_Values";
                    break;
                case "vsGetRealModeLoadingSummaryCurrent":
                    sCmd = "Select Tank_Name, Frames, Cargo, Percent_Full,  Volume, SG, [Weight], LCG, TCG, VCG, FSM,IsDamaged,Tank_ID ";
                    sCmd += "FROM ";
                    sCmd += "( ";
                    sCmd += "SELECT T.Tank_Name, ";
                    sCmd += "	(Cast(T.Frame_Init As Varchar(10)) + ' - ' + Cast(T.Frame_End As Varchar(10))) Frames,";
                    sCmd += "	T.Cargo, L.Percent_Full,  L.Volume, L.SG, L.[Weight], L.LCG, L.TCG, L.VCG, L.FSM,L.IsDamaged,L.Tank_ID, 1 'Orderby' ";
                    sCmd += "	FROM [tblLoading_Condition] L ";
                    sCmd += "	JOIN [tblMaster_Tank] T ";
                    sCmd += "	ON L.Tank_ID = T.Tank_ID ";
                    sCmd += "	Where T.[Group] In ('BALLAST_TANK', 'FRESHWATER_TANK', 'FUELOIL_TANK', 'MISC_TANK') ";
                    sCmd += "UNION ";
                    sCmd += "SELECT T.Tank_Name, ";
                    sCmd += "	(Cast(T.Frame_Init As Varchar(10)) + ' - ' + Cast(T.Frame_End As Varchar(10))) Frames, ";
                    sCmd += "	T.Cargo, L.Percent_Full,  L.Volume, L.SG, L.[Weight], L.LCG, L.TCG, L.VCG, L.FSM,L.IsDamaged,L.Tank_ID, 3 'Orderby' ";
                    sCmd += "	FROM [tblLoading_Condition] L ";
                    sCmd += "	JOIN [tblMaster_Tank] T ";
                    sCmd += "	ON L.Tank_ID = T.Tank_ID ";
                    sCmd += "	Where T.[Group] In ('Variable Data') ";
                    sCmd += "UNION ";
                    sCmd += "SELECT T.Tank_Name, ";
                    sCmd += "	(Cast(T.Frame_Init As Varchar(10)) + ' - ' + Cast(T.Frame_End As Varchar(10))) Frames, ";
                    sCmd += "	T.Cargo, L.Percent_Full,  L.Volume, L.SG, L.[Weight], L.LCG, L.TCG, L.VCG, L.FSM,L.IsDamaged,L.Tank_ID, 2 'Orderby' ";
                    sCmd += "	FROM [tblLoading_Condition] L ";
                    sCmd += "	JOIN [tblMaster_Tank] T ";
                    sCmd += "	ON L.Tank_ID = T.Tank_ID ";
                    sCmd += "	Where T.[Group] In ('Compartment','WT_REGION') And L.IsDamaged=1";
                    //sCmd += "UNION ";
                    //sCmd += "SELECT 'Deadweight' As 'Tank_Name', '' As 'Frames', '' As 'Cargo', Null As 'Percent_Full', Null As 'Volume', Null As 'SG', ";
                    //sCmd += "		Sum(L.[Weight]),";
                    //sCmd += "		CASE WHEN SUM(L.[Weight]) > 0 THEN (SUM(Lmom)/Sum(L.[Weight])) ELSE 0 END AS 'LCG',";
                    //sCmd += "		CASE WHEN SUM(L.[Weight]) > 0 THEN (SUM(Tmom)/Sum(L.[Weight])) ELSE 0 END AS 'TCG', ";
                    //sCmd += "		CASE WHEN SUM(L.[Weight]) > 0 THEN (SUM(Vmom)/Sum(L.[Weight])) ELSE 0 END AS 'VCG', ";
                    //sCmd += "		SUM(FSM) As 'FSM','' As 'IsDamaged', 4 'Orderby' ";
                    //sCmd += "	FROM [tblSimulationMode_Loading_Condition] L ";
                    //sCmd += "	JOIN [tblMaster_Tank] T";
                    //sCmd += "	ON L.Tank_ID = T.Tank_ID";
                    //sCmd += "	Where T.[Group] <> ('LightShip') ";
                    sCmd += "UNION ";
                    sCmd += "SELECT T.Tank_Name, ";
                    sCmd += "	(Cast(T.Frame_Init As Varchar(10)) + ' - ' + Cast(T.Frame_End As Varchar(10))) Frames, ";
                    sCmd += "	T.Cargo, L.Percent_Full,  L.Volume, L.SG, L.[Weight], L.LCG, L.TCG, L.VCG, L.FSM,'' As 'IsDamaged',999 AS Tank_ID, 4 'Orderby' ";
                    sCmd += "	FROM [tblLoading_Condition] L ";
                    sCmd += "	JOIN [tblMaster_Tank] T ";
                    sCmd += "	ON L.Tank_ID = T.Tank_ID ";
                    sCmd += "	Where T.[Group] In ('LightShip') ";
                    sCmd += "UNION ";
                    sCmd += "SELECT 'TOTAL DISPLACEMENT' As 'Tank_Name', '' As 'Frames', '' As 'Cargo', Null As 'Percent_Full', Null As 'Volume', Null As 'SG', ";
                    sCmd += "		Sum(L.[Weight]), ";
                    sCmd += "		(SUM(Lmom)/Sum(L.[Weight])) As 'LCG', ";
                    sCmd += "		(SUM(Tmom)/Sum(L.[Weight])) As 'TCG', ";
                    sCmd += "		(SUM(Vmom)/Sum(L.[Weight])) As 'VCG', ";
                    sCmd += "		SUM(FSM) As 'FSM','' As 'IsDamaged', 1000 AS Tank_ID, 5 'Orderby' ";
                    sCmd += "	FROM [tblLoading_Condition] L ";
                    sCmd += "	JOIN [tblMaster_Tank] T ";
                    sCmd += "	ON L.Tank_ID = T.Tank_ID ";
                    sCmd += ") A ";
                    sCmd += "Order by Tank_ID";
                    break;



                //case "vsGetRealModeLoadingSummaryCurrent":
                //    sCmd = "Select Tank_Name, Frames, Cargo, Percent_Full,  Volume, SG, [Weight], LCG, TCG, VCG, FSM,IsDamaged ";
                //    sCmd+="FROM ";
                //    sCmd+="( ";
                //    sCmd += "SELECT T.Tank_Name, "; 
                //    sCmd+="	(Cast(T.Frame_Init As Varchar(10)) + ' - ' + Cast(T.Frame_End As Varchar(10))) Frames,";  
                //    sCmd+="	T.Cargo, L.Percent_Full,  L.Volume, L.SG, L.[Weight], L.LCG, L.TCG, L.VCG, L.FSM,L.IsDamaged, 1 'Orderby' ";     
                //    sCmd+="	FROM [tblLoading_Condition] L ";
                //    sCmd+="	JOIN [tblMaster_Tank] T ";
                //    sCmd+="	ON L.Tank_ID = T.Tank_ID ";
                //    sCmd+="	Where T.[Group] In ('BALLAST_TANK', 'FRESHWATER_TANK', 'FUELOIL_TANK', 'MISC_TANK') ";
                //    sCmd+="UNION ";	
                //    sCmd+="SELECT T.Tank_Name, "; 
                //    sCmd+="	(Cast(T.Frame_Init As Varchar(10)) + ' - ' + Cast(T.Frame_End As Varchar(10))) Frames, ";  
                //    sCmd+="	T.Cargo, L.Percent_Full,  L.Volume, L.SG, L.[Weight], L.LCG, L.TCG, L.VCG, L.FSM,L.IsDamaged, 2 'Orderby' ";     
                //    sCmd+="	FROM [tblLoading_Condition] L ";
                //    sCmd+="	JOIN [tblMaster_Tank] T ";
                //    sCmd+="	ON L.Tank_ID = T.Tank_ID ";
                //    sCmd+="	Where T.[Group] In ('Variable Data') "; 
                //    sCmd+="UNION ";	
                //    sCmd+="SELECT T.Tank_Name, ";  
                //    sCmd+="	(Cast(T.Frame_Init As Varchar(10)) + ' - ' + Cast(T.Frame_End As Varchar(10))) Frames, ";  
                //    sCmd+="	T.Cargo, L.Percent_Full,  L.Volume, L.SG, L.[Weight], L.LCG, L.TCG, L.VCG, L.FSM,L.IsDamaged, 3 'Orderby' ";     
                //    sCmd+="	FROM [tblLoading_Condition] L ";
                //    sCmd+="	JOIN [tblMaster_Tank] T ";
                //    sCmd+="	ON L.Tank_ID = T.Tank_ID ";
                //    sCmd+="	Where T.[Group] In ('Compartment') And L.Volume > 0 ";
                //    //sCmd+="UNION ";
                //    //sCmd+="SELECT 'Deadweight' As 'Tank_Name', '' As 'Frames', '' As 'Cargo', Null As 'Percent_Full', Null As 'Volume', Null As 'SG', ";
                //    //sCmd+="		Sum(L.[Weight]),";
                //    //sCmd+="		CASE WHEN SUM(L.[Weight]) > 0 THEN (SUM(Lmom)/Sum(L.[Weight])) ELSE 0 END AS 'LCG',"; 
                //    //sCmd+="		CASE WHEN SUM(L.[Weight]) > 0 THEN (SUM(Tmom)/Sum(L.[Weight])) ELSE 0 END AS 'TCG', ";
                //    //sCmd+="		CASE WHEN SUM(L.[Weight]) > 0 THEN (SUM(Vmom)/Sum(L.[Weight])) ELSE 0 END AS 'VCG', ";
                //    //sCmd+="		SUM(FSM) As 'FSM', 4 'Orderby' ";     
                //    //sCmd+="	FROM [tblLoading_Condition] L ";
                //    //sCmd+="	JOIN [tblMaster_Tank] T";
                //    //sCmd+="	ON L.Tank_ID = T.Tank_ID";
                //    //sCmd+="	Where T.[Group] <> ('LightShip') ";
                //    sCmd+="UNION ";
                //    sCmd+="SELECT T.Tank_Name, "; 
                //    sCmd+="	(Cast(T.Frame_Init As Varchar(10)) + ' - ' + Cast(T.Frame_End As Varchar(10))) Frames, "; 
                //    sCmd+="	T.Cargo, L.Percent_Full,  L.Volume, L.SG, L.[Weight], L.LCG, L.TCG, L.VCG, L.FSM,'' As 'IsDamaged', 4 'Orderby' ";     
                //    sCmd+="	FROM [tblLoading_Condition] L ";
                //    sCmd+="	JOIN [tblMaster_Tank] T ";
                //    sCmd+="	ON L.Tank_ID = T.Tank_ID ";
                //    sCmd+="	Where T.[Group] In ('LightShip') "; 	
                //    sCmd+="UNION ";
                //    sCmd+="SELECT 'TOTAL DISPLACEMENT' As 'Tank_Name', '' As 'Frames', '' As 'Cargo', Null As 'Percent_Full', Null As 'Volume', Null As 'SG', ";
                //    sCmd+="		Sum(L.[Weight]), ";
                //    sCmd+="		(SUM(Lmom)/Sum(L.[Weight])) As 'LCG', ";
                //    sCmd+="		(SUM(Tmom)/Sum(L.[Weight])) As 'TCG', ";
                //    sCmd+="		(SUM(Vmom)/Sum(L.[Weight])) As 'VCG', ";
                //    sCmd+="		SUM(FSM) As 'FSM','' As 'IsDamaged', 5 'Orderby' ";    
                //    sCmd+="	FROM [tblLoading_Condition] L ";
                //    sCmd+="	JOIN [tblMaster_Tank] T ";
                //    sCmd+="	ON L.Tank_ID = T.Tank_ID ";
                //    sCmd+=") A ";
                //    sCmd+="Order by Orderby";
                //    break;
                case "vsGetRealModeGzDataCurrent":
                    sCmd = "SELECT a.heelAng as HeelAngle,a.heelGZ as GZ,b.heelArm AS WH,c.heelArm AS HL,d.heelArm AS HS,e.heelArm AS PC  from GZDataReal_New a,tblWindHeelReal b,tblHeavyLiftingReal c,tblHighSpeedReal d,tblPassengerCrowdingReal e where a.heelAng=b.heelAng and a.heelAng=c.heelAng and a.heelAng=d.heelAng and a.heelAng=e.heelAng";
                    //sCmd = "SELECT a.heelAng,a.heelGZ,d.heelArm AS HS,e.heelArm AS PC,f.lw1 as LW1,g.lw2 as LW2,h.Min_DF as Downflooding from GZData_New a,tblHighSpeed d,tblPassengerCrowding e ,tblWindHeel_New f,tblWindHeel_New g,tblMin_DF_Angle h where a.heelAng=d.heelAng and a.heelAng=e.heelAng ;"; 

                  break;

                case "vsGetRealModeGzDataCurrentDamaged":
                    sCmd="SELECT a.HeelAngle,ISNULL(a.GZ,0)GZ,b.heelArm AS WH from tbl_RealHeeledCondition_Damage a ";
                    sCmd+= "INNER JOIN tblWindHeelReal b ON a.HeelAngle=b.heelAng where  A.HeelAngle between -60 and 60";

                     break;

                case "vsGetRealModeLongitudinalDataCurrent":
                    sCmd = "SELECT [Length],BuoyanceUDL,NetUDL,SF,BM FROM tblSFAndBM order by [Length]";
                    break;
                case "vsGetRealModeIntactStabilityCriteriaCurrent":
                    sCmd = "Select S.Criterion, C.CriticalValue Critical_Value , C.Actual_Value,C.[Status] ";
		            sCmd+="From tblMaster_Stability_Criteria_Summary S ";
                    sCmd += "JOIN [tblRealMode_Stability_Actual_Criteria_Calc] C ";
		            sCmd+="ON S.Stability_Summary_ID = C.Stability_Summary_ID "; 
		            sCmd+="Where Summary_Type = 'Intact'";
                    break;
                case "vsGetRealModeDamageStabilityCriteriaCurrent":
                    sCmd = "Select S.Criterion, C.CriticalValue Critical_Value , C.Actual_Value,C.[Status] ";
		            sCmd+="From tblMaster_Stability_Criteria_Summary S ";
                    sCmd += "JOIN [tblRealMode_Stability_Actual_Criteria_Calc] C ";
		            sCmd+="ON S.Stability_Summary_ID = C.Stability_Summary_ID "; 
		            sCmd+="Where Summary_Type = 'Damage'";
                    break;
                case "vsGetRealModeHydrostaticDataCurrent":
                    sCmd = " Select Displacement, LCB,TCB,VCB,LCF,TCF,LCG,TCG,[KG(Fluid)],KMT,KML,BMT,BML,WPA,Buoyancy FROM tblRealMode_Equilibrium_Values ";        
                    //sCmd = "Select Displacement, TRIM, Heel,GMT, FSC,[KG(Solid)],[KG(Fluid)],LCG,TCG,LCF,MCT,TPC,Roll_Period From tblEquilibrium_Values";
                    break;
                case "vsGetRealModeDraftsCurrent":
                   // sCmd="Select [openingNo],[openingName],[openingX],[openingY],[openingZ],[actualImAng] From tblSimulationDownFloodingAngle";    
                    sCmd = "Select [Draft_Mean],[Draft_AP],[Draft_FP],[Draft_Aft_Mark],[Draft_Fore_Mark],Draft_Sonar_Dome, Draft_Propeller From tblRealMode_Equilibrium_Values";
                    break;

                case "vsGetRealImersionParticulars":
                    sCmd = "Select [openingNo],[openingName],[openingX],[openingY],[openingZ],[actualImAng] From tblDownFloodingAngleReal";    
                    //sCmd = "Select [Draft_Mean],[Draft_AP],[Draft_FP],[Draft_Aft_Mark],[Draft_Fore_Mark],Draft_Sonar_Dome, Draft_Propeller From tblEquilibrium_Values";
                    break;

                case "vsGetRealModeManualLoadingConditionEntriesCurrent":
                    sCmd = "Select Tank_Name, [Weight], Sounding_Level ";
		            sCmd+="From ";
			        sCmd+="(SELECT T.Tank_Name, L.[Weight], L.Sounding_Level, ";     
			        sCmd+="(Case When T.DataComingFromSensor = 1 And L.IsManualEntry = 1 Then 'Manual Input' End) IsManualInput "; 
			        sCmd+="FROM [tblLoading_Condition] L ";
			        sCmd+="JOIN [tblMaster_Tank] T ";
			        sCmd+="ON L.Tank_ID = T.Tank_ID ";
			        sCmd+="Where IsManualEntry = 1 ";
			        sCmd+=") A";
                    break;

                case "vsGetRealModeMouldedDraft":
                    sCmd = "Select [Draft_AP_MLD] as Draft_AP,[Draft_Aft_Mark_MLD] as Draft_Aft_Mark ,[Draft_Mean_MLD] as Draft_Mean ,[Draft_Fore_Mark_MLD] as Draft_Fore_Mark,[Draft_FP_MLD] as Draft_FP From tblRealMode_Equilibrium_Values";    
                   
                    break;

                case "vsGetDamagedDisplacement":
                    sCmd = "SELECT [Items],[Weight],[LCG],[TCG],[VCG] FROM tblDamagedDisp";

                    break;
                ////////////////////Simulation Mode/////////////////////////

                case "vsGetSimulationSFandBMMax":
                    sCmd = @"  SELECT [SFAndBMId]
                                      ,[Distance_SF]
                                      ,[Max_SF]
                                      ,[Distance_BM]
                                      ,[Max_BM] FROM tbl_SF_BM_Max_Simulation";
                    break;
                case "vsGetSimulationAllTankLoadingStatusDetails":
                    sCmd = "Select M.Tank_ID,M.[Group], M.Tank_Name,M.Max_Volume,L.[Sounding_Level], L.Volume, L.SG, L.Weight,L.Percent_Full, L.LCG, L.TCG, L.VCG, L.FSM ,S.IsDamaged,L.Status,L.FSMType,L.IsVisible, L.FloodRate,L.FloodTime  ";
                    sCmd += "From tblSimulationMode_Loading_Condition L ";
	                sCmd+="JOIN dbo.tblMaster_Tank M ";
	                sCmd+="On L.Tank_ID = M.Tank_ID ";
                    sCmd += "Join tblSimulationMode_Tank_Status S ";
	                sCmd+="On L.Tank_ID = S.Tank_ID ";
	                sCmd+=" ORDER BY Tank_ID";
                    break;
                case "vsGetSimulationAllTankLoadingStatusDetails1":
                    sCmd = "Select M.Tank_ID,M.[Group], M.Tank_Name,M.Max_Volume,L.[Sounding_Level], L.Volume, L.SG, L.Weight,L.Percent_Full, L.LCG, L.TCG, L.VCG, L.FSM ,L.FloodTime,S.IsDamaged  ";
                    sCmd += "From tblLoading_Condition_LB L ";
                    sCmd += "JOIN dbo.tblMaster_Tank M ";
                    sCmd += "On L.Tank_ID = M.Tank_ID ";
                    sCmd += "Join tblSimulationMode_Tank_Status S ";
                    sCmd += "On L.Tank_ID = S.Tank_ID ";
                    sCmd += " ORDER BY Tank_ID";
                    break;
                case "vsGetSimulationModeBallastTankFillDetails":
                    sCmd = "Select T.Tank_ID, T.Tank_Name, L.Volume, L.Percent_Full,S.IsSensorFaulty,S.IsDamaged,L.SG,L.Status,L.FSMType ";
                     sCmd += " From tblSimulationMode_Loading_Condition L ";
                     sCmd+="Join tblMaster_Tank T ";
                     sCmd+="ON L.Tank_ID = T.Tank_ID ";
                     sCmd += "Join tblSimulationMode_Tank_Status S ";
                     sCmd+= "On L.Tank_ID = S.Tank_ID ";
	                 sCmd+="Where T.[Group] = 'BALLAST_TANK' ORDER BY Tank_ID";
                    break;
                case "vsGetSimulationModeFuelOilTankFillDetails":
                    sCmd = "Select T.Tank_ID, T.Tank_Name, L.Volume, L.Percent_Full,S.IsSensorFaulty,S.IsDamaged,L.SG,L.Status,L.FSMType ";
                     sCmd += " From tblSimulationMode_Loading_Condition L ";
                     sCmd+="Join tblMaster_Tank T ";
                     sCmd+="ON L.Tank_ID = T.Tank_ID ";
                     sCmd += "Join tblSimulationMode_Tank_Status S ";
                     sCmd+= "On L.Tank_ID = S.Tank_ID ";
                     sCmd += "Where T.[Group] = 'FUELOIL_TANK' ORDER BY Tank_ID";
                    break;
                case "vsGetSimulationModeFreshWaterTankFillDetails":
                    sCmd = "Select T.Tank_ID, T.Tank_Name, L.Volume, L.Percent_Full,S.IsSensorFaulty,S.IsDamaged,L.SG,L.Status,L.FSMType ";
                     sCmd += " From tblSimulationMode_Loading_Condition L ";
                     sCmd+="Join tblMaster_Tank T ";
                     sCmd+="ON L.Tank_ID = T.Tank_ID ";
                     sCmd += "Join tblSimulationMode_Tank_Status S ";
                     sCmd+= "On L.Tank_ID = S.Tank_ID ";
                     sCmd += "Where T.[Group] = 'FRESHWATER_TANK' ORDER BY Tank_ID";
                    break;

                case "vsGetSimulationImersionParticulars":
                     sCmd="Select [openingNo],[openingName],[openingX],[openingY],[openingZ],[actualImAng] From tblSimulationDownFloodingAngle";    
                    //sCmd = "Select [Draft_Mean],[Draft_AP],[Draft_FP],[Draft_Aft_Mark],[Draft_Fore_Mark],Draft_Sonar_Dome, Draft_Propeller From tblEquilibrium_Values";
                    break;
                case "vsGetSimulationModeMiscTankFillDetails":
                    sCmd = "Select T.Tank_ID, T.Tank_Name, L.Volume, L.Percent_Full,S.IsSensorFaulty,S.IsDamaged,L.SG,L.Status,L.FSMType ";
                     sCmd += " From tblSimulationMode_Loading_Condition L ";
                     sCmd+="Join tblMaster_Tank T ";
                     sCmd+="ON L.Tank_ID = T.Tank_ID ";
                     sCmd += "Join tblSimulationMode_Tank_Status S ";
                     sCmd+= "On L.Tank_ID = S.Tank_ID ";
                     sCmd += "Where T.[Group] = 'MISC_TANK' ORDER BY Tank_ID";
                    break;
                case "vsGetSimulationModeCompartmentFillDetails":
                    sCmd = "Select T.Tank_ID, T.Tank_Name, L.Volume, L.Percent_Full,S.IsSensorFaulty,S.IsDamaged,L.SG,L.Status,L.FSMType,L.FloodTime ";
                     sCmd += " From tblSimulationMode_Loading_Condition L ";
                     sCmd+="Join tblMaster_Tank T ";
                     sCmd+="ON L.Tank_ID = T.Tank_ID ";
                     sCmd += "Join tblSimulationMode_Tank_Status S ";
                     sCmd+= "On L.Tank_ID = S.Tank_ID ";
                     sCmd += "Where T.[Group] = 'Compartment' ORDER BY Tank_ID";
                    break;

                case "vsGetSimulationModeWTRegionFillDetails":
                    sCmd = "Select T.Tank_ID, T.Tank_Name, L.Volume, L.Percent_Full,S.IsSensorFaulty,S.IsDamaged,L.SG,L.Status,L.FSMType,L.FloodTime ";
                    sCmd += "From tblSimulationMode_Loading_Condition L";
                    sCmd += "Join tblMaster_Tank T ";
                    sCmd += "ON L.Tank_ID = T.Tank_ID ";
                    sCmd += "Join tblSimulationMode_Tank_Status S ";
                    sCmd += "On L.Tank_ID = S.Tank_ID ";
                    sCmd += "Where T.[Group] = 'WT_REGION' ORDER BY Tank_ID";
                    break;
                case "vsGetSimulationModeVariableDetails":
                     sCmd = @"SELECT [Load_Id],[Load_Name],[Weight],[LCG],[TCG],[VCG],[Length],[Breadth],[Depth]
                            FROM [tblFixedLoad_Simulation]";
                    break;
                case "vsGetSimulationBallastTankLoadingStatusDetails":
                    sCmd = "Select M.Tank_ID, M.Tank_Name,L.[Sounding_Level], L.Volume, L.SG, L.Weight,L.Percent_Full, L.LCG, L.TCG, L.VCG, L.FSM ,S.IsDamaged,L.Status,L.FSMType ";
                    sCmd += "From tblSimulationMode_Loading_Condition L ";
	                sCmd+="JOIN dbo.tblMaster_Tank M ";
	                sCmd+="On L.Tank_ID = M.Tank_ID ";
                    sCmd += "Join tblSimulationMode_Tank_Status S ";
	                sCmd+="On L.Tank_ID = S.Tank_ID ";
	                sCmd+="Where M.[Group] = 'BALLAST_TANK' ORDER BY Tank_ID";
                    break;
                case "vsGetSimulationFuelOilTankLoadingStatusDetails":
                    sCmd = "Select M.Tank_ID, M.Tank_Name,L.[Sounding_Level], L.Volume, L.SG, L.Weight,L.Percent_Full, L.LCG, L.TCG, L.VCG, L.FSM ,S.IsDamaged,L.Status,L.FSMType ";
                      sCmd += "From dbo.tblSimulationMode_Loading_Condition L ";
	                sCmd+="JOIN dbo.tblMaster_Tank M ";
	                sCmd+="On L.Tank_ID = M.Tank_ID ";
                    sCmd += "Join tblSimulationMode_Tank_Status S ";
	                sCmd+="On L.Tank_ID = S.Tank_ID ";
                    sCmd += "Where M.[Group] = 'FUELOIL_TANK' ORDER BY Tank_ID";
                    break;
                case "vsGetSimulationFreshWaterTankLoadingStatusDetails":
                    sCmd = "Select M.Tank_ID, M.Tank_Name,L.[Sounding_Level], L.Volume, L.SG, L.Weight,L.Percent_Full, L.LCG, L.TCG, L.VCG, L.FSM ,S.IsDamaged,L.Status,L.FSMType ";
                      sCmd += "From dbo.tblSimulationMode_Loading_Condition L ";
	                sCmd+="JOIN dbo.tblMaster_Tank M ";
	                sCmd+="On L.Tank_ID = M.Tank_ID ";
                    sCmd += "Join tblSimulationMode_Tank_Status S ";
	                sCmd+="On L.Tank_ID = S.Tank_ID ";
                    sCmd += "Where M.[Group] = 'FRESHWATER_TANK' ORDER BY Tank_ID";
                    break;
                case "vsGetSimulationMiscTankLoadingStatusDetails":
                    sCmd = "Select M.Tank_ID, M.Tank_Name,L.[Sounding_Level], L.Volume, L.SG, L.Weight,L.Percent_Full, L.LCG, L.TCG, L.VCG, L.FSM ,S.IsDamaged,L.Status,L.FSMType ";
                      sCmd += "From dbo.tblSimulationMode_Loading_Condition L ";
	                sCmd+="JOIN dbo.tblMaster_Tank M ";
	                sCmd+="On L.Tank_ID = M.Tank_ID ";
                    sCmd += "Join tblSimulationMode_Tank_Status S ";
	                sCmd+="On L.Tank_ID = S.Tank_ID ";
                    sCmd += "Where M.[Group] = 'MISC_TANK' ORDER BY Tank_ID";
                    break;
                case "vsGetSimulationCompartmentLoadingStatusDetails":
                    sCmd = "Select M.Tank_ID, M.Tank_Name,L.[Sounding_Level], L.Volume, L.SG, L.Weight,L.Percent_Full, L.LCG, L.TCG, L.VCG, L.FSM ,S.IsDamaged,L.Status,L.FSMType,L.FloodTime  ";
                      sCmd += "From dbo.tblSimulationMode_Loading_Condition L ";
	                sCmd+="JOIN dbo.tblMaster_Tank M ";
	                sCmd+="On L.Tank_ID = M.Tank_ID ";
                    sCmd += "Join tblSimulationMode_Tank_Status S ";
	                sCmd+="On L.Tank_ID = S.Tank_ID ";
                    sCmd += "Where M.[Group] = 'Compartment' ORDER BY Tank_ID";
                    break;
                case "vsGettypeValues":
                    sCmd = " select COUNT(Tank_ID) as type from tblSimulationMode_Loading_Condition where IsDamaged=1";
                    
                    break;
                case "vsGetSimulationModeEquillibriumValues":
                     sCmd = " SELECT [Stability_Values_ID],[Lightship_Weight],[Displacement]";
                     sCmd += ", [Draft_Mean],[Draft_AP],[Draft_FP],[Draft_Aft_Mark],[Draft_Fore_Mark]";
                     sCmd += ",[TRIM],[Heel],[GMT],[SF],[Buoyancy],Roll_Period ";
                    sCmd += "FROM [tblSimulationMode_Equilibrium_Values]";
                    break;
                case "vsGetSimulationModeStabilitySummary":
                    sCmd = " Select (Case M_Status When 1 Then 'OK' "; 
					sCmd+="When 0 Then 'NOT OK' ";
					sCmd+="Else 'NA' END) Stability_Status, ";
					sCmd+="Stability_Type ";
                    sCmd+="From ";					
                    sCmd+="(Select Min(Cast(C.Status As Int)) M_Status, S.Summary_Type AS Stability_Type ";
                    sCmd+= "From tblSimulationMode_Stability_Actual_Criteria_Calc C ";
                    sCmd+="Join tblMaster_Stability_Criteria_Summary S ";
                    sCmd+="ON C.Stability_Summary_ID = S.Stability_Summary_ID ";
                    sCmd += "where C.[USER] = 'dbo' and S.Summary_Type IN ('Damage', 'Intact','Longitudinal') ";
                    sCmd+="Group By S.Summary_Type) A ";
                    sCmd+= "Where A.M_Status Is Not Null ";
                    break;
                    case "vsGetSimulationModeStabilityfloodSummary":
                    //sCmd = "select lc.status from [tblSimulationMode_Loading_Condition] lc where  lc.tank_id>55 and tank_id <516 and lc.volume>0";

                    sCmd = "Select distinct status,IsDamaged,Tank_ID ";
                    sCmd += "FROM ";
                    sCmd += "( ";
                    sCmd += "SELECT L.status,";
                    sCmd += "L.IsDamaged,L.Tank_ID, 1 'Orderby' ";
                    sCmd += "	FROM [tblSimulationMode_Loading_Condition] L ";
                    sCmd += "	JOIN [tblMaster_Tank] T ";
                    sCmd += "	ON L.Tank_ID = T.Tank_ID ";
                    sCmd += "	Where T.[Group] In ('BALLAST_TANK', 'FRESHWATER_TANK', 'FUELOIL_TANK', 'MISC_TANK') ";
                    sCmd += "UNION ";
                    sCmd += "SELECT L.status, ";
                    sCmd += "L.IsDamaged,L.Tank_ID, 3 'Orderby' ";
                    sCmd += "	FROM [tblSimulationMode_Loading_Condition] L ";
                    sCmd += "	JOIN [tblMaster_Tank] T ";
                    sCmd += "	ON L.Tank_ID = T.Tank_ID ";
                    sCmd += "	Where T.[Group] In ('Variable Data') ";
                    sCmd += "UNION ";
                    sCmd += "SELECT distinct L.status, ";
                    sCmd += "L.IsDamaged,L.Tank_ID, 2 'Orderby' ";
                    sCmd += "	FROM [tblSimulationMode_Loading_Condition] L ";
                    sCmd += "	JOIN [tblMaster_Tank] T ";
                    sCmd += "	ON L.Tank_ID = T.Tank_ID ";
                    sCmd += "	Where T.[Group] In ('Compartment','WT_REGION') And L.IsDamaged=1 or L.status=2";

                    sCmd += "UNION ";
                    sCmd += "SELECT L.status, ";
                    sCmd += "L.IsDamaged,L.Tank_ID, 2 'Orderby' ";
                    sCmd += "	FROM [tblSimulationMode_Loading_Condition] L ";
                    sCmd += "	JOIN [tblMaster_Tank] T ";
                    sCmd += "	ON L.Tank_ID = T.Tank_ID ";
                    sCmd += "	Where T.[Group] In ('Compartment','WT_REGION') And L.Volume!=0";
                    //sCmd += "UNION ";
                    //sCmd += "SELECT 'Deadweight' As 'Tank_Name', '' As 'Frames', '' As 'Cargo', Null As 'Percent_Full', Null As 'Volume', Null As 'SG', ";
                    //sCmd += "		Sum(L.[Weight]),";
                    //sCmd += "		CASE WHEN SUM(L.[Weight]) > 0 THEN (SUM(Lmom)/Sum(L.[Weight])) ELSE 0 END AS 'LCG',";
                    //sCmd += "		CASE WHEN SUM(L.[Weight]) > 0 THEN (SUM(Tmom)/Sum(L.[Weight])) ELSE 0 END AS 'TCG', ";
                    //sCmd += "		CASE WHEN SUM(L.[Weight]) > 0 THEN (SUM(Vmom)/Sum(L.[Weight])) ELSE 0 END AS 'VCG', ";
                    //sCmd += "		SUM(FSM) As 'FSM','' As 'IsDamaged', 4 'Orderby' ";
                    //sCmd += "	FROM [tblSimulationMode_Loading_Condition] L ";
                    //sCmd += "	JOIN [tblMaster_Tank] T";
                    //sCmd += "	ON L.Tank_ID = T.Tank_ID";
                    //sCmd += "	Where T.[Group] <> ('LightShip') ";
                    sCmd += "UNION ";
                    sCmd += "SELECT L.status, ";
                    sCmd += "'' As 'IsDamaged',999 AS Tank_ID, 4 'Orderby' ";
                    sCmd += "	FROM [tblSimulationMode_Loading_Condition] L ";
                    sCmd += "	JOIN [tblMaster_Tank] T ";
                    sCmd += "	ON L.Tank_ID = T.Tank_ID ";
                    sCmd += "	Where T.[Group] In ('LightShip') ";
                    sCmd += "UNION ";
                    sCmd += "SELECT '' As 'status', ";
                    sCmd += " '' As 'IsDamaged', 1000 AS Tank_ID, 5 'Orderby' ";
                    sCmd += "	FROM [tblSimulationMode_Loading_Condition] L ";
                    sCmd += "	JOIN [tblMaster_Tank] T ";
                    sCmd += "	ON L.Tank_ID = T.Tank_ID ";
                    sCmd += ") A ";
                    sCmd += "Order by Tank_ID";
                    break;
                      
                case "vsGetSimulationModeStabilityType":
                     sCmd = " Select Stability_Type ";
	                sCmd+="From ";					
	                sCmd+="(Select Min(Cast(C.Status As Int)) M_Status, S.Summary_Type AS Stability_Type ";
                    sCmd += "From tblSimulationMode_Stability_Actual_Criteria_Calc C ";
	                sCmd+="Join tblMaster_Stability_Criteria_Summary S ";
	                sCmd+="ON C.Stability_Summary_ID = S.Stability_Summary_ID ";
	                sCmd+="Where S.Summary_Type IN ('Damage', 'Intact') ";
                    sCmd += "Group By S.Summary_Type) A ";    
	                sCmd+="Where A.M_Status Is Not Null";
                    break;
                case "vsGetSimulationModeLoadingSummaryCurrent":
                    sCmd = "Select distinct Tank_Name, Frames, Cargo, Percent_Full,  Volume, SG, [Weight], LCG, TCG, VCG, FSM,IsDamaged,Tank_ID ";
                    sCmd += "FROM ";
                    sCmd += "( ";
                    sCmd += "SELECT T.Tank_Name, ";
                    sCmd += "	(Cast(T.Frame_Init As Varchar(10)) + ' - ' + Cast(T.Frame_End As Varchar(10))) Frames,";
                    sCmd += "	T.Cargo, L.Percent_Full,  L.Volume, L.SG, L.[Weight], L.LCG, L.TCG, L.VCG, L.FSM,L.IsDamaged,L.Tank_ID, 1 'Orderby' ";
                    sCmd += "	FROM [tblSimulationMode_Loading_Condition] L ";
                    sCmd += "	JOIN [tblMaster_Tank] T ";
                    sCmd += "	ON L.Tank_ID = T.Tank_ID ";
                    sCmd += "	Where T.[Group] In ('BALLAST_TANK', 'FRESHWATER_TANK', 'FUELOIL_TANK', 'MISC_TANK') ";
                    sCmd += "UNION ";
                    sCmd += "SELECT T.Tank_Name, ";
                    sCmd += "	(Cast(T.Frame_Init As Varchar(10)) + ' - ' + Cast(T.Frame_End As Varchar(10))) Frames, ";
                    sCmd += "	T.Cargo, L.Percent_Full,  L.Volume, L.SG, L.[Weight], L.LCG, L.TCG, L.VCG, L.FSM,L.IsDamaged,L.Tank_ID, 3 'Orderby' ";
                    sCmd += "	FROM [tblSimulationMode_Loading_Condition] L ";
                    sCmd += "	JOIN [tblMaster_Tank] T ";
                    sCmd += "	ON L.Tank_ID = T.Tank_ID ";
                    sCmd += "	Where T.[Group] In ('Variable Data') ";
                    sCmd += "UNION ";
                    sCmd += "SELECT T.Tank_Name, ";
                    sCmd += "	(Cast(T.Frame_Init As Varchar(10)) + ' - ' + Cast(T.Frame_End As Varchar(10))) Frames, ";
                    sCmd += "	T.Cargo, L.Percent_Full,  L.Volume, L.SG, L.[Weight], L.LCG, L.TCG, L.VCG, L.FSM,L.IsDamaged,L.Tank_ID, 2 'Orderby' ";
                    sCmd += "	FROM [tblSimulationMode_Loading_Condition] L ";
                    sCmd += "	JOIN [tblMaster_Tank] T ";
                    sCmd += "	ON L.Tank_ID = T.Tank_ID ";
                    sCmd += "	Where T.[Group] In ('Compartment','WT_REGION') And L.IsDamaged=1 or L.status=2";

                    sCmd += "UNION ";
                    sCmd += "SELECT T.Tank_Name, ";
                    sCmd += "	(Cast(T.Frame_Init As Varchar(10)) + ' - ' + Cast(T.Frame_End As Varchar(10))) Frames, ";
                    sCmd += "	T.Cargo, L.Percent_Full,  L.Volume, L.SG, L.[Weight], L.LCG, L.TCG, L.VCG, L.FSM,L.IsDamaged,L.Tank_ID, 2 'Orderby' ";
                    sCmd += "	FROM [tblSimulationMode_Loading_Condition] L ";
                    sCmd += "	JOIN [tblMaster_Tank] T ";
                    sCmd += "	ON L.Tank_ID = T.Tank_ID ";
                    sCmd += "	Where T.[Group] In ('Compartment','WT_REGION') And L.Volume!=0";
                    //sCmd += "UNION ";
                    //sCmd += "SELECT 'Deadweight' As 'Tank_Name', '' As 'Frames', '' As 'Cargo', Null As 'Percent_Full', Null As 'Volume', Null As 'SG', ";
                    //sCmd += "		Sum(L.[Weight]),";
                    //sCmd += "		CASE WHEN SUM(L.[Weight]) > 0 THEN (SUM(Lmom)/Sum(L.[Weight])) ELSE 0 END AS 'LCG',";
                    //sCmd += "		CASE WHEN SUM(L.[Weight]) > 0 THEN (SUM(Tmom)/Sum(L.[Weight])) ELSE 0 END AS 'TCG', ";
                    //sCmd += "		CASE WHEN SUM(L.[Weight]) > 0 THEN (SUM(Vmom)/Sum(L.[Weight])) ELSE 0 END AS 'VCG', ";
                    //sCmd += "		SUM(FSM) As 'FSM','' As 'IsDamaged', 4 'Orderby' ";
                    //sCmd += "	FROM [tblSimulationMode_Loading_Condition] L ";
                    //sCmd += "	JOIN [tblMaster_Tank] T";
                    //sCmd += "	ON L.Tank_ID = T.Tank_ID";
                    //sCmd += "	Where T.[Group] <> ('LightShip') ";
                    sCmd += "UNION ";
                    sCmd += "SELECT T.Tank_Name, ";
                    sCmd += "	(Cast(T.Frame_Init As Varchar(10)) + ' - ' + Cast(T.Frame_End As Varchar(10))) Frames, ";
                    sCmd += "	T.Cargo, L.Percent_Full,  L.Volume, L.SG, L.[Weight], L.LCG, L.TCG, L.VCG, L.FSM,'' As 'IsDamaged',999 AS Tank_ID, 4 'Orderby' ";
                    sCmd += "	FROM [tblSimulationMode_Loading_Condition] L ";
                    sCmd += "	JOIN [tblMaster_Tank] T ";
                    sCmd += "	ON L.Tank_ID = T.Tank_ID ";
                    sCmd += "	Where T.[Group] In ('LightShip') ";
                    sCmd += "UNION ";
                    sCmd += "SELECT 'TOTAL DISPLACEMENT' As 'Tank_Name', '' As 'Frames', '' As 'Cargo', Null As 'Percent_Full', Null As 'Volume', Null As 'SG', ";
                    sCmd += "		Sum(L.[Weight]), ";
                    sCmd += "		(SUM(Lmom)/Sum(L.[Weight])) As 'LCG', ";
                    sCmd += "		(SUM(Tmom)/Sum(L.[Weight])) As 'TCG', ";
                    sCmd += "		(SUM(Vmom)/Sum(L.[Weight])) As 'VCG', ";
                    sCmd += "		SUM(FSM) As 'FSM','' As 'IsDamaged', 1000 AS Tank_ID, 5 'Orderby' ";
                    sCmd += "	FROM [tblSimulationMode_Loading_Condition] L ";
                    sCmd += "	JOIN [tblMaster_Tank] T ";
                    sCmd += "	ON L.Tank_ID = T.Tank_ID ";
                    sCmd += ") A ";
                    sCmd += "Order by Tank_ID";
                    break;

                case "vsGetSimulationModeGzDataCurrent":
                    //sCmd = " SELECT a.heelAng,a.heelGZ,b.heelArm AS HS, c.heelArm AS PC, d.lw1 as LW1,e.lw2 as LW2,f.Min_DF as Downflooding from GZDataSimulationMode_New a,tblWindHeelSimulation b,tblHeavyLiftingSimulation c,tblWindHeelSimulation_New d,tblWindHeelSimulation_New e,tblMin_DF_Angle_Simulation f where a.heelAng=b.heelAng and a.heelAng=c.heelAng and  a.[User]=b.c_User and a.[User]=c.c_User and a.[User]=d.c_User and  a.[User]=e.c_User AND a.[User] = 'dbo'";
                   // sCmd = "SELECT a.heelAng,a.heelGZ,b.heelArm AS WH,c.heelArm AS HL,d.heelArm AS HS,e.heelArm AS PC from GZDataSimulationMode_New a,tblWindHeelSimulation b,tblHeavyLiftingSimulation c,tblHighSpeedSimulation d,tblPassengerCrowdingSimulation e where a.heelAng=b.heelAng and a.heelAng=c.heelAng and a.heelAng=d.heelAng and a.heelAng=e.heelAng And a.[User]=b.c_User and a.[User]=c.c_User and a.[User]=d.c_User and a.[User]=e.c_User AND a.[User] = 'dbo'";
                    //sCmd = "SELECT a.heelAng,a.heelGZ,b.heelArm AS WH,c.heelArm AS HL,d.heelArm AS HS,e.heelArm AS PC from GZDataSimulationMode_New a,tblWindHeelSimulation b,tblHeavyLiftingSimulation c,tblHighSpeedSimulation d,tblPassengerCrowdingSimulation e where a.heelAng=b.heelAng and a.heelAng=c.heelAng and a.heelAng=d.heelAng and a.heelAng=e.heelAng And a.[User]=b.c_User and a.[User]=c.c_User and a.[User]=d.c_User and a.[User]=e.c_User AND a.[User] = 'dbo'";
                    sCmd = "SELECT a.HeelAngle,ISNULL(a.GZ,0)GZ,b.heelArm AS WH,c.heelArm AS HL,d.heelArm AS HS,e.heelArm AS PC from tbl_HeeledCondition_Intact a,tblWindHeelSimulation b,tblHeavyLiftingSimulation c,tblHighSpeedSimulation d,tblPassengerCrowdingSimulation e where a.HeelAngle=b.heelAng and a.HeelAngle=c.heelAng and a.HeelAngle=d.heelAng and a.HeelAngle=e.heelAng  ";
                    break;

                case "vsGetSimulationModeGzDataCurrentDamaged":
                    //sCmd = " SELECT a.heelAng,a.heelGZ,b.heelArm AS HS, c.heelArm AS PC, d.lw1 as LW1,e.lw2 as LW2,f.Min_DF as Downflooding from GZDataSimulationMode_New a,tblWindHeelSimulation b,tblHeavyLiftingSimulation c,tblWindHeelSimulation_New d,tblWindHeelSimulation_New e,tblMin_DF_Angle_Simulation f where a.heelAng=b.heelAng and a.heelAng=c.heelAng and  a.[User]=b.c_User and a.[User]=c.c_User and a.[User]=d.c_User and  a.[User]=e.c_User AND a.[User] = 'dbo'";
                    // sCmd = "SELECT a.heelAng,a.heelGZ,b.heelArm AS WH,c.heelArm AS HL,d.heelArm AS HS,e.heelArm AS PC from GZDataSimulationMode_New a,tblWindHeelSimulation b,tblHeavyLiftingSimulation c,tblHighSpeedSimulation d,tblPassengerCrowdingSimulation e where a.heelAng=b.heelAng and a.heelAng=c.heelAng and a.heelAng=d.heelAng and a.heelAng=e.heelAng And a.[User]=b.c_User and a.[User]=c.c_User and a.[User]=d.c_User and a.[User]=e.c_User AND a.[User] = 'dbo'";
                    //sCmd = "SELECT a.heelAng,a.heelGZ,b.heelArm AS WH,c.heelArm AS HL,d.heelArm AS HS,e.heelArm AS PC from GZDataSimulationMode_New a,tblWindHeelSimulation b,tblHeavyLiftingSimulation c,tblHighSpeedSimulation d,tblPassengerCrowdingSimulation e where a.heelAng=b.heelAng and a.heelAng=c.heelAng and a.heelAng=d.heelAng and a.heelAng=e.heelAng And a.[User]=b.c_User and a.[User]=c.c_User and a.[User]=d.c_User and a.[User]=e.c_User AND a.[User] = 'dbo'";
                    sCmd = "SELECT a.HeelAngle,ISNULL(a.GZ,0)GZ,b.heelArm AS WH from tbl_HeeledCondition_Damage a ";
                    sCmd+= "INNER JOIN tblWindHeelSimulation b ON a.HeelAngle=b.heelAng where  A.HeelAngle between -60 and 60";
                    break;
                case "vsGetSimulationModeLongitudinalDataCurrent":
                    sCmd = "SELECT [Length],BuoyanceUDL,NetUDL,SF,BM FROM tbl_SimulationMode_SFAndBM Where [User] = 'dbo' order by [Length]";
                    break;
                case "vsGetSimulationModeIntactStabilityCriteriaCurrent":
                     sCmd = "Select S.Criterion, C.CriticalValue Critical_Value , C.Actual_Value,C.[Status] ";
		            sCmd+="From tblMaster_Stability_Criteria_Summary S ";
                    sCmd += "JOIN tblSimulationMode_Stability_Actual_Criteria_Calc C ";
		            sCmd+="ON S.Stability_Summary_ID = C.Stability_Summary_ID "; 
		            sCmd+="Where Summary_Type = 'Intact'";
                    break;
                case "vsGetSimulationModeDamageStabilityCriteriaCurrent":
                     sCmd = "Select S.Criterion, C.CriticalValue Critical_Value , C.Actual_Value,C.[Status] ";
		            sCmd+="From tblMaster_Stability_Criteria_Summary S ";
                    sCmd += "JOIN tblSimulationMode_Stability_Actual_Criteria_Calc C ";
		            sCmd+="ON S.Stability_Summary_ID = C.Stability_Summary_ID ";
                    sCmd += "Where Summary_Type = 'Damage'";
                    break;
                case "vsGetSimulationModeHydrostaticDataCurrent":
                    sCmd = " Select Displacement, LCB,TCB,VCB,LCF,TCF,LCG,TCG,[KG(Fluid)],KMT,KML,BMT,BML,WPA,Buoyancy FROM tblSimulationMode_Equilibrium_Values Where [USER] = 'dbo'";    
                //sCmd = "Select Displacement, TRIM, Heel,GMT, FSC,[KG(Solid)],[KG(Fluid)],LCG,TCG,LCF,MCT,TPC,Roll_Period From tblSimulationMode_Equilibrium_Values Where [USER] = 'dbo'";
                    break;
                case "vsGetSimulationModeDraftsCurrent":
                    sCmd = "Select  [TRIM],[Heel],[GMT],[Draft_Mean],[Draft_AP],[Draft_FP],[Draft_Aft_Mark],[Draft_Fore_Mark], [Draft_Sonar_Dome],[Draft_Propeller],[KG(Solid)], [KG(Fluid)],[LCG],[FSC],[TPC],[MCT] From tblSimulationMode_Equilibrium_Values Where [USER] = 'dbo'";
                    break;


                case "vsGetSimulationModeDraftsReport":
                   // sCmd = "Select Draft_AP,Draft_Propeller, Draft_Aft_Mark, Draft_Mean, Draft_Fore_Mark, Draft_Sonar_Dome, Draft_FP From tblSimulationMode_Equilibrium_Values Where [USER] = 'dbo'";
                    sCmd = "Select Draft_AP, Draft_Aft_Mark, Draft_Mean, Draft_Fore_Mark,Draft_FP From tblSimulationMode_Equilibrium_Values Where [USER] = 'dbo'";
                    break;
                case "vsGetRealModeHydrostatics2":
                    sCmd = "Select [TRIM],[Heel],[GMT],[KG(Solid)], [KG(Fluid)],[LCG],[FSC],[TPC],[MCT],[WPA] From tblRealMode_Equilibrium_Values";
                    break;
                case "vsGetSimulationModeHydrostatics2":
                    sCmd = "Select[TRIM],case when[Heel] < 0 then -[Heel] else[Heel]  end as[Heel] ,[GMT],[KG(Solid)], [KG(Fluid)],[LCG],[FSC],[TPC],[MCT],[WPA] From tblSimulationMode_Equilibrium_Values Where[USER] = 'dbo'";
                    //sCmd = "Select [TRIM],[Heel],[GMT],[KG(Solid)], [KG(Fluid)],[LCG],[FSC],[TPC],[MCT],[WPA] From tblSimulationMode_Equilibrium_Values Where [USER] = 'dbo'";
                    break;

                case "vsGetSimulationModeMouldedDraft":
                //    sCmd = "Select [Draft_AP_MLD] as Draft_AP ,[Draft_Propeller_MLD] as Draft_Propeller  ,[Draft_Aft_Mark_MLD] as Draft_Aft_Mark  ,[Draft_Mean_MLD] as Draft_Mean,[Draft_Fore_Mark_MLD] as Draft_Fore_Mark ,[Draft_Sonar_Dome_MLD] as Draft_Sonar_Dome ,[Draft_FP_MLD] as Draft_FP  From tblSimulationMode_Equilibrium_Values";
                    sCmd = "Select [Draft_AP_MLD] as Draft_AP ,[Draft_Aft_Mark_MLD] as Draft_Aft_Mark  ,[Draft_Mean_MLD] as Draft_Mean,[Draft_Fore_Mark_MLD] as Draft_Fore_Mark ,[Draft_FP_MLD] as Draft_FP  From tblSimulationMode_Equilibrium_Values";

                    break;

                case "vsGetSoundingPercentage":
                    sCmd = "SELECT [Tank_Name],[Percent_Full] ,[CapacityMC] ,[CapacityTonnes] ,[LCG] ,[TCG] ,[VCG] From tblSoundingPercentageHeelAngle";

                    break;
                ////////////////////Corrective Mode///////////////////////// 
                case "vsGetRealCorrectiveEquillibriumValues":
                    sCmd = @"SELECT TRIM,Heel, [Draft_Mean],[Draft_AP],[Draft_FP],[Draft_Aft_Mark],[Draft_Fore_Mark],[Draft_Mid_Mark] FROM tblCorrective_Equilibrium_Values";
                    break;
                case "vsGetRealCorrectiveGZ":
                    sCmd = @"SELECT a.heelAng,a.heelGZ from GZData_Corrective a";
                    break;

                case "vsGetRealDFlootingCorrectiveMeasures":
                    sCmd = @"SELECT a.heelAng,a.heelGZ from GZData_Corrective a";
                    break;
                case "vsGetRealCorrectiveStabilityCriteria":
                    sCmd = @"Select S.Criterion, S.Critical_Value, C.Actual_Value,C.[Status] 
		                        From tblMaster_Stability_Criteria_Summary S
		                        JOIN [tblCorrective_Stability_Actual_Criteria_Calc] C
		                        ON S.Stability_Summary_ID = C.Stability_Summary_ID 
		                        Where Summary_Type = 'Damage'";
                    break;
                case "vsGetRealCorrectiveMeasures":
                    sCmd = @"Select Currective_Measures_ID, Measures_Suggested  From tblCurrective_Measures 
	                            Where [Status] = 0";
                    break;


                case "vsGetSimulationDFlootingCorrectiveMeasures":
                    sCmd = "SELECT  Deflooding_Option FROM tblSimulationMode_Deflooding_Option";
                    break;


                case "vsGetRealDamageZones":
                    sCmd = @"SELECT zoneName,damage_Status from tblZoneLength";
                    break;


                case "CorrectiveSimulationEquilibriumvalues":
                    sCmd = @"select * from [tblCorrective_SimulationMode_Equilibrium_Values]";
                    break;

                case "CorrectiveSimulationDraftvalues":
                    sCmd = @"select  Draft_AP, Draft_Propeller,Draft_Aft_Mark,Draft_Mean,Draft_FP, Draft_Fore_Mark, Draft_Sonar_Dome, TRIM, Heel from [tblCorrective_SimulationMode_Equilibrium_Values]";
                    break;


                case "vsGetSimulationCorrectiveEquillibriumValues":
                    sCmd = @"SELECT TRIM,Heel, [Draft_Mean],[Draft_AP],[Draft_FP],[Draft_Aft_Mark],[Draft_Fore_Mark] FROM tblCorrective_SimulationMode_Equilibrium_Values";

                   // sCmd = @"SELECT TRIM,Heel, [Draft_Mean],[Draft_AP],[Draft_FP],[Draft_Aft_Mark],[Draft_Fore_Mark], [Draft_Mid_Mark] FROM tblCorrective_SimulationMode_Equilibrium_Values";
                    break;
                case "vsGetSimulationCorrectiveGZ":
                    sCmd = @"SELECT a.heelAng,a.heelGZ from GZData_Corrective_SimulationMode_New a";
                    break;
                case "vsGetSimulationCorrectiveStabilityCriteria":
                    sCmd = @"
		                    Select S.Criterion, S.Critical_Value, C.Actual_Value,C.[Status] 
		                    From tblMaster_Stability_Criteria_Summary S
		                    JOIN [tblCorrective_SimulationMode_Stability_Actual_Criteria_Calc] C
		                    ON S.Stability_Summary_ID = C.Stability_Summary_ID 
		                    Where Summary_Type = 'Damage'";
                    break;
                case "vsGetSimulationCorrectiveMeasures":
                    sCmd = @"Select Currective_Measures_ID, Measures_Suggested  From tblSimulation_Currective_Measures";
	                            
                    break;

                case "vsGetSimulationCorrectiveMeasuresBallast":
                    sCmd = @"Select Currective_Measures_ID, Measures_Suggested  From tblSimulation_Currective_Measures where Measures_Suggested like 'ballast%' ";

                    break;

                case "vsGetSimulationCorrectiveMeasuresDeBallast":
                    sCmd = @"Select Currective_Measures_ID, Measures_Suggested  From tblSimulation_Currective_Measures where Measures_Suggested like '%Deballast%' ";

                    break;

                case "vsGetSimulationCorrectiveMeasuresFluidTransfer":
                    sCmd = @"Select Currective_Measures_ID, Measures_Suggested  From tblSimulation_Currective_Measures where Measures_Suggested like '%Fresh water%' ";

                    break;
                case "vsGetSimulationCorrectiveMeasuresFluidTransferfuel":
                    sCmd = @"Select Currective_Measures_ID, Measures_Suggested  From tblSimulation_Currective_Measures where Measures_Suggested like '%Fuel oil%' ";

                    break;

                case "vsGetSimulationDamageZones":
                    sCmd = @"SELECT zoneName,damage_Status from tblCorrectiveZoneLength";
                    break;

                case "vsLightShhipDataDetails":
                    sCmd=" select Lightship_Weight, [Lightship_LCG],[Lightship_VCG], [Lightship_TCG] from [tblMaster_Config_Addi]";
                    //sCmd = " select Lightship_Weight, [Lightship_LCG],[Lightship_VCG], [Lightship_TCG] from [tblMaster_Config_Addi]";
                    break;


                    // Simulation mode corrective

                case "vsGetSimulationModeHydrostaticDataCorrective":
                    sCmd = " Select Displacement, LCB,TCB,VCB,LCF,TCF,LCG,TCG,[KG(Fluid)],KMT,KML,BMT,BML,WPA,Buoyancy FROM tblSimulationMode_Equilibrium_Values_Corrective";
                    
                    break;

                case "vsGetSimulationModeHydrostatics2Corrective":
                    sCmd = "Select [TRIM],[Heel],[GMT],[KG(Solid)], [KG(Fluid)],[LCG],[FSC],[TPC],[MCT],[WPA] From tblSimulationMode_Equilibrium_Values_Corrective ";
                    break;

                case "vsGetSimulationModeDraftsReportCorrective":
                    sCmd = "Select Draft_AP, Draft_Aft_Mark, Draft_Mean, Draft_Fore_Mark, Draft_FP From tblSimulationMode_Equilibrium_Values_Corrective ";
                    break;

                case "vsGetSimulationModeGzDataCorrective":

                    sCmd = "SELECT a.HeelAngle,ISNULL(a.GZ,0)GZ,b.heelArm AS WH from tbl_HeeledCondition_Damage_Corrective a,tblWindHeelSimulation b where a.HeelAngle=b.heelAng  ";
                    break;
                case  "vsGetSimulationModeStabilitycorrectivefloodSummary":
                    sCmd=" Select distinct status,Tank_ID ";
                     sCmd += "FROM ";
                      sCmd +="( ";
                      sCmd +="SELECT L.status,L.Tank_ID, 1 'Orderby' ";
                     	 sCmd +="FROM [tblSimulationMode_Loading_Condition_Corrective] L,[tblSimulationMode_Loading_Condition] sL, ";
                     	  sCmd +="[tblMaster_Tank] T where";
                     	  sCmd +="L.Tank_ID = T.Tank_ID and sL.Tank_ID = T.Tank_ID and";
                     	  sCmd +="T.[Group] In ('BALLAST_TANK', 'FRESHWATER_TANK', 'FUELOIL_TANK', 'MISC_TANK') ";
                      sCmd +="UNION ";
                      sCmd +="SELECT L.status,L.Tank_ID, 3 'Orderby' ";
                     	 sCmd +="FROM [tblSimulationMode_Loading_Condition_Corrective] L ";
                     	 sCmd +="JOIN [tblMaster_Tank] T ";
                     	 sCmd +="ON L.Tank_ID = T.Tank_ID ";
                     	 sCmd +="Where T.[Group] In ('Variable Data') ";
                      sCmd +="UNION ";
                      sCmd +="SELECT distinct L.status ,L.Tank_ID, 2 'Orderby' ";
                     	 sCmd +="FROM [tblSimulationMode_Loading_Condition_Corrective] L,[tblSimulationMode_Loading_Condition] sL,";
                     	  sCmd +="[tblMaster_Tank] T where  ";
                     	  sCmd +="L.Tank_ID = T.Tank_ID and sL.Tank_ID = T.Tank_ID and ";
                     	  sCmd +="T.[Group] In ('Compartment','WT_REGION') And sL.IsDamaged=1 or L.status=2";
                    
                      sCmd +="UNION ";
                      sCmd +="SELECT L.status,999 AS Tank_ID, 4 'Orderby' ";
                     	 sCmd +="FROM [tblSimulationMode_Loading_Condition_Corrective] L ";
                     	 sCmd +="JOIN [tblMaster_Tank] T ";
                     	 sCmd +="ON L.Tank_ID = T.Tank_ID ";
                     	 sCmd +="Where T.[Group] In ('LightShip') ";
                      sCmd +="UNION ";
                      sCmd +="SELECT L.status ,1000 AS Tank_ID, 5 'Orderby' ";
                     	 sCmd +="FROM [tblSimulationMode_Loading_Condition_Corrective] L ";
                     	 sCmd +="JOIN [tblMaster_Tank] T ";
                     	 sCmd +="ON L.Tank_ID = T.Tank_ID ";
                      sCmd +=") A ";
                      sCmd += "Order by Tank_ID";
                          break;
                case "vsGetSimulationModeLoadingSummaryCorrective":
                    sCmd = "Select DISTINCT Tank_Name, Frames, Cargo, Percent_Full,  Volume, SG, [Weight], LCG, TCG, VCG, FSM,IsDamaged,Tank_ID ";
                    sCmd += "FROM ";
                    sCmd += "( ";
                    sCmd += "SELECT T.Tank_Name, ";
                    sCmd += "	(Cast(T.Frame_Init As Varchar(10)) + ' - ' + Cast(T.Frame_End As Varchar(10))) Frames,";
                    sCmd += "	T.Cargo, L.Percent_Full,  L.Volume, L.SG, L.[Weight], L.LCG, L.TCG, L.VCG, L.FSM,sL.IsDamaged,L.Tank_ID, 1 'Orderby' ";
                    sCmd += "	FROM [tblSimulationMode_Loading_Condition_Corrective] L,[tblSimulationMode_Loading_Condition] sL, ";
                    sCmd += "	 [tblMaster_Tank] T where";
                    sCmd += "	 L.Tank_ID = T.Tank_ID and sL.Tank_ID = T.Tank_ID and";
                    sCmd += "	 T.[Group] In ('BALLAST_TANK', 'FRESHWATER_TANK', 'FUELOIL_TANK', 'MISC_TANK') ";
                    sCmd += "UNION ";
                    sCmd += "SELECT T.Tank_Name, ";
                    sCmd += "	(Cast(T.Frame_Init As Varchar(10)) + ' - ' + Cast(T.Frame_End As Varchar(10))) Frames, ";
                    sCmd += "	T.Cargo, L.Percent_Full,  L.Volume, L.SG, L.[Weight], L.LCG, L.TCG, L.VCG, L.FSM,L.IsDamaged,L.Tank_ID, 3 'Orderby' ";
                    sCmd += "	FROM [tblSimulationMode_Loading_Condition_Corrective] L ";
                    sCmd += "	JOIN [tblMaster_Tank] T ";
                    sCmd += "	ON L.Tank_ID = T.Tank_ID ";
                    sCmd += "	Where T.[Group] In ('Variable Data') ";
                    sCmd += "UNION ";
                    sCmd += "SELECT DISTINCT T.Tank_Name, ";
                    sCmd += "	(Cast(T.Frame_Init As Varchar(10)) + ' - ' + Cast(T.Frame_End As Varchar(10))) Frames, ";
                    sCmd += "	T.Cargo, L.Percent_Full,  L.Volume, L.SG, L.[Weight], L.LCG, L.TCG, L.VCG, L.FSM,sL.IsDamaged,L.Tank_ID, 2 'Orderby' ";
                    sCmd += "	FROM [tblSimulationMode_Loading_Condition_Corrective] L,[tblSimulationMode_Loading_Condition] sL,";
                    sCmd += "	 [tblMaster_Tank] T where  ";
                    sCmd += "	 L.Tank_ID = T.Tank_ID and sL.Tank_ID = T.Tank_ID and ";
                    sCmd += "	 T.[Group] In ('Compartment','WT_REGION') And sL.Status=2 ";
                     sCmd += "UNION ";
                    sCmd += "SELECT DISTINCT T.Tank_Name, ";
                    sCmd += "	(Cast(T.Frame_Init As Varchar(10)) + ' - ' + Cast(T.Frame_End As Varchar(10))) Frames, ";
                    sCmd += "	T.Cargo, L.Percent_Full,  L.Volume, L.SG, L.[Weight], L.LCG, L.TCG, L.VCG, L.FSM,sL.IsDamaged,L.Tank_ID, 2 'Orderby' ";
                    sCmd += "	FROM [tblSimulationMode_Loading_Condition_Corrective] L,[tblSimulationMode_Loading_Condition] sL,";
                    sCmd += "	 [tblMaster_Tank] T where  ";
                    sCmd += "	 L.Tank_ID = T.Tank_ID and sL.Tank_ID = T.Tank_ID and ";
                    sCmd += "	 T.[Group] In ('Compartment','WT_REGION') And sL.IsDamaged=1 ";
                    sCmd += "UNION ";
                        // sCmd += "UNION ";
                    sCmd += "SELECT DISTINCT T.Tank_Name, ";
                    sCmd += "	(Cast(T.Frame_Init As Varchar(10)) + ' - ' + Cast(T.Frame_End As Varchar(10))) Frames, ";
                    sCmd += "	T.Cargo, L.Percent_Full,  L.Volume, L.SG, L.[Weight], L.LCG, L.TCG, L.VCG, L.FSM,sL.IsDamaged,L.Tank_ID, 2 'Orderby' ";
                    sCmd += "	FROM [tblSimulationMode_Loading_Condition_Corrective] L,[tblSimulationMode_Loading_Condition] sL,";
                    sCmd += "	 [tblMaster_Tank] T where  ";
                    sCmd += "	 L.Tank_ID = T.Tank_ID and sL.Tank_ID = T.Tank_ID and ";
                    sCmd += "	 T.[Group] In ('Compartment','WT_REGION')  And L.Volume!=0 ";
                   // sCmd += "UNION ";
                    //sCmd += "UNION ";
                    //sCmd += "SELECT 'Deadweight' As 'Tank_Name', '' As 'Frames', '' As 'Cargo', Null As 'Percent_Full', Null As 'Volume', Null As 'SG', ";
                    //sCmd += "		Sum(L.[Weight]),";
                    //sCmd += "		CASE WHEN SUM(L.[Weight]) > 0 THEN (SUM(Lmom)/Sum(L.[Weight])) ELSE 0 END AS 'LCG',";
                    //sCmd += "		CASE WHEN SUM(L.[Weight]) > 0 THEN (SUM(Tmom)/Sum(L.[Weight])) ELSE 0 END AS 'TCG', ";
                    //sCmd += "		CASE WHEN SUM(L.[Weight]) > 0 THEN (SUM(Vmom)/Sum(L.[Weight])) ELSE 0 END AS 'VCG', ";
                    //sCmd += "		SUM(FSM) As 'FSM','' As 'IsDamaged', 4 'Orderby' ";
                    //sCmd += "	FROM [tblSimulationMode_Loading_Condition] L ";
                    //sCmd += "	JOIN [tblMaster_Tank] T";
                    //sCmd += "	ON L.Tank_ID = T.Tank_ID";
                    //sCmd += "	Where T.[Group] <> ('LightShip') ";
                    sCmd += "UNION ";
                    sCmd += "SELECT T.Tank_Name, ";
                    sCmd += "	(Cast(T.Frame_Init As Varchar(10)) + ' - ' + Cast(T.Frame_End As Varchar(10))) Frames, ";
                    sCmd += "	T.Cargo, L.Percent_Full,  L.Volume, L.SG, L.[Weight], L.LCG, L.TCG, L.VCG, L.FSM,'' As 'IsDamaged',999 AS Tank_ID, 4 'Orderby' ";
                    sCmd += "	FROM [tblSimulationMode_Loading_Condition_Corrective] L ";
                    sCmd += "	JOIN [tblMaster_Tank] T ";
                    sCmd += "	ON L.Tank_ID = T.Tank_ID ";
                    sCmd += "	Where T.[Group] In ('LightShip') ";
                    sCmd += "UNION ";
                    sCmd += "SELECT 'TOTAL DISPLACEMENT' As 'Tank_Name', '' As 'Frames', '' As 'Cargo', Null As 'Percent_Full', Null As 'Volume', Null As 'SG', ";
                    sCmd += "		Sum(L.[Weight]), ";
                    sCmd += "		(SUM(Lmom)/Sum(L.[Weight])) As 'LCG', ";
                    sCmd += "		(SUM(Tmom)/Sum(L.[Weight])) As 'TCG', ";
                    sCmd += "		(SUM(Vmom)/Sum(L.[Weight])) As 'VCG', ";
                    sCmd += "		SUM(FSM) As 'FSM','' As 'IsDamaged', 1000 AS Tank_ID, 5 'Orderby' ";
                    sCmd += "	FROM [tblSimulationMode_Loading_Condition_Corrective] L ";
                    sCmd += "	JOIN [tblMaster_Tank] T ";
                    sCmd += "	ON L.Tank_ID = T.Tank_ID ";
                    sCmd += ") A ";
                    sCmd += "Order by Tank_ID";
                    break;

                case "vsGetSimulationImersionParticularsCorrective":
                    sCmd = "Select [openingNo],[openingName],[openingX],[openingY],[openingZ],[actualImAng] From tblSimulationDownFloodingAngle_Corrective";
                    //sCmd = "Select [Draft_Mean],[Draft_AP],[Draft_FP],[Draft_Aft_Mark],[Draft_Fore_Mark],Draft_Sonar_Dome, Draft_Propeller From tblEquilibrium_Values";
                    break;

                case "vsGetSimulationModeMouldedDraftCorrective":
                    //sCmd = "Select [Draft_AP_MLD],[Draft_Propeller_MLD] ,[Draft_Aft_Mark_MLD] ,[Draft_Mean_MLD],[Draft_Fore_Mark_MLD] ,[Draft_Sonar_Dome_MLD],[Draft_FP_MLD] From tblSimulationMode_Equilibrium_Values_Corrective";
                    sCmd = "Select [Draft_AP_MLD] ,[Draft_Aft_Mark_MLD] ,[Draft_Mean_MLD],[Draft_Fore_Mark_MLD] ,[Draft_FP_MLD] From tblSimulationMode_Equilibrium_Values_Corrective";

                    break;

                case "vsGetSimulationModeDamageStabilityCriteriaCorrective":
                    sCmd = "Select S.Criterion, C.CriticalValue Critical_Value , C.Actual_Value,C.[Status] ";
                    sCmd += "From tblMaster_Stability_Criteria_Summary_Corrective S ";
                    sCmd += "JOIN tblSimulationMode_Stability_Actual_Criteria_Calc_Corrective C ";
                    sCmd += "ON S.Stability_Summary_ID = C.Stability_Summary_ID ";
                    sCmd += "Where Summary_Type = 'Damage'";
                    break;
                
                //Simulation mode corrective END

                default:
                    sCmd = "";
                    break;




            }
            dtable = DAL.clsDAL.GetAllRecsDT(sCmd);
           // dtHeelingArms = DAL.clsDAL.GetAllRecsDT(sCmd1);
            return dtable;
          //  return dtHeelingArms;
        }
    }
}
