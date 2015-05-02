# Android

* **Android:** - change your base class to one of the Reactive Activity /
  Fragment classes (i.e. ReactiveActivity<T>), *or* implement
  `INotifyPropertyChanged` on your View and ensure that ViewModel signals
  changes.

```csharp
[Activity (Label = "RxUISample-Android", MainLauncher = true)]
public class TestActivity : ReactiveActivity, IViewFor<TestViewModel>
{
    protected override async void OnCreate(Bundle bundle)
    {
        base.OnCreate(bundle);
        BlobCache.ApplicationName = "RxUISample";

        // Set our view from the "main" layout resource
        SetContentView(Resource.Layout.Main);

        ViewModel = await BlobCache.LocalMachine.GetOrCreateObject("TestViewModel", () => {
            return new TestViewModel();
        });
    }

    TestViewModel _ViewModel;
    public TestViewModel ViewModel
    {
        get { return _ViewModel; }
        set { this.RaiseAndSetIfChanged(ref _ViewModel, value); }
    }

    object IViewFor.ViewModel
    {
        get { return ViewModel; }
        set { ViewModel = (TestViewModel)value; }
    }
}
```
