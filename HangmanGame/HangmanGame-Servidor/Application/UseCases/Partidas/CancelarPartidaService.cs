using Biblioteca.DTO;
using HangmanGame_Servidor.Application.Interfaces;
using HangmanGame_Servidor.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangmanGame_Servidor.Application.UseCases.Partidas
{
    public class CancelarPartidaService
    {
        private readonly IHangmanDbContextFactory _contextFactory;

        public CancelarPartidaService(
            IHangmanDbContextFactory contextFactory)
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
                        .FirstOrDefault(p =>
                            p.Codigo == codigoPartida);

                    if (partida == null)
                    {
                        return new ResponsePartidaDTO
                        {
                            success = false,
                            message = "Partida no encontrada."
                        };
                    }

                    if (partida.IdEstadoPartida ==
                        (int)EstadoPartidaEnum.Cancelada)
                    {
                        return new ResponsePartidaDTO
                        {
                            success = false,
                            message = "La partida ya se encuentra cancelada."
                        };
                    }

                    partida.IdEstadoPartida =
                        (int)EstadoPartidaEnum.Cancelada;

                    context.SaveChanges();

                    return new ResponsePartidaDTO
                    {
                        success = true,
                        message = "Partida cancelada exitosamente."
                    };
                }
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(
                    $"Error de base de datos en CancelarPartidaService: {ex}");

                return new ResponsePartidaDTO
                {
                    success = false,
                    message =
                        "No fue posible cancelar la partida debido a un error de base de datos."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Error en CancelarPartidaService: {ex}");

                return new ResponsePartidaDTO
                {
                    success = false,
                    message =
                        $"Error al cancelar la partida: {ex.Message}"
                };
            }
        }
    }
}
