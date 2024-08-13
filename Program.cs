using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using InvestProject.Repositories;
using InvestProject.Services;

var builder = WebApplication.CreateBuilder(args);

// Adiciona a configuração do appsettings.json
var configuration = builder.Configuration;

// Configura serviços
builder.Services.AddControllers();

// Registro do CompanyRepository, utilizando a configuração
builder.Services.AddHttpClient<CompanyRepository>()
    .ConfigureHttpClient((provider, httpClient) =>
    {
        var apiKey = configuration["AlphaVantage:ApiKey"];
        if(string.IsNullOrEmpty(apiKey))
        {
            throw new ArgumentException(nameof(apiKey), "Api key is missing in conguration.");
        }
    });

builder.Services.AddScoped<CompanyService>();

builder.Services.AddScoped(provider =>
{
    var httpClient = provider.GetRequiredService<HttpClient>();
    var apiKey = configuration["AlphaVantage:ApiKey"] ?? throw new ArgumentNullException("Api Key não encontrada no appsetting.json");
    return new CompanyRepository(httpClient, apiKey);
});

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
