// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;

namespace ReactiveUI
{
    /// <summary>
    /// An update for the index normalizer.
    /// </summary>
    public sealed class Update
    {
        private Update(UpdateType type, int index, bool isDuplicate = false)
        {
            Type = type;
            Index = index;
            IsDuplicate = isDuplicate;
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        public UpdateType Type { get; }

        /// <summary>
        /// Gets the index.
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is duplicate.
        /// </summary>
        public bool IsDuplicate { get; }

        /// <summary>
        /// Creates an update for the added index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The updated add.</returns>
        public static Update CreateAdd(int index) => new(UpdateType.Add, index);

        /// <summary>
        /// Creates an update for the the deleted index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The updated delete.</returns>
        public static Update CreateDelete(int index) => new(UpdateType.Delete, index);

        /// <summary>
        /// Creates an update of the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="index">The index.</param>
        /// <returns>And update.</returns>
        public static Update Create(UpdateType type, int index) => new(type, index);

        /// <inheritdoc/>
        public override string ToString() => Type.ToString()[0] + Index.ToString(CultureInfo.InvariantCulture);

        /// <summary>
        /// Created a duplicate update.
        /// </summary>
        /// <returns>The duplicate update.</returns>
        public Update AsDuplicate() => new(Type, Index, isDuplicate: true);
    }
}
