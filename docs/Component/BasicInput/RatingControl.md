# RatingControl

类型：iNKORE.UI.WPF.Modern.Controls.RatingControl

继承：System.Windows.Controls.Control

## 属性

### Caption

- 类型：`string`
- 默认值：`null`
- 描述：控件的标题，显示在控件旁边

### InitialSetValue

- 类型：`double`
- 默认值：`1`
- 描述：控件的初始值

### IsClearEnabled

- 类型：`bool`
- 默认值：`false`
- 描述：控件是否支持清除评级

### IsReadOnly

- 类型：`bool`
- 默认值：`false`
- 描述：控件是否只读

### ItemInfo

- 类型：`RatingItemInfo`
- 默认值：`null`
- 描述：控件的评级项信息
- 注意事项：没啥给用户用的

### MaxRating

- 类型：`double`
- 默认值：`5`
- 描述：最大评分

### PlaceholderValue

- 类型：`double`
- 默认值：`-1`
- 描述：占位值

### Value

- 类型：`double`
- 默认值：`this.PlaceholderValue`
- 描述：控件的分值

## 样例

与 `slider` 双向绑定

```xaml
<StackPanel Orientation="Vertical"  VerticalAlignment="Center">
	<Grid Margin="5">
		<ui:RatingControl PlaceholderValue="{Binding Value, ElementName=slider, Mode=TwoWay}"/>
	</Grid>
	<Grid Margin="5">
		<Slider x:Name="slider" Width="150" VerticalAlignment="Center" Maximum="5"></Slider>
	</Grid>
</StackPanel>
```

![](./../../images/RatingControl/1.gif)

## 参考

1. [Control Class (System.Windows.Controls) | Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/api/system.windows.controls.control?view=windowsdesktop-8.0)
