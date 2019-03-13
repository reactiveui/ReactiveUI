// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            if (fromType.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>) && x.GetGenericArguments().First().IsSubclassOf(typeof(Control))))
            {
                return 15;
            }

            return 0;
        }

        /// <inheritdoc />
        public object PerformSet(object toTarget, object newValue, object[] arguments)
        {
            IEnumerable<Control> newValueEnumerable = (IEnumerable<Control>)newValue;
            TableLayoutControlCollection targetCollection = (TableLayoutControlCollection)toTarget;

            targetCollection.Container.SuspendLayout();

            targetCollection.Clear();

            targetCollection.AddRange(newValueEnumerable.ToArray());

            targetCollection.Container.ResumeLayout();
            return targetCollection;
        }
    }
}
