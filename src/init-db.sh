#!/bin/bash

sleep 20

/opt/mssql-tools/bin/sqlcmd -S mssql -U sa -P SqlServer2022! -Q "
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'MovieListDb')
BEGIN
    CREATE DATABASE MovieListDb;
END;
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'Hangfire')
BEGIN
    CREATE DATABASE Hangfire;
END;"
