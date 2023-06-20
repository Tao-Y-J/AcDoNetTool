using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace AcDoNetTool.Common
{
    public static class CommonTool
    {
        public static string Guid => System.Guid.NewGuid().ToString().Replace("-", "").ToUpper();

        /// <summary>
        /// 通过转化为Json字符串进行深拷贝
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T DeepCopyByJson<T>(T obj)
        {
            string json = JsonHelper.GetJsonFromObj(obj);
            return JsonHelper.GetObjFromJson<T>(json);
        }

        /// <summary>
        /// 替换空格
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ReplaceBlank(this string str)
        {
            return str.Replace(" ", "");
        }

        /// <summary>
        /// 数字1-9转换为中文数字
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string OneBitNumberToChinese(string num)
        {
            string numStr = "123456789";
            string chineseStr = "一二三四五六七八九";
            string result = "";
            int numIndex = numStr.IndexOf(num);
            if (numIndex > -1)
            {
                result = chineseStr.Substring(numIndex, 1);
            }
            return result;
        }

        /// <summary>
        /// 阿拉伯数字转换为中文数字（0-99999）
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string NumberToChinese(int num)
        {
            if (num > 99999)
            {
                throw new Exception("无法转换大于99999的数字");
            }
            string strNum = num.ToString();
            string result = "";
            if (strNum.Length == 5)
            {//万
                result += OneBitNumberToChinese(strNum.Substring(0, 1)) + "万";
                strNum = strNum.Substring(1);
            }
            if (strNum.Length == 4)
            {//千
                string secondBitNumber = strNum.Substring(0, 1);
                if (secondBitNumber == "0") result += "零";
                else result += OneBitNumberToChinese(secondBitNumber) + "千";
                strNum = strNum.Substring(1);
            }
            if (strNum.Length == 3)
            {//百
                string hundredBitNumber = strNum.Substring(0, 1);
                if (hundredBitNumber == "0" && result.Substring(result.Length - 1) != "零") result += "零";
                else result += OneBitNumberToChinese(hundredBitNumber) + "百";
                strNum = strNum.Substring(1);
            }
            if (strNum.Length == 2)
            {//十
                string hundredBitNumber = strNum.Substring(0, 1);
                if (hundredBitNumber == "0" && result.Substring(result.Length - 1) != "零") result += "零";
                else if (hundredBitNumber == "1" && string.IsNullOrEmpty(result)) result += "十";//10->十
                else result += OneBitNumberToChinese(hundredBitNumber) + "十";
                strNum = strNum.Substring(1);
            }
            if (strNum.Length == 1)
            {//个
                if (strNum == "0") result += "零";
                else result += OneBitNumberToChinese(strNum);
            }
            //100->一百零零
            if (!string.IsNullOrEmpty(result) && result != "零") result = result.TrimEnd('零');
            return result;
        }

        /// <summary>
        /// string类型减法
        /// </summary>
        /// <param name="firstString"></param>
        /// <param name="secondString"></param>
        /// <returns></returns>
        public static string Subtraction(string firstString, string secondString)
        {
            double firstNum = 0, secondNum = 0;
            if (!string.IsNullOrEmpty(firstString))
            {
                double.TryParse(firstString, out firstNum);
            }
            if (!string.IsNullOrEmpty(secondString))
            {
                double.TryParse(secondString, out secondNum);
            }
            return (firstNum - secondNum).ToString();
        }

        /// <summary>
        /// string类型加法
        /// </summary>
        /// <param name="firstString"></param>
        /// <param name="secondString"></param>
        /// <returns></returns>
        public static string Addition(string firstString, string secondString)
        {
            double firstNum = 0, secondNum = 0;
            if (!string.IsNullOrEmpty(firstString))
            {
                double.TryParse(firstString, out firstNum);
            }
            if (!string.IsNullOrEmpty(secondString))
            {
                double.TryParse(secondString, out secondNum);
            }
            return (firstNum + secondNum).ToString();
        }

        /// <summary>
        /// string类型乘法
        /// </summary>
        /// <param name="firstString"></param>
        /// <param name="secondString"></param>
        /// <returns></returns>
        public static string Multiplication(string firstString, string secondString)
        {
            double firstNum = 0, secondNum = 0;
            if (!string.IsNullOrEmpty(firstString))
            {
                double.TryParse(firstString, out firstNum);
            }
            if (!string.IsNullOrEmpty(secondString))
            {
                double.TryParse(secondString, out secondNum);
            }
            return (firstNum * secondNum).ToString();
        }

        /// <summary>
        /// string类型除法
        /// </summary>
        /// <param name="firstString"></param>
        /// <param name="secondString"></param>
        /// <returns></returns>
        public static string Division(string firstString, string secondString)
        {
            double firstNum = 0, secondNum = 0;
            if (!string.IsNullOrEmpty(firstString))
            {
                double.TryParse(firstString, out firstNum);
            }
            if (!string.IsNullOrEmpty(secondString))
            {
                double.TryParse(secondString, out secondNum);
            }
            return (firstNum / secondNum).ToString();
        }

        /// <summary>
        /// 获取随机数
        /// </summary>
        /// <param name="minValue">起始</param>
        /// <param name="maxValue">结束</param>
        /// <returns></returns>
        public static int GetRandomNumber(int minValue, int maxValue)
        {
            Thread.Sleep(1);
            string guid = Guid;
            Random randomNum = new Random(guid.GetHashCode());
            return randomNum.Next(minValue, maxValue);
        }

        /// <summary>
        /// 获取随机数
        /// </summary>
        /// <param name="maxValue">范围</param>
        /// <returns></returns>
        public static int GetRandomNumber(int maxValue)
        {
            Thread.Sleep(1);
            string guid = Guid;
            Random randomNum = new Random(guid.GetHashCode());
            return randomNum.Next(maxValue);
        }

        /// <summary>
        /// 获取随机数
        /// </summary>
        /// <returns></returns>
        public static int GetRandomNumber()
        {
            Thread.Sleep(1);
            string guid = Guid;
            Random randomNum = new Random(guid.GetHashCode());
            return randomNum.Next();
        }

        /// <summary>
        /// 创建哈希字符串适用于任何 MD5 哈希函数 （在任何平台） 上创建 32 个字符的十六进制格式哈希字符串
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static string Get32MD51(string source)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(source));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                string hash = sBuilder.ToString();
                return hash.ToUpper();
            }
        }

        /// <summary>
        /// 获取16位md5加密
        /// </summary>
        /// <param name="strSource">需要加密的明文</param>
        /// <returns>返回32位加密结果，该结果取32位加密结果的第9位到25位</returns>
        private static string Get32MD52(string source)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            //获取密文字节数组
            byte[] bytResult = md5.ComputeHash(Encoding.Default.GetBytes(source));
            //转换成字符串，32位
            string strResult = BitConverter.ToString(bytResult);
            //BitConverter转换出来的字符串会在每个字符中间产生一个分隔符，需要去除掉
            strResult = strResult.Replace("-", "");
            return strResult.ToUpper();
        }

        /// <summary>
        /// 获取16位md5加密
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static string Get16MD51(string source)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(source));
                //转换成字符串，并取9到25位
                string sBuilder = BitConverter.ToString(data, 4, 8);
                //BitConverter转换出来的字符串会在每个字符中间产生一个分隔符，需要去除掉
                sBuilder = sBuilder.Replace("-", "");
                return sBuilder.ToString().ToUpper();
            }
        }

        /// <summary>
        /// 获取16位md5加密
        /// </summary>
        /// <param name="strSource">需要加密的明文</param>
        /// <returns>返回16位加密结果，该结果取32位加密结果的第9位到25位</returns>
        private static string Get16MD52(string source)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            //获取密文字节数组
            byte[] bytResult = md5.ComputeHash(Encoding.Default.GetBytes(source));
            //转换成字符串，并取9到25位
            string strResult = BitConverter.ToString(bytResult, 4, 8);
            //BitConverter转换出来的字符串会在每个字符中间产生一个分隔符，需要去除掉
            strResult = strResult.Replace("-", "");
            return strResult.ToUpper();
        }

        /// <summary>
        /// 简单的MD5加密
        /// </summary>
        /// <param name="strs"></param>
        /// <returns></returns>
        private static string ToMD5(string source)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] bytes = Encoding.Default.GetBytes(source);//将要加密的字符串转换为字节数组
            byte[] encryptdata = md5.ComputeHash(bytes);//将字符串加密后也转换为字符数组
            return Convert.ToBase64String(encryptdata);//将加密后的字节数组转换为加密字符串
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string Encipherment(string source)
        {
            string result = ToMD5(source);
            result = Get32MD52(result);
            result = Get16MD51(result);
            result = Get16MD52(result);
            return Get32MD51(result)
        }
    }
}