using System;
using HangmanGame_Servidor.Dominio;
using HangmanGame_Servidor.Modelo;

namespace HangmanGame_Servidor.Logica
{
    public class JugadorService
    {

        public bool RegistrarJugador(JugadorDTO nuevoJugador)
        {
            bool status = false;
            try
            {
                using (var context = new HangmanEntidades())
                {
                    jugador recordPlayer = new jugador()
                    {
                        usuario = nuevoJugador.Usuario,
                        nombre = nuevoJugador.Nombre,
                        fecha_nacimiento = nuevoJugador.FechaNacimiento,
                        telefono = nuevoJugador.Telefono,
                        correo = nuevoJugador.Correo,
                        contrasena = nuevoJugador.Contrasena,
                        puntuacion = 0
                    };
                    context.jugador.Add(recordPlayer);
                    status = context.SaveChanges() > 0;
                }
                return status;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return status;
            }
        }
    }
}
