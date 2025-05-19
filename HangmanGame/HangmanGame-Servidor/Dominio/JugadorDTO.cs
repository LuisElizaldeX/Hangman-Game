using System;
using System.Runtime.Serialization;

namespace HangmanGame_Servidor.Dominio
{
    public class JugadorDTO
    {
        [DataMember]
        public int IdJugador { get; set; }

        [DataMember]
        public String Usuario { get; set; }

        [DataMember]
        public String Nombre { get; set; }

        [DataMember]
        public DateTime FechaNacimiento { get; set; }

        [DataMember]
        public int Telefono { get; set; }

        [DataMember]
        public String Correo { get; set; }

        [DataMember]
        public String Contrasena { get; set; }

        [DataMember]
        public int Puntuacion { get; set; }
    }
}
