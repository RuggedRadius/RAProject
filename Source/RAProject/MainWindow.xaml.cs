using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RAProject.Connection;
using RAProject.Models;
using RAProject.Utilities;
using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RAProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public FileInfo currentDataFile;

        public MainWindow()
        {
            // Load data or initialise new data
            if (!StoredData.FileHandling.LoadData())
            {
                StoredData.myData = new DataFile();
            }
            
            InitializeComponent();
        }

        // General Methods
        private void exitApplication()
        {
            Environment.Exit(0);
        }
        public void status(string message)
        {
            Console.WriteLine(message);

            if (lblStatus == null)
            {
                lblStatus.Content = message;
            }            
        }

        // Visual styles
        private void tglVisualStyles_Checked(object sender, RoutedEventArgs e)
        {
            // Text mode
            Panel.SetZIndex(dgConsoles, 0);
            Panel.SetZIndex(wrpConsoles, -1);
            Console.WriteLine("Text mode set.");
        }
        private void tglVisualStyles_Unchecked(object sender, RoutedEventArgs e)
        {
            // Visual mode
            Panel.SetZIndex(dgConsoles, -1);
            Panel.SetZIndex(wrpConsoles, 0);
            Console.WriteLine("Visual mode set.");
        }


        // DEBUG ONLY
        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            // TEST
            Console.WriteLine("Consoles:");

            foreach (SupportedConsole sc in StoredData.myData.consoles)
            {
                Console.WriteLine(sc.Name);
            }
        }


        // Menu
        private void miLaunchHelp_Click(object sender, RoutedEventArgs e)
        {
            string path = Environment.CurrentDirectory;
            path = path.Substring(0, path.Length - 9);
            path += "\\Help\\index.html";
            Console.WriteLine(path);
            System.Diagnostics.Process.Start(path);
        }
        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            exitApplication();
        }


        // Tab Control
        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (tabControl.SelectedIndex)
            {
                case 0:
                    // User Profile
                    displayTab_UserProfile();
                    break;

                case 1:
                    // Consoles
                    displayTab_Consoles();
                    break;

                case 2:
                    // Games
                    displayTab_Games();
                    break;

                case 3:
                    // Achievements
                    displayTab_Achievements();
                    break;

                case 4:
                    // Leader Board
                    displayTab_LeaderBoard();
                    break;

                case 5:
                    // Settings
                    displayTab_Settings();
                    break;

                case 6:
                    // Help
                    displayTab_Help();
                    break;

                default:
                    Console.WriteLine("No tab selected");
                    break;
            }
        }

        #region Console Tab
        private void displayTab_Consoles()
        {
            Console.WriteLine("Console tab selected.");
            if (tglVisualStyles.IsChecked)
            {
                // Text mode
                // Populate datagrid...
                foreach (SupportedConsole console in StoredData.myData.consoles)
                {
                    dgConsoles.Items.Add(console);
                }
            }
            else
            {
                // Visual mode
                // Populate wrap panel...
            }

        }
        #endregion



        private void displayTab_Games() { 

        }
        private void displayTab_Achievements() { 

        }
        private void displayTab_LeaderBoard() { 

        }

        #region User Tab
        private void displayTab_UserProfile() 
        {
            Console.WriteLine("User Profile tab selected.");

            if (StoredData.myData.currentUser == null)
            {
                StoredData.myData.currentUser = new User();
            }

            // Populate fields with user data
            populateUserDetails(StoredData.myData.currentUser);
            populateRecentlyPlayedGames(StoredData.myData.currentUser);
        }

        private void populateUserDetails(User user)
        {
            if (user.user == null)
            {
                Console.WriteLine("No user found in myData.");
                user.fetchUserData();
            }

            lblUsername.Content = user.user;
            //imgUserAvatar = user.userAvatar;
        }
        private void populateRecentlyPlayedGames(User user)
        {
            // Fetch list of user's recently played games
            if (user.RecentlyPlayedGames == null)
            {
                user.getRecentGames();
            }

            // For each game in list
            foreach (Game game in user.RecentlyPlayedGames)
            {
                // Download game's Box Art, if necessary
                if (game.imgBoxArt == null)
                {
                    game.downloadImage_BoxArt();
                }


                //Create image object for BoxArt

                System.Windows.Controls.Image img = ConvertDrawingImageToWPFImage(game.imgBoxArt);                

                //Size object
                img.Height = 150;
                img.Width = 150;
                img.Tag = game.ID;

                // Add click event handler
                img.AddHandler(MouseDownEvent, new RoutedEventHandler(RecentlyPlayedGame_Click));

                // Add object to wrap panel
                wrpRecentlyPlayed.Children.Add(img);
            }
        }

        private void RecentlyPlayedGame_Click(object sender, RoutedEventArgs e)
        {
            // Game BoxArt clicked
            System.Windows.Controls.Image img = (System.Windows.Controls.Image) sender;
            string gameID = img.Tag.ToString();

            Console.WriteLine("Game ID {0} clicked.", gameID);
        }


        private System.Windows.Controls.Image ConvertDrawingImageToWPFImage(System.Drawing.Image gdiImg)
        {


            System.Windows.Controls.Image img = new System.Windows.Controls.Image();

            //convert System.Drawing.Image to WPF image
            Bitmap bmp = new Bitmap(gdiImg);
            IntPtr hBitmap = bmp.GetHbitmap();
            ImageSource WpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            img.Source = WpfBitmap;
            img.Width = 500;
            img.Height = 600;
            img.Stretch = Stretch.Fill;
            return img;
        }
        #endregion

        #region Console Tab
        public void populateConsoleDataGrid()
        {

        }
        #endregion

        private void displayTab_Settings() { 

        }
        private void displayTab_Help()
        {

        }


        // Control Event Handlers
        private void miDownloadConsoles_Click(object sender, RoutedEventArgs e)
        {
            StoredData.DownloadConsoles();
        }

        private void miSaveFile_Click(object sender, RoutedEventArgs e)
        {
            StoredData.FileHandling.SaveData();
            status("Data saved to file.");
        }
        private void miLoadData_Click(object sender, RoutedEventArgs e)
        {
            StoredData.FileHandling.LoadData(); 
            status("Data saved to file.");
        }
    }
}
