# Static State Test Isolation

This directory contains helper classes for managing static/global state in ReactiveUI tests.

## Problem

ReactiveUI uses several static/global entry points for configuration:
- `RxApp.MainThreadScheduler` and `RxApp.TaskpoolScheduler`
- `RxApp.EnsureInitialized()` (initializes the service locator)
- `MessageBus.Current`
- `Locator.Current` / `Locator.CurrentMutable` (from Splat)

When tests access or modify these static states, they can cause interference between test executions, leading to intermittent failures and hidden state pollution.

## Solution: NonParallelizable + State Restoration

Tests that use static state should be marked `[NonParallelizable]` **AND** use state restoration scopes to ensure clean state between tests.

### Available Helper Scopes

#### 1. RxAppSchedulersScope

Snapshots and restores `RxApp.MainThreadScheduler` and `RxApp.TaskpoolScheduler`.

**Usage:**
```csharp
[TestFixture]
[NonParallelizable]
public class MyTests
{
    private RxAppSchedulersScope? _schedulersScope;

    [SetUp]
    public void SetUp()
    {
        _schedulersScope = new RxAppSchedulersScope();
        // Now safe to modify RxApp schedulers
    }

    [TearDown]
    public void TearDown()
    {
        _schedulersScope?.Dispose();
    }
}
```

#### 2. MessageBusScope

Snapshots and restores `MessageBus.Current`.

**Usage:**
```csharp
[TestFixture]
[NonParallelizable]
public class MyTests
{
    private MessageBusScope? _messageBusScope;

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

#### 3. StaticStateScope (Generic)

A generic helper for capturing and restoring arbitrary static state using getter/setter pairs.

**Usage:**
```csharp
[TestFixture]
[NonParallelizable]
public class MyTests
{
    private StaticStateScope? _stateScope;

    [SetUp]
    public void SetUp()
    {
        _stateScope = new StaticStateScope(
            () => MyClass.StaticProperty,
            (object? value) => MyClass.StaticProperty = value,
            () => AnotherClass.StaticField,
            (object? value) => AnotherClass.StaticField = value);
    }

    [TearDown]
    public void TearDown()
    {
        _stateScope?.Dispose();
    }
}
```

## When to Use These Scopes

**Always use state restoration scopes when:**

1. The test calls `RxApp.EnsureInitialized()`
2. The test modifies `RxApp` properties (schedulers, SuspensionHost, etc.)
3. The test modifies `MessageBus.Current`
4. The test accesses or modifies `Locator.CurrentMutable`
5. The test creates instances that depend on service locator registrations

**Why both NonParallelizable AND state restoration?**

- `[NonParallelizable]` prevents concurrent access issues
- State restoration ensures clean state even when tests run sequentially
- This prevents hidden state pollution that can cause issues in future test runs

## Important Notes

1. **Always mark test fixtures as `[NonParallelizable]`** when using these scopes
2. **State restoration does NOT make tests safe for parallel execution** - it only ensures cleanup
3. **For Splat's Locator**: Due to API complexity, use `StaticStateScope` or manual cleanup if needed

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
