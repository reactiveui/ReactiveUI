    rdavisau [11:41 AM] 
    Any advice on handing exceptions from tasks that run indefinitely? I have a piece of work set up during init of a viewmodel in PCL that runs "forever" - currently kicked off by `Task.Factory.StartNew`. I can't `await` it because it would prevent the rest of application from proceeding, but without `await`ing, any exceptions it might throw are allowed to run rampant and unchecked. Please send help :raised_hands: (edited)
    
    paulcbetts [12:06 PM] 
    Observable.Defer(() => Observable.Start().Catch()).Retry()
    
    rdavisau [12:13 PM] 
    thanks
    
    rdavisau [12:13 PM] 
    reads up on `Defer` and `Start
