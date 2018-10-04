// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ReactiveUI
{
    /// <summary>
    /// An update for the index normalizer.
    /// </summary>
    public sealed class Update
    {
        private readonly UpdateType _type;
        private readonly int _index;
        private readonly bool _isDuplicate;

        private Update(UpdateType type, int index, bool isDuplicate = false)
        {
            _type = type;
            _index = index;
            _isDuplicate = isDuplicate;
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        public UpdateType Type => _type;

        /// <summary>
        /// Gets the index.
        /// </summary>
        public int Index => _index;

        /// <summary>
        /// Gets a value indicating whether this instance is duplicate.
        /// </summary>
        public bool IsDuplicate => _isDuplicate;

        /// <inheritdoc/>
        public override string ToString()
        {
            return _type.ToString()[0] + _index.ToString();
        }

        /// <summary>
        /// Created a duplicate update.
        /// </summary>
        /// <returns>The duplicate update.</returns>
        public Update AsDuplicate()
        {
            return new Update(_type, _index, isDuplicate: true);
        }

        /// <summary>
        /// Creates an update of the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="index">The index.</param>
        /// <returns>And update.</returns>
        public static Update Create(UpdateType type, int index)
        {
            return new Update(type, index);
        }

        /// <summary>
        /// Creates an update for the added index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The updated add.</returns>
        public static Update CreateAdd(int index)
        {
            return new Update(UpdateType.Add, index);
        }

        /// <summary>
        /// Creates an update for the the deleted index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The updated delete.</returns>
        public static Update CreateDelete(int index)
        {
            return new Update(UpdateType.Delete, index);
        }
    }
}
