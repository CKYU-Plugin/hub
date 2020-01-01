
namespace Robot.Code
{
    public class CQCode
    {
        /// <summary>
        /// 獲取 @指定QQ 的操作代碼
        /// </summary>
        /// <param name="qq">指定的QQ號碼
        /// <para>當該參數為-1時，操作為 @全部成員</para>
        /// </param>
        /// <returns>CQ @操作代碼</returns>
        public static string CQCode_At(long qq)
        {
            return "[CQ:at,qq=" + (qq == -1 ? "all" : qq.ToString()) + "]";
        }
    }
}
