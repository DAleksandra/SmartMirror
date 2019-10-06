using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.UI.Xaml.Media;

namespace SmartMirrorRpPi
{
    class ParseWeatherLinq
    {
        public static WeatherDataEntry Parse(System.IO.Stream stream)
        {
            XElement xml = XElement.Load(stream);
            var nameQuery = (from element in xml.Elements()
                             let elementName = element.Name
                             where (elementName == "city")
                             select new
                             {
                                 City = element.Attributes("name").FirstOrDefault(),
                             });
            var temperatureQuery = (from element in xml.Elements()
                                    let elementName = element.Name
                                    where (elementName == "temperature")
                                    select new
                                    {
                                        Temperature = element.Attributes("value").FirstOrDefault(),
                                    });
            var humidityQuery = (from element in xml.Elements()
                                 let elementName = element.Name
                                 where (elementName == "humidity")
                                 select new
                                 {
                                     Humidity = element.Attributes("value").FirstOrDefault(),
                                 });
            var pressureQuery = (from element in xml.Elements()
                                 let elementName = element.Name
                                 where (elementName == "pressure")
                                 select new
                                 {
                                     Pressure = element.Attributes("value").FirstOrDefault(),
                                 });
            var windQuery = (from element in xml.Elements()
                             let elementName = element.Name
                             where (elementName == "wind.speed")
                             select new
                             {
                                 Wind = element.Attributes("value").FirstOrDefault(),
                             });
            var iconQuery = (from element in xml.Elements()
                             let elementName = element.Name
                             where (elementName == "weather")
                             select new
                             {
                                 WeatherIcon = element.Attributes("icon").FirstOrDefault(),
                             });

            return new WeatherDataEntry()
            {
                City = nameQuery.FirstOrDefault().City.Value,
                Temperature = float.Parse(
                    temperatureQuery.FirstOrDefault().Temperature.Value,
                    System.Globalization.CultureInfo.InvariantCulture),
                Pressure = float.Parse(
                    pressureQuery.FirstOrDefault().Pressure.Value),

                Humidity = float.Parse(
                    humidityQuery.FirstOrDefault().Humidity.Value),
                weatherIcon = (string) iconQuery.FirstOrDefault().WeatherIcon.Value
            };
        }

    }

}
