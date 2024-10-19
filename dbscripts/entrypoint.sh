#!/bin/bash

# Start SQL Server in the background
/opt/mssql/bin/sqlservr &

# Wait for SQL Server to start
sleep 20

# Deploy the DACPAC using SqlPackage
sqlpackage /Action:Publish \
    /SourceFile:/data/StrudelShop.Database.dacpac \
    /TargetServerName:localhost \
    /TargetDatabaseName:StrudelShop \
    /TargetUser:sa \
    /TargetPassword:$SA_PASSWORD \
    /TargetTrustServerCertificate:True

# Keep the container running
wait
