./dotnetgo.sh
cf push redis_app_A -p pub -b dotnet_core_buildpack
cf push redis_app_B -p pub -b dotnet_core_buildpack
