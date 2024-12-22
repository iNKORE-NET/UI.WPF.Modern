using iNKORE.UI.WPF.Modern.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Page = iNKORE.UI.WPF.Modern.Controls.Page;
using MessageBox = iNKORE.UI.WPF.Modern.Controls.MessageBox;
using iNKORE.UI.WPF.Modern.Common.IconKeys;
using iNKORE.UI.WPF.Modern.Common;
using iNKORE.UI.WPF.Modern.Helpers.Styles;
using System.Media;

namespace iNKORE.UI.WPF.Modern.Gallery.Pages.Controls.Extended
{
    /// <summary>
    /// BorderPage.xaml 的交互逻辑
    /// </summary>
    public partial class MessageBoxPage : Page
    {
        public MessageBoxPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateExampleCode();
        }

        public static readonly string UsingMessageBoxDirective = "using MessageBox = iNKORE.UI.WPF.Modern.Controls.MessageBox;";

        #region Example 1

        static readonly MessageBoxPayload Msg1 = new MessageBoxPayload()
        {
            Message = "But nobody came.",
        };

        private void Button_ShowMsg1_Click(object sender, RoutedEventArgs e)
        {
            Msg1.Show();
        }

        public string Example1CS => Msg1.ToCode();

        #endregion

        #region Example 2

        static readonly MessageBoxPayload Msg2Warning = new MessageBoxPayload()
        {
            Message = "IN THIS WORLD, IT'S KILL OR BE KILLED.",
            Title = "Your Best Friend",
            Image = new MsgBoxIcon(MessageBoxImage.Warning),
        };

        static readonly MessageBoxPayload Msg2Info = new MessageBoxPayload()
        {
            Message = "\"Friendship\" is just a hot person's way of making you their slave.",
            Title = "You Wanna Find Out",
            Image = new MsgBoxIcon(MessageBoxImage.Information)
        };

        static readonly MessageBoxPayload Msg2Question = new MessageBoxPayload()
        {
            Message = "Are you sure to destroy the world with this nuclear bomb? You're gonna pay for it someday.",
            Title = "Wanna Have A Bad Time?",
            Image = new MsgBoxIcon(MessageBoxImage.Information),
            Button = MessageBoxButton.YesNo
        };

        static readonly MessageBoxPayload Msg2Error = new MessageBoxPayload()
        {
            Message = "You can't understand how this feels. Knowing that one day, without any warning, it's all going to be reset.",
            Title = "The End",
            Image = new MsgBoxIcon(MessageBoxImage.Error)
        };

        static readonly MessageBoxPayload Msg2FontIconData = new MessageBoxPayload()
        {
            Message = "You never gained any LOVE, but you gained love. Does that even make sense?",
            Title = "What is LOVE?",
            Image = new MsgBoxIcon(SegoeFluentIcons.Heart, "SegoeFluentIcons.Heart"),
        };

        private void Button_ShowMsg2_Warning_Click(object sender, RoutedEventArgs e)
        {
            Msg2Warning.Show();
        }

        private void Button_ShowMsg2_Info_Click(object sender, RoutedEventArgs e)
        {
            Msg2Info.Show();
        }

        private void Button_ShowMsg2_Question_Click(object sender, RoutedEventArgs e)
        {
            Msg2Question.Show();
        }

        private void Button_ShowMsg2_FontIconData_Click(object sender, RoutedEventArgs e)
        {
            Msg2FontIconData.Show();
        }

        private void Button_ShowMsg2_Error_Click(object sender, RoutedEventArgs e)
        {
            Msg2Error.Show();
        }


        static readonly MessageBoxPayload[] Example2Data =
        [
            Msg2Warning, Msg2Info, Msg2Question, Msg2Error, Msg2FontIconData
        ];

        public string Example2CS => string.Join("\n", Example2Data.Select(data => data.ToCode()));


        #endregion

        #region Example 3

        static readonly MessageBoxPayload Msg3YesNo = new MessageBoxPayload()
        {
            Message = "This ghost keeps saying \"Z\" out loud repeatedly, pretending to sleep. Do you want to move it with force?",
            Title = "The Ruins",
            Image = new MsgBoxIcon(MessageBoxImage.Question),
            Button = MessageBoxButton.YesNo
        };

        static readonly MessageBoxPayload Msg3YesNoCancel = new MessageBoxPayload()
        {
            Message = "Do you want to save the changes you made to this file?",
            Title = "Macrohard Windoze",
            Image = new MsgBoxIcon(MessageBoxImage.Question),
            Button = MessageBoxButton.YesNoCancel
        };

        static readonly MessageBoxPayload Msg3OKCancel = new MessageBoxPayload()
        {
            Message = "If by any chance you have any unfinished business, please do what you must.",
            Title = "The Is It",
            Image = new MsgBoxIcon(MessageBoxImage.Question),
            Button = MessageBoxButton.OKCancel
        };

