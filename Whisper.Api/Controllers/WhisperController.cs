using Microsoft.AspNetCore.Mvc;
using Whisper.Aplicacao.Rotas.WhisperRota;

namespace Whisper.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WhisperController : ControllerBase
    {
        private readonly TranscricaoHandler _handler;

        public WhisperController(TranscricaoHandler handler)
        {
            _handler = handler;
        }

        [HttpPost("TranscricaoAudio")]
        public async Task<IActionResult> Transcricao(IFormFile arquivo, CancellationToken cancellationToken)
        {
            var request = new TranscricaoRequest { Arquivo = arquivo };
            var resultado = await _handler.Executar(request, cancellationToken);
            return Ok(resultado);
        }

    }
}
