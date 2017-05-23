# ![ReactiveUI Logo](https://i.imgur.com/23kfbS9.png) ReactiveUI

  [![NuGet Stats](https://img.shields.io/nuget/v/reactiveui.svg)](https://www.nuget.org/packages/reactiveui) [![#yourfirstpr](https://img.shields.io/badge/first--timers--only-friendly-blue.svg)](https://github.com/reactiveui/ReactiveUI/issues?utf8=%E2%9C%93&q=label%3Afirst-timers-only+) [![Pull Request Stats](http://www.issuestats.com/github/reactiveui/reactiveui/badge/pr?style=flat)](http://www.issuestats.com/github/reactiveui/reactiveui) [![Build status](https://ci.appveyor.com/api/projects/status/nhsndp7v788t6a2m/branch/develop?svg=true)](https://ci.appveyor.com/project/ghuntley/reactiveui/branch/develop) [![Coverage Status](https://coveralls.io/repos/github/reactiveui/ReactiveUI/badge.svg?branch=develop)](https://coveralls.io/github/reactiveui/ReactiveUI?branch=develop) [![OpenCollective](https://opencollective.com/reactiveui/backers/badge.svg)](#backers) 
[![OpenCollective](https://opencollective.com/reactiveui/sponsors/badge.svg)](#sponsors)

Use the Reactive Extensions for .NET to create elegant, testable User Interfaces that run on any mobile or desktop platform. Supports Xamarin.iOS, Xamarin.Android, Xamarin.Mac, Xamarin Forms, WPF, Windows Forms, Windows Phone 8, Windows Store and Universal Windows Platform (UWP).

If you’re already familiar with [functional reactive programming](http://docs.reactiveui.net/en/fundamentals/history.html) or what ReactiveUI is about, check out the [documentation](http://docs.reactiveui.net/en/index.html) for more in-depth information about how it all works or our comprehensive [collection of samples](https://github.com/reactiveui/samples).

If you have a question, please see if any discussions in our [GitHub issues](github.com/reactiveui/ReactiveUI/issues) or [Stack Overflow](https://stackoverflow.com/questions/tagged/reactiveui) have already answered it. If not, please [feel free to file your own](https://github.com/reactiveui/ReactiveUI/issues/new)! 

We have our very own [Slack organization](https://reactivex.slack.com/) which contains some of the best user interface/reactive extension developers in the industry. All software engineers, young and old, regardless of experience are welcome to join our campfire but you'll need to send an email to [hello@reactiveui.net](mailto:hello@reactiveui.net) with the email address you'd like to be invited, and we'll send you an invite. Sit tight, it's worth it.
#### Table of Contents
* [Introduction](#introduction)
* [Fundamentals](#fundamentals)
* [A Compelling Example](#a-compelling-example)
* [Slack](#slack)
* [Support](#support)
* [Contribute](#contribute)
* [Showcase](#showcase)
* [Licensing](#licensing)
* [Sponsorship](#sponsorship)

## Introduction

ReactiveUI is inspired by [functional reactive programming](https://en.wikipedia.org/wiki/Functional_reactive_programming) and is the father of the [ReactiveCocoa](https://github.com/ReactiveCocoa/ReactiveCocoa) (Cocoa/Swift) framework. Rather than using mutable variables which are replaced and modified in-place, ReactiveUI offers "event streams", represented by the `IObserver` and `IObservable` types, that send values over time.

If you are new to these concepts then we highly recommend watching the following videos before progressing too far:

* [Rx Under the Hood (Video #1)](https://channel9.msdn.com/Shows/Going+Deep/E2E-Erik-Meijer-and-Wes-Dyer-Reactive-Framework-Rx-Under-the-Hood-1-of-2) by Erik Meijer (Inventor of Reactive Extensions/Microsoft Research)
* [Rx Under the Hood (Video #2)](https://channel9.msdn.com/Shows/Going+Deep/E2E-Erik-Meijer-and-Wes-Dyer-Reactive-Framework-Rx-Under-the-Hood-2-of-2) by Erik Meijer (Inventor of Reactive Extensions/Microsoft Research)
* [Controlling Time and Space: understanding the many formulations of FRP](https://www.youtube.com/watch?v=Agu6jipKfYw) by Evan Czaplicki (Elm language designer/Prezi)
* [FRP In Practice: Taking a look at Reactive[UI/Cocoa]](https://www.youtube.com/watch?v=1XNATGjqM6U) by Paul Betts (Slack/GitHub)
* [ReactiveUI - It's pretty neat](https://www.youtube.com/watch?v=HPyKHxy7X0w) by Brendan Forster (GitHub)
* [ReactiveUI - Turning MVVM up to 11](https://vimeo.com/97329155) by Brendan Forster (GitHub)
* [Let Me Tell You About Our Lord And Saviour FRP](https://www.youtube.com/watch?v=0_LX3zT4m9I) by Brendan Forster (GitHub)

## Fundamentals 

One of the most confusing aspects of the Reactive Extensions is that of ["hot" and "cold" observables](http://www.introtorx.com/content/v1.0.10621.0/14_HotAndColdObservables.html) (event streams). In short, given just a method or function declaration like this:

	IObservable<string> Search(string query)

It is impossible to tell whether subscribing to (observing) that `IObservable` will involve side effects. If it does involve side effects, it’s also impossible to tell whether each subscription has a side effect, or if only the first one does. Whilst this example is contrived, it demonstrates a real, pervasive problem that makes it harder  at first for newcomers to understand Rx code at first glance. 

As such we also recommend [watching this video](https://www.youtube.com/watch?v=IDy21J75eyU), reading [our documentation](http://docs.reactiveui.net/en/fundamentals/reactive-extensions.html) and [playing with the marbles](http://rxmarbles.com/) to familiarize yourself with the fundamentals.


## A Compelling Example

Let’s say you have a text field, and whenever the user types something into it, you want to make a network request which searches for that query.


![](http://i.giphy.com/xTka02wR2HiFOFACoE.gif)

```csharp
public interface ISearchViewModel
{
    ReactiveList<SearchResults> SearchResults { get; }
    string SearchQuery { get; }	 
    ReactiveCommand<string, List<SearchResults>> Search { get; }
    ISearchService SearchService { get; }
}
```
#### Define under what conditions a network request will be made

```csharp
// Here we're describing here, in a *declarative way*, the conditions in
// which the Search command is enabled.  Now our Command IsEnabled is
// perfectly efficient, because we're only updating the UI in the scenario
// when it should change.
var canSearch = this.WhenAny(x => x.SearchQuery, x => !String.IsNullOrWhiteSpace(x.Value));
```
#### Make the network connection
```csharp
// ReactiveCommand has built-in support for background operations and
// guarantees that this block will only run exactly once at a time, and
// that the CanExecute will auto-disable and that property IsExecuting will
// be set accordingly whilst it is running.
Search = ReactiveCommand.CreateFromTask<string, List<SearchResults>>(_ => 
        searchService.Search(this.SearchQuery), canSearch);
```

#### Update the user interface 
```csharp
// ReactiveCommands are themselves IObservables, whose value are the results
// from the async method, guaranteed to arrive on the UI thread. We're going
// to take the list of search results that the background operation loaded, 
// and them into our SearchResults.
Search.Subscribe(results => {
    SearchResults.Clear();
    SearchResults.AddRange(results);
});

```
#### Handling failures
```csharp
// ThrownExceptions is any exception thrown from the CreateAsyncTask piped
// to this Observable. Subscribing to this allows you to handle errors on
// the UI thread. 
Search.ThrownExceptions
    .Subscribe(ex => {
        UserError.Throw("Potential Network Connectivity Error", ex);
    });
```
#### Throttling network requests and automatic search execution behaviour
```csharp
// Whenever the Search query changes, we're going to wait for one second
// of "dead airtime", then automatically invoke the subscribe command.
this.WhenAnyValue(x => x.SearchQuery)
    .Throttle(TimeSpan.FromSeconds(1), RxApp.MainThreadScheduler)
    .InvokeCommand(Search);
```

## Slack

We have our very own [Slack organization](https://reactivex.slack.com/) which contains some of the best user interface/reactive extension developers in the industry. All software engineers, young and old, regardless of experience are welcome to join our campfire but you'll need to send an email to [hello@reactiveui.net](mailto:hello@reactiveui.net) with the Email address you'd like to be invited, and we'll send you an invite. Sit tight, it's worth it.

## Support

ReactiveUI is an open source project that is community supported by people just like you. We keep [a bunch of curated tasks specifically for new contributors](https://github.com/reactiveui/reactiveui/labels/up-for-grabs) which are [a great way to get started with open source](https://yourfirstpr.github.io/). They also provide a fantastic avenue for getting to know the ReactiveUI maintainers.

If you have a question, please see if any discussions in our [GitHub issues](github.com/reactiveui/ReactiveUI/issues) or [Stack Overflow](https://stackoverflow.com/questions/tagged/reactiveui) have already answered it. If not, please [feel free to file your own](https://github.com/reactiveui/ReactiveUI/issues/new)! 


## Contribute

Here are some pointers for anyone looking for mini-features and work items that would make a positive contribution to ReactiveUI.

* Let us know if (and how) [you are using ReactiveUI in production](https://github.com/reactiveui/ReactiveUI/issues/979).
* We keep [a bunch of curated tasks specifically for new contributors](https://github.com/reactiveui/reactiveui/labels/up-for-grabs), which are a great way to get started with open source and provide a fantastic avenue for getting to know the ReactiveUI maintainers.
* Write a blog post about `#ReactiveUI` and then [tweet the link to our twitter account](https://twitter.com/ReactiveXUI). We will retweet you.
* Contribute [a repro case](https://quaid.fedorapeople.org/TOS/Practical_Open_Source_Software_Exploration/html/sn-Debugging_the_Code-The_Anatomy_of_a_Good_Bug_Report.html) or [help resolve known issues](https://github.com/reactiveui/ReactiveUI/issues?q=is%3Aopen).
* Help flesh out and [improve our documentation](https://github.com/reactiveui/ReactiveUI/pull/771) by providing [content writing](https://jacobian.org/writing/what-to-write/), [structure enforcement](https://jacobian.org/writing/technical-style/) or [editing](https://jacobian.org/writing/editors/) services.

We try not to be too OCD about coding style wars, but we do [have our own convention](https://github.com/reactiveui/ReactiveUI/blob/master/CONTRIBUTING.md) and [best design practices documented](https://github.com/reactiveui/rxui-design-guidelines) - please respect them and your pull-request experience will be much smoother. If you are using Visual Studio, please [install the rebracer plugin](https://visualstudiogallery.msdn.microsoft.com/410e9b9f-65f3-4495-b68e-15567e543c58) which will automatically apply the correct source formatting settings.

We're also looking for people to assist with code reviews of ReactiveUI contributions. If you're experienced with any of the below technologies, you can join the team and receive notifications:

 - [Android reviewers](https://github.com/orgs/reactiveui/teams/reviewers-android)
 - [Core reviewers](https://github.com/orgs/reactiveui/teams/reviewers-core)
 - [iOS reviewers](https://github.com/orgs/reactiveui/teams/reviewers-ios)
 - [Mac reviewers](https://github.com/orgs/reactiveui/teams/reviewers-mac)
 - [UWP reviewers](https://github.com/orgs/reactiveui/teams/reviewers-uwp)
 - [WinForms reviewers](https://github.com/orgs/reactiveui/teams/reviewers-winforms)
 - [WPF reviewers](https://github.com/orgs/reactiveui/teams/reviewers-wpf) 
 - [Xamarin Forms reviewers](https://github.com/orgs/reactiveui/teams/reviewers-xf)

## Showcase
We encourage our community to [showcase where and how they have used ReactiveUI in their applications](https://github.com/reactiveui/ReactiveUI/issues/979), some members have even gone as far as open-sourcing their app and [sharing their entire codebase](https://github.com/reactiveui/ReactiveUI/issues/687#issuecomment-166772487). You are of course under no-obligation share these insights (or code) with us but it is greatly appreciated by the project maintainers and you'll usually get a [retweet out of it](https://twitter.com/ReactiveXUI/status/679532005155966977).

## Licensing

The ReactiveUI project is licensed under the [MS-PL license](http://opensource.org/licenses/ms-pl.html).

## Sponsorship

Through contributions, donations, and sponsorship, you allow ReactiveUI to thrive. Your donations directly support meetups, office hours, continued enhancements, and most importantly, great documentation and learning material!

### Backers 

[[Become a backer](https://opencollective.com/reactiveui#backer)]

<a href="https://opencollective.com/reactiveui/backer/0/website" target="_blank"><img src="https://opencollective.com/reactiveui/backer/0/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/backer/1/website" target="_blank"><img src="https://opencollective.com/reactiveui/backer/1/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/backer/2/website" target="_blank"><img src="https://opencollective.com/reactiveui/backer/2/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/backer/3/website" target="_blank"><img src="https://opencollective.com/reactiveui/backer/3/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/backer/4/website" target="_blank"><img src="https://opencollective.com/reactiveui/backer/4/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/backer/5/website" target="_blank"><img src="https://opencollective.com/reactiveui/backer/5/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/backer/6/website" target="_blank"><img src="https://opencollective.com/reactiveui/backer/6/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/backer/7/website" target="_blank"><img src="https://opencollective.com/reactiveui/backer/7/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/backer/8/website" target="_blank"><img src="https://opencollective.com/reactiveui/backer/8/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/backer/9/website" target="_blank"><img src="https://opencollective.com/reactiveui/backer/9/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/backer/10/website" target="_blank"><img src="https://opencollective.com/reactiveui/backer/10/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/backer/11/website" target="_blank"><img src="https://opencollective.com/reactiveui/backer/11/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/backer/12/website" target="_blank"><img src="https://opencollective.com/reactiveui/backer/12/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/backer/13/website" target="_blank"><img src="https://opencollective.com/reactiveui/backer/13/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/backer/14/website" target="_blank"><img src="https://opencollective.com/reactiveui/backer/14/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/backer/15/website" target="_blank"><img src="https://opencollective.com/reactiveui/backer/15/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/backer/16/website" target="_blank"><img src="https://opencollective.com/reactiveui/backer/16/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/backer/17/website" target="_blank"><img src="https://opencollective.com/reactiveui/backer/17/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/backer/18/website" target="_blank"><img src="https://opencollective.com/reactiveui/backer/18/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/backer/19/website" target="_blank"><img src="https://opencollective.com/reactiveui/backer/19/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/backer/20/website" target="_blank"><img src="https://opencollective.com/reactiveui/backer/20/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/backer/21/website" target="_blank"><img src="https://opencollective.com/reactiveui/backer/21/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/backer/22/website" target="_blank"><img src="https://opencollective.com/reactiveui/backer/22/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/backer/23/website" target="_blank"><img src="https://opencollective.com/reactiveui/backer/23/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/backer/24/website" target="_blank"><img src="https://opencollective.com/reactiveui/backer/24/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/backer/25/website" target="_blank"><img src="https://opencollective.com/reactiveui/backer/25/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/backer/26/website" target="_blank"><img src="https://opencollective.com/reactiveui/backer/26/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/backer/27/website" target="_blank"><img src="https://opencollective.com/reactiveui/backer/27/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/backer/28/website" target="_blank"><img src="https://opencollective.com/reactiveui/backer/28/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/backer/29/website" target="_blank"><img src="https://opencollective.com/reactiveui/backer/29/avatar.svg"></a>

### Sponsors
 [[Become a sponsor](https://opencollective.com/reactiveui#sponsor)]

<a href="https://opencollective.com/reactiveui/sponsor/0/website" target="_blank"><img src="https://opencollective.com/reactiveui/sponsor/0/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/sponsor/1/website" target="_blank"><img src="https://opencollective.com/reactiveui/sponsor/1/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/sponsor/2/website" target="_blank"><img src="https://opencollective.com/reactiveui/sponsor/2/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/sponsor/3/website" target="_blank"><img src="https://opencollective.com/reactiveui/sponsor/3/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/sponsor/4/website" target="_blank"><img src="https://opencollective.com/reactiveui/sponsor/4/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/sponsor/5/website" target="_blank"><img src="https://opencollective.com/reactiveui/sponsor/5/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/sponsor/6/website" target="_blank"><img src="https://opencollective.com/reactiveui/sponsor/6/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/sponsor/7/website" target="_blank"><img src="https://opencollective.com/reactiveui/sponsor/7/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/sponsor/8/website" target="_blank"><img src="https://opencollective.com/reactiveui/sponsor/8/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/sponsor/9/website" target="_blank"><img src="https://opencollective.com/reactiveui/sponsor/9/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/sponsor/10/website" target="_blank"><img src="https://opencollective.com/reactiveui/sponsor/10/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/sponsor/11/website" target="_blank"><img src="https://opencollective.com/reactiveui/sponsor/11/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/sponsor/12/website" target="_blank"><img src="https://opencollective.com/reactiveui/sponsor/12/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/sponsor/13/website" target="_blank"><img src="https://opencollective.com/reactiveui/sponsor/13/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/sponsor/14/website" target="_blank"><img src="https://opencollective.com/reactiveui/sponsor/14/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/sponsor/15/website" target="_blank"><img src="https://opencollective.com/reactiveui/sponsor/15/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/sponsor/16/website" target="_blank"><img src="https://opencollective.com/reactiveui/sponsor/16/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/sponsor/17/website" target="_blank"><img src="https://opencollective.com/reactiveui/sponsor/17/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/sponsor/18/website" target="_blank"><img src="https://opencollective.com/reactiveui/sponsor/18/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/sponsor/19/website" target="_blank"><img src="https://opencollective.com/reactiveui/sponsor/19/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/sponsor/20/website" target="_blank"><img src="https://opencollective.com/reactiveui/sponsor/20/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/sponsor/21/website" target="_blank"><img src="https://opencollective.com/reactiveui/sponsor/21/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/sponsor/22/website" target="_blank"><img src="https://opencollective.com/reactiveui/sponsor/22/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/sponsor/23/website" target="_blank"><img src="https://opencollective.com/reactiveui/sponsor/23/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/sponsor/24/website" target="_blank"><img src="https://opencollective.com/reactiveui/sponsor/24/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/sponsor/25/website" target="_blank"><img src="https://opencollective.com/reactiveui/sponsor/25/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/sponsor/26/website" target="_blank"><img src="https://opencollective.com/reactiveui/sponsor/26/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/sponsor/27/website" target="_blank"><img src="https://opencollective.com/reactiveui/sponsor/27/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/sponsor/28/website" target="_blank"><img src="https://opencollective.com/reactiveui/sponsor/28/avatar.svg"></a>
<a href="https://opencollective.com/reactiveui/sponsor/29/website" target="_blank"><img src="https://opencollective.com/reactiveui/sponsor/29/avatar.svg"></a>
