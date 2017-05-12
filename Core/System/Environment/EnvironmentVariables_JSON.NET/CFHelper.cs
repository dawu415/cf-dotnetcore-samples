/*
    David Wu, 2017
*/
using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace CFHelper
{
    public class CFEnvironmentVariables
    {
        private IConfigurationRoot _configuration { get; set; }
        public dynamic vcap_services_data { get; set; }
        public dynamic vcap_application_data {get; set; }

        public CFEnvironmentVariables(IConfigurationRoot configuration)
        {
            _configuration = configuration;

            var raw_vcap_app = _configuration["VCAP_APPLICATION"];
            var raw_vcap_services = _configuration["VCAP_SERVICES"];

            // If there's a vcap services entry, parse to a dynamic JObject;
            if (raw_vcap_services != null)
                vcap_services_data = JObject.Parse(raw_vcap_services);

            // If there's a vcap application entry, parse to a dynamic JObject;
            if (raw_vcap_app != null)
                vcap_application_data = JObject.Parse(raw_vcap_app);
        }

        public dynamic getInfoForUserProvidedService(string serviceName)
        {
            // Try to access the user-provided service.
            // Unfortunately, we can't do the dot notation here, since user-provided would be
            // an invalid property name.
            var upsArray = vcap_services_data["user-provided"];
            
            if (upsArray != null)
            {
                foreach(var ups in upsArray)
                    if(ups.name == serviceName)
                        return ups;
            }
            
            return null;
        }

        public dynamic getInfoForService(string serviceTypeName, string serviceInstanceName = "")
        {
            var serviceArray = vcap_services_data[serviceTypeName];

            if (serviceArray != null)
            {
                // If serviceInstanceName is empty, just return the first element of our services info array
                if (serviceInstanceName == "")
                {
                    return serviceArray[0];
                }

                foreach(var service in serviceArray)
                    if(service.name == serviceInstanceName)
                        return service;
            }

            return null;
        }
    }
}