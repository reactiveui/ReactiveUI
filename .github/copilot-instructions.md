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
   
   // Method that needs to access properties via reflection
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
   
   // In constructor - when using strongly-typed expressions, no AOT attributes needed
   public ViewModel()
   {
       _computedValue = this.WhenAnyValue(x => x.InputValue)
           .Select(x => $"Computed: {x}")
           .ToProperty(this, nameof(ComputedValue));
   }
   
   // For methods that register view models with dependency injection
#if NET6_0_OR_GREATER
   public void RegisterViewModel<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TViewModel>()
#else
   public void RegisterViewModel<TViewModel>()
#endif
   {
       // Registration logic that uses reflection on TViewModel
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

**Preferred Approach**: Use `DynamicallyAccessedMembersAttribute` to inform the AOT compiler about required members:

```csharp
// For methods that access type constructors
private static object CreateInstance(
#if NET6_0_OR_GREATER
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
#endif
    Type type)
{
    return Activator.CreateInstance(type);
}

// For methods that access properties via reflection
private static PropertyInfo? GetProperty(
#if NET6_0_OR_GREATER
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
#endif
    Type type, string propertyName)
{
    return type.GetProperty(propertyName);
}

// For methods that need all members (like view model registration)
public void RegisterViewModel<
#if NET6_0_OR_GREATER
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
    TViewModel>()
{
    // Registration logic
}
```

**Fallback Approach**: When `DynamicallyAccessedMembersAttribute` isn't sufficient, use suppression attributes:
```csharp
[UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Description of why this is safe")]
[UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Description of why this is safe")]
public void SetupObservation()
{
    // Reflection-based code that can't be made AOT-safe with DynamicallyAccessedMembersAttribute
}
```

**Best Practices**:
- Prefer `DynamicallyAccessedMembersAttribute` over `UnconditionalSuppressMessage` when possible
- Use specific `DynamicallyAccessedMemberTypes` values rather than `All` when you know what's needed
- Prefer strongly-typed expressions over string-based property names when possible
- Use `nameof()` for compile-time property name checking

**ReactiveUI Codebase Examples**:
```csharp
// From ReactiveUIBuilder.cs - View model registration
public IReactiveUIBuilder RegisterSingletonViewModel<
#if NET6_0_OR_GREATER
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
    TViewModel>()

// From ExpressionRewriter.cs - Property access
private static PropertyInfo? GetItemProperty(
#if NET6_0_OR_GREATER
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
#endif
    Type type)

// From DependencyResolverMixins.cs - Constructor access
private static Func<object> TypeFactory(
#if NET6_0_OR_GREATER
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
#endif
    TypeInfo typeInfo)
```

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

## 🎨 Code Style & Formatting

ReactiveUI enforces strict code style and formatting guidelines:

### EditorConfig & StyleCop Compliance
- **EditorConfig**: The repository has a comprehensive `.editorconfig` file with detailed formatting rules
- **StyleCop Analyzers**: Extensive StyleCop analyzer rules are configured with error-level enforcement
- **Code Analysis**: Over 200 analyzer rules are configured for code quality and consistency

### Key Style Requirements
- **Indentation**: 4 spaces (configured in `.editorconfig`)
- **Braces**: Allman style (opening braces on new lines)
- **Field naming**: Private fields use `_camelCase` prefix, static fields use `s_` prefix
- **Var usage**: Prefer `var` when type is apparent
- **Documentation**: All public APIs require XML documentation
- **Null checking**: Use modern C# null-conditional operators

### Official Style Guide
Follow the comprehensive ReactiveUI style guide:
- **Code Style Guide**: https://www.reactiveui.net/contribute/software-style-guide/code-style.html
- **Commit Message Convention**: https://reactiveui.net/contribute/software-style-guide/commit-message-convention

### Build with Style Enforcement
```powershell
# Build with warnings as errors (includes StyleCop violations)
dotnet build src/ReactiveUI.sln -c Release -warnaserror
```

**Important**: Style violations will cause build failures. Use an IDE with EditorConfig support (Visual Studio, VS Code, Rider) to automatically format code according to project standards.

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