using Biblioteca.DTO;
using HangmanGame_Servidor.Application.Interfaces;
using HangmanGame_Servidor.Domain.Entities;
using System;
using System.Data;
using System.Linq;

namespace HangmanGame_Servidor.Application.UseCases.Resultados
{
    public class GuardarResultadosService
    {
        private readonly IHangmanDbContextFactory _contextFactory;

        public GuardarResultadosService(IHangmanDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public ResponseResultadoDTO Ejecutar(int idJugador, int idPartida, bool gano, bool esAbandono = false)
        {
            try
            {
                using (var context = _contextFactory.Crear())
                {
                    var partida = context.Partidas
                        .FirstOrDefault(p =>
                            p.IdPartida == idPartida);

                    if (partida == null)
                    {
                        return new ResponseResultadoDTO
                        {
                            success = false,
                            message = "Partida no encontrada."
                        };
                    }

                    var jugador = context.Jugadores
                        .FirstOrDefault(j =>
                            j.IdJugador == idJugador);

                    if (jugador == null)
                    {
                        return new ResponseResultadoDTO
                        {
                            success = false,
                            message = "Jugador no encontrado."
                        };
                    }

                    bool jugadorPertenecePartida =
                        partida.IdAdivinador == idJugador
                        || partida.IdRetador == idJugador;

                    if (!jugadorPertenecePartida)
                    {
                        return new ResponseResultadoDTO
                        {
                            success = false,
                            message =
                                "El jugador no pertenece a esta partida."
                        };
                    }

                    bool resultadoExistente = context.Resultados
                        .Any(r =>
                            r.IdJugador == idJugador
                            && r.IdPartida == idPartida);

                    if (resultadoExistente)
                    {
                        return new ResponseResultadoDTO
                        {
                            success = false,
                            message =
                                "El resultado de este jugador ya fue registrado."
                        };
                    }

                    int puntos = CalcularPuntos(
                        gano,
                        esAbandono);

                    int? idOponente =
                        partida.IdAdivinador == idJugador
                            ? partida.IdRetador
                            : partida.IdAdivinador;

                    string usuarioOponente = "Desconocido";

                    if (idOponente.HasValue)
                    {
                        usuarioOponente = context.Jugadores
                            .Where(j =>
                                j.IdJugador == idOponente.Value)
                            .Select(j => j.Usuario)
                            .FirstOrDefault()
                            ?? "Desconocido";
                    }

                    string palabraAdivinar = context.Palabras
                        .Where(p =>
                            p.IdPalabra == partida.IdPalabra)
                        .Select(p => p.Nombre)
                        .FirstOrDefault();

                    if (string.IsNullOrWhiteSpace(
                        palabraAdivinar))
                    {
                        return new ResponseResultadoDTO
                        {
                            success = false,
                            message =
                                "No se encontró la palabra asociada a la partida."
                        };
                    }

                    var resultado = new Resultado
                    {
                        IdJugador = idJugador,
                        IdPartida = idPartida,
                        Gano = gano,
                        Puntos = puntos
                    };

                    context.Resultados.Add(resultado);

                    jugador.Puntuacion = Math.Max(0,(jugador.Puntuacion ?? 0) + puntos);

                    context.SaveChanges();

                    var resultadoDTO = new ResultadoDTO
                    {
                        FechaPartida =
                            partida.FechaCreacion
                            ?? DateTime.MinValue,

                        PalabraAdivinar =
                            palabraAdivinar,

                        Gano = gano,
                        Puntos = puntos,
                        UsuarioOponente =
                            usuarioOponente
                    };

                    return new ResponseResultadoDTO
                    {
                        success = true,
                        message =
                            "Resultado guardado exitosamente.",
                        data = resultadoDTO
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Error en GuardarResultadosService: {ex}");

                return new ResponseResultadoDTO
                {
                    success = false,
                    message =
                        $"Error al guardar resultado: {ex.Message}"
                };
            }
        }

        private static int CalcularPuntos(bool gano, bool esAbandono)
        {
            if (esAbandono)
            {
                return -3;
            }

            return gano ? 10 : 5;
        }
    }
}
