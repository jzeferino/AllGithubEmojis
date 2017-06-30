#addin Cake.Http
#addin Cake.Json

public class GithubEmojiGenerator
{
    private StringBuilder _builder;
    private int _colums;
    private string _token;
    private Action<string> _writeAllText;
    private ICakeContext _context;

    private GithubEmojiGenerator(ICakeContext context, int colums, string token, Action<string> writeAllText)
    {
        _context = context;
        _builder = new StringBuilder();
        _colums = colums;
        _token = token;
        _writeAllText = writeAllText;
    }

    public static void Generate(ICakeContext context, int colums, string token, Action<string> writeAllText) => new GithubEmojiGenerator(context, colums, token, writeAllText).Generate();

    private void Generate()
    {
        var emojis = GetEmojis();
        CreateDescription(emojis);
        CreateEmojiTable(emojis);
        _writeAllText(_builder.ToString());
    }

    private void CreateDescription(Dictionary<string, string> emojis)
    {
        _builder.AppendLine("# All Github Emojis");
        _builder.AppendLine("* A list of all supported github emojis updated daily.");    
        _builder.AppendLine("* Automatically generated from github emoji API.");    
        _builder.AppendFormat("## <p align=\"center\"><b>{0} emojis available</b></p>\n", NumberToEmoji(emojis.Count));
    }

    private string NumberToEmoji(int number)
    {
        string[] numbersToLetters = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };

        var builder = new StringBuilder();

        while(number > 0)
        {
            builder.Insert(0, string.Format(" :{0}:", numbersToLetters[number % 10]));
            number = number / 10;
        }
        return builder.ToString();
    }

    private void CreateEmojiTable(Dictionary<string, string> emojis)
    {
        AddTableHeader();
        AddEmojiContent(emojis);
    }

    private void AddEmojiContent(Dictionary<string, string> emojis)
    {
        for (int i = 0; i < emojis.Count; i++)
        {
            if (i % _colums == 0 && i > 0)
            {
                _builder.AppendLine("|");
            }
            var emoji = emojis.ElementAt(i).Key;
            _builder.AppendFormat("| :{0}: `:{1}:` ", emoji, emoji);
        }
    }

    private void AddTableHeader()
    {
        _builder.Append("| ");
        _builder.AppendFormat("Last updated (UTC) {0}", DateTime.UtcNow);
        _builder.Insert(_builder.Length, " | ", _colums);
        _builder.AppendLine();
        _builder.Insert(_builder.Length, "| --- ", _colums);
        _builder.AppendLine("|");
    }

    private Dictionary<string, string> GetEmojis()
    {
        var responseBody = _context.HttpGet($"https://api.github.com/emojis?access_token={_token}", settings =>
        {
            settings.AppendHeader("User-Agent", "github-emoji-generator");
        });

        return _context.DeserializeJson<Dictionary<string, string>>(responseBody);
    }
}
