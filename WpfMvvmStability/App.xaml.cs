using System.Windows;
using System.Windows.Threading;
using System.Threading;
using System;
using System.Diagnostics;
using System.Windows.Navigation;
using System.IO;

namespace WpfMvvmStability
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
     
    public partial class App : Application
    {   
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        }
    }
}
