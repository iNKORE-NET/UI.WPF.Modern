using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* 项目“iNKORE.UI.WPF.Modern (net452)”的未合并的更改
在此之前:
using System.Windows;
在此之后:
using System.Windows;
using iNKORE;
using iNKORE.UI;
using iNKORE.UI.WPF;
using iNKORE.UI.WPF.Modern;
using iNKORE.UI.WPF.Modern.Controls;
using iNKORE.UI.WPF.Modern.Controls;
using iNKORE.UI.WPF.Modern.Controls.AnimatedVisuals;
*/
using System.Windows;

namespace iNKORE.UI.WPF.Modern.Controls
{
    /// <summary>
    /// Represents an animation for a back arrow that can be used as an animated icon source.
    /// </summary>
    public class AnimatedBackVisualSource : AnimatedVisualSource
    {
        static AnimatedBackVisualSource()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnimatedBackVisualSource), new FrameworkPropertyMetadata(typeof(AnimatedBackVisualSource)));
        }

        public AnimatedBackVisualSource()
        {
        }

        protected override void OnStatePropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            var state = (string)e.NewValue;
            if (state == "Normal" || state == "Pressed" || state == "PointerOver")
            {
                VisualStateManager.GoToState(this, (string)e.NewValue, true);
            }
            else
            {
                VisualStateManager.GoToState(this, "Normal", true);
            }
        }
    }
}
