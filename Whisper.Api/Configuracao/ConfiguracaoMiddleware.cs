using Whisper.Api.Configuracao.Middleware;

namespace Whisper.Api.Configuracao
{
    public static class ConfiguracaoMiddleware
    {
        public static IApplicationBuilder UseApiKeyMiddleware(this IApplicationBuilder builder)
        {
            //Faz a validação da chave de API via X-Api-Key no header
            //Retorna 401 Unauthorized com mensagem em JSON se a chave estiver ausente ou inválida.
            return builder.UseMiddleware<ApiKeyMiddleware>();
        }

     


        public static IApplicationBuilder ConfigurarMiddlewaresApi(this IApplicationBuilder app)
        {
 
            app.UseApiKeyMiddleware();
            return app;
        }
    }
}
