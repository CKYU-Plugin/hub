using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using link.toroko.rsshub;
using Robot.API;
using Robot.Property;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Robot.Code;
using Robot.Extension;
using Microsoft.Owin.Hosting;
using System.Diagnostics;
using link.toroko.rsshub.Services.bilibili.task;
using link.toroko.rsshub.Services.bilibili.web_api;
using link.toroko.rsshub.OwinStartup;
using System.Runtime.ExceptionServices;
using System.Security;
using link.toroko.rsshub.startup;
using static link.toroko.rsshub.startup.Data;
using Hangfire;
using link.toroko.rsshub.Services;
using link.toroko.rsshub.Application;
using System.Drawing.Text;
using System.Drawing;
using TheArtOfDev.HtmlRenderer.WinForms;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Wpf.Data;
using Wpf.Models;

namespace link.toroko.rsshub
{
    public class Program
    {
        public static CancellationTokenSource close = new CancellationTokenSource();
        private static Dictionary<string, IServices> command_Reg = new Dictionary<string, IServices>();
        private static Dictionary<string, IServices> command_UnReg = new Dictionary<string, IServices>();
        private static Dictionary<string, IServices> command_List = new Dictionary<string, IServices>();
        private static Dictionary<string, IServices> command_Status = new Dictionary<string, IServices>();
        private static Dictionary<string, IServices> command_Start = new Dictionary<string, IServices>();
        private static Dictionary<string, IServices> command_Stop = new Dictionary<string, IServices>();
        public static ConcurrentDictionary<string, RequestArgs> requestNextList = new ConcurrentDictionary<string, RequestArgs>();
        public static ConcurrentDictionary<string, RequestArgs> requestBeforeList = new ConcurrentDictionary<string, RequestArgs>();
        public static ConcurrentDictionary<string, RequestArgs> requestOnceUnReg = new ConcurrentDictionary<string, RequestArgs>();
        public static ConcurrentDictionary<string, RequestArgs> pandingEnd = new ConcurrentDictionary<string, RequestArgs>();
        public static ConcurrentDictionary<string, RequestArgs> selected = new ConcurrentDictionary<string, RequestArgs>();


        public class RequestArgs
        {
            
            public IServices Service { get; set; }
            public Dictionary<string, object> Args { get; set; }
            public string Content { get; set; }
        }

