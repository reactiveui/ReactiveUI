# Reactive UI 101

Let's create a simple application demonstrating a number of ReactiveUI 
functionalities, without getting into too many under-the-hood details. 

We will create a WPF application, which will allow us to search through 
Flickr public images.   
The full code of the application is shown at the end of this chapter, 
and we will show relevant snippets as we go.

In Visual Studio create a new WPF application (Please note as of ReactiveUI v6.x .Net 4.5 or above is required).

Our view has been already created for us, the `MainWindow`, so we will proceed 
with creating our ViewModel.

**Add a new field**

```csharp
public AppViewModel ViewModel { get; private set;} 
```

**Then assign it a new value inside the MainWindow constructor**

```csharp
public MainWindow()
{
  ViewModel = new AppViewModel();
  InitializeComponent();
}  
```

**Create a new class "AppViewModel"**

```csharp
// AppViewModel is where we will describe the interaction of our application
// (we can describe the entire application in one class since this is very 
// small). 
public class AppViewModel : ReactiveObject
{
    // In ReactiveUI, this is the syntax to declare a read-write property
    // that will notify Observers (as well as WPF) that a property has 
    // changed. If we declared this as a normal property, we couldn't tell 
    // when it has changed!

    string _SearchTerm;
    public string SearchTerm
    {
        get { return _SearchTerm; }
        set { this.RaiseAndSetIfChanged(ref _SearchTerm, value); }
    }

    // We will describe this later, but ReactiveCommand is a Command
    // (like "Open", "Copy", "Delete", etc), that manages a task running
    // in the background.

    public ReactiveCommand<List<FlickrPhoto>> ExecuteSearch { get; protected set; }


    /* ObservableAsPropertyHelper
     * 
     * Here's the interesting part: In ReactiveUI, we can take IObservables
     * and "pipe" them to a Property - whenever the Observable yields a new
     * value, we will notify ReactiveObject that the property has changed.
     * 
     * To do this, we have a class called ObservableAsPropertyHelper - this
     * class subscribes to an Observable and stores a copy of the latest value.
     * It also runs an action whenever the property changes, usually calling
     * ReactiveObject's RaisePropertyChanged.
     */

    ObservableAsPropertyHelper<List<FlickrPhoto>> _SearchResults;
    public List<FlickrPhoto> SearchResults => _SearchResults.Value;

    // Here, we want to create a property to represent when the application 
    // is performing a search (i.e. when to show the "spinner" control that 
    // lets the user know that the app is busy). We also declare this property
    // to be the result of an Observable (i.e. its value is derived from 
    // some other property)

    ObservableAsPropertyHelper<Visibility> _SpinnerVisibility;
    public Visibility SpinnerVisibility => _SpinnerVisibility.Value;

    public AppViewModel()
    {
        ExecuteSearch = ReactiveCommand.CreateAsyncTask(parameter => GetSearchResultsFromFlickr(this.SearchTerm));

        /* Creating our UI declaratively
         * 
         * The Properties in this ViewModel are related to each other in different 
         * ways - with other frameworks, it is difficult to describe each relation
         * succinctly; the code to implement "The UI spinner spins while the search 
         * is live" usually ends up spread out over several event handlers.
         *
         * However, with RxUI, we can describe how properties are related in a very 
         * organized clear way. Let's describe the workflow of what the user does in
         * this application, in the order they do it.
         */

        // We're going to take a Property and turn it into an Observable here - this
        // Observable will yield a value every time the Search term changes (which in
        // the XAML, is connected to the TextBox). 
        //
        // We're going to use the Throttle operator to ignore changes that 
        // happen too quickly, since we don't want to issue a search for each 
        // key pressed! We then pull the Value of the change, then filter 
        // out changes that are identical, as well as strings that are empty.
        //
        // Finally, we use RxUI's InvokeCommand operator, which takes the String 
        // and calls the Execute method on the ExecuteSearch Command, after 
        // making sure the Command can be executed via calling CanExecute.
        this.WhenAnyValue(x => x.SearchTerm)
            .Throttle(TimeSpan.FromMilliseconds(800), RxApp.MainThreadScheduler)
            .Select(x => x.Trim())
            .DistinctUntilChanged()
            .Where(x => !String.IsNullOrWhiteSpace(x))
            .InvokeCommand(ExecuteSearch);

        // How would we describe when to show the spinner in English? We 
        // might say something like, "The spinner's visibility is whether
        // the search is running". RxUI lets us write these kinds of 
        // statements in code.
        //
        // ExecuteSearch has an IObservable<bool> called IsExecuting that
        // fires every time the command changes execution state. We Select() that into
        // a Visibility then we will use RxUI's
        // ToProperty operator, which is a helper to create an 
        // ObservableAsPropertyHelper object.

        _SpinnerVisibility = ExecuteSearch.IsExecuting.Select(x=> x ? Visibility.Visible : Visibility.Collapsed)                
            .ToProperty(this, x => x.SpinnerVisibility, Visibility.Hidden);

        // Here, we're going to actually describe what happens when the Command
        // gets invoked - we're going to run the GetSearchResultsFromFlickr every
        // time the Command is executed. 
        //
        // The important bit here is the return value - an Observable. We're going
        // to end up here with a Stream of FlickrPhoto Lists: every time someone 
        // calls Execute, we eventually end up with a new list which we then 
        // immediately put into the SearchResults property, that will then 
        // automatically fire INotifyPropertyChanged.
        _SearchResults = ExecuteSearch.ToProperty(this, x => x.SearchResults, new List<FlickrPhoto>());
    }

    public static async Task<List<FlickrPhoto>> GetSearchResultsFromFlickr(string searchTerm)
    {
        var doc = await Task.Run(() => XDocument.Load(String.Format(CultureInfo.InvariantCulture,
            "http://api.flickr.com/services/feeds/photos_public.gne?tags={0}&format=rss_200",
            HttpUtility.UrlEncode(searchTerm))));

        if (doc.Root == null)
            return null;

        var titles = doc.Root.Descendants("{http://search.yahoo.com/mrss/}title")
            .Select(x => x.Value);

        var tagRegex = new Regex("<[^>]+>", RegexOptions.IgnoreCase);
        var descriptions = doc.Root.Descendants("{http://search.yahoo.com/mrss/}description")
            .Select(x => tagRegex.Replace(HttpUtility.HtmlDecode(x.Value), ""));

        var items = titles.Zip(descriptions,
            (t, d) => new FlickrPhoto { Title = t, Description = d }).ToArray();

        var urls = doc.Root.Descendants("{http://search.yahoo.com/mrss/}thumbnail")
            .Select(x => x.Attributes("url").First().Value);

        var ret = items.Zip(urls, (item, url) => { item.Url = url; return item; }).ToList();
        return ret;
    }
}
```

