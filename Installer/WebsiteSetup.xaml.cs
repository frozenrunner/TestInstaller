using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Installer
{
    /// <summary>
    /// Interaction logic for WebsiteSetup.xaml
    /// </summary>
    public partial class WebsiteSetup : Page
    {
        public WebsiteSetup()
        {
            InitializeComponent();
        }

        private void FinishInstallation(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TxtWebsiteName.Text))
            {
                Application.Current.Properties.Add("websiteName", TxtWebsiteName.Text);
                //System.Threading.Tasks.Task.Factory.StartNew(() =>
                //{
                    try
                    {
                        var process = new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = @"appcmd.exe",
                                WorkingDirectory = Environment.SystemDirectory + @"\inetsrv\",
                                UseShellExecute = true,
                                CreateNoWindow = false
                            }
                        };

                        var arguments = string.Format("add site /name:{0} ", TxtWebsiteName.Text);
                        arguments += string.Format("/physicalPath:{0} ", Application.Current.Properties["installPath"]);
                        arguments += string.Format("/+bindings.[protocol='http',bindingInformation='*:80:{0}'] ",
                            TxtWebsiteName.Text);
                        arguments += string.Format("/applicationDefaults.applicationPool:{0} ",
                            Application.Current.Properties["applicationPoolName"]);

                        process.StartInfo.Arguments = arguments;

                        process.Start();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                //}).ContinueWith(t =>
                //{
                    if (NavigationService != null)
                    {
                        NavigationService.Navigate(new Uri("ScriptSetupPage.xaml", UriKind.Relative));
                    }
                //}, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());

                BtnNext.IsEnabled = false;
                ExtractFilesProgress.Visibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("You must enter an website name", "Install Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
