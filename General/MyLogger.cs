using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.General
{
    public class MyLogger
    {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Get logger.
        /// </summary>
        /// <returns>The instance.</returns>
        public static log4net.ILog GetInstance()
        {
            return Logger;
        }
    }
}