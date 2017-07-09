using System;
using System.IO;
using AllGithubEmojis.Core;
using AllGithubEmojis.Core.Generator;

namespace AllGithubEmojis.Console
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var emojis = GithubEmojiParser.Parse("").Result;
            File.WriteAllText("./README.md", GithubReadmeGenerator.Generate(emojis, 3));
        }
    }
}
