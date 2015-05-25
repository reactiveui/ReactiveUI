# Activation

    @paulcbetts: any chance of an official comment on http://stackoverflow.com/questions/29508167/reactiveui-onewaybind-leaks-handles ? Should we be wrapping all bindings like `this.WhenActivated(disposeOfMe => {` something that should be done?
    
    ReactiveUI OneWayBind leaks handles
    I have a simple ViewModel: public class MeetingPageViewModel : ReactiveObject, IRoutableViewModel { public MeetingPageViewModel(IScreen hs, IMeetingRef mRef) { HostScreen = hs; ...
    
    ota [10:04 AM] 
    btw ghuntley also when using an observable instead of reactivelist, an add should cause a redraw afaik. i think thats more a xamarin.forms issue
    
    ota [10:04 AM]
    but we should get that reactivelist bug fixed, its an annoyance :disappointed:

    paulcbetts [10:18 AM] 
    @ghuntley Gordon's answer is basically 100% on-point
    
    ota [10:19 AM] 
    yeah but should we do it all the time?
    
    ota [10:19 AM]
    or just if we know it leaks?
    
    ota [10:30 AM] 
    @paulcbetts: does the whenactived method only work when using the reactiveui routing?
    
    paulcbetts [10:31 AM] 
    It works for any IViewFor
    
    paulcbetts [10:32 AM]10:32
    You don't need to do routing for views to implement IViewFor<T>
    
    paulcbetts [10:32 AM]
    You should do it whenever you're using a XAML'ish platform
    
    ota [10:32 AM] 
    yeah, just thought that the routing of reactiveui has some play in the lifecycle
    
    paulcbetts [10:32 AM] 
    Because DependencyProperties suck
    
    ota [10:33 AM] 
    is there a IViewFor impl. that doesnt require a viewmodel? (edited)
    
    paulcbetts [10:33 AM] 
    No
    
    paulcbetts [10:33 AM]
    How would you be using bindings without a ViewModel?
    
    paulcbetts [10:34 AM]
    I guess you could want to dispose other stuff, you could just never set the ViewModel
    
    ota [10:35 AM] 
    well, our ExtendedListView is a simple view without a viewmodel, but hast a dependencyproperty
    
    ota [10:38 AM]
    thats the reason why i was asking :simple_smile:
