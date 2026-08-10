using System;
using System.Collections.Generic;

namespace HangmanGame_Servidor.Domain.Entities
{
    public class Jugador
    {
        public Jugador()
        {
            Resultados = new HashSet<Resultado>();
            PartidasComoRetador = new HashSet<Partida>();
            PartidasComoAdivinador = new HashSet<Partida>();
        }

        public int IdJugador { get; set; }
        public string Usuario { get; set; }
        public string Nombre { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string Contrasena { get; set; }
        public int? Puntuacion { get; set; }

        public virtual ICollection<Resultado> Resultados { get; set; }

        public virtual ICollection<Partida> PartidasComoRetador { get; set; }

        public virtual ICollection<Partida> PartidasComoAdivinador { get; set; }
    }
}
