# Creating ViewModels with ReactiveObject

At the core of every MVVM framework is the *ViewModel* - while this class is
the most interesting aspect of the MVVM pattern, it is also the most
misunderstood. Properly reasoning about what a ViewModel is and is not, is
crucial to correctly applying the MVVM pattern.

### The Zen of The ViewModel

Most UI frameworks have applied almost zero thought to unit testing when their
framework was designed, or those concerns were deemed as out-of-scope. As a
result, UI objects are often very difficult to test in unit test runners, as
they are not just *plain objects*. They may have dependencies on a runloop
existing, or often expect static classes / globals to be initialized in a
certain way.

So, since UI classes are untestable, our new goal is to put as much of our
*interesting code*, into a class that *represents* the View, but is just a
regular class we can create. Then, we want the actual code in the View to be
as boring, mechanical, and as short as possible, because it is inherently
untestable.

This class is called the **ViewModel**, and it is the *Model* of a *View*.
This means, that you usually have one ViewModel per View. This doesn't have to
be strictly true, but it is generally the case. 

Another important aspect of understanding ViewModels is that they are an
abstraction to separate *policy* from *mechanism*. ViewModels do not deal in
the specifics of Buttons and Menus and TextBoxes, they only describe how the
data in these elements are related. For example, the "Copy" `ICommand` has no
direct knowledge of the MenuItem or the Button that it is connected to, it
only models the *Action* of Copying. The View has the responsibility of
mapping the Copy command to the controls that invoke it.

### ViewModels are Reusable

Because ViewModels do not explicitly reference UI frameworks or controls, this
means that ViewModels can often be *reused across platforms*. This is a very
powerful pattern that can drastically reduce the time required to port to a
new platform, especially in conjunction with portable libraries designed to
help in this task, such as [Splat](https://github.com/paulcbetts/splat) and
[Akavache](https://github.com/akavache/Akavache). The majority of your
application's interesting code (models / network handling / caching / image
loading / viewmodels) can be used on all platforms, and only the View-related
classes need to be rewritten.

### Common Mistakes and Misconceptions

Many people believe that the MVVM pattern means that there should be zero code
in the View code-behind, or that everything should be in XAML. While certain
patterns such as Blend Triggers generally promote code reuse, this is
generally an Antipattern. C# is a much more expressive, more concise language
than XAML, and while it may be *possible* for you to create an entire complex
View without writing any code, the result will be an unmaintainable, difficult
to read mess.

So then, how do I decide what to put in the View? Concepts such as scroll
position and control focus are great examples of code that is View-specific.
Handling animation and Window position / minimize are also great examples of
code that often should be in the View.

Another common misconception is that of separation - while it is very
important that the ViewModel have no reference to the View or any of the
controls that the View creates, **the reverse is not true**. The View is free
to be very tightly bound to the ViewModel, and in fact, it is often useful for
the View to "reach into" the ViewModel via `WhenAny` and `WhenAnyObservable`.

With the theory out of the way, let's see how to create ViewModels in
ReactiveUI.

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
public ReactiveCommand<Object> PostTweet { get; protected set; }

public PostViewModel()
{
    PostTweet = ReactiveCommand.Create(/*...*/);
}
```

### Output Properties

So far, nothing here has been particularly surprising, just boilerplate
MVVM features. However, there is another type of Property in ReactiveUI that
doesn't exist in other frameworks that is **very important** to effectively
use ReactiveUI, the "Output Property".

Output properties are a way to take *Observables* and convert them into
*ViewModel Properties*. We'll often use them with the opposite method, which
turns ViewModel Properties into Observables, `WhenAny`. As the name implies,
Output Properties are usually read-only (i.e. the source Observable dictates
when the property changes).

First, we need to be able to declare an Output Property, using a class called
`ObservableAsPropertyHelper<T>`:

```cs
readonly ObservableAsPropertyHelper<string> firstName;
public string FirstName {
    get { return firstName.Value; }
}
```

Similar to read-write properties, this code should always be 100% boilerplate.
Next, we'll use a helper method `ToProperty` to initialize `firstName` in the
constructor:

```cs
this.WhenAnyValue(x => x.Name)
    .Select(x => x.Split(' ')[0])
    .ToProperty(this, x => x.FirstName, out firstName);
```

Here, `ToProperty` creates an `ObservableAsPropertyHelper` instance which will
signal that the "FirstName" property has changed. `ToProperty` is an extension
method on `IObservable<T>` and semantically acts like a "Subscribe".

### Best Practices

One of the core concepts of Functional Reactive Programming, is that instead
of writing *imperative code* (i.e. "Do A right now, then do B, then do C"), we
want to write *Functional, Declarative code* - instead of writing event
handlers and methods to change properties, we want **to Describe how properties
are related to each other**, using `WhenAny` and `ToProperty`.

As a result, almost all of the interesting code in a well-written ReactiveUI
ViewModel will be in the *constructor*; this code will describe how the
properties in the ViewModel are related to each other. Your goal when writing
the code for a ViewModel, is to take statements that describe how the view
*should* work in terms of its commands and properties, and to translate them
into things to put into the constructor. For example:

* "The Login button can be pressed when the username and password aren't blank"
* "The error message should be cleared 10 seconds after it is displayed"
* "The DirectMessageToSend object consists of the target user and the message
   to send""

All of these statements are concise descriptions of parts of how your UI
should work, and these statements can all be directly translated into Rx
expressions in your ViewModel constructor.
