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
        public async Task<IActionResult> GetCompanyData(string sigla, string interval = "5min")
        {
            _logger.LogInformation($"Iniciando solicitação para a sigla: {sigla}, intervalo: {interval}");
            try
            {
                var data = await _service.GetCompanyDataAsync(sigla, interval);

                if (data == null || data.Count == 0)
                {
                    _logger.LogWarning($"Nenhum dado encontrado para a sigla: {sigla}");
                    return NotFound("Dados não encontrados.");
                }
                return Ok(data);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar a solicitação");
                return StatusCode(500, "Erro interno no servidor.");
            }
        }
    }
}