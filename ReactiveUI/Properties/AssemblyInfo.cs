using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

[assembly: AssemblyDescription("An MVVM framework that integrates the Reactive Extensions")]
[assembly: AssemblyProduct("ReactiveUI")]
[assembly: AssemblyVersion("5.0.0")]

[assembly: InternalsVisibleTo("ReactiveUI.Xaml")]
[assembly: InternalsVisibleTo("ReactiveUI.Tests_Net45")]
[assembly: InternalsVisibleTo("ReactiveUI.Tests_WinRT")]
[assembly: InternalsVisibleTo("ReactiveUI.NLog")]
[assembly: InternalsVisibleTo("ReactiveUI.Gtk")]
[assembly: InternalsVisibleTo("ReactiveUI.Cocoa")]
[assembly: InternalsVisibleTo("ReactiveUI.Android")]
[assembly: InternalsVisibleTo("ReactiveUI.Mobile")]