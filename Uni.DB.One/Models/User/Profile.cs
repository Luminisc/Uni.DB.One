using System.Collections.Generic;

namespace Uni.DB.One.Models.User
{
    public class Profile : DbModel
    {
        public string Name { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public List<string> Friends { get; set; } = new List<string>();
        public byte[] UserImage { get; set; } = new byte[0];
    }
}
