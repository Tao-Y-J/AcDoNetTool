using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web.UI;
using System.Windows.Forms;

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
            //123
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
            return Get32MD51(result);
        }

        /// <summary>
        /// 合并单元格
        /// </summary>
        /// <param name="e"></param>
        public static void DataGridViewMergeRows(DataGridView dataGridView, DataGridViewCellPaintingEventArgs e, IEnumerable<int> mergeColumns)
        {
            bool flag = false;
            foreach (var item in mergeColumns)
            {
                if (item == e.ColumnIndex)
                {
                    flag = true;
                    break;
                }
            }

            if (flag == false)
            {
                return;
            }

            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && e.Value.ToString() != string.Empty)
            {
                #region
                int UpRows = 0;//上面相同的行数
                int DownRows = 0;//下面相同的行数
                int cellwidth = e.CellBounds.Width;//列宽
                //获取下面的行数
                for (int i = e.RowIndex; i < dataGridView.Rows.Count; i++)
                {
                    if (dataGridView.Rows[i].Cells[e.ColumnIndex].Value.ToString().Equals(e.Value.ToString()) && dataGridView.Rows[i].Cells[0].Value.ToString().Equals(dataGridView.Rows[e.RowIndex].Cells[0].Value.ToString()))
                    {
                        DownRows++;
                    }
                    else
                    {
                        break;
                    }
                }
                //获取上面的行数
                for (int i = e.RowIndex; i >= 0; i--)
                {
                    if (dataGridView.Rows[i].Cells[e.ColumnIndex].Value.ToString().Equals(e.Value.ToString()))
                    {
                        UpRows++;
                    }
                    else
                    {
                        break;
                    }
                }
                int count = UpRows + DownRows - 1;
                using (Brush gridBrush = new SolidBrush(dataGridView.GridColor), backColorBrush = new SolidBrush(e.CellStyle.BackColor))
                {
                    using (Pen gridLinePen = new Pen(gridBrush))
                    {
                        //清除单元格
                        e.Graphics.FillRectangle(backColorBrush, e.CellBounds);

                        if (e.Value != null)
                        {
                            int cellheight = e.CellBounds.Height;
                            SizeF size = e.Graphics.MeasureString(e.Value.ToString(), e.CellStyle.Font);

                            e.Graphics.DrawString((e.Value).ToString(), e.CellStyle.Font, Brushes.Black, e.CellBounds.X + (cellwidth - size.Width) / 2, e.CellBounds.Y - cellheight * (UpRows - 1) + (cellheight * count - size.Height) / 2, StringFormat.GenericDefault);
                        }
                        //如果下一行数据不等于当前行数据，则画当前单元格底边线
                        if (e.RowIndex < dataGridView.Rows.Count - 1 && (dataGridView.Rows[e.RowIndex + 1].Cells[e.ColumnIndex].Value.ToString() != e.Value.ToString()))
                        {
                            e.Graphics.DrawLine(gridLinePen, e.CellBounds.Left, e.CellBounds.Bottom - 1, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);
                        }
                        if (e.RowIndex == dataGridView.Rows.Count - 1)
                        {
                            e.Graphics.DrawLine(gridLinePen, e.CellBounds.Left + 2, e.CellBounds.Bottom - 1, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);
                            count = 0;
                        }
                        //画grid右边线
                        e.Graphics.DrawLine(gridLinePen, e.CellBounds.Right - 1, e.CellBounds.Top, e.CellBounds.Right - 1, e.CellBounds.Bottom);
                        e.Handled = true;
                    }
                }
                #endregion
            }
        }
    }
}