﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Celerik.NetCore.Util
{
    /// <summary>
    /// Provides test utilities.
    /// </summary>
    public static class TestUtility
    {
        /// <summary>
        /// Asserts that a function throws a specific exception.
        /// </summary>
        /// <typeparam name="TException">Type of the expected exception.</typeparam>
        /// <param name="func">The function to test.</param>
        /// <exception cref="ArgumentNullException">The function to test is null.</exception>
        /// <exception cref="AssertFailedException">The function did not throw
        /// the expected exception.</exception>
        public static void AssertThrows<TException>(Action func) where TException : Exception
        {
            if (func == null)
                throw new ArgumentNullException(
                    UtilResources.Get("ArgumentCanNotBeNull"));

            var exceptionThrown = false;

            try
            {
                func.Invoke();
            }
            catch (TException)
            {
                exceptionThrown = true;
            }

            if (!exceptionThrown)
                throw new AssertFailedException(
                    UtilResources.Get("AssertExceptionNotThrown", nameof(TException)));
        }
    }
}