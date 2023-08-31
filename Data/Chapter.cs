using Squill.Shared;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Squill.Data;

[ElementDisplay(Icon = "EditNote")]
public class Chapter : ElementComponent<Manuscript>
{
    public const string WORD_COUNT_ATTRIB_KEY = "word_count";
    public const string PARENT_INDEX_ATTRIB_KEY = "index";

    public string Content { get; set; }
    public List<Comment> Comments { get; set; } = new List<Comment>();

    [JsonIgnore]
    public int WordCount => !string.IsNullOrEmpty(Content) ? Content.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length : 0;

    public override bool ShouldTag => true;

    public override IEnumerable<(string, string)> GetAttributes(ProjectSession session)
    {
        yield return (WORD_COUNT_ATTRIB_KEY, WordCount.ToString());
        if (!string.IsNullOrEmpty(Content))
        {
            var strip = Content
                .Replace(Environment.NewLine, " ")
                .Replace("\n", " ")
                .Replace(".", "")
                .Replace("\"", "")
                .Replace("!", "")
                .Replace("?", "")
                .Split(' ')
                .ToHashSet();
            var links = session.ElementMeta.Where(m => m.ShouldTag() && strip.Contains(m.Name)).Select(m => m.Guid);
            yield return ("links", JsonSerializer.Serialize(links));
        }
    }
}
