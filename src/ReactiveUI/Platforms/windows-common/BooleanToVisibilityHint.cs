using System;
using System.Diagnostics.CodeAnalysis;
#if NETFX_CORE
using Windows.UI.Xaml;
#else
using System.Windows;
#endif

namespace ReactiveUI
{
    /// <summary>
    /// Enum that hints at the visibility of a ui element.
    /// </summary>
    [SuppressMessage("Name", "CA1714: Flags enums should have plural names", Justification = "For legacy support")]
    [Flags]
    public enum BooleanToVisibilityHint
    {
        /// <summary>
        /// Do not modify the boolean type conversion from it's default action of using the Visibility.Collapsed.
        /// </summary>
        None = 0,

        /// <summary>
        /// Inverse the action of the boolean type conversion, when it's true collapse the visibility.
        /// </summary>
        Inverse = 1 << 1,

#if !NETFX_CORE
        /// <summary>
        /// Use the hidden version rather than the Collapsed.
        /// </summary>
        UseHidden = 1 << 2,
#endif
    }
}
