using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Uni.DB.One.Models
{
    public class ShoppingCart
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string UserID { get; set; }
        public List<ShoppingCartItem> Items { get; set; } = new List<ShoppingCartItem>();
    }
}
