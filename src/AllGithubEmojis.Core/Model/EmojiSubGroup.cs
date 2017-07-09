using System;
using System.Collections.Generic;

namespace AllGithubEmojis.Core.Model
{
    public class EmojiSubGroup
    {
        public string Name { get; set; }
        public List<Emoji> Emojis { get; set; } = new List<Emoji>();
    }
}
