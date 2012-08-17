using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;

namespace ReactiveUI.Xaml
{
    public class SampleData
    {
        public static string ProjectRootDirectory { get; set; }

        public static T OneOf<T>(params T[] items)
        {
            var prng = new Random();
            return items[prng.Next(0, items.Length - 1)];
        }
        
        public static IEnumerable<T> ShuffledList<T>(IEnumerable<T> items)
        {
            var prng = new Random();
            var arr = items.ToArray();
            for(int i = arr.Length; i > 0; i--) {
                var idx = prng.Next(0, i);
                var tmp = arr[i]; arr[i] = arr[idx]; arr[idx] = tmp;
            }

            return arr.ToArray();
        }

        public static IEnumerable<T> RepeatingListOf<T>(IEnumerable<T> items, int count)
        {
            int toYield = count;
            while (toYield > 0) {
                if (!items.Any()) {
                    throw new Exception("List must have more than ");
                }

                foreach(var v in ShuffledList(items)) {
                    yield return v;
                    toYield--;
                    if (toYield == 0) break;
                }
            }
        }

        public static IEnumerable<T> RandomSamples<T>(int count)
        {
            var sampleType = findSampleClassForViewModel(typeof (T)) ?? typeof (T);

            return Enumerable.Range(0, count)
                .Select(_ => Activator.CreateInstance(sampleType))
                .OfType<T>();
        }

        public static IEnumerable<T> Shuffled<T>(params T[] items)
        {
            return ShuffledList(items);
        }

#if !SILVERLIGHT
        public string ImageFrom(string pathOrRelativePath)
        {
            var di = new DirectoryInfo(pathOrRelativePath);
            if (!di.Exists) {
                var rootDir = ProjectRootDirectory ?? findRootDirectory();
                if (rootDir == null) {
                    throw new FileNotFoundException("Couldn't guess project root. Set SampleData.ProjectRootDirectory.");
                }

                di = new DirectoryInfo(Path.Combine(rootDir, pathOrRelativePath));
                if (!di.Exists) {
                    throw new FileNotFoundException("Couldn't find dir: " + di.FullName);
                }
            }

            return RepeatingListOf(di.EnumerateFiles().Select(x => x.FullName), 1).FirstOrDefault();
        }

        public IEnumerable<string> ImagesFrom(string pathOrRelativePath, int count)
        {
            return Enumerable.Range(0, count).Select(_ => ImageFrom(pathOrRelativePath));
        }

        string findRootDirectory()
        {
            var thisAssembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();

            var di = new DirectoryInfo(Path.GetDirectoryName(thisAssembly.FullName));
            while (di != null) {
                if (di.EnumerateFiles().Any(x => x.Name.EndsWith(".sln", StringComparison.OrdinalIgnoreCase))) {
                    return di.FullName;
                }
            }

            return null;
        }
#endif

        internal static Type findSampleClassForViewModel(Type objectType)
        {
            var sampleAttr = objectType.GetCustomAttributes(true)
                .OfType<SampleClassAttribute>()
                .Select(x => x.SampleType)
                .FirstOrDefault();

            return sampleAttr;
        }
    }

    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
    public class SampleClassAttribute : Attribute
    {
        public Type SampleType { get; protected set; }
        public SampleClassAttribute(Type sampleType)
        {
            SampleType = sampleType;
        }
    }

    public class SampleDataProviderBinder : IPropertyBinderImplementation
    {
        static readonly PropertyBinderImplementation inner = new PropertyBinderImplementation();

        public IDisposable Bind<TViewModel, TView, TProp>(TViewModel viewModel, TView view, Expression<Func<TViewModel, TProp>> vmProperty, Expression<Func<TView, TProp>> viewProperty) 
            where TViewModel : class 
            where TView : IViewForViewModel
        {
            return OneWayBind(viewModel, view, vmProperty, viewProperty);
        }

        public IDisposable OneWayBind<TViewModel, TView, TProp>(TViewModel viewModel, TView view, Expression<Func<TViewModel, TProp>> vmProperty, Expression<Func<TView, TProp>> viewProperty, Func<TProp> fallbackValue = null) 
            where TViewModel : class 
            where TView : IViewForViewModel
        {
            var sampleVmType = SampleData.findSampleClassForViewModel(typeof (TViewModel));
            var sampleVm = Activator.CreateInstance(sampleVmType);
                
            var vmPropChain = Reflection.ExpressionToPropertyNames(vmProperty);
            if (viewProperty == null) {
                var viewPropChain = Reflection.getDefaultViewPropChain(view, Reflection.ExpressionToPropertyNames(vmProperty));

                return sampleVm.WhenAnyDynamic(vmPropChain, x => x.Value)
                    .Subscribe(x => Reflection.SetValueToPropertyChain(view, viewPropChain, x, false));
            }
                
            return sampleVm.WhenAnyDynamic(vmPropChain, x => (TProp)x.Value)
                .BindTo(view, viewProperty, fallbackValue);
        }

        public IDisposable OneWayBind<TViewModel, TView, TProp, TOut>(TViewModel viewModel, TView view, Expression<Func<TViewModel, TProp>> vmProperty, Expression<Func<TView, TOut>> viewProperty, Func<TProp, TOut> selector, Func<TOut> fallbackValue = null) 
            where TViewModel : class 
            where TView : IViewForViewModel
        {
            var sampleVmType = SampleData.findSampleClassForViewModel(typeof (TViewModel));
            var sampleVm = Activator.CreateInstance(sampleVmType);
                
            var vmPropChain = Reflection.ExpressionToPropertyNames(vmProperty);
            if (viewProperty == null) {
                var viewPropChain = Reflection.getDefaultViewPropChain(view, Reflection.ExpressionToPropertyNames(vmProperty));

                return sampleVm.WhenAnyDynamic(vmPropChain, x => (TProp)x.Value)
                    .Select(selector)
                    .Subscribe(x => Reflection.SetValueToPropertyChain(view, viewPropChain, x, false));
            }
                
            return sampleVm.WhenAnyDynamic(vmPropChain, x => (TProp)x.Value)
                .Select(selector)
                .BindTo(view, viewProperty, fallbackValue);
        }

        public IDisposable AsyncOneWayBind<TViewModel, TView, TProp, TOut>(TViewModel viewModel, TView view, Expression<Func<TViewModel, TProp>> vmProperty, Expression<Func<TView, TOut>> viewProperty, Func<TProp, IObservable<TOut>> selector, Func<TOut> fallbackValue = null) 
            where TViewModel : class 
            where TView : IViewForViewModel
        {
            var sampleVmType = SampleData.findSampleClassForViewModel(typeof (TViewModel));
            var sampleVm = Activator.CreateInstance(sampleVmType);
                
            var vmPropChain = Reflection.ExpressionToPropertyNames(vmProperty);
            if (viewProperty == null) {
                var viewPropChain = Reflection.getDefaultViewPropChain(view, Reflection.ExpressionToPropertyNames(vmProperty));

                return sampleVm.WhenAnyDynamic(vmPropChain, x => (TProp)x.Value)
                    .SelectMany(selector)
                    .Subscribe(x => Reflection.SetValueToPropertyChain(view, viewPropChain, x, false));
            }
                
            return sampleVm.WhenAnyDynamic(vmPropChain, x => (TProp)x.Value)
                .SelectMany(selector)
                .BindTo(view, viewProperty, fallbackValue);
        }
    }
}