using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Windows;

[assembly: AssemblyDescription("An MVVM framework that integrates the Reactive Extensions")]
[assembly: AssemblyProduct("ReactiveUI.Xaml")]
[assembly: AssemblyVersion("4.6.3")]

#if !(WINRT || SILVERLIGHT || MONO)
[assembly: ThemeInfo(
   ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
    //(used if a resource is not found in the page, 
    // or application resource dictionaries)
   ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
    //(used if a resource is not found in the page, 
    // app, or any theme specific resource dictionaries)
)]
#endif

[assembly: InternalsVisibleTo("ReactiveUI.Tests")]
[assembly: InternalsVisibleTo("ReactiveUI.Tests_SL4")]
[assembly: InternalsVisibleTo("ReactiveUI.Tests_SL5")]
[assembly: InternalsVisibleTo("ReactiveUI.Tests_WinRT")]
