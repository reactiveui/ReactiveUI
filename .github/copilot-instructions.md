# Copilot Instructions for ReactiveUI

You are working on the ReactiveUI repository — a composable, cross-platform Model–View–ViewModel framework for all .NET platforms inspired by functional reactive programming.

---

## 🏗️ Build Environment Requirements

### Cloning the Repository

**CRITICAL:** You must perform a **full, recursive clone** of the repository. A shallow clone (`--depth 1`) will fail to build because it won't pull the necessary git version information used by the build system.

```bash
# Full recursive clone
git clone --recursive https://github.com/reactiveui/reactiveui.git
```

### Required Tools

* .NET **8.0**, **9.0**, and **10.0** SDKs

### Installing .NET SDKs

**CRITICAL:** The project will **not** build without the correct .NET SDKs. You **must** install .NET 8, 9, and 10.

Use the official Microsoft **dotnet-install** script by running the following commands in **PowerShell**:

```powershell
# Download the installation script
Invoke-WebRequest -Uri https://dot.net/v1/dotnet-install.ps1 -OutFile dotnet-install.ps1

# Install .NET 8 SDK
./dotnet-install.ps1 -Channel 8.0 -InstallDir ./.dotnet

# Install .NET 9 SDK
./dotnet-install.ps1 -Channel 9.0 -InstallDir ./.dotnet

# Install .NET 10 SDK
./dotnet-install.ps1 -Channel 10.0 -InstallDir ./.dotnet
```

### Solution Files

* Main solution: `src/ReactiveUI.sln` (repository `root/src` directory)
* Integration tests: `integrationtests/` directory contains platform-specific solutions. These are not required for most tasks to compile.

---

## 🛠️ Build & Test Commands

**Critical**: `dotnet workload restore` is required to compile. Do this before any `dotnet build` commands. 

**Run these commands in Windows PowerShell or CMD from the repository root:**

```powershell
# Check .NET installation
dotnet --info

# CRITICAL: Restore platform-specific workloads (MAUI, etc.).
# This MUST be run from the /src directory or the build will fail.
cd src
dotnet workload restore
cd ..

# Restore NuGet packages
dotnet restore src/ReactiveUI.sln

# Build the solution (requires Windows for platform-specific targets)
dotnet build src/ReactiveUI.sln -c Release -warnaserror

# Run tests (includes AOT tests that require .NET 9.0)
dotnet test src/ReactiveUI.sln -c Release --no-build
```

> **Note:** The repository contains Windows-specific target frameworks (`net8.0-windows`, `net9.0-windows`) and AOT tests that require .NET 9.0. Building on Linux/macOS will fail due to these platform dependencies.

> **For non-Windows environments:** If working in this repository on Linux/macOS (such as in GitHub Codespaces), focus on documentation changes, code analysis, and understanding patterns rather than attempting to build. The build failure is expected and normal.

If any step fails due to Windows-specific tooling requirements, create a draft PR describing the minimal change needed.

---

## 🎯 AOT-Friendly Development Patterns

ReactiveUI supports Ahead-of-Time (AOT) compilation. Follow these patterns:

### ✅ Preferred AOT-Friendly Patterns

1. **String-based property observation** (AOT-safe when using `nameof`):

