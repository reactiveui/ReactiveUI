# ReactiveUI

Use the Reactive Extensions for .NET along with Silverlight, WPF, or 
Windows Phone to create elegant, testable User Interfaces.

This library is organized into several high-level assembly:

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
    set { this.RaiseAndSetIfChanged(value); }
  }

  byte _Green;
  public byte Green {
    get { return _Green; }
    set { this.RaiseAndSetIfChanged(value); }
  }

  byte _Blue;
  public byte Blue {
    get { return _Blue; }
    set { this.RaiseAndSetIfChanged(value); }
  }

  //
  // Declaring a Property that's based on an Observable
  // 

  ObservableAsPropertyHelper<Color> _Color;
  public Color Color {
    get { return _Color.Value; }
  }

  ReactiveCommand OkButton { get; protected set; }

  public ColorChooserThatDoesntLikeGreen()
  {
    var finalColor = this.WhenAny(x => x.Red, x => x.Green, x => x.Blue, 
        (r,g,b) => Color.FromRGB(r.Value, g.Value, b.Value));

    finalColor.ToProperty(this, x => x.Color);

    // When the finalColor has full green, the Ok button is disabled
    OkButton = new ReactiveCommand(finalColor.Select(x => x.Green != 255));
  }
}
```

## Learn more

For more information on how to use ReactiveUI, check out
[ReactiveUI](http://www.reactiveui.net).
