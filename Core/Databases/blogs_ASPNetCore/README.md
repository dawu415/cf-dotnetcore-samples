# Cloud Foundry MySql Service Sample Code
This sample code demonstrates the basics of connecting to a MySQL Database and in
particular to a MySQL service.

# Running the sample application
Step 1:
```
Edit create-service.sh and ensure that the MySql service and plan matches those on the cf market (run the command: cf m)
```

Step 2:
```
Run ./create-service.sh
```

Step 3: 
```
Operations:
  A. <route>  : Display the contents of the database
  B. HTTP post <route>/Home/addBlog with the paramter 'blogUrl' having the value <urlstring>: Add a new blog entry to the database.
```

# Important Tips and Notes

1. Database Hostname must be fully qualified. This is very important. If not adhered, you will receive connection exceptions.
2. When setting up an new database, ensure that you run dotnet ef migrations add <name> . This will connect to the database server and generate a template
   snapshot code (in Migrations directory) of the models in the Models directory. You will need this template to initialise a new database. Note that you may not have access to the 
   database on CF as it could be on its own private network. So either find a database server you can access or spin one up locally.
   Thereafter, have some database migration/initialisation task app, or utilise the Database.Migrate(), as was done so in this sample. 
   For safety, the latter approach is probably best avoided in production.
3. Database.Migrate() was used instead of EnsureCreated/EnsureDeleted because of compatibility. These two sets of method apis are mutually exclusive. i.e., either run migrate
   or the Ensure* methods.  Do not mix them.   Ensure* methods create a database on the fly without migrations and prevent future migrations.  So these 
   are better for testing purposes only.   Further, Ensure* methods may not work properly on CF. 


