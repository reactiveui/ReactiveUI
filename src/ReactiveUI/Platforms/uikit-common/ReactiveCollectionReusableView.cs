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
    /// <summary>
    /// This is a UICollectionReusableView that is both an UICollectionReusableView and has ReactiveObject powers
    /// (i.e. you can call RaiseAndSetIfChanged).
    /// </summary>
    public abstract class ReactiveCollectionReusableView : UICollectionReusableView,
        IReactiveNotifyPropertyChanged<ReactiveCollectionReusableView>, IHandleObservableErrors, IReactiveObject, ICanActivate
    {
        private Subject<Unit> _activated = new Subject<Unit>();
        private Subject<Unit> _deactivated = new Subject<Unit>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveCollectionReusableView"/> class.
        /// </summary>
        /// <param name="frame">The frame.</param>
        protected ReactiveCollectionReusableView(CGRect frame)
            : base(frame)
        {
            SetupRxObj();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveCollectionReusableView"/> class.
        /// </summary>
        protected ReactiveCollectionReusableView()
        {
            SetupRxObj();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveCollectionReusableView"/> class.
        /// </summary>
        /// <param name="handle">The pointer.</param>
        protected ReactiveCollectionReusableView(IntPtr handle)
            : base(handle)
        {
            SetupRxObj();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveCollectionReusableView"/> class.
        /// </summary>
        /// <param name="t">The object flag.</param>
        protected ReactiveCollectionReusableView(NSObjectFlag t)
            : base(t)
        {
            SetupRxObj();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveCollectionReusableView"/> class.
        /// </summary>
        /// <param name="coder">The coder.</param>
        protected ReactiveCollectionReusableView(NSCoder coder)
            : base(coder)
        {
            SetupRxObj();
        }

        /// <inheritdoc/>
        public event PropertyChangingEventHandler PropertyChanging;

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

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

        /// <inheritdoc/>
        public IObservable<Unit> Activated => _activated.AsObservable();

        /// <inheritdoc/>
        public IObservable<Unit> Deactivated => _deactivated.AsObservable();

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
        void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, args);
            }
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

        private void SetupRxObj()
        {
        }
    }

    /// <summary>
    /// This is a UICollectionReusableView that is both an UICollectionReusableView and has ReactiveObject powers
    /// (i.e. you can call RaiseAndSetIfChanged).
    /// </summary>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    public abstract class ReactiveCollectionReusableView<TViewModel> : ReactiveCollectionReusableView, IViewFor<TViewModel>
        where TViewModel : class
    {
        private TViewModel _viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveCollectionReusableView{TViewModel}"/> class.
        /// </summary>
        protected ReactiveCollectionReusableView()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveCollectionReusableView{TViewModel}"/> class.
        /// </summary>
        /// <param name="handle">The pointer.</param>
        protected ReactiveCollectionReusableView(IntPtr handle)
            : base(handle)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveCollectionReusableView{TViewModel}"/> class.
        /// </summary>
        /// <param name="t">The object flag.</param>
        protected ReactiveCollectionReusableView(NSObjectFlag t)
            : base(t)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveCollectionReusableView{TViewModel}"/> class.
        /// </summary>
        /// <param name="coder">The coder.</param>
        protected ReactiveCollectionReusableView(NSCoder coder)
            : base(coder)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveCollectionReusableView{TViewModel}"/> class.
        /// </summary>
        /// <param name="frame">The frame.</param>
        protected ReactiveCollectionReusableView(CGRect frame)
            : base(frame)
        {
        }

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
