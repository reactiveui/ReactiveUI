using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Splat;

namespace ReactiveUI.Android
{

    /// <summary>
    /// AndroidLogger
    /// </summary>
    public class AndroidLogger : ILogger
    {
        private readonly string _logTag;

        public LogLevel Level { get; set; }

        public AndroidLogger(string logTag = "RxUI", LogLevel logLevel = LogLevel.Debug)
        {
            Level = logLevel;
            _logTag = logTag;
        }

        public void Write(string message, LogLevel logLevel)
        {
            Log.Debug(_logTag, "#### (" + logLevel + "): " + message);
        }
        
    }
}