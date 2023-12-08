# RE4-RTP-TOOL
Extract and repack RE4 RTP files (RE4 ubisoft/2007/steam/uhd/Ps2)


Translate from Portuguese Brazil

Programas destinados a extrair e reempacotar arquivo .RTP do RE4 de PS2, 2007, UHD.
<br>Nota: esse é o arquivo que define quais rotas os inimigos vão seguir para chegar ate o player. Então tem o "problema do caixeiro viajante".

**Update B.1.1.0.0**

Atualizado o algoritmo de geração de caminho, agora as rotas geradas são boas o suficiente em comparação ao algoritmo original do jogo e em comparação com o algoritmo da tool do "Son of percia";
<br> Mudei para o ".Net Framework 4.8", para compatibilidade com o Windows 7;
<br> Os arquivos da versão anterior são compatíveis com a versão atual;

## RE4_RTP_EXTRACT.exe

Programa destinado a extrair o arquivo RTP, (escolha o .bat da mesma versão do seu jogo) sendo que ira gerar os arquivo:

* .OBJ, esse é o arquivo no qual vai ser editado, veja as informações abaixo.
* .IDXRTP, esse arquivo é necessário para o repack, mas seu conteúdo não importa.
* .TXT2, esse é um arquivo de debug, apenas informacional, por padrão não é gerado.


**OBJ FILE**

A escala do arquivo é 100 vezes menor que a do jogo, sendo Y a altura.

Veja o Exemplo:
<br>![exemplo](exemplo.png)

<br> O nome dos objetos tem que ser exatamente como é descrito abaixo:

* **_RTP#Node_000#** onde 000 é um numero decimal que pode ir de 0 a 254, esse são os pontos onde o inimigos vai passar, a localização é definida pelo ponto de menor altura do triangulo.

* **Connection#000:True#001:True#** são linhas, a localização da linha não é importante para o repack, é apenas para referencia, o importante é o nome do objeto, no qual faz a ligação entre dois pontos: seguindo a definição abaixo:

*_Connection#000:True#001:True#_* vai ter uma conexão entre o node 0 e o node 1, sendo que pode ir do node 0 para 1 , e do node 1 para o 0

*_Connection#001:False#002:True#_* cria uma conexão onde pode ir do node 2 para o 1, porem não pode fazer o caminho inverso.

*_Connection#003:True#004:False#_* cria uma conexão onde pode ir do node 3 para o 4, porem não pode fazer o caminho inverso.

## RE4_RTP_REPACK.exe

Programa destinado a reempacotar o arquivo RTP, necessita de um arquivo .idxrtp com o mesmo nome do arquivo .obj.
<br> Nota: escolha o .bat da mesma versão do seu jogo.

## Problema do caixeiro viajante

O arquivo .RTP é pode ser dividido em 3 blocos, e o terceiro bloco é o qual define os caminho entre dois nodes, na atualização "B.1.1.0.0", corrigi o problema dos caminhos, não é necessariamente o caminho mais curto, mas é bom o suficiente para não se notar diferença com o algoritmo que foi usado no jogo;


## Código de terceiro:

[ObjLoader by chrisjansson](https://github.com/chrisjansson/ObjLoader):
Encontra-se no RE4_PMD_Repack, código modificado, as modificações podem ser vistas aqui: [link](https://github.com/JADERLINK/ObjLoader).

**At.te: JADERLINK**
<br>2023-12-08
