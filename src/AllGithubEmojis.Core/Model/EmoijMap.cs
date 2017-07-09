using System;
using System.Collections.Generic;

namespace AllGithubEmojis.Core.Model
{
    public class EmoijMap
    {
        public Dictionary<string, List<Emoji>> CodeEmojiDictionary { get; set; } = new Dictionary<string, List<Emoji>>(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, string> NameCodeDictionary { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }
}
