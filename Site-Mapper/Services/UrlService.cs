namespace Site_Mapper.Services
{
    using HtmlAgilityPack;
    using Site_Mapper.Services.Interfaces;
    using Site_Mapper.Services.Logger;
    using System.Net;
    using System.Xml;

    internal class UrlService : IUrlService
    {
        public IEnumerable<string> GetUrls(string url)
        {
            HtmlWeb hw = new HtmlWeb();
            HtmlDocument doc;

            if (url.StartsWith("/"))
            {
                url = Startup._baseUrl + url;
            }

            try
            {
                doc = hw.Load(url);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                LogsFolder.Logs.Add(ex.Message);
                yield break;
            }

            var res = doc.DocumentNode.SelectNodes("//a[@href]");

            if (res != null)
            {
                foreach (HtmlNode link in res)
                {
                    var gotUrl = link.GetAttributeValue("href", "");

                    if (gotUrl.StartsWith(Startup._baseUrl))
                    {
                        yield return gotUrl;
                        continue;
                    }

                    if (gotUrl.StartsWith("/"))
                    {
                        yield return Startup._baseUrl + gotUrl;
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
                sitemapString = wc.DownloadString(Startup._baseUrl + "/sitemap.xml");
                urldoc.LoadXml(sitemapString);
            }
            catch (Exception ex)
            {
                LogsFolder.Logs.Add(ex.Message);
                Console.WriteLine(ex.Message);
                yield break;
            }

            if (urldoc != null)
            {
                XmlNodeList xmlSitemapList = urldoc.GetElementsByTagName("url");
                if (xmlSitemapList.Count <= 0)
                {
                    xmlSitemapList = urldoc.GetElementsByTagName("loc");
                    foreach (XmlNode node in xmlSitemapList)
                    {
                        yield return node.InnerText;
                    }
                }
                else
                {
                    foreach (XmlNode node in xmlSitemapList)
                    {
                        if (node["loc"] != null)
                        {
                            if (node["loc"].InnerText.StartsWith("/"))
                            {
                                yield return Startup._baseUrl + node["loc"].InnerText;
                                continue;
                            }

                            yield return node["loc"].InnerText;
                        }
                    }
                }
            }
        }
    }
}
