#addin Cake.Git
#addin Cake.FileHelpers

#r src/AllGithubEmojis.core/bin/Release/AllGithubEmojis.Core.dll
using AllGithubEmojis.Core;
using AllGithubEmojis.Core.Generator;

// Environment variables.
var gitpassword = EnvironmentVariable("githubPassword") ?? Argument("password", string.Empty);
var githubAPIToken = EnvironmentVariable("githubAPIToken") ?? Argument("token", string.Empty);

// Declarations.
var clonedRepo = Directory("./repo");
var username = "jzeferino";
var userEmail = "jorgevalentzeferino@gmail.com";
var numberOfColumns = 3;
Func<string> readmePath = () => System.IO.Path.Combine(clonedRepo, "README.md");
var repoUrl = "https://github.com/jzeferino/AllGithubEmojis.git";
var master = "master";
var isLocalBuild = BuildSystem.IsLocalBuild;

// Tasks.
Task("Default")
    .Does(() =>
{
    if(DirectoryExists(clonedRepo))
    {
        DeleteDirectory(clonedRepo, recursive:true);
    }  
    
    Information($"Cloning {master}...");
    GitClone(repoUrl, clonedRepo, new GitCloneSettings{ BranchName = master });
    
    Information("Parsing emojis...");
    var emojis = GithubEmojiParser.Parse(githubAPIToken).Result;

    Information("Generating README.md...");
    FileWriteText(readmePath(), GithubReadmeGenerator.Generate(emojis, numberOfColumns));

    if(!isLocalBuild)
    {
        Information("Git add, commit and push...");
        GitAdd(clonedRepo,  new FilePath[] { readmePath() });
        GitCommit(clonedRepo, username, userEmail, "Update README.md");
        GitPush(clonedRepo, username, gitpassword, master);
    }    
});

// Execution.
RunTarget("Default");
