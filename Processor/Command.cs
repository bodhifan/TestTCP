using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Processor
{
    public abstract class Command
    {
        // 执行的命令
        public string excuteCmm { get; set; }
        // 命令行名称
        public string cmmName { get; set; }
        // 延迟时间
        public string delayMills { get; set; } 

        public Page page { get; set; }

        public abstract void Excute();
    }
}
