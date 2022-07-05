[![Build status](https://dev.azure.com/APS-SD-Stewards/APS-SD/_apis/build/status/proscrumdev.battleship-dotnetcore-CI)](https://dev.azure.com/APS-SD-Stewards/APS-SD/_build/latest?definitionId=28)

[![build and test dotnet](https://github.com/proscrumdev/battleship-dotnet/actions/workflows/buildpipeline.yaml/badge.svg)](https://github.com/proscrumdev/battleship-dotnet/actions/workflows/buildpipeline.yaml)

# Battleship .NET
A simple game of Battleship, written in C# based on .NET 6. The purpose of this repository is to serve as an entry point into coding exercises and it was especially created for scrum.orgs Applying Professional Scrum for Software Development course (www.scrum.org/apssd). The code in this repository is unfinished by design.

# Build the application 
In the root folder of the application there is a Battleship.sln which can be used to build all projects by executing
```bash
dotnet build 
```

# Run the application

## Running locally
You neet to have the .NET6 runtime installed on your local machine
https://www.microsoft.com/net/download

You can now run the game with
```bash
dotnet run --project .\Battleship.Ascii\Battleship.Ascii.csproj
```


## Running within a docker container
You can easily run the game within a docker container.

Change into the Battleship folder

```bash
docker run -it -v ${PWD}:/battleship mcr.microsoft.com/dotnet/sdk:6.0 bash
```

This starts a new container and maps your current folder on your host machine as the folder battleship in your container and opens a bash console. In bash then change to the folder battleship/Battleship.Ascii and run
```bash
dotnet run 
```


## Running within a DevContainer
DevContainers are a way to run your development environment inside of an Docker container. 
The container contains all frameworks, tools etc. to develop and run the application.
If you open the folder of this repo in VS Code, it asks if you want to run this insiede a DevContainer. 
If you agree, VS Code will run inside this container with the needed extensions and frameworks.

See [Developing inside a Container](https://code.visualstudio.com/docs/remote/containers) for more information on local container development with VS Code.

Prerequisites:
* Docker
* Visual Studio Code


## Running within GitHub CodeSpaces
You can also use GitHub CodeSpaces to run the development environment completely in a browser. 

See [Introduction to CodeSpaces](https://docs.github.com/en/codespaces) for more information about using GitHub Codespaces.

# Execute Tests
You can run tests on the console by using
```bash
dotnet test 
```

If you want to run tests within VSCode, you can install the [.NET Core Test Explorer Extension](https://marketplace.visualstudio.com/items?itemName=formulahendry.dotnet-test-explorer). In this case make sure, you set the Propery "Test Project Path" in the extension setting to this value:
```bash
**/*[Test|ATDD]*/*.csproj
```

# Telemetry data
This application is collecting telemetry data with Microsoft Application Insights.
For more details see https://docs.microsoft.com/en-us/azure/azure-monitor/app/console.

To send the telemetry data to a specific instance of Application Insights, the connection string has to be adjusted in ApplicationInsights.config.
