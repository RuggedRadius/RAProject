using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;

namespace RAProject.Activities
{
    class LoadingData
    {
        /// <summary>
        /// Sets all images and labels to loading status, indicating to the user the values are coming.
        /// </summary>
        public static void SetLoadingLabels()
        {
            string loadingString = "Loading...";

            Application.Current.Dispatcher.Invoke(() => {

                // Image
                var image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri("Resources/loading.gif", UriKind.Relative);
                image.EndInit();
                

                // Labels
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.GetType() == typeof(MainWindow))
                    {
                        (window as MainWindow).lblConsoleName.Content = loadingString;
                        (window as MainWindow).lblGameTitle.Content = loadingString;
                        (window as MainWindow).lblGames_Developer.Content = loadingString;
                        (window as MainWindow).lblGames_Publisher.Content = loadingString;
                        (window as MainWindow).lblGames_Released.Content = loadingString;

                        ImageBehavior.SetAnimatedSource((window as MainWindow).imgGame, image);
                    }
                }
            });
        }
    }
}
