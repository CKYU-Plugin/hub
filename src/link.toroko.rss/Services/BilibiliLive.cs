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
    public class BilibiliLive : IServices
    {
        public bool List(string qq, string gdid, string msgContent, int messageid, out string _content, bool ispro = false, Dictionary<string, object> Args = null)
        {
            string content = _Code.Code_At(qq) + Environment.NewLine;
            try
            {
                List<long> keys = Data.bilibili_live.bilibili_live_user.Where(
                          ww => (ww.Value.Where(s => s.Gdid == gdid && s.Qq == qq).Count() > 0)).Select(s => s.Key).ToList();

                if (keys.Count == 0)
                {
                    content += "沒有任何订阅";
                    _content = content;
                    return true;
                }

                content += String.Join(Environment.NewLine, Data.bilibili_live.bilibili_room.Where(
                      w => keys.Contains(w.Key)
                          ).ToArray());
                _content = content;
                return true;
            }
            catch (Exception ex)
            {
                CQAPI.AddLog(RobotBase.CQ_AuthCode, CQAPI.LogPriority.CQLOG_ERROR, "BilibiliLive.List", ex.ToString());
                _content = content;
                return false;
            }
        }

        public bool Reg(string qq, string gdid, string msgContent, int messageid, out string _content, Dictionary<string, object> args = null)
        {
            try
            {
                string content = _Code.Code_At(qq) + Environment.NewLine;
                string room_id = "";
                room_id = new String(msgContent.Where(Char.IsDigit).ToArray());
                if (msgContent.Contains("live.bilibili.com"))
                {
                    var pattern = @".*\/(\d*)";
                    var match = Regex.Match(msgContent, pattern);
                    if (match.Success)
                    {
                        if (match.Groups.Count == 2)
                        {
                            if (match.Groups[1].Value != "")
                            {
                                room_id = match.Groups[1].Value;
                            }
                        }
                    }
                }
                long room = 0;
                Int64.TryParse(room_id, out room);
                bool isfound = true;
                string roomname = "";

                if (!Data.bilibili_live.bilibili_room.TryGetValue(room, out roomname))
                {
                    isfound = link.toroko.rsshub.Services.Diygod.Rsshub.Live.GetLive(room, new Action<diygod.Rss>((r) =>
                    {
                        roomname = r.Channel.Title.Replace(" 直播间开播状态","");
                    }));
                }

                if (!isfound)
                {
                    content += $"订阅B站直播房间失败,找不到该房间或连线失败";
                    _content = content;
                    return false;
                }

                Data.bilibili_live.bilibili_room.TryAdd(room, roomname);
                RecurringJob.AddOrUpdate($"B站开播通知{room}", () => Hangfire.job.Bilibili.live(roomname,room), Cron.MinuteInterval(Data.hangfire_conf.CronMinuteInterval));
                Data.bilibili_live.bilibili_live_user.TryAdd(room, new List<User>());
                Data.bilibili_live.bilibili_live_user.TryGetValue(room, out List<User> bu);
                DataController.Save();

                if (bu?.Where(w => w.Qq == qq && w.Gdid == gdid).Count() == 0)
                {
                    bu.Add(new User { Qq = qq, Gdid = gdid });
                    content += $"成功订阅B站开播通知{room_id}";
                    _content = content;
                    return true;
                }
                else
                {
                    content += $"已订阅该B站开播通知({room_id})";
                    _content = content;
                    return false;
                }
            }
            catch (Exception ex)
            {
                _content = ex.Message;
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
            string room_id = "";
            room_id = new String(msgContent.Where(Char.IsDigit).ToArray());
            long room = 0;
            Int64.TryParse(room_id, out room);
            if (room < 1) { _content = content; return false; }
            string roomname = "";

            Data.bilibili_live.bilibili_live_user.TryGetValue(room, out List<User> bu);
            Data.bilibili_live.bilibili_room.TryGetValue(room, out roomname);
            if (bu == null | bu?.Where(w => w.Qq == qq && w.Gdid == gdid).Count() == 0)
            {
                content += $"找不到你订阅了该B站直播房间";
                if (roomname != null)
                {
                    content += (roomname == "" ? "" : $"《{roomname}》");
                }
                content += ",退订失敗";
            }
            else
            {
                bu?.Remove(bu?.Where(w => w.Qq == qq && w.Gdid == gdid).First());
                content += $"B站直播房间《{roomname}》退订成功";
            }
            if (bu?.Count == 0) { Data.bilibili_live.bilibili_live_user.TryRemove(room, out List<User> tmp); RecurringJob.RemoveIfExists($"B站开播通知{room}"); }
            DataController.Save();
            _content = content;
            return true;
        }
    }
}
