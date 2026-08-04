import os
import sys
import shutil

# --- Configuração do ffmpeg ---
FFMPEG_DIR = r"C:\Ffmpeg\ffmpeg-8.1.2-essentials_build\bin"

if os.path.isdir(FFMPEG_DIR):
    os.environ["PATH"] = FFMPEG_DIR + os.pathsep + os.environ.get("PATH", "")

# Verifica se o ffmpeg está realmente acessível antes de prosseguir
ffmpeg_encontrado = shutil.which("ffmpeg")
if not ffmpeg_encontrado:
    print(f"ERRO: ffmpeg não encontrado no PATH. FFMPEG_DIR configurado: {FFMPEG_DIR}", file=sys.stderr)
    sys.exit(1)

import whisper

audio_path = sys.argv[1]
model_name = sys.argv[2] if len(sys.argv) > 2 else "base"
language = sys.argv[3] if len(sys.argv) > 3 else "pt"

model = whisper.load_model(model_name)
resultado = model.transcribe(audio_path, language=language, fp16=False)

# imprime apenas o texto completo
print(resultado.get("text", "").strip())