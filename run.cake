#addin Cake.Git
#addin Cake.FileHelpers
#addin "Cake.Json"

#r src/AllGithubEmojis.Core/bin/Release/AllGithubEmojis.Core.dll
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
Func<string> jsonPath = () => System.IO.Path.Combine(clonedRepo, "emojis.json");
var repoUrl = "https://github.com/jzeferino/AllGithubEmojis.git";
var master = "master";
var ghPages = "gh-pages";
var isLocalBuild = BuildSystem.IsLocalBuild;

// Do common logic.
Action<string, Action, Func<string>, string> CloneExecuteAndPush = (branch, execute, filePath, commitMessage) =>
{
    if(DirectoryExists(clonedRepo))
    {
        DeleteDirectory(clonedRepo, recursive:true);
    }  

    Information($"Cloning {branch}...");
    GitClone(repoUrl, clonedRepo, new GitCloneSettings{ BranchName = branch });
    
    execute();

    if(!isLocalBuild)
    {
        if(GitHasUncommitedChanges(clonedRepo))
        {
            Information("Git add, commit and push...");
            GitAdd(clonedRepo,  new FilePath[] { filePath() });
            GitCommit(clonedRepo, username, userEmail, commitMessage);
            GitPush(clonedRepo, username, gitpassword, branch);
        }
        else
        {
            Information("Nothing to commit.");
        }
        
    }
};

// Tasks.
Task("Default")
    .Does(() =>
{
    Information("Parsing emojis...");
    var emojis = GithubEmojiParser.Parse(githubAPIToken).Result;

    CloneExecuteAndPush(
        master,
        () => {
            Information("Generating README.md...");
            FileWriteText(readmePath(), GithubReadmeGenerator.Generate(emojis, numberOfColumns));
        },
        readmePath,
        "Update README.md");
    
    CloneExecuteAndPush(
        ghPages,
        () => {
            Information("Generating emojis.json...");
            FileWriteText(jsonPath(), SerializeJson(emojis));
        },
        jsonPath,
        "Update emojis.json");
});

// Execution.
RunTarget("Default");
