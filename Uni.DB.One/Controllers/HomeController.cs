using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Uni.DB.One.DataAccess;
using Uni.DB.One.Models;

namespace Uni.DB.One.Controllers
{
    public class HomeController : Controller
    {
        UserManager<IdentityUser> _userManager;

        public HomeController(UserManager<IdentityUser> manager)
        {
            _userManager = manager;
        }

        public async Task<IActionResult> Index()
        {
            var user = UserDb.GetUser(await User());
            var store = GamesDb.GetAllGames().ToList();
            return View("Index", new GamesStoreViewModel() { Games = store });
        }

        public IActionResult About()
        {
            ViewData["Message"] = "О нас";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string reason = "")
        {
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Reason = reason });
        }

        public async Task<IActionResult> App(string appid)
        {
            if (string.IsNullOrWhiteSpace(appid))
                return await Index();

            var gameInfo = Get($"https://store.steampowered.com/api/appdetails?appids={appid}&cc=ru&l=ru");
            JObject jgames = JsonConvert.DeserializeObject<JObject>(gameInfo);
            if (jgames[appid]["success"].Value<bool>() == false)
            {
                return Error("О такой игре у нас нет информации, но вы держитесь и всего хорошего.");
            }
            JObject data = jgames[appid]["data"] as JObject;
            var model = new GameDetailsViewModel()
            {
                AppId = appid,
                Description = data["detailed_description"].ToString(),
                Name = data["name"].ToString(),
                Price = data["price_overview"]["final"].Value<int>(),
                Reviews = data["reviews"].ToString()
            };

            JArray screenshots = data["screenshots"] as JArray;
            List<ScreenshotInfo> screens = new List<ScreenshotInfo>();

            foreach (var scr in screenshots)
            {
                screens.Add(scr.ToObject<ScreenshotInfo>());
            }
            model.Screens = screens;

            JArray jachievements = data["achievements"]["highlighted"] as JArray;
            List<AchievementViewModel> achievements = jachievements.AsQueryable()
                .Select(x => x.ToObject<AchievementViewModel>())
                .ToList();
            model.Achievements = achievements;

            return View(model);
        }

        public async Task<IActionResult> ShoppingCart()
        {
            var cart = ShoppingCartDb.Get(await User());

            return View(cart);
        }

        public async Task<IActionResult> Library()
        {
            var library = LibraryDb.Get(await User()) ?? new GamesLibrary();

            return View(library);
        }

        public async Task<IActionResult> AddGameToCart(string appid)
        {
            var gameInfo = Get($"https://store.steampowered.com/api/appdetails?appids={appid}&cc=ru&l=ru");
            JObject jgames = JsonConvert.DeserializeObject<JObject>(gameInfo);
            if (jgames[appid]["success"].Value<bool>() == false)
            {
                return Error("О такой игре у нас нет информации, но вы держитесь и всего хорошего.");
            }

            var game = GamesDb.GetGameInfo(appid);

            var item = new ShoppingCartItem()
            {
                AppId = appid,
                Name = jgames[appid]["data"]["name"].ToString(),
                Price = jgames[appid]["data"]["price_overview"]["final"].Value<int>() / 100,
                PathThumbnail = game.GetLogoUrl
            };
            ShoppingCartDb.AddItem(await User(), item);

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> RemoveGameFromCart(string id, bool gotocart = false)
        {
            ShoppingCartDb.RemoveItem(await User(), ObjectId.Parse(id));
            if (gotocart)
                return RedirectToAction("ShoppingCart", "Home");
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> CartCheckout()
        {
            var cart = ShoppingCartDb.Get(await User());
            var libraryItems = cart.Items.Select(x => new GamesLibraryItem() { Name = x.Name, BuyDate = DateTime.Today, PathThumbnail = x.PathThumbnail, AppId = x.AppId });
            LibraryDb.AddGames(await User(), libraryItems);
            ShoppingCartDb.CleanCart(await User());
            return RedirectToAction("Library", "Home");
        }


        protected string Get(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
        protected async Task<IdentityUser> User() => await _userManager.GetUserAsync(HttpContext.User);


    }

}
