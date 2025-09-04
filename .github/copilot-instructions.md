# Copilot Instructions for ReactiveUI

You are working on the ReactiveUI repository - a composable, cross-platform model-view-viewmodel framework for all .NET platforms inspired by functional reactive programming.

## 🏗️ Build Environment Requirements

**IMPORTANT**: This repository requires Windows for building. Linux/macOS are not supported.

### Required Tools
- Windows 10/11 with Visual Studio 2019+ 
- PowerShell or Command Prompt
- .NET 9.0 SDK (required for AOT tests) or .NET 8.0 SDK minimum
- Windows-specific target frameworks: `net8.0-windows10.0.17763.0`, `net9.0-windows10.0.17763.0`

### Solution Files
- Main solution: `src/ReactiveUI.sln` (repository root/src directory)
- Integration tests: `integrationtests/` directory contains platform-specific solutions

## 🛠️ Build & Test Commands

**Run these commands in Windows PowerShell or CMD from the repository root:**

```powershell
# Check .NET installation (requires .NET 9.0 SDK for full build including AOT tests)
dotnet --info

# Restore packages (may fail on non-Windows due to Windows-specific target frameworks)
dotnet restore src/ReactiveUI.sln

# Build the solution (requires Windows for platform-specific targets)
dotnet build src/ReactiveUI.sln -c Release -warnaserror

# Run tests (includes AOT tests that require .NET 9.0)
dotnet test src/ReactiveUI.sln -c Release --no-build
```

**Note**: The repository contains Windows-specific target frameworks (`net8.0-windows`, `net9.0-windows`) and AOT tests that require .NET 9.0. Building on Linux/macOS will fail due to these platform dependencies.

**For non-Windows environments**: If working in this repository on Linux/macOS (such as in GitHub Codespaces), focus on documentation changes, code analysis, and understanding patterns rather than attempting to build. The build failure is expected and normal.

If any step fails due to Windows-specific tooling requirements, create a draft PR describing the minimal change needed.

## 🎯 AOT-Friendly Development Patterns

ReactiveUI supports Ahead-of-Time (AOT) compilation. Follow these patterns:

### ✅ Preferred AOT-Friendly Patterns

1. **String-based property observation** (AOT-safe when using nameof):
   ```csharp
   // Use ObservableForProperty with nameof - AOT-safe
   obj.ObservableForProperty<TestReactiveObject, string?>(
       nameof(TestReactiveObject.TestProperty), 
       beforeChange: false, 
       skipInitial: false)
       .Select(x => x.Value)
       .Subscribe(HandlePropertyChange);
   
   // String-based WhenAnyValue with proper AOT suppression
   [UnconditionalSuppressMessage("AOT", "IL3050")]
   [UnconditionalSuppressMessage("Trimming", "IL2026")]
   public void SetupObservation()
   {
       this.WhenAnyValue(nameof(PropertyName))
           .Subscribe(HandleChange);
   }
   ```

2. **ReactiveCommand creation**:
   ```csharp
   // Preferred: Async command with explicit typing
   public ReactiveCommand<Unit, Unit> SubmitCommand { get; }
   
   public ViewModel()
   {
       SubmitCommand = ReactiveCommand.CreateFromTask(ExecuteSubmit);
   }
   
   private async Task ExecuteSubmit(CancellationToken cancellationToken)
   {
       // Implementation
   }
   ```

3. **Observable property helpers**:
   ```csharp
   private readonly ObservableAsPropertyHelper<string> _computedValue;
   
   public string ComputedValue => _computedValue.Value;
   
   // In constructor with proper AOT attributes
   [UnconditionalSuppressMessage("AOT", "IL3050")]
   [UnconditionalSuppressMessage("Trimming", "IL2026")]
   public ViewModel()
   {
       _computedValue = this.WhenAnyValue(x => x.InputValue)
           .Select(x => $"Computed: {x}")
           .ToProperty(this, nameof(ComputedValue));
   }
   ```

4. **Property change notification**:
   ```csharp
   private string? _myProperty;
   
   public string? MyProperty
   {
       get => _myProperty;
       set => this.RaiseAndSetIfChanged(ref _myProperty, value);
   }
   ```

### ⚠️ AOT Considerations

- When using reflection-based APIs (WhenAnyValue, ToProperty, etc.), add appropriate AOT suppression attributes:
  ```csharp
  [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Description of why this is safe")]
  [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Description of why this is safe")]
  ```

- Prefer strongly-typed expressions over string-based property names when possible
- Use `nameof()` for compile-time property name checking

## 📚 Reference Materials

### Official Samples & Documentation
Use these as references for patterns and implementation examples:

- **ReactiveUI Documentation**: https://reactiveui.net/docs/
- **Official Samples Repository**: Various platform examples (WPF, Avalonia, Uno, etc.)
- **Getting Started Guide**: https://reactiveui.net/docs/getting-started/
- **ViewModels Documentation**: https://reactiveui.net/docs/handbook/view-models/

### In-Repository Examples
- `src/ReactiveUI.AOTTests/` - AOT compatibility test examples showing proper attribute usage
- `src/ReactiveUI.Tests/` - Comprehensive test patterns and API usage examples
- `integrationtests/` - Platform-specific integration examples
- `src/ReactiveUI.Builder.Tests/` - Builder pattern examples for dependency injection

### Key Project Structure
- `src/ReactiveUI/` - Core ReactiveUI library
- `src/ReactiveUI.WPF/` - WPF-specific extensions
- `src/ReactiveUI.WinUI/` - WinUI-specific extensions  
- `src/ReactiveUI.Maui/` - MAUI-specific extensions
- `src/ReactiveUI.Testing/` - Testing utilities

## 🎨 Good Development Suggestions

### Creating ViewModels
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

// String-based observation (AOT-friendly with nameof)
obj.ObservableForProperty<MyClass, string>(
    nameof(MyClass.PropertyName), 
    beforeChange: false, 
    skipInitial: false)
    .Select(x => x.Value)
    .Subscribe(HandleChange);
```

### ReactiveUI Builder Pattern
```csharp
// Platform-specific builder configuration
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        
        builder
            .UseMauiApp<App>()
            .UseReactiveUI() // Extension for MAUI
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        return builder.Build();
    }
}

// WinUI builder configuration
public void ConfigureServices(IServiceCollection services)
{
    services.AddReactiveUI(builder =>
    {
        builder.WithWinUI(); // Platform-specific setup
    });
}
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

## 📋 Testing Guidelines

- Always write unit tests for new features or bug fixes
- Use `ReactiveUI.Testing` package for testing reactive code
- Follow existing test patterns in `src/ReactiveUI.Tests/`
- For AOT scenarios, reference patterns in `src/ReactiveUI.AOTTests/`

## 🚫 What to Avoid

- **Reflection-heavy patterns** without proper AOT suppression
- **Expression trees** in hot paths without caching
- **Platform-specific code** in the core ReactiveUI library
- **Breaking changes** to public APIs without proper versioning

## 🔄 Common Tasks

### Adding a new feature:
1. Create failing tests first
2. Implement minimal functionality
3. Ensure AOT compatibility
4. Update documentation if needed
5. Add XML documentation to public APIs

### Fixing bugs:
1. Create reproduction test
2. Fix with minimal changes
3. Verify AOT compatibility
4. Ensure no regression in existing tests

Remember: ReactiveUI emphasizes functional reactive programming patterns, immutability where possible, and clean separation of concerns through the MVVM pattern. When in doubt, prefer reactive streams over imperative code, and always consider the AOT implications of your changes.