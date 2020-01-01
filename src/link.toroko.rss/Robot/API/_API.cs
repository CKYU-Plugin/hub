using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using static Robot.Property.RobotProperty;
using Robot.Property;
using Robot.Extension;
using System.Runtime.ExceptionServices;
using System.Security;
using link.toroko.rsshub.Robot.API;

namespace Robot.API
{
    public class _API
    {
        [HandleProcessCorruptedStateExceptions, SecurityCritical]
        public static string Api_GuidGetPicLink(string imagecode)
        {
            switch (RobotBase.robot)
            {
                case RobotType.MPQ:
                    return MPQMessageAPI.Api_GuidGetPicLink(imagecode);
                case RobotType.CQ:
                    try
                    {
                        string path = String.Format(@"data\image\{0}.cqimg", imagecode.Split('=')[1].Replace("]", ""));
                        if (File.Exists(path))
                        {
                            IniFile ini = new IniFile(path);
                            string url = ini.IniReadValue("image", "url");
                            return url;
                        }
                    }
                    catch { }
                    return "";
                default:
                    return "";
            }
        }
        [HandleProcessCorruptedStateExceptions, SecurityCritical]
        public static string Api_Sha1(Image image)
        {
            string image_sha1 = "";

            ImageConverter converter = new ImageConverter();
            byte[] imgBytes = new byte[1];
            imgBytes = (byte[])converter.ConvertTo(image, imgBytes.GetType());
            SHA1Managed sha = new SHA1Managed();
            byte[] imgHash1 = sha.ComputeHash(imgBytes);
            var sb = new StringBuilder(imgHash1.Length * 2);

            foreach (byte b in imgHash1)
            {
                sb.Append(b.ToString("X2"));
            }
            image_sha1 = sb.ToString();

            return image_sha1;
        }
        [HandleProcessCorruptedStateExceptions, SecurityCritical]
        public static string Api_UploadPic(Image image,out string outpath, string pre ="", string qq = "", int uploadtype = 0, string gdid = "")
        {

            var file_image_format = typeof(System.Drawing.Imaging.ImageFormat).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).ToList().ConvertAll(property => property.GetValue(null, null)).Single(image_format => image_format.Equals(image.RawFormat));

            string image_sha1 = "";

            image_sha1 = Api_Sha1(image);

            string filename = $"{pre}_{image_sha1}_{(int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds}.{file_image_format.ToString().ToLower()}";

            System.IO.Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "data");
            System.IO.Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"data\image");
            System.IO.Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"data\image\" + RobotBase.PluginName);

            string path = @"data\image\";
            string pluginpath = RobotBase.PluginName + @"\";
            string fullpath = AppDomain.CurrentDomain.BaseDirectory + path + pluginpath + filename;
            outpath = fullpath;

            try
            {
                image.Save(fullpath, image.RawFormat);
            }
            catch { }

            switch (RobotBase.robot)
            {
                case RobotType.MPQ:
                    using (var ms = new MemoryStream())
                    {
                        image.Save(ms, image.RawFormat);
                        string imageguid = MPQMessageAPI.Api_Upload(qq, fullpath, ms.ToArray());
                        return imageguid;
                    }
                case RobotType.Test:
                case RobotType.CQ:
                    string name = "[CQ:image,file=" + pluginpath + filename + "]";
                    return name;
                default:
                    return "";
            }
        }

        [HandleProcessCorruptedStateExceptions, SecurityCritical]
        public static int Api_OutError(string outstring)
        {
            switch (RobotBase.robot)
            {
                case RobotType.MPQ:
                    return MPQMessageAPI.Api_OutPut(outstring);
                case RobotType.CQ:
                    return CQAPI.AddLog(RobotBase.CQ_AuthCode, CQAPI.LogPriority.CQLOG_ERROR, "ERROR", outstring);
                case RobotType.Test:
                    Console.WriteLine(outstring);
                    return 0;
                default:
                    return 0;
            }
        }

        [HandleProcessCorruptedStateExceptions, SecurityCritical]
        public static int Api_OutPut(string outstring)
        {
            switch (RobotBase.robot)
            {
                case RobotType.MPQ:
                    return MPQMessageAPI.Api_OutPut(outstring);
                case RobotType.CQ:
                    return CQAPI.AddLog(RobotBase.CQ_AuthCode, CQAPI.LogPriority.CQLOG_INFO, "INFO", outstring);
                case RobotType.Test:
                    Console.WriteLine(outstring);
                    return 0;
                default:
                    return 0;
            }
        }

        [HandleProcessCorruptedStateExceptions, SecurityCritical]
        public static void SendMessage(string qq, int msgType, string content, string gdid, string robotQQ, int msgSubType)
        {
            long _qq = 0;
            long _gdid = 0;
            Int64.TryParse(qq, out _qq);
            Int64.TryParse(gdid, out _gdid);
            try
            {
                switch (RobotBase.robot)
                {

                    case RobotType.MPQ:
                        MPQMessageAPI.Api_SendMsg(robotQQ, msgType, msgSubType, gdid, qq, content);
                        break;
                    case RobotType.CQ:
                        if (msgType.In(1, 4))
                        {
                            CQAPI.SendPrivateMessage(RobotBase.CQ_AuthCode, _qq, content);
                        }
                        else if (msgType.In(2, 3))
                        {
                            CQAPI.SendGroupMessage(RobotBase.CQ_AuthCode, _gdid, content);
                        }
                        break;
                    default:
                        Console.WriteLine(content);
                        break;
                }
            }
            catch { }
        }

        [HandleProcessCorruptedStateExceptions, SecurityCritical]
        public static string GetNickName(string qq)
        {
            try
            {
                string Qqid = new String(qq.Where(Char.IsDigit).ToArray());
                long lQqid = 0;
                long.TryParse(Qqid, out lQqid);
                string _tmp = CQAPI.GetStrangerInfo(RobotBase.CQ_AuthCode, lQqid, 1);
                var data = _tmp.DeBase64();
                if (data.Length < 10) { return ""; }
                var unpack = new Unpack(data);
                var qqid = unpack.GetLong();
                if (qqid.ToString() != qq) { return ""; }
                var nickname = unpack.GetLenStr();
                return nickname;
            }
            catch { return ""; }
        }


    }
}
