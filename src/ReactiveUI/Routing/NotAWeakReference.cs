using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI
{
    internal class NotAWeakReference
    {
        public NotAWeakReference(object target)
        {
            Target = target;
        }

        public object Target { get; private set; }

        [SuppressMessage("Microsoft.Maintainability", "CA1822", Justification = "Keep existing API.")]
        public bool IsAlive => true;
    }
}