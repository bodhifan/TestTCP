
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Mulator
{
    /// <summary>
    /// 模拟器管理器
    /// </summary>
    public class MulatorManager
    {
        // 模拟器列表
        List<Mulator> mulators = new List<Mulator>();
        /// <summary>
        /// 启动一个模拟器，并一直等待其启动完成
        /// </summary>
        public Mulator Setup()
        {
            return null;
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
