page_title: ReactiveUI 6.x Series Release Notes

#Release Notes

You can view release notes for earlier version of ReactiveUI by selecting the
desired version from the drop-down list at the top right of this page.

# Version 6.0
These are the major changes that are likely to affect application developers in a way that may take more work to resolve.

*ReactiveCommand is New And Different*

ReactiveCommand is completely rewritten (again). To create ReactiveCommands, instead of using `new` you almost always want to use the ``ReactiveCommand.CreateXYZ`` family of methods depending on what kind of async method you are using:

Old:


```csharp
var someCommand = new ReactiveCommand();

var someAsyncCommand = new ReactiveCommand();
someAsyncCommand.RegisterAsyncTask(someTaskMethod);
```

New:
```
var someCommand = ReactiveCommand.Create();

var someAsyncCommand = ReactiveCommand.CreateAsyncTask(someTaskMethod);
```

* ReactiveCommand now provides an ``ExecuteAsync`` method that returns the result of the background task that was invoked. You **must** await or Subscribe to it in some way, or it does not execute (i.e. it is a Cold Observable).

As with previous versions, the old version of ReactiveCommand is provided in ``ReactiveUI.Legacy``, to help with migration.

*Scheduling Changes*

Many operations that were previously automatically scheduled to the UI thread now do not do so, such as `ToProperty`. While the old behavior made it easier for developers, it also made it much more difficult to write larger applications while ensuring responsiveness, as items would be scheduled multiple times before being displayed.

This means, you probably need to add some ``ObserveOn(RxApp.MainThreadScheduler)`` calls to your application.

*ToProperty / OAPH changes*

* ``ObservableAsPropertyHelper`` no longer is itself an ``IObservable``, use ``WhenAny`` to observe it.

* ``ObservableAsPropertyHelper`` now lazily Subscribes to the source only when the Value is read for the first time. This significantly improves performance and memory usage, but at the cost of some "Why doesn't my test
work??" confusion. If you find that your ToProperty "isn't working", this may be why.

*Suspension / ReactiveUI.Mobile*

The philosophy behind ``ReactiveUI.Mobile`` has been greatly simplified, and is no longer tied to routing in any way. The new goal of ReactiveUI.Mobile is, "The framework will save/load/create a *single object* on your behalf, and persist that throughout app suspend / resume".

This object can be of any type you want, as long as the entire object graph (i.e. this object and its children) can be serialized. This object is created from scratch when needed, via ``RxApp.SuspensionHost.CreateNewAppState`` Func that your app provides.

*Other changes*
* ``ObservableAsyncMRUCache`` is removed, you probably want to use Akavache instead, or copy-paste the code from an old release if you really want it.

* Several Routing classes that never worked properly have been removed, and routing is Generally Discouraged from being used in anything other than a WPF app.

*Easy to handle changes*

* Namespaces are much simpler now - everything is in ``ReactiveUI`` except for platform-specific functionality.

* ``ReactiveUI-Platforms`` is deprecated, as well as all of the platform-specific libraries - they are now all integrated into a single ``ReactiveUI.dll``. This means that if you are _manually_ referencing ``ReactiveUI.dll``, you should *always* pick the most specific version for your platform. If your _App_ references the PLib version of ReactiveUI.dll, you'll have a Bad Time. If you use NuGet, everything will be handled automatically.

* ``ReactiveUI.Mobile`` has moved into ``ReactiveUI-Core``, and the namespace no longer exists.

* ReactiveUI is now updated to depend on Rx 2.2.4. This shouldn't affect existing code.

* Service Location is now moved to Splat, you may have to add a ``using Splat;`` to several files in your solution.

* ``InitializeResolver`` is now a combination of ``InitializeSplat``

* ReactiveUI on iOS now **requires** Xamarin.iOS 7.2.1 or higher. You probably already have this by now.

* MemoizingMRUCache is now in Splat

* The ``Router`` property on ``IScreen`` is now of type `RoutingState` instead of `IRoutingState`.

*Find-and-replace changes*

* ``RxApp.DependencyResolver`` => ``Locator.Current``
* ``RxApp.MutableResolver`` => ``Locator.CurrentMutable``
* ``RxApp.InUnitTestRunner`` => ``ModeDetector.InUnitTestRunner``


# Version 5.0

Moving to ReactiveUI 5.0 is usually straightforward, but there are a few things to know.

* ReactiveUI 5.0 is .NET 4.5 only - this means that Silverlight 5, .NET 4.0, and WP7.x are all unsupported. If you want to use ReactiveUI with these platforms, you have to stay on the 4.x series. You can do this by changing the lines in your "packages.config" to always use the latest version from the 4.x series:


```xml
<package id="reactiveui-core" version="(4.0.0, 5.0.0)" />
```

* ReactiveCommand now does not have an imperative constructor (i.e. ``ReactiveCommand.Create``). This constructor is something that you probably shouldn't be using anyways, but if you really need it, you can find the original ReactiveCommand in the ``ReactiveUI.Legacy`` namespace.

* ReactiveUI now uses a much more simplified Service Location (i.e. IoC without injection) model than RxUI 4.x. However, this new interface (``IMutableDependencyResolver``) is not always straightforward to implement with
existing IoC containers. The method ``RxApp.InitializeCustomResolver`` as well as the ``FuncDependencyResolver`` can be used during IoC setup to help you out. If you never used a custom IoC container, then you don't have to do anything here, It Just Works™.

* Validation has been removed, this will be re-added in a future release. If you need this, grab the old version of the class from  https://github.com/reactiveui/ReactiveUI/blob/4.6.4/ReactiveUI/Validation.cs.

*Changes that are pretty easy to deal with*

* ``ReactiveCollection`` is now ``ReactiveList``

* Many things that were in ``ReactiveUI.Xaml`` are now in ``ReactiveUI``

* The `ReactiveUI.Routing` namespace is gone, it has been moved into ``ReactiveUI`` and ``ReactiveUI.{Cocoa/Xaml/Android}`` - in general, the "Platform" DLLs are now much smaller and only contain platform-specific
controls.

* ``ReactiveAsyncCommand`` and ``ReactiveCommand`` are now the same class, and some of the async registration methods have changed.

* ``MakeObjectReactiveHelper`` is removed, because you don't need it anymore, RxUI will Just Work™ without it. Just remove the boilerplate code if you were using it.

* The old syntax for declaring read-write properties is now removed, the *only* correct way to declare properties is now:

```csharp
int foo;
public int Foo {
  get { return foo; }
  set { this.RaiseAndSetIfChanged(ref foo, value); }
}
```
* ToProperty no longer sets the ObservableAsPropertyHelper variable via reflection - instead, it is set via an `out` property.

* ``RxApp.DeferredScheduler`` is now called ``RxApp.MainThreadScheduler``

* Many old "for compatibility only" methods have now been removed - there is no functionality loss.
