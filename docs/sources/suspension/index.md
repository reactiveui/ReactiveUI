# Suspension
  
    Make sure you [IgnoreDataMember] the HostScreen or you will get a circular reference.
    
    phil.cleveland [1:57 PM] 
    @paulcbetts: Cool.  Do I do that for every VM?
    
    paulcbetts [2:07 PM] 
    Yeah
    
    phil.cleveland [2:08 PM] 
    Cool.  Cleaning it all up now. Adding serialization and IViewFor to CB
    
    phil.cleveland [2:09 PM]
    Other than the [Attributes] does it all just work Magically?
    
    paulcbetts [2:14 PM] 
    Yep
    
    paulcbetts [2:14 PM]
    Well, kinda
    
    paulcbetts [2:14 PM]
    ViewModel serialization is Tricky Business
    
    paulcbetts [2:14 PM]
    You have to decide what to serialize and what to recreate
    
    paulcbetts [2:14 PM]
    Some stuff you should recalculate / reload when the app wakes up instead of trying to save it out
    
    phil.cleveland [2:16 PM] 
    Ok.  The one I am really thinking about now is a timer.  I have a screen with a count down timer and notifications.  So even if the app goes away I want that timer running.
    
    paulcbetts [2:16 PM] 
    Hm, Android definitely won't let you do that directly
    
    paulcbetts [2:16 PM]
    Your app will eventually get knocked out
    
    paulcbetts [2:17 PM]
    You need what's called a "Service" to do that
    
    phil.cleveland [2:17 PM] 
    hmm.  Well...I guess I could at least store the timestamp for start.....
    
    phil.cleveland [2:17 PM]
    Ok.  I'll look into it
    
    paulcbetts [2:17 PM] 
    ^^ that works
    
    phil.cleveland [2:17 PM] 
    I mean the standard clock that comes with....does more or less what I want
    
    paulcbetts [2:17 PM] 
    Oh wait, maybe you don't - there's a built-in API to wake your own app up at certain times as I recall (edited)
    
    phil.cleveland [2:18 PM] 
    Oh...that would be cool.  If I could set a promise so to speak.  Like ...hey come talk to me in 20 minutes.
    
    kentcb [2:20 PM] 
    @paulcbetts: (regarding a conversation yesterday) I always thought the thing `ObservableForProperty` could do that `WhenAny` can not is specify `beforeChange`. That is, be notified about the old value that is about to be swapped out for the new.
    
    kentcb [2:20 PM]
    I use this quite a bit to proactively clean up disposable stuff
    
    kentcb [2:21 PM]
    e.g.
    ```public SomeDisposableType Property
    {
        // usual get/set here
    }
    
    // elsewhere
    this
        .ObservableForProperty(x => x.Property, beforeChange: true)
        .Where(x => x != null)
        .Subscribe(x => x.Dispose());
    ```
    
    kentcb [2:21 PM]
    is this doable with `WhenAnyValue` somehow?
    
    phil.cleveland [2:22 PM] 
    could you use WhenActivated?
    
    kentcb [2:22 PM] 
    it's not necessarily about activation - more for when some disposable instance is being swapped out for another
    
    paulcbetts [2:23 PM] 
    You should use `SerialDisposable` instead
    
    paulcbetts [2:23 PM]
    It'll change your life :simple_smile:
    
    kentcb [2:24 PM] 
    I see, so just assign the new instance to the `SerialDisposable`
    
    phil.cleveland [2:24 PM] 
    Oh man.  I just learned WhenActivate....now comes along a trump
    
    paulcbetts [2:24 PM] 
    @kentcb: Got it
    
    kentcb [2:24 PM] 
    hmm, very cool - different way of thinking about it
    
    paulcbetts [2:25 PM] 
`this.WhenAnyValue(x => x.MyDisposableProp).Subscribe(x => latestDisposableProp.Disposable = x);  // Trashes the old one on assign
