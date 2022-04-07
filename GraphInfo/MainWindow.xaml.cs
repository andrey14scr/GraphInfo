using Microsoft.Win32;

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace GraphInfo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly FileInfoService _fileInfoService = new();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadFile_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = true, 
            };

            if (openFileDialog.ShowDialog() == true)
            {
                LoadFiles(openFileDialog.FileNames);
            }
        }

        private void LoadFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(dialog.SelectedPath))
                {
                    var files = Directory.GetFiles(dialog.SelectedPath);
                    LoadFiles(files);
                }
            }
        }

        private void LoadFiles(IEnumerable<string> files)
        {
            var list = new ConcurrentBag<FileInfoViewModel>();
            var tasks = new List<Task>();

            foreach (var fileName in files)
            {
                var task = new Task(() =>
                {
                    var info = _fileInfoService.GetInfo(fileName).ToFileInfoViewModel();
                    list.Add(info);
                });
                tasks.Add(task);
                task.Start();
            }

            Task.WaitAll(tasks.ToArray());

            var res = list.ToList();
            if (FilesList.ItemsSource != null && FilesList.Items.Count != 0)
            {
                res.AddRange((IEnumerable<FileInfoViewModel>)FilesList.ItemsSource);
            }

            res.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.Ordinal));
            FilesList.ItemsSource = res;
        }
    }
}
