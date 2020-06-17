using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RAProject.Connection;
using System;
using System.Drawing;

namespace RAProject.Models
{
    [Serializable]
    public class Achievement
    {
        [JsonProperty("ID")]                    public string ID                    { get; set; }
        [JsonProperty("Title")] public string Title { get; set; }
        [JsonProperty("Description")] public string Description { get; set; }
        [JsonProperty("DateEarned")] public string DateEarned { get; set; }
        [JsonProperty("DateEarnedHardCore")] public string DateEarnedHardCore { get; set; }
        [JsonProperty("NumAwarded")]            public string NumAwarded            { get; set; }
        [JsonProperty("NumAwardedHardcore")]    public string NumAwardedHardcore    { get; set; }
        [JsonProperty("Points")]                public string Points                { get; set; }
        [JsonProperty("TrueRatio")] private string TrueRatio             { get; set; }
        [JsonProperty("Author")]                public string Author                { get; set; }
        [JsonProperty("DateModified")]          private string DateModified          { get; set; }
        [JsonProperty("DateCreated")] private string DateCreated           { get; set; }
        [JsonProperty("BadgeName")] private string BadgeName             { get; set; }
        [JsonProperty("DisplayOrder")] private string DisplayOrder          { get; set; }
        [JsonProperty("MemAddr")] private string MemAddr               { get; set; }


        public Image badge;
        
        public Achievement (JToken jo)
        {
            ID =                    (string) jo["ID"];
            NumAwarded =            (string) jo["NumAwarded"];
            NumAwardedHardcore =    (string) jo["NumAwardedHardcore"];
            Title =                 (string) jo["Title"];
            Description =           (string) jo["Description"];
            Points =                (string) jo["Points"];
            TrueRatio =             (string) jo["TruePoints"];
            Author =                (string) jo["Author"];
            DateModified =          (string) jo["DateModified"];
            DateCreated =           (string) jo["DateCreated"];
            BadgeName =             (string) jo["BadgeName"];
            DisplayOrder =          (string) jo["DisplayOrder"];
            MemAddr =               (string) jo["MemAddr"];


            if ((string) jo["DateEarned"] != null)
            {
                DateEarned = (string) jo["DateEarned"];

                if ((string)jo["DateEarnedHardCore"] != null)
                {
                    DateEarnedHardCore = (string)jo["DateEarnedHardCore"];
                }
            }
        }

        public Image getBadge()
        {
            if (badge == null)
            {
                Console.WriteLine("Downloading badge for " + BadgeName);
                badge = fetchBadge();
            }
            return badge;
        }

        public string GetBadgeUrl()
        {
            if (this.DateEarned != null)
            {
                return String.Format("http://retroachievements.org/Badge/{0}.png", this.BadgeName);
            }
            else
            {
                return String.Format("http://retroachievements.org/Badge/{0}_lock.png", this.BadgeName);
            }
        }
        public Image fetchBadge()
        {
            return Requests.DownloadImageFromUrl(GetBadgeUrl());
        }

    }
}
