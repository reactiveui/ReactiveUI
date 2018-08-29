// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Xaml.Interactivity;
using Windows.ApplicationModel;
using Windows.UI.Xaml;

namespace ReactiveUI.Blend
{
    public class Behavior<T> : DependencyObject, IBehavior
        where T : DependencyObject
    {
        /// <inheritdoc/>
        public virtual void Attach(DependencyObject associatedObject)
        {
            if (associatedObject == AssociatedObject || DesignMode.DesignModeEnabled)
            {
                return;
            }

            if (AssociatedObject != null)
            {
                throw new InvalidOperationException("Cannot attach multiple objects.");
            }

            AssociatedObject = associatedObject as T;
            OnAttached();
        }

        /// <inheritdoc/>
        public virtual void Detach()
        {
            OnDetaching();
        }

        protected virtual void OnAttached()
        {
        }

        protected virtual void OnDetaching()
        {
        }

        public T AssociatedObject { get; private set; }

        /// <inheritdoc/>
        DependencyObject IBehavior.AssociatedObject
        {
            get { return AssociatedObject; }
        }
    }
}
