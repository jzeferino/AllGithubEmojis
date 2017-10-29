using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AllGithubEmojis.Core.Extensions;
using AllGithubEmojis.Core.Model;
using AllGithubEmojis.Core.Network;

namespace AllGithubEmojis.Core
{
    public class GithubEmojiParser
    {
        private const string SubGroupDelimiter = "# subgroup: ";
        private const string GroupDelimiter = "# group: ";

        private GithubEmojiParser() { }

        public static Task<Emojis> Parse(string token) => new GithubEmojiParser().InternalParse(token);

        private async Task<Emojis> InternalParse(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentException(nameof(token));
            }

            var emojis = new Emojis();

            var tuple = await TaskEx.WhenAll(ParseGithubEmojis(token), Requests.ReadEmojiTestFileAsync());
            var groupedEmojiText = tuple.Item2;
            var map = tuple.Item1;

            using (var file = new StringReader(groupedEmojiText))
            {
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    // Don't care about empty lines
                    if (string.IsNullOrEmpty(line))
                    {
                        continue;
                    }

                    if (line.Contains(GroupDelimiter))
                    {
                        CreateGroup(line, emojis);
                    }
                    else if (line.Contains(SubGroupDelimiter))
                    {
                        CreateSubGroup(line, emojis);
                    }
                    // If the line isn't a comment, add the emoji.
                    else if (!line.StartsWith("#", StringComparison.Ordinal))
                    {
                        CreateEmoji(map, line, emojis);
                    }
                }
            }

            // Add reaming emojis to miscellaneous bag.
            emojis.Groups.Add(new EmojiGroup
            {
                Name = "Miscellaneous",
                SubGroups = new List<EmojiSubGroup> {new EmojiSubGroup {
                        Emojis = map.CodeEmojiDictionary.Values.SelectMany(x => x).ToList(),
                        Name = "other"
                    }}
            });

            return emojis;
        }

        private EmojiGroup CreateGroup(string line, Emojis emojis)
        {
            return emojis
                .Groups
                .AddAndPick(new EmojiGroup { Name = line.Replace(GroupDelimiter, string.Empty) });
        }

        private EmojiSubGroup CreateSubGroup(string line, Emojis emojis)
        {
            return emojis
                .Groups
                .Last()
                .SubGroups
                .AddAndPick(new EmojiSubGroup { Name = line.Replace(SubGroupDelimiter, string.Empty) });
        }

        private void CreateEmoji(EmoijMap map, string line, Emojis emojis)
        {
            var unicode = line.Split(';').First().Trim().Replace(' ', '-');

            var emojiMatch = Regex.Match(line, @"[^\u0000-\u007F]+ ");
            var emojiEnd = emojiMatch.Index + emojiMatch.Length;
            var emojiName = line.Substring(emojiEnd, line.Length - emojiEnd).Replace("-", " ").Replace(":", string.Empty).Replace(",", string.Empty);

            if (map.CodeEmojiDictionary.ContainsKey(unicode))
            {
                foreach (var emoji in map.CodeEmojiDictionary[unicode])
                {
                    emoji.Unicode = unicode;

                    AddEmoji(emojis, emoji);

                    map.NameCodeDictionary.Remove(emoji.Name);

                }
                map.CodeEmojiDictionary.Remove(unicode);
            }
            else
            {
                if (map.NameCodeDictionary.ContainsKey(emojiName))
                {
                    var index = map.CodeEmojiDictionary[map.NameCodeDictionary[emojiName]].FindIndex(e => e.Name.Equals(emojiName));
                    if (index == -1) { return; }
                    var emoji = map.CodeEmojiDictionary[map.NameCodeDictionary[emojiName]][index];
                    emoji.Unicode = unicode;

                    AddEmoji(emojis, emoji);

                    map.CodeEmojiDictionary[map.NameCodeDictionary[emojiName]].RemoveAt(index);
                    if (map.CodeEmojiDictionary[map.NameCodeDictionary[emojiName]].Count == 0)
                    {
                        map.CodeEmojiDictionary.Remove(map.NameCodeDictionary[emojiName]);
                    }

                    map.NameCodeDictionary.Remove(emojiName);
                }
            }
        }

        private static void AddEmoji(Emojis emojis, Emoji emoji)
        {
            emojis.Groups
                .Last()
                .SubGroups
                .Last()
                .Emojis
                .Add(emoji);
        }

        private async Task<EmoijMap> ParseGithubEmojis(string token)
        {
            var githubEmojis = await Requests.GetGitHubEmojisAsync(token);

            var map = new EmoijMap();

            foreach (var githubEmoji in githubEmojis)
            {
                var emoji = new Emoji
                {
                    Unicode = Path.GetFileNameWithoutExtension(githubEmoji.Value.Segments.Last()), // Get the .png name (wich is the code).
                    Code = githubEmoji.Key,
                    Url = githubEmoji.Value,
                    Name = githubEmoji.Key.Replace("_", " ") // Github emoji name are _ separated.
                };

                // If we have duplicated keys, add to the list.
                if (map.CodeEmojiDictionary.ContainsKey(emoji.Unicode))
                {
                    map.CodeEmojiDictionary[emoji.Unicode].Add(emoji);
                }
                else
                {
                    map.CodeEmojiDictionary.Add(emoji.Unicode, new List<Emoji> { emoji });
                }

                map.NameCodeDictionary.Add(emoji.Name, emoji.Unicode);
            }
            return map;
        }

    }

}
