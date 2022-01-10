using RAProject.Models;
using RAProject.Modules;
using RAProject.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RAProject.Consoles
{
    class PopulateConsoleInformation
    {
        /// <summary>
        /// Populates the selected console's details in the informaiton panel.
        /// </summary>
        /// <param name="console">The console to display details from</param>
        public static void s_PopulateConsoleInfo(GameConsole console)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                // Set image
                System.Drawing.Image consoleImage = ConsoleInformation.getConsoleImage(console);

                if (consoleImage == null)
                {
                    // Debug error.
                    Console.WriteLine("Image not found...");
                }
                else
                {
                    // Update console image.
                    s_SetConsoleImage(console);

                    // Set title
                    s_SetConsoleName(console.Name);

                    // Set details
                    s_SetConsoleGamesCount(console.games.Count);
                }
            });
        }

        public static void s_SetConsoleGamesCount(int gamesCount)
        {
            // Get MainWindow reference.
            MainWindow wndMain = GetMainWindowUIControls.s_GetMainWindow();

            // Get console games count label control reference.
            wndMain.lblConsoleGamesCount.Content = gamesCount.ToString();
        }

        public static void s_SetConsoleName(string consoleName)
        {
            // Get MainWindow reference.
            MainWindow wndMain = GetMainWindowUIControls.s_GetMainWindow();

            // Get console name label control reference.
            wndMain.lblConsoleName.Content = consoleName;
        }

        public static void s_SetConsoleImage(GameConsole console)
        {
            System.Drawing.Image consoleImage = ConsoleInformation.getConsoleImage(console);

            if (consoleImage == null)
            {
                Console.WriteLine("Image not found...");
            }
            else
            {
                // Get image for console.
                System.Drawing.Image image = ConsoleInformation.getConsoleImage(console);

                // Convert image to ImageSource.
                ImageSource imageSource = s_ConvertImageToImageSource(consoleImage);

                // Get ImageSource destination
                GetMainWindowUIControls.s_GetMainWindow().imgConsole.Source = imageSource;
            }
        }

        public static ImageSource s_ConvertImageToImageSource(System.Drawing.Image image)
        {
            Bitmap bmp = new Bitmap(image);

            IntPtr hBitmap = bmp.GetHbitmap();
            
            ImageSource WpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            return WpfBitmap;
        }
    }
}
