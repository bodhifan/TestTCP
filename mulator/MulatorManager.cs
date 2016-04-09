
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Common.Utility;
using System.Threading;

namespace Common.Mulator
{
    /// <summary>
    /// 模拟器管理器
    /// </summary>
    public class MulatorManager
    {
        ILog log = LogManager.GetLogger(typeof(MulatorManager));
        // 模拟器列表
        List<Mulator> mulators = new List<Mulator>();

        // 模拟器远程端口

        /// <summary>
        /// 启动一个模拟器，并一直等待其启动完成
        /// </summary>
        public Mulator Setup(string mulatroName,bool restart)
        {
            if (restart)
            {
                // 1.启动一个模拟器
                 SetupMulatorInstance(mulatroName);
                // 2.等待其启动完全，硬等待其一分钟
                 Thread.Sleep(1000 * 60);


            }


            // 3.连接该模拟器
            AdbUtility.GetAllDevices();

            // 3.模拟器
            Mulator mulator = new Mulator();
            mulator.name = mulatroName;
            mulator.localPort = 42223;
            mulator.remotePort = 42222;
            mulator.localIPAddr = "127.0.0.1:21503";

            return mulator;
        }

        /// <summary>
        /// 启动一个新的模拟器实例
        /// </summary>
        private bool SetupMulatorInstance(string mulatorName)
        {
            string consolePath = string.Format("{0} {1}", Constants.MULATOR_CONSOLE_PATH, mulatorName);
            ProcessUtility.ExecAync(Constants.MULATOR_CONSOLE_PATH, " "+mulatorName,mulatorName);

            return true;

        }

        /// <summary>
        /// 关闭一个模拟器
        /// </summary>
        /// <param name="mulator"></param>
        public void Stop(Mulator mulator)
        {

        }

        /// <summary>
        /// 根据名称获取模拟器
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Mulator GetEMuByName(string name)
        {
            foreach(Mulator emu in mulators)
            {
                if (emu.name.Equals(name))
                    return emu;
            }

            return null;
        }
    }
}
