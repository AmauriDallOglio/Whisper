using System.Diagnostics;
using Whisper.Aplicacao.Util;

namespace Whisper.Aplicacao.Rotas.WhisperRota
{
    public class TranscricaoHandler : IContratoBaseHandler<TranscricaoRequest, ResultadoOperacao>
    {

        private static readonly string[] ExtensoesPermitidas = { ".mp3", ".wav", ".m4a", ".mp4", ".ogg", ".flac" };
        public TranscricaoHandler()
        {


        }

        public async Task<ResultadoOperacao> Executar(TranscricaoRequest request, CancellationToken cancellationToken = default)
        {
            if (request.Arquivo == null || request.Arquivo.Length == 0)
                return ResultadoOperacao.GerarErro("Nenhum arquivo foi enviado.", 400);

            var extensao = Path.GetExtension(request.Arquivo.FileName).ToLowerInvariant();
            if (!ExtensoesPermitidas.Contains(extensao))
            {
                string mensagemErro = $"Formato de áudio não suportado. Permitidos: {string.Join(", ", ExtensoesPermitidas)}";
          
                return ResultadoOperacao.GerarErro(mensagemErro, 400);
            }

            var caminhoTemp = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}{extensao}");

            await using (var stream = new FileStream(caminhoTemp, FileMode.Create))
            {
                await request.Arquivo.CopyToAsync(stream, cancellationToken);
            }

            try
            {
                var pythonPath = Environment.GetEnvironmentVariable("PYTHON_PATH") ?? "python";
                var scriptPath = Path.Combine(AppContext.BaseDirectory, "Scripts", "transcrever_whisper.py");

                if (!File.Exists(scriptPath))
                    return ResultadoOperacao.GerarErro("Script de transcrição não encontrado.", 500);

                var processo = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = pythonPath,
                        Arguments = $"-u \"{scriptPath}\" \"{caminhoTemp}\" base pt",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                processo.Start();
                var stdoutTask = processo.StandardOutput.ReadToEndAsync(cancellationToken);
                var stderrTask = processo.StandardError.ReadToEndAsync(cancellationToken);
                await processo.WaitForExitAsync(cancellationToken);
                var stdout = await stdoutTask;
                var stderr = await stderrTask;

                if (processo.ExitCode != 0)
                    return ResultadoOperacao.GerarErro($"Erro ao transcrever o áudio: {stderr.Trim()}", 500);

                var response = new TranscricaoResponse { Texto = stdout.Trim() };
                return ResultadoOperacao.GerarSucesso(response, "Áudio transcrito com sucesso.");
            }
            finally
            {
                if (File.Exists(caminhoTemp))
                    File.Delete(caminhoTemp);
            }
        }
    }
}
