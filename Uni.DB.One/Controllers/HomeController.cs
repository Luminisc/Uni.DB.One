using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Uni.DB.One.DataAccess;
using Uni.DB.One.Models;

namespace Uni.DB_Task.Controllers
{
    public class HomeController : Controller
    {
        static MongoClient client;
        static IMongoDatabase database;
        UserManager<IdentityUser> _userManager;

        public HomeController(UserManager<IdentityUser> manager)
        {
            _userManager = manager;
            if (client == null)
                client = new MongoClient("mongodb://localhost:27017");
            if (database == null)
                database = client.GetDatabase("SteamDB");
            //var collection = database.GetCollection<ShoppingCart>("ShoppingCart").Find(x=>x.UserID == );
            //Statics.ShoppingCart = collection.AsQueryable().Select(x => x).ToList();
            //var userID = await _userManager.GetUserAsync(HttpContext.User);


        }

        public async Task<IActionResult> Index()
        {
            var collection = database.GetCollection<GameInfo>("Games").AsQueryable();
            var store = collection.Select(x => new GameInfo()
            {
                AppId = x.AppId,
                Name = x.Name,
                LogoUrl = x.LogoUrl,
                IconUrl = x.IconUrl
            }).ToList();
            return View("Index", new GamesStoreViewModel() { Games = store });
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

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

            //var app = database.GetCollection<GameInfo>("Games").FindSync($"{{AppId = {appid}}}").FirstOrDefault();

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
                Price = data["price_overview"]["final"].Value<int>()
            };

            JArray screenshots = data["screenshots"] as JArray;
            List<ScreenshotInfo> screens = new List<ScreenshotInfo>();

            foreach (var scr in screenshots)
            {
                screens.Add(scr.ToObject<ScreenshotInfo>());
            }
            model.Screens = screens;
            return View(model);
        }

        public async Task<IActionResult> AddGameToCart(string appid)
        {
            var gameInfo = Get($"https://store.steampowered.com/api/appdetails?appids={appid}&cc=ru&l=ru");
            JObject jgames = JsonConvert.DeserializeObject<JObject>(gameInfo);
            if (jgames[appid]["success"].Value<bool>() == false)
            {
                return Error("О такой игре у нас нет информации, но вы держитесь и всего хорошего.");
            }

            var games = database.GetCollection<GameInfo>("Games").Find(x => x.AppId == appid).ToList();
            var game = games.Select(x => new GameInfo()
            {
                AppId = x.AppId,
                Name = x.Name,
                LogoUrl = x.LogoUrl,
                IconUrl = x.IconUrl
            }).First();

            var item = new ShoppingCartItem()
            {
                Name = jgames[appid]["data"]["name"].ToString(),
                Price = jgames[appid]["data"]["price_overview"]["final"].Value<int>() / 100,
                PathThumbnail = game.GetLogoUrl
            };
            var user = await _userManager.GetUserAsync(HttpContext.User);
            ShoppingCartDb.AddItem(user, item);

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> RemoveGameFromCart(string id)
        {
            ShoppingCartDb.RemoveItem(await User(), ObjectId.Parse(id));
            return RedirectToAction("Index", "Home");
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
