namespace Squill.Data.Elements;

public abstract class ElementBase : IElement
{
    public Guid Guid { get; set; }
    public abstract bool ShouldTag { get; }
    public string ScratchPad { get; set; }
    public List<string> Labels { get; set; } = new List<string>();
    public List<ElementComponent> Components { get; set; } = new List<ElementComponent>();

    public ElementBase()
    {
        Guid = Guid.NewGuid();
    }

    public virtual IEnumerable<(string, string)> GetAttributes(ProjectSession session)
    {
        if (!ShouldTag)
        {
            yield return ("notag", "true");
        }
        yield break;
    }
}
