VERSION=$(dotnet-gitversion | jq -r '.FullSemVer')
INFOVERSION=$(dotnet-gitversion | jq -r '.InformationalVersion')
SEMVER=$(dotnet-gitversion | jq -r '.SemVer')
echo $VERSION
echo $INFOVERSION
echo $SEMVER
docker build -f Smartbot.Dockerfile -t registry.digitalocean.com/johnkors/smartbot:$SEMVER -t registry.digitalocean.com/johnkors/smartbot:latest --rm --build-arg VERSION=$VERSION --build-arg INFOVERSION=$INFOVERSION . 
docker push registry.digitalocean.com/johnkors/smartbot:$SEMVER
