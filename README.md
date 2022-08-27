# Code Army WebApi (.NET 6.0)

## Prerequisites and Installation

- You will need to install dotnet 6.0 (https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- You will need to install SQL server (SQL server runs only on Windows OS, if your machine has a different OS install docker and then install SQL server in docker [Helpful guide for Mac users](https://setapp.com/how-to/install-sql-server))
- You will need to install [JobeServer](https://github.com/Khalil-baydoun/CustomJobeServer) which only runs on Linux, to use it with a different OS install docker and then pull the [JobeInABox](https://hub.docker.com/r/trampgeek/jobeinabox/) Docker image and run it (Follow the README for more info)
- To work with your local DB you can install SSMS or Azure Data Studio which will allow you to work with the database of this app using a very powerful UI
- You can develop on Visual Studio or Visual Studio Code, in case you chose to work with VSCode you can install the C# extension which offers a very powerful tooling while working with .NET projects

This project uses multiple connection strings to connect to the database, the jobe server and the service bus. It's not advised to push this data to git as it will be in the history and can be exploited. For a better and more secure development practice, it's advised to use environment variables to define these connection params. 
Since the app settings of a .NET project is defined in JSON, and the environment variables are key value pairs the environment variables must be defined as follows:
For the following appsettings.json:
json
```
{
  "JwtSettings": {
    "Secret": "TOP_SECRET_THAT_SHOULD_NOT_BE_STORED_IN_CODE",
    "Issuer": "AubCodingPlatform",
    "Audience": "Coders",
    "AccessTokenExpiryTimeInSeconds": 86400
  },

  "DatabaseSettings": {
    "ConnectionString": "SECRET"
  },

  "JobeServerSettings": {
    "Endpoint": "SECRET"
  },

  "ServiceBusSettings": {
    "ConnectionString": "SECRET",
    "SubmissionQueueName": "submissionqueue"
  },

  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

- For a windows user:
  - Key is `DatabaseSettings:ConnectionString` Value is `YOUR_DB_CONNECTION_STRING`
  - Key is `JobeServerSettings:Endpoint` Value is `YOUR_JOBE_SERVER_ENDPOINT`
  - Key is `ServiceBusSettings:ConnectionString` Value is `YOUR_SB_CONNECTION_STRING`
- For a mac user:
You can use the export command to set the environment variables for your current terminal session, to permanently set the environment variables use the same command in your terminal profile (.bash_profile or .zshrc)
  - Key is `DatabaseSettings__ConnectionString` Value is `YOUR_DB_CONNECTION_STRING`
  - Key is `JobeServerSettings__Endpoint` Value is `YOUR_JOBE_SERVER_ENDPOINT`
  - Key is `ServiceBusSettings__ConnectionString` Value is `YOUR_SB_CONNECTION_STRING`

Example: `export DatabaseSettings__ConnectionString="YOUR_DB_CONNECTION_STRING"`


## Usage
You should start by running the following command: `dotnet restore`. This will restore all the project dependencies and it's the only needed command to start using the project. 
The restore command needs to be ran once from the root of the project and then you can `cd` into `WebApi` and execute the following command: `dotnet run`.

## Using the API
### Authentication
To authenticate, make an API call to `POST /api/authentication/login` which returns an authentication token which will allow you to use the other APIs, the json body must contain two fields: `Email` and `Password`.
You can also use the token to check the role of the user for that token, to do that make a call to `GET /api/authentication/getrole` which will return the role of that user.

### **APIs**
#### User
- `POST /api/user`: Creates a new user with a specific role, the user can then login using its new credentials.
- `GET /api/user/{email}`: Returns the user using its email
- `DELETE /api/user/{email}`: Deleted the user from the system.

#### Course
- `GET /api/course/{id}`
- `POST /api/course`
- `POST /api/course/adduser`
- `POST /api/course/removeusers`

#### Problem Set
- `GET /api/problemset/{id}`
- `POST /api/problemset`
- `POST /api/problemset/addproblem`

#### Problem
- `POST /api/problem`
- `GET /api/problem/{id}`
- `PUT /api/problem/{id}`

#### Submit
- `POST /api/submission`
- `GET /api/submission?offset={INT}&limit={INT}`

#### Tests
- `POST /api/test`
- `GET /api/test/{problemId}`
- `POST /api/test/uploadTests`

#### Statistics
- `GET /api/statistics/user`
- `GET /api/statistics/course/{courseId}`
- `GET /api/statistics/problem/{problemId}`


