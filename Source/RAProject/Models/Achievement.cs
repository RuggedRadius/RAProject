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
        [JsonProperty("DateEarned")] public string DateEarned { get; set; }
        [JsonProperty("DateEarnedHardCore")] public string DateEarnedHardcore { get; set; }


        public Image badge;
        
        public Achievement (JToken token)
        {
            ID =                    (string) token["ID"];
            NumAwarded =            (string) token["NumAwarded"];
            NumAwardedHardcore =    (string) token["NumAwardedHardcore"];
            Title =                 (string) token["Title"];
            Description =           (string) token["Description"];
            Points =                (string) token["Points"];
            TrueRatio =             (string) token["TruePoints"];
            Author =                (string) token["Author"];
            DateModified =          (string) token["DateModified"];
            DateCreated =           (string) token["DateCreated"];
            BadgeName =             (string) token["BadgeName"];
            DisplayOrder =          (string) token["DisplayOrder"];
            MemAddr =               (string) token["MemAddr"];

            
            if ((string)token["DateEarned"] != null)
            {
                DateEarned = (string)token["DateEarned"];

                if ((string)token["DateEarnedHardCore"] != null)
                {
                    DateEarnedHardcore = (string)token["DateEarnedHardCore"];
                }
            }
            else if ((string)token["DateAwarded"] != null)
            {
                DateEarned = (string)token["DateAwarded"];

                if ((string)token["HardcoreAchieved"] != null)
                {
                    DateEarnedHardcore = (string)token["HardcoreAchieved"]; // Not tested!!
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
        
        private Image fetchBadge()
        {
            return Requests.DownloadImageFromUrl(GetBadgeUrl());
        }

    }
}
