using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangmanGame_Servidor.Domain.Entities
{
    public class Palabra
    {
        public Palabra()
        {
            Partidas = new HashSet<Partida>();
        }

        public int IdPalabra { get; set; }
        public string Nombre { get; set; }
        public string NombreEn { get; set; }
        public string Pista { get; set; }
        public string Descripcion { get; set; }
        public string DescripcionEn { get; set; }

        public int IdCategoria { get; set; }

        public virtual Categoria Categoria { get; set; }

        public virtual ICollection<Partida> Partidas { get; set; }
    }
}
