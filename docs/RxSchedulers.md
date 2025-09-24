# RxSchedulers: Consuming ReactiveUI Schedulers Without RequiresUnreferencedCode

## Problem

When using `RxApp.MainThreadScheduler` or `RxApp.TaskpoolScheduler` in your code, since the entire `RxApp` class triggers initialization that is marked with `RequiresUnreferencedCode` attributes, any code that consumes these schedulers must also be marked with the same attributes.

This is particularly problematic when creating observables in ViewModels, Repositories, or other deeper code that is consumed by multiple sources, as it forces all consumers to add `RequiresUnreferencedCode` attributes.

## Solution

The new `RxSchedulers` static class provides access to the same scheduler functionality without requiring unreferenced code attributes. This class contains only the scheduler properties and doesn't trigger the Splat dependency injection initialization that requires reflection.

## Usage Examples

### Basic Usage

```csharp
// Old way - requires RequiresUnreferencedCode attribute
[RequiresUnreferencedCode("Uses RxApp which may require unreferenced code")]
public IObservable<string> GetDataOld()
{
    return Observable.Return("data")
        .ObserveOn(RxApp.MainThreadScheduler);  // Triggers RequiresUnreferencedCode
}

// New way - no attributes required
public IObservable<string> GetDataNew()
{
    return Observable.Return("data")
        .ObserveOn(RxSchedulers.MainThreadScheduler);  // No attributes needed!
}
```

### ViewModel Example

```csharp
public class MyViewModel : ReactiveObject
{
    private readonly ObservableAsPropertyHelper<string> _greeting;

    public MyViewModel()
    {
        // Using RxSchedulers avoids RequiresUnreferencedCode
        _greeting = this.WhenAnyValue(x => x.Name)
            .Select(name => $"Hello, {name ?? "World"}!")
            .ObserveOn(RxSchedulers.MainThreadScheduler)  // No attributes needed!
            .ToProperty(this, nameof(Greeting), scheduler: RxSchedulers.MainThreadScheduler);
    }

    public string? Name { get; set; }
    public string Greeting => _greeting.Value;
}
```

### Repository Pattern

```csharp
public class DataRepository
{
    public IObservable<string> GetProcessedData()
    {
        // Using RxSchedulers in repository code doesn't force consumers 
        // to add RequiresUnreferencedCode attributes
        return GetRawData()
            .ObserveOn(RxSchedulers.TaskpoolScheduler)  // Background processing
            .Select(ProcessData)
            .ObserveOn(RxSchedulers.MainThreadScheduler);  // UI updates
    }
}
```

### ReactiveProperty Factory Methods

```csharp
// New factory methods that use RxSchedulers internally
var property1 = ReactiveProperty<string>.Create();  // No attributes required
var property2 = ReactiveProperty<string>.Create("initial value");
var property3 = ReactiveProperty<int>.Create(42, skipCurrentValueOnSubscribe: false, allowDuplicateValues: true);
```

## API Reference

### RxSchedulers Properties

- `RxSchedulers.MainThreadScheduler` - Scheduler for UI thread operations (no unit test detection)
- `RxSchedulers.TaskpoolScheduler` - Scheduler for background operations (no unit test detection)

### ReactiveProperty Factory Methods

- `ReactiveProperty<T>.Create()` - Creates with default scheduler
- `ReactiveProperty<T>.Create(T initialValue)` - Creates with initial value  
- `ReactiveProperty<T>.Create(T initialValue, bool skipCurrentValueOnSubscribe, bool allowDuplicateValues)` - Full configuration
- `ReactiveProperty<T>.Create(T initialValue, IScheduler scheduler, bool skipCurrentValueOnSubscribe, bool allowDuplicateValues)` - Custom scheduler

## Compatibility

- `RxApp` schedulers still work as before - no breaking changes
- `RxApp` and `RxSchedulers` are kept synchronized when schedulers are set
- For code that needs unit test detection, continue using `RxApp` schedulers
- For new code that doesn't need unit test detection, prefer `RxSchedulers`

## When to Use Each

### Use `RxSchedulers` when:
- Creating library code that shouldn't require `RequiresUnreferencedCode` attributes
- Building ViewModels, repositories, or services consumed by multiple sources
- You don't need automatic unit test scheduler detection
- You want to avoid triggering ReactiveUI's dependency injection initialization

### Use `RxApp` schedulers when:
- You need automatic unit test scheduler detection
- You're already using other `RxApp` features
- Existing code that's already marked with `RequiresUnreferencedCode`
- You need the full ReactiveUI initialization sequence

## Migration Guide

To migrate existing code from `RxApp` to `RxSchedulers`:

1. Replace `RxApp.MainThreadScheduler` with `RxSchedulers.MainThreadScheduler`
2. Replace `RxApp.TaskpoolScheduler` with `RxSchedulers.TaskpoolScheduler`
3. Remove `RequiresUnreferencedCode` and `RequiresDynamicCode` attributes if they were only needed for scheduler access
4. Use `ReactiveProperty<T>.Create()` factory methods instead of constructors
5. Test that unit tests still work (you may need to manually set test schedulers if you relied on automatic detection)

## Notes

- `RxSchedulers` provides a simplified version without unit test detection
- In unit test environments, you may need to manually set the schedulers if you were relying on automatic detection
- The schedulers default to `DefaultScheduler.Instance` for main thread and `TaskPoolScheduler.Default` for background
- This solution maintains full backwards compatibility with existing code