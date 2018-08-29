using System;

namespace ReactiveUI
{
    /// <summary>
    /// Indicates that this View should be constructed _once_ and then used
    /// every time its ViewModel View is resolved.
    /// Obviously, this is not supported on Views that may be reused multiple
    /// times in the Visual Tree.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SingleInstanceViewAttribute : Attribute
    {
    }
}