using MahApps.Metro.Controls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using UADInstaller.Jsons;

using ErrorEventArgs = Newtonsoft.Json.Serialization.ErrorEventArgs;
using System.Threading.Tasks;

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

        private void Event_GetFolder(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if(dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                installLoc.Text = dialog.SelectedPath;
            }

        }

        private async void Event_Install(object sender, RoutedEventArgs e)
        {
            string installLocal = installLoc.Text;
            string btnContent = btnInstall.Content.ToString();
            if (!string.IsNullOrEmpty(installLocal))
            {
                try
                {
                    await Task.Run(() => 
                    {
                        Dispatcher.Invoke(() =>
                        {
                            rpProgress.Visibility = Visibility.Visible;
                            pgStatus.Value = 0;
                            btnInstall.IsEnabled = false;
                            txblStatus.Text = "Getting infomation...";
                            pgStatus.IsIndeterminate = true;

                            if (!Directory.Exists(installLocal))
                            {
                                Directory.CreateDirectory(installLocal);
                            }
                        });

                        var lastestFile = GetLastestVersionManger();

                        Dispatcher.Invoke(() => txblStatus.Text = "Checking files...");

                        List<string> filesToDownload = btnContent == "Install" ? GetAllFileToDownload(lastestFile) : CompareFilesVersion(null, null);
                        filesToDownload.Add("VersionManager.json");

                        DownloadFileFromGithub(filesToDownload, installLocal);

                        if (btnContent == "Install")
                        {
                            Dispatcher.Invoke(() => pgStatus.Value = 0.95);
                            GetDefaultMod();
                            CreateShortCut();
                        }

                        Dispatcher.Invoke(() =>
                        {
                            txblStatus.Text = "Done!";
                            pgStatus.Value = 1;
                        });

                        File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UADInstallationLocation.txt"), Path.Combine(installLocal, "VersionManager.json"));
                    });

                    await Task.Delay(5000);
                    rpProgress.Visibility = Visibility.Collapsed;
                    btnInstall.IsEnabled = false;
                    btnInstall.Content = "Update";
                    btnLaunch.IsEnabled = true;
                    btnUnistall.IsEnabled = true;
                    btnLocate.IsEnabled = false;
                }
                catch { }
            }

        }

        private void CreateShortCut()
        {
            //Desktop shortcut
            IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
            string shortcutAddress = (string)shell.SpecialFolders.Item("Desktop") + @"\Universal Anime Downloader.lnk";
            IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(shortcutAddress);
            shortcut.Description = "Universal Anime Downloader (UAD) is a anime downloader/extractor tool to get and watch anime with minimal effort to search and download. This program also a video player with unique feature help you watch anime easier ";
            string isnLoc = string.Empty;
            Dispatcher.Invoke(() => installLoc.Text);
            shortcut.TargetPath = Path.Combine(isnLoc, "UniversalAnimeDownloader.exe");
            //shortcut.IconLocation = 
            shortcut.Save();

            //StartMenu shortcut
            object shStartMenu = (object)"Desktop";
            IWshRuntimeLibrary.WshShell shell2 = new IWshRuntimeLibrary.WshShell();
            string shortcutAddress2 = (string)shell2.SpecialFolders.Item("Programs") + @"\Universal Anime Downloader.lnk";
            IWshRuntimeLibrary.IWshShortcut shortcut2 = (IWshRuntimeLibrary.IWshShortcut)shell2.CreateShortcut(shortcutAddress2);
            shortcut2.Description = "Universal Anime Downloader (UAD) is a anime downloader/extractor tool to get and watch anime with minimal effort to search and download. This program also a video player with unique feature help you watch anime easier ";
            shortcut2.TargetPath = Path.Combine(isnLoc, "UniversalAnimeDownloader.exe");
            //shortcut2.IconLocation = 
            shortcut2.Save();
        }

        private void GetDefaultMod()
        {
            string installLocal = string.Empty;
            Dispatcher.Invoke(() => 
            {
                txblStatus.Text = "Downloading default mod...";
                installLocal = installLoc.Text;
            });
            var modsFolder = Path.Combine(installLocal, "Mods");
            if (!Directory.Exists(modsFolder))
                Directory.CreateDirectory(modsFolder);

            var req = (HttpWebRequest)WebRequest.Create("https://github.com/quangaming2929/UADInstaller/raw/master/UADVersions/Default%20Assets/QGExtractor.dll");
            req.UserAgent = UserAgent;
            using (var resp = req.GetResponse())
            {
                using (var stream = resp.GetResponseStream())
                {
                    using (var fs = File.Create(Path.Combine(modsFolder, "QGExtractor.dll")))
                    {
                        fs.Position = 0;
                        stream.CopyTo(fs);
                    }
                }
            }

            var req2 = (HttpWebRequest)WebRequest.Create("https://github.com/quangaming2929/UADInstaller/raw/master/UADVersions/Default%20Assets/HtmlAgilityPack.dll");
            req2.UserAgent = UserAgent;
            using (var resp2 = req2.GetResponse())
            {
                using (var stream2 = resp2.GetResponseStream())
                {
                    using (var fs2 = File.Create(Path.Combine(modsFolder, "HtmlAgilityPack.dll")))
                    {
                        fs2.Position = 0;
                        stream2.CopyTo(fs2);
                    }
                }
            }

        }

        private void DownloadFileFromGithub(List<string> filesToDownload, string installLoc)
        {
            int currentFile = 1;
            foreach (var item in filesToDownload)
            {
                Dispatcher.Invoke(() =>
                {
                    txblStatus.Text = $"Downloading {item} ({currentFile}/{filesToDownload.Count})...";
                    pgStatus.IsIndeterminate = false;
                    pgStatus.Value = currentFile / (double)filesToDownload.Count * 0.9d;
                });
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
                            currentFile++;
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
