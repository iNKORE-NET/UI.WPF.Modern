# TabControl Migration Guide

This guide helps you migrate from the old TabControl API to the new refactored API introduced in this PR.

## Overview of Changes

This refactoring brings two major improvements:

1. **Event System Refactoring**: Replaced custom event handler with standard WPF routed events for better flexibility and consistency with WPF patterns.
2. **Close Button Visibility Control**: Introduced `ui:TabItemHelper.IsClosable` attached property for per-tab control, replacing the previous reliance on `ui:TabControlHelper.IsAddTabButtonVisible`.

These changes ensure maximum flexibility, clearer architecture, and consistency with Windows UI behaviors.

---

## Breaking Changes

### 1. Event Renaming

**Old API:**
- Event: `TabControlHelper.TabItemClosing`
- Handler methods: `AddTabItemClosingHandler()` / `RemoveTabItemClosingHandler()`

**New API:**
- Event: `TabControlHelper.TabCloseRequested`
- Handler methods: `AddTabCloseRequestedHandler()` / `RemoveTabCloseRequestedHandler()`

### 2. Tab-Level Close Event

**New Addition:**
- Event: `TabItemHelper.CloseRequested`
- Handler methods: `AddCloseRequestedHandler()` / `RemoveCloseRequestedHandler()`

This new event allows you to handle close requests at the individual tab level, providing more granular control.

### 3. Close Button Visibility

**Old Behavior:**
- Close button visibility was controlled by `ui:TabControlHelper.IsAddTabButtonVisible` on the TabControl.
- When set to `false`, it would hide both the add button AND the close buttons on all tabs.

**New Behavior:**
- Close button visibility is now controlled by `ui:TabItemHelper.IsClosable` on each individual TabItem.
- Default value is `true`.
- `ui:TabControlHelper.IsAddTabButtonVisible` now only controls the add (+) tab button visibility.

### 4. Manual Tab Removal Required

**Important:** The close button no longer automatically removes tabs. You **must** handle the close event and manually remove the tab or its corresponding data item from the item source.

---

## Migration Steps

### Step 1: Update Event Subscriptions in XAML

**Before:**
```xml
<TabControl ui:TabControlHelper.TabItemClosing="TabControl_TabItemClosing">
    <!-- tabs -->
</TabControl>
```

**After:**
```xml
<TabControl ui:TabControlHelper.TabCloseRequested="TabControl_TabCloseRequested">
    <!-- tabs -->
</TabControl>
```

### Step 2: Update Event Handlers in Code-Behind

**Before:**
```csharp
private void TabControl_TabItemClosing(object sender, TabViewTabCloseRequestedEventArgs e)
{
    // Event was named TabItemClosing
    // Tabs were automatically removed after this event
}
```

**After:**
```csharp
private void TabControl_TabCloseRequested(object sender, TabViewTabCloseRequestedEventArgs e)
{
    // You MUST manually remove the tab to close it
    if (sender is TabControl tabControl)
    {
        tabControl.Items.Remove(e.Tab);
    }
}
```

### Step 3: Update Programmatic Event Subscriptions

**Before:**
```csharp
TabControlHelper.AddTabItemClosingHandler(myTabControl, OnTabClosing);
```

**After:**
```csharp
TabControlHelper.AddTabCloseRequestedHandler(myTabControl, OnTabCloseRequested);
```

### Step 4: Update Close Button Visibility Control

**Before:**
```xml
<!-- Hide close buttons by hiding the add button -->
<TabControl ui:TabControlHelper.IsAddTabButtonVisible="False">
    <TabItem Header="Tab 1" />
    <TabItem Header="Tab 2" />
</TabControl>
```

**After:**
```xml
<!-- Control add button and close buttons independently -->
<TabControl ui:TabControlHelper.IsAddTabButtonVisible="True">
    <!-- This tab has a close button (default) -->
    <TabItem Header="Tab 1" />
    
    <!-- This tab does NOT have a close button -->
    <TabItem Header="Tab 2" ui:TabItemHelper.IsClosable="False" />
</TabControl>
```

