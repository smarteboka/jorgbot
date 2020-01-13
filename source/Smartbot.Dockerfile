FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY Smartbot.sln ./Smartbot.sln

COPY src/Smartbot/Smartbot.csproj ./src/Smartbot/Smartbot.csproj
COPY src/Smartbot.Web/Smartbot.Web.csproj ./src/Smartbot.Web/Smartbot.Web.csproj
COPY src/Smartbot.Data/Smartbot.Data.csproj ./src/Smartbot.Data/Smartbot.Data.csproj
COPY src/Slackbot.Net.Extensions.Smartbot.Worker/Slackbot.Net.Extensions.Smartbot.Worker.csproj ./src/Slackbot.Net.Extensions.Smartbot.Worker/Slackbot.Net.Extensions.Smartbot.Worker.csproj
COPY src/Slackbot.Net.Extensions.Smartbot.Endpoints/Slackbot.Net.Extensions.Smartbot.Endpoints.csproj ./src/Slackbot.Net.Extensions.Smartbot.Endpoints/Slackbot.Net.Extensions.Smartbot.Endpoints.csproj
COPY src/Slackbot.Net.Extensions.Smartbot.SharedWorkers/Slackbot.Net.Extensions.Smartbot.SharedWorkers.csproj ./src/Slackbot.Net.Extensions.Smartbot.SharedWorkers/Slackbot.Net.Extensions.Smartbot.SharedWorkers.csproj


COPY test/Smartbot.Tests/Smartbot.Tests.csproj ./test/Smartbot.Tests/Smartbot.Tests.csproj
COPY test/Smartbot.Web.Tests/Smartbot.Web.Tests.csproj ./test/Smartbot.Web.Tests/Smartbot.Web.Tests.csproj

RUN dotnet restore Smartbot.sln

# Copy everything else and build
COPY . ./
RUN dotnet publish src/Smartbot/Smartbot.csproj -c Release -o /app/out/smartbot
RUN dotnet publish src/Smartbot.Web/Smartbot.Web.csproj -c Release -o /app/out/smartbot.web

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /smartbot
COPY --from=build-env /app/out/smartbot .
WORKDIR /smartbot.web
COPY --from=build-env /app/out/smartbot.web .
WORKDIR /
