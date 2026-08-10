using Biblioteca.DTO;
using HangmanGame_Servidor.Application.Interfaces;
using System;
using HangmanGame_Servidor.Domain.Enums;
using HangmanGame_Servidor.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangmanGame_Servidor.Application.UseCases.Partidas
{
    public class CrearPartidaService
    {
        private readonly IHangmanDbContextFactory _contextFactory;

        public CrearPartidaService(IHangmanDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public ResponsePartidaDTO Ejecutar(int idAdivinador, int idPalabra, string codigo, int idIdiomaPartida)
        {
            try
            {
                using (var context = _contextFactory.Crear())
                {
                    if (string.IsNullOrWhiteSpace(codigo))
                    {
                        return new ResponsePartidaDTO
                        {
                            success = false,
                            message = "El código de la partida es obligatorio."
                        };
                    }

                    var existeAdivinador = context.Jugadores
                        .Any(j => j.IdJugador == idAdivinador);

                    if (!existeAdivinador)
                    {
                        return new ResponsePartidaDTO
                        {
                            success = false,
                            message = "El jugador anfitrión no existe."
                        };
                    }

                    var existePalabra = context.Palabras
                        .Any(p => p.IdPalabra == idPalabra);

                    if (!existePalabra)
                    {
                        return new ResponsePartidaDTO
                        {
                            success = false,
                            message = "La palabra seleccionada no existe."
                        };
                    }

                    var idioma = context.IdiomasPartida
                        .Where(i => i.IdIdiomaPartida == idIdiomaPartida)
                        .Select(i => i.Nombre)
                        .FirstOrDefault();

                    if (idioma == null)
                    {
                        return new ResponsePartidaDTO
                        {
                            success = false,
                            message = "El idioma seleccionado no existe."
                        };
                    }

                    var codigoExistente = context.Partidas
                        .Any(p => p.Codigo == codigo);

                    if (codigoExistente)
                    {
                        return new ResponsePartidaDTO
                        {
                            success = false,
                            message = "El código de partida ya está registrado."
                        };
                    }

                    var partida = new Partida
                    {
                        Codigo = codigo,
                        IdPalabra = idPalabra,
                        IdAdivinador = idAdivinador, // Anfitrión
                        IdRetador = null, // Todavía no hay retador
                        IdEstadoPartida = (int)EstadoPartidaEnum.EnEspera,
                        FechaCreacion = DateTime.Now,
                        IdIdiomaPartida = idIdiomaPartida
                    };

                    context.Partidas.Add(partida);
                    context.SaveChanges();

                    return new ResponsePartidaDTO
                    {
                        success = true,
                        data = new PartidaDTO
                        {
                            IdPartida = partida.IdPartida,
                            Codigo = partida.Codigo,
                            Idioma = partida.IdiomaPartida != null
                                    ? partida.IdiomaPartida.Nombre
                                    : null,
                            IdPalabra = partida.IdPalabra,
                            IdRetador = partida.IdRetador ?? 0,
                            IdAdivinador = partida.IdAdivinador ?? 0,
                            IdEstadoPartida = partida.IdEstadoPartida,
                            IdIdiomaPartida = partida.IdIdiomaPartida
                        },
                        message = "Partida creada exitosamente"
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                  $"Error en CrearPartidaService: {ex}");

                return new ResponsePartidaDTO
                {
                    success = false,
                    message =
                        $"Error al crear la partida: {ex.Message}"
                };
            }
        }
    }
}
