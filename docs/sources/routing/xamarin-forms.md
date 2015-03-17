# Xamarin Forms

    paulcbetts [7:36 AM] 
    Are you using Forms?
    
    phil.cleveland [7:36 AM] 
    yea
    
    paulcbetts [7:37 AM] 
    Oh ok, so that's a different story
    
    phil.cleveland [7:37 AM] 
    Sorry I didn't mention it
    
    paulcbetts [7:37 AM] 
    Forms hides away a lot of the Special Snowflakeness
    
    paulcbetts [7:37 AM]
    In that case, just create a new ViewModel and Navigate to it
    
    paulcbetts [7:37 AM]
    Then let RoutedViewHost find the view
    
    phil.cleveland [7:38 AM] 
    That is working :sparkle: for me. but when i nav away to a diff VM.....where does my data go?
    
    phil.cleveland [7:39 AM]
    So say View1 and View2 are data entry and View3 uses said data to display some "answers"
    
    phil.cleveland [7:39 AM]
    In VM3 I need the data from VM1 and VM2.
    
    phil.cleveland [7:39 AM]
    Does the RoutedViewHost have those?
    
    paulcbetts [7:39 AM] 
    So VM2 would _create_ VM3 and Navigate to it
    
    phil.cleveland [7:39 AM] 
    yes
    
    paulcbetts [7:40 AM] 
    When you use ViewModel-first routing, you basically write your entire app in ViewModels
    
    phil.cleveland [7:40 AM] 
    ok. I see my problem
    
    paulcbetts [7:40 AM] 
    Like, the View doesn't know anything wrt navigation
    
    phil.cleveland [7:40 AM] 
    I am using the View to navigate
    
    paulcbetts [7:40 AM] 
    It just gets pulled around
    
    phil.cleveland [7:40 AM] 
    Got it.  That's where I went astray
    
    phil.cleveland [7:40 AM]
    Thanks
    
    paulcbetts [7:41 AM] 
    That's the cool part of ViewModel-based navigation, you can basically test the flow of your entire app
    
    paulcbetts [7:42 AM]
    Whereas with View-first, you can only test the pieces
    
    phil.cleveland [7:42 AM] 
    Never really thought of it that way.  One thing that seems like it would suck is if you want to change the order....you end up having to change several VMs.
    
    phil.cleveland [7:42 AM]
    But I do like the testability standpoint
    
    phil.cleveland [7:44 AM]
    So for instance in my VMs I am thinking of having Next and Prev commands.....where the cmd will be HostScreen.Router.NavigateCmdFOr<NextVM>.  Legit? (edited)
    
    paulcbetts [7:45 AM] 
    Sure, if that makes sense for your app
    
    paulcbetts [7:45 AM]
    But usually the next VM will need data from the previous VM
    
    paulcbetts [7:45 AM]
    So creating a blank one isn't super useful
    
    phil.cleveland [7:45 AM] 
    Ahh. Ok yes I see
    
    phil.cleveland [7:47 AM]
    So instead I would use `hostScreen.Router.Navigate(myNewViewModel(dataFromOldVM)`
