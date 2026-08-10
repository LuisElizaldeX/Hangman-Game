using Npgsql;
using System.Data.Entity;

namespace HangmanGame_Servidor.ModeloPostgreSQL
{
    public class NpgsqlEfConfiguration : DbConfiguration
    {
        public NpgsqlEfConfiguration()
        {
            const string providerName = "Npgsql";

            SetProviderFactory(
                providerName,
                NpgsqlFactory.Instance);

            SetProviderServices(
                providerName,
                NpgsqlServices.Instance);

            SetDefaultConnectionFactory(
                new NpgsqlConnectionFactory());
        }
    }
}
