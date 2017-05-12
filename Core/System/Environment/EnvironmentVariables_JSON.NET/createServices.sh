
cf cups Service1 -p '{"username":"admin","password":"pa55woRD"}'
cf cups Service2 -p '{"username":"dave","password":"something"}'
cf cups Service3 -p '{"username":"john","password":"foobar"}'

cf bind-service EnvironmentVariables Service1
cf bind-service EnvironmentVariables Service2
cf bind-service EnvironmentVariables Service3

cf create-service p-redis shared-vm myredis-service
cf create-service p-redis shared-vm myredis_xyz-service

cf bind-service EnvironmentVariables myredis-service
cf bind-service EnvironmentVariables myredis_xyz-service

cf restage EnvironmentVariables
