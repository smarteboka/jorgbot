FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY source/Smartbot.sln ./source/Smartbot.sln

COPY source/src/Smartbot/Smartbot.csproj ./source/src/Smartbot/Smartbot.csproj
COPY source/src/Smartbot.Utilities/Smartbot.Utilities.csproj ./source/src/Smartbot.Utilities/Smartbot.Utilities.csproj

COPY source/src/Slackbot.Net/Slackbot.Net.csproj ./source/src/Slackbot.Net/Slackbot.Net.csproj

COPY source/test/Smartbot.Tests/Smartbot.Tests.csproj ./source/test/Smartbot.Tests/Smartbot.Tests.csproj
COPY source/test/Slackbot.Net.Tests/Slackbot.Net.Tests.csproj ./source/test/Slackbot.Net.Tests/Slackbot.Net.Tests.csproj

RUN dotnet restore source/Smartbot.sln

# Copy everything else and build
COPY . ./
RUN dotnet publish source/src/Smartbot/Smartbot.csproj -c Release -o /app/out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/runtime:2.2
WORKDIR /app
COPY --from=build-env /app/out .
CMD ["dotnet", "Smartbot.dll"]