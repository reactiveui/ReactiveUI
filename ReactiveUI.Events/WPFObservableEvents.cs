




















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



        public static IObservable<EventPattern<MouseButtonEventArgs>> PreviewMouseLeftButtonDownObservable(this IInputElement This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.PreviewMouseLeftButtonDown += h, h => This.PreviewMouseLeftButtonDown -= h);
        }



        public static IObservable<EventPattern<MouseButtonEventArgs>> MouseLeftButtonDownObservable(this IInputElement This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.MouseLeftButtonDown += h, h => This.MouseLeftButtonDown -= h);
        }



        public static IObservable<EventPattern<MouseButtonEventArgs>> PreviewMouseLeftButtonUpObservable(this IInputElement This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.PreviewMouseLeftButtonUp += h, h => This.PreviewMouseLeftButtonUp -= h);
        }



        public static IObservable<EventPattern<MouseButtonEventArgs>> MouseLeftButtonUpObservable(this IInputElement This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.MouseLeftButtonUp += h, h => This.MouseLeftButtonUp -= h);
        }



        public static IObservable<EventPattern<MouseButtonEventArgs>> PreviewMouseRightButtonDownObservable(this IInputElement This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.PreviewMouseRightButtonDown += h, h => This.PreviewMouseRightButtonDown -= h);
        }



        public static IObservable<EventPattern<MouseButtonEventArgs>> MouseRightButtonDownObservable(this IInputElement This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.MouseRightButtonDown += h, h => This.MouseRightButtonDown -= h);
        }



        public static IObservable<EventPattern<MouseButtonEventArgs>> PreviewMouseRightButtonUpObservable(this IInputElement This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.PreviewMouseRightButtonUp += h, h => This.PreviewMouseRightButtonUp -= h);
        }



        public static IObservable<EventPattern<MouseButtonEventArgs>> MouseRightButtonUpObservable(this IInputElement This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.MouseRightButtonUp += h, h => This.MouseRightButtonUp -= h);
        }



        public static IObservable<EventPattern<MouseEventArgs>> PreviewMouseMoveObservable(this IInputElement This){
            return Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(h => This.PreviewMouseMove += h, h => This.PreviewMouseMove -= h);
        }



        public static IObservable<EventPattern<MouseEventArgs>> MouseMoveObservable(this IInputElement This){
            return Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(h => This.MouseMove += h, h => This.MouseMove -= h);
        }



        public static IObservable<EventPattern<MouseWheelEventArgs>> PreviewMouseWheelObservable(this IInputElement This){
            return Observable.FromEventPattern<MouseWheelEventHandler, MouseWheelEventArgs>(h => This.PreviewMouseWheel += h, h => This.PreviewMouseWheel -= h);
        }



        public static IObservable<EventPattern<MouseWheelEventArgs>> MouseWheelObservable(this IInputElement This){
            return Observable.FromEventPattern<MouseWheelEventHandler, MouseWheelEventArgs>(h => This.MouseWheel += h, h => This.MouseWheel -= h);
        }



        public static IObservable<EventPattern<MouseEventArgs>> MouseEnterObservable(this IInputElement This){
            return Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(h => This.MouseEnter += h, h => This.MouseEnter -= h);
        }



        public static IObservable<EventPattern<MouseEventArgs>> MouseLeaveObservable(this IInputElement This){
            return Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(h => This.MouseLeave += h, h => This.MouseLeave -= h);
        }



        public static IObservable<EventPattern<MouseEventArgs>> GotMouseCaptureObservable(this IInputElement This){
            return Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(h => This.GotMouseCapture += h, h => This.GotMouseCapture -= h);
        }



        public static IObservable<EventPattern<MouseEventArgs>> LostMouseCaptureObservable(this IInputElement This){
            return Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(h => This.LostMouseCapture += h, h => This.LostMouseCapture -= h);
        }



        public static IObservable<EventPattern<StylusDownEventArgs>> PreviewStylusDownObservable(this IInputElement This){
            return Observable.FromEventPattern<StylusDownEventHandler, StylusDownEventArgs>(h => This.PreviewStylusDown += h, h => This.PreviewStylusDown -= h);
        }



        public static IObservable<EventPattern<StylusDownEventArgs>> StylusDownObservable(this IInputElement This){
            return Observable.FromEventPattern<StylusDownEventHandler, StylusDownEventArgs>(h => This.StylusDown += h, h => This.StylusDown -= h);
        }



        public static IObservable<EventPattern<StylusEventArgs>> PreviewStylusUpObservable(this IInputElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.PreviewStylusUp += h, h => This.PreviewStylusUp -= h);
        }



        public static IObservable<EventPattern<StylusEventArgs>> StylusUpObservable(this IInputElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.StylusUp += h, h => This.StylusUp -= h);
        }



        public static IObservable<EventPattern<StylusEventArgs>> PreviewStylusMoveObservable(this IInputElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.PreviewStylusMove += h, h => This.PreviewStylusMove -= h);
        }



        public static IObservable<EventPattern<StylusEventArgs>> StylusMoveObservable(this IInputElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.StylusMove += h, h => This.StylusMove -= h);
        }



        public static IObservable<EventPattern<StylusEventArgs>> PreviewStylusInAirMoveObservable(this IInputElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.PreviewStylusInAirMove += h, h => This.PreviewStylusInAirMove -= h);
        }



        public static IObservable<EventPattern<StylusEventArgs>> StylusInAirMoveObservable(this IInputElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.StylusInAirMove += h, h => This.StylusInAirMove -= h);
        }



        public static IObservable<EventPattern<StylusEventArgs>> StylusEnterObservable(this IInputElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.StylusEnter += h, h => This.StylusEnter -= h);
        }



        public static IObservable<EventPattern<StylusEventArgs>> StylusLeaveObservable(this IInputElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.StylusLeave += h, h => This.StylusLeave -= h);
        }



        public static IObservable<EventPattern<StylusEventArgs>> PreviewStylusInRangeObservable(this IInputElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.PreviewStylusInRange += h, h => This.PreviewStylusInRange -= h);
        }



        public static IObservable<EventPattern<StylusEventArgs>> StylusInRangeObservable(this IInputElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.StylusInRange += h, h => This.StylusInRange -= h);
        }



        public static IObservable<EventPattern<StylusEventArgs>> PreviewStylusOutOfRangeObservable(this IInputElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.PreviewStylusOutOfRange += h, h => This.PreviewStylusOutOfRange -= h);
        }



        public static IObservable<EventPattern<StylusEventArgs>> StylusOutOfRangeObservable(this IInputElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.StylusOutOfRange += h, h => This.StylusOutOfRange -= h);
        }



        public static IObservable<EventPattern<StylusSystemGestureEventArgs>> PreviewStylusSystemGestureObservable(this IInputElement This){
            return Observable.FromEventPattern<StylusSystemGestureEventHandler, StylusSystemGestureEventArgs>(h => This.PreviewStylusSystemGesture += h, h => This.PreviewStylusSystemGesture -= h);
        }



        public static IObservable<EventPattern<StylusSystemGestureEventArgs>> StylusSystemGestureObservable(this IInputElement This){
            return Observable.FromEventPattern<StylusSystemGestureEventHandler, StylusSystemGestureEventArgs>(h => This.StylusSystemGesture += h, h => This.StylusSystemGesture -= h);
        }



        public static IObservable<EventPattern<StylusButtonEventArgs>> StylusButtonDownObservable(this IInputElement This){
            return Observable.FromEventPattern<StylusButtonEventHandler, StylusButtonEventArgs>(h => This.StylusButtonDown += h, h => This.StylusButtonDown -= h);
        }



        public static IObservable<EventPattern<StylusButtonEventArgs>> PreviewStylusButtonDownObservable(this IInputElement This){
            return Observable.FromEventPattern<StylusButtonEventHandler, StylusButtonEventArgs>(h => This.PreviewStylusButtonDown += h, h => This.PreviewStylusButtonDown -= h);
        }



        public static IObservable<EventPattern<StylusButtonEventArgs>> PreviewStylusButtonUpObservable(this IInputElement This){
            return Observable.FromEventPattern<StylusButtonEventHandler, StylusButtonEventArgs>(h => This.PreviewStylusButtonUp += h, h => This.PreviewStylusButtonUp -= h);
        }



        public static IObservable<EventPattern<StylusButtonEventArgs>> StylusButtonUpObservable(this IInputElement This){
            return Observable.FromEventPattern<StylusButtonEventHandler, StylusButtonEventArgs>(h => This.StylusButtonUp += h, h => This.StylusButtonUp -= h);
        }



        public static IObservable<EventPattern<StylusEventArgs>> GotStylusCaptureObservable(this IInputElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.GotStylusCapture += h, h => This.GotStylusCapture -= h);
        }



        public static IObservable<EventPattern<StylusEventArgs>> LostStylusCaptureObservable(this IInputElement This){
            return Observable.FromEventPattern<StylusEventHandler, StylusEventArgs>(h => This.LostStylusCapture += h, h => This.LostStylusCapture -= h);
        }



        public static IObservable<EventPattern<KeyEventArgs>> PreviewKeyDownObservable(this IInputElement This){
            return Observable.FromEventPattern<KeyEventHandler, KeyEventArgs>(h => This.PreviewKeyDown += h, h => This.PreviewKeyDown -= h);
        }



        public static IObservable<EventPattern<KeyEventArgs>> KeyDownObservable(this IInputElement This){
            return Observable.FromEventPattern<KeyEventHandler, KeyEventArgs>(h => This.KeyDown += h, h => This.KeyDown -= h);
        }



        public static IObservable<EventPattern<KeyEventArgs>> PreviewKeyUpObservable(this IInputElement This){
            return Observable.FromEventPattern<KeyEventHandler, KeyEventArgs>(h => This.PreviewKeyUp += h, h => This.PreviewKeyUp -= h);
        }



        public static IObservable<EventPattern<KeyEventArgs>> KeyUpObservable(this IInputElement This){
            return Observable.FromEventPattern<KeyEventHandler, KeyEventArgs>(h => This.KeyUp += h, h => This.KeyUp -= h);
        }



        public static IObservable<EventPattern<KeyboardFocusChangedEventArgs>> PreviewGotKeyboardFocusObservable(this IInputElement This){
            return Observable.FromEventPattern<KeyboardFocusChangedEventHandler, KeyboardFocusChangedEventArgs>(h => This.PreviewGotKeyboardFocus += h, h => This.PreviewGotKeyboardFocus -= h);
        }



        public static IObservable<EventPattern<KeyboardFocusChangedEventArgs>> GotKeyboardFocusObservable(this IInputElement This){
            return Observable.FromEventPattern<KeyboardFocusChangedEventHandler, KeyboardFocusChangedEventArgs>(h => This.GotKeyboardFocus += h, h => This.GotKeyboardFocus -= h);
        }



        public static IObservable<EventPattern<KeyboardFocusChangedEventArgs>> PreviewLostKeyboardFocusObservable(this IInputElement This){
            return Observable.FromEventPattern<KeyboardFocusChangedEventHandler, KeyboardFocusChangedEventArgs>(h => This.PreviewLostKeyboardFocus += h, h => This.PreviewLostKeyboardFocus -= h);
        }



        public static IObservable<EventPattern<KeyboardFocusChangedEventArgs>> LostKeyboardFocusObservable(this IInputElement This){
            return Observable.FromEventPattern<KeyboardFocusChangedEventHandler, KeyboardFocusChangedEventArgs>(h => This.LostKeyboardFocus += h, h => This.LostKeyboardFocus -= h);
        }



        public static IObservable<EventPattern<TextCompositionEventArgs>> PreviewTextInputObservable(this IInputElement This){
            return Observable.FromEventPattern<TextCompositionEventHandler, TextCompositionEventArgs>(h => This.PreviewTextInput += h, h => This.PreviewTextInput -= h);
        }



        public static IObservable<EventPattern<TextCompositionEventArgs>> TextInputObservable(this IInputElement This){
            return Observable.FromEventPattern<TextCompositionEventHandler, TextCompositionEventArgs>(h => This.TextInput += h, h => This.TextInput -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   ContentElement
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<RoutedEventArgs>> GotFocusObservable(this ContentElement This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.GotFocus += h, h => This.GotFocus -= h);
        }



        public static IObservable<EventPattern<RoutedEventArgs>> LostFocusObservable(this ContentElement This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.LostFocus += h, h => This.LostFocus -= h);
        }



        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsEnabledChangedObservable(this ContentElement This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsEnabledChanged += h, h => This.IsEnabledChanged -= h);
        }



        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> FocusableChangedObservable(this ContentElement This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.FocusableChanged += h, h => This.FocusableChanged -= h);
        }



        public static IObservable<EventPattern<MouseButtonEventArgs>> PreviewMouseDownObservable(this ContentElement This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.PreviewMouseDown += h, h => This.PreviewMouseDown -= h);
        }



        public static IObservable<EventPattern<MouseButtonEventArgs>> MouseDownObservable(this ContentElement This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.MouseDown += h, h => This.MouseDown -= h);
        }



        public static IObservable<EventPattern<MouseButtonEventArgs>> PreviewMouseUpObservable(this ContentElement This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.PreviewMouseUp += h, h => This.PreviewMouseUp -= h);
        }



        public static IObservable<EventPattern<MouseButtonEventArgs>> MouseUpObservable(this ContentElement This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.MouseUp += h, h => This.MouseUp -= h);
        }



        public static IObservable<EventPattern<QueryCursorEventArgs>> QueryCursorObservable(this ContentElement This){
            return Observable.FromEventPattern<QueryCursorEventHandler, QueryCursorEventArgs>(h => This.QueryCursor += h, h => This.QueryCursor -= h);
        }



        public static IObservable<EventPattern<QueryContinueDragEventArgs>> PreviewQueryContinueDragObservable(this ContentElement This){
            return Observable.FromEventPattern<QueryContinueDragEventHandler, QueryContinueDragEventArgs>(h => This.PreviewQueryContinueDrag += h, h => This.PreviewQueryContinueDrag -= h);
        }



        public static IObservable<EventPattern<QueryContinueDragEventArgs>> QueryContinueDragObservable(this ContentElement This){
            return Observable.FromEventPattern<QueryContinueDragEventHandler, QueryContinueDragEventArgs>(h => This.QueryContinueDrag += h, h => This.QueryContinueDrag -= h);
        }



        public static IObservable<EventPattern<GiveFeedbackEventArgs>> PreviewGiveFeedbackObservable(this ContentElement This){
            return Observable.FromEventPattern<GiveFeedbackEventHandler, GiveFeedbackEventArgs>(h => This.PreviewGiveFeedback += h, h => This.PreviewGiveFeedback -= h);
        }



        public static IObservable<EventPattern<GiveFeedbackEventArgs>> GiveFeedbackObservable(this ContentElement This){
            return Observable.FromEventPattern<GiveFeedbackEventHandler, GiveFeedbackEventArgs>(h => This.GiveFeedback += h, h => This.GiveFeedback -= h);
        }



        public static IObservable<EventPattern<DragEventArgs>> PreviewDragEnterObservable(this ContentElement This){
            return Observable.FromEventPattern<DragEventHandler, DragEventArgs>(h => This.PreviewDragEnter += h, h => This.PreviewDragEnter -= h);
        }



        public static IObservable<EventPattern<DragEventArgs>> DragEnterObservable(this ContentElement This){
            return Observable.FromEventPattern<DragEventHandler, DragEventArgs>(h => This.DragEnter += h, h => This.DragEnter -= h);
        }



        public static IObservable<EventPattern<DragEventArgs>> PreviewDragOverObservable(this ContentElement This){
            return Observable.FromEventPattern<DragEventHandler, DragEventArgs>(h => This.PreviewDragOver += h, h => This.PreviewDragOver -= h);
        }



        public static IObservable<EventPattern<DragEventArgs>> DragOverObservable(this ContentElement This){
            return Observable.FromEventPattern<DragEventHandler, DragEventArgs>(h => This.DragOver += h, h => This.DragOver -= h);
        }



        public static IObservable<EventPattern<DragEventArgs>> PreviewDragLeaveObservable(this ContentElement This){
            return Observable.FromEventPattern<DragEventHandler, DragEventArgs>(h => This.PreviewDragLeave += h, h => This.PreviewDragLeave -= h);
        }



        public static IObservable<EventPattern<DragEventArgs>> DragLeaveObservable(this ContentElement This){
            return Observable.FromEventPattern<DragEventHandler, DragEventArgs>(h => This.DragLeave += h, h => This.DragLeave -= h);
        }



        public static IObservable<EventPattern<DragEventArgs>> PreviewDropObservable(this ContentElement This){
            return Observable.FromEventPattern<DragEventHandler, DragEventArgs>(h => This.PreviewDrop += h, h => This.PreviewDrop -= h);
        }



        public static IObservable<EventPattern<DragEventArgs>> DropObservable(this ContentElement This){
            return Observable.FromEventPattern<DragEventHandler, DragEventArgs>(h => This.Drop += h, h => This.Drop -= h);
        }













        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsMouseDirectlyOverChangedObservable(this ContentElement This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsMouseDirectlyOverChanged += h, h => This.IsMouseDirectlyOverChanged -= h);
        }



        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsKeyboardFocusWithinChangedObservable(this ContentElement This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsKeyboardFocusWithinChanged += h, h => This.IsKeyboardFocusWithinChanged -= h);
        }



        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsMouseCapturedChangedObservable(this ContentElement This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsMouseCapturedChanged += h, h => This.IsMouseCapturedChanged -= h);
        }



        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsMouseCaptureWithinChangedObservable(this ContentElement This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsMouseCaptureWithinChanged += h, h => This.IsMouseCaptureWithinChanged -= h);
        }



        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsStylusDirectlyOverChangedObservable(this ContentElement This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsStylusDirectlyOverChanged += h, h => This.IsStylusDirectlyOverChanged -= h);
        }



        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsStylusCapturedChangedObservable(this ContentElement This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsStylusCapturedChanged += h, h => This.IsStylusCapturedChanged -= h);
        }



        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsStylusCaptureWithinChangedObservable(this ContentElement This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsStylusCaptureWithinChanged += h, h => This.IsStylusCaptureWithinChanged -= h);
        }



        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsKeyboardFocusedChangedObservable(this ContentElement This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsKeyboardFocusedChanged += h, h => This.IsKeyboardFocusedChanged -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   UIElement
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<MouseButtonEventArgs>> PreviewMouseDownObservable(this UIElement This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.PreviewMouseDown += h, h => This.PreviewMouseDown -= h);
        }



        public static IObservable<EventPattern<MouseButtonEventArgs>> MouseDownObservable(this UIElement This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.MouseDown += h, h => This.MouseDown -= h);
        }



        public static IObservable<EventPattern<MouseButtonEventArgs>> PreviewMouseUpObservable(this UIElement This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.PreviewMouseUp += h, h => This.PreviewMouseUp -= h);
        }



        public static IObservable<EventPattern<MouseButtonEventArgs>> MouseUpObservable(this UIElement This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.MouseUp += h, h => This.MouseUp -= h);
        }



        public static IObservable<EventPattern<QueryCursorEventArgs>> QueryCursorObservable(this UIElement This){
            return Observable.FromEventPattern<QueryCursorEventHandler, QueryCursorEventArgs>(h => This.QueryCursor += h, h => This.QueryCursor -= h);
        }



        public static IObservable<EventPattern<QueryContinueDragEventArgs>> PreviewQueryContinueDragObservable(this UIElement This){
            return Observable.FromEventPattern<QueryContinueDragEventHandler, QueryContinueDragEventArgs>(h => This.PreviewQueryContinueDrag += h, h => This.PreviewQueryContinueDrag -= h);
        }



        public static IObservable<EventPattern<QueryContinueDragEventArgs>> QueryContinueDragObservable(this UIElement This){
            return Observable.FromEventPattern<QueryContinueDragEventHandler, QueryContinueDragEventArgs>(h => This.QueryContinueDrag += h, h => This.QueryContinueDrag -= h);
        }



        public static IObservable<EventPattern<GiveFeedbackEventArgs>> PreviewGiveFeedbackObservable(this UIElement This){
            return Observable.FromEventPattern<GiveFeedbackEventHandler, GiveFeedbackEventArgs>(h => This.PreviewGiveFeedback += h, h => This.PreviewGiveFeedback -= h);
        }



        public static IObservable<EventPattern<GiveFeedbackEventArgs>> GiveFeedbackObservable(this UIElement This){
            return Observable.FromEventPattern<GiveFeedbackEventHandler, GiveFeedbackEventArgs>(h => This.GiveFeedback += h, h => This.GiveFeedback -= h);
        }



        public static IObservable<EventPattern<DragEventArgs>> PreviewDragEnterObservable(this UIElement This){
            return Observable.FromEventPattern<DragEventHandler, DragEventArgs>(h => This.PreviewDragEnter += h, h => This.PreviewDragEnter -= h);
        }



        public static IObservable<EventPattern<DragEventArgs>> DragEnterObservable(this UIElement This){
            return Observable.FromEventPattern<DragEventHandler, DragEventArgs>(h => This.DragEnter += h, h => This.DragEnter -= h);
        }



        public static IObservable<EventPattern<DragEventArgs>> PreviewDragOverObservable(this UIElement This){
            return Observable.FromEventPattern<DragEventHandler, DragEventArgs>(h => This.PreviewDragOver += h, h => This.PreviewDragOver -= h);
        }



        public static IObservable<EventPattern<DragEventArgs>> DragOverObservable(this UIElement This){
            return Observable.FromEventPattern<DragEventHandler, DragEventArgs>(h => This.DragOver += h, h => This.DragOver -= h);
        }



        public static IObservable<EventPattern<DragEventArgs>> PreviewDragLeaveObservable(this UIElement This){
            return Observable.FromEventPattern<DragEventHandler, DragEventArgs>(h => This.PreviewDragLeave += h, h => This.PreviewDragLeave -= h);
        }



        public static IObservable<EventPattern<DragEventArgs>> DragLeaveObservable(this UIElement This){
            return Observable.FromEventPattern<DragEventHandler, DragEventArgs>(h => This.DragLeave += h, h => This.DragLeave -= h);
        }



        public static IObservable<EventPattern<DragEventArgs>> PreviewDropObservable(this UIElement This){
            return Observable.FromEventPattern<DragEventHandler, DragEventArgs>(h => This.PreviewDrop += h, h => This.PreviewDrop -= h);
        }



        public static IObservable<EventPattern<DragEventArgs>> DropObservable(this UIElement This){
            return Observable.FromEventPattern<DragEventHandler, DragEventArgs>(h => This.Drop += h, h => This.Drop -= h);
        }













        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsMouseDirectlyOverChangedObservable(this UIElement This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsMouseDirectlyOverChanged += h, h => This.IsMouseDirectlyOverChanged -= h);
        }



        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsKeyboardFocusWithinChangedObservable(this UIElement This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsKeyboardFocusWithinChanged += h, h => This.IsKeyboardFocusWithinChanged -= h);
        }



        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsMouseCapturedChangedObservable(this UIElement This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsMouseCapturedChanged += h, h => This.IsMouseCapturedChanged -= h);
        }



        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsMouseCaptureWithinChangedObservable(this UIElement This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsMouseCaptureWithinChanged += h, h => This.IsMouseCaptureWithinChanged -= h);
        }



        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsStylusDirectlyOverChangedObservable(this UIElement This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsStylusDirectlyOverChanged += h, h => This.IsStylusDirectlyOverChanged -= h);
        }



        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsStylusCapturedChangedObservable(this UIElement This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsStylusCapturedChanged += h, h => This.IsStylusCapturedChanged -= h);
        }



        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsStylusCaptureWithinChangedObservable(this UIElement This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsStylusCaptureWithinChanged += h, h => This.IsStylusCaptureWithinChanged -= h);
        }



        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsKeyboardFocusedChangedObservable(this UIElement This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsKeyboardFocusedChanged += h, h => This.IsKeyboardFocusedChanged -= h);
        }



        public static IObservable<EventPattern<EventArgs>> LayoutUpdatedObservable(this UIElement This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.LayoutUpdated += h, h => This.LayoutUpdated -= h);
        }



        public static IObservable<EventPattern<RoutedEventArgs>> GotFocusObservable(this UIElement This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.GotFocus += h, h => This.GotFocus -= h);
        }



        public static IObservable<EventPattern<RoutedEventArgs>> LostFocusObservable(this UIElement This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.LostFocus += h, h => This.LostFocus -= h);
        }



        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsEnabledChangedObservable(this UIElement This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsEnabledChanged += h, h => This.IsEnabledChanged -= h);
        }



        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsHitTestVisibleChangedObservable(this UIElement This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsHitTestVisibleChanged += h, h => This.IsHitTestVisibleChanged -= h);
        }



        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsVisibleChangedObservable(this UIElement This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsVisibleChanged += h, h => This.IsVisibleChanged -= h);
        }



        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> FocusableChangedObservable(this UIElement This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.FocusableChanged += h, h => This.FocusableChanged -= h);
        }







 
