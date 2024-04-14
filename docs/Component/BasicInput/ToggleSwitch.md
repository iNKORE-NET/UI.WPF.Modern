# ToggleSwitch

类型：iNKORE.UI.WPF.Modern.Controls.ToggleSwitch

继承：System.Windows.Controls.Control

## 属性

### Header:

- 类型: `object`
- 默认值: `null`
- 描述: 设置开关按钮的标题或标签。可以是任何对象，如字符串、图像或自定义控件。

### HeaderTemplate:

- 类型: `DataTemplate`
- 默认值: `null`
- 描述: 用于定义`Header`内容的数据模板。通过这个属性，可以对`Header`进行更复杂的定制和布局。

### IsOn:
   - 类型: `bool`
   - 默认值: `false`
   - 描述: 指示开关按钮当前的状态，`true` 表示开启，`false` 表示关闭。该属性支持双向数据绑定，当状态发生变化时会触发`Toggled`事件。
### OffContent:
   - 类型: `object`
   - 默认值: `"Off"`
   - 描述: 设置开关按钮关闭状态时显示的内容。可以是任何对象，如字符串、图像或自定义控件。
### OffContentTemplate:
   - 类型: `DataTemplate`
   - 默认值: `null`
   - 描述: 用于定义`OffContent`内容的数据模板。通过这个属性，可以对`OffContent`进行更复杂的定制和布局。
### OnContent:
   - 类型: `object`
   - 默认值: `"On"`
   - 描述: 设置开关按钮打开状态时显示的内容。可以是任何对象，如字符串、图像或自定义控件。
### OnContentTemplate:
   - 类型: `DataTemplate`
   - 默认值: `null`
   - 描述: 用于定义`OnContent`内容的数据模板。通过这个属性，可以对`OnContent`进行更复杂的定制和布局。
### UseSystemFocusVisuals:
   - 类型: `bool`
   - 默认值: `false`
   - 描述: 指示是否使用系统提供的焦点可视化样式。如果为 `true`，开关按钮获得焦点时会显示系统默认的焦点可视化效果。
### FocusVisualMargin:
   - 类型: `Thickness`
   - 默认值: 自动计算的值
   - 描述: 指定焦点可视化效果的边距。可以设置上、下、左、右边距的大小。
### CornerRadius:
   - 类型: `CornerRadius`
   - 默认值: `0`
   - 描述: 设置开关按钮的圆角半径。可以分别设置四个角的半径大小，也可以统一设置所有角的半径大小。

## 事件

- Toggled；当开关状态发生改变时，会触发这个事件

## 样例