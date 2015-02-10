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

### Learning resources

* [Becoming a C# Time Lord](http://channel9.msdn.com/Events/TechEd/Australia/2013/DEV422)

* [101 Rx Sample](http://rxwiki.wikidot.com/101samples)

* [Programming Reactive Extensions and LINQ](http://www.apress.com/programming-reactive-extensions-and-linq?gtmf=s)
