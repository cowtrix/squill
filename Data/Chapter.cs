using Squill.Shared;
using System.Text.Json.Serialization;

namespace Squill.Data;

[ElementDisplay(Icon = "EditNote")]
public class Chapter : ElementComponent<Manuscript>
{
    public const string WORD_COUNT_ATTRIB_KEY = "word_count";
    public const string PARENT_INDEX_ATTRIB_KEY = "index";

    public string Content { get; set; }

    [JsonIgnore]
    public int WordCount => !string.IsNullOrEmpty(Content) ? Content.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length : 0;

    public override IEnumerable<(string, string)> GetAttributes()
    {
        yield return (WORD_COUNT_ATTRIB_KEY, WordCount.ToString());
    }
}
