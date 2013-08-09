using System;
using ReactiveUI;
using System.Collections.Generic;
using MonoTouch.UIKit;
using System.Linq;
using MonoTouch.Foundation;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Reactive.Disposables;

namespace ReactiveUI.Cocoa
{
    public class UIKitObservableForProperty : UIKitObservableForPropertyBase
    {
        public static Lazy<UIKitObservableForProperty> Instance = new Lazy<UIKitObservableForProperty>();

        public UIKitObservableForProperty ()
        {
            Register(typeof(UIControl), "Value", 20, (s, p)=> ObservableFromUIControlEvent(s, p, UIControlEvent.ValueChanged));
            Register(typeof(UITextField), "Text", 30, (s, p) => ObservableFromNotification(s, p, UITextField.TextFieldTextDidChangeNotification));
            Register(typeof(UIDatePicker), "Date", 30, (s, p)=> ObservableFromUIControlEvent(s, p, UIControlEvent.ValueChanged));
            Register(typeof(UISegmentedControl), "SelectedSegment", 30, (s, p)=> ObservableFromUIControlEvent(s, p, UIControlEvent.ValueChanged));
            Register(typeof(UISwitch), "On", 30, (s, p)=> ObservableFromUIControlEvent(s, p, UIControlEvent.ValueChanged));
            Register(typeof(UISegmentedControl), "SelectedSegment", 30, (s, p)=> ObservableFromUIControlEvent(s, p, UIControlEvent.ValueChanged));
            
            // Warning: This will stomp the Control's delegate
            Register(typeof(UITabBar), "SelectedItem", 30, (s, p) => ObservableFromEvent(s, p, "ItemSelected"));

            // Warning: This will stomp the Control's delegate
            Register(typeof(UISearchBar), "Text", 30, (s, p) => ObservableFromEvent(s, p, "TextChanged"));
        }
    }
}

