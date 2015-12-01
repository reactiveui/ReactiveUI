# Android

Change your base class to one of the Reactive Activity / Fragment classes 
(i.e. ReactiveActivity<T>), *or* implement `IViewFor` on your View 
and ensure that your ViewModel signals changes.
  
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
// You can derive from ReactiveActivity<T> if you don't want to re-implement IViewFor<T>
[Activity (Label = "RxUISample-Android", MainLauncher = true)]
public class TestActivity : ReactiveActivity, IViewFor<TheViewModel> 
{    
    protected override void OnCreate(Bundle bundle)
    {
        base.OnCreate(bundle);
        
        // Set our view from the "main" layout resource
        SetContentView(Resource.Layout.Main);

        ViewModel = new TheViewModel();
        
        // WireUpControls looks through your layout file, finds all controls 
        // with an id defined, and binds them to the controls defined in this class
        // This is basically the same functionality as http://jakewharton.github.io/butterknife/ provides
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
