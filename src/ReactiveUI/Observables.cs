namespace System.Reactive.Linq
{
    /// <summary>
    /// Provides commonly required, statically-allocated, pre-canned observables.
    /// </summary>
    /// <typeparam name="T">
    /// The observable type.
    /// </typeparam>
    internal static class Observable<T>
    {
        /// <summary>
        /// An empty observable of type <typeparamref name="T"/>.
        /// </summary>
        public static readonly IObservable<T> Empty = Observable.Empty<T>();

        /// <summary>
        /// An observable of type <typeparamref name="T"/> that never ticks a value.
        /// </summary>
        public static readonly IObservable<T> Never = Observable.Never<T>();

        /// <summary>
        /// An observable of type <typeparamref name="T"/> that ticks a single, default value.
        /// </summary>
        public static readonly IObservable<T> Default = Observable.Return(default(T));
    }

    /// <summary>
    /// Provides commonly required, statically-allocated, pre-canned observables.
    /// </summary>
    internal static class Observables
    {
        /// <summary>
        /// An observable that ticks a single, Boolean value of <c>true</c>.
        /// </summary>
        public static readonly IObservable<bool> True = Observable.Return(true);

        /// <summary>
        /// An observable that ticks a single, Boolean value of <c>false</c>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This observable is equivalent to <c>Observable&lt;bool&gt;.Default</c>, but is provided for convenience.
        /// </para>
        /// </remarks>
        public static readonly IObservable<bool> False = Observable.Return(false);

        /// <summary>
        /// An observable that ticks <c>Unit.Default</c> as a single value.</summary>
        /// <remarks>
        /// <para>
        /// This observable is equivalent to <c>Observable&lt;Unit&gt;.Default</c>, but is provided for convenience.
        /// </para>
        /// </remarks>
        public static readonly IObservable<Unit> Unit = Observable<Unit>.Default;
    }
}