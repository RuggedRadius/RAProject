using RAProject.Models;
using RAProject.Modules;
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
        public static void populateConsoleInfo(GameConsole console, System.Windows.Controls.Image imgConsole, Label lblConsoleName, Label lblConsoleGamesCount)
        {
            Application.Current.Dispatcher.Invoke(() => {
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
    }
}
