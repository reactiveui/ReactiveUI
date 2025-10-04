# Static State Helper Scopes

This directory contains helper classes for managing static/global state in tests.

## Purpose

ReactiveUI uses several static/global entry points for configuration:
- `RxApp.MainThreadScheduler` and `RxApp.TaskpoolScheduler`
- `MessageBus.Current`
- `Locator.Current` / `Locator.CurrentMutable` (from Splat)

When tests modify these static states, they can cause interference between parallel test executions, leading to intermittent failures. The helper scopes in this directory provide a pattern for snapshot/restore of static state.

## Available Scopes

### RxAppSchedulersScope

Snapshots and restores `RxApp.MainThreadScheduler` and `RxApp.TaskpoolScheduler`.

**Usage:**
```csharp
[TestFixture]
[NonParallelizable]
public class MyTests
{
    private RxAppSchedulersScope _schedulersScope;

    [SetUp]
    public void SetUp()
    {
        _schedulersScope = new RxAppSchedulersScope();
        // Now safe to modify RxApp schedulers
        RxApp.MainThreadScheduler = ImmediateScheduler.Instance;
    }

    [TearDown]
    public void TearDown()
    {
        _schedulersScope?.Dispose();
    }
}
```

### MessageBusScope

Snapshots and restores `MessageBus.Current`.

**Usage:**
```csharp
[TestFixture]
[NonParallelizable]
public class MyTests
{
    private MessageBusScope _messageBusScope;

    [SetUp]
    public void SetUp()
    {
        _messageBusScope = new MessageBusScope();
        // Now safe to use or replace MessageBus.Current
    }

    [TearDown]
    public void TearDown()
    {
        _messageBusScope?.Dispose();
    }
}
```

## Important Notes

1. **Always mark test fixtures as `[NonParallelizable]`** if they:
   - Call `RxApp.EnsureInitialized()`
   - Modify `RxApp` properties (schedulers, SuspensionHost, etc.)
   - Access or modify `Locator.CurrentMutable`
   - Use `MessageBus.Current`
   - Create instances that depend on service locator registrations (e.g., `HostTestFixture`)

2. **These scopes do NOT make tests safe for parallel execution** - they only help with cleanup. Tests touching static state should always be marked `[NonParallelizable]`.

3. **For Splat's Locator**: Due to the complexity of Splat's dependency resolver API, we don't provide a `LocatorScope`. Instead, tests that modify Locator state should be marked `[NonParallelizable]` and clean up their own registrations if needed.

## Test Fixtures Already Marked as NonParallelizable

The following test fixtures are marked `[NonParallelizable]` because they use static/global state:

- `AutoPersistHelperTest` - uses HostTestFixture which depends on service locator
- `MessageBusTest` - uses Locator.CurrentMutable and MessageBus.Current
- `RandomTests` - uses RxApp, Locator.CurrentMutable, MessageBus.Current
- `RxAppTest` - accesses RxApp.MainThreadScheduler
- `ReactiveCommandTest` - calls RxApp.EnsureInitialized() in constructor
- `PocoObservableForPropertyTests` - calls RxApp.EnsureInitialized()
- `AwaiterTest` - accesses RxApp.TaskpoolScheduler
- `RxAppDependencyObjectTests` - calls RxApp.EnsureInitialized() and accesses Locator.Current
- `RoutedViewHostTests` - uses Locator.CurrentMutable to register services
- `ViewModelViewHostTests` - uses Locator.CurrentMutable.Register()
- `WpfCommandBindingImplementationTests` - registers test logger in Locator
- `DefaultPropertyBindingTests` - calls RxApp.EnsureInitialized() in constructor

Each of these fixtures has XML documentation explaining the specific reason for being NonParallelizable.
