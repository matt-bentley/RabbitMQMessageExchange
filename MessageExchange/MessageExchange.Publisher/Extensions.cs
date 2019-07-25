using Newtonsoft.Json;
using System;
using System.Text;

namespace MessageExchange.Publisher
{
    internal static class Extensions
    {
        internal static byte[] Serialize(this Object obj)
        {
            if (obj == null)
                return null;
            var json = JsonConvert.SerializeObject(obj);
            return Encoding.ASCII.GetBytes(json);
        }
    }
}
