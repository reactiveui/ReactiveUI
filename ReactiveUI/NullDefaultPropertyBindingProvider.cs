using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveUI
{
    public class NullDefaultPropertyBindingProvider : IDefaultPropertyBindingProvider
    {
        public Tuple<string, int> GetPropertyForControl(object control)
        {
            return null;
        }
    }
}
