# MessageBus

Like many other MVVM frameworks, ReactiveUI includes an implementation of the
message bus pattern. This allows you to send and recieve messages between
different parts of the code without them directly accessing each other.

One unique property of the default MessageBus (`MessageBus.Current`) in
ReactiveUI is that it schedules messages via the UI thread. This means that
messages sent from background threads will automatically arrive on the main
thread. The MessageBus is also useful for marshaling messages between
different layers of the code (usually sending messages from View to ViewModel)

While this class is provided because it is sometimes necessary, the MessageBus
should be used only as **a last resort**. The MessageBus is effectively a
*global variable*, which means it is subject to memory and event leaks, and
furthermore, the detached nature of MessageBus means that it's a `goto` whose
destination is invisible.

### The Basics

MessageBus is quite straightforward. First, set up a listener:

```cs
// Listen for anyone sending instances of the KeyUpEventArgs class. Since
// MessageBus simply returns an IObservable, it can be combined or used in
// many different ways
MessageBus.Current.Listen<KeyUpEventArgs>()
    .Where(e => e.KeyCode == KeyCode.Up)
    .Subscribe(x => Console.WriteLine("Up Pressed!"));
```

Now, connect an IObservable to the bus via `RegisterMessageSource`:

```cs
MessageBus.Current.RegisterMessageSource(RootVisual.Events().KeyUpObs);
```

Or, if you're feeling very imperative and not very Functional:

```cs
MessageBus.Current.SendMessage(new KeyUpEventArgs());
```

### Ways to avoid using MessageBus

