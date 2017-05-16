# Cloud Foundry .NET Steeltoe Redis Sample App
This application demonstrates the use of Redis with the Steeltoe connector.
Running the push script, gocf.sh, will push two instances with separate routes of this sample app. 
Both instances are connected to a single Redis cache service.

There are two main endpoints /Home/ReadCacheData and /Home/WriteCacheData, which reads from and writes to the redis cache, respectively.
Execute the /Home/WriteCacheData on one instance and execute /Home/ReadCacheData, to see the redis cache in action.

# What you will need to do to get started

1. Ensure your PCF supports the p-redis service.
2. run ``` ./gocf.sh ``` to create and push two instances to PCF.
3. run ``` ./createservice.sh ``` to create and bind the redis service to the two instances of this sample app.

# Test Operations 

1. On redis-app-a, execute the ReadCacheData. This will be blank
2. On redis-app-b, execute the WriteCacheData. This will write the server time into the cache.
3. Return to redis-app-a and execute ReadCacheData. The server time written by redis-app-b will be shown on the browser.
