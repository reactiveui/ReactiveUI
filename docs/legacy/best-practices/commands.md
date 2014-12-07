Prefer binding user interactions to commands rather than methods.

# Do

```cs
// In XAML
<Button Command="{Binding Delete}" .../>

public class RepositoryViewModel : ReactiveObject
{
    public RepositoryViewModel() 
    {
      Delete = ReactiveCommand.CreateAsyncObservable(x => DeleteImpl());
      Delete.ThrownExceptions.Subscribe(ex => /*...*/);
    }

    public ReactiveAsyncCommand Delete { get; private set; }

    public IObservable<Unit> DeleteImpl() {...}
}
```
# Don't

Use the Caliburn.Micro conventions for associating buttons and commands:

```
// In XAML
    <Button x:Name="Delete" .../>

    public class RepositoryViewModel : PropertyChangedBase
    {
      public void Delete() {...}	
    }
```

# Why? 
- ReactiveCommand exposes the `CanExecute` property of the command to enable applications to introduce additional behaviour.
- It handles marshaling the result back to the UI thread.
- It tracks in-flight items.