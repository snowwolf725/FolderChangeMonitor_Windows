using System;
using System.Collections.Generic;
using System.Linq;
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
using System.IO;
using System.Security.Permissions;
using System.Collections.ObjectModel;

namespace FileSystemMonitor
{
    public class Item
    {
        public string Path { get; set; }
        public string Status { get; set; }
    }

    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        

        public ObservableCollection<Item> Items = new ObservableCollection<Item>();

        public MainWindow()
        {
            InitializeComponent();
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public void StartMonitor(string _path)
        {
            // Create a new FileSystemWatcher and set its properties.
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = _path;
            /* Watch for changes in LastAccess and LastWrite times, and
               the renaming of files or directories. */
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
               | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            // Only watch text files.
            watcher.Filter = "*";

            // Add event handlers.
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.Deleted += new FileSystemEventHandler(OnChanged);
            watcher.Renamed += new RenamedEventHandler(OnRenamed);

            // Begin watching.
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;
        }

        // Define the event handlers. 
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted.
            Console.WriteLine("File: " + e.FullPath + " " + e.ChangeType);
            Dispatcher.Invoke((Action)(() =>
            {
                Items.Add(new Item() { Path = e.FullPath, Status = e.ChangeType.ToString() });
                //grid_changeList.Items.Add(new Item() { Path = e.FullPath, Status = e.ChangeType.ToString() });
            }));
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            // Specify what is done when a file is renamed.
            Console.WriteLine("File: {0} renamed to {1}", e.OldFullPath, e.FullPath);
            Dispatcher.Invoke((Action)(() =>
            {
                Items.Add(new Item() { Path = e.OldFullPath, Status = "Rename From" });
                Items.Add(new Item() { Path = e.FullPath, Status = "Rename To" });
                //grid_changeList.Items.Add(new Item() { Path = e.OldFullPath, Status = "Rename From" });
                //grid_changeList.Items.Add(new Item() { Path = e.FullPath, Status = "Rename To" });
            }));
            
        }

        private void btn_monitor_Click(object sender, RoutedEventArgs e)
        {
            grid_changeList.ItemsSource = Items;
            StartMonitor(txt_path.Text);
        }

        private void btn_chooseDir_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            txt_path.Text = dialog.SelectedPath;
        }
    }
}
