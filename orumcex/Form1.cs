using System;
using System.Net;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace orumcex
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        
        private void Form1_Load(object sender, EventArgs e)
        {
            String link = "http://turkishseries.li";

            string htmlcontent = GetContent(link);

            DizilerJson dJson = new DizilerJson();


            HtmlAgilityPack.HtmlDocument dokuman = new HtmlAgilityPack.HtmlDocument();

            dokuman.LoadHtml(htmlcontent);
            try
            {
                // Html dökümanı içndeki h2 etiketlerinden class'ı  tutheader olanları liste halinde alıyoruzç     
                var dizinodelar = dokuman.DocumentNode
                    .Descendants("li")
                    .Where(d =>
                       d.Attributes.Contains("class")
                       &&
                       d.Attributes["class"].Value.Contains("cat-item")
                    );

                List<DiziAnasayfa> diziler = new List<DiziAnasayfa>();
                foreach (HtmlAgilityPack.HtmlNode baslik in dizinodelar)
                {
                    
                    DiziAnasayfa mDizi = new DiziAnasayfa();
                    mDizi.Ad = baslik.InnerText;
                    mDizi.Url = baslik.SelectNodes("a").FirstOrDefault().Attributes["href"].Value;
                    
                    diziler.Add(mDizi);
                }

                foreach (DiziAnasayfa item in diziler)
                {
                    string dizicontent = GetContent(item.Url);
                    //HtmlAgilityPack.HtmlDocument dokuman2 = new HtmlAgilityPack.HtmlDocument();
                    dokuman.LoadHtml(dizicontent);
                    var bolumnodeler = dokuman.DocumentNode
                    .Descendants("div")
                    .Where(d =>
                       d.Attributes.Contains("class")
                       &&
                       d.Attributes["class"].Value.Contains("archive-text")
                    );
                    
                    List<DiziBolum> bolumler = new List<DiziBolum>();
                    foreach (var bolum in bolumnodeler) {
                        
                        DiziBolum mBolum = new DiziBolum();
                        mBolum.BolumAd = bolum.Descendants("a").FirstOrDefault().InnerText;
                        mBolum.Url= bolum.Descendants("a").FirstOrDefault().Attributes["href"].Value;
                        
                        bolumler.Add(mBolum);
                    }

                    item.Bolumler = bolumler;
                }

                foreach (DiziAnasayfa dx in diziler)
                {
                    foreach (DiziBolum b in dx.Bolumler)
                    {
                        string bolumcontent = GetContent(b.Url);
                        dokuman.LoadHtml(bolumcontent);
                        //string a = "";
                        try
                        {
                            var bolumvideonodeler = dokuman.DocumentNode
                           .Descendants("div")
                           .Where(d =>
                              d.Attributes.Contains("id")
                              &&
                              d.Attributes["id"].Value.Contains("content-area")
                           ).FirstOrDefault().Descendants("p").Skip(1).FirstOrDefault().Descendants("iframe").FirstOrDefault();
                            b.VideoUrl = bolumvideonodeler.Attributes["src"].Value;
                        }
                        catch (Exception exd)
                        {
                            b.VideoUrl = "";
                            MessageBox.Show(exd.Message);
                        }
                        
                    }
                }

                dJson.dizilerJson = diziler;
                listBox1.DataSource = diziler;
                listBox1.DisplayMember = "Url";
                String jsoum = getJson(dJson);
                Console.WriteLine(jsoum);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private static string GetContent(string urlAddress)
        {
            Uri url = new Uri(urlAddress);
            WebClient client = new WebClient();
            client.Encoding = System.Text.Encoding.UTF8;

            string html = client.DownloadString(url);
            return html;
        }

        public static string getJson(Object o) {
            string json = JsonConvert.SerializeObject(o);
            return json;
        }

    }

    public class DizilerJson {
        public List<DiziAnasayfa> dizilerJson;
    }

    public class DiziAnasayfa
    {
        public string Ad { get; set; }
        public string Url { get; set; }
        public List<DiziBolum> Bolumler { get; set; }

    }

    public class DiziBolum
    {
        public string BolumAd { get; set; }
        public string Url { get; set; }
        public string VideoUrl { get; set; }
    }

}
