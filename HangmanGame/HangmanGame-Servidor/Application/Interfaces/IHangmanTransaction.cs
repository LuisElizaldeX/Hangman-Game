using System;

namespace HangmanGame_Servidor.Application.Interfaces
{
    public interface IHangmanTransaction : IDisposable
    {
        void Commit();
        void Rollback();
    }
}
