using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RAProject.Connection;
using RAProject.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAProject.Utilities
{
    class FetchImage
    {
        // SLOW METHOD OVER LOTS OF GAMES
        //public static string GetImageURL_BoxArtbyID(int id)
        //{
        //    string reqURL = Requests.Games.getGameInfoBasic(id.ToString());

        //    // SLOW!!!!!!!!!!!!!!!!! OVER 1000's of games
        //    string json = Requests.FetchJSON(reqURL);

        //    // Init base of image url
        //    dynamic data = JsonConvert.DeserializeObject(json);
        //    string boxArtURL = "https://s3-eu-west-1.amazonaws.com/i.retroachievements.org";
        //    boxArtURL += data["ImageBoxArt"];
        //    Console.WriteLine("Fetching Box Art URL: " + boxArtURL);
        //    return boxArtURL;
        //}

        //public static string GetImageURL_InGamebyID(int id)
        //{
        //    string reqURL = Requests.requestURL(Constants.QueryTypes.WEB_GAME_INFO_BASIC, id.ToString());
        //    string json = Requests.FetchJSON(reqURL);

        //    // Init base of image url
        //    dynamic data = JsonConvert.DeserializeObject(json);
        //    string inGameArtURL = "https://s3-eu-west-1.amazonaws.com/i.retroachievements.org";
        //    inGameArtURL += data["ImageIngame"];
        //    Console.WriteLine("Fetching Screenshot URL: " + inGameArtURL);
        //    return inGameArtURL;
        //}

        //public static string GetImageURL_TitleScreenbyID(int id)
        //{
        //    string reqURL = Requests.requestURL(Constants.QueryTypes.WEB_GAME_INFO_BASIC, id.ToString());
        //    string json = Requests.FetchJSON(reqURL);

        //    // Init base of image url
        //    dynamic data = JsonConvert.DeserializeObject(json);
        //    string inGameArtURL = "https://s3-eu-west-1.amazonaws.com/i.retroachievements.org";
        //    inGameArtURL += data["ImageTitle"];
        //    Console.WriteLine("Fetching Screenshot URL: " + inGameArtURL);
        //    return inGameArtURL;
        //}

        //public static string GetBadgeUrlFromAchievement(Achievement achievement)
        //{
        //    if (achievement.DateEarned != null)
        //    {
        //        return String.Format("http://retroachievements.org/Badge/{0}.png", achievement.BadgeName);
        //    }
        //    else
        //    {
        //        return String.Format("http://retroachievements.org/Badge/{0}_lock.png", achievement.BadgeName);
        //    }
        //}

        //public static Image DownloadImageFromUrl(string url)
        //{
        //    // Create new empty image
        //    Image img = null;

        //    // Request the image through webRequest
        //    var webRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(url);
        //    webRequest.AllowWriteStreamBuffering = true;
        //    webRequest.Timeout = 30000;

        //    // Create webreponse to stream from
        //    System.Net.WebResponse webResponse = webRequest.GetResponse();

        //    // Create stream from webResponse
        //    Stream stream = webResponse.GetResponseStream();

        //    // Write stream to image
        //    img = Image.FromStream(stream);

        //    // Close webResponse
        //    webResponse.Close();

        //    // Return the new image
        //    return img;
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
        //        //coverImgURL += data["RecentlyPlayed"] .Value<JToken>("ImageIcon").ToString();
        //        //break;
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

        //    string reqURL = Requests.requestURL(Constants.QueryTypes.WEB_GAME_INFO_BASIC, g.ID);
        //    string json = Requests.FetchJSON(reqURL);
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
