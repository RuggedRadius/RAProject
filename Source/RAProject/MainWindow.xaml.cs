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
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WpfAnimatedGif;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;

namespace RAProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Properties
        // States
        bool initialised;
        bool initialising;
        bool populatingGames;
        static bool initTrigger;

        // Threads
        CancellationTokenSource tknSrcTracker;
        CancellationTokenSource tknSrcInit;
        CancellationToken tknTracker;
        CancellationToken tknInit;

        public FileInfo currentDataFile;
        #endregion

        // Constructor
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
            }

            applyTheme();

            // Check if initialisation is required
            if (MyData.myData.consoles.Count == 0)
            {
                Initialise();
            }

     

            initialised = true;

            LaunchTracker();
            
            updateStatus("Browser initialised");

        }


        #region General Methods
        private void exitApplication()
        {
            tknSrcTracker.Cancel();
            Environment.Exit(0);
        }
        public void updateStatus(string status)
        {
            Dispatcher.Invoke(() => {
                lblStatus.Content = status;
            });
        }

        private void LaunchTracker()
        {
            tknSrcTracker = new CancellationTokenSource();
            tknTracker = tknSrcTracker.Token;

            Task.Run(() => { 
                while (!tknTracker.IsCancellationRequested)
                {
                    try
                    {
                        Dispatcher.Invoke(() =>
                        {
                            if (MyData.myData != null)
                            {
                                if (MyData.myData.currentConsole != null)
                                {
                                    lblCurrentConsole.Content = MyData.myData.currentConsole.Name;

                                    if (MyData.myData.currentGame != null)
                                    {
                                        lblCurrentGame.Content = MyData.myData.currentGame.Title;
                                        icoTrackerArrow.Visibility = Visibility.Visible;
                                    }
                                    else
                                    {
                                        lblCurrentGame.Content = string.Empty;
                                        icoTrackerArrow.Visibility = Visibility.Hidden;
                                    }
                                }
                                else
                                {
                                    lblCurrentConsole.Content = string.Empty;
                                    lblCurrentGame.Content = string.Empty;
                                    icoTrackerArrow.Visibility = Visibility.Hidden;
                                }
                            }
                        });
                        Thread.Sleep(100);
                    }
                    catch (Exception ex) { ex.ToString(); }
                }
            });
        }
        private void LaunchInitDetector()
        {
            tknSrcInit = new CancellationTokenSource();
            tknInit = tknSrcInit.Token;

            Task.Run(() => {
                while (!tknInit.IsCancellationRequested)
                {
                    try
                    {
                        Dispatcher.Invoke(async () =>
                        {
                            if (initTrigger)
                            {
                                Initialise();
                            }
                        });
                        Thread.Sleep(100);
                    }
                    catch (Exception ex) { ex.ToString(); }
                }
            });
        }

        private async void Initialise()
        {
            if (initialising)
                return;

            // Begin initialisation
            initialising = true;

            // Show window
            InitialisationWindow wndInit = new InitialisationWindow();
            wndInit.Show();
            this.Visibility = Visibility.Hidden;
            wndInit.Focus();

            // Download data
            await downloadAllMainData(wndInit);

            // Reset init flag
            initTrigger = false;
            initialising = false;

            // Close init window
            this.Visibility = Visibility.Visible;
            wndInit.Close();
        }
        private Task downloadAllMainData(InitialisationWindow initWindow)
        {
            return Task.Factory.StartNew(() =>
            {
                // Download consoles
                if (MyData.myData.consoles.Count == 0)
                {
                    MyData.DownloadConsoles();
                }

                // setup up progress bar
                Dispatcher.Invoke(() => {
                    initWindow.pbInit.Maximum = MyData.myData.consoles.Count;
                    initWindow.pbInit.Value = 0;
                });

                // Download games
                foreach (GameConsole console in MyData.myData.consoles)
                {
                    string status = String.Format("Downloading {0} games...", console.Name);
                    Console.WriteLine(status);

                    Dispatcher.Invoke(() => {
                        initWindow.UpdateStatus(status);
                    });

                    console.DownloadConsoleGames();

                    Dispatcher.Invoke(() => {
                        initWindow.pbInit.Value++;
                    });
                }
            });
        }
        #endregion

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
            //downloadAllMainData();
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
        // Init
        private void tabConsoles_Selected(object sender, RoutedEventArgs e)
        {
            populateConsoleDataGrid();
        }     

        // Population Methods
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
            foreach (GameConsole console in MyData.myData.consoles)
            {
                // Filter non-games, don't know why they exist in the db.. they must have their reasons
                if (console.Name == "Events" || console.Name == "Hubs" || console.Name == "[Unused]")
                    continue;

                // Add to datagrid
                dgConsoleList.Items.Add(new ConsoleDataRow(console.Name, console.released, console.games.Count));
            }

            // Select first item
            if (MyData.myData.currentConsole == null)
            {
                dgConsoleList.SelectedIndex = 0;
            }
        }
        private void populateConsoleInfo (GameConsole console)
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

        // Event handlers
        private void btnDownloadConsoleGames_Click(object sender, RoutedEventArgs e)
        {
            // Get selected game
            ConsoleDataRow selectedConsoleRow = (ConsoleDataRow)dgConsoleList.SelectedItem;
            GameConsole selectedConsole = null;

            foreach (GameConsole console in MyData.myData.consoles)
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
            foreach (GameConsole console in MyData.myData.consoles)
            {
                if (console.Name == consoleName)
                {
                    // Update current console
                    MyData.myData.currentConsole = console;
                    MyData.myData.currentGame = null;

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
            foreach (GameConsole console in MyData.myData.consoles)
            {
                if (console.Name == consoleName)
                {
                    MyData.myData.currentConsole = console;
                    MyData.myData.currentGame = null;

                    //// Populate console information panel
                    //populateGamesTab(console);

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
        // Init
        private void tabGames_Selected(object sender, RoutedEventArgs e)
        {
            if (MyData.myData.currentConsole != null)
            {
                populateGamesTab(MyData.myData.currentConsole);
            }
        }
        private void SetLoadingLabels()
        {
            string loadingString = "Loading...";

            Dispatcher.Invoke(() => {
                // Image
                var image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri("Resources/loading.gif", UriKind.Relative);
                image.EndInit();
                ImageBehavior.SetAnimatedSource(imgGame, image);

                // Labels
                lblConsoleName.Content = loadingString;
                lblGameTitle.Content = loadingString;
                lblGames_Developer.Content = loadingString;
                lblGames_Publisher.Content = loadingString;
                lblGames_Released.Content = loadingString;
            });
        }

        // Population Methods
        private void populateGamesTab(GameConsole console)
        {
            Console.WriteLine("Populating games tab...");

            if (console.games.Count == 0)
            {
                console.DownloadConsoleGames();
            }

            if (console.games.Count == 0)
            {
                wrapGames.Visibility = Visibility.Hidden;
                dgGames.Visibility = Visibility.Hidden;

                txtblkNoGames.Visibility = Visibility.Visible;
            }
            else
            {
                if (miVisualStyles.IsChecked == true)
                {
                    wrapGames.Visibility = Visibility.Visible;

                    dgGames.Visibility = Visibility.Hidden;
                    txtblkNoGames.Visibility = Visibility.Hidden;

                    populateGamesWrapPanel(console);
                }
                else
                {
                    dgGames.Visibility = Visibility.Visible;

                    wrapGames.Visibility = Visibility.Hidden;
                    txtblkNoGames.Visibility = Visibility.Hidden;

                    populateGamesDataGrid(console);
                }
            }
        }
        private async void populateGamesWrapPanel(GameConsole console)
        {
            Console.WriteLine("Outputting visual style games...");

            populatingGames = true;

            await Dispatcher.Invoke(async () =>
            {
                // Clear wrap panel
                wrapGames.Children.Clear();

                // Populate
                foreach (Game game in console.games)
                {
                    if (!populatingGames)
                        break;

                    if (tabControl.SelectedIndex != 2)
                        break;

                    await Task.Run(() => {

                        if (game.imgBoxArt == null)
                            game.downloadImage_BoxArt();

                        // Create image
                        Dispatcher.Invoke(() => {

                            System.Windows.Controls.Image newImageTile = ImageConversion.ConvertDrawingImageToWPFImage(game.imgBoxArt);

                            // Size image
                            DetermineBoxArtSize(newImageTile, console);

                            // Add uniform margin
                            newImageTile.Margin = new Thickness(2);

                            // Add ID as tag
                            newImageTile.Tag = game.ID;

                            // Add click event handler
                            newImageTile.MouseDown += GameTile_MouseDown;

                            // Cursor
                            newImageTile.Cursor = Cursors.Hand;

                            // Add to games wrap panel
                            wrapGames.Children.Add(newImageTile);
                        });
                    });
                }
            });

            populatingGames = false;

            Console.WriteLine("Populating {0} games completed/stopped", console.Name);
        }
        private void populateGamesDataGrid(GameConsole console)
        {

            Dispatcher.Invoke(() =>
            {
                // Clear list
                dgGames.ItemsSource = console.games;
            });
        }
        private async void FillGameInfoPanel(Game game)
        {
            // Set all fields to loading status
            SetLoadingLabels();

            // Load details
            await LoadGameDetails(game);

            // Load art
            await LoadGameBoxArt(game);
        }

        private void DetermineBoxArtSize(System.Windows.Controls.Image img, GameConsole console)
        {
            switch (console.Name)
            {
                case ConsoleInformation.ConsoleNames.GameBoy:
                    img.Width = 160;
                    img.Height = 160;
                    break;

                case ConsoleInformation.ConsoleNames.GameBoyAdvance:
                    img.Width = 160;
                    img.Height = 160;
                    break;

                case ConsoleInformation.ConsoleNames.GameBoyColor:
                    img.Width = 160;
                    img.Height = 160;
                    break;

                case ConsoleInformation.ConsoleNames.SNES:
                    img.Width = 240;
                    img.Height = 160;
                    break;

                case ConsoleInformation.ConsoleNames.PlayStation:
                    img.Width = 160;
                    img.Height = 160;
                    break;

                default:
                    img.Width = 160;
                    img.Height = 240;
                    break;
            }
        }

        private Task LoadGameBoxArt(Game game)
        {
            return Task.Factory.StartNew(() => {
                
                System.Drawing.Image gameArt = null;
                if (game.imgBoxArt == null)
                {
                    gameArt = game.downloadImage_BoxArt();
                }
                else
                {
                    gameArt = game.imgBoxArt;
                }

                Dispatcher.Invoke(() => {
                    // Reset UI image
                    imgGame.Source = null;
                });

                // Set image
                if (gameArt == null)
                {
                    Console.WriteLine("Image not found...");
                }
                else
                {
                    Dispatcher.Invoke(() => {
                        // Create new source
                        Bitmap bmp = new Bitmap(gameArt);
                        IntPtr hBitmap = bmp.GetHbitmap();
                        ImageSource WpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                        
                        // Turn off animated gif
                        ImageBehavior.SetAnimatedSource(imgGame, null);

                        // Set new source
                        imgGame.Source = WpfBitmap;
                    });
                }
            });
        }
        private Task LoadGameDetails(Game game)
        {
            return Task.Factory.StartNew(() => {                
                // Check details for null
                if (string.IsNullOrEmpty(game.Developer) ||
                string.IsNullOrEmpty(game.Released) ||
                string.IsNullOrEmpty(game.Publisher) ||
                string.IsNullOrEmpty(game.Title)) {
                    game.downloadGameData();
                }

                // Fill details
                Dispatcher.Invoke(() => {
                    lblConsoleName.Content = game.ConsoleName;
                    lblGameTitle.Content = game.Title;
                    lblGames_Developer.Content = game.Developer;
                    lblGames_Publisher.Content = game.Publisher;
                    lblGames_Released.Content = game.Released;
                });
            });
        }


        // Event Handlers
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

            // Get selected game
            Game selectedGame = (Game)dgGames.SelectedItem;
            MyData.myData.currentGame = selectedGame;

            // Display game info
            Task.Run(() =>
            {
                FillGameInfoPanel(selectedGame);
            });            
        }
        private void GameTile_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
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
                    //// Populate achievement data grid
                    //populateAchievementsTab(selectedGame);

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

        private void btnDownloadAchievements_Click(object sender, RoutedEventArgs e)
        {
            if (MyData.myData.currentGame != null)
            {
                MyData.myData.currentGame.DownloadAchievements();
                MyData.myData.currentGame.downloadImage_BoxArt();

                dgGames.SelectedIndex++;
                dgGames.SelectedIndex--;
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
        // Init
        private void tabAchievements_Selected(object sender, RoutedEventArgs e)
        {
            if (MyData.myData.currentGame != null)
            {
                populateAchievementsTab(MyData.myData.currentGame);
            }
        }
        private void ClearPanelLabels()
        {
            lblSelectedAchievementTitle.Content = string.Empty;
            lblAchievementDescription.Content = string.Empty;
            lblDateEarned.Content = string.Empty;
            lblDateHardcore.Content = string.Empty;
            lblAuthor.Content = string.Empty;
            lblPoints.Content = string.Empty;
            lblNumAwarded.Content = string.Empty;
            lblNumAwardedHardcore.Content = string.Empty;
        }

        // Populate methods
        private void populateAchievementsTab(Game game)
        {
            ClearPanelLabels();

            if (game.Achievements.Count <= 0)
            {
                dgAchievementList.Visibility = Visibility.Hidden;
                wrapAchievements.Visibility = Visibility.Hidden;
                txtblkNoAchievements.Visibility = Visibility.Visible;
            }
            else
            {
                if (miVisualStyles.IsChecked == true)
                {
                    dgAchievementList.Visibility = Visibility.Hidden;
                    txtblkNoAchievements.Visibility = Visibility.Hidden;

                    wrapAchievements.Visibility = Visibility.Visible;

                    populateAchievementsWrapPanel(game);
                }
                else
                {
                    wrapAchievements.Visibility = Visibility.Hidden;
                    txtblkNoAchievements.Visibility = Visibility.Hidden;

                    dgAchievementList.Visibility = Visibility.Visible;

                    populateAchievementsDataGrid(game);
                }
            }


            // Show game details
            FillGameDetails(game);
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
        private async void FillGameDetails(Game game)
        {
            if (game.imgBoxArt == null)
            {
                await game.downloadAsyncBoxArt();
            }

            // Set game info
            imgGameBoxArt.Source = ImageConversion.ImageToSource(game.imgBoxArt);
            lblSelectedGameTitle.Content = game.Title;

            // Progress bar
            pbAchievementsTotal.Maximum = game.Achievements.Count;
            int completedCount = 0;
            foreach (Achievement ach in game.Achievements)
            {
                if (!string.IsNullOrEmpty(ach.DateEarned) || !string.IsNullOrEmpty(ach.DateEarnedHardcore))
                {
                    completedCount++;
                }
            }
            pbAchievementsTotal.Value = completedCount;
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
            lblDateHardcore.Content = achievement.DateEarnedHardcore;

            lblAuthor.Content = achievement.Author;
            lblNumAwarded.Content = achievement.NumAwarded;
            lblNumAwardedHardcore.Content = achievement.NumAwardedHardcore;
            lblPoints.Content = achievement.Points;
        }

        // Event Handlers
        private void dgAchievementList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgAchievementList.SelectedItem != null)
            {
                FillAchievementDetails((Achievement)dgAchievementList.SelectedItem);
            }
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
        private void btnDownloadGameAchievements_Click(object sender, RoutedEventArgs e)
        {
            if (MyData.myData.currentGame != null)
            {
                MyData.myData.currentGame.DownloadAchievements();
                MyData.myData.currentGame.downloadImage_BoxArt();

                if (MyData.myData.currentGame.Achievements.Count > 0)
                {
                    populateAchievementsTab(MyData.myData.currentGame);
                }
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

        #region Leader Board Tab
        // Init
        private void tabLeaderBoard_Selected(object sender, RoutedEventArgs e)
        {
            populateLeaderBoard();
        }
        private void ShowLoadingImages() 
        {

        }



        // Populate Methods
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

                    Dispatcher.Invoke(() => {
                        // Create group box
                        GroupBox newUser = CreateUserBox(rank, username, score, trueratio);

                        

                        // Add group box
                        stackLeaderBoard.Children.Add(newUser);
                    });
                }

            });
        }
        public string getUserSummary(string username)
        {
            return String.Format(
                "{0}{1}?user={2}&key={3}&member={4}&results=10&mode=json",
                Constants.BASE_URL,
                Constants.QueryTypes.WEB_USER_SUMMARY,
                Properties.Settings.Default.Credential_Username,
                Properties.Settings.Default.Credential_APIKey,
                username
                );
        }
        private GroupBox CreateUserBox(int rank, string name, int score, int ratio)
        {
            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri("Resources/loading.gif", UriKind.Relative);
            image.EndInit();

            GroupBox newUser = new GroupBox();
            newUser.Width = 420;

            Dispatcher.Invoke(() => {
                newUser.Header = "#" + rank;
                newUser.Margin = new Thickness(20, 5, 20, 5);

                StackPanel userLayout = new StackPanel();
                userLayout.Orientation = Orientation.Horizontal;
                newUser.Content = userLayout;

                System.Windows.Controls.Image userAvatar = new System.Windows.Controls.Image();
                userAvatar.Width = 100;
                userAvatar.Height= 100;
                ImageBehavior.SetAnimatedSource(userAvatar, image);
                userLayout.Children.Add(userAvatar);

                Task.Run(() => {
                    string url = getUserSummary(name);
                    string jsonString = Requests.FetchJSON(url);
                    dynamic data = JsonConvert.DeserializeObject(jsonString);

                    User newUserObj = new User(data);
                    newUserObj.fetchUserAvatar();
                    Dispatcher.Invoke(() => {
                        ImageBehavior.SetAnimatedSource(userAvatar, null);
                        userAvatar.Source = ImageConversion.ImageToSource(newUserObj.userAvatar);
                    });
                });

                StackPanel userStack = new StackPanel();
                userLayout.Children.Add(userStack);

                Label uName = new Label();
                uName.Content = name;
                uName.FontSize = 30;
                uName.Foreground = new SolidColorBrush(Colors.White);
                userStack.Children.Add(uName);

                userStack.Children.Add(CreateLabelPair("Score", score.ToString()));
                userStack.Children.Add(CreateLabelPair("True Ratio", ratio.ToString()));
            });

            return newUser;
        }
        private StackPanel CreateLabelPair(string _label, string _value)
        {
            StackPanel labelPair = new StackPanel();
            labelPair.Orientation = Orientation.Horizontal;

            Label label = new Label();
            label.Content = _label + ": ";
            label.Foreground = new SolidColorBrush(Colors.White);
            labelPair.Children.Add(label);

            Label value = new Label();
            value.Content = _value;
            value.Foreground = new SolidColorBrush(Colors.LightGray);
            labelPair.Children.Add(value);

            return labelPair;
        }
        #endregion

        #region User Tab
        // Init
        private void tabUserProfile_Selected(object sender, RoutedEventArgs e)
        {
            displayTab_UserProfile();
        }
        private async void displayTab_UserProfile() 
        {
            await Task.Run(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    // Check user object exists, if not create it and fetch it's data
                    if (MyData.myData.currentUser == null)
                    {
                        Console.WriteLine("No user found in myData.");
                        MyData.myData.currentUser = new User();
                    }
                });
            });

            // Populate fields with user data
            FillUserDetails();

            populateRankScoreAndRecentlyPlayed();

            FillLastGamePlayed();

            populateRecentAchievements();
        }

        // Population Methods
        private void FillLastGamePlayed()
        {
            if (MyData.myData.currentUser.LastGameId > 0)
            {
                Game[] allGames = Search.getAllGames();
                Game lastGamePlayed = null;

                if (allGames.Length == 0)
                {
                    initTrigger = true;
                    Initialise();
                }

                if (allGames.Length > 0)
                {
                    // Find game
                    foreach (Game game in allGames)
                    {
                        if (Int32.Parse(game.ID) == MyData.myData.currentUser.LastGameId)
                        {
                            // Found
                            lastGamePlayed = game;
                        }
                    }

                    if (lastGamePlayed != null)
                    {
                        // Add image to group box
                        Task.Run(() =>
                        {
                            System.Windows.Controls.Image img = null;

                            // Download game's Box Art, if necessary
                            if (lastGamePlayed.imgBoxArt == null)
                            {
                                // Download the box art
                                lastGamePlayed.downloadImage_BoxArt();
                            }
                            // Create image object for BoxArt
                            img = ImageConversion.ConvertDrawingImageToWPFImage(lastGamePlayed.imgBoxArt);

                            Dispatcher.Invoke(() =>
                            {
                            //Size object
                            img.Height = 142;
                            img.Width = 142;
                            img.Tag = lastGamePlayed.ID;
                            img.Margin = new Thickness(10);

                            // Add click event handler
                            img.AddHandler(MouseDownEvent, new RoutedEventHandler(RecentlyPlayedGame_Click));

                            // Add object to wrap panel
                            grdLastPlayedGame.Children.Add(img);
                            });
                        });
                    }
                    else
                    {
                        // Not found
                    }
                }
            }
        }
        private void populateRankScoreAndRecentlyPlayed()
        {
            Dispatcher.Invoke(() => {

                if (MyData.myData.currentUser.RecentlyPlayedGames == null)
                {
                    // Fetch list of user's recently played games
                    //MyData.myData.currentUser.getRecentGames();
                }

                lblUserScore.Content = MyData.myData.currentUser.score;
                lblUserRank.Content = string.Format("#{0}", MyData.myData.currentUser.Rank);

                // Clear wrap panel
                Dispatcher.Invoke(() =>
                {
                    wrpRecentlyPlayed.Children.Clear();
                });

                // For each game in list
                foreach (Game game in MyData.myData.currentUser.RecentlyPlayedGames)
                {
                    Task.Run(() =>
                    {


                        System.Windows.Controls.Image img = null;

                        // Download game's Box Art, if necessary
                        if (game.imgBoxArt == null)
                        {
                            // Download the box art
                            game.downloadImage_BoxArt();
                        }
                        // Create image object for BoxArt
                        img = ImageConversion.ConvertDrawingImageToWPFImage(game.imgBoxArt);

                        Dispatcher.Invoke(() =>
                        {
                            //Size object
                            img.Height = 150;
                            img.Width = 150;
                            img.Tag = game.ID;

                            // Add click event handler
                            img.AddHandler(MouseDownEvent, new RoutedEventHandler(RecentlyPlayedGame_Click));

                            // Add object to wrap panel
                            wrpRecentlyPlayed.Children.Add(img);

                        });
                    });
                }
            });

            

            
        }
        private void populateRecentAchievements()
        {
            Console.WriteLine("Populating recent achievements...");

            // Fetch list of user's recently played games
            if (MyData.myData.currentUser.RecentAchievements == null)
            {
                //MyData.myData.currentUser.getRecentAchievements();
            }

            // Clear wrap panel
            Dispatcher.Invoke(() =>
            {
                wrpRecentAchievements.Children.Clear();

                if (MyData.myData.currentUser.RecentAchievements.Count == 0)
                {
                    Label newLabel = new Label();
                    newLabel.Content = "No achievements in the last week :(";
                    newLabel.Foreground = new SolidColorBrush(Colors.White);
                    newLabel.Margin = new Thickness(60);
                    wrpRecentAchievements.Children.Add(newLabel);
                }
            });



            // For each game in list
            foreach (Achievement achievement in MyData.myData.currentUser.RecentAchievements)
            {
                Task.Run(() =>
                {
                    System.Windows.Controls.Image img = null;

                    // Download game's Box Art, if necessary
                    if (achievement.badge == null)
                    {
                        // Download the box art
                        achievement.getBadge();
                    }

                    // Create image object for BoxArt
                    img = ImageConversion.ConvertDrawingImageToWPFImage(achievement.badge);

                    Dispatcher.Invoke(() =>
                    {
                        //Size object
                        img.Height = 140;
                        img.Width = 140;
                        img.Tag = achievement.ID;

                        // Add click event handler
                        img.AddHandler(MouseDownEvent, new RoutedEventHandler(RecentAchievement_Click));

                        // Add object to wrap panel
                        wrpRecentAchievements.Children.Add(img);
                    });
                });
            }
        }
        private void populateRecentlyPlayedGames()
        {
            Console.WriteLine("Populating recently played games");

            // Fetch list of user's recently played games
            if (MyData.myData.currentUser.RecentlyPlayedGames == null)
            {
                //MyData.myData.currentUser.getRecentGames();
            }

        }
        private async void FillUserDetails()
        {
            Console.WriteLine("Populating user details...");
            // Update status
            Dispatcher.Invoke(() =>
            {
                lblStatus.Content = "Populating user details...";

                // Update label
                lblUsername.Content = Properties.Settings.Default.Credential_Username;
            });

            await Task.Run(() =>
            {
                if (MyData.myData != null && MyData.myData.currentUser.userAvatar == null)
                {
                    // Fetch user avatar
                    MyData.myData.currentUser.fetchUserAvatar();
                }

                // Update User Profile Avatar
                Dispatcher.Invoke(() => {
                    imgUserAvatar.Source = new BitmapImage(new Uri(MyData.myData.currentUser.UserPic));
                });
            });
        }

        // Event Handlers
        private void RecentAchievement_Click(object sender, RoutedEventArgs e)
        {
            // Game BoxArt clicked
            System.Windows.Controls.Image img = (System.Windows.Controls.Image)sender;
            string achievementID = img.Tag.ToString();

            Console.WriteLine("Achievement ID {0} clicked.", achievementID);
        }
        private void RecentlyPlayedGame_Click(object sender, RoutedEventArgs e)
        {
            // Game BoxArt clicked
            System.Windows.Controls.Image img = (System.Windows.Controls.Image) sender;
            string gameID = img.Tag.ToString();

            Game clickedGame = null;
            foreach (Game game in MyData.myData.currentUser.RecentlyPlayedGames)
            {
                if (game.ID.Equals(gameID))
                {
                    clickedGame = game;
                }
            }

            MyData.myData.currentConsole = Search.SearchConsoles(clickedGame.ConsoleName);
            MyData.myData.currentGame = clickedGame;


            Task.Run(() =>
            {
                clickedGame.DownloadAchievements();

                Console.WriteLine(clickedGame.Title + " clicked");

                GameConsole console = Search.SearchConsoles(clickedGame.ConsoleName);

                MyData.myData.currentConsole = console;
                MyData.myData.currentGame = clickedGame;

                Application.Current.Dispatcher.BeginInvoke((Action)delegate {
                    tabControl.SelectedIndex = 3;
                }, DispatcherPriority.Render, null);
            });



        }
        #endregion

        #region Settings Tab
        // Init
        private void tabSettings_Selected(object sender, RoutedEventArgs e)
        {
            displayTab_Settings();
        }
        private void displayTab_Settings() { 

        }

        // Event Handlers
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
            //string pathToHTMLHelpFile = @"Help/index.html";
            //Uri helpPath = new Uri(pathToHTMLHelpFile, UriKind.Relative);
            //Uri AbsolutePath = Uri.
            //System.Diagnostics.Process.Start("Chrome", new Uri(pathToHTMLHelpFile, UriKind.Relative).ToString());

            string exeFile = new System.Uri(Assembly.GetEntryAssembly().CodeBase).AbsolutePath;
            string Dir = Path.GetDirectoryName(exeFile);
            string path = Path.GetFullPath(Path.Combine(Dir, @"..\..\Help\index.html"));
            System.Diagnostics.Process.Start(path);
        }
        #endregion

        #region Search
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (cmbSearchCategory.SelectedIndex >= 0)
            {
                if (chkExactMatchesOnly.IsChecked == true)
                {
                    // Search in selected category
                    string searchQuery = txtSearchQuery.Text;

                    // Filter search query here
                    // ...

                    switch (cmbSearchCategory.SelectedIndex)
                    {
                        // Consoles
                        case 0:
                            GameConsole searchConsole = Search.SearchConsoles(txtSearchQuery.Text);
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
                            // Search for game named EXACTLY as search query
                            Game searchGame = Search.SearchGames(txtSearchQuery.Text);

                            if (searchGame != null)
                            {

                                // Update current game
                                MyData.myData.currentGame = searchGame;

                                // Find relevant console
                                GameConsole console = Search.SearchConsoles(searchGame.Console);
                                MyData.myData.currentConsole = console;



                                // Switch to console tab
                                tabControl.SelectedIndex = 2;

                                // Populate games tab
                                populateGamesTab(console);


                                if (miVisualStyles.IsChecked == false)
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

                                    // Some how scroll down to cover image
                                    // ...

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
                    // Implement similar-match search function
                }
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
            if (miVisualStyles.IsChecked == true)
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
                        if (!populatingGames)
                        {
                            populateGamesTab(MyData.myData.currentConsole);
                        }
                        else
                        {
                            populatingGames = false;
                            populateGamesTab(MyData.myData.currentConsole);
                        }
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
        public void VisualStyles_Checked(object sender, RoutedEventArgs e)
        {
            if (initialised)
                applyVisualStyles();
        }
        #endregion

        

        

        

        

        

        
    }
}
