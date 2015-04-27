# Apple iOS

    System.ExecutionEngineException: Attempting to JIT compile method 'System.Reactive.Linq.QueryLanguage:Switch (System.IObservable1<System.IObservable1>)' while running with --aot-only. See http://docs.xamarin.com/ios/about/limitations for more information.

    ghuntley [1:12 PM]
    When deploying on iOS 8.3, Unified API, physical hardware. Simulator works fine.

    ghuntley [1:12 PM]
    found this https://bugzilla.xamarin.com/show_bug.cgi?id=26060 and https://github.com/michaeldimoudis/RxFormsTest

    GitHub
    michaeldimoudis/RxFormsTest
    RxFormsTest

    ghuntley [1:13 PM]
    It seems that on iOS " enable generic value type sharing" on the advanced page must be toggled otherwise the application crashes immediately on startup when doing the  
    ```RxApp.SuspensionHost.SetupDefaultSuspendResume();```

    ghuntley [1:14 PM]
    Still not completely resolved, now got another exception - most likely my code but hopefully the above helps someone else out :simple_smile:

    ghuntley [1:23 PM] 
    @michaelstonis: what's your experience with
    ```enable generic value type sharing```
    - do you still your iOS apps with it? What linker settings do you use as well.

    ghuntley [1:24 PM]
    oh different michael sorry :simple_smile:

    michaelstonis [1:25 PM] 
    no worries

    michaelstonis [1:25 PM]
    FWIW, I always use generic type sharing

    ghuntley [1:25 PM] 
    It's not a default option w/ Right Click -> Add Project

    michaelstonis [1:29 PM] 
    yea, it will make your apps a lil' bit bigger, but the benefit is that you to use code that would have nuked out from JIT compilation

    michaelstonis [1:29 PM]
    it should be a default in newer releases

    michaelstonis [1:30 PM]
    it was a while back, but I think they flipped it to be a default in Xamarin.iOS 6 or 7

    ghuntley [1:48 PM] 
    FYI :simple_smile:

    paulcbetts [2:16 PM] 
    Check every experimental box

    paulcbetts [2:17 PM]
    You can often also get around this class of bugs by turning your value-typed Observable into a reference typed one

    paulcbetts [2:18 PM]
    I.e. don't use IObservable<int>, use IObservable<int?>
