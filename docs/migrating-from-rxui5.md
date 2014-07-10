## Migrating to ReactiveUI 6.0

### Changes that may be more difficult to deal with

These are the major changes that are likely to affect application developers
in a way that may take more work to resolve.

#### ReactiveCommand is New And Different

* ReactiveCommand is completely rewritten (again). To create ReactiveCommands,
  instead of using `new` you almost always want to use
  the `ReactiveCommand.CreateXYZ` family of methods depending on what kind of
  async method you are using:

Old:

```cs
var someCommand = new ReactiveCommand();

var someAsyncCommand = new ReactiveCommand();
someAsyncCommand.RegisterAsyncTask(someTaskMethod);
```

New:

```cs
var someCommand = ReactiveCommand.Create();

var someAsyncCommand = ReactiveCommand.CreateAsyncTask(someTaskMethod);
```

* ReactiveCommand now provides an `ExecuteAsync` method that returns the
  result of the background task that was invoked. You **must** await or
  Subscribe to it in some way, or it does not execute (i.e. it is a Cold
  Observable).

As with previous versions, the old version of ReactiveCommand is provided in
ReactiveUI.Legacy, to help with migration. 

#### Scheduling Changes

Many operations that were previously automatically scheduled to the UI thread
now do not do so, such as `ToProperty`. While the old behavior made it easier
for developers, it also made it much more difficult to write larger
applications while ensuring responsiveness, as items would be scheduled
multiple times before being displayed.

This means, you probably need to add some `ObserveOn(RxApp.MainThreadScheduler)` 
calls to your application.

#### ToProperty / OAPH changes

* `ObservableAsPropertyHelper` no longer is itself an `IObservable`, use
  `WhenAny` to observe it.

* `ObservableAsPropertyHelper` now lazily Subscribes to the source only when
  the Value is read for the first time. This significantly improves
  performance and memory usage, but at the cost of some "Why doesn't my test
  work??" confusion. If you find that your ToProperty "isn't working", this
  may be why.

#### Suspension / ReactiveUI.Mobile

The philosophy behind ReactiveUI.Mobile has been greatly simplified, and is no
longer tied to routing in any way. The new goal of ReactiveUI.Mobile is, "The
framework will save/load/create a *single object* on your behalf, and persist
that throughout app suspend / resume". 

This object can be of any type you want, as long as the entire object graph
(i.e. this object and its children) can be serialized. This object is created
from scratch when needed, via `RxApp.SuspensionHost.CreateNewAppState` Func
that your app provides. 

#### Other changes

* `ObservableAsyncMRUCache` is removed, you probably want to use Akavache
  instead, or copy-paste the code from an old release if you really want it.

* Several Routing classes that never worked properly have been removed, and
  routing is Generally Discouraged from being used in anything other than a
  WPF app.

### Easy to handle changes

* Namespaces are much simpler now - everything is in `ReactiveUI` except for
  platform-specific functionality.

* ReactiveUI-Platforms is deprecated, as well as all of the platform-specific
  libraries - they are now all integrated into a single "ReactiveUI.dll". This
  means that if you are manually referencing ReactiveUI.dll, you should
  *always* pick the most specific version for your platform. If your **App**
  references the PLib version of ReactiveUI.dll, you'll have a Bad Time. If
  you use NuGet, everything will be handled automatically.

* ReactiveUI.Mobile has moved into ReactiveUI-Core, and the namespace no
  longer exists.

* ReactiveUI is now updated to depend on Rx 2.2.4. This shouldn't affect
  existing code.

* Service Location is now moved to Splat, you may have to add a `using Splat;`
  to several files in your solution.

* XXX: `InitializeResolver` is now a combination of `InitializeSplat`

* ReactiveUI on iOS now **requires** Xamarin.iOS 7.2.1 or higher. You probably
  already have this by now.

* MemoizingMRUCache is now in Splat

### Find-and-replace changes

* RxApp.DependencyResolver => Locator.Current
* RxApp.MutableResolver => Locator.CurrentMutable
* RxApp.InUnitTestRunner => ModeDetector.InUnitTestRunner
