using Biblioteca.DTO;
using HangmanGame_Servidor.Application.Interfaces;
using HangmanGame_Servidor.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangmanGame_Servidor.Application.UseCases.Jugadores
{
    public class RegistrarJugadorService
    {
        private readonly IHangmanDbContextFactory _contextFactory;

        public RegistrarJugadorService(
            IHangmanDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public ResponseDTO Ejecutar(JugadorDTO nuevoJugador)
        {
            try
            {
                if (nuevoJugador == null)
                {
                    return new ResponseDTO
                    {
                        success = false,
                        message = "Los datos del jugador son obligatorios."
                    };
                }

                if (string.IsNullOrWhiteSpace(nuevoJugador.Usuario))
                {
                    return new ResponseDTO
                    {
                        success = false,
                        message = "El nombre de usuario es obligatorio."
                    };
                }

                if (string.IsNullOrWhiteSpace(nuevoJugador.Correo))
                {
                    return new ResponseDTO
                    {
                        success = false,
                        message = "El correo es obligatorio."
                    };
                }

                if (string.IsNullOrWhiteSpace(nuevoJugador.Contrasena))
                {
                    return new ResponseDTO
                    {
                        success = false,
                        message = "La contraseña es obligatoria."
                    };
                }

                using (var context = _contextFactory.Crear())
                {
                    bool usuarioExistente = context.Jugadores.Any(j =>
                        j.Usuario == nuevoJugador.Usuario);

                    if (usuarioExistente)
                    {
                        return new ResponseDTO
                        {
                            success = false,
                            message = "El nombre de usuario ya está registrado."
                        };
                    }

                    bool correoExistente = context.Jugadores.Any(j =>
                        j.Correo == nuevoJugador.Correo);

                    if (correoExistente)
                    {
                        return new ResponseDTO
                        {
                            success = false,
                            message = "El correo ya está registrado."
                        };
                    }

                    var jugador = new Jugador
                    {
                        Usuario = nuevoJugador.Usuario.Trim(),
                        Nombre = nuevoJugador.Nombre?.Trim(),
                        FechaNacimiento = nuevoJugador.FechaNacimiento,
                        Telefono = nuevoJugador.Telefono?.Trim(),
                        Correo = nuevoJugador.Correo.Trim(),
                        Contrasena = nuevoJugador.Contrasena,
                        Puntuacion = 0
                    };

                    context.Jugadores.Add(jugador);
                    context.SaveChanges();

                    return new ResponseDTO
                    {
                        success = true,
                        message = "Jugador registrado exitosamente."
                    };
                }
            }
            catch (DbUpdateException ex)
            {
                string errorTecnico = ObtenerMensajeCompleto(ex);

                Console.WriteLine(
                    $"Error en RegistrarJugadorService: {errorTecnico}");

                return new ResponseDTO
                {
                    success = false,
                    message =
                        "No fue posible registrar al jugador debido a un error de base de datos."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Error en RegistrarJugadorService: {ex}");

                return new ResponseDTO
                {
                    success = false,
                    message =
                        $"Error al registrar jugador: {ex.Message}"
                };
            }
        }

        private static string ObtenerMensajeCompleto(Exception exception)
        {
            string mensaje = string.Empty;
            Exception actual = exception;

            while (actual != null)
            {
                if (!string.IsNullOrWhiteSpace(mensaje))
                {
                    mensaje += " --> ";
                }

                mensaje += actual.Message;
                actual = actual.InnerException;
            }

            return mensaje;
        }

    }
}
