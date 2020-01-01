using link.toroko.rsshub.startup;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Robot.API;
using Robot.Code;
using Robot.Property;
using System.Globalization;
using System.Web;

namespace link.toroko.rsshub.Hangfire.job
{
    public class Bilibili
    {
        [DisplayName("投稿通知:{1}-{0}")]
        public static void up(string username, long user_id)
        {
            if (!RobotBase.isenableplugin) { return; }
            link.toroko.rsshub.Services.Diygod.Rsshub.Up.GetUp(user_id, new Action<Services.diygod.Rss>((r) =>
            {
                string content = "";
                if (r?.Channel.Item.Count() == 0) { return; }

                Data.bilibili_up.bilibili_up_status.TryGetValue(user_id, out string id);
                if (id == r.Channel.Item.First().Guid) { return; }

                Data.bilibili_up.bilibili_up_status.AddOrUpdate(user_id, r.Channel.Item.First().Guid, (key, oldValue) => r.Channel.Item.First().Guid);
                if (Data.bilibili_up.bilibili_up.TryGetValue(user_id, out string tmp))
                {
                    if (r.Channel.Title != tmp)
                    {
                        Data.bilibili_up.bilibili_up.AddOrUpdate(user_id, r.Channel.Title, (key, oldValue) => r.Channel.Title);
                    }
                }
                DataController.Save();

                if (r.Channel.Item.First().PubDate.GMTStrParse(out DateTime dt))
                {
                    if (dt.Date.Ticks + TimeSpan.FromDays(1).Ticks < DateTime.Now.Date.Ticks) { return; }
                }

                Data.bilibili_up.bilibili_up_user.TryGetValue(user_id, out List<User> bu);
                foreach (var g in bu.GroupBy(g => g.Gdid).Select(s => s.First().Gdid))
                {
                    content = "";
                    foreach (var u in bu.Where(w => w.Gdid == g))
                    {
                        content += $"{_Code.Code_At(u.Qq)}";
                    }
                    if (long.TryParse(g, out long gdid))
                    {
                        content += $"{Environment.NewLine}關注的UP主有新投稿啦~{Environment.NewLine}《{r.Channel.Title}》{Environment.NewLine}{HttpUtility.HtmlDecode(r.Channel.Item.First().Title)}{Environment.NewLine}{r.Channel.Item.First().Link}";
                        CQAPI.SendGroupMessage(RobotBase.CQ_AuthCode, gdid, content);
                    }
                }
            }));
        }

        [DisplayName("開播通知:{1}-{0}")]
        public static void live(string roomname, long room_id)
        {
            if (!RobotBase.isenableplugin) { return; }
            link.toroko.rsshub.Services.Diygod.Rsshub.Live.GetLive(room_id, new Action<Services.diygod.Rss>((r) =>
            {
                string content = "";
                if (r?.Channel.Item.Count() == 0) { return; }

                Data.bilibili_live.bilibili_live_status.TryGetValue(room_id, out string id);
                if (id == r.Channel.Item.First().Guid) { return; }

                Data.bilibili_live.bilibili_live_status.AddOrUpdate(room_id, r.Channel.Item.First().Guid, (key, oldValue) => r.Channel.Item.First().Guid);
                if (Data.bilibili_live.bilibili_room.TryGetValue(room_id, out string tmp))
                {
                    if (r.Channel.Title != tmp)
                    {
                        Data.bilibili_live.bilibili_room.AddOrUpdate(room_id, r.Channel.Title, (key, oldValue) => r.Channel.Title);
                    }
                }
                DataController.Save();

                if (r.Channel.Item.First().PubDate.GMTStrParse(out DateTime dt))
                {
                    if (dt.Date.Ticks + TimeSpan.FromDays(1).Ticks < DateTime.Now.Date.Ticks) { return; }
                }

                Data.bilibili_live.bilibili_live_user.TryGetValue(room_id, out List<User> bu);
                foreach (var g in bu.GroupBy(g => g.Gdid).Select(s => s.First().Gdid))
                {
                    content = "";
                    foreach (var u in bu.Where(w => w.Gdid == g))
                    {
                        content += $"{_Code.Code_At(u.Qq)}";
                    }
                    if (long.TryParse(g, out long gdid))
                    {
                        content += $"{Environment.NewLine}關注的B站直播主《{r.Channel.Title.Replace(" 直播間開播狀態","")}》開播啦~{Environment.NewLine}{HttpUtility.HtmlDecode(r.Channel.Item.First().Title)}{Environment.NewLine}{r.Channel.Item.First().Link}";
                        CQAPI.SendGroupMessage(RobotBase.CQ_AuthCode, gdid, content);
                    }
                }
            }));
        }

        [DisplayName("番劇更新通知:{1}-{0}")]
        public static void Bangumi(string bangumi, long seasonid)
        {
            if (!RobotBase.isenableplugin) { return; }
            link.toroko.rsshub.Services.diygod.rsshub.api.Bangumi.GetBangumi(seasonid, new Action<Services.diygod.Rss>((r) =>
            {
                bool isfirst = false;
                string content = "";

                if (!Data.bilibili_bangumi.bilibili_bangumi_status.TryGetValue(seasonid, out long items_count)) { isfirst = true; }
                if (r?.Channel.Item.Count() == 0) { return; }
                if (!(r.Channel.Item.Count() > items_count | isfirst)) { return; }

                Data.bilibili_bangumi.bilibili_bangumi_status.AddOrUpdate(seasonid, r.Channel.Item.Count(), (key, oldValue) => r.Channel.Item.Count());
                DataController.Save();
                //if (r.Channel.Item.First().PubDate.GMTStrParse(out DateTime dt))
                //{
                //    if (dt.Date.Ticks + TimeSpan.FromDays(1).Ticks < DateTime.Now.Date.Ticks) { return; }
                //}
                Data.bilibili_bangumi.bilibili_bangumi_user.TryGetValue(seasonid, out List<User> bu);

                foreach (var g in bu.GroupBy(g => g.Gdid).Select(s => s.First().Gdid))
                {
                    content = "";
                    foreach (var u in bu.Where(w => w.Gdid == g))
                    {
                        content += $"{_Code.Code_At(u.Qq)}";
                    }
                    if (long.TryParse(g, out long gdid))
                    {
						//content += $"{Environment.NewLine}番劇《{r.Channel.Title}》更新啦~{Environment.NewLine}{HttpUtility.HtmlDecode(r.Channel.Item.First().Title)}{Environment.NewLine}{r.Channel.Item.First().Description.Substring(0, 24)}{Environment.NewLine}{r.Channel.Item.First().Link}";
						content += $"{Environment.NewLine}番劇《{r.Channel.Title}》更新啦~{Environment.NewLine}{HttpUtility.HtmlDecode(r.Channel.Item.First().Title)}{Environment.NewLine}{r.Channel.Item.First().Link}";
                        CQAPI.SendGroupMessage(RobotBase.CQ_AuthCode, gdid, content);
                    }
                }
            }));
        }
    }
}
