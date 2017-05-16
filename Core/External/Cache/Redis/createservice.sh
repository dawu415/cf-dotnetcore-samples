cf create-service p-redis shared-vm redissamplecache

cf bind-service redis_app_A redissamplecache
cf bind-service redis_app_B redissamplecache

cf restage redis_app_A
cf restage redis_app_B

