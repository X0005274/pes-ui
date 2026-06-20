using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Pes.Ui.Messaging
{
    /// <summary>Java(camelCase, null 생략) 와 동일한 JSON 직렬화 규약.</summary>
    public static class PesJson
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        public static string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value, Settings);
        }

        public static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, Settings);
        }
    }
}
