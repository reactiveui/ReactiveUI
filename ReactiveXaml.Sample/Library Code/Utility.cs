using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Interactivity;
using System.Security.Principal;
using System.Text.RegularExpressions;

namespace ReactiveXamlSample
{
    /// <summary>
    /// This class contains miscellaneous methods to do UI-related tasks.
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// This function converts colors expressed as HSV into the RGB color
       	/// space.
        /// </summary>
        /// <param name="h">Hue</param>
        /// <param name="s">Saturation</param>
        /// <param name="v">Value</param>
        /// <returns>The associated Color.</returns>
        public static Color ColorFromHSV(float h, float s, float v)
        {
            float r, g, b;

            if( s == 0.0 ) {
                // achromatic (grey)
                r = g = b = v;
                return Color.FromScRgb(1.0f, r, g, b);
            }

            h *= 360.0f;
            h /= 60.0f;                      // sector 0 to 5
            int i = (int)Math.Floor( h );
            float f = h - i;
            float p = v * ( 1.0f - s );
            float q = v * ( 1.0f - s * f );
            float t = v * ( 1.0f - s * ( 1.0f - f ) );

            switch( i ) {
                case 0:
                    r = v; g = t; b = p;
                    break;
                case 1:
                    r = q; g = v; b = p;
                    break;
                case 2:
                    r = p; g = v; b = t;
                    break;
                case 3:
                    r = p; g = q; b = v;
                    break;
                case 4:
                    r = t; g = p; b = v;
                    break;
                default:                // case 5:
                    r = v; g = p; b = q;
                    break;
            }

            return Color.FromScRgb(1.0f, r, g, b);
        }
        
        /// <summary>
        /// This class registers a callback on a dependency object to be called
	    /// when the value of the DP changes.
        /// </summary>
        /// <param name="owner">The owning object.</param>
        /// <param name="property">The DependencyProperty to watch.</param>
        /// <param name="handler">The action to call out when the DP changes.</param>
        public static void RegisterDepPropCallback(object owner, DependencyProperty property, EventHandler handler)
        {
            // FIXME: We could implement this as an extension, but let's not get
            // too Ruby-like
            var dpd = DependencyPropertyDescriptor.FromProperty(property, owner.GetType());
            dpd.AddValueChanged(owner, handler);
        }

        public static T FindContainingElement<T>(DependencyObject element)
            where T : class
        {
            var ret = element as T;
            if (ret != null)
                return ret;

            if (element == null)
                return null;

            return FindContainingElement<T>(VisualTreeHelper.GetParent(element));
        }

        public static int MatchingCharactersInSubstring(string search_for, string data, bool break_if_found)
        {
            var haystack = data.ToLowerInvariant();
            var needle = search_for.ToLowerInvariant();
            int start_at = 0;
            int ret = 0;
            for(int i = 0; i < needle.Length; i++) {
                int index = haystack.IndexOf(needle[i], start_at);
                if (index < 0)
                    continue;

                if (break_if_found)
                    return 1;
                ret++;
                start_at = index;
            }

            return ret;        
        }