The goal of the syntax of ReactiveUI for read-write properties is to notify 
Observers that a property has changed. 
Otherwise we would not be able to know when it was changed. 

The ExecuteSearch is basically an asynchronous task, executing in the background.
  
In cases when we don't need to provide for two-way binding between the View and the ViewModel, we can utilize
one of many ReactiveUI Helpers, to notify Observers of a changing read-only value in the ViewModel. We use the
ObservableAsPropertyHelper twice, once to turn a generic List<T> into an observable read-only collection,
and then to change the visibility of an indicator to show that a request is currently executing.

This also works in the opposite direction, when we take the `SearchTerm` property and turn it into an observable,
so every time a change occurs in the UI, we get notified. Using Reactive Extensions, we trottle those events,
and ensure that the search occurs no sooner than 800ms after the last keystroke. If the user did not change the
last value, or if the search term is blank, we ignore the event completely.

Using the `IsExecuting` observable of `ReactiveCommand`, we derive another 
observable to change the visibility of the "processing indicator".

The `GetSearchResultsFromFlickr` method gets invoked every time there is a 
throttled change in the UI, so let's define what should happen when a user executes a new search.

**Our ViewModel is now complete**

## MainWindow.xaml

```xml
﻿<Window x:Class="FlickrBrowser.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        x:Name="Window" Height="350" Width="525">
    <Window.Resources>
        <DataTemplate x:Key="PhotoDataTemplate">
            <Grid MaxHeight="100">
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="Auto" />
                    <ColumnDefinition Width="*" />
                </Grid.ColumnDefinitions>

                <Image Source="{Binding Url, IsAsync=True}" Margin="6" MaxWidth="128"
                       HorizontalAlignment="Center" VerticalAlignment="Center" />

                <StackPanel Grid.Column="1" Margin="6">
                    <TextBlock FontSize="14" FontWeight="Bold" Text="{Binding Title}" />
                    <TextBlock FontStyle="Italic" Text="{Binding Description}" 
                               TextWrapping="WrapWithOverflow" Margin="6" />
                </StackPanel>
            </Grid>
        </DataTemplate>
    </Window.Resources>

    <Grid DataContext="{Binding ViewModel, ElementName=Window}" Margin="12">
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="Auto" />
            <ColumnDefinition Width="*" />
            <ColumnDefinition Width="Auto" />
        </Grid.ColumnDefinitions>

        <Grid.RowDefinitions>
            <RowDefinition Height="Auto" />
            <RowDefinition Height="*" />
        </Grid.RowDefinitions>

        <TextBlock FontSize="16" FontWeight="Bold" VerticalAlignment="Center">Search For:</TextBlock>
        <TextBox Grid.Column="1" Margin="6,0,0,0" Text="{Binding SearchTerm, UpdateSourceTrigger=PropertyChanged}"/>
        <TextBlock Grid.Column="2" Margin="6,0,0,0" FontSize="16" FontWeight="Bold" Text="..." Visibility="{Binding SpinnerVisibility}" />

        <ListBox Grid.ColumnSpan="3" Grid.Row="1" Margin="0,6,0,0" 
                 ScrollViewer.HorizontalScrollBarVisibility="Disabled"
                 ItemsSource="{Binding SearchResults}" ItemTemplate="{DynamicResource PhotoDataTemplate}"  />
    </Grid>
</Window>
```

## MainWindow.xaml.cs (code behind)

 
```csharp
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows;
using System.Xml.Linq;
using ReactiveUI;

namespace FlickrBrowser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Since we're using the MVVM pattern, we're going to bind to our 
        // ViewModel object in the code, and our View code-behind will be
        // concerned only with things that are solely view-based, like 
        // minimizing/maximizing a window
        public AppViewModel ViewModel { get; protected set; }

        public MainWindow()
        {
            ViewModel = new AppViewModel();
            InitializeComponent();
        }
    }

    // Create a simple model class to store our Flickr results - since we will 
    // never update the properties once we create the object, we don't have to
    // use ReactiveObject, just good-old auto-properties.
    public class FlickrPhoto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
    }
}
```
 