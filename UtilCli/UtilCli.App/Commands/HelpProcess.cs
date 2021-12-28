using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;

namespace UtilCli.App.Commands
{
    public class HelpProcess
    {
        private readonly IConfigurationRoot? _configuration;

        public HelpProcess(IConfigurationRoot? configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> Execute(string[] args)
        {
            Console.WriteLine("Help");



            return await Task.FromResult(true);
        }
    }
}
