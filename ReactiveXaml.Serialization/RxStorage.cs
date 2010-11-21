using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReactiveXaml.Serialization
{
    public static class RxStorage
    {
        public static IStorageEngine Engine { get; set; }

        static RxStorage()
        {
            if (RxApp.InUnitTestRunner()) {
                Engine = new DictionaryStorageEngine();
            } else {
                // TODO: Provide a path to the user's AppData folder
                Engine = new DictionaryStorageEngine();
            }
        }
    }
}

// vim: tw=120 ts=4 sw=4 et enc=utf8 :