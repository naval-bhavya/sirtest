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
using System.IO;

namespace WpfApplicationForPdf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow1 : Window
    {
        public MainWindow1()
        {
            InitializeComponent();
        }
        string path = "";
        private void ViewPDF_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                listBoxPDF.Items.Clear();
                lblListBoxError.Visibility = Visibility.Hidden;
                string st = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                path = st + "\\Reports";

                var dir = Directory.GetFiles(path);

                string names;
                foreach (string s in dir)
                {
                    //names = s.Remove(0, index);
                    listBoxPDF.Items.Add(System.IO.Path.GetFileName(s));
                }
                listBoxPDF.Items.SortDescriptions.Add(
                        new System.ComponentModel.SortDescription("",
                        System.ComponentModel.ListSortDirection.Ascending));
                if (listBoxPDF.Items.Count == 0)
                {
                    lblListBoxError.Visibility = Visibility.Visible;
                }

                lblBrowserError.Visibility = Visibility.Hidden;

                webBrowserPDF.Navigate(path + "\\" + "Default_Report.pdf");
            }
            catch
            {

            }
        }

        private void listBoxPDF_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                lblBrowserError.Visibility = Visibility.Hidden;
                webBrowserPDF.Navigate(path + "\\" + listBoxPDF.SelectedItem.ToString());
            }
            catch
            {
            }
        }
    }
}
