All the magic of how ReactiveUI overrides the Back button is here:

https://github.com/reactiveui/ReactiveUI/blob/master/ReactiveUI.Mobile/WP8AutoSuspendApplication.cs#L91

The way that this works in ReactiveUI is that there is a content control named RoutedViewHost that is listening to the Back being signaled (you can do whatever you want in response to the hardware Back button and cancel the default action). ReactiveUI maintains its own ViewModel-based back stack and manipulates that instead of using WP8s, and you never call WP8s navigation methods.

This effectively means that, from WP8's perspective, there is only ever one page in the entire application. WP8 really wants to create that page itself, and it's specified in WMAppManifest.xml.

Don't try to participate in WP8's Frame system, it really wants to work its own way and you won't be able to convince it otherwise.

One last important thing, if you're at the bottom of your back stack, you must allow the default Back action to happen (i.e. what WP8 wanted to do, take you out of the app). Otherwise you'll probably fail Certification and you're Doing It Wrong™.