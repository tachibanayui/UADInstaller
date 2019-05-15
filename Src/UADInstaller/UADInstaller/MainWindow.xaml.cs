using MahApps.Metro.Controls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using UADInstaller.Jsons;
using ErrorEventArgs = Newtonsoft.Json.Serialization.ErrorEventArgs;

namespace UADInstaller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow, INotifyPropertyChanged
    {
        public List<Product> AllProducts { get; set; } = new List<Product>();
        public string UADLocStore { get; set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UADInstallationLocation.txt");
        public VersionManager LocalVersionManager;
        public const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.119 Safari/537.36";
        public Release[] ReleasesChangelog { get; set; }

        public const string Version = "v1.0";

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();

            GetInfomation();
            GetChangeLog();
        }

        private bool CheckForUADLocStore(out string res)
        {
            try
            {
                if (File.Exists(UADLocStore))
                {
                    var uadLoc = File.ReadAllText(UADLocStore);
                    if (File.Exists(uadLoc))
                    {
                        res = uadLoc;
                        return true;
                    }
                }

            }
            catch
            {
                res = string.Empty;
                return false;
            }

            res = string.Empty;
            return false;
        }

        private void CheckInstallationStatus()
        {
            string res;
            if (CheckForUADLocStore(out res))
            {
                btnInstall.Content = "Update";
                btnUnistall.IsEnabled = true;
                btnLocate.IsEnabled = false;

                var localVerionText = File.ReadAllText(res);
                LocalVersionManager = JsonConvert.DeserializeObject<VersionManager>(localVerionText);
                txblVersion.Text = $"Version: {LocalVersionManager.GlobalVersion}";
                installLoc.Text = new DirectoryInfo(res).Parent.FullName;
                if (CompareVersion(ReleasesChangelog[0].tag_name, LocalVersionManager.GlobalVersion))
                {
                    btnInstall.IsEnabled = true;
                    btnLaunch.IsEnabled = false;
                }
                else
                {
                    btnInstall.IsEnabled = false;
                    btnLaunch.IsEnabled = true;
                }
            }
            else
            {
                btnInstall.Content = "Install";
                btnInstall.IsEnabled = true;
                btnLaunch.IsEnabled = false;
                btnLocate.IsEnabled = true;
                btnUnistall.IsEnabled = false;
            }
        }

        private async void GetChangeLog()
        {
            try
            {
                var infoReq = (HttpWebRequest)WebRequest.Create("https://api.github.com/repos/quangaming2929/UniversalAnimeDownloader/releases");
                infoReq.UserAgent = UserAgent;
                using (var infoResp = await infoReq.GetResponseAsync())
                {
                    using (var stream = infoResp.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream);
                        string content = await reader.ReadToEndAsync();
                        var jsonSetting = new JsonSerializerSettings()
                        {
                            Error = new EventHandler<ErrorEventArgs>((s, e) => e.ErrorContext.Handled = true),
                            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full,
                        };
                        ReleasesChangelog = JsonConvert.DeserializeObject<Release[]>(content);
                        foreach (var item in ReleasesChangelog)
                            item.body = AddHtmlColorBody(item.body, Colors.White);
                        changelog.ItemsSource = ReleasesChangelog;
                    }
                }

                CheckInstallationStatus();
            }
            catch
            {
                btnInstall.Content = "Offline";
                btnInstall.IsEnabled = false;
                string res;
                if(CheckForUADLocStore(out res))
                {
                    DirectoryInfo info = new DirectoryInfo(res);
                    var exe = Path.Combine(info.Parent.FullName, "UniversalAnimeDownloader.exe");
                    if (File.Exists(exe))
                    {
                        btnLaunch.IsEnabled = true;
                        btnUnistall.IsEnabled = true;
                        btnLocate.IsEnabled = false;
                    }
                    else
                    {
                        btnLaunch.IsEnabled = false;
                        btnUnistall.IsEnabled = false;
                        btnLocate.IsEnabled = true;
                    }
                }
            }

        }

        private async void GetInfomation()
        {
            try
            {
                var infoReq = (HttpWebRequest)WebRequest.Create("https://raw.githubusercontent.com/quangaming2929/UniversalAnimeDownloader/master/README.md");
                infoReq.UserAgent = UserAgent;
                using (var infoResp = await infoReq.GetResponseAsync())
                {
                    using (var stream = infoResp.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream);
                        string content = await reader.ReadToEndAsync();
                        htmlInfo.Text = AddHtmlColorBody(content, Colors.White);
                    }
                }
            }
            catch
            {
                htmlInfo.Text = "Failed to get info data :(";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static string AddHtmlColorBody(string content, Color color)
        {
            string rgbValue = $"rgb({color.R}, {color.G}, {color.B})";
            return $"<body style=\"color: {rgbValue}\">" + content + " </body>";
        }

        private void FeedbackButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Process.Start("https://github.com/quangaming2929/UniversalAnimeDownloader/issues");
        }

        private static bool CompareVersion(string v1, string v2)
        {
            var newVer = v1.Substring(1).Split('.');
            var oldVer = v2.Substring(1).Split('.');

            for (int i = 0; i < Math.Min(newVer.Length, oldVer.Length); i++)
            {
                if (int.Parse(newVer[i]) > int.Parse(oldVer[i]))
                    return true;

                if (int.Parse(newVer[i]) < int.Parse(oldVer[i]))
                    return false;
            }
            return false;
        }

        private void Event_LaunchUAD(object sender, System.Windows.RoutedEventArgs e)
        {
            string res;
            if (CheckForUADLocStore(out res))
            {
                DirectoryInfo info = new DirectoryInfo(res);
                var exe = Path.Combine(info.Parent.FullName, "UniversalAnimeDownloader.exe");
                Process.Start(exe);
            }
        }

        private void Event_GetFolder(object sender, System.Windows.RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if(dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                installLoc.Text = dialog.SelectedPath;
            }

        }

        private void Event_Install(object sender, System.Windows.RoutedEventArgs e)
        {
            if(!string.IsNullOrEmpty(installLoc.Text))
            {
                try
                {
                    if(!Directory.Exists(installLoc.Text))
                    {
                        Directory.CreateDirectory(installLoc.Text);
                    }

                    var lastestFile = GetLastestVersionManger();


                    List<string> filesToDownload = btnInstall.Content.ToString() == "Install" ? GetAllFileToDownload(lastestFile) : CompareFilesVersion(null, null);

                    DownloadFileFromGithub(filesToDownload, installLoc.Text);

                    if(btnInstall.Content.ToString() == "Install")
                    {
                        GetDefaultMod();
                    }
                }
                catch { }
            }

        }

        private void GetDefaultMod()
        {
            var modsFolder = Path.Combine(installLoc.Text, "Mods");

            //HttpWebRequest req = (HttpWebRequest)WebRequest.Create()
        }

        private void DownloadFileFromGithub(List<string> filesToDownload, string installLoc)
        {
            foreach (var item in filesToDownload)
            {
                string url = $"https://raw.githubusercontent.com/quangaming2929/UADInstaller/master/UADVersions/{ReleasesChangelog[0].tag_name}/{item}";
                var req = (HttpWebRequest)WebRequest.Create(url);
                req.UserAgent = UserAgent;
                using (var resp = req.GetResponse())
                {
                    using (var stream = resp.GetResponseStream())
                    {
                        var loc = Path.Combine(installLoc, item);
                        if (File.Exists(installLoc))
                            File.Delete(installLoc);

                        using (var fs = File.Create(loc))
                        {
                            fs.Position = 0;
                            stream.CopyTo(fs);
                        }
                    }
                }
            }
        }

        private List<string> CompareFilesVersion(VersionManager oldVersion, VersionManager newVersion)
        {
            return null;
        }

        private List<string> GetAllFileToDownload(VersionManager manager)
        {
            var filesName = new List<string>();
            foreach (var item in manager.FileVersion)
                filesName.Add(item.FileName);

            return filesName;
        }

        private VersionManager GetLastestVersionManger()
        {
            string url = $"https://raw.githubusercontent.com/quangaming2929/UADInstaller/master/UADVersions/{ReleasesChangelog[0].tag_name}/VersionManager.json";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.UserAgent = UserAgent;
            using (var resp = req.GetResponse())
            {
                using (var stream = resp.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream);
                    string content = reader.ReadToEnd();
                    return JsonConvert.DeserializeObject<VersionManager>(content);
                }
            }
        }
    }

    public class Settings
    {
        public string UADInstallLocation { get; set; }
    }
}
