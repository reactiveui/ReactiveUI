using System;

namespace ReactiveXaml.Serialization
{
    /// <summary>
    ///
    /// </summary>
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
            } else {
                // TODO: Provide a path to the user's AppData folder
                InitializeWithEngine(new DictionaryStorageEngine());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="engine"></param>
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