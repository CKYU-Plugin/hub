using System;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Robot.API;
using Robot.Property;
using link.toroko.rsshub;
using Wpf;
using System.Collections.Concurrent;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Diagnostics;
using System.IO;

namespace Robot
{
    public class CQ
    {
        private static ConcurrentQueue<Action> startupAction = new ConcurrentQueue<Action>();
        private static Task initializeTask = null;

        [DllExport("AppInfo", CallingConvention = CallingConvention.StdCall)]
        public static string AppInfo()
        {
            return string.Concat(RobotBase.ApiVer, ",", RobotBase.AppID);
        }

        [HandleProcessCorruptedStateExceptions, SecurityCritical]
        [DllExport("Initialize", CallingConvention = CallingConvention.StdCall)]
        public static Int32 Initialize(int AuthCode)
        {
            RobotBase.robot = RobotProperty.RobotType.CQ;
            RobotBase.CQ_AuthCode = AuthCode;
            initializeTask = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    SpinWait.SpinUntil(() => startupAction.Count != 0);
                    SpinWait.SpinUntil(() => false, 100);
                    if (startupAction.TryDequeue(out Action act))
                    {
                        try
                        {
                            act?.Invoke();
                        }
                        catch (Exception ex)
                        {
                            CQAPI.AddLog(RobotBase.CQ_AuthCode, CQAPI.LogPriority.CQLOG_ERROR, "Initialize", $"{ex.ToString()}");
                        }
                    }
                }
            }, TaskCreationOptions.LongRunning);
            return 0;
        }

        [DllExport("_eventStartup", CallingConvention = CallingConvention.StdCall)]
        public static Int32 AppStartup()
        {
            return 0;
        }

        [DllExport("_eventExit", CallingConvention = CallingConvention.StdCall)]
        public static Int32 AppExit()
        {
            Main.Close();
            return 0;
        }

        [HandleProcessCorruptedStateExceptions, SecurityCritical]
        [DllExport("_eventEnable", CallingConvention = CallingConvention.StdCall)]
        public static Int32 AppEnable()
        {
            if (initializeTask.Status != TaskStatus.Running) { return 1; }
            try
            {
                RobotBase.LoginQQ = CQAPI.GetLoginQQ(RobotBase.CQ_AuthCode).ToString();
                RobotBase.appfolder = CQAPI.GetCQAppFolder(RobotBase.CQ_AuthCode);
                RobotBase.currentfloder = Directory.GetCurrentDirectory();
                RobotBase.ispro = Process.GetCurrentProcess().MainModule.FileVersionInfo.FileDescription == "酷Q Pro";
            }
            catch(Exception ex)
            {
                CQAPI.AddLog(RobotBase.CQ_AuthCode, CQAPI.LogPriority.CQLOG_ERROR, "_eventEnable", $"{ex.ToString()}");
            }
            startupAction.Enqueue(() =>
            {
                Main.Init();
            });
            return 0;
        }

        [HandleProcessCorruptedStateExceptions, SecurityCritical]
        [DllExport("_eventDisable", CallingConvention = CallingConvention.StdCall)]
        public static Int32 AppDisable()
        {
            startupAction.Enqueue(() =>
            {
                try
                {
                    Main.Disable();
                }
                catch (Exception ex)
                {
                    CQAPI.AddLog(RobotBase.CQ_AuthCode, CQAPI.LogPriority.CQLOG_ERROR, "_eventDisable", $"{ex.ToString()}");
                }
            });
            return 0;
        }

        [DllExport("_eventPrivateMsg", CallingConvention = CallingConvention.StdCall)]
        public static Int32 PrivateMessage(int subType, int msgId, long fromQQ, string msg, int font)
        {
            Main.Run(CQAPI.GetLoginQQ(RobotBase.CQ_AuthCode).ToString(), 1, subType, fromQQ.ToString(), fromQQ.ToString(), fromQQ.ToString(), msg, msgId);
            return RobotBase.blockallmessages ? 1 : 0;
        }

        [DllExport("_eventGroupMsg", CallingConvention = CallingConvention.StdCall)]
        public static Int32 GroupMessage(int subType, int msgId, long fromGroup, long fromQQ, string fromAnonymous, string msg, int font)
        {
            Main.Run(CQAPI.GetLoginQQ(RobotBase.CQ_AuthCode).ToString(), 2, subType, fromQQ.ToString(), fromGroup.ToString(), fromQQ.ToString(), msg, msgId);
            return RobotBase.blockallmessages ? 1 : 0;
        }

        [DllExport("_eventDiscussMsg", CallingConvention = CallingConvention.StdCall)]
        public static Int32 DiscussMessage(int subType, int msgId, long fromDiscuss, long fromQQ, string msg, int font)
        {
            Main.Run(CQAPI.GetLoginQQ(RobotBase.CQ_AuthCode).ToString(), 3, subType, fromQQ.ToString(), fromDiscuss.ToString(), fromQQ.ToString(), msg, msgId);
            return RobotBase.blockallmessages ? 1 : 0;
        }

        [DllExport("_eventGroupUpload", CallingConvention = CallingConvention.StdCall)]
        public static Int32 GroupUpload(int subType, int sendTime, long fromGroup, long fromQQ, string file)
        {
            // 處理群文件上傳事件。
            //  CQ.SendGroupMessage(fromGroup, String.Format("[{0}]{1}你上傳了一個文件：{2}", CQ.ProxyType, CQ.CQCode_At(fromQQ), file));
            return 0;
        }

        [DllExport("_eventSystem_GroupAdmin", CallingConvention = CallingConvention.StdCall)]
        public static Int32 GroupAdmin(int subType, int sendTime, long fromGroup, long beingOperateQQ)
        {
            // 處理群事件-管理員變動。
            //CQ.SendGroupMessage(fromGroup, String.Format("[{0}]{2}({1})被{3}管理員權限。", CQ.ProxyType, beingOperateQQ, CQ.GetQQName(beingOperateQQ), subType == 1 ? "取消了" : "設置為"));
            return 0;
        }

        [DllExport("_eventSystem_GroupMemberDecrease", CallingConvention = CallingConvention.StdCall)]
        public static Int32 GroupMemberDecrease(int subType, int sendTime, long fromGroup, long fromQQ, long beingOperateQQ)
        {
            // 處理群事件-群成員減少。
            //CQ.SendGroupMessage(fromGroup, String.Format("[{0}]群員{2}({1}){3}", CQ.ProxyType, beingOperateQQ, CQ.GetQQName(beingOperateQQ), subType == 1 ? "退群。" : String.Format("被{0}({1})踢除。", CQ.GetQQName(fromQQ), fromQQ)));
            return 0;
        }

        [DllExport("_eventSystem_GroupMemberIncrease", CallingConvention = CallingConvention.StdCall)]
        public static Int32 GroupMemberIncrease(int subType, int sendTime, long fromGroup, long fromQQ, long beingOperateQQ)
        {
            // 處理群事件-群成員增加。
            //CQ.SendGroupMessage(fromGroup, String.Format("[{0}]群裡來了新人{2}({1})，管理員{3}({4}){5}", CQ.ProxyType, beingOperateQQ, CQ.GetQQName(beingOperateQQ), CQ.GetQQName(fromQQ), fromQQ, subType == 1 ? "同意。" : "邀請。"));
            return 0;
        }

        [DllExport("_eventFriend_Add", CallingConvention = CallingConvention.StdCall)]
        public static Int32 FriendAdded(int subType, int sendTime, long fromQQ)
        {
            // 處理好友事件-好友已添加。
            // CQ.SendPrivateMessage(fromQQ, String.Format("[{0}]你好，我的朋友！", CQ.ProxyType));
            return 0;
        }

        [DllExport("_eventRequest_AddFriend", CallingConvention = CallingConvention.StdCall)]
        public static Int32 RequestAddFriend(int subType, int sendTime, long fromQQ, string msg, string responseFlag)
        {
            // 處理請求-好友添加。
            // CQ.SetFriendAddRequest(responseFlag, CQReactType.Allow, "新來的朋友");
            return 0;
        }

        [DllExport("_eventRequest_AddGroup", CallingConvention = CallingConvention.StdCall)]
        public static Int32 RequestAddGroup(int subType, int sendTime, long fromGroup, long fromQQ, string msg, string responseFlag)
        {
            // 處理請求-群添加。
            // CQ.SetGroupAddRequest(responseFlag, CQRequestType.GroupAdd, CQReactType.Allow, "新群友");
            return 0;
        }

        [DllExport("_menuA", CallingConvention = CallingConvention.Cdecl)]
        public static Int32 _menuA()
        {
            return 0;
        }

        [DllExport("_menuB", CallingConvention = CallingConvention.Cdecl)]
        public static Int32 _menuB()
        {
            return 0;
        }

        private static MainWindow mw = new MainWindow();
        [DllExport("_set", CallingConvention = CallingConvention.Cdecl)]
        public static Int32 Set()
        {
            try
            {
                if (!mw.IsActive)
                {
                    mw = new MainWindow();
                }
                mw.ShowDialog();
            }
            catch
            {
            }
            return 0;
        }

        [DllExport("_about", CallingConvention = CallingConvention.Cdecl)]
        public static Int32 About()
        {
            return 0;
        }
    }
}
