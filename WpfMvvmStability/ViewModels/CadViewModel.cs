//-----------------------------------------------------------------------------------
//.dwg/.dxf file Reader through CADLIB
//
//-----------------------------------------------------------------------------------

namespace WpfMvvmStability.ViewModels
{
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
    using HelixToolkit.Wpf;
    using System.Windows.Media.Media3D;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Threading;
    using System.Data;
    using System.Reflection;
    #region CADLIB
    using WW.Cad.Base;
    using WW.Cad.Drawing;
    using WW.Cad.Drawing.GDI;
    using WW.Cad.IO;
    using WW.Cad.Model;
    using WW.Math;
    using WW.Cad.Drawing.Wpf;
    using WW.Cad.Model.Objects;
    using WW.Math.Geometry;
    #endregion 
    class CadViewModel : Helpers.Observable
    {
       
        public static void Cad2dModels()
        {
            DxfModel model;
            Assembly assembly = Assembly.GetExecutingAssembly();
           
            var input = assembly.GetManifestResourceStream("WpfMvvmStability.Images.Profile.dwg");
            model = DwgReader.Read(input);
            Models.BO.clsGlobVar.Profile = model;



            input = assembly.GetManifestResourceStream("WpfMvvmStability.Images.PlanA.dwg");
            model = DwgReader.Read(input);
            Models.BO.clsGlobVar.PlanA = model;

            input = assembly.GetManifestResourceStream("WpfMvvmStability.Images.PlanB.dwg");
            model = DwgReader.Read(input);
            Models.BO.clsGlobVar.PlanB = model;

            input = assembly.GetManifestResourceStream("WpfMvvmStability.Images.PlanC.dwg");
            model = DwgReader.Read(input);
            Models.BO.clsGlobVar.PlanC = model;

            input = assembly.GetManifestResourceStream("WpfMvvmStability.Images.PlanD.dwg");
            model = DwgReader.Read(input);
            Models.BO.clsGlobVar.PlanD = model;

            input = assembly.GetManifestResourceStream("WpfMvvmStability.Images.PlanL.dwg");
            model = DwgReader.Read(input);
            Models.BO.clsGlobVar.PlanALL = model;
            
           // string [] gg =assembly.GetManifestResourceNames();
           //AssemblyName hh= assembly.GetName();
      
           // System.IO.Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("WpfMvvmStability.Images.PlanL.dwg");
        
           // model = DwgReader.Read(s);
           // Models.BO.clsGlobVar.PlanALL = model;

            model = null;
            input = null;
            assembly = null;

        }
    }
}
