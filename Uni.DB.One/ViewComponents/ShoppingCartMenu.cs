using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uni.DB.One.DataAccess;
using Uni.DB.One.Models;

namespace Uni.DB.One.ViewComponents
{
    public class ShoppingCartMenuViewComponent : ViewComponent
    {
        UserManager<IdentityUser> _userManager;

        public ShoppingCartMenuViewComponent(UserManager<IdentityUser> manager)
        {
            _userManager = manager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var shoppingCart = ShoppingCartDb.Get(user) ?? new ShoppingCart();
            return View(shoppingCart);
        }
    }
}
