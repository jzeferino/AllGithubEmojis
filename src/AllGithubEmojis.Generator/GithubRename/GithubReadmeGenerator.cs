using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllGithubEmojis.Core.Model;

namespace AllGithubEmojis.Generator.GithubReadme
{
    public class GithubReadmeGenerator
    {
        private readonly StringBuilder _builder;
        private readonly int _columns;
        private Emojis _emojis;

        private GithubReadmeGenerator(Emojis emojis, int columns)
        {
            _builder = new StringBuilder();
            _columns = columns;
            _emojis = emojis;
        }

        public static string Generate(Emojis emojis, int columns) => new GithubReadmeGenerator(emojis, columns).Generate();

        private string Generate()
        {
            CreateHeaderDescription();
            CreateIndex();
            CreateTables();

            return _builder.ToString();
        }

        private void CreateIndex()
        {
            foreach (var emojiGroup in _emojis.Groups)
            {
                _builder.AppendLine($"* [{emojiGroup.Name}](#{emojiGroup.Name.ToLower().Replace(" & ", "--")})");
                foreach (var subGroup in emojiGroup.SubGroups)
                {
                    _builder.AppendLine($"\t * [{subGroup.Name}](#{subGroup.Name.ToLower().Replace(" & ", "--")})");
                }
            }
        }

        private void CreateTables()
        {
            foreach (var emojiGroup in _emojis.Groups)
            {
                _builder.AppendLine($"# {emojiGroup.Name}");
                foreach (var subGroup in emojiGroup.SubGroups)
                {
                    CreateEmojiTable(subGroup);
                }
            }
        }

        private void CreateHeaderDescription()
        {
            _builder.AppendLine("# [AllGithubEmojis](https://jzeferino.github.io/AllGithubEmojis/)");

            _builder.AppendLine("* A list of **all supported github** emojis updated weekly.");
            _builder.AppendLine("* **Automatically generated** from github emoji API.");
            _builder.AppendLine("* Grouped following official emoji [convention](http://unicode.org/emoji/charts/full-emoji-list.html).");

            _builder.AppendFormat("## <p align=\"center\"><b>{0} Emojis available</b></p>\n", NumberToEmoji(_emojis.Count));
            _builder.AppendFormat("### <p align=\"center\">Last updated (UTC) {0}</p>\n", DateTime.UtcNow);
        }

        private string NumberToEmoji(int number)
        {
            string[] numbersToLetters = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };

            var builder = new StringBuilder();

            while (number > 0)
            {
                builder.Insert(0, string.Format(" :{0}:", numbersToLetters[number % 10]));
                number = number / 10;
            }
            return builder.ToString();
        }

        private void CreateEmojiTable(EmojiSubGroup emojiSubGroup)
        {
            AddTableTitle(emojiSubGroup);
            AddTAbleContent(emojiSubGroup.Emojis);
        }

        private void AddTAbleContent(List<Emoji> emojis)
        {
            var columns = Math.Min(emojis.Count, _columns);
            for (int i = 0; i < emojis.Count; i++)
            {
                if (i % columns == 0 && i > 0)
                {
                    _builder.AppendLine("|");
                }
                var emoji = emojis.ElementAt(i);
                _builder.AppendFormat("| :{0}: **`:{1}:`** ", emoji.Code, emoji.Code);

                if (i == (columns - 1))
                {
                    _builder.AppendLine();
                    _builder.Insert(_builder.Length, "|:---:", columns);
                }
            }
            _builder.AppendLine();
        }

        private void AddTableTitle(EmojiSubGroup subgroup)
        {
            _builder.AppendLine($"### {subgroup.Name}");
        }
    }
}