        public static void Init()
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    Startup.Setup();
                    Startup.Setup_OutPut();
                    Startup.Setup_LoadData();
                    Startup.Setup_bangumi();
                    Startup.Setup_services();
                    Startup.Setup_TorrentClient();
                    Startup.Setup_RunTask();
                    CommandList();
                    RobotBase.isinit = true;
                }
                catch (Exception ex)
                {
                    CQAPI.AddLog(RobotBase.CQ_AuthCode, CQAPI.LogPriority.CQLOG_ERROR, "Init", $"{ex.ToString()}");
                }
            }).Wait();
        }

        public static void CommandList()
        {
            command_List = new Dictionary<string, IServices>();
            command_Reg = new Dictionary<string, IServices>();
            command_UnReg = new Dictionary<string, IServices>();
            command_Status = new Dictionary<string, IServices>();
            command_Start = new Dictionary<string, IServices>();
            command_Stop = new Dictionary<string, IServices>();

            command_List.Add("已訂閱YOUTUBE頻道", new YoutubeChannel());
            command_UnReg.Add("退訂YOUTUBE頻道", new YoutubeChannel());

            command_List.Add("已訂閱B站UP主", new BilibiliUp());
            command_Reg.Add("關注B站UP主", new BilibiliUp());
            command_UnReg.Add("取關B站UP主", new BilibiliUp());

            command_List.Add("已訂閱B站開播", new BilibiliLive());
            command_UnReg.Add("退訂B站開播", new BilibiliLive());

            command_List.Add("已訂閱番劇", new BangumiNews());

            command_Reg.Add("訂閱番劇下載", new Mikananime());

            command_Reg.Add("追番", new BangumiNews());
            command_UnReg.Add("棄番", new BangumiNews());

            command_List.Add("查新番", new BangumiInfo());
            command_List.Add("DMHY搜番劇", new DmhyBangumiInfo());
            command_List.Add("搜番劇", new MikanInfo());
            command_List.Add("搜度盤", new BaiduPanSearch());

            command_List.Add("下載列表", new TorrentClient());
            command_Reg.Add("種子下載", new TorrentClient());
            command_UnReg.Add("移除種子下載", new TorrentClient());
            command_Status.Add("下載狀態", new TorrentClient());

            command_Start.Add("打開BT", new TorrentClient());
            command_Stop.Add("關閉BT", new TorrentClient());
            command_List.Add(ViewModelData.g.BEnable_Prefix ? "[CQ:share,url=http://www.rr-sc.com/".ToUpper() : "_搜人人素材", new SucaiSearch());
            command_List.Add(ViewModelData.g.BEnable_Prefix ? "[CQ:share,url=https://www.rr-sc.com/".ToUpper() : "搜人人素材", new SucaiSearch());
            command_List.Add("搜素材", new SucaiSearch3());
            command_List.Add("搜資源", new EzhanzySearch());
            command_Status.Add("素材剩餘次數", new SucaiSearch3());
			command_Reg.Add("加載KEY", new SucaiSearch3());


			if (ViewModelData.g.BEnable_Prefix)
            {
                command_List.Add("https://www.rr-sc.com/".ToUpper(), new SucaiSearch());
                command_List.Add("http://www.rr-sc.com/".ToUpper(), new SucaiSearch());
            }

            command_Reg.Add(ViewModelData.g.BEnable_Prefix ? "YOUTUBE頻道訂閱" : "訂閱YOUTUBE頻道", new YoutubeChannel());
            command_Reg.Add(ViewModelData.g.BEnable_Prefix ? "B站開播訂閱" : "訂閱B站開播", new BilibiliLive());
        }

        public static void Main(string robotQQ, Int32 msgType, Int32 msgSubType, string msgSrc, string targetActive, string targetPassive, string msgContent, int messageid)
        {
            try
            {
                if (!RobotBase.isinit && !RobotBase.isenableplugin) { return; }
                string content = _Code.Code_At(msgSrc) + Environment.NewLine;

                //if (msgContent.Trim().ToUpper().StartsWith("G"))
                //{
                //    string Qqid = new String(msgContent.Where(Char.IsDigit).ToArray());
                //    _API.SendMessage(msgSrc, msgType, _API.GetNickName(Qqid), targetActive, robotQQ, msgSubType);
                //}

                if (msgContent.ToLower().StartsWith("^") | msgContent.ToLower().StartsWith("︿"))
                {
                    if (selected.TryRemove(msgSrc, out RequestArgs s))
                    {
                        if (!s.Service.List(msgSrc, targetActive, msgContent.Substring(1), messageid, out string _content, RobotBase.ispro, s.Args))
                        {
                            selected.TryAdd(msgSrc, s);
                        }
                        if (!String.IsNullOrEmpty(_content))
                        {
                            _API.SendMessage(msgSrc, msgType, _content, targetActive, robotQQ, msgSubType);
                        }
                        return;
                    }
                }

                if (msgContent.ToLower().StartsWith("<") | msgContent.ToLower().StartsWith("＜"))
                {
                    if (requestBeforeList.TryRemove(msgSrc, out RequestArgs s))
                    {
                        s.Service.List(msgSrc, targetActive, msgContent.Substring(1), messageid, out string _content, RobotBase.ispro, s.Args);
                        _API.SendMessage(msgSrc, msgType, _content, targetActive, robotQQ, msgSubType);
                        return;
                    }
                }

                if (msgContent.ToLower().StartsWith(">") | msgContent.ToLower().StartsWith("＞"))
                {
                    if (requestNextList.TryRemove(msgSrc, out RequestArgs s))
                    {
                        s.Service.List(msgSrc, targetActive, msgContent.Substring(1), messageid, out string _content, RobotBase.ispro, s.Args);
                        _API.SendMessage(msgSrc, msgType, _content, targetActive, robotQQ, msgSubType);
                        return;
                    }
                }

                if (requestOnceUnReg.Count > 0)
                {
                    lock (requestOnceUnReg)
                    {
                        requestOnceUnReg.ToList().ForEach(f =>
                        {
                            requestOnceUnReg.TryGetValue(f.Key, out RequestArgs s);
                            s.Service.UnReg(msgSrc, targetActive, msgContent.Substring(1), messageid, out string _content, s.Args);
                            _API.SendMessage(msgSrc, msgType, _content, targetActive, robotQQ, msgSubType);
                        });
                        requestOnceUnReg.Clear();
                    }
                }

                //if (msgContent.ToLower().StartsWith("/test"))
                //{
                //    content += msgContent.Replace("&#91;", "[").Replace("&#93;", "]");
                //    _API.SendMessage(msgSrc, msgType, content, targetActive, robotQQ, msgSubType);
                //    return;
                //}

                //if (msgContent.ToLower().StartsWith("/shorttest"))
                //{
                //    content += Services.Ft12.Ft12.GetShort("http://xiaobai.xssvip.cn/download?p=eyJhdHRyIjp7ImRhdGEiOiIxNDE1OTgwIiwiY2xhc3MiOiJkb3duLWJpZy1pbWcgZG93bmxvYWQtYnRuIHB1YmxpYy1idy1kdyIsImRhdGEtZnJvbSI6IiJ9LCJ0aW1lc3RhbXAiOjE1NDAxNzY3NzEsInNvdXJjZVVybCI6Imh0dHA6XC9cLzU4OGt1LmNvbVwvc3VjYWlcLzE0MTU5ODAuaHRtbCIsImFzc2V0VHlwZSI6IjU4OGt1In0&k=21a453569cbf775a87f62be539090434ec0baf7677d5c9971167636530f1d680");
                //    content += Environment.NewLine;
                //    content += Services.Ft12.Ft12.GetShort(msgContent.Replace("/shorttest", ""));
                //    content += Environment.NewLine;
                //    content += Services.T_im.T_im.GetShort("http://xiaobai.xssvip.cn/download?p=eyJhdHRyIjp7ImRhdGEiOiIxNDE1OTgwIiwiY2xhc3MiOiJkb3duLWJpZy1pbWcgZG93bmxvYWQtYnRuIHB1YmxpYy1idy1kdyIsImRhdGEtZnJvbSI6IiJ9LCJ0aW1lc3RhbXAiOjE1NDAxNzY3NzEsInNvdXJjZVVybCI6Imh0dHA6XC9cLzU4OGt1LmNvbVwvc3VjYWlcLzE0MTU5ODAuaHRtbCIsImFzc2V0VHlwZSI6IjU4OGt1In0&k=21a453569cbf775a87f62be539090434ec0baf7677d5c9971167636530f1d680");
                //    content += Environment.NewLine;
                //    content += Services.T_im.T_im.GetShort(msgContent.Replace("/shorttest", ""));
                //    _API.SendMessage(msgSrc, msgType, content, targetActive, robotQQ, msgSubType);
                //    return;
                //}

                if (msgContent.ToLower().Contains("/hub"))
                {
                    content += "新番列表" + Environment.NewLine;
                    content += "以圖搜番" + Environment.NewLine;
                    content += string.Join(Environment.NewLine, command_List.Where(w => !w.Key.StartsWith("[CQ") & !w.Key.StartsWith("HTTP")).OrderByDescending(o => o.Key).Select(s => s.Key)) + Environment.NewLine;
                    content += string.Join(Environment.NewLine, command_Reg.Select(s => s.Key)) + Environment.NewLine;
                    content += string.Join(Environment.NewLine, command_UnReg.Select(s => s.Key)) + Environment.NewLine;
                    content += string.Join(Environment.NewLine, command_Status.Select(s => s.Key)) + Environment.NewLine;
                    _API.SendMessage(msgSrc, msgType, content, targetActive, robotQQ, msgSubType);
                    return;
                }

                foreach (var t in command_List)
                {
                    if (msgContent.Trim().ToUpper().StartsWith($"#{t.Key}"))
                    {
                        if (t.Value.List(msgSrc, targetActive, msgContent.Substring(1).Replace(t.Key, ""), messageid, out string _content, RobotBase.ispro))
                        {
                            _API.SendMessage(msgSrc, msgType, _content, targetActive, robotQQ, msgSubType);
                        }
                        return;
                    }
                    if (msgContent.Trim().ToUpper().StartsWith(t.Key))
                    {

                        Regex r = new Regex(Regex.Escape(t.Key), RegexOptions.IgnoreCase);
                        bool ManyRequest = false;
                        if (t.Value.List(msgSrc, targetActive, r.Replace(msgContent, "", 1), messageid, out string _content))
                        {
                            _API.SendMessage(msgSrc, msgType, _content, targetActive, robotQQ, msgSubType);
                        }
                        if (pandingEnd.TryGetValue(msgSrc, out RequestArgs tmp_RA)) { ManyRequest = true; }
                        while (pandingEnd.TryRemove(msgSrc, out RequestArgs RA))
                        {
                            r = new Regex(Regex.Escape(t.Key), RegexOptions.IgnoreCase);
                            RA.Service.List(msgSrc, targetActive, RA.Content != "" ? RA.Content : r.Replace(msgContent, "", 1), messageid, out string _content1, RobotBase.ispro, RA.Args);
                            _API.SendMessage(msgSrc, msgType, _content1, targetActive, robotQQ, msgSubType);
                        }

                        if (ManyRequest)
                        {
                            content += "已執行所有操作";
                            _API.SendMessage(msgSrc, msgType, content, targetActive, robotQQ, msgSubType);
                        }
                        return;
                    }
                }

                foreach (var t in command_Start)
                {
                    if (msgContent.Trim().ToUpper().StartsWith(t.Key))
                    {
                        t.Value.Start(msgSrc, targetActive, msgContent.Replace(t.Key, ""), messageid, out string _content);
                        _API.SendMessage(msgSrc, msgType, _content, targetActive, robotQQ, msgSubType);
                        return;
                    }
                }

                foreach (var t in command_Stop)
                {
                    if (msgContent.Trim().ToUpper().StartsWith(t.Key))
                    {
                        t.Value.Stop(msgSrc, targetActive, msgContent.Replace(t.Key, ""), messageid, out string _content);
                        _API.SendMessage(msgSrc, msgType, _content, targetActive, robotQQ, msgSubType);
                        return;
                    }
                }

                foreach (var t in command_Status)
                {
                    if (msgContent.Trim().ToUpper().StartsWith(t.Key))
                    {
                        if (t.Value.Status(msgSrc, targetActive, msgContent.Replace(t.Key, ""), messageid, out string _content))
                        {
                            _API.SendMessage(msgSrc, msgType, _content, targetActive, robotQQ, msgSubType);
                        }
                        return;
                    }
                }

                foreach (var t in command_Reg)
                {
                    if (msgContent.Trim().ToUpper().StartsWith(t.Key))
                    {
                        if (t.Value.Reg(msgSrc, targetActive, msgContent.Replace(t.Key, ""), messageid, out string _content))
                        {
                            _API.SendMessage(msgSrc, msgType, _content, targetActive, robotQQ, msgSubType);
                        }
                        return;
                    }
                }

                foreach (var t in command_UnReg)
                {
                    if (msgContent.Trim().ToUpper().StartsWith(t.Key))
                    {
                        if (t.Value.UnReg(msgSrc, targetActive, msgContent.Replace(t.Key, ""), messageid, out string _content))
                        {
                            _API.SendMessage(msgSrc, msgType, _content, targetActive, robotQQ, msgSubType);
                        }
                        return;
                    }
                }
                Javsearch.Run(robotQQ, msgType, msgSubType, msgSrc, targetActive, targetPassive, msgContent, messageid);
                BangumiList.Run(robotQQ, msgType, msgSubType, msgSrc, targetActive, targetPassive, msgContent, messageid);
                Animesearch.Run(robotQQ, msgType, msgSubType, msgSrc, targetActive, targetPassive, msgContent, messageid);
            }catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

    }

}
