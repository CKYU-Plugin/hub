using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Robot.API;
using Robot.Code;
using Robot.Property;

namespace link.toroko.rsshub.Hangfire.job
{
    public class Bilibili
    {
        [DisplayName("通知更新资讯;番剧:{0}(id:{1})")]
        public static void Bangumi(string bangumi , long seasonid)
        {
            link.toroko.rsshub.diygod.rsshub.api.Bangumi.GetBangumi(seasonid, new Action<diygod.RssHub>((r) =>
            {
                long items_count = -1;
                if(!Data.bangumi_status.TryGetValue(seasonid,out items_count))
                {
                    Data.bangumi_status.TryAdd(seasonid,r.items.Count());
                }

                if (r.items.Count() > items_count)
                {
                    System.Collections.Concurrent.ConcurrentQueue<Data.BangumiUser> bu = new System.Collections.Concurrent.ConcurrentQueue<Data.BangumiUser>();
                    Data.bangumi_user.TryGetValue(seasonid, out bu);

                    ConcurrentDictionary<string, ConcurrentBag<string>> GroupQq = new ConcurrentDictionary<string, ConcurrentBag<string>>();
                    foreach (var u in bu)
                    {
                        ConcurrentBag<string> Qq = new ConcurrentBag<string>();
                        if(!GroupQq.TryGetValue(u.Gdid,out Qq))
                        {
                            GroupQq.TryAdd(u.Gdid, new ConcurrentBag<string>() { u.Qq });
                        }
                        else
                        {
                            Qq.Add(u.Qq);
                        }
                    }
                    foreach (var g in GroupQq)
                    {
                        string content = "";

                        foreach (var u in g.Value)
                        {
                            content +=  $"{_Code.Code_At(u)}";
                        }

                        content += $"{Environment.NewLine}{r.title}更新啦~{Environment.NewLine}{r.items.First().title}{Environment.NewLine}{r.items.First().url}";

                        long gdid = -1;
                        long.TryParse(g.Key, out gdid);
                        if (gdid != -1)
                        {
                            CQAPI.SendGroupMessage(RobotBase.CQ_AuthCode, gdid, content);
                        }
                    }

                    GroupQq = null;
                }
            }));
        }
    }
}
