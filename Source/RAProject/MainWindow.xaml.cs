using RAProject.Models;
using RAProject.Modules;
using System;
using System.Collections;
using System.Data;
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
            if (!MyData.FileHandling.LoadData())
            {
                MyData.myData = new DataFile();
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
            //Panel.SetZIndex(dgConsoles, 0);
            //Panel.SetZIndex(wrpConsoles, -1);
            Console.WriteLine("Text mode set.");
        }
        private void tglVisualStyles_Unchecked(object sender, RoutedEventArgs e)
        {
            // Visual mode
            //Panel.SetZIndex(dgConsoles, -1);
            //Panel.SetZIndex(wrpConsoles, 0);
            Console.WriteLine("Visual mode set.");
        }


        // DEBUG ONLY
        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            // TEST
            Console.WriteLine("Consoles:");

            foreach (SupportedConsole sc in MyData.myData.consoles)
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
            populateConsoleList();
            populateConsoleDataGrid();
        }
        private void populateConsoleList()
        {
            // Clear list
            lstConsoles.Items.Clear();

            // Populate list with each downloaded console
            foreach (SupportedConsole console in MyData.myData.consoles)
            {
                // Add to list box
                lstConsoles.Items.Add(console.Name);
            }
        }
        public void populateConsoleDataGrid()
        {
            dgConsoleList.Items.Clear();

            // Populate list with each downloaded console
            foreach (SupportedConsole console in MyData.myData.consoles)
            {
                // Add to datagrid
                dgConsoleList.Items.Add(new ConsoleDataRow(console.Name, console.released, console.games.Count));
            }

        }
        private void populateConsoleInfo (SupportedConsole console)
        {
            //// Set image
            System.Drawing.Image consoleImage = ConsoleInformation.getConsoleImage(console);

            if (consoleImage == null)
            {
                Console.WriteLine("Image not found...");
            }
            else
            {
                Bitmap bmp = new Bitmap(consoleImage);
                IntPtr hBitmap = bmp.GetHbitmap();
                ImageSource WpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

                imgConsole.Source = WpfBitmap;

                // Set title
                lblConsoleName.Content = console.Name;

                // Set details
            }
        }

        

        private void lstConsoles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstConsoles.SelectedIndex < 0)
                return;

            // Get console from name
            string listValue = (string)lstConsoles.Items.GetItemAt(lstConsoles.SelectedIndex);
            Console.WriteLine("{0} clicked.", listValue);

            // Search for selected console
            foreach (SupportedConsole console in MyData.myData.consoles)
            {
                if (console.Name == listValue)
                {
                    // Populate console information panel
                    populateConsoleInfo(console);
                }
            }
        }
        private void lstConsoles_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (lstConsoles.SelectedIndex < 0)
                return;

            // Get console from name
            string listValue = (string)lstConsoles.Items.GetItemAt(lstConsoles.SelectedIndex);
            Console.WriteLine("{0} double-clicked.", listValue);

            // Search for selected console
            foreach (SupportedConsole console in MyData.myData.consoles)
            {
                if (console.Name == listValue)
                {
                    // Populate console information panel
                    populateConsoleInfo(console);
                }
            }

            // Select Games Tab, and populate it
            tabControl.SelectedIndex = 2;


        }
        private void lstConsoles_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Console.WriteLine("Mouse down...");
            if (e.ClickCount >= 2)
            {
                // Double click
                string listValue = (string)lstConsoles.Items.GetItemAt(lstConsoles.SelectedIndex);
                Console.WriteLine("{0} double-clicked.", listValue);

                // Search for selected console
                foreach (SupportedConsole console in MyData.myData.consoles)
                {
                    if (console.Name == listValue)
                    {
                        // Populate console information panel
                        populateConsoleInfo(console);
                    }
                }

                // Select Games Tab, and populate it
                tabControl.SelectedIndex = 2;
            }
        }

        private void dgConsoleList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ConsoleDataRow row = (ConsoleDataRow)dgConsoleList.SelectedItem;

            if (row == null)
            {
                return;
            }

            string consoleName = row.ConsoleName;

            Console.WriteLine("{0} clicked.", consoleName);

            // Search for selected console
            foreach (SupportedConsole console in MyData.myData.consoles)
            {
                if (console.Name == consoleName)
                {
                    // Populate console information panel
                    populateConsoleInfo(console);

                    // Prevent event from bubbling and re-triggering this method
                    e.Handled = true;
                    return;
                }
            }
        }
        private void dgConsoleList_Sorting(object sender, DataGridSortingEventArgs e)
        {
            // Implement sorting method here
            // ...
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
            // Check user object exists, if not create it and fetch it's data
            if (MyData.myData.currentUser == null)
            {
                Console.WriteLine("No user found in myData.");
                MyData.myData.currentUser = new User();
            }

            // Populate fields with user data
            populateUserDetails();
            populateRecentlyPlayedGames();
        }

        private void populateUserDetails()
        {
            // Update label
            lblUsername.Content = Properties.Settings.Default.Credential_Username;

            if (MyData.myData.currentUser.userAvatar == null)
            {
                // Fetch user avatar
                MyData.myData.currentUser.fetchUserAvatar();
            }

            // Update User Profile Avatar
            imgUserAvatar.Source = new BitmapImage(new Uri(MyData.myData.currentUser.UserPic));
        }
        private void populateRecentlyPlayedGames()
        {
            // Fetch list of user's recently played games
            if (MyData.myData.currentUser.RecentlyPlayedGames == null)
            {
                MyData.myData.currentUser.getRecentGames();
            }

            // Clear wrap panel
            wrpRecentlyPlayed.Children.Clear();

            // For each game in list
            foreach (Game game in MyData.myData.currentUser.RecentlyPlayedGames)
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



        private void displayTab_Settings() { 

        }
        private void displayTab_Help()
        {

        }


        // Control Event Handlers
        private void miDownloadConsoles_Click(object sender, RoutedEventArgs e)
        {
            MyData.DownloadConsoles();
        }

        private void miSaveFile_Click(object sender, RoutedEventArgs e)
        {
            MyData.FileHandling.SaveData();
            status("Data saved to file.");
        }
        private void miLoadData_Click(object sender, RoutedEventArgs e)
        {
            MyData.FileHandling.LoadData(); 
            status("Data saved to file.");
        }


    }
}
