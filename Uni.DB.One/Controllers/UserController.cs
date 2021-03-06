﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Uni.DB.One.DataAccess;
using Uni.DB.One.Models.User;

namespace Uni.DB.One.Controllers
{
    public class UserController : Controller
    {
        UserManager<IdentityUser> _userManager;

        public UserController(UserManager<IdentityUser> manager)
        {
            _userManager = manager;
        }

        public async Task<IActionResult> Index()
        {
            return await Profile((await User())?.Id);
        }

        public async Task<IActionResult> Profile(string userId)
        {
            if (userId == null)
                return RedirectToAction("Index", "Home");

            var profile = UserDb.GetUser(await User(), false);
            if (profile == null)
                return RedirectToAction("Index", "Home");

            bool canFriendship = userId != (await User())?.Id;

            ProfileViewModel model = new ProfileViewModel()
            {
                Profile = profile,
                CanFriendship = canFriendship
            };
            return View("Profile", model);
        }

        public async Task<IActionResult> Friends(string userId)
        {
            if (userId == null)
                return RedirectToAction("Index", "Home");

            var profile = UserDb.GetUser(await User(), false);
            if (profile == null)
                return RedirectToAction("Index", "Home");

            var user = await User();
            bool canFriendship = user != null && userId != user.Id;
            
            ProfileFriendsViewModel model = new ProfileFriendsViewModel()
            {
                Profile = profile,
                CanFriendship = canFriendship,
                Friends = UserDb.GetFriends(user).ToList()
            };
            return View("Friends", model);
        }

        public async Task<IActionResult> Achievements(string userId)
        {
            if (userId == null)
                return RedirectToAction("Index", "Home");

            var profile = UserDb.GetUser(await User(), false);
            if (profile == null)
                return RedirectToAction("Index", "Home");

            bool canFriendship = userId != (await User())?.Id;

            ProfileViewModel model = new ProfileViewModel()
            {
                Profile = profile,
                CanFriendship = canFriendship
            };
            return View("Achievements", model);
        }

        public async Task<IActionResult> ChangeName(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                var user = await User();
                user.UserName = name;
                UserDb.ChangeName(user, name);
                await _userManager.UpdateAsync(user);
            }
            return await Profile((await User())?.Id);
        }

        protected async Task<IdentityUser> User() => await _userManager.GetUserAsync(HttpContext.User);
    }
}