# Dependency Resolution

Dependency resolution is a feature built into the core framework, which allows
libraries and ReactiveUI itself to use classes that are in other libraries
without taking a direct reference to it. This is quite useful for
cross-platform applications, as it allows portable code to use non-portable
APIs, as long as they can be described via an Interface.

ReactiveUI's Dependency Resolution can more properly be called the Service
Location pattern. Put thought into how you use this API, as it can either be
used effectively to make code more testable, or when used poorly, makes code
more difficult to test and understand, as the [Resolver itself can effectively
become part of the class's
state](http://blog.ploeh.dk/2010/02/03/ServiceLocatorisanAnti-Pattern/), but in an implicit and non-obvious way.

### Using Dependency Resolution

At its simplest, ReactiveUI provides the following methods to resolve
services (note that this is the simplified version, not the actual definition):

```cs
public interface IDependencyResolver
{
    // Returns the most recent service registered to this type and contract
    T GetService<T>(string contract = null);

    // Returns all of the services registerd to this type and contract
    IEnumerable<T> GetServices<T>(string contract = null)
}
```

Given a type T (usually an interface), you can now recieve an implementation
of T. If the T registered is very common ("string" for example), or you want
to distinguish by a method other than type, you can use the "contract"
parameter which is an arbitrary key that you provide.

The current Resolver that ReactiveUI itself will use (as well as what your app
should use as well), is provided at `RxApp.DependencyResolver`.

### Registering new dependencies

The default implementation of `RxApp.DependencyResolver` also implements
another interface (accessible via the convenience property
`RxApp.MutableResolver`):

```cs
public interface IMutableDependencyResolver : IDependencyResolver
{
    void Register(Func<object> factory, Type serviceType, string contract = null);
}
```

This resolver allows you to register new implementations for interfaces. This
is usually done on app startup (on Cocoa, in `AppDelegate`, or on WPF, in
`App`).

This design seems overly simplistic, but in fact, can represent most of the
useful lifetime scopes that we would want to use in a desktop / mobile
application. For example:

```cs
var r = RxApp.MutableResolver;

// Create a new instance every time
r.Register(() => new FooBar(), typeof(IFooBar));

// Return a singleton instance
var foobar = new FooBar();
r.Register(() => foobar, typeof(IFooBar));

// Return a singleton instance but delay its creation until first requested.
var foobar = new Lazy<FooBar>();
r.Register(() => foobar.Value, typeof(IFooBar));
```

### Common Cross-Platform Patterns

Dependency resolution is very useful for moving logic that would normally have
to be in platform-specific code, into the shared platform code. First, we need
to define an Interface for something that we want to use - this example isn't
a Best Practice, but it's illustrative.

```
public interface IYesNoDialog
{
    // Returns 'true' if yes, 'false' if no.
    IObservable<bool> Prompt(string title, string description);
}
```

Now this interface can be used in a ViewModel:

```cs
public class MainViewModel
{
    public ReactiveCommand DeleteData { get; protected set; }

    public MainViewModel(IYesNoDialog dialogFactory = null)
    {
        // If the constructor hasn't passed in its own implementation,
        // use one from the resolver. This makes it easy to test DeleteData
        // via providing a dummy implementation.
        dialogFactory = dialogFactory ?? RxApp.DependencyResolver.GetService<IYesNoDialog>();
        DeleteData = new ReactiveCommand();

        var title = "Delete the data?";
        var desc = "Should we delete your important Data?";

        DeleteData.RegisterAsync(() => dialogFactory.Prompt(title, desc))
            .Where(x => x == true)
            .SelectMany(async x => DeleteTheData())
            .Subscribe(
                x => this.Log().Info("Deleted the Data"), 
                ex => this.Log().WarnException(ex, "Couldn't delete the data"));
    }
}
```

Now, our implementations could be very different between iOS and Android -
here's a sample iOS implementation:

```cs
public class AlertDialog : IYesNoDialog
{
    public IObservable<bool> Prompt(string title, string description)
    {
        var dlgDelegate = new UIAlertViewDelegateRx();
        var dlg = new UIAlertView(title, description, dlgDelegate, "No", "Yes");
        dlg.Show();

        return dlgDelegate.ClickedObs
            .Take(1)
            .Select(x => x.Item2 == 1);
    }
}
```

### ModernDependencyResolver and Resolver Initialization

The default implementation of `IDependencyResolver` in ReactiveUI is a public
class called `ModernDependencyResolver`. To initialize this class or any other
`IMutableDependencyResolver` implementation with the implementations that
ReactiveUI requires to function, call the `InitializeResolver` extension
method.
