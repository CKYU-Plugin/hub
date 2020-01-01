using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Robot.API;
using Robot.Property;
using static link.toroko.rsshub.startup.Data;

namespace link.toroko.rsshub.startup
{

    public class User
    {
        public string Qq { get; set; }
        public string Gdid { get; set; }
    }

    public static class Data
    {
        public static Bilibili_bangumi bilibili_bangumi = new Bilibili_bangumi();
        public static Bilibili_up bilibili_up = new Bilibili_up();
        public static Bilibili_live bilibili_live = new Bilibili_live();
        public static Youtube youtube = new Youtube();
        public static Hangfire_config hangfire_conf = new Hangfire_config();
        public static ConcurrentDictionary<string, Action> taskObjk = new ConcurrentDictionary<string, Action>();
        public static ConcurrentQueue<Assembly_config> plugins = new ConcurrentQueue<Assembly_config>();
        public static ConcurrentDictionary<Assembly_config, object> plugins_object = new ConcurrentDictionary<Assembly_config, object>();
        public static ConcurrentDictionary<string, Assembly_store> plugins_store = new ConcurrentDictionary<string, Assembly_store>();
    }

    public class Hangfire_config
    {
        public int CronMinuteInterval = 5;
    }
    public class Assembly_config
    {
        public string UniqueCode { get; set; }
        public string Keyword { get; set; }
        public string Route { get; set; }
        public string DisplayName { get; set; }
        public string NotFoundMessage { get; set; }
        public string SuccessMessage { get; set; }
        public string ExistMessage { get; set; }
    }

    public class Assembly_store
    {
        public ConcurrentDictionary<string, string> assembly_up = new ConcurrentDictionary<string, string>();
        public ConcurrentDictionary<string, List<User>> assembly_user = new ConcurrentDictionary<string, List<User>>();
        public ConcurrentDictionary<string, string> assembly_status = new ConcurrentDictionary<string, string>();
    }

    public class Bilibili_bangumi
    {
        public ConcurrentDictionary<long, string> bilibili_bangumi_session = new ConcurrentDictionary<long, string>();
        public ConcurrentDictionary<long, List<User>> bilibili_bangumi_user = new ConcurrentDictionary<long, List<User>>();
        public ConcurrentDictionary<long, long> bilibili_bangumi_status = new ConcurrentDictionary<long, long>();
    }

    public class Bilibili_up
    {
        public ConcurrentDictionary<long, string> bilibili_up = new ConcurrentDictionary<long, string>();
        public ConcurrentDictionary<long, List<User>> bilibili_up_user = new ConcurrentDictionary<long, List<User>>();
        public ConcurrentDictionary<long, string> bilibili_up_status = new ConcurrentDictionary<long, string>();
    }

    public class Bilibili_live
    {
        public ConcurrentDictionary<long, string> bilibili_room = new ConcurrentDictionary<long, string>();
        public ConcurrentDictionary<long, List<User>> bilibili_live_user = new ConcurrentDictionary<long, List<User>>();
        public ConcurrentDictionary<long, string> bilibili_live_status = new ConcurrentDictionary<long, string>();
    }

    public class Youtube
    {
        public ConcurrentDictionary<string, string> youtube_channel = new ConcurrentDictionary<string, string>();
        public ConcurrentDictionary<string, List<User>> youtube_channel_user = new ConcurrentDictionary<string, List<User>>();
        public ConcurrentDictionary<string, string> youtube_channel_status = new ConcurrentDictionary<string, string>();
    }

    public static class DataController
    {
        private static object tasklist = new object();
        private static object bilibili_bangumi_locker = new object();
        private static object bilibili_up_locker = new object();
        private static object bilibili_live_locker = new object();
        private static object youtube_locker = new object();
        private static object hangfire_conf_locker = new object();

