using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public string BaseFolderPath
        {
            get => baseFolderPath; set
            {
                baseFolderPath = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BaseFolderPath"));
            }
        }
        private string _modFolderPath;
        private string _loaderFolderPath;
        private string _unModFolderPath;
        private string _unLoaderFolderPath;
        private string _settingsConfigPath;
        private string baseFolderPath;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();
            PathTextBox.DataContext = this;
            _settingsConfigPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\mod-unmod\";
            if (!Directory.Exists(_settingsConfigPath))
            {
                Directory.CreateDirectory(_settingsConfigPath);
            }
            if (File.Exists($"{_settingsConfigPath}config.json"))
            {
                GetSettings();
            }
            else
            {
                MessageBox.Show("No path found, browse to satisfactory exe", "No Path Found");
                GetPath();
            }
            _modFolderPath = $@"{BaseFolderPath}\mods";
            _loaderFolderPath = $@"{BaseFolderPath}\loaders";
            _unModFolderPath = $@"{BaseFolderPath}\mods-suspend";
            _unLoaderFolderPath = $@"{BaseFolderPath}\loaders-suspend";

        }

        private void GetSettings()
        {
            var settingsString = File.ReadAllText($"{_settingsConfigPath}config.json");
            var mySettings = JsonSerializer.Deserialize<Settings>(settingsString);
            if (mySettings.BaseFolderPath == "null" || mySettings.BaseFolderPath == null)
            {
                GetPath();
            }
            BaseFolderPath = mySettings.BaseFolderPath;
        }

        private void ModButtonOnClick(object sender, RoutedEventArgs e)
        {
            MoveFolders(_modFolderPath, _unModFolderPath);
            MoveFolders(_loaderFolderPath, _unLoaderFolderPath);
        }

        private void UnModButtonOnClick(object sender, RoutedEventArgs e)
        {
            MoveFolders(_unModFolderPath, _modFolderPath);
            MoveFolders(_unLoaderFolderPath, _loaderFolderPath);
        }

        private void MoveFolders(string oldPath, string newPath)
        {
            if (Directory.Exists(BaseFolderPath))
            {
                if (Directory.Exists(oldPath) && Directory.GetFiles(oldPath).Length < 1)
                {
                    Directory.Delete(oldPath);
                    Directory.Move(newPath, oldPath);
                }
                else if (!Directory.Exists(oldPath))
                {
                    Directory.Move(newPath, oldPath);
                }
            }
        }

        private void GetPath()
        {
            var dialog = new OpenFileDialog
            {
                DefaultExt = ".exe"
            };
            var settings = new Settings();
            if (dialog.ShowDialog() ?? false)
            {
                BaseFolderPath = System.IO.Path.GetDirectoryName(dialog.FileName);
                settings.BaseFolderPath = BaseFolderPath;
            }
            File.WriteAllText($"{_settingsConfigPath}config.json", JsonSerializer.Serialize(settings));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GetPath();
        }
    }
}
