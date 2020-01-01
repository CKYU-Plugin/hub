using System;
using System.Runtime.InteropServices;

namespace Robot.API
{
    public class MPQMessageAPI
    {
        /// <summary>
        /// 根據提交的QQ號計算得到頁面操作用參數Bkn或G_tk
        /// </summary>
        /// <param name="qq">響應的QQ</param>
        [DllImport("Message.dll", EntryPoint = "Api_GetGtk_Bkn", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern string Api_GetGtk_Bkn(string qq);
        /// <summary>
        /// 根據提交的QQ號計算得到頁面操作用參數長Bkn或長G_tk
        /// </summary>
        /// <param name="qq">響應的QQ</param>
        [DllImport("Message.dll", EntryPoint = "Api_GetBkn32", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern string Api_GetBkn32(string qq);
        /// <summary>
        /// 根據提交的QQ號計算得到頁面操作用參數長Ldw
        /// </summary>
        /// <param name="qq">響應的QQ</param>
        [DllImport("Message.dll", EntryPoint = "Api_GetLdw", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern string Api_GetLdw(string qq);
        /// <summary>
        /// 取得框架所在目錄.可能雞肋了。
        /// </summary>
        [DllImport("Message.dll", EntryPoint = "Api_GetRunPath", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern string Api_GetRunPath();
        /// <summary>
        /// 取得當前框架內在線可用的QQ列表
        /// </summary>
        [DllImport("Message.dll", EntryPoint = "Api_GetOnlineQQlist", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern string Api_GetOnlineQQlist();
        /// <summary>
        /// 取得框架內所有QQ列表。包括未登錄以及登錄失敗的QQ
        /// </summary>
        [DllImport("Message.dll", EntryPoint = "Api_GetQQlist", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern string Api_GetQQlist();
        /// <summary>
        /// 根據QQ取得對應的會話秘鑰
        /// </summary>
        /// <param name="qq">響應的QQ</param>
        [DllImport("Message.dll", EntryPoint = "Api_GetSessionkey", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern string Api_GetSessionkey(string qq);
        /// <summary>
        /// 取得頁面登錄用的Clientkey
        /// </summary>
        /// <param name="qq">響應的QQ</param>
        [DllImport("Message.dll", EntryPoint = "Api_GetClientkey", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern string Api_GetClientkey(string qq);
        /// <summary>
        /// 取得頁面登錄用的長Clientkey
        /// </summary>
        /// <param name="qq">響應的QQ</param>
        [DllImport("Message.dll", EntryPoint = "Api_GetLongClientkey", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern string Api_GetLongClientkey(string qq);
        /// <summary>
        /// 取得頁面操作用的Cookies
        /// </summary>
        /// <param name="qq">響應的QQ</param>
        [DllImport("Message.dll", EntryPoint = "Api_GetCookies", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern string Api_GetCookies(string qq);
        /// <summary>
        /// 取得框架內設置的信息發送前綴
        /// </summary>
        [DllImport("Message.dll", EntryPoint = "Api_GetPrefix", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern string Api_GetPrefix();
        /// <summary>
        /// 將群名片加入高速緩存當作.
        /// </summary>
        /// <param name="gid">群號</param>
        /// <param name="QQ">QQ</param>
        /// <param name="gnamecard">名片</param>
        [DllImport("Message.dll", EntryPoint = "Api_Cache_NameCard", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern void Api_Cache_NameCard(string gid, string QQ, string gnamecard);
        /// <summary>
        /// 將指定QQ移出QQ黑名單
        /// </summary>
        /// <param name="qq">響應的QQ</param>
        /// <param name="QQ">QQ</param>
        [DllImport("Message.dll", EntryPoint = "Api_DBan", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern void Api_DBan(string qq, string QQ);
        /// <summary>
        /// 將指定QQ列入QQ黑名單
        /// </summary>
        /// <param name="qq">響應的QQ</param>
        /// <param name="QQ">QQ</param>
        [DllImport("Message.dll", EntryPoint = "Api_Ban", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern void Api_Ban(string qq, string QQ);
        /// <summary>
        /// 禁言群成員
        /// </summary>
        /// <param name="qq">響應的QQ</param>
        /// <param name="gid">群號 禁言對象所在群.</param>
        /// <param name="QQ">QQ 禁言對象.留空為全群禁言</param>
        /// <param name="time">時長  單位:秒 最大為1個月. 為零解除對象或全群禁言</param>
        [DllImport("Message.dll", EntryPoint = "Api_Shutup", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool Api_Shutup(string qq, string gid, string QQ, int time);
        /// <summary>
        /// 根據群號+QQ判斷指定群員是否被禁言  獲取失敗的情況下亦會返回假
        /// </summary>
        /// <param name="qq">響應的QQ</param>
        /// <param name="gid">欲判斷對象所在群.</param>
        /// <param name="QQ">欲判斷對象</param>
        [DllImport("Message.dll", EntryPoint = "Api_IsShutup", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool Api_IsShutup(string qq, string gid, string QQ);
        /// <summary>
        /// 發群公告
        /// </summary>
        /// <param name="qq">響應的QQ</param>
        /// <param name="gid">群號</param>
        /// <param name="title">標題</param>
        /// <param name="content">內容</param>
        [DllImport("Message.dll", EntryPoint = "Api_SetNotice", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern void Api_SetNotice(string qq, string gid, string title, string content);
        /// <summary>
        /// 取群公告
        /// </summary>
        /// <param name="qq">響應的QQ</param>
        /// <param name="gid">群號</param>
        [DllImport("Message.dll", EntryPoint = "Api_GetNotice", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern string Api_GetNotice(string qq, string gid);
        /// <summary>
        /// 取群名片
        /// </summary>
        /// <param name="qq">響應的QQ</param>
        /// <param name="gid">群號</param>
        /// <param name="QQ">QQ</param>
        [DllImport("Message.dll", EntryPoint = "Api_GetNameCard", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern string Api_GetNameCard(string qq, string gid, string QQ);
        /// <summary>
        /// 設置群名片
        /// </summary>
        /// <param name="qq">響應的QQ</param>
        /// <param name="gid">群號</param>
        /// <param name="QQ">QQ</param>
        /// <param name="namecard">名片</param>
        [DllImport("Message.dll", EntryPoint = "Api_SetNameCard", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern void Api_SetNameCard(string qq, string gid, string QQ, string namecard);
        /// <summary>
        /// 退出討論組
        /// </summary>
        /// <param name="qq">響應的QQ</param>
        /// <param name="did">討論組ID</param>
        [DllImport("Message.dll", EntryPoint = "Api_QuitDG", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern void Api_QuitDG(string qq, string did);
        /// <summary>
        /// 刪除好友
        /// </summary>
        /// <param name="qq">響應的QQ</param>
        /// <param name="QQ">QQ</param>
        [DllImport("Message.dll", EntryPoint = "Api_DelFriend", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool Api_DelFriend(string qq, string QQ);
        /// <summary>
        /// 將對象移除群
        /// </summary>
        /// <param name="qq">響應的QQ</param>
        /// <param name="gid">群號</param>
        /// <param name="tqq">對象</param>
        [DllImport("Message.dll", EntryPoint = "Api_Kick", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool Api_Kick(string qq, string gid, string tqq);
        /// <summary>
        /// 主動加群.為了避免廣告、群發行為。出現驗證碼時將會直接失敗不處理
        /// </summary>
        /// <param name="qq">響應的QQ</param>
        /// <param name="gid">群號</param>
        /// <param name="remark">附加理由</param>
        [DllImport("Message.dll", EntryPoint = "Api_JoinGroup", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern void Api_JoinGroup(string qq, string gid, string remark);
        /// <summary>
        /// 退出群
        /// </summary>
        /// <param name="qq">響應的QQ</param>
        /// <param name="gid">群號</param>
        [DllImport("Message.dll", EntryPoint = "Api_QuitGroup", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern void Api_QuitGroup(string qq, string gid);
        /// <summary>
        /// 返回值:成功返回圖片GUID用於發送該圖片.失敗返回空.  圖片尺寸應小於4MB
        /// </summary>
        /// <param name="qq">機器人QQ</param>
        /// <param name="uploadtype">1好友2群 注:好友圖和群圖的GUID並不相同並不通用 需要非別上傳。群、討論組用類型2 臨時會話、好友信息需要類型1</param>
        /// <param name="QQ">上傳該圖片所屬的群號或QQ</param>
        /// <param name="img">圖片字節集數據或字節集數據指針()</param>
        [DllImport("Message.dll", EntryPoint = "Api_UploadPic", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern string Api_UploadPic(string qq, int uploadtype, string QQ, byte[] img);
        /// <summary>
        /// 根據圖片GUID取得圖片下載連接 失敗返回空
        /// </summary>
        /// <param name="guid">{xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx}.jpg這樣的GUID</param>
        [DllImport("Message.dll", EntryPoint = "Api_GuidGetPicLink", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern string Api_GuidGetPicLink(string guid);
        /// <summary>
        /// 向對象、目標發送信息 支持好友 群 討論組 群臨時會話 討論組臨時會話
        /// </summary>
        /// <param name="qq"></param>
        /// <param name="msgtype">1好友 2群 3討論組 4群臨時會話 5討論組臨時會話</param>
        /// <param name="subtype">無特殊說明情況下留空或填零</param>
        /// <param name="gdid">發送群信息、討論組信息、群臨時會話信息、討論組臨時會話信息時填寫</param>
        /// <param name="QQ">最終接收這條信息的對象QQ</param>
        /// <param name="content">信息內容</param>
        [DllImport("Message.dll", EntryPoint = "Api_SendMsg", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int Api_SendMsg(string qq, int msgtype, int subtype, string gdid, string QQ, string content);
        /// <summary>
        /// 向服務器直接發送一個加密封裝完成後的封包。成功返回服務器回傳加密後的響應包體。失敗或超時返回空
        /// </summary>
        /// <param name="package">封包內容</param>
        [DllImport("Message.dll", EntryPoint = "Api_Send", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern string Api_Send(string package);
        /// <summary>
        /// 在框架記錄頁輸出一行信息
        /// </summary>
        /// <param name="content">輸出的內容</param>
        [DllImport("Message.dll", EntryPoint = "Api_OutPut", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int Api_OutPut(string content);
        /// <summary>
        /// 取得本插件啟用狀態
        /// </summary>
        [DllImport("Message.dll", EntryPoint = "Api_IsEnable", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool Api_IsEnable();
        /// <summary>
        /// 登錄一個現存的QQ
        /// </summary>
        /// <param name="QQ">欲登錄的Q</param>
        [DllImport("Message.dll", EntryPoint = "Api_Login", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool Api_Login(string QQ);
        /// <summary>
        /// 讓指定QQ下線
        /// </summary>
        /// <param name="QQ">QQ</param>
        [DllImport("Message.dll", EntryPoint = "Api_Logout", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern void Api_Logout(string QQ);
        /// <summary>
        /// tean加密算法
        /// </summary>
        /// <param name="encodeString">加密內容</param>
        /// <param name="Key">Key</param>
        [DllImport("Message.dll", EntryPoint = "Api_Tea加密", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern string Api_Tea加密(string encodeString, string Key);
        /// <summary>
        /// tean解密算法
        /// </summary>
        /// <param name="decryptString">解密內容</param>
        /// <param name="Key">Key</param>
        [DllImport("Message.dll", EntryPoint = "Api_Tea解密", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern string Api_Tea解密(string decryptString, string Key);
        /// <summary>
        /// 取用戶名
        /// </summary>
        /// <param name="QQ">QQ</param>
        [DllImport("Message.dll", EntryPoint = "Api_GetNick", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern string Api_GetNick(string QQ);
        /// <summary>
        /// 取QQ等級+QQ會員等級 返回json格式信息
        /// </summary>
        /// <param name="QQ">QQ</param>
        [DllImport("Message.dll", EntryPoint = "Api_GetQQLevel", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern string Api_GetQQLevel(string QQ);
        /// <summary>
        /// 群號轉群ID
        /// </summary>
        /// <param name="gid">群號</param>
        [DllImport("Message.dll", EntryPoint = "Api_GNGetGid", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern string Api_GNGetGid(string gid);
        /// <summary>
        /// 群ID轉群號
        /// </summary>
        /// <param name="gid">群ID</param>
        [DllImport("Message.dll", EntryPoint = "Api_GidGetGN", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern string Api_GidGetGN(string gid);
        /// <summary>
        /// 取框架版本號(發布時間戳
        /// </summary>
        [DllImport("Message.dll", EntryPoint = "Api_GetVersion", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int Api_GetVersion();
        /// <summary>
        /// 取框架版本名
        /// </summary>
        [DllImport("Message.dll", EntryPoint = "Api_GetVersionName", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern string Api_GetVersionName();
        /// <summary>
        /// 取當前框架內部時間戳_週期性與服務器時間同步
        /// </summary>
        [DllImport("Message.dll", EntryPoint = "Api_GetTimeStamp", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int Api_GetTimeStamp();
        /// <summary>
        /// 取得框架輸出列表內所有信息
        /// </summary>
        [DllImport("Message.dll", EntryPoint = "Api_GetLog", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern string Api_GetLog();
        /// <summary>
        /// 判斷是否處於被屏蔽群信息狀態。
        /// </summary>
        /// <param name="qq">響應的QQ</param>
        [DllImport("Message.dll", EntryPoint = "Api_IfBlock", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool Api_IfBlock(string qq);
        /// <summary>
        /// 取包括群主在內的群管列表
        /// </summary>
        /// <param name="qq">響應的QQ</param>
        /// <param name="gid">群號</param>
        [DllImport("Message.dll", EntryPoint = "Api_GetAdminList", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern string Api_GetAdminList(string qq, string gid);
        /// <summary>
        /// 發說說
        /// </summary>
        /// <param name="qq">響應的QQ</param>
        /// <param name="content">內容</param>
        [DllImport("Message.dll", EntryPoint = "Api_AddTaotao", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern string Api_AddTaotao(string qq, string content);
        /// <summary>
        /// 取個簽
        /// </summary>
        /// <param name="qq">響應的QQ</param>
        /// <param name="QQ">對象</param>
        [DllImport("Message.dll", EntryPoint = "Api_GetSign", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern string Api_GetSign(string qq, string QQ);
        /// <summary>
        /// 設置個簽
        /// </summary>
        /// <param name="qq">響應的QQ</param>
        /// <param name="content">內容</param>
        [DllImport("Message.dll", EntryPoint = "Api_SetSign", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern string Api_SetSign(string qq, string content);
        /// <summary>
        /// 通過qun.qzone.qq.com接口取得取群列表.成功返回轉碼後的JSON格式文本信息
        /// </summary>
        /// <param name="qq">響應的QQ</param>
        [DllImport("Message.dll", EntryPoint = "Api_GetGroupListA", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern string Api_GetGroupListA(string qq);
        /// <summary>
        /// 通過qun.qq.com接口取得取群列表.成功返回轉碼後的JSON格式文本信息
        /// </summary>
        /// <param name="qq">響應的QQ</param>
        [DllImport("Message.dll", EntryPoint = "Api_GetGroupListB", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern string Api_GetGroupListB(string qq);
        /// <summary>
        /// 通過qun.qq.com接口取得群成員列表 成功返回轉碼後的JSON格式文本
        /// </summary>
        /// <param name="qq">響應的QQ</param>
        /// <param name="gid">群號</param>
        [DllImport("Message.dll", EntryPoint = "Api_GetGroupMemberA", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern string Api_GetGroupMemberA(string qq, string gid);
        /// <summary>
        /// 通過qun.qzone.qq.com接口取得群成員列表 成功返回轉碼後的JSON格式文本
        /// </summary>
        /// <param name="qq">響應的QQ</param>
        /// <param name="gid">群號</param>
        [DllImport("Message.dll", EntryPoint = "Api_GetGroupMemberB", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern string Api_GetGroupMemberB(string qq, string gid);
        /// <summary>
        /// 通過qun.qq.com接口取得好友列表。成功返回轉碼後的JSON文本
        /// </summary>
        /// <param name="qq">響應的QQ</param>
        [DllImport("Message.dll", EntryPoint = "Api_GetFriendList", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern string Api_GetFriendList(string qq);
        /// <summary>
        /// 取Q齡 成功返回Q齡 失敗返回-1
        /// </summary>
        /// <param name="qq">響應的QQ</param>
        /// <param name="QQ">QQ</param>
        [DllImport("Message.dll", EntryPoint = "Api_GetQQAge", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int Api_GetQQAge(string qq, string QQ);
        /// <summary>
        /// 取年齡 成功返回年齡 失敗返回-1
        /// </summary>
        /// <param name="qq">響應的QQ</param>
        /// <param name="QQ">QQ</param>
        [DllImport("Message.dll", EntryPoint = "Api_GetAge", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int Api_GetAge(string qq, string QQ);
        /// <summary>
        /// 取個人說明
        /// </summary>
        /// <param name="qq">響應的QQ</param>
        /// <param name="QQ">對象QQ</param>
        [DllImport("Message.dll", EntryPoint = "Api_GetPersonalProfile", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern string Api_GetPersonalProfile(string qq, string QQ);
        /// <summary>
        /// 取郵箱 成功返回郵箱 失敗返回空
        /// </summary>
        /// <param name="qq">響應的QQ</param>
        /// <param name="QQ">QQ</param>
        [DllImport("Message.dll", EntryPoint = "Api_GetEmail", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern string Api_GetEmail(string qq, string QQ);
        /// <summary>
        /// 取對象性別 1男 2女  未或失敗返回-1
        /// </summary>
        /// <param name="qq">響應的QQ</param>
        /// <param name="QQ">QQ</param>
        [DllImport("Message.dll", EntryPoint = "Api_GetGender", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int Api_GetGender(string qq, string QQ);
        /// <summary>
        /// 向好友發送‘正在輸入’的狀態信息.當好友收到信息之後 “正在輸入”狀態會立刻被打斷
        /// </summary>
        /// <param name="qq">響應的QQ</param>
        /// <param name="QQ">QQ</param>
        [DllImport("Message.dll", EntryPoint = "Api_SendTyping", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int Api_SendTyping(string qq, string QQ);
        /// <summary>
        /// 向好友發送窗口抖動信息
        /// </summary>
        /// <param name="qq">響應的QQ</param>
        /// <param name="QQ">QQ</param>
        [DllImport("Message.dll", EntryPoint = "Api_SendShake", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int Api_SendShake(string qq, string QQ);
        /// <summary>
        /// 取得框架內隨機一個在線且可以使用的QQ
        /// </summary>
        [DllImport("Message.dll", EntryPoint = "Api_GetRadomOnlineQQ", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern string Api_GetRadomOnlineQQ();
        /// <summary>
        /// 往帳號列表添加一個Q.當該Q已存在時則覆蓋密碼
        /// </summary>
        /// <param name="QQ">QQ</param>
        /// <param name="pwd">密碼</param>
        /// <param name="autologon">運行框架時是否自動登錄該Q.若添加後需要登錄該Q則需要通過Api_Login操作</param>
        [DllImport("Message.dll", EntryPoint = "Api_AddQQ", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool Api_AddQQ(string QQ, string pwd, bool autologon);
        /// <summary>
        /// 設置在線狀態+附加信息
        /// </summary>
        /// <param name="qq">響應的QQ</param>
        /// <param name="onlineStatus">在線狀態 1~6 分別對應我在線上, Q我吧, 離開, 忙碌, 請勿打擾, 隱身</param>
        /// <param name="info">狀態附加信息 最大255字節</param>
        [DllImport("Message.dll", EntryPoint = "Api_SetOLStatus", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool Api_SetOLStatus(string qq, int onlineStatus, string info);
        /// <summary>
        /// 取得機器碼
        /// </summary>
        [DllImport("Message.dll", EntryPoint = "Api_GetMC", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern string Api_GetMC();
        /// <summary>
        /// 邀請對象加入群 失敗返回錯誤理由
        /// </summary>
        /// <param name="qq">響應的QQ</param>
        /// <param name="QQ">好友QQ 多個好友用換行分割</param>
        /// <param name="gid">群號</param>
        [DllImport("Message.dll", EntryPoint = "Api_GroupInvitation", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern string Api_GroupInvitation(string qq, string QQ, string gid);
        /// <summary>
        /// 創建一個討論組 成功返回討論組ID 失敗返回空 注:每24小時只能創建100個討論組 悠著點用
        /// </summary>
        /// <param name="qq">響應的QQ</param>
        [DllImport("Message.dll", EntryPoint = "Api_CreateDG", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern string Api_CreateDG(string qq);
        /// <summary>
        /// 將對象移除討論組.成功返回空 失敗返回理由
        /// </summary>
        /// <param name="qq">響應的QQ</param>
        /// <param name="did">討論組ID</param>
        /// <param name="QQ">成員</param>
        [DllImport("Message.dll", EntryPoint = "Api_KickDG", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern string Api_KickDG(string qq, string did, string QQ);
        /// <summary>
        /// 邀請對象加入討論組 成功返回空 失敗返回理由
        /// </summary>
        /// <param name="qq">響應的QQ</param>
        /// <param name="did">討論組ID</param>
        /// <param name="QQl">成員組 多個成員用換行符分割</param>
        [DllImport("Message.dll", EntryPoint = "Api_DGInvitation", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern string Api_DGInvitation(string qq, string 討論組ID, string QQl);
        /// <summary>
        /// 成功返回以換行符分割的討論組號列表.最大為100個討論組  失敗返回空
        /// </summary>
        /// <param name="qq">響應的QQ</param>
        [DllImport("Message.dll", EntryPoint = "Api_GetDGList", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern string Api_GetDGList(string qq);
        /// <summary>
        /// 向對象發送一條音樂信息（所謂的點歌）次數不限
        /// </summary>
        /// <param name="qq">響應的QQ</param>
        /// <param name="msgtype">同Api_SendMsg  1好友 2群 3討論組 4群臨時會話 5討論組臨時會話</param>
        /// <param name="gdid">收信對象所屬群_討論組 發群內、臨時會話必填 好友可不填</param>
        /// <param name="QQ">臨時會話、好友必填 發至群內可不填</param>
        /// <param name="musicInfo">留空默認‘QQ音樂 的分享’</param>
        /// <param name="musicPageURL">任意直連或短鏈接均可 留空為空 無法點開</param>
        /// <param name="musicPicURL">任意直連或短鏈接均可 可空 例:http://url.cn/cDiJT4</param>
        /// <param name="musicURL">任意直連或短鏈接均可 不可空 例:http://url.cn/djwXjr</param>
        /// <param name="musicName">可空</param>
        /// <param name="musicSinger">可空</param>
        /// <param name="musicSource">可空 為空默認QQ音樂</param>
        /// <param name="musicIconURL">可空 為空默認QQ音樂 http://qzonestyle.gtimg.cn/ac/qzone/applogo/64/308/100497308_64.gif</param>
        [DllImport("Message.dll", EntryPoint = "Api_SendMusic", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool Api_SendMusic(string qq, int msgtype, string gdid, string QQ, string musicInfo, string musicPageURL, string musicPicURL, string musicURL, string musicName, string musicSinger, string musicSource, string musicIconURL);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="qq">響應的QQ</param>
        /// <param name="msgtype">同Api_SendMsg  1好友 2群 3討論組 4群臨時會話 5討論組臨時會話</param>
        /// <param name="gdid">收信對象所屬群_討論組 發群內、臨時會話必填 好友可不填</param>
        /// <param name="QQ">臨時會話、好友必填 發至群內可不填</param>
        /// <param name="ObjectMsg"></param>
        /// <param name="musictype">00 基本 02 點歌 其他不明</param>
        [DllImport("Message.dll", EntryPoint = "Api_SendObjectMsg", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool Api_SendObjectMsg(string qq, int msgtype, string gdid, string QQ, string ObjectMsg, int subtype);
        /// <summary>
        /// 判斷對象是否為好友（雙向）
        /// </summary>
        /// <param name="qq">響應的QQ</param>
        /// <param name="QQ">對象QQ</param>
        [DllImport("Message.dll", EntryPoint = "Api_IsFriend", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool Api_IsFriend(string qq, string QQ);
        /// <summary>
        /// 主動加好友 成功返回真 失敗返回假 當對象設置需要正確回答問題或不允許任何人添加時無條件失敗
        /// </summary>
        /// <param name="qq">機器人QQ</param>
        /// <param name="QQ">加誰</param>
        /// <param name="remark">加好友提交的理由</param>
        [DllImport("Message.dll", EntryPoint = "Api_AddFriend", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool Api_AddFriend(string qq, string QQ, string remark);
        /// <summary>
        /// 無參 用於插件自身請求禁用插件自身
        /// </summary>
        [DllImport("Message.dll", EntryPoint = "Api_SelfDisable", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern void Api_SelfDisable();
        /// <summary>
        /// 取協議客戶端類型常量 失敗返回0
        /// </summary>
        [DllImport("Message.dll", EntryPoint = "Api_GetClientType", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int Api_GetClientType();
        /// <summary>
        /// 取協議客戶端版本號常量  失敗返回0
        /// </summary>
        [DllImport("Message.dll", EntryPoint = "Api_GetClientVer", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int Api_GetClientVer();
        /// <summary>
        /// 取協議客戶端公開版本號常量  失敗返回0
        /// </summary>
        [DllImport("Message.dll", EntryPoint = "Api_GetPubNo", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int Api_GetPubNo();
        /// <summary>
        /// 取協議客戶端主版本號常量  失敗返回0
        /// </summary>
        [DllImport("Message.dll", EntryPoint = "Api_GetMainVer", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int Api_GetMainVer();
        /// <summary>
        /// 取協議客戶端通信模塊(TXSSO)版本號常量  失敗返回0
        /// </summary>
        [DllImport("Message.dll", EntryPoint = "Api_GetTXSSOVer", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int Api_GetTXSSOVer();
        /// <summary>
        /// 上傳音頻文件 成功返回guid用於發送
        /// </summary>
        /// <param name="qq">響應的QQ</param>
        /// <param name="uploadtype">1好友2群 注:好友圖和群圖的GUID並不相同並不通用 需要非別上傳。群、討論組用類型2 臨時會話、好友信息需要類型1</param>
        /// <param name="QQ">上傳該圖片所屬的群號或QQ</param>
        /// <param name="amrData">音頻字節集數據</param>
        /// <returns></returns>
        [DllImport("Message.dll", EntryPoint = "Api_UploadVoice", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern string Api_UploadVoice(string qq ,int uploadtype, string QQ, byte[] amrData);
        /// <summary>
        /// 通过音频、语音guid取得下载连接
        /// </summary>
        /// <param name="qq">响应的QQ</param>
        /// <param name="GUID">格式:{xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxx}.amr</param>
        [DllImport("Message.dll", EntryPoint = "Api_GuidGetVoiceLink", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern string Api_GuidGetVoiceLink(string qq, string GUID);
        /// <summary>
        /// 添加一个日志处理函数。每条新日志信息输出都会投递给该函数处理、重复添加将覆盖旧的、之前的接口
        /// </summary>
        /// <param name="pointer">回调子程序、函数指针(内存地址)。函数仅一个参数。参数1为 结构体LOGSTRUCT指针</param>
        [DllImport("Message.dll", EntryPoint = "Api_AddLogHandler", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern void Api_AddLogHandler(IntPtr pointer);
        /// <summary>
        /// 移除由Api_AddLogHandler添加、设置的日志处理函数
        /// </summary>
        [DllImport("Message.dll", EntryPoint = "Api_RemoveLogHandler", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern void Api_RemoveLogHandler();
        /// <summary>
        /// 获取群名
        /// </summary>
        /// <param name="qq">响应的QQ</param>
        /// <param name="gid">群号</param>
        [DllImport("Message.dll", EntryPoint = "Api_GetGroupName", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern string Api_GetGroupName(string qq, string gid);
        /// <summary>
        /// 添加一个待发送处理函数。每条待发送信息都会投递给该函数处理、重复添加将覆盖旧的、之前的接口
        /// </summary>
        /// <param name="pointer">回调子程序、函数指针(内存地址)。</param>
        [DllImport("Message.dll", EntryPoint = "Api_SetMsgFilter", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool Api_SetMsgFilter(IntPtr pointer);
        /// <summary>
        /// 移除\取消由Api_SetMsgFilter所添加\设置的处理函数
        /// </summary>
        [DllImport("Message.dll", EntryPoint = "Api_RemoveMsgFilter", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern void Api_RemoveMsgFilter();
        /// <summary>
        /// 回复信息 请尽量避免使用该API
        /// </summary>
        /// <param name="qq">响应的QQ</param>
        /// <param name="msgtype">1好友 2群 3讨论组 4群临时会话 5讨论组临时会话</param>
        /// <param name="QQ">接收这条信息的对象</param>
        /// <param name="content">信息内容</param>
        [DllImport("Message.dll", EntryPoint = "Api_Reply", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int Api_Reply(string qq, int msgtype, string QQ, string content);
        /// <summary>
        /// 上传图片.成功返回图片GUID用于发送该图片.失败返回空.  图片尺寸应小于4MB
        /// </summary>
        /// <param name="qq">响应的QQ</param>
        /// <param name="path">本地文件路径 选填</param>
        /// <param name="imgData">图片字节集数据 选填</param>
        [DllImport("Message.dll", EntryPoint = "Api_Upload", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern string Api_Upload(string qq, string path, byte[] imgData);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="qq">响应的QQ</param>
        /// <param name="msgtype">同Api_SendMsg  1好友 2群 3讨论组 4群临时会话 5讨论组临时会话</param>
        /// <param name="gdid">发群内、临时会话必填 好友可不填</param>
        /// <param name="QQ">临时会话、好友必填 发至群内可不填</param>
        /// <param name="xmlData"></param>
        /// <param name="xmltype">00 基本 02 点歌 其他不明</param>
        [DllImport("Message.dll", EntryPoint = "Api_SendXml", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool Api_SendXml(string qq, int msgtype, string gdid, string QQ, string xmlData, int xmltype);

    }
}
