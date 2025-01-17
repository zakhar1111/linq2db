docker run -d -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Password12!" -p 1433:1433 -h mssql --name=mssql iamrjindal/sqlserverexpress:latest
docker ps -a

echo "Waiting for SQL Server to accept connections"
set max=100
:repeat
set /a max=max-1
if %max% EQU 0 goto fail
echo pinging sql server
sleep 1
docker exec mssql sqlcmd -S localhost -U sa -P Password12! -Q "SELECT 1"
if %errorlevel% NEQ 0 goto repeat
echo "SQL Server is operational"

docker exec mssql sqlcmd -S localhost -U sa -P Password12! -Q "SELECT @@Version"
echo "create TestData"
docker exec mssql sqlcmd -S localhost -U sa -P Password12! -Q "CREATE DATABASE TestData;"
echo "create TestData2019"
REM both db and catalog are case-sensitive
docker exec mssql sqlcmd -S localhost -U sa -P Password12! -Q "CREATE DATABASE TestData2019 COLLATE Latin1_General_CS_AS;"
echo "create TestData2019SA"
docker exec mssql sqlcmd -S localhost -U sa -P Password12! -Q "CREATE DATABASE TestData2019SA;"
echo "create TestData2019FEC"
docker exec mssql sqlcmd -S localhost -U sa -P Password12! -Q "CREATE DATABASE TestData2019FEC;"
echo "create TestDataContained"
docker exec mssql sqlcmd -S localhost -U sa -P Password12! -Q "sp_configure 'contained database authentication', 1;"
docker exec mssql sqlcmd -S localhost -U sa -P Password12! -Q "RECONFIGURE;"
docker exec mssql sqlcmd -S localhost -U sa -P Password12! -Q "CREATE DATABASE TestDataContained CONTAINMENT = PARTIAL;"
echo "copy Northwind"
docker cp scripts/northwind.sql mssql:northwind.sql
echo "create Northwind"
docker exec mssql sqlcmd -S localhost -U sa -P Password12! -i northwind.sql
goto:eof

:fail
echo "Fail"
docker logs mssql
