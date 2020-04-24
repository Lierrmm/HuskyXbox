using System.Diagnostics;
using System.Windows;

namespace HuskyUI
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        private void DonateButtonClick(object sender, RoutedEventArgs e)
        {
            Process.Start("http://paypal.me/Scobalula");
        }

        private void HomePageButtonClick(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/Scobalula/Husky/");
        }
    }
}
