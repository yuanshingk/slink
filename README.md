[![Actions Status](https://github.com/yuanshingk/slink/workflows/.NET%20Core/badge.svg)](https://github.com/yuanshingk/slink/actions)

# slink
This is a service that provide url shortening functionality

[Demo Site](https://slinkapp.azurewebsites.net/)

# Prerequisite
Ensure the following are installed to run the projects:
* .NET Core 3.0 framework
* Visual Studio 2019 (Recommended)
* SQL Server

# Local Setup
#### Database
1. Create a new database with name SLinkDB
1. Run the script in [DbScripts/Table_CreateUrlRecords.sql](https://github.com/yuanshingk/slink/blob/master/DbScripts/Table_Create_UrlRecords.sql) to create the UrlRecords table
1. Update your connection string in [appsettings.Development.json](https://github.com/yuanshingk/slink/blob/master/SLink/appsettings.Development.json)
```
"SLINKDB_CONNECTIONSTRING": "Data Source=localhost;Initial Catalog=SLinkDB;
```

# User API
Web API Swagger: https://slinkapp.azurewebsites.net/swagger/index.html

| resource                  | http method | description                              |
|:--------------------------|:------------|:-----------------------------------------|
| `/api/shortlink`          | POST        | returns shortened URL containing hash id |
| `/api/shortlink/{hashid}` | GET         | returns original URL of the hash id      |
| `/{hashid}`               | GET         | redirects page to original URL           |
