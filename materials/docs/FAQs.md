
# Inkore.UI.WPF.Modern FAQs

> 
> This file contains some frequently asked qustions.
>
> If you have a problem, please report an issue or contact us (yoojun@inkore.net)
> 

## ❓ IconElement: Display as rectangle or question mark under Windows 7

- In some operating systems like Windows 7, the `FontIcon`, `SymbolIcon` cannot display correctly. The characters display as `?` or a block.

  在某些操作系统 (比如 Windows 7) 下，字体图标无法正常显示，字符被渲染为 `?` 或方块。

- This is because the font `Segoe Fluent Icons` is missing. To solve this problem, just directly install this font to your computer.

   这是因为系统缺少 'Segoe Fluent Icons' 字体。要解决此问题，安装该字体即可。单击上方链接下载 SegoeIcons.ttf 文件。

2. Open the file and click 'Install'.

   打开此文件，并单击 '安装'。

3. Restart your program or IDE.

   重新启动你的软件或 IDE。

## ❓ FontIcon: Icon size is incorrect in NavigationViewItem

- If the size of FontIcon comes too much to be in the NavigationViewItem like this


- You can use `FontSize="16"` to set a specific value for it, the default value is 20.

   ```xml
  <ui:NavigationViewItem Content="Home">
      <ui:NavigationViewItem.Icon>
          <ui:FontIcon Glyph="{x:Static ui:SegoeIcons.Home}" FontSize="16"/>
      </ui:NavigationViewItem.Icon>
  </ui:NavigationViewItem>
   ```

## ❓ FontIcon: How to add icons from segoe icons library elegantly

- This library provides a bunch of icons definitions in SegoeIcons class.

- We recommend you to use FontIcon as your icon element. Here is a sample:

   ```xml
   <ui:FontIcon Glyph="{x:Static ui:SegoeIcons.Home}" FontSize="16"/>
   ```

- The fields in SegoeIcons class are strings that infers to the icon, you need to use them as Glyph values of FontIcon element. 

- In the example above, icon 'Home' is used, you can use other icons in that similar form of `{x:Static ui:SegoeIcons.***}`.


## ❓ HyperlinkButton: Click event triggers twice every one click

- If the click of hyperlink button is triggered twice every time you clicked, please set the property **RaiseHyperlinkClicks** to **false**. Here is a sample:

   ```xml
   <ui:HyperlinkButton Content="Sample link" RaiseHyperlinkClicks="False" Click="HyperlinkButton_Click"/>
   ```

- When RaiseHyperlinkClicks is to false, the click event will only trigger once, but if you are using NavigateUri property instead of click event, please keep RaiseHyperlinkClicks true.

## ❓ Window: Exception thrown with UseModernWindowStyle on

- If an exception was thrown when a window with UseModernWindowStyle on, sth like this:

   ```
   托管调试助手 "PInvokeStackImbalance":“对 PInvoke 函数 “Inkore.UI.WPF.Modern!Inkore.UI.WPF.Modern.Controls.Primitives.MaximizedWindowFixer::GetWindowPlacement” 的调用导致堆栈不对称。原因可能是托管的 PInvoke 签名与非托管的目标签名不匹配。请检查 PInvoke 签名的调用约定和参数与非托管的目标签名是否匹配。”
   ```

- Theoretically, this exception can be ignored and won't terminate the application. It's caused by an internal error in Microsoft's API.

- However, you can manually turn the MaximizedWindowFixer to fix this problem by applying this:

  ```
  ui:WindowHelper.FixMaximizedWindow="False"
  ```

## ❓ Window: Maximize button doesn't work with touch or stylus

- With touch or stylus, after clicking the maximize button in a window, if the window jumps back immediately from maximized to normal, please consider applying this:

  ```
  ui:TitleBar.MaximizeButtonTouchOptimize="True"
  ```

- Turning this on will disable the snap layout menu (which displays when hovering the maximize button and shows some options for creating split). So make your own trade-offs.