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
    public class ObtenerEstadoPartidaService
    {
        private readonly IHangmanDbContextFactory _contextFactory;

        public ObtenerEstadoPartidaService(IHangmanDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public ResponsePartidaDTO Ejecutar(string codigoPartida)
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

                using (var context = _contextFactory.Crear())
                {
                    var partida = context.Partidas
                    .Where(p =>
                        p.Codigo == codigoPartida && (
                        p.IdEstadoPartida ==
                        (int)EstadoPartidaEnum.EnEspera || p.IdEstadoPartida == (int)EstadoPartidaEnum.ListaParaComenzar))
                    .Select(p => new PartidaDTO
                    {
                        IdPartida = p.IdPartida,
                        Codigo = p.Codigo,
                        Idioma = p.IdiomaPartida.Nombre,
                        fecha = p.FechaCreacion ?? DateTime.MinValue,
                        Nickname = p.Adivinador.Usuario,
                        NicknameRetador = p.Retador != null
                            ? p.Retador.Usuario
                            : null,
                        IdPalabra = p.IdPalabra,
                        IdRetador = p.IdRetador ?? 0,
                        IdAdivinador = p.IdAdivinador ?? 0,
                        IdEstadoPartida = p.IdEstadoPartida,
                        IdIdiomaPartida = p.IdIdiomaPartida,
                        Palabra = p.Palabra.Nombre.ToUpper(),
                        Pista = p.Palabra.Pista,
                        PistaEn = p.Palabra.DescripcionEn})
                        .FirstOrDefault();

                    if (partida == null)
                    {
                        return new ResponsePartidaDTO
                        {
                            success = false,
                            message =
                                "Partida no encontrada o no está disponible."
                        };
                    }

                    return new ResponsePartidaDTO
                    {
                        success = true,
                        partida = partida,
                        message =
                            "Estado de la partida obtenido correctamente."
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Error en ObtenerEstadoPartidaService: {ex}");

                return new ResponsePartidaDTO
                {
                    success = false,
                    message =
                        $"Error al obtener el estado de la partida: {ex.Message}"
                };
            }
        }
    } 
}
