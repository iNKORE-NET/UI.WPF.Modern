using System;

namespace Inkore.UI.WPF.Modern.Controls
{
    public class ContentDialogClosedEventArgs : EventArgs
    {
        internal ContentDialogClosedEventArgs(ContentDialogResult result)
        {
            Result = result;
        }

        public ContentDialogResult Result { get; }
    }
}
