using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMirrorRpPi
{
    class ParseNews
    {
        static int recentArticle = 0;
        public static NewsData Parse(System.IO.Stream stream)
        {
                
                StreamReader reader = new StreamReader(stream);
                string json = reader.ReadToEnd();
                JObject rss = JObject.Parse(json);

                string rssTitle = rss["articles"][recentArticle]["title"].ToString();
                string rssDescription = rss["articles"][recentArticle]["description"].ToString();
                string rssUrl = rss["articles"][recentArticle]["url"].ToString();
                recentArticle++;
                int numberOfArticles = (int)rss["totalResults"];

                if (recentArticle == numberOfArticles)
                     recentArticle = 0;

                return new NewsData()
                 {
                     title = rssTitle,
                     description = rssDescription,
                     url = rssUrl

                };

            
        }
    }
}
