﻿namespace Uni.DB.One.Models
{
    public class ShoppingCartItem : DbModel
    {
        public string AppId { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public string PathThumbnail { get; set; }
    }
}
