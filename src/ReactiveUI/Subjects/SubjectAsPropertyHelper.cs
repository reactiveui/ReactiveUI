using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using System.Text;

namespace ReactiveUI.Subjects
{
    public class SubjectAsPropertyHelper<T> : IHandleObservableErrors
    {
        private readonly ISubject<T> _Subject;
        private readonly ObservableAsPropertyHelper<T> _Inner;

        public IObservable<Exception> ThrownExceptions => _Inner.ThrownExceptions;

        public SubjectAsPropertyHelper
            ( ISubject<T> subject
            , Action<T>      onChanged
            , Action<T>      onChanging
            , T              initialValue      = default(T)
            , bool           deferSubscription = false
            , IScheduler     scheduler         = null )
        {
            _Subject = subject;
            _Inner = new ObservableAsPropertyHelper<T>(subject
                                                       ,onChanged
                                                       ,onChanging
                                                       ,initialValue
                                                       ,deferSubscription
                                                       ,scheduler
                                                       );

        }

        public T Value { get => _Inner.Value; set => _Subject.OnNext( value ); }
    }


    public static class SubjectAsPropertyHelperMixins
    {
        public static SubjectAsPropertyHelper<TRet> ToReadWriteProperty<TObj, TRet>
            ( this ISubject<TRet>          subject
            , TObj                         This
            , Expression<Func<TObj, TRet>> property
            , TRet                         initialValue      = default(TRet)
            , bool                         deferSubscription = false
            , IScheduler                   scheduler         = null ) where TObj : IReactiveObject
        {
            var expression = ReactiveUI.Reflection.Rewrite(property.Body);
            if (expression.GetParent().NodeType != ExpressionType.Parameter)
                throw new ArgumentException("Property expression must be of the form 'x => x.SomeProperty'");
            var name = expression.GetMemberInfo().Name;
            if (expression is IndexExpression)
                name += "[]";
            return new SubjectAsPropertyHelper<TRet>
                ( subject
                , _ => This.RaisePropertyChanged( name )
                , _ => This.RaisePropertyChanging( name )
                , initialValue
                , deferSubscription
                , scheduler );
        }

    }

}
