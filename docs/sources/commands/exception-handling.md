# Exception Handling

## Root Cause
http://stackoverflow.com/questions/26219105/what-is-the-reactiveui-way-to-handle-exceptions-when-executing-inferior-reactive

### Question

    var reactiveCommandA = ReactiveCommand.CreateAsyncTask(_ => CanPossiblyThrowAsync());
    reactiveCommandA.ThrownExceptions
                    .Subscribe(ex => UserError.Throw("Oh no A", ex));
    
    var reactiveCommandB = ReactiveCommand.CreateAsyncTask(_ => CanAlsoPossiblyThrowAsync());
    reactiveCommandB.ThrownExceptions
                    .Subscribe(ex => UserError.Throw("Oh no B", ex));
    
    var reactiveCommandC = ReactiveCommand.CreateAsyncTask
       (
         async _ =>
                   {
                     await reactiveCommandA.ExecuteAsync(); // <= Could throw here
                     await reactiveCommandB.ExecuteAsync();
                     DoSomethingElse();
                   }
        );
    
    reactiveCommandC.ThrownExceptions
                    .Subscribe(ex => UserError.Throw("Oh no C", ex));

So assume my background implementation for reactiveCommandA might throw an exception. That is OK, since I have subscribed to .ThrownExceptions and will theoretically notify the user and retry/fail/abort (not shown here for brevity). So it will not bubble up to the dispatcher.

So that is great when reactiveCommandA is executed by itself. However, I have reactiveCommandC which executes reactiveCommandA and reactiveCommandB. I also subscribe to its .ThrownExceptions. The problem I'm running into is that if I execute reactiveCommandC and reactiveCommandA implementation throws within it, it also causes reactiveCommandC to blow up. Then I'm notifying the user twice for the same root error becuase reactiveCommandA does its .ThrownExceptions thing, then reactiveCommandC does its .ThrownExceptions thing.

So is there a standard approach to this type of situation? Preferably something somewhat elegant, since I find the existing code fairly clean and I don't want to clutter things up or introduce spaghetti.

Things I have thought of:


* Surrounding the "await..." line with try/catch block and swallowing exception and exiting. Seems ugly if I have to do it a lot.
* Using await reactiveCommandA.ExecuteAsync().Catch(Observable.Never<Unit>()); although I think this will cause reactiveCommandC to never complete so it can never execute again.
* Using the same approach with the .Catch() method but returning a boolean based on whether I made it through successfully or not (e.g. .Catch(Observable.Return(false)). Would still have to check if we could continue between each await statement.

### Solution:

    Observable.Merge(rxCmdA.ThrownExceptions, rxCmdB.ThrownExceptions, rxCmdC.ThrownExceptions)
        .Throttle(TimeSpan.FromMilliseconds(250), RxApp.MainThreadScheduler)
        .Subscribe(ex => UserError.Throw("Oh no C", ex));
