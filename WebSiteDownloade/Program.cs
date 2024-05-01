using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LinkSearch
{
    internal class Program
    {
        private static HttpClient client = new HttpClient();
        static string path = "C:\\Users\\User\\Desktop\\StudyZ\\LongCentrArchive.html";
        static StreamWriter sw = new StreamWriter(path);

        static async Task Main(string[] args)
        {
            //File.Create(path);
            var visited = new HashSet<string>();
            var urls = await CrawlRecursive("https://web.archive.org/web/20191227022509/http://www.long-center.org/index.php/essay", visited);
            foreach (var url in urls)
            {
                Console.WriteLine(url);
            }
            sw.Close();
        }

        private static Regex imageRegex = new Regex(@"(png|ttf|jpeg|jpg|svg)$");

        private static async Task<IEnumerable<string>> CrawlRecursive(string initialUrl, HashSet<string> visited)
        {
            visited.Add(initialUrl);
            var urls = await FindAllUrls(initialUrl);
            urls = urls.Where(u => !visited.Contains(u));

            var result = new HashSet<string>();
            foreach (var url in urls)
            {
                if (imageRegex.IsMatch(url))
                    continue;
                var newUrls = await CrawlRecursive(url, visited);
                foreach (var one in newUrls)
                {
                    result.Add(one);
                }
            }
            return result;
        }

        private static async Task<IEnumerable<string>> FindAllUrls(string url)
        {
            try
            {
                var html = await client.GetStringAsync(url);
                Console.WriteLine(html);
                sw.WriteLine(html);

                var urlRegex = new Regex("https?://[^\"')]*");
                var matches = urlRegex.Matches(html);
                return matches.Select(match => match.Value);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error crawling {url}: {ex.Message}");
                return Enumerable.Empty<string>();
            }
        }
    }
}