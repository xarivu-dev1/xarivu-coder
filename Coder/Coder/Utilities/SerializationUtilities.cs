using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Xarivu.Coder.Utilities
{
    public class SerializationUtilities
    {
        public static JsonSerializerSettings GetDefaultSettings(bool indented = true, JsonConverter[] converters = null)
        {
            return new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = indented ? Formatting.Indented : Formatting.None,
                Converters = converters
            };
        }

        /// <summary>
        /// Serialize object to json string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="indented"></param>
        /// <param name="converters"></param>
        /// <returns></returns>
        public static string SerializeItem<T>(T item, bool indented = true, JsonConverter[] converters = null)
        {
            if (item == null) return null;
            return JsonConvert.SerializeObject(item, GetDefaultSettings(indented, converters));
        }

        /// <summary>
        /// Deserialize string to object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <param name="converters"></param>
        /// <returns></returns>
        public static T DeserializeItem<T>(string json, JsonConverter[] converters = null)
        {
            if (string.IsNullOrWhiteSpace(json)) return default(T);

            var settings = GetDefaultSettings(false, converters);
            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        public static string FromatJsonString(string json)
        {
            if (json == null) return null;
            return JToken.Parse(json).ToString(Formatting.Indented);
        }

        public static bool TryFormatJsonString(string json, out string formattedJson)
        {
            formattedJson = null;

            try
            {
                formattedJson = FromatJsonString(json);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Serialize object to json string and save to file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="indented"></param>
        /// <param name="converters"></param>
        /// <returns></returns>
        public static async Task SerializeItemToFileAsync<T>(string filePath, T item, bool indented = true, JsonConverter[] converters = null)
        {
            var json = SerializationUtilities.SerializeItem(item, indented, converters);
            if (json == null) json = string.Empty;

            Encoding encoding = Encoding.UTF8;
            using (FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
            {
                using (StreamWriter sw = new StreamWriter(stream, encoding))
                {
                    await sw.WriteAsync(json);
                }
            }
        }

        public static async Task<T> DeserializeItemFromFileAsync<T>(string filePath, JsonConverter[] converters = null)
        {
            string fileContent = null;
            Encoding encoding = Encoding.UTF8;
            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None, 4096, true))
            {
                using (StreamReader sw = new StreamReader(stream, encoding))
                {
                    fileContent = await sw.ReadToEndAsync();
                }
            }

            if (fileContent == null) return default(T);

            return DeserializeItem<T>(fileContent, converters);
        }
    }
}
