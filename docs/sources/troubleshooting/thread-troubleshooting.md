# Thread Troubleshooting

    moswald [5:04 AM] 
    does anyone have a good way to find which property on a View is being accessed from the wrong thread?
    sigh. found it, but there should be an easier way than just adding `.ObserveOn(RxApp.MainThreadScheduler)` everywhere until it goes away
    
    paulcbetts [6:43 AM] 
    Can't you just see it on the call stack?
    
    Disable Just My Code
    
    moswald [6:44 AM] 
    oh, you know what, I think that you gave me this same tip like a year ago
    disabling Just My Code
    it's turned off already
    it throws inside the setter for the View's VM, which is weird because the VM has been set already - that's why I have to guess which one is doing it
    
    paulcbetts [7:27 AM] 
    Just go back up the stack
