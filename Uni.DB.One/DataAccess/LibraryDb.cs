using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Uni.DB.One.Models;

namespace Uni.DB.One.DataAccess
{
    public class LibraryDb
    {
        protected static string collectionName = "Libraries";
        public static GamesLibrary Get(IdentityUser user)
        {
            return DbStatics.Database.GetCollection<GamesLibrary>(collectionName).Find(x => x.UserID == user.Id).FirstOrDefault();
        }

        public static void AddGames(IdentityUser user, IEnumerable<GamesLibraryItem> items)
        {
            var collection = DbStatics.Database.GetCollection<GamesLibrary>(collectionName);
            var library = collection.Find(x => x.UserID == user.Id).FirstOrDefault();
            if (library == null)
            {
                collection.InsertOne(new GamesLibrary() { UserID = user.Id, Items = new List<GamesLibraryItem>() });
                library = collection.Find(x => x.UserID == user.Id).FirstOrDefault();
                if (library == null) throw new Exception("Adding library failed.");
            }

            library.Items.AddRange(items);
            var update = Builders<GamesLibrary>.Update.Set(x => x.Items, library.Items);

            collection.FindOneAndUpdate<GamesLibrary>(
                x => x.UserID == library.UserID,
                update,
                new FindOneAndUpdateOptions<GamesLibrary, GamesLibrary>() { IsUpsert = true, ReturnDocument = ReturnDocument.After }
                );

        }
    }
}
