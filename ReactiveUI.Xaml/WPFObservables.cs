using Microsoft.Win32;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Packaging;
using System.Reactive;
using System.Reactive.Linq;
using System.Security.Permissions;
using System.Security.RightsManagement;
using System.Windows;
using System.Windows.Annotations;
using System.Windows.Annotations.Storage;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Baml2006;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Converters;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Documents.DocumentStructures;
using System.Windows.Documents.Serialization;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Input.StylusPlugIns;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Markup.Localizer;
using System.Windows.Markup.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Converters;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.Media3D.Converters;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Shell;
using System.Windows.Threading;

namespace System.Windows.Controls {
    public static class ObservableEventsMixin {

 
////////////////////////////////////////////
////////////////////////////////////////////
////   IInputElement
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<MouseButtonEventArgs>> PreviewMouseLeftButtonDownObserver(this IInputElement This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.PreviewMouseLeftButtonDown += h, h => This.PreviewMouseLeftButtonDown -= h);
        }

        public static IObservable<EventPattern<MouseButtonEventArgs>> MouseLeftButtonDownObserver(this IInputElement This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.MouseLeftButtonDown += h, h => This.MouseLeftButtonDown -= h);
        }

        public static IObservable<EventPattern<MouseButtonEventArgs>> PreviewMouseLeftButtonUpObserver(this IInputElement This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.PreviewMouseLeftButtonUp += h, h => This.PreviewMouseLeftButtonUp -= h);
        }

        public static IObservable<EventPattern<MouseButtonEventArgs>> MouseLeftButtonUpObserver(this IInputElement This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.MouseLeftButtonUp += h, h => This.MouseLeftButtonUp -= h);
        }

        public static IObservable<EventPattern<MouseButtonEventArgs>> PreviewMouseRightButtonDownObserver(this IInputElement This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.PreviewMouseRightButtonDown += h, h => This.PreviewMouseRightButtonDown -= h);
        }

        public static IObservable<EventPattern<MouseButtonEventArgs>> MouseRightButtonDownObserver(this IInputElement This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.MouseRightButtonDown += h, h => This.MouseRightButtonDown -= h);
        }

        public static IObservable<EventPattern<MouseButtonEventArgs>> PreviewMouseRightButtonUpObserver(this IInputElement This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.PreviewMouseRightButtonUp += h, h => This.PreviewMouseRightButtonUp -= h);
        }

        public static IObservable<EventPattern<MouseButtonEventArgs>> MouseRightButtonUpObserver(this IInputElement This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.MouseRightButtonUp += h, h => This.MouseRightButtonUp -= h);
        }

        public static IObservable<EventPattern<MouseEventArgs>> PreviewMouseMoveObserver(this IInputElement This){
            return Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(h => This.PreviewMouseMove += h, h => This.PreviewMouseMove -= h);
        }

        public static IObservable<EventPattern<MouseEventArgs>> MouseMoveObserver(this IInputElement This){
            return Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(h => This.MouseMove += h, h => This.MouseMove -= h);
        }

        public static IObservable<EventPattern<MouseWheelEventArgs>> PreviewMouseWheelObserver(this IInputElement This){
            return Observable.FromEventPattern<MouseWheelEventHandler, MouseWheelEventArgs>(h => This.PreviewMouseWheel += h, h => This.PreviewMouseWheel -= h);
        }

        public static IObservable<EventPattern<MouseWheelEventArgs>> MouseWheelObserver(this IInputElement This){
            return Observable.FromEventPattern<MouseWheelEventHandler, MouseWheelEventArgs>(h => This.MouseWheel += h, h => This.MouseWheel -= h);
        }

        public static IObservable<EventPattern<MouseEventArgs>> MouseEnterObserver(this IInputElement This){
            return Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(h => This.MouseEnter += h, h => This.MouseEnter -= h);
        }

        public static IObservable<EventPattern<MouseEventArgs>> MouseLeaveObserver(this IInputElement This){
            return Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(h => This.MouseLeave += h, h => This.MouseLeave -= h);
        }

        public static IObservable<EventPattern<MouseEventArgs>> GotMouseCaptureObserver(this IInputElement This){
            return Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(h => This.GotMouseCapture += h, h => This.GotMouseCapture -= h);
        }

        public static IObservable<EventPattern<MouseEventArgs>> LostMouseCaptureObserver(this IInputElement This){
            return Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(h => This.LostMouseCapture += h, h => This.LostMouseCapture -= h);
        }

        public static IObservable<EventPattern<StylusDownEventArgs>> PreviewStylusDownObserver(this IInputElement This){
            return Observable.FromEventPattern<StylusDownEventHandler, StylusDownEventArgs>(h => This.PreviewStylusDown += h, h => This.PreviewStylusDown -= h);
        }

        public static IObservable<EventPattern<StylusDownEventArgs>> StylusDownObserver(this IInputElement This){
            return Observable.FromEventPattern<StylusDownEventHandler, StylusDownEventArgs>(h => This.StylusDown += h, h => This.StylusDown -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> PreviewStylusUpObserver(this IInputElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.PreviewStylusUp += h, h => This.PreviewStylusUp -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> StylusUpObserver(this IInputElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.StylusUp += h, h => This.StylusUp -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> PreviewStylusMoveObserver(this IInputElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.PreviewStylusMove += h, h => This.PreviewStylusMove -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> StylusMoveObserver(this IInputElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.StylusMove += h, h => This.StylusMove -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> PreviewStylusInAirMoveObserver(this IInputElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.PreviewStylusInAirMove += h, h => This.PreviewStylusInAirMove -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> StylusInAirMoveObserver(this IInputElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.StylusInAirMove += h, h => This.StylusInAirMove -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> StylusEnterObserver(this IInputElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.StylusEnter += h, h => This.StylusEnter -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> StylusLeaveObserver(this IInputElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.StylusLeave += h, h => This.StylusLeave -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> PreviewStylusInRangeObserver(this IInputElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.PreviewStylusInRange += h, h => This.PreviewStylusInRange -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> StylusInRangeObserver(this IInputElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.StylusInRange += h, h => This.StylusInRange -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> PreviewStylusOutOfRangeObserver(this IInputElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.PreviewStylusOutOfRange += h, h => This.PreviewStylusOutOfRange -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> StylusOutOfRangeObserver(this IInputElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.StylusOutOfRange += h, h => This.StylusOutOfRange -= h);
        }

        public static IObservable<EventPattern<StylusSystemGestureEventArgs>> PreviewStylusSystemGestureObserver(this IInputElement This){
            return Observable.FromEventPattern<StylusSystemGestureEventHandler, StylusSystemGestureEventArgs>(h => This.PreviewStylusSystemGesture += h, h => This.PreviewStylusSystemGesture -= h);
        }

        public static IObservable<EventPattern<StylusSystemGestureEventArgs>> StylusSystemGestureObserver(this IInputElement This){
            return Observable.FromEventPattern<StylusSystemGestureEventHandler, StylusSystemGestureEventArgs>(h => This.StylusSystemGesture += h, h => This.StylusSystemGesture -= h);
        }

        public static IObservable<EventPattern<StylusButtonEventArgs>> StylusButtonDownObserver(this IInputElement This){
            return Observable.FromEventPattern<StylusButtonEventHandler, StylusButtonEventArgs>(h => This.StylusButtonDown += h, h => This.StylusButtonDown -= h);
        }

        public static IObservable<EventPattern<StylusButtonEventArgs>> PreviewStylusButtonDownObserver(this IInputElement This){
            return Observable.FromEventPattern<StylusButtonEventHandler, StylusButtonEventArgs>(h => This.PreviewStylusButtonDown += h, h => This.PreviewStylusButtonDown -= h);
        }

        public static IObservable<EventPattern<StylusButtonEventArgs>> PreviewStylusButtonUpObserver(this IInputElement This){
            return Observable.FromEventPattern<StylusButtonEventHandler, StylusButtonEventArgs>(h => This.PreviewStylusButtonUp += h, h => This.PreviewStylusButtonUp -= h);
        }

        public static IObservable<EventPattern<StylusButtonEventArgs>> StylusButtonUpObserver(this IInputElement This){
            return Observable.FromEventPattern<StylusButtonEventHandler, StylusButtonEventArgs>(h => This.StylusButtonUp += h, h => This.StylusButtonUp -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> GotStylusCaptureObserver(this IInputElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.GotStylusCapture += h, h => This.GotStylusCapture -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> LostStylusCaptureObserver(this IInputElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.LostStylusCapture += h, h => This.LostStylusCapture -= h);
        }

        public static IObservable<EventPattern<KeyEventArgs>> PreviewKeyDownObserver(this IInputElement This){
            return Observable.FromEventPattern<KeyEventHandler, KeyEventArgs>(h => This.PreviewKeyDown += h, h => This.PreviewKeyDown -= h);
        }

        public static IObservable<EventPattern<KeyEventArgs>> KeyDownObserver(this IInputElement This){
            return Observable.FromEventPattern<KeyEventHandler, KeyEventArgs>(h => This.KeyDown += h, h => This.KeyDown -= h);
        }

        public static IObservable<EventPattern<KeyEventArgs>> PreviewKeyUpObserver(this IInputElement This){
            return Observable.FromEventPattern<KeyEventHandler, KeyEventArgs>(h => This.PreviewKeyUp += h, h => This.PreviewKeyUp -= h);
        }

        public static IObservable<EventPattern<KeyEventArgs>> KeyUpObserver(this IInputElement This){
            return Observable.FromEventPattern<KeyEventHandler, KeyEventArgs>(h => This.KeyUp += h, h => This.KeyUp -= h);
        }

        public static IObservable<EventPattern<KeyboardFocusChangedEventArgs>> PreviewGotKeyboardFocusObserver(this IInputElement This){
            return Observable.FromEventPattern<KeyboardFocusChangedEventHandler, KeyboardFocusChangedEventArgs>(h => This.PreviewGotKeyboardFocus += h, h => This.PreviewGotKeyboardFocus -= h);
        }

        public static IObservable<EventPattern<KeyboardFocusChangedEventArgs>> GotKeyboardFocusObserver(this IInputElement This){
            return Observable.FromEventPattern<KeyboardFocusChangedEventHandler, KeyboardFocusChangedEventArgs>(h => This.GotKeyboardFocus += h, h => This.GotKeyboardFocus -= h);
        }

        public static IObservable<EventPattern<KeyboardFocusChangedEventArgs>> PreviewLostKeyboardFocusObserver(this IInputElement This){
            return Observable.FromEventPattern<KeyboardFocusChangedEventHandler, KeyboardFocusChangedEventArgs>(h => This.PreviewLostKeyboardFocus += h, h => This.PreviewLostKeyboardFocus -= h);
        }

        public static IObservable<EventPattern<KeyboardFocusChangedEventArgs>> LostKeyboardFocusObserver(this IInputElement This){
            return Observable.FromEventPattern<KeyboardFocusChangedEventHandler, KeyboardFocusChangedEventArgs>(h => This.LostKeyboardFocus += h, h => This.LostKeyboardFocus -= h);
        }

        public static IObservable<EventPattern<TextCompositionEventArgs>> PreviewTextInputObserver(this IInputElement This){
            return Observable.FromEventPattern<TextCompositionEventHandler, TextCompositionEventArgs>(h => This.PreviewTextInput += h, h => This.PreviewTextInput -= h);
        }

        public static IObservable<EventPattern<TextCompositionEventArgs>> TextInputObserver(this IInputElement This){
            return Observable.FromEventPattern<TextCompositionEventHandler, TextCompositionEventArgs>(h => This.TextInput += h, h => This.TextInput -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   ContentElement
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<RoutedEventArgs>> GotFocusObserver(this ContentElement This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.GotFocus += h, h => This.GotFocus -= h);
        }

        public static IObservable<EventPattern<RoutedEventArgs>> LostFocusObserver(this ContentElement This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.LostFocus += h, h => This.LostFocus -= h);
        }

        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsEnabledChangedObserver(this ContentElement This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsEnabledChanged += h, h => This.IsEnabledChanged -= h);
        }

        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> FocusableChangedObserver(this ContentElement This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.FocusableChanged += h, h => This.FocusableChanged -= h);
        }

        public static IObservable<EventPattern<MouseButtonEventArgs>> PreviewMouseDownObserver(this ContentElement This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.PreviewMouseDown += h, h => This.PreviewMouseDown -= h);
        }

        public static IObservable<EventPattern<MouseButtonEventArgs>> MouseDownObserver(this ContentElement This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.MouseDown += h, h => This.MouseDown -= h);
        }

        public static IObservable<EventPattern<MouseButtonEventArgs>> PreviewMouseUpObserver(this ContentElement This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.PreviewMouseUp += h, h => This.PreviewMouseUp -= h);
        }

        public static IObservable<EventPattern<MouseButtonEventArgs>> MouseUpObserver(this ContentElement This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.MouseUp += h, h => This.MouseUp -= h);
        }

        public static IObservable<EventPattern<MouseButtonEventArgs>> PreviewMouseLeftButtonDownObserver(this ContentElement This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.PreviewMouseLeftButtonDown += h, h => This.PreviewMouseLeftButtonDown -= h);
        }

        public static IObservable<EventPattern<MouseButtonEventArgs>> MouseLeftButtonDownObserver(this ContentElement This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.MouseLeftButtonDown += h, h => This.MouseLeftButtonDown -= h);
        }

        public static IObservable<EventPattern<MouseButtonEventArgs>> PreviewMouseLeftButtonUpObserver(this ContentElement This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.PreviewMouseLeftButtonUp += h, h => This.PreviewMouseLeftButtonUp -= h);
        }

        public static IObservable<EventPattern<MouseButtonEventArgs>> MouseLeftButtonUpObserver(this ContentElement This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.MouseLeftButtonUp += h, h => This.MouseLeftButtonUp -= h);
        }

        public static IObservable<EventPattern<MouseButtonEventArgs>> PreviewMouseRightButtonDownObserver(this ContentElement This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.PreviewMouseRightButtonDown += h, h => This.PreviewMouseRightButtonDown -= h);
        }

        public static IObservable<EventPattern<MouseButtonEventArgs>> MouseRightButtonDownObserver(this ContentElement This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.MouseRightButtonDown += h, h => This.MouseRightButtonDown -= h);
        }

        public static IObservable<EventPattern<MouseButtonEventArgs>> PreviewMouseRightButtonUpObserver(this ContentElement This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.PreviewMouseRightButtonUp += h, h => This.PreviewMouseRightButtonUp -= h);
        }

        public static IObservable<EventPattern<MouseButtonEventArgs>> MouseRightButtonUpObserver(this ContentElement This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.MouseRightButtonUp += h, h => This.MouseRightButtonUp -= h);
        }

        public static IObservable<EventPattern<MouseEventArgs>> PreviewMouseMoveObserver(this ContentElement This){
            return Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(h => This.PreviewMouseMove += h, h => This.PreviewMouseMove -= h);
        }

        public static IObservable<EventPattern<MouseEventArgs>> MouseMoveObserver(this ContentElement This){
            return Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(h => This.MouseMove += h, h => This.MouseMove -= h);
        }

        public static IObservable<EventPattern<MouseWheelEventArgs>> PreviewMouseWheelObserver(this ContentElement This){
            return Observable.FromEventPattern<MouseWheelEventHandler, MouseWheelEventArgs>(h => This.PreviewMouseWheel += h, h => This.PreviewMouseWheel -= h);
        }

        public static IObservable<EventPattern<MouseWheelEventArgs>> MouseWheelObserver(this ContentElement This){
            return Observable.FromEventPattern<MouseWheelEventHandler, MouseWheelEventArgs>(h => This.MouseWheel += h, h => This.MouseWheel -= h);
        }

        public static IObservable<EventPattern<MouseEventArgs>> MouseEnterObserver(this ContentElement This){
            return Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(h => This.MouseEnter += h, h => This.MouseEnter -= h);
        }

        public static IObservable<EventPattern<MouseEventArgs>> MouseLeaveObserver(this ContentElement This){
            return Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(h => This.MouseLeave += h, h => This.MouseLeave -= h);
        }

        public static IObservable<EventPattern<MouseEventArgs>> GotMouseCaptureObserver(this ContentElement This){
            return Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(h => This.GotMouseCapture += h, h => This.GotMouseCapture -= h);
        }

        public static IObservable<EventPattern<MouseEventArgs>> LostMouseCaptureObserver(this ContentElement This){
            return Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(h => This.LostMouseCapture += h, h => This.LostMouseCapture -= h);
        }

        public static IObservable<EventPattern<QueryCursorEventArgs>> QueryCursorObserver(this ContentElement This){
            return Observable.FromEventPattern<QueryCursorEventHandler, QueryCursorEventArgs>(h => This.QueryCursor += h, h => This.QueryCursor -= h);
        }

        public static IObservable<EventPattern<StylusDownEventArgs>> PreviewStylusDownObserver(this ContentElement This){
            return Observable.FromEventPattern<StylusDownEventHandler, StylusDownEventArgs>(h => This.PreviewStylusDown += h, h => This.PreviewStylusDown -= h);
        }

        public static IObservable<EventPattern<StylusDownEventArgs>> StylusDownObserver(this ContentElement This){
            return Observable.FromEventPattern<StylusDownEventHandler, StylusDownEventArgs>(h => This.StylusDown += h, h => This.StylusDown -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> PreviewStylusUpObserver(this ContentElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.PreviewStylusUp += h, h => This.PreviewStylusUp -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> StylusUpObserver(this ContentElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.StylusUp += h, h => This.StylusUp -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> PreviewStylusMoveObserver(this ContentElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.PreviewStylusMove += h, h => This.PreviewStylusMove -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> StylusMoveObserver(this ContentElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.StylusMove += h, h => This.StylusMove -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> PreviewStylusInAirMoveObserver(this ContentElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.PreviewStylusInAirMove += h, h => This.PreviewStylusInAirMove -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> StylusInAirMoveObserver(this ContentElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.StylusInAirMove += h, h => This.StylusInAirMove -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> StylusEnterObserver(this ContentElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.StylusEnter += h, h => This.StylusEnter -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> StylusLeaveObserver(this ContentElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.StylusLeave += h, h => This.StylusLeave -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> PreviewStylusInRangeObserver(this ContentElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.PreviewStylusInRange += h, h => This.PreviewStylusInRange -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> StylusInRangeObserver(this ContentElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.StylusInRange += h, h => This.StylusInRange -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> PreviewStylusOutOfRangeObserver(this ContentElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.PreviewStylusOutOfRange += h, h => This.PreviewStylusOutOfRange -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> StylusOutOfRangeObserver(this ContentElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.StylusOutOfRange += h, h => This.StylusOutOfRange -= h);
        }

        public static IObservable<EventPattern<StylusSystemGestureEventArgs>> PreviewStylusSystemGestureObserver(this ContentElement This){
            return Observable.FromEventPattern<StylusSystemGestureEventHandler, StylusSystemGestureEventArgs>(h => This.PreviewStylusSystemGesture += h, h => This.PreviewStylusSystemGesture -= h);
        }

        public static IObservable<EventPattern<StylusSystemGestureEventArgs>> StylusSystemGestureObserver(this ContentElement This){
            return Observable.FromEventPattern<StylusSystemGestureEventHandler, StylusSystemGestureEventArgs>(h => This.StylusSystemGesture += h, h => This.StylusSystemGesture -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> GotStylusCaptureObserver(this ContentElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.GotStylusCapture += h, h => This.GotStylusCapture -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> LostStylusCaptureObserver(this ContentElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.LostStylusCapture += h, h => This.LostStylusCapture -= h);
        }

        public static IObservable<EventPattern<StylusButtonEventArgs>> StylusButtonDownObserver(this ContentElement This){
            return Observable.FromEventPattern<StylusButtonEventHandler, StylusButtonEventArgs>(h => This.StylusButtonDown += h, h => This.StylusButtonDown -= h);
        }

        public static IObservable<EventPattern<StylusButtonEventArgs>> StylusButtonUpObserver(this ContentElement This){
            return Observable.FromEventPattern<StylusButtonEventHandler, StylusButtonEventArgs>(h => This.StylusButtonUp += h, h => This.StylusButtonUp -= h);
        }

        public static IObservable<EventPattern<StylusButtonEventArgs>> PreviewStylusButtonDownObserver(this ContentElement This){
            return Observable.FromEventPattern<StylusButtonEventHandler, StylusButtonEventArgs>(h => This.PreviewStylusButtonDown += h, h => This.PreviewStylusButtonDown -= h);
        }

        public static IObservable<EventPattern<StylusButtonEventArgs>> PreviewStylusButtonUpObserver(this ContentElement This){
            return Observable.FromEventPattern<StylusButtonEventHandler, StylusButtonEventArgs>(h => This.PreviewStylusButtonUp += h, h => This.PreviewStylusButtonUp -= h);
        }

        public static IObservable<EventPattern<KeyEventArgs>> PreviewKeyDownObserver(this ContentElement This){
            return Observable.FromEventPattern<KeyEventHandler, KeyEventArgs>(h => This.PreviewKeyDown += h, h => This.PreviewKeyDown -= h);
        }

        public static IObservable<EventPattern<KeyEventArgs>> KeyDownObserver(this ContentElement This){
            return Observable.FromEventPattern<KeyEventHandler, KeyEventArgs>(h => This.KeyDown += h, h => This.KeyDown -= h);
        }

        public static IObservable<EventPattern<KeyEventArgs>> PreviewKeyUpObserver(this ContentElement This){
            return Observable.FromEventPattern<KeyEventHandler, KeyEventArgs>(h => This.PreviewKeyUp += h, h => This.PreviewKeyUp -= h);
        }

        public static IObservable<EventPattern<KeyEventArgs>> KeyUpObserver(this ContentElement This){
            return Observable.FromEventPattern<KeyEventHandler, KeyEventArgs>(h => This.KeyUp += h, h => This.KeyUp -= h);
        }

        public static IObservable<EventPattern<KeyboardFocusChangedEventArgs>> PreviewGotKeyboardFocusObserver(this ContentElement This){
            return Observable.FromEventPattern<KeyboardFocusChangedEventHandler, KeyboardFocusChangedEventArgs>(h => This.PreviewGotKeyboardFocus += h, h => This.PreviewGotKeyboardFocus -= h);
        }

        public static IObservable<EventPattern<KeyboardFocusChangedEventArgs>> GotKeyboardFocusObserver(this ContentElement This){
            return Observable.FromEventPattern<KeyboardFocusChangedEventHandler, KeyboardFocusChangedEventArgs>(h => This.GotKeyboardFocus += h, h => This.GotKeyboardFocus -= h);
        }

        public static IObservable<EventPattern<KeyboardFocusChangedEventArgs>> PreviewLostKeyboardFocusObserver(this ContentElement This){
            return Observable.FromEventPattern<KeyboardFocusChangedEventHandler, KeyboardFocusChangedEventArgs>(h => This.PreviewLostKeyboardFocus += h, h => This.PreviewLostKeyboardFocus -= h);
        }

        public static IObservable<EventPattern<KeyboardFocusChangedEventArgs>> LostKeyboardFocusObserver(this ContentElement This){
            return Observable.FromEventPattern<KeyboardFocusChangedEventHandler, KeyboardFocusChangedEventArgs>(h => This.LostKeyboardFocus += h, h => This.LostKeyboardFocus -= h);
        }

        public static IObservable<EventPattern<TextCompositionEventArgs>> PreviewTextInputObserver(this ContentElement This){
            return Observable.FromEventPattern<TextCompositionEventHandler, TextCompositionEventArgs>(h => This.PreviewTextInput += h, h => This.PreviewTextInput -= h);
        }

        public static IObservable<EventPattern<TextCompositionEventArgs>> TextInputObserver(this ContentElement This){
            return Observable.FromEventPattern<TextCompositionEventHandler, TextCompositionEventArgs>(h => This.TextInput += h, h => This.TextInput -= h);
        }

        public static IObservable<EventPattern<QueryContinueDragEventArgs>> PreviewQueryContinueDragObserver(this ContentElement This){
            return Observable.FromEventPattern<QueryContinueDragEventHandler, QueryContinueDragEventArgs>(h => This.PreviewQueryContinueDrag += h, h => This.PreviewQueryContinueDrag -= h);
        }

        public static IObservable<EventPattern<QueryContinueDragEventArgs>> QueryContinueDragObserver(this ContentElement This){
            return Observable.FromEventPattern<QueryContinueDragEventHandler, QueryContinueDragEventArgs>(h => This.QueryContinueDrag += h, h => This.QueryContinueDrag -= h);
        }

        public static IObservable<EventPattern<GiveFeedbackEventArgs>> PreviewGiveFeedbackObserver(this ContentElement This){
            return Observable.FromEventPattern<GiveFeedbackEventHandler, GiveFeedbackEventArgs>(h => This.PreviewGiveFeedback += h, h => This.PreviewGiveFeedback -= h);
        }

        public static IObservable<EventPattern<GiveFeedbackEventArgs>> GiveFeedbackObserver(this ContentElement This){
            return Observable.FromEventPattern<GiveFeedbackEventHandler, GiveFeedbackEventArgs>(h => This.GiveFeedback += h, h => This.GiveFeedback -= h);
        }

        public static IObservable<EventPattern<DragEventArgs>> PreviewDragEnterObserver(this ContentElement This){
            return Observable.FromEventPattern<DragEventHandler, DragEventArgs>(h => This.PreviewDragEnter += h, h => This.PreviewDragEnter -= h);
        }

        public static IObservable<EventPattern<DragEventArgs>> DragEnterObserver(this ContentElement This){
            return Observable.FromEventPattern<DragEventHandler, DragEventArgs>(h => This.DragEnter += h, h => This.DragEnter -= h);
        }

        public static IObservable<EventPattern<DragEventArgs>> PreviewDragOverObserver(this ContentElement This){
            return Observable.FromEventPattern<DragEventHandler, DragEventArgs>(h => This.PreviewDragOver += h, h => This.PreviewDragOver -= h);
        }

        public static IObservable<EventPattern<DragEventArgs>> DragOverObserver(this ContentElement This){
            return Observable.FromEventPattern<DragEventHandler, DragEventArgs>(h => This.DragOver += h, h => This.DragOver -= h);
        }

        public static IObservable<EventPattern<DragEventArgs>> PreviewDragLeaveObserver(this ContentElement This){
            return Observable.FromEventPattern<DragEventHandler, DragEventArgs>(h => This.PreviewDragLeave += h, h => This.PreviewDragLeave -= h);
        }

        public static IObservable<EventPattern<DragEventArgs>> DragLeaveObserver(this ContentElement This){
            return Observable.FromEventPattern<DragEventHandler, DragEventArgs>(h => This.DragLeave += h, h => This.DragLeave -= h);
        }

        public static IObservable<EventPattern<DragEventArgs>> PreviewDropObserver(this ContentElement This){
            return Observable.FromEventPattern<DragEventHandler, DragEventArgs>(h => This.PreviewDrop += h, h => This.PreviewDrop -= h);
        }

        public static IObservable<EventPattern<DragEventArgs>> DropObserver(this ContentElement This){
            return Observable.FromEventPattern<DragEventHandler, DragEventArgs>(h => This.Drop += h, h => This.Drop -= h);
        }

        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsMouseDirectlyOverChangedObserver(this ContentElement This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsMouseDirectlyOverChanged += h, h => This.IsMouseDirectlyOverChanged -= h);
        }

        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsKeyboardFocusWithinChangedObserver(this ContentElement This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsKeyboardFocusWithinChanged += h, h => This.IsKeyboardFocusWithinChanged -= h);
        }

        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsMouseCapturedChangedObserver(this ContentElement This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsMouseCapturedChanged += h, h => This.IsMouseCapturedChanged -= h);
        }

        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsMouseCaptureWithinChangedObserver(this ContentElement This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsMouseCaptureWithinChanged += h, h => This.IsMouseCaptureWithinChanged -= h);
        }

        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsStylusDirectlyOverChangedObserver(this ContentElement This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsStylusDirectlyOverChanged += h, h => This.IsStylusDirectlyOverChanged -= h);
        }

        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsStylusCapturedChangedObserver(this ContentElement This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsStylusCapturedChanged += h, h => This.IsStylusCapturedChanged -= h);
        }

        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsStylusCaptureWithinChangedObserver(this ContentElement This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsStylusCaptureWithinChanged += h, h => This.IsStylusCaptureWithinChanged -= h);
        }

        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsKeyboardFocusedChangedObserver(this ContentElement This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsKeyboardFocusedChanged += h, h => This.IsKeyboardFocusedChanged -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   UIElement
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<MouseButtonEventArgs>> PreviewMouseDownObserver(this UIElement This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.PreviewMouseDown += h, h => This.PreviewMouseDown -= h);
        }

        public static IObservable<EventPattern<MouseButtonEventArgs>> MouseDownObserver(this UIElement This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.MouseDown += h, h => This.MouseDown -= h);
        }

        public static IObservable<EventPattern<MouseButtonEventArgs>> PreviewMouseUpObserver(this UIElement This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.PreviewMouseUp += h, h => This.PreviewMouseUp -= h);
        }

        public static IObservable<EventPattern<MouseButtonEventArgs>> MouseUpObserver(this UIElement This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.MouseUp += h, h => This.MouseUp -= h);
        }

        public static IObservable<EventPattern<MouseButtonEventArgs>> PreviewMouseLeftButtonDownObserver(this UIElement This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.PreviewMouseLeftButtonDown += h, h => This.PreviewMouseLeftButtonDown -= h);
        }

        public static IObservable<EventPattern<MouseButtonEventArgs>> MouseLeftButtonDownObserver(this UIElement This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.MouseLeftButtonDown += h, h => This.MouseLeftButtonDown -= h);
        }

        public static IObservable<EventPattern<MouseButtonEventArgs>> PreviewMouseLeftButtonUpObserver(this UIElement This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.PreviewMouseLeftButtonUp += h, h => This.PreviewMouseLeftButtonUp -= h);
        }

        public static IObservable<EventPattern<MouseButtonEventArgs>> MouseLeftButtonUpObserver(this UIElement This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.MouseLeftButtonUp += h, h => This.MouseLeftButtonUp -= h);
        }

        public static IObservable<EventPattern<MouseButtonEventArgs>> PreviewMouseRightButtonDownObserver(this UIElement This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.PreviewMouseRightButtonDown += h, h => This.PreviewMouseRightButtonDown -= h);
        }

        public static IObservable<EventPattern<MouseButtonEventArgs>> MouseRightButtonDownObserver(this UIElement This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.MouseRightButtonDown += h, h => This.MouseRightButtonDown -= h);
        }

        public static IObservable<EventPattern<MouseButtonEventArgs>> PreviewMouseRightButtonUpObserver(this UIElement This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.PreviewMouseRightButtonUp += h, h => This.PreviewMouseRightButtonUp -= h);
        }

        public static IObservable<EventPattern<MouseButtonEventArgs>> MouseRightButtonUpObserver(this UIElement This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.MouseRightButtonUp += h, h => This.MouseRightButtonUp -= h);
        }

        public static IObservable<EventPattern<MouseEventArgs>> PreviewMouseMoveObserver(this UIElement This){
            return Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(h => This.PreviewMouseMove += h, h => This.PreviewMouseMove -= h);
        }

        public static IObservable<EventPattern<MouseEventArgs>> MouseMoveObserver(this UIElement This){
            return Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(h => This.MouseMove += h, h => This.MouseMove -= h);
        }

        public static IObservable<EventPattern<MouseWheelEventArgs>> PreviewMouseWheelObserver(this UIElement This){
            return Observable.FromEventPattern<MouseWheelEventHandler, MouseWheelEventArgs>(h => This.PreviewMouseWheel += h, h => This.PreviewMouseWheel -= h);
        }

        public static IObservable<EventPattern<MouseWheelEventArgs>> MouseWheelObserver(this UIElement This){
            return Observable.FromEventPattern<MouseWheelEventHandler, MouseWheelEventArgs>(h => This.MouseWheel += h, h => This.MouseWheel -= h);
        }

        public static IObservable<EventPattern<MouseEventArgs>> MouseEnterObserver(this UIElement This){
            return Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(h => This.MouseEnter += h, h => This.MouseEnter -= h);
        }

        public static IObservable<EventPattern<MouseEventArgs>> MouseLeaveObserver(this UIElement This){
            return Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(h => This.MouseLeave += h, h => This.MouseLeave -= h);
        }

        public static IObservable<EventPattern<MouseEventArgs>> GotMouseCaptureObserver(this UIElement This){
            return Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(h => This.GotMouseCapture += h, h => This.GotMouseCapture -= h);
        }

        public static IObservable<EventPattern<MouseEventArgs>> LostMouseCaptureObserver(this UIElement This){
            return Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(h => This.LostMouseCapture += h, h => This.LostMouseCapture -= h);
        }

        public static IObservable<EventPattern<QueryCursorEventArgs>> QueryCursorObserver(this UIElement This){
            return Observable.FromEventPattern<QueryCursorEventHandler, QueryCursorEventArgs>(h => This.QueryCursor += h, h => This.QueryCursor -= h);
        }

        public static IObservable<EventPattern<StylusDownEventArgs>> PreviewStylusDownObserver(this UIElement This){
            return Observable.FromEventPattern<StylusDownEventHandler, StylusDownEventArgs>(h => This.PreviewStylusDown += h, h => This.PreviewStylusDown -= h);
        }

        public static IObservable<EventPattern<StylusDownEventArgs>> StylusDownObserver(this UIElement This){
            return Observable.FromEventPattern<StylusDownEventHandler, StylusDownEventArgs>(h => This.StylusDown += h, h => This.StylusDown -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> PreviewStylusUpObserver(this UIElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.PreviewStylusUp += h, h => This.PreviewStylusUp -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> StylusUpObserver(this UIElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.StylusUp += h, h => This.StylusUp -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> PreviewStylusMoveObserver(this UIElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.PreviewStylusMove += h, h => This.PreviewStylusMove -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> StylusMoveObserver(this UIElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.StylusMove += h, h => This.StylusMove -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> PreviewStylusInAirMoveObserver(this UIElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.PreviewStylusInAirMove += h, h => This.PreviewStylusInAirMove -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> StylusInAirMoveObserver(this UIElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.StylusInAirMove += h, h => This.StylusInAirMove -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> StylusEnterObserver(this UIElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.StylusEnter += h, h => This.StylusEnter -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> StylusLeaveObserver(this UIElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.StylusLeave += h, h => This.StylusLeave -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> PreviewStylusInRangeObserver(this UIElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.PreviewStylusInRange += h, h => This.PreviewStylusInRange -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> StylusInRangeObserver(this UIElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.StylusInRange += h, h => This.StylusInRange -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> PreviewStylusOutOfRangeObserver(this UIElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.PreviewStylusOutOfRange += h, h => This.PreviewStylusOutOfRange -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> StylusOutOfRangeObserver(this UIElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.StylusOutOfRange += h, h => This.StylusOutOfRange -= h);
        }

        public static IObservable<EventPattern<StylusSystemGestureEventArgs>> PreviewStylusSystemGestureObserver(this UIElement This){
            return Observable.FromEventPattern<StylusSystemGestureEventHandler, StylusSystemGestureEventArgs>(h => This.PreviewStylusSystemGesture += h, h => This.PreviewStylusSystemGesture -= h);
        }

        public static IObservable<EventPattern<StylusSystemGestureEventArgs>> StylusSystemGestureObserver(this UIElement This){
            return Observable.FromEventPattern<StylusSystemGestureEventHandler, StylusSystemGestureEventArgs>(h => This.StylusSystemGesture += h, h => This.StylusSystemGesture -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> GotStylusCaptureObserver(this UIElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.GotStylusCapture += h, h => This.GotStylusCapture -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> LostStylusCaptureObserver(this UIElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.LostStylusCapture += h, h => This.LostStylusCapture -= h);
        }

        public static IObservable<EventPattern<StylusButtonEventArgs>> StylusButtonDownObserver(this UIElement This){
            return Observable.FromEventPattern<StylusButtonEventHandler, StylusButtonEventArgs>(h => This.StylusButtonDown += h, h => This.StylusButtonDown -= h);
        }

        public static IObservable<EventPattern<StylusButtonEventArgs>> StylusButtonUpObserver(this UIElement This){
            return Observable.FromEventPattern<StylusButtonEventHandler, StylusButtonEventArgs>(h => This.StylusButtonUp += h, h => This.StylusButtonUp -= h);
        }

        public static IObservable<EventPattern<StylusButtonEventArgs>> PreviewStylusButtonDownObserver(this UIElement This){
            return Observable.FromEventPattern<StylusButtonEventHandler, StylusButtonEventArgs>(h => This.PreviewStylusButtonDown += h, h => This.PreviewStylusButtonDown -= h);
        }

        public static IObservable<EventPattern<StylusButtonEventArgs>> PreviewStylusButtonUpObserver(this UIElement This){
            return Observable.FromEventPattern<StylusButtonEventHandler, StylusButtonEventArgs>(h => This.PreviewStylusButtonUp += h, h => This.PreviewStylusButtonUp -= h);
        }

        public static IObservable<EventPattern<KeyEventArgs>> PreviewKeyDownObserver(this UIElement This){
            return Observable.FromEventPattern<KeyEventHandler, KeyEventArgs>(h => This.PreviewKeyDown += h, h => This.PreviewKeyDown -= h);
        }

        public static IObservable<EventPattern<KeyEventArgs>> KeyDownObserver(this UIElement This){
            return Observable.FromEventPattern<KeyEventHandler, KeyEventArgs>(h => This.KeyDown += h, h => This.KeyDown -= h);
        }

        public static IObservable<EventPattern<KeyEventArgs>> PreviewKeyUpObserver(this UIElement This){
            return Observable.FromEventPattern<KeyEventHandler, KeyEventArgs>(h => This.PreviewKeyUp += h, h => This.PreviewKeyUp -= h);
        }

        public static IObservable<EventPattern<KeyEventArgs>> KeyUpObserver(this UIElement This){
            return Observable.FromEventPattern<KeyEventHandler, KeyEventArgs>(h => This.KeyUp += h, h => This.KeyUp -= h);
        }

        public static IObservable<EventPattern<KeyboardFocusChangedEventArgs>> PreviewGotKeyboardFocusObserver(this UIElement This){
            return Observable.FromEventPattern<KeyboardFocusChangedEventHandler, KeyboardFocusChangedEventArgs>(h => This.PreviewGotKeyboardFocus += h, h => This.PreviewGotKeyboardFocus -= h);
        }

        public static IObservable<EventPattern<KeyboardFocusChangedEventArgs>> GotKeyboardFocusObserver(this UIElement This){
            return Observable.FromEventPattern<KeyboardFocusChangedEventHandler, KeyboardFocusChangedEventArgs>(h => This.GotKeyboardFocus += h, h => This.GotKeyboardFocus -= h);
        }

        public static IObservable<EventPattern<KeyboardFocusChangedEventArgs>> PreviewLostKeyboardFocusObserver(this UIElement This){
            return Observable.FromEventPattern<KeyboardFocusChangedEventHandler, KeyboardFocusChangedEventArgs>(h => This.PreviewLostKeyboardFocus += h, h => This.PreviewLostKeyboardFocus -= h);
        }

        public static IObservable<EventPattern<KeyboardFocusChangedEventArgs>> LostKeyboardFocusObserver(this UIElement This){
            return Observable.FromEventPattern<KeyboardFocusChangedEventHandler, KeyboardFocusChangedEventArgs>(h => This.LostKeyboardFocus += h, h => This.LostKeyboardFocus -= h);
        }

        public static IObservable<EventPattern<TextCompositionEventArgs>> PreviewTextInputObserver(this UIElement This){
            return Observable.FromEventPattern<TextCompositionEventHandler, TextCompositionEventArgs>(h => This.PreviewTextInput += h, h => This.PreviewTextInput -= h);
        }

        public static IObservable<EventPattern<TextCompositionEventArgs>> TextInputObserver(this UIElement This){
            return Observable.FromEventPattern<TextCompositionEventHandler, TextCompositionEventArgs>(h => This.TextInput += h, h => This.TextInput -= h);
        }

        public static IObservable<EventPattern<QueryContinueDragEventArgs>> PreviewQueryContinueDragObserver(this UIElement This){
            return Observable.FromEventPattern<QueryContinueDragEventHandler, QueryContinueDragEventArgs>(h => This.PreviewQueryContinueDrag += h, h => This.PreviewQueryContinueDrag -= h);
        }

        public static IObservable<EventPattern<QueryContinueDragEventArgs>> QueryContinueDragObserver(this UIElement This){
            return Observable.FromEventPattern<QueryContinueDragEventHandler, QueryContinueDragEventArgs>(h => This.QueryContinueDrag += h, h => This.QueryContinueDrag -= h);
        }

        public static IObservable<EventPattern<GiveFeedbackEventArgs>> PreviewGiveFeedbackObserver(this UIElement This){
            return Observable.FromEventPattern<GiveFeedbackEventHandler, GiveFeedbackEventArgs>(h => This.PreviewGiveFeedback += h, h => This.PreviewGiveFeedback -= h);
        }

        public static IObservable<EventPattern<GiveFeedbackEventArgs>> GiveFeedbackObserver(this UIElement This){
            return Observable.FromEventPattern<GiveFeedbackEventHandler, GiveFeedbackEventArgs>(h => This.GiveFeedback += h, h => This.GiveFeedback -= h);
        }

        public static IObservable<EventPattern<DragEventArgs>> PreviewDragEnterObserver(this UIElement This){
            return Observable.FromEventPattern<DragEventHandler, DragEventArgs>(h => This.PreviewDragEnter += h, h => This.PreviewDragEnter -= h);
        }

        public static IObservable<EventPattern<DragEventArgs>> DragEnterObserver(this UIElement This){
            return Observable.FromEventPattern<DragEventHandler, DragEventArgs>(h => This.DragEnter += h, h => This.DragEnter -= h);
        }

        public static IObservable<EventPattern<DragEventArgs>> PreviewDragOverObserver(this UIElement This){
            return Observable.FromEventPattern<DragEventHandler, DragEventArgs>(h => This.PreviewDragOver += h, h => This.PreviewDragOver -= h);
        }

        public static IObservable<EventPattern<DragEventArgs>> DragOverObserver(this UIElement This){
            return Observable.FromEventPattern<DragEventHandler, DragEventArgs>(h => This.DragOver += h, h => This.DragOver -= h);
        }

        public static IObservable<EventPattern<DragEventArgs>> PreviewDragLeaveObserver(this UIElement This){
            return Observable.FromEventPattern<DragEventHandler, DragEventArgs>(h => This.PreviewDragLeave += h, h => This.PreviewDragLeave -= h);
        }

        public static IObservable<EventPattern<DragEventArgs>> DragLeaveObserver(this UIElement This){
            return Observable.FromEventPattern<DragEventHandler, DragEventArgs>(h => This.DragLeave += h, h => This.DragLeave -= h);
        }

        public static IObservable<EventPattern<DragEventArgs>> PreviewDropObserver(this UIElement This){
            return Observable.FromEventPattern<DragEventHandler, DragEventArgs>(h => This.PreviewDrop += h, h => This.PreviewDrop -= h);
        }

        public static IObservable<EventPattern<DragEventArgs>> DropObserver(this UIElement This){
            return Observable.FromEventPattern<DragEventHandler, DragEventArgs>(h => This.Drop += h, h => This.Drop -= h);
        }

        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsMouseDirectlyOverChangedObserver(this UIElement This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsMouseDirectlyOverChanged += h, h => This.IsMouseDirectlyOverChanged -= h);
        }

        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsKeyboardFocusWithinChangedObserver(this UIElement This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsKeyboardFocusWithinChanged += h, h => This.IsKeyboardFocusWithinChanged -= h);
        }

        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsMouseCapturedChangedObserver(this UIElement This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsMouseCapturedChanged += h, h => This.IsMouseCapturedChanged -= h);
        }

        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsMouseCaptureWithinChangedObserver(this UIElement This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsMouseCaptureWithinChanged += h, h => This.IsMouseCaptureWithinChanged -= h);
        }

        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsStylusDirectlyOverChangedObserver(this UIElement This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsStylusDirectlyOverChanged += h, h => This.IsStylusDirectlyOverChanged -= h);
        }

        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsStylusCapturedChangedObserver(this UIElement This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsStylusCapturedChanged += h, h => This.IsStylusCapturedChanged -= h);
        }

        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsStylusCaptureWithinChangedObserver(this UIElement This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsStylusCaptureWithinChanged += h, h => This.IsStylusCaptureWithinChanged -= h);
        }

        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsKeyboardFocusedChangedObserver(this UIElement This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsKeyboardFocusedChanged += h, h => This.IsKeyboardFocusedChanged -= h);
        }

        public static IObservable<EventPattern<EventArgs>> LayoutUpdatedObserver(this UIElement This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.LayoutUpdated += h, h => This.LayoutUpdated -= h);
        }

        public static IObservable<EventPattern<RoutedEventArgs>> GotFocusObserver(this UIElement This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.GotFocus += h, h => This.GotFocus -= h);
        }

        public static IObservable<EventPattern<RoutedEventArgs>> LostFocusObserver(this UIElement This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.LostFocus += h, h => This.LostFocus -= h);
        }

        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsEnabledChangedObserver(this UIElement This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsEnabledChanged += h, h => This.IsEnabledChanged -= h);
        }

        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsHitTestVisibleChangedObserver(this UIElement This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsHitTestVisibleChanged += h, h => This.IsHitTestVisibleChanged -= h);
        }

        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsVisibleChangedObserver(this UIElement This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsVisibleChanged += h, h => This.IsVisibleChanged -= h);
        }

        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> FocusableChangedObserver(this UIElement This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.FocusableChanged += h, h => This.FocusableChanged -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   UIElement3D
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<MouseButtonEventArgs>> PreviewMouseDownObserver(this UIElement3D This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.PreviewMouseDown += h, h => This.PreviewMouseDown -= h);
        }

        public static IObservable<EventPattern<MouseButtonEventArgs>> MouseDownObserver(this UIElement3D This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.MouseDown += h, h => This.MouseDown -= h);
        }

        public static IObservable<EventPattern<MouseButtonEventArgs>> PreviewMouseUpObserver(this UIElement3D This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.PreviewMouseUp += h, h => This.PreviewMouseUp -= h);
        }

        public static IObservable<EventPattern<MouseButtonEventArgs>> MouseUpObserver(this UIElement3D This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.MouseUp += h, h => This.MouseUp -= h);
        }

        public static IObservable<EventPattern<MouseButtonEventArgs>> PreviewMouseLeftButtonDownObserver(this UIElement3D This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.PreviewMouseLeftButtonDown += h, h => This.PreviewMouseLeftButtonDown -= h);
        }

        public static IObservable<EventPattern<MouseButtonEventArgs>> MouseLeftButtonDownObserver(this UIElement3D This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.MouseLeftButtonDown += h, h => This.MouseLeftButtonDown -= h);
        }

        public static IObservable<EventPattern<MouseButtonEventArgs>> PreviewMouseLeftButtonUpObserver(this UIElement3D This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.PreviewMouseLeftButtonUp += h, h => This.PreviewMouseLeftButtonUp -= h);
        }

        public static IObservable<EventPattern<MouseButtonEventArgs>> MouseLeftButtonUpObserver(this UIElement3D This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.MouseLeftButtonUp += h, h => This.MouseLeftButtonUp -= h);
        }

        public static IObservable<EventPattern<MouseButtonEventArgs>> PreviewMouseRightButtonDownObserver(this UIElement3D This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.PreviewMouseRightButtonDown += h, h => This.PreviewMouseRightButtonDown -= h);
        }

        public static IObservable<EventPattern<MouseButtonEventArgs>> MouseRightButtonDownObserver(this UIElement3D This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.MouseRightButtonDown += h, h => This.MouseRightButtonDown -= h);
        }

        public static IObservable<EventPattern<MouseButtonEventArgs>> PreviewMouseRightButtonUpObserver(this UIElement3D This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.PreviewMouseRightButtonUp += h, h => This.PreviewMouseRightButtonUp -= h);
        }

        public static IObservable<EventPattern<MouseButtonEventArgs>> MouseRightButtonUpObserver(this UIElement3D This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.MouseRightButtonUp += h, h => This.MouseRightButtonUp -= h);
        }

        public static IObservable<EventPattern<MouseEventArgs>> PreviewMouseMoveObserver(this UIElement3D This){
            return Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(h => This.PreviewMouseMove += h, h => This.PreviewMouseMove -= h);
        }

        public static IObservable<EventPattern<MouseEventArgs>> MouseMoveObserver(this UIElement3D This){
            return Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(h => This.MouseMove += h, h => This.MouseMove -= h);
        }

        public static IObservable<EventPattern<MouseWheelEventArgs>> PreviewMouseWheelObserver(this UIElement3D This){
            return Observable.FromEventPattern<MouseWheelEventHandler, MouseWheelEventArgs>(h => This.PreviewMouseWheel += h, h => This.PreviewMouseWheel -= h);
        }

        public static IObservable<EventPattern<MouseWheelEventArgs>> MouseWheelObserver(this UIElement3D This){
            return Observable.FromEventPattern<MouseWheelEventHandler, MouseWheelEventArgs>(h => This.MouseWheel += h, h => This.MouseWheel -= h);
        }

        public static IObservable<EventPattern<MouseEventArgs>> MouseEnterObserver(this UIElement3D This){
            return Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(h => This.MouseEnter += h, h => This.MouseEnter -= h);
        }

        public static IObservable<EventPattern<MouseEventArgs>> MouseLeaveObserver(this UIElement3D This){
            return Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(h => This.MouseLeave += h, h => This.MouseLeave -= h);
        }

        public static IObservable<EventPattern<MouseEventArgs>> GotMouseCaptureObserver(this UIElement3D This){
            return Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(h => This.GotMouseCapture += h, h => This.GotMouseCapture -= h);
        }

        public static IObservable<EventPattern<MouseEventArgs>> LostMouseCaptureObserver(this UIElement3D This){
            return Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(h => This.LostMouseCapture += h, h => This.LostMouseCapture -= h);
        }

        public static IObservable<EventPattern<QueryCursorEventArgs>> QueryCursorObserver(this UIElement3D This){
            return Observable.FromEventPattern<QueryCursorEventHandler, QueryCursorEventArgs>(h => This.QueryCursor += h, h => This.QueryCursor -= h);
        }

        public static IObservable<EventPattern<StylusDownEventArgs>> PreviewStylusDownObserver(this UIElement3D This){
            return Observable.FromEventPattern<StylusDownEventHandler, StylusDownEventArgs>(h => This.PreviewStylusDown += h, h => This.PreviewStylusDown -= h);
        }

        public static IObservable<EventPattern<StylusDownEventArgs>> StylusDownObserver(this UIElement3D This){
            return Observable.FromEventPattern<StylusDownEventHandler, StylusDownEventArgs>(h => This.StylusDown += h, h => This.StylusDown -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> PreviewStylusUpObserver(this UIElement3D This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.PreviewStylusUp += h, h => This.PreviewStylusUp -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> StylusUpObserver(this UIElement3D This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.StylusUp += h, h => This.StylusUp -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> PreviewStylusMoveObserver(this UIElement3D This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.PreviewStylusMove += h, h => This.PreviewStylusMove -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> StylusMoveObserver(this UIElement3D This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.StylusMove += h, h => This.StylusMove -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> PreviewStylusInAirMoveObserver(this UIElement3D This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.PreviewStylusInAirMove += h, h => This.PreviewStylusInAirMove -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> StylusInAirMoveObserver(this UIElement3D This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.StylusInAirMove += h, h => This.StylusInAirMove -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> StylusEnterObserver(this UIElement3D This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.StylusEnter += h, h => This.StylusEnter -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> StylusLeaveObserver(this UIElement3D This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.StylusLeave += h, h => This.StylusLeave -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> PreviewStylusInRangeObserver(this UIElement3D This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.PreviewStylusInRange += h, h => This.PreviewStylusInRange -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> StylusInRangeObserver(this UIElement3D This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.StylusInRange += h, h => This.StylusInRange -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> PreviewStylusOutOfRangeObserver(this UIElement3D This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.PreviewStylusOutOfRange += h, h => This.PreviewStylusOutOfRange -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> StylusOutOfRangeObserver(this UIElement3D This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.StylusOutOfRange += h, h => This.StylusOutOfRange -= h);
        }

        public static IObservable<EventPattern<StylusSystemGestureEventArgs>> PreviewStylusSystemGestureObserver(this UIElement3D This){
            return Observable.FromEventPattern<StylusSystemGestureEventHandler, StylusSystemGestureEventArgs>(h => This.PreviewStylusSystemGesture += h, h => This.PreviewStylusSystemGesture -= h);
        }

        public static IObservable<EventPattern<StylusSystemGestureEventArgs>> StylusSystemGestureObserver(this UIElement3D This){
            return Observable.FromEventPattern<StylusSystemGestureEventHandler, StylusSystemGestureEventArgs>(h => This.StylusSystemGesture += h, h => This.StylusSystemGesture -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> GotStylusCaptureObserver(this UIElement3D This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.GotStylusCapture += h, h => This.GotStylusCapture -= h);
        }

        public static IObservable<EventPattern<StylusEventArgs>> LostStylusCaptureObserver(this UIElement3D This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.LostStylusCapture += h, h => This.LostStylusCapture -= h);
        }

        public static IObservable<EventPattern<StylusButtonEventArgs>> StylusButtonDownObserver(this UIElement3D This){
            return Observable.FromEventPattern<StylusButtonEventHandler, StylusButtonEventArgs>(h => This.StylusButtonDown += h, h => This.StylusButtonDown -= h);
        }

        public static IObservable<EventPattern<StylusButtonEventArgs>> StylusButtonUpObserver(this UIElement3D This){
            return Observable.FromEventPattern<StylusButtonEventHandler, StylusButtonEventArgs>(h => This.StylusButtonUp += h, h => This.StylusButtonUp -= h);
        }

        public static IObservable<EventPattern<StylusButtonEventArgs>> PreviewStylusButtonDownObserver(this UIElement3D This){
            return Observable.FromEventPattern<StylusButtonEventHandler, StylusButtonEventArgs>(h => This.PreviewStylusButtonDown += h, h => This.PreviewStylusButtonDown -= h);
        }

        public static IObservable<EventPattern<StylusButtonEventArgs>> PreviewStylusButtonUpObserver(this UIElement3D This){
            return Observable.FromEventPattern<StylusButtonEventHandler, StylusButtonEventArgs>(h => This.PreviewStylusButtonUp += h, h => This.PreviewStylusButtonUp -= h);
        }

        public static IObservable<EventPattern<KeyEventArgs>> PreviewKeyDownObserver(this UIElement3D This){
            return Observable.FromEventPattern<KeyEventHandler, KeyEventArgs>(h => This.PreviewKeyDown += h, h => This.PreviewKeyDown -= h);
        }

        public static IObservable<EventPattern<KeyEventArgs>> KeyDownObserver(this UIElement3D This){
            return Observable.FromEventPattern<KeyEventHandler, KeyEventArgs>(h => This.KeyDown += h, h => This.KeyDown -= h);
        }

        public static IObservable<EventPattern<KeyEventArgs>> PreviewKeyUpObserver(this UIElement3D This){
            return Observable.FromEventPattern<KeyEventHandler, KeyEventArgs>(h => This.PreviewKeyUp += h, h => This.PreviewKeyUp -= h);
        }

        public static IObservable<EventPattern<KeyEventArgs>> KeyUpObserver(this UIElement3D This){
            return Observable.FromEventPattern<KeyEventHandler, KeyEventArgs>(h => This.KeyUp += h, h => This.KeyUp -= h);
        }

        public static IObservable<EventPattern<KeyboardFocusChangedEventArgs>> PreviewGotKeyboardFocusObserver(this UIElement3D This){
            return Observable.FromEventPattern<KeyboardFocusChangedEventHandler, KeyboardFocusChangedEventArgs>(h => This.PreviewGotKeyboardFocus += h, h => This.PreviewGotKeyboardFocus -= h);
        }

        public static IObservable<EventPattern<KeyboardFocusChangedEventArgs>> GotKeyboardFocusObserver(this UIElement3D This){
            return Observable.FromEventPattern<KeyboardFocusChangedEventHandler, KeyboardFocusChangedEventArgs>(h => This.GotKeyboardFocus += h, h => This.GotKeyboardFocus -= h);
        }

        public static IObservable<EventPattern<KeyboardFocusChangedEventArgs>> PreviewLostKeyboardFocusObserver(this UIElement3D This){
            return Observable.FromEventPattern<KeyboardFocusChangedEventHandler, KeyboardFocusChangedEventArgs>(h => This.PreviewLostKeyboardFocus += h, h => This.PreviewLostKeyboardFocus -= h);
        }

        public static IObservable<EventPattern<KeyboardFocusChangedEventArgs>> LostKeyboardFocusObserver(this UIElement3D This){
            return Observable.FromEventPattern<KeyboardFocusChangedEventHandler, KeyboardFocusChangedEventArgs>(h => This.LostKeyboardFocus += h, h => This.LostKeyboardFocus -= h);
        }

        public static IObservable<EventPattern<TextCompositionEventArgs>> PreviewTextInputObserver(this UIElement3D This){
            return Observable.FromEventPattern<TextCompositionEventHandler, TextCompositionEventArgs>(h => This.PreviewTextInput += h, h => This.PreviewTextInput -= h);
        }

        public static IObservable<EventPattern<TextCompositionEventArgs>> TextInputObserver(this UIElement3D This){
            return Observable.FromEventPattern<TextCompositionEventHandler, TextCompositionEventArgs>(h => This.TextInput += h, h => This.TextInput -= h);
        }

        public static IObservable<EventPattern<QueryContinueDragEventArgs>> PreviewQueryContinueDragObserver(this UIElement3D This){
            return Observable.FromEventPattern<QueryContinueDragEventHandler, QueryContinueDragEventArgs>(h => This.PreviewQueryContinueDrag += h, h => This.PreviewQueryContinueDrag -= h);
        }

        public static IObservable<EventPattern<QueryContinueDragEventArgs>> QueryContinueDragObserver(this UIElement3D This){
            return Observable.FromEventPattern<QueryContinueDragEventHandler, QueryContinueDragEventArgs>(h => This.QueryContinueDrag += h, h => This.QueryContinueDrag -= h);
        }

        public static IObservable<EventPattern<GiveFeedbackEventArgs>> PreviewGiveFeedbackObserver(this UIElement3D This){
            return Observable.FromEventPattern<GiveFeedbackEventHandler, GiveFeedbackEventArgs>(h => This.PreviewGiveFeedback += h, h => This.PreviewGiveFeedback -= h);
        }

        public static IObservable<EventPattern<GiveFeedbackEventArgs>> GiveFeedbackObserver(this UIElement3D This){
            return Observable.FromEventPattern<GiveFeedbackEventHandler, GiveFeedbackEventArgs>(h => This.GiveFeedback += h, h => This.GiveFeedback -= h);
        }

        public static IObservable<EventPattern<DragEventArgs>> PreviewDragEnterObserver(this UIElement3D This){
            return Observable.FromEventPattern<DragEventHandler, DragEventArgs>(h => This.PreviewDragEnter += h, h => This.PreviewDragEnter -= h);
        }

        public static IObservable<EventPattern<DragEventArgs>> DragEnterObserver(this UIElement3D This){
            return Observable.FromEventPattern<DragEventHandler, DragEventArgs>(h => This.DragEnter += h, h => This.DragEnter -= h);
        }

        public static IObservable<EventPattern<DragEventArgs>> PreviewDragOverObserver(this UIElement3D This){
            return Observable.FromEventPattern<DragEventHandler, DragEventArgs>(h => This.PreviewDragOver += h, h => This.PreviewDragOver -= h);
        }

        public static IObservable<EventPattern<DragEventArgs>> DragOverObserver(this UIElement3D This){
            return Observable.FromEventPattern<DragEventHandler, DragEventArgs>(h => This.DragOver += h, h => This.DragOver -= h);
        }

        public static IObservable<EventPattern<DragEventArgs>> PreviewDragLeaveObserver(this UIElement3D This){
            return Observable.FromEventPattern<DragEventHandler, DragEventArgs>(h => This.PreviewDragLeave += h, h => This.PreviewDragLeave -= h);
        }

        public static IObservable<EventPattern<DragEventArgs>> DragLeaveObserver(this UIElement3D This){
            return Observable.FromEventPattern<DragEventHandler, DragEventArgs>(h => This.DragLeave += h, h => This.DragLeave -= h);
        }

        public static IObservable<EventPattern<DragEventArgs>> PreviewDropObserver(this UIElement3D This){
            return Observable.FromEventPattern<DragEventHandler, DragEventArgs>(h => This.PreviewDrop += h, h => This.PreviewDrop -= h);
        }

        public static IObservable<EventPattern<DragEventArgs>> DropObserver(this UIElement3D This){
            return Observable.FromEventPattern<DragEventHandler, DragEventArgs>(h => This.Drop += h, h => This.Drop -= h);
        }

        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsMouseDirectlyOverChangedObserver(this UIElement3D This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsMouseDirectlyOverChanged += h, h => This.IsMouseDirectlyOverChanged -= h);
        }

        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsKeyboardFocusWithinChangedObserver(this UIElement3D This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsKeyboardFocusWithinChanged += h, h => This.IsKeyboardFocusWithinChanged -= h);
        }

        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsMouseCapturedChangedObserver(this UIElement3D This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsMouseCapturedChanged += h, h => This.IsMouseCapturedChanged -= h);
        }

        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsMouseCaptureWithinChangedObserver(this UIElement3D This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsMouseCaptureWithinChanged += h, h => This.IsMouseCaptureWithinChanged -= h);
        }

        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsStylusDirectlyOverChangedObserver(this UIElement3D This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsStylusDirectlyOverChanged += h, h => This.IsStylusDirectlyOverChanged -= h);
        }

        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsStylusCapturedChangedObserver(this UIElement3D This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsStylusCapturedChanged += h, h => This.IsStylusCapturedChanged -= h);
        }

        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsStylusCaptureWithinChangedObserver(this UIElement3D This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsStylusCaptureWithinChanged += h, h => This.IsStylusCaptureWithinChanged -= h);
        }

        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsKeyboardFocusedChangedObserver(this UIElement3D This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsKeyboardFocusedChanged += h, h => This.IsKeyboardFocusedChanged -= h);
        }

        public static IObservable<EventPattern<RoutedEventArgs>> GotFocusObserver(this UIElement3D This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.GotFocus += h, h => This.GotFocus -= h);
        }

        public static IObservable<EventPattern<RoutedEventArgs>> LostFocusObserver(this UIElement3D This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.LostFocus += h, h => This.LostFocus -= h);
        }

        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsEnabledChangedObserver(this UIElement3D This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsEnabledChanged += h, h => This.IsEnabledChanged -= h);
        }

        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsHitTestVisibleChangedObserver(this UIElement3D This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsHitTestVisibleChanged += h, h => This.IsHitTestVisibleChanged -= h);
        }

        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsVisibleChangedObserver(this UIElement3D This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsVisibleChanged += h, h => This.IsVisibleChanged -= h);
        }

        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> FocusableChangedObserver(this UIElement3D This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.FocusableChanged += h, h => This.FocusableChanged -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   PresentationSource
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<EventArgs>> ContentRenderedObserver(this PresentationSource This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.ContentRendered += h, h => This.ContentRendered -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   DocumentPage
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<EventArgs>> PageDestroyedObserver(this DocumentPage This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.PageDestroyed += h, h => This.PageDestroyed -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   DocumentPaginator
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<GetPageCompletedEventArgs>> GetPageCompletedObserver(this DocumentPaginator This){
            return Observable.FromEventPattern<GetPageCompletedEventHandler, GetPageCompletedEventArgs>(h => This.GetPageCompleted += h, h => This.GetPageCompleted -= h);
        }

        public static IObservable<EventPattern<AsyncCompletedEventArgs>> ComputePageCountCompletedObserver(this DocumentPaginator This){
            return Observable.FromEventPattern<AsyncCompletedEventHandler, AsyncCompletedEventArgs>(h => This.ComputePageCountCompleted += h, h => This.ComputePageCountCompleted -= h);
        }

        public static IObservable<EventPattern<PagesChangedEventArgs>> PagesChangedObserver(this DocumentPaginator This){
            return Observable.FromEventPattern<PagesChangedEventHandler, PagesChangedEventArgs>(h => This.PagesChanged += h, h => This.PagesChanged -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   DynamicDocumentPaginator
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<GetPageNumberCompletedEventArgs>> GetPageNumberCompletedObserver(this DynamicDocumentPaginator This){
            return Observable.FromEventPattern<GetPageNumberCompletedEventHandler, GetPageNumberCompletedEventArgs>(h => This.GetPageNumberCompleted += h, h => This.GetPageNumberCompleted -= h);
        }

        public static IObservable<EventPattern<EventArgs>> PaginationCompletedObserver(this DynamicDocumentPaginator This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.PaginationCompleted += h, h => This.PaginationCompleted -= h);
        }

        public static IObservable<EventPattern<PaginationProgressEventArgs>> PaginationProgressObserver(this DynamicDocumentPaginator This){
            return Observable.FromEventPattern<PaginationProgressEventHandler, PaginationProgressEventArgs>(h => This.PaginationProgress += h, h => This.PaginationProgress -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   DrawingAttributes
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<PropertyDataChangedEventArgs>> AttributeChangedObserver(this DrawingAttributes This){
            return Observable.FromEventPattern<PropertyDataChangedEventHandler, PropertyDataChangedEventArgs>(h => This.AttributeChanged += h, h => This.AttributeChanged -= h);
        }

        public static IObservable<EventPattern<PropertyDataChangedEventArgs>> PropertyDataChangedObserver(this DrawingAttributes This){
            return Observable.FromEventPattern<PropertyDataChangedEventHandler, PropertyDataChangedEventArgs>(h => This.PropertyDataChanged += h, h => This.PropertyDataChanged -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   IncrementalLassoHitTester
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<LassoSelectionChangedEventArgs>> SelectionChangedObserver(this IncrementalLassoHitTester This){
            return Observable.FromEventPattern<LassoSelectionChangedEventHandler, LassoSelectionChangedEventArgs>(h => This.SelectionChanged += h, h => This.SelectionChanged -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   IncrementalStrokeHitTester
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<StrokeHitEventArgs>> StrokeHitObserver(this IncrementalStrokeHitTester This){
            return Observable.FromEventPattern<StrokeHitEventHandler, StrokeHitEventArgs>(h => This.StrokeHit += h, h => This.StrokeHit -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   Stroke
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<PropertyDataChangedEventArgs>> DrawingAttributesChangedObserver(this Stroke This){
            return Observable.FromEventPattern<PropertyDataChangedEventHandler, PropertyDataChangedEventArgs>(h => This.DrawingAttributesChanged += h, h => This.DrawingAttributesChanged -= h);
        }

        public static IObservable<EventPattern<DrawingAttributesReplacedEventArgs>> DrawingAttributesReplacedObserver(this Stroke This){
            return Observable.FromEventPattern<DrawingAttributesReplacedEventHandler, DrawingAttributesReplacedEventArgs>(h => This.DrawingAttributesReplaced += h, h => This.DrawingAttributesReplaced -= h);
        }

        public static IObservable<EventPattern<StylusPointsReplacedEventArgs>> StylusPointsReplacedObserver(this Stroke This){
            return Observable.FromEventPattern<StylusPointsReplacedEventHandler, StylusPointsReplacedEventArgs>(h => This.StylusPointsReplaced += h, h => This.StylusPointsReplaced -= h);
        }

        public static IObservable<EventPattern<EventArgs>> StylusPointsChangedObserver(this Stroke This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.StylusPointsChanged += h, h => This.StylusPointsChanged -= h);
        }

        public static IObservable<EventPattern<PropertyDataChangedEventArgs>> PropertyDataChangedObserver(this Stroke This){
            return Observable.FromEventPattern<PropertyDataChangedEventHandler, PropertyDataChangedEventArgs>(h => This.PropertyDataChanged += h, h => This.PropertyDataChanged -= h);
        }

        public static IObservable<EventPattern<EventArgs>> InvalidatedObserver(this Stroke This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.Invalidated += h, h => This.Invalidated -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   StrokeCollection
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<StrokeCollectionChangedEventArgs>> StrokesChangedObserver(this StrokeCollection This){
            return Observable.FromEventPattern<StrokeCollectionChangedEventHandler, StrokeCollectionChangedEventArgs>(h => This.StrokesChanged += h, h => This.StrokesChanged -= h);
        }

        public static IObservable<EventPattern<PropertyDataChangedEventArgs>> PropertyDataChangedObserver(this StrokeCollection This){
            return Observable.FromEventPattern<PropertyDataChangedEventHandler, PropertyDataChangedEventArgs>(h => This.PropertyDataChanged += h, h => This.PropertyDataChanged -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   CommandBinding
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<ExecutedRoutedEventArgs>> PreviewExecutedObserver(this CommandBinding This){
            return Observable.FromEventPattern<ExecutedRoutedEventHandler, ExecutedRoutedEventArgs>(h => This.PreviewExecuted += h, h => This.PreviewExecuted -= h);
        }

        public static IObservable<EventPattern<ExecutedRoutedEventArgs>> ExecutedObserver(this CommandBinding This){
            return Observable.FromEventPattern<ExecutedRoutedEventHandler, ExecutedRoutedEventArgs>(h => This.Executed += h, h => This.Executed -= h);
        }

        public static IObservable<EventPattern<CanExecuteRoutedEventArgs>> PreviewCanExecuteObserver(this CommandBinding This){
            return Observable.FromEventPattern<CanExecuteRoutedEventHandler, CanExecuteRoutedEventArgs>(h => This.PreviewCanExecute += h, h => This.PreviewCanExecute -= h);
        }

        public static IObservable<EventPattern<CanExecuteRoutedEventArgs>> CanExecuteObserver(this CommandBinding This){
            return Observable.FromEventPattern<CanExecuteRoutedEventHandler, CanExecuteRoutedEventArgs>(h => This.CanExecute += h, h => This.CanExecute -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   RoutedCommand
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<EventArgs>> CanExecuteChangedObserver(this RoutedCommand This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.CanExecuteChanged += h, h => This.CanExecuteChanged -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   IManipulator
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<EventArgs>> UpdatedObserver(this IManipulator This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.Updated += h, h => This.Updated -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   InputLanguageManager
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<InputLanguageEventArgs>> InputLanguageChangedObserver(this InputLanguageManager This){
            return Observable.FromEventPattern<InputLanguageEventHandler, InputLanguageEventArgs>(h => This.InputLanguageChanged += h, h => This.InputLanguageChanged -= h);
        }

        public static IObservable<EventPattern<InputLanguageEventArgs>> InputLanguageChangingObserver(this InputLanguageManager This){
            return Observable.FromEventPattern<InputLanguageEventHandler, InputLanguageEventArgs>(h => This.InputLanguageChanging += h, h => This.InputLanguageChanging -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   InputManager
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<PreProcessInputEventArgs>> PreProcessInputObserver(this InputManager This){
            return Observable.FromEventPattern<PreProcessInputEventHandler, PreProcessInputEventArgs>(h => This.PreProcessInput += h, h => This.PreProcessInput -= h);
        }

        public static IObservable<EventPattern<NotifyInputEventArgs>> PreNotifyInputObserver(this InputManager This){
            return Observable.FromEventPattern<NotifyInputEventHandler, NotifyInputEventArgs>(h => This.PreNotifyInput += h, h => This.PreNotifyInput -= h);
        }

        public static IObservable<EventPattern<NotifyInputEventArgs>> PostNotifyInputObserver(this InputManager This){
            return Observable.FromEventPattern<NotifyInputEventHandler, NotifyInputEventArgs>(h => This.PostNotifyInput += h, h => This.PostNotifyInput -= h);
        }

        public static IObservable<EventPattern<ProcessInputEventArgs>> PostProcessInputObserver(this InputManager This){
            return Observable.FromEventPattern<ProcessInputEventHandler, ProcessInputEventArgs>(h => This.PostProcessInput += h, h => This.PostProcessInput -= h);
        }

        public static IObservable<EventPattern<EventArgs>> EnterMenuModeObserver(this InputManager This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.EnterMenuMode += h, h => This.EnterMenuMode -= h);
        }

        public static IObservable<EventPattern<EventArgs>> LeaveMenuModeObserver(this InputManager This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.LeaveMenuMode += h, h => This.LeaveMenuMode -= h);
        }

        public static IObservable<EventPattern<EventArgs>> HitTestInvalidatedAsyncObserver(this InputManager This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.HitTestInvalidatedAsync += h, h => This.HitTestInvalidatedAsync -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   InputMethod
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<InputMethodStateChangedEventArgs>> StateChangedObserver(this InputMethod This){
            return Observable.FromEventPattern<InputMethodStateChangedEventHandler, InputMethodStateChangedEventArgs>(h => This.StateChanged += h, h => This.StateChanged -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   TouchDevice
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<EventArgs>> ActivatedObserver(this TouchDevice This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.Activated += h, h => This.Activated -= h);
        }

        public static IObservable<EventPattern<EventArgs>> DeactivatedObserver(this TouchDevice This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.Deactivated += h, h => This.Deactivated -= h);
        }

        public static IObservable<EventPattern<EventArgs>> UpdatedObserver(this TouchDevice This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.Updated += h, h => This.Updated -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   StylusPointCollection
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<EventArgs>> ChangedObserver(this StylusPointCollection This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.Changed += h, h => This.Changed -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   D3DImage
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsFrontBufferAvailableChangedObserver(this D3DImage This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsFrontBufferAvailableChanged += h, h => This.IsFrontBufferAvailableChanged -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   HwndSource
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<EventArgs>> DisposedObserver(this HwndSource This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.Disposed += h, h => This.Disposed -= h);
        }

        public static IObservable<EventPattern<EventArgs>> SizeToContentChangedObserver(this HwndSource This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.SizeToContentChanged += h, h => This.SizeToContentChanged -= h);
        }

        public static IObservable<EventPattern<AutoResizedEventArgs>> AutoResizedObserver(this HwndSource This){
            return Observable.FromEventPattern<AutoResizedEventHandler, AutoResizedEventArgs>(h => This.AutoResized += h, h => This.AutoResized -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   Clock
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<EventArgs>> CompletedObserver(this Clock This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.Completed += h, h => This.Completed -= h);
        }

        public static IObservable<EventPattern<EventArgs>> CurrentGlobalSpeedInvalidatedObserver(this Clock This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.CurrentGlobalSpeedInvalidated += h, h => This.CurrentGlobalSpeedInvalidated -= h);
        }

        public static IObservable<EventPattern<EventArgs>> CurrentStateInvalidatedObserver(this Clock This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.CurrentStateInvalidated += h, h => This.CurrentStateInvalidated -= h);
        }

        public static IObservable<EventPattern<EventArgs>> CurrentTimeInvalidatedObserver(this Clock This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.CurrentTimeInvalidated += h, h => This.CurrentTimeInvalidated -= h);
        }

        public static IObservable<EventPattern<EventArgs>> RemoveRequestedObserver(this Clock This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.RemoveRequested += h, h => This.RemoveRequested -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   Timeline
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<EventArgs>> CurrentStateInvalidatedObserver(this Timeline This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.CurrentStateInvalidated += h, h => This.CurrentStateInvalidated -= h);
        }

        public static IObservable<EventPattern<EventArgs>> CurrentTimeInvalidatedObserver(this Timeline This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.CurrentTimeInvalidated += h, h => This.CurrentTimeInvalidated -= h);
        }

        public static IObservable<EventPattern<EventArgs>> CurrentGlobalSpeedInvalidatedObserver(this Timeline This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.CurrentGlobalSpeedInvalidated += h, h => This.CurrentGlobalSpeedInvalidated -= h);
        }

        public static IObservable<EventPattern<EventArgs>> CompletedObserver(this Timeline This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.Completed += h, h => This.Completed -= h);
        }

        public static IObservable<EventPattern<EventArgs>> RemoveRequestedObserver(this Timeline This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.RemoveRequested += h, h => This.RemoveRequested -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   BitmapDecoder
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<EventArgs>> DownloadCompletedObserver(this BitmapDecoder This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.DownloadCompleted += h, h => This.DownloadCompleted -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   BitmapSource
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<EventArgs>> DownloadCompletedObserver(this BitmapSource This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.DownloadCompleted += h, h => This.DownloadCompleted -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   MediaPlayer
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<EventArgs>> MediaOpenedObserver(this MediaPlayer This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.MediaOpened += h, h => This.MediaOpened -= h);
        }

        public static IObservable<EventPattern<EventArgs>> MediaEndedObserver(this MediaPlayer This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.MediaEnded += h, h => This.MediaEnded -= h);
        }

        public static IObservable<EventPattern<EventArgs>> BufferingStartedObserver(this MediaPlayer This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.BufferingStarted += h, h => This.BufferingStarted -= h);
        }

        public static IObservable<EventPattern<EventArgs>> BufferingEndedObserver(this MediaPlayer This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.BufferingEnded += h, h => This.BufferingEnded -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   FileDialog
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<CancelEventArgs>> FileOkObserver(this FileDialog This){
            return Observable.FromEventPattern<CancelEventHandler, CancelEventArgs>(h => This.FileOk += h, h => This.FileOk -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   FrameworkElement
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> DataContextChangedObserver(this FrameworkElement This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.DataContextChanged += h, h => This.DataContextChanged -= h);
        }

        public static IObservable<EventPattern<RequestBringIntoViewEventArgs>> RequestBringIntoViewObserver(this FrameworkElement This){
            return Observable.FromEventPattern<RequestBringIntoViewEventHandler, RequestBringIntoViewEventArgs>(h => This.RequestBringIntoView += h, h => This.RequestBringIntoView -= h);
        }

        public static IObservable<EventPattern<SizeChangedEventArgs>> SizeChangedObserver(this FrameworkElement This){
            return Observable.FromEventPattern<SizeChangedEventHandler, SizeChangedEventArgs>(h => This.SizeChanged += h, h => This.SizeChanged -= h);
        }

        public static IObservable<EventPattern<EventArgs>> InitializedObserver(this FrameworkElement This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.Initialized += h, h => This.Initialized -= h);
        }

        public static IObservable<EventPattern<RoutedEventArgs>> LoadedObserver(this FrameworkElement This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Loaded += h, h => This.Loaded -= h);
        }

        public static IObservable<EventPattern<RoutedEventArgs>> UnloadedObserver(this FrameworkElement This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Unloaded += h, h => This.Unloaded -= h);
        }

        public static IObservable<EventPattern<ToolTipEventArgs>> ToolTipOpeningObserver(this FrameworkElement This){
            return Observable.FromEventPattern<ToolTipEventHandler, ToolTipEventArgs>(h => This.ToolTipOpening += h, h => This.ToolTipOpening -= h);
        }

        public static IObservable<EventPattern<ToolTipEventArgs>> ToolTipClosingObserver(this FrameworkElement This){
            return Observable.FromEventPattern<ToolTipEventHandler, ToolTipEventArgs>(h => This.ToolTipClosing += h, h => This.ToolTipClosing -= h);
        }

        public static IObservable<EventPattern<ContextMenuEventArgs>> ContextMenuOpeningObserver(this FrameworkElement This){
            return Observable.FromEventPattern<ContextMenuEventHandler, ContextMenuEventArgs>(h => This.ContextMenuOpening += h, h => This.ContextMenuOpening -= h);
        }

        public static IObservable<EventPattern<ContextMenuEventArgs>> ContextMenuClosingObserver(this FrameworkElement This){
            return Observable.FromEventPattern<ContextMenuEventHandler, ContextMenuEventArgs>(h => This.ContextMenuClosing += h, h => This.ContextMenuClosing -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   Control
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<MouseButtonEventArgs>> PreviewMouseDoubleClickObserver(this Control This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.PreviewMouseDoubleClick += h, h => This.PreviewMouseDoubleClick -= h);
        }

        public static IObservable<EventPattern<MouseButtonEventArgs>> MouseDoubleClickObserver(this Control This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.MouseDoubleClick += h, h => This.MouseDoubleClick -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   Window
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<EventArgs>> SourceInitializedObserver(this Window This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.SourceInitialized += h, h => This.SourceInitialized -= h);
        }

        public static IObservable<EventPattern<EventArgs>> ActivatedObserver(this Window This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.Activated += h, h => This.Activated -= h);
        }

        public static IObservable<EventPattern<EventArgs>> DeactivatedObserver(this Window This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.Deactivated += h, h => This.Deactivated -= h);
        }

        public static IObservable<EventPattern<EventArgs>> StateChangedObserver(this Window This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.StateChanged += h, h => This.StateChanged -= h);
        }

        public static IObservable<EventPattern<EventArgs>> LocationChangedObserver(this Window This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.LocationChanged += h, h => This.LocationChanged -= h);
        }

        public static IObservable<EventPattern<CancelEventArgs>> ClosingObserver(this Window This){
            return Observable.FromEventPattern<CancelEventHandler, CancelEventArgs>(h => This.Closing += h, h => This.Closing -= h);
        }

        public static IObservable<EventPattern<EventArgs>> ClosedObserver(this Window This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.Closed += h, h => This.Closed -= h);
        }

        public static IObservable<EventPattern<EventArgs>> ContentRenderedObserver(this Window This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.ContentRendered += h, h => This.ContentRendered -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   NavigationWindow
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<NavigatingCancelEventArgs>> NavigatingObserver(this NavigationWindow This){
            return Observable.FromEventPattern<NavigatingCancelEventHandler, NavigatingCancelEventArgs>(h => This.Navigating += h, h => This.Navigating -= h);
        }

        public static IObservable<EventPattern<NavigationProgressEventArgs>> NavigationProgressObserver(this NavigationWindow This){
            return Observable.FromEventPattern<NavigationProgressEventHandler, NavigationProgressEventArgs>(h => This.NavigationProgress += h, h => This.NavigationProgress -= h);
        }

        public static IObservable<EventPattern<NavigationFailedEventArgs>> NavigationFailedObserver(this NavigationWindow This){
            return Observable.FromEventPattern<NavigationFailedEventHandler, NavigationFailedEventArgs>(h => This.NavigationFailed += h, h => This.NavigationFailed -= h);
        }

        public static IObservable<EventPattern<NavigationEventArgs>> NavigatedObserver(this NavigationWindow This){
            return Observable.FromEventPattern<NavigatedEventHandler, NavigationEventArgs>(h => This.Navigated += h, h => This.Navigated -= h);
        }

        public static IObservable<EventPattern<NavigationEventArgs>> LoadCompletedObserver(this NavigationWindow This){
            return Observable.FromEventPattern<LoadCompletedEventHandler, NavigationEventArgs>(h => This.LoadCompleted += h, h => This.LoadCompleted -= h);
        }

        public static IObservable<EventPattern<NavigationEventArgs>> NavigationStoppedObserver(this NavigationWindow This){
            return Observable.FromEventPattern<NavigationStoppedEventHandler, NavigationEventArgs>(h => This.NavigationStopped += h, h => This.NavigationStopped -= h);
        }

        public static IObservable<EventPattern<FragmentNavigationEventArgs>> FragmentNavigationObserver(this NavigationWindow This){
            return Observable.FromEventPattern<FragmentNavigationEventHandler, FragmentNavigationEventArgs>(h => This.FragmentNavigation += h, h => This.FragmentNavigation -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   Application
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<StartupEventArgs>> StartupObserver(this Application This){
            return Observable.FromEventPattern<StartupEventHandler, StartupEventArgs>(h => This.Startup += h, h => This.Startup -= h);
        }

        public static IObservable<EventPattern<ExitEventArgs>> ExitObserver(this Application This){
            return Observable.FromEventPattern<ExitEventHandler, ExitEventArgs>(h => This.Exit += h, h => This.Exit -= h);
        }

        public static IObservable<EventPattern<EventArgs>> ActivatedObserver(this Application This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.Activated += h, h => This.Activated -= h);
        }

        public static IObservable<EventPattern<EventArgs>> DeactivatedObserver(this Application This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.Deactivated += h, h => This.Deactivated -= h);
        }

        public static IObservable<EventPattern<SessionEndingCancelEventArgs>> SessionEndingObserver(this Application This){
            return Observable.FromEventPattern<SessionEndingCancelEventHandler, SessionEndingCancelEventArgs>(h => This.SessionEnding += h, h => This.SessionEnding -= h);
        }

        public static IObservable<EventPattern<DispatcherUnhandledExceptionEventArgs>> DispatcherUnhandledExceptionObserver(this Application This){
            return Observable.FromEventPattern<DispatcherUnhandledExceptionEventHandler, DispatcherUnhandledExceptionEventArgs>(h => This.DispatcherUnhandledException += h, h => This.DispatcherUnhandledException -= h);
        }

        public static IObservable<EventPattern<NavigatingCancelEventArgs>> NavigatingObserver(this Application This){
            return Observable.FromEventPattern<NavigatingCancelEventHandler, NavigatingCancelEventArgs>(h => This.Navigating += h, h => This.Navigating -= h);
        }

        public static IObservable<EventPattern<NavigationEventArgs>> NavigatedObserver(this Application This){
            return Observable.FromEventPattern<NavigatedEventHandler, NavigationEventArgs>(h => This.Navigated += h, h => This.Navigated -= h);
        }

        public static IObservable<EventPattern<NavigationProgressEventArgs>> NavigationProgressObserver(this Application This){
            return Observable.FromEventPattern<NavigationProgressEventHandler, NavigationProgressEventArgs>(h => This.NavigationProgress += h, h => This.NavigationProgress -= h);
        }

        public static IObservable<EventPattern<NavigationFailedEventArgs>> NavigationFailedObserver(this Application This){
            return Observable.FromEventPattern<NavigationFailedEventHandler, NavigationFailedEventArgs>(h => This.NavigationFailed += h, h => This.NavigationFailed -= h);
        }

        public static IObservable<EventPattern<NavigationEventArgs>> LoadCompletedObserver(this Application This){
            return Observable.FromEventPattern<LoadCompletedEventHandler, NavigationEventArgs>(h => This.LoadCompleted += h, h => This.LoadCompleted -= h);
        }

        public static IObservable<EventPattern<NavigationEventArgs>> NavigationStoppedObserver(this Application This){
            return Observable.FromEventPattern<NavigationStoppedEventHandler, NavigationEventArgs>(h => This.NavigationStopped += h, h => This.NavigationStopped -= h);
        }

        public static IObservable<EventPattern<FragmentNavigationEventArgs>> FragmentNavigationObserver(this Application This){
            return Observable.FromEventPattern<FragmentNavigationEventHandler, FragmentNavigationEventArgs>(h => This.FragmentNavigation += h, h => This.FragmentNavigation -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   CollectionView
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<CurrentChangingEventArgs>> CurrentChangingObserver(this CollectionView This){
            return Observable.FromEventPattern<CurrentChangingEventHandler, CurrentChangingEventArgs>(h => This.CurrentChanging += h, h => This.CurrentChanging -= h);
        }

        public static IObservable<EventPattern<EventArgs>> CurrentChangedObserver(this CollectionView This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.CurrentChanged += h, h => This.CurrentChanged -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   ContextMenu
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<RoutedEventArgs>> OpenedObserver(this ContextMenu This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Opened += h, h => This.Opened -= h);
        }

        public static IObservable<EventPattern<RoutedEventArgs>> ClosedObserver(this ContextMenu This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Closed += h, h => This.Closed -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   MenuItem
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<RoutedEventArgs>> ClickObserver(this MenuItem This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Click += h, h => This.Click -= h);
        }

        public static IObservable<EventPattern<RoutedEventArgs>> CheckedObserver(this MenuItem This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Checked += h, h => This.Checked -= h);
        }

        public static IObservable<EventPattern<RoutedEventArgs>> UncheckedObserver(this MenuItem This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Unchecked += h, h => This.Unchecked -= h);
        }

        public static IObservable<EventPattern<RoutedEventArgs>> SubmenuOpenedObserver(this MenuItem This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.SubmenuOpened += h, h => This.SubmenuOpened -= h);
        }

        public static IObservable<EventPattern<RoutedEventArgs>> SubmenuClosedObserver(this MenuItem This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.SubmenuClosed += h, h => This.SubmenuClosed -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   DocumentViewerBase
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<EventArgs>> PageViewsChangedObserver(this DocumentViewerBase This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.PageViewsChanged += h, h => This.PageViewsChanged -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   Annotation
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<AnnotationAuthorChangedEventArgs>> AuthorChangedObserver(this Annotation This){
            return Observable.FromEventPattern<AnnotationAuthorChangedEventHandler, AnnotationAuthorChangedEventArgs>(h => This.AuthorChanged += h, h => This.AuthorChanged -= h);
        }

        public static IObservable<EventPattern<AnnotationResourceChangedEventArgs>> AnchorChangedObserver(this Annotation This){
            return Observable.FromEventPattern<AnnotationResourceChangedEventHandler, AnnotationResourceChangedEventArgs>(h => This.AnchorChanged += h, h => This.AnchorChanged -= h);
        }

        public static IObservable<EventPattern<AnnotationResourceChangedEventArgs>> CargoChangedObserver(this Annotation This){
            return Observable.FromEventPattern<AnnotationResourceChangedEventHandler, AnnotationResourceChangedEventArgs>(h => This.CargoChanged += h, h => This.CargoChanged -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   AnnotationStore
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<StoreContentChangedEventArgs>> StoreContentChangedObserver(this AnnotationStore This){
            return Observable.FromEventPattern<StoreContentChangedEventHandler, StoreContentChangedEventArgs>(h => This.StoreContentChanged += h, h => This.StoreContentChanged -= h);
        }

        public static IObservable<EventPattern<AnnotationAuthorChangedEventArgs>> AuthorChangedObserver(this AnnotationStore This){
            return Observable.FromEventPattern<AnnotationAuthorChangedEventHandler, AnnotationAuthorChangedEventArgs>(h => This.AuthorChanged += h, h => This.AuthorChanged -= h);
        }

        public static IObservable<EventPattern<AnnotationResourceChangedEventArgs>> AnchorChangedObserver(this AnnotationStore This){
            return Observable.FromEventPattern<AnnotationResourceChangedEventHandler, AnnotationResourceChangedEventArgs>(h => This.AnchorChanged += h, h => This.AnchorChanged -= h);
        }

        public static IObservable<EventPattern<AnnotationResourceChangedEventArgs>> CargoChangedObserver(this AnnotationStore This){
            return Observable.FromEventPattern<AnnotationResourceChangedEventHandler, AnnotationResourceChangedEventArgs>(h => This.CargoChanged += h, h => This.CargoChanged -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   ButtonBase
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<RoutedEventArgs>> ClickObserver(this ButtonBase This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Click += h, h => This.Click -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   Calendar
////////////////////////////////////////////
////////////////////////////////////////////
 
////////////////////////////////////////////
////////////////////////////////////////////
////   CalendarDateRange
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<PropertyChangedEventArgs>> PropertyChangedObserver(this CalendarDateRange This){
            return Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(h => This.PropertyChanged += h, h => This.PropertyChanged -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   ToggleButton
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<RoutedEventArgs>> CheckedObserver(this ToggleButton This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Checked += h, h => This.Checked -= h);
        }

        public static IObservable<EventPattern<RoutedEventArgs>> UncheckedObserver(this ToggleButton This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Unchecked += h, h => This.Unchecked -= h);
        }

        public static IObservable<EventPattern<RoutedEventArgs>> IndeterminateObserver(this ToggleButton This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Indeterminate += h, h => This.Indeterminate -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   Selector
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<SelectionChangedEventArgs>> SelectionChangedObserver(this Selector This){
            return Observable.FromEventPattern<SelectionChangedEventHandler, SelectionChangedEventArgs>(h => This.SelectionChanged += h, h => This.SelectionChanged -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   ComboBox
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<EventArgs>> DropDownOpenedObserver(this ComboBox This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.DropDownOpened += h, h => This.DropDownOpened -= h);
        }

        public static IObservable<EventPattern<EventArgs>> DropDownClosedObserver(this ComboBox This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.DropDownClosed += h, h => This.DropDownClosed -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   ListBoxItem
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<RoutedEventArgs>> SelectedObserver(this ListBoxItem This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Selected += h, h => This.Selected -= h);
        }

        public static IObservable<EventPattern<RoutedEventArgs>> UnselectedObserver(this ListBoxItem This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Unselected += h, h => This.Unselected -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   DataGrid
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<InitializingNewItemEventArgs>> InitializingNewItemObserver(this DataGrid This){
            return Observable.FromEventPattern<InitializingNewItemEventHandler, InitializingNewItemEventArgs>(h => This.InitializingNewItem += h, h => This.InitializingNewItem -= h);
        }

        public static IObservable<EventPattern<SelectedCellsChangedEventArgs>> SelectedCellsChangedObserver(this DataGrid This){
            return Observable.FromEventPattern<SelectedCellsChangedEventHandler, SelectedCellsChangedEventArgs>(h => This.SelectedCellsChanged += h, h => This.SelectedCellsChanged -= h);
        }

        public static IObservable<EventPattern<DataGridSortingEventArgs>> SortingObserver(this DataGrid This){
            return Observable.FromEventPattern<DataGridSortingEventHandler, DataGridSortingEventArgs>(h => This.Sorting += h, h => This.Sorting -= h);
        }

        public static IObservable<EventPattern<EventArgs>> AutoGeneratedColumnsObserver(this DataGrid This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.AutoGeneratedColumns += h, h => This.AutoGeneratedColumns -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   DataGridColumn
////////////////////////////////////////////
////////////////////////////////////////////
 
////////////////////////////////////////////
////////////////////////////////////////////
////   DataGridCell
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<RoutedEventArgs>> SelectedObserver(this DataGridCell This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Selected += h, h => This.Selected -= h);
        }

        public static IObservable<EventPattern<RoutedEventArgs>> UnselectedObserver(this DataGridCell This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Unselected += h, h => This.Unselected -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   DataGridRow
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<RoutedEventArgs>> SelectedObserver(this DataGridRow This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Selected += h, h => This.Selected -= h);
        }

        public static IObservable<EventPattern<RoutedEventArgs>> UnselectedObserver(this DataGridRow This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Unselected += h, h => This.Unselected -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   DatePicker
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<RoutedEventArgs>> CalendarClosedObserver(this DatePicker This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.CalendarClosed += h, h => This.CalendarClosed -= h);
        }

        public static IObservable<EventPattern<RoutedEventArgs>> CalendarOpenedObserver(this DatePicker This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.CalendarOpened += h, h => This.CalendarOpened -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   FrameworkContentElement
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> DataContextChangedObserver(this FrameworkContentElement This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.DataContextChanged += h, h => This.DataContextChanged -= h);
        }

        public static IObservable<EventPattern<EventArgs>> InitializedObserver(this FrameworkContentElement This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.Initialized += h, h => This.Initialized -= h);
        }

        public static IObservable<EventPattern<RoutedEventArgs>> LoadedObserver(this FrameworkContentElement This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Loaded += h, h => This.Loaded -= h);
        }

        public static IObservable<EventPattern<RoutedEventArgs>> UnloadedObserver(this FrameworkContentElement This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Unloaded += h, h => This.Unloaded -= h);
        }

        public static IObservable<EventPattern<ToolTipEventArgs>> ToolTipOpeningObserver(this FrameworkContentElement This){
            return Observable.FromEventPattern<ToolTipEventHandler, ToolTipEventArgs>(h => This.ToolTipOpening += h, h => This.ToolTipOpening -= h);
        }

        public static IObservable<EventPattern<ToolTipEventArgs>> ToolTipClosingObserver(this FrameworkContentElement This){
            return Observable.FromEventPattern<ToolTipEventHandler, ToolTipEventArgs>(h => This.ToolTipClosing += h, h => This.ToolTipClosing -= h);
        }

        public static IObservable<EventPattern<ContextMenuEventArgs>> ContextMenuOpeningObserver(this FrameworkContentElement This){
            return Observable.FromEventPattern<ContextMenuEventHandler, ContextMenuEventArgs>(h => This.ContextMenuOpening += h, h => This.ContextMenuOpening -= h);
        }

        public static IObservable<EventPattern<ContextMenuEventArgs>> ContextMenuClosingObserver(this FrameworkContentElement This){
            return Observable.FromEventPattern<ContextMenuEventHandler, ContextMenuEventArgs>(h => This.ContextMenuClosing += h, h => This.ContextMenuClosing -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   Expander
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<RoutedEventArgs>> ExpandedObserver(this Expander This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Expanded += h, h => This.Expanded -= h);
        }

        public static IObservable<EventPattern<RoutedEventArgs>> CollapsedObserver(this Expander This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Collapsed += h, h => This.Collapsed -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   Frame
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<EventArgs>> ContentRenderedObserver(this Frame This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.ContentRendered += h, h => This.ContentRendered -= h);
        }

        public static IObservable<EventPattern<NavigatingCancelEventArgs>> NavigatingObserver(this Frame This){
            return Observable.FromEventPattern<NavigatingCancelEventHandler, NavigatingCancelEventArgs>(h => This.Navigating += h, h => This.Navigating -= h);
        }

        public static IObservable<EventPattern<NavigationProgressEventArgs>> NavigationProgressObserver(this Frame This){
            return Observable.FromEventPattern<NavigationProgressEventHandler, NavigationProgressEventArgs>(h => This.NavigationProgress += h, h => This.NavigationProgress -= h);
        }

        public static IObservable<EventPattern<NavigationFailedEventArgs>> NavigationFailedObserver(this Frame This){
            return Observable.FromEventPattern<NavigationFailedEventHandler, NavigationFailedEventArgs>(h => This.NavigationFailed += h, h => This.NavigationFailed -= h);
        }

        public static IObservable<EventPattern<NavigationEventArgs>> NavigatedObserver(this Frame This){
            return Observable.FromEventPattern<NavigatedEventHandler, NavigationEventArgs>(h => This.Navigated += h, h => This.Navigated -= h);
        }

        public static IObservable<EventPattern<NavigationEventArgs>> LoadCompletedObserver(this Frame This){
            return Observable.FromEventPattern<LoadCompletedEventHandler, NavigationEventArgs>(h => This.LoadCompleted += h, h => This.LoadCompleted -= h);
        }

        public static IObservable<EventPattern<NavigationEventArgs>> NavigationStoppedObserver(this Frame This){
            return Observable.FromEventPattern<NavigationStoppedEventHandler, NavigationEventArgs>(h => This.NavigationStopped += h, h => This.NavigationStopped -= h);
        }

        public static IObservable<EventPattern<FragmentNavigationEventArgs>> FragmentNavigationObserver(this Frame This){
            return Observable.FromEventPattern<FragmentNavigationEventHandler, FragmentNavigationEventArgs>(h => This.FragmentNavigation += h, h => This.FragmentNavigation -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   Thumb
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<DragStartedEventArgs>> DragStartedObserver(this Thumb This){
            return Observable.FromEventPattern<DragStartedEventHandler, DragStartedEventArgs>(h => This.DragStarted += h, h => This.DragStarted -= h);
        }

        public static IObservable<EventPattern<DragDeltaEventArgs>> DragDeltaObserver(this Thumb This){
            return Observable.FromEventPattern<DragDeltaEventHandler, DragDeltaEventArgs>(h => This.DragDelta += h, h => This.DragDelta -= h);
        }

        public static IObservable<EventPattern<DragCompletedEventArgs>> DragCompletedObserver(this Thumb This){
            return Observable.FromEventPattern<DragCompletedEventHandler, DragCompletedEventArgs>(h => This.DragCompleted += h, h => This.DragCompleted -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   Image
////////////////////////////////////////////
////////////////////////////////////////////
 
////////////////////////////////////////////
////////////////////////////////////////////
////   InkCanvas
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<InkCanvasStrokeCollectedEventArgs>> StrokeCollectedObserver(this InkCanvas This){
            return Observable.FromEventPattern<InkCanvasStrokeCollectedEventHandler, InkCanvasStrokeCollectedEventArgs>(h => This.StrokeCollected += h, h => This.StrokeCollected -= h);
        }

        public static IObservable<EventPattern<InkCanvasGestureEventArgs>> GestureObserver(this InkCanvas This){
            return Observable.FromEventPattern<InkCanvasGestureEventHandler, InkCanvasGestureEventArgs>(h => This.Gesture += h, h => This.Gesture -= h);
        }

        public static IObservable<EventPattern<InkCanvasStrokesReplacedEventArgs>> StrokesReplacedObserver(this InkCanvas This){
            return Observable.FromEventPattern<InkCanvasStrokesReplacedEventHandler, InkCanvasStrokesReplacedEventArgs>(h => This.StrokesReplaced += h, h => This.StrokesReplaced -= h);
        }

        public static IObservable<EventPattern<DrawingAttributesReplacedEventArgs>> DefaultDrawingAttributesReplacedObserver(this InkCanvas This){
            return Observable.FromEventPattern<DrawingAttributesReplacedEventHandler, DrawingAttributesReplacedEventArgs>(h => This.DefaultDrawingAttributesReplaced += h, h => This.DefaultDrawingAttributesReplaced -= h);
        }

        public static IObservable<EventPattern<RoutedEventArgs>> ActiveEditingModeChangedObserver(this InkCanvas This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.ActiveEditingModeChanged += h, h => This.ActiveEditingModeChanged -= h);
        }

        public static IObservable<EventPattern<RoutedEventArgs>> EditingModeChangedObserver(this InkCanvas This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.EditingModeChanged += h, h => This.EditingModeChanged -= h);
        }

        public static IObservable<EventPattern<RoutedEventArgs>> EditingModeInvertedChangedObserver(this InkCanvas This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.EditingModeInvertedChanged += h, h => This.EditingModeInvertedChanged -= h);
        }

        public static IObservable<EventPattern<InkCanvasSelectionEditingEventArgs>> SelectionMovingObserver(this InkCanvas This){
            return Observable.FromEventPattern<InkCanvasSelectionEditingEventHandler, InkCanvasSelectionEditingEventArgs>(h => This.SelectionMoving += h, h => This.SelectionMoving -= h);
        }

        public static IObservable<EventPattern<EventArgs>> SelectionMovedObserver(this InkCanvas This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.SelectionMoved += h, h => This.SelectionMoved -= h);
        }

        public static IObservable<EventPattern<InkCanvasStrokeErasingEventArgs>> StrokeErasingObserver(this InkCanvas This){
            return Observable.FromEventPattern<InkCanvasStrokeErasingEventHandler, InkCanvasStrokeErasingEventArgs>(h => This.StrokeErasing += h, h => This.StrokeErasing -= h);
        }

        public static IObservable<EventPattern<RoutedEventArgs>> StrokeErasedObserver(this InkCanvas This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.StrokeErased += h, h => This.StrokeErased -= h);
        }

        public static IObservable<EventPattern<InkCanvasSelectionEditingEventArgs>> SelectionResizingObserver(this InkCanvas This){
            return Observable.FromEventPattern<InkCanvasSelectionEditingEventHandler, InkCanvasSelectionEditingEventArgs>(h => This.SelectionResizing += h, h => This.SelectionResizing -= h);
        }

        public static IObservable<EventPattern<EventArgs>> SelectionResizedObserver(this InkCanvas This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.SelectionResized += h, h => This.SelectionResized -= h);
        }

        public static IObservable<EventPattern<InkCanvasSelectionChangingEventArgs>> SelectionChangingObserver(this InkCanvas This){
            return Observable.FromEventPattern<InkCanvasSelectionChangingEventHandler, InkCanvasSelectionChangingEventArgs>(h => This.SelectionChanging += h, h => This.SelectionChanging -= h);
        }

        public static IObservable<EventPattern<EventArgs>> SelectionChangedObserver(this InkCanvas This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.SelectionChanged += h, h => This.SelectionChanged -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   ItemContainerGenerator
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<ItemsChangedEventArgs>> ItemsChangedObserver(this ItemContainerGenerator This){
            return Observable.FromEventPattern<ItemsChangedEventHandler, ItemsChangedEventArgs>(h => This.ItemsChanged += h, h => This.ItemsChanged -= h);
        }

        public static IObservable<EventPattern<EventArgs>> StatusChangedObserver(this ItemContainerGenerator This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.StatusChanged += h, h => This.StatusChanged -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   MediaElement
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<RoutedEventArgs>> MediaOpenedObserver(this MediaElement This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.MediaOpened += h, h => This.MediaOpened -= h);
        }

        public static IObservable<EventPattern<RoutedEventArgs>> BufferingStartedObserver(this MediaElement This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.BufferingStarted += h, h => This.BufferingStarted -= h);
        }

        public static IObservable<EventPattern<RoutedEventArgs>> BufferingEndedObserver(this MediaElement This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.BufferingEnded += h, h => This.BufferingEnded -= h);
        }

        public static IObservable<EventPattern<RoutedEventArgs>> MediaEndedObserver(this MediaElement This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.MediaEnded += h, h => This.MediaEnded -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   PasswordBox
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<RoutedEventArgs>> PasswordChangedObserver(this PasswordBox This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.PasswordChanged += h, h => This.PasswordChanged -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   TextBoxBase
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<TextChangedEventArgs>> TextChangedObserver(this TextBoxBase This){
            return Observable.FromEventPattern<TextChangedEventHandler, TextChangedEventArgs>(h => This.TextChanged += h, h => This.TextChanged -= h);
        }

        public static IObservable<EventPattern<RoutedEventArgs>> SelectionChangedObserver(this TextBoxBase This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.SelectionChanged += h, h => This.SelectionChanged -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   DocumentPageView
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<EventArgs>> PageConnectedObserver(this DocumentPageView This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.PageConnected += h, h => This.PageConnected -= h);
        }

        public static IObservable<EventPattern<EventArgs>> PageDisconnectedObserver(this DocumentPageView This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.PageDisconnected += h, h => This.PageDisconnected -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   Popup
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<EventArgs>> OpenedObserver(this Popup This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.Opened += h, h => This.Opened -= h);
        }

        public static IObservable<EventPattern<EventArgs>> ClosedObserver(this Popup This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.Closed += h, h => This.Closed -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   RangeBase
////////////////////////////////////////////
////////////////////////////////////////////
 
////////////////////////////////////////////
////////////////////////////////////////////
////   ScrollBar
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<ScrollEventArgs>> ScrollObserver(this ScrollBar This){
            return Observable.FromEventPattern<ScrollEventHandler, ScrollEventArgs>(h => This.Scroll += h, h => This.Scroll -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   ScrollViewer
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<ScrollChangedEventArgs>> ScrollChangedObserver(this ScrollViewer This){
            return Observable.FromEventPattern<ScrollChangedEventHandler, ScrollChangedEventArgs>(h => This.ScrollChanged += h, h => This.ScrollChanged -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   ToolTip
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<RoutedEventArgs>> OpenedObserver(this ToolTip This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Opened += h, h => This.Opened -= h);
        }

        public static IObservable<EventPattern<RoutedEventArgs>> ClosedObserver(this ToolTip This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Closed += h, h => This.Closed -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   TreeView
////////////////////////////////////////////
////////////////////////////////////////////
 
////////////////////////////////////////////
////////////////////////////////////////////
////   TreeViewItem
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<RoutedEventArgs>> ExpandedObserver(this TreeViewItem This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Expanded += h, h => This.Expanded -= h);
        }

        public static IObservable<EventPattern<RoutedEventArgs>> CollapsedObserver(this TreeViewItem This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Collapsed += h, h => This.Collapsed -= h);
        }

        public static IObservable<EventPattern<RoutedEventArgs>> SelectedObserver(this TreeViewItem This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Selected += h, h => This.Selected -= h);
        }

        public static IObservable<EventPattern<RoutedEventArgs>> UnselectedObserver(this TreeViewItem This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Unselected += h, h => This.Unselected -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   HwndHost
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<Int32>> MessageHookObserver(this HwndHost This){
            return Observable.FromEventPattern<HwndSourceHook, Int32>(h => This.MessageHook += h, h => This.MessageHook -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   WebBrowser
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<NavigatingCancelEventArgs>> NavigatingObserver(this WebBrowser This){
            return Observable.FromEventPattern<NavigatingCancelEventHandler, NavigatingCancelEventArgs>(h => This.Navigating += h, h => This.Navigating -= h);
        }

        public static IObservable<EventPattern<NavigationEventArgs>> NavigatedObserver(this WebBrowser This){
            return Observable.FromEventPattern<NavigatedEventHandler, NavigationEventArgs>(h => This.Navigated += h, h => This.Navigated -= h);
        }

        public static IObservable<EventPattern<NavigationEventArgs>> LoadCompletedObserver(this WebBrowser This){
            return Observable.FromEventPattern<LoadCompletedEventHandler, NavigationEventArgs>(h => This.LoadCompleted += h, h => This.LoadCompleted -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   CollectionViewSource
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<FilterEventArgs>> FilterObserver(this CollectionViewSource This){
            return Observable.FromEventPattern<FilterEventHandler, FilterEventArgs>(h => This.Filter += h, h => This.Filter -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   DocumentReferenceCollection
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<NotifyCollectionChangedEventArgs>> CollectionChangedObserver(this DocumentReferenceCollection This){
            return Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(h => This.CollectionChanged += h, h => This.CollectionChanged -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   Hyperlink
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<RequestNavigateEventArgs>> RequestNavigateObserver(this Hyperlink This){
            return Observable.FromEventPattern<RequestNavigateEventHandler, RequestNavigateEventArgs>(h => This.RequestNavigate += h, h => This.RequestNavigate -= h);
        }

        public static IObservable<EventPattern<RoutedEventArgs>> ClickObserver(this Hyperlink This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Click += h, h => This.Click -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   PageContent
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<GetPageRootCompletedEventArgs>> GetPageRootCompletedObserver(this PageContent This){
            return Observable.FromEventPattern<GetPageRootCompletedEventHandler, GetPageRootCompletedEventArgs>(h => This.GetPageRootCompleted += h, h => This.GetPageRootCompleted -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   TextRange
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<EventArgs>> ChangedObserver(this TextRange This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.Changed += h, h => This.Changed -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   SerializerWriter
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<WritingPrintTicketRequiredEventArgs>> WritingPrintTicketRequiredObserver(this SerializerWriter This){
            return Observable.FromEventPattern<WritingPrintTicketRequiredEventHandler, WritingPrintTicketRequiredEventArgs>(h => This.WritingPrintTicketRequired += h, h => This.WritingPrintTicketRequired -= h);
        }

        public static IObservable<EventPattern<WritingProgressChangedEventArgs>> WritingProgressChangedObserver(this SerializerWriter This){
            return Observable.FromEventPattern<WritingProgressChangedEventHandler, WritingProgressChangedEventArgs>(h => This.WritingProgressChanged += h, h => This.WritingProgressChanged -= h);
        }

        public static IObservable<EventPattern<WritingCompletedEventArgs>> WritingCompletedObserver(this SerializerWriter This){
            return Observable.FromEventPattern<WritingCompletedEventHandler, WritingCompletedEventArgs>(h => This.WritingCompleted += h, h => This.WritingCompleted -= h);
        }

        public static IObservable<EventPattern<WritingCancelledEventArgs>> WritingCancelledObserver(this SerializerWriter This){
            return Observable.FromEventPattern<WritingCancelledEventHandler, WritingCancelledEventArgs>(h => This.WritingCancelled += h, h => This.WritingCancelled -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   NavigationService
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<NavigationFailedEventArgs>> NavigationFailedObserver(this NavigationService This){
            return Observable.FromEventPattern<NavigationFailedEventHandler, NavigationFailedEventArgs>(h => This.NavigationFailed += h, h => This.NavigationFailed -= h);
        }

        public static IObservable<EventPattern<NavigatingCancelEventArgs>> NavigatingObserver(this NavigationService This){
            return Observable.FromEventPattern<NavigatingCancelEventHandler, NavigatingCancelEventArgs>(h => This.Navigating += h, h => This.Navigating -= h);
        }

        public static IObservable<EventPattern<NavigationEventArgs>> NavigatedObserver(this NavigationService This){
            return Observable.FromEventPattern<NavigatedEventHandler, NavigationEventArgs>(h => This.Navigated += h, h => This.Navigated -= h);
        }

        public static IObservable<EventPattern<NavigationProgressEventArgs>> NavigationProgressObserver(this NavigationService This){
            return Observable.FromEventPattern<NavigationProgressEventHandler, NavigationProgressEventArgs>(h => This.NavigationProgress += h, h => This.NavigationProgress -= h);
        }

        public static IObservable<EventPattern<NavigationEventArgs>> LoadCompletedObserver(this NavigationService This){
            return Observable.FromEventPattern<LoadCompletedEventHandler, NavigationEventArgs>(h => This.LoadCompleted += h, h => This.LoadCompleted -= h);
        }

        public static IObservable<EventPattern<FragmentNavigationEventArgs>> FragmentNavigationObserver(this NavigationService This){
            return Observable.FromEventPattern<FragmentNavigationEventHandler, FragmentNavigationEventArgs>(h => This.FragmentNavigation += h, h => This.FragmentNavigation -= h);
        }

        public static IObservable<EventPattern<NavigationEventArgs>> NavigationStoppedObserver(this NavigationService This){
            return Observable.FromEventPattern<NavigationStoppedEventHandler, NavigationEventArgs>(h => This.NavigationStopped += h, h => This.NavigationStopped -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   PageFunction`1
////////////////////////////////////////////
////////////////////////////////////////////
 
////////////////////////////////////////////
////////////////////////////////////////////
////   XamlReader
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<AsyncCompletedEventArgs>> LoadCompletedObserver(this XamlReader This){
            return Observable.FromEventPattern<AsyncCompletedEventHandler, AsyncCompletedEventArgs>(h => This.LoadCompleted += h, h => This.LoadCompleted -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   BamlLocalizer
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<BamlLocalizerErrorNotifyEventArgs>> ErrorNotifyObserver(this BamlLocalizer This){
            return Observable.FromEventPattern<BamlLocalizerErrorNotifyEventHandler, BamlLocalizerErrorNotifyEventArgs>(h => This.ErrorNotify += h, h => This.ErrorNotify -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   JumpList
////////////////////////////////////////////
////////////////////////////////////////////
 
////////////////////////////////////////////
////////////////////////////////////////////
////   ThumbButtonInfo
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<EventArgs>> ClickObserver(this ThumbButtonInfo This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.Click += h, h => This.Click -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   VisualStateGroup
////////////////////////////////////////////
////////////////////////////////////////////
 
////////////////////////////////////////////
////////////////////////////////////////////
////   ICollectionView
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<CurrentChangingEventArgs>> CurrentChangingObserver(this ICollectionView This){
            return Observable.FromEventPattern<CurrentChangingEventHandler, CurrentChangingEventArgs>(h => This.CurrentChanging += h, h => This.CurrentChanging -= h);
        }

        public static IObservable<EventPattern<EventArgs>> CurrentChangedObserver(this ICollectionView This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.CurrentChanged += h, h => This.CurrentChanged -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   PackageDigitalSignatureManager
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<SignatureVerificationEventArgs>> InvalidSignatureEventObserver(this PackageDigitalSignatureManager This){
            return Observable.FromEventPattern<InvalidSignatureEventHandler, SignatureVerificationEventArgs>(h => This.InvalidSignatureEvent += h, h => This.InvalidSignatureEvent -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   Freezable
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<EventArgs>> ChangedObserver(this Freezable This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.Changed += h, h => This.Changed -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   DataSourceProvider
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<EventArgs>> DataChangedObserver(this DataSourceProvider This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.DataChanged += h, h => This.DataChanged -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   Dispatcher
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<EventArgs>> ShutdownStartedObserver(this Dispatcher This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.ShutdownStarted += h, h => This.ShutdownStarted -= h);
        }

        public static IObservable<EventPattern<EventArgs>> ShutdownFinishedObserver(this Dispatcher This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.ShutdownFinished += h, h => This.ShutdownFinished -= h);
        }

        public static IObservable<EventPattern<DispatcherUnhandledExceptionFilterEventArgs>> UnhandledExceptionFilterObserver(this Dispatcher This){
            return Observable.FromEventPattern<DispatcherUnhandledExceptionFilterEventHandler, DispatcherUnhandledExceptionFilterEventArgs>(h => This.UnhandledExceptionFilter += h, h => This.UnhandledExceptionFilter -= h);
        }

        public static IObservable<EventPattern<DispatcherUnhandledExceptionEventArgs>> UnhandledExceptionObserver(this Dispatcher This){
            return Observable.FromEventPattern<DispatcherUnhandledExceptionEventHandler, DispatcherUnhandledExceptionEventArgs>(h => This.UnhandledException += h, h => This.UnhandledException -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   DispatcherHooks
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<EventArgs>> DispatcherInactiveObserver(this DispatcherHooks This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.DispatcherInactive += h, h => This.DispatcherInactive -= h);
        }

        public static IObservable<EventPattern<DispatcherHookEventArgs>> OperationPostedObserver(this DispatcherHooks This){
            return Observable.FromEventPattern<DispatcherHookEventHandler, DispatcherHookEventArgs>(h => This.OperationPosted += h, h => This.OperationPosted -= h);
        }

        public static IObservable<EventPattern<DispatcherHookEventArgs>> OperationStartedObserver(this DispatcherHooks This){
            return Observable.FromEventPattern<DispatcherHookEventHandler, DispatcherHookEventArgs>(h => This.OperationStarted += h, h => This.OperationStarted -= h);
        }

        public static IObservable<EventPattern<DispatcherHookEventArgs>> OperationCompletedObserver(this DispatcherHooks This){
            return Observable.FromEventPattern<DispatcherHookEventHandler, DispatcherHookEventArgs>(h => This.OperationCompleted += h, h => This.OperationCompleted -= h);
        }

        public static IObservable<EventPattern<DispatcherHookEventArgs>> OperationPriorityChangedObserver(this DispatcherHooks This){
            return Observable.FromEventPattern<DispatcherHookEventHandler, DispatcherHookEventArgs>(h => This.OperationPriorityChanged += h, h => This.OperationPriorityChanged -= h);
        }

        public static IObservable<EventPattern<DispatcherHookEventArgs>> OperationAbortedObserver(this DispatcherHooks This){
            return Observable.FromEventPattern<DispatcherHookEventHandler, DispatcherHookEventArgs>(h => This.OperationAborted += h, h => This.OperationAborted -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   DispatcherOperation
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<EventArgs>> AbortedObserver(this DispatcherOperation This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.Aborted += h, h => This.Aborted -= h);
        }

        public static IObservable<EventPattern<EventArgs>> CompletedObserver(this DispatcherOperation This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.Completed += h, h => This.Completed -= h);
        }
 
////////////////////////////////////////////
////////////////////////////////////////////
////   DispatcherTimer
////////////////////////////////////////////
////////////////////////////////////////////

        public static IObservable<EventPattern<EventArgs>> TickObserver(this DispatcherTimer This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.Tick += h, h => This.Tick -= h);
        }

    }
}