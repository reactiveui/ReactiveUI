# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Test Commands

This project uses **Microsoft Testing Platform (MTP)** with the **TUnit** testing framework. Test commands differ significantly from traditional VSTest.

See: https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-test?tabs=dotnet-test-with-mtp

### Prerequisites

```powershell
# Check .NET installation
dotnet --info

# CRITICAL: Restore platform-specific workloads (MAUI, etc.)
# This MUST be run from the /src directory or the build will fail
cd src
dotnet workload restore
cd ..

# Restore NuGet packages
dotnet restore ReactiveUI.sln
```

### Build Commands

**CRITICAL:** The working folder must be `./src` folder. These commands won't function properly without the correct working folder.

```powershell
# Build the solution (requires Windows for platform-specific targets)
dotnet build ReactiveUI.sln -c Release

# Build with warnings as errors (includes StyleCop violations)
dotnet build ReactiveUI.sln -c Release -warnaserror

# Clean the solution
dotnet clean ReactiveUI.sln
```

### Test Commands (Microsoft Testing Platform)

**CRITICAL:** This repository uses MTP configured in `global.json`. All TUnit-specific arguments must be passed after `--`:

The working folder must be `./src` folder. These commands won't function properly without the correct working folder.

```powershell
# Run all tests in the solution
dotnet test --solution ReactiveUI.sln -c Release

# Run all tests without building first (faster when code hasn't changed)
dotnet test --solution ReactiveUI.sln -c Release --no-build

# Run all tests in a specific project
dotnet test --project tests/ReactiveUI.Tests/ReactiveUI.Tests.csproj

# Run a single test method using treenode-filter
# Syntax: /{AssemblyName}/{Namespace}/{ClassName}/{TestMethodName}
dotnet test --project tests/ReactiveUI.Tests/ReactiveUI.Tests.csproj -- --treenode-filter "/*/*/*/MyTestMethod"

# Run all tests in a specific class
dotnet test --project tests/ReactiveUI.Tests/ReactiveUI.Tests.csproj -- --treenode-filter "/*/*/MyClassName/*"

# Run tests in a specific namespace
dotnet test --project tests/ReactiveUI.Tests/ReactiveUI.Tests.csproj -- --treenode-filter "/*/MyNamespace/*/*"

# Filter by test property (e.g., Category)
dotnet test --solution ReactiveUI.sln -- --treenode-filter "/*/*/*/*[Category=Integration]"

# Run tests with code coverage (Microsoft Code Coverage)
dotnet test --solution ReactiveUI.sln -- --coverage --coverage-output-format cobertura

# Run tests with detailed output
dotnet test --solution ReactiveUI.sln -- --output Detailed

# List all available tests without running them
dotnet test --project tests/ReactiveUI.Tests/ReactiveUI.Tests.csproj -- --list-tests

# Fail fast (stop on first failure)
dotnet test --solution ReactiveUI.sln -- --fail-fast

# Control parallel test execution
dotnet test --solution ReactiveUI.sln -- --maximum-parallel-tests 4

# Generate TRX report
dotnet test --solution ReactiveUI.sln -- --report-trx

# Disable logo for cleaner output
dotnet test --project tests/ReactiveUI.Tests/ReactiveUI.Tests.csproj -- --disable-logo

# Combine options: coverage + TRX report + detailed output
dotnet test --solution ReactiveUI.sln -- --coverage --coverage-output-format cobertura --report-trx --output Detailed
```

**Alternative: Using `dotnet run` for single project**
```powershell
# Run tests using dotnet run (easier for passing flags)
dotnet run --project tests/ReactiveUI.Tests/ReactiveUI.Tests.csproj -c Release -- --treenode-filter "/*/*/*/MyTest"

# Disable logo for cleaner output
dotnet run --project tests/ReactiveUI.Tests/ReactiveUI.Tests.csproj -- --disable-logo --treenode-filter "/*/*/*/Test1"
```

### TUnit Treenode-Filter Syntax

