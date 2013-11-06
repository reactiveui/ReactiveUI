# Basic Property Binding 

A core part of being able to use the MVVM pattern is the very specific
relationship between the ViewModel and View - that is, the View is connected
in a one-way dependent manner to the ViewModel via *bindings*. 

ReactiveUI provides its own implementation of this concept, which has a number
of advantages compared to platform-specific implementations such as XAML-based
bindings.

* Bindings work on **all platforms** and operate the same.
* Bindings are written via Expressions. This means that renaming a
  control in the UI layout without updating a binding, the build will fail.
* Controlling how types bind to properties is flexible and can be customized.

### Getting Started

In order to use bindings in the View, you must first implement
`IViewFor<TViewModel>` on your View. Depending on the platform, you must
implement it differently:

* **iOS** - change your base class to one of the Reactive UIKit classes (i.e.
  ReactiveUIViewController) and implement `ViewModel` using
  RaiseAndSetIfChanged, *or* implement `INotifyPropertyChanged` on your View and
  ensure that ViewModel signals changes.

* **Android:** - change your base class to one of the Reactive Activity /
  Fragment classes (i.e. ReactiveActivity<T>), *or* implement
  `INotifyPropertyChanged` on your View and ensure that ViewModel signals
  changes.

* **Xaml-based:** - Implement `IViewFor<T>` by hand and ensure that ViewModel
  is a DependencyProperty.

### Types of Bindings

Once you implement `IViewFor<T>`, binding methods are now available as
extension methods on your class. Like many other things in ReactiveUI, you
should only set up bindings in a constructor or setup method when the view is
created.

* **OneWayBind:** - Sets up a one-way binding from a property on the ViewModel
  to the View.

```cs
var disp = this.OneWayBind(ViewModel, x => x.Name, x => x.Name.Text);
disp.Dispose();   // Disconnect the binding early.
```

* **Bind:** - Sets up a two-way binding between a property on the ViewModel to
  the View.

```cs
this.Bind(ViewModel, x => x.Name, x => x.Name.Text);
```

### Converting between types

Direct bindings between properties are convenient, but often the two types are
not assignable to each other. For example, binding an "Age" property to a
TextBox would normally fail, because TextBox expects a string value. Instead,
ReactiveUI has an extensible system for coercing between types.

See the details about Binding Type Converters in the "Customization" section
for more information about how to extend property type conversion.

### "Hack" bindings and BindTo

Should you find that direct one and two-way bindings aren't enough to get the
job done (or should you want View => ViewModel bindings), a flexible, Rx-based
approach is also available, via combining `WhenAny` with the `BindTo`
operator, which allows you to bind an arbitrary `IObservable` to a property on
an object.

For example, here is a simple example of binding a ListBox's `SelectedItem` to
a ViewModel:

```cs
public MainView()
{
    // Bind the View's SelectedItem to the ViewModel
    this.WhenAny(x => x.SomeList.SelectedItem)
        .BindTo(this, x => x.ViewModel.SelectedItem);

    // Bind ViewModel's IsSelected via SelectedItem. Note that this
    // is only for illustrative purposes, it'd be better to bind this
    // at the ViewModel layer (i.e. WhenAny + ToProperty)
    this.WhenAny(x => x.SomeList.SelectedItem)
        .Select(x => x != null)
        .BindTo(this, x => x.ViewModel.IsSelected);
}
```

BindTo applies the same binding hooks and type conversion that other property
binding methods do, so the types don't necessarily have to match between the
source and the target property.

While you could certainly build complex bindings (even ones between two view
models!), keep in mind that binding logic that you put in the View is
untestable, so keeping the meaningful logic out of bindings is usually a Good
Idea.
