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
using System.Linq;
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

        #region Constructor
        private bool receivedTestData()
        {
            string url = Requests.Users.getUserSummary();
            string jsonString = Requests.FetchJSON(url);
            dynamic testFetchData = JsonConvert.DeserializeObject(jsonString);

            if (testFetchData == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            // Load data
            MyData.FileHandling.LoadData();

            // If connection settings are empty, 
            if (string.IsNullOrEmpty(Properties.Settings.Default.Credential_Username) ||
                string.IsNullOrEmpty(Properties.Settings.Default.Credential_APIKey))
            {
                int counter = 1;

                // While NOT receiving test data...
                while (!receivedTestData())
                {
                    // Show input window
                    Credentials wndCredentials = new Credentials(counter);
                    wndCredentials.ShowDialog();
                    counter++;
                }
            }
            
            // Apply theme
            applyTheme();

            // Start on user profile
            tabControl.SelectedIndex = 0;

            // Check if initialisation is required
            if (MyData.myData.consoles.Count == 0)
            {
                Initialise();
            }

            initialised = true;

            LaunchTracker();
        }
        #endregion

        #region General Methods
        /// <summary>
        /// Exits the app, calling all relevant clean ups before closing.
        /// </summary>
        private void exitApplication()
        {
            tknSrcTracker.Cancel();
            Environment.Exit(0);
        }

        /// <summary>
        /// Launches thread to track the current console and game.
        /// </summary>
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
                                        lblTrackerSeparator.Visibility = Visibility.Visible;
                                    }
                                    else
                                    {
                                        lblCurrentGame.Content = string.Empty;
                                        lblTrackerSeparator.Visibility = Visibility.Hidden;
                                    }
                                }
                                else
                                {
                                    lblCurrentConsole.Content = string.Empty;
                                    lblCurrentGame.Content = string.Empty;
                                    lblTrackerSeparator.Visibility = Visibility.Hidden;
                                }
                            }
                        });
                        Thread.Sleep(100);
                    }
                    catch (Exception ex) { ex.ToString(); }
                }
            });
        }
        /// <summary>
        /// Launching thread to detect if key data is missing, triggering the initialisation.
        /// </summary>
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

        /// <summary>
        /// Updates credentials in the app's settings.
        /// </summary>
        /// <param name="username">Username to store</param>
        /// <param name="key">API Key to store</param>
        public static void UpdateCredentials(string username, string key)
        {
            Properties.Settings.Default.Credential_Username = username;
            Properties.Settings.Default.Credential_APIKey = key;
        }
        /// <summary>
        /// Initialises the app on first-time run, downloads required data.
        /// </summary>
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

            // Init MyData
            MyData.myData = new DataFile();

            // Download data
            await downloadAllMainData(wndInit);

            // Reset init flag
            initTrigger = false;
            initialising = false;

            // Close init window
            this.Visibility = Visibility.Visible;
            wndInit.Close();
        }
        /// <summary>
        /// Sub method of intialise, downloads all consoles and their games from the retroachievements servers.
        /// </summary>
        /// <param name="initWindow"></param>
        /// <returns></returns>
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
        }
        private void miLoadData_Click(object sender, RoutedEventArgs e)
        {
            MyData.FileHandling.LoadData();
        }
        #endregion

        #region Tabs
        #region Console Tab
        // Init
        private void tabConsoles_Selected(object sender, RoutedEventArgs e)
        {
            populateConsoleDataGrid();
        }     

        // Population Methods
        /// <summary>
        /// Populates the console tab data grid.
        /// </summary>
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
        /// <summary>
        /// Populates the selected console's details in the informaiton panel.
        /// </summary>
        /// <param name="console">The console to display details from</param>
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

            selectedConsole.DownloadConsoleGames();

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
            else
            {
                MessageBox.Show("Select a console from the previous tab first.", "No Console Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                tabControl.SelectedIndex = 1;
            }
        }
        /// <summary>
        /// Sets all images and labels to loading status, indicating to the user the values are coming.
        /// </summary>
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
        /// <summary>
        /// Displays all the games for a particular console in the Games tab.
        /// </summary>
        /// <param name="console">The console of which to display games</param>
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
        /// <summary>
        /// Displays all the games for a particular console in the Games tab wrap panel.
        /// </summary>
        /// <param name="console">The console of which to display games</param>
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
        /// <summary>
        /// Displays all the games for a particular console in the Games tab data grid.
        /// </summary>
        /// <param name="console">The console of which to display games</param>
        private void populateGamesDataGrid(GameConsole console)
        {
            Dispatcher.Invoke(() =>
            {
                // Clear list
                dgGames.ItemsSource = console.games;
            });
        }
        /// <summary>
        /// Display the game information of a selected game.
        /// </summary>
        /// <param name="game">Game of which to show information</param>
        private async void FillGameInfoPanel(Game game)
        {
            // Set all fields to loading status
            SetLoadingLabels();

            // Load details
            await LoadGameDetails(game);

            // Load art
            await LoadGameBoxArt(game);
        }
        /// <summary>
        /// Determine appropriate box size/ratio depending on original ratio of various console's box art.
        /// </summary>
        /// <param name="img"></param>
        /// <param name="console"></param>
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

        /// <summary>
        /// Loads box art of selected game.
        /// </summary>
        /// <param name="game">The game of which to load box art.</param>
        /// <returns>Completed task.</returns>
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
        /// <summary>
        /// Loads game details of selected game.
        /// </summary>
        /// <param name="game"></param>
        /// <returns>Completed task.</returns>
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
        private void dgGames_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            string[] unwantedHeaders = {
            "ForumTopicID",
            "ConsoleID",
            "Flags",
            "ImageIcon",
            "GameIcon",
            "ImageTitle",
            "ImageBoxArt",
            "GameTitle",
            "Console",
            "ConsoleName",
            "ImageIngame",
            "Genre",
            "Achievements",
            "AchievementCount",
            };

            if (unwantedHeaders.Contains(e.Column.Header))
            {
                e.Column.Visibility = Visibility.Hidden;
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
        /// <summary>
        /// Sets all value labels on achievements panel to empty strings.
        /// </summary>
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
        /// <summary>
        /// Populates the achievements tab.
        /// </summary>
        /// <param name="game">Game with which to populate the achievements tab with.</param>
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
        /// <summary>
        /// Populates achievements tab data grid.
        /// </summary>
        /// <param name="game">Game with which to populate the achievements tab data grid with.</param>
        private void populateAchievementsDataGrid(Game game)
        {
            if (game.Achievements.Count == 0)
                game.DownloadAchievements();

            Dispatcher.Invoke(() =>
            {
                dgAchievementList.ItemsSource = game.Achievements;
            });
        }
        /// <summary>
        /// Populates achievements tab wrap panel.
        /// </summary>
        /// <param name="game">Game with which to populate the achievements tab wrap panel with.</param>
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
        /// <summary>
        /// Fills game details.
        /// </summary>
        /// <param name="game">Game with which to fill details with.</param>
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
        /// <summary>
        /// Displays achievement details of selected achievement.
        /// </summary>
        /// <param name="achievement">Achievement of which to show details of</param>
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
        private void tabAchievements_Selected(object sender, RoutedEventArgs e)
        {
            if (MyData.myData.currentGame != null)
            {
                populateAchievementsTab(MyData.myData.currentGame);
            }
        }
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
        /// <summary>
        /// Sets all images on leader board to loading images.
        /// </summary>
        private void ShowLoadingImages() 
        {

        }



        // Populate Methods
        /// <summary>
        /// Populates leader board with current top 10 users.
        /// </summary>
        private void populateLeaderBoard()
        {
            // Dont re-populate. Not the best way to do this but it'll do for now.
            if (stackLeaderBoard.Children.Count > 0)
                return;

            Task.Run(() => {
                // Init list
                List<User> leaders = new List<User>();

                // Fetch data
                string requestURL = Requests.Users.getTop10Users();
                string json = Requests.FetchJSON(requestURL);
                dynamic data = JsonConvert.DeserializeObject(json);

                Dispatcher.Invoke(() => {
                    // Clear stack panel
                    stackLeaderBoard.Children.Clear();
                });

                // Populate leader board
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
        /// <summary>
        /// Fetches a particular user's summary.
        /// </summary>
        /// <param name="username">Username of profile to fetch summary for</param>
        /// <returns></returns>
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
        /// <summary>
        /// Creates a user box to display user.
        /// </summary>
        /// <param name="rank">User's rank</param>
        /// <param name="name">User's name</param>
        /// <param name="score">User's score</param>
        /// <param name="ratio">User's true ratio</param>
        /// <returns></returns>
        private GroupBox CreateUserBox(int rank, string name, int score, int ratio)
        {
            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri("Resources/loading.gif", UriKind.Relative);
            image.EndInit();

            GroupBox newUser = new GroupBox();
            

            Dispatcher.Invoke(() => {
                newUser.Width = 370;
                newUser.Header = "#" + rank;
                newUser.Margin = new Thickness(20, 5, 20, 5);
                newUser.Background = new SolidColorBrush(Color.FromArgb(204, 0, 0, 0));
                newUser.BorderThickness = new Thickness(0);

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
        /// <summary>
        /// Sub-method of CreateUserBox, creates a label pair in horizontal stack panel.
        /// </summary>
        /// <param name="_label"></param>
        /// <param name="_value"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Displays user's profile.
        /// </summary>
        private void displayTab_UserProfile() 
        {
            // Populate fields with user data
            FillUserDetails();

            FillRecentlyPlayed();

            //FillLastGamePlayed();

            FillRecentAchievements();
        }

        // Population Methods
        /// <summary>
        /// Displays last game played by current user.
        /// </summary>
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
                                StackPanel gamePanel = new StackPanel();
                                gamePanel.Margin = new Thickness(5);
                                gamePanel.Tag = lastGamePlayed.ID;

                                //Size object
                                img.Height = 142;
                                img.Width = 142;
                                img.Tag = lastGamePlayed.ID;
                                img.Margin = new Thickness(10);
                                img.Cursor = Cursors.Hand;
                                gamePanel.Children.Add(img);

                                // Add click event handler
                                gamePanel.AddHandler(MouseDownEvent, new RoutedEventHandler(RecentlyPlayedGame_Click));

                                // Add object to wrap panel
                                grdLastPlayedGame.Children.Add(gamePanel);
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

        /// <summary>
        /// Populates user's score, rank and recently played games. All in one as the
        /// JSON that delivers this information is.
        /// </summary>
        private void FillRecentlyPlayed()
        {
            Dispatcher.Invoke(() => {
                // Clear wrap panel
                Dispatcher.Invoke(() =>
                {
                    wrpRecentlyPlayed.Children.Clear();
                });

                // For each game in list
                foreach (Game game in MyData.myData.currentUser.RecentlyPlayedGames)
                {
                    GroupBox newGamePanel = new GroupBox();

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
                            
                            newGamePanel.BorderThickness = new Thickness(0);
                            newGamePanel.Margin = new Thickness(5);
                            newGamePanel.Background = new SolidColorBrush(Color.FromArgb(204, 0, 0, 0));
                            newGamePanel.Header = "#" + (MyData.myData.currentUser.RecentlyPlayedGames.IndexOf(game) + 1);
                            

                            StackPanel gamePanel = new StackPanel();
                            //gamePanel.Margin = new Thickness(5);
                            gamePanel.Tag = game.ID;
                            //gamePanel.Background = new SolidColorBrush(Color.FromRgb(0, 128, 128));
                            newGamePanel.Content = gamePanel;

                            // Add Image
                            img.Height = 200;
                            img.Width = 200;
                            img.Cursor = Cursors.Hand;
                            gamePanel.Children.Add(img);

                            // Add Progress Bar
                            Task.Run(() => {
                                if (game.Achievements.Count <= 0)
                                {
                                    game.DownloadAchievements();
                                }

                                if (game.Achievements.Count > 0)
                                {
                                    // Get count of progress                                
                                    int completedCount = 0;
                                    foreach (Achievement ach in game.Achievements)
                                    {
                                        if (!string.IsNullOrEmpty(ach.DateEarned) || !string.IsNullOrEmpty(ach.DateEarnedHardcore))
                                        {
                                            completedCount++;
                                        }
                                    }

                                    // Create bar
                                    Dispatcher.Invoke(() => {
                                        ProgressBar pb = new ProgressBar();
                                        pb.Height = 20;
                                        pb.Width = img.Width;
                                        pb.Margin = new Thickness(0, -5, 0, 5);
                                        pb.Maximum = game.Achievements.Count;
                                        pb.Value = completedCount;
                                        gamePanel.Children.Add(pb);
                                    });
                                }
                            });


                            // Add click event handler
                            gamePanel.AddHandler(MouseDownEvent, new RoutedEventHandler(RecentlyPlayedGame_Click));



                        });
                    });

                    // Add object to wrap panel
                    wrpRecentlyPlayed.Children.Add(newGamePanel);

                }
            });
        }
        /// <summary>
        /// Populates user's recent achievements.
        /// </summary>
        private void FillRecentAchievements()
        {
            Console.WriteLine("Populating recent achievements...");

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
                        GroupBox newAchievement = new GroupBox();
                        newAchievement.Margin = new Thickness(5);
                        newAchievement.BorderThickness = new Thickness(0);
                        newAchievement.Width = 300;

                        StackPanel content = new StackPanel();
                        newAchievement.Content = content;
                        //content.Cursor = Cursors.Hand;
                        content.Tag = achievement.ID;
                        content.Background = new SolidColorBrush(Color.FromArgb(204, 0, 0, 0));

                        // Title
                        Label title = new Label();
                        title.Content = achievement.Title;
                        title.Foreground = new SolidColorBrush(Colors.White);
                        title.FontWeight = FontWeights.Bold;
                        title.FontStyle = FontStyles.Oblique;
                        title.FontSize = 20;
                        title.HorizontalContentAlignment = HorizontalAlignment.Center;

                        // Image
                        img.Height = 140;
                        img.Width = 140;
                        img.Tag = achievement.ID;
                        
                        // Description
                        TextBox achDescription = new TextBox();
                        achDescription.Foreground = new SolidColorBrush(Colors.White);
                        achDescription.BorderBrush = new SolidColorBrush(Colors.Transparent);
                        achDescription.Background = new SolidColorBrush(Colors.Transparent);
                        achDescription.MaxWidth = 260;
                        achDescription.Margin = new Thickness(10);
                        achDescription.TextWrapping = TextWrapping.WrapWithOverflow;
                        achDescription.Text = achievement.Description;
                        achDescription.IsReadOnly = true;
                        achDescription.IsHitTestVisible = false;

                        // Add content
                        content.Children.Add(title);
                        content.Children.Add(img);
                        content.Children.Add(achDescription);

                        // Add click event handler
                        content.AddHandler(MouseDownEvent, new RoutedEventHandler(RecentAchievement_Click));

                        // Add object to wrap panel
                        
                        wrpRecentAchievements.Children.Add(newAchievement);
                    });
                });
            }
        }
        /// <summary>
        /// Fills user details.
        /// </summary>
        private async void FillUserDetails()
        {
            Console.WriteLine("Populating user details...");

            Dispatcher.Invoke(() =>
            {
                // Update labels
                lblUsername.Content = Properties.Settings.Default.Credential_Username;
                lblUserScore.Content = MyData.myData.currentUser.score;
                lblUserRank.Content = string.Format("#{0}", MyData.myData.currentUser.Rank);
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
        private void tabUserProfile_Selected(object sender, RoutedEventArgs e)
        {
            displayTab_UserProfile();
        }
        private void RecentAchievement_Click(object sender, RoutedEventArgs e)
        {
            // Game BoxArt clicked
            StackPanel contentClicked = (StackPanel) sender;
            string achievementID = contentClicked.Tag.ToString();

            Achievement clickedAchievement = Search.SearchAchievements(achievementID);

            if (clickedAchievement != null)
            {
                Console.WriteLine("Achievement ID {0} clicked.", achievementID);
            }
            else
            {
                Console.WriteLine("Could not find Achievement ID {0}.", achievementID);
            }

        }
        private void RecentlyPlayedGame_Click(object sender, RoutedEventArgs e)
        {
            // Game BoxArt clicked
            StackPanel clickedContent = (StackPanel) sender;
            string gameID = clickedContent.Tag.ToString();

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
        /// <summary>
        /// Displays the settings tab.
        /// </summary>
        private void displayTab_Settings() { 

        }

        // Event Handlers
        private async void btnSaveCredentials_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => {
                // Update app settings
                Dispatcher.Invoke(() => {
                    // Update settings
                    Properties.Settings.Default.Credential_Username = txtSettingsUsername.Text;
                    Properties.Settings.Default.Credential_APIKey = txtSettingsAPIKey.Password;

                    // Store credentials in user object
                    MyData.myData.currentUser.addToCredentials(txtSettingsUsername.Text, Security.ComputeSha256Hash(txtSettingsAPIKey.Password));
                });

                Console.WriteLine("Displaying user profile");
                displayTab_UserProfile();
            });

        }

        #endregion

        #region Help Tab
        /// <summary>
        /// Display the help files.
        /// </summary>
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
                                MessageBox.Show("No results found", "No results found", MessageBoxButton.OK, MessageBoxImage.Information);
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
                                MessageBox.Show("No results found", "No results found", MessageBoxButton.OK, MessageBoxImage.Information);
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
        /// <summary>
        /// Shows all visual style elements on each tab.
        /// </summary>
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
        /// <summary>
        /// Hides all visual style elements on each tab.
        /// </summary>
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

        /// <summary>
        /// Shows a particular UI element.
        /// </summary>
        /// <param name="elem">UI element to show</param>
        private void showUIElement(UIElement elem)
        {
            elem.IsEnabled = true;
            elem.Visibility = Visibility.Visible;
            Panel.SetZIndex(elem, 1);
        }
        /// <summary>
        /// Hides a particular UI element.
        /// </summary>
        /// <param name="elem">UI element to hide</param>
        private void hideUIElement(UIElement elem)
        {
            elem.IsEnabled = false;
            elem.Visibility = Visibility.Hidden;
            Panel.SetZIndex(elem, -1);
        }

        /// <summary>
        /// 
        ///         *** [Currently in Alpha] ***
        /// 
        /// Applies the selected colour scheme to the application.
        /// </summary>
        private void applyTheme()
        {
            //tabControl.Background = new SolidColorBrush(Theme.primaryColour);
            //toolBarMain.Background = new SolidColorBrush(Theme.primaryColour);
            //statBarMain.Background = new SolidColorBrush(Theme.primaryColour);
            //lblStatus.Background = new SolidColorBrush(Theme.primaryColour);


            //bgUserProfile.Background = new SolidColorBrush(Theme.secondaryColour);
            //bgConsoles.Background = new SolidColorBrush(Theme.secondaryColour);
            //bgGames.Background = new SolidColorBrush(Theme.secondaryColour);
            //bgAchievements.Background = new SolidColorBrush(Theme.secondaryColour);
            ////bgLeaderboard.Background = new SolidColorBrush(Theme.secondaryColour);
            //bgSettings.Background = new SolidColorBrush(Theme.secondaryColour);

        }
        /// <summary>
        /// Master method of hideVisualStyles and showVisualStyles. 
        /// Call this to use current visual settings.
        /// </summary>
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

        private void groupBox_MouseEnter(object sender, MouseEventArgs e)
        {
            //UIElement source = (UIElement)e.Source;
            //source.
        }

        private void groupBox_MouseLeave(object sender, MouseEventArgs e)
        {

        }
    }
}
