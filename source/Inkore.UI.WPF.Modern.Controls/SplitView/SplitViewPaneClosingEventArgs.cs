using System;

namespace Inkore.UI.WPF.Modern.Controls
{
    public sealed class SplitViewPaneClosingEventArgs : EventArgs
    {
        internal SplitViewPaneClosingEventArgs()
        {
        }

        public bool Cancel { get; set; }
    }
}