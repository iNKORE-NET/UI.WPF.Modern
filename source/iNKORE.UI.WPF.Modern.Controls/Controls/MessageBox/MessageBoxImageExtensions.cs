using System;
using System.Media;
using System.Windows;

namespace iNKORE.UI.WPF.Modern.Extensions
{
    internal static class MessageBoxImageExtensions
    {
        public static string ToSymbol(this MessageBoxImage image)
        {
            return image switch
            {
                MessageBoxImage.Error => SegoeIcons.ErrorBadge,
                MessageBoxImage.Information => SegoeIcons.Info,
                MessageBoxImage.Warning => SegoeIcons.Warning,
                MessageBoxImage.Question => SegoeIcons.Unknown,
                MessageBoxImage.None => char.Parse("0x2007").ToString(),
                _ => char.Parse("0x2007").ToString(),
            };
        }


        public static SystemSound ToAlertSound(this MessageBoxImage image)
        {
            return image switch
            {
                MessageBoxImage.Error => SystemSounds.Hand,
                MessageBoxImage.Information => SystemSounds.Asterisk,
                MessageBoxImage.Warning => SystemSounds.Exclamation,
                MessageBoxImage.Question => SystemSounds.Question,
                MessageBoxImage.None => null,
                _ => null,
            };

        }
    }
}
