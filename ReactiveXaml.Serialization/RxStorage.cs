using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReactiveXaml.Serialization
{
    public static class RxStorage
    {
        public static IExtendedStorageEngine Engine { get; private set; }

        static RxStorage()
        {
            if (RxApp.InUnitTestRunner()) {
                InitializeWithEngine(new DictionaryStorageEngine());
            } else {
                // TODO: Provide a path to the user's AppData folder
                InitializeWithEngine(new DictionaryStorageEngine());
            }
        }

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

// vim: tw=120 ts=4 sw=4 et enc=utf8 :