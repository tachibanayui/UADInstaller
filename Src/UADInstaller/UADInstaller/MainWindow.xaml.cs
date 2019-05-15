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
        public Release[] ReleasesChangelog { get; set; }

        public const string Version = "v1.0";

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();

            GetInfomation();
            GetChangeLog();
        }

        private async void GetChangeLog()
        {
            var infoReq = (HttpWebRequest)WebRequest.Create("https://api.github.com/repos/quangaming2929/UniversalAnimeDownloader/releases");
            infoReq.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.119 Safari/537.36";
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
        }

        private async void GetInfomation()
        {
            var infoReq = (HttpWebRequest)WebRequest.Create("https://raw.githubusercontent.com/quangaming2929/UniversalAnimeDownloader/master/README.md");
            infoReq.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.119 Safari/537.36";
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
    }

    public class Settings
    {
        public string UADInstallLocation { get; set; }
    }
}
