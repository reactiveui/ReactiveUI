// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReactiveUI;

/// <summary>
/// Observables not already built into Rx.
/// </summary>
internal static class ObservableEx
{
    /// <summary>
    /// Adapts a factory of <see cref="Task"/> in a way that enables observation to continue after
    /// cancellation has been requested.
    /// </summary>
    /// <param name="actionAsync">
    /// The factory method that will be invoked to start a task each time an observer subscribes.
    /// </param>
    /// <returns>
    /// An observable source which executes the factory method each time an observer subscribes, and
    /// which provides a single item: a tuple containing an <see cref="IObservable{T}"/> representing
    /// the outcome of the task, and a callback that can be invoked to cancel this particular
    /// invocation of the task.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This is conceptually similar to <see cref="Observable.FromAsync(Func{CancellationToken, Task})"/>,
    /// except it better supports scenarios in which application code wants to observe what the task
    /// does even after cancellation.
    /// </para>
    /// <para>
    /// Rx's <see cref="Observable.FromAsync(Func{CancellationToken, Task})"/> is simpler, but it has
    /// a consequent limitation. It is simpler because it enables cancellation through unsubscription:
    /// if you want to cancel the task that was started when a particular observer subscribed to the
    /// observable it returns, all you need to do is unsubscribe. (To be more precise, if <c>taskSource</c>
    /// is some <see cref="IObservable{T}"/> returned by this method, each time you call
    /// <c>taskSource.Subscribe</c>, it will start a new task and return an <see cref="IDisposable"/>.
    /// As with any Rx subscription, you can unsubscribe by calling <see cref="IDisposable.Dispose"/>
    /// on that object returned by <see cref="IObservable{T}.Subscribe(IObserver{T})"/>. And in this case,
    /// where the observable is a wrapper for a task returned by
    /// <see cref="Observable.FromAsync(Func{CancellationToken, Task})"/>, that will also attempt
    /// to cancel the task, via the <see cref="CancellationToken"/> that was passed to the factory method.)
    /// But there is a fundamental limitation with this: the rules of Rx state that from the moment you
    /// call <c>Dispose</c> (and before <c>Dispose</c> has returned), you cannot rely on getting any
    /// further notifications. The rules permit further notifications (until such time as <c>Dispose</c>
    /// returns) but they do not require them. And in practice, <c>System.Reactive</c> implements all
    /// of its observables in a way that tends to shut down notifications very early on.
    /// </para>
    /// <para>
    /// The upshot of this is that if you cancel a task by unsubscribing from the observer that was
    /// going to tell you what the task did, you can't rely on receiving any notification about the
    /// outcome of the task. This has two upshots. Firstly, if cancellation takes a while to process
    /// there's no way to discover when it has finally completed. Secondly, if an error occurs while
    /// the task is attempting to stop, you'll have no way to observe that.
    /// </para>
    /// <para>
    /// These proplems are fundamentally unavoidable if the mechanism by which you cancel the task is
    /// to unsubscribe from notifications about the outcome of the task. This method therefore takes
    /// a different approach: it separates observation from cancellation. It supplies a delegate you
    /// can invoke to initiate cancellation without having to unsubscribe. This enabes you to discover
    /// when and how the task finally completes.
    /// </para>
    /// <para>
    /// This method might seem more complex than it needs to be. The 'obvious' simpler method might
    /// have this signature:
    /// </para>
    /// <code>
    /// <![CDATA[
    /// (IObservable<Unit> Result, Action Cancel) FromAsyncWithPostCancelNotifications(Func<CancellationToken, Task> actionAsync)
    /// ]]>
    /// </code>
    /// <para>
    /// However, the problem with that is that it's only good for one shot. By design, FromAsync invokes
    /// its action callback every time you subscribe to the source it returns and this method does the same.
    /// So it's no good returning a single cancellation callback. We need one each time the operation is
    /// invoked. And since invocation is triggered by subscribing to the source, the observable
    /// itself is going to need to return the means of cancellation.
    /// </para>
    /// <para>
    /// So as with <see cref="Observable.FromAsync(Func{CancellationToken, Task})"/>, each subscriber to
    /// an observable returned by this method will receive a single notification. That notification
    /// is a tuple. The first value is another <see cref="IObservable{T}"/>, which provides the
    /// outcome when the task completes (just like the observable returned by
    /// <see cref="Observable.FromAsync(Func{CancellationToken, Task})"/>), but this tuple also provides
    /// a callback that you can invoke to cancel the task. If you remain subscribed to the inner observable
    /// it will tell you when the task eventually completes (which might take some time, because cancellation
    /// is never instantaneous, and can sometimes be quite slow) and whether it did so successfully or
    /// by producing an error.
    /// </para>
    /// <para>
    /// As per https://github.com/reactiveui/ReactiveUI/issues/2153#issuecomment-1495544227 this could
    /// become a feature of Rx in the future if the ReactiveUI team wants it. Since it's not in there now, we
    /// need to roll our own.
    /// </para>
    /// </remarks>
    internal static IObservable<(IObservable<Unit> Result, Action Cancel)> FromAsyncWithPostCancelNotifications(
        Func<CancellationToken, Task> actionAsync)
    {
        return Observable.Defer(
            () =>
            {
                var cancelThisInvocationSource = new CancellationTokenSource();
                var result = Observable.FromAsync(
                    async cancelFromRx =>
                    {
                        var combinedCancel = CancellationTokenSource.CreateLinkedTokenSource(
                            cancelThisInvocationSource.Token, cancelFromRx);
                        await actionAsync(combinedCancel.Token);
                    });

                return Observable.Return<(IObservable<Unit> Result, Action Cancel)>(
                    (result, () => cancelThisInvocationSource.Cancel()));
            });
    }
}
