using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace BatchPrintProgram
{
    class JsonHelper
    {
        /// <summary>
        /// 解析JSON数组生成对象实体集合
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="json">json数组</param>
        /// <returns>对象实体集合</returns>
        public static List<T> DeserializeJsonToList<T>(string json) where T : class
        {
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(json);
            object obj = serializer.Deserialize(new JsonTextReader(sr), typeof(List<T>));
            List<T> list = obj as List<T>;
            return list;
        }
    }
}
