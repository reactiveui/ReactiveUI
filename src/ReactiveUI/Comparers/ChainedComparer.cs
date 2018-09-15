using System;
using System.Collections.Generic;

namespace ReactiveUI
{
    internal sealed class ChainedComparer<T> : IComparer<T>
    {
        private readonly IComparer<T> _parent;
        private readonly Comparison<T> _inner;

        public ChainedComparer(IComparer<T> parent, Comparison<T> comparison)
        {
            if (comparison == null)
            {
                throw new ArgumentNullException(nameof(comparison));
            }

            _parent = parent;
            _inner = comparison;
        }

        /// <inheritdoc />
        public int Compare(T x, T y)
        {
            int parentResult = _parent == null ? 0 : _parent.Compare(x, y);

            return parentResult != 0 ? parentResult : _inner(x, y);
        }
    }
}