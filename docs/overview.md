# Overview

ReactiveUI is a compelling combination of MVVM and Reactive Extensions (Rx).
Combining these two make managing concurrency as well as expressing complicated
interactions between objects possible in a declarative, functional way. Put
simply, if you’ve ever had to chain events / callbacks together and declare
state ints/booleans to keep track of what’s going on, Reactive Extensions
provides a sane alternative. 

## What’s in this library

- **ReactiveObject** - a ViewModel object based on Josh Smith’s implementation,
  that also implements IObservable as a way to notify property changes. It also
  allows a straightforward way to observe the changes of a single property.
- **ReactiveCommand** - an implementation of ICommand that is also a Subject
  whose OnNext is raised when Execute is executed. Its CanExecute can also be
  deﬁned by an IObservable which means the UI will instantly update instead of
  implementations which rely on RequerySuggested in WPF. ReactiveCommand
  encapsulates the common pattern of “Fire asynchronous command, then marshal
  result back onto dispatcher thread”. It also allows you to control if
  concurrency is allowed. 
- **ObservableAsPropertyHelper<T>** - a class that easily lets you convert an
  IObservable into a property that stores its latest value, as well as ﬁres
  NotifyPropertyChanged when the property changes. This is really useful for
  combining existing properties together and replacing IValueConverters, since
  your ViewModels will also be IObservables.
- **ReactiveList<T>** - a custom implementation of an ObservableCollection which
  allows you to see changes in the collection as observables.
- **MessageBus** - a reactive implementation of the Publish/Subscribe pattern,
  usefull to decouple your objects, while still being able to communicate.
- **MemoizingMRUCache** - a cache that only remembers a specified number of
  recently used items.
- **ObservableAsyncMRUCache** - a thread-safe, asynchronous MemoizingMRUCache.
- **ReactiveBinding** - a powerful and flexible cross-platform binding framework
  as an alternative for Xaml bindings.

## Organization

This library is organized into several high-level assemblies:

- **ReactiveUI** - Core library that doesn't rely on any particular UI
  framework. `ReactiveObject`, the base ViewModel object, as well as
  `ReactiveList<T>`, a more awesome ObservableCollection, `ReactiveCommand`, an
  implementation of ICommand, and the Binding framework are in here.

- **ReactiveUI.Platforms** - Classes that require references to a Xaml'ly
  framework, like WPF or WinRT. This assembly also contains the Xaml part of the
  Binding framework and a screens and navigation framework usefull for
  navigating back and forward between views based on ViewModels.

- **ReactiveUI.Blend** - This class has several Blend Behaviors and Triggers
  that make attaching ViewModel changes to Visual State Manager states.

- **ReactiveUI.Mobile** - Useful classes when developing for a mobile platforms
  such as Windows Phone or the Windows Runtime. These classes handle things
  like persisting state and reacting to application lifetime events.

## ReactiveObject 

Like any other MVVM framework, ReactiveUI has an object designed as a ViewModel
class. This object is based on Josh Smith’s ObservableObject implementation in
MVVM Foundation (actually, many of the classes’ inspiration come from MVVM
Foundation, Josh does awesome work!). The Reactive version as you can imagine,
implements INotifyPropertyChanged as well as IObservable so that you can
subscribe to object property changes.

ReactiveObject also does a few nice things for you: ﬁrst, when you compile
ReactiveUI in Debug mode, it will print debug messages using its logging
framework whenever a property changes. Another example is, implementing the
standard pattern of a property that raises the changed event is a few lines
shorter and makes effective use of the new CallerMemberName attribute:

```cs
int _someProp; 
public int SomeProp { 
  get { return _someProp; } 
  set { this.RaiseAndSetIfChanged(ref _someProp, value); } 
}
```

## ReactiveCommand

ReactiveCommand is an ICommand implementation that is simultaneously a
RelayCommand implementation, as well as some extra bits that are pretty
motivating. We can provide an IObservable as our CanExecute. For example, here’s
a command that can only run when the mouse is up:

```cs
var mouseIsUp = Observable.Merge(
   Observable.FromEvent<MouseButtonEventArgs>(window, ”MouseDown”).Select(_ => false), 
   Observable.FromEvent<MouseButtonEventArgs>(window, ”MouseUp”).Select(_ => true),
).StartWith(true);

var cmd = new ReactiveCommand(mouseIsUp); 
cmd.Subscribe(x => Console.WriteLine(x));
```

