using HangmanGame_Servidor.Application.Interfaces;
using System.Data.Entity;

namespace HangmanGame_Servidor.Infrastructure.Persistence
{
    public class HangmanTransaction : IHangmanTransaction
    {
        private readonly DbContextTransaction _transaction;

        public HangmanTransaction(
            DbContextTransaction transaction)
        {
            _transaction = transaction;
        }

        public void Commit()
        {
            _transaction.Commit();
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }

        public void Dispose()
        {
            _transaction.Dispose();
        }
    }
}
