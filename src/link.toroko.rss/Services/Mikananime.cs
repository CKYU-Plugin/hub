using Robot.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace link.toroko.rsshub.Services
{
    public class Mikananime : IServices
    {
        public bool List(string qq, string gdid, string msgContent, int messageid, out string _content, bool ispro = false, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

        public bool Reg(string qq, string gdid, string msgContent, int messageid, out string _content, Dictionary<string, object> args = null)
        {
            string content = _Code.Code_At(qq);
            Dictionary<string, string> tmp = new Dictionary<string, string>();
            if (args == null)
            {
                tmp = link.toroko.rsshub.Services.Mikanani.Mikan.GetBangumiId(msgContent);
            }
            else
            {
                if (args.TryGetValue("SelectedList", out object disc))
                {
                    if (int.TryParse(msgContent, out int _index))
                    {
                        tmp.Add(((Dictionary<string, string>)disc).ElementAt(_index).Key, ((Dictionary<string, string>)disc).ElementAt(_index).Value);
                    }
                }
            }

            if (tmp.Count == 0)
            {
                content += $"{Environment.NewLine}找不到相关番剧";
            }
            else if (tmp.Count == 1)
            {
                content += $"{Environment.NewLine}已订阅";
            }
            else
            {
                content += $"{Environment.NewLine}请选择番剧:";

                Parallel.ForEach(tmp, (line, state, index) =>
                {
                    content += $"{Environment.NewLine}[{index + 1}]{line.Value}";
                });
                Program.RequestArgs RA = new Program.RequestArgs();
                RA.Args.Add("SelectedList", tmp);
                RA.Service = this;
                RA.Content = "";
                Program.requestNextList.AddOrUpdate(qq, RA, (key, oldValue) => oldValue = RA);
            }
            _content = content;
            return true;
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
