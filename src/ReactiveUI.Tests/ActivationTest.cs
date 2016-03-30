using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Splat;
using Xunit;

namespace ReactiveUI.Tests
{
    public class ActivatingViewModel : ReactiveObject, ISupportsActivation
    {
        public ViewModelActivator Activator { get; protected set; }

        public int IsActiveCount { get; protected set; }

        public ActivatingViewModel()
        {
            Activator = new ViewModelActivator();
                
            this.WhenActivated(d => {
                IsActiveCount++;
                d(Disposable.Create(() => IsActiveCount--));
            });
        }
    }

    public class DerivedActivatingViewModel : ActivatingViewModel
    {
        public int IsActiveCountAlso { get; protected set; }

        public DerivedActivatingViewModel()
        {
            this.WhenActivated(d => {
                IsActiveCountAlso++;
                d(Disposable.Create(() => IsActiveCountAlso--));
            });
        }
    }

    public class ActivatingView : ReactiveObject, IViewFor<ActivatingViewModel>
    {
        ActivatingViewModel viewModel;
        public ActivatingViewModel ViewModel {
            get { return viewModel; }
            set { this.RaiseAndSetIfChanged(ref viewModel, value); }
        }

        object IViewFor.ViewModel {
            get { return ViewModel; }
            set { ViewModel = (ActivatingViewModel)value; }
        }

        public ActivatingView()
        {
            this.WhenActivated(d => {
                IsActiveCount++;
                d(Disposable.Create(() => IsActiveCount--));
            });
        }

        public int IsActiveCount { get; set; }

        public Subject<Unit> Loaded = new Subject<Unit>();
        public Subject<Unit> Unloaded = new Subject<Unit>();
    }

    public class ActivatingViewFetcher : IActivationForViewFetcher
    {
        public int GetAffinityForView(Type view)
        {
            return view == typeof(ActivatingView) ? 100 : 0;
        }

        public IObservable<bool> GetActivationForView(IActivatable view)
        {
            var av = view as ActivatingView;
            return av.Loaded.Select(_ => true).Merge(av.Unloaded.Select(_ => false));
        }
    }

    public class ActivatingViewModelTests
    {
        [Fact]
        public void ActivationsGetRefCounted()
        {
            var fixture = new ActivatingViewModel();
            Assert.Equal(0, fixture.IsActiveCount);

            fixture.Activator.Activate();
            Assert.Equal(1, fixture.IsActiveCount);

            fixture.Activator.Activate();
            Assert.Equal(1, fixture.IsActiveCount);

            fixture.Activator.Deactivate();
            Assert.Equal(1, fixture.IsActiveCount);

            // Refcount drops to zero
            fixture.Activator.Deactivate();
            Assert.Equal(0, fixture.IsActiveCount);
        }

        [Fact]
        public void DerivedActivationsDontGetStomped()
        {
            var fixture = new DerivedActivatingViewModel();
            Assert.Equal(0, fixture.IsActiveCount);
            Assert.Equal(0, fixture.IsActiveCountAlso);

            fixture.Activator.Activate();
            Assert.Equal(1, fixture.IsActiveCount);
            Assert.Equal(1, fixture.IsActiveCountAlso);

            fixture.Activator.Activate();
            Assert.Equal(1, fixture.IsActiveCount);
            Assert.Equal(1, fixture.IsActiveCountAlso);

            fixture.Activator.Deactivate();
            Assert.Equal(1, fixture.IsActiveCount);
            Assert.Equal(1, fixture.IsActiveCountAlso);

            fixture.Activator.Deactivate();
            Assert.Equal(0, fixture.IsActiveCount);
            Assert.Equal(0, fixture.IsActiveCountAlso);
        }
    }

    public class ActivatingViewTests
    {
        [Fact]
        public void ActivatingViewSmokeTest()
        {
            var locator = new ModernDependencyResolver();
            locator.InitializeSplat();
            locator.InitializeReactiveUI();
            locator.Register(() => new ActivatingViewFetcher(), typeof(IActivationForViewFetcher));

            using (locator.WithResolver()) {
                var vm = new ActivatingViewModel();
                var fixture = new ActivatingView();

                fixture.ViewModel = vm;
                Assert.Equal(0, vm.IsActiveCount);
                Assert.Equal(0, fixture.IsActiveCount);

                fixture.Loaded.OnNext(Unit.Default);
                Assert.Equal(1, vm.IsActiveCount);
                Assert.Equal(1, fixture.IsActiveCount);

                fixture.Unloaded.OnNext(Unit.Default);
                Assert.Equal(0, vm.IsActiveCount);
                Assert.Equal(0, fixture.IsActiveCount);
            }
        }

