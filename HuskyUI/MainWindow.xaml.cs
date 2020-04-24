using System;
using System.Windows;
using System.Threading;
using System.Windows.Shell;
using Husky;
using System.Reflection;

namespace HuskyUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool ThreadActive = false;

        public MainWindow()
        {
            InitializeComponent();
            Title = $"Husky - Version {Assembly.GetExecutingAssembly().GetName().Version}";
            // Initial Print
            PrintLine("Load a supported CoD Game, then click the paper plane to export loaded BSP data.");
            PrintLine("");
        }

        private void ExportClick(object sender, RoutedEventArgs e)
        {
            // Check is a thread already active
            if (ThreadActive)
                return;
            // Create new thread and export
            new Thread(delegate ()
            {
                UpdateProgressState(TaskbarItemProgressState.Indeterminate);
                ThreadActive = true;
                HuskyUtil.LoadGame(PrintLine);
                PrintLine("");
                ThreadActive = false;
                UpdateProgressState(TaskbarItemProgressState.Normal);
            }).Start();
        }

        /// <summary>
        /// Prints to the ConsoleBox
        /// </summary>
        /// <param name="value">Value to print</param>
        private void PrintLine(object value)
        {
            Dispatcher.BeginInvoke(new Action(() => ConsoleBox.AppendText(value.ToString() + Environment.NewLine)));
        }

        /// <summary>
        /// Updates Progress State
        /// </summary>
        /// <param name="state">State to set</param>
        private void UpdateProgressState(TaskbarItemProgressState state)
        {
            Dispatcher.BeginInvoke(new Action(() => TaskBarProgress.ProgressState = state));
        }

        /// <summary>
        /// Shows the About Window and Dims the Main Window
        /// </summary>
        private void AboutClick(object sender, RoutedEventArgs e)
        {
            var aboutWindow = new AboutWindow
            {
                Owner = this,
                VersionLabel = { Content = $"Version: {Assembly.GetExecutingAssembly().GetName().Version}" }
            };
            DimBox.Visibility = Visibility.Visible;
            aboutWindow.ShowDialog();
            DimBox.Visibility = Visibility.Hidden;
        }
    }
}
