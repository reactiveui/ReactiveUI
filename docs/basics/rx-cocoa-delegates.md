# Rx Cocoa Delegates

As part of the Events library, ReactiveUI.Events on Cocoa-based platforms
creates a subclass of all Delegate classes in AppKit/UIKit, and creates
Observables for all delegate event methods that don't return a value. These
classes are all under the same namespace as their parent, but are suffixed
with "Rx", and all Observables end with "Obs".

For example:

```cs

var tvd = new UITableViewDelegateRx();
tvd.ScrolledObs
    .Subscribe(_ => Console.WriteLine("Hey we scrolled!"));
tableView.Delegate = tvd;
```

### Caveats

Note that events that return a value can't be turned into Observables - many
of these events begin with "Should". For example:

```cs

var tvd = new UITableViewDelegateRx();

// Compiler error, doesn't exist
tvd.ShouldScrollToTopObs
    .Subscribe(_ => Console.WriteLine("How do we return true?!?"))
```

Since this class is still a normal class, you can subclass it yourself and
implement the ShouldXXXX methods. 
