using System;

namespace Inkore.UI.WPF.Modern
{
    internal static class PackUriHelper
    {
        public static Uri GetAbsoluteUri(string path)
        {
            return new Uri($"pack://application:,,,/Inkore.UI.WPF.Modern;component/{path}");
        }
    }
}
