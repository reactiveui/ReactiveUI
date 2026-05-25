# CLAUDE.md

This file is the single source of truth for AI/agent assistance in this repository (Claude Code, GitHub Copilot, and other coding agents). It consolidates build/test commands, architecture context, coding standards, and AOT guidance.

If there is any conflict between other agent instruction files and this file, follow **CLAUDE.md**.

---

## Repository Orientation

- **Repository root**
- **Primary working directory for build/test:** `./src`
- **Main solution:** `src/reactiveui.slnx`
- **Benchmarks solution:** `Benchmarks/ReactiveUI.Benchmarks.sln`
- **Integration tests:** `integrationtests/` (platform-specific solutions; not required for most tasks)

### Full Clone Required

**CRITICAL:** Use a full, recursive clone. Shallow clones can fail because build/versioning relies on git history. If a clone has already been done you must use the unshallow commit command in git.

```bash
git clone --recursive https://github.com/reactiveui/reactiveui.git
````

---

## Solution Format: SLNX

This repository uses **SLNX** (XML-based solution format) instead of legacy `.sln`.

* Introduced in Visual Studio 2022 17.10+
* Rider 2024.1+ support
* Works with `dotnet build/test` the same way `.sln` does
* Main file: `src/reactiveui.slnx`

---

## Build Environment Requirements

### Required SDKs

* .NET **8.0**, **9.0**, **10.0** SDKs (all required)

### Workload Restore (Required)

**CRITICAL:** Platform workloads must be restored or the build will fail. Run from the `./src` directory.

```powershell
dotnet --info

cd src
dotnet workload restore
cd ..
```

### Restore & Build

**CRITICAL:** Run build/test commands from `./src` unless the command explicitly uses `src/`-prefixed paths.

```powershell
cd src

dotnet restore reactiveui.slnx

dotnet build reactiveui.slnx -c Release
dotnet build reactiveui.slnx -c Release -warnaserror

dotnet clean reactiveui.slnx
```

### Windows Requirements

Building the full solution requires **Windows** due to Windows-only target frameworks (WPF, WinUI, .NET Framework). Non-Windows builds may fail; this is expected. In non-Windows environments, focus on documentation, targeted library changes, or analysis that does not require full compilation.

---

## Running Windows/WPF Tests on Linux via Wine

The Windows-only test projects (`ReactiveUI.Wpf.Tests`, `ReactiveUI.WinForms.Tests`) can be **built and run on Linux** through Wine, giving a fast local feedback loop without a Windows VM or waiting on CI. This works because the Windows targeting/runtime packs restore cross-platform (`EnableWindowsTargeting=true`) and Wine can host the .NET Desktop runtime.

> Treat results as a strong signal, not gospel: this is Wine, not Windows. **CI on `windows-latest` is authoritative.** A green run here is high-confidence; investigate a red one before assuming a product bug.
>
> **Known Wine limitation — dispatcher thread-affinity:** Wine does **not** enforce WPF `Dispatcher` thread affinity the way Windows does, so `DispatcherObject.CheckAccess()` can return `true` on a non-dispatcher thread. Tests that marshal work from a background thread (e.g. `*FromBackgroundThread*`, anything asserting `Dispatcher.BeginInvoke`/scheduler hand-off after `DispatcherUtilities.DoEvents()`) may behave differently under Wine than on Windows — a Wine pass or fail for those is **not** conclusive. Verify background-thread/marshalling tests on CI. Wine is reliable for the large majority of WPF tests that run on a single (dispatcher) thread.

### 1. One-time runtime + Wine prefix setup

Assemble a Windows .NET Desktop runtime (base runtime gives `dotnet.exe` + host/fxr + `Microsoft.NETCore.App`; the desktop pack adds `Microsoft.WindowsDesktop.App` for WPF/WinForms). Match the version to the net8 windows TFM (bump as the SDK moves):

```bash
VER=8.0.27
mkdir -p ~/wine-dotnet8 && cd ~/wine-dotnet8
curl -fsSL -o /tmp/dnr.zip https://builds.dotnet.microsoft.com/dotnet/Runtime/$VER/dotnet-runtime-$VER-win-x64.zip
curl -fsSL -o /tmp/wdr.zip https://builds.dotnet.microsoft.com/dotnet/WindowsDesktop/$VER/windowsdesktop-runtime-$VER-win-x64.zip
unzip -oq /tmp/dnr.zip          # dotnet.exe + host/ + shared/Microsoft.NETCore.App
unzip -oq /tmp/wdr.zip          # + shared/Microsoft.WindowsDesktop.App

