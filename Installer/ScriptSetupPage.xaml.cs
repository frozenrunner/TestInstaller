using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

//TODO: Error handling for inputs.
//TODO: Sanitize input if necessary

namespace Installer
{
    /// <summary>
    /// Interaction logic for ScriptSetupPage.xaml
    /// </summary>
    public partial class ScriptSetupPage : Page
    {
        public ScriptSetupPage()
        {
            InitializeComponent();
        }

        private void CreateDbTest_OnClick(object sender, RoutedEventArgs e)
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = TxtServerName.Text,
                UserID = TxtUserName.Text,
                Password = TxtPassword.Password
            };
            //System.Threading.Tasks.Task.Factory.StartNew(() =>
            //{
                try
                {
                    using (var connection = new SqlConnection(builder.ToString()))
                    {
                        var dataBaseAttachText = string.Format("CREATE DATABASE [{0}]", TxtPrimaryDbName.Text);

                        connection.Open();
                        var cmd = new SqlCommand(dataBaseAttachText, connection);
                        cmd.ExecuteNonQuery();
                    }

                    using (var connection = new SqlConnection(builder.ToString()))
                    {
                        connection.Open();
                        connection.ChangeDatabase(TxtPrimaryDbName.Text);
                        var transaction = connection.BeginTransaction("insertScriptTransaction");
                        var cmd = new SqlCommand
                        {
                            Connection = connection,
                            Transaction = transaction
                        };

                        //var server = new Server(new ServerConnection(connection));
                        try
                        {
                            var removeGoStrings = new [] {"SPLIT_ME"};
                            const string splitMeString = "SPLIT_ME";
                            var regexReplace = new Regex("\\bGO\\b(\\r|\\n|\\s+$|$)", RegexOptions.IgnoreCase|RegexOptions.Multiline);

                            var newCustomerScript = new FileInfo(Application.Current.Properties["installPath"] +
                                                                 @"\SQL\Schema.sql").OpenText()
                                .ReadToEnd();
                            var newCustomerScriptQueries = regexReplace.Replace(newCustomerScript, splitMeString).Split(removeGoStrings, StringSplitOptions.RemoveEmptyEntries);

                            var post42Script = new FileInfo(Application.Current.Properties["installPath"] +
                                                            @"\SQL\Schema - New.sql").OpenText().ReadToEnd().Replace("GP_DATABASE_NAME_GOES_HERE", TxtGpDbName.Text);
                            var post42ScriptQueries = regexReplace.Replace(post42Script, splitMeString).Split(removeGoStrings, StringSplitOptions.RemoveEmptyEntries);

                            var configScript = new FileInfo(Application.Current.Properties["installPath"] +
                                                                  @"\SQL\Schema - Config.sql").OpenText()
                                .ReadToEnd();
                            var configScriptQueries = regexReplace.Replace(configScript, splitMeString).Split(removeGoStrings, StringSplitOptions.RemoveEmptyEntries);

                            var quadarStoredProcedureScript = new FileInfo(Application.Current.Properties["installPath"] +
                                                                           @"\SQL\ - StoredProcedures.sql").OpenText()
                                .ReadToEnd();
                            var quadarStoredProcedureScriptQueries = regexReplace.Replace(quadarStoredProcedureScript, splitMeString).Split(removeGoStrings, StringSplitOptions.RemoveEmptyEntries);

                            var sampleQuoteScript = new FileInfo(Application.Current.Properties["installPath"] +
                                                                           @"\SQL\sample_quote_template_insert.sql").OpenText()
                                .ReadToEnd();
                            var sampleQuoteScriptQueries = regexReplace.Replace(sampleQuoteScript, splitMeString).Split(removeGoStrings, StringSplitOptions.RemoveEmptyEntries);

                            var gpStoredProcedureScript = new FileInfo(Application.Current.Properties["installPath"] +
                                                                       @"\SQL\GP - StoredProcedures.sql").OpenText()
                                .ReadToEnd();
                            var gpStoredProcedureScriptQueries = regexReplace.Replace(gpStoredProcedureScript, splitMeString).Split(removeGoStrings, StringSplitOptions.RemoveEmptyEntries);

                            var createReadOnlyUser = new FileInfo(Application.Current.Properties["installPath"] +
                                                                       @"\SQL\createReadOnlyUserEXEC.sql").OpenText()
                                .ReadToEnd().Replace("REPLACE_WITH_DB_NAME", TxtPrimaryDbName.Text);
                            var createReadOnlyUserQueries = regexReplace.Replace(createReadOnlyUser, splitMeString).Split(removeGoStrings, StringSplitOptions.RemoveEmptyEntries);

                            var createReadOnlyUserFinancial = new FileInfo(Application.Current.Properties["installPath"] +
                                                                       @"\SQL\createReadOnlyUserEXEC.sql").OpenText()
                                .ReadToEnd().Replace("REPLACE_WITH_DB_NAME", TxtGpDbName.Text);
                            var createReadOnlyUserFinancialQueries = regexReplace.Replace(createReadOnlyUserFinancial, splitMeString).Split(removeGoStrings, StringSplitOptions.RemoveEmptyEntries);
                            //server.ConnectionContext.BeginTransaction();

                            //TODO: FIX COMMENT IN FILE THAT FAAAAAILS QU-8014

                            RunQueries(cmd, newCustomerScriptQueries);
                            RunQueries(cmd, post42ScriptQueries);
                            RunQueries(cmd, configScriptQueries);
                            RunQueries(cmd, quadarStoredProcedureScriptQueries);
                            RunQueries(cmd, sampleQuoteScriptQueries);
                            RunQueries(cmd, createReadOnlyUserQueries);

                            connection.ChangeDatabase(TxtGpDbName.Text);
                            
                            RunQueries(cmd, gpStoredProcedureScriptQueries);
                            RunQueries(cmd, createReadOnlyUserFinancialQueries);
                            
                            cmd.Transaction.Commit();
                            //server.ConnectionContext.ExecuteNonQuery(newCustomerScript);
                            //server.ConnectionContext.ExecuteNonQuery(post42Script);
                            //server.ConnectionContext.ExecuteNonQuery(ConfigScript);
                            //server.ConnectionContext.ExecuteNonQuery(quadarStoredProcedureScript);
                            //server.ConnectionContext.ExecuteNonQuery(sampleQuoteScript);

                            //createReadOnlyUser = createReadOnlyUser.Replace("REPLACE_WITH_DB_NAME", TxtDbName.Text);
                            //server.ConnectionContext.ExecuteNonQuery(createReadOnlyUser);

                            //connection.ChangeDatabase(TxtGpDbName.Text);
                            //server.ConnectionContext.ExecuteNonQuery(gpStoredProcedureScript);

                            //createReadOnlyUserFinancial = createReadOnlyUserFinancial.Replace("REPLACE_WITH_DB_NAME", TxtGpDbName.Text);
                            //server.ConnectionContext.ExecuteNonQuery(createReadOnlyUserFinancial);
                        
                            //server = new Server(new ServerConnection(connection));

                            //server.ConnectionContext.CommitTransaction();

                            if (Application.Current.Properties.Contains("installPath"))
                            {
                                var sqlInstallPath = (string)Application.Current.Properties["installPath"] + "\\SQL";

                                using (var pluginZipResourceStream = System.Reflection.Assembly.GetExecutingAssembly()
                                        .GetManifestResourceStream("Installer.EmbeddedResources.GPSetupDatabaseScripts.zip"))
                                {
                                    var tempInstallPath = Application.Current.Properties["installPath"] + "\\SQL\\tempZip.zip";
                                    using (var bw = new FileStream(tempInstallPath, FileMode.Create))
                                    {
                                        if (pluginZipResourceStream != null) pluginZipResourceStream.CopyTo(bw);
                                    }
                                    //TODO: Proper error handling if problem pulling zip from resource manifest
                                    try
                                    {
                                        ZipFile.ExtractToDirectory(tempInstallPath, sqlInstallPath);
                                        var process1 = new Process
                                        {
                                            StartInfo = new ProcessStartInfo
                                            {
                                                FileName = @"deploy.bat",
                                                WorkingDirectory = sqlInstallPath + @"\ Connector Customizations\SQL Scripts\",
                                                UseShellExecute = true,
                                                CreateNoWindow = true
                                            }
                                        };

                                        var arguments1 = string.Format(@"""{0}"" ""{1}"" ""{2}"" ""{3}""", TxtServerName.Text, TxtGpDbName.Text, TxtUserName.Text, TxtPassword.Password);
                                        process1.StartInfo.Arguments = arguments1;

                                        process1.Start();

                                        var process2 = new Process
                                        {
                                            StartInfo = new ProcessStartInfo
                                            {
                                                FileName = @"deploy.bat",
                                                WorkingDirectory = sqlInstallPath + @"\ Launch Customizations\SQL Scripts\",
                                                UseShellExecute = true,
                                                CreateNoWindow = true
                                            }
                                        };

                                        var arguments2 = string.Format(@"""{0}"" ""{1}"" ""{2}"" ""{3}""", TxtServerName.Text, TxtGpDbName.Text, TxtUserName.Text, TxtPassword.Password);
                                        process2.StartInfo.Arguments = arguments2;

                                        process2.Start();
                                    }
                                    catch (IOException iox)
                                    {
                                        MessageBox.Show(iox.Message);
                                    }
                                    finally
                                    {
                                        File.Delete(tempInstallPath);
                                    }
                                }
                            }

                            using (var webConfigResourceStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Installer.EmbeddedResources.Web.config"))
                            {
                                var xmlDoc = new XmlDocument();
                                if (webConfigResourceStream != null)
                                {
                                    string userName, password;
                                    //Setup account if needed
                                    if (ChkNewAccount.IsChecked != null && ChkNewAccount.IsChecked.Value)
                                    {
                                        userName = TxtLoginUserName.Text;
                                        password = TxtLoginPassword.Password;
                                    }
                                    else
                                    {
                                        userName = TxtPrimaryDbName.Text;
                                        password = TxtPassword.Password;
                                    }

                                    xmlDoc.Load(webConfigResourceStream);
                                    var aNodes = xmlDoc.SelectNodes("/configuration/connectionStrings/add");
                                    var ConnectionString = string.Format(aNodes[0].Attributes[1].Value, TxtServerName.Text, TxtPrimaryDbName.Text, userName, password);
                                    aNodes[0].Attributes[1].Value = ConnectionString;

                                    var financialConnectionString = string.Format(aNodes[1].Attributes[1].Value, TxtServerName.Text, TxtGpDbName.Text, userName, password);
                                    aNodes[1].Attributes[1].Value = financialConnectionString;

                                    var readOnlyConnectionString = string.Format(aNodes[2].Attributes[1].Value, TxtServerName.Text, TxtPrimaryDbName.Text);
                                    aNodes[2].Attributes[1].Value = readOnlyConnectionString;

                                    var readOnlyFinancialConnectionString = string.Format(aNodes[3].Attributes[1].Value, TxtServerName.Text, TxtGpDbName.Text);
                                    aNodes[3].Attributes[1].Value = readOnlyFinancialConnectionString;

                                    xmlDoc.Save(Application.Current.Properties["installPath"] + "\\web.config");
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            cmd.Transaction.Rollback();
                            //server.ConnectionContext.RollBackTransaction();
                            throw;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    using (var connection = new SqlConnection(builder.ToString()))
                    {
                        try
                        {
                            connection.Open();
                            var dataBaseAttachText = string.Format("DROP DATABASE {0}", TxtPrimaryDbName.Text);

                            var cmd = new SqlCommand(dataBaseAttachText, connection);
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception innerEx)
                        {
                            MessageBox.Show(innerEx.Message);
                        }
                    }
                }

                if (NavigationService != null)
                {
                    NavigationService.Navigate(new Uri("FinishedInstallPage.xaml", UriKind.Relative));
                }

            BtnNext.IsEnabled = false;
            ExtractFilesProgress.Visibility = Visibility.Visible;
        }

        private static void RunQueries(IDbCommand cmd, IEnumerable<string> queries)
        {
            foreach (var query in queries)
            {
                try
                {
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    MessageBox.Show(query);
                }
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            PnlNewAccount.Visibility = Visibility.Visible;
        }


        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            PnlNewAccount.Visibility = Visibility.Hidden;
        }
    }
}
