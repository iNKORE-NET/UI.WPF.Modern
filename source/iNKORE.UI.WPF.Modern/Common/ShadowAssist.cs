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

        #region AttachedProperty : UseBitmapCacheProperty

        public static readonly DependencyProperty UseBitmapCacheProperty = DependencyProperty.RegisterAttached(
            "UseBitmapCache", typeof(bool), typeof(ShadowAssist), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));

        public static void SetUseBitmapCache(DependencyObject element, bool value)
        {
            element.SetValue(UseBitmapCacheProperty, value);
        }

        public static bool GetUseBitmapCache(DependencyObject element)
        {
            return (bool)element.GetValue(UseBitmapCacheProperty);
        }

        #endregion
    }
}
