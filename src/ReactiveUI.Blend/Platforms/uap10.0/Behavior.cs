// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using Windows.UI.Xaml;
using Microsoft.Xaml.Interactivity;
using Windows.ApplicationModel;
using System;

namespace ReactiveUI.Blend
{
    public class Behavior<T> : DependencyObject, IBehavior where T: DependencyObject
    {
        public virtual void Attach(DependencyObject associatedObject)
        {
            if (associatedObject == this.AssociatedObject || DesignMode.DesignModeEnabled) {
                return;
            }

            if (this.AssociatedObject != null) {
                throw new InvalidOperationException("Cannot attach multiple objects.");
            }

            AssociatedObject = associatedObject as T;
            OnAttached();
        }

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

        DependencyObject IBehavior.AssociatedObject {
            get { return this.AssociatedObject; }
        }
    }
}
