using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace UtilCli.App.Commands
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

            if (string.IsNullOrEmpty(city) && _configuration != null)
                city = _configuration.GetSection("weather:city").Value;

            if (string.IsNullOrEmpty(city))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Argument not found.");
                return false;
            }

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.openweathermap.org");
                var response = await client.GetAsync($"/data/2.5/weather?q={city}&appid={_configuration.GetSection("weather:appkey").Value}&lang={_configuration.GetSection("weather:lang").Value}&units={_configuration.GetSection("weather:units").Value}");

                if(!response.IsSuccessStatusCode)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Error to get weather informations.");
                    return false;

                }

                var w = JsonConvert.DeserializeObject<Root>(await response.Content.ReadAsStringAsync());

                Console.ForegroundColor = ConsoleColor.Green;

                Console.Write($"\t Previsão do tempo \r\n");
                Console.Write($"----------------------------------------\r\n");
                Console.Write($"Cidade\t\t | {w.name} \r\n");
                Console.Write($"----------------------------------------\r\n");
                Console.Write($"Temperatura\t | {w.main?.temp} °C\r\n");
                Console.Write($"Temp. mín.\t | {w.main?.temp_min} °C\r\n");
                Console.Write($"Temp. máx.\t | {w.main?.temp_max} °C\r\n");
                Console.Write($"Sen. térmica\t | {w.main?.feels_like} °C\r\n");
                Console.Write($"Pressão atm.\t | {w.main?.pressure} hPa\r\n");
                Console.Write($"Umidade\t\t | {w.main?.humidity} %\r\n");
                Console.Write($"Vol. chuva -1h\t | {w.rain?._1h} mm\r\n");
                Console.Write($"Nebulosidade\t | {w.clouds?.all} %\r\n");
                Console.Write($"Vel. vento\t | {w.wind?.speed} metros/seg\r\n");
                Console.Write($"----------------------------------------\r\n");
                Console.WriteLine("Condições do tempo: \r\n");
                foreach (var item in w.weather)
                    Console.Write($" * {item.description}\r\n");
            }

            return await Task.FromResult(true);
        }
    }
}
