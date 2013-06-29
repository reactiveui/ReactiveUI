## How to migrate from ReactiveUI 4.x

Moving to ReactiveUI 5.0 is usually straightforward, but there are a few things
to know.

### Changes that may be more difficult to deal with

* ReactiveUI 5.0 is .NET 4.5 only - this means that Silverlight 5, .NET 4.0, and
  WP7.x are all unsupported. If you want to use ReactiveUI with these platforms,
  you have to stay on the 4.x series. You can do this by changing the lines in
  your "packages.config" to always use the latest version from the 4.x series:

  ```xml
  <package id="reactiveui-core" version="(4.0.0, 5.0.0)" />
  ```

* ReactiveCommand now does not have an imperative constructor (i.e.
  `ReactiveCommand.Create`). This constructor is something that
  you probably shouldn't be using anyways, but if you really need it, you can
  find the original ReactiveCommand in the `ReactiveUI.Legacy` namespace.

* ReactiveUI now uses a much more simplified Service Location (i.e. IoC without
  injection) model than RxUI 4.x. However, this new interface
  (`IMutableDependencyResolver`) is not always straightforward to implement with
  existing IoC containers. The method `RxApp.InitializeCustomResolver` as well
  as the `FuncDependencyResolver` can be used during IoC setup to help you out.

### Changes that are pretty easy to deal with

* `ReactiveCollection` is now `ReactiveList`

* Many things that were in `ReactiveUI.Xaml` are now in `ReactiveUI`

* The `ReactiveUI.Routing` namespace is gone, it has been moved into
  `ReactiveUI` and `ReactiveUI.{Cocoa/Xaml/Android}` - in general, the
  "Platform" DLLs are now much smaller and only contain platform-specific
  controls.

* ReactiveAsyncCommand and ReactiveCommand are now the same class, and some of
  the async registration methods have changed.

* The old syntax for declaring read-write properties is now removed, the *only*
  correct way to declare properties is now:

```cs
int foo;
public int Foo {
    get { return foo; }
    set { this.RaiseAndSetIfChanged(ref foo, value); }
}
```

* ToProperty no longer sets the ObservableAsPropertyHelper variable via
  reflection - instead, it is set via an `out` property.

* `RxApp.DeferredScheduler` is now called `RxApp.MainThreadScheduler`
