using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMirrorRpPi
{
    public struct NewsData
    {
        public string author { set; get; }
        public string title { set; get; }
        public string description { set; get; }
        public string content { set; get; }
        public string urlToImage { set; get; }
        public string publishedAt { set; get; }
        public string url { set; get; }
    }
}
