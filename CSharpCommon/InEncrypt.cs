using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Web;

namespace Utility
{
    /// <summary>
    /// Encrypt 的摘要说明。
    /// </summary>
    public class InEncrypt
    {
        //默认密钥向量
        private static byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

        public InEncrypt()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="encryptKey">加密密钥,要求为8位 private string encryptKey = "1234567890"; //用户密码Key</param>
        /// <returns>加密成功返回加密后的字符串,失败返回源串</returns>
        public static string Encode(string encryptString, string encryptKey)
        {
            encryptKey = Utils.GetSubString(encryptKey, 8, "");
            encryptKey = encryptKey.PadRight(8, ' ');
            byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
            byte[] rgbIV = Keys;
            byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
            DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            return Convert.ToBase64String(mStream.ToArray());

        }

        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
        /// <returns>解密成功返回解密后的字符串,失败返源串</returns>
        public static string Decode(string decryptString, string decryptKey)
        {
            try
            {
                decryptKey = Utils.GetSubString(decryptKey, 8, "");
                decryptKey = decryptKey.PadRight(8, ' ');
                byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey);
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();

                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return "";
            }

        }

        /// <summary>
        /// MD5 SHA1加密
        /// </summary>
        /// <param name="InputString">要加密的字串</param>
        /// <param name="pwdType">加密类型,eg: MD5,SHA1</param>
        /// <returns>密文</returns>
        public static string Encrypt(string InputString, string pwdType)
        {
            InputString = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(InputString, pwdType);
            return InputString;
        }


        //md5 加密
        public static string MD5(String str)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.Default.GetBytes(str);
            byte[] result = md5.ComputeHash(data);
            string s = BitConverter.ToString(result);
            return s;
        }

        public static string MD5Encrypt(Stream stream)
        {
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(stream);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }

        public static string MD5Encrypt(string str)
        {
            if (str == null) return "";
            byte[] bs = System.Text.Encoding.ASCII.GetBytes(str);
            bs = new System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(bs);
            string result = "";
            for (int i = 0; i < bs.Length; i++)
                result += bs[i].ToString("x").PadLeft(2, '0');
            return result;
        }
        public static string MD5Utf8Encrypt(string str)
        {
            if (str == null) return "";
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(str);
            bs = new System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(bs);
            string result = "";
            for (int i = 0; i < bs.Length; i++)
                result += bs[i].ToString("x").PadLeft(2, '0');
            return result;
        }

        ///<summary>
        /// Base 64 Encoding with URL and Filename Safe Alphabet using UTF-8 character set.
        ///</summary>
        ///<param name="str">The origianl string</param>
        ///<returns>The Base64 encoded string</returns>
        public static string Base64ForUrlEncode(string str)
        {
            byte[] encbuff = Encoding.UTF8.GetBytes(str);
            return HttpServerUtility.UrlTokenEncode(encbuff);
        }

        ///<summary>
        /// Decode Base64 encoded string with URL and Filename Safe Alphabet using UTF-8.
        ///</summary>
        ///<param name="str">Base64 code</param>
        ///<returns>The decoded string.</returns>
        public static string Base64ForUrlDecode(string str)
        {
            byte[] decbuff = HttpServerUtility.UrlTokenDecode(str);
            return Encoding.UTF8.GetString(decbuff);
        }


        /// <summary>
        /// Base64解码类
        /// 将Base64编码的string类型转换成byte[]类型
        /// </summary>
        public class Base64Decoder
        {
            char[] source;
            int length, length2, length3;
            int blockCount;
            int paddingCount;
            public static Base64Decoder Decoder = new Base64Decoder();

            public Base64Decoder()
            {
            }

            private void init(char[] input)
            {
                int temp = 0;
                source = input;
                length = input.Length;

                for (int x = 0; x < 2; x++)
                {
                    if (input[length - x - 1] == '=')
                        temp++;
                }
                paddingCount = temp;

                blockCount = length / 4;
                length2 = blockCount * 3;
            }

            public byte[] GetDecoded(string strInput)
            {
                //初始化
                init(strInput.ToCharArray());

                byte[] buffer = new byte[length];
                byte[] buffer2 = new byte[length2];

                for (int x = 0; x < length; x++)
                {
                    buffer[x] = char2sixbit(source[x]);
                }

                byte b, b1, b2, b3;
                byte temp1, temp2, temp3, temp4;

                for (int x = 0; x < blockCount; x++)
                {
                    temp1 = buffer[x * 4];
                    temp2 = buffer[x * 4 + 1];
                    temp3 = buffer[x * 4 + 2];
                    temp4 = buffer[x * 4 + 3];

                    b = (byte)(temp1 << 2);
                    b1 = (byte)((temp2 & 48) >> 4);
                    b1 += b;

                    b = (byte)((temp2 & 15) << 4);
                    b2 = (byte)((temp3 & 60) >> 2);
                    b2 += b;

                    b = (byte)((temp3 & 3) << 6);
                    b3 = temp4;
                    b3 += b;

                    buffer2[x * 3] = b1;
                    buffer2[x * 3 + 1] = b2;
                    buffer2[x * 3 + 2] = b3;
                }

                length3 = length2 - paddingCount;
                byte[] result = new byte[length3];

                for (int x = 0; x < length3; x++)
                {
                    result[x] = buffer2[x];
                }

                return result;
            }

            private byte char2sixbit(char c)
            {
                char[] lookupTable = new char[64]{
                 'A','B','C','D','E','F','G','H','I','J','K','L','M','N',
                 'O','P','Q','R','S','T','U','V','W','X','Y', 'Z',
                 'a','b','c','d','e','f','g','h','i','j','k','l','m','n',
                 'o','p','q','r','s','t','u','v','w','x','y','z',
                 '0','1','2','3','4','5','6','7','8','9','+','/'};
                if (c == '=')
                    return 0;
                else
                {
                    for (int x = 0; x < 64; x++)
                    {
                        if (lookupTable[x] == c)
                            return (byte)x;
                    }

                    return 0;
                }

            }
        }
    }
}
