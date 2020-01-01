using Robot.Property;
using System;
using static Robot.Property.RobotProperty;

namespace Robot.Code
{
    public class _Code
    {
        /// <summary>
        /// 獲取 @指定QQ 的操作代碼
        /// </summary>
        /// <param name="qq">指定的QQ號碼</param>
        /// <returns>MPQ @操作代碼</returns>
        public static string Code_At(string qq)
        {
            long _qq = -1;
            Int64.TryParse(qq, out _qq);

            switch (RobotBase.robot)
            {
                case RobotType.CQ:
                    return CQCode.CQCode_At(_qq);
                case RobotType.Test:
                    return CQCode.CQCode_At(_qq);
                default:
                    return string.Empty;
            }
        }

        public static string Code_At_Regex()
        {

            switch (RobotBase.robot)
            {
                case RobotType.CQ:
                    return @"\[CQ:at,qq=(-1|all|[0-9]{4,13})\]";
                case RobotType.MPQ:
                    return @"\[@(-1|all|[0-9]{4,13})\]";
                case RobotType.Test:
                    return @"\[CQ:at,qq=(-1|all|[0-9]{4,13})\]";
                default:
                    return string.Empty;
            }

        }

        public static string Code_Image_Regex()
        {

            switch (RobotBase.robot)
            {
                case RobotType.CQ:
                    return @"\[CQ:image,file=([A-Z0-9]{32}.[a-z]{3,4})\]";
                case RobotType.MPQ:
                    return @"{(\w{8}-\w{4}-\w{4}-\w{4}-\w{12})}.(jpg|jpeg|gif|png|apng)";
                case RobotType.Test:
                    return @"{(\w{8}-\w{4}-\w{4}-\w{4}-\w{12})}.(jpg|jpeg|gif|png|apng)";
                default:
                    return string.Empty;
            }

        }

    }
}
