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
        public dynamic vcap_application_data { get; set; }

        public CFEnvironmentVariables(IConfigurationRoot configuration)
        {
            _configuration = configuration;

            string raw_vcap_app = _configuration["VCAP_APPLICATION"];
            string raw_vcap_services = _configuration["VCAP_SERVICES"];

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
            if (Object.ReferenceEquals(null, vcap_services_data) == false)
            {
                var upsArray = vcap_services_data["user-provided"];

                if (upsArray != null && upsArray.HasValues)
                {
                    foreach (var ups in upsArray)
                        if (ups.name == serviceName)
                            return ups;
                }
            }

            return null;
        }

        public dynamic getInfoForService(string serviceTypeName, string serviceInstanceName = "")
        {

            if (Object.ReferenceEquals(null, vcap_services_data) == false)
            {
                dynamic serviceArray = vcap_services_data[serviceTypeName];

                if (serviceArray != null && serviceArray.HasValues)
                {
                    // If serviceInstanceName is empty, just return the first element of our services info array
                    if (serviceInstanceName == "")
                    {
                        return serviceArray[0];
                    }

                    foreach (var service in serviceArray)
                        if (service.name == serviceInstanceName)
                            return service;
                }
            }

            return null;
        }

        public string getConnectionStringForDbService(string serviceTypeName, string serviceInstanceName = "", IDbConnectionStringFormatter formatter = null)
        {
            string connectionString = "";

            if (Object.ReferenceEquals(null, vcap_services_data) == false)
            {
                dynamic serviceInfo = getInfoForService(serviceTypeName, serviceInstanceName);

                if (Object.ReferenceEquals(null, serviceInfo) == false)
                {
                    // Default to use a Basic MS SQL Server connection string, if our formatter was not specified.
                    if (formatter == null)
                        formatter = new BasicSQLServerConnectionStringFormatter();

                    var host = Convert.ToString(serviceInfo.credentials.hostname);
                    var username = Convert.ToString(serviceInfo.credentials.username);
                    var password = Convert.ToString(serviceInfo.credentials.password);
                    var port = Convert.ToString(serviceInfo.credentials.port);
                    var databaseName = Convert.ToString(serviceInfo.credentials.name);

                    connectionString = formatter.Format(host, username, password, databaseName, port);
                }
            }
            return connectionString;
        }
    }

    // Connection String Formatting helper classes
    public interface IDbConnectionStringFormatter
    {
        string Format(string host, string username, string password, string databaseName, string port = null);
    }

    public class BasicMySQLConnectionStringFormatter : IDbConnectionStringFormatter
    {
        public string Format(string host, string username, string password, string databaseName, string port = null)
        {
            string connectionString = $"Server={host};Database={databaseName};Uid={username};Pwd={password};";

            if (port != null)
                connectionString += $"Port={port};";


            return connectionString;
        }
    }

    public class BasicSQLServerConnectionStringFormatter : IDbConnectionStringFormatter
    {
        public string Format(string host, string username, string password, string databaseName, string port = null)
        {
            return $"Server={host};Database={databaseName};User ID={username};Password={password};";
        }
    }
}