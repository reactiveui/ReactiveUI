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
        /// <summary>
        /// Private constructor to prevent instantiation.
        /// </summary>
        private DesignModeDetector() { }

        static bool? isInDesignMode;

        /// <summary>
        /// Determines is this application is currently running in a designer.
        /// </summary>
        /// <returns>true if in designer; otherwise false</returns>
        public static bool IsInDesignMode()
        {
            if (!isInDesignMode.HasValue)
            {
                // Check Silverlight Design Mode
                var type = Type.GetType("System.ComponentModel.DesignerProperties, System.Windows, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", false);
                if (type != null)
                {
                    MethodInfo mInfo = type.GetMethod("GetIsInDesignMode");
                    Type dependencyObject = Type.GetType("System.Windows.DependencyObject, System.Windows, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", false);
                    if (dependencyObject != null)
                    {
                        isInDesignMode = (bool)mInfo.Invoke(null, new object[] { Activator.CreateInstance(dependencyObject) });
                    }
                } else if((type = Type.GetType("System.ComponentModel.DesignerProperties, PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", false)) != null) {
                    // loaded the assembly, could be .net 
                    MethodInfo mInfo = type.GetMethod("GetIsInDesignMode");
                    Type dependencyObject = Type.GetType("System.Windows.DependencyObject, WindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", false);
                    if (dependencyObject != null)
                    {
                        isInDesignMode = (bool)mInfo.Invoke(null, new object[] { Activator.CreateInstance(dependencyObject) });
                    }
                } else if ((type = Type.GetType("Windows.ApplicationModel.DesignMode, Windows, ContentType=WindowsRuntime", false)) != null) {
                    // check WinRT next
                    isInDesignMode = (bool)type.GetProperty("IsDesignModeEnabled").GetMethod.Invoke(null, null);
                } else {
                    isInDesignMode = false;
                }
            }

            return isInDesignMode.GetValueOrDefault(false);
        }
    }
}