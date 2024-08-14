using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using InvestProject.Services;
using Microsoft.Extensions.Logging;

namespace InvestProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController : ControllerBase
    {
        private readonly CompanyService _service;
        private readonly ILogger<CompanyController> _logger;

        public CompanyController(CompanyService service, ILogger<CompanyController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("{sigla}")]
        public async Task<IActionResult> GetCompanyData(string sigla)
        {
            _logger.LogInformation($"Iniciando solicita��o para a sigla: {sigla}");
            try
            {
                var (data, standardDeviation) = await _service.GetCompanyDataAsync(sigla);

                if (data == null || data.Count == 0)
                {
                    _logger.LogWarning($"Nenhum dado encontrado para a sigla: {sigla}");
                    return NotFound("Dados n�o encontrados.");
                }
                var result = new
                {
                    Data = data,
                    StandardDeviation = standardDeviation
                };

                return Ok(result);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar a solicita��o");
                return StatusCode(500, "Erro interno no servidor.");
            }

            
        }
    }
}