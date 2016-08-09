using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navico.B2B.Buybook.Web.Tests.TestHelpers
{
    /// <summary>
    /// http://stackoverflow.com/questions/113395/how-can-i-test-for-an-expected-exception-with-a-specific-exception-message-from
    /// </summary>
    public static class ExceptionAssert
    {
        public static T Throws<T>(Action action) where T : Exception
        {
            try
            {
                action();
            }
            catch (T ex)
            {
                return ex;
            }
            Assert.Fail("Exception of type {0} should be thrown.", typeof(T));

            //  The compiler doesn't know that Assert.Fail
            //  will always throw an exception
            return null;
        }        
    }
}