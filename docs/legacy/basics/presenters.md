paulcbetts [8:59 AM] 
@ghuntley: At a high level, the single View is being pushed on and off the IScreen, yeah

paulcbetts [9:00 AM]
Not familiar with the concept of Presenters

ghuntley [9:11 AM] 
Customization of the behavior of views  when navigating between ViewModels. For example pushing on and off UINavigationViewController, switching between uitabbarcontroller options, displaying as splitviewcontroller on iPad but hamburger on iPhone.

ghuntley [9:11 AM]
http://4.bp.blogspot.com/-Vg7F4u_Unew/UaXgifRFsfI/AAAAAAAAA2o/GIVg5-VM6tM/s1600/redblue1.png (27KB)


ghuntley [9:11 AM]
http://4.bp.blogspot.com/-6UzllelBSj4/UaXgjuLbkcI/AAAAAAAAA2w/Cs23N8j8sIM/s1600/redblue2.png (7KB)


paulcbetts [9:12 AM] 
Ah, in that case you'd have to create your own `RoutedViewHost` which isn't too hard

ghuntley [9:13 AM] 
:heart:

https://github.com/reactiveui/ReactiveUI/blob/master/ReactiveUI/Xaml/RoutedViewHost.cs