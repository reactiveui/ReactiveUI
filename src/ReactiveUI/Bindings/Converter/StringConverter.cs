// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

namespace ReactiveUI
{
    /// <summary>
    /// Calls ToString on types. In WPF, ComponentTypeConverter should win
    /// instead of this, since It's Better™.
    /// </summary>
    public class StringConverter : IBindingTypeConverter
    {
        /// <inheritdoc/>
        public int GetAffinityForObjects(Type fromType, Type toType) => toType == typeof(string) ? 2 : 0;

        /// <inheritdoc/>
        public bool TryConvert(object? @from, Type toType, object? conversionHint, out object? result)
        {
            // XXX: All Of The Localization
            result = from?.ToString();
            return true;
        }
    }
}
