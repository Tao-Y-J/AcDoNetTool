using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Script.Serialization;

namespace AcDoNetTool.Common
{
    public class JsonHelper
    {
        public static string GetJsonFromObj<T>(T obj)
        {
            if (obj == null)
            {
                return null;
            }

            DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(obj.GetType());
            MemoryStream memoryStream = new MemoryStream();
            dataContractJsonSerializer.WriteObject(memoryStream, obj);
            return Encoding.UTF8.GetString(memoryStream.ToArray(), 0, memoryStream.ToArray().Length);
        }

        public static T GetObjFromJson<T>(string jsonStr)
        {
            if (string.IsNullOrEmpty(jsonStr))
            {
                return default;
            }

            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonStr));
            DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(T));
            return (T)dataContractJsonSerializer.ReadObject(stream);
        }

        public static List<T> GetObjListFromJson<T>(string jsonStr)
        {
            if (string.IsNullOrEmpty(jsonStr))
            {
                return null;
            }

            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            return javaScriptSerializer.Deserialize<List<T>>(jsonStr);
        }

        public static string GetKendoUIGridJson<T>(T obj, int totalCount)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (obj != null)
            {
                DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(obj.GetType());
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    dataContractJsonSerializer.WriteObject(memoryStream, obj);
                    stringBuilder.Append("{ ");
                    stringBuilder.Append("\"data\":");
                    stringBuilder.Append(Encoding.UTF8.GetString(memoryStream.ToArray(), 0, memoryStream.ToArray().Length));
                    stringBuilder.Append(",");
                    stringBuilder.Append("\"TotalCount\":");
                    stringBuilder.Append(totalCount);
                    stringBuilder.Append("}");
                }

                return stringBuilder.ToString();
            }

            return "{ \"data\":[],\"TotalCount\":0}";
        }
    }
}