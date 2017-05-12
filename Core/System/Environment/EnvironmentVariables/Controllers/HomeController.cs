using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using CFHelper;

namespace EnvironmentVariables.Controllers
{
    public class HomeController : Controller
    {
        private IConfigurationRoot      _configuration { get; set; }
        private CFEnvironmentVariables  _cfEnvVars {get; set; }
        // By configuring the services to use IConfigurationRoot in Startup.cs,
        // We can have a constructor that takes in an IConfigurationRoot.  This is 
        // automagically injected during the construction of the HomeController.
        public HomeController(IConfigurationRoot configuration)
        {
            // Store the configuration object into _configuration, so that it can
            // be used throughout this class. 
            _configuration = configuration;

            _cfEnvVars = new CFEnvironmentVariables(configuration);
        }

        // Perform a dump of the environment variables that are contained in the 
        // IConfigurationRoot object.
        // Hint: Use some an in-browser JSON formatter to see pretty print of the JSON
        // e.g. https://github.com/callumlocke/json-formatter
        public IEnumerable<KeyValuePair<string, string>> DumpEnvironmentVariables()
        {
            return _configuration.AsEnumerable();
        }

        // **** VCAP_SERVICES ****

        // Do a raw dump of the VCAP_SERVICES directly from configuration.
        // Hint: Use some an in-browser JSON formatter to see pretty print of the JSON
        // e.g. https://github.com/callumlocke/json-formatter
        public string VCAP_SERVICES()
        {
            return _configuration["VCAP_SERVICES"];
        }

        // Output the credentials and info of a known user provided service
        // Hint: Use some an in-browser JSON formatter to see pretty print of the JSON
        // e.g. https://github.com/callumlocke/json-formatter
        public dynamic VCAP_SERVICES_USER_PROVIDED_SERVICE_CREDENTIALS()
        {
            var upsInfo = _cfEnvVars.getInfoForUserProvidedService("Service2");

            // upsInfo elements can be access like this: 
            //   upsInfo.credentials.password  or  upsInfo.credentials.username;

            return upsInfo;
        }

        // Output the credentials and info of a service
        // Hint: Use some an in-browser JSON formatter to see pretty print of the JSON
        // e.g. https://github.com/callumlocke/json-formatter
        public dynamic VCAP_SERVICES_REDIS_SERVICE_CREDENTIALS()
        {
            var serviceInfo = _cfEnvVars.getInfoForService("p-redis","myredis_xyz-service");

            // serviceInfo elements can be access like this, e.g., 
            //  serviceInfo.credentials.password or serviceInfo.credentials.username;

            return serviceInfo;
        }

        // **** VCAP_APPLICATION ****

        // Do a raw dump of the VCAP_APPLICATION directly from configuration.
        // Hint: Use some an in-browser JSON formatter to see pretty print of the JSON
        // e.g. https://github.com/callumlocke/json-formatter
        public string VCAP_APPLICATION()
        {
            return _configuration["VCAP_APPLICATION"];
        }

        // Do an output of the VCAP_APPLICATION.application_name.
        // Hint: Use some an in-browser JSON formatter to see pretty print of the JSON
        // e.g. https://github.com/callumlocke/json-formatter
        public dynamic VCAP_APPLICATION_APP_NAME()
        {
            return _cfEnvVars.vcap_application_data.application_name;
        }

        // Do an output of the VCAP_APPLICATION.Limits.
        // Hint: Use some an in-browser JSON formatter to see pretty print of the JSON
        // e.g. https://github.com/callumlocke/json-formatter
        public dynamic VCAP_APPLICATION_LIMITS()
        {
            return _cfEnvVars.vcap_application_data.limits;
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
