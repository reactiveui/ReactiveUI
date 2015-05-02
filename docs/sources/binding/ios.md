# iOS

In order to use bindings in the View, you must first implement
`IViewFor<TViewModel>` on your View. Depending on the platform, you must
implement it differently:

* **iOS** - change your base class to one of the Reactive UIKit classes (i.e.
  ReactiveUIViewController) and implement `ViewModel` using
  RaiseAndSetIfChanged, *or* implement `INotifyPropertyChanged` on your View and
  ensure that ViewModel signals changes.

```csharp
public partial class NotificationsListViewController : ReactiveTableViewController, IViewFor<NotificationsListViewModel>
{
    public NotificationsListViewController()
    {
    }

    public override void ViewDidLoad()
    {
        base.ViewDidLoad();
        ViewModel = new NotificationsListViewModel();
        // ... view stuff
    }

    NotificationsListViewModel _ViewModel;
    public NotificationsListViewModel ViewModel
    {
        get { return _ViewModel; }
        set { this.RaiseAndSetIfChanged(ref _ViewModel, value); }
    }

    object IViewFor.ViewModel
    {
        get { return _ViewModel; }
        set { ViewModel = (NotificationsListViewModel)value; }
    }
}
```

# iOS Binding Helpers
ReactiveTableViewSource, ReactiveTableViewController, ReactiveTableViewCell etc. 
