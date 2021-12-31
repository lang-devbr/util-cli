using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;
using UtilCli.App.Commands.Weather.Contracts;
using UtilCli.App.Shared;

namespace UtilCli.App.Commands.Weather
{
    public class WeatherProcess
    {
        private readonly IConfigurationRoot? _configuration;

        public WeatherProcess(IConfigurationRoot? configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> Execute(string[] args)
        {
            string city = string.Empty;

            if (args.Length > 1 && !string.IsNullOrEmpty(args[1]))
            {
                for (int i = 1; i < args.Length; i++)
                {
                    city += $" {args[i]}";
                }
            }

            city = city.Trim();

            if (string.IsNullOrEmpty(city) && _configuration != null)
                city = _configuration.GetSection("weather-city").Value;

            if (string.IsNullOrEmpty(city))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Argument not found. For help use -h.");
                return false;
            }

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://weather.com");

                client.DefaultRequestHeaders.Add("Referer", "https://weather.com/");
                client.DefaultRequestHeaders.Add("Origin", "https://weather.com");
                client.DefaultRequestHeaders.Add("Accept", "*/*");

                var request = new List<WeatherRequest>();
                request.Add(new WeatherRequest()
                {
                    name = "getSunV3LocationSearchUrlConfig",
                    @params = new Params()
                    {
                        language = "pt-BR",
                        locationType = "locale",
                        query = city
                    }
                });

                var response = await client.PostAsync("/api/v1/p/redux-dal", new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));

                if (!response.IsSuccessStatusCode)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Error to get weather informations. For help use -h.");
                    return false;
                }

                var stringResponse = await response.Content.ReadAsStringAsync();
                
                var weatherResponse = JsonConvert.DeserializeObject<WeatherResponse>(stringResponse.Replace($"language:pt-BR;locationType:locale;query:{city}", "queryUtilCli"));

                var id = weatherResponse.dal.getSunV3LocationSearchUrlConfig.queryUtilCli.data.location.placeId.First();
                var cityResponse = weatherResponse.dal.getSunV3LocationSearchUrlConfig.queryUtilCli.data.location.address.First();

                var pageResponse = await client.GetAsync($"/pt-BR/clima/hoje/l/{id}");

                HtmlDocument d = new HtmlDocument();
                d.Load(GenerateStreamFromString(pageResponse.Content.ReadAsStringAsync().Result));

                var tempMaxMin = d.DocumentNode.QuerySelector("[data-testid='WeatherDetailsListItem']").InnerText.Replace("TemperatureMax. / Mín.", string.Empty);
                var wind = d.DocumentNode.QuerySelector("[data-testid='WeatherDetailsListItem']").NextSibling.InnerText.Replace("WindVentoWind Direction", string.Empty);
                var humidity = d.DocumentNode.QuerySelector("[data-testid='WeatherDetailsListItem']").NextSibling.NextSibling.InnerText.Replace("HumidityUmidade", string.Empty);
                var pressure = d.DocumentNode.QuerySelector("[data-testid='WeatherDetailsListItem']").NextSibling.NextSibling.NextSibling.NextSibling.InnerText.Replace("PressurePressãoArrow Up", string.Empty).Replace("PressurePressãoArrow Down", string.Empty);
                var uv = d.DocumentNode.QuerySelector("[data-testid='WeatherDetailsListItem']").NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.InnerText.Replace("UV LevelÍndice UV", string.Empty);
                var vis = d.DocumentNode.QuerySelector("[data-testid='WeatherDetailsListItem']").NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.InnerText.Replace("VisibilityVisibilidade", string.Empty);
                var moon = d.DocumentNode.QuerySelector("[data-testid='WeatherDetailsListItem']").NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.NextSibling.InnerText.Replace("Moon PhaseFase da lua", string.Empty);
                var sens = d.DocumentNode.QuerySelector("div[data-testid='FeelsLikeSection']").FirstChild.InnerText;
                var temp = d.DocumentNode.SelectNodes("/html/body/div[1]/main/div[2]/main/div[1]/div/section/div/div[2]/div[1]/span")?.First().InnerText;
                var condition = d.DocumentNode.SelectNodes("/html/body/div[1]/main/div[2]/main/div[1]/div/section/div/div[2]/div[1]/div")?.First().InnerText;
                var condition2 = d.DocumentNode.SelectNodes("/html/body/div[1]/main/div[2]/main/div[1]/div/section/div/div[3]/span")?.First().InnerText;

                var prob = d.DocumentNode.QuerySelector("[data-testid='DailyWeatherModule']")
                    .ChildNodes[1].ChildNodes[0].ChildNodes[0].QuerySelector("[data-testid='SegmentPrecipPercentage']").InnerText.Replace("Probabilidade de chuva", String.Empty).Replace("Rain", String.Empty);

                Console.ForegroundColor = ConsoleColor.Green;

                Console.Write($"\t Previsão do tempo \r\n");
                ConsoleUtil.CreateConsoleLine(Console.WindowWidth);
                Console.Write($"\r\n");
                Console.Write($"Cidade\t\t | {cityResponse} \r\n");
                Console.Write($"----------------------------------------\r\n");
                Console.Write($"Temperatura\t | {temp} \r\n");
                Console.Write($"Temp. máx/mín.\t | {tempMaxMin} \r\n");
                Console.Write($"Sen. térmica\t | {sens} \r\n");
                Console.Write($"Pressão atm.\t | {pressure} \r\n");
                Console.Write($"Umidade\t\t | {humidity} \r\n");
                Console.Write($"Visibilidade\t | {vis} \r\n");
                Console.Write($"Vel. vento\t | {wind} \r\n");
                Console.Write($"Índice UV\t | {uv} \r\n");
                Console.Write($"Fase da lua\t | {moon} \r\n");
                Console.Write($"Prob. chuva\t | {prob} \r\n");
                ConsoleUtil.CreateConsoleLine(Console.WindowWidth);
                Console.Write($"\r\n");
                Console.WriteLine("Condições do tempo: \r\n");
                Console.Write($" * {condition}\r\n");
                if(!string.IsNullOrEmpty(condition2))
                    Console.Write($" * {condition2}\r\n");
            }

            return await Task.FromResult(true);
        }

        private Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}