using System;
using System.IO;
using static Robot.Property.RobotProperty;

namespace Robot.Property
{
    public static class RobotBase
    {
        public static RobotType robot = RobotType.CQ;
        public static int CQ_AuthCode = 0;
        public static string PluginName = "rsshub";
        public static string ApiVer = "9";
        public static string AppID = "link.toroko.rsshub";
        public static string AppName = "rsshub";
        public static string LoginQQ = "";
        public static string AdminQQ = "";
        public static bool ispro = false;
        public static bool isinit = false;
        public static bool isinitFileMonitor = false;
        public static bool blockallmessages = false;
        public static bool isenableplugin = false;
        public static string appfolder = "";
        public static string currentfloder = "";
        public static string iniconf = "conf\\init.ini";
        public static string SauceNAOApiToken = "";
        public static string WhatAnimeApiToken = "";
        public static string path_rrsc = "data\\filesystem\\Rrsc\\";
        public static string path_torrents = "data\\filesystem\\Mono\\Torrents";
        public static string path_downloads = "data\\filesystem\\Mono\\Downloads";
        public static string path_dhtNodeFile = "data\\filesystem\\Mono\\DhtNodes";
        public static string path_fastResumeFile = "data\\filesystem\\Mono\\fastresume.data";
    }

    public static class RobotProperty
    {
        public enum RobotType : int
        {
            Test = -1,
            CQ = 0,
            MPQ = 1,
            Amanda = 2,
            IRQQ = 3,
            Huajing = 4,
            QY = 5
        }
    }
}
