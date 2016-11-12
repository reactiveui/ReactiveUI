namespace ReactiveUI
{
    using System;

    /// <summary>
    /// Allows an additional string to make view resolution more specific than
    /// just a type. When applied to your <see cref="IViewFor{T}"/> -derived
    /// View, you can select between different Views for a single ViewModel
    /// instance.
    /// </summary>
    public class ViewContractAttribute : Attribute
    {
        /// <summary>
        /// Constructs the ViewContractAttribute with a specific contract value.
        /// </summary>
        /// <param name="contract">The value of the contract for view
        /// resolution.</param>
        public ViewContractAttribute(string contract)
        {
            Contract = contract;
        }

        internal string Contract { get; }
    }

    /// <summary>
    /// Indicates that this View should be constructed _once_ and then used
    /// every time its ViewModel View is resolved.
    /// Obviously, this is not supported on Views that may be reused multiple
    /// times in the Visual Tree.
    /// </summary>
    public class SingleInstanceViewAttribute : Attribute
    {
    }
}