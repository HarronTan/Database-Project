using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json;
using Group_8_Project;

namespace csharp_api
{
    class apipull
    {
        HttpClient client = new HttpClient();
        public static string key;
        public async Task GetResponse()
        {
            string response = await client.GetStringAsync(
                "https://api.data.gov.sg/v1/transport/carpark-availability"); // URL


            Root root = JsonConvert.DeserializeObject<Root>(response);
            foreach (var item in root.items[0].carpark_data)
            {   

                Redis.SaveData("localhost", item.carpark_number, Convert.ToInt32(item.carpark_info[0].lots_available));

            }

        }

        

    }

    public class CarparkInfo
    {
        public string lot_type { get; set; }
        public string lots_available { get; set; }
        public string total_lots { get; set; }
    }

    public class CarparkData
    {
        
        public List<CarparkInfo> carpark_info { get; set; }
        public string carpark_number { get; set; }
        public DateTime update_datetime { get; set; }
    }

    public class Item
    {
        public List<CarparkData> carpark_data { get; set; }
        public DateTime timestamp { get; set; }
    }

    public class Root
    {
        public List<Item> items { get; set; }
    }

}
