// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.Reactive.Testing
{
    /// <summary>
    /// Record of a value including the virtual time it was produced on.
    /// </summary>
    /// <typeparam name="T">Type of the value.</typeparam>
#if !NO_DEBUGGER_ATTRIBUTES
    [DebuggerDisplay("{Value}@{Time}")]
#endif
#if !NO_SERIALIZABLE
    [Serializable]
#endif
    public struct Recorded<T> : IEquatable<Recorded<T>>
    {
        private readonly long _time;
        private readonly T _value;

        /// <summary>
        /// Gets the virtual time the value was produced on.
        /// </summary>
        public long Time { get { return _time; } }

        /// <summary>
        /// Gets the recorded value.
        /// </summary>
        public T Value { get { return _value; } }

        /// <summary>
        /// Creates a new object recording the production of the specified value at the given virtual time.
        /// </summary>
        /// <param name="time">Virtual time the value was produced on.</param>
        /// <param name="value">Value that was produced.</param>
        public Recorded(long time, T value)
        {
            _time = time;
            _value = value;
        }

        /// <summary>
        /// Checks whether the given recorded object is equal to the current instance.
        /// </summary>
        /// <param name="other">Recorded object to check for equality.</param>
        /// <returns>true if both objects are equal; false otherwise.</returns>
        public bool Equals(Recorded<T> other)
        {
            return Time == other.Time && EqualityComparer<T>.Default.Equals(Value, other.Value);
        }

        /// <summary>
        /// Determines whether the two specified Recorded&lt;T&gt; values have the same Time and Value.
        /// </summary>
        /// <param name="left">The first Recorded&lt;T&gt; value to compare.</param>
        /// <param name="right">The second Recorded&lt;T&gt; value to compare.</param>
        /// <returns>true if the first Recorded&lt;T&gt; value has the same Time and Value as the second Recorded&lt;T&gt; value; otherwise, false.</returns>
        public static bool operator ==(Recorded<T> left, Recorded<T> right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether the two specified Recorded&lt;T&gt; values don't have the same Time and Value.
        /// </summary>
        /// <param name="left">The first Recorded&lt;T&gt; value to compare.</param>
        /// <param name="right">The second Recorded&lt;T&gt; value to compare.</param>
        /// <returns>true if the first Recorded&lt;T&gt; value has a different Time or Value as the second Recorded&lt;T&gt; value; otherwise, false.</returns>
        public static bool operator !=(Recorded<T> left, Recorded<T> right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Determines whether the specified System.Object is equal to the current Recorded&lt;T&gt; value.
        /// </summary>
        /// <param name="obj">The System.Object to compare with the current Recorded&lt;T&gt; value.</param>
        /// <returns>true if the specified System.Object is equal to the current Recorded&lt;T&gt; value; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Recorded<T>)
                return Equals((Recorded<T>)obj);
            return false;
        }

        /// <summary>
        /// Returns the hash code for the current Recorded&lt;T&gt; value.
        /// </summary>
        /// <returns>A hash code for the current Recorded&lt;T&gt; value.</returns>
        public override int GetHashCode()
        {
            return Time.GetHashCode() + EqualityComparer<T>.Default.GetHashCode(Value);
        }

        /// <summary>
        /// Returns a string representation of the current Recorded&lt;T&gt; value.
        /// </summary>
        /// <returns>String representation of the current Recorded&lt;T&gt; value.</returns>
        public override string ToString()
        {
            return Value.ToString() + "@" + Time.ToString(CultureInfo.CurrentCulture);
        }
    }
}
