## Migrating to ReactiveUI 7.0

### Changes that may be more difficult to deal with

These are the major changes that are likely to affect application developers in a way that may take more work to resolve.

#### ReactiveCommand is Different

`ReactiveCommand` is completely rewritten again (sorry).

* interfaces are gone. Any use of `IReactiveCommand` should be replaced with `ReactiveCommand`, possibly with type information (see below).
* static creation methods have changed:
    * execution information is now _always_ required when calling `CreateXxx` methods, including with "synchronous" commands (i.e. those created with `Create`). So rather than calling `Create` and then subscribing, you call `Create` and pass in your execution logic right then and there.
    * for consistency, the execution behavior is always provided as the first parameter. Other parameters (`canExecute`, `scheduler`) are optional
* parameter types are formalized by `TParam` in `ReactiveCommand<TParam, TResult>`
    * if your command takes a parameter, you no longer take an `object` and cast it. Instead, you explicitly specify the parameter type when creating the command (of course, you can still choose `object` if that makes sense, or as an intermediary migration step)
* `ICommand` is now implemented explicitly. As a result:
    * any calls to `Execute` should be replaced with a call to `ExecuteAsync`
    * `CanExecuteObservable` is now simply called `CanExecute`
* execution of a command occurs when you invoke `ExecuteAsync`. You no longer have to subscribe to the returned observable for the execution logic to occur. Late subscribers will still receive the result of the execution.
* observables such as `CanExecute` and `IsExecuting` are now behavioral. That is, they will always provide the last known value to subscribers.
* `RoutingState` has been updated to use the new implementation. Consequently, any use of its commands will be affected per the above.
* the `ToCommand` extension method has been removed. This was a simple convenience to take an `IObservable<bool>` and use it as the `canExecute` pipeline for a new command. If you're using `ToCommand`, you can just replace it with a call to one of the creation methods on `ReactiveCommand`.

Old:

```cs
var canExecute = ...;
var someCommand = ReactiveCommand.Create(canExecute);
someCommand.Subscribe(x => /* execution logic */);

var someAsyncCommand1 = ReactiveCommand.CreateAsyncObservable(canExecute, someObservableMethod);
var someAsyncCommand2 = ReactiveCommand.CreateAsyncTask(canExecute, someTaskMethod);
```

New:

```cs
var canExecute = ...;
var someCommand = ReactiveCommand.Create(() => /* execution logic */);

var someAsyncCommand1 = ReactiveCommand.CreateAsyncObservable(someObservableMethod, canExecute);
var someAsyncCommand2 = ReactiveCommand.CreateAsyncTask(someTaskMethod, canExecute);
```

For reference, here is a more detailed look at the ways in which you can create `ReactiveCommand` instances:

```cs
// take no parameter, and return nothing of interest
// the type of all these commands is ReactiveCommand<Unit, Unit>
ReactiveCommand.Create(() => Console.WriteLine("hello")));
ReactiveCommand.CreateAsyncObservable(() => Observable.Return(Unit.Default));
ReactiveCommand.CreateAsyncTask(async () => await Task.Delay(TimeSpan.FromSeconds(1)));

// take an int parameter, but return nothing of interest
// the type of all these commands is ReactiveCommand<int, Unit>
ReactiveCommand.Create<int>(param => Console.WriteLine(param)));
ReactiveCommand.CreateAsyncObservable<int, Unit>(param => Observable.Return(Unit.Default));
ReactiveCommand.CreateAsyncTask<int, Unit>(async param => await Task.Delay(TimeSpan.FromSeconds(param));

// take no parameter, and return an int
// the type of all these commands is ReactiveCommand<Unit, int>
ReactiveCommand.Create(() => 5);
ReactiveCommand.CreateAsyncObservable(() => Observable.Return(42));
ReactiveCommand.CreateAsyncTask(() => Task.FromResult(42));

// take an int parameter, and return a string
// the type of all these commands is ReactiveCommand<int, string>
ReactiveCommand.Create<int, string>(param => param.ToString());
ReactiveCommand.CreateAsyncObservable<int, string>(param => Observable.Return(param.ToString()));
ReactiveCommand.CreateAsyncTask<int, string>(param => Task.FromResult(param.ToString()));

// in all cases, you can also pass in canExecute and scheduler
var canExecute = ...;
var scheduler = ...;
ReactiveCommand.Create(() => {}, canExecute, scheduler);
```

To enable you to ease into the migration, all previous types are available under the `ReactiveUI.Legacy` namespace. Note, however, that there is no legacy version of `RoutingState`, so any code you have that interacts with its command may require minor updates.