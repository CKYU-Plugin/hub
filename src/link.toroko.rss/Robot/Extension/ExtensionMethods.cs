using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace System
{
    public static class ExtensionMethods
    {

    }

    public class IAwaiter : INotifyCompletion
    {
        public bool IsCompleted { get; }

        public void GetResult()
        {
        }

        public void OnCompleted(Action continuation)
        {
            continuation();
        }
    }

    public static class Awaiter
    {
        public static IAwaiter GetAwaiter(this int i)
        {
            return new IAwaiter();
        }
        public static IAwaiter GetAwaiter(this string s)
        {
            return new IAwaiter();
        }
    }

    public static class EnumExtension
    {
        public static string ToDescription(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }
    }

    public static class StringExtension
    {
        public static DateTime ToDateTime(this string gmtStr, string format)
        {
            try
            {
                return DateTime.ParseExact(gmtStr, format,
                                           System.Globalization.CultureInfo.InvariantCulture);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        public static bool GMTStrParse(this string gmtStr, out DateTime gmtTime)
        {
            CultureInfo enUS = new CultureInfo("en-US");
            bool s = DateTime.TryParseExact(gmtStr, "r", enUS, DateTimeStyles.None, out gmtTime);
            return s;
        }

        public static DateTime GMTStrParse(this string gmtStr)
        {
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            CultureInfo enUS = new CultureInfo("en-US");
            DateTime.TryParseExact(gmtStr, "r", enUS, DateTimeStyles.None, out dt);
            return dt;
        }

        public static DateTime UnixTimeStampToDateTime(this string unixTimeStamp)
        {
            double.TryParse(unixTimeStamp, out double _unixTimeStamp);
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(_unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }

    public static class DoubleExtension
    {
        public static DateTime UnixTimeStampToDateTime(this double unixTimeStamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }

    public static class IntExtension
    {
        public static DateTime UnixTimeStampToDateTime(this int unixTimeStamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }


    public static class ImageExtension
    {
        public static Image ConvertImage(this Image originalImage, ImageFormat format)
        {
            var stream = new MemoryStream();
            originalImage.Save(stream, format);
            stream.Position = 0;
            return Bitmap.FromStream(stream);
        }
    }

    public static class StreamExtension
    {
        public static Image ConvertImage(this Stream originalStream, ImageFormat format)
        {
            using (var image = Image.FromStream(originalStream))
            {
                using (var stream = new MemoryStream())
                {
                    image.Save(stream, format);
                    stream.Position = 0;
                    return Bitmap.FromStream(stream);
                }
            }
        }
    }
}
