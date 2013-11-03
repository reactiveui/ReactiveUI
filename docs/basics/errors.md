# Reporting errors to the user with UserError

Handling errors and displaying them to the user in a friendly way is a core
job of every good desktop / mobile application. Without great error
presentation, users can't resolve problems and get their work done, which
leads to bad experiences. 

There are several design documents online that describe what makes a good
error experience, such as the [Apple
HIG](https://developer.apple.com/library/mac/documentation/userexperience/conceptual/applehiguidelines/Windows/Windows.html#//apple_ref/doc/uid/20000961-TP10) and the [GNOME
HIG](https://developer.gnome.org/hig-book/3.0/windows-alert.html.en#alert-text).
However, writing the code to back these error workflows in the naive way often
results in error-prone, spaghetti code, or more often, the developer deems it
"Too Hard To Do Right", and creates a [substandard
experience](http://cl.ly/image/100X3E2C2o3M).

ReactiveUI's Error framework makes it easy to separate the presentation of
an error from its source, to present errors in an MVVM-friendly, testable way,
and to allow errors to be handled at the point they occur, which often results
in much cleaner error-handling code, that doesn't end up crossing module
boundaries.

### UserError class

The core class is the [`UserError`
class](https://github.com/reactiveui/ReactiveUI/blob/master/ReactiveUI/Errors.cs).
UserErrors are conceptually similar to Exceptions, in that they are created at
the time an error occurs and are "Thrown" to a handler. However, unlike
Exceptions, UserErrors represent errors that can be resolved by the **user**,
not programming errors (i.e. UserErrors do not *replace* Exceptions).

UserErrors consist of several important pieces:

* The `ErrorMessage`, which is the primary text displayed to the user
* The `ErrorCauseOrResolution`, which is secondary information detailing
  either the root cause of the error, or suggestions on how to resolve it.
* An `InnerException`, which optionally gives the exception that caused this
  error to be displayed.
* `RecoveryOptions`, which are a list of Commands that can resolve this error.
  More on this later.

Once a UserError has been created, it can be thrown via `UserError.Throw`.
Handlers will be invoked in reverse order (similar to how an Exception travels
up through the stack), until the UserError is handled. If a UserError isn't
handled by any registered handler, an Exception is thrown.

`UserError.Throw` will return what the user decided to do, as one of three
options: either `CancelOperation`, to indicate the caller should simply give
up, `RetryOperation`, to indicate the error condition has been resolved and
the caller should retry, or `FailOperation`, to indicate that the error cannot
be resolved and the caller should throw an Exception.

```cs
var exception = default(Exception);
try {
    TheTweets = await LoadTweets();
} catch (Exception ex) {
    exception = ex;
}

if (exception != null) {
    // Note: This isn't a very good error message
    var errorMessage = "The Tweets could not be loaded";
    var errorResolution = "Check your Internet connection";
    var userError = new UserError(errorMessage, errorResolution);

    switch (await UserError.Throw(userError)) {
    case RecoveryOptionResult.RetryOperation:
        LoadTweets.Execute();
        break;
    case RecoveryOptionResult.FailOperation:
        throw exception;
    }
}
```

Combining this with ReactiveCommand's `ThrownExceptions` often results in very
clean code:

```cs
//
// Note: We are in a ViewModel here
//

LoadTweetsCommand = new ReactiveCommand();

LoadTweetsCommand.RegisterAsyncTask(() => LoadTweets())
    .Subscribe(x => TheTweets = x);

var errorMessage = "The Tweets could not be loaded";
var errorResolution = "Check your Internet connection";

// Any exceptions thrown by LoadTweets will end up being
// sent through ThrownExceptions
LoadTweetsCommand.ThrownExceptions
    .Select(ex => 
        new UserError(errorMessage, errorResolution))
    .SelectMany(UserError.Throw);
    .Where(x => x == RecoveryOptionResult.RetryOperation)
    .InvokeCommand(LoadTweetsCommand);
```

### The Handler Chain

Using `Throw` is the first half of using the Errors framework. However, you
must also write code to actually present the error to the user. This code is
commonly written in the View layer, since it often involves opening a dialog
box or presenting other UI. To do this, we can use
`UserError.RegisterHandler`:

```cs
var disconnectHandler = UserError.RegisterHandler(async error => {
    // We don't know what thread a UserError can be thrown from, we usually 
    // need to move things to the Main thread.
    await RxApp.MainThreadScheduler.ScheduleAsync(() => {
        // NOTE: This code is Incorrect, as it throws away 
        // Recovery Options and just returns Cancel. This is Bad™.
        return MesssageBox.Show(error.ErrorMessage);
    });

    return RecoveryOptionResult.CancelOperation;
});
```

Handlers are invoked in reverse order of their registration, which effectively
means that Views that have been shown the most recently, have first chance to
handle UserErrors.

**Important Note:** if you register a UserError handler in a View, you need to
register it only when the View is presented to the user, and deregister it
when the View is deactivated. Failing to do this usually results in multiple
dialogs popping up and Developer Confusion™.

### Recovery Options

Recovery Options are options that are presented to the user in order to
resolve the issue - they usually manifest themselves as buttons on a dialog.
Great recovery options are *descriptive actions* that solve the user's problem
in a certain way. For example, an "out of disk space" UserError might present
a "Open Finder" recovery option so that the user can find files to delete.
Poorly thought-out recovery options often have the titles "Ok" or "Cancel",
and don't help the user to resolve the underlying issue.

Recovery options are registered when the `UserError` is created - they are
subclass of `ICommand`, but usually the default `RecoveryCommand` class is
sufficient. Should you decide to be particularly lazy, default recovery
commands are provided as well (`RecoveryCommand.Yes/No`, and
`RecoveryCommand.Ok/Cancel`)

Handling the Recovery Options is usually done via this not particularly
obvious snippet of code:

```cs
var disconnectHandler = UserError.RegisterHandler(error => {
    // TODO: Display the dialog and wire up RecoveryCommands to Buttons
    ShowTheErrorDialog(error);

    // Return the RecoveryOptionResult of the button that was
    // clicked.
    return error.RecoveryCommands
        .Select(x => x.Select(_ => x.RecoveryOptionResult))
        .Merge()
        .ObserveOn(RxApp.MainThreadScheduler);
});
```

### Less obvious uses of the Handler Chain

The most straightforward way to present errors is via an alert dialog, but
that is often not the best way - UserError can represent many different kinds
of error UXs. For example, the Undo button in GitHub for Windows is
represented by a UserError, which implicitly hits the "Cancel" Recovery Option
after a certain amount of time has elapsed:

![](http://cl.ly/image/3s3W3Y0r1S2P/content#png)

Handlers can also decline to actually resolve the error, but instead add
Recovery Commands to a UserError as it winds up the stack. This is useful for
complex code, where it would be more convenient to resolve an error higher in
the ViewModel code. This pattern is encapsulated via the
`UserError.AddRecoveryOption` convenience method.

### Testing

Testing UserError can be accomplished in a manner similar to other globals in
the framework, via calling the `UserError.OverrideHandlersForTesting` method.
This allows you to test the user choosing different recovery options or
responding in different ways, without actually having a View.

### Conclusion

UserErrors encapsulate the aspects of a great error dialog - it gives
information on what is wrong, how to resolve it, and actions the user can take
to solve the problem, but allowing the developer to solve the problem at the
level of code where it's easiest in the ViewModel, without calling into the
View or moving error handling code to the platform-specific View code.
