using System.Collections.Generic;

namespace Uni.DB.One.Models
{
    public class GameInfo : DbModel
    {
        public string AppId { get; set; }
        public string Name { get; set; }
        public string IconUrl { get; set; }
        public string LogoUrl { get; set; }
        public List<Review> Reviews { get; set; }

        public string GetLogoUrl => $"http://media.steampowered.com/steamcommunity/public/images/apps/{AppId}/{LogoUrl}.jpg";
        public string GetIconUrl => $"http://media.steampowered.com/steamcommunity/public/images/apps/{AppId}/{IconUrl}.jpg";
    }

    public class Review : DbModel
    {
        public string Author { get; set; }
        public string ReviewText { get; set; }
    }
}
