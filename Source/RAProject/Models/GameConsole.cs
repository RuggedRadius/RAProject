using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RAProject.Connection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace RAProject.Models
{
    [Serializable]
    public class GameConsole
    {
        [JsonProperty("ID")]    public string ID { get; set; }
        [JsonProperty("Name")]  public string Name { get; set; }

        public List<Game> games;
        public string released { get; set; }

        public GameConsole(JToken j)
        {            
            ID = (string)j["ID"];
            Name = (string)j["Name"];
            games = new List<Game>();
        }

        /// <summary>
        /// Downloads all games for current console.
        /// </summary>
        public void DownloadConsoleGames()
        {
            Console.WriteLine("Downloading games list for {0}...", Name);

            // Fetch console list
            string url = Requests.Consoles.getConsoleGames(ID);
            string json = Requests.FetchJSON(url);
            dynamic data = JsonConvert.DeserializeObject(json);

            // Create list of Games
            if (data["game"][0] != null)
            {
                this.games = new List<Game>();

                foreach (JObject j in data["game"][0])
                {
                    // Create console object
                    Game newGame = new Game(j);

                    // Adds Game object to list in console's class
                    this.games.Add(newGame);

                    Console.WriteLine("Added game: " + newGame.Title);
                }
            }
        }

        public static GameConsole GetGameConsoleByName(string consoleName)
        {
            if (MyData.myData.consoles.Count == 0)
            {
                Console.WriteLine("WARNING: No consoles in MyData.");

                return null;
            }

            foreach (GameConsole gameConsole in MyData.myData.consoles)
            {
                if (gameConsole.Name == consoleName)
                {
                    return gameConsole;
                }
            }

            Console.WriteLine("WARNING: No console by given name found.");

            return null;
        }
    }
}