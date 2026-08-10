using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangmanGame_Servidor.Domain.Entities
{
    public class EstadoPartida
    {
        public EstadoPartida()
        {
            Partidas = new HashSet<Partida>();
        }

        public int IdEstadoPartida { get; set; }
        public string Nombre { get; set; }

        public virtual ICollection<Partida> Partidas { get; set; }
    }
}
