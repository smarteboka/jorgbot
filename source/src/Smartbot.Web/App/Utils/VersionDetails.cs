using System.Reflection;

internal record DebugInfo(string MajorMinorPatch, string Informational, string Sha);

internal static class VersionDetails
{
    private static DebugInfo _debugInfo;

    public static DebugInfo Versions()
    {
        if (_debugInfo != null)
        {
            return _debugInfo;
        }
        var entryAssembly = Assembly.GetEntryAssembly();
        var version = entryAssembly?.GetName()?.Version;
        var majorMinorPatch = $"{version?.Major}.{version?.Minor}.{version?.Build}";
        var informationalVersion = entryAssembly?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
        var sha = informationalVersion?.Split(".").Last();
        _debugInfo = new DebugInfo(majorMinorPatch,informationalVersion, sha) ;
        return _debugInfo;
    }
}