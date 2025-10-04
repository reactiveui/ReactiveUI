# Static State Test Isolation

This directory documents the approach for handling static/global state in ReactiveUI tests.

## Problem

ReactiveUI uses several static/global entry points for configuration:
- `RxApp.MainThreadScheduler` and `RxApp.TaskpoolScheduler`
- `RxApp.EnsureInitialized()` (initializes the service locator)
- `MessageBus.Current`
- `Locator.Current` / `Locator.CurrentMutable` (from Splat)

When tests access or modify these static states, they can cause interference between parallel test executions, leading to intermittent failures.

## Solution: NonParallelizable Attribute

The approach taken is to mark test fixtures that use static state as `[NonParallelizable]` to prevent concurrent access rather than attempting complex state snapshot/restore mechanisms.

**When to mark a test fixture as `[NonParallelizable]`:**

1. The test calls `RxApp.EnsureInitialized()`
2. The test modifies or reads `RxApp` properties (schedulers, SuspensionHost, etc.)
3. The test accesses or modifies `Locator.CurrentMutable`
4. The test uses `MessageBus.Current`
5. The test creates instances that depend on service locator registrations (e.g., `HostTestFixture`)

**Example:**
```csharp
[TestFixture]
[NonParallelizable]
public class MyTests
{
    [Test]
    public void MyTest()
    {
        RxApp.EnsureInitialized();
        // Test code that uses static state
    }
}
```

## Why Not State Restoration?

While it might seem appealing to create helper scopes that snapshot and restore static state, this approach has significant drawbacks:

1. **Splat's Locator API complexity**: `Locator.SetLocator()` requires `IDependencyResolver` but `Locator.Current` returns `IReadonlyDependencyResolver`, making proper restoration difficult.
2. **Fragility**: Attempting to snapshot/restore complex DI container state is error-prone and can lead to subtle bugs.
3. **Simplicity**: The `[NonParallelizable]` attribute is explicit, simple, and foolproof.

Tests that truly need to modify and restore state can do so manually in their `[SetUp]`/`[TearDown]` methods, but in most cases simply preventing parallel execution is sufficient.

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
