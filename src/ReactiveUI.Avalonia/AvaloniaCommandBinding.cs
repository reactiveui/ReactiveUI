using System;
using System.Windows.Input;

namespace ReactiveUI.Avalonia
{
    public class AvaloniaCommandBinding : ICreatesCommandBinding
    {
        public IDisposable BindCommandToObject(ICommand command, object target, IObservable<object> commandParameter)
        {
            throw new NotImplementedException();
        }

        public IDisposable BindCommandToObject<TEventArgs>(ICommand command, object target, IObservable<object> commandParameter, string eventName)
        {
            throw new NotImplementedException();
        }

        public int GetAffinityForObject(Type type, bool hasEventTarget)
        {
            throw new NotImplementedException();
        }
    }
}
