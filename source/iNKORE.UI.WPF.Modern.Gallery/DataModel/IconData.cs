// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace iNKORE.UI.WPF.Modern.Gallery.DataModel
{
    public class IconData
    {
        public string Name { get; set; }
        public string Code { get; set; }
        // Which icon set this icon came from (e.g. "SegoeFluentIcons", "FluentSystemIcons.Regular")
        public string Set { get; set; }
        public string[] Tags { get; set; } = Array.Empty<string>();
        public bool IsSegoeFluentOnly { get; set; }
        // The actual font to use for rendering this glyph (important for Fluent System Icons)
        public FontFamily FontFamily { get; set; }

        public string Character
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Code)) return string.Empty;
                try
                {
                    int value = Convert.ToInt32(Code, 16);
                    return char.ConvertFromUtf32(value);
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        public string CodeGlyph => string.IsNullOrWhiteSpace(Code) ? string.Empty : "\\u" + Code;
        public string TextGlyph => string.IsNullOrWhiteSpace(Code) ? string.Empty : "&#x" + Code + ";";

        // WPF doesn't have Symbol enum like WinUI
        public string SymbolName => null;
    }
}