The `--treenode-filter` follows the pattern: `/{AssemblyName}/{Namespace}/{ClassName}/{TestMethodName}`

**Examples:**
- Single test: `--treenode-filter "/*/*/*/MyTestMethod"`
- All tests in class: `--treenode-filter "/*/*/MyClassName/*"`
- All tests in namespace: `--treenode-filter "/*/MyNamespace/*/*"`
- Filter by property: `--treenode-filter "/*/*/*/*[Category=Integration]"`
- Multiple wildcards: `--treenode-filter "/*/*/MyTests*/*"`

**Note:** Use single asterisks (`*`) to match segments. Double asterisks (`/**`) are not supported in treenode-filter.

### Key TUnit Command-Line Flags

- `--treenode-filter` - Filter tests by path pattern or properties (syntax: `/{Assembly}/{Namespace}/{Class}/{Method}`)
- `--list-tests` - Display available tests without running
- `--fail-fast` - Stop after first failure
- `--maximum-parallel-tests` - Limit concurrent execution (default: processor count)
- `--coverage` - Enable Microsoft Code Coverage
- `--coverage-output-format` - Set coverage format (cobertura, xml, coverage)
- `--report-trx` - Generate TRX format reports
- `--output` - Control verbosity (Normal or Detailed)
- `--no-progress` - Suppress progress reporting
- `--disable-logo` - Remove TUnit logo display
- `--diagnostic` - Enable diagnostic logging (Trace level)
- `--timeout` - Set global test timeout
- `--reflection` - Enable reflection mode instead of source generation

See https://tunit.dev/docs/reference/command-line-flags for complete TUnit flag reference.

### Key Configuration Files

- `global.json` - Specifies `"Microsoft.Testing.Platform"` as the test runner
- `testconfig.json` - Configures test execution (`"parallel": false`) and code coverage (Cobertura format)
- `Directory.Build.props` - Enables `TestingPlatformDotnetTestSupport` for test projects
- `.github/copilot-instructions.md` - Comprehensive development guidelines

## Architecture Overview

### Core Framework Structure

ReactiveUI is a cross-platform MVVM framework built on Reactive Extensions (Rx.NET). The architecture follows functional reactive programming principles:

**Core Library (`ReactiveUI/`)**
- `ReactiveObject/` - Base class implementing `INotifyPropertyChanged` with reactive extensions
- `ReactiveCommand/` - Encapsulates user actions as observable command execution pipelines
- `ReactiveProperty/` - Observable properties with change notification and subscription management
- `Activation/` - View and ViewModel activation lifecycle management
- `Bindings/` - One-way and two-way property binding infrastructure
- `Expression/` - Expression tree analysis for property observation (e.g., `WhenAnyValue`)
- `ObservableForProperty/` - Platform-specific property change notification adapters
- `Routing/` - Navigation and view model routing infrastructure
- `Interactions/` - Request/response pattern for view-viewmodel communication
- `Builder/` - Dependency injection and service registration patterns

**Platform Extensions (`ReactiveUI.*`)**
- `ReactiveUI.Wpf/` - WPF-specific view activation and bindings
- `ReactiveUI.WinUI/` - WinUI platform integration
- `ReactiveUI.Maui/` - .NET MAUI cross-platform support
- `ReactiveUI.AndroidX/` - Android-specific extensions
- `ReactiveUI.Blazor/` - Blazor web framework integration
- `ReactiveUI.Winforms/` - Windows Forms support
- `ReactiveUI.Blend/` - Expression Blend design-time support
- `ReactiveUI.Testing/` - Testing utilities and schedulers

### Key Architectural Patterns

**Scheduler Abstraction**
- `RxApp` - Main application entry point with dependency injection initialization (requires `RequiresUnreferencedCode`)
- `RxSchedulers` - Scheduler access without reflection/AOT attributes (preferred for library code)
- See `docs/RxSchedulers.md` for detailed guidance on choosing between `RxApp` and `RxSchedulers`