        [Fact]
        public void NullingViewModelShouldDeactivateIt()
        {
            var locator = new ModernDependencyResolver();
            locator.InitializeSplat();
            locator.InitializeReactiveUI();
            locator.Register(() => new ActivatingViewFetcher(), typeof(IActivationForViewFetcher));

            using (locator.WithResolver()) {
                var vm = new ActivatingViewModel();
                var fixture = new ActivatingView();

                fixture.ViewModel = vm;
                Assert.Equal(0, vm.IsActiveCount);
                Assert.Equal(0, fixture.IsActiveCount);

                fixture.Loaded.OnNext(Unit.Default);
                Assert.Equal(1, vm.IsActiveCount);
                Assert.Equal(1, fixture.IsActiveCount);

                fixture.ViewModel = null;
                Assert.Equal(0, vm.IsActiveCount);
            }
        }

        [Fact]
        public void SwitchingViewModelShouldDeactivateIt()
        {
            var locator = new ModernDependencyResolver();
            locator.InitializeSplat();
            locator.InitializeReactiveUI();
            locator.Register(() => new ActivatingViewFetcher(), typeof(IActivationForViewFetcher));

            using (locator.WithResolver()) {
                var vm = new ActivatingViewModel();
                var fixture = new ActivatingView();

                fixture.ViewModel = vm;
                Assert.Equal(0, vm.IsActiveCount);
                Assert.Equal(0, fixture.IsActiveCount);

                fixture.Loaded.OnNext(Unit.Default);
                Assert.Equal(1, vm.IsActiveCount);
                Assert.Equal(1, fixture.IsActiveCount);

                var newVm = new ActivatingViewModel();
                Assert.Equal(0, newVm.IsActiveCount);

                fixture.ViewModel = newVm;
                Assert.Equal(0, vm.IsActiveCount);
                Assert.Equal(1, newVm.IsActiveCount);
            }
        }

        [Fact]
        public void SettingViewModelAfterLoadedShouldLoadIt()
        {
            var locator = new ModernDependencyResolver();
            locator.InitializeSplat();
            locator.InitializeReactiveUI();
            locator.Register(() => new ActivatingViewFetcher(), typeof(IActivationForViewFetcher));

            using (locator.WithResolver()) {
                var vm = new ActivatingViewModel();
                var fixture = new ActivatingView();

                Assert.Equal(0, vm.IsActiveCount);
                Assert.Equal(0, fixture.IsActiveCount);

                fixture.Loaded.OnNext(Unit.Default);
                Assert.Equal(1, fixture.IsActiveCount);

                fixture.ViewModel = vm;
                Assert.Equal(1, fixture.IsActiveCount);
                Assert.Equal(1, vm.IsActiveCount);

                fixture.Unloaded.OnNext(Unit.Default);
                Assert.Equal(0, fixture.IsActiveCount);
                Assert.Equal(0, vm.IsActiveCount);
            }
        }

        [Fact]
        public void CanUnloadAndLoadViewAgain()
        {
            var locator = new ModernDependencyResolver();
            locator.InitializeSplat();
            locator.InitializeReactiveUI();
            locator.Register(() => new ActivatingViewFetcher(), typeof(IActivationForViewFetcher));

            using (locator.WithResolver())
            {
                var vm = new ActivatingViewModel();
                var fixture = new ActivatingView();

                fixture.ViewModel = vm;
                Assert.Equal(0, vm.IsActiveCount);
                Assert.Equal(0, fixture.IsActiveCount);

                fixture.Loaded.OnNext(Unit.Default);
                Assert.Equal(1, vm.IsActiveCount);
                Assert.Equal(1, fixture.IsActiveCount);

                fixture.Unloaded.OnNext(Unit.Default);
                Assert.Equal(0, vm.IsActiveCount);
                Assert.Equal(0, fixture.IsActiveCount);

                fixture.Loaded.OnNext(Unit.Default);
                Assert.Equal(1, vm.IsActiveCount);
                Assert.Equal(1, fixture.IsActiveCount);
            }
        }

    }
}