////////////////////////////////////////////
////////////////////////////////////////////
////   UIElement3D
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<MouseButtonEventArgs>> PreviewMouseDownObservable(this UIElement3D This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.PreviewMouseDown += h, h => This.PreviewMouseDown -= h);
        }



        public static IObservable<EventPattern<MouseButtonEventArgs>> MouseDownObservable(this UIElement3D This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.MouseDown += h, h => This.MouseDown -= h);
        }



        public static IObservable<EventPattern<MouseButtonEventArgs>> PreviewMouseUpObservable(this UIElement3D This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.PreviewMouseUp += h, h => This.PreviewMouseUp -= h);
        }



        public static IObservable<EventPattern<MouseButtonEventArgs>> MouseUpObservable(this UIElement3D This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.MouseUp += h, h => This.MouseUp -= h);
        }



        public static IObservable<EventPattern<QueryCursorEventArgs>> QueryCursorObservable(this UIElement3D This){
            return Observable.FromEventPattern<QueryCursorEventHandler, QueryCursorEventArgs>(h => This.QueryCursor += h, h => This.QueryCursor -= h);
        }



        public static IObservable<EventPattern<QueryContinueDragEventArgs>> PreviewQueryContinueDragObservable(this UIElement3D This){
            return Observable.FromEventPattern<QueryContinueDragEventHandler, QueryContinueDragEventArgs>(h => This.PreviewQueryContinueDrag += h, h => This.PreviewQueryContinueDrag -= h);
        }



        public static IObservable<EventPattern<QueryContinueDragEventArgs>> QueryContinueDragObservable(this UIElement3D This){
            return Observable.FromEventPattern<QueryContinueDragEventHandler, QueryContinueDragEventArgs>(h => This.QueryContinueDrag += h, h => This.QueryContinueDrag -= h);
        }



        public static IObservable<EventPattern<GiveFeedbackEventArgs>> PreviewGiveFeedbackObservable(this UIElement3D This){
            return Observable.FromEventPattern<GiveFeedbackEventHandler, GiveFeedbackEventArgs>(h => This.PreviewGiveFeedback += h, h => This.PreviewGiveFeedback -= h);
        }



        public static IObservable<EventPattern<GiveFeedbackEventArgs>> GiveFeedbackObservable(this UIElement3D This){
            return Observable.FromEventPattern<GiveFeedbackEventHandler, GiveFeedbackEventArgs>(h => This.GiveFeedback += h, h => This.GiveFeedback -= h);
        }



        public static IObservable<EventPattern<DragEventArgs>> PreviewDragEnterObservable(this UIElement3D This){
            return Observable.FromEventPattern<DragEventHandler, DragEventArgs>(h => This.PreviewDragEnter += h, h => This.PreviewDragEnter -= h);
        }



        public static IObservable<EventPattern<DragEventArgs>> DragEnterObservable(this UIElement3D This){
            return Observable.FromEventPattern<DragEventHandler, DragEventArgs>(h => This.DragEnter += h, h => This.DragEnter -= h);
        }



        public static IObservable<EventPattern<DragEventArgs>> PreviewDragOverObservable(this UIElement3D This){
            return Observable.FromEventPattern<DragEventHandler, DragEventArgs>(h => This.PreviewDragOver += h, h => This.PreviewDragOver -= h);
        }



        public static IObservable<EventPattern<DragEventArgs>> DragOverObservable(this UIElement3D This){
            return Observable.FromEventPattern<DragEventHandler, DragEventArgs>(h => This.DragOver += h, h => This.DragOver -= h);
        }



        public static IObservable<EventPattern<DragEventArgs>> PreviewDragLeaveObservable(this UIElement3D This){
            return Observable.FromEventPattern<DragEventHandler, DragEventArgs>(h => This.PreviewDragLeave += h, h => This.PreviewDragLeave -= h);
        }



        public static IObservable<EventPattern<DragEventArgs>> DragLeaveObservable(this UIElement3D This){
            return Observable.FromEventPattern<DragEventHandler, DragEventArgs>(h => This.DragLeave += h, h => This.DragLeave -= h);
        }



        public static IObservable<EventPattern<DragEventArgs>> PreviewDropObservable(this UIElement3D This){
            return Observable.FromEventPattern<DragEventHandler, DragEventArgs>(h => This.PreviewDrop += h, h => This.PreviewDrop -= h);
        }



        public static IObservable<EventPattern<DragEventArgs>> DropObservable(this UIElement3D This){
            return Observable.FromEventPattern<DragEventHandler, DragEventArgs>(h => This.Drop += h, h => This.Drop -= h);
        }













        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsMouseDirectlyOverChangedObservable(this UIElement3D This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsMouseDirectlyOverChanged += h, h => This.IsMouseDirectlyOverChanged -= h);
        }



        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsKeyboardFocusWithinChangedObservable(this UIElement3D This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsKeyboardFocusWithinChanged += h, h => This.IsKeyboardFocusWithinChanged -= h);
        }



        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsMouseCapturedChangedObservable(this UIElement3D This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsMouseCapturedChanged += h, h => This.IsMouseCapturedChanged -= h);
        }



        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsMouseCaptureWithinChangedObservable(this UIElement3D This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsMouseCaptureWithinChanged += h, h => This.IsMouseCaptureWithinChanged -= h);
        }



        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsStylusDirectlyOverChangedObservable(this UIElement3D This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsStylusDirectlyOverChanged += h, h => This.IsStylusDirectlyOverChanged -= h);
        }



        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsStylusCapturedChangedObservable(this UIElement3D This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsStylusCapturedChanged += h, h => This.IsStylusCapturedChanged -= h);
        }



        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsStylusCaptureWithinChangedObservable(this UIElement3D This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsStylusCaptureWithinChanged += h, h => This.IsStylusCaptureWithinChanged -= h);
        }



        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsKeyboardFocusedChangedObservable(this UIElement3D This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsKeyboardFocusedChanged += h, h => This.IsKeyboardFocusedChanged -= h);
        }



        public static IObservable<EventPattern<RoutedEventArgs>> GotFocusObservable(this UIElement3D This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.GotFocus += h, h => This.GotFocus -= h);
        }



        public static IObservable<EventPattern<RoutedEventArgs>> LostFocusObservable(this UIElement3D This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.LostFocus += h, h => This.LostFocus -= h);
        }



        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsEnabledChangedObservable(this UIElement3D This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsEnabledChanged += h, h => This.IsEnabledChanged -= h);
        }



        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsHitTestVisibleChangedObservable(this UIElement3D This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsHitTestVisibleChanged += h, h => This.IsHitTestVisibleChanged -= h);
        }



        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsVisibleChangedObservable(this UIElement3D This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsVisibleChanged += h, h => This.IsVisibleChanged -= h);
        }



        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> FocusableChangedObservable(this UIElement3D This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.FocusableChanged += h, h => This.FocusableChanged -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   PresentationSource
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<EventArgs>> ContentRenderedObservable(this PresentationSource This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.ContentRendered += h, h => This.ContentRendered -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   DocumentPage
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<EventArgs>> PageDestroyedObservable(this DocumentPage This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.PageDestroyed += h, h => This.PageDestroyed -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   DocumentPaginator
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<GetPageCompletedEventArgs>> GetPageCompletedObservable(this DocumentPaginator This){
            return Observable.FromEventPattern<GetPageCompletedEventHandler, GetPageCompletedEventArgs>(h => This.GetPageCompleted += h, h => This.GetPageCompleted -= h);
        }



        public static IObservable<EventPattern<AsyncCompletedEventArgs>> ComputePageCountCompletedObservable(this DocumentPaginator This){
            return Observable.FromEventPattern<AsyncCompletedEventHandler, AsyncCompletedEventArgs>(h => This.ComputePageCountCompleted += h, h => This.ComputePageCountCompleted -= h);
        }



        public static IObservable<EventPattern<PagesChangedEventArgs>> PagesChangedObservable(this DocumentPaginator This){
            return Observable.FromEventPattern<PagesChangedEventHandler, PagesChangedEventArgs>(h => This.PagesChanged += h, h => This.PagesChanged -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   DynamicDocumentPaginator
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<GetPageNumberCompletedEventArgs>> GetPageNumberCompletedObservable(this DynamicDocumentPaginator This){
            return Observable.FromEventPattern<GetPageNumberCompletedEventHandler, GetPageNumberCompletedEventArgs>(h => This.GetPageNumberCompleted += h, h => This.GetPageNumberCompleted -= h);
        }



        public static IObservable<EventPattern<EventArgs>> PaginationCompletedObservable(this DynamicDocumentPaginator This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.PaginationCompleted += h, h => This.PaginationCompleted -= h);
        }



        public static IObservable<EventPattern<PaginationProgressEventArgs>> PaginationProgressObservable(this DynamicDocumentPaginator This){
            return Observable.FromEventPattern<PaginationProgressEventHandler, PaginationProgressEventArgs>(h => This.PaginationProgress += h, h => This.PaginationProgress -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   DrawingAttributes
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<PropertyDataChangedEventArgs>> AttributeChangedObservable(this DrawingAttributes This){
            return Observable.FromEventPattern<PropertyDataChangedEventHandler, PropertyDataChangedEventArgs>(h => This.AttributeChanged += h, h => This.AttributeChanged -= h);
        }



        public static IObservable<EventPattern<PropertyDataChangedEventArgs>> PropertyDataChangedObservable(this DrawingAttributes This){
            return Observable.FromEventPattern<PropertyDataChangedEventHandler, PropertyDataChangedEventArgs>(h => This.PropertyDataChanged += h, h => This.PropertyDataChanged -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   IncrementalLassoHitTester
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<LassoSelectionChangedEventArgs>> SelectionChangedObservable(this IncrementalLassoHitTester This){
            return Observable.FromEventPattern<LassoSelectionChangedEventHandler, LassoSelectionChangedEventArgs>(h => This.SelectionChanged += h, h => This.SelectionChanged -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   IncrementalStrokeHitTester
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<StrokeHitEventArgs>> StrokeHitObservable(this IncrementalStrokeHitTester This){
            return Observable.FromEventPattern<StrokeHitEventHandler, StrokeHitEventArgs>(h => This.StrokeHit += h, h => This.StrokeHit -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   Stroke
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<PropertyDataChangedEventArgs>> DrawingAttributesChangedObservable(this Stroke This){
            return Observable.FromEventPattern<PropertyDataChangedEventHandler, PropertyDataChangedEventArgs>(h => This.DrawingAttributesChanged += h, h => This.DrawingAttributesChanged -= h);
        }



        public static IObservable<EventPattern<DrawingAttributesReplacedEventArgs>> DrawingAttributesReplacedObservable(this Stroke This){
            return Observable.FromEventPattern<DrawingAttributesReplacedEventHandler, DrawingAttributesReplacedEventArgs>(h => This.DrawingAttributesReplaced += h, h => This.DrawingAttributesReplaced -= h);
        }



        public static IObservable<EventPattern<StylusPointsReplacedEventArgs>> StylusPointsReplacedObservable(this Stroke This){
            return Observable.FromEventPattern<StylusPointsReplacedEventHandler, StylusPointsReplacedEventArgs>(h => This.StylusPointsReplaced += h, h => This.StylusPointsReplaced -= h);
        }



        public static IObservable<EventPattern<EventArgs>> StylusPointsChangedObservable(this Stroke This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.StylusPointsChanged += h, h => This.StylusPointsChanged -= h);
        }



        public static IObservable<EventPattern<PropertyDataChangedEventArgs>> PropertyDataChangedObservable(this Stroke This){
            return Observable.FromEventPattern<PropertyDataChangedEventHandler, PropertyDataChangedEventArgs>(h => This.PropertyDataChanged += h, h => This.PropertyDataChanged -= h);
        }



        public static IObservable<EventPattern<EventArgs>> InvalidatedObservable(this Stroke This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.Invalidated += h, h => This.Invalidated -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   StrokeCollection
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<StrokeCollectionChangedEventArgs>> StrokesChangedObservable(this StrokeCollection This){
            return Observable.FromEventPattern<StrokeCollectionChangedEventHandler, StrokeCollectionChangedEventArgs>(h => This.StrokesChanged += h, h => This.StrokesChanged -= h);
        }



        public static IObservable<EventPattern<PropertyDataChangedEventArgs>> PropertyDataChangedObservable(this StrokeCollection This){
            return Observable.FromEventPattern<PropertyDataChangedEventHandler, PropertyDataChangedEventArgs>(h => This.PropertyDataChanged += h, h => This.PropertyDataChanged -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   CommandBinding
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<ExecutedRoutedEventArgs>> PreviewExecutedObservable(this CommandBinding This){
            return Observable.FromEventPattern<ExecutedRoutedEventHandler, ExecutedRoutedEventArgs>(h => This.PreviewExecuted += h, h => This.PreviewExecuted -= h);
        }



        public static IObservable<EventPattern<ExecutedRoutedEventArgs>> ExecutedObservable(this CommandBinding This){
            return Observable.FromEventPattern<ExecutedRoutedEventHandler, ExecutedRoutedEventArgs>(h => This.Executed += h, h => This.Executed -= h);
        }



        public static IObservable<EventPattern<CanExecuteRoutedEventArgs>> PreviewCanExecuteObservable(this CommandBinding This){
            return Observable.FromEventPattern<CanExecuteRoutedEventHandler, CanExecuteRoutedEventArgs>(h => This.PreviewCanExecute += h, h => This.PreviewCanExecute -= h);
        }



        public static IObservable<EventPattern<CanExecuteRoutedEventArgs>> CanExecuteObservable(this CommandBinding This){
            return Observable.FromEventPattern<CanExecuteRoutedEventHandler, CanExecuteRoutedEventArgs>(h => This.CanExecute += h, h => This.CanExecute -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   IManipulator
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<EventArgs>> UpdatedObservable(this IManipulator This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.Updated += h, h => This.Updated -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   InputLanguageManager
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<InputLanguageEventArgs>> InputLanguageChangedObservable(this InputLanguageManager This){
            return Observable.FromEventPattern<InputLanguageEventHandler, InputLanguageEventArgs>(h => This.InputLanguageChanged += h, h => This.InputLanguageChanged -= h);
        }



        public static IObservable<EventPattern<InputLanguageEventArgs>> InputLanguageChangingObservable(this InputLanguageManager This){
            return Observable.FromEventPattern<InputLanguageEventHandler, InputLanguageEventArgs>(h => This.InputLanguageChanging += h, h => This.InputLanguageChanging -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   InputManager
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<PreProcessInputEventArgs>> PreProcessInputObservable(this InputManager This){
            return Observable.FromEventPattern<PreProcessInputEventHandler, PreProcessInputEventArgs>(h => This.PreProcessInput += h, h => This.PreProcessInput -= h);
        }



        public static IObservable<EventPattern<NotifyInputEventArgs>> PreNotifyInputObservable(this InputManager This){
            return Observable.FromEventPattern<NotifyInputEventHandler, NotifyInputEventArgs>(h => This.PreNotifyInput += h, h => This.PreNotifyInput -= h);
        }



        public static IObservable<EventPattern<NotifyInputEventArgs>> PostNotifyInputObservable(this InputManager This){
            return Observable.FromEventPattern<NotifyInputEventHandler, NotifyInputEventArgs>(h => This.PostNotifyInput += h, h => This.PostNotifyInput -= h);
        }



        public static IObservable<EventPattern<ProcessInputEventArgs>> PostProcessInputObservable(this InputManager This){
            return Observable.FromEventPattern<ProcessInputEventHandler, ProcessInputEventArgs>(h => This.PostProcessInput += h, h => This.PostProcessInput -= h);
        }



        public static IObservable<EventPattern<EventArgs>> EnterMenuModeObservable(this InputManager This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.EnterMenuMode += h, h => This.EnterMenuMode -= h);
        }



        public static IObservable<EventPattern<EventArgs>> LeaveMenuModeObservable(this InputManager This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.LeaveMenuMode += h, h => This.LeaveMenuMode -= h);
        }



        public static IObservable<EventPattern<EventArgs>> HitTestInvalidatedAsyncObservable(this InputManager This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.HitTestInvalidatedAsync += h, h => This.HitTestInvalidatedAsync -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   InputMethod
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<InputMethodStateChangedEventArgs>> StateChangedObservable(this InputMethod This){
            return Observable.FromEventPattern<InputMethodStateChangedEventHandler, InputMethodStateChangedEventArgs>(h => This.StateChanged += h, h => This.StateChanged -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   TouchDevice
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<EventArgs>> ActivatedObservable(this TouchDevice This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.Activated += h, h => This.Activated -= h);
        }



        public static IObservable<EventPattern<EventArgs>> DeactivatedObservable(this TouchDevice This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.Deactivated += h, h => This.Deactivated -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   StylusPointCollection
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<EventArgs>> ChangedObservable(this StylusPointCollection This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.Changed += h, h => This.Changed -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   D3DImage
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> IsFrontBufferAvailableChangedObservable(this D3DImage This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.IsFrontBufferAvailableChanged += h, h => This.IsFrontBufferAvailableChanged -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   HwndSource
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<EventArgs>> DisposedObservable(this HwndSource This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.Disposed += h, h => This.Disposed -= h);
        }



        public static IObservable<EventPattern<EventArgs>> SizeToContentChangedObservable(this HwndSource This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.SizeToContentChanged += h, h => This.SizeToContentChanged -= h);
        }



        public static IObservable<EventPattern<AutoResizedEventArgs>> AutoResizedObservable(this HwndSource This){
            return Observable.FromEventPattern<AutoResizedEventHandler, AutoResizedEventArgs>(h => This.AutoResized += h, h => This.AutoResized -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   Clock
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<EventArgs>> CompletedObservable(this Clock This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.Completed += h, h => This.Completed -= h);
        }



        public static IObservable<EventPattern<EventArgs>> CurrentGlobalSpeedInvalidatedObservable(this Clock This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.CurrentGlobalSpeedInvalidated += h, h => This.CurrentGlobalSpeedInvalidated -= h);
        }



        public static IObservable<EventPattern<EventArgs>> CurrentStateInvalidatedObservable(this Clock This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.CurrentStateInvalidated += h, h => This.CurrentStateInvalidated -= h);
        }



        public static IObservable<EventPattern<EventArgs>> CurrentTimeInvalidatedObservable(this Clock This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.CurrentTimeInvalidated += h, h => This.CurrentTimeInvalidated -= h);
        }



        public static IObservable<EventPattern<EventArgs>> RemoveRequestedObservable(this Clock This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.RemoveRequested += h, h => This.RemoveRequested -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   Timeline
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<EventArgs>> CurrentStateInvalidatedObservable(this Timeline This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.CurrentStateInvalidated += h, h => This.CurrentStateInvalidated -= h);
        }



        public static IObservable<EventPattern<EventArgs>> CurrentTimeInvalidatedObservable(this Timeline This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.CurrentTimeInvalidated += h, h => This.CurrentTimeInvalidated -= h);
        }



        public static IObservable<EventPattern<EventArgs>> CurrentGlobalSpeedInvalidatedObservable(this Timeline This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.CurrentGlobalSpeedInvalidated += h, h => This.CurrentGlobalSpeedInvalidated -= h);
        }



        public static IObservable<EventPattern<EventArgs>> CompletedObservable(this Timeline This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.Completed += h, h => This.Completed -= h);
        }



        public static IObservable<EventPattern<EventArgs>> RemoveRequestedObservable(this Timeline This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.RemoveRequested += h, h => This.RemoveRequested -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   BitmapDecoder
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<EventArgs>> DownloadCompletedObservable(this BitmapDecoder This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.DownloadCompleted += h, h => This.DownloadCompleted -= h);
        }



 
////////////////////////////////////////////
////////////////////////////////////////////
////   BitmapSource
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<EventArgs>> DownloadCompletedObservable(this BitmapSource This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.DownloadCompleted += h, h => This.DownloadCompleted -= h);
        }




 
////////////////////////////////////////////
////////////////////////////////////////////
////   MediaPlayer
////////////////////////////////////////////
////////////////////////////////////////////




        public static IObservable<EventPattern<EventArgs>> MediaOpenedObservable(this MediaPlayer This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.MediaOpened += h, h => This.MediaOpened -= h);
        }



        public static IObservable<EventPattern<EventArgs>> MediaEndedObservable(this MediaPlayer This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.MediaEnded += h, h => This.MediaEnded -= h);
        }



        public static IObservable<EventPattern<EventArgs>> BufferingStartedObservable(this MediaPlayer This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.BufferingStarted += h, h => This.BufferingStarted -= h);
        }



        public static IObservable<EventPattern<EventArgs>> BufferingEndedObservable(this MediaPlayer This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.BufferingEnded += h, h => This.BufferingEnded -= h);
        }


 
////////////////////////////////////////////
////////////////////////////////////////////
////   FileDialog
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<CancelEventArgs>> FileOkObservable(this FileDialog This){
            return Observable.FromEventPattern<CancelEventHandler, CancelEventArgs>(h => This.FileOk += h, h => This.FileOk -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   FrameworkElement
////////////////////////////////////////////
////////////////////////////////////////////





        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> DataContextChangedObservable(this FrameworkElement This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.DataContextChanged += h, h => This.DataContextChanged -= h);
        }



        public static IObservable<EventPattern<RequestBringIntoViewEventArgs>> RequestBringIntoViewObservable(this FrameworkElement This){
            return Observable.FromEventPattern<RequestBringIntoViewEventHandler, RequestBringIntoViewEventArgs>(h => This.RequestBringIntoView += h, h => This.RequestBringIntoView -= h);
        }



        public static IObservable<EventPattern<SizeChangedEventArgs>> SizeChangedObservable(this FrameworkElement This){
            return Observable.FromEventPattern<SizeChangedEventHandler, SizeChangedEventArgs>(h => This.SizeChanged += h, h => This.SizeChanged -= h);
        }



        public static IObservable<EventPattern<EventArgs>> InitializedObservable(this FrameworkElement This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.Initialized += h, h => This.Initialized -= h);
        }



        public static IObservable<EventPattern<RoutedEventArgs>> LoadedObservable(this FrameworkElement This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Loaded += h, h => This.Loaded -= h);
        }



        public static IObservable<EventPattern<RoutedEventArgs>> UnloadedObservable(this FrameworkElement This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Unloaded += h, h => This.Unloaded -= h);
        }



        public static IObservable<EventPattern<ToolTipEventArgs>> ToolTipOpeningObservable(this FrameworkElement This){
            return Observable.FromEventPattern<ToolTipEventHandler, ToolTipEventArgs>(h => This.ToolTipOpening += h, h => This.ToolTipOpening -= h);
        }



        public static IObservable<EventPattern<ToolTipEventArgs>> ToolTipClosingObservable(this FrameworkElement This){
            return Observable.FromEventPattern<ToolTipEventHandler, ToolTipEventArgs>(h => This.ToolTipClosing += h, h => This.ToolTipClosing -= h);
        }



        public static IObservable<EventPattern<ContextMenuEventArgs>> ContextMenuOpeningObservable(this FrameworkElement This){
            return Observable.FromEventPattern<ContextMenuEventHandler, ContextMenuEventArgs>(h => This.ContextMenuOpening += h, h => This.ContextMenuOpening -= h);
        }



        public static IObservable<EventPattern<ContextMenuEventArgs>> ContextMenuClosingObservable(this FrameworkElement This){
            return Observable.FromEventPattern<ContextMenuEventHandler, ContextMenuEventArgs>(h => This.ContextMenuClosing += h, h => This.ContextMenuClosing -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   Control
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<MouseButtonEventArgs>> PreviewMouseDoubleClickObservable(this Control This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.PreviewMouseDoubleClick += h, h => This.PreviewMouseDoubleClick -= h);
        }



        public static IObservable<EventPattern<MouseButtonEventArgs>> MouseDoubleClickObservable(this Control This){
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => This.MouseDoubleClick += h, h => This.MouseDoubleClick -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   Window
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<EventArgs>> SourceInitializedObservable(this Window This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.SourceInitialized += h, h => This.SourceInitialized -= h);
        }



        public static IObservable<EventPattern<EventArgs>> ActivatedObservable(this Window This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.Activated += h, h => This.Activated -= h);
        }



        public static IObservable<EventPattern<EventArgs>> DeactivatedObservable(this Window This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.Deactivated += h, h => This.Deactivated -= h);
        }



        public static IObservable<EventPattern<EventArgs>> StateChangedObservable(this Window This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.StateChanged += h, h => This.StateChanged -= h);
        }



        public static IObservable<EventPattern<EventArgs>> LocationChangedObservable(this Window This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.LocationChanged += h, h => This.LocationChanged -= h);
        }



        public static IObservable<EventPattern<CancelEventArgs>> ClosingObservable(this Window This){
            return Observable.FromEventPattern<CancelEventHandler, CancelEventArgs>(h => This.Closing += h, h => This.Closing -= h);
        }



        public static IObservable<EventPattern<EventArgs>> ClosedObservable(this Window This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.Closed += h, h => This.Closed -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   Application
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<StartupEventArgs>> StartupObservable(this Application This){
            return Observable.FromEventPattern<StartupEventHandler, StartupEventArgs>(h => This.Startup += h, h => This.Startup -= h);
        }



        public static IObservable<EventPattern<ExitEventArgs>> ExitObservable(this Application This){
            return Observable.FromEventPattern<ExitEventHandler, ExitEventArgs>(h => This.Exit += h, h => This.Exit -= h);
        }



        public static IObservable<EventPattern<EventArgs>> ActivatedObservable(this Application This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.Activated += h, h => This.Activated -= h);
        }



        public static IObservable<EventPattern<EventArgs>> DeactivatedObservable(this Application This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.Deactivated += h, h => This.Deactivated -= h);
        }



        public static IObservable<EventPattern<SessionEndingCancelEventArgs>> SessionEndingObservable(this Application This){
            return Observable.FromEventPattern<SessionEndingCancelEventHandler, SessionEndingCancelEventArgs>(h => This.SessionEnding += h, h => This.SessionEnding -= h);
        }



        public static IObservable<EventPattern<DispatcherUnhandledExceptionEventArgs>> DispatcherUnhandledExceptionObservable(this Application This){
            return Observable.FromEventPattern<DispatcherUnhandledExceptionEventHandler, DispatcherUnhandledExceptionEventArgs>(h => This.DispatcherUnhandledException += h, h => This.DispatcherUnhandledException -= h);
        }



        public static IObservable<EventPattern<NavigatingCancelEventArgs>> NavigatingObservable(this Application This){
            return Observable.FromEventPattern<NavigatingCancelEventHandler, NavigatingCancelEventArgs>(h => This.Navigating += h, h => This.Navigating -= h);
        }



        public static IObservable<EventPattern<NavigationEventArgs>> NavigatedObservable(this Application This){
            return Observable.FromEventPattern<NavigatedEventHandler, NavigationEventArgs>(h => This.Navigated += h, h => This.Navigated -= h);
        }



        public static IObservable<EventPattern<NavigationProgressEventArgs>> NavigationProgressObservable(this Application This){
            return Observable.FromEventPattern<NavigationProgressEventHandler, NavigationProgressEventArgs>(h => This.NavigationProgress += h, h => This.NavigationProgress -= h);
        }



        public static IObservable<EventPattern<NavigationFailedEventArgs>> NavigationFailedObservable(this Application This){
            return Observable.FromEventPattern<NavigationFailedEventHandler, NavigationFailedEventArgs>(h => This.NavigationFailed += h, h => This.NavigationFailed -= h);
        }



        public static IObservable<EventPattern<NavigationEventArgs>> LoadCompletedObservable(this Application This){
            return Observable.FromEventPattern<LoadCompletedEventHandler, NavigationEventArgs>(h => This.LoadCompleted += h, h => This.LoadCompleted -= h);
        }



        public static IObservable<EventPattern<NavigationEventArgs>> NavigationStoppedObservable(this Application This){
            return Observable.FromEventPattern<NavigationStoppedEventHandler, NavigationEventArgs>(h => This.NavigationStopped += h, h => This.NavigationStopped -= h);
        }



        public static IObservable<EventPattern<FragmentNavigationEventArgs>> FragmentNavigationObservable(this Application This){
            return Observable.FromEventPattern<FragmentNavigationEventHandler, FragmentNavigationEventArgs>(h => This.FragmentNavigation += h, h => This.FragmentNavigation -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   CollectionView
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<CurrentChangingEventArgs>> CurrentChangingObservable(this CollectionView This){
            return Observable.FromEventPattern<CurrentChangingEventHandler, CurrentChangingEventArgs>(h => This.CurrentChanging += h, h => This.CurrentChanging -= h);
        }



        public static IObservable<EventPattern<EventArgs>> CurrentChangedObservable(this CollectionView This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.CurrentChanged += h, h => This.CurrentChanged -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   ContextMenu
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<RoutedEventArgs>> OpenedObservable(this ContextMenu This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Opened += h, h => This.Opened -= h);
        }



        public static IObservable<EventPattern<RoutedEventArgs>> ClosedObservable(this ContextMenu This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Closed += h, h => This.Closed -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   MenuItem
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<RoutedEventArgs>> ClickObservable(this MenuItem This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Click += h, h => This.Click -= h);
        }



        public static IObservable<EventPattern<RoutedEventArgs>> CheckedObservable(this MenuItem This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Checked += h, h => This.Checked -= h);
        }



        public static IObservable<EventPattern<RoutedEventArgs>> UncheckedObservable(this MenuItem This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Unchecked += h, h => This.Unchecked -= h);
        }



        public static IObservable<EventPattern<RoutedEventArgs>> SubmenuOpenedObservable(this MenuItem This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.SubmenuOpened += h, h => This.SubmenuOpened -= h);
        }



        public static IObservable<EventPattern<RoutedEventArgs>> SubmenuClosedObservable(this MenuItem This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.SubmenuClosed += h, h => This.SubmenuClosed -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   DocumentViewerBase
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<EventArgs>> PageViewsChangedObservable(this DocumentViewerBase This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.PageViewsChanged += h, h => This.PageViewsChanged -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   Annotation
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<AnnotationAuthorChangedEventArgs>> AuthorChangedObservable(this Annotation This){
            return Observable.FromEventPattern<AnnotationAuthorChangedEventHandler, AnnotationAuthorChangedEventArgs>(h => This.AuthorChanged += h, h => This.AuthorChanged -= h);
        }



        public static IObservable<EventPattern<AnnotationResourceChangedEventArgs>> AnchorChangedObservable(this Annotation This){
            return Observable.FromEventPattern<AnnotationResourceChangedEventHandler, AnnotationResourceChangedEventArgs>(h => This.AnchorChanged += h, h => This.AnchorChanged -= h);
        }



        public static IObservable<EventPattern<AnnotationResourceChangedEventArgs>> CargoChangedObservable(this Annotation This){
            return Observable.FromEventPattern<AnnotationResourceChangedEventHandler, AnnotationResourceChangedEventArgs>(h => This.CargoChanged += h, h => This.CargoChanged -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   AnnotationStore
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<StoreContentChangedEventArgs>> StoreContentChangedObservable(this AnnotationStore This){
            return Observable.FromEventPattern<StoreContentChangedEventHandler, StoreContentChangedEventArgs>(h => This.StoreContentChanged += h, h => This.StoreContentChanged -= h);
        }



        public static IObservable<EventPattern<AnnotationAuthorChangedEventArgs>> AuthorChangedObservable(this AnnotationStore This){
            return Observable.FromEventPattern<AnnotationAuthorChangedEventHandler, AnnotationAuthorChangedEventArgs>(h => This.AuthorChanged += h, h => This.AuthorChanged -= h);
        }



        public static IObservable<EventPattern<AnnotationResourceChangedEventArgs>> AnchorChangedObservable(this AnnotationStore This){
            return Observable.FromEventPattern<AnnotationResourceChangedEventHandler, AnnotationResourceChangedEventArgs>(h => This.AnchorChanged += h, h => This.AnchorChanged -= h);
        }



        public static IObservable<EventPattern<AnnotationResourceChangedEventArgs>> CargoChangedObservable(this AnnotationStore This){
            return Observable.FromEventPattern<AnnotationResourceChangedEventHandler, AnnotationResourceChangedEventArgs>(h => This.CargoChanged += h, h => This.CargoChanged -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   ButtonBase
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<RoutedEventArgs>> ClickObservable(this ButtonBase This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Click += h, h => This.Click -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   Calendar
////////////////////////////////////////////
////////////////////////////////////////////





 
////////////////////////////////////////////
////////////////////////////////////////////
////   ToggleButton
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<RoutedEventArgs>> CheckedObservable(this ToggleButton This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Checked += h, h => This.Checked -= h);
        }



        public static IObservable<EventPattern<RoutedEventArgs>> UncheckedObservable(this ToggleButton This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Unchecked += h, h => This.Unchecked -= h);
        }



        public static IObservable<EventPattern<RoutedEventArgs>> IndeterminateObservable(this ToggleButton This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Indeterminate += h, h => This.Indeterminate -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   Selector
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<SelectionChangedEventArgs>> SelectionChangedObservable(this Selector This){
            return Observable.FromEventPattern<SelectionChangedEventHandler, SelectionChangedEventArgs>(h => This.SelectionChanged += h, h => This.SelectionChanged -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   ComboBox
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<EventArgs>> DropDownOpenedObservable(this ComboBox This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.DropDownOpened += h, h => This.DropDownOpened -= h);
        }



        public static IObservable<EventPattern<EventArgs>> DropDownClosedObservable(this ComboBox This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.DropDownClosed += h, h => This.DropDownClosed -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   ListBoxItem
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<RoutedEventArgs>> SelectedObservable(this ListBoxItem This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Selected += h, h => This.Selected -= h);
        }



        public static IObservable<EventPattern<RoutedEventArgs>> UnselectedObservable(this ListBoxItem This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Unselected += h, h => This.Unselected -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   DataGrid
////////////////////////////////////////////
////////////////////////////////////////////












        public static IObservable<EventPattern<InitializingNewItemEventArgs>> InitializingNewItemObservable(this DataGrid This){
            return Observable.FromEventPattern<InitializingNewItemEventHandler, InitializingNewItemEventArgs>(h => This.InitializingNewItem += h, h => This.InitializingNewItem -= h);
        }






        public static IObservable<EventPattern<SelectedCellsChangedEventArgs>> SelectedCellsChangedObservable(this DataGrid This){
            return Observable.FromEventPattern<SelectedCellsChangedEventHandler, SelectedCellsChangedEventArgs>(h => This.SelectedCellsChanged += h, h => This.SelectedCellsChanged -= h);
        }



        public static IObservable<EventPattern<DataGridSortingEventArgs>> SortingObservable(this DataGrid This){
            return Observable.FromEventPattern<DataGridSortingEventHandler, DataGridSortingEventArgs>(h => This.Sorting += h, h => This.Sorting -= h);
        }



        public static IObservable<EventPattern<EventArgs>> AutoGeneratedColumnsObservable(this DataGrid This){
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



        public static IObservable<EventPattern<RoutedEventArgs>> SelectedObservable(this DataGridCell This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Selected += h, h => This.Selected -= h);
        }



        public static IObservable<EventPattern<RoutedEventArgs>> UnselectedObservable(this DataGridCell This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Unselected += h, h => This.Unselected -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   DataGridRow
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<RoutedEventArgs>> SelectedObservable(this DataGridRow This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Selected += h, h => This.Selected -= h);
        }



        public static IObservable<EventPattern<RoutedEventArgs>> UnselectedObservable(this DataGridRow This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Unselected += h, h => This.Unselected -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   DatePicker
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<RoutedEventArgs>> CalendarClosedObservable(this DatePicker This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.CalendarClosed += h, h => This.CalendarClosed -= h);
        }



        public static IObservable<EventPattern<RoutedEventArgs>> CalendarOpenedObservable(this DatePicker This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.CalendarOpened += h, h => This.CalendarOpened -= h);
        }



 
////////////////////////////////////////////
////////////////////////////////////////////
////   FrameworkContentElement
////////////////////////////////////////////
////////////////////////////////////////////





        public static IObservable<EventPattern<DependencyPropertyChangedEventArgs>> DataContextChangedObservable(this FrameworkContentElement This){
            return Observable.FromEventPattern<DependencyPropertyChangedEventHandler, DependencyPropertyChangedEventArgs>(h => This.DataContextChanged += h, h => This.DataContextChanged -= h);
        }



        public static IObservable<EventPattern<EventArgs>> InitializedObservable(this FrameworkContentElement This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.Initialized += h, h => This.Initialized -= h);
        }



        public static IObservable<EventPattern<RoutedEventArgs>> LoadedObservable(this FrameworkContentElement This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Loaded += h, h => This.Loaded -= h);
        }



        public static IObservable<EventPattern<RoutedEventArgs>> UnloadedObservable(this FrameworkContentElement This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Unloaded += h, h => This.Unloaded -= h);
        }



        public static IObservable<EventPattern<ToolTipEventArgs>> ToolTipOpeningObservable(this FrameworkContentElement This){
            return Observable.FromEventPattern<ToolTipEventHandler, ToolTipEventArgs>(h => This.ToolTipOpening += h, h => This.ToolTipOpening -= h);
        }



        public static IObservable<EventPattern<ToolTipEventArgs>> ToolTipClosingObservable(this FrameworkContentElement This){
            return Observable.FromEventPattern<ToolTipEventHandler, ToolTipEventArgs>(h => This.ToolTipClosing += h, h => This.ToolTipClosing -= h);
        }



        public static IObservable<EventPattern<ContextMenuEventArgs>> ContextMenuOpeningObservable(this FrameworkContentElement This){
            return Observable.FromEventPattern<ContextMenuEventHandler, ContextMenuEventArgs>(h => This.ContextMenuOpening += h, h => This.ContextMenuOpening -= h);
        }



        public static IObservable<EventPattern<ContextMenuEventArgs>> ContextMenuClosingObservable(this FrameworkContentElement This){
            return Observable.FromEventPattern<ContextMenuEventHandler, ContextMenuEventArgs>(h => This.ContextMenuClosing += h, h => This.ContextMenuClosing -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   Expander
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<RoutedEventArgs>> ExpandedObservable(this Expander This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Expanded += h, h => This.Expanded -= h);
        }



        public static IObservable<EventPattern<RoutedEventArgs>> CollapsedObservable(this Expander This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Collapsed += h, h => This.Collapsed -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   Thumb
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<DragStartedEventArgs>> DragStartedObservable(this Thumb This){
            return Observable.FromEventPattern<DragStartedEventHandler, DragStartedEventArgs>(h => This.DragStarted += h, h => This.DragStarted -= h);
        }



        public static IObservable<EventPattern<DragDeltaEventArgs>> DragDeltaObservable(this Thumb This){
            return Observable.FromEventPattern<DragDeltaEventHandler, DragDeltaEventArgs>(h => This.DragDelta += h, h => This.DragDelta -= h);
        }



        public static IObservable<EventPattern<DragCompletedEventArgs>> DragCompletedObservable(this Thumb This){
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



        public static IObservable<EventPattern<InkCanvasStrokeCollectedEventArgs>> StrokeCollectedObservable(this InkCanvas This){
            return Observable.FromEventPattern<InkCanvasStrokeCollectedEventHandler, InkCanvasStrokeCollectedEventArgs>(h => This.StrokeCollected += h, h => This.StrokeCollected -= h);
        }



        public static IObservable<EventPattern<InkCanvasGestureEventArgs>> GestureObservable(this InkCanvas This){
            return Observable.FromEventPattern<InkCanvasGestureEventHandler, InkCanvasGestureEventArgs>(h => This.Gesture += h, h => This.Gesture -= h);
        }



        public static IObservable<EventPattern<InkCanvasStrokesReplacedEventArgs>> StrokesReplacedObservable(this InkCanvas This){
            return Observable.FromEventPattern<InkCanvasStrokesReplacedEventHandler, InkCanvasStrokesReplacedEventArgs>(h => This.StrokesReplaced += h, h => This.StrokesReplaced -= h);
        }



        public static IObservable<EventPattern<DrawingAttributesReplacedEventArgs>> DefaultDrawingAttributesReplacedObservable(this InkCanvas This){
            return Observable.FromEventPattern<DrawingAttributesReplacedEventHandler, DrawingAttributesReplacedEventArgs>(h => This.DefaultDrawingAttributesReplaced += h, h => This.DefaultDrawingAttributesReplaced -= h);
        }



        public static IObservable<EventPattern<RoutedEventArgs>> ActiveEditingModeChangedObservable(this InkCanvas This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.ActiveEditingModeChanged += h, h => This.ActiveEditingModeChanged -= h);
        }



        public static IObservable<EventPattern<RoutedEventArgs>> EditingModeChangedObservable(this InkCanvas This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.EditingModeChanged += h, h => This.EditingModeChanged -= h);
        }



        public static IObservable<EventPattern<RoutedEventArgs>> EditingModeInvertedChangedObservable(this InkCanvas This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.EditingModeInvertedChanged += h, h => This.EditingModeInvertedChanged -= h);
        }



        public static IObservable<EventPattern<InkCanvasSelectionEditingEventArgs>> SelectionMovingObservable(this InkCanvas This){
            return Observable.FromEventPattern<InkCanvasSelectionEditingEventHandler, InkCanvasSelectionEditingEventArgs>(h => This.SelectionMoving += h, h => This.SelectionMoving -= h);
        }



        public static IObservable<EventPattern<EventArgs>> SelectionMovedObservable(this InkCanvas This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.SelectionMoved += h, h => This.SelectionMoved -= h);
        }



        public static IObservable<EventPattern<InkCanvasStrokeErasingEventArgs>> StrokeErasingObservable(this InkCanvas This){
            return Observable.FromEventPattern<InkCanvasStrokeErasingEventHandler, InkCanvasStrokeErasingEventArgs>(h => This.StrokeErasing += h, h => This.StrokeErasing -= h);
        }



        public static IObservable<EventPattern<RoutedEventArgs>> StrokeErasedObservable(this InkCanvas This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.StrokeErased += h, h => This.StrokeErased -= h);
        }



        public static IObservable<EventPattern<InkCanvasSelectionEditingEventArgs>> SelectionResizingObservable(this InkCanvas This){
            return Observable.FromEventPattern<InkCanvasSelectionEditingEventHandler, InkCanvasSelectionEditingEventArgs>(h => This.SelectionResizing += h, h => This.SelectionResizing -= h);
        }



        public static IObservable<EventPattern<EventArgs>> SelectionResizedObservable(this InkCanvas This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.SelectionResized += h, h => This.SelectionResized -= h);
        }



        public static IObservable<EventPattern<InkCanvasSelectionChangingEventArgs>> SelectionChangingObservable(this InkCanvas This){
            return Observable.FromEventPattern<InkCanvasSelectionChangingEventHandler, InkCanvasSelectionChangingEventArgs>(h => This.SelectionChanging += h, h => This.SelectionChanging -= h);
        }



        public static IObservable<EventPattern<EventArgs>> SelectionChangedObservable(this InkCanvas This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.SelectionChanged += h, h => This.SelectionChanged -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   ItemContainerGenerator
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<ItemsChangedEventArgs>> ItemsChangedObservable(this ItemContainerGenerator This){
            return Observable.FromEventPattern<ItemsChangedEventHandler, ItemsChangedEventArgs>(h => This.ItemsChanged += h, h => This.ItemsChanged -= h);
        }



        public static IObservable<EventPattern<EventArgs>> StatusChangedObservable(this ItemContainerGenerator This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.StatusChanged += h, h => This.StatusChanged -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   MediaElement
////////////////////////////////////////////
////////////////////////////////////////////




        public static IObservable<EventPattern<RoutedEventArgs>> MediaOpenedObservable(this MediaElement This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.MediaOpened += h, h => This.MediaOpened -= h);
        }



        public static IObservable<EventPattern<RoutedEventArgs>> BufferingStartedObservable(this MediaElement This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.BufferingStarted += h, h => This.BufferingStarted -= h);
        }



        public static IObservable<EventPattern<RoutedEventArgs>> BufferingEndedObservable(this MediaElement This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.BufferingEnded += h, h => This.BufferingEnded -= h);
        }




        public static IObservable<EventPattern<RoutedEventArgs>> MediaEndedObservable(this MediaElement This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.MediaEnded += h, h => This.MediaEnded -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   PasswordBox
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<RoutedEventArgs>> PasswordChangedObservable(this PasswordBox This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.PasswordChanged += h, h => This.PasswordChanged -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   TextBoxBase
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<TextChangedEventArgs>> TextChangedObservable(this TextBoxBase This){
            return Observable.FromEventPattern<TextChangedEventHandler, TextChangedEventArgs>(h => This.TextChanged += h, h => This.TextChanged -= h);
        }



        public static IObservable<EventPattern<RoutedEventArgs>> SelectionChangedObservable(this TextBoxBase This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.SelectionChanged += h, h => This.SelectionChanged -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   DocumentPageView
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<EventArgs>> PageConnectedObservable(this DocumentPageView This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.PageConnected += h, h => This.PageConnected -= h);
        }



        public static IObservable<EventPattern<EventArgs>> PageDisconnectedObservable(this DocumentPageView This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.PageDisconnected += h, h => This.PageDisconnected -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   Popup
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<EventArgs>> OpenedObservable(this Popup This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.Opened += h, h => This.Opened -= h);
        }



        public static IObservable<EventPattern<EventArgs>> ClosedObservable(this Popup This){
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



        public static IObservable<EventPattern<ScrollEventArgs>> ScrollObservable(this ScrollBar This){
            return Observable.FromEventPattern<ScrollEventHandler, ScrollEventArgs>(h => This.Scroll += h, h => This.Scroll -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   ScrollViewer
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<ScrollChangedEventArgs>> ScrollChangedObservable(this ScrollViewer This){
            return Observable.FromEventPattern<ScrollChangedEventHandler, ScrollChangedEventArgs>(h => This.ScrollChanged += h, h => This.ScrollChanged -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   ToolTip
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<RoutedEventArgs>> OpenedObservable(this ToolTip This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Opened += h, h => This.Opened -= h);
        }



        public static IObservable<EventPattern<RoutedEventArgs>> ClosedObservable(this ToolTip This){
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



        public static IObservable<EventPattern<RoutedEventArgs>> ExpandedObservable(this TreeViewItem This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Expanded += h, h => This.Expanded -= h);
        }



        public static IObservable<EventPattern<RoutedEventArgs>> CollapsedObservable(this TreeViewItem This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Collapsed += h, h => This.Collapsed -= h);
        }



        public static IObservable<EventPattern<RoutedEventArgs>> SelectedObservable(this TreeViewItem This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Selected += h, h => This.Selected -= h);
        }



        public static IObservable<EventPattern<RoutedEventArgs>> UnselectedObservable(this TreeViewItem This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Unselected += h, h => This.Unselected -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   HwndHost
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<Int32>> MessageHookObservable(this HwndHost This){
            return Observable.FromEventPattern<HwndSourceHook, Int32>(h => This.MessageHook += h, h => This.MessageHook -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   WebBrowser
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<NavigatingCancelEventArgs>> NavigatingObservable(this WebBrowser This){
            return Observable.FromEventPattern<NavigatingCancelEventHandler, NavigatingCancelEventArgs>(h => This.Navigating += h, h => This.Navigating -= h);
        }



        public static IObservable<EventPattern<NavigationEventArgs>> NavigatedObservable(this WebBrowser This){
            return Observable.FromEventPattern<NavigatedEventHandler, NavigationEventArgs>(h => This.Navigated += h, h => This.Navigated -= h);
        }



        public static IObservable<EventPattern<NavigationEventArgs>> LoadCompletedObservable(this WebBrowser This){
            return Observable.FromEventPattern<LoadCompletedEventHandler, NavigationEventArgs>(h => This.LoadCompleted += h, h => This.LoadCompleted -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   CollectionViewSource
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<FilterEventArgs>> FilterObservable(this CollectionViewSource This){
            return Observable.FromEventPattern<FilterEventHandler, FilterEventArgs>(h => This.Filter += h, h => This.Filter -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   Hyperlink
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<RequestNavigateEventArgs>> RequestNavigateObservable(this Hyperlink This){
            return Observable.FromEventPattern<RequestNavigateEventHandler, RequestNavigateEventArgs>(h => This.RequestNavigate += h, h => This.RequestNavigate -= h);
        }



        public static IObservable<EventPattern<RoutedEventArgs>> ClickObservable(this Hyperlink This){
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => This.Click += h, h => This.Click -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   PageContent
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<GetPageRootCompletedEventArgs>> GetPageRootCompletedObservable(this PageContent This){
            return Observable.FromEventPattern<GetPageRootCompletedEventHandler, GetPageRootCompletedEventArgs>(h => This.GetPageRootCompleted += h, h => This.GetPageRootCompleted -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   TextRange
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<EventArgs>> ChangedObservable(this TextRange This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.Changed += h, h => This.Changed -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   SerializerWriter
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<WritingPrintTicketRequiredEventArgs>> WritingPrintTicketRequiredObservable(this SerializerWriter This){
            return Observable.FromEventPattern<WritingPrintTicketRequiredEventHandler, WritingPrintTicketRequiredEventArgs>(h => This.WritingPrintTicketRequired += h, h => This.WritingPrintTicketRequired -= h);
        }



        public static IObservable<EventPattern<WritingProgressChangedEventArgs>> WritingProgressChangedObservable(this SerializerWriter This){
            return Observable.FromEventPattern<WritingProgressChangedEventHandler, WritingProgressChangedEventArgs>(h => This.WritingProgressChanged += h, h => This.WritingProgressChanged -= h);
        }



        public static IObservable<EventPattern<WritingCompletedEventArgs>> WritingCompletedObservable(this SerializerWriter This){
            return Observable.FromEventPattern<WritingCompletedEventHandler, WritingCompletedEventArgs>(h => This.WritingCompleted += h, h => This.WritingCompleted -= h);
        }



        public static IObservable<EventPattern<WritingCancelledEventArgs>> WritingCancelledObservable(this SerializerWriter This){
            return Observable.FromEventPattern<WritingCancelledEventHandler, WritingCancelledEventArgs>(h => This.WritingCancelled += h, h => This.WritingCancelled -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   NavigationService
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<NavigationFailedEventArgs>> NavigationFailedObservable(this NavigationService This){
            return Observable.FromEventPattern<NavigationFailedEventHandler, NavigationFailedEventArgs>(h => This.NavigationFailed += h, h => This.NavigationFailed -= h);
        }



        public static IObservable<EventPattern<NavigatingCancelEventArgs>> NavigatingObservable(this NavigationService This){
            return Observable.FromEventPattern<NavigatingCancelEventHandler, NavigatingCancelEventArgs>(h => This.Navigating += h, h => This.Navigating -= h);
        }



        public static IObservable<EventPattern<NavigationEventArgs>> NavigatedObservable(this NavigationService This){
            return Observable.FromEventPattern<NavigatedEventHandler, NavigationEventArgs>(h => This.Navigated += h, h => This.Navigated -= h);
        }



        public static IObservable<EventPattern<NavigationProgressEventArgs>> NavigationProgressObservable(this NavigationService This){
            return Observable.FromEventPattern<NavigationProgressEventHandler, NavigationProgressEventArgs>(h => This.NavigationProgress += h, h => This.NavigationProgress -= h);
        }



        public static IObservable<EventPattern<NavigationEventArgs>> LoadCompletedObservable(this NavigationService This){
            return Observable.FromEventPattern<LoadCompletedEventHandler, NavigationEventArgs>(h => This.LoadCompleted += h, h => This.LoadCompleted -= h);
        }



        public static IObservable<EventPattern<FragmentNavigationEventArgs>> FragmentNavigationObservable(this NavigationService This){
            return Observable.FromEventPattern<FragmentNavigationEventHandler, FragmentNavigationEventArgs>(h => This.FragmentNavigation += h, h => This.FragmentNavigation -= h);
        }



        public static IObservable<EventPattern<NavigationEventArgs>> NavigationStoppedObservable(this NavigationService This){
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



        public static IObservable<EventPattern<AsyncCompletedEventArgs>> LoadCompletedObservable(this XamlReader This){
            return Observable.FromEventPattern<AsyncCompletedEventHandler, AsyncCompletedEventArgs>(h => This.LoadCompleted += h, h => This.LoadCompleted -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   BamlLocalizer
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<BamlLocalizerErrorNotifyEventArgs>> ErrorNotifyObservable(this BamlLocalizer This){
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



        public static IObservable<EventPattern<EventArgs>> ClickObservable(this ThumbButtonInfo This){
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



        public static IObservable<EventPattern<CurrentChangingEventArgs>> CurrentChangingObservable(this ICollectionView This){
            return Observable.FromEventPattern<CurrentChangingEventHandler, CurrentChangingEventArgs>(h => This.CurrentChanging += h, h => This.CurrentChanging -= h);
        }



        public static IObservable<EventPattern<EventArgs>> CurrentChangedObservable(this ICollectionView This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.CurrentChanged += h, h => This.CurrentChanged -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   PackageDigitalSignatureManager
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<SignatureVerificationEventArgs>> InvalidSignatureEventObservable(this PackageDigitalSignatureManager This){
            return Observable.FromEventPattern<InvalidSignatureEventHandler, SignatureVerificationEventArgs>(h => This.InvalidSignatureEvent += h, h => This.InvalidSignatureEvent -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   Freezable
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<EventArgs>> ChangedObservable(this Freezable This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.Changed += h, h => This.Changed -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   DataSourceProvider
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<EventArgs>> DataChangedObservable(this DataSourceProvider This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.DataChanged += h, h => This.DataChanged -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   Dispatcher
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<EventArgs>> ShutdownStartedObservable(this Dispatcher This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.ShutdownStarted += h, h => This.ShutdownStarted -= h);
        }



        public static IObservable<EventPattern<EventArgs>> ShutdownFinishedObservable(this Dispatcher This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.ShutdownFinished += h, h => This.ShutdownFinished -= h);
        }



        public static IObservable<EventPattern<DispatcherUnhandledExceptionFilterEventArgs>> UnhandledExceptionFilterObservable(this Dispatcher This){
            return Observable.FromEventPattern<DispatcherUnhandledExceptionFilterEventHandler, DispatcherUnhandledExceptionFilterEventArgs>(h => This.UnhandledExceptionFilter += h, h => This.UnhandledExceptionFilter -= h);
        }



        public static IObservable<EventPattern<DispatcherUnhandledExceptionEventArgs>> UnhandledExceptionObservable(this Dispatcher This){
            return Observable.FromEventPattern<DispatcherUnhandledExceptionEventHandler, DispatcherUnhandledExceptionEventArgs>(h => This.UnhandledException += h, h => This.UnhandledException -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   DispatcherHooks
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<EventArgs>> DispatcherInactiveObservable(this DispatcherHooks This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.DispatcherInactive += h, h => This.DispatcherInactive -= h);
        }



        public static IObservable<EventPattern<DispatcherHookEventArgs>> OperationPostedObservable(this DispatcherHooks This){
            return Observable.FromEventPattern<DispatcherHookEventHandler, DispatcherHookEventArgs>(h => This.OperationPosted += h, h => This.OperationPosted -= h);
        }



        public static IObservable<EventPattern<DispatcherHookEventArgs>> OperationStartedObservable(this DispatcherHooks This){
            return Observable.FromEventPattern<DispatcherHookEventHandler, DispatcherHookEventArgs>(h => This.OperationStarted += h, h => This.OperationStarted -= h);
        }



        public static IObservable<EventPattern<DispatcherHookEventArgs>> OperationCompletedObservable(this DispatcherHooks This){
            return Observable.FromEventPattern<DispatcherHookEventHandler, DispatcherHookEventArgs>(h => This.OperationCompleted += h, h => This.OperationCompleted -= h);
        }



        public static IObservable<EventPattern<DispatcherHookEventArgs>> OperationPriorityChangedObservable(this DispatcherHooks This){
            return Observable.FromEventPattern<DispatcherHookEventHandler, DispatcherHookEventArgs>(h => This.OperationPriorityChanged += h, h => This.OperationPriorityChanged -= h);
        }



        public static IObservable<EventPattern<DispatcherHookEventArgs>> OperationAbortedObservable(this DispatcherHooks This){
            return Observable.FromEventPattern<DispatcherHookEventHandler, DispatcherHookEventArgs>(h => This.OperationAborted += h, h => This.OperationAborted -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   DispatcherOperation
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<EventArgs>> AbortedObservable(this DispatcherOperation This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.Aborted += h, h => This.Aborted -= h);
        }



        public static IObservable<EventPattern<EventArgs>> CompletedObservable(this DispatcherOperation This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.Completed += h, h => This.Completed -= h);
        }

 
////////////////////////////////////////////
////////////////////////////////////////////
////   DispatcherTimer
////////////////////////////////////////////
////////////////////////////////////////////



        public static IObservable<EventPattern<EventArgs>> TickObservable(this DispatcherTimer This){
            return Observable.FromEventPattern<EventHandler, EventArgs>(h => This.Tick += h, h => This.Tick -= h);
        }



    }
}