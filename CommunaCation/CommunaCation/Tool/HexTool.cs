using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace CommunaCation
{
    /// <summary>
    /// 十六进制和string转换
    /// </summary>
    public class HexTool
    {

        #region HexToByte

        public static byte[] HexToByte(string msg)
        {
            //remove any spaces from the string
            msg = msg.Replace(" ", "");
            //create a byte array the length of the
            //divided by 2 (Hex is 2 characters in length)
            byte[] comBuffer = new byte[msg.Length / 2];
            //loop through the length of the provided string
            for (int i = 0; i < msg.Length; i += 2)
                //convert each set of 2 characters to a byte
                //and add to the array
                comBuffer[i / 2] = (byte)Convert.ToByte(msg.Substring(i, 2), 16);
            //return the array
            return comBuffer;
        }

        #endregion

        #region 返回处理后的十六进制字符串

        /// <summary>
        /// 返回处理后的十六进制字符串
        /// </summary>
        /// <param name="mStr"></param>
        /// <returns></returns>
        public static string StrToHexStr(string mStr) 
        {
            string str = BitConverter.ToString(
            ASCIIEncoding.Default.GetBytes(mStr)).Replace("-", " ");

            return str;
        }

        #endregion

        #region 返回十六进制代表的字符串

        /// <summary>
        /// 返回十六进制代表的字符串
        /// </summary>
        /// <param name="mHex"></param>
        /// <returns></returns>
        public static string HexStrToStr(string mHex) 
        {
            try
            {
                mHex = mHex.Replace(" ", "");
                if (mHex.Length <= 0) return "";
                byte[] vBytes = new byte[mHex.Length / 2];
                for (int i = 0; i < mHex.Length; i += 2)
                    if (!byte.TryParse(mHex.Substring(i, 2), NumberStyles.HexNumber, null, out vBytes[i / 2]))
                        vBytes[i / 2] = 0;
                return ASCIIEncoding.Default.GetString(vBytes);
            }
            catch (Exception)
            {
                Log.Error("无法将十六进制的[{mHex}]转换为string");
                return "";
            }

        }

        #endregion

        #region 字节转十六进制

        public static string ByteToHex(byte[] comByte)
        {
            //create a new StringBuilder object
            StringBuilder builder = new StringBuilder(comByte.Length * 3);
            //loop through each byte in the array
            foreach (byte data in comByte)
                //convert the byte to a string and add to the stringbuilder
                builder.Append(Convert.ToString(data, 16).PadLeft(2, '0').PadRight(3, ' '));
            //return the converted value
            return builder.ToString().ToUpper();
        }

        #endregion

    }
}
