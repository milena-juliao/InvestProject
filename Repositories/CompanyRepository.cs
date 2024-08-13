using System;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Runtime.InteropServices.JavaScript;

namespace InvestProject.Repositories
{
    public class CompanyRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        //Recebe HttpClient e a ApiKey
        public CompanyRepository(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        }

        public async Task<Dictionary<string, decimal>> GetCompanyDataAsync(string sigla, string interval)
        {
            var url = $"https://www.alphavantage.co/query?function=TIME_SERIES_INTRADAY&symbol={sigla}&interval={interval}&apikey={_apiKey}";
            var response = await _httpClient.GetStringAsync(url);

            using (JsonDocument doc = JsonDocument.Parse(response))
            {
                var root = doc.RootElement;

                // Verifique o nome da propriedade com base na resposta da API
                if (root.TryGetProperty($"Time Series ({interval})", out JsonElement timeSeries))
                {
                    var result = new Dictionary<string, decimal>();

                    foreach (JsonProperty entry in timeSeries.EnumerateObject())
                    {
                        var dateTime = entry.Name;

                        // Verifique se a propriedade "4. close" existe e se é do tipo número
                        if (entry.Value.TryGetProperty("4. close", out JsonElement closeElement) && closeElement.ValueKind == JsonValueKind.String)
                        {
                            // Tente converter o valor para decimal
                            if (Decimal.TryParse(closeElement.GetString(), out var closeValue))
                            {
                                result[dateTime] = closeValue;
                            }
                            else
                            {
                                Console.WriteLine($"Erro ao converter o valor para decimal: {closeElement.GetString()}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Propriedade '4. close' não encontrada ou não é uma string.");
                        }
                    }

                    return result;
                }
                else
                {
                    Console.WriteLine($"Propriedade 'Time Series ({interval})' não encontrada.");
                    return new Dictionary<string, decimal>();
                }
            }
        }

    }
}