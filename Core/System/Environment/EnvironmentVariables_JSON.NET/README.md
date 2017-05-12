# Cloud Foundry Environment Variables Sample Application
This application demonstrates how to access environment variables within an application using JSON.NET
This utilises a helper class, CFEnvironmentVariables, to cleanly access (or get access to) the environment variables. 
In addition, it also shows an implementation approach to using dependency injection.

# Getting started for the first time

1. Ensure that p-redis is available in the market place of your CF. If p-redis is not available, you can use something different.
   Modify the createService.sh script and modify the HomeController.cs's method VCAP_SERVICES_REDIS_SERVICE_CREDENTIALS - 
   so that it points to the updated service name.
2. Run ./gocf.sh            -- This will build the source, publish and push the publish content to Cloud Foundry
3. Run ./createServices.sh  -- This will create and bind the associated services to the target app
4. Test by pointing your browser to each of the pages, for example,  <route>/Home/VCAP_SERVICES_USER_PROVIDED_SERVICE_CREDENTIALS

