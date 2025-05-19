using System;

namespace HangmanGame_Cliente.Cliente.Dominio
{
    public class JugadorDTO
    {
        public string Usuario { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public int Telefono { get; set; }
        public string Correo { get; set; }
        public string Contrasena { get; set; }
        public int Puntuacion { get; set; }
    }
}
