# XAML
* **Xaml-based:** - Implement `IViewFor<T>` by hand and ensure that ViewModel
  is a DependencyProperty.

```csharp
public partial class ShellView : IViewFor<ShellViewModel>
{
    public ShellView()
    {
        InitializeComponent();
        ViewModel = new ShellViewModel();
    }

    object IViewFor.ViewModel
    {
        get { return ViewModel; }
        set { ViewModel = (ShellViewModel)value; }
    }

    public ShellViewModel ViewModel
    {
        get { return (ShellViewModel)GetValue(ViewModelProperty); }
        set { SetValue(ViewModelProperty, value); }
    }

    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register("ViewModel", typeof(ShellViewModel), typeof(ShellView));
}
```
