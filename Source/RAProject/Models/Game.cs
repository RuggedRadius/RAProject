using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RAProject.Connection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit;

namespace RAProject.Models
{
    [Serializable]
    public class Game
    {
        [JsonProperty("ID")] public string ID { get; set; }
        [JsonProperty("Title")] public string Title { get; set; }
        [JsonProperty("ForumTopicID")] private string ForumTopicID { get; set; }
        [JsonProperty("ConsoleID")] public string ConsoleID { get; set; }
        [JsonProperty("ConsoleName")] public string ConsoleName { get; set; }
        [JsonProperty("Flags")] private object Flags { get; set; }
        [JsonProperty("ImageIcon")] public string ImageIcon { get; set; }
        [JsonProperty("GameIcon")] public string GameIcon { get; set; }
        [JsonProperty("ImageTitle")] public string ImageTitle { get; set; }
        [JsonProperty("ImageIngame")] public string ImageIngame { get; set; }
        [JsonProperty("ImageBoxArt")] public string ImageBoxArt { get; set; }
        [JsonProperty("Publisher")] public string Publisher { get; set; }
        [JsonProperty("Developer")] public string Developer { get; set; }
        [JsonProperty("Genre")] public string Genre { get; set; }
        [JsonProperty("Released")] public string Released { get; set; }
        [JsonProperty("GameTitle")] public string GameTitle { get; set; }
        [JsonProperty("Console")] public string Console { get; set; }
        [JsonProperty("Achievements")] public List<Achievement> Achievements { get; set; }
        [JsonProperty("LastPlayed")] public string LastPlayed { get; set; }
        public int AchievementCount { get; set; }

        public bool hasAchievements = true;

        public Image imgTitleScreen;
        public Image imgIngame;
        public Image imgBoxArt;
        public Image imgIcon;

        #region Constructors
        public Game()
        {
            Achievements = new List<Achievement>();
        }
        public Game(JToken j)
        {
            // Assign Values
            if (j["Title"] != null)
                Title = (string)j["Title"];
            if (j["ID"] != null)
                ID = (string)j["ID"];
            if (j["ConsoleID"] != null)
                ConsoleID = (string)j["ConsoleID"];
            if (j["ConsoleName"] != null)
                Console = (string)j["ConsoleName"];
            if (j["ForumTopicID"] != null)
                ForumTopicID = (string)j["ForumTopicID"];
            if (j["ConsoleName"] != null)
                ConsoleName = (string)j["ConsoleName"];
            if (j["Flags"] != null)
                Flags = (string)j["Flags"];
            if (j["ImageIcon"] != null)
                ImageIcon = (string)j["ImageIcon"];
            if (j["GameIcon"] != null)
                GameIcon = (string)j["GameIcon"];
            if (j["ImageTitle"] != null)
                ImageTitle = (string)j["ImageTitle"];
            if (j["ImageIngame"] != null)
                ImageIngame = (string)j["ImageIngame"];
            if (j["ImageBoxArt"] != null)
                ImageBoxArt = (string)j["ImageBoxArt"];
            if (j["Publisher"] != null)
                Publisher = (string)j["Publisher"];
            if (j["Developer"] != null)
                Developer = (string)j["Developer"];
            if (j["Genre"] != null)
                Genre = (string)j["Genre"];
            if (j["Released"] != null)
                Released = (string)j["Released"];

            // Create new list of achievements
            Achievements = new List<Achievement>();

            // Extra attributes from other queries
            if (j["LastPlayed"] != null)
            {
                LastPlayed = (string)j["LastPlayed"];
            }
            if (j["GameID"] != null)
            {
                ID = (string)j["GameID"];
            }
        }
        #endregion  

        public void DownloadAchievements()
        {
            string url = Requests.Games.getGameInfoExtendedProgress(this.ID);
            string json = Requests.FetchJSON(url);
            dynamic data = JsonConvert.DeserializeObject(json);

            if (!hasAchievements)
                return;

            if (data != null)
            {
                if (data["Achievements"] == null)
                {
                    this.hasAchievements = false;
                }
                else
                {
                    if (this.Achievements == null)                    
                        this.Achievements = new List<Achievement>();

                    foreach (JProperty a in data["Achievements"])
                    {
                        Achievement ac = new Achievement(a.Value);
                        this.Achievements.Add(ac);
                    }
                }                
            }

            // Update achievements count
            AchievementCount = Achievements.Count;
        }

