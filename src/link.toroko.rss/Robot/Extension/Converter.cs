using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robot.Extension
{
    public static class Converter
    {
        public static string ByteArrayToHexString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        public static string IntToHexString(int intValue)
        {
            string hexValue = intValue.ToString("X");
            return hexValue;
        }

        public static int HexStringToInt(string hexValue)
        {
            int intValue = -1;
            try
            {
                intValue = int.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            return intValue;
        }

        public class QQEncoding
        {

            private string SkeyToBkn(string str)
            {
                int hash = 5381;
                for (int i = 0; i < str.Length; ++i)
                {
                    hash += (hash << 5) + (int)str[i];
                }
                return (hash & 2147483647).ToString();
            }

            private string BknToSkey(string str)
            {
                int hash = 5381;
                for (int i = 0; i < str.Length; ++i)
                {
                    hash += (hash << 5) + (int)str[i];
                }
                return (hash & 2147483647).ToString();
            }

        }
        public class HexadecimalEncoding
        {
            public static string ToHexString(string str)
            {
                var sb = new StringBuilder();

                var bytes = Encoding.Unicode.GetBytes(str);
                foreach (var t in bytes)
                {
                    sb.Append(t.ToString("X2"));
                }

                return sb.ToString(); // returns: "48656C6C6F20776F726C64" for "Hello world"
            }

            public static string FromHexString(string hexString)
            {
                var bytes = new byte[hexString.Length / 2];
                for (var i = 0; i < bytes.Length; i++)
                {
                    bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
                }

                return Encoding.Default.GetString(bytes); // returns: "Hello world" for "48656C6C6F20776F726C64"
            }

        }
    }
}
