using link.toroko.rsshub;
using link.toroko.rsshub.Services;
using link.toroko.rsshub.startup;
using Robot.API;
using Robot.Property;
using System;
using System.Diagnostics;
namespace Robot
{
    public static class Main
    {
        private static object enablelocker = new object();

        public static void Run(string robotQQ, Int32 msgType, Int32 msgSubType, string msgSrc, string targetActive, string targetPassive, string msgContent, int messageid)
        {
            if (!RobotBase.isinit) { return; }
            if (!RobotBase.isenableplugin) { return; }
            Program.Main(robotQQ, msgType, msgSubType, msgSrc, targetActive, targetPassive, msgContent, messageid);
        }

        public static void Init()
        {
            Stopwatch sw = new Stopwatch();
            CQAPI.AddLog(RobotBase.CQ_AuthCode, CQAPI.LogPriority.CQLOG_DEBUG, $"初始化", "開始加載");
            sw.Start();
            RobotBase.isenableplugin = true;
            Program.Init();
            sw.Stop();
            CQAPI.AddLog(RobotBase.CQ_AuthCode, CQAPI.LogPriority.CQLOG_DEBUG, $"初始化", $"加載完成({sw.ElapsedMilliseconds}ms)");
            sw = null;
        }

        public static void Close()
        {
            try
            {
                RobotBase.isenableplugin = false;
                Program.close.Cancel();
            }
            catch { }
        }

        public static void Disable()
        {
            RobotBase.isenableplugin = false;
            TorrentClient.Stop();
            Startup.Setup_services_stop();
        }
    }
}