        public void downloadGameData()
        {
            string url = Requests.Games.getGameInfoExtendedProgress(ID);// requestURL(Constants.QueryTypes.WEB_GAME_INFO_AND_PROGRESS, this.ID);
            string json = Requests.FetchJSON(url);
            dynamic data = JsonConvert.DeserializeObject(json);


            if (data != null)
            {
                if (this.Console == null)
                {
                    if (data["Console"] == null)
                    {
                        this.Console = "<No data>";
                    }
                    else
                    {
                        this.Console = data["Console"];
                    }

                    System.Console.WriteLine(this.Title + " console updated.");
                }
                if (this.Publisher == null)
                {
                    if (data["Publisher"] == null)
                    {
                        this.Publisher = "<No data>";
                    }
                    else
                    {
                        this.Publisher = data["Publisher"];
                    }
                    System.Console.WriteLine(this.Title + " publisher updated.");
                }
                if (this.Developer == null)
                {
                    if (data["Developer"] == null)
                    {
                        this.Developer = "<No data>";
                    }
                    else
                    {
                        this.Developer = data["Developer"];
                    }
                    System.Console.WriteLine(this.Title + " developer updated.");
                }
                if (this.Released == null)
                {
                    if (data["Released"] == null)
                    {
                        this.Released = "<No data>";
                    }
                    else
                    {
                        this.Released = data["Released"];
                    }
                    System.Console.WriteLine(this.Title + " release date updated.");
                }
                if (this.imgBoxArt == null)
                {
                    if (data["ImageBoxArt"] != null)
                    {
                        this.imgBoxArt = fetchImage_BoxArt(data);
                    }
                }
                if (this.imgTitleScreen == null)
                {
                    if (data["ImageTitle"] != null)
                    {
                        try
                        {
                            this.imgTitleScreen = fetchImage_TitleScreen(data);
                        }
                        catch (Exception ex)
                        {
                            System.Console.WriteLine("Error downloading Title screen for {1}: {0}", ex.Message, this.Title);
                        }
                    }
                }
                if (this.imgIngame == null)
                {
                    if (data["ImageIngame"] != null)
                    {
                        try
                        {
                            this.imgIngame = fetchImage_InGame(data);
                        }
                        catch (Exception ex)
                        {
                            System.Console.WriteLine("Error downloading in-game image for {1}: {0}", ex.Message, this.Title);
                        }
                    }
                    if (this.Achievements == null)
                    {
                        if (data["Achievements"] == null)
                        {
                            this.hasAchievements = false;
                        }
                        else
                        {
                            this.Achievements = new List<Achievement>();

                            foreach (JProperty a in data["Achievements"])
                            {
                                Achievement ac = new Achievement(a.Value);
                                this.Achievements.Add(ac);
                            }
                        }
                    }
                }
                else
                {
                    System.Console.WriteLine("Error: Could not download data.");
                }
            }
        }

        public void downloadAllImages()
        {
            System.Console.WriteLine("Downloading Box Art for {0}", Title);
            string reqURL = Requests.Games.getGameInfoBasic(ID);
            string json = Requests.FetchJSON(reqURL); // Here is slow
            dynamic data = JsonConvert.DeserializeObject(json);
            string url;

            url = string.Format("https://s3-eu-west-1.amazonaws.com/i.retroachievements.org{0}", data["ImageBoxArt"]);
            imgBoxArt = Requests.DownloadImageFromUrl(url);

            url = string.Format("https://s3-eu-west-1.amazonaws.com/i.retroachievements.org{0}", data["ImageInGame"]);
            imgIngame = Requests.DownloadImageFromUrl(url);

            url = string.Format("https://s3-eu-west-1.amazonaws.com/i.retroachievements.org{0}", data["ImageTitle"]);
            imgTitleScreen = Requests.DownloadImageFromUrl(url);
        }

