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

namespace WpfMvvmStability.Views
{
    /// <summary>
    /// Interaction logic for CustomeInpute.xaml
    /// </summary>
    public partial class CustomeInpute : Window
    {
        public string loading_Name = string.Empty;
        public CustomeInpute()
        {
                InitializeComponent();
            txtLoadingSave.Focus();
        }

        private void btnsave_Click(object sender, RoutedEventArgs e)
        {
             loading_Name = txtLoadingSave.Text;
            txtLoadingSave.Focus();
            txtLoadingSave.Focus();
            this.DialogResult = true;
        }

        private void btnCancell_Click(object sender, RoutedEventArgs e)
        {
            txtLoadingSave.Clear();
            this.DialogResult = false;
            this.Close();
        }
    }
}