        private void Button_ShowMsg3_YesNo_Click(object sender, RoutedEventArgs e)
        {
            Msg3YesNo.Show();
        }

        private void Button_ShowMsg3_YesNoCancel_Click(object sender, RoutedEventArgs e)
        {
            Msg3YesNoCancel.Show();
        }

        private void Button_ShowMsg3_OKCancel_Click(object sender, RoutedEventArgs e)
        {
            Msg3OKCancel.Show();
        }

        static MessageBoxPayload[] Example3Data =
        [
            Msg3YesNo, Msg3YesNoCancel, Msg3OKCancel
        ];

        public string Example3CS => string.Join("\n", Example3Data.Select(data => data.ToCode()));

        #endregion

        #region Example 4

        private void RadioButtons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RadioButtons_DefaultBackdropStyle.SelectedItem == null) return;

            MessageBox.DefaultBackdropType = Enum.Parse<BackdropType>(RadioButtons_DefaultBackdropStyle.SelectedItem.ToString());
            UpdateExampleCode();
        }

        public string Example4CS => $@"
MessageBox.DefaultBackdropType = BackdropType.{MessageBox.DefaultBackdropType};
";


        #endregion


        #region Example Code

        public void UpdateExampleCode()
        {
            if (!this.IsLoaded) return;

            Example1.CSharp = Example1CS;
            Example2.CSharp = Example2CS;
            Example3.CSharp = Example3CS;
            Example4.CSharp = Example4CS;
        }


        #endregion

    }


    class MessageBoxPayload
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public MessageBoxButton Button { get; set; } = MessageBoxButton.OK;
        public MsgBoxIcon Image { get; set; } = MsgBoxIcon.None;

        public MessageBoxResult Show()
        {
            switch (Image.Type)
            {
                case MsgBoxIconType.Preset:
                    return MessageBox.Show(Message, Title, Button, Image.Value_Preset);
                case MsgBoxIconType.FontIcon:
                    return MessageBox.Show(Message, Title, Button, Image.Value_FontIcon);
                case MsgBoxIconType.IconSource:
                    return MessageBox.Show(Message, Title, Button, Image.Value_IconSource);
                default:
                    return MessageBox.Show(Message, Title, Button);
            }
        }

        public string ToCode()
        {
            var method = "MessageBox.Show";
            var c_msg = Message.Replace("\"", "\\\"");
            if (!string.IsNullOrEmpty(Message) && string.IsNullOrEmpty(Title) && Button == MessageBoxButton.OK && Image.IsEmpty)
            {
                return method + $"(\"{c_msg}\");";
            }
            else if (Button == MessageBoxButton.OK && Image.IsEmpty)
            {
                return method + $"(\"{c_msg}\", \"{Title}\");";
            }
            else if (Image.IsEmpty)
            {
                return method + $"(\"{c_msg}\", \"{Title}\", MessageBoxButton.{Button});";
            }
            else
            {
                return method + $"(\"{c_msg}\", \"{Title}\", MessageBoxButton.{Button}, {Image});";
            }
        }
    }

    struct MsgBoxIcon
    {
        public MsgBoxIconType Type { get; private set; }

        public MessageBoxImage Value_Preset { get; private set; }
        public FontIconData Value_FontIcon { get; private set; }
        public string Value_FontIconInput { get; private set; }
        public IconSource Value_IconSource { get; private set; }
        public string Value_IconSourceInput { get; private set; }

        public bool IsEmpty => Type == MsgBoxIconType.None || (Type == MsgBoxIconType.Preset && Value_Preset == MessageBoxImage.None);

        public MsgBoxIcon()
        {
            Type = MsgBoxIconType.None;
        }

        public MsgBoxIcon(MessageBoxImage preset)
        {
            Type = MsgBoxIconType.Preset;
            Value_Preset = preset;
        }

        public MsgBoxIcon(FontIconData fontIcon, string input)
        {
            Type = MsgBoxIconType.FontIcon;
            Value_FontIcon = fontIcon;
            Value_FontIconInput = input;
        }

        public MsgBoxIcon(IconSource iconSource, string input)
        {
            Type = MsgBoxIconType.IconSource;
            Value_IconSource = iconSource;
            Value_IconSourceInput = input;
        }

        public override string ToString()
        {
            switch (Type)
            {
                case MsgBoxIconType.Preset:
                    return "MessageBoxImage." + Value_Preset.ToString();
                case MsgBoxIconType.FontIcon:
                    return Value_FontIconInput;
                case MsgBoxIconType.IconSource:
                    return Value_IconSourceInput;
                default:
                    return "MessageBoxImage.None";
            }
        }

        public static MsgBoxIcon None = new MsgBoxIcon();
    }

    enum MsgBoxIconType
    {
        Preset,
        None,
        FontIcon,
        IconSource
    }
}
