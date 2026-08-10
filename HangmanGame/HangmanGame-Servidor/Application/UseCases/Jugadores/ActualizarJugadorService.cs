using Biblioteca.DTO;
using HangmanGame_Servidor.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangmanGame_Servidor.Application.UseCases.Jugadores
{
    public class ActualizarJugadorService
    {
        private readonly IHangmanDbContextFactory _contextFactory;

        public ActualizarJugadorService(
            IHangmanDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public ResponseDTO Ejecutar(JugadorDTO jugadorActualizado)
        {
            try
            {
                if (jugadorActualizado == null)
                {
                    return new ResponseDTO
                    {
                        success = false,
                        message = "Los datos del jugador son obligatorios."
                    };
                }

                if (jugadorActualizado.IdJugador <= 0)
                {
                    return new ResponseDTO
                    {
                        success = false,
                        message = "El identificador del jugador no es válido."
                    };
                }

                if (string.IsNullOrWhiteSpace(jugadorActualizado.Usuario))
                {
                    return new ResponseDTO
                    {
                        success = false,
                        message = "El nombre de usuario es obligatorio."
                    };
                }

                if (string.IsNullOrWhiteSpace(jugadorActualizado.Nombre))
                {
                    return new ResponseDTO
                    {
                        success = false,
                        message = "El nombre del jugador es obligatorio."
                    };
                }

                using (var context = _contextFactory.Crear())
                {
                    var jugador = context.Jugadores
                        .FirstOrDefault(j =>
                            j.IdJugador == jugadorActualizado.IdJugador);

                    if (jugador == null)
                    {
                        return new ResponseDTO
                        {
                            success = false,
                            message = "Jugador no encontrado."
                        };
                    }

                    string usuarioNormalizado =
                        jugadorActualizado.Usuario.Trim();

                    bool usuarioOcupado = context.Jugadores.Any(j =>
                        j.IdJugador != jugadorActualizado.IdJugador
                        && j.Usuario == usuarioNormalizado);

                    if (usuarioOcupado)
                    {
                        return new ResponseDTO
                        {
                            success = false,
                            message =
                                "El nombre de usuario ya pertenece a otro jugador."
                        };
                    }

                    jugador.Usuario = usuarioNormalizado;
                    jugador.Nombre = jugadorActualizado.Nombre.Trim();
                    jugador.FechaNacimiento =
                        jugadorActualizado.FechaNacimiento;
                    jugador.Telefono =
                        jugadorActualizado.Telefono?.Trim();

                    context.SaveChanges();

                    return new ResponseDTO
                    {
                        success = true,
                        message = "Jugador actualizado exitosamente."
                    };
                }
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(
                    $"Error de base de datos en ActualizarJugadorService: {ex}");

                return new ResponseDTO
                {
                    success = false,
                    message =
                        "No fue posible actualizar al jugador debido a un error de base de datos."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Error en ActualizarJugadorService: {ex}");

                return new ResponseDTO
                {
                    success = false,
                    message =
                        $"Error al actualizar jugador: {ex.Message}"
                };
            }
        }
    }
}
