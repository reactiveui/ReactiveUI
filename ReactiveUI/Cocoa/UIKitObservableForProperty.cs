using System;
using ReactiveUI;
using System.Collections.Generic;
using MonoTouch.UIKit;
using System.Linq;
using MonoTouch.Foundation;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Reactive.Disposables;

namespace ReactiveUI
{
    public class UIKitObservableForProperty : UIKitObservableForPropertyBase
    {
        public static Lazy<UIKitObservableForProperty> Instance = new Lazy<UIKitObservableForProperty>();

        public UIKitObservableForProperty ()
        {
            //TODO
            //Register(typeof(UIControl), "Value", 20, (s, p)=> ObservableFromUIControlEvent(s, p, UIControlEvent.ValueChanged, ns => ((UIControl)ns).Value));
            Register(typeof(UITextField), "Text", 30, (s, p) => ObservableFromNotification(s, p, UITextField.TextFieldTextDidChangeNotification, ns => ((UITextField)ns).Text));
            Register(typeof(UITextView), "Text", 30, (s, p) => ObservableFromNotification(s, p, UITextView.TextDidChangeNotification, ns => ((UITextView)ns).Text));
            Register(typeof(UIDatePicker), "Date", 30, (s, p)=> ObservableFromUIControlEvent(s, p, UIControlEvent.ValueChanged, ns => ((UIDatePicker)ns).Date));
            Register(typeof(UISegmentedControl), "SelectedSegment", 30, (s, p)=> ObservableFromUIControlEvent(s, p, UIControlEvent.ValueChanged, ns => ((UISegmentedControl)ns).SelectedSegment));
            Register(typeof(UISwitch), "On", 30, (s, p)=> ObservableFromUIControlEvent(s, p, UIControlEvent.ValueChanged, ns => ((UISwitch)ns).On));
            Register(typeof(UISegmentedControl), "SelectedSegment", 30, (s, p)=> ObservableFromUIControlEvent(s, p, UIControlEvent.ValueChanged, ns => ((UISegmentedControl)ns).SelectedSegment));
            
            // Warning: This will stomp the Control's delegate
            Register(typeof(UITabBar), "SelectedItem", 30, (s, p) => ObservableFromEvent(s, p, "ItemSelected", ns => ((UITabBar)ns).SelectedItem));

            // Warning: This will stomp the Control's delegate
            Register(typeof(UISearchBar), "Text", 30, (s, p) => ObservableFromEvent(s, p, "TextChanged", ns => ((UISearchBar)ns).Text));
        }
    }
}

