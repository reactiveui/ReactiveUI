# ReactiveUI

Use the Reactive Extensions for .NET to create elegant, testable User 
Interfaces that run on any mobile or desktop platform.

### Supported Platforms

* Xamarin.iOS
* Xamarin.Android
* Xamarin.Mac
* WPF
* Windows Forms
* Windows Phone 8
* Windows Store Apps
* Universal Windows Platform (UWP)

This library is organized into several high-level assemblies:

- **ReactiveUI** - Core library that doesn't rely on any particular UI
  framework. `ReactiveObject`, the base ViewModel object, as well as
  `ReactiveCollection`, a more awesome ObservableCollection, is in here.

- **ReactiveUI.Xaml** - Classes that require references to a Xaml'ly
  framework, like WPF or WinRT. `ReactiveCommand`, an implementation of
  ICommand, as well as the UserError classes are in this assembly.

- **ReactiveUI.Blend** - This class has several Blend Behaviors and Triggers
  that make attaching ViewModel changes to Visual State Manager states.

- **ReactiveUI.Routing** - A screens and navigation framework as well as
  ViewModel locator. This framework helps you to write applications using IoC
  containers to locate views, as well as navigating back and forwards between
  views.

## A Compelling Example

```cs
public class ColorChooserThatDoesntLikeGreen : ReactiveObject
{
  //
  // Declaring a read/write property
  //

  byte _Red;
  public byte Red {
    get { return _Red; }
    set { this.RaiseAndSetIfChanged(ref _Red, value); }
  }

  byte _Green;
  public byte Green {
    get { return _Green; }
    set { this.RaiseAndSetIfChanged(ref _Green, value); }
  }

  byte _Blue;
  public byte Blue {
    get { return _Blue; }
    set { this.RaiseAndSetIfChanged(ref _Blue, value); }
  }

  //
  // Declaring a Property that's based on an Observable
  // 

  ObservableAsPropertyHelper<Color> _Color;
  public Color Color {
    get { return _Color.Value; }
  }

  public ReactiveCommand<object> OkButton { get; protected set; }

  public ColorChooserThatDoesntLikeGreen()
  {
    var finalColor = this.WhenAny(x => x.Red, x => x.Green, x => x.Blue,
      (r,g,b) => Color.FromArgb(r.Value, g.Value, b.Value));

    _Color = finalColor.ToProperty(this, x => x.Color);

    // When the finalColor has full green, the Ok button is disabled
    OkButton = ReactiveCommand.Create(finalColor.Select(x => x.G != 255));
  }
}
```

## Learn more

For more information on how to use ReactiveUI, check out
[ReactiveUI](http://www.reactiveui.net).
