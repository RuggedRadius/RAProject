using RAProject.Models;
using RAProject.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAProject.Consoles
{
    class PopulateConsoleGames
    {
        public static void s_PopulateConsoleGames(System.Windows.Input.MouseButtonEventArgs e)
        {
            // Get reference to MainWindow.
            MainWindow wndMain = GetMainWindowUIControls.s_GetMainWindow();

            ConsoleDataRow row = (ConsoleDataRow)wndMain.dgConsoleList.SelectedItem;
            string consoleName = row.ConsoleName;

            // Search for selected console
            foreach (GameConsole console in MyData.myData.consoles)
            {
                if (console.Name == consoleName)
                {
                    // Set current console in MyData.
                    MyData.myData.currentConsole = console;
                    
                    // Reset current game to none in MyData.
                    MyData.myData.currentGame = null;

                    //// Populate console information panel
                    //populateGamesTab(console);

                    // Switch selected tab to Games tab.
                    wndMain.tabControl.SelectedIndex = 2;

                    // Prevent event from bubbling and re-triggering this method
                    e.Handled = true;

                    return;
                }
            }
        }
    }
}
