// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    public static class BuilderExtensions
    {
        public static TBuilder With<TBuilder, TField>(this TBuilder @this, ref TField field, TField value)
            where TBuilder : IBuilder
        {
            field = value;
            return @this;
        }

        public static TBuilder With<TBuilder, TField>(this TBuilder @this, ref List<TField> field, IEnumerable<TField> values)
            where TBuilder : IBuilder
        {
            if (values == null)
            {
                field = null;
            }
            else
            {
                field.AddRange(values);
            }

            return @this;
        }

        public static TBuilder With<TBuilder, TField>(this TBuilder @this, ref List<TField> field, TField value)
            where TBuilder : IBuilder
        {
            field.Add(value);
            return @this;
        }
    }
}
