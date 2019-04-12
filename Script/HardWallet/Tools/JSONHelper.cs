using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Data;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections;

namespace Common
{
    /// <summary>
    /// JSON 帮助类
    /// </summary>
    public class JSONHelper
    {
        /// <summary>
        /// 将JSON转换为指定类型的对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="json">json字符串</param>
        /// <returns></returns>
        public static T ConvertToObject<T>(string json)
        {
            var jsetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            //jsetting.DefaultValueHandling = DefaultValueHandling.Include;
            return JsonConvert.DeserializeObject<T>(json, jsetting);
        }


        /// <summary>
        /// 生成压缩的json 字符串
        /// </summary>
        /// <param name="obj">生成json的对象</param>
        /// <returns></returns>
        public static string ToJson(object obj)
        {
            return ToJson(obj, false);
        }

        /// <summary>
        /// 生成JSON字符串
        /// </summary>
        /// <param name="obj">生成json的对象</param>
        /// <param name="formatjson">是否格式化</param>
        /// <returns></returns>
        public static string ToJson(object obj, bool formatjson)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            IsoDateTimeConverter idtc = new IsoDateTimeConverter();
            idtc.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(idtc);
            JsonWriter jw = new JsonTextWriter(sw);

            if (formatjson)
            {
                jw.Formatting = Formatting.Indented;
            }

            serializer.Serialize(jw, obj);

            //JsonConvert.SerializeObject(dt, idtc).ToString();

            return sb.ToString();
        }

        public static DateTime JsonToDateTime(string jsonDate)
        {
            string value = jsonDate.Substring(6, jsonDate.Length - 8);
            DateTimeKind kind = DateTimeKind.Utc;
            int index = value.IndexOf('+', 1);
            if (index == -1)
                index = value.IndexOf('-', 1);
            if (index != -1)
            {
                kind = DateTimeKind.Local;
                value = value.Substring(0, index);
            }
            long javaScriptTicks = long.Parse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
            long InitialJavaScriptDateTicks = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks;
            DateTime utcDateTime = new DateTime((javaScriptTicks * 10000) + InitialJavaScriptDateTicks, DateTimeKind.Utc);
            DateTime dateTime;
            switch (kind)
            {
                case DateTimeKind.Unspecified:
                    dateTime = DateTime.SpecifyKind(utcDateTime.ToLocalTime(), DateTimeKind.Unspecified);
                    break;
                case DateTimeKind.Local:
                    dateTime = utcDateTime.ToLocalTime();
                    break;
                default:
                    dateTime = utcDateTime;
                    break;
            }
            return dateTime;
        }    

      
        public static string FormatJSONForJQgrid(int totalpages, int pageindex, int recordcount, object list)
        {
            var json = new { total = totalpages, page = pageindex, records = recordcount, rows = list };
            return ToJson(json);
        }      
       
        /// <summary>
        /// 格式化EASYUI DATAGRID JSON
        /// </summary>
        /// <param name="recordcount">总记录数</param>
        /// <param name="rows">每页记录的JSON格式</param>
        /// <returns></returns>
        public static string FormatJSONForEasyuiDataGrid(int recordcount, object rowsList)
        {
            return ToJson(new { total = recordcount, rows = rowsList });
        }

    }
}

