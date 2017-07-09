using System;
using System.Collections.Generic;
using System.Linq;

namespace AllGithubEmojis.Core.Model
{
    public class Emojis
    {
        public List<EmojiGroup> Groups { get; set; } = new List<EmojiGroup>();

        public int Count => Groups.SelectMany(p => p.SubGroups).Sum(subGroup => subGroup.Emojis.Count());
    }
}
