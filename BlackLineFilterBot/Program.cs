using Newtonsoft.Json;
using System;
using System.IO;

namespace BlackLineFilterBot
{
    internal static class Program
    {
        private const string ConfigFileName = "config.json";

        private static Bot _botClient;

        private static void Main()
        {
            try
            {
                var config = ReadConfigFromJsonFile();
                _botClient = new Bot(config.Token);
                _botClient.StartPolling();
                Console.WriteLine("Bot started");
                while (true)
                {
                    if (Console.ReadLine() == "!")
                    {
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (_botClient != null)
                    _botClient.StopPolling();
            }
        }

        private static Config ReadConfigFromJsonFile()
        {
            string configPath = Path.Combine(Directory.GetCurrentDirectory(), ConfigFileName);
            string configJsonContent = File.ReadAllText(configPath);
            return JsonConvert.DeserializeObject<Config>(configJsonContent);
        }
    }
}