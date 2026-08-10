using Biblioteca.DTO;
using HangmanGame_Servidor.Application.Interfaces;
using HangmanGame_Servidor.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangmanGame_Servidor.Application.UseCases.Partidas
{
    public class ObtenerPartidasDisponiblesService
    {
        private readonly IHangmanDbContextFactory _contextFactory;

        public ObtenerPartidasDisponiblesService(IHangmanDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public ResponsePartidaDTO Ejecutar()
        {
            try
            {
                using (var context = _contextFactory.Crear())
                {
                    var partidas = context.Partidas
                        .Where(p =>p.IdEstadoPartida == (int)EstadoPartidaEnum.EnEspera)
                        .Select(p => new PartidaDTO
                        {
                            IdPartida = p.IdPartida,
                            Codigo = p.Codigo,
                            Idioma = p.IdiomaPartida != null
                                        ? p.IdiomaPartida.Nombre
                                        : null,
                            fecha = p.FechaCreacion ?? DateTime.MinValue,
                            Nickname = p.Adivinador != null
                                        ? p.Adivinador.Usuario
                                        : null,
                            IdPalabra = p.IdPalabra,
                            IdRetador = p.IdRetador ?? 0,
                            IdAdivinador = p.IdAdivinador ?? 0,
                            IdEstadoPartida = p.IdEstadoPartida,
                            IdIdiomaPartida = p.IdIdiomaPartida
                        }).ToList();
                    return new ResponsePartidaDTO
                    {
                        success = true,
                        data = new PartidaDTO { Partidas = partidas },
                        message = "Partidas disponibles obtenidas correctamente."
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResponsePartidaDTO
                {
                    success = false,
                    message = $"Error al obtener partidas disponibles: {ex.Message}"
                };
            }
        }
    }
}
