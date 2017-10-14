using System;
using System.Windows;
using System.Windows.Controls;

namespace Installer
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void ChooseInstallDirectory(object sender, RoutedEventArgs e)
        {
            var dlg = new System.Windows.Forms.FolderBrowserDialog();
            var result = dlg.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                if (Application.Current.Properties.Contains("installPath"))
                {
                    Application.Current.Properties["installPath"] = dlg.SelectedPath;
                }
                else
                {
                    Application.Current.Properties.Add("installPath", dlg.SelectedPath);
                }
            }
        }

        private void NavigateToApplicationPoolSetup(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null)
                NavigationService.Navigate(new Uri("ScriptSetupPage.xaml", UriKind.Relative));
        }
    }
}