using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Windows;
using Android;
using Android.App;
using Android.Widget;
using Android.Content;
using Xunit;
using ReactiveUI;

namespace ReactiveUI.Tests
{
    public class PropertyBindView : ReactiveFragment<PropertyBindViewModel>
    {
        public TextView SomeTextBox;
        public TextView Property2;
        public PropertyBindFakeControl FakeControl;

        public PropertyBindView()
        {
            SomeTextBox = new TextView(Application.Context);
            Property2 = new TextView(Application.Context);
            FakeControl = new PropertyBindFakeControl();
        }
    }

    public class PropertyBindFakeControl : ReactiveFragment, INotifyPropertyChanged
    {
        double? _NullableDouble;
        public double? NullableDouble {
            get { return _NullableDouble; }
            set { this.RaiseAndSetIfChanged(ref _NullableDouble, value); }
        }

        double _JustADouble;
        public double JustADouble {
            get { return _JustADouble; }
            set { this.RaiseAndSetIfChanged(ref _JustADouble, value); }
        }

        string _NullHatingString = "";
        public string NullHatingString {
            get { return _NullHatingString; }
            set {
                if (value == null) throw new ArgumentNullException("No nulls! I get confused!");
                this.RaiseAndSetIfChanged(ref _NullHatingString, value);
            }
        }
    }
}
