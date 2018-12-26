using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uni.DB.One.Models.User;

namespace Uni.DB.One.DataAccess
{
    public class UserDb
    {
        protected static IMongoCollection<Profile> collection => DbStatics.Database.GetCollection<Profile>("Users");
        public static Profile GetUser(IdentityUser iuser, bool createIfNotExist = true)
        {
            if (iuser == null)
                return null;

            var user = collection.Find(x => x.UserId == iuser.Id).FirstOrDefault();
            if (user == null && createIfNotExist)
            {
                var newuser = new Profile()
                {
                    Name = iuser.UserName,
                    UserId = iuser.Id,
                    Email = iuser.Email
                };
                collection.InsertOne(newuser);
                user = collection.Find(x => x.UserId == iuser.Id).FirstOrDefault();
                if (user == null) throw new Exception("Adding user failed.");
            }

            iuser.UserName = user.Name;
            iuser.Email = user.Email;
            return user;
        }

        public static IEnumerable<Profile> GetFriends(IdentityUser iuser)
        {
            if (iuser == null)
                return null;

            var user = collection.Find(x => x.UserId == iuser.Id).FirstOrDefault();
            if (user == null)
                return null;

            return from friend in collection.AsQueryable()
                   where user.Friends.Contains(friend.UserId)
                   select friend;
        }

        public static void ChangeName(IdentityUser iuser, string name)
        {
            var update = Builders<Profile>.Update.Set(x => x.Name, name);
            collection.FindOneAndUpdate(x => x.UserId == iuser.Id, update);
        }
    }
}
