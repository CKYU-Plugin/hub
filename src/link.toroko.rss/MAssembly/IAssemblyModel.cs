using link.toroko.rsshub.Services.diygod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace link.toroko.rsshub.MAssembly
{
    public interface IAssemblyModel
    {
        bool Reg(string _qq, string _gdid, string _msgContent, out string _content, string _uniqueCode);
        bool UnReg(string _qq, string _gdid, string _msgContent, out string _content, string _uniqueCode);
        bool List(string _qq, string _gdid, string _msgContent, out string _content, string _uniqueCode);
        bool Get(string _router, List<string> _parameters, Action<Rss> _action);
        void HangfireJob(string _title, string _router, List<string> _parameters, string _uniqueCode);
    }
}
