using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inkore.UI.WPF.Modern.Controls
{
    public class InfoBarClosedEventArgs : EventArgs
    {
        public InfoBarCloseReason Reason { get; internal set; }
    }
}
