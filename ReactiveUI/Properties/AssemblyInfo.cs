using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

[assembly: AssemblyDescription("An MVVM framework that integrates the Reactive Extensions")]
[assembly: AssemblyProduct("ReactiveUI")]

[assembly: InternalsVisibleTo("ReactiveUI.Xaml")]
[assembly: InternalsVisibleTo("ReactiveUI.Tests_Net45")]
[assembly: InternalsVisibleTo("ReactiveUI.Tests_WinRT")]
[assembly: InternalsVisibleTo("ReactiveUI_Tests_iOS")] // NB: iOS apps can't have '.' in the name
[assembly: InternalsVisibleTo("ReactiveUI.Tests_Android")]
[assembly: InternalsVisibleTo("ReactiveUI.NLog")]
[assembly: InternalsVisibleTo("ReactiveUI.Gtk")]
[assembly: InternalsVisibleTo("ReactiveUI.Cocoa")]
[assembly: InternalsVisibleTo("ReactiveUI.Android")]
[assembly: InternalsVisibleTo("ReactiveUI.Mobile")]
[assembly: InternalsVisibleTo("ReactiveUI.Winforms")]
