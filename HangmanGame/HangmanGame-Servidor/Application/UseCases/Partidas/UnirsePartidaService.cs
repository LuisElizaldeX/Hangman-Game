using Biblioteca.DTO;
using HangmanGame_Servidor.Application.Interfaces;
using HangmanGame_Servidor.Domain.Enums;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangmanGame_Servidor.Application.UseCases.Partidas
{
    public class UnirsePartidaService
    {
        private readonly IHangmanDbContextFactory _contextFactory;
        private readonly ISocketNotifier _socketNotifier;

        public UnirsePartidaService(IHangmanDbContextFactory contextFactory, ISocketNotifier socketNotifier)
        {
            _contextFactory = contextFactory;
            _socketNotifier = socketNotifier;
        }

        public ResponsePartidaDTO Ejecutar(string codigoPartida, int idJugador)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(codigoPartida))
                {
                    return new ResponsePartidaDTO
                    {
                        success = false,
                        message = "El código de la partida es obligatorio."
                    };
                }

                string nicknameRetador;
                PartidaDTO partidaDto;

                using (var context = _contextFactory.Crear())
                using (var transaction =
                    context.BeginTransaction(
                    IsolationLevel.Serializable))
                {
                    var partida = context.Partidas
                        .FirstOrDefault(p =>
                            p.Codigo == codigoPartida &&
                            p.IdEstadoPartida ==
                            (int)EstadoPartidaEnum.EnEspera);

                    if (partida == null)
                    {
                        return new ResponsePartidaDTO
                        {
                            success = false,
                            message =
                                "Partida no encontrada o ya no está disponible."
                        };
                    }

                    if (partida.IdRetador.HasValue)
                    {
                        return new ResponsePartidaDTO
                        {
                            success = false,
                            message = "La partida ya tiene un retador."
                        };
                    }

                    if (partida.IdAdivinador == idJugador)
                    {
                        return new ResponsePartidaDTO
                        {
                            success = false,
                            message =
                                "No puedes unirte a tu propia partida."
                        };
                    }

                    var retador = context.Jugadores
                        .FirstOrDefault(j =>
                            j.IdJugador == idJugador);

                    if (retador == null)
                    {
                        return new ResponsePartidaDTO
                        {
                            success = false,
                            message = "El jugador no existe."
                        };
                    }

                    partida.IdRetador = idJugador;
                    partida.IdEstadoPartida =
                        (int)EstadoPartidaEnum.ListaParaComenzar;

                    nicknameRetador =
                        string.IsNullOrWhiteSpace(retador.Usuario)
                            ? retador.Nombre ?? "Jugador"
                            : retador.Usuario;

                    context.SaveChanges();
                    transaction.Commit();

                    partidaDto = new PartidaDTO
                    {
                        IdPartida = partida.IdPartida,
                        Codigo = partida.Codigo,

                        Idioma = partida.IdiomaPartida != null
                            ? partida.IdiomaPartida.Nombre
                            : null,

                        fecha = partida.FechaCreacion
                            ?? DateTime.MinValue,

                        Nickname = partida.Adivinador != null
                            ? partida.Adivinador.Usuario
                            : "Anónimo",

                        IdPalabra = partida.IdPalabra,
                        IdRetador = partida.IdRetador ?? 0,
                        IdAdivinador = partida.IdAdivinador ?? 0,
                        IdEstadoPartida = partida.IdEstadoPartida,
                        IdIdiomaPartida = partida.IdIdiomaPartida
                    };
                }

                try
                {
                    _socketNotifier
                        .NotificarUnionRetadorAsync(
                            codigoPartida,
                            nicknameRetador)
                        .GetAwaiter()
                        .GetResult();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"La partida se actualizó, pero falló la notificación: {ex}");
                }

                return new ResponsePartidaDTO
                {
                    success = true,
                    message =
                        $"{nicknameRetador} se ha unido a la partida.",
                    data = partidaDto
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Error en UnirsePartidaService: {ex}");

                return new ResponsePartidaDTO
                {
                    success = false,
                    message =
                        $"Error al unirse a la partida: {ex.Message}"
                };
            }
        }
    }
}
