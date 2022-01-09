using RAProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace RAProject.Activities
{
    class FetchData
    {
        /// <summary>
        /// Sub method of intialise, downloads all consoles and their games from the retroachievements servers.
        /// </summary>
        /// <param name="initWindow"></param>
        /// <returns></returns>
        public static Task downloadAllMainData(InitialisationWindow initWindow)
        {
            return Task.Factory.StartNew(() =>
            {
                // Download consoles
                if (MyData.myData.consoles.Count == 0)
                {
                    MyData.DownloadConsoles();
                }

                // setup up progress bar
                Application.Current.Dispatcher.Invoke(() => {
                    initWindow.pbInit.Maximum = MyData.myData.consoles.Count;
                    initWindow.pbInit.Value = 0;
                });

                // Download games
                foreach (GameConsole console in MyData.myData.consoles)
                {
                    string status = String.Format("Downloading {0} games...", console.Name);
                    Console.WriteLine(status);

                    Application.Current.Dispatcher.Invoke(() => {
                        initWindow.UpdateStatus(status);
                    });

                    console.DownloadConsoleGames();

                    Application.Current.Dispatcher.Invoke(() => {
                        initWindow.pbInit.Value++;
                    });
                }
            });
        }
    }
}
