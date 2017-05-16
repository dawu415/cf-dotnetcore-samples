using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// Include this for the IDistributed Cache Usage
using Microsoft.Extensions.Caching.Distributed;

// For Encoding
using System.Text;

namespace Redis.Controllers
{
    public class HomeController : Controller
    {

        private IDistributedCache _cache;
        public HomeController(IDistributedCache cache)
        {
            _cache = cache;
        }

        public string WriteCacheData()
        {
            var serverStartTimeString = DateTime.Now.ToString();
            byte[] val = Encoding.UTF8.GetBytes(serverStartTimeString);
            var cacheEntryOptions = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(5));
            _cache.Set("lastServerStartTime", val, cacheEntryOptions);
            return "Server Start Time Written: " + serverStartTimeString;
        }

        public string ReadCacheData()
        {
            var bytes = _cache.Get("lastServerStartTime");
            string output = "";

            if (bytes != null)
            {
                string lastServerStartTime = Encoding.UTF8.GetString(bytes);
                output = "lastServerStartTime:  " + lastServerStartTime;
            }

            return output;
        }

        public IActionResult Index()
        {
            return View();
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

        public IActionResult Error()
        {
            return View();
        }
    }
}
