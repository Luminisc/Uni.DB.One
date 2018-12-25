using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Uni.DB.One.DataAccess
{
    public static class DbStatics
    {
        private static MongoClient _client = new MongoClient("mongodb://localhost:27017");
        private static IMongoDatabase _database;
        

        public static IMongoDatabase Database {
            get {
                if (_database == null)
                    _database = _client.GetDatabase("SteamDB");
                return _database;
            } }
    }
}
