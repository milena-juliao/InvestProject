//// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");
using System;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Runtime.InteropServices.JavaScript;

namespace InvestProject
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            string? apiKey = config["ApiKey"];
            string symbol = "MSFT";
            string interval = "5min";

            string url = $"https://www.alphavantage.co/query?function=TIME_SERIES_INTRADAY&symbol={symbol}&interval={interval}&apikey={apiKey}";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                using (JsonDocument doc = JsonDocument.Parse(responseBody))
                {
                    JsonElement root = doc.RootElement;

                    if (root.TryGetProperty("Time Series (5min)", out JsonElement timeSeries))
                    {
                        foreach (JsonProperty entry in timeSeries.EnumerateObject())
                        {
                            string time = entry.Name;
                            JsonElement value = entry.Value;
                            if (value.TryGetProperty("4. close", out JsonElement closeElement))
                            {
                                string? closingPrice = closeElement.GetString();
                                Console.WriteLine($"{time}: {closingPrice}");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Propriedade 'Time Series (5 min)' não encontrada.");
                    }
                }
            }

            Console.WriteLine("Dados recuperados com sucesso!");
        }
    }
}