export WINEPREFIX=~/.wine-rxui WINEARCH=win64
wineboot -i
wine ~/wine-dotnet8/dotnet.exe --list-runtimes   # must list NETCore.App AND WindowsDesktop.App
```

### 2. Build the Windows-TFM test assembly (on Linux)

The UI test TFMs are gated to Windows in `Directory.Build.props` (`ReactiveUITestingUITargets`), so force a single Windows TFM with a global property. **Clear `obj`/`bin` first** — stale non-Windows assets break the WPF `_wpftmp` markup pass with `NETSDK1005`:

```bash
cd src
rm -rf tests/ReactiveUI.Wpf.Tests/obj tests/ReactiveUI.Wpf.Tests/bin
dotnet build tests/ReactiveUI.Wpf.Tests/ReactiveUI.Wpf.Tests.csproj -c Release \
  -p:ReactiveUITestingUITargets=net8.0-windows10.0.19041.0 -p:CheckEolTargetFramework=false
```

### 3. Run under Wine (MTP + TUnit `--treenode-filter`)

```bash
cd src/tests/ReactiveUI.Wpf.Tests/bin/Release/net8.0-windows10.0.19041.0
export WINEPREFIX=~/.wine-rxui WINEARCH=win64 WINEDEBUG=-all
wine ~/wine-dotnet8/dotnet.exe ReactiveUI.Wpf.Tests.dll \
  --treenode-filter "/*/*/*/ViewModelToViewBindingFromBackgroundThreadDoesNotTouchWpfControlDirectly"
```

`WINEDEBUG=-all` silences `fixme:`/`err:` chatter; pipe through `grep -viE 'fixme|^err:|wine:'` if needed. WinForms tests work identically (`ReactiveUI.WinForms.Tests`, same TFM override).

---

## Testing: Microsoft Testing Platform (MTP) + TUnit

This repo uses **Microsoft Testing Platform (MTP)** with **TUnit**. This differs from VSTest.

* MTP is configured via `global.json`
* Additional test settings in `testconfig.json`
* Test projects enable `TestingPlatformDotnetTestSupport` in `Directory.Build.props`

**Key rule:** TUnit/MTP arguments go **after** `--`.

### Testing Best Practices

* **Do NOT use `--no-build`**. Always build before testing to avoid stale binaries.
* To see test output, use `--output Detailed` **before** `--`.
* Repository configuration runs tests **non-parallel** (`"parallel": false` in `testconfig.json`) to avoid interference.

### Test Commands (run from `./src`)

```powershell
cd src

# Run all tests
dotnet test --solution reactiveui.slnx -c Release

# Run tests for a specific project
dotnet test --project tests/ReactiveUI.Tests/ReactiveUI.Tests.csproj

# Run with code coverage (Microsoft Code Coverage)
dotnet test --solution reactiveui.slnx --coverage --coverage-output-format cobertura

# Detailed output (place BEFORE --)
dotnet test --solution reactiveui.slnx -- --output Detailed
dotnet test --solution reactiveui.slnx --coverage --coverage-output-format cobertura -- --report-trx --output Detailed

# List tests
dotnet test --project tests/ReactiveUI.Tests/ReactiveUI.Tests.csproj -- --list-tests

# Fail fast
dotnet test --solution reactiveui.slnx -- --fail-fast

# Limit parallelism if needed (even though repo defaults non-parallel)
dotnet test --solution reactiveui.slnx -- --maximum-parallel-tests 4
```

### TUnit `--treenode-filter` Syntax

Pattern: `/{AssemblyName}/{Namespace}/{ClassName}/{TestMethodName}`

Examples:

```powershell
# Single test
dotnet test --project tests/ReactiveUI.Tests/ReactiveUI.Tests.csproj -- --treenode-filter "/*/*/*/MyTestMethod"

