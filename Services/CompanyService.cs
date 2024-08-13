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

        public async Task<Dictionary<string, decimal>> GetCompanyDataAsync(string sigla, string interval)
        {
            _logger.LogInformation($"Buscando dados para a sigla: {sigla}, intervalo: {interval}");

            try
            {
                var data = await _repository.GetCompanyDataAsync(sigla, interval);

                if (data == null || data.Count == 0)
                {
                    _logger.LogWarning($"Nenhum dado encontrado para a sigla: {sigla} no intervalo: {interval}");
                    return new Dictionary<string, decimal>();
                }

                _logger.LogInformation($"Dados encontrados: {data.Count} entradas para a sigla: {sigla}");
                return data;
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