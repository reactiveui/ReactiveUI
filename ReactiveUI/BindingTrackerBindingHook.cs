using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Text;

namespace ReactiveUI
{
    public class BindingInfo
    {
        readonly ReadOnlyCollection<string> sourcePath;
        readonly ReadOnlyCollection<string> targetPath;
        readonly object source;
        readonly object target;

        public BindingInfo(object source, object target, IEnumerable<string> sourcePath, IEnumerable<string> targetPath)
        {
            this.sourcePath = sourcePath.ToList().AsReadOnly();
            this.targetPath = targetPath.ToList().AsReadOnly();

            this.source = source;
            this.target = target;
        }

        public IEnumerable<string> SourcePath
        {
            get { return sourcePath; }
        }

        public IEnumerable<string> TargetPath
        {
            get { return targetPath; }
        }

        public object Source
        {
            get { return source; }
        }

        public object Target
        {
            get { return target; }
        }
    }

    /// <summary>
    /// This class implements a method of tracking all the bindings that are set on a given object.
    /// </summary>
    internal class BindingTrackerBindingHook : IPropertyBindingHook, IBindingRegistry, IEnableLogger
    {
        readonly Dictionary<object, ReplaySubject<BindingInfo>> allBindings = new Dictionary<object, ReplaySubject<BindingInfo>>(); 

        public bool ExecuteHook(object source, object target, 
            Func<IObservedChange<object, object>[]> getCurrentViewModelProperties, 
            Func<IObservedChange<object, object>[]> getCurrentViewProperties,
            BindingDirection direction)
        {
            if (!Monitor) {
                // this is not active, don't do anything.
                return true;
            }

            var sourcePath = getCurrentViewModelProperties().Select(x => x.PropertyName);
            var targetPath = getCurrentViewProperties().Select(x => x.PropertyName);

            var bindingInfo = new BindingInfo(source, target, sourcePath, targetPath);

            lock (allBindings) {
                ReplaySubject<BindingInfo> bindings;

                if (!allBindings.TryGetValue(target, out bindings)) {
                    bindings = new ReplaySubject<BindingInfo>();
                }

                bindings.OnNext(bindingInfo);

                allBindings[source] = bindings;
            }

            return true;
        }

        public IObservable<BindingInfo> GetBindingForView(object view)
        {
            if (!Monitor) {
                this.Log().Warn("You are tryong to get bindings for a view object, " +
                    "but since monitoring is not enabled, they might be out of date!");
            }

            lock (allBindings) {
                ReplaySubject<BindingInfo> bindings;

                if (!allBindings.TryGetValue(view, out bindings)) {
                    bindings = new ReplaySubject<BindingInfo>();

                    allBindings.Add(view, bindings);
                }

                return bindings;
            }
        }

        public bool Monitor { get; set; }
    }

    public interface IBindingRegistry
    {
        /// <summary>
        /// Gets a collection of all the bindings applied to a given <see cref="view"/>.
        /// </summary>
        /// <param name="view">The target object for which to get all the bindings for.</param>
        /// <returns>An enumerable containing all the bindings applied to the <paramref name="view"/>.</returns>
        IObservable<BindingInfo> GetBindingForView(object view);

        /// <summary>
        /// Sets a value indicating whether this instance of <see cref="IBindingRegistry"/>
        /// should monitor the bindings.
        /// </summary>
        bool Monitor { get; set; }
    }
}
