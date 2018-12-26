using System.Collections.Generic;

namespace Uni.DB.One.Models
{
    public class ShoppingCart : DbModel
    {
        public string UserID { get; set; }
        public List<ShoppingCartItem> Items { get; set; } = new List<ShoppingCartItem>();
    }
}
