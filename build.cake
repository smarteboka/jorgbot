var target = Argument("target", "Pack");
var configuration = Argument("configuration", "Release");
var packageName = "Slackbot.Net";
var proj = $"./source/src/{packageName}/{packageName}.csproj";

var version = "1.0.1-beta008";
var outputDir = "./output";

Task("Build")
    .Does(() => {
        DotNetCoreBuild(proj, new DotNetCoreBuildSettings { Configuration = "Release" });
    });

Task("Test")
    .IsDependentOn("Build")
    .Does(() => {
        Warning("Lacking tests.");
});

Task("Pack")
    .IsDependentOn("Test")
    .Does(() => {
        var coresettings = new DotNetCorePackSettings
        {
            Configuration = "Release",
            OutputDirectory = outputDir,
        };
        coresettings.MSBuildSettings = new DotNetCoreMSBuildSettings()
                                        .WithProperty("Version", new[] { version });


        DotNetCorePack(proj, coresettings);
});

Task("Publish")
    .IsDependentOn("Pack")
    .Does(() => {
        var settings = new DotNetCoreNuGetPushSettings
        {
            Source = "https://api.nuget.org/v3/index.json",
            ApiKey = Argument("nugetapikey", "must-be-given")
        };

        DotNetCoreNuGetPush($"{outputDir}/{packageName}.{version}.nupkg", settings);
});

RunTarget(target);