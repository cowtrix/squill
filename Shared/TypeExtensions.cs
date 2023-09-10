

using MudBlazor;
using MudBlazor.Utilities;
using Squill.Data.ElementComponents;
using Squill.Data.Elements;
using System.Reflection;
using static MudBlazor.CategoryTypes;

namespace Squill.Shared;

public class ElementDisplay : Attribute
{
    public string Icon { get; set; }
    public string Name { get; set; }
    public string Color { get; set; }
}

public static class TypeExtensions
{
    public static IEnumerable<Type> GetTypesImplementing(this Type t)
    {
        Type interfaceType = t;

        // Get all loaded assemblies
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

        // Iterate through each assembly and find types that implement the given interface
        foreach (Assembly assembly in assemblies)
        {
            IEnumerable<Type> typesInAssembly;
            try
            {
                // Get all types from the assembly (ignoring any exceptions)
                typesInAssembly = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                // Handle exceptions if any type cannot be loaded
                typesInAssembly = ex.Types.Where(t => t != null);
            }

            foreach (Type type in typesInAssembly)
            {
                if (interfaceType.IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                {
                    yield return type;
                }
            }
        }
    }

    public static IEnumerable<Type> GetApplicableComponents(this IComponentOwner owner)
    {
        var componentsCanAdd = typeof(ElementComponent).GetTypesImplementing().Where(t => ElementComponent.CanApplyTo(t, owner.GetType()))
                            .Where(t => t.GetCustomAttributes(true).Any(t => t is MultipleComponentAttribute) || !owner.GetComponents(owner.GetType()).Any())
                            .ToList();
        return componentsCanAdd;
    }

    public static string GetName(this Type type)
    {
        if (type == null)
        {
            return "NULL";
        }
        var attr = type.GetCustomAttributes(typeof(ElementDisplay), true)
            .Cast<ElementDisplay>()
            .FirstOrDefault();
        return attr?.Name ?? type.Name;
    }

    public static string GetIcon(this Type type)
    {
        if (type == null)
        {
            return "ERROR";
        }
        var attr = type.GetCustomAttributes(typeof(ElementDisplay), true)
            .Cast<ElementDisplay>()
            .FirstOrDefault();

        if(!string.IsNullOrEmpty(attr?.Icon))
        {
            var allIcons = typeof(Icons.Material.Filled).GetFields(BindingFlags.Public | BindingFlags.Static);
            var pickIcon = allIcons.SingleOrDefault(i => i.Name == attr.Icon);
            if (pickIcon != null)
            {
                return (string)pickIcon.GetValue(null);
            }
        }
        return Icons.Material.Filled.InsertDriveFile;
    }

    public static MudColor GetColor(this Type type)
    {
        if(type == null)
        {
            return new MudColor(255, 255, 255, 255);
        }
        var attr = type.GetCustomAttributes(typeof(ElementDisplay), true)
            .Cast<ElementDisplay>()
            .FirstOrDefault();
        if(attr.Color != null)
        {
            return new MudColor(attr.Color);
        }
        return type.GetDefaultColor();
    }

    public static MudColor GetDefaultColor(this object obj)
    {
        var rnd = new Random(obj.ToString().Sum(c => (int)c));
        return new MudColor(rnd.NextDouble() * 360, .8, .7, 255);
    }

}
