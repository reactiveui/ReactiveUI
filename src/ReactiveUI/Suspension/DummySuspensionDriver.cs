using System;
using System.Reactive;
using System.Reactive.Linq;

namespace ReactiveUI
{
    /// <summary>
    ///
    /// </summary>
    public class DummySuspensionDriver : ISuspensionDriver
    {
        /// <inheritdoc/>
        public IObservable<object> LoadState()
        {
            return Observable<object>.Default;
        }

        /// <inheritdoc/>
        public IObservable<Unit> SaveState(object state)
        {
            return Observables.Unit;
        }

        /// <inheritdoc/>
        public IObservable<Unit> InvalidateState()
        {
            return Observables.Unit;
        }
    }
}