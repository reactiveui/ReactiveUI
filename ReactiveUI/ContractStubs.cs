using System;

#if DOTNETISOLDANDSAD || WP7

namespace System.Diagnostics.Contracts
{
    internal class ContractInvariantMethodAttribute : Attribute {}
    
    internal class Contract
    {
        public static void Requires(bool b, string s = null) {}
        public static void Ensures(bool b, string s = null) {}
        public static void Invariant(bool b, string s = null) {}
        public static T Result<T>() { return default(T); }
    }
}

#endif

#if IOS || WINRT

namespace System.ComponentModel
{
    public class PropertyChangingEventArgs : EventArgs
    {
        public PropertyChangingEventArgs(string PropertyName)
        {
            this.PropertyName = PropertyName;
        }

        public string PropertyName { get; protected set; }
    }

    public delegate void PropertyChangingEventHandler(
    	Object sender,
    	PropertyChangingEventArgs e
    );

    public interface INotifyPropertyChanging 
    {
        event PropertyChangingEventHandler PropertyChanging;
    }
}
#endif

#if SILVERLIGHT || WINRT
namespace System
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class LocalizableAttribute : Attribute
    {
        // This is a positional argument
        public LocalizableAttribute(bool isLocalizable)
        {
        }
    }
}
#endif

// vim: tw=120 ts=4 sw=4 et :
