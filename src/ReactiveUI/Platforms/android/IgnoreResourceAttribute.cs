using System;

namespace ReactiveUI
{
    /// <summary>
    /// Attribute that marks a resource to be ignored.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class IgnoreResourceAttribute : Attribute
    {
    }
}
