// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ReactiveUI.Winforms
{
    /// <summary>
    /// A binding set converter which will convert from a Table.
    /// </summary>
    public class TableContentSetMethodBindingConverter : ISetMethodBindingConverter
    {
        /// <inheritdoc />
        public int GetAffinityForObjects(Type fromType, Type toType)
        {
            if (toType != typeof(TableLayoutControlCollection))
            {
                return 0;
            }

            if (fromType?.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>) && x.GetGenericArguments().First().IsSubclassOf(typeof(Control))) ?? false)
            {
                return 15;
            }

            return 0;
        }

        /// <inheritdoc />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1011:Closing square brackets should be spaced correctly", Justification = "Valid syntax.")]
        public object PerformSet(object? toTarget, object? newValue, object?[]? arguments)
        {
            if (toTarget is null)
            {
                throw new ArgumentNullException(nameof(toTarget));
            }

            if (!(toTarget is TableLayoutControlCollection targetCollection))
            {
                throw new ArgumentException($"{nameof(toTarget)} must be of type {nameof(TableLayoutControlCollection)}", nameof(toTarget));
            }

            if (!(newValue is IEnumerable<Control> newValueEnumerable))
            {
                throw new ArgumentException($"newValue must be {nameof(newValue)}", nameof(newValue));
            }

            targetCollection.Container.SuspendLayout();

            targetCollection.Clear();

            targetCollection.AddRange(newValueEnumerable.ToArray());

            targetCollection.Container.ResumeLayout();
            return targetCollection;
        }
    }
}
