using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net;
using System.IO;
using System.Xml;
using System.Reflection;

namespace WPCParseProvider
{
    public class Parser
    {
        List<string> PossibleResolution = new List<string>();
        List<string> WallpaperURLs = new List<string>();
        HttpWebRequest Request;
        HtmlDocument HtmlDoc = new HtmlDocument();
        string Response;
        public string Theme;
        private string resolution;

        public string Resolution
        {
            get
            {
                return resolution;
            }

            set
            {
                FetchPossibleResolutions();
                if (PossibleResolution.Contains(value))
                resolution = value;
                else Console.WriteLine("Invalid resolution!");
            }
        }

        public string Url
        {
            get
            {
                // https://wallpaperscraft.ru/catalog/hi-tech/1920x1080/page2
                return String.Format("https://wallpaperscraft.ru/catalog/{0}/{1}/", Theme, Resolution);
            }
        }

        public Parser(string theme, string res)
        {
            Theme = theme;
            Resolution = res;
        }

        public void GetWallpapers()
        {
            Init();
            FetchImgNodes();
            DownloadWallpapers();
        }

        public void DownloadWallpapers()
        {
            using (var client = new WebClient())
            {
                foreach (string url in WallpaperURLs)
                {
                    client.DownloadFile(url, "DL\\"+ url.Split('/').Last());
                }
            }
        }

        private void FetchImgNodes()
        {
            var img_node = HtmlDoc.DocumentNode.SelectNodes("//img[contains(@class, 'wallpapers__image')]");
            if (img_node != null)
            {
                foreach (var tag in img_node)
                {
                    if (tag.Attributes["src"] != null)
                    {
                        var source = tag.Attributes["src"].Value;
                        source = source.Substring(0, source.Length - 11);
                        source += Resolution + ".jpg";
#if DEBUG
                        Console.WriteLine(source);
#endif
                        WallpaperURLs.Add(source);
                    }
                }
            }
        }

        public void Init()
        {
            Request = (HttpWebRequest)WebRequest.Create(Url);
            Response = new StreamReader(Request.GetResponse().GetResponseStream()).ReadToEnd();
            HtmlDoc.LoadHtml(Response);
        }

        private void FetchPossibleResolutions()
        {
#if DEBUG
            Console.WriteLine("Fetching resolutions from .xml");
#endif
            XmlDocument xDocument = new XmlDocument();
            try
            {
                xDocument.Load("PossibleRes.xml");
            }
            catch (Exception exp)
            {
                Console.WriteLine("Can't find PossibleRes.xml, it must in same catalog as this .exe!");
                Console.WriteLine("exp Code: {0}", exp.Message);
                throw;
            }
            XmlNodeList AvaliableRes = xDocument.GetElementsByTagName("resolution");
            foreach(XmlNode node in AvaliableRes)
            {
                PossibleResolution.Add(node.InnerText);
            }
            
        }
    }
}
