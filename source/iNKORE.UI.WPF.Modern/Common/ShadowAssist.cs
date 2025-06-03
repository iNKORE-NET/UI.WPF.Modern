using System.Windows.Media;
using System.Windows;

namespace iNKORE.UI.WPF.Modern.Common
{
    public static class ShadowAssist
    {
        #region AttachedProperty : CacheModeProperty

        public static readonly DependencyProperty CacheModeProperty = DependencyProperty.RegisterAttached(
            "CacheMode", typeof(CacheMode), typeof(ShadowAssist), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

        public static void SetCacheMode(DependencyObject element, CacheMode value)
        {
            element.SetValue(CacheModeProperty, value);
        }

        public static CacheMode GetCacheMode(DependencyObject element)
        {
            return (CacheMode)element.GetValue(CacheModeProperty);
        }

        #endregion

        #region Property : UseBitmapCache

        /// <summary>
        /// For applications with multiple windows, please set this property to false to avoid possible freezing issues.
        /// <see href="https://github.com/dotnet/wpf/issues/2158"/>
        /// </summary>
        public static bool UseBitmapCache { get; set; } = true;

        #endregion
    }
}
