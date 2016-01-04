using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Alan.Utils.ExtensionMethods
{
    /// <summary>
    /// 基础类型转化扩展方法
    /// </summary>
    public static class ExConvert
    {
        /// <summary>
        /// 类型转换
        /// </summary>
        /// <typeparam name="TOut">输出类型</typeparam>
        /// <param name="obj"></param>
        /// <param name="defWhenFail">转换失败时的替换值</param>
        /// <returns></returns>
        public static TOut ExChangeType<TOut>(this object obj, TOut defWhenFail = default(TOut))
        {
            try
            {
                return (TOut)Convert.ChangeType(obj, typeof(TOut));
            }
            catch
            {
                return defWhenFail;
            }
        }

        /// <summary>
        /// 转成JSON
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ExToJson<T>(this T obj)
          where T : class
        {
            string json = String.Empty;
            try
            {
                json = JsonConvert.SerializeObject(obj);
            }
            catch (JsonSerializationException)
            {
                var setting = new JsonSerializerSettings();
                setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                json = JsonConvert.SerializeObject(obj, setting);
            }
            return json;

        }


        /// <summary>
        /// JSON转实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T ExJsonToEntity<T>(this string json)
            where T : class
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// JSON以安全的方式转实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <param name="whenFail"></param>
        /// <returns></returns>
        public static T ExJsonTryToEntity<T>(this string json, T whenFail = default(T))
            where T : class
        {
            try
            {
                return json.ExJsonToEntity<T>();
            }
            catch
            {
                return whenFail;
            }
        }

        /// <summary>
        /// 转成XML字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ExToXml<T>(this T obj)
            where T : class
        {
            XmlSerializer serialize = new XmlSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream())
            {
                serialize.Serialize(ms, obj);
                var xml = Encoding.UTF8.GetString(ms.ToArray());
                return xml;
            }
        }

        /// <summary>
        /// XML转实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T ExXmlToEntity<T>(this string xml)
            where T : class
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(T));
            var xmlBytes = Encoding.UTF8.GetBytes(xml);
            using (var ms = new MemoryStream(xmlBytes))
            {
                using (TextReader reader = new StreamReader(ms))
                {
                    object obj = deserializer.Deserialize(reader);
                    return obj.ExChangeType<T>();
                }
            }
        }

        /// <summary>
        /// XML以安全的方式转实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <param name="whenFail"></param>
        /// <returns></returns>
        public static T ExXmlTryToEntity<T>(this string xml, T whenFail = default(T))
            where T : class
        {
            try
            {
                return xml.ExXmlToEntity<T>();
            }
            catch
            {
                return whenFail;
            }
        }



        /// <summary>
        /// 转成int类型
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="whenFail"></param>
        /// <returns></returns>
        public static int ExToInt(this object obj, int whenFail = -1)
        {
            if (obj == null) return whenFail;
            int convertValue;
            return int.TryParse(obj.ToString(), out convertValue) ? convertValue : whenFail;
        }

        /// <summary>
        /// 转成short类型
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="whenFail"></param>
        /// <returns></returns>
        public static short ExToShort(this object obj, short whenFail = -1)
        {
            if (obj == null) return whenFail;
            short convertValue;
            return short.TryParse(obj.ToString(), out convertValue) ? convertValue : whenFail;
        }

        /// <summary>
        /// 转成long类型
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="whenFail"></param>
        /// <returns></returns>
        public static long ExToLong(this object obj, long whenFail = -1)
        {
            if (obj == null) return whenFail;
            long convertValue;
            return long.TryParse(obj.ToString(), out convertValue) ? convertValue : whenFail;
        }

        /// <summary>
        /// 转成double类型
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="whenFail"></param>
        /// <returns></returns>
        public static double ExToDouble(this object obj, int whenFail = -1)
        {
            if (obj == null) return whenFail;

            double convertValue;
            return double.TryParse(obj.ToString(), out convertValue) ? convertValue : whenFail;
        }

        /// <summary>
        /// 转成decimal类型
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="whenFail"></param>
        /// <returns></returns>
        public static decimal ExToDecimal(this object obj, int whenFail = -1)
        {
            if (obj == null) return whenFail;

            decimal value;
            return decimal.TryParse(obj.ToString(), out value) ? value : whenFail;
        }

        /// <summary>
        /// 转成日期类型, 如果转换失败默认日期使用1970-1-1
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DateTime ExToDateOr1970(this object obj)
        {
            return obj.ExToDate(new DateTime(1970, 1, 1));
        }

        /// <summary>
        /// 转换日期类型, 如果转换失败默认日期使用现在时间
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DateTime ExToDateOrNow(this object obj)
        {
            return obj.ExToDate(DateTime.Now);
        }

        /// <summary>
        /// 转换成日期类型
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="whenFail"></param>
        /// <returns></returns>
        public static DateTime ExToDate(this object obj, DateTime whenFail = default(DateTime))
        {
            if (obj == null) return whenFail;
            DateTime dt;
            return DateTime.TryParse(obj.ToString(), out dt) ? dt : whenFail;
        }
    }
}
