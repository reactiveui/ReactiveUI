using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveUI
{
    public class DesignModeDetector
    {
        public static bool IsInDesignMode()
        {
            // Check Silverlight Design Mode
            var dm = Type.GetType("System.ComponentModel.DesignerProperties, System.Windows, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", false);
            if (dm != null)
            {
                MethodInfo mInfo = dm.GetMethod("GetIsInDesignMode");
                Type dependencyObject = Type.GetType("System.Windows.DependencyObject, System.Windows, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", false);
                if (dependencyObject != null)
                {
                    return (bool)mInfo.Invoke(null, new object[] { Activator.CreateInstance(dependencyObject) });
                }
                return false;
            }

            // Check .NET 
            var cmdm = Type.GetType("System.ComponentModel.DesignerProperties, PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", false);
            if (cmdm != null) // loaded the assembly, could be .net 
            {
                MethodInfo mInfo = cmdm.GetMethod("GetIsInDesignMode");
                Type dependencyObject = Type.GetType("System.Windows.DependencyObject, WindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", false);
                if(dependencyObject != null)
                {
                    return (bool)mInfo.Invoke(null, new object[] { Activator.CreateInstance(dependencyObject) });
                }
                return false;
            }

            // check WinRT next
            var wadm = Type.GetType("Windows.ApplicationModel.DesignMode, Windows, ContentType=WindowsRuntime", false);
            if (wadm != null)
            {
                return (bool)cmdm.GetProperty("IsDesignModeEnabled").GetMethod.Invoke(null, null);
            }

            return false;
        }
    }
}
