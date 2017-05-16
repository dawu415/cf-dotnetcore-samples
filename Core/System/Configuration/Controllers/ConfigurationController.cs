using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Microsoft.Extensions.Configuration;

namespace Configuration.Controllers
{
    [Route("[controller]")]
    public class ConfigurationController : Controller
    {
        private IConfigurationRoot      _configuration { get; set; }
        // By configuring the services to use IConfigurationRoot in Startup.cs,
        // We can have a constructor that takes in an IConfigurationRoot.  This is 
        // automagically injected during the construction of the HomeController.
        public ConfigurationController(IConfigurationRoot configuration)
        {
            // Store the configuration object into _configuration, so that it can
            // be used throughout this class. 
            _configuration = configuration;
        }

        // GET Configuration/dump
        [HttpGet("dump")]
        public IEnumerable<KeyValuePair<string,string>> DumpConfiguration()
        {
            return _configuration.AsEnumerable();
        }

        // POST Configuration/refresh
        [HttpPost("refresh")]
        public void RefreshConfiguration()
        {
            _configuration.Reload();
        }
    }
}
