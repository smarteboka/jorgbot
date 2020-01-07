var target = Argument("target", "Test");
var configuration = Argument("configuration", "Release");

Task("Build")
    .Does(() => {
        DotNetCoreBuild("./source/Smartbot.sln", new DotNetCoreBuildSettings { Configuration = "Release" });
    });

Task("Test")
    .IsDependentOn("Build")
    .Does(() => {
        Warning("Lacking tests.");
});

RunTarget(target);