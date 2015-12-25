@moswald has experienced issues with dotnet native and RxUI but @onovotny has not. 


https://reactivex.slack.com/archives/reactiveui/p1450928587001692


do you use the RxUI bind methods, or are you using `{x:Bind}`?

I pretty much only use x:Bind


I can see how Bind* would cause issues with dotnet native. 

if it comes down to it, I can always refactor things to use x:Bind. it doesn't support observables, so I'd have to convert some things to properties...but doable. 



