using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace Utilities
{
    public static class JsonHelper
    {
        public static bool IsValidJsonObjectString(string val)
        {
            try
            {
                JToken.Parse(val);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static bool IsValidJsonObjectPerScheme(string val, string schema, out JObject jsonObj)
        {
            jsonObj = null;
            if (!IsValidJsonObjectString(val)) return false;
            if (!IsValidJsonObjectString(schema))
                throw new Exception("Invalid Json schema");

            var jsonSchema = JsonSchema.Parse(schema);
            jsonObj = JObject.Parse(val);
            return jsonObj.IsValid(jsonSchema);
        }

        public static T GetValue<T>(string key, JObject jsonObject)
        {
            return jsonObject.Value<T>(key);
        }

        public static JObject ExtractJsonObject(string content)
        {
            var rx = new System.Text.RegularExpressions.Regex(@"\{(.|\s)*\}");
            var jsonString = rx.Match(content).Value;
            return !IsValidJsonObjectString(jsonString) ? null : JsonConvert.DeserializeObject<JObject>(jsonString);
        }

    }
}
