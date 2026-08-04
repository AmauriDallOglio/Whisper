# Whisper Transcrição de Áudio para Texto

Atualização do readme: https://github.com/AmauriDallOglio/Wiki/blob/main/Whisper%20no%20Windows%20(para%20fazer%20transcri%C3%A7%C3%A3o%20de%20%C3%A1udio).md


Desenvolvendo uma api para fornecer um serviço web (endpoint HTTP) que recebe um arquivo de áudio e devolve o texto transcrito daquele áudio, em português.

Este serviço é útil para: transcrever gravações de reuniões, notas de voz, entrevistas, aulas gravadas, ou qualquer necessidade de converter áudio em texto de forma automatizada expondo isso como uma API que outros sistemas podem chamar.

<img width="1432" height="786" alt="image" src="https://github.com/user-attachments/assets/45ecb123-bf99-433d-a339-ec5cd6751af3" />

Pré-requisitos de ambiente

A configuração do ambiente envolve três componentes principais: Python, FFmpeg e a biblioteca Whisper. As etapas a seguir descrevem o processo de instalação de cada um.


# Instalar Python

- Baixe o Python em https://www.python.org/downloads/

<img width="1283" height="313" alt="image" src="https://github.com/user-attachments/assets/4594d6e6-983b-4226-b6f8-29e9a0a1ef38" />

- Baixe a versão: https://www.python.org/downloads/release/python-3140/ 

Neste projeto foi utilizada a versão 3.14.0.

<img width="1426" height="487" alt="image" src="https://github.com/user-attachments/assets/4004379f-c3db-47a8-b1ac-236f273efc1c" />

Durante a instalação, é fundamental marcar a opção "Add Python to PATH", garantindo que o interpretador Python fique acessível a partir de qualquer terminal do sistema.

<img width="1438" height="533" alt="image" src="https://github.com/user-attachments/assets/6841a007-2874-4a9e-93f4-86f736ee2e78" />


# Instalar o FFmpeg

O Whisper depende internamente do FFmpeg para decodificar os arquivos de áudio antes de processá-los. No PowerShell, rodar o comando: winget install Gyan.FFmpeg

<img width="1898" height="625" alt="image" src="https://github.com/user-attachments/assets/ec1ba8e9-0b3c-4510-af7e-e810e5813011" />

O teste a seguir confirma que o FFmpeg foi instalado e está funcionando corretamente no Windows PowerShell:

<img width="1883" height="632" alt="image" src="https://github.com/user-attachments/assets/f4c03fd3-5713-4aa1-ae7d-462fc7f0bdd8" />

Para confirmar que o executável do FFmpeg está de fato presente no sistema, é possível realizar uma busca recursiva no disco:

Validar se foi instalado com o comando: PS C:\amauri> Get-ChildItem -Path C:\ -Filter ffmpeg.exe -Recurse -ErrorAction SilentlyContinue | Select-Object FullName
PS C:\amauri>

Se o resultado vier vazio, significa que o FFmpeg não está instalado como um executável autônomo no sistema, sendo necessário realizar a instalação manual descrita a seguir.

## Baixar o ffmpeg

Acessar: https://www.gyan.dev/ffmpeg/builds/

<img width="1798" height="503" alt="image" src="https://github.com/user-attachments/assets/dfc2df45-8f7c-4fe3-9fae-e02e1fcbc470" />


- No Windows PowerShell executar: 
  - PS C:\WINDOWS\system32> Test-Path "C:\Ffmpeg\ffmpeg-8.1.2-essentials_build\bin\ffmpeg.exe"
  - PS C:\WINDOWS\system32> C:\Ffmpeg\ffmpeg-8.1.2-essentials_build\bin\ffmpeg.exe

<img width="1385" height="428" alt="image" src="https://github.com/user-attachments/assets/1d18d59b-1a7b-4743-a994-4558482e79e3" />

Antes de testar a transcrição por meio da API, é recomendável validar o funcionamento do script Python de forma isolada, executando-o diretamente pela linha de comando:

python C:\Amauri\GitHub\Whisper\Whisper.Api\bin\Debug\net8.0\Scripts\transcrever_whisper.py "C:\Amauri\Gravando.mp3" base pt

<img width="1544" height="476" alt="image" src="https://github.com/user-attachments/assets/628765a4-c75c-456a-bbae-ae2dae326380" />



# Instalar o Whisper via pip

O Whisper é uma biblioteca Python (não existe uma versão nativa robusta em C#/.NET com a mesma qualidade de modelo), então a solução foi criar uma ponte: a API .NET recebe as requisições HTTP normalmente (o que é natural em .NET), mas delega o trabalho pesado de IA para um subprocesso Python, que é onde o modelo de fala-para-texto realmente roda.

Com o Python e o FFmpeg devidamente configurados, o próximo passo é instalar a biblioteca Whisper: No PowerShell, rode: pip install -U openai-whisper

O pip baixa e instala automaticamente o Whisper e todas as dependências necessárias, incluindo o PyTorch.

## Atualização

Para manter o pip e o Whisper atualizados em suas versões mais recentes:
- python -m pip install --upgrade pip
- pip install -U openai-whisper

<img width="1673" height="332" alt="image" src="https://github.com/user-attachments/assets/ab1680a5-f4b0-4b8c-885f-1ab2a7ce9d5a" />


# Atualização

python -m pip install --upgrade pip
pip install -U openai-whisper

<img width="1819" height="799" alt="image" src="https://github.com/user-attachments/assets/8ac916fc-b0ef-4b5d-b06e-f3df1802f30a" />


# Testando a instalação

Com todos os componentes instalados, é possível validar o funcionamento completo do Whisper por meio da linha de comando, transcrevendo um arquivo de áudio diretamente:

Usar o comando: whisper C:\Amauri\Gravando.mp3 --model small --language pt

<img width="1337" height="115" alt="image" src="https://github.com/user-attachments/assets/f098060c-3a51-4fe8-a1a7-3169f44f6412" />

# Componentes do projeto

Camada API (.NET)

- O WhisperController expõe o endpoint POST /api/whisper/enviar, responsável por:
  - Receber o arquivo de áudio via upload (IFormFile), aceitando os formatos .mp3, .wav, .m4a, .mp4, .ogg e .flac;
  - Validar a extensão do arquivo enviado;
  - Salvar o áudio temporariamente em disco;
  - Acionar o processo Python externo, via Process.Start, para realizar a transcrição;
  - Aguardar o resultado, capturar o texto retornado pelo processo e devolvê-lo como JSON ({ "texto": "..." });
  - Remover o arquivo temporário ao final da execução, independentemente de sucesso ou falha.
  - Camada de processamento (Python)

- O script transcrever_whisper.py é responsável por:
  - Receber como argumentos de linha de comando o caminho do áudio, o nome do modelo (base, small, entre outros) e o idioma (pt);
  - Garantir que o FFmpeg esteja acessível no PATH do processo, configurando o diretório do executável antes de importar a biblioteca Whisper;
  - Carregar o modelo Whisper e executar a transcrição do áudio;
  - Imprimir o texto transcrito na saída padrão (stdout), que é então capturado e repassado pela API .NET ao cliente que fez a requisição.
