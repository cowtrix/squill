using Squill.Data.ElementComponents;

namespace Squill.Data.Elements;

public abstract class ElementComponentOwnerBase : IComponentOwner
{
    public Guid Guid { get; set; } = Guid.NewGuid();
    public List<ElementComponent> Components { get; set; } = new List<ElementComponent>();

    public T AddComponent<T>() where T : ElementComponent => (T)AddComponent(typeof(T));

    public ElementComponent AddComponent(Type t)
    {
        var newComponent = Activator.CreateInstance(t) as ElementComponent;
        if (newComponent == null)
        {
            throw new Exception($"Failed to create type {t}");
        }
        newComponent.Owner = Guid;
        Components.Add(newComponent);
        return newComponent;
    }

    public void RemoveComponent(ElementComponent component)
    {
        Components.Remove(component);
    }

    public IEnumerable<T> GetComponents<T>() where T : ElementComponent => Components.OfType<T>();

    public T GetComponent<T>() where T : ElementComponent => Components.OfType<T>().SingleOrDefault();

    public IEnumerable<ElementComponent> GetComponents(Type t) => Components.Where(c => c.GetType().IsAssignableFrom(t));
}
