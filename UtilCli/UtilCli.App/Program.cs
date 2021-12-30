// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.PlatformAbstractions;
using System.Diagnostics;
using UtilCli.App.Commands;
using UtilCli.App.Shared;

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
ConsoleUtil.CreateConsoleLine(Console.WindowWidth);
string title = $"util-cli";
Console.SetCursorPosition((Console.WindowWidth - title.Length) / 2, Console.CursorTop);
Console.WriteLine(title);
string subTitle = $"Arguments: {string.Join(", ", args)}";
Console.SetCursorPosition((Console.WindowWidth - subTitle.Length) / 2, Console.CursorTop);
Console.WriteLine(subTitle);
string version = $"(version: { PlatformServices.Default.Application.ApplicationVersion})";
Console.SetCursorPosition((Console.WindowWidth - version.Length) / 2, Console.CursorTop);
Console.WriteLine(version);

Console.ForegroundColor = ConsoleColor.White;
ConsoleUtil.CreateConsoleLine(Console.WindowWidth);

if (args.Length <= 0)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Argument not found. For help use -h.");
    return;
}

if (args[0].Equals("-c"))
{
    string strCmdText = "/C clear";
    System.Diagnostics.Process.Start("CMD.exe", strCmdText);
}

if (args[0].Equals("-m"))
{
    //For this command this repo is necessary https://github.com/gsass1/NTop
    string strCmdText = "/C ntop";
    System.Diagnostics.Process.Start("CMD.exe", strCmdText);
}

if (args[0].Equals("-b"))
{
    control = false;
    BlogProcess b = new BlogProcess(_configuration);
    await b.Execute(args);
}

if (args[0].Equals("-dp"))
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
ConsoleUtil.CreateConsoleLine(Console.WindowWidth);

if (control)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("No command for this argument. For help use -h.");
}