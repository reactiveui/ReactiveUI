using System;

namespace ReactiveUI
{
    /// <summary>
    /// Attribute that marks a resource for wiring.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class WireUpResourceAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WireUpResourceAttribute"/> class.
        /// </summary>
        public WireUpResourceAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WireUpResourceAttribute"/> class.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        public WireUpResourceAttribute(string resourceName)
        {
            ResourceNameOverride = resourceName;
        }

        /// <summary>
        /// Gets the resource name override.
        /// </summary>
        public string ResourceNameOverride { get; }
    }
}
