# Semantics of WhenAny + WhenAnyValue

One of the core features of ReactiveUI is to be able to convert properties to
Observables, via `WhenAny`, and to convert Observables into Properties, via a
method called `ToProperty`.

### What can WhenAny do?

* Watch a single property:

```cs
this.WhenAny(x => x.Foo, x => x.Value)
```

* Watch a number of properties:

```cs
// This signals when any one of these three properties change
this.WhenAny(x => x.Red, x => x.Green, x => x.Blue,
    (r,g,b) => new Color(r.Value, g.Value, b.Value));
```

* Watch a nested property

```cs
this.WhenAny(x => x.Foo.Bar.Baz, x => x.Value);
```

### Different Kinds of WhenAnys

There are several variants of WhenAny that are fairly useful:

* **WhenAny** - You provide a list of properties, and a Selector to determine the original value.

* **WhenAnyValue** - You provide a single property and you get the latest value.

* **WhenAnyObservable** - You provide a list of properties which are Observables, and they are combined via `CombineLatest`. This is particularly useful for subscribing to Commands in a View:

```cs
// Not This - Subscription is now on the *old* object, but we replaced it
// so now it will never recieve items
this.ViewModel.SomeCommand
    .Subscribe(_ => Console.WriteLine("Hello"));
ViewModel = new ToasterViewModel();

// Instead:
this.WhenAnyObservable(x => x.ViewModel.SomeCommand)
    .Subscribe(_ => Console.WriteLine("Hello"));

// Still works!
ViewModel = new ToasterViewModel();
```

WhenAny has a few specific properties that you should know about:

### Expression Evaluation Semantics

Consider the following code:

```cs
this.WhenAny(x => x.Foo.Bar.Baz, _ => "Hello!")
    .Subscribe(x => Console.WriteLine(x));

// Example 1
this.Foo.Bar.Baz = null;
>>> Hello!

// Example 2: Nothing printed!
this.Foo.Bar = null;

// Example 3
this.Foo.Bar = new Bar() { Baz = "Something" };
>>> Hello!
```

`WhenAny` will only send notifications if reading the given Expression would not
throw a Null reference exception. In Example 1, even though Baz is `null`,
because the expression could be evaluated, you get a notification.

In Example 2 however, evaluating `this.Foo.Bar.Baz` wouldn't give you `null`, it
would crash. `WhenAny` therefore suppresses any notifications from being
generated. Setting `Bar` to a new value generates a new notification.

### Distinctness

`WhenAny` only tells you when the *final value* of the expression has
**changed**. This is true even if the resulting change is because of an
intermediate value in the expression chain. Here's an explaining example:

```cs
this.WhenAny(x => x.Foo.Bar.Baz, _ => "Hello!")
    .Subscribe(x => Console.WriteLine(x));

// Example 1
this.Foo.Bar.Baz = "Something";
>>> Hello!

// Example 2: Nothing printed!
this.Foo.Bar.Baz = "Something";

// Example 3: Still nothing
this.Foo.Bar = new Bar() { Baz = "Something" };

// Example 4: The result changes, so we print
this.Foo.Bar = new Bar() { Baz = "Else" };
>>> Hello!
```

### More things

* `WhenAny` always provides you with the current value as soon as you Subscribe
   to it - it is effectively a BehaviorSubject.

* `WhenAny` is a purely cold Observable, which eventually directly connects to
   UI component events. For events such as DependencyProperties, this could
   potentially be a (minor) place to optimize, via `Publish`.
