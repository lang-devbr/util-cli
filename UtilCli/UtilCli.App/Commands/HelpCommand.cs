using Microsoft.Extensions.Configuration;

namespace UtilCli.App.Commands
{
    public class HelpCommand
    {
        private readonly IConfigurationRoot? _configuration;

        public HelpCommand(IConfigurationRoot? configuration)
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
