using System;
using System.Threading.Tasks;

namespace HangmanGame_Cliente.Application.Interfaces
{
    public interface ISocketClient : IDisposable
    {
        event Action<string> MessageReceived;
        event Action<string> ConnectionLost;

        bool IsConnected { get; }

        Task ConectarAsync(
            string ip,
            int port);

        Task EnviarAsync(
            string message);

        void Desconectar();
    }
}
