# Inkore.UI.WPF.Modern FAQs

## ❓IconElement: Display as rectangle or question mark under Windows 7

- In some operating systems like Windows 7, the `FontIcon`, `SymbolIcon` cannot display correctly.

- The characters display as `?` or a block.

  在某些操作系统 (比如 Windows 7) 下，字体图标无法正常显示，字符被渲染为 `?` 或方块。

    ![](https://github.com/InkoreStudios/UI.WPF.Modern/blob/main/docs/images/screenshot_1.png?raw=true)


- This is because the font `Segoe Fluent Icons` is missing.

- To solve this problem, just directly install this font to your computer.

  这是因为系统缺少 'Segoe Fluent Icons' 字体。要解决此问题，安装该字体即可

1. Download [`SegoeIcons.ttf`](https://github.com/InkoreStudios/UI.WPF.Modern/raw/main/assets/fonts/SegoeIcons.ttf) to your computer.

   单击上方链接下载 SegoeIcons.ttf 文件。

2. Open the file and click 'Install'.

   打开此文件，并单击 '安装'。

   ![](https://github.com/InkoreStudios/UI.WPF.Modern/blob/main/docs/images/screenshot_2.png?raw=true)

3. Restart your program or IDE.

   重新启动你的软件或 IDE。