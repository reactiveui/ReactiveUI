// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CoreGraphics;
using Foundation;
using UIKit;

namespace ReactiveUI
{
    public abstract class ReactiveCollectionViewCell : UICollectionViewCell, IReactiveNotifyPropertyChanged<ReactiveCollectionViewCell>, IHandleObservableErrors, IReactiveObject, ICanActivate
    {
        protected ReactiveCollectionViewCell(CGRect frame)
            : base(frame)
        {
            SetupRxObj();
        }

        protected ReactiveCollectionViewCell(NSObjectFlag t)
            : base(t)
        {
            SetupRxObj();
        }

        protected ReactiveCollectionViewCell(NSCoder coder)
            : base(NSObjectFlag.Empty)
        {
            SetupRxObj();
        }

        protected ReactiveCollectionViewCell()
        {
            SetupRxObj();
        }

        protected ReactiveCollectionViewCell(IntPtr handle)
            : base(handle)
        {
            SetupRxObj();
        }

        /// <inheritdoc/>
        public event PropertyChangingEventHandler PropertyChanging
        {
            add => PropertyChangingEventManager.AddHandler(this, value);
            remove => PropertyChangingEventManager.RemoveHandler(this, value);
        }

        /// <inheritdoc/>
        void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args)
        {
            PropertyChangingEventManager.DeliverEvent(this, args);
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged
        {
            add => PropertyChangedEventManager.AddHandler(this, value);
            remove => PropertyChangedEventManager.RemoveHandler(this, value);
        }

        /// <inheritdoc/>
        void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChangedEventManager.DeliverEvent(this, args);
        }

        /// <summary>
        /// Represents an Observable that fires *before* a property is about to
        /// be changed.
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveCollectionViewCell>> Changing => this.GetChangingObservable();

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveCollectionViewCell>> Changed => this.GetChangedObservable();

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
            (newsuper != null ? _activated : _deactivated).OnNext(Unit.Default);
        }
    }

    public abstract class ReactiveCollectionViewCell<TViewModel> : ReactiveCollectionViewCell, IViewFor<TViewModel>
        where TViewModel : class
    {
        protected ReactiveCollectionViewCell(NSObjectFlag t)
            : base(t)
        {
        }

        protected ReactiveCollectionViewCell(NSCoder coder)
            : base(NSObjectFlag.Empty)
        {
        }

        protected ReactiveCollectionViewCell()
        {
        }

        protected ReactiveCollectionViewCell(CGRect frame)
            : base(frame)
        {
        }

        protected ReactiveCollectionViewCell(IntPtr handle)
            : base(handle)
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
