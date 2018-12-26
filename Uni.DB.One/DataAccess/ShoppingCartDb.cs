using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Uni.DB.One.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Uni.DB.One.DataAccess
{
    public class ShoppingCartDb
    {
        protected static IMongoCollection<ShoppingCart> collection => DbStatics.Database.GetCollection<ShoppingCart>("ShoppingCart");

        public static ShoppingCart Get(IdentityUser user)
        {
            return collection.Find(x => x.UserID == user.Id).FirstOrDefault();
        }

        public static void AddItem(IdentityUser user, ShoppingCartItem item)
        {
            var cart = collection.Find(x => x.UserID == user.Id).FirstOrDefault();
            if (cart == null)
            {
                collection.InsertOne(new ShoppingCart() { UserID = user.Id, Items = new List<ShoppingCartItem>() });
                cart = collection.Find(x => x.UserID == user.Id).FirstOrDefault();
                if (cart == null) throw new Exception("Adding cart failed.");
            }
            cart.Items.Add(item);
            var update = Builders<ShoppingCart>.Update.Set(x => x.Items, cart.Items);

            collection.FindOneAndUpdate<ShoppingCart>(
                x => x.UserID == cart.UserID,
                update,
                new FindOneAndUpdateOptions<ShoppingCart, ShoppingCart>() { IsUpsert = true, ReturnDocument = ReturnDocument.After }
                );
        }

        public static void RemoveItem(IdentityUser user, ObjectId id)
        {
            var cart = collection.Find(x => x.UserID == user.Id).FirstOrDefault();
            if (cart == null) return;
            var item = cart.Items.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                cart.Items.Remove(item);
                var update = Builders<ShoppingCart>.Update.Set(x => x.Items, cart.Items);
                collection.FindOneAndUpdate(x => x.Id == cart.Id, update);
            }
        }

        public static void CleanCart(IdentityUser user)
        {
            var update = Builders<ShoppingCart>.Update.Set(x => x.Items, new List<ShoppingCartItem>());
            collection.FindOneAndUpdate(x => x.UserID == user.Id, update);
        }
    }
}
