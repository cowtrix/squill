using Microsoft.AspNetCore.Components;
using Squill.Data.Elements;

namespace Squill.Data.ElementComponents;

public abstract class ElementComponent : IAttributeProvider
{
    public Guid Owner { get; set; }

    public abstract RenderFragment GetEditor(ProjectSession session, IComponentOwner owner);

    public static bool CanApplyTo(Type componentType, Type targetType)
    {
        var attr = targetType.GetCustomAttributes(typeof(ComponentTypeAttribute), true)
            .Cast<ComponentTypeAttribute>()
            .FirstOrDefault();
        if (attr == null || attr.ValidTypes == null)
        {
            return false;
        }
        return attr.ValidTypes.Contains(componentType);
    }

    public virtual IEnumerable<(string, string)> GetAttributes(ProjectSession session)
    {
        yield break;
    }
}