**Property Observation Pipeline**
1. `WhenAnyValue` / `WhenAny` - Expression-based property change observation
2. `ObservableAsPropertyHelper` - Computed properties derived from observables
3. `RaiseAndSetIfChanged` - Manual property change notification

**Command Pipeline**
- `ReactiveCommand.Create*` factory methods for synchronous/asynchronous commands
- Observable-based `CanExecute` conditions
- Automatic error handling and execution state tracking

**View Activation**
- `WhenActivated` pattern for managing subscriptions during view lifecycle
- Automatic disposal when views are deactivated
- Platform-specific activation hooks

### Multi-Platform Target Framework Strategy

The project uses granular target framework definitions in `Directory.Build.props`:

- `ReactiveUICoreTargets` - net8.0, net9.0, net10.0 (cross-platform)
- `ReactiveUIFrameworkTargets` - net462, net472, net481 (Windows-only legacy)
- `ReactiveUIWindowsTargets` - net8.0/9.0/10.0-windows10.0.19041.0
- `ReactiveUIAppleTargets` - iOS, tvOS, macOS, Mac Catalyst
- `ReactiveUIAndroidTargets` - Android platform targets
- `ReactiveMauiTargets` - MAUI-specific multi-platform targets

**Windows-Only Requirements:** Building requires Windows for platform-specific targets (WPF, WinUI, .NET Framework). Non-Windows builds will fail - this is expected.

### AOT (Ahead-of-Time) Compilation Support

All code targeting net8.0+ must be AOT-compatible (`IsAotCompatible=true`).

**Key AOT Patterns:**
- Prefer `DynamicallyAccessedMembersAttribute` over `UnconditionalSuppressMessage`
- Use specific `DynamicallyAccessedMemberTypes` values rather than `All` when possible
- Prefer strongly-typed expressions over string-based property names
- Use `nameof()` for compile-time property name checking
- See `tests/ReactiveUI.AOTTests/` for AOT test examples

**Example AOT-Safe Code:**
```csharp
// For methods that access properties via reflection
#if NET6_0_OR_GREATER
private static PropertyInfo? GetPropertyInfo(
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
    Type type, string propertyName)
#else
private static PropertyInfo? GetPropertyInfo(Type type, string propertyName)
#endif
{
    return type.GetProperty(propertyName);
}
```

See `.github/copilot-instructions.md` for comprehensive AOT patterns and examples.

## Code Style & Quality Requirements

**CRITICAL:** All code must comply with ReactiveUI contribution guidelines: https://www.reactiveui.net/contribute/index.html

### Style Enforcement

- EditorConfig rules (`.editorconfig`) - comprehensive C# formatting and naming conventions
- StyleCop Analyzers - builds fail on violations
- Roslynator Analyzers - additional code quality rules
- Analysis level: latest with enhanced .NET analyzers
- `WarningsAsErrors`: nullable, CS4014
- **All public APIs require XML documentation comments** (including protected methods of public classes)

### C# Style Rules

- **Braces:** Allman style (each brace on new line)
- **Indentation:** 4 spaces, no tabs
- **Fields:** `_camelCase` for private/internal, `readonly` where possible, `static readonly` (not `readonly static`)
- **Visibility:** Always explicit (e.g., `private string _foo` not `string _foo`), visibility first modifier
- **Namespaces:** File-scoped preferred, imports outside namespace, sorted (system then third-party)
- **Types:** Use keywords (`int`, `string`) not BCL types (`Int32`, `String`)
- **Modern C#:** Use nullable reference types, pattern matching, switch expressions, records, init setters, target-typed new, collection expressions, file-scoped namespaces, primary constructors
- **Avoid `this.`** unless necessary
- **Use `nameof()`** instead of string literals
- **Use `var`** when it improves readability or aids refactoring

See `.github/copilot-instructions.md` for complete style guide.

## Testing Guidelines

