# ToProperty and Output Properties

One of the core features of ReactiveUI is to be able to convert properties to
Observables, via `WhenAny`, and to convert Observables into Properties, via a
method called `ToProperty`. These properties are called *Output Properties* in
ReactiveUI, and they are a huge part of using the framework effectively.

Consider a color picker dialog - this dialog might have four properties:

* Red
* Green
* Blue
* FinalColor

The 4th property, `FinalColor`, isn't like the others however. It isn't a
read-write property, its value is determined by the sum of the other three
properties. In other frameworks, this would also be a read-only property, which
can now be set by multiple parts of the code. This is the beginning of UI
spaghetti code ugliness.

ReactiveUI allows you to explicitly describe the dependencies between
properties, in a way that makes it difficult to write incorrect, difficult to
debug spaghetti code.

### Basic Usage

First, we need to create an Output Property, using a class called
`ObservableAsPropertyHelper<T>`. All Output Properties are always written the
same way, it is 100% boilerplate code.

```cs
ObservableAsPropertyHelper<Color> finalColor;
public Color FinalColor {
    get { return finalColor.Value; }
}
```

Note that there is no setter for `FinalColor`. We'll describe how the color
changes using Observables instead. In the Constructor, we'll set it up:

```cs
var colorValues = this.WhenAnyValue(x => x.Red, x => x.Green, x => x.Blue,
        (r,g,b) => new {r,g,b})
    .Select(x => new Color(x.r, x.g, x.b));

colorValues.ToProperty(this, x => x.FinalColor, out finalColor);
```

### Lazy Observation

It's important to know the semantics of `ToProperty` - you should conceptualize
this method as similar to `Subscribe`: it causes the Observable to be evaluated,
just like `foreach` causes Enumerables to be evaluated.

However, beginning with ReactiveUI 6.0, `ToProperty` is **lazy** - it doesn't
Subscribe until someone (usually a View Binding) requests the property for the
first time (via `Value`). `ToProperty` is morally similar to the following code:

```cs
// This is similar
theObservable.ToProperty(this, x => x.Foo, out foo);

//
// ...to this
//

var backingFoo = theObservable
    .Do(x => Foo = x)
    .Publish()
    .RefCount();

public string Foo {
    get {
        backingDisposable = backingDisposable ?? backingFoo.Subscribe();
        return foo;
    }
}
```

Should you have problems with ToProperty "missing" events, the easiest way to
resolve it is via Concat'ting a canned value. For example:

```cs
// Has problems, because CanExecuteObservable is Hot
someCommand.CanExecuteObservable
    .ToProperty(this, x => x.CanExecute, out canExecute);

// Hack around via making the Observable cold
Observable.Defer(() => Observable.Return(someCommand.CanExecute(null)))
    .Concat(someCommand.CanExecuteObservable)
    .ToProperty(this, x => x.CanExecute, out canExecute);
```
