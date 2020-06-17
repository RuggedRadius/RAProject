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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;

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
                MyData.FileHandling.LoadData();
                updateStatus("Data loaded from local file.");

                // Populate user profle
                populateUserDetails();

                // Populate consoles
                populateConsoleDataGrid();

                // Populate leader board
                populateLeaderBoard();
            }

            applyTheme();

     

            initialised = true;
            updateStatus("Browser initialised");
        }

        // General Methods
        private void exitApplication()
        {
            Environment.Exit(0);
        }
        public void updateStatus(string status)
        {
            Dispatcher.Invoke(() => {
                lblStatus.Content = status;
            });
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

        // Tab Control
        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {


            foreach (TabItem item in tabControl.Items)
            {
                if (item == tabControl.SelectedItem)
                {
                    item.Background = new SolidColorBrush(Theme.secondaryColour);
                }
                else
                {
                    item.Background = new SolidColorBrush(Theme.primaryColour);
                }
            }

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
                populateGamesTab(console);
            });
        }


        #region Menu
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
        private void miDownloadConsoles_Click(object sender, RoutedEventArgs e)
        {
            MyData.DownloadConsoles();
        }
        private void miDownloadAllData_Click(object sender, RoutedEventArgs e)
        {
            downloadAllMainData();
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
        #endregion        

        #region Console Tab
        private void displayTab_Consoles()
        {            
            //populateConsoleList();
            populateConsoleDataGrid();
        }
        //private void populateConsoleList()
        //{
        //    // Clear list
        //    cmbConsoleSelection.Items.Clear();

        //    // Populate list with each downloaded console
        //    foreach (SupportedConsole console in MyData.myData.consoles)
        //    {
        //        // Add to list box
        //        cmbConsoleSelection.Items.Add(console.Name);
        //    }
        //}
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
            Dispatcher.Invoke(() => {
                // Set image
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
                    lblConsoleGamesCount.Content = console.games.Count.ToString();
                }
            });

        }
        private void btnDownloadConsoleGames_Click(object sender, RoutedEventArgs e)
        {
            // Get selected game
            ConsoleDataRow selectedConsoleRow = (ConsoleDataRow)dgConsoleList.SelectedItem;
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




        //private void lstConsoles_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    if (cmbConsoleSelection.SelectedIndex < 0)
        //        return;

        //    // Get console from name
        //    string listValue = (string)cmbConsoleSelection.Items.GetItemAt(cmbConsoleSelection.SelectedIndex);
        //    Console.WriteLine("{0} double-clicked.", listValue);

        //    // Search for selected console
        //    foreach (SupportedConsole console in MyData.myData.consoles)
        //    {
        //        if (console.Name == listValue)
        //        {
        //            // Populate console information panel
        //            populateConsoleInfo(console);
        //        }
        //    }

        //    // Select Games Tab, and populate it
        //    tabControl.SelectedIndex = 2;


        //}
        //private void lstConsoles_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    Console.WriteLine("Mouse down...");
        //    if (e.ClickCount >= 2)
        //    {
        //        // Double click
        //        string listValue = (string)cmbConsoleSelection.Items.GetItemAt(cmbConsoleSelection.SelectedIndex);
        //        Console.WriteLine("{0} double-clicked.", listValue);

        //        // Search for selected console
        //        foreach (SupportedConsole console in MyData.myData.consoles)
        //        {
        //            if (console.Name == listValue)
        //            {
        //                // Populate console information panel
        //                populateConsoleInfo(console);
        //            }
        //        }

        //        // Select Games Tab, and populate it
        //        tabControl.SelectedIndex = 2;
        //    }
        //}

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
                    populateGamesTab(console);

                    tabControl.SelectedIndex = 2;

                    MyData.myData.currentConsole = console;

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
        private void displayTab_Games() 
        {
            //populateSelectConsoleComboBox();
        }
        private void populateGamesTab(SupportedConsole console)
        {
            Console.WriteLine("Populating games tab...");

            if (console.games.Count == 0)
            {
                console.DownloadConsoleGames();
            }
            // Ensure selected style is used
            //applyVisualStyles();

            if (rdoVisualStyles.IsChecked == true)
            {
                wrapGames.Visibility = Visibility.Visible;

                dgGames.Visibility = Visibility.Hidden;

                populateGamesWrapPanel(console);
            }
            else
            {
                dgGames.Visibility = Visibility.Visible;

                wrapGames.Visibility = Visibility.Hidden;

                populateGamesDataGrid(console);
            }
        }

        private void populateGamesWrapPanel(SupportedConsole console)
        {
            Console.WriteLine("Outputting visual style games...");

            Dispatcher.Invoke(() => {
                // Clear wrap panel
                wrapGames.Children.Clear();

                // Populate
                foreach (Game game in console.games)
                {
                    Task.Run(() =>
                    {
                        if (game.imgBoxArt == null)
                            game.downloadImage_BoxArt();

                        // Create image
                        Dispatcher.Invoke(() => { 
                            System.Windows.Controls.Image newImageTile = ImageConversion.ConvertDrawingImageToWPFImage(game.imgBoxArt);

                            // Size image
                            newImageTile.Width = 100;
                            newImageTile.Height = 160;
                            newImageTile.Margin = new Thickness(10);

                            // Add ID as tag
                            newImageTile.Tag = game.ID;

                            // Add click event handler
                            newImageTile.MouseDown += NewImageTile_MouseDown;

                            // Add to games wrap panel
                            wrapGames.Children.Add(newImageTile);
                        });
                    });
                }
            });
        }
        private void populateGamesDataGrid(SupportedConsole console)
        {

            Dispatcher.Invoke(() =>
            {
                // Clear list
                dgGames.ItemsSource = console.games;
            });
        }

        private void FillGameInfoPanel(Game game)
        {
            System.Drawing.Image gameArt = null;
            if (game.imgBoxArt == null)
            {
                gameArt = game.downloadImage_BoxArt();

            }
            else
            {
                gameArt = game.imgBoxArt;
            }

            // Set image
            imgGame.Source = null;

            if (gameArt == null)
            {
                Console.WriteLine("Image not found...");
            }
            else
            {
                Bitmap bmp = new Bitmap(gameArt);
                IntPtr hBitmap = bmp.GetHbitmap();
                ImageSource WpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

                imgGame.Source = WpfBitmap;
            }

            // Set details
            if (string.IsNullOrEmpty(game.Developer) ||
                string.IsNullOrEmpty(game.Released) ||
                string.IsNullOrEmpty(game.Publisher) ||
                string.IsNullOrEmpty(game.Title))
            {
                game.downloadGameData();
            }

            // Set title
            lblConsoleName.Content = game.Title;

            // Set details
            lblGameTitle.Content = game.Title;
            lblGames_Developer.Content = game.Developer;
            lblGames_Publisher.Content = game.Publisher;
            lblGames_Released.Content = game.Released;
        }

        //private void populateSelectConsoleComboBox()
        //{
        //    // Clear combo box
        //    cmbConsoleSelection.Items.Clear();

        //    // Populate combo box
        //    foreach (SupportedConsole console in MyData.myData.consoles)
        //    {
        //        cmbConsoleSelection.Items.Add(console.Name);
        //    }
        //}

        private void NewImageTile_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                Console.WriteLine("Double clicked");

                

                // Get selected game
                Game selectedGame = null;
                foreach (Game game in MyData.myData.currentConsole.games)
                {
                    if (game.ID == ((System.Windows.Controls.Image)sender).Tag.ToString())
                    {
                        selectedGame = game;
                    }
                }

                if (selectedGame != null)
                {
                    // Populate achievement data grid
                    populateAchievementsTab(selectedGame);

                    // Switch selected tab to achievements
                    tabControl.SelectedIndex = 3;

                    // Prevent event from bubbling and re-triggering this method
                    e.Handled = true;
                }
                else
                {
                    Console.WriteLine("ERROR: Could not find selected game");
                }

                return;
            }


            System.Windows.Controls.Image tile = (System.Windows.Controls.Image)sender;
            Game clickedGame = null;
            foreach (Game game in MyData.myData.currentConsole.games)
            {
                if (game.ID == tile.Tag.ToString())
                {
                    clickedGame = game;
                }
            }

            if (clickedGame != null)
            {
                MyData.myData.currentGame = clickedGame;
                Console.WriteLine(clickedGame.Title + " tile clicked");
                FillGameInfoPanel(clickedGame);
            }
            else
            {
                Console.WriteLine("ERROR: Unknown tile clicked " + tile.Tag);
            }
        }


        private void dgGames_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!dgGames.IsEnabled)
                return;

            Console.WriteLine("Double clicked game");



            // Get selected game
            Game selectedGame = (Game)dgGames.SelectedItem;
            MyData.myData.currentGame = selectedGame;

            if (selectedGame.Achievements.Count == 0)
            {
                selectedGame.DownloadAchievements();
            }

            // Populate achievement data grid
            populateAchievementsTab(selectedGame);

            // Switch selected tab to achievements
            tabControl.SelectedIndex = 3;

            // Prevent event from bubbling and re-triggering this method
            e.Handled = true;
        }
        private void dgGames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgGames.SelectedIndex < 0)
                return;

            Console.WriteLine("Selection chnged");

            // Get selected game
            Game selectedGame = (Game)dgGames.SelectedItem;
            MyData.myData.currentGame = selectedGame;

            // Display game info
            FillGameInfoPanel(selectedGame);

            Console.WriteLine("Mouse clicked game");
        }

        //private async void cmbConsoleSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (cmbConsoleSelection.SelectedIndex < 0)
        //        return;

        //    // Get console from name
        //    string listValue = (string)cmbConsoleSelection.Items.GetItemAt(cmbConsoleSelection.SelectedIndex);
        //    Console.WriteLine("{0} clicked.", listValue);

        //    // Search for selected console
        //    foreach (SupportedConsole console in MyData.myData.consoles)
        //    {
        //        if (console.Name == listValue)
        //        {
        //            // Populate games list for selected console
        //            await Task.Run(() => {
        //                populateGamesTab(console);
        //            });
        //            MyData.myData.currentConsole = console;
        //            break;
        //        }
        //    }
        //}

        private void btnDownloadAchievements_Click(object sender, RoutedEventArgs e)
        {
            if (MyData.myData.currentGame != null)
            {
                MyData.myData.currentGame.DownloadAchievements();
                MyData.myData.currentGame.downloadImage_BoxArt();

                dgGames.SelectedIndex--;
                dgGames.SelectedIndex++;
            }
            else
            {
                MessageBox.Show(
                    "No current game selected.",
                    "No current game",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                    );
            }
        }


        #endregion

        #region Achievements Tab
        //private void displayTab_Achievements() { 

        //}
        private void populateAchievementsTab(Game game)
        {
            if (rdoVisualStyles.IsChecked == true)
            {
                dgAchievementList.Visibility = Visibility.Hidden;
                populateAchievementsWrapPanel(game);
            }
            else
            {
                wrapAchievements.Visibility = Visibility.Hidden;
                populateAchievementsDataGrid(game);
            }
        }
        private void populateAchievementsDataGrid(Game game)
        {
            if (game.Achievements.Count == 0)
                game.DownloadAchievements();

            Dispatcher.Invoke(() =>
            {
                dgAchievementList.ItemsSource = game.Achievements;
            });
        }
        private void populateAchievementsWrapPanel(Game game)
        {
            Console.WriteLine("Outputting visual style games...");

            if (game.Achievements.Count == 0)
                game.DownloadAchievements();

            Dispatcher.Invoke(() => 
            {
                // Clear wrap panel
                wrapAchievements.Children.Clear();

                // Populate
                foreach (Achievement achievement in game.Achievements)
                {
                    Task.Run(() => {                     
                        // Create UI element
                        var newImageTile = ImageConversion.ConvertDrawingImageToWPFImage(achievement.getBadge());

                        if (achievement.badge != null)
                        {
                            Dispatcher.Invoke(() => 
                            {
                                // Size image
                                int tileSize = 100;
                                newImageTile.Width = tileSize;
                                newImageTile.Height = tileSize;
                                newImageTile.Margin = new Thickness(10);

                                // Add ID as tag
                                newImageTile.Tag = achievement.ID;

                                // Add click event handler
                                newImageTile.MouseDown += AchievementTile_MouseDown;

                                // Add to games wrap panel
                                wrapAchievements.Children.Add(newImageTile);
                            });
                        }
                        else
                        {
                            Console.WriteLine("No achievement Badge for " + achievement.Title);
                        }
                    });
                }
            });
        }
        
        private void FillAchievementDetails(Achievement achievement)
        {
            // Set badge image
            imgSelectedAchievementBadge.Source = new BitmapImage(new Uri(achievement.GetBadgeUrl()));

            // Title
            lblSelectedAchievementTitle.Content = achievement.Title;

            // Description
            lblAchievementDescription.Content = achievement.Description;

            // Dates
            lblDateEarned.Content = achievement.DateEarned;
            lblDateHardcore.Content = achievement.DateEarnedHardCore;
        }
        private void dgAchievementList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Achievement seletedAchievement = (Achievement)dgAchievementList.SelectedItem;

            lblSelectedAchievementTitle.Content = seletedAchievement.Title;

            lblAchievementDescription.Content = seletedAchievement.Description;

            imgSelectedAchievementBadge.Source = new BitmapImage(new Uri(seletedAchievement.GetBadgeUrl())); // seletedAchievement.badge;
        }
        private void AchievementTile_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Get achievement game
            Achievement selectedAchievement = null;
            foreach (Achievement achievement in MyData.myData.currentGame.Achievements)
            {
                if (achievement.ID == ((System.Windows.Controls.Image)sender).Tag.ToString())
                {
                    selectedAchievement = achievement;
                }
            }

            if (selectedAchievement != null)
            {
                // Populate achievement data grid
                FillAchievementDetails(selectedAchievement);

                // Prevent event from bubbling and re-triggering this method
                e.Handled = true;
            }
            else
            {
                Console.WriteLine("ERROR: Could not find selected achievement");
            }

            return;
        }

        #endregion

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

        #region Settings Tab
        private void displayTab_Settings() { 

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
        private void cpPrimaryColour_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            Theme.primaryColour = (Color)cpPrimaryColour.SelectedColor;
            applyTheme();

            Style currentStyle = (Style)Application.Current.Resources["tabStylings"];
            Style newStyle = new Style(typeof(TabItem), currentStyle);

            newStyle.Setters.Add(new Setter(BackgroundProperty, Theme.primaryColour));
            newStyle.Setters.Add(new Setter(ForegroundProperty, Theme.secondaryColour));

            //Application.Current.Resources["tabStylings"] = newStyle;
        }
        private void cpSecondaryColour_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Theme.secondaryColour = (Color)cpSecondaryColour.SelectedColor;
            applyTheme();
        }
        #endregion

        #region Help Tab
        private void displayTab_Help()
        {

        }
        #endregion

        #region Search
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
                            // Switch to console tab
                            tabControl.SelectedIndex = 1;

                            dgConsoleList.Focus();

                            // Highlight search result
                            for (int i = 0; i < dgConsoleList.Items.Count; i++)
                            {
                                if (((ConsoleDataRow)dgConsoleList.Items[i]).ConsoleName == searchConsole.Name)
                                {
                                    dgConsoleList.SelectedIndex = i;
                                    break;
                                }
                            }

                            dgConsoleList.ScrollIntoView(searchConsole);
                        }
                        else { MessageBox.Show("No results found", "No results found", MessageBoxButton.OK, MessageBoxImage.Information); }
                        break;

                    // Games
                    case 1:
                        Game searchGame = Search.SearchGames(txtSearchQuery.Text);
                        MyData.myData.currentGame = searchGame;

                        SupportedConsole console = Search.SearchConsoles(searchGame.ConsoleName);
                        if (searchGame != null)
                        {
                            // Switch to console tab
                            tabControl.SelectedIndex = 2;

                            // Populate games tab
                            populateGamesTab(console);


                            if (rdoTextStyles.IsChecked == true)
                            {
                                // Focus datagrid
                                dgGames.Focus();

                                // Highlight search result
                                for (int i = 0; i < dgGames.Items.Count; i++)
                                {
                                    if (((Game)dgGames.Items[i]).Title == searchGame.Title)
                                    {
                                        dgGames.SelectedIndex = i;
                                        break;
                                    }
                                }

                                // Scroll into view
                                dgGames.ScrollIntoView(searchGame);
                            }
                            else
                            {
                                wrapGames.Focus();

                                //for (int i = 0; i < wrapGames.Children.Count; i++)
                                //{
                                //    if (((System.Windows.Controls.Image)wrapGames.Children[i]).Tag.Equals(searchGame.ID))
                                //    {
                                //        wrapGames..SelectedIndex = i;
                                //        break;
                                //    }
                                //}

                                
                            }




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
        private void txtSearchQuery_GotFocus(object sender, RoutedEventArgs e)
        {
            txtSearchQuery.Text = string.Empty;
        }
        private void txtSearchQuery_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter || e.Key == System.Windows.Input.Key.Return)
            {
                btnSearch_Click(sender, e);
            }
        }
        #endregion

        #region Visual Styles
        private void showVisualStyles()
        {
            if (!initialised)
                return;

            Console.WriteLine("Showing visual styles");

            // Hide data grids
            hideUIElement(dgGames);
            hideUIElement(dgAchievementList);

            // Show wrap panels
            showUIElement(wrapGames);
            showUIElement(wrapAchievements);
        }

        private void hideVisualStyles()
        {
            if (!initialised)
                return;

            Console.WriteLine("Hiding visual styles");

            // Show data-grids
            showUIElement(dgGames);
            showUIElement(dgAchievementList);

            // Hide wrap panels
            hideUIElement(wrapGames);            
            hideUIElement(wrapAchievements);            
        }

        private void showUIElement(UIElement elem)
        {
            elem.IsEnabled = true;
            elem.Visibility = Visibility.Visible;
            Panel.SetZIndex(elem, 1);
        }
        private void hideUIElement(UIElement elem)
        {
            elem.IsEnabled = false;
            elem.Visibility = Visibility.Hidden;
            Panel.SetZIndex(elem, -1);
        }


        private void applyTheme()
        {
            tabControl.Background = new SolidColorBrush(Theme.primaryColour);
            toolBarMain.Background = new SolidColorBrush(Theme.primaryColour);
            statBarMain.Background = new SolidColorBrush(Theme.primaryColour);
            lblStatus.Background = new SolidColorBrush(Theme.primaryColour);


            bgUserProfile.Background = new SolidColorBrush(Theme.secondaryColour);
            bgConsoles.Background = new SolidColorBrush(Theme.secondaryColour);
            bgGames.Background = new SolidColorBrush(Theme.secondaryColour);
            bgAchievements.Background = new SolidColorBrush(Theme.secondaryColour);
            //bgLeaderboard.Background = new SolidColorBrush(Theme.secondaryColour);
            bgSettings.Background = new SolidColorBrush(Theme.secondaryColour);

        }
        private void applyVisualStyles()
        {
            if (rdoVisualStyles.IsChecked == true)
            {
                showVisualStyles();
            }
            else
            {
                hideVisualStyles();
            }
            if (tabControl != null)
            {
                switch (tabControl.SelectedIndex)
                {
                    case 1:
                        // Consoles
                        //displayTab_Consoles();
                        break;

                    case 2:
                        // Games
                        populateGamesTab(MyData.myData.currentConsole);
                        break;

                    case 3:
                        // Achievements
                        populateAchievementsTab(MyData.myData.currentGame);
                        break;

                    default:
                        Console.WriteLine("No tab selected");
                        break;
                }
            }
        }
        private void rdoVisualStyles_Checked(object sender, RoutedEventArgs e)
        {
            applyVisualStyles();
            //showVisualStyles();
        }

        private void rdoTextStyles_Checked(object sender, RoutedEventArgs e)
        {
            applyVisualStyles();
            //hideVisualStyles();
        }

        #endregion

    }
}
