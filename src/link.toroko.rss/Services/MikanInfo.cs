using Robot.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace link.toroko.rsshub.Services
{
    public class MikanInfo : IServices
    {
        public bool List(string qq, string gdid, string msgContent, int messageid, out string _content, bool ispro = false, Dictionary<string, object> args = null)
        {
            string content = _Code.Code_At(qq);

            bool IsSuccess = link.toroko.rsshub.Services.Mikanani.Mikan.GetList(msgContent, ((r) =>
            {
                if (r.channel.item == null | r.channel.item?.Count() == 0)
                {
                    content += "没找到任何结果";
                }
                else
                {
                    for (int s = 0; s < (r.channel.item.Count() > 5 ? 5 : r.channel.item.Count()); s++)
                    {
                        var match1 = Regex.Matches(r.channel.item[s].link, @"([0-9a-fA-F]{40})", RegexOptions.IgnoreCase);
                        if (match1.Count == 0) { continue; }
                        if (match1[0].Groups.Count < 1) { continue; }
                        string MT = match1[0].Groups[1].Value.Trim();
                        content += Environment.NewLine;
                        content += r.channel.item[s].description;
                        content += Environment.NewLine;
                        content += MT;
                    }
                }
            }));

            if (!IsSuccess)
            {
                content += "连线失败";
            }

            _content = content;
            return true;
        }

        public bool Reg(string qq, string gdid, string msgContent, int messageid, out string _content, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

        public bool Start(string qq, string gdid, string msgContent, int messageid, out string _content)
        {
            throw new NotImplementedException();
        }

        public bool Status(string qq, string gdid, string msgContent, int messageid, out string _content, bool ispro = false, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

        public bool Stop(string qq, string gdid, string msgContent, int messageid, out string _content)
        {
            throw new NotImplementedException();
        }

        public bool UnReg(string qq, string gdid, string msgContent, int messageid, out string _content, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }
    }
}
