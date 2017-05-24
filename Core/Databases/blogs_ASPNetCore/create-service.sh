./dotnetgo.sh
cf push --no-start
cf create-service p-mysql 100mb mysql-service
cf bind-service blogsASPNetCore mysql-service
cf start blogsASPNetCore
