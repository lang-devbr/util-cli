// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.PlatformAbstractions;
using System.Diagnostics;
using UtilCli.App.Commands;

bool control = true;

string configPath = "C:/UtilCli/config/settings.json";
IConfigurationRoot? _configuration = null;

if (File.Exists(configPath))
{
    _configuration = new ConfigurationBuilder()
               .AddJsonFile(configPath)
               .AddEnvironmentVariables()
               .Build();
}

Console.ForegroundColor = ConsoleColor.Cyan;
CreateConsoleLine(Console.WindowWidth);
Console.WriteLine($"Util Cli (v.{PlatformServices.Default.Application.ApplicationVersion})");
Console.WriteLine($"Arguments: {string.Join(", ", args)}");
Console.ForegroundColor = ConsoleColor.White;
CreateConsoleLine(Console.WindowWidth);

if (args.Length <= 0)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Argument not found. For help use -h.");
    return;
}

if(args[0].Equals("-dp"))
{
    control = false;
    DetranProcess dp = new DetranProcess(_configuration);
    await dp.Execute(args);
}

if (args[0].Equals("-w"))
{
    control = false;
    WeatherProcess dp = new WeatherProcess(_configuration);
    await dp.Execute(args);
}

if (args[0].Equals("-h"))
{
    control = false;
    HelpProcess h = new HelpProcess(_configuration);
    await h.Execute(args);
}

if (args[0].Equals("-s"))
{
    Process.Start("shutdown", "/s /t 0");
}

if (args[0].Equals("-r"))
{
    Process.Start("shutdown", "/r /t 0");
}

Console.ForegroundColor = ConsoleColor.Cyan;
CreateConsoleLine(Console.WindowWidth);

if (control)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("No command for this argument. For help use -h.");
}

static void CreateConsoleLine(int width)
{
    string line = string.Empty;

    for (int i = 0; i < width-2; i++)
    {
        line += "-";
    }

    Console.WriteLine(line);
}