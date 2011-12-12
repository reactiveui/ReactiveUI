using System;

namespace ReactiveUI.Serialization
{
    public static class RxStorage
    {
        static IExtendedStorageEngine _Engine;
        [ThreadStatic] static IExtendedStorageEngine _UnitTestEngine;

        public static IExtendedStorageEngine Engine {
            get { return _UnitTestEngine ?? _Engine; }
            set {
                if (RxApp.InUnitTestRunner()) {
                    _UnitTestEngine = value;
                    _Engine = _Engine ?? value;
                } else {
                    _Engine = value;
                }
            }
        }

        static RxStorage()
        {
            if (RxApp.InUnitTestRunner()) {
                InitializeWithEngine(new DictionaryStorageEngine());
            }
        }

        /// <summary>
        /// InitializeWithEngine initializes ReactiveUI.Serialization with a
        /// storage engine to load and save objects.
        /// </summary>
        /// <param name="engine">The engine to use.</param>
        public static void InitializeWithEngine(IStorageEngine engine)
        {
            var extEngine = engine as IExtendedStorageEngine;
            if (extEngine != null) {
                Engine = extEngine;
            } else {
                Engine = new NaiveExtendedEngine(engine);
            }
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :