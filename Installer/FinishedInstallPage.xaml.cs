using System.Windows;
using System.Windows.Controls;

namespace Installer
{
    /// <summary>
    /// Interaction logic for FinishedInstallPage.xaml
    /// </summary>
    public partial class FinishedInstallPage : Page
    {
        public FinishedInstallPage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
