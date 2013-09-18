// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.Reactive.Testing
{
    /// <summary>
    /// Records information about subscriptions to and unsubscriptions from observable sequences.
    /// </summary>
#if !NO_DEBUGGER_ATTRIBUTES
    [DebuggerDisplay("({Subscribe}, {Unsubscribe})")]
#endif
#if !NO_SERIALIZABLE
    [Serializable]
#endif
    public struct Subscription : IEquatable<Subscription>
    {
        /// <summary>
        /// Infinite virtual time value, used to indicate an unsubscription never took place.
        /// </summary>
        public const long Infinite = long.MaxValue;

        private long _subscribe;
        private long _unsubscribe;

        /// <summary>
        /// Gets the subscription virtual time.
        /// </summary>
        public long Subscribe { get { return _subscribe; } }

        /// <summary>
        /// Gets the unsubscription virtual time.
        /// </summary>
        public long Unsubscribe { get { return _unsubscribe; } }

        /// <summary>
        /// Creates a new subscription object with the given virtual subscription time.
        /// </summary>
        /// <param name="subscribe">Virtual time at which the subscription occurred.</param>-
        public Subscription(long subscribe)
        {
            _subscribe = subscribe;
            _unsubscribe = Infinite;
        }

        /// <summary>
        /// Creates a new subscription object with the given virtual subscription and unsubscription time.
        /// </summary>
        /// <param name="subscribe">Virtual time at which the subscription occurred.</param>
        /// <param name="unsubscribe">Virtual time at which the unsubscription occurred.</param>
        public Subscription(long subscribe, long unsubscribe)
        {
            _subscribe = subscribe;
            _unsubscribe = unsubscribe;
        }

        /// <summary>
        /// Checks whether the given subscription is equal to the current instance.
        /// </summary>
        /// <param name="other">Subscription object to check for equality.</param>
        /// <returns>true if both objects are equal; false otherwise.</returns>
        public bool Equals(Subscription other)
        {
            return Subscribe == other.Subscribe && Unsubscribe == other.Unsubscribe;
        }

        /// <summary>
        /// Determines whether the two specified Subscription values have the same Subscribe and Unsubscribe.
        /// </summary>
        /// <param name="left">The first Subscription value to compare.</param>
        /// <param name="right">The second Subscription value to compare.</param>
        /// <returns>true if the first Subscription value has the same Subscribe and Unsubscribe as the second Subscription value; otherwise, false.</returns>
        public static bool operator==(Subscription left, Subscription right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether the two specified Subscription values don't have the same Subscribe and Unsubscribe.
        /// </summary>
        /// <param name="left">The first Subscription value to compare.</param>
        /// <param name="right">The second Subscription value to compare.</param>
        /// <returns>true if the first Subscription value has a different Subscribe or Unsubscribe as the second Subscription value; otherwise, false.</returns>
        public static bool operator !=(Subscription left, Subscription right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Determines whether the specified System.Object is equal to the current Subscription value.
        /// </summary>
        /// <param name="obj">The System.Object to compare with the current Subscription value.</param>
        /// <returns>true if the specified System.Object is equal to the current Subscription value; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Subscription)
                return Equals((Subscription)obj);
            return false;
        }

        /// <summary>
        /// Returns the hash code for the current Subscription value.
        /// </summary>
        /// <returns>A hash code for the current Subscription value.</returns>
        public override int GetHashCode()
        {
            return Subscribe.GetHashCode() ^ Unsubscribe.GetHashCode();
        }

        /// <summary>
        /// Returns a string representation of the current Subscription value.
        /// </summary>
        /// <returns>String representation of the current Subscription value.</returns>
        public override string ToString()
        {
            if (Unsubscribe == Infinite)
                return string.Format(CultureInfo.CurrentCulture, "({0}, Infinite)", Subscribe);
            else
                return string.Format(CultureInfo.CurrentCulture, "({0}, {1})", Subscribe, Unsubscribe);
        }
    }
}
