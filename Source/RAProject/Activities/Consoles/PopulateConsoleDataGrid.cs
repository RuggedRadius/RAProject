using RAProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RAProject.Consoles
{
    class PopulateConsoleDataGrid
    {
        // Population Methods
        /// <summary>
        /// Populates the console tab data grid.
        /// </summary>
        public static void populateConsoleDataGrid(DataGrid dgConsoleList)
        {
            // Clear list
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
    }
}
