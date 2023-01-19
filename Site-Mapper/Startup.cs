namespace Site_Mapper
{
    using Site_Mapper.Services.Interfaces;

    internal class Startup
    {
        private readonly IUrlServise _urlService;
        private readonly IWatchService _watchService;

        public Startup(IUrlServise servise, IWatchService watchService)
        {
            _watchService = watchService;
            _urlService = servise;
        }

        public static string BaseUrl;
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
            while (true)
            {
                Console.WriteLine("write url");
                string url = Console.ReadLine();

                if (!string.IsNullOrEmpty(url))
                {
                    try
                    {
                        Uri uriAddress = new Uri(url);
                        BaseUrl = uriAddress.Scheme + "://" + uriAddress.Host;

                        Parallel.Invoke(() => GetUrls(url), () =>
                        {
                            foreach (var a in _urlService.GetSitemapUrls(url))
                            {
                                _sitemapUrls.Add(a);
                            }
                        });

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

                        Console.WriteLine();
                        Console.WriteLine("Urls FOUNDED IN SITEMAP.XML but not founded after crawling a web site");
                        foreach (var (item, index) in sitemaplist1.Select((it, ind) => (it, ind + 1)))
                        {
                            Console.WriteLine($"{index})  {item}");
                        }

                        Console.WriteLine();
                        Console.WriteLine("Urls FOUNDED BY CRAWLING THE WEBSITE but not in sitemap.xml\r\n");
                        foreach (var (item, index) in crawlinglist2.Select((it, ind) => (it, ind + 1)))
                        {
                            Console.WriteLine($"{index})  {item}");
                        }

                        Console.WriteLine();
                        Console.WriteLine("Url                time ms");
                        foreach (var (item, index) in _watchService.WatchRequestAsync(_allUrls).Result.Select((it, ind) => (it, ind + 1)))
                        {
                            Console.WriteLine($"{index}) {item.Key}    {item.Value} ms");
                        }

                        Console.WriteLine($" total urls count is: {_allUrls.Count}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    finally
                    {
                        _allUrls.Clear();
                        _sitemapUrls.Clear();
                    }
                }
            }
        }
    }
}
