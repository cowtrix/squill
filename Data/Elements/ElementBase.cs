namespace Squill.Data.Elements;

public abstract class ElementBase : ElementComponentOwnerBase, IElement
{
    public abstract bool ShouldTag { get; }
    public string ScratchPad { get; set; }
    public List<string> Labels { get; set; } = new List<string>();

    public IEnumerable<(string, string)> GetAttributes(ProjectSession session)
    {
        if (!ShouldTag)
        {
            yield return ("notag", "true");
        }
        foreach (var attr in Components.SelectMany(c => c.GetAttributes(session)))
        {
            yield return attr;
        }
        foreach (var attr in GetUniqueAttributes(session))
        {
            yield return attr;
        }
    }

    protected virtual IEnumerable<(string, string)> GetUniqueAttributes(ProjectSession session)
    {
        yield break;
    }

    public virtual void OnBeforeSerialize()
    {

    }
}
