using Newtonsoft.Json;
using RAProject.Connection;
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

            string url = Requests.Users.getUserSummary(); // Determine URL of user summary
            string jsonString = Requests.FetchJSON(url); // Fetch user JSON data
            dynamic data = JsonConvert.DeserializeObject(jsonString); // Deserialize JSON into object

            if (data == null)
            {
                //MessageBox.Show("Error creating user.\n\nCheck credentials, internet connection and firewall settings.",
                //    "Error creating user", 
                //    MessageBoxButton.OK, 
                //    MessageBoxImage.Error);
            }
            else
            {
                currentUser = new User(data);
            }
        }
    }
}
