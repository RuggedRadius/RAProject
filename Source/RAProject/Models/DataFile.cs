using System;
using System.Collections.Generic;

namespace RAProject.Models
{
    [Serializable]
    public class DataFile
    {
        // Attributes
        public List<SupportedConsole> consoles;

        public User currentUser;
        public SupportedConsole currentConsole;

        public DataFile()
        {
            consoles = new List<SupportedConsole>();
            currentUser = new User();
        }
    }
}
