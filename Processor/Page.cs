using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Processor
{
   public abstract class Page
   {
        // 当前页名称
        public string pageName { get; set; }

        // 当前页的title名称
        public string titleName { get; set; }

        // 当前页需要执行的命令集合
        public List<Command> cmmList { get; set; }

        public PageManager pageManager { get; set; }

        /// <summary>
        /// 开始执行命令，可以通过cmmName指定开始执行命令的起始位置
        /// </summary>
        /// <param name="cmmName"></param>
        public abstract void Excute(string cmmName = "");
   }

   public class RootPage:Page
   {
        public override void Excute(string cmmName = "")
        {

        }
   }
}
