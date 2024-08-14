using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;
using InvestProject.Repositories;
using Microsoft.Extensions.Logging;

namespace InvestProject.Services
{
    public class CompanyService
    {
        private readonly CompanyRepository _repository;
        private readonly ILogger<CompanyService> _logger;

        public CompanyService(CompanyRepository repository, ILogger<CompanyService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<(Dictionary<string, decimal> Data, double StandardDeviation)> GetCompanyDataAsync(string sigla)
        {
            _logger.LogInformation($"Buscando dados para a sigla: {sigla}");

            try
            {
                var data = await _repository.GetCompanyDataAsync(sigla);

                if (data == null || data.Count == 0)
                {
                    _logger.LogWarning($"Nenhum dado encontrado para a sigla: {sigla}");
                    return (new Dictionary<string, decimal>(), 0.0);
                }

                var prices = data.Values.Select(p => (double)p).ToList();
                if (prices.Count == 0)
                {
                    _logger.LogWarning("Sem preços disponíveis para calcular o desvio padrão.");
                    return (data, 0.0);
                }

                var mean = prices.Average();
                var variance = prices.Average(v => Math.Pow(v - mean, 2));
                var standardDeviation = Math.Sqrt(variance);

                _logger.LogInformation($"Dados encontrados: {data.Count} entradas para a sigla: {sigla}");
                return (data, standardDeviation);
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "Erro ao processar os dados recebidos no repositório");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao buscar dados no repositório");
                throw;
            }
        }
    }
}