# All tests in class
dotnet test --project tests/ReactiveUI.Tests/ReactiveUI.Tests.csproj -- --treenode-filter "/*/*/MyClassName/*"

# All tests in namespace
dotnet test --project tests/ReactiveUI.Tests/ReactiveUI.Tests.csproj -- --treenode-filter "/*/MyNamespace/*/*"

# Filter by property (e.g., Category)
dotnet test --solution reactiveui.slnx -- --treenode-filter "/*/*/*/*[Category=Integration]"
```

See: [https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-test?tabs=dotnet-test-with-mtp](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-test?tabs=dotnet-test-with-mtp)
TUnit flags reference: [https://tunit.dev/docs/reference/command-line-flags](https://tunit.dev/docs/reference/command-line-flags)

---

## Key Configuration Files

* `src/global.json` — sets `"Microsoft.Testing.Platform"` runner
* `src/testconfig.json` — test execution settings (parallel false, coverage format, etc.)
* `src/Directory.Build.props` — repository-wide build configuration (incl. `TestingPlatformDotnetTestSupport`)
* `.github/copilot-instructions.md` — may exist, but should defer to this `agent.md`

---

## Architecture Overview

ReactiveUI is a cross-platform MVVM framework built on Rx.NET and functional reactive programming principles.

### Core Library (`src/ReactiveUI/`)

* `ReactiveObject/` — reactive `INotifyPropertyChanged` base
* `ReactiveCommand/` — observable command pipelines
* `Activation/` — view/viewmodel activation lifecycle
* `Bindings/` — one-way/two-way binding infrastructure
* `Expression/` — expression tree analysis for observation (`WhenAnyValue`)
* `Routing/` — navigation/routing
* `Interactions/` — request/response patterns
* `Builder/` — DI and service registration patterns

### Platform Extensions

Examples:

* `ReactiveUI.Wpf/`, `ReactiveUI.WinUI/`, `ReactiveUI.Maui/`, `ReactiveUI.AndroidX/`,
  `ReactiveUI.Blazor/`, `ReactiveUI.Winforms/`, `ReactiveUI.Testing/`, etc.

### Scheduler Abstraction

* Prefer `RxSchedulers` (AOT-friendly, avoids reflection/AOT attribute propagation)
* Use `RxApp` only when required (e.g., unit test scheduler detection)

See `docs/RxSchedulers.md`.

---

## AOT Guidance (Critical)

This repository targets net8.0+ and supports AOT/trimming scenarios.

### Primary Rule: Avoid Reflection Paths

Prefer strongly-typed and source-generator-friendly approaches. Avoid reflection-heavy patterns that require trimming/AOT attributes.

### Attributes: Use Only If Necessary

* Avoid introducing DAC/RDC/RUC attributes unless required.
* If an attribute is required, apply it directly (no `#if NET6_0_OR_GREATER` guards). Polyfills are available.

Example (only when truly needed):

```csharp
private static object CreateInstance(
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    Type type)
{
    return Activator.CreateInstance(type)!;
}
```

### Suppressions: Last Resort Only — Never Without Human Approval

**Do NOT add any suppression without explicit human approval.** This covers `[SuppressMessage]`, `[UnconditionalSuppressMessage]`, `#pragma warning`, and `.editorconfig` severity changes. A suppression is a true last resort, used only when a warning genuinely cannot be resolved by fixing the code without harming the design.

Before suppressing anything:

1. **Fix the underlying issue first.** Most analyzer warnings indicate a real fix. For example, `S3398` ("method should be moved") means *move the method into the type that uses it* — never suppress it. `SA****` (StyleCop) must always be fixed, never suppressed.
2. **If you believe a suppression is genuinely unavoidable, stop and ask the human.** Present the specific analyzer ID, why it cannot be fixed in code, and the proposed justification. Wait for explicit approval.
3. **Only after approval**, apply it with minimal scope, the specific ID, and a clear `Justification`.

