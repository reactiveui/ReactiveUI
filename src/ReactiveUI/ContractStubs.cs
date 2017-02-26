using System;
using System.Collections.Specialized;

namespace ReactiveUI
{
    /// <summary>
    /// Property Changing Event Arguments
    /// </summary>
    /// <seealso cref="System.EventArgs"/>
    public class PropertyChangingEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyChangingEventArgs"/> class.
        /// </summary>
        /// <param name="PropertyName">Name of the property.</param>
        public PropertyChangingEventArgs(string PropertyName)
        {
            this.PropertyName = PropertyName;
        }

        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        /// <value>The name of the property.</value>
        public string PropertyName { get; protected set; }
    }

    /// <summary>
    /// Property Changing Event Handler
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">
    /// The <see cref="PropertyChangingEventArgs"/> instance containing the event data.
    /// </param>
    public delegate void PropertyChangingEventHandler(
        Object sender,
        PropertyChangingEventArgs e
    );

    /// <summary>
    /// interface for NotifyPropertyChanging
    /// </summary>
    public interface INotifyPropertyChanging
    {
        /// <summary>
        /// Occurs when [property changing].
        /// </summary>
        event PropertyChangingEventHandler PropertyChanging;
    }
}

namespace ReactiveUI
{
    /// <summary>
    /// interface for NotifyCollectionChanging
    /// </summary>
    public interface INotifyCollectionChanging
    {
        /// <summary>
        /// Occurs when [collection changing].
        /// </summary>
        event NotifyCollectionChangedEventHandler CollectionChanging;
    }
}

#if PORTABLE || NETFX_CORE
namespace ReactiveUI
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