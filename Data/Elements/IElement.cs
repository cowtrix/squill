using Squill.Data.ElementComponents;

namespace Squill.Data.Elements;

public interface IElement : IAttributeProvider, IComponentOwner
{
    bool ShouldTag { get; }
    string ScratchPad { get; set; }
    void OnBeforeSerialize();
}

public interface IAttributeProvider
{
    IEnumerable<(string, string)> GetAttributes(ProjectSession session);
}

public interface IComponentOwner
{
    Guid Guid { get; }
    IEnumerable<ElementComponent> GetComponents(Type t);
    IEnumerable<T> GetComponents<T>() where T : ElementComponent;
    T GetComponent<T>() where T : ElementComponent;
    ElementComponent AddComponent(Type t);
    T AddComponent<T>() where T : ElementComponent;
    void RemoveComponent(ElementComponent component);
}

public interface IContainerElement : IElement
{
    IEnumerable<Guid> Children { get; }
}