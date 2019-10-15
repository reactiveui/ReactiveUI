# Security Policy

## Supported Versions

ReactiveUI supports the latest versions of the following packages.

| Platform          | ReactiveUI Package                       | NuGet                                  | [Events][EventsDocs] Package            | Supported          |
| ----------------- | ---------------------------------------- | -------------------------------------- | --------------------------------------- | ------------------ |
| .NET Standard     | [ReactiveUI][CoreDoc]                    | [![CoreBadge]][Core]                   | None                                    | :white_check_mark: |
|                   | [ReactiveUI.Fody][FodyDoc]               | [![FodyBadge]][Fody]                   | None                                    | :white_check_mark: |
| Unit Testing      | [ReactiveUI.Testing][TestDoc]            | [![TestBadge]][Test]                   | None                                    | :white_check_mark: |
| Universal Windows | [ReactiveUI][UniDoc]                     | [![CoreBadge]][Core]                   | [ReactiveUI.Events][CoreEvents]         | :white_check_mark: |
| WPF               | [ReactiveUI.WPF][WpfDoc]                 | [![WpfBadge]][Wpf]                     | [ReactiveUI.Events.WPF][WpfEvents]      | :white_check_mark: |
| Windows Forms     | [ReactiveUI.WinForms][WinDoc]            | [![WinBadge]][Win]                     | [ReactiveUI.Events.WinForms][WinEvents] | :white_check_mark: |
| Xamarin.Forms     | [ReactiveUI.XamForms][XamDoc]            | [![XamBadge]][Xam]                     | [ReactiveUI.Events.XamForms][XamEvents] | :white_check_mark: |
| Xamarin.Essentials| [ReactiveUI][XamDoc]                     | [![CoreBadge]][Core]                   | [ReactiveUI.Events.XamEssentials][XamE] | :white_check_mark: |
| Xamarin.Android   | [ReactiveUI.AndroidSupport][DroDoc]      | [![DroBadge]][Dro]                     | [ReactiveUI.Events][CoreEvents]         | :white_check_mark: |
| Xamarin.iOS       | [ReactiveUI][IosDoc]                     | [![CoreBadge]][Core]                   | [ReactiveUI.Events][CoreEvents]         | :white_check_mark: |
| Xamarin.Mac       | [ReactiveUI][MacDoc]                     | [![CoreBadge]][Core]                   | [ReactiveUI.Events][CoreEvents]         | :white_check_mark: |
| Tizen             | [ReactiveUI][CoreDoc]                    | [![CoreBadge]][Core]                   | [ReactiveUI.Events][CoreEvents]         | :white_check_mark: |
| Platform Uno      | ReactiveUI.Uno                           | [![UnoBadge]][Uno]                     | None                                    | :white_check_mark: |
| Avalonia          | [Avalonia.ReactiveUI][AvaDoc]            | [![AvaBadge]][Ava]                     | None                                    | :white_check_mark: |
| Any               | [ReactiveUI.Validation][ValidationsDocs] | [![ValidationsBadge]][ValidationsCore] | None                                    | :white_check_mark: |

[Core]: https://www.nuget.org/packages/ReactiveUI/
[CoreEvents]: https://www.nuget.org/packages/ReactiveUI.Events/
[CoreBadge]: https://img.shields.io/nuget/v/ReactiveUI.svg
[CoreDoc]: https://reactiveui.net/docs/getting-started/installation/

[Fody]: https://www.nuget.org/packages/ReactiveUI.Fody/
[FodyDoc]: https://reactiveui.net/docs/handbook/view-models/#managing-boilerplate-code
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

[MacDoc]: https://reactiveui.net/docs/getting-started/installation/xamarin-mac
[IosDoc]: https://reactiveui.net/docs/getting-started/installation/xamarin-ios

[Uno]: https://www.nuget.org/packages/ReactiveUI.Uno/
[UnoBadge]: https://img.shields.io/nuget/v/ReactiveUI.Uno.svg
[UnoDoc]: https://reactiveui.net/docs/getting-started/installation/uno-platform


[Ava]: https://www.nuget.org/packages/Avalonia.ReactiveUI/
[AvaBadge]: https://img.shields.io/nuget/v/Avalonia.ReactiveUI.svg
[AvaDoc]: https://reactiveui.net/docs/getting-started/installation/avalonia
[EventsDocs]: https://reactiveui.net/docs/handbook/events/

[ValidationsCore]: https://www.nuget.org/packages/ReactiveUI.Validation/
[ValidationsBadge]: https://img.shields.io/nuget/v/ReactiveUI.Validation.svg
[ValidationsDocs]: https://reactiveui.net/docs/handbook/user-input-validation/

## Reporting a Vulnerability

We will publish security report instructions and encryption keys.

Please do not open issues for anything you think might have a security implication.
