using System.Reflection;

namespace Maui.ServerDrivenUI.Xaml;

public static class Extensions
{
    public static TXaml LoadXaml<TXaml>(this TXaml view, Type callingType)
    {
        XamlLoader.Load(view, callingType);
        return view;
    }

    public static TXaml LoadXaml<TXaml>(this TXaml view, string xaml)
    {
        XamlLoader.Load(view, xaml);
        return view;
    }

    internal static TXaml LoadXaml<TXaml>(this TXaml view, string xaml, Assembly rootAssembly)
    {
        XamlLoader.Load(view, xaml, rootAssembly);
        return view;
    }
}
