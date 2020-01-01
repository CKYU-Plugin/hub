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
    public class Youtube
    {
        [DisplayName("YouTube頻道:{0}")]
        public static void Channel(string clannelName , string clannel)
        {
            if (!RobotBase.isenableplugin) { return; }
            link.toroko.rsshub.Services.Diygod.Rsshub.youtube.Channel.GetChannel(clannel, new Action<Services.diygod.Rss>((r) =>
            {
                string content = "";
                if (r?.Channel.Item.Count() == 0) { return; }

                Data.youtube.youtube_channel_status.TryGetValue(clannel, out string id);
                if (id == r.Channel.Item.First().Guid) { return; }

                Data.youtube.youtube_channel_status.AddOrUpdate(clannel, r.Channel.Item.First().Guid, (key, oldValue) => r.Channel.Item.First().Guid);
                if (Data.youtube.youtube_channel.TryGetValue(clannel, out string tmp))
                {
                    if (r.Channel.Title != tmp)
                    {
                        Data.youtube.youtube_channel.AddOrUpdate(clannel, r.Channel.Title, (key, oldValue) => r.Channel.Title);
                    }
                }
                DataController.Save();

                if (r.Channel.Item.First().PubDate.GMTStrParse(out DateTime dt))
                {
                    if (dt.Date.Ticks + TimeSpan.FromDays(1).Ticks < DateTime.Now.Date.Ticks) { return; }
                }

                Data.youtube.youtube_channel_user.TryGetValue(clannel, out List<User> bu);

                foreach (var g in bu.GroupBy(g => g.Gdid).Select(s => s.First().Gdid))
                {
                    content = "";
                    foreach (var u in bu.Where(w => w.Gdid == g))
                    {
                        content += $"{_Code.Code_At(u.Qq)}";
                    }
                    if (long.TryParse(g, out long gdid))
                    {
                        content += $"{Environment.NewLine}YouTube頻道《{r.Channel.Title}》更新!{Environment.NewLine}{HttpUtility.HtmlDecode(r.Channel.Item.First().Title)}{Environment.NewLine}{r.Channel.Item.First().Link}";
                        CQAPI.SendGroupMessage(RobotBase.CQ_AuthCode, gdid, content);
                    }
                }
            }));
        }

    }
}
