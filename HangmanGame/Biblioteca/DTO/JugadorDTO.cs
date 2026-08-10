using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.DTO
{
    [DataContract]
    public class JugadorDTO
    {
        [DataMember]
        public int IdJugador { get; set; }
        [DataMember]
        public string Correo { get; set; }
        [DataMember]
        public string Contrasena { get; set; } 
        [DataMember]
        public string Usuario { get; set; }
        [DataMember]
        public string Nombre { get; set; }
        [DataMember]
        public DateTime FechaNacimiento { get; set; }
        [DataMember]
        public string Telefono { get; set; }
        [DataMember]
        public int Puntuacion { get; set; }
    }
}
