# ReactiveCommand Basics

One of the core goals of every MVVM library is to provide an implementation of
`ICommand`. This interface represents one of the two major parts of what
constitutes a ViewModel - Properties and Commands. In keeping with the goal of
MVVM + Rx, ReactiveUI provides its own implementation of `ICommand` called
`ReactiveCommand`, that works a bit differently than most other
implementations.

Commands represent discrete actions that are taken in the UI - "Copy", "Open",
and "Ok" are good examples of Commands. Usually these Commands are bound to a
control that is built to handle Commands, like a Button. Cocoa represents this
concept via the [Target Action
Framework](https://developer.apple.com/library/ios/documentation/general/conceptual/CocoaEncyclopedia/Target-Action/Target-Action.html).

Many Commands are invoked directly by the user, but some operations are also
useful to model via Commands despite being primarily invoked progmatically.
For example, many code paths involving periodically loading or refreshing
resources (i.e. "LoadTweets") can be modeled well using Commands.

### Basics

Since the act of invoking a Command represents an Event, ReactiveCommand
itself is an `IObservable<object>`. The object that is passed along via the
IObservable is the command parameter given to the `Execute` method of
`ICommand`:

```cs
var command = new ReactiveCommand();

command.Subscribe(x => this.Log().Info("The number is {0}", x));

command.Execute(4);
>>> The number is 4
```

While ReactiveCommand supports the Command parameter, it is recommended to not
use it, and simply always pass `null` to the Execute method. Instead of using
the parameter, you should be using properties that are on the ViewModel.

Since ReactiveCommand is an Observable, all of the Rx operators can be used
with it. Here are some practical examples:

```cs

// Note: This is for illustration purposes, see the Asynchronous
// ReactiveCommand chapter for a better way to do this
LoadTweets
    .Where(_ => IsLoggedIn == true)
    .SelectMany(async x => await FetchTweets())
    .ObserveOn(RxApp.MainThreadScheduler)
    .Subscribe(x => LoadedTweets = x);

// Refresh when either the Command is invoked *or* the window is activated
shouldRefreshTweets = Observable.Merge(
    this.Events().ActivatedObs.Select(_ => Unit.Default),
    this.WhenAnyObservable(x => x.ViewModel.Refresh).Select(_ => Unit.Default));

shouldRefreshTweets
    .Where(x => this.ViewModel != null)
    .Subscribe(_ => ViewModel.RefreshData());
```

### CanExecute via Observable

All of the Commands we've created so far, can always be executed - their
`CanExecute` simply returns 'true'. To specify when a Command can be executed,
instead of using a `Func<object, bool>`, we'll use an `IObservable<bool>`.
Because we're describing not only whether a Command can be executed, but when
that value changes, we'll also get the implementation of `CanExecuteChanged`
for free.

Note that the parameter to `CanExecute` is ignored in ReactiveCommand. This is
because it is fundamentally incompatible with the notion of
`CanExecuteChanged` - if `CanExecute(bar)` is `true` and `CanExecute(baz)` is
`false`, when should we fire `CanExecuteChanged`?

The simplest thing we can possibly do to pass along CanExecute information, is
to use a `Subject<bool>`, which is an Observable that you control yourself by
hand. Here's how it works:

```cs
var commandCanExecute = new Subject<bool>();
var command = new ReactiveCommand(commandCanExecute);

commandCanExecute.OnNext(false);
command.CanExecute(null);
>>> false

commandCanExecute.OnNext(true);
command.CanExecute(null);
>>> true
```

### Combining WhenAny and CanExecute

While a Subject might be the easiest thing to understand, it certainly isn't
the most effective. Oftentimes, a far more appropriate `CanExecute` is one
that is based on other properties on the ViewModel. Since we want to be
notified when a Property changes, we use the `WhenAny` method, and we `Select`
it into a boolean value. For example:

```cs
// Whether we can post a Tweet, is based on whether the user has typed any
// text and whether it is short enough.
PostTweet = new ReactiveCommand(
    this.WhenAnyValue(x => x.TweetContents)
        .Select(x => !String.IsNullOrWhitespace(x) && x.Length < 140));

// You can often leave off the extra Select by using the selector built into
// WhenAny
OkButton = new ReactiveCommand(
    this.WhenAny(x => x.Red, x => x.Green, x => x.Blue,
        (r,g,b) => r.Value != null && g.Value != null && b.Value != null));
```

Nearly all of your Commands will use this pattern to define when they can be
executed. Since your Commands will be tied to Properties, many validation-type
tasks can be accomplished in this way.

### Listening to Commands from the View via WhenAnyObservable

Unlike traditional `ICommand` implementations, ReactiveCommands can have as
many people listening to the `Executed` signal as you want. This is **very**
useful for decoupling, as the View can now listen to the ViewModels and
execute View specific code, such as setting control focus or scroll positions.

One may be tempted to simply write 
`ViewModel.SomeCommand.Subscribe(x => ...)`, but this code fails whenever the
ViewModel changes - you will be subscribed to the wrong command and it will
appear to never fire. A method called `WhenAnyObservable` solves this for you:

```cs
// Instead of doing this wrong code:
this.ViewModel.ClearMessageText
    .Subscribe(x => MessageTextBox.GetFocus());

// Do this instead, which will handle null and changing ViewModels
this.WhenAnyObservable(x => x.ViewModel.ClearMessageText)
    .Subscribe(x => MessageTextBox.GetFocus());
```

### Combining Commands Together

One thing that is sometimes useful, is to create a Command which simply
invokes several other commands. ReactiveCommand helps you with this, via the
`CreateCombined` method. The advantage to using this method, is that the
`CanExecute` of the new Command will reflect the `and` of the child commands
(i.e. if any of the child commands can't be invoked, the parent can't be
 either). This is especially useful when one of the commands has an
asynchronous action attached to it.

```cs
RefreshUsers.Subscribe(_ => this.Log().Info("Refreshing Users!"));
RefreshLists.Subscribe(_ => this.Log().Info("Refreshing Lists!"));

RefreshAll = ReactiveCommand.CreateCombined(
    RefreshUsers, RefreshLists);

RefreshAll.Execute(null);

>>> Refreshing Users!
>>> Refreshing Lists!
```

### Invoking and Creating Commands via Observables

There are a few convenience methods built into the framework for invoking
commands. Any Observable can be used as a signal to invoke a command via
`InvokeCommand`:

```cs
// Invoke the Close command whenever the user hits escape. This will
// automatically do the CanExecute check for us before calling Execute.
this.Events().KeyUpObs
    .Where(x => x.EventArgs.Key == Key.Escape)
    .InvokeCommand(this, x => x.ViewModel.Close);
```

Another convenience method called `ToCommand` allows you to create commands
directly from `IObservable<bool>`. The above `CanExecute` examples could be
more tersely written:

```cs
PostTweet = this.WhenAny(x => x.TweetContents, 
        x => !String.IsNullOrWhitespace(x.Value) && x.Value.Length < 140)
    .ToCommand();

OkButton = this.WhenAny(x => x.Red, x => x.Green, x => x.Blue,
        (r,g,b) => r.Value != null && g.Value != null && b.Value != null)
    .ToCommand();
```
