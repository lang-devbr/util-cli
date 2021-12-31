using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using UtilCli.App.Shared;

namespace UtilCli.App.Commands
{
    public class SicopCommand
    {
        private readonly IConfigurationRoot? _configuration;

        public SicopCommand(IConfigurationRoot? configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> Execute(string[] args)
        {
            string processNumber = string.Empty;

            if (args.Length > 1 && !string.IsNullOrEmpty(args[1]))
                processNumber = args[1];

            if (string.IsNullOrEmpty(processNumber) && _configuration != null)
                processNumber = _configuration.GetSection("protocol-number").Value;

            if (string.IsNullOrEmpty(processNumber))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Argument not found. For help use -h.");
                return false;
            }

            using (HttpClient client = new HttpClient())
            {
                var firstRequest = new Dictionary<string, string>();
                firstRequest.Add("processo", "");
                firstRequest.Add("nova", "Nova+Consulta");

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                client.DefaultRequestHeaders.Add("Referer", "http://www2.rio.rj.gov.br/sicop/con60.asp");
                client.BaseAddress = new Uri("http://www2.rio.rj.gov.br");
                var firstResponse = await client.PostAsync("/sicop/sicop.asp", new FormUrlEncodedContent(firstRequest));

                var secondRequest = new Dictionary<string, string>();
                secondRequest.Add("processo", processNumber);
                secondRequest.Add("bt_consultar", "Consultar");

                var secondResponse = await client.PostAsync("/sicop/sicop.asp", new FormUrlEncodedContent(secondRequest));

                HtmlDocument d = new HtmlDocument();
                d.Load(ConsoleUtil.GenerateStreamFromString(secondResponse.Content.ReadAsStringAsync().Result));

                var despacho = d.DocumentNode.SelectSingleNode("//input[@name='despacho']");

                if (despacho == null)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Process number: {processNumber} not found. For help use -h.");
                    return false;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    
                    var processo = d.DocumentNode.SelectSingleNode("//input[@name='processo']");
                    var requerente = d.DocumentNode.SelectSingleNode("//input[@name='requerente']");
                    var assunto = d.DocumentNode.SelectSingleNode("//input[@name='assunto']");
                    var inform_compl = d.DocumentNode.SelectSingleNode("//input[@name='inform_compl']");
                    var multa = d.DocumentNode.SelectSingleNode("//input[@name='multa']");
                    var data_despacho = d.DocumentNode.SelectSingleNode("//input[@name='data_despacho']");
                    var tel_orgao_destino = d.DocumentNode.SelectSingleNode("//input[@name='tel_orgao_destino']");
                    var orgao_origem = d.DocumentNode.SelectSingleNode("//input[@name='orgao_origem']");
                    var orgao_destino = d.DocumentNode.SelectSingleNode("//input[@name='orgao_destino']");
                    var endereco = d.DocumentNode.SelectSingleNode("//input[@name='endereco']");

                    Console.Write($"Detran - Processo \r\n");
                    ConsoleUtil.CreateConsoleLine(Console.WindowWidth);
                    Console.Write($"\r\n");
                    Console.WriteLine($"Processo\t\t | {processo.Attributes["value"].Value}");
                    Console.WriteLine($"Requerente\t\t | {requerente.Attributes["value"].Value}");
                    Console.WriteLine($"Assunto\t\t\t | {assunto.Attributes["value"].Value}");
                    Console.WriteLine($"Info. Compl.\t\t | {inform_compl.Attributes["value"].Value}");
                    Console.WriteLine($"Multa\t\t\t | {multa.Attributes["value"].Value}");
                    Console.WriteLine($"Data Despacho\t\t | {data_despacho.Attributes["value"].Value}");
                    Console.WriteLine($"Tel. Orgão Destino\t | {tel_orgao_destino.Attributes["value"].Value}");
                    Console.WriteLine($"Orgão Origem\t\t | {orgao_origem.Attributes["value"].Value}");
                    Console.WriteLine($"Orgão Destino\t\t | {orgao_destino.Attributes["value"].Value}");
                    Console.WriteLine($"Endereço\t\t | {endereco.Attributes["value"].Value}");
                    Console.WriteLine($"Despacho\t\t | {despacho.Attributes["value"].Value}");
                }
            }

            return await Task.FromResult(true);
        }
    }
}
