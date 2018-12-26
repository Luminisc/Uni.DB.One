using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Uni.DB.One.Models
{
    public class GamesLibrary : DbModel
    {
        public string UserID { get; set; }
        public List<GamesLibraryItem> Items { get; set; } = new List<GamesLibraryItem>();
    }
}
