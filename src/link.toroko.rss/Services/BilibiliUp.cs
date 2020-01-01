using Hangfire;
using link.toroko.rsshub.startup;
using Robot.API;
using Robot.Code;
using Robot.Property;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace link.toroko.rsshub.Services
{
    public class BilibiliUp : IServices
    {
        public bool List(string qq, string gdid, string msgContent, int messageid, out string _content, bool ispro = false, Dictionary<string, object> Args = null)
        {
            string content = _Code.Code_At(qq) + Environment.NewLine;
            try
            {
                List<long> keys = Data.bilibili_up.bilibili_up_user.Where(
                          ww => (ww.Value.Where(s => s.Gdid == gdid && s.Qq == qq).Count() > 0)).Select(s => s.Key).ToList();

                if (keys.Count == 0)
                {
                    content += "沒有任何订阅";
                    _content = content;
                    return true;
                }

                content += String.Join(Environment.NewLine, Data.bilibili_up.bilibili_up.Where(
                      w => keys.Contains(w.Key)
                          ).ToArray());
                _content = content;
                return true;
            }
            catch (Exception ex)
            {
                CQAPI.AddLog(RobotBase.CQ_AuthCode, CQAPI.LogPriority.CQLOG_ERROR, "BilibiliUp.List", ex.ToString());
                _content = content;
                return false;
            }
        }

        public bool Reg(string qq, string gdid, string msgContent, int messageid, out string _content, Dictionary<string, object> args = null)
        {
            string content = _Code.Code_At(qq) + Environment.NewLine;
            string user_id = new String(msgContent.Where(Char.IsDigit).ToArray());
            long user = 0;
            bool isfound = true;
            string username = "";
            Int64.TryParse(user_id, out user);

            if (!Data.bilibili_up.bilibili_up.TryGetValue(user, out username))
            {
                isfound = link.toroko.rsshub.Services.Diygod.Rsshub.Up.GetUp(user, new Action<diygod.Rss>((r) =>
                {
                    username = r.Channel.Title;
                }));
            }

            if (!isfound)
            {
                content += $"关注B站UP主失败,找不到该UP主或连线失败";
                _content = content;
                return false;
            }

            Data.bilibili_up.bilibili_up.TryAdd(user, username);
            RecurringJob.AddOrUpdate($"关注B站UP主{user}", () => Hangfire.job.Bilibili.up(username,user), Cron.MinuteInterval(Data.hangfire_conf.CronMinuteInterval));
            Data.bilibili_up.bilibili_up_user.TryAdd(user, new List<User>());
            Data.bilibili_up.bilibili_up_user.TryGetValue(user, out List<User> bu);
            DataController.Save();

            if (bu?.Where(w => w.Qq == qq && w.Gdid == gdid).Count() == 0)
            {
                bu.Add(new User { Qq = qq, Gdid = gdid });
                content += $"成功关注B站UP主《{username}》({user})";
                _content = content;
                return true;
            }
            else
            {
                content += $"已关注该B站UP主《{username}》({user})";
                _content = content;
                return false;
            }
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
            string user_id = new String(msgContent.Where(Char.IsDigit).ToArray());
            long user = 0;
            string username = "";
            Int64.TryParse(user_id, out user);
            if (user < 1) { _content = content; return false; }
            Data.bilibili_up.bilibili_up_user.TryGetValue(user, out List<User> bu);
            Data.bilibili_up.bilibili_up.TryGetValue(user, out username);
            if (bu == null | bu?.Where(w => w.Qq == qq && w.Gdid == gdid).Count() == 0)
            {
                content += $"找不到你关注了该B站UP主";
                if (username != null)
                {
                    content += (username == "" ? "" : $"《{username}》");
                }
                content += ",取关失敗";
            }
            else
            {
                bu?.Remove(bu?.Where(w => w.Qq == qq && w.Gdid == gdid).First());
                content += $"B站UP主《{username}》取关成功";
            }
            if (bu?.Count == 0) { Data.bilibili_up.bilibili_up_user.TryRemove(user, out List<User> tmp); RecurringJob.RemoveIfExists($"关注B站UP主{user}"); }
            DataController.Save();
            _content = content;
            return true;
        }
    }
}
