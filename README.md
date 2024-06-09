# RE4-2007-GCA-TOOL

Extract and repack RE4 2007 GCA files

**Translate from Portuguese Brazil**

Programa destinado a extrair e reempacotar arquivos .DAT, do formato GCA do re4 2007.
<br>Ao extrair será gerado um arquivo de extenção .idxgca, ele será usado para o repack.
<br>Nota: O repack não é obrigatório, pois o jogo lê os arquivos primeiro das pastas extraídas.
<br>Nota2: O programa não suporta arquivos comprimidos, e os arquivos originais do jogo também não têm compressão.

## Extract
Exemplo:
<br>RE4_2007_GCA_TOOL.exe "xfile.dat"

! Irá gerar um arquivo de nome "xfile.idxgca";
<br>! A estrutura de arquivos e pasta vai ser conforme estava dentro do arquivo dat;

## Repack
Exemplo:
<br>RE4_2007_GCA_TOOL.exe "xfile.idxgca"

! No arquivo .idxgca vão conter os nomes dos arquivos/diretórios que vão ser colocados no DAT;
<br>! Os arquivos têm que seguir o mesmo diretório/nome que está no idxgca;
<br>! No arquivo .idxgca as linhas iniciadas com o carácter # são consideradas comentários e não arquivos;
<br>! O nome do arquivo gerado é o mesmo nome do idxgca, mas com a extensão .dat;

## Agradecimentos:

Agradecimento ao "zatarita", por sua documentação do formato GCA e por sua ajuda;
<br>[link da documentação.](https://wiki.zatarita.io/definitions/gca/)

## Código de terceiro:

O arquivo "Crc32.cs" contém código de terceiros:
<br>[Topico](https://damieng.com/blog/2006/08/08/calculating_crc32_in_c_and_net/) [Codigo](https://github.com/damieng/DamienGKit/blob/master/CSharp/DamienG.Library/Security/Cryptography/Crc32.cs)
 Licenças: [link](https://github.com/damieng/DamienGKit/blob/master/LICENSE.txt) [link](https://github.com/damieng/DamienGKit/blob/master/LICENSE-MIT.txt) [link](https://github.com/damieng/DamienGKit/blob/master/LICENSE-APACHE.txt)

**At.te: JADERLINK**
<br>2024-06-09