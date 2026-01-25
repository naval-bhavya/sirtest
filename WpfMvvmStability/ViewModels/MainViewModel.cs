// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace WpfMvvmStability.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media.Media3D;
    using System.Windows.Threading;
    using System.IO;
    using HelixToolkit.Wpf;
    using WpfMvvmStability.Helpers;
    using System.Windows.Media;
    //using _3DTools;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class MainViewModel : Observable
    {
       
        
        //private readonly IHelixViewport3D viewport;

        private readonly Dispatcher dispatcher=null;

        private string currentModelPath;

        //private string applicationTitle;
  
        //private double expansion;

        private Model3D currentModel;

        //public MainViewModel(IFileDialogService fds, HelixViewport3D viewport)
        //{
        //    if (viewport == null)
        //    {
        //        throw new ArgumentNullException("viewport");
        //    }

        //    this.dispatcher = Dispatcher.CurrentDispatcher;
        //    this.Expansion = 1;
        //    this.fileDialogService = fds;
        //    this.viewport = viewport;
        //    this.FileOpenCommand = new DelegateCommand(this.FileOpen);
        //    this.FileExportCommand = new DelegateCommand(this.FileExport);
        //    this.FileExitCommand = new DelegateCommand(FileExit);
        //    this.ViewZoomExtentsCommand = new DelegateCommand(this.ViewZoomExtents);
        //    this.EditCopyXamlCommand = new DelegateCommand(this.CopyXaml);
        //    this.ApplicationTitle = "3D Model viewer";
        //    this.Elements = new List<VisualViewModel>();
        //    foreach (var c in viewport.Children)
        //    {
        //        this.Elements.Add(new VisualViewModel(c));
        //    }
        //}


        //Models.cls3DModel objcls = new Models.cls3DModel();
        public MainViewModel(string filename, HelixViewport3D viewport)
        {

            try
            {
                viewport.Children.Clear();
             
                if (Flag3D == false)
                {
                    if (viewport == null)
                    {
                        throw new ArgumentNullException("viewport");
                    }
                    DefaultLights light = new DefaultLights();
                    viewport.Children.Add(light);
                    //this.dispatcher = Dispatcher.CurrentDispatcher;
                    //this.Expansion = 1;
                    ////this.fileDialogService = fds;

                    //this.viewport = viewport;
                    //this.currentModelPath = filename;
                    //foreach (string file in Directory.EnumerateFiles(filename + "FuelOilTanks\\", "*.stl"))
                    //{
                    //    this.A = 150;
                    //    this.R = 128;
                    //    this.G = 0;
                    //    this.B = 0;
                    //    ModelVisual3D dev3D = new ModelVisual3D();
                    //    dev3D.Content = this.LoadAsync(file, false);
                    //    //viewport.Children.Add(dev3D);

                    //}
                    //foreach (string file in Directory.EnumerateFiles(filename + "Compartments\\", "*.stl"))
                    //{
                    //    this.A = 100;
                    //    this.R = 255;
                    //    this.G = 0;
                    //    this.B = 0;
                    //    ModelVisual3D dev3D = new ModelVisual3D();
                    //    dev3D.Content = this.LoadAsync(file, false);
                    //    //viewport.Children.Add(dev3D);
                    //}
                    //foreach (string file in Directory.EnumerateFiles(filename + "FreshWaterTanks\\", "*.stl"))
                    //{
                    //    this.A = 180;
                    //    this.R = 0;
                    //    this.G = 0;
                    //    this.B = 100;
                    //    ModelVisual3D dev3D = new ModelVisual3D();
                    //    dev3D.Content = this.LoadAsync(file, false);
                    //    //viewport.Children.Add(dev3D);
                    //}
                    //foreach (string file in Directory.EnumerateFiles(filename + "MiscTanks\\", "*.stl"))
                    //{
                    //    this.A = 180;
                    //    this.R = 255;
                    //    this.G = 128;
                    //    this.B = 192;
                    //    ModelVisual3D dev3D = new ModelVisual3D();
                    //    dev3D.Content = this.LoadAsync(file, false);
                    //    //viewport.Children.Add(dev3D);
                    //}
                    //foreach (string file in Directory.EnumerateFiles(filename + "WaterBallastTanks\\", "*.stl"))
                    //{
                    //    //this.A = 100;
                    //    //this.R = 0;
                    //    //this.G = 200;
                    //    //this.B = 0;
                    //    //ModelVisual3D dev3D = new ModelVisual3D();
                    //    //dev3D.Content = this.LoadAsync(file, false);
                    //    //viewport.Children.Add(dev3D);

                    //    this.A = 255;
                    //    this.R = 0;
                    //    this.G = 255;
                    //    this.B = 0;
                    //     ModelVisual3D dev3D2 = new ModelVisual3D();
                    //     Model3D vis3D= this.LoadAsync(file, false);
                    //     CuttingPlaneGroup  cuttingGrp = new CuttingPlaneGroup();
                    //     cuttingGrp.CuttingPlanes = new List<Plane3D> { new Plane3D(new Point3D(vis3D.Bounds.Location.X, vis3D.Bounds.Location.Y, vis3D.Bounds.Location.Z+4), new Vector3D(0, 0, 1)), new Plane3D(new Point3D(vis3D.Bounds.Location.X+20, vis3D.Bounds.Location.Y, vis3D.Bounds.Location.Z), new Vector3D(1, 0, 0)), new Plane3D(new Point3D(vis3D.Bounds.Location.X, vis3D.Bounds.Location.Y, vis3D.Bounds.Location.Z), new Vector3D(0, 0, 1)) };
                    //     cuttingGrp.Operation=CuttingOperation.Subtract;
                      
                    //    dev3D2.Content =vis3D;
                    //    viewport.Children.Add(dev3D2);
                    //    viewport.Children.Add(cuttingGrp);

                    //}
                    //viewport.Children.Add(Addplane());
                    foreach (string file in Directory.EnumerateFiles(filename, "*.stl"))
                    {
                        this.A = 90;
                        this.R = 120;
                        this.G = 120;
                        this.B = 120;
                        ModelVisual3D dev3D = new ModelVisual3D();
                        //dev3D.AnimateOpacity(20, 0);
                        dev3D.Content = this.LoadAsync(file, false);


                        //dev3D.Transform = new ScaleTransform3D(2000, 2000, 2000);
                        //dev3D.GetTransform();

                        viewport.Children.Add(dev3D);
                       
                        //viewport.Camera.Reset();
                        viewport.ZoomExtents(new Rect3D(new System.Windows.Media.Media3D.Point3D(80000, 0, -30000), new Size3D(0, 0, 60000)));
                        //viewport.ZoomExtents(0);
                    }
    //                List<ScreenSpaceLines3D> remove =
    //new List<ScreenSpaceLines3D>();
    //                List<ModelVisual3D> add =
    //                    new List<ModelVisual3D>();
    //                foreach (ModelVisual3D mv3d in viewport.Viewport.Children)
    //                {
    //                    if (mv3d is ScreenSpaceLines3D)
    //                    {
    //                        ScreenSpaceLines3D ss = mv3d as ScreenSpaceLines3D;
    //                        ModelVisual3D plain = new ModelVisual3D();
    //                        plain.Content = ss.Content;
    //                        add.Add(plain);
    //                        remove.Add(ss);
    //                    }
    //                }
                    //foreach (ModelVisual3D mv3d in add)
                    //{
                    //    viewport.Children.Add(mv3d);
                    //}
                    //foreach (ScreenSpaceLines3D ss in remove)
                    //{
                    //    viewport.Children.Remove(ss);
                    //}
                    viewport.IsZoomEnabled = true;
                    //viewPort3d.ZoomExtents(new Rect3D(new System.Windows.Media.Media3D.Point3D(80, 0, -30), new Size3D(0, 0, 60)));
                    viewport.ZoomExtents(new Rect3D(new System.Windows.Media.Media3D.Point3D(80000, 0, -30000), new Size3D(0, 0, 60000)));
                    //this.ViewZoomExtentsCommand = new DelegateCommand(this.ViewZoomExtents);
                    //this.EditCopyXamlCommand = new DelegateCommand(this.CopyXaml);
                    //this.ApplicationTitle = "3D Model viewer";
                    this.Elements = new List<VisualViewModel>();
                    foreach (var c in viewport.Children)
                    {
                        this.Elements.Add(new VisualViewModel(c));
                        //viewPortClone.Children.Add(c);

                    }
                    viewport.Children.CopyTo(Models.cls3DModel.Array3dModels, 0);
                    Flag3D = true;
                }
                else
                {
                    viewport.Children.Clear();
                    var arr = Models.cls3DModel.Array3dModels;
                    for (int i = 1; i < 3; i++)
                    //foreach (var c in Models.cls3DModel.Array3dModels)
                    {

                        viewport.Children.Add(arr[i]);
                    }
                }
            }
            catch
            {

            }
        }

      
       // public static Rect Get2DBoundingBox(System.Windows.Controls.Viewport3D vp)
       // {
       //     bool bOK;

       //     Viewport3DVisual vpv =
       //         VisualTreeHelper.GetParent(
       //             vp.Children[0]) as Viewport3DVisual;

       //     Matrix3D m =
       //         MathUtils.TryWorldToViewportTransform(vpv, out bOK);

       //     bool bFirst = true;
       //     Rect r = new Rect();

       //     foreach (Visual3D v3d in vp.Children)
       //     {
       //         if (v3d is ModelVisual3D)
       //         {
       //             ModelVisual3D mv3d = (ModelVisual3D)v3d;
       //             if (mv3d.Content is GeometryModel3D)
       //             {
       //                 GeometryModel3D gm3d =
       //                     (GeometryModel3D)mv3d.Content;

       //                 if (gm3d.Geometry is MeshGeometry3D)
       //                 {
       //                     MeshGeometry3D mg3d =
       //                         (MeshGeometry3D)gm3d.Geometry;

       //                     foreach (Point3D p3d in mg3d.Positions)
       //                     {
       //                         Point3D pb = m.Transform(p3d);
       //                         Point p2d = new Point(pb.X, pb.Y);
       //                         if (bFirst)
       //                         {
       //                             r = new Rect(p2d, new Size(1, 1));
       //                             bFirst = false;
       //                         }
       //                         else
       //                         {
       //                             r.Union(p2d);
       //                         }
       //                     }
       //                 }
       //             }
       //         }
       //     }

       //     return r;
       // }
       // public ModelVisual3D Addplane()
       // {
       //     ModelVisual3D floorModel = new ModelVisual3D();

       //     GeometryModel3D floorGeometry = new GeometryModel3D();
       //     MeshGeometry3D floorMesh = new MeshGeometry3D();
       //     floorMesh.Positions = FloorPoints3D;
       //     floorMesh.TriangleIndices = FloorPointsIndices;
       //     floorGeometry.Geometry = floorMesh;
       //     floorGeometry.Material= MaterialHelper.CreateMaterial(new SolidColorBrush(System.Windows.Media.Color.FromArgb(255,0,0,255)));
       //     floorGeometry.BackMaterial = MaterialHelper.CreateMaterial(new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 0, 255)));
       //     floorModel.Content = floorGeometry;
            
       //     return floorModel;

       // }
       // /// <summary>
       // /// Creates a ModelVisual3D containing a text label.
       // /// </summary>
       // /// <param name="text">The string</param>
       // /// <param name="textColor">The color of the text.</param>
       // /// <param name="bDoubleSided">Visible from both sides?</param>
       // /// <param name="height">Height of the characters</param>
       // /// <param name="center">The center of the label</param>
       // /// <param name="over">Horizontal direction of the label</param>
       // /// <param name="up">Vertical direction of the label</param>
       // /// <returns>Suitable for adding to your Viewport3D</returns>
       // public static ModelVisual3D CreateTextLabel3D(
       //     string text,
       //     Brush textColor,
       //     bool bDoubleSided,
       //     double height,
       //     Point3D center,
       //     Vector3D over,
       //     Vector3D up)
       // {
       //     // First we need a textblock containing the text of our label
       //     System.Windows.Controls.TextBlock tb = new System.Windows.Controls.TextBlock(new System.Windows.Documents.Run(text));
       //     tb.Foreground = textColor;
       //     tb.FontFamily = new FontFamily("Arial");

       //     // Now use that TextBlock as the brush for a material
       //     DiffuseMaterial mat = new DiffuseMaterial();
       //     mat.Brush = new VisualBrush(tb);

       //     // We just assume the characters are square
       //     double width = text.Length * height;

       //     // Since the parameter coming in was the center of the label,
       //     // we need to find the four corners
       //     // p0 is the lower left corner
       //     // p1 is the upper left
       //     // p2 is the lower right
       //     // p3 is the upper right
       //     Point3D p0 = center - width / 2 * over - height / 2 * up;
       //     Point3D p1 = p0 + up * 1 * height;
       //     Point3D p2 = p0 + over * width;
       //     Point3D p3 = p0 + up * 1 * height + over * width;

       //     // Now build the geometry for the sign.  It's just a
       //     // rectangle made of two triangles, on each side.

       //     MeshGeometry3D mg = new MeshGeometry3D();
       //     mg.Positions = new Point3DCollection();
       //     mg.Positions.Add(p0);    // 0
       //     mg.Positions.Add(p1);    // 1
       //     mg.Positions.Add(p2);    // 2
       //     mg.Positions.Add(p3);    // 3

       //     if (bDoubleSided)
       //     {
       //         mg.Positions.Add(p0);    // 4
       //         mg.Positions.Add(p1);    // 5
       //         mg.Positions.Add(p2);    // 6
       //         mg.Positions.Add(p3);    // 7
       //     }

       //     mg.TriangleIndices.Add(0);
       //     mg.TriangleIndices.Add(3);
       //     mg.TriangleIndices.Add(1);
       //     mg.TriangleIndices.Add(0);
       //     mg.TriangleIndices.Add(2);
       //     mg.TriangleIndices.Add(3);

       //     if (bDoubleSided)
       //     {
       //         mg.TriangleIndices.Add(4);
       //         mg.TriangleIndices.Add(5);
       //         mg.TriangleIndices.Add(7);
       //         mg.TriangleIndices.Add(4);
       //         mg.TriangleIndices.Add(7);
       //         mg.TriangleIndices.Add(6);
       //     }

       //     // These texture coordinates basically stretch the
       //     // TextBox brush to cover the full side of the label.

       //     mg.TextureCoordinates.Add(new Point(0, 1));
       //     mg.TextureCoordinates.Add(new Point(0, 0));
       //     mg.TextureCoordinates.Add(new Point(1, 1));
       //     mg.TextureCoordinates.Add(new Point(1, 0));

       //     if (bDoubleSided)
       //     {
       //         mg.TextureCoordinates.Add(new Point(1, 1));
       //         mg.TextureCoordinates.Add(new Point(1, 0));
       //         mg.TextureCoordinates.Add(new Point(0, 1));
       //         mg.TextureCoordinates.Add(new Point(0, 0));
       //     }

       //     // And that's all.  Return the result.

       //     ModelVisual3D mv3d = new ModelVisual3D();
       //     mv3d.Content = new GeometryModel3D(mg, mat); ;
       //     return mv3d;
       // }


       // private Point3DCollection FloorPoints3D
       // {
       //     get
       //     {
       //         double x = 10.0; // floor width / 2
       //         double z = 10.0; // floor length / 2
       //         double floorDepth = 50; // give the floor some depth so it's not a 2 dimensional plane 

       //         Point3DCollection points = new Point3DCollection(20);
       //         System.Windows.Media.Media3D.Point3D point;
       //         //top of the floor
       //         point = new System.Windows.Media.Media3D.Point3D(0, z,0);// Floor Index - 0
       //         points.Add(point);
       //         point = new System.Windows.Media.Media3D.Point3D(x, z, 0);// Floor Index - 1
       //         points.Add(point);
       //         point = new System.Windows.Media.Media3D.Point3D(x, -z, 0);// Floor Index - 2
       //         points.Add(point);
       //         point = new System.Windows.Media.Media3D.Point3D(0, -z, 0);// Floor Index - 3
       //         points.Add(point);
       //         //front side
       //         point = new System.Windows.Media.Media3D.Point3D(0, z, 0);// Floor Index - 4
       //         points.Add(point);
       //         point = new System.Windows.Media.Media3D.Point3D(0, z, floorDepth);// Floor Index - 5
       //         points.Add(point);
       //         point = new System.Windows.Media.Media3D.Point3D(x, z, floorDepth);// Floor Index - 6
       //         points.Add(point);
       //         point = new System.Windows.Media.Media3D.Point3D(x, z, 0);// Floor Index - 7
       //         points.Add(point);
       //         //right side
       //         point = new System.Windows.Media.Media3D.Point3D(x, z, 0);// Floor Index - 8
       //         points.Add(point);
       //         point = new System.Windows.Media.Media3D.Point3D(x, z, floorDepth);// Floor Index - 9
       //         points.Add(point);
       //         point = new System.Windows.Media.Media3D.Point3D(x, -z, floorDepth);// Floor Index - 10
       //         points.Add(point);
       //         point = new System.Windows.Media.Media3D.Point3D(x, -z, 0);// Floor Index - 11
       //         points.Add(point);
       //         //back side
       //         point = new System.Windows.Media.Media3D.Point3D(x, -z, 0);// Floor Index - 12
       //         points.Add(point);
       //         point = new System.Windows.Media.Media3D.Point3D(x, -z, floorDepth);// Floor Index - 13
       //         points.Add(point);
       //         point = new System.Windows.Media.Media3D.Point3D(0, -z, floorDepth);// Floor Index - 14
       //         points.Add(point);
       //         point = new System.Windows.Media.Media3D.Point3D(0, -z, 0);// Floor Index - 15
       //         points.Add(point);
       //         //left side
       //         point = new System.Windows.Media.Media3D.Point3D(0, -z, 0);// Floor Index - 16
       //         points.Add(point);
       //         point = new System.Windows.Media.Media3D.Point3D(0, -z, floorDepth);// Floor Index - 17
       //         points.Add(point);
       //         point = new System.Windows.Media.Media3D.Point3D(0, z, floorDepth);// Floor Index - 18
       //         points.Add(point);
       //         point = new System.Windows.Media.Media3D.Point3D(0, z, 0);// Floor Index - 19
       //         points.Add(point);
       //         return points;
       //     }
       // }
       // public Int32Collection FloorPointsIndices
       // {
       //     get
       //     {
       //         int[] indices = new int[] { 0, 1, 2, 0, 2, 3, 4, 5, 7, 5, 6, 7, 8, 9, 11, 9, 10, 11, 12, 13, 15, 13,
       //14, 15, 16, 17, 19, 17, 18, 19 };
       //         //int[] indices = new int[] { 0, 1, 2, 0, 2,3 };
       //         return new Int32Collection(indices);
       //     }
       // }


        public static bool Flag3D { get; set; }
        
        public string CurrentModelPath
        {
            get
            {
                return this.currentModelPath;
            }

            set
            {
                this.currentModelPath = value;
                this.RaisePropertyChanged("CurrentModelPath");
            }
        }

        //public string ApplicationTitle
        //{
        //    get
        //    {
        //        return this.applicationTitle;
        //    }

        //    set
        //    {
        //        this.applicationTitle = value;
        //        this.RaisePropertyChanged("ApplicationTitle");
        //    }
        //}

        public List<VisualViewModel> Elements { get; set; }
       
        //public double Expansion
        //{
        //    get
        //    {
        //        return this.expansion;
        //    }

        //    set
        //    {
        //        if (!this.expansion.Equals(value))
        //        {
        //            this.expansion = value;
        //            this.RaisePropertyChanged("Expansion");
        //        }
        //    }
        //}
        public byte A { get; set; }
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        public Model3D CurrentModel
        {
            get
            {
                return this.currentModel;
            }

            set
            {
                this.currentModel = value;
                this.RaisePropertyChanged("CurrentModel");
            }
        }

        //public ICommand FileOpenCommand { get; set; }

        //public ICommand FileExportCommand { get; set; }

        //public ICommand FileExitCommand { get; set; }

        //public ICommand HelpAboutCommand { get; set; }

        //public ICommand ViewZoomExtentsCommand { get; set; }

        //public ICommand EditCopyXamlCommand { get; set; }

        //private static void FileExit()
        //{
        //    Application.Current.Shutdown();
        //}

        //private void FileExport()
        //{
        //    var path = this.fileDialogService.SaveFileDialog(null, null, Exporters.Filter, ".png");
        //    if (path == null)
        //    {
        //        return;
        //    }

        //    this.viewport.Export(path);
        //}

        //private void CopyXaml()
        //{
        //    var rd = XamlExporter.WrapInResourceDictionary(this.CurrentModel);
        //    Clipboard.SetText(XamlHelper.GetXaml(rd));
        //}

        //private void ViewZoomExtents()
        //{
        //    this.viewport.ZoomExtents(500);
        //}
        //private  void FileOpen()
        //{

        //    this.CurrentModelPath = this.fileDialogService.OpenFileDialog("models", null, OpenFileFilter, ".3ds");
        //    this.CurrentModel = this.LoadAsync(this.CurrentModelPath, false);
        //    this.ApplicationTitle = string.Format(TitleFormatString, this.CurrentModelPath);
        //    this.viewport.ZoomExtents(0);
        //}

        private  Model3D LoadAsync(string model3DPath, bool freeze)
        {
           // return  Task.Factory.StartNew(() =>
            {
                var mi = new ModelImporter();
                SolidColorBrush br = new SolidColorBrush(System.Windows.Media.Color.FromArgb(this.A, this.R, this.G, this.B));
                br.Opacity = 0.3;
                mi.DefaultMaterial = MaterialHelper.CreateMaterial(br);

                //if (freeze)
                //{
                //    // Alt 1. - freeze the model 
                //    return mi.Load(model3DPath, null,false);
                //}
                Model3D model = mi.Load(model3DPath, this.dispatcher, false);
                //model.Transform = new ScaleTransform3D(new Vector3D(1, 1, 1));
                return model;
                // Alt. 2 - create the model on the UI dispatcher
                //return mi.Load(model3DPath, this.dispatcher,false);
            }//);
        }
        //private async void FileOpen()
        //{
        //    this.CurrentModelPath = this.fileDialogService.OpenFileDialog("models", null, OpenFileFilter, ".3ds");
        //    this.CurrentModel = await this.LoadAsync(this.CurrentModelPath, false);
        //    this.ApplicationTitle = string.Format(TitleFormatString, this.CurrentModelPath);
        //    this.viewport.ZoomExtents(0);
        //}

        //private async Task<Model3DGroup> LoadAsync(string model3DPath, bool freeze)
        //{
        //    return await Task.Factory.StartNew(() =>
        //    {
        //        var mi = new ModelImporter();
                
        //        if (freeze)
        //        {
        //            // Alt 1. - freeze the model 
        //            return mi.Load(model3DPath, null, true);
        //        }

        //        // Alt. 2 - create the model on the UI dispatcher
        //        return mi.Load(model3DPath, this.dispatcher);
        //    });
        //}
    }
}