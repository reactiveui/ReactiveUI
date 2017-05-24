using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using ReactiveUI;
using Xunit;

namespace ReactiveUI.Tests
{
    public class PropertyBindView : Control, IViewFor<PropertyBindViewModel>
    {
        public PropertyBindViewModel ViewModel {
            get { return (PropertyBindViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(PropertyBindViewModel), typeof(PropertyBindView), new PropertyMetadata(null));

        object IViewFor.ViewModel { 
            get { return ViewModel; }
            set { ViewModel = (PropertyBindViewModel)value; } 
        }
        
        public TextBox SomeTextBox;
        public TextBox Property2;
        public PropertyBindFakeControl FakeControl;
        public ListBox FakeItemsControl;

        public PropertyBindView()
        {
            SomeTextBox = new TextBox();
            Property2 = new TextBox();
            FakeControl = new PropertyBindFakeControl();
            FakeItemsControl = new ListBox();
        }
    }

    public class PropertyBindFakeControl : Control
    {
        public double? NullableDouble {
            get { return (double?)GetValue(NullableDoubleProperty); }
            set { SetValue(NullableDoubleProperty, value); }
        }
        public static readonly DependencyProperty NullableDoubleProperty =
            DependencyProperty.Register("NullableDouble", typeof(double?), typeof(PropertyBindFakeControl), new PropertyMetadata(null));

        public double JustADouble {
            get { return (double)GetValue(JustADoubleProperty); }
            set { SetValue(JustADoubleProperty, value); }
        }
        public static readonly DependencyProperty JustADoubleProperty =
            DependencyProperty.Register("JustADouble", typeof(double), typeof(PropertyBindFakeControl), new PropertyMetadata(0.0));

        public string NullHatingString {
            get { return (string)GetValue(NullHatingStringProperty); }
            set {
                if (value == null) throw new ArgumentNullException("No nulls! I get confused!");
                SetValue(NullHatingStringProperty, value); 
            }
        }
        public static readonly DependencyProperty NullHatingStringProperty =
            DependencyProperty.Register("NullHatingString", typeof(string), typeof(PropertyBindFakeControl), new PropertyMetadata(""));
    }
}
