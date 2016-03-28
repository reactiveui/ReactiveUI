using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Windows;

#if NET_45
[assembly: ThemeInfo(
   ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
                                    //(used if a resource is not found in the page, 
                                    // or application resource dictionaries)
   ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
                                             //(used if a resource is not found in the page, 
                                             // app, or any theme specific resource dictionaries)
)]
#endif

[assembly: InternalsVisibleTo("ReactiveUI.Tests_Net45")]
[assembly: InternalsVisibleTo("ReactiveUI.Tests_WinRT")]
[assembly: InternalsVisibleTo("ReactiveUI_Tests_iOS")] // NB: iOS apps can't have '.' in the name
[assembly: InternalsVisibleTo("ReactiveUI.Tests_Android")]
[assembly: InternalsVisibleTo("ReactiveUI.Winforms")]
[assembly: InternalsVisibleTo("ReactiveUI.XamForms")]
[assembly: InternalsVisibleTo("ReactiveUI.AndroidSupport")]
