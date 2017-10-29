using System;
using System.Configuration;
using System.IO;
using AllGithubEmojis.Core;
using AllGithubEmojis.Generator.GithubReadme;

namespace AllGithubEmojis.Console
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var emojis = GithubEmojiParser.Parse(ConfigurationManager.AppSettings["githubToken"]).Result;
            File.WriteAllText("./README.md", GithubReadmeGenerator.Generate(emojis, 3));
        }
    }
}
