using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Uni.DB.One.Models.User
{
    public class ProfileFriendsViewModel
    {
        public Profile Profile { get; set; }
        public bool CanFriendship { get; set; }

        public List<Profile> Friends { get; set; }
    }
}
