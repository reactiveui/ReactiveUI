// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CoreGraphics;
using Foundation;
using UIKit;

namespace ReactiveUI
{
    public abstract class ReactiveCollectionReusableView : UICollectionReusableView,
        IReactiveNotifyPropertyChanged<ReactiveCollectionReusableView>, IHandleObservableErrors, IReactiveObject, ICanActivate
    {
        protected ReactiveCollectionReusableView(CGRect frame)
            : base(frame)
        {
            SetupRxObj();
        }

        protected ReactiveCollectionReusableView()
        {
            SetupRxObj();
        }

        protected ReactiveCollectionReusableView(IntPtr handle)
            : base(handle)
        {
            SetupRxObj();
        }

        protected ReactiveCollectionReusableView(NSObjectFlag t)
            : base(t)
        {
            SetupRxObj();
        }

        protected ReactiveCollectionReusableView(NSCoder coder)
            : base(coder)
        {
            SetupRxObj();
        }

        /// <inheritdoc/>
        public event PropertyChangingEventHandler PropertyChanging;

        /// <inheritdoc/>
        void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args)
        {
            var handler = PropertyChanging;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc/>
        void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Represents an Observable that fires *before* a property is about to
        /// be changed.
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveCollectionReusableView>> Changing => this.GetChangingObservable();

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveCollectionReusableView>> Changed => this.GetChangedObservable();

        /// <inheritdoc/>
        public IObservable<Exception> ThrownExceptions => this.GetThrownExceptionsObservable();

        private void SetupRxObj()
        {
        }

        /// <summary>
        /// When this method is called, an object will not fire change
        /// notifications (neither traditional nor Observable notifications)
        /// until the return value is disposed.
        /// </summary>
        /// <returns>An object that, when disposed, reenables change
        /// notifications.</returns>
        public IDisposable SuppressChangeNotifications()
        {
            return IReactiveObjectExtensions.SuppressChangeNotifications(this);
        }

        private Subject<Unit> _activated = new Subject<Unit>();

        /// <inheritdoc/>
        public IObservable<Unit> Activated => _activated.AsObservable();

        private Subject<Unit> _deactivated = new Subject<Unit>();

        /// <inheritdoc/>
        public IObservable<Unit> Deactivated => _deactivated.AsObservable();

        /// <inheritdoc/>
        public override void WillMoveToSuperview(UIView newsuper)
        {
            base.WillMoveToSuperview(newsuper);
            _activated.OnNext(Unit.Default);
        }

        /// <inheritdoc/>
        public override void RemoveFromSuperview()
        {
            base.RemoveFromSuperview();
            _deactivated.OnNext(Unit.Default);
        }
    }

    public abstract class ReactiveCollectionReusableView<TViewModel> : ReactiveCollectionReusableView, IViewFor<TViewModel>
        where TViewModel : class
    {
        protected ReactiveCollectionReusableView()
        {
        }

        protected ReactiveCollectionReusableView(IntPtr handle)
            : base(handle)
        {
        }

        protected ReactiveCollectionReusableView(NSObjectFlag t)
            : base(t)
        {
        }

        protected ReactiveCollectionReusableView(NSCoder coder)
            : base(coder)
        {
        }

        protected ReactiveCollectionReusableView(CGRect frame)
            : base(frame)
        {
        }

        private TViewModel _viewModel;

        /// <inheritdoc/>
        public TViewModel ViewModel
        {
            get => _viewModel;
            set => this.RaiseAndSetIfChanged(ref _viewModel, value);
        }

        /// <inheritdoc/>
        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TViewModel)value;
        }
    }
}
