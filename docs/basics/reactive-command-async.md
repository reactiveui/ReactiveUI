# Asynchronous operations with ReactiveCommand

One of the most important features of ReactiveCommand is its built-in
facilities for orchestrating asynchronous operations. In previous versions of
ReactiveUI, this was in a separate command class, but starting with ReactiveUI
5.0, this is built-in.

### Commands and CreateAsyncXYZ

To use ReactiveCommand with async operations, create your Command via the
`CreateAsync` family of methods

* **Create** - Create a standard ReactiveCommand
* **CreateAsyncObservable** - Creates a command whose a async method returns
  `IObservable<T>`
* **CreateAsyncTask** - Create a command whose async method returns `Task` or
  `Task<T>`; use this method if you want to write a method with `async/await`.

All of these methods will parameterize the resulting ReactiveCommand to be the
return result of the method (i.e. if your async method returns `Task<String>`,
your Command will be `ReactiveCommand<String>`). This means, that Subscribing
to the Command itself returns the results of the async method as an
Observable.

ReactiveCommand itself guarantees that its results will *always* be delivered
on the UI thread, so extra `ObserveOn`s are unnecessary.

It is important to know, that ReactiveCommand itself as an `IObservable` will
never complete or OnError - errors that happen in the async method will
instead show up on the `ThrownExceptions` property.

If it is possible that your async method can throw an exception (and most
can!), you **must** Subscribe to `ThrownExceptions` or the exception will be
rethrown on the UI thread.

Here's a simple example:

```cs
LoadUsersAndAvatars = ReactiveCommand.CreateAsyncTask(async _ => {
    var users = await LoadUsers();

    foreach(var u in users) {
        u.Avatar = await LoadAvatar(u.Id);
    }

    return users;
});

LoadUsersAndAvatars.ToProperty(this, x => x.Users, out users);

LoadUsersAndAvatars.ThrownExceptions
    .Subscribe(ex => this.Log().WarnException("Failed to load users", ex));
```

### How can I execute the command?

The best way to execute ReactiveCommands is via the `ExecuteAsync` method:

```cs
LoadUsersAndAvatars = ReactiveCommand.CreateAsyncTask(async _ => {
    var users = await LoadUsers();

    foreach(var u in users) {
        u.Avatar = await LoadAvatar(u.Id);
    }

    return users;
});

var results = await LoadUsersAndAvatars.ExecuteAsync();
Console.WriteLine("You've got {0} users!", results.Count());
```

It is important that you **must await ExecuteAsync** or else it doesn't do
anything! `ExecuteAsync` returns a *Cold Observable*, which means that it only
does work once someone Subscribes to it.

For legacy code and for binding to UI frameworks, the Execute method is still
provided.

### Why CreateAsyncTask?

Since ReactiveCommand itself is an Observable, it's quite easy to invoke async
actions based on a ReactiveCommand. Something like:

```cs
searchButton
    .SelectMany(async x => await executeSearch(x))
    .ObserveOn(RxApp.MainThreadScheduler)
    .ToProperty(this, x => x.SearchResults, out searchResults);
```

However, while this pattern is approachable if you're handy with Rx, one thing
that ends up being Difficult™ is to disable the Command itself when the search
is running (i.e. to prevent more than one search from running at the same
time). CreateAsyncTask does the work to make this happen for you.

Another difficult aspect of this code is that it can't handle exceptions - if
`executeSearch` ever fails once, it will never signal again because of the Rx
Contract. ReactiveCommand handles marshaling exceptions to the
`ThrownExceptions` property, which can be handled.

### Common Patterns

This example from UserError also illustrates the canonical usage of
CreateAsyncTask:

```cs

// When LoadTweetsCommand is invoked, LoadTweets will be run in the
// background, the result will be Observed on the Main thread, and
// ToProperty will then store it in an Output Property
LoadTweetsCommand = ReactiveCommand.CreateAsyncTask(() => LoadTweets())

LoadTweetsCommand.ToProperty(this, x => x.TheTweets, ref theTweets);

var errorMessage = "The Tweets could not be loaded";
var errorResolution = "Check your Internet connection";

// Any exceptions thrown by LoadTweets will end up being
// sent through ThrownExceptions
LoadTweetsCommand.ThrownExceptions
    .Select(ex => new UserError(errorMessage, errorResolution))
    .Subscribe(x => UserError.Throw(x));
```
