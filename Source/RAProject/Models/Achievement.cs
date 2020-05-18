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
        [JsonProperty("NumAwarded")]            public string NumAwarded            { get; set; }
        [JsonProperty("NumAwardedHardcore")]    public string NumAwardedHardcore    { get; set; }
        [JsonProperty("Title")]                 public string Title                 { get; set; }
        [JsonProperty("Description")]           public string Description           { get; set; }
        [JsonProperty("Points")]                public string Points                { get; set; }
        [JsonProperty("TrueRatio")]             public string TrueRatio             { get; set; }
        [JsonProperty("Author")]                public string Author                { get; set; }
        [JsonProperty("DateModified")]          public string DateModified          { get; set; }
        [JsonProperty("DateCreated")]           public string DateCreated           { get; set; }
        [JsonProperty("BadgeName")]             public string BadgeName             { get; set; }
        [JsonProperty("DisplayOrder")]          public string DisplayOrder          { get; set; }
        [JsonProperty("MemAddr")]               public string MemAddr               { get; set; }
        [JsonProperty("DateEarned")]            public string DateEarned            { get; set; }
        [JsonProperty("DateEarnedHardCore")]    public string DateEarnedHardCore    { get; set; }

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
        public Image downloadBadge()
        {
            return Requests.DownloadImageFromUrl(GetBadgeUrl());
        }

    }
}
