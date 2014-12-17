```cs
/// <summary>
/// It's usually a good idea to create an interface for every ViewModel and
/// reference that instead of the implementation. This makes creating fake
/// versions or design-time versions of ViewModels much easier.
/// </summary>
public interface ITeamLoginTitleViewModel
{
    [IgnoreDataMember]
    ReactiveCommand<Object> LoginToThisTeam { get; }

    [DataMember]
    TeamWithUser Model { get; }
}


public class TeamLoginTitleViewModel : ReactiveObject, ITeamLoginTitleViewModel
{

}
```