### Step 5: Handle Close Logic Explicitly

You now have full control over tab closing behavior. Here are common patterns:

#### Simple Close (Always Allow)
```csharp
private void TabControl_TabCloseRequested(object sender, TabViewTabCloseRequestedEventArgs e)
{
    if (sender is TabControl tabControl)
    {
        tabControl.Items.Remove(e.Tab);
    }
}
```

#### Conditional Close (With Confirmation)
```csharp
private void TabControl_TabCloseRequested(object sender, TabViewTabCloseRequestedEventArgs e)
{
    if (e.Tab.Tag is "RequiresConfirmation")
    {
        var result = MessageBox.Show(
            "Do you want to close this tab?", 
            "Confirm", 
            MessageBoxButton.YesNo);
            
        if (result != MessageBoxResult.Yes)
        {
            // Don't remove the tab - user cancelled
            return;
        }
    }
    
    if (sender is TabControl tabControl)
    {
        tabControl.Items.Remove(e.Tab);
    }
}
```

#### Prevent Close (Unclosable Tab)
```csharp
private void TabControl_TabCloseRequested(object sender, TabViewTabCloseRequestedEventArgs e)
{
    if (e.Tab.Tag is "DoNotClose")
    {
        // Simply don't remove the tab
        MessageBox.Show("This tab cannot be closed!");
        return;
    }
    
    if (sender is TabControl tabControl)
    {
        tabControl.Items.Remove(e.Tab);
    }
}
```

#### Using Tab-Level Event (New Feature)
```csharp
// In your Window constructor or initialization
TabItemHelper.AddCloseRequestedHandler(myTabItem, OnTabItemCloseRequested);

private void OnTabItemCloseRequested(object sender, TabViewTabCloseRequestedEventArgs e)
{
    // Handle close for this specific tab
    if (MessageBox.Show("Close this tab?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
    {
        var tabControl = (sender as TabItem)?.Parent as TabControl;
        tabControl?.Items.Remove(sender);
    }
}
```

---

## Working with Data-Bound TabControls

If you're using `ItemsSource` binding (MVVM pattern), remove the data item instead of the TabItem:

```csharp
private void TabControl_TabCloseRequested(object sender, TabViewTabCloseRequestedEventArgs e)
{
    if (sender is TabControl tabControl && 
        tabControl.ItemsSource is IList collection)
    {
        // Remove the data item, not the TabItem
        collection.Remove(e.Item);
    }
}
```

The `TabViewTabCloseRequestedEventArgs` provides both:
- `e.Tab` - The TabItem control
- `e.Item` - The underlying data item (when using ItemsSource)

---

## Summary Checklist

- [ ] Replace `TabItemClosing` event with `TabCloseRequested` event in XAML
- [ ] Update event handler names in code-behind
- [ ] Add manual tab removal logic in event handlers
- [ ] Replace programmatic event subscriptions with new method names
- [ ] If you were using `IsAddTabButtonVisible="False"` to hide close buttons, replace with `ui:TabItemHelper.IsClosable="False"` on individual tabs
- [ ] Test that tabs close as expected
- [ ] Consider using the new `TabItemHelper.CloseRequested` event for tab-specific close logic

---

## Benefits of the New API

1. **Explicit Control**: You have full control over when and how tabs are closed
2. **Granular Configuration**: Control close button visibility per tab, not globally
3. **Better Separation**: Add button and close button visibility are now independent
4. **Flexible Event Handling**: Choose between TabControl-level or TabItem-level event handling
5. **Consistent with WPF Patterns**: Uses standard routed events instead of custom event handlers
6. **Consistent with Windows UI**: Matches the behavior of modern Windows applications

---

## Need Help?

If you encounter issues during migration:
1. Check the example in `source/iNKORE.UI.WPF.Modern.Gallery/Pages/Controls/Windows/TabViewPage.xaml`
2. Review the code-behind at `TabViewPage.xaml.cs`
3. Open an issue on GitHub if you need additional assistance
