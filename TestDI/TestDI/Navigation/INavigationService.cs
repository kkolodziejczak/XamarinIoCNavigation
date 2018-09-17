using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestDI.Navigation
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
        T NavigationParameters<T>(string parameterKey);

        /// <summary>
        /// Removes all pages from Navigation Stack.
        /// </summary>
        Task PopPageToRootAsync();

        /// <summary>
        /// Removes all pages from Navigation Stack.
        /// </summary>
        /// <param name="animated">Animate the passage.</param>
        Task PopPageToRootAsync(bool animated);

        /// <summary>
        /// Removes current page from Navigation Stack.
        /// </summary>
        Task PopPageAsync();

        /// <summary>
        /// Removes current page from Navigation Stack.
        /// </summary>
        /// <param name="animated">Animate the passage.</param>
        Task PopPageAsync(bool animated);

        /// <summary>
        /// Removes <paramref name="count"/> pages from Navigation Stack.
        /// </summary>s
        /// <param name="count">Number of pages to pop.</param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        Task PopPageAsync(byte count);

        /// <summary>
        /// Removes <paramref name="count"/> pages from Navigation Stack.
        /// </summary>s
        /// <param name="count">Number of pages to pop.</param>
        /// <param name="animated">Animate the passage.</param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        Task PopPageAsync(byte count, bool animated);

        /// <summary>
        /// Navigate to <paramref name="pageName"/> page.
        /// </summary>
        /// <param name="pageName">Page name to navigate to.</param>
        /// <param name="navigationParameters">Parameters to pass with this navigation.</param>
        Task GoToAsync(string pageName, params (string key, object value)[] navigationParameters);

        /// <summary>
        /// Navigate to <paramref name="pageName"/> page.
        /// </summary>
        /// <param name="pageName">Page name to navigate to.</param>
        /// <param name="animated">Animate the passage.</param>
        /// <param name="navigationParameters">Parameters to pass with this navigation.</param>
        Task GoToAsync(string pageName, bool animated, params (string key, object value)[] navigationParameters);

        /// <summary>
        /// Pop current page and go to <paramref name="pageName"/> page.
        /// </summary>
        /// <param name="pageName">Page name to navigate to.</param>
        /// <param name="navigationParameters">Parameters to pass with this navigation.</param>
        Task PopPageAndGoToAsync(string pageName, params (string key, object value)[] navigationParameters);

        /// <summary>
        /// Pop current page and go to <paramref name="pageName"/> page.
        /// </summary>
        /// <param name="pageName">Page name to navigate to.</param>
        /// <param name="animated">Animate the passage.</param>
        /// <param name="navigationParameters">Parameters to pass with this navigation.</param>
        Task PopPageAndGoToAsync(string pageName, bool animated, params (string key, object value)[] navigationParameters);

        /// <summary>
        /// Pop <paramref name="amount"/> of pages and go to <paramref name="pageName"/> page.
        /// </summary>
        /// <param name="amount">The amount of pages to pop.</param>
        /// <param name="pageName">Page name to navigate to.</param>
        /// <param name="navigationParameters">Parameters to pass with this navigation.</param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        Task PopPageAndGoToAsync(byte amount, string pageName, params (string key, object value)[] navigationParameters);

        /// <summary>
        /// Pop <paramref name="amount"/> of pages and go to <paramref name="pageName"/> page.
        /// </summary>
        /// <param name="amount">The amount of pages to pop.</param>
        /// <param name="pageName">Page name to navigate to.</param>
        /// <param name="animated">Animate the passage.</param>
        /// <param name="navigationParameters">Parameters to pass with this navigation.</param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        Task PopPageAndGoToAsync(byte amount, string pageName, bool animated, params (string key, object value)[] navigationParameters);

    }
}
