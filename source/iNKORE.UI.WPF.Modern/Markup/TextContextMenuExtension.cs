/* 项目“iNKORE.UI.WPF.Modern (net452)”的未合并的更改
在此之前:
using System;
在此之后:
using iNKORE.UI.WPF.Modern.Controls.TextContextMenu;
using iNKORE.UI.WPF.Modern.Controls.TextContextMenu.TextContextMenu;
using System;
*/
using iNKORE.UI.WPF.Modern.Controls;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Markup;

namespace iNKORE.UI.WPF.Modern.Markup
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    [MarkupExtensionReturnType(typeof(ContextMenu))]
    public class TextContextMenuExtension : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return DefaultContextMenu.Value;
        }

        private static readonly ThreadLocal<TextContextMenu> DefaultContextMenu = new ThreadLocal<TextContextMenu>(() => new TextContextMenu());
    }
}
