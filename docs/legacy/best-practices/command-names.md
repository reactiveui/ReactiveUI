# Don't
Don't suffix `ReactiveCommand` properties' names with `Command`; instead, name the property using a verb that describes the command's action. For example:

```cs

public ReactiveCommand Synchronize { get; private set; }

// and then in the ctor:

Synchronize = ReactiveCommand.CreateAsyncObservable(
_ => SynchronizeImpl(mergeInsteadOfRebase: !IsAhead));
```

When a `ReactiveCommand`'s implementation is too large or too complex for an anonymous delegate, name the implementation's method the same name as the command, but with `Impl` suffixed (for example, `SychronizeImpl` above).
