using System;
using System.Collections.Generic;

namespace HangmanGame_Servidor.Domain.Entities
{
    public class Categoria
    {
        public Categoria()
        {
            Palabras = new HashSet<Palabra>();
        }

        public int IdCategoria { get; set; }
        public string Nombre { get; set; }
        public string NombreEn { get; set; }

        public virtual ICollection<Palabra> Palabras { get; set; }
    }
}
