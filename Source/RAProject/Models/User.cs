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
using System.Windows;
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

        public Game lastGame;
        public List<Game> RecentlyPlayedGames;
        public List<Achievement> RecentAchievements;
        public Image userAvatar;
        private Hashtable credentials;

        // Constructors
        public User() {
            fetchUserData();
            credentials = new Hashtable();
        }
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

        /// <summary>
        /// Adds username and hashed API key to hashtable of credentials.
        /// </summary>
        /// <param name="username">username stored as key</param>
        /// <param name="pwdHash">hashed password stored as value</param>
        public void addToCredentials(string username, string pwdHash)
        {
            credentials.Clear();

            // Add username as key and hashed API key as the value
            credentials.Add(username, pwdHash);
        }

        /// <summary>
        /// Extracts recent achievements from user summary JSON data.
        /// </summary>
        /// <param name="data">Deserialised JSON data</param>
        /// <returns>true on completion</returns>
        public bool getRecentAchievements(dynamic data)
        {
            Console.WriteLine("Fetching user's recent achievements...");

            // Initialise a new list of games
            RecentAchievements = new List<Achievement>();

            int maxCounter = 10;
            if (data["achievement"][0] != null)
            {
                // For each game found...
                foreach (JObject achievement in data["achievement"][0])
                {
                    if (maxCounter > 0)
                    {
                        // Create game
                        Achievement newAchievement = new Achievement(achievement);

                        // Adds Game object to user's list of recently played games
                        RecentAchievements.Add(newAchievement);

                        maxCounter--;

                        Console.WriteLine("Added to user's recently achieved list: " + newAchievement.Title);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return true;
        }
        /// <summary>
        /// Extracts last game played from user summary JSON data.
        /// </summary>
        /// <param name="data">Deserialised JSON data</param>
        public void getLastGame(dynamic data)
        {
            score = (string)data["Points"];
            trueratio = (string)data["trueratio"];

            if (data["LastGameID"] != null)
            {
                LastGameId = (int)data["LastGameID"];
            }
        }



        /// <summary>
        /// Fetches user's recent games.
        /// </summary>
        /// <returns></returns>
        public bool getRecentGames(dynamic data)
        {
            Console.WriteLine("Fetching {0}'s recently played games...", Properties.Settings.Default.Credential_Username);

            // Initialise a new list of games
            RecentlyPlayedGames = new List<Game>();
            
            if (data["RecentlyPlayed"][0] != null)
            {
                // For each game found...
                foreach (JObject game in data["RecentlyPlayed"])
                {
                    // Search for existing game
                    Game newGame = RAProject.Utilities.Search.SearchGames(game["Title"].ToString());

                    // Create game
                    if (newGame == null)
                    {
                        Console.WriteLine("Game not found locally, creating new record..");
                        newGame = new Game(game);
                    }

                    // Adds Game object to user's list of recently played games
                    RecentlyPlayedGames.Add(newGame);

                    Console.WriteLine("Added to user's recently played list: " + newGame.Title);
                }
            }

            score = (string)data["Points"];
            trueratio = (string)data["trueratio"];
            Rank = (long)data["Rank"];

            if (data["LastGameID"] != null)
            {
                LastGameId = (int)data["LastGameID"];
            }

            if (data["LastGame"] != null)
            {
                lastGame = new Game(data["LastGame"]);
            }

            if (data["UserPic"] != null)
            {
                UserPic = "http://retroachievements.org";
                UserPic += data["UserPic"].ToString();
            }

            return true;
        }

        /// <summary>
        /// Fetches user's avatar.
        /// </summary>
        /// <returns>true on completion</returns>
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

        /// <summary>
        /// Sub-method of fetchUserData. Fetches user's data.
        /// </summary>
        /// <returns>Current user's data</returns>
        public dynamic fetchData()
        {
            // Determine URL
            string url = Requests.Users.getUserSummary();

            // Fetch user JSON data
            string jsonString = Requests.FetchJSON(url);

            // Deserialize JSON into object
            return JsonConvert.DeserializeObject(jsonString);
        }

        /// <summary>
        /// Fetches current user's data.
        /// </summary>
        public void fetchUserData()
        {
            dynamic data = fetchData();

            if (data == null)
            {
                MessageBox.Show("No response from server.\n\n1. Check your internet connection/firewall.\n2. Check your credentials in the Settings tab.", "No response", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

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

            if (data["RecentAchievements"] != null)
            {
                // Init list
                RecentAchievements = new List<Achievement>();

                foreach (JToken gameSet in data["RecentAchievements"])
                {
                    foreach (JToken set in gameSet.Children())
                    {
                        foreach (JToken ach in set)
                        {
                            foreach (JToken woah in ach)
                            {
                                RecentAchievements.Add(new Achievement(woah));
                            }
                        }                        
                    }
                }
            }
        }
    }
}

