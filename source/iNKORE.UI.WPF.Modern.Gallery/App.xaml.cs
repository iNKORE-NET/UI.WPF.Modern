using iNKORE.UI.WPF.Modern.Gallery.DataModel;
using System.Reflection;
using System;

namespace iNKORE.UI.WPF.Modern.Gallery
{
    public partial class App
    {
        public static bool IsMultiThreaded { get; } = false;

        public static TEnum GetEnum<TEnum>(string text) where TEnum : struct
        {
            if (!typeof(TEnum).GetTypeInfo().IsEnum)
            {
                throw new InvalidOperationException("Generic parameter 'TEnum' must be an enum.");
            }
            return (TEnum)Enum.Parse(typeof(TEnum), text);
        }
    }
}
