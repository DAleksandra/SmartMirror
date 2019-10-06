using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace SmartMirrorRpPi
{
    public struct WeatherDataEntry
    {
        public string City { set; get; }
        public float Temperature { set; get; }
        public float Pressure { set; get; }
        public float Humidity { set; get; }
        public float Wind { set; get; }
        public string weatherIcon { get; set; }
    }
}
