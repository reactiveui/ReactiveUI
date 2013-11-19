# Creating ViewModels with ReactiveObject

Blah blah here's some theory about what a ViewModel is

### Read-Write Properties

Properties that participate in change notification (i.e. that signal when they
are changed), are written in the following way:

```cs
string name;
public string Name {
    get { return name; }
    set { this.RaiseAndSetIfChanged(ref name, value); }
}
```

Note, that unlike in other frameworks, they are **always written this way**,
using the exact same boilerplate code. If you are attempting to put *anything*
in the setter, you are almost certainly Doing It Wrong, and instead should be
using `WhenAny` and `ToProperty` instead.

### Read-Only Properties

Properties that are only initialized in the constructor and don't ever change,
don't need to be written via `RaiseAndSetIfChanged`, they can be declared as
normal properties:

```cs
// Since Commands should almost always be initialized in the constructor and
// never change, they are good candidates for this pattern
public ReactiveCommand PostTweet { get; protected set; }

public PostViewModel()
{
    PostTweet = new ReactiveCommand(/*...*/);
}
```

### Output Properties

### Best Practices
