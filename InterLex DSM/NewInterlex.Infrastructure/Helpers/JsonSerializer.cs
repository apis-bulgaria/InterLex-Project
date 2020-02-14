namespace NewInterlex.Infrastructure.Helpers
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public sealed class JsonSerializer
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
//            NullValueHandling = NullValueHandling.Ignore
        };

        public static string SerializeObject(object o)
        {
            return JsonConvert.SerializeObject(o, Formatting.Indented, Settings);
        }

        public static T DeserializeObject<T>(string str) where T: class
        {
            return JsonConvert.DeserializeObject<T>(str, Settings);
        }
    }
}