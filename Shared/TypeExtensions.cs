﻿

using MudBlazor;
using MudBlazor.Utilities;
using System.Reflection;

namespace Squill.Shared;

public class ElementDisplay : Attribute
{
    public string Icon { get; set; }
    public string Name { get; set; }
    public string Color { get; set; }
}

public static class TypeExtensions
{
    public static string GetName(this Type type)
    {
        var attr = type.GetCustomAttributes(typeof(ElementDisplay), true)
            .Cast<ElementDisplay>()
            .FirstOrDefault();
        return attr?.Name ?? type.Name;
    }

    public static string GetIcon(this Type type)
    {
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
        var attr = type.GetCustomAttributes(typeof(ElementDisplay), true)
            .Cast<ElementDisplay>()
            .FirstOrDefault();
        if(attr.Color != null)
        {
            return new MudColor(attr.Color);
        }
        var rnd = new Random(type.FullName.Sum(c => (int)c));
        return new MudColor(rnd.NextDouble() * 360, .8, .7, 255);
    }

}