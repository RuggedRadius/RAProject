using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RAProject.Connection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace RAProject.Models
{
    [Serializable]
    public class User
    {

        [JsonProperty("score")] public string score { get; set; }
        [JsonProperty("trueratio")] public string trueratio { get; set; }
        [JsonProperty("ID")] public long Id { get; set; }
        [JsonProperty("TotalPoints")] public long TotalPoints { get; set; }
        [JsonProperty("TotalTruePoints")] public long TotalTruePoints { get; set; }
        [JsonProperty("Status")] public string Status { get; set; }
        [JsonProperty("Points")] public long Points { get; set; }
        [JsonProperty("LastGameID")] public int LastGameId { get; set; }
        [JsonProperty("MemberSince")] public DateTimeOffset MemberSince { get; set; }
        [JsonProperty("Rank")] public long Rank { get; set; }
        [JsonProperty("UserPic")] public string UserPic { get; set; }

        public List<Game> RecentlyPlayedGames;
        public Image userAvatar;

        private Hashtable credentials;


        // Default Constructor
        public User() {
            fetchUserData();
            

            credentials = new Hashtable();
        }

        // Constructor
        public User(JToken j)
        {
            score = (string) j["score"];
            trueratio = (string) j["trueratio"];

            if (j["LastGameID"] != null)
            {
                LastGameId = (int)j["LastGameID"];
            }

            if (j["RecentlyPlayed"] != null)
            {
                // Get recent games
                RecentlyPlayedGames = new List<Game>();
                foreach (JObject jo in j["RecentlyPlayed"])
                {
                    Game recentGame = new Game(jo);
                    RecentlyPlayedGames.Add(recentGame);
                }
            }
            if (j["UserPic"] != null)
            {
                UserPic = "http://retroachievements.org";
                UserPic += j["UserPic"].ToString();
            }
        }


        public void addToCredentials(string username, string pwdHash)
        {
            credentials.Clear();

            // Add username as key and hashed API key as the value
            credentials.Add(username, pwdHash);
        }



        public bool getRecentGames()
        {
            Console.WriteLine("Fetching {0}'s recently played games...");

            // Initialise a new list of games
            RecentlyPlayedGames = new List<Game>();

            // Fetch JSON Data
            string url = Requests.Users.getUserSummary(); 
            string jsonString = Requests.FetchJSON(url);
            dynamic data = JsonConvert.DeserializeObject(jsonString);

            
            if (data["RecentlyPlayed"][0] != null)
            {
                // For each game found...
                foreach (JObject game in data["RecentlyPlayed"])
                {
                    // Create game
                    Game newGame = new Game(game);

                    // Adds Game object to user's list of recently played games
                    RecentlyPlayedGames.Add(newGame);

                    Console.WriteLine("Added to user's recently played list: " + newGame.Title);
                }
            }

            return true;
        }

        public bool fetchUserAvatar()
        {
            Console.WriteLine("Fetching user avatar...");
            if (UserPic == null)
            {
                Console.WriteLine("No user avatar available.");
                userAvatar = Image.FromFile("Resources/maxresdefault.jpg");
            }
            else
            {
                Console.WriteLine("Downloading user avatar...");
                userAvatar = Requests.DownloadImageFromUrl(UserPic); // ...    /RuggedRadius.png
            }
            return true;
        }

        public void fetchUserData()
        {
            // Determine URL
            string url = Requests.Users.getUserSummary();

            // Fetch user JSON data
            string jsonString = Requests.FetchJSON(url);

            // Deserialize JSON into object
            dynamic data = JsonConvert.DeserializeObject(jsonString);

            score = (string) data["score"];
            trueratio = (string) data["trueratio"];

            if (data["LastGameID"] != null)
            {
                LastGameId = (int) data["LastGameID"];
            }

            if (data["RecentlyPlayed"] != null)
            {
                // Get recent games
                RecentlyPlayedGames = new List<Game>();
                foreach (JObject jo in data["RecentlyPlayed"])
                {
                    Game recentGame = new Game(jo);
                    RecentlyPlayedGames.Add(recentGame);
                }
            }
            if (data["UserPic"] != null)
            {
                UserPic = "http://retroachievements.org";
                UserPic += data["UserPic"].ToString();
            }
        }
    }
}

