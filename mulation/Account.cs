using Common.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Common.Mulation
{
    public class Account
    {
        public string userName { get; set; }
        public string password { get; set; }
        public string phone { get; set; }
        public bool isSuc { get; set; }

        public string reason { get; set; }

        public override string ToString()
        {
            return string.Format("{0}\t{1}\t{2}\t{3}\t{4}", phone, userName, password, reason,DateTime.Now.ToShortTimeString());
        }
    }

    public class AccountFactory
    {
        string fileName = "";
        List<Account> accountList = new List<Account>();
        int curIndex = 0;
        public AccountFactory(string fileName)
        {
            this.fileName = fileName;
            Load(fileName);
        }

        public void Load(string fileName)
        {
            StreamReader sr = new StreamReader(fileName, Encoding.Default);
            String line;
            while ((line = sr.ReadLine()) != null)
            {
                Account cur = new Account();
                cur.phone = line.Trim();

                accountList.Add(cur);
            }

            LogHelper.Info("得到手机号：" + accountList.Count);
         }

        public bool HasNext()
        {
            if (curIndex < accountList.Count)
                return true;
            return false;
        }

        public Account Next()
        {
            return accountList[curIndex++];
        }
    }
}
