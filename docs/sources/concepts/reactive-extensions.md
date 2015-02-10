## What are Reactive Extensions?

Rx can be summarized in the following sentence which can also be read on the DevLabs homepage:

*Rx is a library for composing asynchronous and event-based programs using observable collections.*

Three core properties are reflected in here, all of which will be addressed throughout this lab:

* Asynchronous and event-based - As reflected in the title, the bread and butter of Rx&apos;s mission statement is to simplify those programming models. Everyone knows what stuck user interfaces look like, both on the Windows platform and on the web. And with the cloud around the corner, asynchrony becomes quintessential. Low-level technologies like .NET events, the asynchronous pattern, tasks, AJAX, etc. are often too hard.

* Composition - Combining asynchronous computations today is way too hard. It involves a lot of plumbing code that has little to nothing to do with the problem being solved. 
In particular, the data flow of the operations involved in the problem is not clear at all, and code gets spread out throughout event handlers, 
asynchronous callback procedures, and whatnot.

* Observable collections - By looking at asynchronous computations as data sources, we can leverage the active knowledge of LINQ&apos;s programming model. 
That&apos;s right: your mouse is a database of mouse moves and clicks. In the world of Rx, such asynchronous data sources are composed using various combinators 
in the LINQ sense, allowing things like filters, projections, joins, time-based operations, etc.

## Code Sample 

1. Create a new Console Application

2. Add the Reactive Extension using NuGet package manager
```
PM> Install-Package Rx-Main
```

3. Copy and paste this code
```cs
static void Main(string[] args)
{
    var blocker = new ManualResetEvent(false);
    IObservable<long> observable = Observable.Interval(TimeSpan.FromSeconds(1));
    var observer = observable
       .Take(3)
       .Subscribe(
           value => { Console.WriteLine("{0:T} {1}", DateTime.UtcNow, value); }, 
           exception => { Console.WriteLine(exception.Message); },
           () => {
               Console.WriteLine("Completed");
               blocker.Set();
           }
       );
    blocker.WaitOne(); 
}
```

In the first line we declare a simple blocking mechanism. Than we proceed with a defition of interval based Observable, which will fire an event every second. 
We follow with constructing an observer, instructing it to take only 3 published events, and during subscription, we specify what it needs to do with each value - display along with current time, 
encountered exception - print an error message, as well as upon completion of the sequence it is subscribing to - print completion message and release blocker defined in line one.


It will print
```
4:52:27 AM 0
4:52:28 AM 1
4:52:29 AM 2
Completed
```


### Learning resources

* [Becoming a C# Time Lord](http://channel9.msdn.com/Events/TechEd/Australia/2013/DEV422)

* [101 Rx Samples](http://rxwiki.wikidot.com/101samples)

* [Programming Reactive Extensions and LINQ](http://www.apress.com/programming-reactive-extensions-and-linq?gtmf=s)