        public Task downloadAsyncBoxArt()
        {
            return Task.Factory.StartNew(() => {

                string boxArtURL = null;

                // Get image URL if required
                if (ImageBoxArt == null || string.IsNullOrEmpty(ImageBoxArt))
                {
                    System.Console.WriteLine("Fetching Box Art URL for {0}", Title);

                    string reqURL = Requests.Games.getGameInfoBasic(ID);
                    string json = Requests.FetchJSON(reqURL); // Here is slow
                    dynamic data = JsonConvert.DeserializeObject(json);

                    boxArtURL = string.Format("https://s3-eu-west-1.amazonaws.com/i.retroachievements.org{0}", data["ImageBoxArt"]);
                }
                else
                {
                    boxArtURL = ImageBoxArt;
                }

                // Download image
                using (WebClient client = new WebClient())
                {
                    System.Console.WriteLine("Downloading Box Art for {0}", Title);

                    byte[] imgData = client.DownloadData(boxArtURL);
                    using (var memStream = new MemoryStream(imgData))
                    {
                        imgBoxArt = Image.FromStream(memStream);
                    }
                }
            });
        }

        public Image downloadImage_BoxArt()
        {
            System.Console.WriteLine("Downloading Box Art for {0}", Title);
            string reqURL = Requests.Games.getGameInfoBasic(ID);
            string json = Requests.FetchJSON(reqURL); // Here is slow
            dynamic data = JsonConvert.DeserializeObject(json);
            string boxArtURL = string.Format("https://s3-eu-west-1.amazonaws.com/i.retroachievements.org{0}", data["ImageBoxArt"]);
            imgBoxArt = Requests.DownloadImageFromUrl(boxArtURL);
            return imgBoxArt;
        }
        public Image downloadImage_InGame() {
            System.Console.WriteLine("Downloading InGame Art for {0}", Title);
            string reqURL = Requests.Games.getGameInfoBasic(ID);
            string json = Requests.FetchJSON(reqURL); // Here is slow
            dynamic data = JsonConvert.DeserializeObject(json);

            string boxArtURL = string.Format("https://s3-eu-west-1.amazonaws.com/i.retroachievements.org{0}", data["ImageInGame"]);
            imgIngame = Requests.DownloadImageFromUrl(boxArtURL);
            return imgIngame;
        }
        public Image downloadImage_TitleScreen()
        {
            System.Console.WriteLine("Downloading TitleScreen Art for {0}", Title);
            string reqURL = Requests.Games.getGameInfoBasic(ID);
            string json = Requests.FetchJSON(reqURL); // Here is slow
            dynamic data = JsonConvert.DeserializeObject(json);
            string boxArtURL = string.Format("https://s3-eu-west-1.amazonaws.com/i.retroachievements.org{0}", data["ImageTitle"]);
            imgTitleScreen = Requests.DownloadImageFromUrl(boxArtURL);
            return imgTitleScreen;
        }


        public Image fetchImage_BoxArt(dynamic data)
        {
            System.Console.WriteLine("Downloading Box Art for {0}", Title);
            string boxArtURL = string.Format("https://s3-eu-west-1.amazonaws.com/i.retroachievements.org{0}", data["ImageBoxArt"]);
            imgBoxArt = Requests.DownloadImageFromUrl(boxArtURL);
            return imgBoxArt;
        }
        public Image fetchImage_TitleScreen(dynamic data)
        {
            System.Console.WriteLine("Downloading Title Screen Art for {0}", Title);
            string boxArtURL = string.Format("https://s3-eu-west-1.amazonaws.com/i.retroachievements.org{0}", data["ImageTitle"]);
            imgTitleScreen = Requests.DownloadImageFromUrl(boxArtURL);
            return imgTitleScreen;
        }
        public Image fetchImage_InGame(dynamic data)
        {
            System.Console.WriteLine("Downloading InGame Art for {0}", Title);
            string boxArtURL = "https://s3-eu-west-1.amazonaws.com/i.retroachievements.org" + data["ImageIngame"];
            imgIngame = Requests.DownloadImageFromUrl(boxArtURL);
            return imgIngame;
        }
    }
}
