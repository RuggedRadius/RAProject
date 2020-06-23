using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RAProject.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace RAProject.Connection
{
    public static class Requests
    {
        /// <summary>
        /// Fetches JSON data from RetroAchievement servers.
        /// </summary>
        /// <param name="url">The URL of the request being made</param>
        /// <returns>JSON data</returns>
        public static string FetchJSON (string url)
        {
            // Fetch JSON string
            using (var webClient = new WebClient())
            {
                string jsonString = webClient.DownloadString(url);
                Console.WriteLine("Downloading JSON from " + url);
                return jsonString;
            }
        }

        /// <summary>
        /// Downloads an image from a given URL.
        /// </summary>
        /// <param name="url">URL to download image from</param>
        /// <returns>Image downloaded from given URL</returns>
        public static Image DownloadImageFromUrl(string url)
        {
            using (WebClient client = new WebClient())
            {
                // Download image data to byte[]
                byte[] imgData = client.DownloadData(url);


                using (var memStream = new MemoryStream(imgData))
                {
                    return Image.FromStream(memStream);
                }
            }
        }

        public struct Consoles
        {
            public static string getConsoleIDs()
            {
                return String.Format(
                    "{0}{1}?user={2}&key={3}&mode=json",
                    Constants.BASE_URL,
                    Constants.QueryTypes.WEB_CONSOLE_IDs,
                    Properties.Settings.Default.Credential_Username,
                    Properties.Settings.Default.Credential_APIKey
                    ); ;
            }
            public static string getConsoleGames(string consoleID)
            {
                return String.Format(
                    "{0}{1}?user={2}&key={3}&console={4}&mode=json",
                    Constants.BASE_URL,
                    Constants.QueryTypes.WEB_GAME_LIST,
                    Properties.Settings.Default.Credential_Username,
                    Properties.Settings.Default.Credential_APIKey,
                    consoleID
                    );
            }
        }
        public struct Games
        {
            public static string GetBoxArtURL(string gameID)
            {
                Console.WriteLine("Getting BoxArt URL for {0}", gameID);
                // SLOW!!!!!!!!!!!!!!!!! OVER 1000's of games
                string reqURL = Requests.Games.getGameInfoBasic(gameID);
                string json = FetchJSON(reqURL); // Here is slow
                dynamic data = JsonConvert.DeserializeObject(json);
                return string.Format("https://s3-eu-west-1.amazonaws.com/i.retroachievements.org{0}", data["ImageBoxArt"]);
            }
            public static string GetBoxArtURL(Game game)
            {
                string boxArtURL = "https://s3-eu-west-1.amazonaws.com/i.retroachievements.org";
                boxArtURL += game.ImageBoxArt;
                return boxArtURL;
            }
            public static string getGameInfoBasic(string gameID)
            {
                return String.Format(
                    "{0}{1}?user={2}&key={3}&game={4}&mode=json",
                    Constants.BASE_URL,
                    Constants.QueryTypes.WEB_GAME_INFO_BASIC,
                    Properties.Settings.Default.Credential_Username,
                    Properties.Settings.Default.Credential_APIKey,
                    gameID
                    );
            }

            public static string getGameInfoExtended(string gameID)
            {
                    return String.Format(
                        "{0}{1}?user={2}&key={3}&game={4}&mode=json",
                        Constants.BASE_URL,
                        Constants.QueryTypes.WEB_GAME_INFO_EXTENDED,
                        Properties.Settings.Default.Credential_Username,
                        Properties.Settings.Default.Credential_APIKey,
                        gameID
                        );
            }

            public static string getGameInfoExtendedProgress(string gameID)
            {
                return String.Format(
                        "{0}{1}?user={2}&key={3}&game={4}&mode=json",
                        Constants.BASE_URL,
                        Constants.QueryTypes.WEB_GAME_INFO_AND_PROGRESS,
                        Properties.Settings.Default.Credential_Username,
                        Properties.Settings.Default.Credential_APIKey,
                        gameID
                        );
            }
        }
        public struct Users
        {
            public static string getUserSummary()
            {
                return String.Format(
                    "{0}{1}?user={2}&key={3}&member={4}&results=10&mode=json",
                    Constants.BASE_URL,
                    Constants.QueryTypes.WEB_USER_SUMMARY,
                    Properties.Settings.Default.Credential_Username,
                    Properties.Settings.Default.Credential_APIKey,
                    Properties.Settings.Default.Credential_Username
                    );
            }
            public static string getTop10Users()
            {
                return String.Format(
                    "https://ra.hfc-essentials.com/{0}?user={1}&key={2}&mode=json",
                    Constants.QueryTypes.WEB_TOP_TEN_USERS,
                    Properties.Settings.Default.Credential_Username,
                    Properties.Settings.Default.Credential_APIKey
                    );
            }

            public static string getLastWeekAcheivements()
            {
                DateTime currentDate = DateTime.Today;
                DateTime lastWeek = DateTime.Today.AddDays(-7);

                DateTimeOffset currentOffset = new DateTimeOffset(currentDate);
                DateTimeOffset lastWeekOffset = new DateTimeOffset(lastWeek);

                var unixCurrent = currentOffset.ToUnixTimeSeconds();
                var unixLastWeek = lastWeekOffset.ToUnixTimeSeconds();

                return String.Format(
                    "https://ra.hfc-essentials.com/{0}?user={1}&key={2}&member={3}&startdate={4}&enddate={5}&mode=json",
                    Constants.QueryTypes.WEB_USER_USER_ACHIEVEMENTS_BY_DATE_RANGE,
                    Properties.Settings.Default.Credential_Username,
                    Properties.Settings.Default.Credential_APIKey,
                    "Adultery",
                    unixLastWeek,
                    unixCurrent
                    );
            }
        }  
    }
}
