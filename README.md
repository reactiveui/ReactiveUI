[![Build](https://github.com/reactiveui/ReactiveUI/actions/workflows/ci-build.yml/badge.svg)](https://github.com/reactiveui/ReactiveUI/actions/workflows/ci-build.yml)
[![Code Coverage](https://codecov.io/gh/reactiveui/ReactiveUI/branch/main/graph/badge.svg)](https://codecov.io/gh/reactiveui/ReactiveUI)
[![#yourfirstpr](https://img.shields.io/badge/first--timers--only-friendly-blue.svg)](https://reactiveui.net/contribute)
[![](https://img.shields.io/badge/chat-slack-blue.svg)](https://reactiveui.net/slack)

<br>
<a href="https://github.com/reactiveui/reactiveui">
  <img width="160" heigth="160" src="https://raw.githubusercontent.com/reactiveui/styleguide/master/logo/main.png">
</a>
<br>

# What is ReactiveUI?

[ReactiveUI](https://reactiveui.net/) is a composable, cross-platform model-view-viewmodel framework for all .NET
platforms that is inspired by functional reactive programming, which is a paradigm that allows you
to [abstract mutable state away from your user interfaces and express the idea around a feature in one readable place](https://www.youtube.com/watch?v=3HwEytvngXk)
and improve the testability of your application.

[🔨 Get Started](https://reactiveui.net/documentation/getting-started/) [🛍 Install Packages](https://reactiveui.net/documentation/getting-started/installation/) [🎞 Watch Videos](https://reactiveui.net/documentation/resources/videos) [🎓 View Samples](https://reactiveui.net/documentation/resources/samples/) [🎤 Discuss ReactiveUI](https://reactiveui.net/slack)

## Your first view model

A view shows data that changes, so it needs to know when a value changes. Written by hand, each property needs a
field and a setter that raises a change notification. Each command needs a property that wraps a method. ReactiveUI
writes that code for you while your project builds.

A **source generator** is a compiler add-on that writes C# code during the build. The ReactiveUI packages bring two
of them.
[ReactiveUI.SourceGenerators](https://www.reactiveui.net/documentation/source-generators/) writes reactive properties
and commands. [ReactiveUI.Binding](https://www.reactiveui.net/documentation/binding/) writes the code behind
`WhenAnyValue`, `ToProperty` and the view bindings. You install neither yourself.

**1. Install the ReactiveUI package for your UI framework.** A class library that holds only view models installs
`ReactiveUI`.

```bash
dotnet add package ReactiveUI.WPF
```

**2. Declare the view model as a `partial` class and mark its members.**

```csharp
using ReactiveUI;
using ReactiveUI.SourceGenerators;

public partial class LoginViewModel : ReactiveObject
{
    private readonly IObservable<bool> _canLogIn;

    public LoginViewModel()
    {
        _canLogIn = this.WhenAnyValue(
            static x => x.UserName,
            static x => x.Password,
            static (userName, password) => userName.Length > 0 && password.Length > 0);

        _isValidHelper = _canLogIn.ToProperty(this, static x => x.IsValid);
    }

    [Reactive]
    public partial string UserName { get; set; } = string.Empty;

    [Reactive]
    public partial string Password { get; set; } = string.Empty;

    [ObservableAsProperty]
    public partial bool IsValid { get; }

    [ReactiveCommand(CanExecute = nameof(_canLogIn))]
    private async Task<bool> LogInAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        return Password == "secret";
    }
}
```

`WhenAnyValue` returns a **stream**, an `IObservable<T>` that emits a new value each time either property changes.

**3. Build.** The generators write the rest of the class:

- The bodies of `UserName` and `Password`. Setting either one raises the change notification.
- The body of `IsValid` and its `_isValidHelper` field. `ToProperty` keeps `IsValid` equal to the latest value of
  `_canLogIn`.
- A `LogInCommand` property. It holds a **command**, an `ICommand` that also reports each result as a stream. The
  command runs `LogInAsync`, and it is enabled only while `_canLogIn` emits `true`. The generator drops the `Async`
  suffix from the name.

Next, bind the properties and the command to your view with `Bind` and `BindCommand`. The
[ReactiveUI.Binding documentation](https://www.reactiveui.net/documentation/binding/) shows how. The
[source generators documentation](https://www.reactiveui.net/documentation/source-generators/) lists every attribute
and option.

Partial properties with an initial value need C# 14, the default language version for .NET 10.

## How the packages fit together

ReactiveUI is a set of packages. You install the package for your UI framework, and it brings the rest.

| Package | What it gives you |
|---|---|
| `ReactiveUI` | `ReactiveObject`, `ReactiveCommand`, activation, routing and schedulers, built on ReactiveUI.Primitives. It brings `ReactiveUI.Core` and `ReactiveUI.Binding`. |
| `ReactiveUI.Reactive` | The same API built for System.Reactive. It brings `ReactiveUI.Core` and `ReactiveUI.Binding.Reactive`. |
| `ReactiveUI.Core` | The parts both flavours share. It brings `ReactiveUI.SourceGenerators`. |
| `ReactiveUI.Binding` | `WhenAnyValue`, `Bind`, `OneWayBind`, `BindCommand`, `ToProperty`, `[ObservableAsProperty]` and view location. |
| `ReactiveUI.SourceGenerators` | `[Reactive]`, `[ReactiveCommand]`, `[ReactiveCollection]`, `[BindableDerivedList]` and `[IReactiveObject]`. |

The [ReactiveUI documentation](https://www.reactiveui.net/documentation/reactiveui/) covers the core package, and the
[installation guide](https://www.reactiveui.net/documentation/reactiveui/getting-started/installation/) shows which
package to install for each platform.

### Pick a flavour

ReactiveUI comes in two flavours with the same API. They differ only in the reactive types that appear in that API.

| You want | Install | Reactive types in the API |
|---|---|---|
| No System.Reactive dependency | `ReactiveUI`, `ReactiveUI.WPF`, `ReactiveUI.WinForms`, `ReactiveUI.WinUI`, `ReactiveUI.Maui`, `ReactiveUI.Blazor`, `ReactiveUI.AndroidX` | ReactiveUI.Primitives: `RxVoid`, `ISequencer`, `Signal<T>` |
| To mix with code that uses System.Reactive | `ReactiveUI.Reactive`, `ReactiveUI.WPF.Reactive`, `ReactiveUI.WinForms.Reactive`, `ReactiveUI.WinUI.Reactive`, `ReactiveUI.Maui.Reactive`, `ReactiveUI.Blazor.Reactive`, `ReactiveUI.AndroidX.Reactive` | System.Reactive: `Unit`, `IScheduler`, `Subject<T>` |

[ReactiveUI.Primitives](https://www.reactiveui.net/documentation/primitives/) is ReactiveUI's own library of streams,
operators and schedulers. Both flavours run on it. The default flavour does not depend on System.Reactive, so your app
ships fewer assemblies.

The System.Reactive flavour puts its own types in the `ReactiveUI.Reactive` namespace. A file that uses
`ReactiveObject` or `ReactiveCommand` from it adds `using ReactiveUI.Reactive;`.

### Bindings come from ReactiveUI.Binding

ReactiveUI runs `WhenAnyValue`, `Bind`, `OneWayBind`, `BindCommand`, `ToProperty` and view location on
[ReactiveUI.Binding](https://www.reactiveui.net/documentation/binding/). Its source generator writes the code for each
binding while your project builds. Nothing looks up a property by name at run time, so bindings are safe to trim and
to publish with Native AOT.

The ReactiveUI package imports the `ReactiveUI.Binding` namespace into your project, so binding calls need no `using`.
Moving from an earlier version? The
[migration guide](https://www.reactiveui.net/documentation/reactiveui/upgrading/reactiveui-binding-migration/) covers
the calls that need an `Unsafe` twin and the behavior that changed.

### Source generators come with ReactiveUI.Core

`ReactiveUI.Core` references
[ReactiveUI.SourceGenerators](https://www.reactiveui.net/documentation/source-generators/) and passes its generators
on. Every project that installs a ReactiveUI package can use `[Reactive]`, `[ReactiveCommand]` and the other
attributes. Add `using ReactiveUI.SourceGenerators;` to each file that uses them. The generators check which flavour
your project references and write code for that flavour.

- **Remove your own reference to ReactiveUI.SourceGenerators**, or set it to the version ReactiveUI brings or later.
  An older version fails to restore with error NU1605.

### Analyzers

An **analyzer** checks your code as you type and reports problems as warnings or errors. ReactiveUI.Binding and
ReactiveUI.SourceGenerators run their analyzers in your project. They report binding calls and attributes that the
generators cannot handle. Diagnostic RXUIBIND021 flags a binding call with no generated binding behind it, such as
one that targets a member another source generator adds or a stored lambda; that call throws at run time, so treat
the warning as a bug to fix rather than suppress.

The analyzers inside ReactiveUI.Primitives stay out of your project. ReactiveUI references ReactiveUI.Primitives with
`ExcludeAssets="analyzers"`. To use them, reference ReactiveUI.Primitives directly:

```xml
<PackageReference Include="ReactiveUI.Primitives" Version="x.y.z" />
```

### Routing

Routing (`RoutingState` and `IScreen`) is part of the ReactiveUI package. The WPF, WinUI and MAUI packages add a
`RoutedViewHost` control that shows the current view, and Windows Forms adds `RoutedControlHost`. `RoutingState` reports
navigation as streams. `CurrentViewModel` emits the view model on top of the stack. `NavigationStackChanged` emits a
read-only copy of the whole stack after each change. `CanNavigateBack` emits whether there is a view model to go back
to. The [routing migration guide](https://www.reactiveui.net/documentation/reactiveui/upgrading/routing-migration/)
covers moving from the `ReactiveUI.Routing` package.

### Platform packages

Install the package for your UI framework. Each one brings `ReactiveUI`. Each one except `ReactiveUI.Uno` has a
`.Reactive` twin for the System.Reactive flavour.

| Platform | Package | NuGet |
|---|---|---|
| Class libraries | [ReactiveUI][CoreDoc] | [![CoreBadge]][Core] |
| WPF | [ReactiveUI.WPF][WpfDoc] | [![WpfBadge]][Wpf] |
| WinUI | [ReactiveUI.WinUI][WinUiDoc] | [![WinUiBadge]][WinUi] |
| MAUI | [ReactiveUI.Maui][MauiDoc] | [![MauiBadge]][Maui] |
| Windows Forms | [ReactiveUI.WinForms][WinDoc] | [![WinBadge]][Win] |
| Android (AndroidX) | [ReactiveUI.AndroidX][DroDoc] | [![DroXBadge]][DroX] |
| Blazor | [ReactiveUI.Blazor][BlazDoc] | [![BlazBadge]][Blaz] |
| Avalonia | [ReactiveUI.Avalonia][AvaDoc] | [![AvaBadge]][Ava] |
| Uno Platform | [ReactiveUI.Uno][UnoDoc] | [![UnoBadge]][Uno] |
| Unit tests | [ReactiveUI.Testing][TestDoc] | [![TestBadge]][Test] |

`ReactiveUI.Avalonia` and `ReactiveUI.Uno` live in their own repositories.

[ReactiveUI.Validation][ValDocs] adds validation rules for view models. It lives in its own repository.
[![ValBadge]][ValCore]

## Documentation

- [RxSchedulers](docs/RxSchedulers.md) - Using ReactiveUI schedulers without RequiresUnreferencedCode attributes

## Book

There has been an excellent [book](https://kent-boogaart.com/you-i-and-reactiveui/) written by our Alumni maintainer
Kent Boogart.

[Core]: https://www.nuget.org/packages/ReactiveUI/
[CoreBadge]: https://img.shields.io/nuget/v/ReactiveUI.svg
[CoreDoc]: https://www.reactiveui.net/documentation/reactiveui/getting-started/installation/

[Wpf]: https://www.nuget.org/packages/ReactiveUI.WPF/
[WpfBadge]: https://img.shields.io/nuget/v/ReactiveUI.WPF.svg
[WpfDoc]: https://www.reactiveui.net/documentation/reactiveui/getting-started/installation/windows-presentation-foundation/

[WinUi]: https://www.nuget.org/packages/ReactiveUI.WinUI/
[WinUiBadge]: https://img.shields.io/nuget/v/ReactiveUI.WinUI.svg
[WinUiDoc]: https://www.reactiveui.net/documentation/reactiveui/getting-started/installation/winui/

[Maui]: https://www.nuget.org/packages/ReactiveUI.Maui/
[MauiBadge]: https://img.shields.io/nuget/v/ReactiveUI.Maui.svg
[MauiDoc]: https://www.reactiveui.net/documentation/reactiveui/getting-started/installation/maui/

[Win]: https://www.nuget.org/packages/ReactiveUI.WinForms/
[WinBadge]: https://img.shields.io/nuget/v/ReactiveUI.WinForms.svg
[WinDoc]: https://www.reactiveui.net/documentation/reactiveui/getting-started/installation/windows-forms/

[DroX]: https://www.nuget.org/packages/ReactiveUI.AndroidX/
[DroXBadge]: https://img.shields.io/nuget/v/ReactiveUI.AndroidX.svg
[DroDoc]: https://www.reactiveui.net/documentation/reactiveui/getting-started/installation/androidx/

[Blaz]: https://www.nuget.org/packages/ReactiveUI.Blazor/
[BlazBadge]: https://img.shields.io/nuget/v/ReactiveUI.Blazor.svg
[BlazDoc]: https://www.reactiveui.net/documentation/reactiveui/getting-started/installation/blazor/

[Ava]: https://www.nuget.org/packages/ReactiveUI.Avalonia/
[AvaBadge]: https://img.shields.io/nuget/v/ReactiveUI.Avalonia.svg
[AvaDoc]: https://www.reactiveui.net/documentation/reactiveui/getting-started/installation/avalonia/

[Uno]: https://www.nuget.org/packages/ReactiveUI.Uno/
[UnoBadge]: https://img.shields.io/nuget/v/ReactiveUI.Uno.svg
[UnoDoc]: https://github.com/reactiveui/ReactiveUI.Uno

[Test]: https://www.nuget.org/packages/ReactiveUI.Testing/
[TestBadge]: https://img.shields.io/nuget/v/ReactiveUI.Testing.svg
[TestDoc]: https://www.reactiveui.net/documentation/reactiveui/handbook/testing/

[ValCore]: https://www.nuget.org/packages/ReactiveUI.Validation/
[ValBadge]: https://img.shields.io/nuget/v/ReactiveUI.Validation.svg
[ValDocs]: https://www.reactiveui.net/documentation/validation/

## Sponsors

[JetBrains](https://www.jetbrains.com/) gives ReactiveUI's maintainers licences for its tools through its
[open source support programme](https://www.jetbrains.com/community/opensource/).
[Anthropic](https://www.anthropic.com/) supports them with [Claude](https://claude.com/) through
[Claude for Open Source](https://claude.com/contact-sales/claude-for-oss).
[OpenAI](https://openai.com/) supports them with [Codex](https://openai.com/codex/) through
[Codex for Open Source](https://developers.openai.com/community/codex-for-oss).

[![JetBrains](https://raw.githubusercontent.com/reactiveui/website/main/docs/images/sponsors/jetbrains.svg)](https://www.jetbrains.com/)
[![Claude by Anthropic](https://raw.githubusercontent.com/reactiveui/website/main/docs/images/sponsors/claude.svg)](https://claude.com/)
[![OpenAI](https://raw.githubusercontent.com/reactiveui/website/main/docs/images/sponsors/openai.svg)](https://openai.com/codex/)

See [our sponsors](https://www.reactiveui.net/sponsors/) for more information.
JetBrains, Claude, Anthropic, OpenAI and Codex names and logos are trademarks of their respective owners.

## Sponsorship

The core team members, ReactiveUI contributors and contributors in the ecosystem do this open-source work in their free
time. If you use ReactiveUI, a serious task, and you'd like us to invest more time on it, please donate. This project
increases your income/productivity too. It makes development and applications faster and it reduces the required
bandwidth.

[Become a sponsor](https://github.com/sponsors/reactivemarbles).

## Migration from Xamarin and .NET 8 MAUI

### Xamarin Users

As of May 2024, Microsoft ended support for Xamarin per
their [support policy](https://docs.microsoft.com/dotnet/maui/what-is-maui#xamarin-retirement). ReactiveUI has removed
support for legacy Xamarin platforms in favor of modern .NET MAUI. For Xamarin projects:

- **Xamarin.Forms** → Migrate to **MAUI** and use `ReactiveUI.Maui`
- **Xamarin.Android** → Migrate to **MAUI Android** or use `ReactiveUI.AndroidX` for native Android
- **Xamarin.iOS/Mac** → Migrate to **MAUI iOS/Mac Catalyst**

For guidance on migrating from Xamarin to MAUI, see
the [official migration documentation](https://docs.microsoft.com/dotnet/maui/migration/).

### MAUI Users

ReactiveUI supports .NET 9 and .NET 10 for MAUI platforms:

- `net10.0-android` / `net9.0-android`
- `net10.0-ios` / `net9.0-ios`
- `net10.0-maccatalyst` / `net9.0-maccatalyst`
- `net10.0-windows10.0.19041.0` / `net9.0-windows10.0.19041.0`

Non-MAUI `net8.0` library targets remain fully supported.

## Examples

Platform-specific sample applications are included in [`src/examples/`](src/examples/):

| Sample                                                                          | Platform      | Description                                                        |
|---------------------------------------------------------------------------------|---------------|--------------------------------------------------------------------|
| [ReactiveUI.Samples.Wpf](src/examples/ReactiveUI.Samples.Wpf)                   | WPF           | Login form with reactive bindings, PasswordBox event marshaling    |
| [ReactiveUI.Samples.Winforms](src/examples/ReactiveUI.Samples.Winforms)         | WinForms      | Login form with IViewFor, programmatic UI layout                   |
| [ReactiveUI.Samples.Maui](src/examples/ReactiveUI.Samples.Maui)                 | MAUI          | Cross-platform login with Shell navigation, ReactiveContentPage    |
| [ReactiveUI.Builder.WpfApp](src/examples/ReactiveUI.Builder.WpfApp)             | WPF           | Multi-instance chat app with routing, suspension, and network sync |
| [ReactiveUI.Builder.BlazorServer](src/examples/ReactiveUI.Builder.BlazorServer) | Blazor Server | Chat app with server-side Blazor and reactive components           |

All samples target .NET 10, use `RxAppBuilder` for initialization, and demonstrate `WhenActivated`, `Bind`/
`BindCommand`, and proper subscription disposal.

This is how we use the donations:

* Allow the core team to work on ReactiveUI
* Thank contributors if they invested a large amount of time in contributing
* Support projects in the ecosystem

## Support

If you have a question, please see if any discussions in
our [GitHub issues](https://github.com/reactiveui/ReactiveUI/issues)
or [Stack Overflow](https://stackoverflow.com/questions/tagged/reactiveui) have already answered it.

If you want to discuss something or just need help, here is our [Slack room](https://reactiveui.net/slack), where there
are always individuals looking to help out!

Please do not open GitHub issues for support requests.

## Contribute

ReactiveUI is developed under an OSI-approved open source license, making it freely usable and distributable, even for
commercial use.

If you want to submit pull requests please first open
a [GitHub issue](https://github.com/reactiveui/ReactiveUI/issues/new/choose) to discuss. We are first time PR
contributors friendly.

See [Contribution Guidelines](https://www.reactiveui.net/contribute/) for further information how to contribute changes.

## Core Team

<table>
  <tbody>
    <tr>
      <td align="center" valign="top" width="105">
        <img width="100" height="100" src="https://github.com/glennawatson.png?s=150">
        <br>
        <a href="https://github.com/glennawatson">Glenn Watson</a>
        <p>Melbourne, Australia</p>
      </td>
      <td align="center" valign="top" width="105">
        <img width="100" height="100" src="https://github.com/chrispulman.png?s=150">
        <br>
        <a href="https://github.com/chrispulman">Chris Pulman</a>
        <p>United Kingdom</p>
      </td>
    </tr>
  </tbody>
</table>

## Alumni Core Team

The following have been core team members in the past.

<table>
  <tbody>
    <tr>
      <td align="center" valign="top" width="105">
        <img width="100" height="100" src="https://github.com/ghuntley.png?s=150">
        <br>
        <a href="https://github.com/ghuntley">Geoffrey Huntley</a>
        <p>Sydney, Australia</p>
      </td>
      <td align="center" valign="top" width="105">
        <img width="100" height="100" src="https://github.com/kentcb.png?s=150">
        <br>
        <a href="https://github.com/kentcb">Kent Boogaart</a>
        <p>Brisbane, Australia</p>
      </td>
      <td align="center" valign="top" width="105">
        <img width="100" height="100" src="https://github.com/olevett.png?s=150">
        <br>
        <a href="https://github.com/olevett">Olly Levett</a>
        <p>London, United Kingdom</p>
      </td>
    </tr>
    <tr>
      <td align="center" valign="top" width="105">
        <img width="100" height="100" src="https://github.com/anaisbetts.png?s=150">
        <br>
        <a href="https://github.com/anaisbetts">Anaïs Betts</a>
        <p>San Francisco, USA</p>
      </td>
      <td align="center" valign="top" width="105">
        <img width="100" height="100" src="https://github.com/shiftkey.png?s=150">
        <br>
        <a href="https://github.com/shiftkey">Brendan Forster</a>
        <p>Melbourne, Australia</p>
      </td>
      <td align="center" valign="top" width="105">
        <img width="100" height="100" src="https://github.com/clairernovotny.png?s=150">
        <br>
        <a href="https://github.com/clairernovotny">Claire Novotny</a>
        <p>New York, USA</p>
      </td>
    </tr>
    <tr>
      <td align="center" valign="top" width="105">
        <img width="100" height="100" src="https://github.com/worldbeater.png?s=150">
        <br>
        <a href="https://github.com/worldbeater">Artyom Gorchakov</a>
        <p>Moscow, Russia</p>
      </td>
      <td align="center" valign="top" width="105">
        <img width="100" height="100" src="https://github.com/rlittlesii.png?s=150">
        <br>
        <a href="https://github.com/rlittlesii">Rodney Littles II</a>
        <p>Texas, USA</p>
      </td>
      <td align="center" valign="top" width="105">
        <img width="100" height="100" src="https://github.com/cabauman.png?s=150">
        <br>
        <a href="https://github.com/cabauman">Colt Bauman</a>
        <p>South Korea</p>
      </td>
     </tr>
  </tbody>
</table>

## .NET Foundation

ReactiveUI is part of the [.NET Foundation](https://www.dotnetfoundation.org/). Other projects that are associated with
the foundation include the Microsoft .NET Compiler Platform ("Roslyn") as well as the Microsoft ASP.NET family of
projects, and Microsoft .NET Core.
