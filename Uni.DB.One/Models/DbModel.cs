using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Uni.DB.One.Models
{
    public class DbModel
    {
        [BsonId]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
    }
}
