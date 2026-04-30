using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfMvvmStability.Views
{
    public partial class ModernMessageBox : UserControl
    {
        private Window hostWindow;
        private MessageBoxResult result = MessageBoxResult.None;

        public ModernMessageBox()
        {
            InitializeComponent();
        }

        public static MessageBoxResult Show(string message, string title = "Information", MessageBoxType type = MessageBoxType.Info, MessageBoxButton buttons = MessageBoxButton.OK)
        {
            var messageBox = new ModernMessageBox();
            var window = new Window
            {
                Width = 460,
                SizeToContent = SizeToContent.Height,
                MinHeight = 220,
                WindowStyle = WindowStyle.None,
                AllowsTransparency = true,
                Background = Brushes.Transparent,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize,
                ShowInTaskbar = false,
                Content = messageBox
            };

            var owner = Application.Current != null ? Application.Current.MainWindow : null;
            if (owner != null && owner.IsVisible && !ReferenceEquals(owner, window))
            {
                window.Owner = owner;
            }
            else
            {
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            messageBox.hostWindow = window;
            messageBox.Configure(message, title, type, buttons);
            window.ShowDialog();
            return messageBox.result == MessageBoxResult.None ? MessageBoxResult.OK : messageBox.result;
        }

        private void Configure(string message, string title, MessageBoxType type, MessageBoxButton buttons)
        {
            txtTitle.Text = title;
            txtMessage.Text = message;
            btnOK.Content = buttons == MessageBoxButton.YesNo || buttons == MessageBoxButton.YesNoCancel ? "Yes" : "OK";
            btnNo.Visibility = buttons == MessageBoxButton.YesNo || buttons == MessageBoxButton.YesNoCancel ? Visibility.Visible : Visibility.Collapsed;
            btnCancel.Visibility = buttons == MessageBoxButton.OKCancel || buttons == MessageBoxButton.YesNoCancel ? Visibility.Visible : Visibility.Collapsed;

            Color headerColor = Color.FromRgb(31, 72, 255);
            Color iconColor = Color.FromRgb(37, 99, 235);
            switch (type)
            {
                case MessageBoxType.Success:
                    txtIcon.Text = "OK";
                    break;
                case MessageBoxType.Warning:
                    txtIcon.Text = "!";
                    break;
                case MessageBoxType.Error:
                    txtIcon.Text = "X";
                    break;
                default:
                    txtIcon.Text = "i";
                    break;
            }

            HeaderBorder.Background = new SolidColorBrush(headerColor);
            IconBorder.Background = new SolidColorBrush(iconColor);
            btnOK.Background = new SolidColorBrush(headerColor);
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            result = btnOK.Content != null && btnOK.Content.ToString() == "Yes" ? MessageBoxResult.Yes : MessageBoxResult.OK;
            hostWindow?.Close();
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            result = MessageBoxResult.No;
            hostWindow?.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            result = MessageBoxResult.Cancel;
            hostWindow?.Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            result = MessageBoxResult.Cancel;
            hostWindow?.Close();
        }
    }

    public enum MessageBoxType
    {
        Info,
        Success,
        Warning,
        Error
    }
}
