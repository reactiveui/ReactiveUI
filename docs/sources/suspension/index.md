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


    paulcbetts [2:26 PM] 
    ```this.WhenAnyValue(x => x.MyDisposableProp)
        .Subscribe(x => latestDisposableProp.Disposable = x != null ? x : Disposable.Empty);

    kentcb [2:27 PM] 
    nice
    
    phil.cleveland [2:27 PM] 
    So would this be something you would do for instance VM1 has a ReactiveObject prop which changes based on state?
    
    kentcb [2:29 PM] 
    is it disposable though?
    
    phil.cleveland [2:30 PM] 
    The VM....I guess in this particular case no
    
    phil.cleveland [2:31 PM]2:31
    I guess I don't impl IDisposable that often TBH.  Wonder if I am missing some leaks
    
    kentcb [2:34 PM] 
    I've gotten into the practice of following *any* call to `Subscribe` or `Connect` or `Bind` et cetera with my own `AddTo` extension:
    ```this.someProperty = this.WhenAnyValue(x => x.SomeProperty)
    .Select(x => x == null)
    .ToProperty(this, x => x.SomeOtherProperty)
    .AddTo(this.disposables);
    ```
    (edited)
    
    kentcb [2:34 PM]
    and `AddTo` is an extension method that adds any `IDisposable` to a `CompositeDisposable`
    
    kentcb [2:34 PM]
    and it's the `CompositeDisposable` that I dispose of
    
    phil.cleveland [2:35 PM] 
    Per a discussion with Paul the other day I think that is only necessary in the code behind
    
    kentcb [2:35 PM] 
    in my `Dispose()`
    
    phil.cleveland [2:35 PM] 
    Yea @kentcb I saw your DisposableReactiveObject in the excercise app.  I think that could be part of RxUI
    
    phil.cleveland [2:36 PM]
    I actually think that looks cleaner than using the WhenActivated too.  Again I might be misunderstanding when each is appropriate
    
    phil.cleveland [2:37 PM]
    this is what my CB WhenAny's look like
    ```this.WhenActivated (d => {
                d (this.WhenAnyValue (x => x.ViewModel.AnticipatedWortLossVolume).Subscribe (x => this.stpWortLoss.Value = x));
            });
    ```
    
    phil.cleveland [2:38 PM]
    the curlys and parens can cause a headache sometimes

    kentcb [2:43 PM] 
    yeah, could be a little neater with an extension method:
    ```this.WhenActivated(
        d => this
                .WhenAnyValue(x => x.ViewModel.AnticipatedWortLossVolume)
                .Subscribe(x => this.stpWortLoss.Value = x)
                .AddTo(d));
    ```
    (just using `AddTo` as example name - might be able to come up with a better one) (edited)
    
    kentcb [2:45 PM]
    am a little surprised that `WhenActivated` doesn't have an overload giving you a `CompositeDisposable`. That would make things a bit easier
    
    rdavisau [2:56 PM] 
    ^ I have used that approach too, @kentcb
