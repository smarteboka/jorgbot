FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY Slackbot.Net.sln ./Slackbot.Net.sln

COPY src/samples/Smartbot/Smartbot.csproj ./src/samples/Smartbot/Smartbot.csproj
COPY src/samples/Smartbot.Web/Smartbot.Web.csproj ./src/samples/Smartbot.Web/Smartbot.Web.csproj
COPY src/samples/Smartbot.Utilities/Smartbot.Utilities.csproj ./src/samples/Smartbot.Utilities/Smartbot.Utilities.csproj

COPY src/Slackbot.Net/Slackbot.Net.csproj ./src/Slackbot.Net/Slackbot.Net.csproj
COPY src/Slackbot.Net.SlackClients/Slackbot.Net.SlackClients.csproj ./src/Slackbot.Net.SlackClients/Slackbot.Net.SlackClients.csproj
COPY src/Slackbot.Net.Endpoints/Slackbot.Net.Endpoints.csproj ./src/Slackbot.Net.Endpoints/Slackbot.Net.Endpoints.csproj

COPY src/Slackbot.Net.Abstractions.Handlers/Slackbot.Net.Abstractions.Handlers.csproj ./src/Slackbot.Net.Abstractions.Handlers/Slackbot.Net.Abstractions.Handlers.csproj
COPY src/Slackbot.Net.Abstractions.Hosting/Slackbot.Net.Abstractions.Hosting.csproj ./src/Slackbot.Net.Abstractions.Hosting/Slackbot.Net.Abstractions.Hosting.csproj
COPY src/Slackbot.Net.Abstractions.Publishers/Slackbot.Net.Abstractions.Publishers.csproj ./src/Slackbot.Net.Abstractions.Publishers/Slackbot.Net.Abstractions.Publishers.csproj

COPY src/Slackbot.Net.Extensions.Publishers.Logger/Slackbot.Net.Extensions.Publishers.Logger.csproj ./src/Slackbot.Net.Extensions.Publishers.Logger/Slackbot.Net.Extensions.Publishers.Logger.csproj
COPY src/Slackbot.Net.Extensions.Publishers.Slack/Slackbot.Net.Extensions.Publishers.Slack.csproj ./src/Slackbot.Net.Extensions.Publishers.Slack/Slackbot.Net.Extensions.Publishers.Slack.csproj



COPY test/Smartbot.Tests/Smartbot.Tests.csproj ./test/Smartbot.Tests/Smartbot.Tests.csproj
COPY test/Slackbot.Net.SlackClients.Tests/Slackbot.Net.SlackClients.Tests.csproj ./test/Slackbot.Net.SlackClients.Tests/Slackbot.Net.SlackClients.Tests.csproj
COPY test/Smartbot.Web.Tests/Smartbot.Web.Tests.csproj ./test/Smartbot.Web.Tests/Smartbot.Web.Tests.csproj

RUN dotnet restore Slackbot.Net.sln

# Copy everything else and build
COPY . ./
RUN dotnet publish src/samples/Smartbot/Smartbot.csproj -c Release -o /app/out/smartbot
RUN dotnet publish src/samples/Smartbot.Web/Smartbot.Web.csproj -c Release -o /app/out/smartbot.web

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/runtime:2.2
WORKDIR /smartbot
COPY --from=build-env /app/out/smartbot .
WORKDIR /smartbot.web
COPY --from=build-env /app/out/smartbot.web .
WORKDIR /
