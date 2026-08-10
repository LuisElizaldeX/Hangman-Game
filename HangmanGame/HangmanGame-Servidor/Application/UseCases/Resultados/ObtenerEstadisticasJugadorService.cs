using Biblioteca.DTO;
using HangmanGame_Servidor.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangmanGame_Servidor.Application.UseCases.Resultados
{
    public class ObtenerEstadisticasJugadorService
    {
        private readonly IHangmanDbContextFactory _contextFactory;

        public ObtenerEstadisticasJugadorService(IHangmanDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public ResponseEstadisticaDTO Ejecutar(int idJugador) 
        {
            try
            {
                if (idJugador <= 0)
                {
                    return new ResponseEstadisticaDTO
                    {
                        success = false,
                        message = "El identificador del jugador no es válido."
                    };
                }

                using (var context = _contextFactory.Crear())
                {
                    bool jugadorExiste = context.Jugadores
                        .Any(j => j.IdJugador == idJugador);

                    if (!jugadorExiste)
                    {
                        return new ResponseEstadisticaDTO
                        {
                            success = false,
                            message = "Jugador no encontrado."
                        };
                    }

                    var estadisticas =
                        (from resultado in context.Resultados

                         join partida in context.Partidas
                             on resultado.IdPartida
                             equals partida.IdPartida

                         join anfitrion in context.Jugadores
                             on partida.IdAdivinador
                             equals anfitrion.IdJugador

                         join retador in context.Jugadores
                             on partida.IdRetador
                             equals retador.IdJugador
                             into retadorGroup

                         from retador in retadorGroup.DefaultIfEmpty()

                         where resultado.IdJugador == idJugador

                         orderby partida.FechaCreacion descending

                         select new EstadisticaDTO
                         {
                             FechaCreacion =
                                 partida.FechaCreacion
                                 ?? DateTime.MinValue,

                             Puntos = resultado.Puntos,

                             Resultado = resultado.Gano
                                 ? "Victoria"
                                 : "Derrota",

                             Nickname =
                                 idJugador == partida.IdAdivinador
                                     ? retador != null
                                         ? retador.Usuario
                                         : "Desconocido"
                                     : anfitrion.Usuario
                         })
                        .ToList();

                    return new ResponseEstadisticaDTO
                    {
                        success = true,
                        data = new EstadisticaDTO
                        {
                            Estadisticas = estadisticas
                        },
                        message = estadisticas.Any()
                            ? "Estadísticas obtenidas correctamente."
                            : "El jugador todavía no tiene partidas registradas."
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Error en ObtenerEstadisticasJugadorService: {ex}");

                return new ResponseEstadisticaDTO
                {
                    success = false,
                    message =
                        $"Error al obtener estadísticas: {ex.Message}"
                };
            }
        }
    }
}
