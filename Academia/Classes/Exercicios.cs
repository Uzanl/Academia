using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academia.Classes
{
    class Exercicios
    {
        int id_exercicio;
        string nome;
        int nseries;
        int peso;
        string descricao;
        int repeticao;

        public Exercicios()
        {
        }

        public Exercicios(int id, string nome, int nseries,int repeticao, int peso, string descricao)
        {
            Id_exercicio = id;
            Nome = nome;
            Nseries = nseries;
            Repeticao = repeticao;
            Peso = peso;
            Descricao = descricao;
        }

        public int Id_exercicio { get => id_exercicio; set => id_exercicio = value; }
        public string Nome { get => nome; set => nome = value; }
        public int Nseries { get => nseries; set => nseries = value; }
        public int Peso { get => peso; set => peso = value; }
        public string Descricao { get => descricao; set => descricao = value; }
        public int Repeticao { get => repeticao; set => repeticao = value; }
    }
}
