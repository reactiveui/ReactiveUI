using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Linq.Expressions;

namespace ReactiveUI
{
    public interface IInputCommand : IReactiveCommand
    {
        string Shortcut { get; }
        string Description { get; }
    }

    public class InputSection
    {
        public string SectionHeader { get; set; }
        public ReactiveList<IObservable<IInputCommand>> Commands { get; protected set; }

        public InputSection(string sectionHeader, params IObservable<IInputCommand>[] commands)
        {
            SectionHeader = sectionHeader;
            Commands = new ReactiveList<IObservable<IInputCommand>>(commands);
        }
    }

    public interface IInputScope
    {
        IDisposable Register(params InputSection[] sections);
        void InvokeShortcut(string shortcut);
    }

    public class InputScope
    {
        readonly ReactiveList<InputSection> registeredSections = new ReactiveList<InputSection>();

        public static IInputScope Current {
            get { return RxApp.DependencyResolver.GetService<IInputScope>(); }
        }

        internal InputScope() { }

        public IDisposable Register(params InputSection[] sections)
        {
            var currentSize = registeredSections.Count;
            var lengthToRemove = sections.Length;
            registeredSections.AddRange(sections);

            // NB: This is to hold the RefCount open in AsInputCommand until we
            // drop the input section scope
            var disp = sections.Select(x => x.Commands.ToObservable()).Merge().Subscribe();

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
                .Select(x => x.Commands.ToObservable()).Concat()
                .Merge()
                .Where(x => x != null && x.Shortcut == shortcut && x.CanExecute(null))
                .Take(1)
                .Subscribe(x => x.Execute(null));
        }
    }

    public static class InputMixins
    {
        public static IObservable<IInputCommand> AsInputCommand(this IReactiveCommand This, string shortcut, string description = null)
        {
            return Observable.Return(new InputCommand(This, shortcut, description))
                .Publish(null)
                .RefCount();
        }

        public static IObservable<IInputCommand> GetInputCommand<TTarget>(this TTarget This, Expression<Func<TTarget, IReactiveCommand>> property, string shortcut, string description = null)
        {
            return This.WhenAnyValue(property)
                .Select(x => new InputCommand(x, shortcut, description))
                .Publish(null)
                .RefCount();
        }
    }

    class InputCommand : IInputCommand
    {
        IReactiveCommand inner;

        public InputCommand(IReactiveCommand innerCommand, string shortcut, string description)
        {
            inner = innerCommand;
            Shortcut = shortcut;
            Description = description;
        }

        public string Shortcut { get; protected set; }
        public string Description { get; protected set; }

        #region Boring Code
        public IObservable<T> RegisterAsync<T>(Func<object, IObservable<T>> asyncBlock)
        {
            return inner.RegisterAsync(asyncBlock);
        }

        public IObservable<bool> CanExecuteObservable {
            get { return inner.CanExecuteObservable; }
        }

        public IObservable<bool> IsExecuting {
            get { return inner.IsExecuting; }
        }

        public bool AllowsConcurrentExecution {
            get { return inner.AllowsConcurrentExecution; }
        }

        public IObservable<Exception> ThrownExceptions {
            get { return inner.ThrownExceptions; }
        }

        public IDisposable Subscribe(IObserver<object> observer)
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

        public void Dispose()
        {
            inner.Dispose();
        }
        #endregion
    }
}