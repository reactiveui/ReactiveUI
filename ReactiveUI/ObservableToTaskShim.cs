using System;
using System.Net;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace System.Reactive.Threading.Tasks
{
    public static class TaskToObservableMixin
    {
        public static IObservable<T> ToObservable<T>(this Task<T> This)
        {
            var ret = new AsyncSubject<T>();
            This.ContinueWith(x => {
                try {
                    if (x.Exception != null) {
                        ret.OnError(x.Exception);
                        return;
                    }
                    ret.OnNext(x.Result);
                    ret.OnCompleted();
                } catch (Exception ex) {
                    ret.OnError(ex);
                }
            });

            return ret;
        }

        public static IObservable<Unit> ToObservable(this Task This)
        {
            var ret = new AsyncSubject<Unit>();
            This.ContinueWith(x => {
                try {
                    if (x.Exception != null) {
                        ret.OnError(x.Exception);
                        return;
                    }
                    ret.OnNext(Unit.Default);
                    ret.OnCompleted();
                } catch (Exception ex) {
                    ret.OnError(ex);
                }
            });

            return ret;
        }
    }
}