
using System;
namespace ReactiveUI.Samples.Testing.SimpleViewModels
{
    public interface IWebCaller
    {
        /// <summary>
        /// Return the web service call string result given the input.
        /// Can take an indeterminate amount of time.
        /// </summary>
        /// <param name="searchItems"></param>
        /// <returns></returns>
        IObservable<string> GetResult(string searchItems);
    }
}
