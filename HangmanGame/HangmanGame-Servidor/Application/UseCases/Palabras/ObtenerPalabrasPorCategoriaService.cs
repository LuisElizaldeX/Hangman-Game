using Biblioteca.DTO;
using HangmanGame_Servidor.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangmanGame_Servidor.Application.UseCases.Palabras
{
    public class ObtenerPalabrasPorCategoriaService
    {
        private readonly IHangmanDbContextFactory _contextFactory;

        public ObtenerPalabrasPorCategoriaService(
            IHangmanDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public ResponsePalabrasConIdDTO Ejecutar(string categoria)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(categoria))
                {
                    return new ResponsePalabrasConIdDTO
                    {
                        success = false,
                        message = "La categoría es obligatoria."
                    };
                }

                using (var context = _contextFactory.Crear())
                {
                    var categoriaEncontrada = context.Categorias
                        .Where(c => c.Nombre == categoria)
                        .Select(c => new
                        {
                            c.IdCategoria
                        })
                        .FirstOrDefault();

                    if (categoriaEncontrada == null)
                    {
                        return new ResponsePalabrasConIdDTO
                        {
                            success = false,
                            message = "Categoría no encontrada."
                        };
                    }

                    var palabras = context.Palabras
                        .Where(p =>
                            p.IdCategoria ==
                            categoriaEncontrada.IdCategoria)
                        .Select(p => new PalabraDTO
                        {
                            IdPalabra = p.IdPalabra,
                            nombre = p.Nombre
                        })
                        .ToList();

                    if (!palabras.Any())
                    {
                        return new ResponsePalabrasConIdDTO
                        {
                            success = false,
                            message =
                                "No se encontraron palabras para esta categoría."
                        };
                    }

                    return new ResponsePalabrasConIdDTO
                    {
                        success = true,
                        data = new PalabrasResponseDTO
                        {
                            PalabrasConId = palabras
                        },
                        message = "Palabras obtenidas exitosamente."
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Error en ObtenerPalabrasPorCategoriaService: {ex}");

                return new ResponsePalabrasConIdDTO
                {
                    success = false,
                    message =
                        $"Error al obtener palabras: {ex.Message}"
                };
            }
        }
    }
}
