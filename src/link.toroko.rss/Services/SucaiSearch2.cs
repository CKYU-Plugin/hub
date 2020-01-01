using Robot.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf.Data;

namespace link.toroko.rsshub.Services
{
    public class SucaiSearch2 : IServices
    {
        public bool List(string qq, string gdid, string msgContent, int messageid, out string _content, bool ispro = false, Dictionary<string, object> args = null)
        {
            if (!ViewModelData.g.BEnable_Xssvip) { _content = string.Empty;  return false; }
            string content = _Code.Code_At(qq);
            link.toroko.rsshub.Services.Sucai.XssvipItem i = null;
            Program.RequestArgs RA = new Program.RequestArgs();
            _content = content;
            try
            {
                i = link.toroko.rsshub.Services.Sucai.Xssvip.GetData(msgContent.Trim());
            }
            catch { }
            if (i?.DownloadUrls?.Count != 0)
            {
                i?.DownloadUrls?.ToList().ForEach(f =>
                {
                    string url = Ft12.Ft12.GetShort(f.Key);//f.Key.Contains("&") ? Wangjie.I8e.GetShort(f.Key) : Catnetwork.Unu.GetShort(f.Key);
                    content += $"{Environment.NewLine}{f.Value}{Environment.NewLine}{url}";
                });
            }
            else
            {
                content += i?.Message;
            }
            _content = content;
            if(i == null)
            {
                return false;
            }
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
            if (!ViewModelData.g.BEnable_Xssvip) { _content = string.Empty; return false; }

            Dictionary<string, string> status = new Dictionary<string, string>();
            try
            {
                status = link.toroko.rsshub.Services.Sucai.Xssvip.GetStatus();
            }
            catch { }

            string content = _Code.Code_At(qq);
            if (status == null)
            {
                content += "读取失败";
            }
            else
            {
                foreach (var d in status)
                {
                    if (d.Key != "有效期")
                    {
                        content += $"{Environment.NewLine}{d.Key}：{d.Value}";
                    }
                }
            }
            _content = content;
            return true;
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
