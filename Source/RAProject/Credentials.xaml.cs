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
        public Credentials()
        {
            InitializeComponent();
        }

        private void btnConfirmCredentials_Click(object sender, RoutedEventArgs e)
        {
            // Update credentials
            MainWindow.UpdateCredentials(txtInputUsername.Text, txtInputAPIKey.Text);

            // Close window
            this.Close();
        }
    }
}
