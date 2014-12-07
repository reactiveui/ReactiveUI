Almost always use `this` as the left hand side of a `WhenAny` call.

# Do

```cs
public class MyViewModel
{
    public MyViewModel(IDependency dependency)
    {
        Ensure.ArgumentNotNull(dependency, "dependency");

        this.Dependency = dependency;

        this.stuff = this.WhenAny(x => x.Dependency.Stuff, x => x.Value)
           .ToProperty(this, x => x.Stuff);
    }

    public IDependency Dependency { get; private set; }

    readonly ObservableAsPropertyHelper<IStuff> stuff;
    public IStuff Stuff
    {
        get { return this.stuff.Value; }
    }
}
```

# Don't

```cs
public class MyViewModel(IDependency dependency)
{
    stuff = dependency.WhenAny(x => x.Stuff, x => x.Value)
        .ToProperty(this, x => x.Stuff);
}
```

# Why?

- The lifetime of `dependency` is unknown - if it is a singleton it could introduce memory leaks into your application.