using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SmartMirrorRpPi
{
    public class WeatherApiConnection
    {
        static string apiKey = "bfb998299994c72ace91dc32f3a5b565";
        static string apiBaseUrl = "https://api.openweathermap.org/data/2.5/weather";

        public static async Task<string> LoadDataAsync(string cityName)
        {
            string apiCall = apiBaseUrl + "?q=" + cityName + "&apikey=" + apiKey + "&mode=xml";
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
