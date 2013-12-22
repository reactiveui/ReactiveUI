# ReactiveUI.Events

While ReactiveUI is primarily concerned with creating ViewModels, a separate
library exists to help you write code that is intended to live in the View,
called "ReactiveUI.Events" (NuGet package name, `ReactiveUI-Events`).

This library is a code generated library that adds Observables for all events
in the UI framework, via a new extension methods `Events()`. Most usages of
`Observable.FromEventPattern` can be replaced in the Views. Events are
straightforward mappings of the event parameters.

### Examples

```cs
var router = RxApp.GetService<IScreen>().Router;

this.Events().KeyUp
	.Where(x => x.Key == Key.Escape)
	.InvokeCommand(router.NavigateBack);
```

```cs
var windowChanged = Observable.Merge(
	this.Events().SizeChanged.Select(_ => Unit.Default),
	this.WhenAny(x => x.Left, x => x.Top, (l,t) => Unit.Default));

windowChanged
	.Throttle(TimeSpan.FromMilliseconds(700), RxApp.MainThreadScheduler)
	.Subscribe(_ => SaveWindowPosition());
```
