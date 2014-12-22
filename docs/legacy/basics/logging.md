# Logging

ReactiveUI comes with its own logging framework which can be used to debug
your applications as well as ReactiveUI itself. You may ask yourself,
"Seriously, another logging framework?". The reason RxUI does this itself is
for portability - none of the common popular logging frameworks support all of
the platforms that ReactiveUI supports, and many are server-oriented
frameworks and ill-suited for simple mobile app logging.

### this.Log() and IEnableLogger

ReactiveUI's logger works a bit differently than other frameworks - its
design is inspired by Rails 'logger'. To use it, make your class implement the
`IEnableLogger` interface:

```cs
public class MyClass : IEnableLogger
{
    // IEnableLogger doesn't actually require anything of us
}
```

Now, you can call the `Log` method on your class. Because of how extension
methods work, you must prepend `this` to it:

```cs
this.Log().Info("Downloaded {0} tweets", tweets.Count);
```

There are **five** levels of logging, `Debug`, `Info`, `Warn`, `Error`, and
`Fatal`. Additionally, there are special methods to log exceptions - for
example, `this.Log().InfoException(ex, "Failed to post the message")`.

This trick doesn't work for static methods though, you have to settle for an
alternate method, `LogHost.Default.Info(...)`.

### Debugging Observables

ReactiveUI has several helpers for debugging IObservables. The most
straightforward one is `Log`, which logs events that happen to an Observable:

```cs
// Note: Since Log acts like another Rx operator like Select or Where,
// it won't do anything by itself unless someone Subscribes to it.
this.WhenAny(x => x.Name, x => x.Value)
    .SelectMany(async x => GoogleForTheName(x))
    .Log(this, "Result of Search")
    .Subscribe();
```

Another useful method to debug Observables is `LoggedCatch`. This method works
identically to Rx's `Catch` operator, except that it also logs the exception
to the Logger. For example:

```cs
var userAvatar = await FetchUserAvatar()
    .LoggedCatch(this, Observable.Return(default(Avatar)));
```

### Configuring the logger

To configure the logger, register an implementation of `ILogger` (there are
several built-in ones, such as `DebugLogger`). Here's an example where we use
a built-in logger, but a custom log level:

```cs
// I only want to hear about errors
var logger = new DebugLogger() { LogLevel = LogLevel.Error };
RxApp.MutableResolver.RegisterConstant(logger, typeof(ILogger));
```

If you really need to control how things are logged, you can implement
`IFullLogger`, which will allow you to control every logging overload.

### Getting log messages to Visual Studio's IntelliTrace

1. Modify Visual Studio's IntelliTrace settings via Debug>IntelliTrace>Open
   IntelliTrace Settings>IntelliTrace Events>Tracing> Check everything except
   Assertion under tracing.  If you leave the default settings, you won't pick
   up Debug level tracing - by checking all of the Trace events, you will pick
   up Debug level messages.

1. Make sure you have the `reactiveui-nlog` package installed to your unit test
   assembly (unfortunately, you are out of luck if you are using a Windows
   Store Test Library, but a "normal" unit test library works fine)

1. Add a file called nlog.config to your unit test project.  __Make sure you
   set the "copy to output directory" property to "Copy if newer" or "Copy
   always"__  If you leave the default action of "Do not copy" then the
   nlog.config file won't be copied to your bin directory and nlog won't be
   able to find its config file, which means it won't know to write to the
   trace listener.

1. Here is the `nlog.config` file

```xml
<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
         xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" >
  <targets>
    <target name="trace" xsi:type="trace"  layout="RxUI:${message}"/>
  </targets>
  <rules>
    <logger name="ReactiveUI.*"  writeTo="trace" />
  </rules>
</nlog>
```

1. Register NLogger at the start of your unit test with:

``` cs
var logManager = RxApp.MutableResolver.GetService<ILogManager>();
RxApp.MutableResolver.RegisterConstant(logManager.GetLogger<NLogLogger>(),typeof(IFullLogger));   
```
*Hint: An easy way to filter the IntelliTrace view to only show ReactiveUI
events is to type RxUI into the IntelliTrace window search box*

# Notes 
    haacked [8:55 AM] 
    Hey man, whatever you need to tell yourself to maintain your sanity.
    
    haacked [8:56 AM]
    So does rxui6 get rid of this.Log()?
    
    paulcbetts [8:57 AM] 
    No, it's part of Splat
    
    paulcbetts [8:57 AM]
    It just got moved
    
    haacked [8:59 AM] 
    But there's no implementation for nlog yet.
    
    paulcbetts [9:00 AM] 
    Correct - you should be able to copy-paste the reactiveui-nlog version though
    
    paulcbetts [9:01 AM]
    Like, the code is exactly the same, it's just in a different assembly
    
    haacked [9:01 AM] 
    BTW, this.Log() is a static method. So it's effectively the same thing. :wink:
    
    haacked [9:04 AM]
    I guess the benefit is you don't have to define a static variable in every class, which is nice.
    
    haacked [9:04 AM]
    Does it somehow use the class defined by `this` to create the scope of the logger? So each class still gets its own?
    
    paulcbetts [9:04 AM] 
    Yeah
    
    paulcbetts [9:05 AM]
    That's the scam, is that the `this` is used to set the class name for the logger
    
    haacked [9:07 AM] 
    But I assume every call to `this.Log()` doesn't create a new logger. Instead, you look it up based on the class name in some concurrent dictionary?
    
    paulcbetts [9:08 AM] 
    It's stored in a MemoizedMRUCache as I recall
    
    paulcbetts [9:09 AM]
    Can't remember the details
    
    haacked [9:10 AM] 
    :cool: thanks!
