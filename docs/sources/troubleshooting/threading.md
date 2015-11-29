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

	ghuntley [2:17 PM] 
	@kentcb what if that was baked in by default, framework inclusion? It’s such a common mistake and pain in the ass to debug. Splat.Verbose(s => s changed off ui thread) - etc.


	kentcb [2:39 PM] 
	@ghuntley @moswald Maybe. There are some concerns though: assuming UI thread is 1 (true on iOS, but probably not everywhere and maybe not even safe to assume there). Also, might be better with `PropertyChanging` rather than `PropertyChanged`, just to get in as early as possible. I’m not sure it’s framework-worthy, but certainly a useful trick to know about.



	paulcbetts [5:58 PM] 
	I'm not super excited about this change, because it costs a lot of perf in order to basically just change an error message (i.e. on most platforms it will crash anyways, we're just changing how it crashes)

	​[5:59] 
	On :apple:-based platforms where threading issues silently do Weird Shit it's maybe a different story, but definitely not worth it anywhere else


	kentcb [7:39 PM] 
	I feel like this is something that should be both platform and build specific (debug only). To my knowledge, that would require the call site to be in charge of whether it happens or not, and therefore can’t be baked into the framework (unless we shipped both debug and release NuGets, which would be terribly unorthodox and painful for consumers)
