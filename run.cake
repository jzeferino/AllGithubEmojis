#addin "nuget:https://api.nuget.org/v3/index.json?package=Cake.FileHelpers&version=7.0.0"
#addin "nuget:https://api.nuget.org/v3/index.json?package=Cake.Json&version=7.0.1"

#r src/AllGithubEmojis.Core/bin/Release/net8.0/AllGithubEmojis.Core.dll
#r src/AllGithubEmojis.Generator/bin/Release/net8.0/AllGithubEmojis.Generator.dll

using AllGithubEmojis.Core;
using AllGithubEmojis.Generator.GithubReadme;

var numberOfColumns = 3;
var readmePath = "./README.md";
var jsonPath = "./emojis.json";

Task("Default")
    .Does(() =>
{
    Information("Parsing emojis...");
    var emojis = GithubEmojiParser.Parse().Result;

    Information("Generating README.md...");
    FileWriteText(readmePath, GithubReadmeGenerator.Generate(emojis, numberOfColumns));

    Information("Generating emojis.json...");
    FileWriteText(jsonPath, SerializeJson(emojis));
});

RunTarget("Default");
