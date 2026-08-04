using Microsoft.AspNetCore.Http;
using Whisper.Aplicacao.Util;

namespace Whisper.Aplicacao.Rotas.WhisperRota
{
    public class TranscricaoRequest : IRequest<ResultadoOperacao>
    {

        public IFormFile Arquivo { get; set; } = default!;
    }
}
