using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RAProject.Utilities
{
    class GetMainWindowUIControls
    {
        public static MainWindow s_GetMainWindow()
        {
            // Find MainWindow in currently open windows of current application.
            foreach (var window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(MainWindow))
                {
                    return (window as MainWindow);
                }
            }

            // If all fails, return null.
            return null;
        }
    }
}
