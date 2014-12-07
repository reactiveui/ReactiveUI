# CreateDerivedCollection and derived lists

`ReactiveList.CreateDerivedCollection` is an extremely powerful method in MVVM
programming, which allows you to create a projection of a ReactiveList as
another list. `CreateDerivedCollection` allows you to do projection, ordering,
and filtering of a source ReactiveList.

Once a derived list has been created, this list will update dynamically based
on the source collection - as the source list has items added and removed, the
derived list will also update its contents. If `ChangeTrackingEnabled` is
enabled on the source list, changes to the source list will also cause updates
to the derived list.

For example, if you have a source list of MenuItems and you have a `filter`
method of `x => x.IsEnabled`, changing `IsEnabled` on any of the menu items
will hide or show the items in the list.  Note that the derived list is
read-only - you cannot manually modify the derived list.

### A canonical example - creating ViewModels from Models

```cs
public class TweetsListViewModel : ReactiveObject
{
    ReactiveList<Tweet> Tweets = new ReactiveList<Tweet>();
    IReactiveDerivedList<TweetTileViewModel> TweetTiles;

    public TweetsListViewModel()
    {
        TweetTiles = Tweets.CreateDerivedCollection(
            x => new TweetTileViewModel() { Model = x });

        // Adding a new item to Tweets, results in a new ViewModel showing
        // up in TweetTiles
        Tweets.Add(new Tweet() { Title = "Hello!", });
    }
}
```

### A more motivating example - filtering and ordering

`CreateDerivedCollection` can do some more interesting things, but to
illustrate it, we need a more interesting set of classes.

```cs
public class Tweet 
{
    public DateTime CreatedAt { get; set; }
}

public class TweetTileViewModel : ReactiveObject
{
    bool isHidden;
    public bool IsHidden {
        get { return isHidden; }
        set { this.RaiseAndSetIfChanged(ref isHidden, value); }
    }

    public Tweet Model { get; set; }
}
```

Now, let's connect up the filtering and ordering. It's important to note, that
since want to refilter the list based on the IsHidden value, it must be a
ReactiveObject and fire change notifications, or else CreateDerivedList has no
way to know when it should refilter.

Because we had to put `IsHidden` on the ViewModel, we need to set up two
levels of filtering. Models that are ReactiveObject would make this easier,
but is often not possible.

Let's take a look:

```cs
public class TweetsListViewModel : ReactiveObject
{
    ReactiveList<Tweet> Tweets = new ReactiveList<Tweet>();

    IReactiveDerivedList<TweetTileViewModel> TweetTiles;
    IReactiveDerivedList<TweetTileViewModel> VisibleTiles;

    public TweetsListViewModel()
    {
        TweetTiles = Tweets.CreateDerivedCollection(
            x => new TweetTileViewModel() { Model = x },
            x => true,
            (x, y) => x.CreatedAt.CompareTo(y.CreatedAt));

        VisibleTiles = TweetTiles.CreateDerivedCollection(
            x => x,
            x => !x.IsHidden);
    }
}
```

### Wrap it all up

CreateDerivedCollection allows us to declare how a list should be transformed
and keep up with the original list.
