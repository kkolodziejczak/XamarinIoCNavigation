using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Xamarin.BetterNavigation.Core
{
    /// <summary>
    /// <see cref="INavigationService"/> provides navigation through your application.
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// Cancellation token that will be canceled when <see cref="INavigationService"/> will push or pop any page.
        /// </summary>
        CancellationToken CancellationToken { get; }

        /// <summary>
        /// Peeks page name from navigation stack
        /// </summary>
        /// <returns>Top's page name used for navigation.</returns>
        string PeekPageName();

        /// <summary>
        /// Navigation parameters passed while navigating to this page.
        /// </summary>
        /// <typeparam name="T">Type of the parameter to get.</typeparam>
        /// <param name="parameterKey">Key passed while navigating to this page.</param>
        /// <exception cref="KeyNotFoundException">Thrown when key is not present in <see cref="NavigationParameters{T}(string)"/></exception>
        /// <exception cref="InvalidCastException">Thrown when object type is not the same as requested one.</exception>
        T NavigationParameters<T>(string parameterKey);

        /// <summary>
        /// Determines whether the <see cref="NavigationParameters{T}"/> contains the specified key.
        /// </summary>
        /// <param name="parameterKey">The key to locate in the <see cref="NavigationParameters{T}"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="parameterKey"/> is null.</exception>
        /// <returns>true if the <see cref="NavigationParameters{T}"/> contains an element with the specified key; otherwise, false.</returns>
        bool ContainsParameterKey(string parameterKey);

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <typeparam name="T">Type of the parameter to get.</typeparam>
        /// <param name="parameterKey">Key passed while navigating to this page.</param>
        /// <param name="value">
        /// When this method returns, contains the value associated with the specified key, if the key is found;
        /// otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.
        /// </param>
        /// <returns></returns>
        /// <exception cref="InvalidCastException">Thrown when object type is not the same as requested one.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="parameterKey"/> is null.</exception>
        bool TryGetValue<T>(string parameterKey, out T value);

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
        /// Removes <paramref name="amount"/> of pages from Navigation Stack.
        /// </summary>s
        /// <param name="amount">Number of pages to pop.</param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        Task PopPageAsync(byte amount);

        /// <summary>
        /// Removes <paramref name="amount"/> of pages from Navigation Stack.
        /// </summary>s
        /// <param name="amount">Number of pages to pop.</param>
        /// <param name="animated">Animate the passage.</param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        Task PopPageAsync(byte amount, bool animated);

        /// <summary>
        /// Removes all pages from Navigation Stack and navigates to <paramref name="pageName"/> page.
        /// </summary>
        /// <param name="pageName">Page name to navigate to.</param>
        /// <param name="navigationParameters">Parameters to pass with this navigation.</param>
        Task PopAllPagesAndGoToAsync(string pageName, params (string key, object value)[] navigationParameters);

        /// <summary>
        /// Removes all pages from Navigation Stack and navigates to <paramref name="pageName"/> page.
        /// </summary>
        /// <param name="pageName">Page name to navigate to.</param>
        /// <param name="animated">Animate the passage.</param>
        /// <param name="navigationParameters">Parameters to pass with this navigation.</param>
        Task PopAllPagesAndGoToAsync(string pageName, bool animated, params (string key, object value)[] navigationParameters);

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
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="amount"/> is too big </exception>
        Task PopPageAndGoToAsync(byte amount, string pageName, params (string key, object value)[] navigationParameters);

        /// <summary>
        /// Pop <paramref name="amount"/> of pages and go to <paramref name="pageName"/> page.
        /// </summary>
        /// <param name="amount">The amount of pages to pop.</param>
        /// <param name="pageName">Page name to navigate to.</param>
        /// <param name="animated">Animate the passage.</param>
        /// <param name="navigationParameters">Parameters to pass with this navigation.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="amount"/> is too big </exception>
        Task PopPageAndGoToAsync(byte amount, string pageName, bool animated, params (string key, object value)[] navigationParameters);

    }
}
