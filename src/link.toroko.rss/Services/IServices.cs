using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace link.toroko.rsshub.Services
{
    public interface IServices
    {
        bool Start(string qq, string gdid, string msgContent, int messageid, out string _content);
        bool Stop(string qq, string gdid, string msgContent, int messageid, out string _content);
        bool Reg(string qq, string gdid, string msgContent, int messageid, out string _content, Dictionary<string, object> args = null);
        bool UnReg(string qq, string gdid, string msgContent, int messageid, out string _content, Dictionary<string, object> args = null);
        bool List(string qq, string gdid, string msgContent, int messageid, out string _content, bool ispro = false,Dictionary<string,object> args = null);
        bool Status(string qq, string gdid, string msgContent, int messageid, out string _content, bool ispro = false, Dictionary<string, object> args = null);
    }
}
