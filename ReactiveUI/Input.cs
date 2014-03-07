using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Linq.Expressions;
using Splat;

namespace ReactiveUI
{
    public interface IInputCommand : IReactiveCommand
    {
        string Shortcut { get; }
        string Description { get; }
    }

    public interface IInputCommand<T> : IReactiveCommand<T>, IInputCommand { }

    public sealed class InputSection
    {
        public string SectionHeader { get; set; }
        public bool IgnoreWhileTyping { get; set; }

        internal ReactiveList<IObservable<IInputCommand>> CommandObservables { get; private set; }

        public InputSection(string sectionHeader, params IObservable<IInputCommand>[] commands) : this(sectionHeader, true, commands) { }
        public InputSection(string sectionHeader, bool ignoreWhileTyping, params IObservable<IInputCommand>[] commands)
        {
            SectionHeader = sectionHeader;
            IgnoreWhileTyping = ignoreWhileTyping;

            CommandObservables = new ReactiveList<IObservable<IInputCommand>>(commands);
        }

        public IEnumerable<IInputCommand> Commands {
            get {
                // NB: Same deal as below, we know that all of the CommandObservables
                // return an initial value
                return CommandObservables
                    .Select(x => x.Take(1))
                    .Concat()
                    .Aggregate(new List<IInputCommand>(), (acc, x) => { acc.Add(x); return acc; })
                    .First();
            }
        }
    }

    public interface IKeyboardManager
    {
        IEnumerable<InputSection> RegisteredSections { get; }

        IDisposable RegisterScope(params InputSection[] sections);
        void InvokeShortcut(string shortcut);
    }

    public sealed class KeyboardManager : IKeyboardManager
    {
        readonly ReactiveList<InputSection> registeredSections = new ReactiveList<InputSection>();

        public static IKeyboardManager Current {
            get { return Locator.Current.GetService<IKeyboardManager>(); }
        }

        internal KeyboardManager() { }

        public IEnumerable<InputSection> RegisteredSections {
            get { return registeredSections;  }
        }

        public IDisposable RegisterScope(params InputSection[] sections)
        {
            var currentSize = registeredSections.Count;
            var lengthToRemove = sections.Length;
            registeredSections.AddRange(sections);

            // NB: This is to hold the RefCount open in AsInputCommand until we
            // drop the input section scope
            var disp = sections
                .SelectMany(x => x.CommandObservables).Merge()
                .Subscribe();

            return Disposable.Create(() => {
                registeredSections.RemoveRange(currentSize, lengthToRemove);
                disp.Dispose();
            });
        }

        public void InvokeShortcut(string shortcut)
        {
            // NB: This looks asynchronous, but it will always run synchronously
            // because we only add BehaviorSubjects to the Commands list
            registeredSections.Reverse().ToObservable()
                .Select(x => x.CommandObservables.ToObservable()).Concat()
                .Merge()
                .Where(x => x != null && x.Shortcut == shortcut && x.CanExecute(null))
                .Take(1)
                .Subscribe(x => x.Execute(null));
        }
    }

    public static class InputMixins
    {
        public static IInputCommand<T> AsInputCommand<T>(this IReactiveCommand<T> This, string shortcut, string description = null)
        {
            return InputCommand.CreateWith(This, shortcut, description);
        }

        public static IObservable<IInputCommand<TCmd>> GetInputCommand<TTarget, TCmd>(this TTarget This, Expression<Func<TTarget, IReactiveCommand<TCmd>>> property, string shortcut, string description = null)
        {
            return This.WhenAnyValue(property)
                .Select(x => x.AsInputCommand(shortcut, description))
                .Publish(null)
                .RefCount();
        }
    }

    public static class InputCommand
    {
        public static IInputCommand<T> CreateWith<T>(IReactiveCommand<T> innerCommand, string shortcut, string description)
        {
            return new InputCommand<T>(innerCommand, shortcut, description);
        }
    }

    class InputCommand<T> : IInputCommand<T>
    {
        IReactiveCommand<T> inner;

        public InputCommand(IReactiveCommand<T> innerCommand, string shortcut, string description)
        {
            inner = innerCommand;
            Shortcut = shortcut;
            Description = description;
        }

        public string Shortcut { get; protected set; }
        public string Description { get; protected set; }

        #region Boring Code
        public IObservable<bool> CanExecuteObservable {
            get { return inner.CanExecuteObservable; }
        }

        public IObservable<bool> IsExecuting {
            get { return inner.IsExecuting; }
        }

        public IObservable<Exception> ThrownExceptions {
            get { return inner.ThrownExceptions; }
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return inner.Subscribe(observer);
        }

        public bool CanExecute(object parameter)
        {
            return inner.CanExecute(parameter);
        }

        public event EventHandler CanExecuteChanged {
            add { inner.CanExecuteChanged += value; }
            remove { inner.CanExecuteChanged -= value; }
        }

        public void Execute(object parameter)
        {
            inner.Execute(parameter);
        }

        public IObservable<T> ExecuteAsync(object parameter = null)
        {
            return inner.ExecuteAsync(parameter);
        }

        public void Dispose()
        {
            inner.Dispose();
        }
        #endregion
    }
}