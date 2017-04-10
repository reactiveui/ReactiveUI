using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Android.Content;

namespace ReactiveUI
{
    public static class SharedPreferencesExtensions
    {
        /// <summary>
        /// A observable sequence of keys for changed shared preferences.
        /// </summary>
        /// <returns>The observable sequence of keys for changed shared preferences.</returns>
        /// <param name="sharedPreferences">The shared preferences to get the changes from.</param>
        public static IObservable<string> PreferenceChanged(this ISharedPreferences sharedPreferences)
        {
            return Observable.Create<string> (observer => {
                var listener = new OnSharedPreferenceChangeListener(observer);
                sharedPreferences.RegisterOnSharedPreferenceChangeListener(listener);
                return Disposable.Create (() => sharedPreferences.UnregisterOnSharedPreferenceChangeListener (listener));
            });
        }

        /// <summary>
        /// Private implementation of ISharedPreferencesOnSharedPreferenceChangeListener
        /// </summary>
        class OnSharedPreferenceChangeListener
            : Java.Lang.Object
            , ISharedPreferencesOnSharedPreferenceChangeListener
        {
            readonly IObserver<string> observer;

            public OnSharedPreferenceChangeListener(IObserver<string> observer)
            {
                this.observer = observer;
            }

            void ISharedPreferencesOnSharedPreferenceChangeListener.OnSharedPreferenceChanged (ISharedPreferences sharedPreferences, string key)
            {
                observer.OnNext (key);
            }
        }
    }
}

