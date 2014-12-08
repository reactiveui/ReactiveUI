```cs
/* COOLSTUFF: What is the AppBootstrapper?
* 
* The AppBootstrapper is like a ViewModel for the WPF Application class.
* Since Application isn't very testable (just like Window / UserControl), 
* we want to create a class we can test. Since our application only has
* one "screen" (i.e. a place we present Routed Views), we can also use 
* this as our IScreen.
* 
* An IScreen is a ViewModel that contains a Router - practically speaking,
* it usually represents a Window (or the RootFrame of a WinRT app). We 
* should technically create a MainWindowViewModel to represent the IScreen,
* but there isn't much benefit to split those up unless you've got multiple
* windows.
* 
* AppBootstrapper is a good place to implement a lot of the "global 
* variable" type things in your application. It's also the place where
* you should configure your IoC container. And finally, it's the place 
* which decides which View to Navigate to when the application starts.
*/
```