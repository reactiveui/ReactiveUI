# ReactiveUI Routing and Navigation

Many apps, especially mobile apps where the screen is smaller, employ a
page-based navigation structure - one where the user can navigate forward and
back along a view hierarchy. Some platforms, such as WP8, have a hardware back
button specifically designed to facilitate navigation, whereas other platforms
such as WinRT have the back button as part of the page.

However, all of these platforms are designed with a View-first model, where
Views are either created via the framework automatically, or are the core
component in navigation. While this approach is the most direct, it also is
untestable. 

An alternate approach is to create the navigation "stack" as a stack of
ViewModels, and allow the associated View to be created at runtime instead,
based on the data in the VM. This approach also allows us to more easily
serialize the state of the application when asked to suspend by the operating
system, which is particularly important on mobile platforms.

### Where is Routing supported?

ViewModel-based routing is supported on WinRT, WP8, and WPF Desktop
applications. While preliminary support also exists for iOS and Android,
the OS is too opinionated about how Views are created on these platforms.

### tl;dr on how to set up routing

1. Set up a ViewModel for your main window and make it an `IScreen` (it should
   be a ReactiveObject). In this class, navigate to your first ViewModel.
1. Make ViewModels that represent pages in your application derive from
   `IRoutableViewModel`
1. Add an instance of RoutedViewHost to your main page or window, and bind it
   to an instance of your IScreen
1. (On WP8 / WinRT) - Set up ReactiveUI.Mobile so that on WP8, you get correct
   handling of the hardware back button for free.

### Basics of how Routing works

ViewModel-based routing consists of a few parts:

##### ViewModels

* `IRoutableViewModel` - an interface indicating that the given ViewModel is
  a ViewModel that can be navigated to (i.e. it is a page that could be in the
  navigation stack). ViewModels that are marked with this interface can be
  navigated to, via `IScreen.Router.Navigate.Execute`.

  This class also has a property, `UrlPathSegment`, which on certain platforms
  is used as the title for the frame (for example, in a
  UINavigationViewController on iOS).

* `IScreen` - an interface marking the ViewModel that represents the "frame"
  that is hosting a navigation stack. In most applications, you probably only
  have one of these, and it conceptually represents "The ViewModel for your
  main window".

* `RoutingState` - This class represents the "back stack" - the stack of
  currently available ViewModels. The last ViewModel on the stack is the
  currently displayed one. RoutingState also comes with several
  ReactiveCommands to navigate forward and back.

##### Views

* `IViewFor<TViewModel>` - Since every `IRoutableViewModel` will be displayed
  using ReactiveUI's View Location system, Views need to be registered.

* `RoutedViewHost` - The "View" for a RoutingState; this will Subscribe to the
  RoutingState and as new ViewModels are pushed onto the navigation stack, the
  view will be displayed. This class is similar to `ViewModelViewHost`, but
  tracks a RoutingState instead.

### Testing navigation

One of the core benefits of ViewModel-based routing is that navigation between
pages can be verified in a unit test. Since RoutingState is a relatively
simple data object, verifications such as "After the Ok button is hit on the
Login Page, verify we're on the Welcome page" are relatively straightforward
to accomplish.
