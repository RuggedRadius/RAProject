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
        //public static async Task<JObject> FetchData(string gameID, string query)
        //{
        //    if (Utilities.InternetConnected())
        //    {
        //        try
        //        {
        //            // Fetch json string
        //            string jsonString = FetchJSON(requestURL(query, gameID));

        //            // Make JSON object from string                   
        //            return JsonConvert.DeserializeObject<dynamic>(jsonString);
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine("Error fetching data, check credentials.");
        //            Console.WriteLine(ex.ToString());
        //            //MessageBox.Show(
        //            //    "Error fetching data, check credentials.\r\n\r\nDetails:\r\n" + ex.ToString(),
        //            //    "Error fetching data",
        //            //    MessageBoxButtons.OK,
        //            //    MessageBoxIcon.Error
        //            //    );
        //            return null;
        //        }
        //    }
        //    else
        //    {
        //        Console.WriteLine("No internet connection, check connection or firewall.");
        //        //MessageBox.Show(
        //        //    "Check internet connection and/or firewall access",
        //        //    "Internet connection error",
        //        //    MessageBoxButtons.OK,
        //        //    MessageBoxIcon.Error
        //        //    );
        //    }
        //    return null;
        //}

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
        }
        public struct Users
        {
            public static string getUserSummary()
            {
                return String.Format(
                    "{0}{1}?user={2}&key={3}&member=Adultery&results=10&mode=json",
                    Constants.BASE_URL,
                    Constants.QueryTypes.WEB_USER_SUMMARY,
                    Properties.Settings.Default.Credential_Username,
                    Properties.Settings.Default.Credential_APIKey
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
        }




        // Too monolithic
        //public static string requestURL (string query, string param)
        //{
        //    switch (query)
        //    {                   

        //        case Constants.QueryTypes.WEB_GAME_LIST:


        //        case Constants.QueryTypes.WEB_GAME_INFO_BASIC:
        //            return String.Format(
        //                "{0}{1}?user={2}&key={3}&game={4}&mode=json",
        //                Constants.BASE_URL,
        //                query,
        //                Properties.Settings.Default.Credential_Username,
        //                Properties.Settings.Default.Credential_APIKey,
        //                param
        //                );

        //        case Constants.QueryTypes.WEB_GAME_INFO_EXTENDED:
        //            return String.Format(
        //                "{0}{1}?user={2}&key={3}&game={4}&mode=json",
        //                Constants.BASE_URL,
        //                query,
        //                Properties.Settings.Default.Credential_Username,
        //                Properties.Settings.Default.Credential_APIKey,
        //                param
        //                );

        //        case Constants.QueryTypes.WEB_GAME_INFO_AND_PROGRESS:
        //            return String.Format(
        //                "{0}{1}?user={2}&key={3}&game={4}&mode=json",
        //                Constants.BASE_URL,
        //                query,
        //                Properties.Settings.Default.Credential_Username,
        //                Properties.Settings.Default.Credential_APIKey,
        //                param
        //                );

        //        case Constants.QueryTypes.WEB_USER_RANK_AND_SCORE:
        //            //https://ra.hfc-essentials.com/user_rank.php?user=+YOUR_RA_USERNAME+&key=+YOUR_API_KEY+&game=3&member=Adultery&mode=json
        //            return String.Format(
        //                "{0}{1}?user={2}&key={3}&member=Adultery&mode=json",
        //                Constants.BASE_URL,
        //                query,
        //                Properties.Settings.Default.Credential_Username,
        //                Properties.Settings.Default.Credential_APIKey
        //                );

        //        case Constants.QueryTypes.WEB_USER_RECENTLY_PLAYED_GAMES:
        //            return null;

        //        case Constants.QueryTypes.WEB_USER_PROGRESS:
        //            return null;



        //        // looks like a useless feed
        //        case Constants.QueryTypes.WEB_USER_FEED:
        //            return null;

        //        // Dates are in UNIX format... whatever the fuck that is...
        //        case Constants.QueryTypes.WEB_USER_ACHIEVEMENTS_BY_DATE:
        //            //	https://ra.hfc-essentials.com/user_by_date.php?user=+YOUR_RA_USERNAME+&key=+YOUR_API_KEY+&member=Adultery&startdate=1576166048&enddate=1574956448&mode=json



        //            // do fancy magic here param to convert to the offset for date in unix format
        //            //....


        //            // Make UNIX time stamps for query
        //            Int32 unixTimestamp_Start = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 14))).TotalSeconds; // Set 2 two weeks? i think..
        //            Int32 unixTimestamp_End = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

        //            return String.Format(
        //            "{0}{1}?user={2}&key={3}&member=Adultery&startdate={4}&enddate={5}&mode=json",
        //            Constants.BASE_URL,
        //            query,
        //            Properties.Settings.Default.Credential_Username,
        //            Properties.Settings.Default.Credential_APIKey,
        //            unixTimestamp_Start,
        //            unixTimestamp_End
        //            );

        //        default:
        //            return null;
        //    }
        //}


        //public static string GetInGame(int id)
        //{
        //    string reqURL = requestURL(Constants.QueryTypes.WEB_GAME_INFO_BASIC, id.ToString());
        //    string json = FetchJSON(reqURL);

        //    // Init base of image url
        //    dynamic data = JsonConvert.DeserializeObject(json);
        //    string inGameArtURL = "https://s3-eu-west-1.amazonaws.com/i.retroachievements.org";
        //    inGameArtURL += data["ImageIngame"];
        //    Console.WriteLine("Fetching Screenshot URL: " + inGameArtURL);
        //    return inGameArtURL;
        //}
        //public static string GetTitleScreen(int id)
        //{
        //    string reqURL = requestURL(Constants.QueryTypes.WEB_GAME_INFO_BASIC, id.ToString());
        //    string json = FetchJSON(reqURL);

        //    // Init base of image url
        //    dynamic data = JsonConvert.DeserializeObject(json);
        //    string inGameArtURL = "https://s3-eu-west-1.amazonaws.com/i.retroachievements.org";
        //    inGameArtURL += data["ImageTitle"];
        //    Console.WriteLine("Fetching Screenshot URL: " + inGameArtURL);
        //    return inGameArtURL;
        //}






        //// For achievements
        //public static string GetImageURL(dynamic data, int imageSelection)
        //{
        //    // Init base of image url
        //    string coverImgURL = "https://s3-eu-west-1.amazonaws.com/i.retroachievements.org";
        //    Console.WriteLine("Fetching Image URL: " + coverImgURL);
        //    switch (imageSelection)
        //    {
        //        case 0:
        //            //coverImgURL += data["RecentlyPlayed"] .Value<JToken>("ImageIcon").ToString();
        //            //break;
        //        case 1:
        //            coverImgURL += data.Value<JToken>("ImageTitle").ToString();
        //            break;
        //        case 2:
        //            coverImgURL += data.Value<JToken>("ImageIngame").ToString();
        //            break;
        //        case 3:
        //            coverImgURL += data.Value<JToken>("ImageBoxArt").ToString();
        //            break;
        //    }
        //    return coverImgURL;
        //}

        //public static string GetImageURL_byGameObject(Game g, int imageSelection)
        //{
        //    // Init base of image url
        //    string imageURL = "https://s3-eu-west-1.amazonaws.com/i.retroachievements.org";

        //    string reqURL = requestURL(Constants.QueryTypes.WEB_GAME_INFO_BASIC, g.ID);
        //    string json = FetchJSON(reqURL);
        //    dynamic data = JsonConvert.DeserializeObject(json);

        //    //data[""]

        //    switch (imageSelection)
        //    {
        //        case 0:
        //            imageURL += g.imgIcon;
        //            imageURL += data["ImageIcon"];
        //            break;
        //        case 1:
        //            imageURL += g.imgTitleScreen;
        //            imageURL += data["ImageTitle"];
        //            break;
        //        case 2:
        //            imageURL += g.imgIngame;
        //            imageURL += data["ImageIngame"];
        //            break;
        //        case 3:
        //            imageURL += g.imgBoxArt;
        //            imageURL += data["ImageBoxArt"];
        //            break;
        //    }
        //    return imageURL;
        //}
    }
}
