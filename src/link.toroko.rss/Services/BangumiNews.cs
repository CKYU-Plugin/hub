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
using System.Threading.Tasks;

namespace link.toroko.rsshub.Services
{
    public class BangumiNews : IServices
    {
        public bool List(string qq, string gdid, string msgContent, int messageid, out string _content, bool ispro = false, Dictionary<string, object> Args = null)
        {
            string content = _Code.Code_At(qq) + Environment.NewLine;
            try
            {
                List<long> keys = Data.bilibili_bangumi.bilibili_bangumi_user.Where(
                          ww => (ww.Value.Where(s => s.Gdid == gdid && s.Qq == qq).Count() > 0)).ToList().Select(s => s.Key).ToList();

                if (keys.Count == 0)
                {
                    content += "没有任何订阅";
                    _content = content;
                    return true;
                }

                content += String.Join(Environment.NewLine, Data.bilibili_bangumi.bilibili_bangumi_session.Where(
                      w => keys.Contains(w.Key)
                          ).ToArray());
                _content = content;
                return true;
            }
            catch (Exception ex)
            {
                CQAPI.AddLog(RobotBase.CQ_AuthCode, CQAPI.LogPriority.CQLOG_ERROR, "BangumiNews.List", ex.ToString());
                _content = content;
                return false;
            }
        }

        public bool Reg(string qq, string gdid, string msgContent, int messageid, out string _content, Dictionary<string, object> args = null)
        {
            try
            {
                msgContent = msgContent.Trim().Replace("追番", "");
                string content = _Code.Code_At(qq);
                List<long> session_id = new List<long>();
                List<string> session_name = new List<string>();
                Dictionary<long, string> success_session_id = new Dictionary<long, string>();
                Dictionary<long, string> failed_session_id = new Dictionary<long, string>();
                Dictionary<long, string> exist_session_id = new Dictionary<long, string>();
                Dictionary<long, string> found_session_id = new Dictionary<long, string>();

                foreach (var s in msgContent.Split(','))
                {
                    if (s.Any(c => (uint)c >= 0x4E00 && (uint)c <= 0x2FA1F))
                    {
                        var tmp = Data.bilibili_bangumi.bilibili_bangumi_session.Where(w => w.Value.Contains(s.Trim()));
                        if (tmp.Count() > 0)
                        {
                            if (tmp.Count() > 1)
                            {
                                tmp.ToList().ForEach(f =>
                                {
                                    found_session_id.Add(f.Key, f.Value);
                                });
                            }
                            else
                            {

                                session_id.Add(tmp.FirstOrDefault().Key);
                            }
                        }
                    }
                }

                foreach (var s in msgContent.Split(','))
                {
                    long id = -1;
                    string sid = new String(s.Where(Char.IsDigit).ToArray());
                    Int64.TryParse(sid, out id);
                    if (id > 0)
                    {
                        session_id.Add(id);
                    }
                }

                foreach (var id in session_id)
                {
                    if (Data.bilibili_bangumi.bilibili_bangumi_session.TryGetValue(id, out string bi))
                    {
                        RecurringJob.AddOrUpdate($"追番{id}", () => Hangfire.job.Bilibili.Bangumi(bi, id), Cron.MinuteInterval(Data.hangfire_conf.CronMinuteInterval));

                        if (!Data.bilibili_bangumi.bilibili_bangumi_user.TryGetValue(id, out List<User> bu))
                        {
                            Data.bilibili_bangumi.bilibili_bangumi_user.TryAdd(id, new List<User>());
                        }
                        Data.bilibili_bangumi.bilibili_bangumi_user.TryGetValue(id, out bu);

                        if (bu?.Where(w => w.Qq == qq && w.Gdid == gdid).Count() == 0)
                        {
                            bu.Add(new User { Qq = qq, Gdid = gdid });
                            success_session_id.Add(id, bi);
                        }
                        else
                        {
                            exist_session_id.Add(id, bi);
                        }
                    }
                    else
                    {
                        failed_session_id.Add(id, "");
                    }
                }

                if (exist_session_id.Count > 0)
                {
                    content += $"{Environment.NewLine}已追番({string.Join(",", exist_session_id.Select(s => s.Value).ToArray())})";
                }
                if (success_session_id.Count > 0)
                {
                    content += $"{Environment.NewLine}成功追番({string.Join(",", success_session_id.Select(s => s.Value).ToArray())})";
                }
                if (failed_session_id.Count > 0)
                {
                    content += $"{Environment.NewLine}追番失败,请确认番号正确及在新番列表内({string.Join(",", failed_session_id.Select(s => s.Key).ToArray())})";
                }
                if (found_session_id.Count > 0)
                {
                    content += $"{Environment.NewLine}存在多个番剧结果,请重新输入{Environment.NewLine}{string.Join(Environment.NewLine, found_session_id.Select(s => s.Value).ToArray())}";
                }
                if (success_session_id.Count > 0 | exist_session_id.Count > 0 | failed_session_id.Count > 0 | found_session_id.Count > 0)
                {
                    _content = content;
                    DataController.Save();
                    return true;
                }
            }
            catch (Exception ex)
            {
                CQAPI.AddLog(RobotBase.CQ_AuthCode, CQAPI.LogPriority.CQLOG_ERROR, "BangumiNews.Reg", ex.ToString());
            }
            _content = "";
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
            try
            {
                string content = _Code.Code_At(qq);
                List<long> session_id = new List<long>();
                Dictionary<long, string> success_session_id = new Dictionary<long, string>();
                Dictionary<long, string> failed_session_id = new Dictionary<long, string>();

                foreach (var s in msgContent.Split(','))
                {
                    long id = -1;
                    string sid = new String(s.Where(Char.IsDigit).ToArray());
                    Int64.TryParse(sid, out id);
                    if (id > 0)
                    {
                        session_id.Add(id);
                    }
                }

                foreach (var id in session_id)
                {
                    if (Data.bilibili_bangumi.bilibili_bangumi_session.TryGetValue(id, out string bi))
                    {
                        Data.bilibili_bangumi.bilibili_bangumi_user.TryGetValue(id, out List<User> bu);

                        if (bu?.Where(w => w.Qq == qq && w.Gdid == gdid).Count() > 0)
                        {
                            bu?.Remove(bu?.Where(w => w.Qq == qq && w.Gdid == gdid).First());
                            success_session_id.Add(id, bi);
                        }
                        else
                        {
                            failed_session_id.Add(id, bi);
                        }
                        if (bu?.Count == 0) { Data.bilibili_bangumi.bilibili_bangumi_user.TryRemove(id, out List<User> tmp); RecurringJob.RemoveIfExists($"追番{id}"); }
                    }
                    else
                    {
                        failed_session_id.Add(id, "");
                    }
                }

                if (success_session_id.Count > 0)
                {
                    content += $"{Environment.NewLine}成功弃番({string.Join(",", success_session_id.ToArray())})";
                }
                if (failed_session_id.Count > 0)
                {
                    content += $"{Environment.NewLine}弃番失败,找不到该番剧在你的订阅内({string.Join(",", success_session_id.ToArray())})";
                }
                if (success_session_id.Count > 0 | failed_session_id.Count > 0)
                {
                    _content = content;
                    DataController.Save();
                    return true;
                }
            }
            catch (Exception ex)
            {
                CQAPI.AddLog(RobotBase.CQ_AuthCode, CQAPI.LogPriority.CQLOG_ERROR, "BangumiNews.UnReg", ex.ToString());
            }
            _content = "";
            return false;
        }
    }
}
