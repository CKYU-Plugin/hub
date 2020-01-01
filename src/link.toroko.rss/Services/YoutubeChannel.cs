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
    public class YoutubeChannel : IServices
    {
        public bool List(string qq, string gdid, string msgContent, int messageid, out string _content, bool ispro = false, Dictionary<string, object> Args = null)
        {
            string content = _Code.Code_At(qq) + Environment.NewLine;
            try
            {
                List<string> keys = Data.youtube.youtube_channel_user.Where(
                          ww => (ww.Value.Where(s => s.Gdid == gdid && s.Qq == qq).Count() > 0)).Select(s => s.Key).ToList();

                if (keys.Count == 0)
                {
                    content += "沒有任何订阅";
                    _content = content;
                    return true;
                }

                content += String.Join(Environment.NewLine, Data.youtube.youtube_channel.Where(
                      w => keys.Contains(w.Key)
                          ).ToArray());
                _content = content;
                return true;
            }
            catch (Exception ex)
            {
                CQAPI.AddLog(RobotBase.CQ_AuthCode, CQAPI.LogPriority.CQLOG_ERROR, "YoutubeChannel.List", ex.ToString());
                _content = content;
                return false;
            }
        }

        public bool Reg(string qq, string gdid, string msgContent, int messageid, out string _content, Dictionary<string, object> args = null)
        {
            string content = _Code.Code_At(qq) + Environment.NewLine;
            string channel = Regex.Replace(msgContent, "订阅youtube频道", "", RegexOptions.IgnoreCase).Trim();
            string channelName = string.Empty;
            bool isfound = true;
            var data = Regex.Split(channel, @"youtube.com\/channel\/");
            if(data.Count() == 2)
            {
                channel = data[1];
            }

            if(!Data.youtube.youtube_channel.TryGetValue(channel, out channelName))
            {
                isfound = link.toroko.rsshub.Services.Diygod.Rsshub.youtube.Channel.GetChannel(channel, new Action<diygod.Rss>((r) =>
                {
                    channelName = r.Channel.Title;
                }));
            }

            if (!isfound)
            {
                content += $"订阅YouTube频道失败,找不到该频道或连线失败";
                _content = content;
                return false;
            }

            Data.youtube.youtube_channel.TryAdd(channel, channelName);
            RecurringJob.AddOrUpdate($"YT_{channel}", () => Hangfire.job.Youtube.Channel(channelName,channel), Cron.MinuteInterval(Data.hangfire_conf.CronMinuteInterval));
            Data.youtube.youtube_channel_user.TryAdd(channel, new List<User>());
            Data.youtube.youtube_channel_user.TryGetValue(channel, out List<User> bu);
            DataController.Save();

            if (bu?.Where(w => w.Qq == qq && w.Gdid == gdid).Count() == 0)
            {
                bu.Add(new User { Qq = qq, Gdid = gdid });
                content += $"成功订阅YouTube频道《{channelName}》";
                _content = content;
                return true;
            }
            _content = $"已订阅YouTube频道《{channelName}》";
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
            string channel = Regex.Replace(msgContent, "退订youtube频道", "", RegexOptions.IgnoreCase).Trim();
            var data = Regex.Split(channel, @"youtube.com\/channel\/");
            if (data.Count() == 2)
            {
                channel = data[1];
            }
            if (channel =="" | channel == null) { _content = content; return false; }
            Data.youtube.youtube_channel_user.TryGetValue(channel, out List<User> bu);
            Data.youtube.youtube_channel.TryGetValue(channel, out string cn);
            if (bu == null | bu?.Where(w => w.Qq == qq && w.Gdid == gdid).Count() == 0)
            {
                content += $"找不到你订阅了该YouTube频道";
                if (cn != null)
                {
                    content += (cn == "" ? "" : $"《{cn}》");
                }
                content += ",退订失敗";
            }
            else
            {
                bu?.Remove(bu?.Where(w => w.Qq == qq && w.Gdid == gdid).First());
                content += $"YouTube频道《{cn}》退订成功";
            }
            if (bu?.Count == 0) { Data.youtube.youtube_channel_user.TryRemove(channel, out List<User> tmp); RecurringJob.RemoveIfExists($"YT_{channel}"); }
            DataController.Save();
            _content = content;
            return true;
        }
    }
}
