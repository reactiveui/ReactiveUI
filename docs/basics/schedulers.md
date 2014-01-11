# Default Schedulers 

Scheduling is a core part of writing any app that uses the Reactive
Extensions, as all operations that are deferred (i.e. run on other threads or
on the UI thread).  Schedulers allow apps to control what context code runs
in, and it is important that libraries that run code on other threads are
scheduler-aware.

ReactiveUI provides two app-wide schedulers that should be used in-place of
other schedulers such as the built-in Rx schedulers:

* **RxApp.MainThreadScheduler** - This scheduler executes on the UI thread.
  On XAML-based platforms, this is equivalent to `Dispatcher.BeginInvoke`.

* **RxApp.TaskpoolScheduler** - This scheduler executes code via the TPL
  taskpool. This is equivalent to `Task.Run`.

These global variables are *global* because they can be globally replaced in a
unit test (see the "Testing schedulers" section below). This means, that in a
unit test, you can make *concurrent code* (i.e. code that runs on many
threads simultaneously) run synchronously and have the same result happen
every time.

### Objects that care about scheduling

* **ReactiveCommand** - RegisterAsync will always schedule RegisterAsync on
  RxApp.TaskpoolScheduler and return results on RxApp.MainThreadScheduler.

* **MessageBus** - Message bus can optionally schedule messages to an
  arbitrary scheduler.

* **ObservableAsPropertyHelper / ToProperty** - OAPH automatically ensures
  that notifications are delivered on RxApp.MainThreadScheduler, since the
  result is almost always going to be bound to the UI.

### When should *I* care about scheduling

You should try to attempt to remove all sources of concurrency other than
scheduling via RxApp. This isn't always possible, but threads created via "new
Thread()" or "Task.Run" can't be controlled in a unit test. The most
straightforward way to fix these is by replacing them with "Observable.Start":

Old:

```cs
var result = await Task.Run(() => {
    int number = ThisCalculationTakesALongTime();
    return number;
});

Dispatcher.BeginInvoke(new Action(() => DoAThing()));
```

New:

```cs
var result = await Observable.Start(() => {
    int number = ThisCalculationTakesALongTime();
    return number;
}, RxApp.TaskpoolScheduler);

RxApp.MainThreadScheduler.Schedule(() => DoAThing());
```

If you create a shared component, you should also consider allowing the
scheduler being specified as an optional constructor parameter.

### Testing schedulers

In a unit test runner, by default, the `MainThreadScheduler` runs code
immediately instead of on the (non-existent) UI thread. The
`TaskpoolScheduler` is left unchanged by default.

The best way to run under an alternate scheduler is via the `With` method,
most often used with TestScheduler. This replaces **both schedulers** with the
specified scheduler:

```cs
(new TestScheduler()).With(sched =>{
    // Code run in this block will have both RxApp.MainThreadScheduler
    // and RxApp.TaskpoolScheduler assigned to the new TestScheduler.
});
```
