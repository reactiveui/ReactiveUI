# WhenActivated (Object Lifecycle Management)

    ghuntley [10:07 AM] @kentcb / @rdavisau / @flagbug do you use
    this.WhenActivated (ie. prevent dependency property leaking) on Xamarin
    Forms/Android/iOS? Is it needed on these platforms or is it just an XAML/WPF
    quirk?(edited)

    ie.  ```this.WhenActivated(d => { d(ViewModel.WhenAnyValue(x =>
    x.Something).Subscribe(...)); });```

    michaelteper [10:08 AM] I use it regularly on Xam.Mac, definitely needed in
    some scenarios

    kentcb [10:08 AM] I always use it, but I have my own variant that takes a
    `CompositeDisposable` and an `AddTo` extension method: ```this .WhenActivated(
    disposables => { this .Bind(…) .AddTo(disposables); }); ```

    ghuntley [10:09 AM] Any reason why in the documentation we don't make it a
    blanket rule / best practice that applies to all platforms?

    kentcb [10:10 AM] maybe because activation detection was/is imperfect? I
    had to fix the XF activation-for-view-fetcher, for example

    ghuntley [10:10 AM] something, something like " if you do a `WhenAny` on
    anything other than `this`, then you need to put it inside a `WhenActivated`"

    michaelteper [10:10 AM] Essentially, you only need it when RxUI is managing
    the lifetime of your views. E.g. when using `ViewModelViewHost`. If you just
    launch a window and then that window goes away when app goes away and you have
    nothing else to manage, you dont need WhenActivated...(edited)

    kentcb [10:12 AM] not ​_entirely_​ true, because it’s the
    activation-for-view-fetcher that defines lifetime so you can still use
    `WhenActivated` outside of routing and hosting. It will still work with a
    `Window` as long as the activation-for-view-fetcher is looking for the right
    things

    michaelteper [10:15 AM] true, it will work, it’s just not strictly required
    (as in, you can opt in if you need it, or ignore it) whereas if you do use
    hosting (sorry, no experience with routing), you pretty much have to use
    WhenActivated, otherwise you will have pretty nasty side-effects


    moswald [10:17 AM] you should use it any time there's something your view
    set up that will outlive the view - on a Xaml platform, you may have a
    subscription that you don't want active when the view isn't part of the visual
    tree

    ​[10:22] it's also useful for setting up things when you get added to
    the visual tree, even if it's not a disposable

    ​[10:22] although usually the correct place for something like that is
    in the ViewModel's `WhenActivated`

    flagbug [10:24 AM] I think it's required in WPF, because of the
    `DependencyProperty` stuff

    ​[10:24] I've only used it on Android for very special cases

    moswald [10:24 AM] definitely required for `DependencyProperty`

    flagbug [10:25 AM] i.e always use `WhenActivated` on WPF, if you're writing
    `this.WhenAnyValue(x => x.ViewModel.CoolProperty).BindWhatever`

    kentcb [10:28 AM] I guess the question is: is there any reason
    ​_not_​ to use it (because then we can’t simply recommend it as a
    default stance)

    flagbug [10:30 AM] I guess it's pretty unnecessary on e.g Android

    kentcb [10:31 AM] unnecessary maybe, but does it cause problems? I think
    it’s useful to be able to say “this is how you can do things” and have that way
    consistent across all platforms

    paulcbetts [10:31 AM] It's because on WPF, DependencyProperties leak

    kentcb [10:34 AM] @paulcbetts: are you saying that if WPF didn’t leak,
    `WhenActivated` simply wouldn’t exist?

    paulcbetts [10:34 AM] Well, it would still be useful in certain
    circumstances

    ​[10:34] You just wouldn't need to wrap every Bind to it

    kentcb [10:35 AM] but Bind takes a VM, and the VM is yet to be assigned in
    your view’s ctor

    ​[10:36] surely one of the things `WhenActivated` gives us is a point
    in time at which we ​_know_​ the VM is assigned?

    paulcbetts [10:37 AM] Bind works like WhenAny, it'll just reapply once the
    VM gets assigned

    kentcb [10:37 AM] ah, that’s right - the VM is just provided for type info

    paulcbetts [10:39 AM] Yep

    grokys [11:56 AM] 
    regarding DP leaks, what exactly causes the problem with rxui? is it `AddValueChanged`
    as in http://sharpfellows.com/post/Memory-Leaks-and-Dependency-Properties ?

    i'd like to know so we can avoid it in perspex

    paulcbetts [12:11 PM] 
    So the problem is, normally when there was a "ValueChanged" event, and you add a handler to it,
    the lifetime of the handler is tied to the lifetime of the object

    So even if you don't free ValueChanged's handler, if the object goes away, you're fine

    In XAML, if you hook change events on the "Value" property
    ​_Even when the object goes way_​, you have leaked the event
    Because it's tied to the static property ValueProperty
