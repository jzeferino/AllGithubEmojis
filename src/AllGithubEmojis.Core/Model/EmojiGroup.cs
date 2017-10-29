using System;
using System.Collections.Generic;

namespace AllGithubEmojis.Core.Model
{
    public class EmojiGroup
    {
        public string Name { get; set; }
        public List<EmojiSubGroup> SubGroups { get; set; } = new List<EmojiSubGroup>();
    }
}
