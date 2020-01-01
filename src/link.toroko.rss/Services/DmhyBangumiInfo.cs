using link.toroko.rsshub.Robot.Extension;
using link.toroko.rsshub.Services.bilibili.task;
using link.toroko.rsshub.Services.bilibili.web_api;
using link.toroko.rsshub.startup;
using Robot.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace link.toroko.rsshub.Services
{
    public class DmhyBangumiInfo : IServices
    {
        public bool List(string qq, string gdid, string msgContent, int messageid, out string _content, bool ispro = false, Dictionary<string, object> Args = null)
        {
            string content = _Code.Code_At(qq) + Environment.NewLine;

           bool IsSuccess = link.toroko.rsshub.Services.Dmhy.Dmhy.GetList(msgContent, ((r) =>
             {
                 if (r.channel.item == null | r.channel.item?.Count() == 0)
                 {
                     content += "沒找到任何结果";
                 }
                 else
                 {
                    for (int s = 0; s < (r.channel.item.Count() > 5 ? 5 : r.channel.item.Count()) ; s++)
                    {
                         var match1 = Regex.Matches(r.channel.item[s].enclosure.url, @"urn:btih:([2-7A-Z]{32})", RegexOptions.IgnoreCase);
                         if (match1.Count == 0) { continue; }
                         if (match1[0].Groups.Count < 2) { continue; }
                         string MT2 = match1[0].Groups[1].Value.Trim();
                         string MT = BitConverter.ToString(Base32.Decode(MT2)).Replace("-","");
                         content += r.channel.item[s].title;
                         content += Environment.NewLine;
                         content += MT;
                         //     content += $"http://dl.dmhy.org/{r.channel.item[s].pubDate.ToDateTime("DDD, dd MMM yyyy hh:mm:ss +0800").ToString("yyyy/MM/dd")}/{MT}.torrent";
                         content += Environment.NewLine;
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
            string content = _Code.Code_At(qq) + Environment.NewLine;
            _content = content;
            return false;
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
            string content = _Code.Code_At(qq) + Environment.NewLine;
            _content = content;
            return false;
        }
    }
}
