// Declarations.
var configuration = "Release";
var solutionFile = "src/AllGithubEmojis.sln";

// Tasks.
Task("Clean")
    .Does(() =>
{
    var settings = new DotNetCleanSettings
    {
        Configuration = configuration,
        Verbosity = DotNetVerbosity.Normal,
    };
    DotNetClean(solutionFile, settings);
});

Task("Restore")
    .Does(() =>
{
    DotNetRestore(solutionFile);
});

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() =>
{
    var settings = new DotNetBuildSettings
    {
        Configuration = configuration,
        Verbosity = DotNetVerbosity.Minimal
    };
    DotNetBuild(solutionFile, settings);
});

Task("Default")
    .IsDependentOn("Build")
    .Does(() =>
{
    CakeExecuteScript("./run.cake", new CakeSettings
    {
        Arguments = new Dictionary<string, string>
        {
            {"--verbosity", "Diagnostic" },
        }
    });
});

// Execution.
RunTarget("Default");
