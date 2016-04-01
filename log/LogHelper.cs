using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTCP.log
{
    public class LogHelper
    {
        private static LogHelper logHelper = null;
        private static ILog DEFAULT = LogManager.GetLogger("DEFAULT");
        public static LogHelper Instance()
        {
            if(null == logHelper)
            {
                logHelper = new LogHelper();
            }
            return logHelper;
        }
        public static ILog GetLog()
        {
            return DEFAULT;
        }
    }
}
