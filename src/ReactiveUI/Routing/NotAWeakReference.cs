namespace ReactiveUI
{
    internal class NotAWeakReference
    {
        public NotAWeakReference(object target)
        {
            Target = target;
        }

        public object Target { get; private set; }

        public bool IsAlive => true;
    }
}