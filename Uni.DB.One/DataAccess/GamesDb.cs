using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uni.DB.One.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.AspNetCore.Identity;

namespace Uni.DB.One.DataAccess
{
    public class GamesDb
    {
        public static List<GameInfo> GetAllGames()
        {
            return DbStatics.Database.GetCollection<GameInfo>("Games").AsQueryable().ToList();
        }

        public static List<GameInfo> GetUserGames(IdentityUser user)
        {
            return DbStatics.Database.GetCollection<GameInfo>("Libraries").Find(x => true).ToList();
        }

        public static GameInfo GetGameInfo(string appId)
        {
            return DbStatics.Database.GetCollection<GameInfo>("Games").Find(x => x.AppId == appId).FirstOrDefault();
        }
    }
}
