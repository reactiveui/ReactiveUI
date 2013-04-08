using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReactiveUI
{
    public class DefaultPropertyBinding
    {
        public static string GetPropertyForControl(object control)
        {
            return RxApp.DependencyResolver.GetServices<IDefaultPropertyBindingProvider>()
                .Select(x => x.GetPropertyForControl(control))
                .Where(x => x != null)
                .OrderByDescending(x => x.Item2)
                .Select(x => x.Item1)
                .FirstOrDefault();
        }
    }
}