- Unit tests use **TUnit** framework with **Microsoft Testing Platform**
- Test projects detected via naming convention (`.Test` in project name)
- Coverage configured in `testconfig.json` (Cobertura format, skip auto-properties)
- Non-parallel test execution (`"parallel": false` in testconfig.json)
- Always write unit tests for new features or bug fixes
- Use `ReactiveUI.Testing` package for testing reactive code
- Follow existing test patterns in `tests/ReactiveUI.Tests/`
- For AOT scenarios, reference patterns in `tests/ReactiveUI.AOTTests/`

## Common Development Patterns

### ViewModel Creation

```csharp
public class SampleViewModel : ReactiveObject
{
    private string? _name;
    private readonly ObservableAsPropertyHelper<bool> _isValid;

    public SampleViewModel()
    {
        // Setup validation
        _isValid = this.WhenAnyValue(x => x.Name)
            .Select(name => !string.IsNullOrWhiteSpace(name))
            .ToProperty(this, nameof(IsValid));

        // Setup commands
        SubmitCommand = ReactiveCommand.CreateFromTask(
            ExecuteSubmit,
            this.WhenAnyValue(x => x.IsValid));
    }

    public string? Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    public bool IsValid => _isValid.Value;

    public ReactiveCommand<Unit, Unit> SubmitCommand { get; }

    private async Task ExecuteSubmit(CancellationToken cancellationToken)
    {
        // Submit logic here
    }
}
```

### Using RxSchedulers (AOT-Friendly)

```csharp
// Preferred: Use RxSchedulers to avoid RequiresUnreferencedCode
public IObservable<string> GetData()
{
    return Observable.Return("data")
        .ObserveOn(RxSchedulers.MainThreadScheduler);  // No AOT attributes needed
}

// Use RxApp only when you need unit test scheduler detection
```

See `docs/RxSchedulers.md` for complete scheduler usage guide.

### Using WhenAnyValue / WhenAny

```csharp
// Combine multiple properties
this.WhenAnyValue(
        x => x.FirstName,
        x => x.LastName,
        (first, last) => $"{first} {last}")
    .Subscribe(fullName => { /* Handle full name */ });

// React to property changes
this.WhenAnyValue(x => x.IsLoading)
    .Where(isLoading => !isLoading)
    .Subscribe(_ => { /* Handle loaded state */ });
```

### ObservableAsPropertyHelper

```csharp
// Computed property that reacts to changes
private readonly ObservableAsPropertyHelper<decimal> _total;
public decimal Total => _total.Value;

// In constructor
_total = this.WhenAnyValue(
        x => x.Quantity,
        x => x.Price,
        (qty, price) => qty * price)
    .ToProperty(this, nameof(Total));
```

## Common Tasks

### Adding a New Feature

1. Create failing tests first
2. Implement minimal functionality
3. Ensure AOT compatibility
4. Update documentation if needed
5. Add XML documentation to public APIs

### Fixing Bugs

1. Create reproduction test
2. Fix with minimal changes
3. Verify AOT compatibility
4. Ensure no regression in existing tests

## What to Avoid

- **Reflection-heavy patterns** without proper AOT suppression
- **Expression trees** in hot paths without caching
- **Platform-specific code** in the core ReactiveUI library
- **Breaking changes** to public APIs without proper versioning

## Important Notes

- **Repository Location:** Working directory is `C:\source\reactiveui\src`
- **Main Solution:** `ReactiveUI.sln`
- **Benchmarks:** Separate solution at `Benchmarks/ReactiveUI.Benchmarks.sln`
- **Integration Tests:** Platform-specific solutions in `integrationtests/` (not required for most development)
- **No shallow clones:** Repository requires full recursive clone for git version information used by build system
- **Required .NET SDKs:** .NET 8.0, 9.0, and 10.0 (all three required)
- **Copilot Instructions:** `.github/copilot-instructions.md` contains comprehensive development guidelines

**Philosophy:** ReactiveUI emphasizes functional reactive programming patterns, immutability where possible, and clean separation of concerns through the MVVM pattern. When in doubt, prefer reactive streams over imperative code, and always consider the AOT implications of your changes.
