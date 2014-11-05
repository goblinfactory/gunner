using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gunner.Engine;

namespace Gunner.Tests.Mocks
{
    public class MockErrorLogger : IErrorLogger
    {
        private readonly bool _throwOnFirstError;

        /// <summary>
        /// Mock error logger.
        /// </summary>
        /// <param name="throwOnFirstError">set to true as a precaution to ensure that your test results contain no unexpected exceptions.</param>
        public MockErrorLogger(bool throwOnFirstError = false)
        {
            _throwOnFirstError = throwOnFirstError;
        }

        public List<Tuple<string, Exception>> Errors = new List<Tuple<string, Exception>>(); 
        public void LogError(string url, Exception ex)
        {
            Errors.Add(new Tuple<string, Exception>(url,ex));
        }

    }
}
