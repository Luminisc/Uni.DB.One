using System;

namespace Uni.DB.One.Models
{
    public class GamesLibraryItem : DbModel
    {
        public string AppId { get; set; }
        public string Name { get; set; }
        public string PathThumbnail { get; set; }
        public DateTime BuyDate { get; set; }
    }
}
