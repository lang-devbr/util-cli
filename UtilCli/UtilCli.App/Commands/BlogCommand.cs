using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using UtilCli.App.Shared;

namespace UtilCli.App.Commands
{
    public class BlogCommand
    {
        private readonly IConfigurationRoot? _configuration;

        public BlogCommand(IConfigurationRoot? configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> Execute(string[] args)
        {
            string blogSection = string.Empty;

            if (args.Length > 1 && !string.IsNullOrEmpty(args[1]))
                blogSection += args[1];

            if (string.IsNullOrEmpty(blogSection) && _configuration != null)
                blogSection = _configuration.GetSection("blog").Value;

            if (string.IsNullOrEmpty(blogSection))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Argument not found. For help use -h.");
                return false;
            }

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://devblogs.microsoft.com");
                var response = await client.GetAsync($"/{blogSection}/");

                if(!response.IsSuccessStatusCode)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Error to get blog informations. For help use -h.");
                    return false;
                }

                HtmlDocument d = new HtmlDocument();
                d.Load(ConsoleUtil.GenerateStreamFromString(response.Content.ReadAsStringAsync().Result));

                var titles = d.DocumentNode.SelectNodes("//div[@class='entry-content col-md-8']");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"Blog - Section: {blogSection} \r\n");
                ConsoleUtil.CreateConsoleLine(Console.WindowWidth);
                Console.Write($"\r\n");
                foreach (var item in titles)
                {
                    string url = item.SelectSingleNode("//h5[@class='entry-title']").ChildNodes.Single().Attributes["href"].Value;

                    Console.WriteLine($"{item.ChildNodes[1].ChildNodes[1].InnerText} " +
                        $"[{item.ChildNodes[1].ChildNodes[3].ChildNodes[1].ChildNodes[3].ChildNodes[0].InnerText} " +
                        $"- {item.ChildNodes[1].ChildNodes[3].ChildNodes[3].ChildNodes[0].InnerText.Replace("\t", string.Empty).Replace("\n", string.Empty)}]");
                    Console.WriteLine($"{url}");
                    ConsoleUtil.CreateConsoleLine(Console.WindowWidth, "-");
                }
                Console.Write($"\r\n");
            }

            return await Task.FromResult(true);
        }
    }
}
