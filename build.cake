// Declarations.
var configuration = "Release";
var solutionFile = new FilePath("src/AllGithubEmojis.sln");

// Environment variables.
var gitpassword = Argument("password", string.Empty);
var githubAPIToken = Argument("token", string.Empty);

// Tasks.
Task("Clean")
    .Does(() =>
{	
    MSBuild(solutionFile, settings => settings
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
    MSBuild(solutionFile, settings => settings
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
            {"--verbosity", "Diagnostic" },
            {"password", gitpassword },
            {"token", githubAPIToken }
        }
    });
});

// Execution.
RunTarget("Default");
