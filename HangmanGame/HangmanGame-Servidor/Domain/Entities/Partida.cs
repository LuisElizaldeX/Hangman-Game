using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangmanGame_Servidor.Domain.Entities
{
    public class Partida
    {
        public Partida()
        {
            Resultados = new HashSet<Resultado>();
        }

        public int IdPartida { get; set; }
        public string Codigo { get; set; }

        public int IdPalabra { get; set; }

        public int? IdRetador { get; set; }

        public int? IdAdivinador { get; set; }

        public int IdEstadoPartida { get; set; }
        public int IdIdiomaPartida { get; set; }

        public DateTime? FechaCreacion { get; set; }

        public virtual Jugador Retador { get; set; }
        public virtual Jugador Adivinador { get; set; }
        public virtual Palabra Palabra { get; set; }
        public virtual EstadoPartida EstadoPartida { get; set; }
        public virtual IdiomaPartida IdiomaPartida { get; set; }
        public virtual ICollection<Resultado> Resultados { get; set; }
    }
}
