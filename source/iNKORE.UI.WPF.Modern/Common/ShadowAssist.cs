using System.Windows.Media;
using System.Windows;

namespace iNKORE.UI.WPF.Modern.Common
{
    public static class ShadowAssist
    {
        #region Property : UseBitmapCache

        /// <summary>
        /// Whether to use BitmapCache for shadow rendering and animations.
        /// </summary>
        /// <remarks>
        /// <para>
        /// For applications with multiple windows, especially when hiding/showing windows or after system lock/unlock,
        /// please set this property to false to avoid possible freezing issues.
        /// </para>
        /// <para>
        /// This property should be set before any windows are created, typically in your application startup code.
        /// Example: <c>ShadowAssist.UseBitmapCache = false;</c>
        /// </para>
        /// <para>
        /// Note: Disabling BitmapCache may result in slightly reduced animation performance but will resolve
        /// freezing issues in multi-window scenarios.
        /// </para>
        /// <para>
        /// Related WPF issue: <see href="https://github.com/dotnet/wpf/issues/2158"/>
        /// </para>
        /// </remarks>
        public static bool UseBitmapCache { get; set; } = true;

        #endregion

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
    }
}
