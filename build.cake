// Declarations.
var configuration = "Release";
var solutionFile = new FilePath("src/AllGithubEmojis.sln");

// Tasks.
Task("Clean")
    .Does(() =>
{	
    DotNetBuild(solutionFile, settings => settings
        .SetConfiguration(configuration)
        .WithTarget("Clean")
        .SetVerbosity(Verbosity.Minimal));
});

Task("Restore")
    .Does(() => 
{
    NuGetRestore(solutionFile);
});

Task("Build")
	.IsDependentOn("Clean")
	.IsDependentOn("Restore")
	.Does(() =>  
{	
    DotNetBuild(solutionFile, settings => settings
        .SetConfiguration(configuration)
        .WithTarget("Build")
        .SetVerbosity(Verbosity.Minimal));
});

Task("Default")
	.IsDependentOn("Build")
    .Does(() =>
{
    CakeExecuteScript("./run.cake", new CakeSettings { 
        Arguments = new Dictionary<string, string>
        {
            {"target", "Default" },
            {"--verbosity", "Diagnostic" }
        }
    });
});

// Execution.
RunTarget("Default");
