using System;
using System.Windows;

namespace iNKORE.UI.WPF.Modern.Extensions
{
    internal static class MessageBoxImageExtensions
    {
        public static string ToSymbol(this MessageBoxImage image)
        {
            return image switch
            {
                MessageBoxImage.Error => SegoeIcons.Error,
                MessageBoxImage.Information => SegoeIcons.Info,
                MessageBoxImage.Warning => SegoeIcons.Warning,
                MessageBoxImage.Question => SegoeIcons.StatusCircleQuestionMark,
                MessageBoxImage.None => char.Parse("0x2007").ToString(),
                _ => throw new NotSupportedException(),
            };
        }
    }
}
