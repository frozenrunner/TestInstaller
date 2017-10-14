using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Installer
{
    /// <summary>
    /// Interaction logic for ApplicationPoolSetup.xaml
    /// </summary>
    public partial class ApplicationPoolSetup : Page
    {
        public ApplicationPoolSetup()
        {
            InitializeComponent();
        }

        private void NavigateToWebsiteSetup(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TxtAppPoolName.Text))
            {
                BtnNext.IsEnabled = false;
                Application.Current.Properties.Add("applicationPoolName", TxtAppPoolName.Text);
                    try
                    {
                        var process = new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = @"appcmd.exe",
                                WorkingDirectory = Environment.SystemDirectory + @"\inetsrv\",
                                UseShellExecute = true,
                                CreateNoWindow = true
                            }
                        };

                        var arguments = string.Format("add apppool /name:{0} ", TxtAppPoolName.Text);
                        arguments += "/managedPipelineMode:Integrated ";
                        arguments += "/enable32BitAppOnWin64:true ";
                        arguments += "/processModel.idleTimeout:00:00:00 ";
                        arguments += "/processModel.loadUserProfile:false ";
                        arguments += "/recycling.periodicRestart.time:00:00:00 ";
                        arguments += "/+recycling.periodicRestart.schedule.[value='03:00:00'] ";
                        arguments += "/startMode:AlwaysRunning ";

                        process.StartInfo.Arguments = arguments;

                        process.Start();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                    if (NavigationService != null)
                    {
                        NavigationService.Navigate(new Uri("WebsiteSetup.xaml", UriKind.Relative));
                    }
                
                
                
            }
            else
            {
                MessageBox.Show("You must enter an application pool name", "Install Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
