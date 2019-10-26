using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace trabalho_maravilhoso
{
    //coisas para fazer:
    //  fazer montagem do arquivo tweets.data, lendo todos arquivos na pasta "raw" e montando um único arquivo
    //  pesquisa de hashtag  
    //  refatorar (modularizar código) (menos importante)

    class Arquivo
    {
        void AddHashtagIndex(Dictionary<string, List<uint>> dict, string index, uint offset)
        {
            if (dict.TryGetValue(index, out var offsetList))
            {
                offsetList.Add(offset);
            }
            else
            {
                var newOffsetList = new List<uint>
                {
                    offset
                };

                dict.Add(index, newOffsetList);
            }
        }

        bool IndexAlreadyExists(Dictionary<string, uint> dict, string index)
        {
            return dict.TryGetValue(index, out var _);
        }

        System.IO.FileStream arquivoSaida { get; set; }
        System.IO.FileStream arquivoEntrada { get; set; }
        StreamReader leitor;
        StreamWriter escritor;

        BinaryReader reader;      //oficial

        string[] arquivos = Directory.GetFiles("../../../raw/", "*.raw", SearchOption.AllDirectories);
        List<string> arquivosFinais = new List<string>();

        public void BuscaArquivos()
        {
            //http://www.linhadecodigo.com.br/artigo/3684/trabalhando-com-arquivos-e-diretorios-em-csharp.aspx

            foreach (string arq in arquivos)
            {
                arquivosFinais.Add(arq.Split("raw/")[1]);
                //Console.WriteLine(arq);
            }
        }

        public void LerArquivo()
        {
            String entrada = "../../../julio_01_02_19.raw";
            this.arquivoEntrada = File.OpenRead(entrada);
            var arquivoSaida = File.OpenWrite("../../../tweets.data");
            this.reader = new BinaryReader(arquivoEntrada);
            var tweetsWriter = new BinaryWriter(arquivoSaida);

            var indexesDict = new Dictionary<string, uint>();
            var hashtagsDict = new Dictionary<string, List<uint>>();

            const int recordSize = 1356;

            byte[] buffer = new byte[recordSize];
            uint index = 0;

            while (reader.Read(buffer, 0, buffer.Count()) > 0)
            {
                var linha = Encoding.UTF8.GetString(buffer);
                string idTwitter = linha.Substring(0, 19);
                string msgTwitter = Encoding.UTF8.GetString(buffer.Skip(219).Take(1120).ToArray());

                if (IndexAlreadyExists(indexesDict, idTwitter))
                    continue;

                //-------separa hashtags da mensagem-------//
                var msgSeparada = msgTwitter.Split('#');
                for (int i = 1; i < msgSeparada.Count(); i++)
                {
                    string hashtagSeparada = msgSeparada[i].Split(' ')[0];
                    if (!Char.IsLetterOrDigit(hashtagSeparada.Last()))
                    {
                        hashtagSeparada = hashtagSeparada.Substring(0, hashtagSeparada.Count() - 1);
                    }

                    AddHashtagIndex(hashtagsDict, hashtagSeparada, index * recordSize);
                }
                //-------separa hashtags da mensagem-------//

                tweetsWriter.Write(buffer);

                indexesDict.Add(idTwitter, index * recordSize);
                index++;
            }

            var indexesFile = File.OpenWrite("../../../id.idx");
            var indexWriter = new BinaryWriter(indexesFile);
            var indexesBuffer = indexesDict.Serialize();
            indexWriter.Write(indexesBuffer);
            indexWriter.Close();
            indexesFile.Close();

            var hashtagsFile = File.OpenWrite("../../../hashtags.idx");
            var hashtagsWriter = new BinaryWriter(hashtagsFile);
            var hashtagsBuffer = hashtagsDict.Serialize();
            hashtagsWriter.Write(hashtagsBuffer);
            hashtagsWriter.Close();
            hashtagsFile.Close();

            arquivoSaida.Close();
        }

        public void PesquisaIndice()
        {
            var idToSearch = "1182384498593259520";

            var buff = File.ReadAllBytes("../../../id.idx");
            var indexesDict = (Dictionary<string, uint>)buff.DeSerialize();
            if (indexesDict.TryGetValue(idToSearch, out var index))
            {
                String entrada = "../../../tweets.data";
                this.arquivoEntrada = File.OpenRead(entrada);
                arquivoEntrada.Seek(index, SeekOrigin.Begin);

                this.reader = new BinaryReader(arquivoEntrada);

                const int recordSize = 1356;
                byte[] buffer = new byte[recordSize];

                reader.Read(buffer, 0, buffer.Count());

                string msgTwitter = Encoding.UTF8.GetString(buffer.Skip(219).Take(1120).ToArray());

                Console.WriteLine("teste de pesquisa: \n" + msgTwitter);
            }
            else
            {
                Console.WriteLine($"Não foi possível encontrar o tweet de id \"{idToSearch}\"");
            }
        }

    }
}
