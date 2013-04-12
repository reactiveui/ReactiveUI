using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Windows;

[assembly: AssemblyDescription("An MVVM framework that integrates the Reactive Extensions")]
[assembly: AssemblyProduct("ReactiveUI.Xaml")]
[assembly: AssemblyVersion("4.5.0")]

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

[assembly: InternalsVisibleTo("ReactiveUI.Tests, PublicKey=002400000480000094000000060200000024000052534131000400000100010081646602314c286f23145e9337a8b0081582ecd7425806b884008b8d5cd414a30399154b69c7aba1faf012737daa54d219f1978e64aa7bd73421a7615e2117d52b37e6d48ebf2028ab247a829758728cd96b89d3bbcd3d4139c68b6781a1c853bbb39dc6eeea007e1ba52ee688fc5acbd0698bedf1000cc9d9b3ff9a7c55debb")]
[assembly: InternalsVisibleTo("ReactiveUI.Tests_SL4, PublicKey=002400000480000094000000060200000024000052534131000400000100010081646602314c286f23145e9337a8b0081582ecd7425806b884008b8d5cd414a30399154b69c7aba1faf012737daa54d219f1978e64aa7bd73421a7615e2117d52b37e6d48ebf2028ab247a829758728cd96b89d3bbcd3d4139c68b6781a1c853bbb39dc6eeea007e1ba52ee688fc5acbd0698bedf1000cc9d9b3ff9a7c55debb")]
[assembly: InternalsVisibleTo("ReactiveUI.Tests_SL5, PublicKey=002400000480000094000000060200000024000052534131000400000100010081646602314c286f23145e9337a8b0081582ecd7425806b884008b8d5cd414a30399154b69c7aba1faf012737daa54d219f1978e64aa7bd73421a7615e2117d52b37e6d48ebf2028ab247a829758728cd96b89d3bbcd3d4139c68b6781a1c853bbb39dc6eeea007e1ba52ee688fc5acbd0698bedf1000cc9d9b3ff9a7c55debb")]
[assembly: InternalsVisibleTo("ReactiveUI.Tests_WinRT, PublicKey=002400000480000094000000060200000024000052534131000400000100010081646602314c286f23145e9337a8b0081582ecd7425806b884008b8d5cd414a30399154b69c7aba1faf012737daa54d219f1978e64aa7bd73421a7615e2117d52b37e6d48ebf2028ab247a829758728cd96b89d3bbcd3d4139c68b6781a1c853bbb39dc6eeea007e1ba52ee688fc5acbd0698bedf1000cc9d9b3ff9a7c55debb")]
