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
        public List<GameConsole> consoles;

        public User currentUser;
        public GameConsole currentConsole;
        public Game currentGame;

        public string username;
        public string apikey;

        public DataFile()
        {
            consoles = new List<GameConsole>();            
            currentUser = new User();
        }
    }
}
