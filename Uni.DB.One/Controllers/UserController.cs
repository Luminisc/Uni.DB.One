using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Uni.DB.One.DataAccess;

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

            return View("Profile", profile);
        }


        protected async Task<IdentityUser> User() => await _userManager.GetUserAsync(HttpContext.User);
    }
}