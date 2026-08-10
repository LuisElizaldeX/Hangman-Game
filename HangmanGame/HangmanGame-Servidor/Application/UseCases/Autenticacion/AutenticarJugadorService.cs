using Biblioteca.DTO;
using HangmanGame_Servidor.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangmanGame_Servidor.Application.UseCases.Autenticacion
{
    public class AutenticarJugadorService
    {
        private readonly IHangmanDbContextFactory _contextFactory;

        public AutenticarJugadorService(
            IHangmanDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public ResponseDTO Ejecutar(string correo, string contrasena)
        {
            try
            {
                using (var context = _contextFactory.Crear())
                {
                    var jugador = context.Jugadores
                        .FirstOrDefault(j =>
                            j.Correo == correo &&
                            j.Contrasena == contrasena);

                    if (jugador == null)
                    {
                        return new ResponseDTO
                        {
                            success = false,
                            message = "Credenciales no encontradas"
                        };
                    }

                    return new ResponseDTO
                    {
                        success = true,
                        data = new JugadorDTO
                        {
                            IdJugador = jugador.IdJugador,
                            Correo = jugador.Correo,
                            Usuario = jugador.Usuario,
                            Nombre = jugador.Nombre,
                            Telefono = jugador.Telefono,
                            Puntuacion = jugador.Puntuacion ?? 0
                        },
                        message = "Autenticación exitosa"
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    success = false,
                    message =
                        $"Error al autenticar jugador: {ex.Message}"
                };
            }
        }
    }
}
