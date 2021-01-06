![Build](https://github.com/reactiveui/ReactiveUI/workflows/Build/badge.svg) 
 [![Code Coverage](https://codecov.io/gh/reactiveui/ReactiveUI/branch/main/graph/badge.svg)](https://codecov.io/gh/reactiveui/ReactiveUI) [![#yourfirstpr](https://img.shields.io/badge/first--timers--only-friendly-blue.svg)](https://reactiveui.net/contribute) 
[![Dependabot Status](https://api.dependabot.com/badges/status?host=github&repo=reactiveui/ReactiveUI)](https://dependabot.com)
<br>
<a href="https://www.nuget.org/packages/reactiveui">
        <img src="https://img.shields.io/nuget/dt/reactiveui.svg">
</a>

<a href="https://reactiveui.net/slack">
        <img src="https://img.shields.io/badge/chat-slack-blue.svg">
</a>
<br>
<br>
<a href="https://github.com/reactiveui/reactiveui">
  <img width="160" heigth="160" src="https://raw.githubusercontent.com/reactiveui/styleguide/master/logo/main.png">
</a>
<br>
<h1>What is ReactiveUI?</h1>

<a href="https://reactiveui.net/">ReactiveUI</a> is a composable, cross-platform model-view-viewmodel framework for all .NET platforms that is inspired by functional reactive programming, which is a paradigm that allows you to <a href="https://www.youtube.com/watch?v=3HwEytvngXk">abstract mutable state away from your user interfaces and express the idea around a feature in one readable place</a> and improve the testability of your application. 

<a href="https://reactiveui.net/docs/getting-started/">🔨 Get Started</a> <a href="https://reactiveui.net/docs/getting-started/installation/">🛍 Install Packages</a> <a href="https://reactiveui.net/docs/resources/videos">🎞 Watch Videos</a> <a href="https://reactiveui.net/docs/resources/samples/">🎓 View Samples</a> <a href="https://reactiveui.net/slack">🎤 Discuss ReactiveUI</a>

<h2>Book</h2>
There has been an excellent <a href="https://kent-boogaart.com/you-i-and-reactiveui/">book</a> written by our Alumni maintainer Kent Boogart.
 
<h2>NuGet Packages</h2>

Install the following packages to start building your own ReactiveUI app. <b>Note:</b> some of the platform-specific packages are required. This means your app won't perform as expected until you install the packages properly. See the <a href="https://reactiveui.net/docs/getting-started/installation/">Installation</a> docs page for more info.

| Platform          | ReactiveUI Package                  | NuGet                | [Events][EventsDocs] Package            |
| ----------------- | ----------------------------------- | -------------------- | --------------------------------------- |
| .NET Standard     | [ReactiveUI][CoreDoc]               | [![CoreBadge]][Core] | None                                    |
|                   | [ReactiveUI.Fody][FodyDoc]          | [![FodyBadge]][Fody] | None                                    |
| Unit Testing      | [ReactiveUI.Testing][TestDoc]       | [![TestBadge]][Test] | None                                    |
| Universal Windows | [ReactiveUI][UniDoc]                | [![CoreBadge]][Core] | [ReactiveUI.Events][CoreEvents]         |
| WPF               | [ReactiveUI.WPF][WpfDoc]            | [![WpfBadge]][Wpf]   | [ReactiveUI.Events.WPF][WpfEvents]      |
| Windows Forms     | [ReactiveUI.WinForms][WinDoc]       | [![WinBadge]][Win]   | [ReactiveUI.Events.WinForms][WinEvents] |
| Xamarin.Forms     | [ReactiveUI.XamForms][XamDoc]       | [![XamBadge]][Xam]   | [ReactiveUI.Events.XamForms][XamEvents] |
| Xamarin.Essentials| [ReactiveUI][XamDoc]                | [![CoreBadge]][Core] | [ReactiveUI.Events.XamEssentials][XamE] |
| AndroidX (Xamarin)| [ReactiveUI.AndroidX][DroDoc]       | [![DroXBadge]][DroX] | [ReactiveUI.Events][CoreEvents]         |
| Xamarin.Android   | [ReactiveUI.AndroidSupport][DroDoc] | [![DroBadge]][Dro]   | [ReactiveUI.Events][CoreEvents]         |
| Xamarin.iOS       | [ReactiveUI][IosDoc]                | [![CoreBadge]][Core] | [ReactiveUI.Events][CoreEvents]         |
| Xamarin.Mac       | [ReactiveUI][MacDoc]                | [![CoreBadge]][Core] | [ReactiveUI.Events][CoreEvents]         |
| Tizen             | [ReactiveUI][CoreDoc]               | [![CoreBadge]][Core] | [ReactiveUI.Events][CoreEvents]         |
| Blazor            | [ReactiveUI.Blazor][BlazDoc]        | [![BlazBadge]][Blaz] | None                                    |
| Platform Uno      | ReactiveUI.Uno                      | [![UnoBadge]][Uno]   | None                                    |
| Avalonia          | [Avalonia.ReactiveUI][AvaDoc]       | [![AvaBadge]][Ava]   | None                                    |
| Any               | [ReactiveUI.Validation][ValDocs]    | [![ValBadge]][ValCore] | None                                  |

[Core]: https://www.nuget.org/packages/ReactiveUI/
[CoreEvents]: https://www.nuget.org/packages/ReactiveUI.Events/
[CoreBadge]: https://img.shields.io/nuget/v/ReactiveUI.svg
[CoreDoc]: https://reactiveui.net/docs/getting-started/installation/

[Fody]: https://www.nuget.org/packages/ReactiveUI.Fody/
[FodyDoc]: https://reactiveui.net/docs/handbook/view-models/boilerplate-code
[FodyBadge]: https://img.shields.io/nuget/v/ReactiveUI.Fody.svg

[Test]: https://www.nuget.org/packages/ReactiveUI.Testing/
[TestBadge]: https://img.shields.io/nuget/v/ReactiveUI.Testing.svg
[TestDoc]: https://reactiveui.net/docs/handbook/testing/

[UniDoc]: https://reactiveui.net/docs/getting-started/installation/universal-windows-platform

[Wpf]: https://www.nuget.org/packages/ReactiveUI.WPF/
[WpfEvents]: https://www.nuget.org/packages/ReactiveUI.Events.WPF/
[WpfBadge]: https://img.shields.io/nuget/v/ReactiveUI.WPF.svg
[WpfDoc]: https://reactiveui.net/docs/getting-started/installation/windows-presentation-foundation

[Win]: https://www.nuget.org/packages/ReactiveUI.WinForms/
[WinEvents]: https://www.nuget.org/packages/ReactiveUI.Events.WinForms/
[WinBadge]: https://img.shields.io/nuget/v/ReactiveUI.WinForms.svg
[WinDoc]: https://reactiveui.net/docs/getting-started/installation/windows-forms

[Xam]: https://www.nuget.org/packages/ReactiveUI.XamForms/
[XamEvents]: https://www.nuget.org/packages/ReactiveUI.Events.XamForms/
[XamBadge]: https://img.shields.io/nuget/v/ReactiveUI.XamForms.svg
[XamDoc]: https://reactiveui.net/docs/getting-started/installation/xamarin-forms
[XamE]: https://www.nuget.org/packages/ReactiveUI.Events.XamEssentials/

[Dro]: https://www.nuget.org/packages/ReactiveUI.AndroidSupport/
[DroBadge]: https://img.shields.io/nuget/v/ReactiveUI.AndroidSupport.svg
[DroDoc]: https://reactiveui.net/docs/getting-started/installation/xamarin-android

[DroX]: https://www.nuget.org/packages/ReactiveUI.AndroidX/
[DroXBadge]: https://img.shields.io/nuget/v/ReactiveUI.AndroidX.svg

[MacDoc]: https://reactiveui.net/docs/getting-started/installation/xamarin-mac
[IosDoc]: https://reactiveui.net/docs/getting-started/installation/xamarin-ios

[Uno]: https://www.nuget.org/packages/ReactiveUI.Uno/
[UnoBadge]: https://img.shields.io/nuget/v/ReactiveUI.Uno.svg
[UnoDoc]: https://reactiveui.net/docs/getting-started/installation/uno-platform

[Blaz]: https://www.nuget.org/packages/ReactiveUI.Blazor/
[BlazBadge]: https://img.shields.io/nuget/v/ReactiveUI.Blazor.svg
[BlazDoc]: https://www.reactiveui.net/blog/2020/07/article-blazor-compelling-example

[Ava]: https://www.nuget.org/packages/Avalonia.ReactiveUI/
[AvaBadge]: https://img.shields.io/nuget/v/Avalonia.ReactiveUI.svg
[AvaDoc]: https://reactiveui.net/docs/getting-started/installation/avalonia
[EventsDocs]: https://reactiveui.net/docs/handbook/events/

[ValCore]: https://www.nuget.org/packages/ReactiveUI.Validation/
[ValBadge]: https://img.shields.io/nuget/v/ReactiveUI.Validation.svg
[ValDocs]: https://reactiveui.net/docs/handbook/user-input-validation/

<h2>Sponsorship</h2>

The core team members, ReactiveUI contributors and contributors in the ecosystem do this open-source work in their free time. If you use ReactiveUI, a serious task, and you'd like us to invest more time on it, please donate. This project increases your income/productivity too. It makes development and applications faster and it reduces the required bandwidth.

[Become a sponsor](https://github.com/sponsors/reactivemarbles).

This is how we use the donations:

* Allow the core team to work on ReactiveUI
* Thank contributors if they invested a large amount of time in contributing
* Support projects in the ecosystem

<h2>Support</h2>

If you have a question, please see if any discussions in our [GitHub issues](https://github.com/reactiveui/ReactiveUI/issues) or [Stack Overflow](https://stackoverflow.com/questions/tagged/reactiveui) have already answered it.

If you want to discuss something or just need help, here is our [Slack room](https://reactiveui.net/slack), where there are always individuals looking to help out!

Please do not open GitHub issues for support requests.

<h2>Contribute</h2>

ReactiveUI is developed under an OSI-approved open source license, making it freely usable and distributable, even for commercial use. 

If you want to submit pull requests please first open a [GitHub issue](https://github.com/reactiveui/ReactiveUI/issues/new/choose) to discuss. We are first time PR contributors friendly.

<h2>Core Team</h2>

<table>
  <tbody>
    <tr>
      <td align="center" valign="top">
        <img width="100" height="100" src="https://github.com/glennawatson.png?s=150">
        <br>
        <a href="https://github.com/glennawatson">Glenn Watson</a>
        <p>Melbourne, Australia</p>
      </td>
      <td align="center" valign="top">
        <img width="100" height="100" src="https://github.com/rlittlesii.png?s=150">
        <br>
        <a href="https://github.com/rlittlesii">Rodney Littles II</a>
        <p>Texas, USA</p>
      </td>
      <td align="center" valign="top">
        <img width="100" height="100" src="https://github.com/worldbeater.png?s=150">
        <br>
        <a href="https://github.com/worldbeater">Artyom Gorchakov</a>
        <p>Moscow, Russia</p>
      </td>
      <td align="center" valign="top">
        <img width="100" height="100" src="https://github.com/cabauman.png?s=150">
        <br>
        <a href="https://github.com/cabauman">Colt Bauman</a>
        <p>South Korea</p>
      </td>
    </tr>
  </tbody>
</table>

<h2>Alumni Core Team</h2>

The following have been core team members in the past.

<table>
  <tbody>
    <tr>
      <td align="center" valign="top">
        <img width="100" height="100" src="https://github.com/ghuntley.png?s=150">
        <br>
        <a href="https://github.com/ghuntley">Geoffrey Huntley</a>
        <p>Sydney, Australia</p>
      </td>
      <td align="center" valign="top">
        <img width="100" height="100" src="https://github.com/kentcb.png?s=150">
        <br>
        <a href="https://github.com/kentcb">Kent Boogaart</a>
        <p>Brisbane, Australia</p>
      </td>
      <td align="center" valign="top">
        <img width="100" height="100" src="https://github.com/olevett.png?s=150">
        <br>
        <a href="https://github.com/olevett">Olly Levett</a>
        <p>London, United Kingdom</p>
      </td>
      <td align="center" valign="top">
        <img width="100" height="100" src="https://github.com/anaisbetts.png?s=150">
        <br>
        <a href="https://github.com/anaisbetts">Anaïs Betts</a>
        <p>San Francisco, USA</p>
      </td>
      <td align="center" valign="top">
        <img width="100" height="100" src="https://github.com/shiftkey.png?s=150">
        <br>
        <a href="https://github.com/shiftkey">Brendan Forster</a>
        <p>Melbourne, Australia</p>
      </td>
      <td align="center" valign="top">
        <img width="120" height="100" src="https://github.com/clairernovotny.png?s=150">
        <br>
        <a href="https://github.com/clairernovotny">Claire Novotny</a>
        <p>New York, USA</p>
      </td>
     </tr>
  </tbody>
</table>

<h2>.NET Foundation</h2>

ReactiveUI is part of the [.NET Foundation](https://www.dotnetfoundation.org/). Other projects that are associated with the foundation include the Microsoft .NET Compiler Platform ("Roslyn") as well as the Microsoft ASP.NET family of projects, Microsoft .NET Core & Xamarin Forms.
