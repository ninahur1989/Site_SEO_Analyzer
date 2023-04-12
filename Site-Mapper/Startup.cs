namespace Site_Mapper
{
    using Microsoft.EntityFrameworkCore;
    using Site_Mapper.Data;
    using Site_Mapper.Data.DB;
    using Site_Mapper.Data.Models;
    using Site_Mapper.Services.Interfaces;
    using Site_Mapper.Services.Logger;

    internal class Startup
    {
        private readonly IUrlService _urlService;
        private readonly IWatchService _watchService;

        public Startup(IUrlService servise, IWatchService watchService)
        {
            _watchService = watchService;
            _urlService = servise;
        }

        public static string _baseUrl;
        public HashSet<string> _allUrls = new HashSet<string>();
        public HashSet<string> _sitemapUrls = new HashSet<string>();

        private void GetUrls(string url)
        {
            foreach (var a in _urlService.GetUrls(url))
            {
                var count = _allUrls.Count;
                _allUrls.Add(a);
                if (count != _allUrls.Count)
                {
                    GetUrls(a);
                }
            }
        }

        public void Start()
        {
            bool program = true;
            while (program)
            {
                try
                {
                    Console.WriteLine("choose a comand");
                    Console.WriteLine("1. check site");
                    Console.WriteLine("2. get list");
                    Console.WriteLine("3. clear list");
                    Console.WriteLine("4. end");
                    string command = Console.ReadLine();
                    if (string.IsNullOrEmpty(command))
                    {
                        continue;
                    }
                    switch (command)
                    {
                        case "check site":
                            Console.WriteLine("write URL");
                            string url = Console.ReadLine().ToLower();

                            if (!string.IsNullOrEmpty(url))
                            {
                                var info = new SiteInfo()
                                {
                                    Name = url,
                                };

                                try
                                {
                                    if (!url.StartsWith("http"))
                                    {
                                        throw new Exception("you missed http/https in your URL");
                                    }
                                    Uri uriAddress = new Uri(url);
                                    _baseUrl = uriAddress.Scheme + "://" + uriAddress.Host;

                                    Parallel.Invoke(() => GetUrls(url), () =>
                                    {
                                        foreach (var a in _urlService.GetSitemapUrls(url))
                                        {
                                            _sitemapUrls.Add(a);
                                        }
                                    });


                                    info.CountSiteMapUrls = _sitemapUrls.Count;
                                    info.CountUrls = _allUrls.Count;


                                    Console.WriteLine($"Urls(html documents) found after crawling a website:{_allUrls.Count}");
                                    Console.WriteLine($"Urls found in sitemap:{_sitemapUrls.Count}");

                                    var crawlinglist1 = new HashSet<string>(_allUrls);
                                    var sitemaplist1 = new HashSet<string>(_sitemapUrls);
                                    var crawlinglist2 = new HashSet<string>(_allUrls);
                                    var sitemaplist2 = new HashSet<string>(_sitemapUrls);

                                    sitemaplist1.ExceptWith(crawlinglist1);
                                    crawlinglist2.ExceptWith(sitemaplist2);
                                    crawlinglist1.Clear();
                                    sitemaplist2.Clear();

                                    _allUrls.UnionWith(sitemaplist1);

                                    Console.WriteLine("\rUrls FOUNDED IN SITEMAP.XML but not founded after crawling a web site\r");
                                    foreach (var (item, index) in sitemaplist1.Select((it, ind) => (it, ind + 1)))
                                    {
                                        Console.WriteLine($"{index})  {item}");
                                    }

                                    Console.WriteLine();
                                    Console.WriteLine("\rUrls FOUNDED BY CRAWLING THE WEBSITE but not in sitemap.xml\r\n");
                                    foreach (var (item, index) in crawlinglist2.Select((it, ind) => (it, ind + 1)))
                                    {
                                        Console.WriteLine($"{index})  {item}");
                                    }

                                    Console.WriteLine();
                                    Console.WriteLine("All urls                time ms");
                                    Console.WriteLine();
                                    foreach (var (item, index) in _watchService.WatchRequestAsync(_allUrls).Result.Select((it, ind) => (it, ind + 1)))
                                    {
                                        Console.WriteLine($"{index}) {item.Key}    {item.Value} ms");
                                    }

                                    Console.WriteLine($" total urls count is: {_allUrls.Count}");
                                }
                                catch (Exception ex)
                                {
                                    LogsFolder.Logs.Add(ex.Message.ToString());
                                    Console.WriteLine(ex.Message);
                                }
                                finally
                                {
                                    if (LogsFolder.Logs.Count <= 0)
                                    {
                                        using (var context = new SiteContext())
                                        {
                                            info.TotalUrlCount = _allUrls.Count;
                                            info.Date = DateTime.UtcNow;
                                            context.SiteInfos.Add(info);
                                            context.SaveChanges();
                                            Logger logger = new();
                                            logger.WriteLogs();
                                        }
                                    }
                                    _allUrls.Clear();
                                    _sitemapUrls.Clear();
                                }
                            }

                            break;
                        case "get list":
                            using (var context = new SiteContext())
                            {
                                var sitesInfo = context.SiteInfos.AsNoTracking();
                                if (sitesInfo.Count() > 0)
                                {
                                    foreach (var info in sitesInfo)
                                    {
                                        Console.WriteLine($"{info.Id}.  checke in {info.Date}  Name:{info.Name}  have urls in site:{info.CountUrls}" +
                                            $"  have urls in sitemap :{info.CountSiteMapUrls}  have Total urls count:{info.TotalUrlCount}");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("here is no items in list");
                                }
                            }

                            break;
                        case "clear list":
                            using (var context = new SiteContext())
                            {
                                context.SiteInfos.RemoveRange(context.SiteInfos);
                                context.SaveChanges();
                            }

                            Console.WriteLine("All siteinfos successfully deleted");

                            break;
                        case "end":
                            program = false;
                            break;
                        default:
                            Console.WriteLine("You wrote the wrong command");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    LogsFolder.Logs.Add(ex.Message.ToString());
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    if (LogsFolder.Logs.Count != 0)
                    {
                        Logger logger = new();
                        logger.WriteLogs();
                    }
                }
            }
        }
    }
}
