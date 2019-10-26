using System;

namespace trabalho_maravilhoso
{
    class Program
    {
        static void Main(string[] args)
        {
            Arquivo arquivo = new Arquivo();

            Console.WriteLine("Hello World!");

            //arquivo.BuscaArquivos();
            //Console.WriteLine("\n ID Twitter:");
            arquivo.LerArquivo();
            //arquivo.PesquisaIndice();
        }
    }
}
