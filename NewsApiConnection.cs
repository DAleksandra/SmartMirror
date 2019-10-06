using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SmartMirrorRpPi
{
    class NewsApiConnection
    {
        static string apiKey = "0c2b3764a92749ec9c932340cd3746ae";
        static string apiBaseUrl = "https://newsapi.org/v2/everything?sources=bbc-news&sortBy=popularity&apiKey=0c2b3764a92749ec9c932340cd3746ae";

        public static async Task<string> LoadDataAsync()
        {
            string apiCall = apiBaseUrl + "&apikey=" + apiKey;
            Task<string> result;
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(apiCall))
            using (HttpContent content = response.Content)
            {
                result = content.ReadAsStringAsync();
            }
            return await result;
        }
    }
}
