using Microsoft.Win32;

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
        private List<string> _currentFiles = new List<string>();
        private List<FileInfoViewModel> _info = new List<FileInfoViewModel>();

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
            _currentFiles = files.ToList();
            LoadProgress.Maximum = _currentFiles.Count();

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += WorkerDoWork;
            worker.ProgressChanged += WorkerProgressChanged;
            worker.RunWorkerCompleted += WorkerCompleted;

            worker.RunWorkerAsync();
        }

        private void WorkerDoWork(object? sender, DoWorkEventArgs e)
        {
            _info = new List<FileInfoViewModel>();
            var progress = 0;

            foreach (var fileName in _currentFiles)
            {
                var info = _fileInfoService.GetInfo(fileName).ToFileInfoViewModel();
                _info.Add(info);
                (sender as BackgroundWorker).ReportProgress(++progress);
            }

            if (FilesList.ItemsSource != null && FilesList.Items.Count != 0)
            {
                _info.AddRange((IEnumerable<FileInfoViewModel>)FilesList.ItemsSource);
            }

            _info.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.Ordinal));
        }

        private void WorkerProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            LoadProgress.Value = e.ProgressPercentage;
        }

        private void WorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            FilesList.ItemsSource = _info;
            System.Windows.MessageBox.Show("Loading completed!", "Info");
            LoadProgress.Value = 0;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _info = new List<FileInfoViewModel>();
            FilesList.ItemsSource = _info;
        }
    }
}