        public static void Load()
        {
            lock (tasklist)
            {
                try
                {
                    if (File.Exists(Path.Combine(RobotBase.appfolder, $"conf\\task.json")))
                    {
                        Data.taskObjk = JsonConvert.DeserializeObject<ConcurrentDictionary<string, Action>>(File.ReadAllText(Path.Combine(RobotBase.appfolder, $"conf\\task.json")));
                    }
                }
                catch (Exception ex) { CQAPI.AddLog(RobotBase.CQ_AuthCode, CQAPI.LogPriority.CQLOG_INFO, "文件读取", $"Json读取失败:{Environment.NewLine}{ex.ToString()}"); }
            }

            lock (hangfire_conf_locker)
            {
                try
                {
                    if (File.Exists(Path.Combine(RobotBase.appfolder, $"conf\\hangfire\\conf.json")))
                    {
                        Data.hangfire_conf = JsonConvert.DeserializeObject<Hangfire_config>(File.ReadAllText(Path.Combine(RobotBase.appfolder, $"conf\\hangfire\\conf.json")));
                    }
                }
                catch (Exception ex) { CQAPI.AddLog(RobotBase.CQ_AuthCode, CQAPI.LogPriority.CQLOG_INFO, "文件读取", $"Json读取失败:{Environment.NewLine}{ex.ToString()}"); }
            }

            lock (bilibili_bangumi_locker)
            {
                try
                {
                    if (File.Exists(Path.Combine(RobotBase.appfolder, $"data\\bilibili\\bangumi.json")))
                    {
                        Data.bilibili_bangumi = JsonConvert.DeserializeObject<Bilibili_bangumi>(File.ReadAllText(Path.Combine(RobotBase.appfolder, $"data\\bilibili\\bangumi.json")));
                    }
                }
                catch (Exception ex) { CQAPI.AddLog(RobotBase.CQ_AuthCode, CQAPI.LogPriority.CQLOG_INFO, "文件读取", $"Json读取失败:{Environment.NewLine}{ex.ToString()}"); }
            }
            lock (bilibili_up_locker)
            {
                try
                {
                    if (File.Exists(Path.Combine(RobotBase.appfolder, $"data\\bilibili\\up.json")))
                    {
                        Data.bilibili_up = JsonConvert.DeserializeObject<Bilibili_up>(File.ReadAllText(Path.Combine(RobotBase.appfolder, $"data\\bilibili\\up.json")));
                    }
                }
                catch (Exception ex) { CQAPI.AddLog(RobotBase.CQ_AuthCode, CQAPI.LogPriority.CQLOG_INFO, "文件读取", $"Json读取失败:{Environment.NewLine}{ex.ToString()}"); }
            }
            lock (bilibili_live_locker)
            {
                try
                {
                    if (File.Exists(Path.Combine(RobotBase.appfolder, $"data\\bilibili\\live.json")))
                    {
                        Data.bilibili_live = JsonConvert.DeserializeObject<Bilibili_live>(File.ReadAllText(Path.Combine(RobotBase.appfolder, $"data\\bilibili\\live.json")));
                    }
                }
                catch (Exception ex) { CQAPI.AddLog(RobotBase.CQ_AuthCode, CQAPI.LogPriority.CQLOG_INFO, "文件读取", $"Json读取失败:{Environment.NewLine}{ex.ToString()}"); }
            }
            lock (youtube_locker)
            {
                try
                {
                    if (File.Exists(Path.Combine(RobotBase.appfolder, $"data\\youtube\\clannel.json")))
                    {
                        Data.youtube = JsonConvert.DeserializeObject<Youtube>(File.ReadAllText(Path.Combine(RobotBase.appfolder, $"data\\youtube\\clannel.json")));
                    }
                }
                catch (Exception ex) { CQAPI.AddLog(RobotBase.CQ_AuthCode, CQAPI.LogPriority.CQLOG_INFO, "文件读取", $"Json读取失败:{Environment.NewLine}{ex.ToString()}"); }
            }
            DataController.Save();
        }

        public static void Save()
        {
            lock (tasklist)
            {
                try
                {
                    File.WriteAllText(Path.Combine(RobotBase.appfolder, $"conf\\task.json"), JsonConvert.SerializeObject(taskObjk));
                }
                catch (Exception ex) { CQAPI.AddLog(RobotBase.CQ_AuthCode, CQAPI.LogPriority.CQLOG_INFO, "文件写入", $"Json写入失败:{Environment.NewLine}{ex.ToString()}"); }
            }
            lock (hangfire_conf_locker)
            {
                File.WriteAllText(Path.Combine(RobotBase.appfolder, $"conf\\hangfire\\conf.json"), JsonConvert.SerializeObject(hangfire_conf));
            }
            lock (bilibili_bangumi_locker)
            {
                File.WriteAllText(Path.Combine(RobotBase.appfolder, $"data\\bilibili\\bangumi.json"), JsonConvert.SerializeObject(bilibili_bangumi));
            }
            lock (bilibili_up_locker)
            {
                File.WriteAllText(Path.Combine(RobotBase.appfolder, $"data\\bilibili\\up.json"), JsonConvert.SerializeObject(bilibili_up));
            }
            lock (bilibili_live_locker)
            {
                File.WriteAllText(Path.Combine(RobotBase.appfolder, $"data\\bilibili\\live.json"), JsonConvert.SerializeObject(bilibili_live));
            }
            lock (youtube_locker)
            {
                File.WriteAllText(Path.Combine(RobotBase.appfolder, $"data\\youtube\\clannel.json"), JsonConvert.SerializeObject(youtube));
            }
        }
    }

}
