# CreateCollection + Testing

One of the advantages of Rx is that it makes previously untestable code into
code that has deterministic, easy to verify tests. Most interesting race
conditions in applications are secretly **Ordering Issues**, not *Timing Issues*;
the issue happens when This happens, then This, then That.

Rx lets you easily control the ordering of events to happen in arbitrary
combinations that happen in a deterministic way. This means that your 'async'
tests execute the same way every time.

### Rx is easy to test

The key to controlling concurrency in Rx, is that anything deferred (i.e.
executed on 'another thread') is sent through the `IScheduler` interface. As
mentioned in the page on Schedulers, ReactiveUI allows you to replace the
global schedulers `MainThreadScheduler` and `TaskpoolScheduler` with your own.

### Use CreateCollection to verify changes

One easy way to test methods that return `IObservable<T>` is to simply dump
it to a collection, then check the contents after-the-fact. `CreateCollection`
differs from `ToList` in that `CreateCollection` returns *immediately*,
whereas `ToList` only returns a value once the Observable completes. 

Check out this test from ReactiveList where we're putting all of the Reset
events into a list:

```cs
public void GetAResetWhenWeAddALotOfItems()
{
    var fixture = new ReactiveList<int> { 1, };
    var reset = fixture.ShouldReset.CreateCollection();
    Assert.Equal(0, reset.Count);

    fixture.AddRange(new[] { 2,3,4,5,6,7,8,9,10,11,12,13, });
    Assert.Equal(1, reset.Count);
}
```

### Subjects are useful for faking inputs

Rx also makes it easy to mock asynchronous methods in a synchronous way, using
Subjects. When you replace concurrent methods (i.e. methods that are running
on background threads) with Subjects, they now run instantly on a single thread -
meaning that the tests execute the same way.

Here's another sample from the ReactiveUI test suite:

```cs
public void DerivedCollectionSignalledToResetShouldFireExactlyOnce()
{
    var input = new List<string> { "Foo" };
    var resetSubject = new Subject<Unit>();
    var derived = input.CreateDerivedCollection(x => x, signalReset: resetSubject);
    
    var changeNotifications = derived.Changed.CreateCollection();

    Assert.Equal(0, changeNotifications.Count);
    Assert.Equal(1, derived.Count);

    input.Add("Bar");

    // Shouldn't have picked anything up since the input isn't reactive
    Assert.Equal(0, changeNotifications.Count);
    Assert.Equal(1, derived.Count);

    resetSubject.OnNext(Unit.Default);

    Assert.Equal(1, changeNotifications.Count);
    Assert.Equal(2, derived.Count);
}
```

We set up things to asynchronously wait on Observables, then use
Subject.OnNext to move the order of events forward to the next part of the
operation.

### When to use TestScheduler
