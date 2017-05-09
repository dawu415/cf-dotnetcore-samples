# Cloud Foundry-Base Sample Code
This sample code serves as a basic template to read in arguments via the command line and
treat them as configuration input.

This is required by Cloud Foundry to ensure that the listening port number PORT 
can be acquired and used by the .NET Core application.

# Applying the patch

CF-CommandLineConfiguration.patch is a patch that you can apply to your project:

Step 1: 
patch -p1 Program.cs CF-CommandLineConfiguration.patch

Step 2:
Either a) Add reference to Microsoft.Extensions.Configuration.Commandline via Visual Studio
or     b) With CLI, using the command dotnet add package Microsoft.Extensions.Configuration.Commandline  
