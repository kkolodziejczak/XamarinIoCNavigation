using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestDI.Interfaces
{
    public interface INavigationService
    {
        /// <summary>
        /// Navigation parameters passed while navigating to this page.
        /// </summary>
        /// <typeparam name="T">Type of the parameter to get.</typeparam>
        /// <param name="parameterKey">Key passed while navigating to this page.</param>
        /// <exception cref="KeyNotFoundException">Thrown when key is not present in <see cref="NavigationParameters{T}(string)"/></exception>
        /// <exception cref="InvalidCastException">Thrown when object type is not the same as requested one.</exception>
        /// <returns></returns>
        T NavigationParameters<T>(string parameterKey);

        /// <summary>
        /// Removes all pages from Navigation Stack.
        /// </summary>
        /// <returns></returns>
        Task PopPageToRootAsync();

        /// <summary>
        /// Removes current page from Navigation Stack.
        /// </summary>
        /// <returns></returns>
        Task PopPageAsync();

        /// <summary>
        /// Removes <paramref name="count"/> pages from Navigation Stack.
        /// </summary>s
        /// <param name="count">Number of pages to pop.</param>
        /// <returns></returns>
        Task PopPageAsync(byte count);

        /// <summary>
        /// Navigate to page.
        /// </summary>
        /// <param name="destinationPageName">Page name to navigate to.</param>
        /// <param name="navigationParameters">Parameters to pass with this navigation.</param>
        /// <returns></returns>
        Task GoToAsync(string destinationPageName, params (string key, object value)[] navigationParameters);

        Task PopPageAndGoToAsync(string destinationPageName, params (string key, object value)[] navigationParameters);

        Task PopPageAndGoToAsync(byte numberOfPagesToPop, string destinationPageName, params (string key, object value)[] navigationParameters);

    }
}
