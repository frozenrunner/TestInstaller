using System;
using System.Reflection;
using System.Windows;

namespace Installer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                var resourceName = "AssemblyLoadingAndReflection." + new AssemblyName(args.Name).Name +".dll";
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    var assemblyData = new byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);
                    return Assembly.Load(assemblyData);
                }
            };
            InstallerFrame.Navigate(new InstallerFrame());
            NavigationFrame.Navigate(new MainPage());
        }
    }
}

//TODO: Add box for license key to be added to web.config if present