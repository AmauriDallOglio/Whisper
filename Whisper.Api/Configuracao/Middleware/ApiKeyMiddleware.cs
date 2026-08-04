using Microsoft.Extensions.Options;
using Whisper.Aplicacao.Dto;

namespace Whisper.Api.Configuracao.Middleware
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppSettingsDto _AppSettingsDtoOptions;
        private readonly AppSettingsDto _AppSettingsDto;

        public ApiKeyMiddleware(RequestDelegate next, IOptions<AppSettingsDto> appSettingsDtoOptions, AppSettingsDto aapSettingsDto)
        {
            _next = next;
            _AppSettingsDtoOptions = appSettingsDtoOptions.Value;
            _AppSettingsDto = aapSettingsDto;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string apiKeyConfigurada = _AppSettingsDto.Seguranca.ApiKey;
            var apiKeyInformada = context.Request.Headers["X-Api-Key"].ToString();

            if (string.IsNullOrWhiteSpace(apiKeyConfigurada) || string.IsNullOrWhiteSpace(apiKeyInformada))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { sucesso = false, mensagem = "API Key ausente." });
                return;
            }

            if (!string.Equals(apiKeyInformada, apiKeyConfigurada, StringComparison.Ordinal))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { sucesso = false, mensagem = "API Key inválida." });
                return;
            }

            await _next(context);
        }
    }
}
