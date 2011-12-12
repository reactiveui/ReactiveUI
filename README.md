# ReactiveUI

This library is an exploration I've been working on for several weeks on
combining WPF Model-View-ViewModel paradigm with the Reactive Extensions for
.NET (Rx). Combining these two make managing concurrency as well as expressing
complicated interactions between objects possible in a declarative, functional
way. Put simply, if you've ever had to chain events / callbacks together and
declare state ints/booleans to keep track of what's going on, Reactive
Extensions provides a sane alternative.

## What's in this library

``ReactiveCommand`` - an implementation of ICommand that is also a Subject whose
OnNext is raised when Execute is executed. Its CanExecute can also be defined by
an IObservable<bool> which means the UI will instantly update instead of
implementations which rely on RequerySuggested.

``ReactiveAsyncCommand`` - a derivative of ReactiveCommand that encapsulates the
common pattern of "Fire asynchronous command, then marshal result back onto
dispatcher thread". Allows you to set a maximum level of concurrency as well
(i.e. "I only want 3 inflight requests" - when the maximum is reached,
CanExecute returns false).

``ReactiveObject`` - a ViewModel object based on Josh Smith's implementation,
that also implements IObservable as a way to notify property changes. It also
allows a straightforward way to observe the changes of a single property.

``ReactiveValidatedObject`` - a derivative of ReactiveObject that is validated
via DataAnnotations by implementing IDataErrorInfo, so properties can be
annotated with their restrictions and the UI will automatically reflect the
errors.

``ObservableAsPropertyHelper<T>`` - a class that easily lets you convert an
IObservable<T> into a property that stores its latest value, as well as fires
NotifyPropertyChanged when the property changes. This is really useful for
combining existing properties together and replacing IValueConverters, since
your ViewModels will also be IObservables.

``StopwatchTestScheduler`` - this class allows you to enforce time limits on
items scheduled on other threads. The main use for this is in unit tests, as
well as being able to say things in Debug mode like, "If any item runs in the
Dispatcher scheduler for longer than 400ms that would've made the UI
unresponsive, crash the application".

## Blend SDK Integration

``AsyncCommandVisualStateBehavior`` - this behavior will watch a
ReactiveAsyncCommand and transition its target to different states based on the
command's status - for example, displaying a Spinner while a command is running. 

``FollowObservableStateBehavior`` - this behavior will use the output of an
IObservable<string> and call VisualStateManager.GoToState on its target; using
Observable.Merge makes it fairly straightforward to build a state machine based
on the changes in the ViewModel.

``ObservableTrigger`` - this trigger will fire when an IObservable calls OnNext
and can be tied to any arbitrary Expression Action.

## Other stuff that's useful

``MemoizingMRUCache`` - this class is non-threadsafe most recently used cache,
and can be used to cache the results of expensive lookups. You provide the
function to use to look up values that aren't known, then it will save the
results. It also allows a "destructor" to be run when an item is released from
the cache, so you can use this to manage an on-disk file cache as well (where
the "Get" function downloads a file, then the "Release" function deletes it).

``QueuedAsyncMRUCache`` - this class is by far the most complicated in this
library, its goals are similar to MemoizingMRUCache, but instead of returning
the result immediately, it will schedule a Task to run in the background and
return an IObservable representing the result (a Future). Once the Future
completes, its result is cached so subsequent requests will come from memory.

The advantage of this class is that subsequent identical requests will block on
the outstanding one (so if you ask for "foo.com" on 3 separate threads, one of
them will send out the web request and the other two threads will receive the
result as well). This class also allows you to place a blocking limit on the
number of outstanding requests, so that further requests will block until some
of the inflight requests have been satisfied. 

``IEnableLogger`` - this is an implementation of a simple logger that combines
some of log4net's syntax with the ubiquity of the Rails logger - any class that
implements the dummy IEnableLogger interface will able to access a logger for
that class (i.e. `this.Log().Warn("Something bad happened!");`)
