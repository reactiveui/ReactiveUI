# XAML

Implement `IViewFor<T>` by hand and ensure that ViewModel is a DependencyProperty.  
Also, always dispose bindings view `WhenActivated`, or else the bindings leak memory.
  
The goal in this example is to two-way bind the `TheText` property of the
ViewModel to the TextBox and one-way bind the `TheText` property to the TextBlock, 
so the TextBlock updates when the user types text into the TextBox.
  
```csharp
public class TheViewModel : ReactiveObject
{
    private string theText;
    
    public string TheText
    {
        get { return this.theText; }
        set { this.RaiseAndSetIfChanged(ref this.theText, value); }
    }
}
```

```xml
<Window /* snip */>
  <StackPanel>
    <TextBox x:Name="TheTextBox" />
    <TextBlock x:Name="TheTextBlock" />
  </StackPanel>
</Window>
```

```csharp
public partial class TheView : IViewFor<TheViewModel>
{
    public TheView()
    {
        InitializeComponent();
        
        ViewModel = new TheViewModel();
        
        // Setup the bindings
        // Note: We have to use WhenActivated here, since we need to dispose the
        // bindings on XAML-based platforms, or else the bindings leak memory.
        this.WhenActivated(d =>
        {
            d(this.Bind(this.ViewModel, x => x.TheText, x => x.TheTextBox.Text));
            d(this.OneWayBind(this.ViewModel, x => x.TheText, x => x.TheTextBlock.Text));
        });
    }

    object IViewFor.ViewModel
    {
        get { return ViewModel; }
        set { ViewModel = (TheViewModel)value; }
    }

    public TheViewModel ViewModel
    {
        get { return (TheViewModel)GetValue(ViewModelProperty); }
        set { SetValue(ViewModelProperty, value); }
    }

    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register("ViewModel", typeof(TheViewModel), typeof(TheView));
}
```
