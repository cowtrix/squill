namespace Squill.Data
{
    public abstract class ElementComponent<T> : ElementBase, IElementComponent
        where T : class, IElement
    {
        public string Owner { get; set; }
    }
}