---

## Code Style & Quality Requirements

**CRITICAL:** Follow ReactiveUI contribution guidelines:
[https://www.reactiveui.net/contribute/index.html](https://www.reactiveui.net/contribute/index.html)

### Enforced Tooling

* `.editorconfig` formatting/naming conventions
* StyleCop analyzers (build fails on violations)
* Roslynator analyzers
* Analysis level: latest
* Warnings treated as errors (notably nullable and CS4014)
* **Public APIs require XML documentation**, including protected methods on public types.

### C# Style Rules (High-level)

* Allman braces
* 4 spaces, no tabs
* Explicit visibility
* Private/internal fields: `_camelCase`, `readonly` where possible, `static readonly` order
* File-scoped namespaces preferred; using directives outside namespace and sorted
* Use C# keywords (`int`, `string`) rather than BCL types
* Prefer modern C# features where appropriate (nullable, pattern matching, switch expressions, records, init, target-typed new, etc.)
* Use `nameof()` over string literals
* Avoid `this.` unless necessary
* Use `var` when it improves readability

If a specific file already follows a local style, adhere to existing file conventions.

---

## Zero Pragma Policy (Critical)

**No `#pragma warning disable`** in production code.

* **StyleCop warnings (SA****) must be fixed**, never suppressed.
* **No analyzer warning (CA****, S**** Sonar, RCS**** Roslynator, IL**** trimming/AOT, etc.) may be suppressed without explicit human approval** — see "Suppressions: Last Resort Only" above. Fix the code first; if a suppression seems unavoidable, stop and ask.

Example:

```csharp
// WRONG
#pragma warning disable CA1062
public void MyMethod(object parameter)
{
    parameter.ToString();
}
#pragma warning restore CA1062

// CORRECT
public void MyMethod(object parameter)
{
    ArgumentNullException.ThrowIfNull(parameter);
    parameter.ToString();
}

// LAST RESORT ONLY
[SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
    Justification = "TUnit guarantees non-null parameters from data sources.")]
public async Task MyTest(IConverter converter, int expectedValue)
{
    var result = converter.GetValue();
    await Assert.That(result).IsEqualTo(expectedValue);
}
```

---

## Testing Guidelines

* Use TUnit + Microsoft Testing Platform
* Write unit tests for new features and bug fixes
* Prefer existing patterns in:

  * `src/tests/ReactiveUI.Tests/`
  * `src/tests/ReactiveUI.AOTTests/`
* Use `ReactiveUI.Testing` utilities for reactive code

---

## Common Development Patterns

### ViewModel Skeleton

```csharp
public class SampleViewModel : ReactiveObject
{
    private string? _name;
    private readonly ObservableAsPropertyHelper<bool> _isValid;

    public SampleViewModel()
    {
        _isValid = this.WhenAnyValue(x => x.Name)
            .Select(name => !string.IsNullOrWhiteSpace(name))
            .ToProperty(this, nameof(IsValid));

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
        // Implementation
    }
}
```

### RxSchedulers (Preferred)

```csharp
public IObservable<string> GetData()
{
    return Observable.Return("data")
        .ObserveOn(RxSchedulers.MainThreadScheduler);
}
```

### WhenAnyValue

```csharp
this.WhenAnyValue(
        x => x.FirstName,
        x => x.LastName,
        (first, last) => $"{first} {last}")
    .Subscribe(fullName => { /* handle */ });

this.WhenAnyValue(x => x.IsLoading)
    .Where(isLoading => !isLoading)
    .Subscribe(_ => { /* handle */ });
```

### ObservableAsPropertyHelper

```csharp
private readonly ObservableAsPropertyHelper<decimal> _total;
public decimal Total => _total.Value;

_total = this.WhenAnyValue(
        x => x.Quantity,
        x => x.Price,
        (qty, price) => qty * price)
    .ToProperty(this, nameof(Total));
```

---

## What to Avoid

* Reflection-heavy implementations in core paths
* Expression trees in hot paths without caching
* Platform-specific code in `src/ReactiveUI/` core library
* Breaking public APIs without proper versioning and documentation
