using System;
using HangmanGame_Servidor.Application.Interfaces;

namespace HangmanGame_Servidor.ModeloPostgreSQL
{
    public class HangmanDbContextFactory : IHangmanDbContextFactory
    {
        private readonly string _connectionString;

        public HangmanDbContextFactory(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException(
                    "La cadena de conexión no puede estar vacía.",
                    nameof(connectionString));
            }

            _connectionString = connectionString;
        }

        public IHangmanDbContext Crear()
        {
            return new HangmanDbContext(_connectionString);
        }
    }
}
