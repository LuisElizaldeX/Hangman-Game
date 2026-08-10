using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangmanGame_Servidor.Domain.Entities
{
    public class Resultado
    {
        public int IdResultado { get; set; }
        public int IdJugador { get; set; }
        public int IdPartida { get; set; }
        public bool Gano { get; set; }
        public int Puntos { get; set; }

        public virtual Jugador Jugador { get; set; }
        public virtual Partida Partida { get; set; }
    }
}
