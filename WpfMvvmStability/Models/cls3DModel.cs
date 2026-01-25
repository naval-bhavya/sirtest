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
#endregion CADLIB

namespace WpfMvvmStability.Models
{
    class cls3DModel
    {
        public static ModelVisual3D[] array = new ModelVisual3D[3];

        public static ModelVisual3D[] Array3dModels
        { get
             { return array; 
             }
            set
            {
                array = value;
            } 
        }
        
    }
}
