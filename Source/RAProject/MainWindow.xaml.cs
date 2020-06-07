using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RAProject.Connection;
using RAProject.Models;
using RAProject.Modules;
using RAProject.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RAProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool initialised;

        public FileInfo currentDataFile;

        public MainWindow()
        {            
            InitializeComponent();
            

            

            // If connection settings are empty, 
            if (string.IsNullOrEmpty(Properties.Settings.Default.Credential_Username) ||
                string.IsNullOrEmpty(Properties.Settings.Default.Credential_APIKey))
            {
                // Start on settings page
                tabControl.SelectedIndex = 5;
                MessageBox.Show("Enter your RetroAchievements credentials here to connect.", "No credentials entered", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                // Start on user profile
                tabControl.SelectedIndex = 0;

                // Load data
                loadDataFromFile();
                updateStatus("Data loaded from local file.");

                // Populate user profle
                populateUserDetails();

                // Populate consoles
                populateConsoleDataGrid();

                // Populate leader board
                populateLeaderBoard();
            }

            initialised = true;
            updateStatus("Browser initialised");
        }

        private async void wndMain_Loaded(object sender, RoutedEventArgs e)
        {
            

            
        }

        private async void loadDataFromFile()
        {
            MyData.FileHandling.LoadData();

            // Load data
            await Task.Run(() => 
            {
                // Load data or initialise new data
                

                // Display user profile
                // SLOW FIXXX THISSSSSSSSSSSSSSSSSSSSSS!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //displayTab_UserProfile();
            });
        }

        public void updateStatus(string status)
        {
            Dispatcher.Invoke(() => {
                lblStatus.Content = status;
            });
        }


        // General Methods
        private void exitApplication()
        {
            Environment.Exit(0);
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
            Console.WriteLine("Test Button Hit");

            // Populate games data grid test
            int consoleNumber = 2;
            SupportedConsole console = MyData.myData.consoles[consoleNumber];
            Console.WriteLine(console.Name);
            if (console.games.Count < 1)
            {
                console.DownloadConsoleGames();
            }
            Task.Run(() => {
                populateGamesDataGrid(console);
            });
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
            //if (initialised)
            //{
            //    switch (tabControl.SelectedIndex)
            //    {
            //        case 0:
            //            // User Profile
            //            displayTab_UserProfile();
            //            break;

            //        case 1:
            //            // Consoles
            //            displayTab_Consoles();
            //            break;

            //        case 2:
            //            // Games
            //            displayTab_Games();
            //            break;

            //        case 3:
            //            // Achievements
            //            displayTab_Achievements();
            //            break;

            //        case 4:
            //            // Leader Board
            //            displayTab_LeaderBoard();
            //            break;

            //        case 5:
            //            // Settings
            //            displayTab_Settings();
            //            break;

            //        case 6:
            //            // Help
            //            displayTab_Help();
            //            break;

            //        default:
            //            Console.WriteLine("No tab selected");
            //            break;
            //    }
            //}
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
            cmbConsoleSelection.Items.Clear();

            // Populate list with each downloaded console
            foreach (SupportedConsole console in MyData.myData.consoles)
            {
                // Add to list box
                cmbConsoleSelection.Items.Add(console.Name);
            }
        }
        public void populateConsoleDataGrid()
        {
            dgConsoleList.Items.Clear();

            if (MyData.myData == null)
            {
                MyData.FileHandling.LoadData();
            }

            if (MyData.myData.consoles.Count == 0)
            {
                MyData.DownloadConsoles();
            }

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




        private void lstConsoles_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (cmbConsoleSelection.SelectedIndex < 0)
                return;

            // Get console from name
            string listValue = (string)cmbConsoleSelection.Items.GetItemAt(cmbConsoleSelection.SelectedIndex);
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
                string listValue = (string)cmbConsoleSelection.Items.GetItemAt(cmbConsoleSelection.SelectedIndex);
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
        private void dgConsoleList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ConsoleDataRow row = (ConsoleDataRow)dgConsoleList.SelectedItem;
            string consoleName = row.ConsoleName;

            // Search for selected console
            foreach (SupportedConsole console in MyData.myData.consoles)
            {
                if (console.Name == consoleName)
                {
                    // Populate console information panel
                    populateGamesDataGrid(console);

                    tabControl.SelectedIndex = 2;

                    // Prevent event from bubbling and re-triggering this method
                    e.Handled = true;
                    return;
                }
            }


            Console.WriteLine("Double CLICK!!!!");
        }
        private void dgConsoleList_Sorting(object sender, DataGridSortingEventArgs e)
        {
            // Implement sorting method here
            // ...
        }
        #endregion


        #region Games Tab
        private void displayTab_Games() {
            populateSelectConsoleComboBox();
        }
        private void populateGamesDataGrid(SupportedConsole console) {
            Dispatcher.Invoke(() => {
                // Clear list
                dgGames.ItemsSource = console.games;
            });
        }
        private void populateSelectConsoleComboBox()
        {
            // Clear combo box
            cmbConsoleSelection.Items.Clear();

            // Populate combo box
            foreach (SupportedConsole console in MyData.myData.consoles)
            {
                cmbConsoleSelection.Items.Add(console.Name);
            }
        }



        private async void cmbConsoleSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbConsoleSelection.SelectedIndex < 0)
                return;

            // Get console from name
            string listValue = (string)cmbConsoleSelection.Items.GetItemAt(cmbConsoleSelection.SelectedIndex);
            Console.WriteLine("{0} clicked.", listValue);

            // Search for selected console
            foreach (SupportedConsole console in MyData.myData.consoles)
            {
                if (console.Name == listValue)
                {
                    // Populate games list for selected console
                    await Task.Run(() => {
                        populateGamesDataGrid(console);
                    });
                    MyData.myData.currentConsole = console;
                    break;
                }
            }
        }



        #endregion

        private void displayTab_Achievements() { 

        }
        private void populateAchievementsDataGrid(Game game)
        {
            dgAchievementList.ItemsSource = game.Achievements;
        }

        #region Leader Board Tab
        private void displayTab_LeaderBoard() {
            populateLeaderBoard();
        }
        private void populateLeaderBoard()
        {
            Task.Run(() => {

                List<User> leaders = new List<User>();

                string requestURL = Requests.Users.getTop10Users();
                string json = Requests.FetchJSON(requestURL);
                dynamic data = JsonConvert.DeserializeObject(json);

                List<TopUser> topUsers = new List<TopUser>();

                for (int i = 0; i < 10; i++)
                {
                    int rank = i + 1;
                    string placeString = "place_" + (i + 1);
                    string username = data["top10"][placeString]["user"];
                    int score = data["top10"][placeString]["score"];
                    int trueratio = data["top10"][placeString]["trueratio"];

                    topUsers.Add(new TopUser(rank, username, score, trueratio));
                }

                Dispatcher.Invoke(() => {
                    dgLeaderBoard.ItemsSource = topUsers;
                });
            });
        }

        #endregion

        #region User Tab
        private async void displayTab_UserProfile() 
        {
            await Task.Run(() => {
                Dispatcher.Invoke(() =>
                {
                    if (MyData.myData == null)
                    {
                        Console.WriteLine("No myData, this shouldn't happen");
                    }

                    // Check user object exists, if not create it and fetch it's data
                    if (MyData.myData.currentUser == null)
                    {
                        Console.WriteLine("No user found in myData.");
                        MyData.myData.currentUser = new User();
                    }

                    // Populate fields with user data
                    populateUserDetails();
                    populateRecentlyPlayedGames();
                });
            });

  
        }

        private void populateUserDetails()
        {
            Console.WriteLine("Populating user details...");

            // Update status
            lblStatus.Content = "Populating user details...";

            // Update label
            lblUsername.Content = Properties.Settings.Default.Credential_Username;

            if (MyData.myData != null && MyData.myData.currentUser.userAvatar == null)
            {
                // Fetch user avatar
                MyData.myData.currentUser.fetchUserAvatar();
            }

            // Update User Profile Avatar
            imgUserAvatar.Source = new BitmapImage(new Uri(MyData.myData.currentUser.UserPic));
        }
        private void populateRecentlyPlayedGames()
        {
            Console.WriteLine("Populating recently played games");

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
            updateStatus("Data saved to file.");
        }
        private void miLoadData_Click(object sender, RoutedEventArgs e)
        {
            MyData.FileHandling.LoadData(); 
            updateStatus("Data loaded from local file.");
        }



        private async void btnSaveCredentials_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => {
                // Update app settings
                Dispatcher.Invoke(() => {
                    updateStatus("Credentials updated.");
                    Properties.Settings.Default.Credential_Username = txtSettingsUsername.Text;
                    Properties.Settings.Default.Credential_APIKey = txtSettingsAPIKey.Password;
                });


                if (MyData.myData == null)
                {
                    // Create new data file for user
                    MyData.myData = new DataFile();
                    updateStatus("New local data file created.");
                }

                if (MyData.myData.currentUser == null)
                {
                    // Create new user
                    MyData.myData.currentUser = new User();
                    updateStatus("New user created.");
                }

                Dispatcher.Invoke(() => {
                    // Store credentials in user object
                    MyData.myData.currentUser.addToCredentials(
                        txtSettingsUsername.Text,
                        Security.ComputeSha256Hash(txtSettingsAPIKey.Password)
                        );
                });


                Console.WriteLine("Displaying user profile");
                displayTab_UserProfile();            
            });

        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (cmbSearchCategory.SelectedIndex >= 0)
            {
                // Search in selected category
                string searchQuery = txtSearchQuery.Text;

                // Filter search query here
                // ...

                switch (cmbSearchCategory.SelectedIndex)
                {
                    // Consoles
                    case 0:
                        SupportedConsole searchConsole = Search.SearchConsoles(txtSearchQuery.Text);
                        if (searchConsole != null)
                        {
                            MessageBox.Show(searchConsole.Name + " released in " + searchConsole.released);
                        }
                        else
                        {
                            MessageBox.Show("No results found");

                        }
                        break;

                    // Games
                    case 1:
                        Game searchGame = Search.SearchGames(txtSearchQuery.Text);
                        if (searchGame != null)
                        {
                            MessageBox.Show(searchGame.Title + " on " + searchGame.ConsoleName);
                        }
                        else
                        {
                            MessageBox.Show("No results found");

                        }
                        break;

                    // Achievements
                    case 2:
                        Achievement searchAchievements = Search.SearchAchievements(txtSearchQuery.Text);
                        if (searchAchievements != null)
                        {
                            MessageBox.Show(searchAchievements.Title + "\n\n" + searchAchievements.Description);
                        }
                        else
                        {
                            MessageBox.Show("No results found");

                        }
                        break;

                    // WTF
                    default:
                        break;
                }

                //// Search the stored data for the specified query
                //Search.SearchData(searchQuery);
            }
            else
            {
                MessageBox.Show("Choose a search category", "No category selected", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }







        private async void downloadAllMainData()
        {
            // Download consoles
            await Task.Run(() => {
                MyData.DownloadConsoles();
            });

            await Task.Run(() =>
            {
                pbMain.Maximum = MyData.myData.consoles.Count - 1;

                var progress = new Progress<int>(value => pbMain.Value = value);

                for (int i = 0; i < MyData.myData.consoles.Count; i++)
                {
                    updateStatus(string.Format("Downloading list of {0} games... ", MyData.myData.consoles[i].Name));
                    
                    MyData.myData.consoles[i].DownloadConsoleGames();

                    ((IProgress<int>)progress).Report(i);
                }

                updateStatus("Data lists download complete.");
            });
        }
        //private void downloadConsoles()
        //{
        //    Console.WriteLine("Downloading console data...");


        //    // Fetch console list
        //    string url = Requests.Consoles.getConsoleIDs();
        //    string json = Requests.FetchJSON(url);
        //    dynamic data = JsonConvert.DeserializeObject(json);

        //    if (data["console"][0] != null)
        //    {
        //        if (MyData.myData == null)
        //        {
        //            MyData.myData = new DataFile();
        //        }

        //        // Create list of consoles
        //        MyData.myData.consoles = new List<SupportedConsole>();

        //        foreach (JObject j in data["console"][0])
        //        {
        //            // Create console object
        //            SupportedConsole sc = new SupportedConsole(j);

        //            // Add console to list
        //            MyData.myData.consoles.Add(sc);


        //            Dispatcher.Invoke(() => {
        //                lblStatus.Content = "Added console: " + sc.Name;
        //            });
        //            Console.WriteLine("Added console: " + sc.Name);
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show(
        //            "No data received for consoles, check your credentials in Settings.",
        //            "No data received",
        //            MessageBoxButton.OK,
        //            MessageBoxImage.Error
        //            );
        //    }
        //}
        











        private void miDownloadAllData_Click(object sender, RoutedEventArgs e)
        {
            downloadAllMainData();
        }

        private void dgGames_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void dgGames_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Console.WriteLine("Double clicked game");



            // Get selected game
            Game selectedGame = (Game) dgGames.SelectedItem;
            MyData.myData.currentGame = selectedGame;

            if (selectedGame.Achievements.Count == 0)
            {
                selectedGame.DownloadAchievements();
            }

            // Populate achievement data grid
            populateAchievementsDataGrid(selectedGame);

            // Switch selected tab to achievements
            tabControl.SelectedIndex = 3;

            // Prevent event from bubbling and re-triggering this method
            e.Handled = true;
        }

        private void dgGames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine("Mouse clicked game");
            int selIndex = dgGames.SelectedIndex;
            Game game = (Game)dgGames.SelectedItem;

            if (game != null)
            {
                lblGameTitle.Content = game.Title;
                MyData.myData.currentGame = game;
            }
        }

        private void btnDownloadConsoleGames_Click(object sender, RoutedEventArgs e)
        {
            // Get selected game
            ConsoleDataRow selectedConsoleRow = (ConsoleDataRow) dgConsoleList.SelectedItem;
            SupportedConsole selectedConsole = null;

            foreach (SupportedConsole console in MyData.myData.consoles)
            {
                if (console.Name == selectedConsoleRow.ConsoleName)
                {
                    selectedConsole = console;
                    break;
                }
            }

            updateStatus(string.Format("Downloading {0} games list...", selectedConsole.Name));

            selectedConsole.DownloadConsoleGames();

            updateStatus(string.Format("{0} games list downloaded.", selectedConsole.Name));

            populateConsoleDataGrid();
        }

        private void btnDownloadAchievements_Click(object sender, RoutedEventArgs e)
        {
            MyData.myData.currentGame.DownloadAchievements();
        }
    }
}
