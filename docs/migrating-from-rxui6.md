## 迁移到 ReactiveUI 7.0

### 稍微麻烦的变化

对于开发者来说，有一些变化可能需要做更多的工作。

#### ReactiveCommand 与之前不同

`ReactiveCommand` 再次被完全重写了（抱歉）。

* 取消了接口。 所有 `IReactiveCommand` 的用法都应该用 `ReactiveCommand` 替代, possibly with type information (see below).
* 静态创建方法的变化：
	* 在调用 `CreateXxx` 方法时，_总是_需求提供执行逻辑，包括 “同步” 命令（比如那些由 Create 创建的命令）。因此，无需调用 `Create` 然后订阅，直接在调用 `Create` 的时候提供执行逻辑。   
    * 为了保持一致性，执行逻辑总是作为第一个参数。其他参数（`canExecute`， `scheduler`）是可选的。
* 在 `ReactiveCommand<TParam, TResult>` 中使用 `TParam` 指定参数类型。
	* 如果命令需要一个参数，无需使用 `object` 并进行转换了。只需要在创建命令的时候显式指定参数类型即可。当然，有必要的话依然可以使用 `object`，或者将其作为迁移的中间步骤。
* 显式实现 `ICommand` 。因此：
    * 所有 `Execute` 都应替换为 `ExecuteAsync`。
    * `CanExecuteObservable` 简写为 `CanExecute`
* 在调用 `ExecuteAsync` 之后马上就会执行命令。不需要为了让命令逻辑开始执行而去订阅其返回了 observable 了。之后的观察者仍然可以收到执行的结果。
* `CanExecute` 和 `IsExecuting` 等 observable 现在是 behavioral 的了。这样，他们将为观察者总是提供最新的值（如果没有最新值，则提供默认值）。
* `RoutingState` 使用了新的实现。因此，它的所有命令都会受到影响。
* 移除了 `ToCommand` 扩展方法。该方法用于方便的创建 `IObservable<bool>`，并将其作为某个命令的 `canExecute` 。如果你使用了这些方法，可以用 ReactiveCommand 上的某个 CreateXxx 方法替代。

Old:

```cs
var canExecute = ...;
var someCommand = ReactiveCommand.Create(canExecute);
someCommand.Subscribe(x => /* e执行逻辑 */);

var someAsyncCommand1 = ReactiveCommand.CreateAsyncObservable(canExecute, someObservableMethod);
var someAsyncCommand2 = ReactiveCommand.CreateAsyncTask(canExecute, someTaskMethod);
```

New:

```cs
var canExecute = ...;
var someCommand = ReactiveCommand.Create(() => /* 执行逻辑 */);

var someAsyncCommand1 = ReactiveCommand.CreateAsyncObservable(someObservableMethod, canExecute);
var someAsyncCommand2 = ReactiveCommand.CreateAsyncTask(someTaskMethod, canExecute);
```

这里有一些更为详细的创建 `ReactiveCommand` 的方式，仅供参考：

```cs
// 没有参数，也不关心返回值
// 这些命令的类型都是 ReactiveCommand<Unit, Unit>
ReactiveCommand.Create(() => Console.WriteLine("hello")));
ReactiveCommand.CreateAsyncObservable(() => Observable.Return(Unit.Default));
ReactiveCommand.CreateAsyncTask(async () => await Task.Delay(TimeSpan.FromSeconds(1)));

// 有一个参数，不关心返回值
// 这些命令的类型都是 ReactiveCommand<int, Unit>
ReactiveCommand.Create<int>(param => Console.WriteLine(param)));
ReactiveCommand.CreateAsyncObservable<int, Unit>(param => Observable.Return(Unit.Default));
ReactiveCommand.CreateAsyncTask<int, Unit>(async param => await Task.Delay(TimeSpan.FromSeconds(param));

// 没有参数，返回 int
// 这些命令的类型都是 ReactiveCommand<Unit, int>
ReactiveCommand.Create(() => 5);
ReactiveCommand.CreateAsyncObservable(() => Observable.Return(42));
ReactiveCommand.CreateAsyncTask(() => Task.FromResult(42));

// 有一个参数，返回字符串
// 这些命令的类型都是 ReactiveCommand<int, string>
ReactiveCommand.Create<int, string>(param => param.ToString());
ReactiveCommand.CreateAsyncObservable<int, string>(param => Observable.Return(param.ToString()));
ReactiveCommand.CreateAsyncTask<int, string>(param => Task.FromResult(param.ToString()));

// 所有的例子中，都可以提供 canExecute 和 scheduler
var canExecute = ...;
var scheduler = ...;
ReactiveCommand.Create(() => {}, canExecute, scheduler);
```

为了方便你的迁移，所有先前的类型都可以在 `ReactiveUI.Legacy` 命名空间中找到。注意，`RoutingState` 没有遗留版本，因此与其交互的所有代码都需要略微改变。
