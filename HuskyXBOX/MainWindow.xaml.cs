using System;
using System.Windows;
using System.Threading;
using System.Windows.Shell;
using Husky;
using System.Reflection;

namespace HuskyUIXBOX
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
    }
}
