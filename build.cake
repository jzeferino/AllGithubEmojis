#addin Cake.Git
#addin Cake.FileHelpers

// Environment variables.
var gitpassword = EnvironmentVariable("githubPassword") ?? Argument("password", string.Empty);
var githubAPIToken = EnvironmentVariable("githubAPIToken") ?? Argument("token", string.Empty);

// Load other scripts.
#load "./github-emoji-generator.cake"

// Declarations.
var clonedRepo = Directory("./repo");
var username = "jzeferino";
var userEmail = "jorgevalentzeferino@gmail.com";
var numberOfColumns = 2;
Func<string> readmePath = () => System.IO.Path.Combine(clonedRepo, "README.md");
var repoUrl = "https://github.com/jzeferino/GithubEmojis.git";
var master = "master";

// Tasks.
Task("Default")
    .Does(() =>
{
    Information($"Cloning {master}...");
    GitClone(repoUrl, clonedRepo, new GitCloneSettings{ BranchName = master });

    Information("Generating README.md...");
    GithubEmojiGenerator.Generate(
        Context,
        numberOfColumns,
        githubAPIToken,
        (finalText) => FileWriteText(readmePath(), finalText));    

    Information("Git addall, commit and push...");
    GitAddAll(clonedRepo);
    GitCommit(clonedRepo, username, userEmail, "Update README.md");
    GitPush(clonedRepo, username, gitpassword, master);
});

// Execution.
RunTarget("Default");
