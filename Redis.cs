using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Group_8_Project
{
    public class Redis
    {
        public static void SaveData(string host, string key, int value)
        {
            using (RedisClient client = new RedisClient(host))
                if (client.Get<string>(key) == null)
                {
                    client.Set(key, value);
                    
                }
        }

        public static string ReadData(string host, string key)
        {
            using (RedisClient client = new RedisClient(host))
            {
                return client.Get<string>(key);
            }

        }

    }
}
