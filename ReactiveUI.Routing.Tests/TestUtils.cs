using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using ReactiveUI.Serialization;

namespace ReactiveUI.Routing.Tests
{
    public static class TestEngineMixins
    {
        public static IDisposable AsPrimaryEngine(this IStorageEngine This)
        {
            var origEngine = RxStorage.Engine;
            RxStorage.InitializeWithEngine(This);

            return Disposable.Create(() => RxStorage.Engine = origEngine);
        }
    }
}
