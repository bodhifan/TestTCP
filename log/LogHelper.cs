using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.log
{
    public class LogHelper
    {
        private static ILog DEFAULT = LogManager.GetLogger("DEFAULT");

        public static void Info(string msg)
        {
            DEFAULT.Info(msg);
        }
        public static void Error(string msg)
        {
            DEFAULT.Error(msg);
        }

        public static void Warn(string msg)
        {
            DEFAULT.Warn(msg);
        }

        public static void Debug(string msg)
        {
            DEFAULT.Debug(msg);
        }
    }
}
