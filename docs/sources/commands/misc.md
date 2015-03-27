# Misc


    paulcbetts [10:41 AM]
    It's not obvious that you should use Commands for things that aren't buttons in ReactiveUI, it's kind of a unique thing

    paulcbetts [10:41 AM]
    But any time you want to run something in the background, then bring its result back, ReactiveCommand is pretty cool

    michaelteper [11:20 AM] 
    I didnt know about `InvokeCommand`. Is that equivalent to `.Subscribe(_ => cmd.Execute(null))`?

    rdavisau [11:21 AM] 
    ^ it passes the value of the observable as the parameter, I believe

    michaelteper [11:22 AM] 
    that would make sense, sure

    michaelteper [11:22 AM]
    so `.Subscribe(x => cmd.Execute(x))`

    rdavisau [11:22 AM] 
    yep

    michaelteper [11:29 AM] 
    that would be good to know indeed

    ghuntley [11:30 AM] 
    would be kinda cool to see a fully fleshed out sample of your navigation pattern if you can afford the time :simple_smile:
    NEW MESSAGES

    paulcbetts [11:43 AM] 
    InvokeCommand respects CanExecute
