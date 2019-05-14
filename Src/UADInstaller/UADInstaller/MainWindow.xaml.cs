using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UADInstaller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow, INotifyPropertyChanged
    {
        public List<Product> AllProducts { get; set; } = new List<Product>();
        public ObservableCollection<Product> InstalledProduct { get; set; } = new ObservableCollection<Product>();

        public MainWindow()
        {
            DataContext = this;
            AllProducts.Add(new Product() { Name = "Universal Anime Downloader", Description = "Searching and downloading anime films easier. In addition, the program's built-in video player has a lot of unique features to enhance the user's experience.", Version = "v0.9.0", IsButtonEnable = false });
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
