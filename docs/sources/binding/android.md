# Android

Change your base class to one of the Reactive Activity / Fragment classes 
(i.e. ReactiveActivity<T>), *or* implement `INotifyPropertyChanged` on your View 
and ensure that ViewModel signals changes.
  
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
<?xml version="1.0" encoding="utf-8"?>
<LinearLayout xmlns:android="http://schemas.android.com/apk/res/android"
  android:orientation="vertical"
  android:layout_width="fill_parent"
  android:layout_height="fill_parent">
  <EditText
    android:id="@+id/TheEditText"
    android:layout_width="match_parent"
    android:layout_height="match_parent" />
  <TextView
    android:id="@+id/TheTextView"
    android:layout_width="match_parent"
    android:layout_height="match_parent" />
</LinearLayout>
```

```csharp
[Activity (Label = "RxUISample-Android", MainLauncher = true)]
public class TestActivity : ReactiveActivity, 
  IViewFor<TheViewModel> // Or use ReactiveActivity<TheViewModel>, which implements IViewFor<TheViewModel>
{    
    protected override void OnCreate(Bundle bundle)
    {
        base.OnCreate(bundle);
        
        // Set our view from the "main" layout resource
        SetContentView(Resource.Layout.Main);

        ViewModel = new TheViewModel();
        
        // Wire up the controls defined in our layout file, to the control properties in this class
        this.WireUpControls();
        
        this.Bind(this.ViewModel, x => x.TheText, x => x.TheEditText.Text);
        this.OneWayBind(this.ViewModel, x => x.TheText, x => x.TheTextView.Text);
    }
    
    public EditText TheEditText { get; private set; }
    
    public TextView TheTextView { get; private set; }

    TheViewModel _ViewModel;
    public TheViewModel ViewModel
    {
        get { return _ViewModel; }
        set { this.RaiseAndSetIfChanged(ref _ViewModel, value); }
    }

    object IViewFor.ViewModel
    {
        get { return ViewModel; }
        set { ViewModel = (TheViewModel)value; }
    }
}
```
