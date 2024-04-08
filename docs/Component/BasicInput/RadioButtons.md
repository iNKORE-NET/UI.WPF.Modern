# RadioButtons

类型： iNKORE.UI.WPF.Modern.Controls.RadioButtons

继承：System.Windows.Controls.Control

## 属性

### ItemsSource

- 类型: `IEnumerable`
- 默认值: `null`
- 描述: 数据源，获取或设置用于生成控件内容的集合

### Items

- 类型: `IList`
- 默认值: `null`
- 描述: 生成空间内容的集合

### ItemTemplate

- 类型: `object`
- 默认值: `null`
- 描述: 获取或设置用于显示控件中每个项目的模板

### SelectedItem

- 类型: `int`
- 默认值: `-1`
- 描述: 选中的 Items 的索引值

### SelectedItem

- 类型: `object`
- 默认值: `null`
- 描述: 获取选中的 Items

### MaxColumns

- 类型: `int`
- 默认值: `1`
- 描述: 获取或设置用于布局的最大列数

### Header

- 类型: `object`
- 默认值: `null`
- 描述: 获取或设置控件标题的内容

### HeaderTemplate

- 类型: `DataTemplate`
- 默认值: `null`
- 描述: 获取或设置用于显示控件标题的模板

## 事件

- SelectionChanged当单选按钮的选择更改时发生

## 方法

- ContainerFromIndex(int index)：检索集合中与指定索引对应的 UI 元素
- SetTestHooksEnabled(boolenabled)：设置是否为控件启用测试挂钩
- GetRows() (return int)：获取控件布局中的行数
- GetColumns() (return int)：获取控件布局中的列数
- GetLargerColumns() (return int)：获取控件布局中项目数最多的列数

## 样例

```xaml
<ui:RadioButtons Header="Options:">
    <RadioButton Content="Option1"/>
    <RadioButton Content="Option2"/>
    <RadioButton Content="Option3"/>
</ui:RadioButtons>
```

![](./../../images/RadioButtons/1.gif)