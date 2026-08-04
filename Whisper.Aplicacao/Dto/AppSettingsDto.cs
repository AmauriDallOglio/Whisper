namespace Whisper.Aplicacao.Dto
{
    public class AppSettingsDto
    {


        public RateLimitDto RateLimit { get; set; } = new RateLimitDto();

        public SegurancaDto Seguranca { get; set; } = new SegurancaDto();
    }

    public class RateLimitDto
    {
        public bool Habilitado { get; set; } = true;
        public int RequisicoesPermitidas { get; set; } = 100;
        public int JanelaEmSegundos { get; set; } = 60;
        public int TamanhoFila { get; set; } = 0;
    }







    public class SegurancaDto
    {
        public string ApiKey { get; set; } = string.Empty;
    }
}
