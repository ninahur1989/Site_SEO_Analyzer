namespace Site_Mapper.Services
{
    using HtmlAgilityPack;
    using Site_Mapper.Services.Interfaces;
    using System.Net;
    using System.Xml;

    internal class UrlServise : IUrlServise
    {
        public IEnumerable<string> GetUrls(string url)
        {
            HtmlWeb hw = new HtmlWeb();
            HtmlDocument doc;

            if (url.StartsWith("/"))
            {
                url = Startup.BaseUrl + url;
            }

            try
            {
                doc = hw.Load(url);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                yield break;
            }

            var res = doc.DocumentNode.SelectNodes("//a[@href]");

            if (res != null)
            {
                foreach (HtmlNode link in res)
                {
                    var gotUrl = link.GetAttributeValue("href", "");

                    if (gotUrl.StartsWith(Startup.BaseUrl))
                    {
                        yield return gotUrl;
                        continue;
                    }

                    if (gotUrl.StartsWith("/"))
                    {
                        yield return Startup.BaseUrl + gotUrl;
                    }
                }
            }
            yield break;
        }

        public IEnumerable<string> GetSitemapUrls(string url)
        {
            WebClient wc = new WebClient();
            string sitemapString;
            XmlDocument urldoc = new XmlDocument();

            try
            {
                sitemapString = wc.DownloadString(Startup.BaseUrl + "/sitemap.xml");
                urldoc.LoadXml(sitemapString);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                yield break;
            }

            if (urldoc != null)
            {
                XmlNodeList xmlSitemapList = urldoc.GetElementsByTagName("url");

                foreach (XmlNode node in xmlSitemapList)
                {
                    if (node["loc"] != null)
                    {
                        yield return node["loc"].InnerText;
                    }
                }
            }
        }
    }
}
