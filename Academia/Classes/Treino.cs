using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academia.Classes
{
    class Treino
    {
        int id_treino;
        string nome;

        public string Nome { get => nome; set => nome = value; }
        public int Id_treino { get => id_treino; set => id_treino = value; }

        public Treino(int id, string nome)
        {
           
            Nome = nome;
            Id_treino = id;
           
        }
    }
}
