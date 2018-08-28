[![NuGet Stats](https://img.shields.io/nuget/v/reactiveui.svg)](https://www.nuget.org/packages/reactiveui) [![Build status](https://dotnetfoundation.visualstudio.com/_apis/public/build/definitions/a5852744-a77d-4d76-a9d2-81ac1fdd5744/11/badge)](https://dotnetfoundation.visualstudio.com/ReactiveUI/ReactiveUI%20Team/_build/index?definitionId=11)
 [![Coverage Status](https://coveralls.io/repos/github/reactiveui/ReactiveUI/badge.svg?branch=develop)](https://coveralls.io/github/reactiveui/ReactiveUI?branch=develop) [![#yourfirstpr](https://img.shields.io/badge/first--timers--only-friendly-blue.svg)](https://reactiveui.net/contribute) <!--[![Pull Request Stats](http://www.issuestats.com/github/reactiveui/reactiveui/badge/pr?style=flat)](http://www.issuestats.com/github/reactiveui/reactiveui)--> [![FOSSA Status](https://app.fossa.io/api/projects/git%2Bhttps%3A%2F%2Fgithub.com%2Freactiveui%2FReactiveUI.svg?type=shield)](https://app.fossa.io/projects/git%2Bhttps%3A%2F%2Fgithub.com%2Freactiveui%2FReactiveUI?ref=badge_shield)
<br>
<a href="https://www.nuget.org/packages/reactiveui">
        <img src="https://img.shields.io/nuget/dt/reactiveui-core.svg">
</a>
<a href="#backers">
        <img src="https://opencollective.com/reactiveui/backers/badge.svg">
</a>
<a href="#sponsors">
        <img src="https://opencollective.com/reactiveui/sponsors/badge.svg">
</a>
<a href="https://reactiveui.net/slack">
        <img src="https://img.shields.io/badge/chat-slack-blue.svg">
</a>
<br>
<br>
<a href="https://github.com/reactiveui/reactiveui">
  <img width="160" heigth="160" src="https://i.imgur.com/23kfbS9.png">
</a>
<br>
<h1>What is ReactiveUI?</h1>

<a href="https://reactiveui.net/">ReactiveUI</a> is a composable, cross-platform model-view-viewmodel framework for all .NET platforms that is inspired by functional reactive programming which is a paradigm that allows you to <a href="https://www.youtube.com/watch?v=3HwEytvngXk">abstract mutable state away from your user interfaces and express the idea around a feature in one readable place</a> and improve the testability of your application. 

<a href="https://reactiveui.net/docs/getting-started/">🔨 Get Started</a>, <a href="https://reactiveui.net/docs/getting-started/installation/nuget-packages/">🛍 Install Packages</a>, <a href="https://reactiveui.net/docs/resources/videos">🎞 Watch Videos</a>, <a href="https://reactiveui.net/docs/samples/">🎓 View Samples</a>, <a href="https://reactiveui.net/slack">🎤 Discuss ReactiveUI</a>

<h2>Introduction to Reactive Programming</h2>

Long ago, when computer programming first came to be, machines had to be programmed quite manually. If the technician entered the correct sequence of machine codes in the correct order, then the resulting program behavior would satisfy the business requirements. Instead of telling a computer how to do its job, which error-prone and relies too heavily on the infallibility of the programmer, why don't we just tell it what it's job is and let it figure the rest out?

ReactiveUI is inspired by the paradigm of Functional Reactive Programming, which allows you to model user input as a function that changes over time. This is super cool because it allows you to abstract mutable state away from your user interfaces and express the idea around a feature in one readable place whilst improving application testability. Reactive programming can look scary and complex at first glance, but the best way to describe reactive programming is to think of a spreadsheet:

![](https://reactiveui.net/docs/frp-excel.gif)

* Three cells, A, B, and C.
* C is defined as the sum of A and B.
* Whenever A or B changes, C reacts to update itself.

That's reactive programming: changes propagate throughout a system automatically. Welcome to the peanut butter and jelly of programming paradigms. For further information please watch the this video from the Xamarin Evolve conference - [Why You Should Be Building Better Mobile Apps with Reactive Programming](http://www.youtube.com/watch?v=DYEbUF4xs1Q) by Michael Stonis.

<h2>A Compelling Example</h2>

Let’s say you have a text field, and whenever the user types something into it, you want to make a network request which searches for that query.

![](http://i.giphy.com/xTka02wR2HiFOFACoE.gif)

```csharp
public interface ISearchViewModel
{
    string SearchQuery { get; set; }	 
    ReactiveCommand<string, List<SearchResult>> Search { get; }
    ObservableCollection<SearchResult> SearchResults { get; }
}
```

<h3>Define under what conditions a network request will be made</h3>

```csharp
// Here we're describing here, in a *declarative way*, the conditions in
// which the Search command is enabled.  Now our Command IsEnabled is
// perfectly efficient, because we're only updating the UI in the scenario
// when it should change.
var canSearch = this.WhenAnyValue(x => x.SearchQuery, query => !string.IsNullOrWhiteSpace(query));
```

<h3>Make the network connection</h3>

```csharp
// ReactiveCommand has built-in support for background operations and
// guarantees that this block will only run exactly once at a time, and
// that the CanExecute will auto-disable and that property IsExecuting will
// be set accordingly whilst it is running.
Search = ReactiveCommand.CreateFromTask(_ => searchService.Search(this.SearchQuery), canSearch);
```

<h3>Update the user interface</h3>

```csharp
// ReactiveCommands are themselves IObservables, whose value are the results
// from the async method, guaranteed to arrive on the UI thread. We're going
// to take the list of search results that the background operation loaded, 
// and them into our SearchResults.
Search.Subscribe(results => 
{
    SearchResults.Clear();
    SearchResults.AddRange(results);
});

```

<h3>Handling failures</h3>

```csharp
// ThrownExceptions is any exception thrown from the CreateAsyncTask piped
// to this Observable. Subscribing to this allows you to handle errors on
// the UI thread. 
Search.ThrownExceptions.Subscribe(error => 
{
    UserError.Throw("Potential Network Connectivity Error", error);
});
```

<h3>Throttling network requests and automatic search execution behaviour</h3>

```csharp
// Whenever the Search query changes, we're going to wait for one second
// of "dead airtime", then automatically invoke the subscribe command.
this.WhenAnyValue(x => x.SearchQuery)
    .Throttle(TimeSpan.FromSeconds(1), RxApp.MainThreadScheduler)
    .InvokeCommand(Search);
```

<h2>Support</h2>

If you have a question, please see if any discussions in our [GitHub issues](github.com/reactiveui/ReactiveUI/issues) or [Stack Overflow](https://stackoverflow.com/questions/tagged/reactiveui) have already answered it.

If you want to discuss something or just need help, here is our [Slack room](https://reactiveui.net/slack) where there are always individuals looking to help out!

If you are twitter savvy you can tweet #reactiveui with your question and someone should be able to reach out and help also.

If you have discovered a 🐜 or have a feature suggestion, feel free to create an issue on GitHub.

<h2>Contribute</h2>

ReactiveUI is developed under an OSI-approved open source license, making it freely usable and distributable, even for commercial use. Because of our Open Collective model for funding and transparency, we are able to funnel support and funds through to our contributors and community. We ❤ the people who are involved in this project, and we’d love to have you on board, especially if you are just getting started or have never contributed to open-source before.

So here's to you, lovely person who wants to join us — this is how you can support us:

* [Responding to questions on StackOverflow](https://stackoverflow.com/questions/tagged/reactiveui)
* [Passing on knowledge and teaching the next generation of developers](http://ericsink.com/entries/dont_use_rxui.html)
* [Donations](https://reactiveui.net/donate) and [Corporate Sponsorships](https://reactiveui.net/sponsorship)
* [Submitting documentation updates where you see fit or lacking](https://reactiveui.net/docs)
* [Making contributions to the code base](https://reactiveui.net/contribute/)
* [Asking your employer to reciprocate and contribute to open-source](https://github.com/github/balanced-employee-ip-agreement)

We're also looking for people to assist with code reviews of ReactiveUI contributions. If you're experienced with any of the below technologies, you can join the team and receive notifications:

 - [Android reviewers](https://github.com/orgs/reactiveui/teams/android-team)
 - [Apple TV reviewers](https://github.com/orgs/reactiveui/teams/tvos-team)
 - [Dot Net Core](https://github.com/orgs/reactiveui/teams/dotnetcore-team)
 - [Fody reviewers](https://github.com/orgs/reactiveui/teams/fody-team)
 - [iOS reviewers](https://github.com/orgs/reactiveui/teams/ios-team)
 - [Learning Team reviewers](https://github.com/orgs/reactiveui/teams/learning-team)
 - [Mac reviewers](https://github.com/orgs/reactiveui/teams/mac-team)
 - [ReactiveUI Core reviewers](https://github.com/orgs/reactiveui/teams/core-team)
 - [Tizen](https://github.com/orgs/reactiveui/teams/tizen-team)
 - [UWP reviewers](https://github.com/orgs/reactiveui/teams/uwp-team)
 - [Web Assembly](https://github.com/orgs/reactiveui/teams/webassembly-team)
 - [WinForms reviewers](https://github.com/orgs/reactiveui/teams/winforms-team)
 - [WPF reviewers](https://github.com/orgs/reactiveui/teams/wpf-team) 
 - [Xamarin Forms reviewers](https://github.com/orgs/reactiveui/teams/xamarin-forms-team)

<h2>.NET Foundation</h2>

ReactiveUI is part of the [.NET Foundation](https://www.dotnetfoundation.org/). Other projects that are associated with the foundation include the Microsoft .NET Compiler Platform ("Roslyn") as well as the Microsoft ASP.NET family of projects, Microsoft .NET Core & Xamarin Forms.

<h2>Core Team</h2>

<table>
  <tbody>
    <tr>
      <td align="center" valign="top">
        <img width="150" height="150" src="https://github.com/ghuntley.png?s=150">
        <br>
        <a href="https://github.com/ghuntley">Geoffrey Huntley</a>
        <p>Sydney, Australia</p>
      </td>
      <td align="center" valign="top">
        <img width="150" height="150" src="https://github.com/kentcb.png?s=150">
        <br>
        <a href="https://github.com/kentcb">Kent Boogaart</a>
        <p>Adelaide, Australia</p>
      </td>
      <td align="center" valign="top">
        <img width="150" height="150" src="https://github.com/olevett.png?s=150">
        <br>
        <a href="https://github.com/olevett">Olly Levett</a>
        <p>London, United Kingdom</p>
      </td>
      <td align="center" valign="top">
        <img width="150" height="150" src="https://github.com/paulcbetts.png?s=150">
        <br>
        <a href="https://github.com/paulcbetts">Paul Betts</a>
        <p>San Francisco, USA</p>
      </td>
      <td align="center" valign="top">
        <img width="150" height="150" src="https://github.com/shiftkey.png?s=150">
        <br>
        <a href="https://github.com/shiftkey">Brendan Forster</a>
        <p>Melbourne, Australia</p>
      </td>
      <td align="center" valign="top">
        <img width="150" height="150" src="https://github.com/onovotny.png?s=150">
        <br>
        <a href="https://github.com/onovotny">Oren Novotny</a>
        <p>New York, USA</p>
      </td>
      <td align="center" valign="top">
        <img width="150" height="150" src="https://github.com/glennawatson.png?s=150">
        <br>
        <a href="https://github.com/glennawatson">Glenn Watson</a>
        <p>Washington, USA</p>
      </td>
     </tr>
  </tbody>
</table>

<h2 align="center">Core Team</h2>

<h2 align="center">Learning Team</h2>

<h2 align="center">Android Team</h2>

<h2 align="center">Apple TV Team</h2>

<h2 align="center">Dot Net Core Team</h2>

<h2 align="center">Fody Team</h2>

<h2 align="center">iOS Team</h2>

<h2 align="center">Mac Team</h2>

<h2 align="center">Tizen Team</h2>

<h2 align="center">UWP Team</h2>

<h2 align="center">Web Assembly Team</h2>

<h2 align="center">WinForms Team</h2>

<h2 align="center">WPF Team</h2>

<h2 align="center">Xamarin Forms Team</h2>

<h2 align="center">Alumni</h2>

<h2 align="center">Sponsorship</h2>

The core team members, ReactiveUI contributors and contributors in the ecosystem do this open source work in their free time. If you use ReactiveUI a serious task, and you'd like us to invest more time on it, please donate. This project increases your income/productivity too. It makes development and applications faster and it reduces the required bandwidth.

This is how we use the donations:

* Allow the core team to work on ReactiveUI
* Thank contributors if they invested a large amount of time in contributing
* Support projects in the ecosystem that are of great value for users
* Support projects that are voted most (work in progress)
* Infrastructure cost
* Fees for money handling

<h2>Backers</h2>

[Become a backer](https://opencollective.com/reactiveui#backer) and get your image on our README on Github with a link to your site.

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

<h2>Sponsors</h2>

[Become a sponsor](https://opencollective.com/reactiveui#sponsor) and get your logo on our README on Github with a link to your site.

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
