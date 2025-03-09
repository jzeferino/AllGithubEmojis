using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AllGithubEmojis.Core.Network
{
    public static class Requests
    {
        public static async Task<Dictionary<string, Uri>> GetGitHubEmojisAsync()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd(nameof(GithubEmojiParser));
            var responseBody = await client.GetStringAsync($"https://api.github.com/emojis");

            return JsonConvert.DeserializeObject<Dictionary<string, Uri>>(responseBody);
        }

        public static Task<string> ReadEmojiTestFileAsync() => new HttpClient().GetStringAsync("http://unicode.org/Public/emoji/latest/emoji-test.txt");
    }
}