Or, how about a command that can only run if two other commands are disabled:

```cs
// Pretend these were already initialized to something more interesting 
var cmd1 = new ReactiveCommand(); 
var cmd2 = new ReactiveCommand();

var can_exec = cmd1.CanExecuteObservable.CombineLatest(cmd2.CanExecuteObservable, (lhs, rhs) => !(lhs && rhs));
var new_cmd = new ReactiveCommand(can_exec);
new_cmd.Subscribe(Console.WriteLine);
```

One thing that’s important to notice here, is that the command’s CanExecute
updates immediately, instead of relying on CommandManager.RequerySuggested. If
you’ve ever had the problem in WPF or Silverlight where your buttons don’t
reenable themselves until you switch focus or click them, you’ve seen this bug.
Using an IObservable means that the Commanding framework knows exactly when the
state changes, and doesn’t need to requery every command object on the page.

### What about Execute?

This is where ReactiveCommand’s IObservable implementation comes in.
ReactiveCommand itself can be observed, and it provides new items whenever
Execute is called (the items being the parameter passed into the Execute call).
This means, that Subscribe can act the same as the Execute Action, or we can
actually get a fair bit more clever. For example:

```cs
var cmd = new ReactiveCommand();
cmd.Where(x => ((int)x) % 2 == 0).Subscribe(x => Console.WriteLine(”Even numbers like {0} are cool!”, x));
cmd.Where(x => ((int)x) % 2 != 0).Timestamps().Subscribe(x => Console.WriteLine(”Odd numbers like {0} are even cooler, especially at {1}!”, x.Value, x.Timestamp));

cmd.Execute(2); 
>>> ”Even numbers like 2 are cool!”

cmd.Execute(5); 
>>> ”Odd numbers like 5 are even cooler, especially at (the current time)!”
```

### Running commands async.
If you’ve done any C#/Xaml programming that does any sort of interesting work,
you know that one of the difﬁcult things is that if you do things in an event
handler that take a lot of time, like reading a large ﬁle or downloading
something over a network, you will quickly ﬁnd that you have a problem: you
either block the UI, or when you can’t even do blocking operations at all,
you’ll just run it on another thread. Then, you ﬁnd the 2nd tricky part that WPF
and Silverlight objects have thread afﬁnity. Meaning, that you can only access
objects from the thread that created them. So, at the end of the computation
when you go to runtextBox.Text = results;, you suddenly get an Exception.
Dispatcher.BeginInvoke solves this So, once you dig around on the Internet a
bit, you ﬁnd out the pattern to solve this problem involves the Dispatcher:

```cs
void SomeUIEvent(object o, EventArgs e) { 
  var some_data = this.SomePropertyICanOnlyGetOnTheUIThread;
  var t = new Task(() => { 
    var result = doSomethingInTheBackground(some_data);
    Dispatcher.BeginInvoke(new Action(() => { this.UIPropertyThatWantsTheCalculation = result; }));
  }

  t.Start();
}
```

We use this pattern a lot, so when we run a command, we often are just:
1. The command executes, we kick off a thread
2. We calculate something that takes a long time
3. We take the result, and set a property on the UI thread, using Dispatcher

ReactiveCommand encapsulates this pattern by allowing you to register a Task or
IObservable to execute. It also gives you other thing for free. For example, you
often only want one async instance running, and the Command should be disabled
while we are still processing. Another common thing you would want to do is,
display some sort of UI while an async action is running - something like a
spinner control or a progress bar being displayed.

Here’s a simple use of a Command, who will run a task in the background, and
only allow one at a time (i.e. its CanExecute will return false until the action
completes)

```cs
var cmd = new ReactiveCommand(null, false, /* do not allow concurrent requests */ null);
cmd.RegisterAsyncAction(i => {
    Thread.Sleep((int)i * 1000); // Pretend to do work
};

cmd.Execute(5 /*seconds*/); 
cmd.CanExecute(5); // False! We’re still chewing on the first one.
```

## Learn more

For more information on how to use ReactiveUI, check out
[ReactiveUI](http://www.reactiveui.net).