```csharp
// Use ObservableForProperty with nameof — AOT-safe
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

// In constructor — when using strongly-typed expressions, no AOT attributes needed
public ViewModel()
{
    _computedValue = this.WhenAnyValue(x => x.InputValue)
        .Select(x => $"Computed: {x}")
        .ToProperty(this, nameof(ComputedValue));
}

// For methods that register view models with dependency injection
#if NET6_0_OR_GREATER
public void RegisterViewModel<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TViewModel>()
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

**Preferred Approach:** Use `DynamicallyAccessedMembersAttribute` to inform the AOT compiler about required members:

```csharp
// For methods that access type constructors
private static object CreateInstance(
#if NET6_0_OR_GREATER
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
#endif
    Type type)
{
    return Activator.CreateInstance(type)!;
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

**Fallback Approach:** When `DynamicallyAccessedMembersAttribute` isn't sufficient, use suppression attributes:

```csharp
[UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Description of why this is safe")]
[UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Description of why this is safe")]
public void SetupObservation()
{
    // Reflection-based code that can't be made AOT-safe with DynamicallyAccessedMembersAttribute
}
```

**Best Practices:**

* Prefer `DynamicallyAccessedMembersAttribute` over `UnconditionalSuppressMessage` when possible
* Use specific `DynamicallyAccessedMemberTypes` values rather than `All` when you know what's needed
* Prefer strongly-typed expressions over string-based property names when possible
* Use `nameof()` for compile-time property name checking

**ReactiveUI Codebase Examples:**

```csharp
// From ReactiveUIBuilder.cs — View model registration
public IReactiveUIBuilder RegisterSingletonViewModel<
#if NET6_0_OR_GREATER
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
    TViewModel>()

// From ExpressionRewriter.cs — Property access
private static PropertyInfo? GetItemProperty(
#if NET6_0_OR_GREATER
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
#endif
    Type type)

// From DependencyResolverMixins.cs — Constructor access
private static Func<object> TypeFactory(
#if NET6_0_OR_GREATER
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
#endif
    TypeInfo typeInfo)
```

---

## 📚 Reference Materials

Use these as references for patterns and implementation examples:

* **ReactiveUI Documentation:** [https://reactiveui.net/docs/](https://reactiveui.net/docs/)
* **Official Samples Repository:** Various platform examples (WPF, Avalonia, Uno, etc.)
* **Getting Started Guide:** [https://reactiveui.net/docs/getting-started/](https://reactiveui.net/docs/getting-started/)
* **ViewModels Documentation:** [https://reactiveui.net/docs/handbook/view-models/](https://reactiveui.net/docs/handbook/view-models/)

### In-Repository Examples

* `src/ReactiveUI.AOTTests/` — AOT compatibility test examples showing proper attribute usage
* `src/ReactiveUI.Tests/` — Comprehensive test patterns and API usage examples
* `integrationtests/` — Platform-specific integration examples
* `src/ReactiveUI.Builder.Tests/` — Builder pattern examples for dependency injection

### Key Project Structure

* `src/ReactiveUI/` — Core ReactiveUI library
* `src/ReactiveUI.WPF/` — WPF-specific extensions
* `src/ReactiveUI.WinUI/` — WinUI-specific extensions
* `src/ReactiveUI.Maui/` — MAUI-specific extensions
* `src/ReactiveUI.Testing/` — Testing utilities

---

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

### Using `WhenAnyValue` / `WhenAny`

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

### `ObservableAsPropertyHelper`

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

---

## Validation and Quality Assurance

### Code Style and Analysis Enforcement

* **EditorConfig Compliance:** Repository uses comprehensive `.editorconfig` with detailed rules for C# formatting, naming conventions, and code analysis.
* **StyleCop Analyzers:** Enforces consistent C# code style with `stylecop.analyzers`.
* **Roslynator Analyzers:** Additional code quality rules with `Roslynator.Analyzers`.
* **Analysis Level:** Set to latest with enhanced .NET analyzers enabled.
* **CRITICAL:** All code must comply with **ReactiveUI contribution guidelines**: [https://www.reactiveui.net/contribute/index.html](https://www.reactiveui.net/contribute/index.html)

### C# Style Guide

**General Rule:** Follow "Visual Studio defaults" with the following specific requirements.

#### Brace Style

* Use **Allman style** braces, where each brace begins on a new line.
* A single-line statement block can go without braces, but the block must be properly indented on its own line and must not be nested in other statement blocks that use braces.
* **Exception:** A `using` statement is permitted to be nested within another `using` statement by starting on the following line at the same indentation level, even if the nested `using` contains a controlled block.

#### Indentation and Spacing

* Use **four spaces** of indentation (no tabs).
* Avoid more than one empty line at any time (e.g., do not have two blank lines between members of a type).
* Avoid spurious free spaces. For example, avoid `if (someVar == 0) ...`. Enable **View White Space** in your editor to help.
* When using labels (for `goto`), indent the label one less than the current indentation.

#### Field and Property Naming

* Use `_camelCase` for internal and private instance fields and use `readonly` where possible.
* When used on static fields, `readonly` should come after `static` (e.g., `static readonly`, not `readonly static`).
* Public fields should be used sparingly and should use **PascalCasing** with no prefix.
* Use **PascalCasing** for all constant local variables and fields (except for interop code).
* Specify fields at the top within type declarations.

#### Visibility and Modifiers

* Always specify visibility, even if it's the default (e.g., `private string _foo` not `string _foo`).
* Visibility should be the first modifier (e.g., `public abstract` not `abstract public`).

#### Namespace and Using Statements

* Namespace imports should be specified at the top of the file, **outside** of namespace declarations.
* They should be sorted with system namespaces alphabetically, then third-party namespaces alphabetically.
* Use **global using** directives where appropriate to reduce repetition across files.

#### Type Usage and Variables

* Use language keywords instead of BCL types (e.g., `int`, `string` instead of `Int32`, `String`).
* The use of `var` is encouraged if it improves readability (e.g., for long type names) or aids refactoring. Use the full type name if clarity is needed.
* Avoid `this.` unless absolutely necessary.
* Use `nameof(...)` instead of string literals ("...") whenever possible and relevant.

#### Modern C# Features and Patterns

* **Nullable Reference Types:** Enable nullable reference types in projects to reduce null-related errors at compile time.
* **Pattern Matching:** Use C# 7+ pattern matching features (recursive, tuple, positional, type, relational, and list patterns) for expressive conditional logic.
* **Switch Expressions:** Prefer switch expressions over statements for concise, value-based decisions.
* **Records and `init` Setters:** Use records (and record structs) for data-centric types with value semantics. Apply init-only setters for properties that should only be set during initialization to support immutable designs.
* **Expression-bodied Members:** Use for simple properties and methods.
* **Ranges and Indices:** Use ranges (`..`) and indices (`^`) for concise sequence and collection slicing.
* **`using` Declarations:** Employ `using` declarations for automatic resource disposal without nested blocks.
* **Static Local Functions & Lambdas:** Declare `static` local functions and lambdas to avoid capturing unnecessary state.
* **Target-Typed `new`:** Utilize target-typed `new` expressions to infer types from context, reducing verbosity.
* **File-Scoped Namespaces:** Use file-scoped namespace declarations for a flatter file structure.
* **Raw String Literals:** Use raw string literals (starting with `"""`) for multi-line or complex strings without escapes.
* **Required Members:** Mark members with the `required` modifier to enforce initialization by consumers.
* **Primary Constructors:** Use primary constructors in classes and structs to centralize initialization logic.
* **Collection Expressions:** Employ collection expressions (`[ ... ]`) for concise initialization of collections, including the spread (`..`) operator.
* **Lambda Defaults:** Add default parameters to lambda expressions to reduce overloads.
* **Inline `out` Variables:** Use the inline variable feature with `out` parameters.
* **Non-ASCII Characters:** Use Unicode escape sequences (`\uXXXX`) instead of literal non-ASCII characters.

#### Documentation Requirements

* All publicly exposed methods and properties must have .NET XML comments. This includes protected methods of public classes.

#### File Style Precedence

* If a file happens to differ in style from these guidelines, the existing style in that file takes precedence.

### Build with Style Enforcement

```powershell
# Build with warnings as errors (includes StyleCop violations)
dotnet build src/ReactiveUI.sln -c Release -warnaserror
```

**Important:** Style violations will cause build failures. Use an IDE with EditorConfig support (Visual Studio, VS Code, Rider) to automatically format code according to project standards.

---

## 📋 Testing Guidelines

* Always write unit tests for new features or bug fixes
* Use `ReactiveUI.Testing` package for testing reactive code
* Follow existing test patterns in `src/ReactiveUI.Tests/`
* For AOT scenarios, reference patterns in `src/ReactiveUI.AOTTests/`

---

## 🚫 What to Avoid

* **Reflection-heavy patterns** without proper AOT suppression
* **Expression trees** in hot paths without caching
* **Platform-specific code** in the core ReactiveUI library
* **Breaking changes** to public APIs without proper versioning

---

## 🔄 Common Tasks

### Adding a new feature

1. Create failing tests first
2. Implement minimal functionality
3. Ensure AOT compatibility
4. Update documentation if needed
5. Add XML documentation to public APIs

### Fixing bugs

1. Create reproduction test
2. Fix with minimal changes
3. Verify AOT compatibility
4. Ensure no regression in existing tests

---

Remember: ReactiveUI emphasizes functional reactive programming patterns, immutability where possible, and clean separation of concerns through the MVVM pattern. When in doubt, prefer reactive streams over imperative code, and always consider the AOT implications of your changes.
