using Whisper.Aplicacao.Dto;

namespace Whisper.Api.Configuracao
{
    public static class AppSettingsConfiguracao
    {
        public static void Carregar(this IServiceCollection services, IConfigurationRoot configuration)
        {
            //classe que receber AppSettingsDto via injeção de dependência terá exatamente esse objeto, sem suporte a reload on change.
            AppSettingsDto appSettingsDto = configuration.Get<AppSettingsDto>() ?? new AppSettingsDto();
            services.AddSingleton(appSettingsDto);

            //é atualizado automaticamente se o arquivo appsettings.json mudar em tempo de execução.
            services.Configure<AppSettingsDto>(configuration);

            services.RegistrarRateLimit(appSettingsDto);
            services.AddSingleton(appSettingsDto);

        }

      
    }
}
