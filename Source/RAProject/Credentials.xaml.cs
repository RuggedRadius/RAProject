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
using System.Windows.Shapes;

namespace RAProject
{
    /// <summary>
    /// Interaction logic for Credentials.xaml
    /// </summary>
    public partial class Credentials : Window
    {
        public Credentials(int count)
        {
            InitializeComponent();

            if (count > 1)
            {
                this.Height += 50;

                Label errorMessage = new Label();
                errorMessage.Content = "No response from the server. Check internet connection, firewall settings and credentials.";
                errorMessage.Foreground = new SolidColorBrush(Color.FromRgb(255, 69, 69));
                errorMessage.HorizontalContentAlignment = HorizontalAlignment.Center;
                mainStack.Children.Add(errorMessage);
            }
        }

        private void btnConfirmCredentials_Click(object sender, RoutedEventArgs e)
        {
            // Update credentials
            MainWindow.UpdateCredentials(txtInputUsername.Text, txtInputAPIKey.Password);

            // Close window
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void txtInputUsername_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnConfirmCredentials_Click(sender, e);
            }
        }
    }
}
