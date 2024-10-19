#!/bin/bash

# Start SQL Server in the background
/opt/mssql/bin/sqlservr &

# Wait for SQL Server to start
sleep 20

# Install SqlPackage
mkdir /opt/sqlpackage
curl -o /tmp/sqlpackage.zip https://go.microsoft.com/fwlink/?linkid=2215461
unzip /tmp/sqlpackage.zip -d /opt/sqlpackage
chmod +x /opt/sqlpackage/sqlpackage

# Deploy the DACPAC
/opt/sqlpackage/sqlpackage /Action:Publish \
    /SourceFile:/data/StrudelShop.Database.dacpac \
    /TargetServerName:localhost \
    /TargetDatabaseName:StrudelShop \
    /TargetUser:sa \
    /TargetPassword:$SA_PASSWORD \
    /Quiet

# Keep the container running
wait
