using Hangfire;
using link.toroko.rsshub.Services.diygod;
using link.toroko.rsshub.startup;
using Newtonsoft.Json;
using RestSharp;
using Robot.API;
using Robot.Code;
using Robot.Property;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace link.toroko.rsshub.MAssembly
{
    public class AssemblyE : IAssemblyModel
    {
        public bool Get(string _router, List<string> _parameters, Action<Rss> _action)
        {
            string tmp = "";
            if (_router.Contains(":"))
            {
                tmp = _router.Split(':')[0];
                tmp += string.Join("/", _parameters);
                //Regex rex = new Regex(@":(\w*)*", RegexOptions.IgnoreCase);
                //MatchCollection matches = rex.Matches(_router);

                //foreach (Match match in matches)
                //{
                //    GroupCollection groups = match.Groups;
                //    if (groups.Count == 1) {
                //        tmp += $"{groups[0].Value}";
                //    }
                //}
            }
            RestClient client = new RestClient("https://rsshub.app");
            RestRequest request = new RestRequest($"/{tmp}", Method.GET);
            client.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 10_0_1 like Mac OS X) AppleWebKit/602.1.50 (KHTML, like Gecko) Version/10.0 Mobile/14A403 Safari/602.1";

            var tcs = new TaskCompletionSource<Rss>();
            var response = client.ExecuteAsync(request, (r) =>
            {
                if (r.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    try
                    {
                        //var data = JsonConvert.DeserializeObject<RssHub>(r.Content, new JsonSerializerSettings
                        //{
                        //    NullValueHandling = NullValueHandling.Ignore,
                        //    MissingMemberHandling = MissingMemberHandling.Ignore,
                        //});
                        Rss data = new Rss();
                        XmlSerializer serializer = new XmlSerializer(typeof(Rss));
                        using (TextReader reader = new StringReader(r.Content))
                        {
                            data = (Rss)serializer.Deserialize(reader);
                        }

                        tcs.SetResult(data);
                    }
                    catch { tcs.SetResult(null); }
                }
                else
                {
                    tcs.SetResult(null);
                }
            });
            SpinWait.SpinUntil(() => tcs.Task.IsCompleted, 10000);
            if (!tcs.Task.IsCompleted) { return false; }
            if (tcs.Task.Result == null) { return false; }
            _action(tcs.Task.Result);
            return true;
        }

        [DisplayName("XX通知:{0}")]
        public void HangfireJob(string _title, string _router, List<string> _parameters, string _uniqueCode)
        {
            if (!RobotBase.isenableplugin) { return; }
            Get(_router, _parameters, new Action<Services.diygod.Rss>((r) =>
            {
                string content = "";
                if (r?.Channel.Item.Count() == 0) { return; }

                Data.plugins_store[$"{_uniqueCode}"].assembly_status.TryGetValue(_parameters[0], out string id);
                if (id == r.Channel.Item.First().Guid) { return; }

                Data.plugins_store[$"{_uniqueCode}"].assembly_status.AddOrUpdate(_parameters[0], r.Channel.Item.First().Guid, (key, oldValue) => r.Channel.Item.First().Guid);
                if (Data.plugins_store[$"{_uniqueCode}"].assembly_up.TryGetValue(_parameters[0], out string tmp))
                {
                    if (r.Channel.Title != tmp)
                    {
                        Data.plugins_store[$"{_uniqueCode}"].assembly_up.AddOrUpdate(_parameters[0], r.Channel.Title, (key, oldValue) => r.Channel.Title);
                    }
                }
                DataController.Save();

                if (r.Channel.Item.First().PubDate.GMTStrParse(out DateTime dt))
                {
                    if (dt.Date.Ticks + TimeSpan.FromDays(1).Ticks < DateTime.Now.Date.Ticks) { return; }
                }

                Data.plugins_store[$"{_uniqueCode}"].assembly_user.TryGetValue(_parameters[0], out List<User> bu);
                foreach (var g in bu.GroupBy(g => g.Gdid).Select(s => s.First().Gdid))
                {
                    foreach (var u in bu.Where(w => w.Gdid == g))
                    {
                        content += $"{_Code.Code_At(u.Qq)}";
                    }
                    if (long.TryParse(g, out long gdid))
                    {
                        content += $"{Environment.NewLine}關注的UP主有新投稿啦~{Environment.NewLine}《{r.Channel.Title}》{Environment.NewLine}{r.Channel.Item.First().Title}{Environment.NewLine}{r.Channel.Item.First().Link}";
                        CQAPI.SendGroupMessage(RobotBase.CQ_AuthCode, gdid, content);
                    }
                }
            }));
        }

        public bool List(string qq, string gdid, string msgContent, out string _content, string _uniqueCode)
        {
            string content = _Code.Code_At(qq) + Environment.NewLine;
            try
            {
                List<string> keys = Data.plugins_store[$"{_uniqueCode}"].assembly_user.Where(
                          ww => (ww.Value.Select(s => s.Gdid == gdid && s.Qq == qq).Count() > 0)).Select(s => s.Key).ToList();

                if (keys.Count == 0)
                {
                    content += "沒有任何訂閱";
                    _content = content;
                    return true;
                }

                content += String.Join(Environment.NewLine, Data.plugins_store[$"{_uniqueCode}"].assembly_up.Where(
                      w => keys.Contains(w.Key)
                          ).ToArray());
                _content = content;
                return true;
            }
            catch (Exception ex)
            {
                CQAPI.AddLog(RobotBase.CQ_AuthCode, CQAPI.LogPriority.CQLOG_ERROR, $"{_uniqueCode}_List", ex.ToString());
                _content = content;
                return false;
            }
        }

        public bool Reg(string _qq, string _gdid, string _msgContent, out string _content, string _uniqueCode)
        {
            string content = _Code.Code_At(_qq) + Environment.NewLine;
            string user_id = new String(_msgContent.Where(Char.IsDigit).ToArray());
            long user = 0;
            bool isfound = true;
            string username = "";
            Int64.TryParse(user_id, out user);

            if (!Data.bilibili_up.bilibili_up.TryGetValue(user, out username))
            {
                isfound = link.toroko.rsshub.Services.Diygod.Rsshub.Up.GetUp(user, new Action<Services.diygod.Rss>((r) =>
                {
                    username = r.Channel.Title;
                }));
            }

            if (!isfound)
            {
                content += $"關注B站UP主失敗,找不到該UP主或連線失敗";
                _content = content;
                return false;
            }

            Data.bilibili_up.bilibili_up.TryAdd(user, username);
            RecurringJob.AddOrUpdate($"關注B站UP主{user}", () => Hangfire.job.Bilibili.up(username, user), Cron.MinuteInterval(Data.hangfire_conf.CronMinuteInterval));
            Data.bilibili_up.bilibili_up_user.TryAdd(user, new List<User>());
            Data.bilibili_up.bilibili_up_user.TryGetValue(user, out List<User> bu);
            DataController.Save();

            if (bu?.Where(w => w.Qq == _qq && w.Gdid == _gdid).Count() == 0)
            {
                bu.Add(new User { Qq = _qq, Gdid = _gdid });
                content += $"成功關注B站UP主《{username}》({user})";
                _content = content;
                return true;
            }
            else
            {
                content += $"已關注該B站UP主《{username}》({user})";
                _content = content;
                return false;
            }
        }

        public bool UnReg(string _qq, string _gdid, string _msgContent, out string _content, string _uniqueCode)
        {
            string content = _Code.Code_At(_qq) + Environment.NewLine;
            string user_id = new String(_msgContent.Where(Char.IsDigit).ToArray());
            long user = 0;
            string username = "";
            Int64.TryParse(user_id, out user);

            Data.bilibili_up.bilibili_up_user.TryGetValue(user, out List<User> bu);
            Data.bilibili_up.bilibili_up.TryGetValue(user, out username);
            if (bu?.Where(w => w.Qq == _qq && w.Gdid == _gdid).Count() == 0)
            {
                content += $"找不到你關注了該B站UP主";
                content += (username == "" ? "" : $"《{username}》");
                content += ",取關失敗";
            }
            else
            {
                bu?.Remove(bu?.Where(w => w.Qq == _qq && w.Gdid == _gdid).First());
                content += $"B站UP主《{username}》取關成功";
            }
            if (bu?.Count == 0) { Data.bilibili_up.bilibili_up_user.TryRemove(user, out List<User> tmp); RecurringJob.RemoveIfExists($"关注B站UP主{user}"); }
            DataController.Save();
            _content = content;
            return false;
        }
    }

}
