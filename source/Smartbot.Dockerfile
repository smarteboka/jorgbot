FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY Smartbot.sln ./Smartbot.sln

COPY src/samples/Smartbot/Smartbot.csproj ./src/samples/Smartbot/Smartbot.csproj
COPY src/samples/Smartbot.Web/Smartbot.Web.csproj ./src/samples/Smartbot.Web/Smartbot.Web.csproj
COPY src/samples/Smartbot.Utilities/Smartbot.Utilities.csproj ./src/samples/Smartbot.Utilities/Smartbot.Utilities.csproj

COPY src/Slackbot.Net/Slackbot.Net.csproj ./src/Slackbot.Net/Slackbot.Net.csproj

COPY test/Smartbot.Tests/Smartbot.Tests.csproj ./test/Smartbot.Tests/Smartbot.Tests.csproj
COPY test/Slackbot.Net.Tests/Slackbot.Net.Tests.csproj ./test/Slackbot.Net.Tests/Slackbot.Net.Tests.csproj

RUN dotnet restore Smartbot.sln

# Copy everything else and build
COPY . ./
RUN dotnet publish src/samples/Smartbot/Smartbot.csproj -c Release -o /app/out/smartbot
RUN dotnet publish src/samples/Smartbot.Web/Smartbot.Web.csproj -c Release -o /app/out/smartbot.web

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/runtime:2.2
WORKDIR /smartbot
COPY --from=build-env /app/out/smartbot .

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/runtime:2.2
WORKDIR /smartbot.web
COPY --from=build-env /app/out/smartbot.web .
