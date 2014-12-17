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
destination is invisible. It also encourages bad design as many people will
directly proxy View events to the ViewModel layer, which makes them not
particularly ViewModelly.

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

Unlike other MVVM frameworks, there are often more correct ways to solve
problems, given a bit of ingenuity. `WhenAny` and `WhenAnyObservable` can
often be used to describe how to reach into objects, even if these objects are
changing over time. This is often most useful in the View:

```cs
public LoginView()
{
    // As soon as the CredentialsAreValid turns to 'true', set the focus
    // to the Ok button.
    this.WhenAny(x => x.ViewModel.CredentialsAreValid, x => x.Value)
        .Where(x => x != false)
        .Subscribe(_ => OkButton.SetFocus());
}
```

Consider another scenario, a ViewModel of open documents containing a list of
Document ViewModels - each Document containing a `Close` command. Many
traditional implementations of MVVM would struggle with implementing this
command, either keeping a reference to the list, or via the MessageBus.

However, instead of doing this, we can use Rx's operators to solve this in a
more elegant way.

```cs
public class DocumentViewModel : ReactiveObject
{
    public ReactiveCommand<Object> Close { get; set; }

    public DocumentViewModel() 
    {
        // Note that we don't actually *subscribe* to Close here or implement
        // anything in DocumentViewModel, because Closing is a responsibility
        // of the document list.
        Close = ReactiveCommand.Create();
    }
}

public class MainViewModel : ReactiveObject
{
    public ReactiveList<DocumentViewModel> OpenDocuments { get; protected set; }

    public MainViewModel()
    {
        OpenDocuments = new ReactiveList<DocumentViewModel>();

        // Whenever the list of documents change, calculate a new Observable
        // to represent whenever any of the *current* documents have been
        // requested to close, then Switch to that. When we get something
        // to close, remove it from the list.
        OpenDocuments.Changed
            .Select(_ => WhenAnyDocumentClosed())
            .Switch()
            .Subscribe(x => OpenDocuments.Remove(x));
    }

    IObservable<DocumentViewModel> WhenAnyDocumentClosed()
    {
        // Select the documents into a list of Observables
        // who return the Document to close when signaled,
        // then flatten them all together.
        return OpenDocuments
            .Select(x => x.Close.Select(_ => x))
            .Merge();
    }
}
```
