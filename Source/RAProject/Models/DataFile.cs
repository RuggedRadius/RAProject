using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;

namespace RAProject.Models
{
    [Serializable]
    public class DataFile
    {
        // Attributes
        public List<SupportedConsole> consoles;

        public User currentUser;
        public SupportedConsole currentConsole;
        public Game currentGame;

        public DataFile()
        {
            consoles = new List<SupportedConsole>();            
            currentUser = new User();
        }
    }
}
