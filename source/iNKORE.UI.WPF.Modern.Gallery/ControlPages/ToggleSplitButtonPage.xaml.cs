using iNKORE.UI.WPF.Modern.Common.IconKeys;
using iNKORE.UI.WPF.Modern.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Page = iNKORE.UI.WPF.Modern.Controls.Page;

namespace iNKORE.UI.WPF.Modern.Gallery.ControlPages
{
    /// <summary>
    /// ToggleSplitButtonPage.xaml 的交互逻辑
    /// </summary>
    public partial class ToggleSplitButtonPage : Page
    {
        private string _type = "•";
        public ToggleSplitButtonPage()
        {
            InitializeComponent();
        }

        private void BulletButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedBullet = (Button)sender;
            var symbol = (FontIcon)clickedBullet.Content;

            if (symbol.Glyph == SegoeFluentIcons.List.Glyph)
            {
                _type = "•";
                mySymbolIcon.Icon = SegoeFluentIcons.List;
                myListButton.SetValue(AutomationProperties.NameProperty, "Bullets");
            }
            else if (symbol.Glyph == SegoeFluentIcons.BulletedList.Glyph)
            {
                _type = "I)";
                mySymbolIcon.Icon = SegoeFluentIcons.BulletedList;
                myListButton.SetValue(AutomationProperties.NameProperty, "Roman Numerals");
            }
            myRichEditBox.Selection.Text = _type;

            myListButton.IsChecked = true;
            myListButton.Flyout.Hide();
            myRichEditBox.Focus();
        }

        private void MyListButton_IsCheckedChanged(ToggleSplitButton sender, ToggleSplitButtonIsCheckedChangedEventArgs args)
        {
            if (sender.IsChecked)
            {
                //add bulleted list
                myRichEditBox.Selection.Text = _type;
            }
            else
            {
                //remove bulleted list
                myRichEditBox.Selection.Text = "";
            }
        }

        #region Example Code

        public void UpdateExampleCode()
        {

        }

        #endregion

    }
}
