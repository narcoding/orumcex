using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace orumcex
{
    public partial class turkishdrama : Form
    {


        public turkishdrama()
        {
            InitializeComponent();
        }

        private void turkishdrama_Load(object sender, EventArgs e)
        {
            String link = "https://www.turkishdrama.com/people/feed";
            //String link = "https://www.turkishdrama.com/tv-series/feed";

            string xml = GetContent(link);

            XmlDocument doc = new XmlDocument();

            doc.LoadXml(xml);

            string json = JsonConvert.SerializeXmlNode(doc);

            Rootobject rssObject= (Rootobject)getObject(json);

            Item[] mc=rssObject.rss.channel.item;

            SeriesItem[] msis = new SeriesItem[190]; //people items count from site
            //SeriesItem[] msis = new SeriesItem[153]; //series items count from site

            int i=0;
            foreach (Item item in mc)
            {
                msis[i] = new SeriesItem();
                msis[i].title=item.title;
                msis[i].link = item.link;
                //msis[i].article = item.description;
                i++;
            }

            for (int j = 0; j < msis.Length; j++) {

                String htmlcontent = GetContent(msis[j].link);
                //burdan devam htmlcontentten imageurl yi bul çek
                                             
                HtmlAgilityPack.HtmlDocument docx = new HtmlAgilityPack.HtmlDocument();

                docx.LoadHtml(htmlcontent);

                var seriesImage = docx.DocumentNode
                    .Descendants("img")
                    .Where(d =>
                       d.Attributes.Contains("class")
                       &&
                       d.Attributes["class"].Value.Contains("alignleft wp-post-image")
                    );
                var img = seriesImage.FirstOrDefault().Attributes["src"].Value;

                msis[j].imageUrl = img;

                var seriesArticle = docx.DocumentNode
                    .Descendants("article")
                    .Where(d =>
                       d.Attributes.Contains("id")
                       &&
                       d.Attributes["id"].Value.Contains("the-post")
                    );
                var article = seriesArticle.FirstOrDefault();

                msis[j].article = article.OuterHtml;



            }
            SeriesItems msits =new SeriesItems();
            msits.mSeriesItems = msis;
            String mJson=Newtonsoft.Json.JsonConvert.SerializeObject(msits);

            textBox1.Text = mJson;

            //Console.WriteLine(mJson);
        }

        private static string GetContent(string urlAddress)
        {
            Uri url = new Uri(urlAddress);
            WebClient client = new WebClient();
            client.Encoding = System.Text.Encoding.UTF8;

            string xml = client.DownloadString(url);
            return xml;
        }

        private static Rootobject getObject(String json) {

            Rootobject newTarget=null;
            try {
                newTarget = JsonConvert.DeserializeObject<Rootobject>(json);
                
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }

            return newTarget;
        }

        public class Rootobject
        {
            public Rss rss { get; set; }
        }

        public class Rss
        {
            public Channel channel { get; set; }
        }

        public class Channel
        {
            public Item[] item { get; set; }
        }

        public class Item
        {
            public string title { get; set; }
            public string link { get; set; }
            //public string description { get; set; }
        }


        public class SeriesItems
        {
            public SeriesItem[] mSeriesItems { get; set; }  //mPeopleItems
        }

        public class SeriesItem {

            public String title { get; set; }
            public String link { get; set; }
            public String imageUrl { get; set; }
            public String article { get; set; }

        }
        

    }

    
}
