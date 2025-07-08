using System;
using System.Threading.Tasks;

namespace Usb.Net.Avalonia.Abstractions.Views
{
    /// <summary>
    /// Service that helps execute actions on the UI thread.
    /// </summary>
    public interface IMainThread
    {
        /// <summary>
        /// Executes the specified action on the UI thread.
        /// </summary>
        /// <param name="callback"> The action to execute. </param>
        void Invoke(Action callback);

        /// <summary>
        /// Executes the specified function on the UI thread and returns a result.
        /// </summary>
        /// <typeparam name="T"> The type of the result returned by the function. </typeparam>
        /// <param name="callback"> The function to execute. </param>
        /// <returns> The result of type <typeparamref name="T" /> returned by the function. </returns>
        T Invoke<T>(Func<T> callback);

        /// <summary>
        /// Executes the specified action asynchronously on the UI thread.
        /// </summary>
        /// <param name="callback"> The action to execute. </param>
        Task InvokeAsync(Action callback);

        /// <summary>
        /// Executes the specified asynchronous function on the UI thread.
        /// </summary>
        /// <param name="callback"> The asynchronous function to execute. </param>
        Task InvokeAsync(Func<Task> callback);
    }
}
