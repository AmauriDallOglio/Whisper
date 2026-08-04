using Microsoft.AspNetCore.Mvc;
using Whisper.Aplicacao.Rotas.WhisperRota;

namespace Whisper.Api.Configuracao
{
    public class InjecaoDependenciaConfiguracao
    {
        public static void RegistrarServicos(WebApplicationBuilder builder)
        {


            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });


            builder.Services.AddScoped<TranscricaoHandler>();

        }

    
    }
}
