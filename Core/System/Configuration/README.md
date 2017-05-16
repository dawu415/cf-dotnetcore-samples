# Cloud Foundry Configuration Sample Application
This sample application demonstrates the setting and use of configuration from:

1) The appsettings.json
2) Environment Variables
3) A Configuration Server

# Running this sample application for the first time
In order for this sample application to work, a configuration server is required.
Ensure that 

1) Setup a git repository, to hold the configuration files.
2) Update createservice.sh by replacing 'http://example.com/config' with the git rep address from step 1.
3) Check-in into the git repo from step one, the files in the directory ./ConfigFilesToBeOnConfigServer
4) Run ./gocf.sh 
5) Run ./createservice.sh

# Switching between development and production configuration

1) Review the two configuration setting files in /ConfigFilesToBeOnConfigServer. Each is a configuration setting designated for Production and Development.  Our application picks this up by the name of our application plus the mode of the application determined by the environment variable ASPNETCORE_ENVIRONMENT.
2) Go to the endpoint <route>/Configuration/dump . This will output all the configuration of our application.
3) Check the key Configuration:IsProduction . The value should be false. 
4) Run the script switchToProduction.sh.  This will set the environment variable ASPNETCORE_ENVIRONMENT to Production.
5) Repeat Step 2.
6) Check the key Configuration:IsProduction . The value should be true.  
 
