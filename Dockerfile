FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY source/JorgBot.sln ./source/JorgBot.sln
COPY source/src/JorgBot/JorgBot.csproj ./source/src/JorgBot/JorgBot.csproj
COPY source/test/JorgBot.Tests/JorgBot.Tests.csproj ./source/test/JorgBot.Tests/JorgBot.Tests.csproj
RUN dotnet restore source/JorgBot.sln

# Copy everything else and build
COPY . ./
RUN dotnet publish source/src/JorgBot/JorgBot.csproj -c Release -o /app/out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/runtime:2.2
WORKDIR /app
COPY --from=build-env /app/out .
CMD ["dotnet", "JorgBot.dll"]