using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.DTO
{
    public class JugadorDTO
    {
        public string Usuario { get; set; }
        public string nombre { get; set; }
        public DateTime fecha_nacimiento { get; set; }
        public int telefono { get; set; }
        public string correo { get; set; }
        public string contrasñea { get; set; }
        public int puntuacion { get; set; }
    }
}
