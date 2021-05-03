using Newtonsoft.Json;

namespace FancyUtil
{
    class JsonUtil
    {
        public delegate bool Logger(string content);

        public static bool ParseStruct<T>(string content, out T sdu)
        {
            try
            {
                sdu = JsonConvert.DeserializeObject<T>(content);
                return true;
            }
            catch (JsonException e)
            {
                // LogClerk.Warn($"Deserialize object failed: {e.Message}");
                sdu = default;
                return false;
            }
        }
    }
}
