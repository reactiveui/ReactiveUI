# Asynchronous operations with ReactiveCommand

One of the most important features of ReactiveCommand is its built-in
facilities for orchestrating asynchronous operations. In previous versions of
ReactiveUI, this was in a separate command class, but starting with ReactiveUI
5.0, this is built-in.

### Commands and RegisterAsync

To use ReactiveCommand with async operations, use the `RegisterAsync` family
of methods, depending on what your async operation returns:

* **RegisterAsync** - Registers an async method that returns `IObservable<T>`
* **RegisterAsyncTask** - Registers an async method that returns `Task` or
  `Task<T>`; use this method if you want to write a method with `async/await`.
* **RegisterAsyncFunc** - Registers a synchronous method that returns a value
  and is run on a background thread.
* **RegisterAsyncAction** - Registers a synchronous method that does not
  return a value and is run on a background thread.

All of these methods return an `IObservable` which, when subscribed to,
returns the results of the computations. All of these methods guarantee to
deliver results *on the main thread*, so extra `ObserveOn`s are unnecessary.

It is important to know, that the returned `IObservable` will never complete
or OnError - errors that happen in the async method will instead show up on
the `ThrownExceptions` property. If it is possible that your async method can
throw an exception (and most can!), you **must** Subscribe to
`ThrownExceptions` or the exception will be rethrown on the UI thread.

Here's a simple example:

```cs
LoadUsersAndAvatars = new ReactiveCommand();

var usersAndAvatarResults = LoadUsersAndAvatars.RegisterAsyncTask(async _ => {
    var users = await LoadUsers();

    foreach(var u in users) {
        u.Avatar = await LoadAvatar(u.Id);
    }

    return users;
});

usersAndAvatarResults.ToProperty(this, x => x.Users, ref users);

usersAndAvatarResults.ThrownExceptions
    .Subscribe(ex => this.Log().WarnException("Failed to load users", ex));
```

### Why RegisterAsync?

Since ReactiveCommand itself is an Observable, it's quite easy to invoke async
actions based on a ReactiveCommand. Something like:

```cs
searchButton
    .SelectMany(async x => executeSearch(x))
    .ObserveOn(RxApp.MainThreadScheduler)
    .ToProperty(this, x => x.SearchResults, out searchResults);
```

However, while this pattern is approachable if you're handy with Rx, one thing
that ends up being Difficult™ is to disable the Command itself when the search
is running (i.e. to prevent more than one search from running at the same
time). RegisterAsync does the work to make this happen for you. 

Another difficult aspect of this code is that it can't handle exceptions - if
`executeSearch` ever fails once, it will never signal again because of the Rx
Contract. ReactiveCommand handles marshaling exceptions to the
`ThrownExceptions` property, which can be handled.

### Common Patterns

This example from UserError also illustrates the canonical usage of
RegisterAsync:

```cs
LoadTweetsCommand = new ReactiveCommand();

// When LoadTweetsCommand is invoked, LoadTweets will be run in the
// background, the result will be Observed on the Main thread, and
// ToProperty will then store it in an Output Property
LoadTweetsCommand.RegisterAsyncTask(() => LoadTweets())
    .ToProperty(this, x => x.TheTweets, ref theTweets);

var errorMessage = "The Tweets could not be loaded";
var errorResolution = "Check your Internet connection";

// Any exceptions thrown by LoadTweets will end up being
// sent through ThrownExceptions
LoadTweetsCommand.ThrownExceptions
    .Select(ex => new UserError(errorMessage, errorResolution))
    .Subscribe(x => UserError.Throw(x));
```