        public static string getCurrentUser()
        {
            //try {
            //    var ret = ClientManager.GetCurrentUsername();
            //    if (!String.IsNullOrEmpty(ret))
            //        return ret;
            //} catch(Exception e) {
            //    this.Log().Error("Failed to fetch username server-side", e);
            //}

            // HACK HACK HACK - this is only to make sure the Company Meeting
            // scenario works; we should always be relying on the server-side
            // confirmation of the user name
            if (Environment.GetEnvironmentVariable("MT_FORCED_USER_NAME") != null)
                return Environment.GetEnvironmentVariable("MT_FORCED_USER_NAME");

            var user = WindowsIdentity.GetCurrent();
            if (user == null || String.IsNullOrEmpty(user.Name))
                throw new Exception("Couldn't determine current user");

            // Knock out the domain from REDMOND\blabla
            return Regex.Replace(user.Name, @"^[^\\]*\\", "");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Visibility) || !(value is bool))
                throw new InvalidCastException();

            return ((bool)value) ? Visibility.Hidden : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(bool) || !(value is Visibility))
                throw new InvalidCastException();

            return ((Visibility)value == Visibility.Visible) ? false : true;
        }
    }

    public class InverseBooleanToCollapsedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Visibility) || !(value is bool))
                throw new InvalidCastException();

            return ((bool)value) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(bool) || !(value is Visibility))
                throw new InvalidCastException();

            return ((Visibility)value == Visibility.Visible) ? false : true;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(bool) || !(value is bool))
                throw new InvalidCastException();
            
            return (!(bool)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(bool) || !(value is bool))
                throw new InvalidCastException();

            return (!(bool)value);
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    public class WindowVisibilityToBooleanConverter : IValueConverter
    {
        /* We need this class because Window does weird things when you set its
         * visibility to collapsed */

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v = (Visibility)value;
            if (targetType != typeof(bool?) || !(value is Visibility))
                throw new InvalidCastException();

            return (v == Visibility.Visible ? true : false);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var b = (bool?)value;
            if (targetType != typeof(Visibility) || !(value is bool?))
                throw new InvalidCastException();

            return (b.HasValue && b.Value ? Visibility.Visible : Visibility.Hidden);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DebugConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Debug.Write(String.Format(CultureInfo.CurrentUICulture, "Convert '{0}' to {1}", value, targetType));
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Debug.Write(String.Format(CultureInfo.CurrentUICulture, "Convert Back '{0}' to {1}", value, targetType));
            return value;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class StringFilterMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(IEnumerable))
                throw new InvalidCastException();
            if (values.Length != 2)
                throw new ArgumentException();

            var needle = values[0] as string;
            var haystack = values[1] as IEnumerable<string>;            
            if (haystack == null)
                return null;
            if (String.IsNullOrEmpty(needle))
                return haystack;

            return haystack.Where(x => x.Contains(needle));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class GrabFocusAction : TriggerAction<FrameworkElement>
    {
        protected override void Invoke(object parameter)
        {
            var fe = AssociatedObject;
            if (fe != null && fe.Focusable)
                fe.Focus();
        }
    }

    public class BringIntoViewAction : TriggerAction<FrameworkElement>
    {
        protected override void Invoke(object parameter)
        {
            AssociatedObject.BringIntoView();
        }
    }

    public class DoubleAdditionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double ret = (double)value;
            double to_add = Double.Parse((string)parameter, NumberStyles.Float, CultureInfo.InvariantCulture);
            return ret + to_add;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double ret = (double)value;
            double to_add = (double)parameter;
            return ret - to_add;

        }
    }

    public class CloseWindowAction : TargetedTriggerAction<Window>
    {
        protected override void Invoke(object parameter)
        {
            Target.Close();
        }
    }

    


    /*
     * P/Invoke Section
     */

    public enum ShowWindow : int
    {
        Hide = 0,
        Normal = 1,
        ShowMinimized = 2,
        Maximize = 3, 
        ShowMaximized = 3,
        ShowNoActivate = 4,
        Show = 5,
        Minimize = 6,
        ShowMinNoActive = 7,
        ShowNA = 8,
        Restore = 9,
        ShowDefault = 10,
        ForceMinimize = 11
    }

    public sealed class PInvoke
    {
        private PInvoke() {} 

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr ShellExecute(IntPtr hwnd, 
                string lpOperation, 
                string lpFile, 
                string lpParameters, 
                string lpDirectory, 
                ShowWindow nShowCmd);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int MessageBox(IntPtr hWnd,
                                    string msg,
                                    string caption,
                                    int type);

        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentProcessId();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
    }
}

// vim: tw=120 ts=4 sw=4 et enc=utf8
