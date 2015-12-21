# WhenActivated (Object Lifecycle Management)

    ghuntley [10:07 AM] 
    @kentcb / @rdavisau / @flagbug do you use this.WhenActivated (ie. prevent dependency property leaking) on Xamarin Forms/Android/iOS? Is it needed on these platforms or is it just an XAML/WPF quirk?(edited)

    ie.
    ```this.WhenActivated(d =>
        {
           d(ViewModel.WhenAnyValue(x => x.Something).Subscribe(...));
        });```

    michaelteper [10:08 AM] 
    I use it regularly on Xam.Mac, definitely needed in some scenarios

    kentcb [10:08 AM] 
    I always use it, but I have my own variant that takes a `CompositeDisposable` and an `AddTo` extension method:
    ```this
        .WhenActivated(
            disposables =>
            {
                this
                    .Bind(…)
                    .AddTo(disposables);
            });
    ```

    ghuntley [10:09 AM] 
    Any reason why in the documentation we don't make it a blanket rule / best practice that applies to all platforms?

    kentcb [10:10 AM] 
    maybe because activation detection was/is imperfect? I had to fix the XF activation-for-view-fetcher, for example

    ghuntley [10:10 AM] 
    something, something like " if you do a `WhenAny` on anything other than `this`, then you need to put it inside a `WhenActivated`"

    michaelteper [10:10 AM] 
    Essentially, you only need it when RxUI is managing the lifetime of your views. E.g. when using `ViewModelViewHost`. If you just launch a window and then that window goes away when app goes away and you have nothing else to manage, you dont need WhenActivated...(edited)

    kentcb [10:12 AM] 
    not ​_entirely_​ true, because it’s the activation-for-view-fetcher that defines lifetime
    so you can still use `WhenActivated` outside of routing and hosting. It will still work with a `Window` as long as the activation-for-view-fetcher is looking for the right things
    
    michaelteper [10:15 AM] 
    true, it will work, it’s just not strictly required (as in, you can opt in if you need it, or ignore it) whereas if you do use hosting (sorry, no experience with routing), you pretty much have to use WhenActivated, otherwise you will have pretty nasty side-effects
    
    moswald [10:17 AM] 
    you should use it any time there's something your view set up that will outlive the view - on a Xaml platform, you may have a subscription that you don't want active when the view isn't part of the visual tree
