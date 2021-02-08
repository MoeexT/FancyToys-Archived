using FancyServer.Log;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyServer.Utils
{
    class JsonUtil
    {
        public static bool ParseStruct<T>(string content, out T sdu)
        {
            try
            {
                sdu = JsonConvert.DeserializeObject<T>(content);
                return true;
            }
            catch (JsonException e)
            {
                LogClerk.Warn($"Deserialize object failed: {e.Message}");
                sdu = default;
                return false;
            }
        }
    }
}
