# View Location + IViewFor

View Location is a feature of ReactiveUI that allows you to associate Views with
ViewModels and set them up Automagically.

### ViewModelViewHost

The easiest way to use View Location is via the `ViewModelViewHost` control,
which is a View (on Cocoa, a UIView/NSView, and on XAML-based platforms a
Control) which has a single `ViewModel` property. When the ViewModel property is
set, View Location looks up the associated View and loads it into the container.
`ViewModelViewHost` is great for lists - so much so, that if you Bind to
`ItemsSource` on XAML-based platforms and don't set a DataTemplate, one gets
configured that just uses `ViewModelViewHost`.

```xml
<ListBox x:Name="ToasterList" />
```

```cs
// Now ListBox automatically gets a DataTemplate
this.OneWayBind(ViewModel, vm => vm.ToasterList, v => v.ToasterList.ItemsSource);
```

### Registering new Views

To use View Location, you must first register types, via Splat's Service Location feature.

```cs
Locator.CurrentMutable.Register(() => new ToasterView(), typeof(IViewFor<ToasterViewModel>));
```

View Location internally uses a class called `ViewLocator` which can either be
replaced, or the default one used. The `ResolveView` method will return the View
associated with a given ViewModel object.


### Registering multiple views for a single ViewModel
In order to register multiple views for the same ViewModel the contract parameter must be used on registration.  Here I register 2 views with 2 contracts. Landscape and Portrait. There is no significance to the contract name. I just used these to be descriptive.

```cs
Locator.CurrentMutable.Register(() => new LandscapeLayoutView(), typeof(IViewFor<MainLayoutViewModel>), "Landscape");
Locator.CurrentMutable.Register(() => new PortraitLayoutView(), typeof(IViewFor<MainLayoutViewModel>), "Portrait");

```

To change your view (note that the view must be displayed in a `ViewModelViewHost`),create a new `ViewContractObservable` for the `ViewModelViewHost`.

For example, if I have a `ViewModelViewHost` named mainPanel declared in XAML, then I could setup changing its displayed view like this:

```cs
public partial class ShellView : Window, IViewFor<ShellViewModel>
{
  var mainLayoutChange = new BehaviorSubject<string>("Landscape"); //start with landscape

  //ViewContractObservable takes an IObservable<string>
  this.mainPanel.ViewContractObservable = mainLayoutChange;
  ...
  //to change between the views, fire the IObservable<string>
  //OnNext with a valid contract identifier.
  someSubsciption
    .Where(_=> someCondition)
    .Subscibe(_=> mainLayoutChange.OnNext("Portrait"));

  someSubsciption
    .Where(_=> someOtherCondition)
    .Subscibe(_=> mainLayoutChange.OnNext("Landscape"));
}
```
